using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteleria.Models
{
    [Table("reservacion")]
    public class Reservacion
    {
        [Key]
        [Column("reservacion_id")]
        public int ReservacionId { get; set; }

        [Column("cliente_nombre")]
        [StringLength(100)]
        public string ClienteNombre { get; set; }

        [Column("empleado_nombre")]
        [StringLength(100)]
        public string EmpleadoNombre { get; set; }

        [Column("hotel_nombre")]
        [StringLength(100)]
        public string HotelNombre { get; set; }

        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateTime FechaFin { get; set; }

        [Column("costo")]
        public decimal Costo { get; set; }
    }
}