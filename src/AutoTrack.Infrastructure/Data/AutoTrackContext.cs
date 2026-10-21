using System.Reflection;
using AutoTrack.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoTrack.Infrastructure.Data;

public partial class AutoTrackContext : DbContext
{
    public AutoTrackContext(DbContextOptions<AutoTrackContext> options) : base(options)
    {
    }

    public DbSet<Marca> Marcas => Set<Marca>();
    public DbSet<Modelo> Modelos => Set<Modelo>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<CargaCombustible> CargasCombustible => Set<CargaCombustible>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
