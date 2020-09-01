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
    public class CausapenalController : Controller
    {
        private readonly penas2Context _context;

        public CausapenalController(penas2Context context)
        {
            _context = context;
        }

        // GET: Causapenal
        public async Task<IActionResult> Index()
        {
            return View(await _context.Causapenal.ToListAsync());
        }

        // GET: Causapenal/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var causapenal = await _context.Causapenal
                .SingleOrDefaultAsync(m => m.IdCausaPenal == id);
            if (causapenal == null)
            {
                return NotFound();
            }

            return View(causapenal);
        }

        // GET: Causapenal/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Causapenal/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCausaPenal,Cnpp,Juez,Cambio,CausaPenal")] Causapenal causapenal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(causapenal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(causapenal);
        }

        // GET: Causapenal/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var causapenal = await _context.Causapenal.SingleOrDefaultAsync(m => m.IdCausaPenal == id);
            if (causapenal == null)
            {
                return NotFound();
            }
            return View(causapenal);
        }

        // POST: Causapenal/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCausaPenal,Cnpp,Juez,Cambio,CausaPenal")] Causapenal causapenal)
        {
            if (id != causapenal.IdCausaPenal)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(causapenal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CausapenalExists(causapenal.IdCausaPenal))
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
            return View(causapenal);
        }

        // GET: Causapenal/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var causapenal = await _context.Causapenal
                .SingleOrDefaultAsync(m => m.IdCausaPenal == id);
            if (causapenal == null)
            {
                return NotFound();
            }

            return View(causapenal);
        }

        // POST: Causapenal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var causapenal = await _context.Causapenal.SingleOrDefaultAsync(m => m.IdCausaPenal == id);
            _context.Causapenal.Remove(causapenal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CausapenalExists(int id)
        {
            return _context.Causapenal.Any(e => e.IdCausaPenal == id);
        }
    }
}
