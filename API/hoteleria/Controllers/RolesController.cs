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
                return NotFound();
            }

            return rol;
        }
    }

    public class RolCreateDto
    {
        [Required(ErrorMessage = "El tipo de rol es obligatorio")]
        public string TipoRol { get; set; } = string.Empty;
    }
}