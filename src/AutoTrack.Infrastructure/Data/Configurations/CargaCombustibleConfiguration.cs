using AutoTrack.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTrack.Infrastructure.Data.Configurations;

public class CargaCombustibleConfiguration : IEntityTypeConfiguration<CargaCombustible>
{
    public void Configure(EntityTypeBuilder<CargaCombustible> entity)
    {
        entity.ToTable("CargaCombustible");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Fecha).HasColumnType("datetime");
        entity.Property(x => x.Litros).HasColumnType("decimal(10,2)");
        entity.Property(x => x.PrecioPorLitro).HasColumnType("decimal(10,2)");
        entity.Property(x => x.Total).HasColumnType("decimal(12,2)");
        entity.HasOne<Vehiculo>().WithMany().HasForeignKey(x => x.VehiculoId).OnDelete(DeleteBehavior.Restrict);
    }
}
