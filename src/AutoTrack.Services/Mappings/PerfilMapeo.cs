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
        CreateMap<Vehiculo, VinDecodificadoDto>()
            .ForMember(x => x.Marca, x => x.MapFrom(v => v.MarcaDecodificada))
            .ForMember(x => x.Modelo, x => x.MapFrom(v => v.ModeloDecodificado))
            .ForMember(x => x.Fabricante, x => x.MapFrom(v => v.FabricanteDecodificado))
            .ForMember(x => x.Combustible, x => x.MapFrom(v => v.TipoCombustible))
            .ForMember(x => x.Estado, x => x.MapFrom(v => v.EstadoDecodificacion))
            .ForMember(x => x.Mensaje, x => x.MapFrom(v => v.MensajeDecodificacion))
            .ForMember(x => x.FechaConsulta, x => x.MapFrom(v => v.FechaDecodificacion));
        CreateMap<Vehiculo, VehiculoDto>()
            .ForMember(x => x.InformacionVin, x => x.MapFrom(v => v));
        CreateMap<CrearVehiculoDto, Vehiculo>();
        CreateMap<ActualizarVehiculoDto, Vehiculo>();
        CreateMap<CargaCombustible, CargaCombustibleDto>();
        CreateMap<CrearCargaCombustibleDto, CargaCombustible>();
    }
}
