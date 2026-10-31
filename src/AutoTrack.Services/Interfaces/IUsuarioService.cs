using AutoTrack.Core.DTOs;

namespace AutoTrack.Services.Interfaces;

public interface IUsuarioService
{
    Task<List<UsuarioDto>> ObtenerTodosAsync();
    Task<UsuarioDto> ObtenerPorIdAsync(int id);
    Task<UsuarioDto> CrearAsync(CrearUsuarioDto datos);
    Task<UsuarioDto> IniciarSesionAsync(IniciarSesionDto datos);
    Task<UsuarioDto> ActualizarAsync(int id, ActualizarUsuarioDto datos);
    Task EliminarAsync(int id);
}
