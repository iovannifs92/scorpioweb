using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Class;
using scorpioweb.Models;

namespace scorpioweb.Controllers
{
    public class CausaspenalesclController : Controller
    {
        private readonly penas2Context _context;

        public CausaspenalesclController(penas2Context context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
        }

        #region -Variables Globales-.
        private readonly UserManager<ApplicationUser> userManager;
        public static List<string> selectedPersona = new List<string>();
        public static List<List<string>> datosDelitos = new List<List<string>>();
        private List<SelectListItem> listaSiNo = new List<SelectListItem>

        {
            new SelectListItem{ Text="Si", Value="SI"},
            new SelectListItem{ Text="No", Value="NO"}
        };
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
        #endregion

        #region -Metodos Generales-
        MetodosGenerales mg = new MetodosGenerales();
        #region -esAdmin-
        public async Task<bool> esAdmin()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            bool flagCoordinador = false;

            foreach (var rol in roles)
            {
                if (rol == "AdminLC" || rol == "SupervisiorLC"|| rol == "Masteradmin")
                {
                    flagCoordinador = true;
                }
            }

            return flagCoordinador;
        }
        #endregion
        #endregion

        // GET: Causaspenalescl
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

            var causa = from p in _context.Causapenalcl
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
            return View(await PaginatedList<Causapenalcl>.CreateAsync(causa.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Causaspenalescl/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var causapenalcl = await _context.Causapenalcl
                .SingleOrDefaultAsync(m => m.IdCausaPenalcl == id);
            if (causapenalcl == null)
            {
                return NotFound();
            }

            return View(causapenalcl);
        }


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


        // GET: Causaspenalescl/Create
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

        // POST: Causaspenalescl/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCausaPenalcl,Cnpp,Juez,Cambio,Distrito,CausaPenal,Fechacreacion,Usuario")] Causapenalcl causapenalcl, Delitocl delitocl)
        {
            var usuario = await userManager.FindByNameAsync(User.Identity.Name);
            String users = usuario.ToString();
            string currentUser = User.Identity.Name;
            causapenalcl.Fechacreacion = DateTime.Now;
            causapenalcl.Usuario = users;
            int idCausa = ((from table in _context.Causapenalcl
                                select table.IdCausaPenalcl).Max()) + 1;
            causapenalcl.IdCausaPenalcl = idCausa;

            if (ModelState.IsValid)
            {
                #region -Delitos-
                for (int i = 0; i < datosDelitos.Count; i = i + 2)
                {
                    if (datosDelitos[i][1] == currentUser)
                    {
                        delitocl.Tipo = mg.normaliza(datosDelitos[i][0]);
                        delitocl.Modalidad = mg.normaliza(datosDelitos[i + 1][0]);
                        delitocl.CausaPenalclIdCausaPenalcl = idCausa;
                        delitocl.EspecificarDelito = mg.normaliza(delitocl.EspecificarDelito);

                        _context.Add(delitocl);
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


                _context.Add(causapenalcl);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(causapenalcl);
        }

        // GET: Causaspenalescl/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var causapenalcl = await _context.Causapenalcl.SingleOrDefaultAsync(m => m.IdCausaPenalcl == id);
            if (causapenalcl == null)
            {
                return NotFound();
            }

            List<Causapenalcl> listaCP = new List<Causapenalcl>();
            List<Causapenalcl> CPVM = _context.Causapenalcl.ToList();
            listaCP = (
                from CPTable in CPVM
                select CPTable
                ).ToList();
            ViewBag.cp = listaCP;
            ViewBag.directorio = _context.Directoriojueces.Select(Directoriojueces => Directoriojueces.Area).ToList();
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

            ViewBag.listaCnpp = listaSiNo;
            ViewBag.idCnpp = BuscaId(listaSiNo, causapenalcl.Cnpp);

            ViewBag.listaCambio = listaSiNo;
            ViewBag.idCambio = BuscaId(listaSiNo, causapenalcl.Cambio);

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
            ViewBag.idDistrito = BuscaId(ListaDistrito, causapenalcl.Distrito);
            #endregion

            List<Delitocl> delitoVM = _context.Delitocl.ToList();
            List<Causapenalcl> causaPenalVM = _context.Causapenalcl.ToList();

            ViewData["joinTablesCausaDelito"] =
                                     from causapenalTable in causaPenalVM
                                     join delitoTabla in delitoVM on causapenalcl.IdCausaPenalcl equals delitoTabla.CausaPenalclIdCausaPenalcl
                                     where causapenalTable.IdCausaPenalcl == id
                                     select new CausaclDelitoclViewModel
                                     {
                                         delitoVM = delitoTabla,
                                         causaPenalVM = causapenalTable

                                     };


            if ((ViewData["joinTablesCausaDelito"] as IEnumerable<scorpioweb.Models.CausaclDelitoclViewModel>).Count() == 0)
            {
                ViewBag.tieneDelitos = false;
            }
            else
            {
                ViewBag.tieneDelitos = true;
            }



            return View(causapenalcl);
        }

        // POST: Causaspenalescl/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCausaPenalcl,Cnpp,Juez,Cambio,Distrito,CausaPenal,Fechacreacion,Usuario")] Causapenalcl causapenalcl, Delitocl delitoDB)
        {
            string currentUser = User.Identity.Name;
            if (id != causapenalcl.IdCausaPenalcl)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    #region -Delitos-
                    for (int i = 0; i < datosDelitos.Count; i = i + 2)
                    {
                        if (datosDelitos[i][1] == currentUser)
                        {
                            delitoDB.Tipo = mg.normaliza(datosDelitos[i][0]);
                            delitoDB.Modalidad = mg.normaliza(datosDelitos[i + 1][0]);
                            delitoDB.CausaPenalclIdCausaPenalcl = causapenalcl.IdCausaPenalcl;
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
                    causapenalcl.Juez =mg.normaliza(causapenalcl.Juez);
                    causapenalcl.CausaPenal =mg.normaliza(causapenalcl.CausaPenal);

                    _context.Update(causapenalcl);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CausapenalclExists(causapenalcl.IdCausaPenalcl))
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
            return View(causapenalcl);
        }

        #region -Delete-
        public JsonResult antesdelete(Causapenalcl causapenalcl, Personaclcausapenalcl personaclcausapenalcl, string[] datocp)
        {
            var borrar = false;
            var id = Int32.Parse(datocp[0]);

            var query = (from c in _context.Causapenalcl
                         where c.IdCausaPenalcl == id
                         select c).FirstOrDefault();

            var antesdel = from pc in _context.Personaclcausapenalcl
                           where pc.CausaPenalclIdCausaPenalcl == id
                           select pc;

            if (antesdel.Any())
            {
                return Json(new { success = true, responseText = Url.Action("Index", "Causaspenalescl"), borrar = borrar });
            }
            else
            {
                borrar = true;
                return Json(new { success = true, responseText = Url.Action("Index", "Causaspenalescl"), borrar = borrar });
            }
        }


        public JsonResult deletecp(Causapenalcl causapenalcl, Historialeliminacion historialeliminacion, string[] datocp)
        {
            var borrar = false;
            var id = Int32.Parse(datocp[0]);
            var razon = mg.normaliza(datocp[1]);
            var user = mg.normaliza(datocp[2]);

            var query = (from c in _context.Causapenalcl
                         where c.IdCausaPenalcl == id
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

                var causapenals = _context.Causapenalcl.FirstOrDefault(m => m.IdCausaPenalcl == id);
                _context.Causapenalcl.Remove(causapenals);
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var causapenal = await _context.Causapenalcl
                .SingleOrDefaultAsync(m => m.IdCausaPenalcl == id);
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
            var causapenal = await _context.Causapenalcl.SingleOrDefaultAsync(m => m.IdCausaPenalcl == id);
            _context.Causapenalcl.Remove(causapenal);
            await _context.SaveChangesAsync(null, 1);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region -Asignacion-
        public async Task<IActionResult> Asignacion(int? id, string cp)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.CausaPenal = cp;
            var user = await userManager.FindByNameAsync(User.Identity.Name);

            List<Personacl> listaPersonas = new List<Personacl>();
            List<Personaclcausapenalcl> listaPersonasAsignadas = new List<Personaclcausapenalcl>();
            List<Personacl> personaVM = _context.Personacl.ToList();
            List<Personaclcausapenalcl> personaCausaPenalVM = _context.Personaclcausapenalcl.ToList();

            listaPersonasAsignadas = (
                from personaCausaPenalTable in personaCausaPenalVM
                where personaCausaPenalTable.CausaPenalclIdCausaPenalcl == id
                select personaCausaPenalTable
                ).ToList();

            if (await esAdmin())
            {
                listaPersonas = (
                from personaTable in personaVM
                where listaPersonasAsignadas.All(
                    per => per.PersonaclIdPersonacl != personaTable.IdPersonaCl
                    )
                orderby personaTable.Paterno
                select personaTable
                ).ToList();
            }
            else
            {
                listaPersonas = (
                from personaTable in personaVM
                where listaPersonasAsignadas.All(
                    per => per.PersonaclIdPersonacl != personaTable.IdPersonaCl
                    )
                && personaTable.Supervisor == user.ToString()
                orderby personaTable.Paterno
                select personaTable
                ).ToList();
            }
            ViewBag.personas = listaPersonas;

            selectedPersona = new List<string>();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignacion(Personaclcausapenalcl personaclcausapenalcl, string PersonaAsignada, Supervisioncl supervisioncl, Suspensionseguimientocl suspensionseguimientocl,Planeacionestrategicacl planeacionestrategicacl, Cierredecasocl cierredecasocl, Cambiodeobligacionescl cambiodeobligacionescl, Revocacioncl revocacioncl, Beneficios beneficios, Victimacl victimacl, int id/*, int persona_idPersona*/)
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

                int idPersonaCausaPenal = ((from table in _context.Personaclcausapenalcl
                                            select table.IdPersonaclCausapenalcl).Max()) + 1;


                #region -AddPesonaCausapenal-
                if (idpersona != 0)
                {

                    personaclcausapenalcl.PersonaclIdPersonacl = Convert.ToInt32(idpersona);
                    personaclcausapenalcl.CausaPenalclIdCausaPenalcl = id;
                    personaclcausapenalcl.IdPersonaclCausapenalcl = idPersonaCausaPenal;
                    _context.Add(personaclcausapenalcl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                }

                #endregion

                #region agregar 1 entrada a Supervision
                int idSupervision = ((from table in _context.Supervisioncl
                                      select table.IdSupervisioncl).Max()) + 1;
                supervisioncl.IdSupervisioncl = idSupervision;
                supervisioncl.PersonaclIdPersonacl = idpersona;
                supervisioncl.CausaPenalclIdCausaPenalcl = id;
                supervisioncl.EstadoSupervision = "VIGENTE";
                supervisioncl.Tta = "NO";
                #endregion

                #region agregar 1 entrada a Suspensionseguimiento
                int idSuspensionSeguimiento = ((from table in _context.Suspensionseguimientocl
                                                select table.IdSuspensionSeguimientocl).Max()) + 1;
                suspensionseguimientocl.IdSuspensionSeguimientocl = idSuspensionSeguimiento;
                suspensionseguimientocl.SupervisionclIdSupervisioncl = idSupervision;
                #endregion

                #region agregar 1 entrada a Planeacionestrategica
                int idPlaneacionestrategica = ((from table in _context.Planeacionestrategicacl
                                                select table.IdPlaneacionEstrategicacl).Max()) + 1;
                planeacionestrategicacl.IdPlaneacionEstrategicacl = idPlaneacionestrategica;
                planeacionestrategicacl.SupervisionclIdSupervisioncl = idSupervision;
                #endregion

                #region agregar 1 entrada a Cierredecaso
                int idCierredecaso = ((from table in _context.Cierredecasocl
                                       select table.IdCierreDeCasocl).Max()) + 1;
                cierredecasocl.IdCierreDeCasocl = idCierredecaso;
                cierredecasocl.SupervisionclIdSupervisioncl = idSupervision;
                #endregion

                #region agregar 1 entrada a Cambiodeobligaciones
                int idCambiodeobligaciones = ((from table in _context.Cambiodeobligacionescl
                                               select table.SupervisionclIdSupervisioncl).Max()) + 1;
                cambiodeobligacionescl.IdCambiodeObligacionescl = idCambiodeobligaciones;
                cambiodeobligacionescl.SupervisionclIdSupervisioncl = idSupervision;
                #endregion

                #region agregar 1 entrada a Revocacion
                int idRevocacion = ((from table in _context.Revocacioncl
                                     select table.IdRevocacioncl).Max()) + 1;
                revocacioncl.IdRevocacioncl = idRevocacion;
                revocacioncl.SupervisionclIdSupervisioncl = idSupervision;
                #endregion


                _context.Add(supervisioncl);
                await _context.SaveChangesAsync(null, 1);
                //Guardar en 2 partes para satisfacer la restriccion de las llaves foraneas
                _context.Add(suspensionseguimientocl);
                _context.Add(planeacionestrategicacl);
                _context.Add(cierredecasocl);
                _context.Add(cambiodeobligacionescl);
                _context.Add(revocacioncl);
                await _context.SaveChangesAsync(null, 1);
                return RedirectToAction("Supervision", "Supervisiones", new { @id = idSupervision });
            }
            return View(personaclcausapenalcl);
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

        #region -PersonaAsignadas-

        public async Task<IActionResult> PersonaAsignadas(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var causapenal = await _context.Causapenalcl
                .SingleOrDefaultAsync(m => m.IdCausaPenalcl == id);

            #region -To List databases-

            List<Personacl> personaVM = _context.Personacl.ToList();
            List<Personaclcausapenalcl> personaCausaPenalVM = _context.Personaclcausapenalcl.ToList();
            List<Causapenalcl> causaPenalVM = _context.Causapenalcl.ToList();

            #endregion

            #region -Jointables-
            ViewData["joinTables"] = from personaTable in personaVM
                                     join personaCausaPenalTable in personaCausaPenalVM on personaTable.IdPersonaCl equals personaCausaPenalTable.PersonaclIdPersonacl
                                     join causaPenalTable in causaPenalVM on personaCausaPenalTable.CausaPenalclIdCausaPenalcl equals causaPenalTable.IdCausaPenalcl
                                     where causaPenalTable.IdCausaPenalcl == id
                                     orderby personaTable.Paterno
                                     select new PersonaclCausaPenalclViewModel
                                     {
                                         personaclVM = personaTable,
                                         causaPenalclVM = causaPenalTable
                                     };
            #endregion

            if (causapenal == null)
            {
                return NotFound();
            }

            return View();
        }
        #endregion
        #region -Delito-
        public async Task<IActionResult> EditDelito(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var delito = await _context.Delitocl.SingleOrDefaultAsync(m => m.IdDelitocl == id);
            if (delito == null)
            {
                return NotFound();
            }
            return View(delito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDelito(int id, [Bind("IdDelitocl,Tipo,Modalidad,CausaPenalclIdCausaPenalcl")] Delitocl delito)
        {
            if (id != delito.IdDelitocl)
            {
                return NotFound();
            }
            var idcausa = delito.CausaPenalclIdCausaPenalcl;

            if (ModelState.IsValid)
            {
                try
                {
                    delito.Tipo = mg.normaliza(delito.Tipo);
                    delito.Modalidad = mg.normaliza(delito.Modalidad);
                    delito.EspecificarDelito = mg.normaliza(delito.EspecificarDelito);
                    var oldDelito = await _context.Delitocl.FindAsync(id);
                    _context.Entry(oldDelito).CurrentValues.SetValues(delito);
                    //_context.Update(delito);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DelitoclExists(delito.IdDelitocl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Causaspenalescl", new { @id = idcausa });
            }
            return View();
        }
        #endregion


        private bool CausapenalclExists(int id)
        {
            return _context.Causapenalcl.Any(e => e.IdCausaPenalcl == id);
        }
        private bool DelitoclExists(int id)
        {
            return _context.Delitocl.Any(e => e.IdDelitocl == id);
        }
    }
}
