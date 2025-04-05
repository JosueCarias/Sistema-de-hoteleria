using Microsoft.EntityFrameworkCore;
using hoteleria.Models;

namespace hoteleria.Data;

public class HoteleriaContext : DbContext
{
    public HoteleriaContext(DbContextOptions<HoteleriaContext> options) : base(options) { }
    
    public DbSet<Hotel> Hoteles { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<TipoHabitacion> TiposHabitacion { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Habitacion> Habitaciones { get; set; }
    public DbSet<Reservacion> Reservaciones { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuración de relaciones
        modelBuilder.Entity<Empleado>()
            .HasOne(e => e.Hotel)
            .WithMany(h => h.Empleados)
            .HasForeignKey(e => e.HotelId);
            
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Empleado)
            .WithOne(e => e.Usuario)
            .HasForeignKey<Usuario>(u => u.EmpleadoId);
            
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RolId);
            
        modelBuilder.Entity<Habitacion>()
            .HasOne(h => h.Hotel)
            .WithMany(h => h.Habitaciones)
            .HasForeignKey(h => h.HotelId);
            
        modelBuilder.Entity<Habitacion>()
            .HasOne(h => h.TipoHabitacion)
            .WithMany(th => th.Habitaciones)
            .HasForeignKey(h => h.TipoHabitacionId);
            
        modelBuilder.Entity<Reservacion>()
            .HasOne(r => r.Cliente)
            .WithMany(c => c.Reservaciones)
            .HasForeignKey(r => r.ClienteDpi);
            
        modelBuilder.Entity<Reservacion>()
            .HasOne(r => r.Empleado)
            .WithMany(e => e.Reservaciones)
            .HasForeignKey(r => r.EmpleadoId);
            
        modelBuilder.Entity<Reservacion>()
            .HasOne(r => r.Hotel)
            .WithMany()
            .HasForeignKey(r => r.HotelId);
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<Usuario>()
        .HasOne(u => u.Rol)
        .WithMany(r => r.Usuarios)
        .HasForeignKey(u => u.RolId);
    }
}