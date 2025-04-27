using Microsoft.AspNetCore.Mvc;
using hoteleria.Data;
using hoteleria.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace hoteleria.Controllers
{
    // Ruta base para los endpoints: api/tiposhabitacion
    [Route("api/[controller]")]
    [ApiController]
    public class TiposHabitacionController : ControllerBase
    {
        // Contexto de base de datos para acceder a tipo_habitacion
        private readonly HoteleriaContext _context;

        // Inyección de dependencias del contexto
        public TiposHabitacionController(HoteleriaContext context)
        {
            _context = context;
        }

        // POST: api/tiposhabitacion
        // Crea un nuevo tipo de habitación en la tabla tipo_habitacion
        [HttpPost]
        public async Task<ActionResult<TipoHabitacion>> CrearTipoHabitacion([FromBody] TipoHabitacionCreateDto tipoHabitacionDto)
        {
            // Valida que los datos cumplan con las reglas del DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Error 400 si validación falla
            }

            // Verifica si ya existe un tipo con el mismo nombre (evita duplicados)
            if (await _context.TiposHabitacion.AnyAsync(th => th.Tipo == tipoHabitacionDto.Tipo))
            {
                return Conflict($"Ya existe un tipo de habitación llamado '{tipoHabitacionDto.Tipo}'"); // Error 409
            }

            // Crea el nuevo objeto TipoHabitacion
            var tipoHabitacion = new TipoHabitacion
            {
                Tipo = tipoHabitacionDto.Tipo,
                Descripcion = tipoHabitacionDto.Descripcion
            };

            try
            {
                // Agrega y guarda en la base de datos
                _context.TiposHabitacion.Add(tipoHabitacion);
                await _context.SaveChangesAsync();

                // Retorna 201 (Created) con la ubicación del nuevo recurso
                return CreatedAtAction(nameof(ObtenerTipoHabitacionPorId), new { id = tipoHabitacion.TipoHabitacionId }, tipoHabitacion);
            }
            catch (DbUpdateException ex)
            {
                // Maneja errores específicos de base de datos
                return StatusCode(500, new
                {
                    Message = "Error al guardar en la base de datos",
                    Error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // GET: api/tiposhabitacion/5
        // Obtiene un tipo de habitación por su ID (tipo_habitacion_id)
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoHabitacion>> ObtenerTipoHabitacionPorId(int id)
        {
            // Busca el tipo por su clave primaria
            var tipoHabitacion = await _context.TiposHabitacion.FindAsync(id);

            if (tipoHabitacion == null)
            {
                return NotFound($"Tipo de habitación con ID {id} no encontrado"); // Error 404
            }

            return tipoHabitacion; // Retorna 200 con el tipo encontrado
        }

        // GET: api/tiposhabitacion
        // Obtiene todos los registros de la tabla tipo_habitacion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoHabitacion>>> ObtenerTodosTiposHabitacion()
        {
            return await _context.TiposHabitacion.ToListAsync(); // Retorna 200 con la lista
        }

        // PUT: api/tiposhabitacion/5
        // Actualiza un tipo de habitación existente
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarTipoHabitacion(int id, [FromBody] TipoHabitacionUpdateDto tipoHabitacionDto)
        {
            // Valida el modelo recibido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Busca el tipo a actualizar
            var tipoHabitacion = await _context.TiposHabitacion.FindAsync(id);
            if (tipoHabitacion == null)
            {
                return NotFound($"Tipo de habitación con ID {id} no encontrado");
            }

            // Verifica que el nuevo nombre no exista en otro registro
            if (await _context.TiposHabitacion.AnyAsync(th => th.Tipo == tipoHabitacionDto.Tipo && th.TipoHabitacionId != id))
            {
                return Conflict($"Ya existe un tipo llamado '{tipoHabitacionDto.Tipo}'");
            }

            // Actualiza las propiedades
            tipoHabitacion.Tipo = tipoHabitacionDto.Tipo;
            tipoHabitacion.Descripcion = tipoHabitacionDto.Descripcion;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(tipoHabitacion); // Retorna 200 con el tipo actualizado
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error al actualizar en la base de datos",
                    Error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // DELETE: api/tiposhabitacion/5
        // Elimina un tipo de habitación si no tiene habitaciones asociadas
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarTipoHabitacion(int id)
        {
            // Busca el tipo incluyendo sus habitaciones relacionadas
            var tipoHabitacion = await _context.TiposHabitacion
                .Include(th => th.Habitaciones)
                .FirstOrDefaultAsync(th => th.TipoHabitacionId == id);

            if (tipoHabitacion == null)
            {
                return NotFound($"Tipo de habitación con ID {id} no encontrado");
            }

            // Verifica que no tenga habitaciones asignadas
            if (tipoHabitacion.Habitaciones.Any())
            {
                return BadRequest(new
                {
                    Message = "No se puede eliminar porque tiene habitaciones asignadas",
                    Habitaciones = tipoHabitacion.Habitaciones.Select(h => h.HabitacionId)
                }); // Error 400 si hay relaciones
            }

            try
            {
                // Elimina y guarda cambios
                _context.TiposHabitacion.Remove(tipoHabitacion);
                await _context.SaveChangesAsync();
                return NoContent(); // Retorna 204 (No Content) para éxito
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    Message = "Error al eliminar de la base de datos",
                    Error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        // GET: api/tiposhabitacion/test
        // Endpoint de prueba para verificar funcionamiento
        [HttpGet("test")]
        public ActionResult ProbarEndpoint()
        {
            return Ok(new
            {
                Estado = "Operativo",
                Controlador = "TiposHabitacion",
                Tabla = "tipo_habitacion",
                Estructura = new
                {
                    tipo_habitacion_id = "int (PK, identity)",
                    tipo = "varchar(50) not null",
                    descripcion = "varchar(150) null"
                },
                Fecha = DateTime.Now
            });
        }
    }

    // DTO para creación de tipos de habitación
    public class TipoHabitacionCreateDto
    {
        [Required(ErrorMessage = "El campo tipo es obligatorio")]
        [StringLength(50, ErrorMessage = "El tipo no puede exceder 50 caracteres")]
        public string Tipo { get; set; }

        [StringLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
        public string Descripcion { get; set; }
    }

    // DTO para actualización de tipos de habitación
    public class TipoHabitacionUpdateDto
    {
        [Required(ErrorMessage = "El tipo es requerido")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres para el tipo")]
        public string Tipo { get; set; }

        [StringLength(150, ErrorMessage = "Máximo 150 caracteres para la descripción")]
        public string Descripcion { get; set; }
    }
}