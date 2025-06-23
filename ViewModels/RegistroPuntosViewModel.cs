// ViewModels/RegistroPuntosViewModel.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Necesario para SelectListItem
using System.Collections.Generic; // Necesario para List

namespace RIFA.ViewModels
{
    public class RegistroPuntosViewModel
    {
        [Required(ErrorMessage = "El número de boleta es obligatorio.")]
        [StringLength(50, ErrorMessage = "El número de boleta no puede exceder los 50 caracteres.")]
        [Display(Name = "Número de Boleta")]
        public string NumeroBoleta { get; set; } = null!;

        [Required(ErrorMessage = "Debes seleccionar un tipo de material.")]
        [Display(Name = "Tipo de Material")] // Añadido Display para la etiqueta en la vista
        public string TipoMaterial { get; set; } = null!;

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(0.01, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")] // Cambiado a double.MaxValue y 0.01
        [Display(Name = "Cantidad (unidades o kg)")] // Ajusta el nombre según lo que represente
        public int Cantidad { get; set; } // <-- CAMBIO AQUÍ: Ahora es double

        // Propiedad para la lista desplegable de materiales
        public List<SelectListItem>? MaterialesDisponibles { get; set; }
    }
}