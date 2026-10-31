using AutoTrack.Api.Response;
using AutoTrack.Core.DTOs;
using AutoTrack.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AutoTrack.Api.Controllers;

[ApiController]
[Route("api/v1/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _servicio;

    public UsuariosController(IUsuarioService servicio)
    {
        _servicio = servicio;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var usuarios = await _servicio.ObtenerTodosAsync();
        return Ok(ApiResponse<List<UsuarioDto>>.Correcto(usuarios, "Usuarios obtenidos"));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var usuario = await _servicio.ObtenerPorIdAsync(id);
        return Ok(ApiResponse<UsuarioDto>.Correcto(usuario, "Usuario obtenido"));
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearUsuarioDto datos)
    {
        var usuario = await _servicio.CrearAsync(datos);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = usuario.Id },
            ApiResponse<UsuarioDto>.Correcto(usuario, "Usuario registrado"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUsuarioDto datos)
    {
        var usuario = await _servicio.ActualizarAsync(id, datos);
        return Ok(ApiResponse<UsuarioDto>.Correcto(usuario, "Usuario actualizado"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        await _servicio.EliminarAsync(id);
        return Ok(ApiResponse<object?>.Correcto(null, "Usuario desactivado"));
    }
}
