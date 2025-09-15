

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sistema.WebApi.Entities.Compra;

namespace Sistema.WebApi.Mapping
{
    public class DetalleIngresoMap : IEntityTypeConfiguration<DetalleIngreso>
    {
        public void Configure(EntityTypeBuilder<DetalleIngreso> builder)
        {

            builder.ToTable("detalle_ingreso");
            builder.HasKey(d => d.iddetalle_ingreso);
            builder.Property(d => d.cantidad).IsRequired();
            builder.Property(d => d.precio).HasColumnType("decimal(11,2)").IsRequired();

            // Relaciones
            builder.HasOne(d => d.Ingreso)
                   .WithMany(i => i.Detalles)
                   .HasForeignKey(d => d.idingreso);
            builder.HasOne(d => d.Articulo)
                   .WithMany()
                   .HasForeignKey(d => d.idarticulo);
        }
    }
}
