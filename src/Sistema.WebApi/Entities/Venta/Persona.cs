

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.WebApi.Entities.Venta
{
    public class Persona
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idpersona { get; set; }
        [Required]
        [StringLength(20)]
        public string tipo_persona { get; set; } = string.Empty;


        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string tipo_documento { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string num_documento { get; set; } = string.Empty;

        [StringLength(250)]
        public string direccion { get; set; } = string.Empty;

        [StringLength(15)]
        public string telefono { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string email { get; set; } = string.Empty;


    }
}
