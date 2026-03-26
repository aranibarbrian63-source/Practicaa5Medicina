using System.ComponentModel.DataAnnotations;

namespace Practica5.Models
{
    public class Estante
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del estante es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es obligatoria")]
        public string Ubicacion { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        // Relación: Un estante contiene muchos medicamentos
        public virtual ICollection<Medicamento>? Medicamentos { get; set; }
    }
}