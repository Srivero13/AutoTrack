using AutoMapper;
using AutoTrack.Core.DTOs;
using AutoTrack.Core.Entities;

namespace AutoTrack.Services.Mappings;

public class PerfilMapeo : Profile
{
    public PerfilMapeo()
    {
        CreateMap<Marca, MarcaDto>();
        CreateMap<Modelo, ModeloDto>();
        CreateMap<Vehiculo, VehiculoDto>();
        CreateMap<CrearVehiculoDto, Vehiculo>();
        CreateMap<ActualizarVehiculoDto, Vehiculo>();
        CreateMap<CargaCombustible, CargaCombustibleDto>();
        CreateMap<CrearCargaCombustibleDto, CargaCombustible>();
    }
}
