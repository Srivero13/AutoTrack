# AutoTrack

AutoTrack es una API REST para llevar el registro de vehículos y sus cargas de combustible. Permite identificar un automóvil por su VIN, consultar información técnica del fabricante y mantener sus datos organizados en una base de datos.

Está desarrollada con **C# y ASP.NET Core 8**, utiliza **MySQL** y consulta el servicio **NHTSA vPIC** para decodificar números VIN.

## Funcionalidades

- **Consulta de VIN:** obtiene información disponible sobre marca, modelo, año, fabricante y país de fabricación antes de registrar un vehículo.
- **Registro de vehículos:** guarda un automóvil por su VIN, con placa, kilometraje y otros datos opcionales.
- **Búsqueda y filtros:** permite consultar vehículos por ID o VIN y filtrar por marca, año, kilometraje, combustible o usuario.
- **Actualización de datos:** modifica la información del vehículo y permite actualizar su decodificación.
- **Control de kilometraje:** evita registrar valores negativos o reducir el kilometraje ya guardado.
- **Cargas de combustible:** almacena fecha, kilometraje, litros y precio por litro; calcula automáticamente el importe total.
- **Usuarios:** permite crear cuentas, iniciar y cerrar sesión, consultar el perfil y actualizar los datos personales.
- **Desactivación de registros:** conserva los datos de usuarios y vehículos desactivados sin eliminarlos físicamente de MySQL.

Los datos disponibles al decodificar un VIN dependen de la cobertura de NHTSA. Es posible recibir información parcial, especialmente para vehículos de otros mercados.

## Tecnologías

| Tecnología | Uso |
| --- | --- |
| .NET 8 / ASP.NET Core | API HTTP |
| C# | Lógica de la aplicación |
| MySQL 8 | Almacenamiento de datos |
| Entity Framework Core | Acceso a la base de datos |
| FluentValidation | Validación de datos |
| AutoMapper | Conversión entre entidades y DTOs |
| Docker Compose | Ejecución local de MySQL |
| Swagger / OpenAPI | Exploración y prueba de endpoints |
| NHTSA vPIC | Decodificación de VIN |

## Instalación

### Requisitos

- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) o Docker Engine con Compose
- Git

### Iniciar el proyecto

Clona el repositorio:

```bash
git clone https://github.com/Srivero13/AutoTrack.git
cd AutoTrack
```

Inicia MySQL:

```bash
docker compose up -d --wait
```

Restaura los paquetes y ejecuta la API:

```bash
dotnet restore AutoTrack.sln
dotnet run --project src/AutoTrack.Api --launch-profile http
```

La API estará disponible en:

- **API:** http://localhost:5117
- **Swagger:** http://localhost:5117/swagger

En la primera ejecución con un volumen de MySQL nuevo, Docker carga automáticamente el esquema de `database/AutoTrack.sql`.

Si utilizas una base creada con una versión anterior, aplica las migraciones que todavía no tenga. Por ejemplo, para una base que ya contiene la tabla `Usuario` pero no las contraseñas:

```bash
docker exec -i autotrack-mysql mysql -u autotrack -pautotrack123 autotrack < database/migrations/004_login.sql
```

Si la tabla `Usuario` tampoco existe, aplica primero `003_usuarios.sql` y después `004_login.sql`. No ejecutes estas migraciones dos veces sobre una base ya actualizada.

> La configuración incluida es para desarrollo local. Antes de publicar la API, configura credenciales propias, HTTPS y controles de acceso adecuados.

## Uso básico

Puedes realizar las peticiones desde Swagger, Postman o cualquier cliente HTTP.

### Consultar un VIN

```http
GET http://localhost:5117/api/v1/vin/1HGCM82633A004352
```

Esta consulta no registra el vehículo.

### Registrar un vehículo

```http
POST http://localhost:5117/api/v1/vehiculos
Content-Type: application/json
```

```json
{
  "vin": "1HGCM82633A004352",
  "placa": "ABC123",
  "kilometrajeActual": 48000
}
```

El VIN debe tener 17 caracteres válidos y no puede estar repetido.

### Registrar un usuario

