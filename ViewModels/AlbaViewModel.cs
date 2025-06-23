// ViewModels/AlbaViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RIFA.Models; // Si PublicUserViewModel es un modelo directamente

namespace RIFA.ViewModels
{
    public class AlbaViewModel
    {
        // Para mostrar la lista de usuarios
        public List<PublicUserViewModel>? Usuarios { get; set; }

        // Para el formulario de "Alta"
        public RegisterViewModel NuevoUsuario { get; set; } = new RegisterViewModel(); // Inicializar para evitar NullReferenceException



        // Para el formulario de "Baja"
        [Display(Name = "Nombre de Usuario")]
        public string? NombreBaja { get; set; }

        [Display(Name = "Número de Boleta")]
        public string? NumeroBoletaBaja { get; set; }

        // --- NUEVO: Para cambiar el rol de un usuario existente ---
        [Display(Name = "ID de Usuario")]
        public int UserIdToChangeRole { get; set; } // ID del usuario cuyo rol se va a cambiar

        [Display(Name = "Nuevo Rol")]
        [Required(ErrorMessage = "El nuevo rol es obligatorio.")]
        public string NewRole { get; set; } = "Usuario"; // El nuevo rol para asignar (ej. "Usuario", "Administrador")

        // Para la edición de usuarios (si decides implementarla, necesitas más propiedades)
        public PublicUserViewModel? UsuarioParaEditar { get; set; }

        // Para controlar la pestaña activa en la vista (útil para JavaScript/HTML)
        public string ActiveTab { get; set; } = "lista"; // Valor por defecto, cambié a "lista" para que sea la primera visible.

        // Mensajes para la vista
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Mensaje { get; set; } // Mensaje general del panel

      



    }
}