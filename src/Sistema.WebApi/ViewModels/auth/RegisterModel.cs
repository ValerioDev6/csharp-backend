
using System.ComponentModel.DataAnnotations;

namespace Sistema.WebApi.ViewModels.auth
{
    public class RegisterModel
    {
        [Required]
        public int Idrol { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [StringLength(20)]
        public string TipoDocumento { get; set; } = string.Empty;
        [Required]
        [StringLength(20)]
        public string NumDocumento { get; set; } = string.Empty;
        [StringLength(250)]
        public string Direccion { get; set; } = string.Empty;
        [StringLength(15)]
        public string Telefono { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
