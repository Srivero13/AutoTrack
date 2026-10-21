using AutoTrack.Core.DTOs;

namespace AutoTrack.Services.Interfaces;

public interface ICargaCombustibleService
{
    Task<List<CargaCombustibleDto>> ObtenerPorVehiculoAsync(int vehiculoId);
    Task<CargaCombustibleDto> CrearAsync(int vehiculoId, CrearCargaCombustibleDto datos);
}
