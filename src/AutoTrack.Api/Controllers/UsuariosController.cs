using AutoTrack.Api.Response;
using AutoTrack.Core.DTOs;
using AutoTrack.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AutoTrack.Api.Controllers;

[ApiController]
[Authorize]
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
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var usuario = await _servicio.ObtenerPorIdAsync(id);
        return Ok(ApiResponse<List<UsuarioDto>>.Correcto(new List<UsuarioDto> { usuario }, "Usuarios obtenidos"));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        if (!EsPropietario(id))
            return Forbid();

        var usuario = await _servicio.ObtenerPorIdAsync(id);
        return Ok(ApiResponse<UsuarioDto>.Correcto(usuario, "Usuario obtenido"));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Crear([FromBody] CrearUsuarioDto datos)
    {
        var usuario = await _servicio.CrearAsync(datos);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = usuario.Id },
            ApiResponse<UsuarioDto>.Correcto(usuario, "Usuario registrado"));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUsuarioDto datos)
    {
        if (!EsPropietario(id))
            return Forbid();

        var usuario = await _servicio.ActualizarAsync(id, datos);
        return Ok(ApiResponse<UsuarioDto>.Correcto(usuario, "Usuario actualizado"));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!EsPropietario(id))
            return Forbid();

        await _servicio.EliminarAsync(id);
        return Ok(ApiResponse<object?>.Correcto(null, "Usuario desactivado"));
    }

    private bool EsPropietario(int id)
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) == id.ToString();
    }
}
