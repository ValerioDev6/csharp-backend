using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.WebApi.Entities
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idusuario { get; set; }

        // 🔗 FK hacia Rol
        [Required]
        [ForeignKey("Rol")]
        public int idrol { get; set; }

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

        public byte[]? password_hash { get; set; }

        public byte[]? password_salt { get; set; }

        public bool condicion { get; set; } = true;

        // 🔗 Navegación
        public virtual Rol Rol { get; set; } = null!;
    }

}
