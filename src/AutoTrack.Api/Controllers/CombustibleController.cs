using AutoTrack.Api.Response;
using AutoTrack.Core.DTOs;
using AutoTrack.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoTrack.Api.Controllers;

[ApiController]
[Route("api/v1/vehiculos/{vehiculoId:int}/combustible")]
public class CombustibleController : ControllerBase
{
    private readonly ICargaCombustibleService _servicio;

    public CombustibleController(ICargaCombustibleService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerHistorial(int vehiculoId)
    {
        var cargas = await _servicio.ObtenerPorVehiculoAsync(vehiculoId);
        return Ok(ApiResponse<List<CargaCombustibleDto>>.Correcto(cargas, "Historial obtenido"));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(int vehiculoId, [FromBody] CrearCargaCombustibleDto datos)
    {
        var carga = await _servicio.CrearAsync(vehiculoId, datos);
        return StatusCode(201, ApiResponse<CargaCombustibleDto>.Correcto(carga, "Carga registrada"));
    }
}
