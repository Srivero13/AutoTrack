set -euo pipefail

python3 scripts/mock-vin.py > /tmp/autotrack-vin.log 2>&1 &
mockpid=$!

export VinDecoder__BaseUrl="http://127.0.0.1:5118/"
dotnet run --project src/AutoTrack.Api --configuration Release --no-build --no-launch-profile --urls http://127.0.0.1:5117 > /tmp/autotrack-api.log 2>&1 &
proceso=$!
trap 'kill "$proceso" "$mockpid" 2>/dev/null || true' EXIT

disponible=0
for intento in $(seq 1 30); do
    if curl -fsS http://127.0.0.1:5117/api/v1/catalogos/marcas > /tmp/autotrack-marcas.json; then
        disponible=1
        break
    fi
    sleep 2
done

if [ "$disponible" -ne 1 ]; then
    cat /tmp/autotrack-api.log
    exit 1
fi

python3 -c 'import json; d=json.load(open("/tmp/autotrack-marcas.json")); assert d["success"] and len(d["data"]) >= 3'

vin="1HGCM82633A004352"
curl -fsS "http://127.0.0.1:5117/api/v1/vin/$vin" > /tmp/autotrack-decoder.json
python3 -c 'import json; d=json.load(open("/tmp/autotrack-decoder.json")); assert d["data"]["marca"]=="HONDA" and d["data"]["modelo"]=="Accord" and d["data"]["estado"]=="Decodificado"'


usuario_creado=$(curl -sS -o /tmp/autotrack-usuario-creado.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/usuarios -H 'Content-Type: application/json' -d '{"nombre":"Ana","apellido":"Perez","correo":"ana.perez@example.com","telefono":"70010001"}')
test "$usuario_creado" = "201"
python3 -c 'import json; d=json.load(open("/tmp/autotrack-usuario-creado.json")); assert d["success"] and d["data"]["id"]==1 and d["data"]["correo"]=="ana.perez@example.com"'

curl -fsS http://127.0.0.1:5117/api/v1/usuarios > /tmp/autotrack-usuarios.json
python3 -c 'import json; d=json.load(open("/tmp/autotrack-usuarios.json")); assert len(d["data"])==1'

curl -fsS http://127.0.0.1:5117/api/v1/usuarios/1 > /tmp/autotrack-usuario.json
python3 -c 'import json; d=json.load(open("/tmp/autotrack-usuario.json")); assert d["data"]["nombre"]=="Ana"'

