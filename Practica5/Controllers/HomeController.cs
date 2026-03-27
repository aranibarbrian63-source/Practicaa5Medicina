using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practica5.Data;
using Practica5.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Practica5.Controllers
{
    [Authorize] // Protege el acceso, solo usuarios logueados pueden entrar
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inyectamos el contexto de la base de datos
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Solo si el usuario tiene el rol de Administrador calculamos las estadísticas
            if (User.IsInRole("Administrador"))
            {
                var hoy = DateTime.Now;
                var proximoVencer = hoy.AddDays(30);

                // 1. Total de medicamentos
                ViewBag.TotalMedicamentos = await _context.Medicamentos.CountAsync();

                // 2. Medicamentos vencidos (fecha menor a hoy)
                ViewBag.Vencidos = await _context.Medicamentos.CountAsync(m => m.FechaVencimiento < hoy);

                // 3. Medicamentos por vencer (próximos 30 días)
                ViewBag.PorVencer = await _context.Medicamentos.CountAsync(m => m.FechaVencimiento >= hoy && m.FechaVencimiento <= proximoVencer);

                // 4. Productos con bajo stock (ejemplo: menos de 5 unidades)
                ViewBag.BajoStock = await _context.Medicamentos.CountAsync(m => m.Stock < 5);

                // 5. Totales de categorías y estantes
                ViewBag.TotalCategorias = await _context.Categorias.CountAsync();
                ViewBag.TotalEstantes = await _context.Estantes.CountAsync();
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}