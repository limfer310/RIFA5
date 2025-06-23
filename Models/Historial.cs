// Models/Historial.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RIFA.Models
{
    public class Historial
    {
        [Key]
        public int HistorialId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int MaterialRecicladoId { get; set; }

        [Required]
        public double Cantidad { get; set; } // Asegúrate de que sea double si manejas decimales

        [Required]
        public int PuntosGanados { get; set; }

        public DateTime FechaRegistro { get; set; }

        // ¡AÑADIR ESTA PROPIEDAD SI LA NECESITAS EN EL MODELO!
        // Si TipoMaterial está aquí, es porque quieres guardarlo directamente en el historial,
        // además de la relación con MaterialReciclado.
        [Required]
        [StringLength(50)]
        public string TipoMaterial { get; set; } = null!;


        // Propiedades de navegación
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; } // Nullable si no siempre se carga

        [ForeignKey("MaterialRecicladoId")]
        public MaterialReciclado? MaterialReciclado { get; set; } // Nullable si no siempre se carga
    }
}