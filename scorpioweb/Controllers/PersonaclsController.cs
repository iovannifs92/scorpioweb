using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace scorpioweb.Models
{
    public class PersonaclsController : Controller
    {
        private readonly penas2Context _context;

        public PersonaclsController(penas2Context context)
        {
            _context = context;
        }

        // GET: Personacls
        public async Task<IActionResult> Index()
        {
            return View(await _context.Personacl.ToListAsync());
        }

        // GET: Personacls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personacl = await _context.Personacl
                .SingleOrDefaultAsync(m => m.IdPersonaCl == id);
            if (personacl == null)
            {
                return NotFound();
            }

            return View(personacl);
        }

        // GET: Personacls/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Personacls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPersonaCl,Nombre,Paterno,Materno,NombrePadre,NombreMadre,Alias,Genero,Edad,Fnacimiento,Lnpais,Lnestado,Lnmunicipio,Lnlocalidad,EstadoCivil,Duracion,OtroIdioma,EspecifiqueIdioma,DatosGeneralescol,LeerEscribir,Traductor,EspecifiqueTraductor,TelefonoFijo,Celular,Hijos,Nhijos,NpersonasVive,Propiedades,Curp,ConsumoSustancias,UltimaActualización,Supervisor,RutaFoto,Familiares,ReferenciasPersonales,Capturista,Candado,MotivoCandado,Colaboracion,UbicacionExpediente,ComLgbtttiq,ComIndigena,TieneResolucion")] Personacl personacl)
        {
            if (ModelState.IsValid)
            {
                _context.Add(personacl);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(personacl);
        }

        // GET: Personacls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personacl = await _context.Personacl.SingleOrDefaultAsync(m => m.IdPersonaCl == id);
            if (personacl == null)
            {
                return NotFound();
            }
            return View(personacl);
        }

        // POST: Personacls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPersonaCl,Nombre,Paterno,Materno,NombrePadre,NombreMadre,Alias,Genero,Edad,Fnacimiento,Lnpais,Lnestado,Lnmunicipio,Lnlocalidad,EstadoCivil,Duracion,OtroIdioma,EspecifiqueIdioma,DatosGeneralescol,LeerEscribir,Traductor,EspecifiqueTraductor,TelefonoFijo,Celular,Hijos,Nhijos,NpersonasVive,Propiedades,Curp,ConsumoSustancias,UltimaActualización,Supervisor,RutaFoto,Familiares,ReferenciasPersonales,Capturista,Candado,MotivoCandado,Colaboracion,UbicacionExpediente,ComLgbtttiq,ComIndigena,TieneResolucion")] Personacl personacl)
        {
            if (id != personacl.IdPersonaCl)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personacl);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaclExists(personacl.IdPersonaCl))
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
            return View(personacl);
        }

        // GET: Personacls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personacl = await _context.Personacl
                .SingleOrDefaultAsync(m => m.IdPersonaCl == id);
            if (personacl == null)
            {
                return NotFound();
            }

            return View(personacl);
        }

        // POST: Personacls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personacl = await _context.Personacl.SingleOrDefaultAsync(m => m.IdPersonaCl == id);
            _context.Personacl.Remove(personacl);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonaclExists(int id)
        {
            return _context.Personacl.Any(e => e.IdPersonaCl == id);
        }
    }
}
