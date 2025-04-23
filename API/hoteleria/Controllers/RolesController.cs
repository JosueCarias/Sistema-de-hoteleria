using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using hoteleria.Models;    // Asegúrate de que Rol esté aquí
using hoteleria.Data;  

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

        // GET: api/roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rol>>> GetAllRoles()
        {
            return await _context.Roles.ToListAsync();
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

        // POST: api/roles
        [HttpPost]
        public async Task<ActionResult<Rol>> PostRol([FromBody] RolCreateDto rolDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar duplicados
            if (await _context.Roles.AnyAsync(r => r.TipoRol == rolDto.TipoRol))
            {
                return Conflict($"Ya existe un rol con el nombre '{rolDto.TipoRol}'");
            }

            var rol = new Rol { TipoRol = rolDto.TipoRol };
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRol), new { id = rol.RolId }, rol);
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
                return NotFound($"No se encontró el rol con ID {id}");
            }

            // Validar duplicados (excepto el mismo registro)
            if (await _context.Roles.AnyAsync(r => r.TipoRol == rolDto.TipoRol && r.RolId != id))
            {
                return Conflict($"Ya existe un rol con el nombre '{rolDto.TipoRol}'");
            }

            rol.TipoRol = rolDto.TipoRol;
            await _context.SaveChangesAsync();

            return Ok(rol);
        }

        // DELETE: api/roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
            {
                return NotFound($"No se encontró el rol con ID {id}");
            }

            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Endpoint de prueba
        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok("El endpoint funciona - " + DateTime.Now);
        }
    }

    // DTOs (Data Transfer Objects)
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