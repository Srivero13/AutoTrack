using AutoMapper;
using AutoTrack.Api.Middleware;
using AutoTrack.Api.Response;
using AutoTrack.Core.DTOs;
using AutoTrack.Core.Interfaces;
using AutoTrack.Infrastructure.Data;
using AutoTrack.Infrastructure.Repositories;
using AutoTrack.Services.Implementations;
using AutoTrack.Services.Interfaces;
using AutoTrack.Services.Mappings;
using AutoTrack.Services.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var conexion = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AutoTrackContext>(opciones =>
    opciones.UseMySql(conexion, ServerVersion.Parse("8.0.36-mysql")));

builder.Services.AddScoped<IVehiculoRepository, VehiculoRepository>();
builder.Services.AddScoped<ICargaCombustibleRepository, CargaCombustibleRepository>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<IVehiculoService, VehiculoService>();
builder.Services.AddScoped<ICargaCombustibleService, CargaCombustibleService>();

builder.Services.AddAutoMapper(typeof(PerfilMapeo).Assembly);
builder.Services.AddScoped<IValidator<CrearVehiculoDto>, CrearVehiculoValidator>();
builder.Services.AddScoped<IValidator<ActualizarVehiculoDto>, ActualizarVehiculoValidator>();
builder.Services.AddScoped<IValidator<CrearCargaCombustibleDto>, CrearCargaCombustibleValidator>();

builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(opciones =>
{
    opciones.InvalidModelStateResponseFactory = contexto =>
    {
        var errores = contexto.ModelState.Values
            .SelectMany(x => x.Errors)
            .Select(x => x.ErrorMessage)
            .ToList();

        return new BadRequestObjectResult(ApiResponse<object>.Fallo("Error de validación", errores));
    };
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ManejadorErroresMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
