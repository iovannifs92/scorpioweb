using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Newtonsoft.Json.Serialization;


namespace scorpioweb.Controllers
{
    [Authorize]
    public class CalendariosController : Controller
    {
        #region -Variables Globales-
        private readonly penas2Context _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        #endregion

        #region -Constructor-
        public CalendariosController(penas2Context context,
            RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        #endregion

        #region -Metodos Generales-
        public string normaliza(string normalizar)
        {
            if (!String.IsNullOrEmpty(normalizar))
            {
                normalizar = normalizar.ToUpper();
            }
            else
            {
                normalizar = "NA";
            }
            return normalizar;
        } 
        #endregion

        public async Task<IActionResult>getCalendarTasks()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            string usuario = user.ToString();

            var tasks = (from task in _context.Calendario
                        where task.Usuario == usuario
                        select task).ToList();

            return Json(tasks, new Newtonsoft.Json.JsonSerializerSettings());
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Calendario.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calendario = await _context.Calendario
                .SingleOrDefaultAsync(m => m.Idcalendario == id);
            if (calendario == null)
            {
                return NotFound();
            }

            return View(calendario);
        }

        public IActionResult Create(int id)
        {
            ViewBag
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mensaje,FechaEvento,Prioridad,Tipo,PersonaIdPersona")] Calendario calendario)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            string usuario = user.ToString();

            if (ModelState.IsValid)
            {
                calendario.Usuario = usuario;
                calendario.FechaCreacion = DateTime.Now;
                calendario.Mensaje = normaliza(calendario.Mensaje);
                _context.Add(calendario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(calendario);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calendario = await _context.Calendario.SingleOrDefaultAsync(m => m.Idcalendario == id);
            if (calendario == null)
            {
                return NotFound();
            }
            return View(calendario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Idcalendario,Mensaje,FechaEvento,Prioridad,Usuario,Tipo,FechaCreacion,PersonaIdPersona")] Calendario calendario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    calendario.Mensaje = normaliza(calendario.Mensaje);
                    var oldCalendario = await _context.Calendario.FindAsync(calendario.Idcalendario);

                    _context.Entry(oldCalendario).CurrentValues.SetValues(calendario);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalendarioExists(calendario.Idcalendario))
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
            return View(calendario);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calendario = await _context.Calendario
                .SingleOrDefaultAsync(m => m.Idcalendario == id);
            if (calendario == null)
            {
                return NotFound();
            }

            return View(calendario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calendario = await _context.Calendario.SingleOrDefaultAsync(m => m.Idcalendario == id);
            _context.Calendario.Remove(calendario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CalendarioExists(int id)
        {
            return _context.Calendario.Any(e => e.Idcalendario == id);
        }
    }
}
