using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hoteleria.Models
{
    public class Reservacion
    {
        public int ReservacionId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal Costo { get; set; }
    
        public long ClienteDpi { get; set; }
        public Cliente Cliente { get; set; }
    
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }
    
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}