using AutoTrack.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTrack.Infrastructure.Data.Configurations;

public class VehiculoConfiguration : IEntityTypeConfiguration<Vehiculo>
{
    public void Configure(EntityTypeBuilder<Vehiculo> entity)
    {
        entity.ToTable("Vehiculo");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Vin).HasMaxLength(17).IsRequired();
        entity.HasIndex(x => x.Vin).IsUnique();
        entity.Property(x => x.Placa).HasMaxLength(15);
        entity.Property(x => x.Anio).HasColumnType("smallint");
        entity.Property(x => x.TipoCombustible).HasMaxLength(50);
        entity.Property(x => x.MarcaDecodificada).HasMaxLength(100);
        entity.Property(x => x.ModeloDecodificado).HasMaxLength(100);
        entity.Property(x => x.FabricanteDecodificado).HasMaxLength(150);
        entity.Property(x => x.PaisFabricacion).HasMaxLength(100);
        entity.Property(x => x.TipoVehiculo).HasMaxLength(100);
        entity.Property(x => x.TipoCarroceria).HasMaxLength(100);
        entity.Property(x => x.EstadoDecodificacion).HasMaxLength(30).HasDefaultValue("Pendiente");
        entity.Property(x => x.MensajeDecodificacion).HasMaxLength(500);
        entity.Property(x => x.FechaDecodificacion).HasColumnType("datetime");
        entity.Property(x => x.Activo).HasDefaultValue(true);
        entity.Property(x => x.FechaRegistro).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.HasOne<Modelo>().WithMany().HasForeignKey(x => x.ModeloId).OnDelete(DeleteBehavior.Restrict);
    }
}
