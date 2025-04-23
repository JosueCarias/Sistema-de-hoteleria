using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteleria.Models
{
    [Table("cliente")]
    public class Cliente
    {
        [Key]
        [Column("cliente_dpi")]
        public long ClienteDPI { get; set; }

        [Required]
        [Column("nombres")]
        [StringLength(50)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [Column("apellidos")]
        [StringLength(50)]
        public string Apellidos { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("telefono")]
        [StringLength(8)]
        [RegularExpression(@"^[0-9]{8}$", ErrorMessage = "El teléfono debe tener 8 dígitos")]
        public string Telefono { get; set; } = string.Empty;
    }
}