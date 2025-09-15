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
    public class PersonaController : ControllerBase
    {

        private readonly DBContexAplication _context;

        public PersonaController(DBContexAplication context)
        {
            _context = context;
        }

        [HttpGet("listar-clientes")]
        public async Task<IActionResult> ListarCliente()
        {
            try
            {
                var clientes = await _context.Personas.Where(c => c.tipo_persona == "Cliente")
                  .Select(c => new PersonaDto
                  {

                      idpersona = c.idpersona,
                      nombre = c.nombre,
                      tipo_persona = c.tipo_persona,
                      tipo_documento = c.tipo_documento,
                      num_documento = c.num_documento,
                      direccion = c.direccion,
                      telefono = c.telefono,
                      email = c.email,

                  })
                  .ToListAsync();
                return Ok(new
                {
                    success = true,
                    data = clientes,
                    message = "Clientes obtenidos correctamente"
                });
            }
            catch (Exception ex)
            {

                return BadRequest(new
                {
                    success = false,
                    message = "Error al obtener los clientes",
                    error = ex.Message
                });
            }
        }
        [HttpGet("listar-proveedores")]
        public async Task<IActionResult> ListarProveedores()
        {
            try
            {
                var proveedor = await _context.Personas.Where(c => c.tipo_persona == "Proveedor")
                  .Select(c => new PersonaDto
                  {

                      idpersona = c.idpersona,
                      nombre = c.nombre,
                      tipo_persona = c.tipo_persona,
                      tipo_documento = c.tipo_documento,
                      num_documento = c.num_documento,
                      direccion = c.direccion,
                      telefono = c.telefono,
                      email = c.email,

                  })
                  .ToListAsync();
                return Ok(new
                {
                    success = true,
                    data = proveedor,
                    message = "proveedor obtenidos correctamente"
                });
            }
            catch (Exception ex)
            {

                return BadRequest(new
                {
                    success = false,
                    message = "Error al obtener los proveedores",
                    error = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPersona([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var persona = await GetPersonaDtoAsync(id);
            if (persona == null)
            {
                return BadRequest();
            }

            return Ok(new
            {
                succes = true,
                data = persona
            });

        }

        [HttpPost]
        public async Task<IActionResult> PostPersona([FromBody] PersonViewModal model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {

                var email = model.email.ToLower();
                if (await _context.Usuarios.AnyAsync(u => u.email == email))
                {
                    return BadRequest("El email ya existe");
                }

                var personaNameExists = await _context.Personas
                    .AnyAsync(c => c.nombre.ToLower() == model.nombre.ToLower());

                if (personaNameExists)
                {
                    return Conflict(new
                    {
                        ok = false,
                        status = 409,
                        message = "Ya exsiste un usuarios con ese nombre",
                        messge_code = "UsuarioDuplicado"
                    });
                }

                var persona = new Persona
                {
                    tipo_persona = model.tipo_persona,
                    nombre = model.nombre,
                    tipo_documento = model.tipo_documento,
                    num_documento = model.num_documento,
                    direccion = model.direccion,
                    telefono = model.telefono,
                    email = model.email,

                };
                _context.Personas.Add(persona);
                await _context.SaveChangesAsync();

                var personaCreado = await GetPersonaDtoAsync(persona.idpersona);

                return Ok(new
                {
                    ok = true,
                    message = "Persona credo exitosamente",
                    message_code = "PersonaCreado",
                    data = personaCreado
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al crear Persona",
                    error = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersona([FromRoute] int id, [FromBody] PersonViewModal model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var persona = await _context.Personas.FindAsync(id);
                if (persona == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Usuarios no encontrado"
                    });
                }


                var email = model.email.ToLower();
                if (await _context.Usuarios.AnyAsync(u => u.email == email))
                {
                    return BadRequest("El email ya existe");
                }


                persona.idpersona = model.idpersona;
                persona.nombre = model.nombre;
                persona.tipo_persona = model.tipo_persona;
                persona.tipo_documento = model.tipo_documento;
                persona.num_documento = model.num_documento;
                persona.direccion = model.direccion;
                persona.telefono = model.telefono;
                persona.email = model.email;

                await _context.SaveChangesAsync();

                var personaUpdated = await GetPersonaDtoAsync(persona.idpersona);

                return Ok(new
                {
                    ok = true,
                    message = "Usuario actualizado exitosamente",
                    data = personaUpdated
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Error al actualizar Persona",
                    error = ex.Message
                });
            }
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePersona(int id)
        {
            var persona = await _context.Personas.FindAsync(id);
            if (persona == null)
            {
                return NotFound();
            }
            _context.Personas.Remove(persona);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest();
            }
            return Ok(persona);
        }
        private async Task<PersonaDto?> GetPersonaDtoAsync(int id)
        {
            return await _context.Personas
                .Where(a => a.idpersona == id)
                .Select(a => new PersonaDto
                {
                    idpersona = a.idpersona,
                    nombre = a.nombre,
                    tipo_persona = a.tipo_persona,
                    tipo_documento = a.tipo_documento,
                    num_documento = a.num_documento,
                    direccion = a.direccion,
                    telefono = a.telefono,
                    email = a.email,
                }).FirstOrDefaultAsync();
        }
        [HttpPost("seed")]
        public async Task<IActionResult> Seed()
        {
            try
            {
                var faker = new Faker<Persona>("es")
                    .RuleFor(p => p.tipo_persona, f => f.PickRandom("Cliente", "Proveedor"))
                    .RuleFor(p => p.nombre, f => f.Name.FullName())
                    .RuleFor(p => p.tipo_documento, f => f.PickRandom("DNI", "RUC", "CE"))
                    .RuleFor(p => p.num_documento, (f, p) =>
                        p.tipo_documento == "DNI" ? f.Random.Replace("########") :
                        p.tipo_documento == "RUC" ? f.Random.Replace("###########") :
                        f.Random.Replace("#########"))
                    .RuleFor(p => p.direccion, f => f.Address.StreetAddress())
                    .RuleFor(p => p.telefono, f => f.Phone.PhoneNumber("9########"))
                    .RuleFor(p => p.email, f => f.Internet.Email());

                var personas = faker.Generate(15);

                await _context.Personas.AddRangeAsync(personas);
                await _context.SaveChangesAsync();

                var clientes = personas.Count(p => p.tipo_persona == "Cliente");
                var proveedores = personas.Count(p => p.tipo_persona == "Proveedor");

                return Ok(new
                {
                    message = "Se generaron 15 personas correctamente",
                    total = personas.Count,
                    clientes = clientes,
                    proveedores = proveedores
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error al insertar personas",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

    }
}
