using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hoteleria.Models
{
    public class Hotel
    {
        public int HotelId { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public string Ubicacion { get; set; }
        public List<Empleado> Empleados { get; set; }
        public List<Habitacion> Habitaciones { get; set; }        
    }
}