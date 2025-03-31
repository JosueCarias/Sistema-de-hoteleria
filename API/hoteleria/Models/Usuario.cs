using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hoteleria.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }
    
        public int RolId { get; set; }
        public Rol Rol { get; set; }
    }
}