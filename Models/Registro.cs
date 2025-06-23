// Models/Registro.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RIFA.Models
{
    public class Registro
    {
        [Key]
        public int RegistroId { get; set; }

        [Required]
        public int AdministradorId { get; set; }

        [Required]
        public int MaterialRecicladoId { get; set; }

        // Propiedad que ya habíamos añadido
        [StringLength(255)]
        public string? Descripcion { get; set; }

        // ¡NUEVA PROPIEDAD! Para la cantidad registrada
        [Required] // Si la cantidad es siempre obligatoria en un registro
        public double CantidadRegistrada { get; set; } // O int si solo manejas cantidades enteras de unidades

        public DateTime Fecha { get; set; }

        // Propiedades de navegación
        [ForeignKey("AdministradorId")]
        public Administrador? Administrador { get; set; }

        [ForeignKey("MaterialRecicladoId")]
        public MaterialReciclado? MaterialReciclado { get; set; }
    }
}