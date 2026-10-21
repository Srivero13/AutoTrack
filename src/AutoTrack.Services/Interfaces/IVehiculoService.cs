using AutoTrack.Core.DTOs;

namespace AutoTrack.Services.Interfaces;

public interface IVehiculoService
{
    Task<List<VehiculoDto>> ObtenerTodosAsync();
    Task<VehiculoDto> ObtenerPorIdAsync(int id);
    Task<VehiculoDto> CrearAsync(CrearVehiculoDto datos);
    Task<VehiculoDto> ActualizarAsync(int id, ActualizarVehiculoDto datos);
    Task EliminarAsync(int id);
}
