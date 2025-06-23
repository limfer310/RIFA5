using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace RIFA.Models
{
    public class Canje
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CanjeId { get; set; }

        [Required]
        public int UsuarioId { get; set; } // Clave foránea al Usuario

        [Required]
        public int ProductoId { get; set; } // Clave foránea al Producto canjeado

        [Required(ErrorMessage = "Los puntos canjeados son obligatorios.")]
        [Range(0, int.MaxValue, ErrorMessage = "Los puntos deben ser un número no negativo.")]
        public int PuntosCanjeados { get; set; }

        [Required]
        public DateTime FechaCanje { get; set; }

        // Propiedades de navegación
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }
    }
}