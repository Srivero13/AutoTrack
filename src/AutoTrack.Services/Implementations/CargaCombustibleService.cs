using AutoMapper;
using AutoTrack.Core.DTOs;
using AutoTrack.Core.Entities;
using AutoTrack.Core.Exceptions;
using AutoTrack.Core.Interfaces;
using AutoTrack.Services.Interfaces;
using FluentValidation;

namespace AutoTrack.Services.Implementations;

public class CargaCombustibleService : ICargaCombustibleService
{
    private readonly IVehiculoRepository _vehiculos;
    private readonly ICargaCombustibleRepository _cargas;
    private readonly IMapper _mapper;
    private readonly IValidator<CrearCargaCombustibleDto> _validator;

    public CargaCombustibleService(
        IVehiculoRepository vehiculos,
        ICargaCombustibleRepository cargas,
        IMapper mapper,
        IValidator<CrearCargaCombustibleDto> validator)
    {
        _vehiculos = vehiculos;
        _cargas = cargas;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<List<CargaCombustibleDto>> ObtenerPorVehiculoAsync(int vehiculoId)
    {
        var vehiculo = await _vehiculos.ObtenerPorIdAsync(vehiculoId);

        if (vehiculo == null)
            throw new RecursoNoEncontradoException("Vehículo no encontrado");

        var cargas = await _cargas.ObtenerPorVehiculoAsync(vehiculoId);
        return _mapper.Map<List<CargaCombustibleDto>>(cargas);
    }

    public async Task<CargaCombustibleDto> CrearAsync(int vehiculoId, CrearCargaCombustibleDto datos)
    {
        await _validator.ValidateAndThrowAsync(datos);

        var vehiculo = await _vehiculos.ObtenerPorIdAsync(vehiculoId);

        if (vehiculo == null)
            throw new RecursoNoEncontradoException("Vehículo no encontrado");

        if (datos.Kilometraje < vehiculo.KilometrajeActual)
            throw new ReglaNegocioException("La carga no puede tener un kilometraje menor al actual");

        var carga = _mapper.Map<CargaCombustible>(datos);
        carga.VehiculoId = vehiculoId;
        carga.Total = Math.Round(datos.Litros * datos.PrecioPorLitro, 2, MidpointRounding.AwayFromZero);

        vehiculo.KilometrajeActual = datos.Kilometraje;
        await _cargas.AgregarAsync(carga);

        return _mapper.Map<CargaCombustibleDto>(carga);
    }
}
