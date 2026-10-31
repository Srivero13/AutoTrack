namespace AutoTrack.Core.DTOs;

public class FiltroVehiculoDto
{
    public string? Marca { get; set; }
    public short? Anio { get; set; }
    public int? KilometrajeMinimo { get; set; }
    public int? KilometrajeMaximo { get; set; }
    public string? TipoCombustible { get; set; }
    public int? UsuarioId { get; set; }
}
