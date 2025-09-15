

using System.Security.Claims;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema.WebApi.Datos.DBContexAplication;
using Sistema.WebApi.Entities.Compra;
using Sistema.WebApi.ViewModels;

namespace Sistema.WebApi.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class CompraController : ControllerBase
    {
        private readonly DBContexAplication _context;

        public CompraController(DBContexAplication context)
        {
            _context = context;
        }
        // GET: api/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingreso>>> GetCompras()
        {
            // Obtener solo las compras (ingresos) sin incluir los detalles
            var compras = await _context.Ingresos
                .Include(i => i.Usuario) // Opcional: Incluir información del usuario
                .Include(i => i.Proveedor) // Opcional: Incluir información del proveedor
                .OrderByDescending(i => i.fecha_hora)
                .ToListAsync();

            return Ok(compras);
        }
        // GET: api/Ingreso/5 (Ver detalles de un ingreso)
        [HttpGet("{id}")]
        public async Task<ActionResult<IngresoViewModel>> GetIngreso(int id)
        {
            var ingreso = await _context.Ingresos
                .Include(i => i.Proveedor)
                .Include(i => i.Usuario)
                .Include(i => i.Detalles)
                .ThenInclude(d => d.Articulo)
                .FirstOrDefaultAsync(i => i.idingreso == id);

            if (ingreso == null)
                return NotFound();

            var ingresoViewModel = new IngresoViewModel
            {
                idingreso = ingreso.idingreso,
                idproveedor = ingreso.idproveedor,
                idusuario = ingreso.idusuario,
                tipo_comprobante = ingreso.tipo_comprobante,
                serie_comprobante = ingreso.serie_comprobante,
                num_comprobante = ingreso.num_comprobante,
                fecha_hora = ingreso.fecha_hora,
                impuesto = ingreso.impuesto,
                total = ingreso.total,
                estado = ingreso.estado,
                Detalles = ingreso.Detalles.Select(d => new DetalleIngresoViewModel
                {
                    idarticulo = d.idarticulo,
                    cantidad = d.cantidad,
                    precio = d.precio
                }).ToList()
            };

            return Ok(ingresoViewModel);
        }


        // POST: api/Ingreso (Registrar un ingreso)
        [HttpPost]
        public async Task<ActionResult<Ingreso>> PostIngreso(IngresoViewModel ingresoViewModel)
        {

            // Obtener el ID del usuario autenticado desde el token
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
                var ingreso = new Ingreso
                {
                    idproveedor = ingresoViewModel.idproveedor,
                    idusuario = idUsuarioAutenticado,
                    tipo_comprobante = ingresoViewModel.tipo_comprobante,
                    serie_comprobante = ingresoViewModel.serie_comprobante,
                    num_comprobante = ingresoViewModel.num_comprobante,
                    fecha_hora = fechaHoraPeru,
                    impuesto = ingresoViewModel.impuesto,
                    total = ingresoViewModel.total,
                    estado = ingresoViewModel.estado,
                    Detalles = ingresoViewModel.Detalles.Select(d => new DetalleIngreso
                    {
                        idarticulo = d.idarticulo,
                        cantidad = d.cantidad,
                        precio = d.precio
                    }).ToList()
                };

                _context.Ingresos.Add(ingreso);
                await _context.SaveChangesAsync();

                // Actualizar stock de artículos
                foreach (var detalle in ingreso.Detalles)
                {
                    var articulo = await _context.Articulos.FindAsync(detalle.idarticulo);
                    articulo!.stock += detalle.cantidad;
                    _context.Articulos.Update(articulo);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction("GetIngreso", new { id = ingreso.idingreso }, ingreso);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error al registrar el ingreso.");
            }
        }


        [HttpPost("seed")]
        public async Task<IActionResult> SeedCompras()
        {
            try
            {
                // Verificar que existan artículos, usuarios y proveedores
                var articulos = await _context.Articulos.ToListAsync();
                if (!articulos.Any())
                    return BadRequest(new { error = "No existen artículos. Primero genera artículos con el seed de artículos." });

                var usuarios = await _context.Usuarios.ToListAsync();
                if (!usuarios.Any())
                    return BadRequest(new { error = "No existen usuarios. Primero genera usuarios." });

                var proveedores = await _context.Personas
                    .Where(p => p.tipo_persona == "Proveedor") // Ajusta según tu lógica
                    .ToListAsync();
                if (!proveedores.Any())
                    return BadRequest(new { error = "No existen proveedores. Primero genera proveedores." });

                // Generar 5 compras (ingresos) con datos falsos
                var fakerIngreso = new Faker<Ingreso>("es")
                    .RuleFor(i => i.idproveedor, f => f.PickRandom(proveedores).idpersona)
                    .RuleFor(i => i.idusuario, f => f.PickRandom(usuarios).idusuario)
                    .RuleFor(i => i.tipo_comprobante, f => f.PickRandom(new[] { "FACTURA", "BOLETA", "TICKET" }))
                    .RuleFor(i => i.serie_comprobante, f => f.Random.AlphaNumeric(4))
                    .RuleFor(i => i.num_comprobante, f => f.Random.AlphaNumeric(6))
                    .RuleFor(i => i.impuesto, f => 0.18m) // Usar el IGV fijo del 18% (Perú)

                    .RuleFor(i => i.fecha_hora, f => f.Date.Recent(30))
                    .RuleFor(i => i.estado, f => f.PickRandom(new[] { "Aprobado", "Anulado", "Pendiente" }));

                // Generar detalles para cada compra
                var compras = new Faker<Ingreso>()
                    .CustomInstantiator(f => fakerIngreso.Generate())
                    .FinishWith((f, ingreso) =>
                    {
                        // Cada compra tendrá entre 1 y 5 artículos
                        var detalles = new Faker<DetalleIngreso>("es")
                            .RuleFor(d => d.idarticulo, f => f.PickRandom(articulos).idarticulo)
                            .RuleFor(d => d.cantidad, f => f.Random.Int(1, 3))
                            .RuleFor(d => d.precio, f => Math.Round(f.Finance.Amount(1m, 50m), 2))
                            .Generate(f.Random.Int(1, 5));

                        // Calcular el subtotal
                        decimal subtotal = detalles.Sum(d => d.cantidad * d.precio);

                        // Calcular el IGV (18% del subtotal)
                        decimal igv = subtotal * 0.18m;

                        // Asignar el total (subtotal + IGV)
                        ingreso.total = subtotal + igv;

                        // Asignar el impuesto (IGV calculado)
                        ingreso.impuesto = igv;

                        // Asignar los detalles al ingreso
                        ingreso.Detalles = detalles;

                    })
                    .Generate(15);

                await _context.Ingresos.AddRangeAsync(compras);
                await _context.SaveChangesAsync();

                // Actualizar el stock de los artículos
                foreach (var compra in compras)
                {
                    foreach (var detalle in compra.Detalles!)
                    {
                        var articulo = articulos.FirstOrDefault(a => a.idarticulo == detalle.idarticulo);
                        if (articulo != null)
                            articulo.stock += detalle.cantidad;
                    }
                }
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Se generaron 15 compras (ingresos) con éxito.",
                    total = compras.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error al generar compras.",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
    }
}
