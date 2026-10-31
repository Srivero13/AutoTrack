using AutoTrack.Core.Entities;
using AutoTrack.Core.Interfaces;
using AutoTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoTrack.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AutoTrackContext _context;

    public UsuarioRepository(AutoTrackContext context)
    {
        _context = context;
    }

    public async Task<List<Usuario>> ObtenerTodosAsync()
    {
        return await _context.Usuarios.AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id && x.Activo);
    }

    public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(x => x.Correo == correo && x.Activo);
    }

    public async Task<bool> ExisteUsuarioAsync(int id)
    {
        return await _context.Usuarios.AnyAsync(x => x.Id == id && x.Activo);
    }

    public async Task<bool> ExisteCorreoAsync(string correo, int? excluirId = null)
    {
        return await _context.Usuarios.AnyAsync(x => x.Correo == correo &&
            (!excluirId.HasValue || x.Id != excluirId.Value));
    }

    public async Task AgregarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarAsync()
    {
        await _context.SaveChangesAsync();
    }
}
