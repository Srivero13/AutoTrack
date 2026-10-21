# Solicitudes Postman

URL base: `http://localhost:5117/api/v1`

Encabezado para operaciones con cuerpo: `Content-Type: application/json`

## 1. Listar marcas

`GET /catalogos/marcas`

## 2. Consultar modelos Jetour

`GET /catalogos/marcas/1/modelos`

## 3. Crear vehículo

`POST /vehiculos`

```json
{
  "modeloId": 1,
  "placa": "ABC1234",
  "anio": 2023,
  "kilometrajeActual": 48000,
  "tipoCombustible": "Gasolina"
}
```

Resultado esperado: 201 Created y JSON con `success: true`.

## 4. Listar vehículos

`GET /vehiculos`

## 5. Consultar vehículo

`GET /vehiculos/1`

## 6. Actualizar vehículo

`PUT /vehiculos/1`

```json
{
  "modeloId": 1,
  "placa": "ABC1234",
  "anio": 2023,
  "kilometrajeActual": 48500,
  "tipoCombustible": "Gasolina"
}
```

## 7. Registrar carga de combustible

`POST /vehiculos/1/combustible`

```json
{
  "fecha": "2026-10-20T18:00:00Z",
  "kilometraje": 48500,
  "litros": 35.50,
  "precioPorLitro": 3.74
}
```

Resultado esperado: 201 Created, total calculado por el servidor de 132.77.

## 8. Historial de combustible

`GET /vehiculos/1/combustible`

## 9. Placa duplicada

Repetir el POST de vehículo con placa `ABC1234`.

Resultado esperado: 409 Conflict.

## 10. Kilometraje negativo

Enviar un POST de vehículo con `kilometrajeActual: -10` y otra placa.

Resultado esperado: 400 Bad Request.

## 11. Vehículo inexistente

`GET /vehiculos/99999`

Resultado esperado: 404 Not Found.

## 12. Desactivar vehículo

`DELETE /vehiculos/1`

Resultado esperado: 200 OK; el vehículo deja de aparecer en listados.

Los resultados reales y capturas se incorporarán después de ejecutar la API localmente.
