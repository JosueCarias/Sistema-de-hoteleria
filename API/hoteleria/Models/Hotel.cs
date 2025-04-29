using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteleria.Models
{
    [Table("hotel")]  // Mapea exactamente con tu tabla SQL
    public class Hotel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("hotel_id")]
        public int HotelId { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        [StringLength(150)]
        public string? Descripcion { get; set; }

        [Required]
        [Column("ubicacion")]
        [StringLength(150)]
        public string Ubicacion { get; set; } = string.Empty;
    }
}