namespace AutoTrack.Core.DTOs;

public class CrearUsuarioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
    public string? Telefono { get; set; }
}
