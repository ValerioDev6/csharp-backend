using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.WebApi.Entities.Almacen
{
    public class Categoria
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idcategoria { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string nombre { get; set; }

        [StringLength(250)]
        public string? descripcion { get; set; }

        public bool condicion { get; set; } = true;
    }
}
