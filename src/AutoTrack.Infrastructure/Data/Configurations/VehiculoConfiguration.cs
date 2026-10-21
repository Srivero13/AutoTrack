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
        entity.Property(x => x.Placa).HasMaxLength(15).IsRequired();
        entity.HasIndex(x => x.Placa).IsUnique();
        entity.Property(x => x.Anio).HasColumnType("smallint");
        entity.Property(x => x.TipoCombustible).HasMaxLength(30).IsRequired();
        entity.Property(x => x.Activo).HasDefaultValue(true);
        entity.Property(x => x.FechaRegistro).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
        entity.HasOne<Modelo>().WithMany().HasForeignKey(x => x.ModeloId).OnDelete(DeleteBehavior.Restrict);
    }
}
