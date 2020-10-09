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
        #region -Variables Globales-
        public static List<string> selectedPersona = new List<string>();
        public static List<List<string>> datosDelitos = new List<List<string>>();
        private readonly penas2Context _context;
        private List<SelectListItem> listaSiNo = new List<SelectListItem>

        {
            new SelectListItem{ Text="Si", Value="SI"},
            new SelectListItem{ Text="No", Value="NO"}
        };
        
        #endregion

        public string normaliza(string normalizar)
        {
            if (!String.IsNullOrEmpty(normalizar))
            {
                normalizar = normalizar.ToUpper();
            }
            return normalizar;
        }
        

        public CausaspenalesController(penas2Context context)
        {
            _context = context;
        }

        #region -Index-
        public async Task<IActionResult> Index(
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

            var causa = from p in _context.Causapenal

                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                causa = causa.Where(p => p.CausaPenal.Contains(searchString)
                                        || p.Juez.Contains(searchString)
                                        || p.Cambio.Contains(searchString)
                                        || p.Distrito.Contains(searchString)
                                        || p.Cnpp.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    causa = causa.OrderByDescending(p => p.CausaPenal);
                    break;
                case "Date":
                    causa = causa.OrderBy(p => p.CausaPenal);
                    break;
                case "date_desc":
                    causa = causa.OrderByDescending(p => p.CausaPenal);
                    break;
                default:
                    causa = causa.OrderBy(p => p.CausaPenal);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Causapenal>.CreateAsync(causa.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        #endregion

        #region -Lista de Causas-
        public async Task<IActionResult> ListadeCausas(
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

            var causa = from p in _context.Causapenal
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                causa = causa.Where(p => p.CausaPenal.Contains(searchString)
                                        || p.Juez.Contains(searchString)
                                        || p.Cambio.Contains(searchString)
                                        || p.Distrito.Contains(searchString)
                                        || p.Cnpp.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    causa = causa.OrderByDescending(p => p.CausaPenal);
                    break;
                case "Date":
                    causa = causa.OrderBy(p => p.CausaPenal);
                    break;
                case "date_desc":
                    causa = causa.OrderByDescending(p => p.CausaPenal);
                    break;
                default:
                    causa = causa.OrderBy(p => p.CausaPenal);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Causapenal>.CreateAsync(causa.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        #endregion


        #region -Details-
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
        #endregion

        #region -Modal Delito-
        public ActionResult GuardarDelito(string[] datosDelito)
        {
            string currentUser = User.Identity.Name;
            datosDelito.DefaultIfEmpty();
            for (int i = 0; i < datosDelito.Length; i++)
            {

                datosDelitos.Add(new List<String> { datosDelito[i], currentUser });
            }

            return Json(new { success = true, responseText = "Datos Guardados con éxito" });

        }
        #endregion

        #region -Create-
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Causapenal causapenal, Delito delitoDB, string cnpp, string juez, string distrito, string cambio, string cp, string delitoM)
        {
            string currentUser = User.Identity.Name;
            if (ModelState.IsValid)
            {
                #region -Delitos-
                int idCausaPenal = ((from table in _context.Causapenal
                                     select table).Count()) + 1;
                causapenal.IdCausaPenal = idCausaPenal;
                for (int i = 0; i < datosDelitos.Count; i = i + 3)
                {
                    if (datosDelitos[i][1] == currentUser)
                    {
                        delitoDB.Tipo = datosDelitos[i][0];
                        delitoDB.Modalidad = datosDelitos[i + 1][0];
                        delitoDB.EspecificarDelito = datosDelitos[i + 2][0];
                        delitoDB.CausaPenalIdCausaPenal = idCausaPenal;

                        _context.Add(delitoDB);
                        await _context.SaveChangesAsync();
                    }
                }

                for (int i = 0; i < datosDelitos.Count; i++)
                {
                    if (datosDelitos[i][1] == currentUser)
                    {
                        datosDelitos.RemoveAt(i);
                        i--;
                    }
                }
                #endregion

                causapenal.Cnpp = cnpp;
                causapenal.Juez = juez;
                causapenal.Distrito = distrito;
                causapenal.Cambio = cambio;
                causapenal.CausaPenal = cp;
                _context.Add(causapenal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(causapenal);
        }
        #endregion
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

        #region -Delete-
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

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var causapenal = await _context.Causapenal.SingleOrDefaultAsync(m => m.IdCausaPenal == id);
            _context.Causapenal.Remove(causapenal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region -Asignacion-
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

            
            listaPersonas.Insert(0, new Persona { IdPersona = 0, Nombre = "Selecciona" });
            ViewBag.personas = listaPersonas;

            selectedPersona = new List<string>();

            if (persona == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignacion(Personacausapenal personacausapenal, int id)
        {
            string currentUser = User.Identity.Name;

            if (ModelState.IsValid)
            {
                if(selectedPersona.Count == 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                int idPersona = Int32.Parse(selectedPersona[0]);
                //Por el "Selecciona"
                if (idPersona == 0) {
                    return RedirectToAction(nameof(Index));
                }

                int idPersonaCausaPenal = ((from table in _context.Personacausapenal
                                            select table).Count()) + 1;

                personacausapenal.IdPersonaCausapenal = idPersonaCausaPenal;
                personacausapenal.PersonaIdPersona = idPersona;
                personacausapenal.CausaPenalIdCausaPenal = id;

                _context.Add(personacausapenal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(personacausapenal);
        }
        #endregion

        #region -AsignarPersona-
        public ActionResult AsignarPersona(string[] datosPersona)
        {
            string currentUser = User.Identity.Name;
            if (datosPersona.Length > 0)
            {
                selectedPersona = new List<String> { datosPersona[0], currentUser };
            }
            return Json(new { success = true, responseText = "Persona seleccionada" });
        }
        #endregion

        #region -DetailsCP-

        public async Task<IActionResult> DetailsCP(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var causapenal = await _context.Causapenal
                .SingleOrDefaultAsync(m => m.IdCausaPenal == id);

            #region -To List databases-

            List<Persona> personaVM = _context.Persona.ToList();
            List<Personacausapenal> personaCausaPenalVM = _context.Personacausapenal.ToList();
            List<Causapenal> causaPenalVM = _context.Causapenal.ToList();

            #endregion

            #region -Jointables-
            ViewData["joinTables"] = from personaTable in personaVM
                                     join personaCausaPenalTable in personaCausaPenalVM on personaTable.IdPersona equals personaCausaPenalTable.PersonaIdPersona
                                     join causaPenalTable in causaPenalVM on personaCausaPenalTable.CausaPenalIdCausaPenal equals causaPenalTable.IdCausaPenal
                                     where causaPenalTable.IdCausaPenal == id
                                     select new PersonaCausaPenalViewModel
                                     {
                                         personaVM = personaTable,
                                         causaPenalVM = causaPenalTable
                                     };
            #endregion

            if (causapenal == null)
            {
                return NotFound();
            }

            return View();
        }
        #endregion

        private bool DelitolExists(int id)
        {
            return _context.Delito.Any(e => e.IdDelito == id);
        }

        #region -DetailsCP-
        public IActionResult CreateDelito(int? id)
        {
            return View();
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDelito(int id, Delito delitoDB, string Tipo, string Modalidad, string EspecificarDelito)
        {
            string currentUser = User.Identity.Name;
            if (ModelState.IsValid)
            {
                delitoDB.Tipo = Tipo;
                delitoDB.Modalidad = Modalidad;
                delitoDB.EspecificarDelito = EspecificarDelito;
                delitoDB.CausaPenalIdCausaPenal = id;
                
                _context.Add(delitoDB);
                await _context.SaveChangesAsync();
                return RedirectToAction("EditCausas", "Causaspenales", new { @id = id });
            }
            return View();
        }
        #endregion

        private bool CausapenalExists(int id)
        {
            return _context.Causapenal.Any(e => e.IdCausaPenal == id);
        }

        #region -EditCausas-
        public async Task<IActionResult> EditCausas(int? id)
        
        {
            var IdCausaPenal = id;
            if (IdCausaPenal == null)
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

            List<Delito> delitoVM = _context.Delito.ToList();
            List<Causapenal> causaPenalVM = _context.Causapenal.ToList();

            #region -Jointables-
            ViewData["joinTablesCausa"] =
                                     from causapenalTable in causaPenalVM
                                     where causapenalTable.IdCausaPenal == IdCausaPenal
                                     select new CausaDelitoViewModel
                                     {
                                         causaPenalVM = causapenalTable
                                     };

            // ViewBag.Delitos = ((ViewData["joinTablesDelito"] as IEnumerable<scorpioweb.Models.CausaDelitoViewModel>).Count()).ToString();
            #endregion


            List<Delito> delitoVMV = _context.Delito.ToList();
            List<Causapenal> causaPenalVMV = _context.Causapenal.ToList();

         
            ViewData["joinTablesCausaDelito"] =
                                     from causapenalTable in causaPenalVM
                                     join delitoTabla in delitoVM on causapenal.IdCausaPenal equals delitoTabla.CausaPenalIdCausaPenal
                                     where causapenalTable.IdCausaPenal == IdCausaPenal
                                     select new CausaDelitoViewModel
                                     {
                                         delitoVM = delitoTabla,
                                         causaPenalVM = causapenalTable

                                     };


            return View();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCausas(int id, [Bind("IdCausaPenal,Cnpp,Juez,Cambio,Distrito,CausaPenal")] Causapenal causa)
        {
            if (id == null)
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
                    if (!DelitolExists(causa.IdCausaPenal))
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
            return View();
        }
        #endregion

        #region -Delito-
        public async Task<IActionResult> Delito(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var delito = await _context.Delito.SingleOrDefaultAsync(m => m.IdDelito == id);
            if (delito == null)
            {
                return NotFound();
            }
            return View(delito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delito(int id, [Bind("IdDelito,Tipo,Modalidad,EspecificarDelito,CausaPenalIdCausaPenal")] Delito delito)
        {
            if (id != delito.IdDelito)
            {
                return NotFound();
            }
            var idcausa = delito.CausaPenalIdCausaPenal;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(delito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CausapenalExists(delito.IdDelito))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("EditCausas", "Causaspenales", new { @id = idcausa });
            }
            return View();
        }
        #endregion
    }
}