using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteleria.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Required]
        [Column("username")]
        [StringLength(25)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Column("password")]
        public string Password { get; set; } = string.Empty;

        [ForeignKey("Rol")]
        [Column("rol_id")]
        public int RolId { get; set; }

        [ForeignKey("Empleado")]
        [Column("empleado_id")]
        public int EmpleadoId { get; set; }

        // Propiedades de navegación
        public virtual Rol Rol { get; set; }
        public virtual Empleado Empleado { get; set; }
    }
}