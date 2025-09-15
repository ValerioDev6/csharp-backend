


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.WebApi.Entities.Venta
{
    public class Venta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idventa { get; set; }
        [Required]


        public int idcliente { get; set; }

        [Required]
        public int idusuario { get; set; }

        [Required]
        [StringLength(50)]
        public string tipo_comprobante { get; set; } = string.Empty;
        [Required]
        [StringLength(7)]
        public string serie_comprobante { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string num_comprobante { get; set; } = string.Empty;

        [Required]
        public DateTime fecha_hora { get; set; }

        [Required]
        [Column(TypeName = "decimal(4, 2)")]
        public decimal impuesto { get; set; }

        [Required]
        [Column(TypeName = "decimal(11, 2)")]
        public decimal total { get; set; }

        [Required]
        [StringLength(20)]
        public string estado { get; set; } = string.Empty;

        // Relaciones
        public virtual Persona? Cliente { get; set; }
        public virtual Usuario? Usuario { get; set; }
        public virtual ICollection<DetalleVenta>? Detalles { get; set; }
    }
}
