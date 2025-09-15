using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema.WebApi.Datos.DBContexAplication;
using Sistema.WebApi.dtos;
using Sistema.WebApi.Entities;
using Sistema.WebApi.ViewModels;

namespace Sistema.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolController : ControllerBase
    {
        private readonly DBContexAplication _context;

        public RolController(DBContexAplication context)
        {
            _context = context;

        }

        // GET: api/rol
        [HttpGet]
        public async Task<ActionResult> GetRoles()
        {
            try
            {
                var roles = await _context.Roles.ToListAsync();

                var roleDto = roles.Select(c => new RolDto
                {
                    idrol = c.idrol,
                    nombre = c.nombre,
                    descripcion = c.descripcion,
                    condicion = c.condicion
                }).ToList();

                return Ok(new
                {
                    success = true,
                    data = roleDto,

                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al obtener roles",
                    error = ex.Message
                });
            }
        }
        // GET: api/rol/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRol([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }

            return Ok(new RolDto
            {
                idrol = rol.idrol,
                nombre = rol.nombre,
                descripcion = rol.descripcion,
                condicion = rol.condicion
            });

        }


        [HttpPost]
        public async Task<ActionResult> PostRoles([FromBody] RolViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var rol = new Rol
                {
                    nombre = model.nombre,
                    descripcion = model.descripcion,
                    condicion = model.condicion,
                };

                _context.Roles.Add(rol);
                await _context.SaveChangesAsync();


                return Ok(new
                {
                    success = true,
                    message = "Rol creada exitosamente",
                    data = rol
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al crear roles",
                    error = ex.Message
                });
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchRol([FromRoute] int id, [FromBody] RolViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {

                var rol = await _context.Roles.FindAsync(id);
                if (rol == null)
                {
                    return NotFound();
                }

                rol.nombre = model.nombre;
                rol.descripcion = model.descripcion;
                rol.condicion = model.condicion;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Rol actualizada exitosamente",
                    data = rol
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al actualizar roles",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }
            _context.Roles.Remove(rol);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            return Ok(rol);
        }
    }
}
