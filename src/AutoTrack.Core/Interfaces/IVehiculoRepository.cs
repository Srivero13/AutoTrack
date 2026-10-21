using AutoTrack.Core.Entities;

namespace AutoTrack.Core.Interfaces;

public interface IVehiculoRepository
{
    Task<List<Vehiculo>> ObtenerTodosAsync();
    Task<Vehiculo?> ObtenerPorIdAsync(int id);
    Task<bool> ExistePlacaAsync(string placa, int? excluirId = null);
    Task<bool> ExisteModeloAsync(int modeloId);
    Task AgregarAsync(Vehiculo vehiculo);
    Task GuardarAsync();
}
