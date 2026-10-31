using System.Security.Claims;
using AutoTrack.Api.Response;
using AutoTrack.Core.DTOs;
using AutoTrack.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AutoTrack.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioService _servicio;

    public AuthController(IUsuarioService servicio)
    {
        _servicio = servicio;
    }

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login([FromBody] IniciarSesionDto datos)
    {
        var usuario = await _servicio.IniciarSesionAsync(datos);

        var identidad = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim(ClaimTypes.Name, usuario.Nombre)
        }, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidad));

        return Ok(ApiResponse<UsuarioDto>.Correcto(usuario, "Sesión iniciada"));
    }

    [HttpGet("perfil")]
    [Authorize]
    public async Task<IActionResult> Perfil()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var usuario = await _servicio.ObtenerPorIdAsync(id);
        return Ok(ApiResponse<UsuarioDto>.Correcto(usuario, "Perfil obtenido"));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(ApiResponse<object?>.Correcto(null, "Sesión cerrada"));
    }
}
