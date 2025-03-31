using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hoteleria.Models
{
    public class Habitacion
    {
        public int HabitacionId { get; set; }
        public string Disponibilidad { get; set; } // "Sí" o "No"
    
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    
        public int TipoHabitacionId { get; set; }
        public TipoHabitacion TipoHabitacion { get; set; }
    }
}