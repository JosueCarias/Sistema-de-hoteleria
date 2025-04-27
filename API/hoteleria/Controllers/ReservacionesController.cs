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

            if (reservacion == null)
            {
                return NotFound();
            }

            return reservacion;
        }

        // POST: api/reservaciones
        [HttpPost]
        public async Task<ActionResult<Reservacion>> PostReservacion(ReservacionSimpleDto reservacionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validación simple de fechas
            if (reservacionDto.FechaInicio >= reservacionDto.FechaFin)
            {
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin");
            }

            var reservacion = new Reservacion
            {
                ClienteNombre = reservacionDto.ClienteNombre,
                EmpleadoNombre = reservacionDto.EmpleadoNombre,
                HotelNombre = reservacionDto.HotelNombre,
                FechaInicio = reservacionDto.FechaInicio,
                FechaFin = reservacionDto.FechaFin,
                Costo = reservacionDto.Costo
            };

            _context.Reservaciones.Add(reservacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReservacion", new { id = reservacion.ReservacionId }, reservacion);
        }

        // PUT: api/reservaciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservacion(int id, ReservacionSimpleDto reservacionDto)
        {
            var reservacion = await _context.Reservaciones.FindAsync(id);
            if (reservacion == null)
            {
                return NotFound();
            }

            // Validación simple de fechas
            if (reservacionDto.FechaInicio >= reservacionDto.FechaFin)
            {
                return BadRequest("La fecha de inicio debe ser anterior a la fecha de fin");
            }

            // Actualizar campos
            reservacion.ClienteNombre = reservacionDto.ClienteNombre;
            reservacion.EmpleadoNombre = reservacionDto.EmpleadoNombre;
            reservacion.HotelNombre = reservacionDto.HotelNombre;
            reservacion.FechaInicio = reservacionDto.FechaInicio;
            reservacion.FechaFin = reservacionDto.FechaFin;
            reservacion.Costo = reservacionDto.Costo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservacionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/reservaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservacion(int id)
        {
            var reservacion = await _context.Reservaciones.FindAsync(id);
            if (reservacion == null)
            {
                return NotFound();
            }

            _context.Reservaciones.Remove(reservacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReservacionExists(int id)
        {
            return _context.Reservaciones.Any(e => e.ReservacionId == id);
        }
    }

    public class ReservacionSimpleDto
    {
        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        public string ClienteNombre { get; set; }

        [Required(ErrorMessage = "El nombre del empleado es obligatorio")]
        public string EmpleadoNombre { get; set; }

        [Required(ErrorMessage = "El nombre del hotel es obligatorio")]
        public string HotelNombre { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "El costo es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor a 0")]
        public decimal Costo { get; set; }
    }
}