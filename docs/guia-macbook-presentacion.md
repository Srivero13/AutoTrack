# AutoTrack — Guía de instalación y presentación en MacBook Pro M2

Preparación para la presentación del 22 de octubre de 2026.

Repositorio: https://github.com/Srivero13/AutoTrack

## 1. Qué es AutoTrack

AutoTrack es una API REST escrita en C# con ASP.NET Core 8. Permite registrar vehículos identificados por VIN, consultar sus datos técnicos a través de NHTSA vPIC, actualizar su kilometraje, registrar cargas de combustible y consultar los registros guardados en MySQL.

El propósito final del proyecto es facilitar el control de los mantenimientos de un automóvil. Las alertas, los recalls, la estimación del valor de mercado, las recomendaciones de talleres y el inicio de sesión son funcionalidades planificadas, no implementadas en esta versión.

No hay interfaz web de usuario final. Swagger es una interfaz para documentar y probar endpoints de la API.

## 2. Instalaciones en Mac

1. Abrir Terminal (Command + Espacio y buscar Terminal).
2. Comprobar Git: `git --version`. Si macOS pide las herramientas de desarrollador, instalarlas; alternativamente ejecutar `xcode-select --install`.
3. Instalar .NET SDK 8 para macOS Arm64 desde https://dotnet.microsoft.com/download/dotnet/8.0 . Comprobar `dotnet --list-sdks`.
4. Instalar Docker Desktop para Mac con Apple silicon desde https://docs.docker.com/desktop/setup/install/mac-install/ . Abrir Docker Desktop y esperar a que esté iniciado.
5. Comprobar `docker version` y `docker compose version`.
6. Instalar Visual Studio Code desde https://code.visualstudio.com/download y la extensión C# Dev Kit.
7. Opcional: Postman desde https://www.postman.com/downloads/ .

## 3. Clonar y ejecutar

Abrir Terminal:

```bash
cd ~/Documents
git clone https://github.com/Srivero13/AutoTrack.git
cd AutoTrack
docker compose up -d --wait
docker compose ps
dotnet restore AutoTrack.sln
dotnet build AutoTrack.sln
dotnet run --project src/AutoTrack.Api --launch-profile http
```

Abrir http://localhost:5117/swagger en el navegador.

Dejar abierto el Terminal donde se ejecuta `dotnet run`, y detenerlo después con Control + C.

Si ya existe una carpeta clonada, usar `git pull --ff-only origin main` desde ella, en lugar de `git clone` otra vez.

## 4. Demostración de Swagger

En Swagger, abrir el endpoint, presionar «Try it out», completar los parámetros o el JSON y luego «Execute».

### 4.1 Consultar marcas

GET /api/v1/catalogos/marcas

Comprobar que aparecen los datos semilla (Jetour, Toyota y Mitsubishi).

### 4.2 Previsualizar VIN

GET /api/v1/vin/1HGCM82633A004352

El resultado depende de la disponibilidad y cobertura real de NHTSA vPIC. Un VIN bien formado no garantiza que el servicio encuentre todos los atributos. No afirmar que la identidad o titularidad del automóvil queda verificada.

### 4.3 Registrar automóvil

POST /api/v1/vehiculos

```json
{
  "vin": "1HGCM82633A004352",
  "placa": "DEMO123",
  "kilometrajeActual": 48000
}
```

Se espera HTTP 201 si ese VIN no estaba registrado. Guardar el `data.id` devuelto. Si ya existe, devolverá 409; usar una base de pruebas nueva o preparar un VIN distinto válido antes de la presentación. No borrar datos por sorpresa.

### 4.4 Buscar por VIN

GET /api/v1/vehiculos/vin/1HGCM82633A004352

Comprobar el objeto `informacionVin`. Estos campos se almacenan en MySQL durante el registro; la consulta no necesita llamar a NHTSA otra vez.

### 4.5 Consultar por ID

GET /api/v1/vehiculos/{id}

Sustituir `{id}` por el ID obtenido en 4.3.

### 4.6 Actualizar kilometraje

PUT /api/v1/vehiculos/{id}

```json
{
  "placa": "DEMO123",
  "kilometrajeActual": 48500
}
```

Se espera HTTP 200. El VIN no forma parte del DTO de actualización, por lo que no se modifica.

### 4.7 Registrar carga de combustible

POST /api/v1/vehiculos/{id}/combustible

```json
{
  "fecha": "2026-10-20T18:00:00Z",
  "kilometraje": 48500,
  "litros": 35.50,
  "precioPorLitro": 3.74
}
```

Se espera HTTP 201 y `total: 132.77`, calculado por el servidor.

### 4.8 Consultar historial

GET /api/v1/vehiculos/{id}/combustible

Se espera HTTP 200 y la carga registrada.

### 4.9 Probar reglas de negocio

