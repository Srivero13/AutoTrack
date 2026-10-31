using AutoTrack.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoTrack.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> entity)
    {
        entity.ToTable("Usuario");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Nombre).HasMaxLength(80).IsRequired();
        entity.Property(x => x.Apellido).HasMaxLength(80).IsRequired();
        entity.Property(x => x.Correo).HasMaxLength(150).IsRequired();
        entity.HasIndex(x => x.Correo).IsUnique();
        entity.Property(x => x.Telefono).HasMaxLength(20);
        entity.Property(x => x.Activo).HasDefaultValue(true);
        entity.Property(x => x.FechaRegistro).HasColumnType("datetime")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
