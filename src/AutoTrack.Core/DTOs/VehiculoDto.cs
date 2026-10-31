namespace AutoTrack.Core.DTOs;

public class VehiculoDto
{
    public int Id { get; set; }
    public string Vin { get; set; } = string.Empty;
    public int? ModeloId { get; set; }
    public string? Placa { get; set; }
    public short? Anio { get; set; }
    public int KilometrajeActual { get; set; }
    public string? TipoCombustible { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
    public VinDecodificadoDto InformacionVin { get; set; } = new();
}
