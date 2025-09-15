using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sistema.WebApi.Entities;

namespace Sistema.WebApi.Mapping
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("usuario");

            builder.HasKey(u => u.idusuario);

            // Relación con Rol (FK)
            builder.HasOne(u => u.Rol)
                   .WithMany()
                   .HasForeignKey(u => u.idrol)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configuración de propiedades
            builder.Property(u => u.nombre)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.tipo_documento)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(u => u.num_documento)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(u => u.direccion)
                   .HasMaxLength(250)
                   .IsRequired(false);

            builder.Property(u => u.telefono)
                   .HasMaxLength(15)
                   .IsRequired(false);

            builder.Property(u => u.email)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(u => u.password_hash)
                   .IsRequired(false);

            builder.Property(u => u.password_salt)
                   .IsRequired(false);

            // Condición con valor por defecto
            builder.Property(u => u.condicion)
                   .HasDefaultValue(true);
        }
    }
}
