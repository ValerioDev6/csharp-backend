namespace Sistema.WebApi.ViewModels
{
    public class UsuarioViewModel
    {
        public int idusuario { get; set; }

        public int idrol { get; set; }

        public string rolNombre { get; set; } = string.Empty;

        public string nombre { get; set; } = string.Empty;

        public string tipo_documento { get; set; } = string.Empty;

        public string num_documento { get; set; } = string.Empty;

        public string direccion { get; set; } = string.Empty;

        public string telefono { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public string password { get; set; } = string.Empty;
        public bool condicion { get; set; } = true;
    }
}
