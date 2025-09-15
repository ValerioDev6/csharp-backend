using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Sistema.WebApi.Entities
{
    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idrol { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "El nombre del rol debe tener un minimo de 3 caracteres")]
        public string nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string descripcion { get; set; } = string.Empty;

        public bool condicion { get; set; } = true;
    }
}
