set -euo pipefail

dotnet run --project src/AutoTrack.Api --configuration Release --no-build --no-launch-profile --urls http://127.0.0.1:5117 > /tmp/autotrack-api.log 2>&1 &
proceso=$!
trap 'kill "$proceso" 2>/dev/null || true' EXIT

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

creado=$(curl -sS -o /tmp/autotrack-creado.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/vehiculos -H 'Content-Type: application/json' -d '{"modeloId":1,"placa":"TEST123","anio":2023,"kilometrajeActual":48000,"tipoCombustible":"Gasolina"}')
test "$creado" = "201"
python3 -c 'import json; d=json.load(open("/tmp/autotrack-creado.json")); assert d["success"] and d["data"]["placa"]=="TEST123" and d["data"]["id"]==1'

curl -fsS http://127.0.0.1:5117/api/v1/vehiculos/1 > /tmp/autotrack-vehiculo.json
python3 -c 'import json; d=json.load(open("/tmp/autotrack-vehiculo.json")); assert d["data"]["kilometrajeActual"]==48000'

actualizado=$(curl -sS -o /tmp/autotrack-actualizado.json -w '%{http_code}' -X PUT http://127.0.0.1:5117/api/v1/vehiculos/1 -H 'Content-Type: application/json' -d '{"modeloId":1,"placa":"TEST123","anio":2023,"kilometrajeActual":48500,"tipoCombustible":"Gasolina"}')
test "$actualizado" = "200"

carga=$(curl -sS -o /tmp/autotrack-carga.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/vehiculos/1/combustible -H 'Content-Type: application/json' -d '{"fecha":"2026-10-20T18:00:00Z","kilometraje":48500,"litros":35.50,"precioPorLitro":3.74}')
test "$carga" = "201"
python3 -c 'import json; d=json.load(open("/tmp/autotrack-carga.json")); assert d["success"] and d["data"]["total"]==132.77'

curl -fsS http://127.0.0.1:5117/api/v1/vehiculos/1/combustible > /tmp/autotrack-historial.json
python3 -c 'import json; d=json.load(open("/tmp/autotrack-historial.json")); assert len(d["data"])==1'

duplicado=$(curl -sS -o /tmp/autotrack-duplicado.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/vehiculos -H 'Content-Type: application/json' -d '{"modeloId":1,"placa":"TEST123","anio":2023,"kilometrajeActual":0,"tipoCombustible":"Gasolina"}')
test "$duplicado" = "409"

invalido=$(curl -sS -o /tmp/autotrack-invalido.json -w '%{http_code}' -X POST http://127.0.0.1:5117/api/v1/vehiculos -H 'Content-Type: application/json' -d '{"modeloId":1,"placa":"OTRO123","anio":2023,"kilometrajeActual":-1,"tipoCombustible":"Gasolina"}')
test "$invalido" = "400"

eliminado=$(curl -sS -o /tmp/autotrack-eliminado.json -w '%{http_code}' -X DELETE http://127.0.0.1:5117/api/v1/vehiculos/1)
test "$eliminado" = "200"

inexistente=$(curl -sS -o /tmp/autotrack-inexistente.json -w '%{http_code}' http://127.0.0.1:5117/api/v1/vehiculos/1)
test "$inexistente" = "404"

echo "AutoTrack: pruebas REST completadas"
