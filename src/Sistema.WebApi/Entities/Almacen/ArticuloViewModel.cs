using System.ComponentModel.DataAnnotations;

namespace Sistema.WebApi.Entities.Almacen
{
    public class ArticuloViewModel
    {
        [Required]
        public int idcategoria { get; set; }
        [Required]
        public required string codigo { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string nombre { get; set; }
        [Required]
        public double precio_venta { get; set; }
        [Required]
        public int stock { get; set; }
        [StringLength(250, MinimumLength = 3)]
        public required string descripcion { get; set; }
        public bool condicion { get; set; } = true;

    }

}
