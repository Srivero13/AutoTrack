using AutoTrack.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTrack.Infrastructure.Data.Configurations;

public class ModeloConfiguration : IEntityTypeConfiguration<Modelo>
{
    public void Configure(EntityTypeBuilder<Modelo> entity)
    {
        entity.ToTable("Modelo");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        entity.HasIndex(x => new { x.MarcaId, x.Nombre }).IsUnique();
        entity.HasOne<Marca>().WithMany().HasForeignKey(x => x.MarcaId).OnDelete(DeleteBehavior.Restrict);
    }
}
