# Solicitudes Postman

URL base: `http://localhost:5117/api/v1`

Importar `postman/AutoTrack.postman_collection.json` en Postman para ejecutar las pruebas en orden.

## 1. Vista previa del VIN

`GET /vin/1HGCM82633A004352`

Decodifica el VIN mediante NHTSA vPIC y devuelve los datos disponibles sin registrar un vehículo.

## 2. Registrar vehículo

`POST /vehiculos`

```json
{
  "vin": "1HGCM82633A004352",
  "placa": "DEMO123",
  "kilometrajeActual": 48000
}
```

Se espera HTTP 201, VIN guardado, año y marca/modelo autocompletados cuando NHTSA disponga de esos datos.

## 3. Listar vehículos

`GET /vehiculos`

## 4. Obtener vehículo por VIN

`GET /vehiculos/vin/1HGCM82633A004352`

Verificar `informacionVin`.

## 5. Obtener por ID interno

`GET /vehiculos/1`

## 6. Actualizar kilometraje

`PUT /vehiculos/1`

```json
{
  "placa": "DEMO123",
  "kilometrajeActual": 48500
}
```

El VIN es inmutable.

## 7. Refrescar decodificación

`POST /vehiculos/1/decodificar`

## 8. Registrar combustible

`POST /vehiculos/1/combustible`

```json
{
  "fecha": "2026-10-20T18:00:00Z",
  "kilometraje": 48500,
  "litros": 35.50,
  "precioPorLitro": 3.74
}
```

Se espera HTTP 201 y total 132.77.

## 9. Historial de combustible

`GET /vehiculos/1/combustible`

## 10. VIN duplicado

Repetir POST con el mismo VIN: HTTP 409.

## 11. VIN inválido

Intentar registrar `VININVALIDO`: HTTP 400.

## 12. VIN sin datos en NHTSA

Usar un VIN de 17 caracteres que no aparezca en la base de NHTSA con año manual. El registro debe ser posible y la respuesta indicará `SinDatos` o `Parcial`.

## 13. Desactivar y consultar

`DELETE /vehiculos/1` seguido de `GET /vehiculos/1`: HTTP 200 y después HTTP 404.

Las capturas y respuestas reales deben incorporarse desde el entorno local durante la presentación.
