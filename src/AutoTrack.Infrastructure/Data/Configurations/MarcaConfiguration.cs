using AutoTrack.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTrack.Infrastructure.Data.Configurations;

public class MarcaConfiguration : IEntityTypeConfiguration<Marca>
{
    public void Configure(EntityTypeBuilder<Marca> entity)
    {
        entity.ToTable("Marca");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Nombre).HasMaxLength(80).IsRequired();
        entity.HasIndex(x => x.Nombre).IsUnique();
    }
}
