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
    public class EmpleadosController : ControllerBase
    {
        private readonly HoteleriaContext _context;

        public EmpleadosController(HoteleriaContext context)
        {
            _context = context;
        }
        // Endpoint de prueba
        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok("El endpoint funciona - " + DateTime.Now);
        }

        // GET: api/empleados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Empleado>>> GetEmpleados()
        {
            return await _context.Empleados.Include(e => e.Hotel).ToListAsync();
        }

        // GET: api/empleados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Empleado>> GetEmpleado(int id)
        {
            var empleado = await _context.Empleados
                .Include(e => e.Hotel)
                .FirstOrDefaultAsync(e => e.EmpleadoId == id);

            if (empleado == null)
            {
                return NotFound();
            }

            return empleado;
        }

        // POST: api/empleados
        [HttpPost]
        public async Task<ActionResult<Empleado>> PostEmpleado([FromBody] EmpleadoCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar email único
            if (await _context.Empleados.AnyAsync(e => e.Email == dto.Email))
            {
                return Conflict("Ya existe un empleado con este email");
            }

            var empleado = new Empleado
            {
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Email = dto.Email,
                Telefono = dto.Telefono,
                FechaNacimiento = dto.FechaNacimiento,
                HotelId = dto.HotelId
            };

            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmpleado", new { id = empleado.EmpleadoId }, empleado);
        }

        // PUT: api/empleados/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpleado(int id, [FromBody] EmpleadoUpdateDto dto)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            // Validar email único si se está actualizando
            if (dto.Email != null && await _context.Empleados.AnyAsync(e => e.Email == dto.Email && e.EmpleadoId != id))
            {
                return Conflict("Ya existe otro empleado con este email");
            }

            // Actualizar solo los campos proporcionados
            if (dto.Nombres != null) empleado.Nombres = dto.Nombres;
            if (dto.Apellidos != null) empleado.Apellidos = dto.Apellidos;
            if (dto.Email != null) empleado.Email = dto.Email;
            if (dto.Telefono != null) empleado.Telefono = dto.Telefono;
            if (dto.FechaNacimiento.HasValue) empleado.FechaNacimiento = dto.FechaNacimiento.Value;
            if (dto.HotelId.HasValue) empleado.HotelId = dto.HotelId.Value;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpleadoExists(id))
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

        // DELETE: api/empleados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpleado(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null) return NotFound();

            _context.Empleados.Remove(empleado);

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return BadRequest("No se pudo eliminar (puede tener datos relacionados)");
            }
        }
        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.EmpleadoId == id);
        }
    }
    public class EmpleadoCreateDto
{
    [Required(ErrorMessage = "Los nombres son obligatorios")]
    [StringLength(50, ErrorMessage = "Los nombres no pueden exceder los 50 caracteres")]
    public string Nombres { get; set; }

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    [StringLength(50, ErrorMessage = "Los apellidos no pueden exceder los 50 caracteres")]
    public string Apellidos { get; set; }

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    [StringLength(100)]
    public string Email { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El teléfono debe tener 8 dígitos")]
    [RegularExpression(@"^[0-9]+$", ErrorMessage = "El teléfono solo puede contener números")]
    public string Telefono { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
    [DataType(DataType.Date)]
    public DateTime FechaNacimiento { get; set; }

    [Required(ErrorMessage = "El ID del hotel es obligatorio")]
    public int HotelId { get; set; }
}

    public class EmpleadoUpdateDto
    {
        [StringLength(50)]
        public string? Nombres { get; set; }

        [StringLength(50)]
        public string? Apellidos { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(8)]
        [RegularExpression(@"^[0-9]{8}$")]
        public string? Telefono { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }

        public int? HotelId { get; set; }
    }
}