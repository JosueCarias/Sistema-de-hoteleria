using Microsoft.AspNetCore.Mvc;
using hoteleria.Data;
using hoteleria.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace hoteleria.Controllers
{
    // Define la ruta base para todos los endpoints del controlador
    [Route("api/[controller]")]
    // Indica que este es un controlador de API con comportamientos predeterminados
    [ApiController]
    public class HotelesController : ControllerBase
    {
        // Contexto de base de datos para acceder a la tabla hotel
        private readonly HoteleriaContext _context;

        // Constructor que recibe el contexto por inyección de dependencias
        public HotelesController(HoteleriaContext context)
        {
            _context = context;
        }

        // POST: api/hoteles
        // Crea un nuevo registro en la tabla hotel
        [HttpPost]
        public async Task<ActionResult<Hotel>> CrearHotel([FromBody] HotelCreateDto hotelDto)
        {
            // Valida que los datos recibidos cumplan con las reglas definidas en el DTO
            if (!ModelState.IsValid)
            {
                // Devuelve error 400 con los detalles de validación fallidos
                return BadRequest(ModelState);
            }

            // Verifica si ya existe un hotel con el mismo nombre (evita duplicados)
            if (await _context.Hoteles.AnyAsync(h => h.Nombre == hotelDto.Nombre))
            {
                // Devuelve error 409 si encuentra un duplicado
                return Conflict($"Ya existe un hotel con el nombre '{hotelDto.Nombre}'");
            }

            // Crea un nuevo objeto Hotel a partir de los datos recibidos
            var hotel = new Hotel
            {
                Nombre = hotelDto.Nombre,
                Descripcion = hotelDto.Descripcion,
                Ubicacion = hotelDto.Ubicacion
            };

            try
            {
                // Agrega el nuevo hotel al contexto y guarda cambios
                _context.Hoteles.Add(hotel);
                await _context.SaveChangesAsync();

                // Devuelve código 201 (Created) con la ubicación del nuevo recurso
                return CreatedAtAction(nameof(ObtenerHotelPorId), new { id = hotel.HotelId }, hotel);
            }
            catch (DbUpdateException ex)
            {
                // Maneja errores específicos de la base de datos
                return StatusCode(500, new
                {
                    Message = "Error al guardar el hotel en la base de datos",
                    Error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // GET: api/hoteles/5
        // Obtiene un hotel específico por su ID (hotel_id)
        [HttpGet("{id}")]
        public async Task<ActionResult<Hotel>> ObtenerHotelPorId(int id)
        {
            // Busca el hotel usando su ID (clave primaria)
            var hotel = await _context.Hoteles.FindAsync(id);

            // Si no encuentra el hotel, devuelve 404
            if (hotel == null)
            {
                return NotFound($"No se encontró el hotel con ID {id}");
            }

            // Devuelve el hotel encontrado (código 200 implícito)
            return hotel;
        }

        // GET: api/hoteles
        // Obtiene todos los registros de la tabla hotel
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> ObtenerTodosHoteles()
        {
            // Retorna lista completa de hoteles
            return await _context.Hoteles.ToListAsync();
        }

        // PUT: api/hoteles/5
        // Actualiza un registro existente en la tabla hotel
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
                return NotFound($"No se encontró el hotel con ID {id}");
            }

            // Verifica que el nuevo nombre no exista en otro hotel
            if (await _context.Hoteles.AnyAsync(h => h.Nombre == hotelDto.Nombre && h.HotelId != id))
            {
                return Conflict($"Ya existe un hotel con el nombre '{hotelDto.Nombre}'");
            }

            // Actualiza las propiedades del hotel
            hotel.Nombre = hotelDto.Nombre;
            hotel.Descripcion = hotelDto.Descripcion;
            hotel.Ubicacion = hotelDto.Ubicacion;

            try
            {
                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();
                // Devuelve el hotel actualizado
                return Ok(hotel);
            }
            catch (DbUpdateException ex)
            {
                // Maneja errores de actualización
                return StatusCode(500, new
                {
                    Message = "Error al actualizar el hotel en la base de datos",
                    Error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // DELETE: api/hoteles/5
        // Elimina un registro de la tabla hotel
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarHotel(int id)
        {
            // Busca el hotel incluyendo sus habitaciones relacionadas
            var hotel = await _context.Hoteles
                .Include(h => h.Habitaciones)
                .FirstOrDefaultAsync(h => h.HotelId == id);

            if (hotel == null)
            {
                return NotFound($"Hotel con ID {id} no encontrado");
            }

            // Verifica que el hotel no tenga habitaciones asignadas
            if (hotel.Habitaciones.Any())
            {
                return BadRequest(new
                {
                    Message = "No se puede eliminar el hotel porque tiene habitaciones asignadas",
                    Habitaciones = hotel.Habitaciones.Select(h => h.HabitacionId)
                });
            }

            try
            {
                // Elimina el hotel y guarda cambios
                _context.Hoteles.Remove(hotel);
                await _context.SaveChangesAsync();
                // Devuelve código 204 (No Content) para operación exitosa
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                // Maneja errores de eliminación
                return StatusCode(500, new
                {
                    Message = "Error al eliminar el hotel",
                    Error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // GET: api/hoteles/test
        // Endpoint simple para probar que el controlador está funcionando
        [HttpGet("test")]
        public ActionResult ProbarEndpoint()
        {
            return Ok(new
            {
                Estado = "Funcionando",
                Mensaje = "El servicio de hoteles está operativo",
                Fecha = DateTime.Now,
                Tabla = "hotel",
                Campos = new
                {
                    hotel_id = "int (PK, identity)",
                    nombre = "varchar(50) not null",
                    descripcion = "varchar(150) null",
                    ubicacion = "varchar(150) not null"
                }
            });
        }
    }

    // Modelo DTO para creación de hoteles
    // (Data Transfer Object - Objeto para transferencia de datos)
    public class HotelCreateDto
    {
        [Required(ErrorMessage = "El campo nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        public string Nombre { get; set; }

        [StringLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La ubicación es requerida")]
        [StringLength(150, ErrorMessage = "La ubicación no puede superar 150 caracteres")]
        public string Ubicacion { get; set; }
    }

    // Modelo DTO para actualización de hoteles
    public class HotelUpdateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "Longitud máxima de nombre: 50 caracteres")]
        public string Nombre { get; set; }

        [StringLength(150, ErrorMessage = "Descripción máxima: 150 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debe especificar una ubicación")]
        [StringLength(150, ErrorMessage = "Ubicación no mayor a 150 caracteres")]
        public string Ubicacion { get; set; }
    }
}
