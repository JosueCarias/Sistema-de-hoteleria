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
    public class TipoHabitacionesController : ControllerBase
    {
        private readonly HoteleriaContext _context;

        // Corrección: El constructor debe coincidir con el nombre de la clase
        public TipoHabitacionesController(HoteleriaContext context)
        {
            _context = context;
        }

        // Endpoint de prueba
        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok("El endpoint de TipoHabitaciones funciona - " + DateTime.Now);
        }

        // GET: api/tipohabitaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoHabitacion>>> GetTipoHabitaciones()
        {
            return await _context.TipoHabitaciones.ToListAsync();
        }

        // GET: api/tipohabitaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoHabitacion>> GetTipoHabitacion(int id)
        {
            var tipoHabitacion = await _context.TipoHabitaciones.FindAsync(id);

            if (tipoHabitacion == null)
            {
                return NotFound();
            }

            return tipoHabitacion;
        }

        // POST: api/tipohabitaciones
        [HttpPost]
        public async Task<ActionResult<TipoHabitacion>> PostTipoHabitacion([FromBody] TipoHabitacion tipoHabitacion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TipoHabitaciones.Add(tipoHabitacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTipoHabitacion), new { id = tipoHabitacion.TipoHabitacionId }, tipoHabitacion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoHabitacion(int id, [FromBody] TipoHabitacionUpdateDto tipoHabitacionDto)
        {
            var tipoHabitacion = await _context.TipoHabitaciones.FindAsync(id);
            if (tipoHabitacion == null)
            {
                return NotFound();
            }

            // Actualizar solo los campos proporcionados
            if (tipoHabitacionDto.Tipo != null)
                tipoHabitacion.Tipo = tipoHabitacionDto.Tipo;

            if (tipoHabitacionDto.Descripcion != null)
                tipoHabitacion.Descripcion = tipoHabitacionDto.Descripcion;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoHabitacionExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DTO para actualización
        public class TipoHabitacionUpdateDto
        {
            [StringLength(50)]
            public string? Tipo { get; set; }

            [StringLength(150)]
            public string? Descripcion { get; set; }
        }

        // DELETE: api/tipohabitaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoHabitacion(int id)
        {
            var tipoHabitacion = await _context.TipoHabitaciones.FindAsync(id);
            if (tipoHabitacion == null)
            {
                return NotFound();
            }

            _context.TipoHabitaciones.Remove(tipoHabitacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TipoHabitacionExists(int id)
        {
            return _context.TipoHabitaciones.Any(e => e.TipoHabitacionId == id);
        }
    }
}
