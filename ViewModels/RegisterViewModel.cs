// ViewModels/RegisterViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace RIFA.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo electrónico inválido.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder los 100 caracteres.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Contrasena", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmarContrasena { get; set; } = null!;

        [Required(ErrorMessage = "El número de boleta es obligatorio.")]
        [StringLength(50, ErrorMessage = "El número de boleta no puede exceder los 50 caracteres.")]
        public string NumeroBoleta { get; set; } = null!;

        [Display(Name = "Rol del Usuario")]
        public string Rol { get; set; } = "Usuario"; // Valor por defecto
    }
}