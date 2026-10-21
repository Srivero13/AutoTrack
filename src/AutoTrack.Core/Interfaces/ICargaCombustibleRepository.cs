using AutoTrack.Core.Entities;

namespace AutoTrack.Core.Interfaces;

public interface ICargaCombustibleRepository
{
    Task<List<CargaCombustible>> ObtenerPorVehiculoAsync(int vehiculoId);
    Task AgregarAsync(CargaCombustible carga);
}
