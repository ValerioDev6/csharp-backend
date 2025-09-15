using System.ComponentModel.DataAnnotations;

namespace Sistema.WebApi.ViewModels
{
    public class RolViewModel
    {

        [Required]
        public string nombre { get; set; } = string.Empty;
        [StringLength(250, MinimumLength = 3)]
        public string descripcion { get; set; } = string.Empty;
        public bool condicion { get; set; }

    }
}
