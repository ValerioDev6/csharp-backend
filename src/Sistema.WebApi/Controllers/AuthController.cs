using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sistema.WebApi.Datos.DBContexAplication;
using Sistema.WebApi.Entities;
using Sistema.WebApi.ViewModels.auth;

namespace Sistema.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly DBContexAplication _context;

        private readonly IConfiguration _configuration;
        public AuthController(DBContexAplication context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {

            if (await _context.Usuarios.AnyAsync(u => u.email == registerModel.Email))
                return BadRequest("El email ya existe.");

            CreatePasswordHash(registerModel.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var usuario = new Usuario
            {
                idrol = registerModel.Idrol,
                nombre = registerModel.Nombre,
                tipo_documento = registerModel.TipoDocumento,
                num_documento = registerModel.NumDocumento,
                direccion = registerModel.Direccion,
                telefono = registerModel.Telefono,
                email = registerModel.Email,
                password_hash = passwordHash,
                password_salt = passwordSalt,
                condicion = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return Ok("Usuario registrado con éxito.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.email == loginModel.Email);

            if (usuario == null || !VerifyPasswordHash(loginModel.Password, usuario.password_hash, usuario.password_salt))
                return Unauthorized("Credenciales incorrectas.");

            var token = GenerateToken(usuario.idusuario, usuario.email);
            return Ok(new
            {
                Token = token,
                Usuario = new
                {
                    usuario.idusuario,
                    usuario.nombre,
                    usuario.email,
                    Rol = new
                    {
                        usuario.Rol.idrol,
                        usuario.Rol.nombre // Asegúrate de que tu entidad Rol tenga un campo "nombre"
                    }
                }
            });
        }

        // GET: api/Auth/me (Verificar autenticación y obtener datos del usuario)

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            // Verificar si el usuario está autenticado (si hay un claim NameIdentifier)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "No autenticado. Inicie sesión para continuar."
                });
            }

            // Buscar al usuario en la base de datos (incluyendo su rol)
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.idusuario == userId);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "Usuario no encontrado."
                });
            }

            // Devolver los datos del usuario
            return Ok(new
            {
                Success = true,
                Usuario = new
                {
                    usuario.idusuario,
                    usuario.nombre,
                    usuario.email,
                    Rol = new
                    {
                        usuario.Rol.idrol,
                        usuario.Rol.nombre
                    }
                }
            });
        }



        // Métodos auxiliares para hashear y verificar contraseñas
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA256();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[]? storedHash, byte[]? storedSalt)
        {
            if (storedHash == null || storedSalt == null)
                return false;

            using var hmac = new HMACSHA256(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }

        private string GenerateToken(int userId, string email)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"]!)),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
    //{
    //  "idrol": 1,
    //  "nombre": "Valerio Email",
    //  "tipoDocumento": "DNI",
    //  "numDocumento": "12345678",
    //  "direccion": "Av. Ejemplo 123",
    //  "telefono": "987654321",
    //  "email": "valerio@example.com",
    //  "password": "acasio2003"
    //}

}
