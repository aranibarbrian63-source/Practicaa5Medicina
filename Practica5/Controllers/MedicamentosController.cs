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
    [Authorize]
    public class MedicamentosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicamentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Medicamentos con Filtros y Buscador
        public async Task<IActionResult> Index(string buscarNombre, int? filtrarCategoria)
        {
            IQueryable<Medicamento> consulta = _context.Medicamentos
                .Include(m => m.Categoria)
                .Include(m => m.Estante);

            if (!string.IsNullOrEmpty(buscarNombre))
            {
                consulta = consulta.Where(m => m.Nombre.Contains(buscarNombre));
            }

            if (filtrarCategoria.HasValue)
            {
                consulta = consulta.Where(m => m.CategoriaId == filtrarCategoria);
            }

            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nombre", filtrarCategoria);

            ViewData["FiltroNombre"] = buscarNombre;
            ViewData["FiltroCat"] = filtrarCategoria;

            return View(await consulta.OrderBy(m => m.Nombre).ToListAsync());
        }

        [Authorize(Roles = "Administrador,Farmacéutico")]
        public async Task<IActionResult> ReporteVencimientos()
        {
            DateTime fechaLimite = DateTime.Now.AddDays(30);

            var vencidos = await _context.Medicamentos
                .Include(m => m.Categoria)
                .Where(m => m.FechaVencimiento <= fechaLimite)
                .OrderBy(m => m.FechaVencimiento)
                .ToListAsync();

            return View(vencidos);
        }

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

        [Authorize(Roles = "Administrador,Farmacéutico")]
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nombre");
            ViewData["EstanteId"] = new SelectList(_context.Estantes, "Id", "Nombre");
            return View();
        }

        // POST: Medicamentos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Farmacéutico")]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Presentacion,Concentracion,Precio,Stock,FechaVencimiento,Descripcion,Estado,CategoriaId,EstanteId")] Medicamento medicamento)
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

        // POST: Medicamentos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Farmacéutico")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Presentacion,Concentracion,Precio,Stock,FechaVencimiento,Descripcion,Estado,CategoriaId,EstanteId")] Medicamento medicamento)
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
            if (medicamento != null)
            {
                _context.Medicamentos.Remove(medicamento);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MedicamentoExists(int id)
        {
            return _context.Medicamentos.Any(e => e.Id == id);
        }
    }
}