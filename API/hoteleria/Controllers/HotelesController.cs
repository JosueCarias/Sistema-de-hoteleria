using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace hoteleria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelesController : ControllerBase
    {
        private readonly HoteleriaContext _context;

        public HotelesController(HoteleriaContext context)
        {
            _context = context;
        }

        // POST: api/hoteles
        [HttpPost]
        public async Task<ActionResult<Hotel>> PostHotel([FromBody] HotelCreateDto hotelDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hotel = new Hotel
            {
                Nombre = hotelDto.Nombre,
                Descripcion = hotelDto.Descripcion,
                Ubicacion = hotelDto.Ubicacion
            };

            _context.Hoteles.Add(hotel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHotel), new { id = hotel.HotelId }, hotel);
        }

        // GET: api/hoteles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hotel>> GetHotel(int id)
        {
            var hotel = await _context.Hoteles.FindAsync(id);

            if (hotel == null)
            {
                return NotFound($"No se encontró el hotel con ID {id}");
            }

            return hotel;
        }

        // GET: api/hoteles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetAllHoteles()
        {
            return await _context.Hoteles.ToListAsync();
        }

        // PUT: api/hoteles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotel(int id, [FromBody] HotelUpdateDto hotelDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hotel = await _context.Hoteles.FindAsync(id);
            if (hotel == null)
            {
                return NotFound(new { Message = $"No se encontró el hotel con ID {id}" });
            }

            hotel.Nombre = hotelDto.Nombre;
            hotel.Descripcion = hotelDto.Descripcion;
            hotel.Ubicacion = hotelDto.Ubicacion;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    Message = "Hotel actualizado correctamente",
                    Hotel = hotel
                });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error al actualizar el hotel en la base de datos",
                    Error = ex.InnerException?.Message
                });
            }
        }

        // DELETE: api/hoteles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            try
            {
                var hotel = await _context.Hoteles.FindAsync(id);

                if (hotel == null)
                {
                    return NotFound(new { Message = $"Hotel con ID {id} no encontrado" });
                }

                _context.Hoteles.Remove(hotel);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error al eliminar el hotel (posiblemente tiene relaciones)",
                    Error = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error inesperado al eliminar el hotel",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok("El endpoint funciona - " + DateTime.Now);
        }

        private bool HotelExists(int id)
        {
            return _context.Hoteles.Any(e => e.HotelId == id);
        }
    }

    public class HotelCreateDto
    {
        [Required(ErrorMessage = "El nombre del hotel es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(150, ErrorMessage = "La ubicación no puede exceder 150 caracteres")]
        public string Ubicacion { get; set; } = string.Empty;
    }

    public class HotelUpdateDto
    {
        [Required(ErrorMessage = "El nombre del hotel es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(150, ErrorMessage = "La ubicación no puede exceder 150 caracteres")]
        public string Ubicacion { get; set; } = string.Empty;
    }
}
