# AutoTrack

API REST para gestión vehicular y combustible con identificación única por VIN y decodificación automática.

## Tecnologías

- ASP.NET Core y .NET 8
- MySQL 8 y Entity Framework Core
- Database First con Scaffold
- AutoMapper y FluentValidation
- NHTSA vPIC para decodificación VIN
- Swagger, Postman y Docker Compose

## Arquitectura

| Proyecto | Responsabilidad |
|---|---|
| `AutoTrack.Api` | Controladores, JSON, configuración y middleware |
| `AutoTrack.Core` | Entidades, DTOs e interfaces de repositorios |
| `AutoTrack.Services` | Reglas de negocio, validadores y decodificador VIN |
| `AutoTrack.Infrastructure` | MySQL, DbContext, configuraciones y repositorios |

## Ejecución

Requiere .NET SDK 8 y Docker Desktop.

```powershell
docker compose up -d
dotnet restore AutoTrack.sln
dotnet run --project src/AutoTrack.Api --launch-profile http
```

Swagger: `http://localhost:5117/swagger`.

En una base nueva, `database/AutoTrack.sql` se ejecuta automáticamente. En una instalación anterior, no se debe borrar el volumen de MySQL para actualizar el esquema.

## Migrar una instalación anterior

1. Realizar una copia de seguridad de MySQL.
2. Ejecutar `database/migrations/001_preparar_vin.sql` una sola vez.
3. Asignar los VIN reales de 17 caracteres a los vehículos existentes con sentencias `UPDATE Vehiculo SET Vin = '...' WHERE Id = ...`.
4. Verificar que no queden registros con VIN nulo y que ninguno esté duplicado.
5. Ejecutar `database/migrations/002_finalizar_vin.sql` una sola vez.

Las instalaciones nuevas no necesitan ejecutar estas migraciones.

## Decodificación VIN

El VIN es obligatorio, de 17 caracteres, sin I, O ni Q, y único incluso si se desactiva el vehículo. La placa es opcional y se permite repetirla.

La API consulta `https://vpic.nhtsa.dot.gov/api/vehicles/DecodeVinValues/{vin}?format=json` al registrar un vehículo. Al buscarlo, devuelve la información guardada: marca, modelo, año, fabricante, país de fabricación, tipo de vehículo, carrocería y combustible cuando están disponibles.

La cobertura de NHTSA vPIC se concentra en vehículos destinados al mercado estadounidense. Para vehículos de Bolivia y otros mercados, la decodificación puede ser parcial o no devolver datos. El registro sigue siendo posible con VIN válido y campos opcionales completados manualmente.

La API conserva los datos decodificados en MySQL para no depender de NHTSA en cada consulta.

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/v1/vin/{vin}` | Vista previa del VIN sin registrar |
| GET | `/api/v1/catalogos/marcas` | Listar marcas |
| GET | `/api/v1/catalogos/marcas/{id}/modelos` | Listar modelos |
| GET | `/api/v1/vehiculos` | Listar vehículos |
| GET | `/api/v1/vehiculos/{id}` | Obtener vehículo y datos VIN |
| GET | `/api/v1/vehiculos/vin/{vin}` | Buscar un vehículo por VIN |
| POST | `/api/v1/vehiculos` | Registrar usando VIN |
| PUT | `/api/v1/vehiculos/{id}` | Actualizar datos editables |
| POST | `/api/v1/vehiculos/{id}/decodificar` | Refrescar datos del decodificador |
| DELETE | `/api/v1/vehiculos/{id}` | Desactivar vehículo |
| GET | `/api/v1/vehiculos/{id}/combustible` | Consultar historial |
| POST | `/api/v1/vehiculos/{id}/combustible` | Registrar carga |

El campo `Id` sigue siendo una clave interna para las relaciones de MySQL; el VIN es el identificador vehicular único.

## Ejemplo de creación

```http
POST /api/v1/vehiculos
Content-Type: application/json
```

```json
{
  "vin": "1HGCM82633A004352",
  "placa": "DEMO123",
  "kilometrajeActual": 48000
}
```

`modeloId`, `anio` y `tipoCombustible` son opcionales.

## Pruebas

GitHub Actions inicia MySQL y un simulador de la API VIN, compila el backend y prueba las operaciones REST sin depender de la disponibilidad externa de NHTSA.

- [Colección Postman](postman/AutoTrack.postman_collection.json)
- [Instrucciones de Postman](docs/solicitudes-postman.md)
- [Avance](docs/avance.md)
- [Esquema SQL](database/AutoTrack.sql)
