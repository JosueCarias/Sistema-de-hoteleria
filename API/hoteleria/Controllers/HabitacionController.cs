using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient; // Necesario para SqlException
using hoteleria.Models;
using hoteleria.Data;
using System.ComponentModel.DataAnnotations;


namespace hoteleria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitacionesController : ControllerBase
    {
        private readonly HoteleriaContext _context;

        public HabitacionesController(HoteleriaContext context)
        {
            _context = context;
        }
        // Endpoint de prueba
        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok("El endpoint funciona - " + DateTime.Now);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Habitacion>>> GetHabitaciones()
        {
            return await _context.Habitaciones.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Habitacion>> GetHabitacion(int id)
        {
            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null) return NotFound();
            return habitacion;
        }

        // POST: api/habitaciones
        [HttpPost]
        public async Task<ActionResult<Habitacion>> PostHabitacion([FromBody] HabitacionCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Normalizar disponibilidad
                dto.Disponibilidad = NormalizarDisponibilidad(dto.Disponibilidad);

                var habitacion = new Habitacion
                {
                    Disponibilidad = dto.Disponibilidad,
                    HotelId = dto.HotelId,
                    TipoHabitacionId = dto.TipoHabitacionId
                };

                _context.Habitaciones.Add(habitacion);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetHabitacion), new { id = habitacion.HabitacionId }, habitacion);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Message.Contains("chk_disponibilidad"))
            {
                return BadRequest("El valor de disponibilidad debe ser exactamente 'Sí' o 'No'");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno al crear la habitación");
            }
        }

        private string NormalizarDisponibilidad(string disponibilidad)
        {
            return disponibilidad?.Trim() switch
            {
                "Si" => "Sí",
                "S" => "Sí",
                "N" => "No",
                _ => disponibilidad
            };
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutHabitacion(int id, HabitacionUpdateDto dto)
        {
            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null) return NotFound();

            if (dto.Disponibilidad != null) habitacion.Disponibilidad = dto.Disponibilidad;
            if (dto.HotelId.HasValue) habitacion.HotelId = dto.HotelId.Value;
            if (dto.TipoHabitacionId.HasValue) habitacion.TipoHabitacionId = dto.TipoHabitacionId.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHabitacion(int id)
        {
            var habitacion = await _context.Habitaciones.FindAsync(id);
            if (habitacion == null) return NotFound();

            _context.Habitaciones.Remove(habitacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class HabitacionCreateDto
    {
        [Required]
        [StringLength(2)]
        [RegularExpression(@"^(Sí|No|S�)$", ErrorMessage = "Debe ser 'Sí' o 'No'")]
        public string Disponibilidad { get; set; } = "Sí";

        [Required]
        public int HotelId { get; set; }

        [Required]
        public int TipoHabitacionId { get; set; }
    }

    public class HabitacionUpdateDto
    {
        [StringLength(2)]
        [RegularExpression(@"^(Sí|No|S�)$", ErrorMessage = "Debe ser 'Sí' o 'No'")]
        public string? Disponibilidad { get; set; }

        public int? HotelId { get; set; }
        public int? TipoHabitacionId { get; set; }
    }
}
