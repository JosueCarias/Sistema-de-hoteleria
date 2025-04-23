using Microsoft.AspNetCore.Mvc;
using hoteleria.Data;
using hoteleria.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace hoteleria.Controllers
{
    [Route("api/[controller]")]  // Define la ruta base como 'api/hoteles'
    [ApiController]  // Indica que es un controlador de API
    public class HotelesController : ControllerBase
    {
        private readonly HoteleriaContext _context;  // Inyección de dependencia del contexto

        public HotelesController(HoteleriaContext context)
        {
            _context = context;  // Asigna el contexto inyectado
        }

        // POST: api/hoteles
        // Crea un nuevo hotel en la base de datos
        [HttpPost]
        public async Task<ActionResult<Hotel>> CrearHotel([FromBody] HotelCreateDto hotelDto)
        {
            // Valida el modelo recibido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Devuelve errores de validación
            }

            // Verifica si ya existe un hotel con el mismo nombre
            if (await _context.Hoteles.AnyAsync(h => h.Nombre == hotelDto.Nombre))
            {
                return Conflict($"Ya existe un hotel con el nombre '{hotelDto.Nombre}'");
            }

            // Crea el nuevo objeto Hotel a partir del DTO
            var hotel = new Hotel
            {
                Nombre = hotelDto.Nombre,
                Descripcion = hotelDto.Descripcion,
                Ubicacion = hotelDto.Ubicacion
            };

            // Agrega y guarda en la base de datos
            _context.Hoteles.Add(hotel);
            await _context.SaveChangesAsync();

            // Devuelve respuesta 201 (Created) con la ubicación del nuevo recurso
            return CreatedAtAction(nameof(ObtenerHotelPorId), new { id = hotel.HotelId }, hotel);
        }

        // GET: api/hoteles/5
        // Obtiene un hotel específico por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Hotel>> ObtenerHotelPorId(int id)
        {
            // Busca el hotel en la base de datos
            var hotel = await _context.Hoteles.FindAsync(id);

            // Si no se encuentra, devuelve 404
            if (hotel == null)
            {
                return NotFound($"No se encontró el hotel con ID {id}");
            }

            return hotel;  // Devuelve el hotel encontrado
        }

        // DELETE: api/hoteles/5
        // Elimina un hotel por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarHotel(int id)
        {
            try
            {
                // Busca el hotel incluyendo sus habitaciones relacionadas
                var hotel = await _context.Hoteles
                    .Include(h => h.Habitaciones)
                    .FirstOrDefaultAsync(h => h.HotelId == id);

                if (hotel == null)
                {
                    return NotFound(new { Message = $"Hotel con ID {id} no encontrado" });
                }

                // Verifica si el hotel tiene habitaciones asignadas
                if (hotel.Habitaciones?.Any() == true)
                {
                    return BadRequest(new
                    {
                        Message = "No se puede eliminar el hotel porque tiene habitaciones asignadas",
                        Habitaciones = hotel.Habitaciones.Select(h => h.HabitacionId)
                    });
                }

                // Elimina el hotel
                _context.Hoteles.Remove(hotel);
                await _context.SaveChangesAsync();

                return NoContent();  // Respuesta 204 (No Content) para operaciones exitosas
            }
            catch (DbUpdateException ex)
            {
                // Maneja errores de base de datos
                return StatusCode(500, new
                {
                    Message = "Error al eliminar el hotel",
                    Error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // GET: api/hoteles
        // Obtiene todos los hoteles registrados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> ObtenerTodosHoteles()
        {
            return await _context.Hoteles.ToListAsync();  // Devuelve lista completa de hoteles
        }

        // PUT: api/hoteles/5
        // Actualiza un hotel existente
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarHotel(int id, [FromBody] HotelUpdateDto hotelDto)
        {
            // Valida el modelo recibido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Busca el hotel a actualizar
            var hotel = await _context.Hoteles.FindAsync(id);
            if (hotel == null)
            {
                return NotFound(new { Message = $"No se encontró el hotel con ID {id}" });
            }

            // Verifica si el nuevo nombre ya existe en otro hotel
            if (await _context.Hoteles.AnyAsync(h => h.Nombre == hotelDto.Nombre && h.HotelId != id))
            {
                return Conflict(new { Message = $"Ya existe un hotel con el nombre '{hotelDto.Nombre}'" });
            }

            // Actualiza las propiedades del hotel
            hotel.Nombre = hotelDto.Nombre;
            hotel.Descripcion = hotelDto.Descripcion;
            hotel.Ubicacion = hotelDto.Ubicacion;

            try
            {
                // Guarda los cambios
                await _context.SaveChangesAsync();
                return Ok(hotel);  // Devuelve el hotel actualizado
            }
            catch (DbUpdateException ex)
            {
                // Maneja errores de base de datos
                return StatusCode(500, new
                {
                    Message = "Error al actualizar el hotel en la base de datos",
                    Error = ex.InnerException?.Message
                });
            }
        }

        // GET: api/hoteles/test
        // Endpoint de prueba para verificar que el controlador funciona
        [HttpGet("test")]
        public ActionResult ProbarEndpoint()
        {
            return Ok(new
            {
                Estado = "Funcionando",
                Controlador = "Hoteles",
                Fecha = DateTime.Now
            });
        }
    }

    // DTO para creación de hoteles
    public class HotelCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string Nombre { get; set; }

        [StringLength(150, ErrorMessage = "La descripción no puede exceder los 150 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(150, ErrorMessage = "La ubicación no puede exceder los 150 caracteres")]
        public string Ubicacion { get; set; }
    }

    // DTO para actualización de hoteles
    public class HotelUpdateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres")]
        public string Nombre { get; set; }

        [StringLength(150, ErrorMessage = "La descripción no puede exceder los 150 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        [StringLength(150, ErrorMessage = "La ubicación no puede exceder los 150 caracteres")]
        public string Ubicacion { get; set; }
    }
}