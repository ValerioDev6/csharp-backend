
namespace Sistema.WebApi.dtos
{
    public class PersonaDto
    {
        public int idpersona { get; set; }
        public string tipo_persona { get; set; } = string.Empty;
        public string nombre { get; set; } = string.Empty;
        public string tipo_documento { get; set; } = string.Empty;
        public string num_documento { get; set; } = string.Empty;
        public string direccion { get; set; } = string.Empty;
        public string telefono { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;


    }
}
