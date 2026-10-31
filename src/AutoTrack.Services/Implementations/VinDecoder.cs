using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using AutoTrack.Core.DTOs;
using AutoTrack.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace AutoTrack.Services.Implementations;

public class VinDecoder : IVinDecoder
{
    private readonly HttpClient _cliente;

    public VinDecoder(HttpClient cliente)
    {
        _cliente = cliente;
    }

    public async Task<VinDecodificadoDto> DecodificarAsync(string vin, short? anio = null)
    {
        vin = vin.Trim().ToUpperInvariant();

        if (!Regex.IsMatch(vin, "^[A-HJ-NPR-Z0-9]{17}$"))
        {
            throw new ValidationException(new List<ValidationFailure>
            {
                new("Vin", "El VIN debe tener 17 caracteres alfanuméricos sin I, O ni Q")
            });
        }

        var resultado = new VinDecodificadoDto
        {
            Vin = vin,
            Estado = "NoDisponible",
            FechaConsulta = DateTime.UtcNow
        };

        try
        {
            var ruta = "api/vehicles/DecodeVinValues/" + vin + "?format=json";

            if (anio.HasValue)
                ruta += "&modelyear=" + anio.Value;

            var respuesta = await _cliente.GetFromJsonAsync<RespuestaNhtsa>(ruta);
            var datos = respuesta?.Results?.FirstOrDefault();

            if (datos == null)
            {
                resultado.Estado = "SinDatos";
                resultado.Mensaje = "El decodificador no devolvió información";
                return resultado;
            }

            resultado.Marca = Limpiar(datos.Make);
            resultado.Modelo = Limpiar(datos.Model);
            resultado.Fabricante = Limpiar(datos.Manufacturer);
            resultado.PaisFabricacion = Limpiar(datos.PlantCountry);
            resultado.TipoVehiculo = Limpiar(datos.VehicleType);
            resultado.TipoCarroceria = Limpiar(datos.BodyClass);
            resultado.Combustible = Limpiar(datos.FuelTypePrimary);
            resultado.Anio = short.TryParse(datos.ModelYear, out var anioDecodificado)
                ? anioDecodificado : null;

            var tieneDatos = resultado.Marca != null || resultado.Modelo != null ||
                resultado.Fabricante != null || resultado.PaisFabricacion != null ||
                resultado.TipoVehiculo != null;

            resultado.Estado = !tieneDatos ? "SinDatos" :
                resultado.Marca != null && resultado.Modelo != null && datos.ErrorCode == "0"
                    ? "Decodificado" : "Parcial";

            resultado.Mensaje = datos.ErrorCode == "0" ? null : Limitar(Limpiar(datos.ErrorText), 500);
            return resultado;
        }
        catch (HttpRequestException)
        {
            resultado.Mensaje = "No se pudo consultar el servicio VIN";
        }
        catch (TaskCanceledException)
        {
            resultado.Mensaje = "La consulta VIN excedió el tiempo de espera";
        }
        catch (JsonException)
        {
            resultado.Mensaje = "La respuesta del decodificador VIN no es válida";
        }

        return resultado;
    }

    private static string? Limpiar(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }

    private static string? Limitar(string? valor, int longitud)
    {
        return valor == null ? null : valor[..Math.Min(valor.Length, longitud)];
    }

    public class RespuestaNhtsa
    {
        public List<ResultadoNhtsa>? Results { get; set; }
    }

    public class ResultadoNhtsa
    {
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? ModelYear { get; set; }
        public string? Manufacturer { get; set; }
        public string? PlantCountry { get; set; }
        public string? VehicleType { get; set; }
        public string? BodyClass { get; set; }
        public string? FuelTypePrimary { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorText { get; set; }
    }
}
