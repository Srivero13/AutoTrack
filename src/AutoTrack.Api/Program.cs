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
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

var conexion = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AutoTrackContext>(opciones =>
    opciones.UseMySql(conexion, ServerVersion.Parse("8.0.36-mysql")));

builder.Services.AddScoped<IVehiculoRepository, VehiculoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICargaCombustibleRepository, CargaCombustibleRepository>();
builder.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
builder.Services.AddScoped<IVehiculoService, VehiculoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICargaCombustibleService, CargaCombustibleService>();

builder.Services.AddHttpClient<IVinDecoder, VinDecoder>(cliente =>
{
    var direccion = builder.Configuration["VinDecoder:BaseUrl"] ?? "https://vpic.nhtsa.dot.gov/";
    cliente.BaseAddress = new Uri(direccion);
    cliente.Timeout = TimeSpan.FromSeconds(8);
});

builder.Services.AddAutoMapper(typeof(PerfilMapeo).Assembly);
builder.Services.AddScoped<IValidator<CrearVehiculoDto>, CrearVehiculoValidator>();
builder.Services.AddScoped<IValidator<FiltroVehiculoDto>, FiltroVehiculoValidator>();
builder.Services.AddScoped<IValidator<CrearUsuarioDto>, CrearUsuarioValidator>();
builder.Services.AddScoped<IValidator<IniciarSesionDto>, IniciarSesionValidator>();
builder.Services.AddScoped<IValidator<ActualizarUsuarioDto>, ActualizarUsuarioValidator>();
builder.Services.AddScoped<IValidator<ActualizarVehiculoDto>, ActualizarVehiculoValidator>();
builder.Services.AddScoped<IValidator<CrearCargaCombustibleDto>, CrearCargaCombustibleValidator>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.Cookie.Name = "AutoTrack.Session";
        opciones.Cookie.HttpOnly = true;
        opciones.Cookie.SameSite = SameSiteMode.Strict;
        opciones.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
        opciones.ExpireTimeSpan = TimeSpan.FromHours(4);
        opciones.SlidingExpiration = false;
        opciones.Events.OnRedirectToLogin = contexto =>
        {
            contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        opciones.Events.OnRedirectToAccessDenied = contexto =>
        {
            contexto.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(opciones =>
{
    opciones.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    opciones.AddPolicy("login", contexto =>
        RateLimitPartition.GetFixedWindowLimiter(
            contexto.Connection.RemoteIpAddress?.ToString() ?? "local",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

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

app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
