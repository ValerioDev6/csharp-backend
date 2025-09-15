
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Sistema.WebApi.Entities.Almacen
{
    public class Articulo
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idarticulo { get; set; }

        [Required]
        [ForeignKey("Categoria")]
        public int idcategoria { get; set; }

        [Required]
        [StringLength(20)]
        public string codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string nombre { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public double precio_venta { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int stock { get; set; }

        [StringLength(250)]
        public string descripcion { get; set; } = string.Empty;

        public bool condicion { get; set; } = true;

        // 🔗 Navegación (relación con Categoria)
        public virtual Categoria Categoria { get; set; } = null!;
    }
}
