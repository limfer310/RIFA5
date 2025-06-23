// ViewModels/LoginViewModel.cs
using System.ComponentModel.DataAnnotations; // Asegúrate de tener este using

namespace RIFA.ViewModels // O el namespace que estés usando
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Por favor, introduce una dirección de correo electrónico válida.")]
        [Display(Name = "Correo Electrónico")] // Esto es opcional, para mostrar un nombre amigable en la vista
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        [Display(Name = "¿Recordarme?")]
        public bool RememberMe { get; set; }

        public bool IsAdminLogin { get; set; } // Campo para distinguir login de admin
    }
}