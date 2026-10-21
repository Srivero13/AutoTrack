using AutoTrack.Core.Entities;
using AutoTrack.Core.Interfaces;
using AutoTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoTrack.Infrastructure.Repositories;

public class CatalogoRepository : ICatalogoRepository
{
    private readonly AutoTrackContext _context;

    public CatalogoRepository(AutoTrackContext context)
    {
        _context = context;
    }

    public async Task<List<Marca>> ObtenerMarcasAsync()
    {
        return await _context.Marcas.AsNoTracking().OrderBy(x => x.Nombre).ToListAsync();
    }

    public async Task<List<Modelo>> ObtenerModelosAsync(int marcaId)
    {
        return await _context.Modelos.AsNoTracking()
            .Where(x => x.MarcaId == marcaId)
            .OrderBy(x => x.Nombre)
            .ToListAsync();
    }
}
