


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sistema.WebApi.Entities.Venta;

namespace Sistema.WebApi.Mapping
{

       public class PersonaMap : IEntityTypeConfiguration<Persona>
       {
              public void Configure(EntityTypeBuilder<Persona> builder)
              {
                     builder.ToTable("persona");
                     builder.HasKey(u => u.idpersona);
                     // Configuración de propiedades
                     builder.Property(u => u.nombre)
                            .HasMaxLength(100)
                            .IsRequired();
                     builder.Property(u => u.tipo_persona)
                                .HasMaxLength(20)
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
              }
       }
}