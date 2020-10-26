using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;

namespace scorpioweb.Controllers
{
    public class FraccionesimpuestasController : Controller
    {
        private readonly penas2Context _context;

        public FraccionesimpuestasController(penas2Context context)
        {
            _context = context;
        }

        // GET: Fraccionesimpuestas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Fraccionesimpuestas.ToListAsync());
        }

        // GET: Fraccionesimpuestas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fraccionesimpuestas = await _context.Fraccionesimpuestas
                .SingleOrDefaultAsync(m => m.IdFracciones == id);
            if (fraccionesimpuestas == null)
            {
                return NotFound();
            }

            return View(fraccionesimpuestas);
        }

        // GET: Fraccionesimpuestas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Fraccionesimpuestas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdFracciones,Tipo,Autoridad,FechaInicio,FechaTermino,Estado,Evidencia,FiguraJudicial,SupervisionIdSupervision")] Fraccionesimpuestas fraccionesimpuestas)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fraccionesimpuestas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fraccionesimpuestas);
        }

        // GET: Fraccionesimpuestas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fraccionesimpuestas = await _context.Fraccionesimpuestas.SingleOrDefaultAsync(m => m.IdFracciones == id);
            if (fraccionesimpuestas == null)
            {
                return NotFound();
            }
            return View(fraccionesimpuestas);
        }

        // POST: Fraccionesimpuestas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdFracciones,Tipo,Autoridad,FechaInicio,FechaTermino,Estado,Evidencia,FiguraJudicial,SupervisionIdSupervision")] Fraccionesimpuestas fraccionesimpuestas)
        {
            if (id != fraccionesimpuestas.IdFracciones)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fraccionesimpuestas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FraccionesimpuestasExists(fraccionesimpuestas.IdFracciones))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fraccionesimpuestas);
        }

        // GET: Fraccionesimpuestas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fraccionesimpuestas = await _context.Fraccionesimpuestas
                .SingleOrDefaultAsync(m => m.IdFracciones == id);
            if (fraccionesimpuestas == null)
            {
                return NotFound();
            }

            return View(fraccionesimpuestas);
        }

        // POST: Fraccionesimpuestas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fraccionesimpuestas = await _context.Fraccionesimpuestas.SingleOrDefaultAsync(m => m.IdFracciones == id);
            _context.Fraccionesimpuestas.Remove(fraccionesimpuestas);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FraccionesimpuestasExists(int id)
        {
            return _context.Fraccionesimpuestas.Any(e => e.IdFracciones == id);
        }
    }
}
