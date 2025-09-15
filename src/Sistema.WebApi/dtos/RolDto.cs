

namespace Sistema.WebApi.dtos
{
    public class RolDto
    {
        public int idrol { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public bool condicion { get; set; }
    }
}
