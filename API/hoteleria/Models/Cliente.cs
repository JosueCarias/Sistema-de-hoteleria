using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoteleria.Models
{
    public class Cliente
    {
        [Key] // Marca el DPI como clave primaria
        [Required(ErrorMessage = "El DPI es obligatorio")]
        public long ClienteDpi { get; set; }

        [Required(ErrorMessage = "Los nombres son obligatorios")]
        [StringLength(50, ErrorMessage = "Los nombres no pueden exceder 50 caracteres")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(50, ErrorMessage = "Los apellidos no pueden exceder 50 caracteres")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El teléfono debe tener 8 dígitos")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "El teléfono solo debe contener números")]
        public string Telefono { get; set; } = string.Empty;

        // Propiedad de navegación (no requiere atributos)
        public List<Reservacion> Reservaciones { get; set; } = new List<Reservacion>();
    }
}