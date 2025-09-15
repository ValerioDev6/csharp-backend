
namespace Sistema.WebApi.dtos
{

    public class VentaDto
    {
        public int idventa { get; set; }
        public int idcliente { get; set; }
        public int idusuario { get; set; }
        public string tipo_comprobante { get; set; }
        public string serie_comprobante { get; set; }
        public string num_comprobante { get; set; }
        public DateTime fecha_hora { get; set; }
        public decimal impuesto { get; set; }
        public decimal total { get; set; }
        public string estado { get; set; }
        public ClienteDto Cliente { get; set; }
        public UsuarioVentaDto Usuario { get; set; }
    }

}
