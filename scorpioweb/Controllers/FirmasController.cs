using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;
using Microsoft.AspNetCore.Authorization;

namespace scorpioweb.Controllers
{
    public class FirmasController : Controller
    {
        private readonly penas2Context _context;

        public FirmasController(penas2Context context)
        {
            _context = context;
        }

        SelectList listaGenero = new SelectList(
                                    new List<SelectListItem>
                                    {
                                        new SelectListItem { Text = "HOMBRE", Value = "H" },
                                        new SelectListItem { Text = "MUJER", Value = "M"},
                                    }, "Value", "Text");
        SelectList listaLibro = new SelectList(
                                    new List<SelectListItem>
                                    {
                                        new SelectListItem { Text = "Suspensión Condicional del Proceso", Value = "SCP" },
                                        new SelectListItem { Text = "Medida Cautelar", Value = "MC"},
                                        new SelectListItem { Text = "Condición en Libertad", Value = "CL"},
                                        new SelectListItem { Text = "Juzgado 1", Value = "J1"},
                                        new SelectListItem { Text = "Juzgado 2", Value = "J2"},
                                        new SelectListItem { Text = "Juzgado 3", Value = "J3"},
                                    }, "Value", "Text");
        // GET: Firmas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Firmas.ToListAsync());
        }

        // GET: Firmas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var firmas = await _context.Firmas
                .SingleOrDefaultAsync(m => m.Idfirmas == id);
            if (firmas == null)
            {
                return NotFound();
            }

            return View(firmas);
        }

        // GET: Firmas/Create
        //[Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewBag.genero = listaGenero;
            ViewBag.libros = listaLibro;
            return View();
        }

        // POST: Firmas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idfirmas,Nombre,Fecha,Sexo,Libro, Codigo")] Firmas firmas)
        {
            if (ModelState.IsValid)
            {
                firmas.Fecha = DateTime.Now;
                firmas.Nombre = (firmas.Nombre).ToUpper();
                _context.Add(firmas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            return View(firmas);
        }

        public IActionResult GeneraQR(string nombre, string sexo, string libro)
        {
            ViewBag.nombre = nombre.ToUpper();
            
            foreach(SelectListItem item in listaGenero.Items)
            {
                if (item.Value == sexo)
                {
                    
                    item.Selected = true;
                    break;
                }
            }
            ViewBag.genero = listaGenero;

            string lib = "";
            foreach (SelectListItem item in listaLibro.Items)
            {
                if (item.Value == libro)
                {
                    lib = (item.Value).ToString();
                    item.Selected = true;
                    break;
                }
            }
            ViewBag.libros = listaLibro;

            //var count = (from table in _context.Firmas                         
            //             select table).Count();
            var count = 1;

            char[] nom = (nombre.ToUpper()).ToCharArray();
            String codigo = lib +""+ nom[0] +""+ (count + 1) + "-" + Convert.ToInt32(DateTime.Now.ToString("ddMM"));
            ViewBag.code = codigo;

            return View("Create");
        }

        // GET: Firmas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var firmas = await _context.Firmas.SingleOrDefaultAsync(m => m.Idfirmas == id);
            if (firmas == null)
            {
                return NotFound();
            }
            return View(firmas);
        }

        // POST: Firmas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idfirmas,Nombre,Fecha,Sexo,Libro")] Firmas firmas)
        {
            if (id != firmas.Idfirmas)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(firmas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FirmasExists(firmas.Idfirmas))
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
            return View(firmas);
        }

        // GET: Firmas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var firmas = await _context.Firmas
                .SingleOrDefaultAsync(m => m.Idfirmas == id);
            if (firmas == null)
            {
                return NotFound();
            }

            return View(firmas);
        }

        // POST: Firmas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var firmas = await _context.Firmas.SingleOrDefaultAsync(m => m.Idfirmas == id);
            _context.Firmas.Remove(firmas);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FirmasExists(int id)
        {
            return _context.Firmas.Any(e => e.Idfirmas == id);
        }
    }
}
