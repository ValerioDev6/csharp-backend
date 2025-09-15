
using System.ComponentModel.DataAnnotations;

namespace Sistema.WebApi.ViewModels
{
  public class VentaViewModel
  {
    [Required]
    public int idcliente { get; set; }

    [Required]
    public string tipo_comprobante { get; set; } = string.Empty;

    [Required]
    public string serie_comprobante { get; set; } = string.Empty;

    [Required]
    public string num_comprobante { get; set; } = string.Empty;

    public List<DetalleVentaViewModel>? Detalles { get; set; }
  }

}