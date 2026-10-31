using AutoTrack.Api.Response;
using AutoTrack.Core.DTOs;
using AutoTrack.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoTrack.Api.Controllers;

[ApiController]
[Route("api/v1/vehiculos")]
public class VehiculosController : ControllerBase
{
    private readonly IVehiculoService _servicio;

    public VehiculosController(IVehiculoService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var vehiculos = await _servicio.ObtenerTodosAsync();
        return Ok(ApiResponse<List<VehiculoDto>>.Correcto(vehiculos, "Vehículos obtenidos"));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var vehiculo = await _servicio.ObtenerPorIdAsync(id);
        return Ok(ApiResponse<VehiculoDto>.Correcto(vehiculo, "Vehículo obtenido"));
    }

    [HttpGet("vin/{vin}")]
    public async Task<IActionResult> ObtenerPorVin(string vin)
    {
        var vehiculo = await _servicio.ObtenerPorVinAsync(vin);
        return Ok(ApiResponse<VehiculoDto>.Correcto(vehiculo, "Vehículo obtenido por VIN"));
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearVehiculoDto datos)
    {
        var vehiculo = await _servicio.CrearAsync(datos);
        var respuesta = ApiResponse<VehiculoDto>.Correcto(vehiculo, "Vehículo registrado");
        return CreatedAtAction(nameof(ObtenerPorId), new { id = vehiculo.Id }, respuesta);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarVehiculoDto datos)
    {
        var vehiculo = await _servicio.ActualizarAsync(id, datos);
        return Ok(ApiResponse<VehiculoDto>.Correcto(vehiculo, "Vehículo actualizado"));
    }

    [HttpPost("{id:int}/decodificar")]
    public async Task<IActionResult> Decodificar(int id)
    {
        var vehiculo = await _servicio.ActualizarDecodificacionAsync(id);
        return Ok(ApiResponse<VehiculoDto>.Correcto(vehiculo, "Información VIN actualizada"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return Ok(ApiResponse<object?>.Correcto(null, "Vehículo desactivado"));
    }
}