```http
POST http://localhost:5117/api/v1/usuarios
Content-Type: application/json
```

```json
{
  "nombre": "Ana",
  "apellido": "Perez",
  "correo": "ana.perez@example.com",
  "clave": "Ejemplo2026#"
}
```

### Iniciar sesión

```http
POST http://localhost:5117/api/v1/auth/login
Content-Type: application/json
```

```json
{
  "correo": "ana.perez@example.com",
  "clave": "Ejemplo2026#"
}
```

Al iniciar sesión, el servidor establece una cookie HTTP que se utiliza para acceder al perfil y administrar los datos de la cuenta. Las contraseñas se almacenan mediante un hash con sal, no como texto legible.

### Registrar una carga de combustible

```http
POST http://localhost:5117/api/v1/vehiculos/1/combustible
Content-Type: application/json
```

```json
{
  "fecha": "2026-10-20T18:00:00Z",
  "kilometraje": 48500,
  "litros": 35.5,
  "precioPorLitro": 3.74
}
```

Reemplaza `1` por el ID del vehículo que deseas utilizar.

## Endpoints principales

| Método | Ruta | Descripción |
| --- | --- | --- |
| GET | `/api/v1/vin/{vin}` | Consultar información de un VIN |
| GET | `/api/v1/catalogos/marcas` | Obtener marcas |
| GET | `/api/v1/catalogos/marcas/{marcaId}/modelos` | Obtener modelos de una marca |
| GET | `/api/v1/vehiculos` | Listar y filtrar vehículos |
| GET | `/api/v1/vehiculos/{id}` | Consultar un vehículo |
| GET | `/api/v1/vehiculos/vin/{vin}` | Buscar por VIN |
| POST | `/api/v1/vehiculos` | Registrar vehículo |
| PUT | `/api/v1/vehiculos/{id}` | Actualizar vehículo |
| POST | `/api/v1/vehiculos/{id}/decodificar` | Actualizar información del VIN |
| DELETE | `/api/v1/vehiculos/{id}` | Desactivar vehículo |
| GET | `/api/v1/vehiculos/{id}/combustible` | Consultar cargas de combustible |
| POST | `/api/v1/vehiculos/{id}/combustible` | Registrar carga de combustible |
| POST | `/api/v1/usuarios` | Crear usuario |
| GET | `/api/v1/usuarios` | Consultar usuario autenticado |
| GET | `/api/v1/usuarios/{id}` | Consultar datos de la cuenta |
| PUT | `/api/v1/usuarios/{id}` | Actualizar cuenta |
| DELETE | `/api/v1/usuarios/{id}` | Desactivar cuenta |
| POST | `/api/v1/auth/login` | Iniciar sesión |
| GET | `/api/v1/auth/perfil` | Consultar perfil |
| POST | `/api/v1/auth/logout` | Cerrar sesión |

Las operaciones de perfil y administración de usuarios requieren una sesión válida, excepto el registro. Los endpoints de vehículos y combustible todavía no están protegidos por autenticación; esta versión debe utilizarse en un entorno de desarrollo y no exponerse públicamente sin añadir esa protección.

## Estructura del proyecto

```text
AutoTrack/
├── src/
│   ├── AutoTrack.Api/
│   ├── AutoTrack.Core/
│   ├── AutoTrack.Services/
│   └── AutoTrack.Infrastructure/
├── database/
├── postman/
├── scripts/
├── docker-compose.yml
└── AutoTrack.sln
```

- **Api:** recibe peticiones HTTP y devuelve respuestas.
- **Core:** contiene los modelos, contratos y objetos de transferencia.
- **Services:** implementa validaciones y reglas de negocio.
- **Infrastructure:** gestiona el acceso a MySQL mediante Entity Framework Core.

La colección de peticiones se encuentra en [`postman/AutoTrack.postman_collection.json`](postman/AutoTrack.postman_collection.json).

## Pruebas

Con Docker y MySQL en funcionamiento, las pruebas de integración pueden ejecutarse con:

```bash
bash scripts/test-api.sh
```

El repositorio también incluye un flujo de GitHub Actions que compila el proyecto y comprueba las operaciones de la API.
