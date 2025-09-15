using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema.WebApi.Datos.DBContexAplication;
using Sistema.WebApi.dtos.alamacen.articulos;
using Sistema.WebApi.Entities.Almacen;

namespace Sistema.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticuloController : ControllerBase
    {
        private readonly DBContexAplication _context;

        public ArticuloController(DBContexAplication context)
        {
            _context = context;
        }
        // GEt : api/articulo
        [HttpGet]
        public async Task<IActionResult> GetArticulos()
        {
            try
            {
                var articulos = await _context.Articulos
                    .Include(a => a.Categoria)
                    .Select(a => new ArticuloDto
                    {
                        idarticulo = a.idarticulo,
                        codigo = a.codigo,
                        nombre = a.nombre,
                        descripcion = a.descripcion,
                        precio_venta = a.precio_venta,
                        stock = a.stock,
                        condicion = a.condicion,
                        idcategoria = a.Categoria.idcategoria,
                        categoriaNombre = a.Categoria.nombre
                    }).ToListAsync();

                return Ok(
                    new
                    {
                        success = true,
                        data = articulos,
                        message = "Articulos obtenidos correctamente"
                    }
                );

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al obtener las articulos",
                    error = ex.Message
                });
            }
        }

        // Get : api/articulo/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticulo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var articulo = await GetArticuloDtoAsync(id);

            if (articulo == null)
            {
                return StatusCode(409, new
                {
                    status = 409,
                    success = false,
                    message = "Artículo no encontrado"
                });
            }


            return Ok(new
            {
                success = true,
                data = articulo
            });
        }
        // POST: api/articulo
        [HttpPost]
        public async Task<IActionResult> PostArticulo([FromBody] ArticuloViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    ok = false,
                    status = 400,
                    message = "El modelo no es válido",
                    message_code = "BadRequest"
                });
            }

            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.idcategoria == model.idcategoria);
            if (!categoriaExiste)
            {
                return BadRequest(new
                {
                    ok = false,
                    status = 400,
                    message = "La categoría especificada no existe",
                    message_code = "CategoriaNotFound"
                });
            }

            var articuloNombreExiste = await _context.Articulos
                .AnyAsync(a => a.nombre.ToLower() == model.nombre.ToLower());
            if (articuloNombreExiste)
            {
                return Conflict(new
                {
                    ok = false,
                    status = 409,
                    message = "Ya existe un artículo con ese nombre",
                    message_code = "ArticuloDuplicado"
                });
            }

            // 🔄 ViewModel → Entidad
            var articulo = new Articulo
            {
                idcategoria = model.idcategoria,
                codigo = model.codigo,
                nombre = model.nombre,
                precio_venta = model.precio_venta,
                stock = model.stock,
                descripcion = model.descripcion,
                condicion = model.condicion
            };

            _context.Articulos.Add(articulo);
            await _context.SaveChangesAsync();

            var articuloCreado = await GetArticuloDtoAsync(articulo.idarticulo);

            return Ok(new
            {
                ok = true,
                message = "Artículo creado exitosamente",
                message_code = "ArticuloCreado",
                data = articuloCreado
            });
        }

        // 📝 PUT: api/articulos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArticulo([FromRoute] int id, [FromBody] ArticuloViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var articulo = await _context.Articulos.FindAsync(id);
            if (articulo == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Artículo no encontrado"
                });
            }
            var existeNombre = await _context.Articulos
                .AnyAsync(a => a.nombre.ToLower() == model.nombre.ToLower());

            if (existeNombre)
            {
                return Conflict(new
                {
                    ok = false,
                    status = 409,
                    message = "El nombre del artículo ya existe",
                    error = "DUPLICATE_NAME"
                });
            }


            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.idcategoria == model.idcategoria);

            if (!categoriaExiste)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "La categoría especificada no existe"
                });
            }

            // 🔄 ViewModel → Entidad (actualizar)
            articulo.idcategoria = model.idcategoria;
            articulo.codigo = model.codigo;
            articulo.nombre = model.nombre;
            articulo.precio_venta = model.precio_venta;
            articulo.stock = model.stock;
            articulo.descripcion = model.descripcion;
            articulo.condicion = model.condicion;

            await _context.SaveChangesAsync();

            var articuloActualizado = await GetArticuloDtoAsync(articulo.idarticulo);


            return Ok(new
            {
                ok = true,
                message = "Artículo actualizado exitosamente",
                data = articuloActualizado
            });
        }


        private async Task<ArticuloDto?> GetArticuloDtoAsync(int id)
        {
            return await _context.Articulos
                .Include(a => a.Categoria)
                .Where(a => a.idarticulo == id)
                .Select(a => new ArticuloDto
                {
                    idarticulo = a.idarticulo,
                    codigo = a.codigo,
                    nombre = a.nombre,
                    precio_venta = a.precio_venta,
                    stock = a.stock,
                    descripcion = a.descripcion,
                    condicion = a.condicion,
                    categoriaNombre = a.Categoria.nombre
                })
                .FirstOrDefaultAsync();
        }

        [HttpPost("seed")]
        public async Task<IActionResult> Seed()
        {
            try
            {
                var categorias = await _context.Categorias.ToListAsync();
                if (!categorias.Any())
                {
                    return BadRequest(new { error = "No existen categorías, primero genera las categorías." });
                }

                var faker = new Faker<Articulo>("es")
                    .RuleFor(a => a.idcategoria, f => f.PickRandom(categorias).idcategoria)
                    .RuleFor(a => a.codigo, f => f.Random.AlphaNumeric(8))
                    .RuleFor(a => a.nombre, f => f.Commerce.ProductName())
                    .RuleFor(a => a.precio_venta, f => Math.Round(f.Random.Double(5, 500), 2))
                    .RuleFor(a => a.stock, f => f.Random.Int(0, 200))
                    .RuleFor(a => a.descripcion, f => f.Commerce.ProductDescription()) // 👈 opcional, puede quedar null
                    .RuleFor(a => a.condicion, f => f.Random.Bool());

                var articulos = faker.Generate(5);

                await _context.Articulos.AddRangeAsync(articulos);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Se generaron 5 artículos", total = articulos.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error al insertar artículos",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // Delete : api/articulso
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteArticulo(int id)
        {
            var articulo = await _context.Articulos.FindAsync(id);
            if (articulo == null)
            {
                return NotFound();
            }
            _context.Articulos.Remove(articulo);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            return Ok(articulo);
        }

        // GET : api/articulo/config
        [HttpGet("config")]
        public async Task<ActionResult> GetConfig()
        {
            try
            {
                var categorias = await _context.Categorias.ToListAsync();
                var categoriasDto = categorias.Select(c => new Categoria
                {
                    idcategoria = c.idcategoria,
                    nombre = c.nombre,
                }).ToList();

                return Ok(categoriasDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al obtener las categorías",
                    error = ex.Message
                });
            }
        }

    }
}
