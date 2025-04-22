using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace hoteleria.Models
{
    [Table("cliente")]
    public class Cliente
    {
        [Key]
        [Column("cliente_dpi")]
        public long ClienteId { get; set; }

        [Required]
        [Column("nombres")]
        [StringLength(50)]
        public string NombreCliente { get; set; }

        [Required]
        [Column("apellidos")]
        [StringLength(50)]
        public string ApellidoCliente { get; set; }

        [Required]
        [Column("email")]
        [EmailAddress]
        public string EmailCliente { get; set; }

        [Required]
        [Column("telefono")]
        [RegularExpression(@"^\d{8}$")]
        public string TelefonoCliente { get; set; }
    }
}