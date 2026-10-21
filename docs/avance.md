# AutoTrack — Avance

## Objetivo

Gestionar vehículos particulares identificados por VIN, con datos decodificados del vehículo e historial de cargas de combustible.

## Implementado

- API ASP.NET Core en cuatro capas.
- MySQL con Marca, Modelo, Vehiculo y CargaCombustible.
- Configuraciones IEntityTypeConfiguration y comprobación de Database First mediante Scaffold.
- CRUD de vehículos, desactivación lógica y persistencia.
- VIN obligatorio y único; placa opcional, sin restricción de unicidad.
- Integración NHTSA vPIC para crear vehículos y consultar datos antes del registro.
- Almacenamiento local de información VIN para mostrarla sin depender de internet.
- Refresco explícito de decodificación.
- Registro y consulta de cargas de combustible.
- DTOs, AutoMapper, FluentValidation, ApiResponse y middleware de errores.
- Pruebas CI con MySQL y simulación HTTP de VIN.

## Pendiente

- Completar CRUD de combustible.
- Evidencias locales en Postman y capturas para la documentación.
- Incorporar mantenimientos y alertas.
- Mejorar cobertura VIN internacional según necesidad.
