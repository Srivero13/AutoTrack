# AutoTrack

API REST de gestión vehicular desarrollada con ASP.NET Core, MySQL, Entity Framework Core, AutoMapper y FluentValidation.

## Tecnologías

- .NET 8 y C#
- MySQL 8
- Entity Framework Core con Pomelo
- AutoMapper
- FluentValidation
- Swagger y Postman
- Docker Compose para la base de datos local

## Estructura

| Proyecto | Responsabilidad |
|---|---|
| `AutoTrack.Api` | Endpoints, respuestas HTTP, middleware |
| `AutoTrack.Services` | Servicios, validaciones, AutoMapper |
| `AutoTrack.Core` | Entidades, DTOs, contratos |
| `AutoTrack.Infrastructure` | EF Core, configuraciones y repositorios |

## Requisitos

.NET SDK 8, Docker Desktop y acceso a puertos 3307 y 5117.

## Ejecución en Windows

Desde la raíz del repositorio:

```powershell
docker compose up -d
dotnet restore AutoTrack.sln
dotnet run --project src/AutoTrack.Api --launch-profile http
```

Abrir `http://localhost:5117/swagger`.

La primera inicialización de MySQL ejecuta automáticamente `database/AutoTrack.sql` y crea las tablas y los datos iniciales.

La conexión de desarrollo se define en `src/AutoTrack.Api/appsettings.json`. Reemplazar la contraseña en un entorno no local mediante configuración segura.

## Base de datos y Database First

El diseño de base de datos está en `database/AutoTrack.sql`. El modelo C# está preparado para ese esquema con `DbContext` y clases de configuración separadas.

Para generar una referencia mediante ingeniería inversa desde MySQL:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/scaffold.ps1
```

El resultado se genera en `scaffold-output`, sin sobrescribir los archivos del proyecto. GitHub Actions comprueba que este procedimiento funciona.

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/v1/catalogos/marcas` | Listar marcas |
| GET | `/api/v1/catalogos/marcas/{id}/modelos` | Listar modelos |
| GET | `/api/v1/vehiculos` | Listar vehículos |
| GET | `/api/v1/vehiculos/{id}` | Consultar vehículo |
| POST | `/api/v1/vehiculos` | Registrar vehículo |
| PUT | `/api/v1/vehiculos/{id}` | Actualizar vehículo |
| DELETE | `/api/v1/vehiculos/{id}` | Desactivar vehículo |
| GET | `/api/v1/vehiculos/{id}/combustible` | Historial de cargas |
| POST | `/api/v1/vehiculos/{id}/combustible` | Registrar carga |

## Reglas de negocio

- La placa es única incluso si el vehículo fue desactivado.
- Solo se admiten modelos existentes.
- El kilometraje no puede ser negativo ni retroceder.
- Los litros y el precio por litro deben ser positivos.
- El total de combustible se calcula en el servidor.
- No se permiten nuevas cargas para vehículos desactivados.

## Documentación

- [Avance y alcance](docs/avance.md)
- [Solicitudes para Postman](docs/solicitudes-postman.md)
- [Colección Postman](postman/AutoTrack.postman_collection.json)
- [Esquema SQL](database/AutoTrack.sql)

## Verificación automática

Cada actualización de `main` ejecuta una acción que compila, levanta MySQL, comprueba Database First y prueba los endpoints REST.