- Repetir la creación con el mismo VIN: HTTP 409 (duplicado).
- Crear con `"vin": "VININVALIDO"`: HTTP 400 (formato).
- Intentar actualizar el kilometraje a 47000: HTTP 409 (no puede retroceder).
- Consultar un ID inexistente: HTTP 404.
- Comprobar `success`, `message`, `data` y `errors` en las respuestas.

No desactivar el vehículo hasta terminar los demás ejemplos. DELETE /api/v1/vehiculos/{id} lo desactiva lógicamente; no lo elimina físicamente de MySQL.

## 5. Ver datos en MySQL

Abrir otra ventana de Terminal:

```bash
docker exec -it autotrack-mysql mysql -u autotrack -p autotrack
```

Escribir la contraseña de desarrollo `autotrack123` cuando la solicite.

```sql
SHOW TABLES;
SELECT * FROM Marca;
SELECT * FROM Modelo;
SELECT Id, Vin, Placa, MarcaDecodificada, ModeloDecodificado, KilometrajeActual FROM Vehiculo;
SELECT * FROM CargaCombustible;
EXIT;
```

MySQL funciona en el puerto 3306 dentro del contenedor y en el 3307 desde el Mac.

## 6. Arquitectura en cuatro capas

El proyecto tiene cuatro carpetas dentro de `src`:

- `AutoTrack.Api`: recibe peticiones HTTP en controladores, configura servicios, Swagger y middleware.
- `AutoTrack.Core`: entidades, DTOs, interfaces y excepciones. No debería depender de infraestructura.
- `AutoTrack.Services`: validación, lógica y reglas de negocio, AutoMapper y consulta de VIN externo.
- `AutoTrack.Infrastructure`: DbContext, configuración EF Core y repositorios que hablan con MySQL.

Flujo básico:

```text
Swagger / Postman
        |
        v
VehiculosController
        |
        v
VehiculoService
     /       \
    v         v
VinDecoder   IVehiculoRepository
                |
                v
         VehiculoRepository
                |
                v
          AutoTrackContext
                |
                v
              MySQL
```

## 7. Archivos esenciales para abrir durante la defensa

- `src/AutoTrack.Api/Program.cs`: inyección de dependencias, conexión MySQL, AutoMapper, FluentValidation, Swagger.
- `src/AutoTrack.Api/Controllers/VehiculosController.cs`: rutas y verbos HTTP de vehículos.
- `src/AutoTrack.Api/Controllers/VinController.cs`: consulta externa de VIN.
- `src/AutoTrack.Core/Entities/Vehiculo.cs`: representación del automóvil.
- `src/AutoTrack.Core/DTOs/CrearVehiculoDto.cs`: información de entrada.
- `src/AutoTrack.Services/Validators/CrearVehiculoValidator.cs`: validación del VIN y kilómetros.
- `src/AutoTrack.Services/Implementations/VehiculoService.cs`: reglas de negocio y coordinación.
- `src/AutoTrack.Services/Implementations/VinDecoder.cs`: llamada HTTP a NHTSA.
- `src/AutoTrack.Services/Mappings/PerfilMapeo.cs`: conversiones AutoMapper.
- `src/AutoTrack.Infrastructure/Data/AutoTrackContext.cs`: EF Core DbContext.
- `src/AutoTrack.Infrastructure/Data/Configurations/VehiculoConfiguration.cs`: mapeo de columnas, restricciones y relación.
- `src/AutoTrack.Infrastructure/Repositories/VehiculoRepository.cs`: consultas SQL a través de EF Core.
- `src/AutoTrack.Api/Response/ApiResponse.cs`: estructura homogénea del JSON.
- `src/AutoTrack.Api/Middleware/ManejadorErroresMiddleware.cs`: errores HTTP.

## 8. Conceptos que debes saber explicar

- **API REST**: servicio que expone recursos mediante rutas y verbos HTTP.
- **Endpoint**: combinación de verbo y ruta que realiza una acción concreta.
- **GET**: consultar; **POST**: crear/ejecutar operación; **PUT**: actualizar; **DELETE**: desactivar en AutoTrack.
- **VIN**: número de identificación del vehículo. No valida legalmente quién es su propietario.
- **MySQL**: base relacional que persiste datos.
- **Docker**: ejecuta MySQL aislado dentro de un contenedor.
- **Entity Framework Core**: ORM que conecta clases C# con tablas relacionales.
- **DbContext**: sesión de trabajo con la base de datos.
- **Repositorio**: encapsula operaciones de acceso a datos.
- **DTO**: objeto de transferencia que define datos de petición o respuesta.
- **AutoMapper**: copia y transforma datos entre entidades y DTOs.
- **FluentValidation**: valida los datos recibidos según reglas configuradas.
- **Regla de negocio**: restricción funcional, como prohibir VIN duplicado o kilometraje regresivo.
- **ApiResponse**: estructura uniforme `success`, `message`, `data`, `errors`.
- **Swagger**: interfaz de documentación y pruebas, no el frontend final.
- **Database First**: partir de tablas MySQL para generar el modelo de EF. El repositorio incluye SQL y un comando Scaffold comprobado automáticamente, pero las clases generadas todavía no sustituyen las entidades existentes en el proyecto principal.

