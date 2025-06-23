// Models/MaterialReciclado.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RIFA.Models
{
    public class MaterialReciclado
    {
        [Key]
        public int MaterialRecicladoId { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoMaterial { get; set; } = null!;

        [Required]
        public int PuntosPorUnidad { get; set; }

        // ¡AÑADIR ESTAS PROPIEDADES!
        [StringLength(500)]
        public string? Descripcion { get; set; }

        [StringLength(500)]
        public string? ImagenUrl { get; set; }

        public ICollection<Historial> Historials { get; set; } = new List<Historial>();
        public ICollection<Registro> Registros { get; set; } = new List<Registro>();
    }
}