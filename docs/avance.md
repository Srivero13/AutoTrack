# AutoTrack — Avance inicial

## Objetivo

Desarrollar una API REST para gestionar vehículos y cargas de combustible, con persistencia relacional y reglas de negocio.

## Funciones implementadas

- Listar marcas y modelos.
- Registrar, listar, consultar, actualizar y desactivar vehículos.
- Registrar y consultar cargas de combustible.
- Calcular el importe de cada carga en el servidor.
- Rechazar placas repetidas, modelos inexistentes y kilometraje regresivo.
- Responder con ApiResponse y códigos HTTP correspondientes.

## Arquitectura

- AutoTrack.Api: controladores REST, manejo de errores y respuestas.
- AutoTrack.Services: lógica de negocio, DTOs validados y perfiles AutoMapper.
- AutoTrack.Core: entidades, DTOs, contratos y excepciones.
- AutoTrack.Infrastructure: repositorios, DbContext y configuraciones Entity Framework.

## Base de datos

La fuente del modelo relacional es `database/AutoTrack.sql`. Contiene Marca, Modelo, Vehiculo y CargaCombustible.

Las clases y configuraciones actuales reflejan ese esquema. El script `scripts/scaffold.ps1` ejecuta ingeniería inversa desde MySQL en `scaffold-output` sin sobrescribir los archivos principales.

## Validación realizada

GitHub Actions ha comprobado:

- Restauración de dependencias y compilación Release de las cuatro capas.
- Inicio de MySQL con esquema y datos iniciales.
- Generación de los modelos con `dotnet ef dbcontext scaffold`.
- Creación, consulta, actualización y desactivación de vehículos.
- Registro de combustible y cálculo del importe.
- Errores 400, 404 y 409.

Se incluye una colección Postman importable en `postman/AutoTrack.postman_collection.json`.

## Pendiente

- Ejecutar Postman localmente durante la presentación y guardar capturas.
- Integrar mantenimientos y alertas en una siguiente versión.
- Añadir autenticación si se exige en la evaluación.
