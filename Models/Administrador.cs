// Models/Administrador.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Importante para [DatabaseGenerated]
using System.Collections.Generic; // Para colecciones de Registros

namespace RIFA.Models
{
    public class Administrador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // CORRECCIÓN CLAVE: Indica que la DB generará el valor
        public int AdministradorId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)] // O el tamaño adecuado para tu hash de contraseña
        public string Contrasena { get; set; }

        // Propiedad de navegación
        public ICollection<Registro>? Registros { get; set; }
    }
}