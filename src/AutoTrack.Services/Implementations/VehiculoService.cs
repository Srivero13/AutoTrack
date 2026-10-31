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
    private readonly IVinDecoder _decoder;
    private readonly IMapper _mapper;
    private readonly IValidator<CrearVehiculoDto> _crearValidator;
    private readonly IValidator<ActualizarVehiculoDto> _actualizarValidator;

    public VehiculoService(
        IVehiculoRepository repository,
        IVinDecoder decoder,
        IMapper mapper,
        IValidator<CrearVehiculoDto> crearValidator,
        IValidator<ActualizarVehiculoDto> actualizarValidator)
    {
        _repository = repository;
        _decoder = decoder;
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

    public async Task<VehiculoDto> ObtenerPorVinAsync(string vin)
    {
        var vehiculo = await _repository.ObtenerPorVinAsync(vin.Trim().ToUpperInvariant());

        if (vehiculo == null)
            throw new RecursoNoEncontradoException("Vehículo no encontrado");

        return _mapper.Map<VehiculoDto>(vehiculo);
    }

    public async Task<VehiculoDto> CrearAsync(CrearVehiculoDto datos)
    {
        datos.Vin = (datos.Vin ?? string.Empty).Trim().ToUpperInvariant();
        await _crearValidator.ValidateAndThrowAsync(datos);

        if (await _repository.ExisteVinAsync(datos.Vin))
            throw new ReglaNegocioException("Ya existe un vehículo con ese VIN");

        if (datos.ModeloId.HasValue && !await _repository.ExisteModeloAsync(datos.ModeloId.Value))
            throw new ReglaNegocioException("El modelo indicado no existe");

        var informacion = await _decoder.DecodificarAsync(datos.Vin, datos.Anio);
        var vehiculo = _mapper.Map<Vehiculo>(datos);

        vehiculo.Placa = Normalizar(datos.Placa);
        vehiculo.TipoCombustible = Normalizar(datos.TipoCombustible) ?? informacion.Combustible;
        vehiculo.Anio ??= informacion.Anio;
        vehiculo.Activo = true;
        vehiculo.FechaRegistro = DateTime.UtcNow;
        AplicarDecodificacion(vehiculo, informacion);

        await _repository.AgregarAsync(vehiculo);
        return _mapper.Map<VehiculoDto>(vehiculo);
    }

    public async Task<VehiculoDto> ActualizarAsync(int id, ActualizarVehiculoDto datos)
    {
        await _actualizarValidator.ValidateAndThrowAsync(datos);

        var vehiculo = await _repository.ObtenerPorIdAsync(id);

        if (vehiculo == null)
            throw new RecursoNoEncontradoException("Vehículo no encontrado");

        if (datos.KilometrajeActual < vehiculo.KilometrajeActual)
            throw new ReglaNegocioException("El kilometraje no puede disminuir");

        if (datos.ModeloId.HasValue && !await _repository.ExisteModeloAsync(datos.ModeloId.Value))
            throw new ReglaNegocioException("El modelo indicado no existe");

        datos.ModeloId ??= vehiculo.ModeloId;
        datos.Anio ??= vehiculo.Anio;
        datos.Placa ??= vehiculo.Placa;
        datos.TipoCombustible ??= vehiculo.TipoCombustible;

        _mapper.Map(datos, vehiculo);
        vehiculo.Placa = Normalizar(vehiculo.Placa);
        await _repository.GuardarAsync();

        return _mapper.Map<VehiculoDto>(vehiculo);
    }

    public async Task<VehiculoDto> ActualizarDecodificacionAsync(int id)
    {
        var vehiculo = await _repository.ObtenerPorIdAsync(id);

        if (vehiculo == null)
            throw new RecursoNoEncontradoException("Vehículo no encontrado");

        var informacion = await _decoder.DecodificarAsync(vehiculo.Vin, vehiculo.Anio);
        AplicarDecodificacion(vehiculo, informacion);
        vehiculo.Anio ??= informacion.Anio;
        vehiculo.TipoCombustible ??= informacion.Combustible;

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

    private static string? Normalizar(string? texto)
    {
        return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim().ToUpperInvariant();
    }

    private static void AplicarDecodificacion(Vehiculo vehiculo, VinDecodificadoDto datos)
    {
        vehiculo.MarcaDecodificada = datos.Marca ?? vehiculo.MarcaDecodificada;
        vehiculo.ModeloDecodificado = datos.Modelo ?? vehiculo.ModeloDecodificado;
        vehiculo.FabricanteDecodificado = datos.Fabricante ?? vehiculo.FabricanteDecodificado;
        vehiculo.PaisFabricacion = datos.PaisFabricacion ?? vehiculo.PaisFabricacion;
        vehiculo.TipoVehiculo = datos.TipoVehiculo ?? vehiculo.TipoVehiculo;
        vehiculo.TipoCarroceria = datos.TipoCarroceria ?? vehiculo.TipoCarroceria;
        vehiculo.EstadoDecodificacion = datos.Estado;
        vehiculo.MensajeDecodificacion = datos.Mensaje;
        vehiculo.FechaDecodificacion = datos.FechaConsulta;
    }
}
