using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hoteleria.Data;
using hoteleria.Models;

namespace hoteleria.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly HoteleriaContext _context;

    public ClientesController(HoteleriaContext context)
    {
        _context = context;
    }

    // GET: api/clientes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
    {
        return await _context.Clientes.ToListAsync();
    }

    // GET: api/clientes/1234567890101
    [HttpGet("{dpi}")]
    public async Task<ActionResult<Cliente>> GetCliente(long dpi)
    {
        var cliente = await _context.Clientes.FindAsync(dpi);
        if (cliente == null) return NotFound();
        return cliente;
    }

    // POST: api/clientes
    [HttpPost]
    public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCliente), new { dpi = cliente.ClienteDpi }, cliente);
    }
}