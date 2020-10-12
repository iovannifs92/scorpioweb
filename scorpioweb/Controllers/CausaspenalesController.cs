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

            var personas = from p in _context.Causapenal

                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                personas = personas.Where(p => p.CausaPenal.Contains(searchString)
                                        || p.Juez.Contains(searchString)
                                        || p.Cambio.Contains(searchString)
                                        || p.Distrito.Contains(searchString)
                                        || p.Cnpp.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.CausaPenal);
                    break;
                case "Date":
                    personas = personas.OrderBy(p => p.CausaPenal);
                    break;
                case "date_desc":
                    personas = personas.OrderByDescending(p => p.CausaPenal);
                    break;
                default:
                    personas = personas.OrderBy(p => p.CausaPenal);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Causapenal>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
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

            var personas = from p in _context.Causapenal

                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                personas = personas.Where(p => p.CausaPenal.Contains(searchString)
                                        || p.Juez.Contains(searchString)
                                        || p.Cambio.Contains(searchString)
                                        || p.Distrito.Contains(searchString)
                                        || p.Cnpp.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.CausaPenal);
                    break;
                case "Date":
                    personas = personas.OrderBy(p => p.CausaPenal);
                    break;
                case "date_desc":
                    personas = personas.OrderByDescending(p => p.CausaPenal);
                    break;
                default:
                    personas = personas.OrderBy(p => p.CausaPenal);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Causapenal>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
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
            List<Personacausapenal> listaPersonasAsignadas = new List<Personacausapenal>();
            List<Persona> personaVM = _context.Persona.ToList();
            List<Personacausapenal> personaCausaPenalVM = _context.Personacausapenal.ToList();

            //SQL nested query
            //select idPersona from penas2.persona p
            //where idPersona <> all(
            //select persona_idPersona from penas2.personacausapenal
            //where penas2.personacausapenal.CausaPenal_idCausaPenal = 1)
            listaPersonasAsignadas = (
                from personaCausaPenalTable in personaCausaPenalVM
                where personaCausaPenalTable.CausaPenalIdCausaPenal == id
                select personaCausaPenalTable
                ).ToList();

            listaPersonas = (
                from personaTable in personaVM
                where listaPersonasAsignadas.All(
                    per => per.PersonaIdPersona != personaTable.IdPersona
                    )
                select personaTable
                ).ToList();

            //Nombre se puede cambiar a cualquier nombre de columna manteniendo el funcionamiento
            listaPersonas.Insert(0, new Persona { IdPersona = 0, Nombre = ""});

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
        public async Task<IActionResult> Asignacion(Personacausapenal personacausapenal, Supervision supervision, Suspensionseguimiento suspensionseguimiento, Aer aer, Planeacionestrategica planeacionestrategica, Cierredecaso cierredecaso, Cambiodeobligaciones cambiodeobligaciones, Revocacion revocacion, int id)
        {
            string currentUser = User.Identity.Name;

            if (ModelState.IsValid)
            {
                if(selectedPersona.Count == 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                int idPersona = Int32.Parse(selectedPersona[0]);
                //Por el la primera opcion vacia
                if (idPersona == 0) {
                    return RedirectToAction(nameof(Index));
                }

                int idPersonaCausaPenal = ((from table in _context.Personacausapenal
                                            select table).Count()) + 1;

                personacausapenal.IdPersonaCausapenal = idPersonaCausaPenal;
                personacausapenal.PersonaIdPersona = idPersona;
                personacausapenal.CausaPenalIdCausaPenal = id;

                #region agregar 1 entrada a Supervision
                int idSupervision = ((from table in _context.Supervision
                                      select table).Count()) + 1;
                supervision.IdSupervision = idSupervision;
                supervision.PersonaIdPersona = idPersona;
                supervision.CausaPenalIdCausaPenal = id;
                #endregion

                #region agregar 1 entrada a Suspensionseguimiento
                int idSuspensionSeguimiento = ((from table in _context.Suspensionseguimiento
                                                select table).Count()) + 1;
                suspensionseguimiento.IdSuspensionSeguimiento = idSuspensionSeguimiento;
                suspensionseguimiento.SupervisionIdSupervision = idSupervision;
                #endregion

                #region agregar 1 entrada a Aer
                int idAer = ((from table in _context.Aer
                              select table).Count()) + 1;
                aer.IdAer = idAer;
                aer.SupervisionIdSupervision = idSupervision;
                #endregion

                #region agregar 1 entrada a Planeacionestrategica
                int idPlaneacionestrategica = ((from table in _context.Planeacionestrategica
                                                select table).Count()) + 1;
                planeacionestrategica.IdPlaneacionEstrategica = idPlaneacionestrategica;
                planeacionestrategica.SupervisionIdSupervision = idSupervision;
                #endregion

                #region agregar 1 entrada a Cierredecaso
                int idCierredecaso = ((from table in _context.Cierredecaso
                                       select table).Count()) + 1;
                cierredecaso.IdCierreDeCaso = idCierredecaso;
                cierredecaso.SupervisionIdSupervision = idSupervision;
                #endregion

                #region agregar 1 entrada a Cambiodeobligaciones
                int idCambiodeobligaciones = ((from table in _context.Cambiodeobligaciones
                                               select table).Count()) + 1;
                cambiodeobligaciones.IdCambiodeObligaciones = idCambiodeobligaciones;
                cambiodeobligaciones.SupervisionIdSupervision = idSupervision;
                #endregion

                #region agregar 1 entrada a Revocacion
                int idRevocacion = ((from table in _context.Revocacion
                                     select table).Count()) + 1;
                revocacion.IdRevocacion = idRevocacion;
                revocacion.SupervisionIdSupervision =  idSupervision;
                #endregion

                _context.Add(personacausapenal);
                _context.Add(supervision);
                await _context.SaveChangesAsync();
                //Guardar en 2 partes para satisfacer la restriccion de las llaves foraneas
                _context.Add(suspensionseguimiento);
                _context.Add(aer);
                _context.Add(planeacionestrategica);
                _context.Add(cierredecaso);
                _context.Add(cambiodeobligaciones);
                _context.Add(revocacion);
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