using Microsoft.AspNetCore.Mvc;
using hoteleria.Data;
using hoteleria.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace hoteleria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class hotelesController : ControllerBase
    {
        private readonly HoteleriaContext _context;

        public hotelesController(HoteleriaContext context)
        {
            _context = context;
        }
        // Endpoint de prueba
        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok("El endpoint funciona - " + DateTime.Now);
        }
        // GET: api/hoteles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetHoteles()
        {
            try
            {
                var hoteles = await _context.Hoteles.ToListAsync();
                
                if (hoteles == null || hoteles.Count == 0)
                {
                    return NotFound("No se encontraron hoteles registrados");
                }
                
                return Ok(hoteles);
            }
            catch (Exception ex)
            {
                // Log del error (puedes implementar un logger)
                return StatusCode(500, "Error interno al obtener los hoteles");
            }
        }
        // GET: api/hoteles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hotel>> GetHotel(int id)
        {
            var hotel = await _context.Hoteles.FindAsync(id);

            if (hotel == null)
            {
                return NotFound($"Hotel con ID {id} no encontrado");
            }

            return hotel;
        }
        // DELETE: api/hoteles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            try
            {
                // Buscar el hotel por ID
                var hotel = await _context.Hoteles.FindAsync(id);

                // Si no se encuentra, retornar NotFound
                if (hotel == null)
                {
                    return NotFound($"Hotel con ID {id} no encontrado");
                }

                // Verificar si hay reservaciones asociadas (opcional)
                var tieneReservaciones = await _context.Reservaciones
                    .AnyAsync(r => r.HotelId == id);

                if (tieneReservaciones)
                {
                    return BadRequest("No se puede eliminar el hotel porque tiene reservaciones asociadas");
                }

                // Eliminar el hotel
                _context.Hoteles.Remove(hotel);
                await _context.SaveChangesAsync();

                return NoContent(); // Retorna 204 No Content
            }
            catch (Exception ex)
            {
                // Log del error (puedes implementar un logger)
                return StatusCode(500, "Error interno al eliminar el hotel");
            }
        }
        // POST: api/hoteles
        [HttpPost]
        public async Task<ActionResult<Hotel>> CreateHotel([FromBody] HotelDto hotelDto)
        {
            try
            {
                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validación personalizada (ejemplo: nombre único)
                if (await _context.Hoteles.AnyAsync(h => h.Nombre == hotelDto.Nombre))
                {
                    return Conflict("Ya existe un hotel con ese nombre");
                }

                // Mapear DTO a modelo
                var hotel = new Hotel
                {
                    Nombre = hotelDto.Nombre,
                    Descripcion = hotelDto.Descripcion,
                    Ubicacion = hotelDto.Ubicacion
                };

                // Agregar y guardar
                _context.Hoteles.Add(hotel);
                await _context.SaveChangesAsync();

                // Retornar respuesta con el hotel creado
                return CreatedAtAction(nameof(GetHotel), new { id = hotel.HotelId }, hotel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno al crear el hotel");
            }
        }
        // PUT: api/hoteles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] HotelDto hotelDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var hotel = await _context.Hoteles.FindAsync(id);
                if (hotel == null)
                {
                    return NotFound();
                }

                if (await _context.Hoteles.AnyAsync(h => h.Nombre == hotelDto.Nombre && h.HotelId != id))
                {
                    return Conflict("Ya existe otro hotel con ese nombre");
                }

                hotel.Nombre = hotelDto.Nombre;
                hotel.Descripcion = hotelDto.Descripcion;
                hotel.Ubicacion = hotelDto.Ubicacion;

                _context.Entry(hotel).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Hoteles.Any(e => e.HotelId == id)) // Solución alternativa
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno al actualizar el hotel");
            }
        }

        // Método auxiliar (Solución 1)
        private bool HotelExists(int id)
        {
            return _context.Hoteles.Any(e => e.HotelId == id);
        }
    }
    public class HotelDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string Nombre { get; set; }

        [StringLength(150, ErrorMessage = "La descripción no puede exceder los 150 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(150, ErrorMessage = "La ubicación no puede exceder los 150 caracteres")]
        public string Ubicacion { get; set; }
    }
}