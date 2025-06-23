namespace RIFA.ViewModels
{

    // Asegúrate de que PublicUserViewModel esté definido
    // Este ViewModel está correcto y es usado por AlbaViewModel.
    public class PublicUserViewModel
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = null!; // Añadido = null!
        public string Email { get; set; } = null!; // Añadido = null!
        public int PuntosAcumulados { get; set; }
        public string Rol { get; set; } = null!; // Añadido = null!
        public string? NumeroBoleta { get; set; }
    }
}