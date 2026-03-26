using System.ComponentModel.DataAnnotations;

namespace Practica5.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        // Relación: Una categoría tiene muchos medicamentos
        public virtual ICollection<Medicamento>? Medicamentos { get; set; }
    }
}