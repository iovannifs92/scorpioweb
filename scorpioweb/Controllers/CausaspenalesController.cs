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
    public class CausaspenalesController : Controller
    {
        #region
        public static List<List<string>> datosPersonas = new List<List<string>>();

        private readonly penas2Context _context;
        private List<SelectListItem> listaSiNo = new List<SelectListItem>
        {
            new SelectListItem{ Text="Si", Value="SI"},
            new SelectListItem{ Text="No", Value="NO"}
        };
        public string normaliza(string normalizar)
        {
            if (!String.IsNullOrEmpty(normalizar))
            {
                normalizar = normalizar.ToUpper();
            }
            return normalizar;
        }
        #endregion

        public CausaspenalesController(penas2Context context)
        {
            _context = context;
        }

        // GET: Causaspenales
        public async Task<IActionResult> Index()
        {
            return View(await _context.Causapenal.ToListAsync());
        }

        // GET: Causaspenales/Details/5
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

        // GET: Causaspenales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Causaspenales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Causapenal causapenal, Delito personaCausaPenalDB, string cnpp, string juez, string cambio, string cp)
        {
            if (ModelState.IsValid)
            {
                string currentUser = User.Identity.Name;

                #region -asignacion causa penal a persona-
                int idCausaPenal = ((from table in _context.Causapenal
                                     select table).Count()) + 1;
                causapenal.IdCausaPenal = idCausaPenal;
                for (int i = 0; i < datosPersonas.Count; i = i + 1)
                {
                    if (datosPersonas[i][1] == currentUser)
                    {
                        personaCausaPenalDB.Tipo = datosPersonas[i][0];
                        personaCausaPenalDB.CausaPenalIdCausaPenal = idCausaPenal;

                        _context.Add(personaCausaPenalDB);
                        await _context.SaveChangesAsync();
                    }
                }
                #endregion

                causapenal.Cnpp = cnpp;
                causapenal.Juez = juez;
                causapenal.Cambio = cambio;
                causapenal.CausaPenal = cp;
                _context.Add(causapenal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(causapenal);
        }

        String BuscaId(List<SelectListItem> lista, String texto)
        {
            foreach (var item in lista)
            {
                if (normaliza(item.Value) == normaliza(texto))
                {
                    return item.Value;
                }
            }
            return "";
        }

        // GET: Causaspenales/Edit/5
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

            ViewBag.listaCnpp = listaSiNo;
            ViewBag.idCnpp = BuscaId(listaSiNo, causapenal.Cnpp);

            ViewBag.listaCambio = listaSiNo;
            ViewBag.idCambio = BuscaId(listaSiNo, causapenal.Cambio);


            return View(causapenal);
        }

        // POST: Causaspenales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCausaPenal,Cnpp,Juez,Cambio,CausaPenal")] Causapenal causa)
        {
            if (id != causa.IdCausaPenal)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(causa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CausapenalExists(causa.IdCausaPenal))
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
            return View(causa);
        }

        // GET: Causaspenales/Delete/5
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

        // POST: Causaspenales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var causapenal = await _context.Causapenal.SingleOrDefaultAsync(m => m.IdCausaPenal == id);
            _context.Causapenal.Remove(causapenal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET: CausasPenales/AsignacionCP
        public async Task<IActionResult> AsignacionCP(
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

            var personas = from p in _context.Persona
                           where p.Supervisor == User.Identity.Name
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                personas = personas.Where(p => p.Paterno.Contains(searchString)
                                        || p.Materno.Contains(searchString)
                                        || p.Nombre.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.Paterno);
                    break;
                case "Date":
                    personas = personas.OrderBy(p => p.UltimaActualización);
                    break;
                case "date_desc":
                    personas = personas.OrderByDescending(p => p.UltimaActualización);
                    break;
                default:
                    personas = personas.OrderBy(p => p.Paterno);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        #region Asignacion
        //GET: CausasPenales/AsignacionCP/14
        public async Task<IActionResult> Asignacion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona
                .SingleOrDefaultAsync(m => m.IdPersona == id);

            List<Persona> listaPersonas = new List<Persona>();
            listaPersonas = (from table in _context.Persona
                             select table).ToList();

            //Nombre se puede cambiar a cualquier nombre de columna manteniendo el funcionamiento
            listaPersonas.Insert(0, new Persona { IdPersona = 0, Nombre = "Selecciona" });
            ViewBag.personas = listaPersonas;

            if (persona == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignacion(int id, [Bind("personaVM")] PersonaCausaPenalViewModel personaCausaPenalVM)
        {
            /*if (id != personaCausaPenalVM.PersonaIdPersona)
            {
                return NotFound();
            }*/

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personaCausaPenalVM);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    /*if (!PersonaExists(abandonoestado.PersonaIdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }*/
                }
                return RedirectToAction(nameof(Index));
            }
            return View(personaCausaPenalVM);
        }
        #endregion

        #region botton asignar
        public ActionResult AsignarPersona(string[] datosPersona)
        {
            string currentUser = User.Identity.Name;
            datosPersona.DefaultIfEmpty();
            for (int i = 0; i < datosPersona.Length; i++)
            {
                datosPersonas.Add(new List<String> { datosPersona[i], currentUser });
            }

            return Json(new { success = true, responseText = "Datos Guardados con éxito" });
        }
        #endregion

        #region Detalles CP
        // GET: Causaspenales/DetailsCP/14
        public async Task<IActionResult> DetailsCP(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona
                .SingleOrDefaultAsync(m => m.IdPersona == id);

            #region -To List databases-

            List<Persona> personaVM = _context.Persona.ToList();
            List<Personacausapenal> personaCausaPenalVM = _context.Personacausapenal.ToList();
            List<Causapenal> causaPenalVM = _context.Causapenal.ToList();

            #endregion

            #region -Jointables-
            ViewData["joinTables"] = from personaTable in personaVM
                                     join personaCausaPenal in personaCausaPenalVM on persona.IdPersona equals personaCausaPenal.PersonaIdPersona
                                     join causaPenal in causaPenalVM on personaCausaPenal.CausaPenalIdCausaPenal equals causaPenal.IdCausaPenal
                                     where personaTable.IdPersona == id
                                     select new PersonaCausaPenalViewModel
                                     {
                                         personaVM = personaTable,
                                         causaPenalVM = causaPenal
                                     };

            #endregion

            if (persona == null)
            {
                return NotFound();
            }

            return View();
        }
        #endregion

        // GET: Causaspenales
        public IActionResult ControlCP()
        {
            return View();
        }

        private bool CausapenalExists(int id)
        {
            return _context.Causapenal.Any(e => e.IdCausaPenal == id);
        }
    }
}
