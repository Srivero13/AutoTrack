namespace AutoTrack.Core.DTOs;

public class VehiculoDto
{
    public int Id { get; set; }
    public int ModeloId { get; set; }
    public string Placa { get; set; } = string.Empty;
    public short Anio { get; set; }
    public int KilometrajeActual { get; set; }
    public string TipoCombustible { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
}
