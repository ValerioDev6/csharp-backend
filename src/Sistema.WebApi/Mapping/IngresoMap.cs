

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sistema.WebApi.Entities.Compra;

namespace Sistema.WebApi.Mapping
{
  public class IngresoMap : IEntityTypeConfiguration<Ingreso>
  {
    public void Configure(EntityTypeBuilder<Ingreso> builder)
    {

      builder.ToTable("ingreso");

      builder.HasKey(i => i.idingreso);
      builder.Property(i => i.tipo_comprobante).IsRequired().HasMaxLength(20);
      builder.Property(i => i.serie_comprobante).IsRequired().HasMaxLength(7);
      builder.Property(i => i.num_comprobante).IsRequired().HasMaxLength(10);

      builder.Property(i => i.fecha_hora).IsRequired();
      builder.Property(i => i.impuesto).HasColumnType("decimal(11,2)").IsRequired();
      builder.Property(i => i.total).HasColumnType("decimal(11,2)").IsRequired();
      builder.Property(i => i.estado).IsRequired().HasMaxLength(20);

      // Relaciones 
      builder.HasOne(i => i.Proveedor)
        .WithMany()
        .HasForeignKey(i => i.idproveedor);

      builder.HasOne(i => i.Usuario)
        .WithMany()
        .HasForeignKey(i => i.idusuario);

    }
  }
}
