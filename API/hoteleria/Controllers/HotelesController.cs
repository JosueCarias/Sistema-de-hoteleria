using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hoteleria.Data;
using hoteleria.Models;

namespace hoteleria.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelesController : ControllerBase
{
    private readonly HoteleriaContext _context;

    public HotelesController(HoteleriaContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Hotel>>> GetHoteles()
    {
        return await _context.Hoteles.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Hotel>> GetHotel(int id)
    {
        var hotel = await _context.Hoteles.FindAsync(id);

        if (hotel == null)
        {
            return NotFound();
        }

        return hotel;
    }

    [HttpPost]
    public async Task<ActionResult<Hotel>> PostHotel(Hotel hotel)
    {
        _context.Hoteles.Add(hotel);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetHotel", new { id = hotel.HotelId }, hotel);
    }
}