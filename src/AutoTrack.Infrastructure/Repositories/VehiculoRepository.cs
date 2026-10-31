using AutoTrack.Core.Entities;
using AutoTrack.Core.DTOs;
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

    public async Task<List<Vehiculo>> ObtenerTodosAsync(FiltroVehiculoDto filtro)
    {
        var consulta = _context.Vehiculos.AsNoTracking().Where(x => x.Activo);

        if (!string.IsNullOrWhiteSpace(filtro.Marca))
        {
            var marca = filtro.Marca.Trim();
            consulta = consulta.Where(x => x.MarcaDecodificada != null &&
                x.MarcaDecodificada.Contains(marca));
        }

        if (filtro.Anio.HasValue)
            consulta = consulta.Where(x => x.Anio == filtro.Anio);

        if (filtro.KilometrajeMinimo.HasValue)
            consulta = consulta.Where(x => x.KilometrajeActual >= filtro.KilometrajeMinimo);

        if (filtro.KilometrajeMaximo.HasValue)
            consulta = consulta.Where(x => x.KilometrajeActual <= filtro.KilometrajeMaximo);

        if (!string.IsNullOrWhiteSpace(filtro.TipoCombustible))
        {
            var combustible = filtro.TipoCombustible.Trim();
            consulta = consulta.Where(x => x.TipoCombustible == combustible);
        }

        if (filtro.UsuarioId.HasValue)
            consulta = consulta.Where(x => x.UsuarioId == filtro.UsuarioId);

        return await consulta.OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<Vehiculo?> ObtenerPorIdAsync(int id)
    {
        return await _context.Vehiculos.FirstOrDefaultAsync(x => x.Id == id && x.Activo);
    }

    public async Task<Vehiculo?> ObtenerPorVinAsync(string vin)
    {
        return await _context.Vehiculos.FirstOrDefaultAsync(x => x.Vin == vin && x.Activo);
    }

    public async Task<bool> ExisteVinAsync(string vin, int? excluirId = null)
    {
        return await _context.Vehiculos.AnyAsync(x => x.Vin == vin &&
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
