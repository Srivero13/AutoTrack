using AutoMapper;
using AutoTrack.Api.Response;
using AutoTrack.Core.DTOs;
using AutoTrack.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoTrack.Api.Controllers;

[ApiController]
[Route("api/v1/catalogos")]
public class CatalogosController : ControllerBase
{
    private readonly ICatalogoRepository _repository;
    private readonly IMapper _mapper;

    public CatalogosController(ICatalogoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet("marcas")]
    public async Task<IActionResult> ObtenerMarcas()
    {
        var marcas = await _repository.ObtenerMarcasAsync();
        var datos = _mapper.Map<List<MarcaDto>>(marcas);
        return Ok(ApiResponse<List<MarcaDto>>.Correcto(datos, "Marcas obtenidas"));
    }

    [HttpGet("marcas/{marcaId:int}/modelos")]
    public async Task<IActionResult> ObtenerModelos(int marcaId)
    {
        var modelos = await _repository.ObtenerModelosAsync(marcaId);
        var datos = _mapper.Map<List<ModeloDto>>(modelos);
        return Ok(ApiResponse<List<ModeloDto>>.Correcto(datos, "Modelos obtenidos"));
    }
}
