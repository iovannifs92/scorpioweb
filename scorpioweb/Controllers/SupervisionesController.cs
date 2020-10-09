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
    public class SupervisionesController : Controller
    {
        private readonly penas2Context _context;

        public SupervisionesController(penas2Context context)
        {
            _context = context;
        }

        #region -Index-
        public async Task<IActionResult> Index()
        {
            return View(await _context.Supervision.ToListAsync());
        }
        #endregion

        #region -Details-
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision
                .SingleOrDefaultAsync(m => m.IdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            return View(supervision);
        }

        #endregion

        #region -Create-
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSupervision,Inicio,Termino,EstadoSupervision,PersonaIdPersona,EstadoCumplimiento,CausaPenalIdCausaPenal")] Supervision supervision)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supervision);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supervision);
        }

        #endregion

        #region -Edit-
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision.SingleOrDefaultAsync(m => m.IdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }
            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSupervision,Inicio,Termino,EstadoSupervision,PersonaIdPersona,EstadoCumplimiento,CausaPenalIdCausaPenal")] Supervision supervision)
        {
            if (id != supervision.IdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supervision);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(supervision.IdSupervision))
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
            return View(supervision);
        }

        #endregion

        #region -Delete-
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision
                .SingleOrDefaultAsync(m => m.IdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            return View(supervision);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supervision = await _context.Supervision.SingleOrDefaultAsync(m => m.IdSupervision == id);
            _context.Supervision.Remove(supervision);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region -SupervisionExists-
        private bool SupervisionExists(int id)
        {
            return _context.Supervision.Any(e => e.IdSupervision == id);
        }

        #endregion

        #region -MenuSupervision-
        public IActionResult MenuSupervision()
        {
            return View();
        }
        #endregion

        #region -PersonaSupervicion-
        public async Task<IActionResult> PersonaSupervision(
           string sortOrder,
           string currentFilter,
           string searchString,
           int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";


            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            ViewData["CurrentFilter"] = searchString;

            var super = from s in _context.Supervision

                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                super = super.Where(s => s.EstadoSupervision.Contains(searchString)
                                        || s.EstadoCumplimiento.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    super = super.OrderByDescending(p => p.EstadoSupervision);
                    break;
                case "Date":
                    super = super.OrderBy(p => p.EstadoCumplimiento);
                    break;
            }


            int pageSize = 10;
            return View(await PaginatedList<Supervision>.CreateAsync(super.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        #endregion
        #region -Supervision-
        public async Task<IActionResult> Supervision(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision.SingleOrDefaultAsync(m => m.IdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }


            List<Supervision> SupervisionVM = _context.Supervision.ToList();
            List<Causapenal> causaPenalVM = _context.Causapenal.ToList();
            List<Persona> personaVM = _context.Persona.ToList();
            #region -Jointables-
            ViewData["joinTablesSupervision"] = from supervisiontable in SupervisionVM
                                          join  personatable in personaVM on supervisiontable.PersonaIdPersona equals personatable.IdPersona
                                          join causapenaltable in causaPenalVM on supervisiontable.CausaPenalIdCausaPenal equals causapenaltable.IdCausaPenal
                                          where supervisiontable.IdSupervision == id
                                 
                                          select new SupervisionPyCP
                                          {
                                              causapenalVM = causapenaltable,
                                              supervisionVM = supervisiontable,
                                              personaVM = personatable 
                                          };
            #endregion


            return View();
        }
        #endregion

        #region -Graficos-
        #endregion
    }
}
