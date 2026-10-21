using AutoMapper;
using AutoTrack.Core.DTOs;
using AutoTrack.Core.Entities;
using AutoTrack.Core.Exceptions;
using AutoTrack.Core.Interfaces;
using AutoTrack.Services.Interfaces;
using FluentValidation;

namespace AutoTrack.Services.Implementations;

public class VehiculoService : IVehiculoService
{
    private readonly IVehiculoRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CrearVehiculoDto> _crearValidator;
    private readonly IValidator<ActualizarVehiculoDto> _actualizarValidator;

    public VehiculoService(
        IVehiculoRepository repository,
        IMapper mapper,
        IValidator<CrearVehiculoDto> crearValidator,
        IValidator<ActualizarVehiculoDto> actualizarValidator)
    {
        _repository = repository;
        _mapper = mapper;
        _crearValidator = crearValidator;
        _actualizarValidator = actualizarValidator;
    }

    public async Task<List<VehiculoDto>> ObtenerTodosAsync()
    {
        var vehiculos = await _repository.ObtenerTodosAsync();
        return _mapper.Map<List<VehiculoDto>>(vehiculos);
    }

    public async Task<VehiculoDto> ObtenerPorIdAsync(int id)
    {
        var vehiculo = await _repository.ObtenerPorIdAsync(id);

        if (vehiculo == null)
            throw new RecursoNoEncontradoException("Vehículo no encontrado");

        return _mapper.Map<VehiculoDto>(vehiculo);
    }

    public async Task<VehiculoDto> CrearAsync(CrearVehiculoDto datos)
    {
        await _crearValidator.ValidateAndThrowAsync(datos);
        datos.Placa = datos.Placa.Trim().ToUpperInvariant();

        if (!await _repository.ExisteModeloAsync(datos.ModeloId))
            throw new ReglaNegocioException("El modelo indicado no existe");

        if (await _repository.ExistePlacaAsync(datos.Placa))
            throw new ReglaNegocioException("Ya existe un vehículo con esa placa");

        var vehiculo = _mapper.Map<Vehiculo>(datos);
        vehiculo.Activo = true;
        vehiculo.FechaRegistro = DateTime.UtcNow;

        await _repository.AgregarAsync(vehiculo);

        return _mapper.Map<VehiculoDto>(vehiculo);
    }

    public async Task<VehiculoDto> ActualizarAsync(int id, ActualizarVehiculoDto datos)
    {
        await _actualizarValidator.ValidateAndThrowAsync(datos);

        var vehiculo = await _repository.ObtenerPorIdAsync(id);

        if (vehiculo == null)
            throw new RecursoNoEncontradoException("Vehículo no encontrado");

        datos.Placa = datos.Placa.Trim().ToUpperInvariant();

        if (datos.KilometrajeActual < vehiculo.KilometrajeActual)
            throw new ReglaNegocioException("El kilometraje no puede disminuir");

        if (!await _repository.ExisteModeloAsync(datos.ModeloId))
            throw new ReglaNegocioException("El modelo indicado no existe");

        if (await _repository.ExistePlacaAsync(datos.Placa, id))
            throw new ReglaNegocioException("Ya existe un vehículo con esa placa");

        _mapper.Map(datos, vehiculo);
        await _repository.GuardarAsync();

        return _mapper.Map<VehiculoDto>(vehiculo);
    }

    public async Task EliminarAsync(int id)
    {
        var vehiculo = await _repository.ObtenerPorIdAsync(id);

        if (vehiculo == null)
            throw new RecursoNoEncontradoException("Vehículo no encontrado");

        vehiculo.Activo = false;
        await _repository.GuardarAsync();
    }
}
