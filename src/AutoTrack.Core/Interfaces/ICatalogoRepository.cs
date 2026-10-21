using AutoTrack.Core.Entities;

namespace AutoTrack.Core.Interfaces;

public interface ICatalogoRepository
{
    Task<List<Marca>> ObtenerMarcasAsync();
    Task<List<Modelo>> ObtenerModelosAsync(int marcaId);
}
