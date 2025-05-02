using Microsoft.AspNetCore.Mvc;
using hoteleria.Data;
using hoteleria.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace hoteleria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly HoteleriaContext _context;

        public UsuarioController(HoteleriaContext context)
        {
            _context = context;
        }

        [HttpGet("test-connection")]
        public async Task<ActionResult<string>> TestConnection()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Ok("Conexi?n exitosa a la base de datos.");
                }
                else
                {
                    return StatusCode(500, "No se pudo conectar a la base de datos.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al conectar a la base de datos: {ex.Message}");
            }
        }
        // Endpoint de prueba
        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok("El endpoint funciona - " + DateTime.Now);
        }
        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            if (usuarios == null || usuarios.Count == 0)
            {
                return NotFound();
            }
            return usuarios;
        }


        /* Obtener una tupla por medio del ID*/
        // GET: api/usuarios/1
        [HttpGet("{id}")]
        // [AutoValidateAntiforgeryToken]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            Console.WriteLine(usuario);
            if (usuario == null)
            {
                return NotFound();
            }
            return usuario;
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario([FromBody] UsuarioCreateDto usuarioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que no exista ya un usuario con el mismo username
            if (await _context.Usuarios.AnyAsync(u => u.Username == usuarioDto.Username))
            {
                return Conflict($"Ya existe un usuario con el nombre '{usuarioDto.Username}'.");
            }

            // Verificar existencia de EmpleadoId y RolId si hay relaciones
            if (!await _context.Empleados.AnyAsync(e => e.EmpleadoId == usuarioDto.EmpleadoId))
            {
                return NotFound($"No se encontró el empleado con ID {usuarioDto.EmpleadoId}.");
            }

            if (!await _context.Roles.AnyAsync(r => r.RolId == usuarioDto.RolId))
            {
                return NotFound($"No se encontró el rol con ID {usuarioDto.RolId}.");
            }

            var usuario = new Usuario
            {
                Username = usuarioDto.Username,
                Password = usuarioDto.Password,
                EmpleadoId = usuarioDto.EmpleadoId,
                RolId = usuarioDto.RolId
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.UsuarioId }, usuario);
        }


        // PUT: api/usuarios/1
        [HttpPut("{id}")]
        public async Task<ActionResult<Usuario>> PutUsuario(int id, [FromBody] UsuarioCreateDto usuarioDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound($"No se encontró el usuario con ID {id}.");
            }

            if (await _context.Usuarios.AnyAsync(u => u.Username == usuarioDto.Username && u.UsuarioId != id))
            {
                return Conflict($"Ya existe otro usuario con el nombre '{usuarioDto.Username}'.");
            }

            if (!await _context.Empleados.AnyAsync(e => e.EmpleadoId == usuarioDto.EmpleadoId))
            {
                return NotFound($"No se encontró el empleado con ID {usuarioDto.EmpleadoId}.");
            }

            if (!await _context.Roles.AnyAsync(r => r.RolId == usuarioDto.RolId))
            {
                return NotFound($"No se encontró el rol con ID {usuarioDto.RolId}.");
            }

            usuario.Username = usuarioDto.Username;
            usuario.Password = usuarioDto.Password;
            usuario.EmpleadoId = usuarioDto.EmpleadoId;
            usuario.RolId = usuarioDto.RolId;

            await _context.SaveChangesAsync();

            return Ok(usuario);
        }


        // DELETE: api/usuarios/1
        [HttpDelete("{id}")]
        public async Task<ActionResult<Usuario>> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }




    }


    public class UsuarioCreateDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contrase?a es obligatoria")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "El identificador del empleado es obligatorio")]
        public int EmpleadoId { get; set; }

        [Required(ErrorMessage = "El identificador del rol es obligatorio")]
        public int RolId { get; set; }
    }

}