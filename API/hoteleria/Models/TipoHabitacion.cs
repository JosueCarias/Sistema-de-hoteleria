using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteleria.Models
{
    [Table("tipo_habitacion")]  
    public class TipoHabitacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("tipo_habitacion_id")]
        public int TipoHabitacionId { get; set; }

        [Required]
        [Column("tipo")]
        [StringLength(50, ErrorMessage = "El tipo no puede exceder 50 caracteres")]
        public string Tipo { get; set; } = string.Empty;

        [Column("descripcion")]
        [StringLength(150, ErrorMessage = "La descripción no puede exceder 150 caracteres")]
        public string? Descripcion { get; set; }
    }
}