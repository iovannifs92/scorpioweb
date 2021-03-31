using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;

namespace scorpioweb.Controllers
{
    public class CausaspenalesController : Controller
    {
        #region -Variables Globales-.
        private readonly UserManager<ApplicationUser> userManager;
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


        public CausaspenalesController(penas2Context context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
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
            var i = PaginatedList<Causapenal>.CreateAsync(causa.AsNoTracking(), pageNumber ?? 1, pageSize);
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

            #region -ListaUsuarios-            
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            List<string> rolUsuario = new List<string>();

            for (int i = 0; i < roles.Count; i++)
            {
                rolUsuario.Add(roles[i]);
            }

            ViewBag.RolesUsuario = rolUsuario;
            #endregion


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
                                     select table.IdCausaPenal).Max()) + 1;
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
                        await _context.SaveChangesAsync(null, 1);
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
                await _context.SaveChangesAsync(null, 1);
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
            await _context.SaveChangesAsync(null, 1);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region -Asignacion-
        public IActionResult Asignacion(int? id, string cp)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.CausaPenal = cp;

            List<Persona> listaPersonas = new List<Persona>();
            List<Personacausapenal> listaPersonasAsignadas = new List<Personacausapenal>();
            List<Persona> personaVM = _context.Persona.ToList();
            List<Personacausapenal> personaCausaPenalVM = _context.Personacausapenal.ToList();

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
                orderby personaTable.Paterno
                select personaTable
                ).ToList();

            ViewBag.personas = listaPersonas;

            selectedPersona = new List<string>();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignacion(Personacausapenal personacausapenal, Supervision supervision, Suspensionseguimiento suspensionseguimiento, Aer aer, Planeacionestrategica planeacionestrategica, Cierredecaso cierredecaso, Cambiodeobligaciones cambiodeobligaciones, Revocacion revocacion, Fraccionesimpuestas fraccionesimpuestas, Victima victima, int id/*, int persona_idPersona*/)
        {
            string currentUser = User.Identity.Name;

            if (ModelState.IsValid)
            {
                if (selectedPersona.Count == 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                int idPersona = Int32.Parse(selectedPersona[0]);
                //int idPersona = persona_idPersona;
                //Por el la primera opcion vacia
                if (idPersona == 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                int idPersonaCausaPenal = ((from table in _context.Personacausapenal
                                            select table.IdPersonaCausapenal).Max()) + 1;

                personacausapenal.IdPersonaCausapenal = idPersonaCausaPenal;
                personacausapenal.PersonaIdPersona = idPersona;
                personacausapenal.CausaPenalIdCausaPenal = id;

                #region agregar 1 entrada a Supervision
                int idSupervision = ((from table in _context.Supervision
                                      select table.IdSupervision).Max()) + 1;
                supervision.IdSupervision = idSupervision;
                supervision.PersonaIdPersona = idPersona;
                supervision.CausaPenalIdCausaPenal = id;
                #endregion

                #region agregar 1 entrada a Suspensionseguimiento
                int idSuspensionSeguimiento = ((from table in _context.Suspensionseguimiento
                                                select table.IdSuspensionSeguimiento).Max()) + 1;
                suspensionseguimiento.IdSuspensionSeguimiento = idSuspensionSeguimiento;
                suspensionseguimiento.SupervisionIdSupervision = idSupervision;
                #endregion

                #region agregar 1 entrada a Aer
                int idAer = ((from table in _context.Aer
                              select table.IdAer).Max()) + 1;
                aer.IdAer = idAer;
                aer.SupervisionIdSupervision = idSupervision;
                #endregion

                #region agregar 1 entrada a Planeacionestrategica
                int idPlaneacionestrategica = ((from table in _context.Planeacionestrategica
                                                select table.IdPlaneacionEstrategica).Max()) + 1;
                planeacionestrategica.IdPlaneacionEstrategica = idPlaneacionestrategica;
                planeacionestrategica.SupervisionIdSupervision = idSupervision;
                #endregion

                #region agregar 1 entrada a Cierredecaso
                int idCierredecaso = ((from table in _context.Cierredecaso
                                       select table.IdCierreDeCaso).Max()) + 1;
                cierredecaso.IdCierreDeCaso = idCierredecaso;
                cierredecaso.SupervisionIdSupervision = idSupervision;
                #endregion

                #region agregar 1 entrada a Cambiodeobligaciones
                int idCambiodeobligaciones = ((from table in _context.Cambiodeobligaciones
                                               select table.IdCambiodeObligaciones).Max()) + 1;
                cambiodeobligaciones.IdCambiodeObligaciones = idCambiodeobligaciones;
                cambiodeobligaciones.SupervisionIdSupervision = idSupervision;
                #endregion

                #region agregar 1 entrada a Revocacion
                int idRevocacion = ((from table in _context.Revocacion
                                     select table.IdRevocacion).Max()) + 1;
                revocacion.IdRevocacion = idRevocacion;
                revocacion.SupervisionIdSupervision = idSupervision;
                #endregion

                _context.Add(personacausapenal);
                _context.Add(supervision);
                await _context.SaveChangesAsync(null, 1);
                //Guardar en 2 partes para satisfacer la restriccion de las llaves foraneas
                _context.Add(suspensionseguimiento);
                _context.Add(aer);
                _context.Add(planeacionestrategica);
                _context.Add(cierredecaso);
                _context.Add(cambiodeobligaciones);
                _context.Add(revocacion);
                await _context.SaveChangesAsync(null, 1);
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
                                     orderby personaTable.Paterno
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
                await _context.SaveChangesAsync(null, 1);
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

            #region Distrito
            List<SelectListItem> ListaDistrito;
            ListaDistrito = new List<SelectListItem>
            {
              new SelectListItem{ Text="I", Value="I"},
              new SelectListItem{ Text="II", Value="II"},
              new SelectListItem{ Text="III", Value="III"},
              new SelectListItem{ Text="IV", Value="IV"},
              new SelectListItem{ Text="V", Value="V"},
              new SelectListItem{ Text="VI", Value="VI"},
              new SelectListItem{ Text="VII", Value="VII"},
              new SelectListItem{ Text="VIII", Value="VIII"},
              new SelectListItem{ Text="IX", Value="IX"},
              new SelectListItem{ Text="X", Value="X"},
              new SelectListItem{ Text="XI", Value="XI"},
              new SelectListItem{ Text="XII", Value="XII"},
              new SelectListItem{ Text="XIII", Value="XIII"},
              new SelectListItem{ Text="XIV", Value="XIV"},
              new SelectListItem{ Text="XV", Value="XV"},
              new SelectListItem{ Text="XVI", Value="XVI"},

            };
            ViewBag.listaDistrito = ListaDistrito;
            ViewBag.idDistrito = BuscaId(ListaDistrito, causapenal.Distrito);
            #endregion

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

            //ViewBag.Delitos = ((ViewData["joinTablesCausaDelito"] as IEnumerable<scorpioweb.Models.CausaDelitoViewModel>).Count()).ToString();
            if ((ViewData["joinTablesCausaDelito"] as IEnumerable<scorpioweb.Models.CausaDelitoViewModel>).Count() == 0)
            {
                ViewBag.tieneDelitos = false;
            }
            else
            {
                ViewBag.tieneDelitos = true;
            }


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
                    var oldCausa = await _context.Causapenal.FindAsync(id);
                    _context.Entry(oldCausa).CurrentValues.SetValues(causa);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
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
                    var oldDelito = await _context.Delito.FindAsync(id);
                    _context.Entry(oldDelito).CurrentValues.SetValues(delito);
                    //_context.Update(delito);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
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