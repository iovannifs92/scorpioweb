using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using scorpioweb.Class;
using scorpioweb.Models;

namespace scorpioweb.Controllers
{
    [Authorize]
    public class ReinsercionController : Controller
    {
        private readonly penas2Context _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<HubNotificacion> _hubContext;
        private readonly RoleManager<IdentityRole> roleManager;

        public ReinsercionController(penas2Context context, IHostingEnvironment hostingEnvironment,
                                  RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IHubContext<HubNotificacion> hubContext)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
            _hubContext = hubContext;

        }

        #region -FichaCanalización-
        public IActionResult FichaCanalizacion()
        {
            return View();
        }
        #endregion

        // GET: Reinsercion
        public async Task<IActionResult> Index()
        {
            return View(await _context.Reinsercion.ToListAsync());
        }

        // GET: Reinsercion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reinsercion = await _context.Reinsercion
                .SingleOrDefaultAsync(m => m.IdReinsercion == id);
            if (reinsercion == null)
            {
                return NotFound();
            }

            return View(reinsercion);
        }

        // GET: Reinsercion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reinsercion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdReinsercion,IdTabla,Tabla,Lugar,Estado")] Reinsercion reinsercion)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            if (ModelState.IsValid)
            {
                //Sacar info de su tabla 
                //string tablaSeleccionada = reinsercion.Tabla; // Define la tabla seleccionada aquí

                //IQueryable<Persona> consultaPersona = null;

                //if (tablaSeleccionada == "Persona")
                //{
                //    consultaPersona = _context.Persona
                //        .Where(persona => persona.IdPersona == Int32.Parse(reinsercion.IdTabla));
                //}
                //else(tablaSeleccionada == "Personacl")
                //{
                //    consultaPersona = _context.Personacl
                //        .Where(personacl => personacl.IdPersonaCl == Int32.Parse(reinsercion.IdTabla));
                //}

                //var info = consultaPersona.ToList();

                _context.Add(reinsercion);
                await _context.SaveChangesAsync();
                //await _hubContext.Clients.All.SendAsync("Recive", libronegro.Nombre, libronegro.Materno);

                foreach (var rol in roles)
                {
                    if (rol != "Vinculacion")
                    {
                        await _hubContext.Clients.Group("nuevaCanalizacion").SendAsync("sendMessage", "Hay una nueva canalizacion"  );
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reinsercion);
        }

        // GET: Reinsercion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reinsercion = await _context.Reinsercion.SingleOrDefaultAsync(m => m.IdReinsercion == id);
            if (reinsercion == null)
            {
                return NotFound();
            }
            return View(reinsercion);
        }

        // POST: Reinsercion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdReinsercion,IdTabla,Tabla,Lugar,Estado")] Reinsercion reinsercion)
        {
            if (id != reinsercion.IdReinsercion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reinsercion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReinsercionExists(reinsercion.IdReinsercion))
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
            return View(reinsercion);
        }

        // GET: Reinsercion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reinsercion = await _context.Reinsercion
                .SingleOrDefaultAsync(m => m.IdReinsercion == id);
            if (reinsercion == null)
            {
                return NotFound();
            }

            return View(reinsercion);
        }

        // POST: Reinsercion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reinsercion = await _context.Reinsercion.SingleOrDefaultAsync(m => m.IdReinsercion == id);
            _context.Reinsercion.Remove(reinsercion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReinsercionExists(int id)
        {
            return _context.Reinsercion.Any(e => e.IdReinsercion == id);
        }
    }
}
