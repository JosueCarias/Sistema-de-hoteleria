using Microsoft.EntityFrameworkCore;
using hoteleria.Models;

namespace hoteleria.Data;

public class HoteleriaContext : DbContext
{
    public HoteleriaContext(DbContextOptions<HoteleriaContext> options) : base(options) { }
    
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Rol> Roles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}