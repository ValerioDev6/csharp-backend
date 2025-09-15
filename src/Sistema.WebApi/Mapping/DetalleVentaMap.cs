

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sistema.WebApi.Entities.Venta;

namespace Sistema.WebApi.Mapping
{
  public class DetalleVentaMap : IEntityTypeConfiguration<DetalleVenta>
  {
    public void Configure(EntityTypeBuilder<DetalleVenta> builder)

    {
      builder.ToTable("detalle_venta");
      builder.HasKey(dv => dv.iddetalle_venta);
      builder.Property(dv => dv.cantidada).IsRequired();
      builder.Property(dv => dv.precio).HasColumnType("decimal(11,2)").IsRequired();

      // Relaciones
      builder.HasOne(dv => dv.Venta)
          .WithMany(v => v.Detalles)
          .HasForeignKey(dv => dv.idventa)
          .OnDelete(DeleteBehavior.Cascade); // Si se elimina una venta, se eliminan sus detalles

      builder.HasOne(dv => dv.Articulo)
          .WithMany()
          .HasForeignKey(dv => dv.idarticulo)
          .OnDelete(DeleteBehavior.Restrict);
    }
  }
}