using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Practica5.Models; // <--- ESTA LÍNEA ES LA QUE SOLUCIONA EL ERROR

namespace Practica5.Data
{
    // Al agregar el using de arriba, ya reconocerá a ApplicationUser y Medicamento
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Medicamento> Medicamentos { get; set; }
    }
}