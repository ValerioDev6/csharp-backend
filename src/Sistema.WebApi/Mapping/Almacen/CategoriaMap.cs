using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sistema.WebApi.Entities.Almacen;

namespace Sistema.WebApi.Mapping.Almacen
{
    public class CategoriaMap : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("categoria")
                .HasKey(c => c.idcategoria);
            builder.Property(c => c.idcategoria)
                .ValueGeneratedOnAdd();
            builder.Property(c => c.nombre)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(c => c.descripcion)
                .HasMaxLength(250);
        }
    }
}