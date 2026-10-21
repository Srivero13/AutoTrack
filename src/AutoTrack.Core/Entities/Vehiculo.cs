namespace AutoTrack.Core.Entities;

public partial class Vehiculo
{
    public int Id { get; set; }
    public string Vin { get; set; } = string.Empty;
    public int? ModeloId { get; set; }
    public string? Placa { get; set; }
    public short? Anio { get; set; }
    public int KilometrajeActual { get; set; }
    public string? TipoCombustible { get; set; }
    public string? MarcaDecodificada { get; set; }
    public string? ModeloDecodificado { get; set; }
    public string? FabricanteDecodificado { get; set; }
    public string? PaisFabricacion { get; set; }
    public string? TipoVehiculo { get; set; }
    public string? TipoCarroceria { get; set; }
    public string EstadoDecodificacion { get; set; } = "Pendiente";
    public string? MensajeDecodificacion { get; set; }
    public DateTime? FechaDecodificacion { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
}
