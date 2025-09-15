using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema.WebApi.Entities.Almacen;

namespace Sistema.WebApi.Entities.Venta
{
  public class DetalleVenta
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int iddetalle_venta { get; set; }

    [Required]
    public int idventa { get; set; }

    [Required]
    public int idarticulo { get; set; }

    [Required]
    public int cantidada { get; set; }

    [Required]
    [Column(TypeName = "decimal(11, 2)")]
    public decimal precio { get; set; }

    // Relaciones
    public virtual Venta? Venta { get; set; }
    public virtual Articulo? Articulo { get; set; }
  }
}