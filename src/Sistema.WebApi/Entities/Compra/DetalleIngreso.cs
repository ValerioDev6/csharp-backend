
using Sistema.WebApi.Entities.Almacen;

namespace Sistema.WebApi.Entities.Compra
{
    public class DetalleIngreso
    {
        public int iddetalle_ingreso { get; set; }
        public int idingreso { get; set; }
        public int idarticulo { get; set; }
        public int cantidad { get; set; }
        public decimal precio { get; set; }


        public virtual Ingreso? Ingreso { get; set; }
        public virtual Articulo? Articulo { get; set; }
    }

}
