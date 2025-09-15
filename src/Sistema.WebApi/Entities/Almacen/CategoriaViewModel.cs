using System.ComponentModel.DataAnnotations;

namespace Sistema.WebApi.Entities.Almacen
{
    public class CategoriaViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string nombre { get; set; }

        [StringLength(250)]
        public required string descripcion { get; set; }

        public bool condicion { get; set; } = true;
    }
}
