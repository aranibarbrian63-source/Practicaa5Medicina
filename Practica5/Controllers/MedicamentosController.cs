using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Practica5.Data;
using Practica5.Models;
using Microsoft.AspNetCore.Authorization;

namespace Practica5.Controllers
{
    [Authorize] // Bloquea el acceso a usuarios no logueados
    public class MedicamentosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicamentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Medicamentos con Filtros
        public async Task<IActionResult> Index(string buscarNombre, int? filtrarCategoria)
        {
            // Cargamos los medicamentos incluyendo sus relaciones
            IQueryable<Medicamento> consulta = _context.Medicamentos
                .Include(m => m.Categoria)
                .Include(m => m.Estante);

            // Filtro por Nombre
            if (!string.IsNullOrEmpty(buscarNombre))
            {
                consulta = consulta.Where(m => m.Nombre.Contains(buscarNombre));
            }

            // Filtro por Categoría
            if (filtrarCategoria.HasValue)
            {
                consulta = consulta.Where(m => m.CategoriaId == filtrarCategoria);
            }

            // Datos para los Dropdowns del filtro en la vista
            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre");

            return View(await consulta.ToListAsync());
        }

        // GET: Medicamentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var medicamento = await _context.Medicamentos
                .Include(m => m.Categoria)
                .Include(m => m.Estante)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicamento == null) return NotFound();

            return View(medicamento);
        }

        // Solo Administrador y Farmacéutico pueden CREAR
        [Authorize(Roles = "Administrador,Farmacéutico")]
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre");
            ViewData["EstanteId"] = new SelectList(_context.Estantes, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Farmacéutico")]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Precio,Stock,FechaVencimiento,Descripcion,Estado,CategoriaId,EstanteId")] Medicamento medicamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicamento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", medicamento.CategoriaId);
            ViewData["EstanteId"] = new SelectList(_context.Estantes, "Id", "Nombre", medicamento.EstanteId);
            return View(medicamento);
        }

        // Solo Administrador y Farmacéutico pueden EDITAR
        [Authorize(Roles = "Administrador,Farmacéutico")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento == null) return NotFound();

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", medicamento.CategoriaId);
            ViewData["EstanteId"] = new SelectList(_context.Estantes, "Id", "Nombre", medicamento.EstanteId);
            return View(medicamento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Farmacéutico")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Precio,Stock,FechaVencimiento,Descripcion,Estado,CategoriaId,EstanteId")] Medicamento medicamento)
        {
            if (id != medicamento.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicamentoExists(medicamento.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre", medicamento.CategoriaId);
            ViewData["EstanteId"] = new SelectList(_context.Estantes, "Id", "Nombre", medicamento.EstanteId);
            return View(medicamento);
        }

        // Solo ADMINISTRADOR puede ELIMINAR
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var medicamento = await _context.Medicamentos
                .Include(m => m.Categoria)
                .Include(m => m.Estante)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicamento == null) return NotFound();

            return View(medicamento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicamento = await _context.Medicamentos.FindAsync(id);
            if (medicamento != null) _context.Medicamentos.Remove(medicamento);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicamentoExists(int id)
        {
            return _context.Medicamentos.Any(e => e.Id == id);
        }
    }
}