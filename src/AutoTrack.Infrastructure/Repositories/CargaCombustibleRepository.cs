using AutoTrack.Core.Entities;
using AutoTrack.Core.Interfaces;
using AutoTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoTrack.Infrastructure.Repositories;

public class CargaCombustibleRepository : ICargaCombustibleRepository
{
    private readonly AutoTrackContext _context;

    public CargaCombustibleRepository(AutoTrackContext context)
    {
        _context = context;
    }

    public async Task<List<CargaCombustible>> ObtenerPorVehiculoAsync(int vehiculoId)
    {
        return await _context.CargasCombustible.AsNoTracking()
            .Where(x => x.VehiculoId == vehiculoId)
            .OrderByDescending(x => x.Fecha)
            .ThenByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task AgregarAsync(CargaCombustible carga)
    {
        await _context.CargasCombustible.AddAsync(carga);
        await _context.SaveChangesAsync();
    }
}
