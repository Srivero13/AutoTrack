namespace AutoTrack.Core.DTOs;

public class VinDecodificadoDto
{
    public string Vin { get; set; } = string.Empty;
    public string? Marca { get; set; }
    public string? Modelo { get; set; }
    public short? Anio { get; set; }
    public string? Fabricante { get; set; }
    public string? PaisFabricacion { get; set; }
    public string? TipoVehiculo { get; set; }
    public string? TipoCarroceria { get; set; }
    public string? Combustible { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public string? Mensaje { get; set; }
    public DateTime? FechaConsulta { get; set; }
}
