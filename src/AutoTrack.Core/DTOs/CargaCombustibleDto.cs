namespace AutoTrack.Core.DTOs;

public class CargaCombustibleDto
{
    public int Id { get; set; }
    public int VehiculoId { get; set; }
    public DateTime Fecha { get; set; }
    public int Kilometraje { get; set; }
    public decimal Litros { get; set; }
    public decimal PrecioPorLitro { get; set; }
    public decimal Total { get; set; }
}
