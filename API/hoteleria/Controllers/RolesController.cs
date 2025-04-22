using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using hoteleria.Data;
using hoteleria.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace hoteleria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly HoteleriaContext _context;

        public RolesController(HoteleriaContext context)
        {
            _context = context;
        }

        // POST: api/roles
        [HttpPost]
        public async Task<ActionResult<Rol>> PostRol([FromBody] RolCreateDto rolDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _context.Roles.AnyAsync(r => r.TipoRol == rolDto.TipoRol))
            {
                return Conflict($"Ya existe un rol con el nombre '{rolDto.TipoRol}'");
            }

            var rol = new Rol
            {
                TipoRol = rolDto.TipoRol
            };

            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRol), new { id = rol.RolId }, rol);
        }


        // GET: api/roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rol>> GetRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);

            if (rol == null)
            {
                return NotFound($"No se encontró el rol con ID {id}");
            }

            return rol;
        }

        // DELETE: api/roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            try
            {     
                var rol = await _context.Roles
                    .AsNoTracking()
                    .Include(r => r.Usuarios)
                    .FirstOrDefaultAsync(r => r.RolId == id);

                if (rol == null)
                {
                    return NotFound(new { Message = $"Rol con ID {id} no encontrado" });
                }

                if (rol.Usuarios?.Any() == true)
                {
                    return BadRequest(new 
                {
                    Message = "No se puede eliminar el rol porque tiene usuarios asignados",
                    Usuarios = rol.Usuarios.Select(u => new { u.UsuarioId, u.Username })
                });
            }

            // Elimina el rol por ID sin necesidad de cargar toda la entidad
            var rolToDelete = new Rol { RolId = id };
            _context.Roles.Attach(rolToDelete);
            _context.Roles.Remove(rolToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new 
            {
                Message = "Error al eliminar el rol (relaciones u otros problemas de base de datos)",
                Error = ex.InnerException?.Message ?? ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            {
                Message = "Error inesperado al eliminar el rol",
                Error = ex.Message
            });
        }
    }

        // GET: api/roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetAllRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        // PUT: api/roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol(int id, [FromBody] RolUpdateDto rolDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
            {
                return NotFound(new { Message = $"No se encontró el rol con ID {id}" });
            }

            if (await _context.Roles.AnyAsync(r => r.TipoRol == rolDto.TipoRol && r.RolId != id))
            {
                return Conflict(new { Message = $"Ya existe un rol con el nombre '{rolDto.TipoRol}'" });
            }

            rol.TipoRol = rolDto.TipoRol;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { 
                    Message = "Rol actualizado correctamente",
                    Rol = rol
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new {
                    Message = "Error al actualizar el rol en la base de datos",
                    Error = ex.InnerException?.Message
                });
            }
        }      

        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok("El endpoint funciona - " + DateTime.Now);
        }

        private bool RolExists(int id)
        {
            return _context.Roles.Any(e => e.RolId == id);
        }
    }

    public class RolCreateDto
    {
        [Required(ErrorMessage = "El tipo de rol es obligatorio")]
        [StringLength(50, ErrorMessage = "El tipo de rol no puede exceder 50 caracteres")]
        public string TipoRol { get; set; } = string.Empty;
    }

    public class RolUpdateDto
    {
        [Required(ErrorMessage = "El tipo de rol es obligatorio")]
        [StringLength(50, ErrorMessage = "El tipo de rol no puede exceder 50 caracteres")]
        public string TipoRol { get; set; } = string.Empty;
    }
}