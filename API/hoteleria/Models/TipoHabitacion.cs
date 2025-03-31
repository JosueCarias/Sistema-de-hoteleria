using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hoteleria.Models
{
    public class TipoHabitacion
    {
        public int TipoHabitacionId { get; set; }
        public string Tipo { get; set; }
        public string? Descripcion { get; set; }
        public List<Habitacion> Habitaciones { get; set; }
    }
}