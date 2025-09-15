using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema.WebApi.Datos.DBContexAplication;
using Sistema.WebApi.dtos;
using Sistema.WebApi.Entities;
using Sistema.WebApi.Utils;
using Sistema.WebApi.ViewModels;

namespace Sistema.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly DBContexAplication _context;
        public UsuarioController(DBContexAplication context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios
                      .Include(a => a.Rol)
                      .Select(a => new UsuarioDto
                      {
                          idusuario = a.idusuario,
                          nombre = a.nombre,
                          tipo_documento = a.tipo_documento,
                          num_documento = a.num_documento,
                          direccion = a.direccion,
                          telefono = a.telefono,
                          email = a.email,
                          condicion = a.condicion,
                          idrol = a.Rol.idrol,
                          rolNombre = a.Rol.nombre
                      }).ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = usuarios,
                    message = "Usuarios obtenidos correctamente"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al obtener los usuarios",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var usuario = await GetUsuarioDtoAsync(id);
            if (usuario == null)
            {
                return BadRequest();
            }

            return Ok(new
            {
                succes = true,
                data = usuario
            });

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario([FromRoute] int id, [FromBody] UsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Usuarios no encontrado"
                });
            }

            var userNameExists = await _context.Usuarios
                .AnyAsync(c => c.nombre.ToLower() == model.nombre.ToLower());

            if (!userNameExists)
            {
                return Conflict(new
                {
                    ok = false,
                    status = 409,
                    message = "Ya exsiste un usuarios con ese nombre",
                    messge_code = "DUPLICATE_NAME"
                });
            }

            usuario.idrol = model.idrol;
            usuario.idusuario = model.idusuario;
            usuario.nombre = model.nombre;
            usuario.tipo_documento = model.tipo_documento;
            usuario.num_documento = model.num_documento;
            usuario.direccion = model.direccion;
            usuario.telefono = model.telefono;
            usuario.email = model.email;
            usuario.condicion = model.condicion;

            await _context.SaveChangesAsync();

            var usuarioActualizado = await GetUsuarioDtoAsync(usuario.idusuario);

            return Ok(new
            {
                ok = true,
                message = "Usuario actualizado exitosamente",
                data = usuarioActualizado
            });

        }

        [HttpPost]
        public async Task<IActionResult> PostUsuario([FromBody] UsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userNameExists = await _context.Usuarios
                .AnyAsync(c => c.nombre.ToLower() == model.nombre.ToLower());

            if (userNameExists)
            {
                return Conflict(new
                {
                    ok = false,
                    status = 409,
                    message = "Ya exsiste un usuarios con ese nombre",
                    messge_code = "UsuarioDuplicado"
                });
            }
            PasswordHasher.CreatePasswordHash(model.password, out byte[] passwordHash, out byte[] passwordSalt);
            var usuario = new Usuario
            {
                idrol = model.idrol,
                idusuario = model.idusuario,
                nombre = model.nombre,
                tipo_documento = model.tipo_documento,
                num_documento = model.num_documento,
                direccion = model.direccion,
                telefono = model.telefono,
                email = model.email,
                condicion = model.condicion,
                password_hash = passwordHash,
                password_salt = passwordSalt
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var usuarioCreado = await GetUsuarioDtoAsync(usuario.idusuario);

            return Ok(new
            {
                ok = true,
                message = "Usuario credo exitosamente",
                message_code = "UsuarioCreado",
                data = usuarioCreado
            });

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            _context.Usuarios.Remove(usuario);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            return Ok(usuario);
        }

        private async Task<UsuarioDto?> GetUsuarioDtoAsync(int id)
        {
            return await _context.Usuarios
                .Include(a => a.Rol)
                .Where(a => a.idusuario == id)
                .Select(a => new UsuarioDto
                {
                    idusuario = a.idusuario,
                    nombre = a.nombre,
                    tipo_documento = a.tipo_documento,
                    num_documento = a.num_documento,
                    direccion = a.direccion,
                    telefono = a.telefono,
                    email = a.email,
                    condicion = a.condicion,
                    rolNombre = a.Rol.nombre,

                }).FirstOrDefaultAsync();
        }
    }

}
