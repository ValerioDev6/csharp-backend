using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema.WebApi.common;
using Sistema.WebApi.Datos.DBContexAplication;
using Sistema.WebApi.Entities.Almacen;

namespace Sistema.WebApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly DBContexAplication _context;

        public CategoriasController(DBContexAplication context)
        {
            _context = context;
        }

        // GET : api/categorias
        [HttpGet]
        public async Task<ActionResult> GetCategorias()
        {
            try
            {
                var categorias = await _context.Categorias.ToListAsync();
                var categoriasDto = categorias.Select(c => new Categoria
                {
                    idcategoria = c.idcategoria,
                    nombre = c.nombre,
                    descripcion = c.descripcion,
                    condicion = c.condicion
                }).ToList();

                return Ok(new
                {
                    success = true,
                    data = categoriasDto,
                    message = "Categorías obtenidas exitosamente"
                });
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


        // GET : api/categorias/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoria([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            // ✅ Devuelve un DTO, no la entidad
            return Ok(new Categoria
            {
                idcategoria = categoria.idcategoria,
                nombre = categoria.nombre,
                descripcion = categoria.descripcion,
                condicion = categoria.condicion
            });
        }


        // PUT : api/categorias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria([FromRoute] int id, [FromBody] CategoriaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            categoria.nombre = model.nombre;
            categoria.descripcion = model.descripcion;
            categoria.condicion = model.condicion;

            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Categoría actualizada exitosamente",
                data = categoria
            });
        }

        // POST : api/categorias/seed
        [HttpPost("seed")]
        public async Task<IActionResult> Seed()
        {
            try
            {
                var faker = new Faker<Categoria>("es")
                    .RuleFor(c => c.nombre, f => f.Commerce.Categories(1)[0])
                    // .RuleFor(c => c.nombre, f => $"{f.Commerce.Department()}_{f.UniqueIndex}")
                    .RuleFor(c => c.descripcion, f => f.Lorem.Sentence(8))
                    .RuleFor(c => c.condicion, f => f.Random.Bool());

                var categorias = faker.Generate(20);

                await _context.Categorias.AddRangeAsync(categorias);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Se generaron 20 categorías únicas con Bogus",
                    total = categorias.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error al insertar categorías",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
        // POST : api/categorias/seed-array

        [HttpPost("seed-array")]
        public async Task<IActionResult> SeedArray()
        {
            try
            {
                // Arreglo con nombres y descripciones
                string[] nombres = {
                    "Electrónica", "Ropa", "Hogar", "Deportes", "Juguetes",
                    "Libros", "Alimentos", "Oficina", "Salud", "Belleza"
                 };

                string[] descripciones = {
                    "Artículos de tecnología y gadgets",
                    "Prendas de vestir para todas las edades",
                    "Muebles y accesorios para el hogar",
                    "Equipos y accesorios deportivos",
                    "Juguetes educativos y de entretenimiento",
                    "Libros de distintos géneros",
                    "Productos alimenticios variados",
                    "Útiles y artículos de oficina",
                    "Productos para el cuidado de la salud",
                    "Cosméticos y productos de belleza"
                };

                // Lista de categorías
                var categorias = new List<Categoria>();

                for (int i = 0; i < 10; i++)
                {
                    categorias.Add(new Categoria
                    {
                        nombre = nombres[i],
                        descripcion = descripciones[i],
                        condicion = i % 2 == 0 // alterna entre true/false
                    });
                }

                // Insertar en DB
                await _context.Categorias.AddRangeAsync(categorias);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Se insertaron 10 categorías desde un arreglo",
                    total = categorias.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error al insertar categorías",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }




        // POST : api/categorias
        [HttpPost]
        public async Task<IActionResult> PostCategoria([FromBody] CategoriaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoria = new Categoria
            {
                nombre = model.nombre,
                descripcion = model.descripcion,
                condicion = model.condicion
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Categoría creada exitosamente",
                data = categoria
            });
        }



        // Delete : api/categorias
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            _context.Categorias.Remove(categoria);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            return Ok(categoria);
        }

        // Action : api/categorias/toogle/id
        [HttpPut("toggle/{id}")]
        public async Task<IActionResult> Toggle([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "ID inválido"
                });
            }

            var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.idcategoria == id);
            if (categoria == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Categoría no encontrada"
                });
            }

            bool estadoAnterior = categoria.condicion;
            categoria.condicion = !categoria.condicion;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al actualizar la categoría"
                });
            }

            return Ok(new
            {
                success = true,
                message = categoria.condicion ? "Categoría activada" : "Categoría desactivada",
            });
        }
        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.idcategoria == id);
        }
    }
}
