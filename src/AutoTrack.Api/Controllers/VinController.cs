using AutoTrack.Api.Response;
using AutoTrack.Core.DTOs;
using AutoTrack.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoTrack.Api.Controllers;

[ApiController]
[Route("api/v1/vin")]
public class VinController : ControllerBase
{
    private readonly IVinDecoder _decoder;

    public VinController(IVinDecoder decoder)
    {
        _decoder = decoder;
    }

    [HttpGet("{vin}")]
    public async Task<IActionResult> Decodificar(string vin, [FromQuery] short? anio = null)
    {
        var datos = await _decoder.DecodificarAsync(vin, anio);
        return Ok(ApiResponse<VinDecodificadoDto>.Correcto(datos, "Consulta VIN finalizada"));
    }
}
