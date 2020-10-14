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
    public class SupervisionPyCPsController : Controller
    {
        private readonly penas2Context _context;

        public SupervisionPyCPsController(penas2Context context)
        {
            _context = context;
        }

        // GET: SupervisionPyCPs
        public async Task<IActionResult> Index(string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CausaPenalSortParm"] = String.IsNullOrEmpty(sortOrder) ? "causa_penal_desc" : "";
            ViewData["EstadoCumplimientoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "estado_cumplimiento_desc" : "";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            ViewData["CurrentFilter"] = searchString;

            var filter = from p in _context.Persona
                         join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona
                         join cp in _context.Causapenal on s.CausaPenalIdCausaPenal equals cp.IdCausaPenal
                         where p.Supervisor == User.Identity.Name
                         select new SupervisionPyCP
                         {
                             personaVM = p,
                             supervisionVM = s,
                             causapenalVM = cp
                        };

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = filter.Where(spcp => spcp.personaVM.Paterno.Contains(searchString) ||
                                              spcp.personaVM.Materno.Contains(searchString) ||
                                              spcp.personaVM.Nombre.Contains(searchString) ||
                                              spcp.causapenalVM.CausaPenal.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(spcp => spcp.personaVM.Paterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(spcp => spcp.causapenalVM.CausaPenal);
                    break;
                case "estado_cumplimiento_desc":
                    filter = filter.OrderByDescending(spcp => spcp.supervisionVM.EstadoCumplimiento);
                    break;
                default:
                    filter = filter.OrderBy(spcp => spcp.personaVM.Paterno);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<SupervisionPyCP>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: SupervisionPyCPs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervisionPyCP = await _context.SupervisionPyCP
                .SingleOrDefaultAsync(m => m.IdSupervisionPyCP == id);
            if (supervisionPyCP == null)
            {
                return NotFound();
            }

            return View(supervisionPyCP);
        }

        // GET: SupervisionPyCPs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SupervisionPyCPs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSupervisionPyCP")] SupervisionPyCP supervisionPyCP)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supervisionPyCP);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supervisionPyCP);
        }

        // GET: SupervisionPyCPs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervisionPyCP = await _context.SupervisionPyCP.SingleOrDefaultAsync(m => m.IdSupervisionPyCP == id);
            if (supervisionPyCP == null)
            {
                return NotFound();
            }
            return View(supervisionPyCP);
        }

        // POST: SupervisionPyCPs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSupervisionPyCP")] SupervisionPyCP supervisionPyCP)
        {
            if (id != supervisionPyCP.IdSupervisionPyCP)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supervisionPyCP);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionPyCPExists(supervisionPyCP.IdSupervisionPyCP))
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
            return View(supervisionPyCP);
        }

        // GET: SupervisionPyCPs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervisionPyCP = await _context.SupervisionPyCP
                .SingleOrDefaultAsync(m => m.IdSupervisionPyCP == id);
            if (supervisionPyCP == null)
            {
                return NotFound();
            }

            return View(supervisionPyCP);
        }

        // POST: SupervisionPyCPs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supervisionPyCP = await _context.SupervisionPyCP.SingleOrDefaultAsync(m => m.IdSupervisionPyCP == id);
            _context.SupervisionPyCP.Remove(supervisionPyCP);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupervisionPyCPExists(int id)
        {
            return _context.SupervisionPyCP.Any(e => e.IdSupervisionPyCP == id);
        }
    }
}
