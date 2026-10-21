# AutoTrack — Avance inicial

## Objetivo

Desarrollar una API REST para gestionar vehículos y cargas de combustible, con persistencia relacional y reglas de negocio.

## Funciones de la primera versión

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

Las clases y configuraciones actuales reflejan ese esquema. El script `scripts/scaffold.ps1` permite comprobar la ingeniería inversa desde MySQL en una carpeta temporal y comparar sus clases con las existentes antes de reemplazarlas.

## Pendiente

- Ejecutar el build y resolver posibles problemas de dependencias.
- Probar las solicitudes con MySQL y Postman.
- Guardar capturas y respuestas observadas.
- Incorporar mantenimientos y alertas en iteraciones posteriores.
