


namespace Sistema.WebApi.ViewModels
{

    public class IngresoViewModel
    {

        public int idingreso { get; set; }


        public int idproveedor { get; set; }

        public int idusuario { get; set; }

        public string tipo_comprobante { get; set; } = string.Empty;


        public string serie_comprobante { get; set; } = string.Empty;

        public string num_comprobante { get; set; } = string.Empty;

        public DateTime fecha_hora { get; set; }

        public string estado { get; set; } = string.Empty;

        public decimal impuesto { get; set; }

        public decimal total { get; set; }


        public List<DetalleIngresoViewModel>? Detalles { get; set; }

    }
}
