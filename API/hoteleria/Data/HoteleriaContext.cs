using Microsoft.EntityFrameworkCore;
using hoteleria.Models;

namespace hoteleria.Data
{
    public class HoteleriaContext : DbContext
    {
        public HoteleriaContext(DbContextOptions<HoteleriaContext> options) 
            : base(options) 
        { 
        }
        
        public DbSet<Habitacion> Habitaciones { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Reservacion> Reservaciones { get; set; }
        public DbSet<Hotel> Hoteles { get; set; }
        public DbSet<TipoHabitacion> TipoHabitaciones { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuraciones adicionales si son necesarias
        }
    }
}