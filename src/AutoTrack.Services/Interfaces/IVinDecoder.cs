using AutoTrack.Core.DTOs;

namespace AutoTrack.Services.Interfaces;

public interface IVinDecoder
{
    Task<VinDecodificadoDto> DecodificarAsync(string vin, short? anio = null);
}
