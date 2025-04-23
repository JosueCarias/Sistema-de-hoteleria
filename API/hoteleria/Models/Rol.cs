using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteleria.Models
{
    [Table("rol")]
    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("rol_id")]
        public int RolId { get; set; }

        [Required]
        [Column("tipo_rol")]
        [StringLength(50)]
        public string TipoRol { get; set; } = null!;
    }
}