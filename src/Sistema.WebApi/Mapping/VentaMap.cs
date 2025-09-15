

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sistema.WebApi.Entities.Venta;

namespace Sistema.WebApi.Mapping
{
  public class VentaMap : IEntityTypeConfiguration<Venta>
  {
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
      builder.ToTable("venta");
      builder.HasKey(v => v.idventa);
      builder.Property(v => v.tipo_comprobante).IsRequired().HasMaxLength(20);
      builder.Property(v => v.serie_comprobante).IsRequired().HasMaxLength(7);
      builder.Property(v => v.num_comprobante).IsRequired().HasMaxLength(10);
      builder.Property(v => v.fecha_hora).IsRequired();
      builder.Property(v => v.impuesto).HasColumnType("decimal(11,2)").IsRequired();
      builder.Property(v => v.total).HasColumnType("decimal(11,2)").IsRequired();
      builder.Property(v => v.estado).IsRequired().HasMaxLength(20);

      // Relaciones
      builder.HasOne(v => v.Cliente)
          .WithMany()
          .HasForeignKey(v => v.idcliente)
          .OnDelete(DeleteBehavior.Restrict);



      builder.HasOne(v => v.Usuario)
          .WithMany()
          .HasForeignKey(v => v.idusuario)
          .OnDelete(DeleteBehavior.Restrict);

    }
  }
}