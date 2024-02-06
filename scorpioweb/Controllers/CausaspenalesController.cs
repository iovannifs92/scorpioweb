using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;
using F23.StringSimilarity;
using Newtonsoft.Json.Linq;
using scorpioweb.Class;
using DocumentFormat.OpenXml.Wordprocessing;

namespace scorpioweb.Controllers
{
    [Authorize]
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
        #region -Metodos Generales-
        MetodosGenerales mg = new MetodosGenerales();
        #endregion
       

        public CausaspenalesController(penas2Context context,RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
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
            ViewBag.user = user;
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
        public async Task<IActionResult> Create()
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

            String users = user.ToString();
            ViewBag.RolesUsuarios = users;
            #endregion

            List<Causapenal> listaCP = new List<Causapenal>();
            List<Causapenal> CPVM = _context.Causapenal.ToList();
            listaCP = (
                from CPTable in CPVM
                select CPTable
                ).ToList();
            ViewBag.cp = listaCP;
            ViewBag.directorio = _context.Directoriojueces.Select(Directoriojueces => Directoriojueces.Area).ToList();
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

            return View();
        }

        public async Task<IActionResult> CreateCausaPenal(string juez, string distrito, string cambio, string cp)
        {
            Causapenal causapenal = new Causapenal();
            Delito delitoDB = new Delito();
            await Create(causapenal, delitoDB, null, juez, distrito, cambio, cp);
            int idCausaPenal = ((from table in _context.Causapenal
                                 select table.IdCausaPenal).Max());
            if (cp == null)
            {
                return Json(new { success = true, registrar = false});
            }
            else
            {
                return Json(new { success = true, responseText = idCausaPenal });
            }
            
        }

