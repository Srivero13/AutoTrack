namespace AutoTrack.Core.DTOs;

public class ActualizarVehiculoDto
{
    public int? ModeloId { get; set; }
    public string? Placa { get; set; }
    public short? Anio { get; set; }
    public int KilometrajeActual { get; set; }
    public string? TipoCombustible { get; set; }
}
