using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sistema.WebApi.Entities.Almacen;

namespace Sistema.WebApi.Mapping.Almacen
{

       public class ArticuloMap : IEntityTypeConfiguration<Articulo>
       {

              public void Configure(EntityTypeBuilder<Articulo> builder)
              {
                     builder.ToTable("articulo");

                     // 🔑 Clave primaria
                     builder.HasKey(a => a.idarticulo);

                     // 🔗 Relación con Categoria (FK)
                     builder.HasOne(a => a.Categoria)
                            .WithMany()
                            .HasForeignKey(a => a.idcategoria)
                            .OnDelete(DeleteBehavior.Restrict);

                     // Campos obligatorios
                     builder.Property(a => a.codigo)
                            .HasMaxLength(50)
                            .IsRequired();

                     builder.Property(a => a.nombre)
                            .HasMaxLength(100)
                            .IsRequired();

                     builder.Property(a => a.precio_venta)
                            .HasColumnType("decimal(10,2)")
                            .IsRequired();

                     builder.Property(a => a.stock)
                            .IsRequired();

                     // Descripción opcional
                     builder.Property(a => a.descripcion)
                            .HasMaxLength(500)
                            .IsRequired(false);

                     // Condición con valor por defecto
                     builder.Property(a => a.condicion)
                            .HasDefaultValue(true);



              }
       }
}
