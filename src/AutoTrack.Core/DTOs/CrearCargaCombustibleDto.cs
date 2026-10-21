namespace AutoTrack.Core.DTOs;

public class CrearCargaCombustibleDto
{
    public DateTime Fecha { get; set; }
    public int Kilometraje { get; set; }
    public decimal Litros { get; set; }
    public decimal PrecioPorLitro { get; set; }
}
