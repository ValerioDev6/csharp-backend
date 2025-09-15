

using System.ComponentModel.DataAnnotations;

namespace Sistema.WebApi.ViewModels.auth
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