        [HttpPost]
        //[ValidateAntiForgeryToken] Para poder llamar el metodo desde Oficialia
        public async Task<IActionResult> Create(Causapenal causapenal, Delito delitoDB, string cnpp, string juez, string distrito, string cambio, string cp)
        {
            var usuario = await userManager.FindByNameAsync(User.Identity.Name);
            String users = usuario.ToString();
   


            string currentUser = User.Identity.Name;

            causapenal.Cnpp = cnpp;
            causapenal.Juez = mg.normaliza(juez);
            causapenal.Distrito = distrito;
            causapenal.Cambio = cambio;
            causapenal.CausaPenal = mg.normaliza(cp);
            causapenal.Fechacreacion = DateTime.Now;
            causapenal.Usuario = users;

            if (cp == null)
            {
                return Json(new { success = true, registrar = false });
            }
            else
            {
                if (ModelState.IsValid)
                {         
                    int idCausaPenal = ((from table in _context.Causapenal
                                         select table.IdCausaPenal).Max()) + 1;
                    causapenal.IdCausaPenal = idCausaPenal;
                    #region -Delitos-
                    for (int i = 0; i < datosDelitos.Count; i = i + 2)
                    {
                        if (datosDelitos[i][1] == currentUser)
                        {
                            delitoDB.Tipo = mg.normaliza(datosDelitos[i][0]);
                            delitoDB.Modalidad = mg.normaliza(datosDelitos[i + 1][0]);
                            delitoDB.CausaPenalIdCausaPenal = idCausaPenal;
                            delitoDB.EspecificarDelito = mg.normaliza(delitoDB.EspecificarDelito);

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

                    _context.Add(causapenal);
                    await _context.SaveChangesAsync(null, 1);
                    return Json(new { success = true, registrar = true,  responseText = Url.Action("Asignacion", "Causaspenales", new { @id = idCausaPenal, @cp = cp }) });
                }
                return View(causapenal);
            }
        }
        #endregion
        String BuscaId(List<SelectListItem> lista, String texto)
        {
            foreach (var item in lista)
            {
                if (mg.normaliza(item.Value) == mg.normaliza(texto))
                {
                    return item.Value;
                }
            }
            return "";
        }

        #region -Delete-
        public JsonResult antesdelete(Causapenal causapenal, Personacausapenal personacausapenal, string[] datocp)
        {
            var borrar = false;
            var id = Int32.Parse(datocp[0]);

            var query = (from c in _context.Causapenal
                         where c.IdCausaPenal == id
                         select c).FirstOrDefault();

            var antesdel = from pc in _context.Personacausapenal
                           where pc.CausaPenalIdCausaPenal == id
                           select pc;

            if (antesdel.Any())
            {
                return Json(new { success = true, responseText = Url.Action("ListadeCausas", "Causaspenales"), borrar = borrar});
            }
            else
            {
                borrar = true;
                return Json(new { success = true, responseText = Url.Action("ListadeCausas", "Causaspenales"), borrar = borrar });
            }
        }


        public JsonResult deletecp(Causapenal causapenal, Historialeliminacion historialeliminacion, string[] datocp)
        {
            var borrar = false;
            var id = Int32.Parse(datocp[0]);
            var razon = mg.normaliza(datocp[1]);
            var user = mg.normaliza(datocp[2]);

            var query = (from c in _context.Causapenal
                         where c.IdCausaPenal == id
                         select c).FirstOrDefault();

            try
            {
                borrar = true;
                historialeliminacion.Id = id;
                historialeliminacion.Descripcion = query.CausaPenal;
                historialeliminacion.Tipo = "CAUSA PENAL";
                historialeliminacion.Razon = mg.normaliza(razon);
                historialeliminacion.Usuario = user;
                historialeliminacion.Fecha = DateTime.Now;
                historialeliminacion.Supervisor = "NA";
                _context.Add(historialeliminacion);
                _context.SaveChanges();

                var causapenals = _context.Causapenal.FirstOrDefault(m => m.IdCausaPenal == id);
                _context.Causapenal.Remove(causapenals);
                _context.SaveChanges();

                return Json(new { success = true, responseText = Url.Action("index", "Personas"), borrar = borrar });
            }
            catch (Exception ex)
            {
                var error = ex; 
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("index", "Personas"), borrar = borrar});
            }
        }




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
        public async Task<IActionResult> Asignacion(
            int? id, 
            string cp,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CausaPenal = cp;
            ViewBag.idCausaPenal = id;

            ViewData["CurrentFilter"] = searchString;


            List<Personacausapenal> personaCausaPenalVM = _context.Personacausapenal.ToList();

            var listaPersonasAsignadas = from personaCausaPenalTable in personaCausaPenalVM
               where personaCausaPenalTable.CausaPenalIdCausaPenal == id
               select personaCausaPenalTable;

            var personas = from personaTable in _context.Persona
                where listaPersonasAsignadas.All(per => per.PersonaIdPersona != personaTable.IdPersona) select personaTable;

            personas = personas.OrderByDescending(p => p.IdPersona);

            if (!String.IsNullOrEmpty(searchString))
            {
                foreach (var item in searchString.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (mg.replaceSlashes(p.Paterno) + " " + mg.replaceSlashes(p.Materno) + " " + mg.replaceSlashes(p.Nombre)).Contains(mg.normaliza(searchString)) ||
                                                   (mg.replaceSlashes(p.Nombre) + " " + mg.replaceSlashes(p.Paterno) + " " + mg.replaceSlashes(p.Materno)).Contains(mg.normaliza(searchString)) || 
                                                   (p.IdPersona.ToString().Contains(searchString)));
                }
            }

