// ViewModels/ProductosViewModel.cs
using System.Collections.Generic;
using RIFA.Models;

namespace RIFA.ViewModels
{
    public class ProductosViewModel
    {
        public List<Producto> ProductosDisponibles { get; set; } // Lista de productos CANJEABLES
        public int PuntosUsuario { get; set; }
        public string? Message { get; set; } // Cambiado a nullable para consistencia
        public string? ErrorMessage { get; set; } // Cambiado a nullable para consistencia
    }
}