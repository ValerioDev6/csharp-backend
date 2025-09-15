using System.ComponentModel.DataAnnotations;

namespace Sistema.WebApi.ViewModels
{
  public class DetalleVentaViewModel
  {
    [Required]
    public int idarticulo { get; set; }

    [Required]
    public int cantidada { get; set; }

    [Required]
    public decimal precio { get; set; }
  }
}