            int pageSize = 10;
            return View(await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        //public async Task<IActionResult> Asignacion(int? id, string cp)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewBag.CausaPenal = cp;
        //    var user = await userManager.FindByNameAsync(User.Identity.Name);

        //    List<Persona> listaPersonas = new List<Persona>();
        //    List<Personacausapenal> listaPersonasAsignadas = new List<Personacausapenal>();
        //    List<Persona> personaVM = _context.Persona.ToList();
        //    List<Personacausapenal> personaCausaPenalVM = _context.Personacausapenal.ToList();

        //    listaPersonasAsignadas = (
        //        from personaCausaPenalTable in personaCausaPenalVM
        //        where personaCausaPenalTable.CausaPenalIdCausaPenal == id
        //        select personaCausaPenalTable
        //        ).ToList();

        //    if (await esAdmin())
        //    {
        //        listaPersonas = (
        //        from personaTable in personaVM
        //        where listaPersonasAsignadas.All(
        //            per => per.PersonaIdPersona != personaTable.IdPersona
        //            )
        //        orderby personaTable.Paterno
        //        select personaTable
        //        ).ToList();
        //    }
        //    else
        //    {
        //        listaPersonas = (
        //        from personaTable in personaVM
        //        where listaPersonasAsignadas.All(
        //            per => per.PersonaIdPersona != personaTable.IdPersona
        //            )
        //        && personaTable.Supervisor == user.ToString()
        //        orderby personaTable.Paterno
        //        select personaTable
        //        ).ToList();
        //    }

        //    ViewBag.personas = listaPersonas;
        //    selectedPersona = new List<string>();

        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignacion(Personacausapenal personacausapenal, string PersonaAsignada, Supervision supervision, Suspensionseguimiento suspensionseguimiento, Aer aer, Planeacionestrategica planeacionestrategica, Cierredecaso cierredecaso, Cambiodeobligaciones cambiodeobligaciones, Revocacion revocacion, Fraccionesimpuestas fraccionesimpuestas, Victima victima, int id/*, int persona_idPersona*/)
        {
            string currentUser = User.Identity.Name;

            if (PersonaAsignada == null)
            {

                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                #region -Sacar solo enteros-
                string personaA = PersonaAsignada;
                string valueINT = string.Empty;
                int idpersona = 0;

                for (int i = 0; i < personaA.Length; i++)
                {
                    if (Char.IsDigit(personaA[i]))
                        valueINT += personaA[i];
                }

                if (valueINT.Length > 0)
                    idpersona = int.Parse(valueINT);
                
                    
                #endregion

                if (idpersona == 0)
                {
                    return RedirectToAction(nameof(Index));
                }

                int idPersonaCausaPenal = ((from table in _context.Personacausapenal
                                            select table.IdPersonaCausapenal).Max()) + 1;


                #region -AddPesonaCausapenal-
                if (idpersona != 0)
                {
                    
                    personacausapenal.PersonaIdPersona = Convert.ToInt32(idpersona);
                    personacausapenal.CausaPenalIdCausaPenal = id;
                    personacausapenal.IdPersonaCausapenal = idPersonaCausaPenal;
                    _context.Add(personacausapenal);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                }

                #endregion

                #region agregar 1 entrada a Supervision
                int idSupervision = ((from table in _context.Supervision
                                      select table.IdSupervision).Max()) + 1;
                supervision.IdSupervision = idSupervision;
                supervision.PersonaIdPersona = idpersona;
                supervision.CausaPenalIdCausaPenal = id;
                supervision.EstadoSupervision = "VIGENTE";
                supervision.Tta = "NO";
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
                return RedirectToAction("Supervision", "Supervisiones", new { @id = idSupervision});
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
            ViewBag.idDelito = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDelito(int id, Delito delitoDB, string Tipo, string Modalidad, string EspecificarDelito)
        {
            string currentUser = User.Identity.Name;
            if (ModelState.IsValid)
            {
                delitoDB.Tipo = mg.normaliza(Tipo);
                delitoDB.Modalidad = mg.normaliza(Modalidad);
                delitoDB.EspecificarDelito = mg.normaliza(delitoDB.EspecificarDelito);
                delitoDB.CausaPenalIdCausaPenal = id;

                int idDelito = ((from table in _context.Delito
                                 select table.IdDelito).Max()) + 1;

                delitoDB.IdDelito = idDelito;
                _context.Add(delitoDB);
                await _context.SaveChangesAsync();
                return RedirectToAction("EditCausas/" + delitoDB.CausaPenalIdCausaPenal, "Causaspenales");
            }
            return View(delitoDB);
        }
        #endregion

        private bool CausapenalExists(int id)
        {
            return _context.Causapenal.Any(e => e.IdCausaPenal == id);
        }

        //#region -EditCausas-
        //public async Task<IActionResult> EditCausapenal(int? id)
        //{
        //    var causapenal = await _context.Causapenal.SingleOrDefaultAsync(m => m.IdCausaPenal == id);
        //    if (causapenal == null)
        //    {
        //        return NotFound();
        //    }

        //    List<Causapenal> listaCP = new List<Causapenal>();
        //    List<Causapenal> CPVM = _context.Causapenal.ToList();
        //    listaCP = (
        //        from CPTable in CPVM
        //        select CPTable
        //        ).ToList();
        //    ViewBag.cp = listaCP;
        //    ViewBag.directorio = _context.Directoriojueces.Select(Directoriojueces => Directoriojueces.Area).ToList();
        //    ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

        //    ViewBag.listaCnpp = listaSiNo;
        //    ViewBag.idCnpp = BuscaId(listaSiNo, causapenal.Cnpp);

        //    ViewBag.listaCambio = listaSiNo;
        //    ViewBag.idCambio = BuscaId(listaSiNo, causapenal.Cambio);

        //    #region Distrito
        //    List<SelectListItem> ListaDistrito;
        //    ListaDistrito = new List<SelectListItem>
        //    {
        //      new SelectListItem{ Text="I", Value="I"},
        //      new SelectListItem{ Text="II", Value="II"},
        //      new SelectListItem{ Text="III", Value="III"},
        //      new SelectListItem{ Text="IV", Value="IV"},
        //      new SelectListItem{ Text="V", Value="V"},
        //      new SelectListItem{ Text="VI", Value="VI"},
        //      new SelectListItem{ Text="VII", Value="VII"},
        //      new SelectListItem{ Text="VIII", Value="VIII"},
        //      new SelectListItem{ Text="IX", Value="IX"},
        //      new SelectListItem{ Text="X", Value="X"},
        //      new SelectListItem{ Text="XI", Value="XI"},
        //      new SelectListItem{ Text="XII", Value="XII"},
        //      new SelectListItem{ Text="XIII", Value="XIII"},
        //      new SelectListItem{ Text="XIV", Value="XIV"},
        //      new SelectListItem{ Text="XV", Value="XV"},
        //      new SelectListItem{ Text="XVI", Value="XVI"},

        //    };
        //    ViewBag.listaDistrito = ListaDistrito;
        //    ViewBag.idDistrito = BuscaId(ListaDistrito, causapenal.Distrito);
        //    #endregion

        //    return View(causapenal);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditCausapenal(int id, [Bind("IdCausaPenal, Cnpp, Cambio, Distrito, CausaPenal, Usuario, Fechacreacion")] Causapenal causapenal, string cp, string Cnpp)
        //{
        //    if (id != causapenal.IdCausaPenal)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            causapenal.Cnpp = mg.normaliza(causapenal.Cnpp);
        //            causapenal.Juez = mg.normaliza(causapenal.Juez);
        //            causapenal.Cambio = mg.normaliza(causapenal.Cambio);
        //            causapenal.Distrito = mg.normaliza(causapenal.Distrito);
        //            causapenal.CausaPenal = mg.normaliza(causapenal.CausaPenal);
        //            causapenal.Usuario = mg.normaliza(causapenal.Usuario);
        //            causapenal.Fechacreacion = causapenal.Fechacreacion;
        //            var oldCausapenal = await _context.Causapenal.FindAsync(causapenal.IdCausaPenal);
        //            _context.Entry(oldCausapenal).CurrentValues.SetValues(oldCausapenal);
        //            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
        //            _context.Update(cambiodeobligaciones);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CausapenalExists(causapenal.IdCausaPenal))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return Json(new { success = true, responseText = Url.Action("ListadeCausas", "Causaspenales", new { @id = id }) });

        //        return RedirectToAction("EditCausapenal/" + causapenal.IdCausaPenal, "Causaspenales");
        //    }
        //    return View(causapenal);
        //}
        //#endregion
        #region -EditCausas-
        public async Task<IActionResult> EditCausapenal(int? id)
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

            List<Causapenal> listaCP = new List<Causapenal>();
            List<Causapenal> CPVM = _context.Causapenal.ToList();
            listaCP = (
                from CPTable in CPVM
                select CPTable
                ).ToList();
            ViewBag.cp = listaCP;
            ViewBag.directorio = _context.Directoriojueces.Select(Directoriojueces => Directoriojueces.Area).ToList();
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

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
            List<Historialcp> historialcps = _context.Historialcp.ToList();


            #region -Jointables-
            ViewData["joinTablesCausa"] =
                                     from causapenalTable in causaPenalVM
                                     where causapenalTable.IdCausaPenal == IdCausaPenal
                                     select new CausaDelitoViewModel
                                     {
                                         causaPenalVM = causapenalTable
                                     };

            ViewData["joinTableshistory"] =
                                     from hcp in historialcps
                                     where hcp.CausapenalIdCausapenal == IdCausaPenal
                                     select new CausaDelitoViewModel
                                     {
                                         historialcp = hcp
                                     };
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
        public async Task<IActionResult> EditCausapenal(int id, [Bind("IdCausaPenal,Cnpp,Juez,Cambio,Distrito,CausaPenal")] Causapenal causa, Delito delitoDB, Historialcp historialcp, string cnpp, string juez, string distrito, string cambio, string cp)
        {
            string currentUser = User.Identity.Name;

            if (id == null)
            {
                return NotFound();
            }

            var queryhcp = from hcp in _context.Historialcp
                           where hcp.CausapenalIdCausapenal == id
                           select hcp.Cnpp + " " + hcp.Juez + " " + hcp.Cambio + " " + hcp.Distrito + " " + hcp.Causapenal;


            var quer2 = from g in _context.Historialcp
                        where g.CausapenalIdCausapenal == id
                        select g;

            var queryhcpN = mg.normaliza(cnpp) + " " + mg.normaliza(juez) + " " + mg.normaliza(cambio) + " " + mg.normaliza(distrito) + " " + mg.normaliza(cp);

            var cosine = new Cosine(2);
            double r = 0;
            var simi = false;
            //foreach (var q in queryhcp)
            //{
            //    r = cosine.Similarity(q, queryhcpN);
            //    if (r == 1)
            //    {
            //        simi = true;
            //        break;
            //    }
            //}

            if (ModelState.IsValid)
            {
                try
                {

                    causa.IdCausaPenal = id;
                    causa.Cnpp = cnpp;
                    causa.Juez = mg.normaliza(juez);
                    causa.Distrito = distrito;
                    causa.Cambio = cambio;
                    causa.CausaPenal = mg.normaliza(cp);
                    //causa.CausaPenalCompleta = mg.normaliza(cp) + ", Distrito " + distrito + ", " + juez;

                    #region -Delitos-
                    for (int i = 0; i < datosDelitos.Count; i = i + 2)
                    {
                        if (datosDelitos[i][1] == currentUser)
                        {
                            delitoDB.Tipo = mg.normaliza(datosDelitos[i][0]);
                            delitoDB.Modalidad = mg.normaliza(datosDelitos[i + 1][0]);
                            delitoDB.CausaPenalIdCausaPenal = causa.IdCausaPenal;
                            delitoDB.EspecificarDelito = mg.normaliza(delitoDB.EspecificarDelito);

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
                    var oldCausa = await _context.Causapenal.FindAsync(id);
             
                        if (oldCausa.CausaPenal != causa.CausaPenal || oldCausa.Juez != causa.Juez || oldCausa.Distrito != causa.Distrito)
                        {
                            historialcp.Cnpp = oldCausa.Cnpp;
                            historialcp.Juez = mg.normaliza(oldCausa.Juez);
                            historialcp.Distrito = oldCausa.Distrito;
                            historialcp.Cambio = oldCausa.Cambio;
                            historialcp.Causapenal = mg.normaliza(oldCausa.CausaPenal);
                            historialcp.FechaModificacion = DateTime.Now;
                            historialcp.CausapenalIdCausapenal = id;

                            _context.Add(historialcp);
                            await _context.SaveChangesAsync(null, 1);
                        }
                    


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

                return Json(new { success = true, responseText = Url.Action("ListadeCausas", "Causaspenales", new { @id = id }) });
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
        public async Task<IActionResult> Delito(int id, [Bind("IdDelito,Tipo,Modalidad,CausaPenalIdCausaPenal")] Delito delito)
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
                    delito.Tipo = mg.normaliza(delito.Tipo);
                    delito.Modalidad = mg.normaliza(delito.Modalidad);
                    delito.EspecificarDelito = mg.normaliza(delito.EspecificarDelito);


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

        #region -esAdmin-
        public async Task<bool> esAdmin()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            bool flagCoordinador = false;

            foreach (var rol in roles)
            {
                if (rol == "SupervisorMCSCP" || rol == "AdminMCSCP"|| rol == "Masteradmin")
                {
                    flagCoordinador = true;
                }
            }

            return flagCoordinador;
        }
        #endregion

        public JsonResult Borrar(int id)
        {
            var borrar = false;
  
            var query = (from c in _context.Historialcp
                         where c.IdHistorialcp == id
                         select c).FirstOrDefault();
            try
            {
                borrar = true;
                var causapenals = _context.Historialcp.FirstOrDefault(m => m.IdHistorialcp == id);
                _context.Historialcp.Remove(causapenals);
                _context.SaveChanges();

                return Json(new { success = true, responseText = Url.Action("index", "Personas"), borrar = borrar });
            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("index", "Personas"), borrar = borrar });
            }
        }
    }
}