using AutoTrack.Core.Entities;
using AutoTrack.Core.Interfaces;
using AutoTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoTrack.Infrastructure.Repositories;

public class VehiculoRepository : IVehiculoRepository
{
    private readonly AutoTrackContext _context;

    public VehiculoRepository(AutoTrackContext context)
    {
        _context = context;
    }

    public async Task<List<Vehiculo>> ObtenerTodosAsync()
    {
        return await _context.Vehiculos.AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<Vehiculo?> ObtenerPorIdAsync(int id)
    {
        return await _context.Vehiculos.FirstOrDefaultAsync(x => x.Id == id && x.Activo);
    }

    public async Task<bool> ExistePlacaAsync(string placa, int? excluirId = null)
    {
        return await _context.Vehiculos.AnyAsync(x => x.Placa == placa &&
            (!excluirId.HasValue || x.Id != excluirId.Value));
    }

    public async Task<bool> ExisteModeloAsync(int modeloId)
    {
        return await _context.Modelos.AnyAsync(x => x.Id == modeloId);
    }

    public async Task AgregarAsync(Vehiculo vehiculo)
    {
        await _context.Vehiculos.AddAsync(vehiculo);
        await _context.SaveChangesAsync();
    }

    public async Task GuardarAsync()
    {
        await _context.SaveChangesAsync();
    }
}
