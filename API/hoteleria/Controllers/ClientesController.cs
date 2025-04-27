using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hoteleria.Models;
using hoteleria.Data;
using System.ComponentModel.DataAnnotations;

namespace hoteleria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly HoteleriaContext _context;

        public ClientesController(HoteleriaContext context)
        {
            _context = context;
        }
        // Endpoint de prueba
        [HttpGet("test")]
        public ActionResult Test()
        {
            return Ok("El endpoint funciona - " + DateTime.Now);
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _context.Clientes.ToListAsync();
        }

        // GET: api/clientes/1234567890123 (DPI)
        [HttpGet("{dpi}")]
        public async Task<ActionResult<Cliente>> GetCliente(long dpi)
        {
            var cliente = await _context.Clientes.FindAsync(dpi);
            if (cliente == null) return NotFound();
            return cliente;
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(ClienteCreateDto clienteDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Validar DPI único
            if (await _context.Clientes.AnyAsync(c => c.ClienteDPI == clienteDto.ClienteDPI))
                return Conflict("Ya existe un cliente con este DPI");

            var cliente = new Cliente
            {
                ClienteDPI = clienteDto.ClienteDPI,
                Nombres = clienteDto.Nombres,
                Apellidos = clienteDto.Apellidos,
                Email = clienteDto.Email,
                Telefono = clienteDto.Telefono
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { dpi = cliente.ClienteDPI }, cliente);
        }

        // PUT: api/clientes/1234567890123
        [HttpPut("{dpi}")]
        public async Task<IActionResult> PutCliente(long dpi, ClienteUpdateDto clienteDto)
        {
            var cliente = await _context.Clientes.FindAsync(dpi);
            if (cliente == null) return NotFound();

            // Actualizar solo campos proporcionados
            if (clienteDto.Nombres != null) cliente.Nombres = clienteDto.Nombres;
            if (clienteDto.Apellidos != null) cliente.Apellidos = clienteDto.Apellidos;
            if (clienteDto.Email != null) cliente.Email = clienteDto.Email;
            if (clienteDto.Telefono != null) cliente.Telefono = clienteDto.Telefono;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/clientes/1234567890123
        [HttpDelete("{dpi}")]
        public async Task<IActionResult> DeleteCliente(long dpi)
        {
            var cliente = await _context.Clientes.FindAsync(dpi);
            if (cliente == null) return NotFound();

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
    public class ClienteCreateDto
    {
        [Required(ErrorMessage = "El DPI es obligatorio")]
        public long ClienteDPI { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^[0-9]{8}$", ErrorMessage = "Debe tener 8 dígitos")]
        public string Telefono { get; set; } = string.Empty;
    }
    public class ClienteUpdateDto
    {
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string? Nombres { get; set; }

        [StringLength(50, ErrorMessage = "Máximo 50 caracteres")]
        public string? Apellidos { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string? Email { get; set; }

        [RegularExpression(@"^[0-9]{8}$", ErrorMessage = "Debe tener 8 dígitos")]
        public string? Telefono { get; set; }
    }
}
