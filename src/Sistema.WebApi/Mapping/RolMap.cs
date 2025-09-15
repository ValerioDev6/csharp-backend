using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


using Sistema.WebApi.Entities;
namespace Sistema.WebApi.Mapping

{
    public class RolMap : IEntityTypeConfiguration<Rol>
    {

        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            builder.ToTable("rol");
            builder.HasKey(a => a.idrol);

            builder.Property(a => a.nombre)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(a => a.descripcion)
                .HasMaxLength(250)
                .IsRequired(false);

            builder.Property(a => a.condicion)
                .HasDefaultValue(true);


        }

    }
}
