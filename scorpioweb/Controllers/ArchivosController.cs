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
    public class ArchivosController : Controller
    {
        private readonly penas2Context _context;

        public ArchivosController(penas2Context context)
        {
            _context = context;
        }

        // GET: Archivos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Archivo.ToListAsync());
        }

        // GET: Archivos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivo = await _context.Archivo
                .SingleOrDefaultAsync(m => m.IdArchivo == id);
            if (archivo == null)
            {
                return NotFound();
            }

            return View(archivo);
        }

        // GET: Archivos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Archivos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdArchivo,NoArchivo,Nombre,CausaPenal,Delito,Sentencia,Situacion,FechaAcuerdo,CarpetaEjecucion,Observaciones,Envia,Prestado,AreaPrestamo,FechaPrestamo")] Archivo archivo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(archivo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(archivo);
        }

        // GET: Archivos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivo = await _context.Archivo.SingleOrDefaultAsync(m => m.IdArchivo == id);
            if (archivo == null)
            {
                return NotFound();
            }
            return View(archivo);
        }

        // POST: Archivos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdArchivo,NoArchivo,Nombre,CausaPenal,Delito,Sentencia,Situacion,FechaAcuerdo,CarpetaEjecucion,Observaciones,Envia,Prestado,AreaPrestamo,FechaPrestamo")] Archivo archivo)
        {
            if (id != archivo.IdArchivo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(archivo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArchivoExists(archivo.IdArchivo))
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
            return View(archivo);
        }

        // GET: Archivos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivo = await _context.Archivo
                .SingleOrDefaultAsync(m => m.IdArchivo == id);
            if (archivo == null)
            {
                return NotFound();
            }

            return View(archivo);
        }

        // POST: Archivos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var archivo = await _context.Archivo.SingleOrDefaultAsync(m => m.IdArchivo == id);
            _context.Archivo.Remove(archivo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArchivoExists(int id)
        {
            return _context.Archivo.Any(e => e.IdArchivo == id);
        }
    }
}
