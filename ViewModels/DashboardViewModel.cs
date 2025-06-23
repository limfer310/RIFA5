// ViewModels/DashboardViewModel.cs
using System.Collections.Generic;
using RIFA.Models;

namespace RIFA.ViewModels
{
    public class DashboardViewModel
    {
        public string Mensaje { get; set; } = "Bienvenido al Dashboard";

        // Estadísticas generales de usuarios
        public int TotalUsuarios { get; set; }

        // Estadísticas de reciclaje - CAMBIADO A DOUBLE
        public double TotalMaterialesReciclados { get; set; } // Suma total de todos los materiales
        public double TotalBotellaPET { get; set; } // Cantidad específica para PET
        public double TotalTapas { get; set; }      // Cantidad específica para Tapas
        public double TotalHojasPapel { get; set; }  // Cantidad específica para Papel

        // Estadísticas de canjes - NOMBRES Y TIPOS CORRECTOS (ya eran int)
        public int TotalProductosCanjeados { get; set; } // Número de veces que se ha realizado un canje
        public int TotalPuntosCanjeadosGlobal { get; set; } // Total de puntos que se han canjeado en todos los canjes

        // Puedes añadir aquí otras propiedades si tu dashboard las necesita
    }
}