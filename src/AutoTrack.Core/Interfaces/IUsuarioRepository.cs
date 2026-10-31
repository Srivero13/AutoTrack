using AutoTrack.Core.Entities;

namespace AutoTrack.Core.Interfaces;

public interface IUsuarioRepository
{
    Task<List<Usuario>> ObtenerTodosAsync();
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<Usuario?> ObtenerPorCorreoAsync(string correo);
    Task<bool> ExisteUsuarioAsync(int id);
    Task<bool> ExisteCorreoAsync(string correo, int? excluirId = null);
    Task AgregarAsync(Usuario usuario);
    Task GuardarAsync();
}
