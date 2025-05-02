using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace hoteleria.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(25)]
        [Column("username")]
        public string Username { get; set; }

        [Required]
        [StringLength(50)]
        [Column("password")]
        public string Password { get; set; }

        [ForeignKey("empleado_id")]
        [Column("empleado_id")]
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }

        [ForeignKey("rol_id")]
        [Column("rol_id")]
        public int RolId { get; set; }
        public Rol Rol { get; set; }
    }
}