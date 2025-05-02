using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteleria.Models
{
    [Table("habitacion")]
    public class Habitacion
    {
        [Key]
        [Column("habitacion_id")]
        public int HabitacionId { get; set; }
    
        [Required]
        [Column("disponibilidad")]
        [StringLength(2)]
        [RegularExpression(@"^(Sí|No)$", ErrorMessage = "Disponibilidad debe ser 'Sí' o 'No'")]
        public string Disponibilidad { get; set; } = "Sí";
    
        [Required]
        [Column("hotel_id")]
        public int HotelId { get; set; }  // Sin ForeignKey
    
        [Required]
        [Column("tipo_habitacion_id")]
        public int TipoHabitacionId { get; set; }  // Sin ForeignKey
    }
}
