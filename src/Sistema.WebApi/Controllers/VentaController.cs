
using System.Security.Claims;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema.WebApi.Datos.DBContexAplication;
using Sistema.WebApi.dtos;
using Sistema.WebApi.Entities.Venta;
using Sistema.WebApi.ViewModels;

namespace Sistema.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentaController : ControllerBase
    {
        private readonly DBContexAplication _context;

        public VentaController(DBContexAplication context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDto>>> GetVentas()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .OrderByDescending(v => v.fecha_hora)
                .ToListAsync();

            // Mapear a VentaDto para limitar la información del usuario y del cliente
            var ventasDto = ventas.Select(v => new VentaDto
            {
                idventa = v.idventa,
                idcliente = v.idcliente,
                idusuario = v.idusuario,
                tipo_comprobante = v.tipo_comprobante,
                serie_comprobante = v.serie_comprobante,
                num_comprobante = v.num_comprobante,
                fecha_hora = v.fecha_hora,
                impuesto = v.impuesto,
                total = v.total,
                estado = v.estado,
                Cliente = new ClienteDto
                {
                    idpersona = v.Cliente.idpersona,
                    nombre = v.Cliente.nombre
                },
                Usuario = new UsuarioVentaDto
                {
                    idusuario = v.Usuario.idusuario,
                    nombre = v.Usuario.nombre
                }
            }).ToList();

            return Ok(ventasDto);
        }
        // POST: api/Venta 
        [HttpPost]
        public async Task<ActionResult<Venta>> PostVenta(VentaViewModel ventaViewModel)
        {
            // Obtener el ID del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int idUsuarioAutenticado))
            {
                return Unauthorized("Usuario no autenticado.");
            }

            // Asignar la fecha y hora actual en la zona horaria de Perú
            TimeZoneInfo peruTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            DateTime fechaHoraPeru = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, peruTimeZone);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Calcular el subtotal y el IGV
                decimal subtotal = ventaViewModel.Detalles!.Sum(d => d.cantidada * d.precio);
                decimal igv = subtotal * 0.18m;

                var venta = new Venta
                {
                    idcliente = ventaViewModel.idcliente,
                    idusuario = idUsuarioAutenticado,
                    tipo_comprobante = ventaViewModel.tipo_comprobante,
                    serie_comprobante = ventaViewModel.serie_comprobante,
                    num_comprobante = ventaViewModel.num_comprobante,
                    fecha_hora = fechaHoraPeru,
                    impuesto = igv,
                    total = subtotal + igv,
                    estado = "Aprobado",
                    Detalles = ventaViewModel.Detalles!.Select(d => new DetalleVenta
                    {
                        idarticulo = d.idarticulo,
                        cantidada = d.cantidada,
                        precio = d.precio
                    }).ToList()
                };

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                // Actualizar el stock de los artículos (restar)
                foreach (var detalle in venta.Detalles)
                {
                    var articulo = await _context.Articulos.FindAsync(detalle.idarticulo);
                    if (articulo != null)
                    {
                        articulo.stock -= detalle.cantidada;
                        _context.Articulos.Update(articulo);
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction("GetVenta", new { id = venta.idventa }, venta);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    error = "Error al registrar la venta.",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpPost("seed")]
        public async Task<IActionResult> SeedVentas()
        {
            try
            {
                // Verificar que existan artículos, usuarios y clientes
                var articulos = await _context.Articulos.ToListAsync();
                if (!articulos.Any())
                    return BadRequest(new { error = "No existen artículos. Primero genera artículos." });

                var usuarios = await _context.Usuarios.ToListAsync();
                if (!usuarios.Any())
                    return BadRequest(new { error = "No existen usuarios. Primero genera usuarios." });

                var clientes = await _context.Personas
                    .Where(p => p.tipo_persona == "Cliente")
                    .ToListAsync();
                if (!clientes.Any())
                    return BadRequest(new { error = "No existen clientes. Primero genera clientes." });

                // Generar ventas con datos falsos
                var fakerVenta = new Faker<Venta>("es")
                    .RuleFor(v => v.idcliente, f => f.PickRandom(clientes).idpersona)
                    .RuleFor(v => v.idusuario, f => f.PickRandom(usuarios).idusuario)
                    .RuleFor(v => v.tipo_comprobante, f => f.PickRandom(new[] { "FACTURA", "BOLETA", "TICKET" }))
                    .RuleFor(v => v.serie_comprobante, f => f.Random.AlphaNumeric(4))
                    .RuleFor(v => v.num_comprobante, f => f.Random.AlphaNumeric(6))
                    .RuleFor(v => v.fecha_hora, f => f.Date.Recent(30))
                    .RuleFor(v => v.estado, f => f.PickRandom(new[] { "Aprobado", "Anulado", "Pendiente" }));

                // Generar detalles para cada venta
                var ventas = new Faker<Venta>()
                    .CustomInstantiator(f => fakerVenta.Generate())
                    .FinishWith((f, venta) =>
                    {
                        // Cada venta tendrá entre 1 y 5 artículos
                        var detalles = new Faker<DetalleVenta>("es")
                            .RuleFor(d => d.idarticulo, f => f.PickRandom(articulos).idarticulo)
                            .RuleFor(d => d.cantidada, f => f.Random.Int(1, 3))
                            .RuleFor(d => d.precio, f => Math.Round(f.Finance.Amount(1m, 50m), 2))
                            .Generate(f.Random.Int(1, 3));

                        // Calcular el subtotal
                        decimal subtotal = detalles.Sum(d => d.cantidada * d.precio);

                        // Calcular el IGV (18% del subtotal)
                        decimal igv = subtotal * 0.18m;

                        // Asignar el total (subtotal + IGV)
                        venta.total = subtotal + igv;

                        // Asignar el impuesto (IGV calculado)
                        venta.impuesto = igv;

                        // Asignar los detalles a la venta
                        venta.Detalles = detalles;
                    })
                    .Generate(5); // Generar 5 ventas

                await _context.Ventas.AddRangeAsync(ventas);
                await _context.SaveChangesAsync();

                // Actualizar el stock de los artículos (restar)
                foreach (var venta in ventas)
                {
                    foreach (var detalle in venta.Detalles)
                    {
                        var articulo = articulos.FirstOrDefault(a => a.idarticulo == detalle.idarticulo);
                        if (articulo != null)
                        {
                            articulo.stock -= detalle.cantidada;
                        }
                    }
                }
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Se generaron 5 ventas con éxito.",
                    total = ventas.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error al generar ventas.",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

    }
}
