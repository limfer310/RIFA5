using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Para las listas de navegación

namespace RIFA.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [StringLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Contrasena { get; set; } // Contraseña HASHED

        [Required(ErrorMessage = "El número de boleta es obligatorio.")]
        [StringLength(20, ErrorMessage = "El número de boleta no puede exceder los 20 caracteres.")]
        public string NumeroBoleta { get; set; }

        public int PuntosAcumulados { get; set; } = 0; // Puntos iniciales

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [StringLength(50)]
        public string Rol { get; set; } = "Usuario"; // "Usuario" o "Administrador"

        // Propiedades de navegación (si las usas en tu DbContext)
        public ICollection<Historial> Historials { get; set; }
        public ICollection<Canje> Canjes { get; set; }
    }
}