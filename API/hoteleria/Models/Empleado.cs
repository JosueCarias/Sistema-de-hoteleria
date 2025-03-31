using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hoteleria.Models
{
    public class Empleado
    {
        public int EmpleadoId { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaNacimiento { get; set; }
    
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    
        public Usuario Usuario { get; set; }
        public List<Reservacion> Reservaciones { get; set; }
    }
}