usuario_actualizado=$(curl -sS -o /tmp/autotrack-usuario-actualizado.json -w '%{http_code}' -X PUT http://127.0.0.1:5117/api/v1/usuarios/1 -H 'Content-Type: application/json' -d '{"telefono":"70020002"}')
test "$usuario_actualizado" = "200"
python3 -c 'import json; d=json.load(open("/tmp/autotrack-usuario-actualizado.json")); assert d["data"]["telefono"]=="70020002"'

correo_repetido=$(curl -sS -o /tmp/autotrack-correo-duplicado.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/usuarios -H 'Content-Type: application/json' -d '{"nombre":"Otro","apellido":"Usuario","correo":"ana.perez@example.com"}')
test "$correo_repetido" = "409"

correo_invalido=$(curl -sS -o /tmp/autotrack-correo-invalido.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/usuarios -H 'Content-Type: application/json' -d '{"nombre":"Otro","apellido":"Usuario","correo":"correo-invalido"}')
test "$correo_invalido" = "400"

creado=$(curl -sS -o /tmp/autotrack-creado.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/vehiculos -H 'Content-Type: application/json' -d '{"vin":"1HGCM82633A004352","placa":"DEMO123","kilometrajeActual":48000,"usuarioId":1}')
test "$creado" = "201"
python3 -c 'import json; d=json.load(open("/tmp/autotrack-creado.json")); assert d["success"] and d["data"]["vin"]=="1HGCM82633A004352" and d["data"]["anio"]==2003 and d["data"]["informacionVin"]["modelo"]=="Accord" and d["data"]["id"]==1'


curl -fsS 'http://127.0.0.1:5117/api/v1/vehiculos?marca=HONDA&anio=2003&kilometrajeMinimo=40000&kilometrajeMaximo=50000&usuarioId=1' > /tmp/autotrack-filtrados.json
python3 -c 'import json; d=json.load(open("/tmp/autotrack-filtrados.json")); assert len(d["data"])==1 and d["data"][0]["usuarioId"]==1'

rango_invalido=$(curl -sS -o /tmp/autotrack-rango-invalido.json -w '%{http_code}' 'http://127.0.0.1:5117/api/v1/vehiculos?kilometrajeMinimo=50000&kilometrajeMaximo=100')
test "$rango_invalido" = "400"

usuario_inexistente=$(curl -sS -o /tmp/autotrack-usuario-inexistente.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/vehiculos -H 'Content-Type: application/json' -d '{"vin":"3FA6P0H73HR000001","usuarioId":999,"kilometrajeActual":0}')
test "$usuario_inexistente" = "409"

curl -fsS "http://127.0.0.1:5117/api/v1/vehiculos/vin/$vin" > /tmp/autotrack-vin-guardado.json
python3 -c 'import json; d=json.load(open("/tmp/autotrack-vin-guardado.json")); assert d["data"]["informacionVin"]["marca"]=="HONDA"'

actualizado=$(curl -sS -o /tmp/autotrack-actualizado.json -w '%{http_code}' -X PUT http://127.0.0.1:5117/api/v1/vehiculos/1 -H 'Content-Type: application/json' -d '{"placa":"DEMO123","kilometrajeActual":48500}')
test "$actualizado" = "200"
python3 -c 'import json; d=json.load(open("/tmp/autotrack-actualizado.json")); assert d["data"]["vin"]=="1HGCM82633A004352" and d["data"]["kilometrajeActual"]==48500'

refrescado=$(curl -sS -o /tmp/autotrack-refrescado.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/vehiculos/1/decodificar)
test "$refrescado" = "200"
python3 -c 'import json; d=json.load(open("/tmp/autotrack-refrescado.json")); assert d["data"]["informacionVin"]["estado"]=="Decodificado"'

carga=$(curl -sS -o /tmp/autotrack-carga.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/vehiculos/1/combustible -H 'Content-Type: application/json' -d '{"fecha":"2026-10-20T18:00:00Z","kilometraje":48500,"litros":35.50,"precioPorLitro":3.74}')
test "$carga" = "201"
python3 -c 'import json; d=json.load(open("/tmp/autotrack-carga.json")); assert d["data"]["total"]==132.77'

curl -fsS http://127.0.0.1:5117/api/v1/vehiculos/1/combustible > /tmp/autotrack-historial.json
python3 -c 'import json; d=json.load(open("/tmp/autotrack-historial.json")); assert len(d["data"])==1'

duplicado=$(curl -sS -o /tmp/autotrack-duplicado.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/vehiculos -H 'Content-Type: application/json' -d '{"vin":"1HGCM82633A004352","kilometrajeActual":0}')
test "$duplicado" = "409"

invalido=$(curl -sS -o /tmp/autotrack-invalido.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/vehiculos -H 'Content-Type: application/json' -d '{"vin":"VININVALIDO","kilometrajeActual":0}')
test "$invalido" = "400"

desconocido=$(curl -sS -o /tmp/autotrack-desconocido.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/vehiculos -H 'Content-Type: application/json' -d '{"vin":"WVWZZZ1JZXW000001","placa":"DEMO123","anio":2000,"kilometrajeActual":220000}')
test "$desconocido" = "201"
python3 -c 'import json; d=json.load(open("/tmp/autotrack-desconocido.json")); assert d["data"]["informacionVin"]["estado"]=="SinDatos" and d["data"]["anio"]==2000 and d["data"]["placa"]=="DEMO123"'

eliminado=$(curl -sS -o /tmp/autotrack-eliminado.json -w '%{http_code}' -X DELETE http://127.0.0.1:5117/api/v1/vehiculos/1)
test "$eliminado" = "200"

inexistente=$(curl -sS -o /tmp/autotrack-inexistente.json -w '%{http_code}' http://127.0.0.1:5117/api/v1/vehiculos/1)
test "$inexistente" = "404"


usuario_eliminado=$(curl -sS -o /tmp/autotrack-usuario-eliminado.json -w '%{http_code}' -X DELETE http://127.0.0.1:5117/api/v1/usuarios/1)
test "$usuario_eliminado" = "200"

usuario_desactivado=$(curl -sS -o /tmp/autotrack-usuario-desactivado.json -w '%{http_code}' http://127.0.0.1:5117/api/v1/usuarios/1)
test "$usuario_desactivado" = "404"

echo "AutoTrack: pruebas VIN, usuarios, filtros y REST completadas"
