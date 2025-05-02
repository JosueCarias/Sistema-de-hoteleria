using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteleria.Models
{
    [Table("empleado")]
    public class Empleado
    {
        [Key]
        [Column("empleado_id")]
        public int EmpleadoId { get; set; }

        [Required]
        [Column("nombres")]
        [StringLength(50)]
        public string Nombres { get; set; }

        [Required]
        [Column("apellidos")]
        [StringLength(50)]
        public string Apellidos { get; set; }

        [Required]
        [Column("email")]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Column("telefono")]
        [StringLength(8)]
        [RegularExpression(@"^[0-9]{8}$", ErrorMessage = "El teléfono debe tener 8 dígitos")]
        public string Telefono { get; set; }

        [Required]
        [Column("fecha_nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [Column("hotel_id")]
        public int HotelId { get; set; }

        [ForeignKey("HotelId")]
        public virtual Hotel Hotel { get; set; }
    }
}