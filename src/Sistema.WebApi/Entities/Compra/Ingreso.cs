using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sistema.WebApi.Entities.Venta;

namespace Sistema.WebApi.Entities.Compra
{
    public class Ingreso
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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



        // Relaciones
        public virtual Persona? Proveedor { get; set; }
        public virtual Usuario? Usuario { get; set; }
        public virtual ICollection<DetalleIngreso>? Detalles { get; set; }

    }
}
