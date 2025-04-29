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
    }
}