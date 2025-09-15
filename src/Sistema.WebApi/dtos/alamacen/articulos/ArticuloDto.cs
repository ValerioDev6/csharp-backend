

namespace Sistema.WebApi.dtos.alamacen.articulos
{
    public class ArticuloDto
    {
        public int idarticulo { get; set; }
        public string codigo { get; set; } = string.Empty;
        public string nombre { get; set; } = string.Empty;
        public double precio_venta { get; set; }
        public int stock { get; set; }
        public string descripcion { get; set; } = string.Empty;
        public bool condicion { get; set; }
        public int idcategoria { get; set; }

        public string categoriaNombre { get; set; } = string.Empty;
    }
}