## 9. Modelo relacional

Tablas actuales:

- `Marca` (Id, Nombre): catálogo de marcas.
- `Modelo` (Id, MarcaId, Nombre): cada modelo pertenece a una marca.
- `Vehiculo` (Id, Vin, ModeloId opcional, Placa opcional, Anio, KilometrajeActual, datos decodificados, Activo, FechaRegistro): VIN único.
- `CargaCombustible` (Id, VehiculoId, Fecha, Kilometraje, Litros, PrecioPorLitro, Total): cada carga pertenece a un vehículo.

Cardinalidades:

- Marca 1:N Modelo.
- Modelo 1:N Vehiculo (la FK de Vehiculo puede ser nula).
- Vehiculo 1:N CargaCombustible.

`Id` es una clave interna para la base de datos. `Vin` es el identificador único del automóvil. La placa es opcional y no única.

## 10. Evidencias y límites

Revisar el workflow aprobado en https://github.com/Srivero13/AutoTrack/actions .

Las pruebas CI compilan, inician MySQL, ejecutan Scaffold y prueban operaciones REST. Para garantizar reproducibilidad, el flujo de pruebas usa un simulador local de la respuesta NHTSA. La respuesta real depende de internet y de la cobertura del servicio.

Todavía no están hechos: inicio de sesión, usuarios con permisos, mantenimientos, alertas, recalls, valoración y recomendaciones de talleres. Son el alcance planificado, no funcionalidades para afirmar como terminadas.

## 11. Errores frecuentes y solución

- `dotnet: command not found`: instalar SDK para macOS Arm64 y reiniciar Terminal.
- `docker: command not found`: abrir Docker Desktop o revisar su configuración de CLI.
- `Cannot connect to Docker daemon`: iniciar Docker Desktop.
- `port is already allocated: 3307`: algún servicio ocupa el puerto. Comprobar `lsof -nP -iTCP:3307 -sTCP:LISTEN`.
- `Address already in use: 5117`: otra API está usando el puerto. Comprobar `lsof -nP -iTCP:5117 -sTCP:LISTEN`.
- `Unable to connect to database`: comprobar `docker compose ps` y `docker compose logs mysql --tail=60`.
- `404 al abrir Swagger`: iniciar con `--launch-profile http`; Swagger se habilita en Development.
- `409 VIN duplicado`: el registro ya existe, buscarlo por VIN en lugar de recrearlo.
- Decodificación incompleta o no disponible: el servicio externo puede no tener información. Se puede completar la ficha manualmente y seguir demostrando almacenamiento y validaciones.

`docker compose down` detiene los contenedores y conserva los datos. **No ejecutar `docker compose down -v` salvo que se quiera borrar deliberadamente la base local y todas sus cargas.**

## 12. Guion de exposición de 5–8 minutos

1. Problema: las personas olvidan mantenimientos por tiempo o kilometraje.
2. Solución y alcance: AutoTrack gestionará vehículos y mantenimientos; la primera versión implementa la base de identificación y registro.
3. Arquitectura: explicar las cuatro capas y cómo pasa una solicitud del controlador a MySQL.
4. Modelo de datos: mostrar Marca, Modelo, Vehiculo y CargaCombustible y sus relaciones.
5. Swagger: consultar VIN, registrar, buscar, actualizar kilometraje, cargar combustible.
6. Pruebas de negocio: VIN duplicado (409) y kilometraje inválido.
7. Evidencia: mostrar las tablas MySQL y el workflow verde de GitHub Actions.
8. Próximos módulos: autenticación, mantenimientos, alertas, recalls, valoración y talleres.

## 13. Lista de comprobación la noche anterior

- [ ] Git, .NET 8, Docker y VS Code instalados.
- [ ] Repositorio clonado y actualizado.
- [ ] Docker Desktop abierto y MySQL healthy.
- [ ] `dotnet build AutoTrack.sln` sin errores.
- [ ] `http://localhost:5117/swagger` abre.
- [ ] GET de marcas funciona.
- [ ] VIN real de prueba consultado y resultado entendido.
- [ ] POST de vehículo probado sin VIN duplicado.
- [ ] Consulta SQL muestra el vehículo.
- [ ] Evidencia de GitHub Actions abierta.
- [ ] Cargador y adaptador de pantalla listos.

No dependas de internet para mostrar MySQL, el registro ya persistido o el código. Sí se necesita conexión para una consulta nueva a NHTSA vPIC.
