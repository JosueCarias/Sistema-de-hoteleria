using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hoteleria.Models;
using hoteleria.Data;
using System.ComponentModel.DataAnnotations;

namespace hoteleria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservacionesController : ControllerBase
    {
        private readonly HoteleriaContext _context;

        public ReservacionesController(HoteleriaContext context)
        {
            _context = context;
        }
        // Endpoint de prueba
        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok("El endpoint funciona - " + DateTime.Now);
        }

        // GET: api/reservaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservacion>>> GetReservaciones()
        {
            return await _context.Reservaciones.ToListAsync();
        }

        // GET: api/reservaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservacion>> GetReservacion(int id)
        {
            var reservacion = await _context.Reservaciones.FindAsync(id);
            if (reservacion == null) return NotFound();
            return reservacion;
        }
        // DELETE: api/reservaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservacion(int id)
        {
            var reservacion = await _context.Reservaciones.FindAsync(id);
            if (reservacion == null) return NotFound();
            
            _context.Reservaciones.Remove(reservacion);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        // PUT: api/reservaciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservacion(int id, [FromBody] Reservacion reservacion)
        {
            if (id != reservacion.ReservacionId) return BadRequest();
            
            _context.Entry(reservacion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
        // POST: api/reservaciones
        [HttpPost]
        public async Task<ActionResult<Reservacion>> PostReservacion([FromBody] Reservacion reservacion)
        {
            // Validación básica de fechas
            if (reservacion.FechaInicio >= reservacion.FechaFin)
            {
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin");
            }

            _context.Reservaciones.Add(reservacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservaciones), new { id = reservacion.ReservacionId }, reservacion);
        }
    }
}