using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using F23.StringSimilarity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.Edm.Library.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Remotion.Linq.Clauses;
using Remotion.Linq.Utilities;
using scorpioweb.Class;
using scorpioweb.Migrations.ApplicationDb;
using scorpioweb.Models;
using Syncfusion.EJ2.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;




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
        MetodosGenerales mg = new MetodosGenerales();
        public ReinsercionController(penas2Context context, IHostingEnvironment hostingEnvironment,
                                  RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IHubContext<HubNotificacion> hubContext)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
            _hubContext = hubContext;

        }

        #region Lista general
        private List<SelectListItem> listaEstadoRe = new List<SelectListItem>
        {
            new SelectListItem{ Text="ACTIVO", Value="ACTIVO"},
            new SelectListItem{ Text="CONCLUIDO", Value="CONCLUIDO"}
        };

        private List<SelectListItem> listaOficios = new List<SelectListItem>
        {
           new SelectListItem { Text = "Seleccione", Value = "" },
            new SelectListItem { Text = "Informe Alta", Value = "INFORME ALTA" },
            new SelectListItem { Text = "Informe", Value = "INFORME" },
            new SelectListItem { Text = "Informe de Asistencia", Value = "INFORME DE ASISTENCIA" },
            new SelectListItem { Text = "Solicitud de Informe", Value = "SOLICITUD DE INFORME" },
            new SelectListItem { Text = "Cancelación de Vinculación", Value = "CANCELACIÓN DE VINCULACIÓN" },
            new SelectListItem { Text = "Ficha de Antidoping", Value = "FICHA DE ANTIDOPING" },
            new SelectListItem { Text = "Resultado de Antidoping", Value = "RESULTADO DE ANTIDOPING" },
            new SelectListItem { Text = "Oficio Antidoping", Value = "OFICIO ANTIDOPING" }
        };

        Dictionary<string, List<string>> ListaDinamicaEstados = new Dictionary<string, List<string>>
        {
            { "Terapia", new List<string> { "Activo", "Alta(Concluido)", "Baja", "Espera", "Termino" } },
            { "Jornada", new List<string> { "Activo", "Cancelado", "Concluido", "Cumplió", "En espera" } },
            { "Educativa", new List<string> { "Activo", "Cancelado", "Certificado", "En espera" } },
            { "Laboral", new List<string> { "Activo", "Cancelado", "Concluido", "Contratado", "En espera" } },
            { "Extraordinarios", new List<string> { "Activo", "Cancelado", "Concluido", "En espera", "Satisfactorio" } },
            { "Antidoping", new List<string> { "Concluido", "En trámite", "No realizado" } }
        };

        private static readonly Dictionary<string, List<string>> ListaTipoInforme = new Dictionary<string, List<string>>
        {
            { "Terapia", new List<string> { "Informe", "Informe alta", "Informe Asistencia", "Ficha de resultados" } },
            { "Otro", new List<string> { "Solicitud de Informe", "Cancelación de Vinculación", "Oficio de vinculación", "Ficha de Antidoping", "Resultado Antidoping", "Oficio Antidoping" } }
        };
        #endregion

        #region - index-
        // GET: Reinsercion
        public async Task<IActionResult> Index(
            string sortOrder,
           string currentFilter,
           string searchString,
           string estadoSuper,
           string figuraJudicial,
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
            ViewBag.Vinculacion = true;
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            var fechaActualizacionLimite = new DateTime(2023, 12, 30, 0, 0, 0);


            //var query = (from persona in _context.Persona
            //             join supervision in _context.Supervision on persona.IdPersona equals supervision.PersonaIdPersona into s_join
            //             from s in s_join.DefaultIfEmpty()
            //             join causapenal in _context.Causapenal on s.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal into cp_join
            //             from cp in cp_join.DefaultIfEmpty()
            //             join delito in _context.Delito on cp.IdCausaPenal equals delito.CausaPenalIdCausaPenal into dd_join
            //             from dd in dd_join.DefaultIfEmpty()
            //             where (s.EstadoSupervision == null || s.EstadoSupervision != "CONCLUIDO")
            //             select new ReinsercionMCYSCPLCCURSVM
            //             {
            //                 IdTabla = persona.IdPersona.ToString(),
            //                 Nombre = persona.Paterno + " " + persona.Materno + " " + persona.Nombre,
            //                 Causapenal = cp.CausaPenal,
            //                 Delito = dd.Tipo,
            //                 NomTabla = "MCySCP",
            //                 tabla = "persona",
            //                 Vinculacion = "1",
            //                 //EstadoSupervision = s.EstadoSupervision,
            //                 //ClaveUnica = persona.ClaveUnicaScorpio,
            //                 Supervisor = persona.Supervisor
            //             }).Distinct().
            //             Union(
            //                from personacl in _context.Personacl
            //                join supervisioncl in _context.Supervisioncl on personacl.IdPersonaCl equals supervisioncl.PersonaclIdPersonacl into scl_join
            //                from scl in scl_join.DefaultIfEmpty()
            //                join causapenalcl in _context.Causapenalcl on scl.CausaPenalclIdCausaPenalcl equals causapenalcl.IdCausaPenalcl into cpcl_join
            //                from cpcl in cpcl_join.DefaultIfEmpty()
            //                join delitocl in _context.Delitocl on cpcl.IdCausaPenalcl equals delitocl.CausaPenalclIdCausaPenalcl into dcl_join
            //                from dcl in dcl_join.DefaultIfEmpty()
            //                join expedienteunico in _context.Expedienteunico on personacl.IdPersonaCl.ToString() equals expedienteunico.Persona into ex_join
            //                from ex in ex_join.DefaultIfEmpty()
            //                join ejecucion in _context.Ejecucion on ex.Ejecucion equals ejecucion.IdEjecucion.ToString() into ep_join
            //                from ep in ep_join.DefaultIfEmpty()
            //                join epcausapenal in _context.Epcausapenal on ep.IdEjecucion equals epcausapenal.EjecucionIdEjecucion into epcp_join
            //                from epcp in epcp_join.DefaultIfEmpty()
            //                where (scl.EstadoSupervision == null || scl.EstadoSupervision != "CONCLUIDO")
            //                select new ReinsercionMCYSCPLCCURSVM
            //                {
            //                    IdTabla = personacl.IdPersonaCl.ToString(),
            //                    Nombre = personacl.Paterno + " " + personacl.Materno + " " + personacl.Nombre,
            //                    Causapenal = epcp.Causapenal ?? cpcl.CausaPenal,
            //                    Delito = epcp.Delito ?? dcl.Tipo,
            //                    NomTabla = "Libertad Condicionada",
            //                    tabla = "personacl",
            //                    Vinculacion = "1",
            //                    //EstadoSupervision = scl.EstadoSupervision,
            //                    //ClaveUnica = personacl.ClaveUnicaScorpio,
            //                    Supervisor = personacl.Supervisor
            //                }).Distinct();
            //query = query.Where(t1 => !_context.Reinsercion.Any(r => (r.Tabla == "persona" && r.IdTabla == t1.IdTabla.ToString())))
            //  .OrderBy(r => r.NombreCompleto)
            //  .Select(t1 => new ReinsercionMCYSCPLCCURSVM
            //  {
            //      IdTabla = t1.IdTabla,
            //      Nombre = t1.Nombre,
            //      Causapenal = t1.Causapenal,
            //      Delito = t1.Delito,
            //      NomTabla = t1.NomTabla,
            //      tabla = t1.tabla,
            //      Supervisor = t1.Supervisor,
            //      Vinculacion = "1"
            //  });



            var query = (from persona in _context.Persona
                         join supervision in _context.Supervision on persona.IdPersona equals supervision.PersonaIdPersona into s_join
                         from s in s_join.DefaultIfEmpty()
                         join causapenal in _context.Causapenal on s.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal into cp_join
                         from cp in cp_join.DefaultIfEmpty()
                         join delito in _context.Delito on cp.IdCausaPenal equals delito.CausaPenalIdCausaPenal into dd_join
                         from dd in dd_join.DefaultIfEmpty()
                         where (s.EstadoSupervision == null || s.EstadoSupervision != "CONCLUIDO")
                         group new ReinsercionMCYSCPLCCURSVM
                         {
                             IdTabla = persona.IdPersona.ToString(),
                             Nombre = persona.Paterno + " " + persona.Materno + " " + persona.Nombre,
                             Causapenal = cp.CausaPenal,
                             Delito = dd.Tipo,
                             NomTabla = "MCySCP",
                             tabla = "persona",
                             //EstadoSupervision = s.EstadoSupervision,
                             //ClaveUnica = persona.ClaveUnicaScorpio,
                             Supervisor = persona.Supervisor
                         } by new { persona.IdPersona, persona.Paterno, persona.Materno, persona.Nombre } into g
                         select g.FirstOrDefault()
                    ).Union(from personacl in _context.Personacl
                            join supervisioncl in _context.Supervisioncl on personacl.IdPersonaCl equals supervisioncl.PersonaclIdPersonacl into scl_join
                            from scl in scl_join.DefaultIfEmpty()
                            join causapenalcl in _context.Causapenalcl on scl.CausaPenalclIdCausaPenalcl equals causapenalcl.IdCausaPenalcl into cpcl_join
                            from cpcl in cpcl_join.DefaultIfEmpty()
                            join delitocl in _context.Delitocl on cpcl.IdCausaPenalcl equals delitocl.CausaPenalclIdCausaPenalcl into dcl_join
                            from dcl in dcl_join.DefaultIfEmpty()
                            join expedienteunico in _context.Expedienteunico on personacl.IdPersonaCl.ToString() equals expedienteunico.Persona into ex_join
                            from ex in ex_join.DefaultIfEmpty()
                            join ejecucion in _context.Ejecucion on ex.Ejecucion equals ejecucion.IdEjecucion.ToString() into ep_join
                            from ep in ep_join.DefaultIfEmpty()
                            join epcausapenal in _context.Epcausapenal on ep.IdEjecucion equals epcausapenal.EjecucionIdEjecucion into epcp_join
                            from epcp in epcp_join.DefaultIfEmpty()
                            where (scl.EstadoSupervision == null || scl.EstadoSupervision != "CONCLUIDO")
                            group new ReinsercionMCYSCPLCCURSVM
                            {
                                IdTabla = personacl.IdPersonaCl.ToString(),
                                Nombre = personacl.Paterno + " " + personacl.Materno + " " + personacl.Nombre,
                                Causapenal = epcp.Causapenal ?? cpcl.CausaPenal,
                                Delito = epcp.Delito ?? dcl.Tipo,
                                NomTabla = "Libertad Condicionada",
                                tabla = "personacl",
                                //EstadoSupervision = scl.EstadoSupervision,
                                //ClaveUnica = personacl.ClaveUnicaScorpio,
                                Supervisor = personacl.Supervisor
                            } by new { personacl.IdPersonaCl, personacl.Paterno, personacl.Materno, personacl.Nombre } into g
                            select g.FirstOrDefault()
                            ).Where(t1 => !_context.Reinsercion.Any(r => (r.Tabla == "persona" && r.IdTabla == t1.IdTabla.ToString()))).OrderBy(r => r.NombreCompleto)
                            .Select(t1 => new ReinsercionMCYSCPLCCURSVM
                            {
                                IdTabla = t1.IdTabla,
                                Nombre = t1.Nombre,
                                Causapenal = t1.Causapenal,
                                Delito = t1.Delito,
                                NomTabla = t1.NomTabla,
                                tabla = t1.tabla,
                                //EstadoSupervision = t1.EstadoSupervision,
                                //ClaveUnica = t1.ClaveUnica,
                                Supervisor = t1.Supervisor
                            });

            var tamano = await query.CountAsync();

            // Ejecutar la consulta


            ViewData["CurrentFilter"] = searchString;
            ViewData["EstadoS"] = estadoSuper;
            ViewData["FiguraJ"] = figuraJudicial;

            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => (x.Paterno + " " + x.Materno + " " + x.Nombre).Contains(searchString.ToUpper()) ||
                                              (x.Nombre + " " + x.Paterno + " " + x.Materno).Contains(searchString.ToUpper()) ||
                                              (x.IdTabla).ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(x => x.Paterno);
                    break;
                case "causa_penal_desc":
                    query = query.OrderByDescending(x => x.Materno);
                    break;
                case "estado_cumplimiento_desc":
                    query = query.OrderByDescending(x => x.Nombre);
                    break;
                default:
                    query = query.OrderByDescending(x => x.Materno);
                    break;
            }
            int pageSize = 15;
            return View(await PaginatedList<ReinsercionMCYSCPLCCURSVM>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Reinsercion
        public async Task<IActionResult> Reinsercion(
            string sortOrder,
           string currentFilter,
           string searchString,
           string estadoSuper,
           string figuraJudicial,
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

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            List<Canalizacion> queryFracciones = (from f in _context.Canalizacion
                                                  group f by f.ReincercionIdReincercion into grp
                                                  select grp.OrderByDescending(f => f.IdCanalizacion).FirstOrDefault()).ToList();

            var query = (from r in _context.Reinsercion
                         join p in _context.Personacl on r.IdTabla equals p.IdPersonaCl.ToString()
                         join d in _context.Domiciliocl on p.IdPersonaCl equals d.PersonaclIdPersonacl
                         join e in _context.Estudioscl on p.IdPersonaCl equals e.PersonaClIdPersonaCl
                         join t in _context.Trabajocl on p.IdPersonaCl equals t.PersonaClIdPersonaCl
                         join a in _context.Actividadsocialcl on p.IdPersonaCl equals a.PersonaClIdPersonaCl
                         join s in _context.Saludfisicacl on p.IdPersonaCl equals s.PersonaClIdPersonaCl
                         join pv in _context.Psicologiavincu on p.IdPersonaCl equals pv.PersonaClIdPersonaCl into pvGroup
                         from pv in pvGroup.DefaultIfEmpty()
                         join ex in _context.Expedienteunico on p.IdPersonaCl.ToString() equals ex.Personacl into exGroup
                         from ex in exGroup.DefaultIfEmpty()
                         join ep in _context.Ejecucion on ex.Ejecucion equals ep.IdEjecucion.ToString() into epGroup
                         from ep in epGroup.DefaultIfEmpty()
                         join epcp in _context.Epcausapenal on ep.IdEjecucion equals epcp.EjecucionIdEjecucion into epcpGroup
                         from epcp in epcpGroup.DefaultIfEmpty()
                         join scl in _context.Supervisioncl on p.IdPersonaCl equals scl.PersonaclIdPersonacl into sclGroup
                         from scl in sclGroup.DefaultIfEmpty()
                         join cpcl in _context.Causapenalcl on scl.CausaPenalclIdCausaPenalcl equals cpcl.IdCausaPenalcl into cpclGroup
                         from cpcl in cpclGroup.DefaultIfEmpty()
                         join dcl in _context.Delitocl on cpcl.IdCausaPenalcl equals dcl.CausaPenalclIdCausaPenalcl into dclGroup
                         from dcl in dclGroup.DefaultIfEmpty()
                         join cana in queryFracciones on r.IdReinsercion equals cana.ReincercionIdReincercion
                         where r.Tabla == "personacl"
                         group new { r.IdReinsercion, r.IdTabla, p.IdPersonaCl, p.Paterno, p.Materno, p.Nombre, r.Estado, scl.EstadoSupervision, ep.Ce, cpcl.CausaPenal, dcl.Tipo, epcp.Delito } by new { r.IdReinsercion, r.IdTabla } into g
                         select new ReinsercionMCYSCPLCCURSVM
                         {
                             IdReinsercion = g.Key.IdReinsercion,
                             IdTabla = g.Key.IdTabla,
                             Nombre = string.Concat(g.FirstOrDefault().Paterno, " ", g.FirstOrDefault().Materno, " ", g.FirstOrDefault().Nombre),
                             Causapenal = g.FirstOrDefault().CausaPenal ?? g.FirstOrDefault().CausaPenal,
                             Delito = g.FirstOrDefault().Delito ?? g.FirstOrDefault().Tipo,
                             EstadoVinculacion = g.FirstOrDefault().Estado,
                             NomTabla = "Libertad Condicionada",
                             EstadoSupervision = g.FirstOrDefault().EstadoSupervision
                         }).Union(
                          from r in _context.Reinsercion
                          join p in _context.Persona on r.IdTabla equals p.IdPersona.ToString()
                          join d in _context.Domicilio on p.IdPersona equals d.PersonaIdPersona
                          join e in _context.Estudios on p.IdPersona equals e.PersonaIdPersona
                          join t in _context.Trabajo on p.IdPersona equals t.PersonaIdPersona
                          join a in _context.Actividadsocial on p.IdPersona equals a.PersonaIdPersona
                          join s in _context.Saludfisica on p.IdPersona equals s.PersonaIdPersona
                          join ex in _context.Expedienteunico on p.IdPersona.ToString() equals ex.Personacl into exGroup
                          from ex in exGroup.DefaultIfEmpty()
                          join ep in _context.Ejecucion on ex.Ejecucion equals ep.IdEjecucion.ToString() into epGroup
                          from ep in epGroup.DefaultIfEmpty()
                          join epcp in _context.Epcausapenal on ep.IdEjecucion equals epcp.EjecucionIdEjecucion into epcpGroup
                          from epcp in epcpGroup.DefaultIfEmpty()
                          join scl in _context.Supervision on p.IdPersona equals scl.PersonaIdPersona into sclGroup
                          from scl in sclGroup.DefaultIfEmpty()
                          join cpcl in _context.Causapenal on scl.CausaPenalIdCausaPenal equals cpcl.IdCausaPenal into cpclGroup
                          from cpcl in cpclGroup.DefaultIfEmpty()
                          join dcl in _context.Delitocl on cpcl.IdCausaPenal equals dcl.CausaPenalclIdCausaPenalcl into dclGroup
                          from dcl in dclGroup.DefaultIfEmpty()
                          where r.Tabla == "persona"
                          group new { r.IdReinsercion, r.IdTabla, p.IdPersona, p.Paterno, p.Materno, p.Nombre, r.Estado, scl.EstadoSupervision, ep.Ce, cpcl.CausaPenal, dcl.Tipo, epcp.Delito } by new { r.IdReinsercion, r.IdTabla } into g
                          select new ReinsercionMCYSCPLCCURSVM
                          {
                              IdReinsercion = g.Key.IdReinsercion,
                              IdTabla = g.Key.IdTabla,
                              Nombre = string.Concat(g.FirstOrDefault().Paterno, " ", g.FirstOrDefault().Materno, " ", g.FirstOrDefault().Nombre),
                              Causapenal = g.FirstOrDefault().CausaPenal,
                              Delito = g.FirstOrDefault().Tipo,
                              EstadoVinculacion = g.FirstOrDefault().Estado,
                              NomTabla = "MCYSCP",
                              EstadoSupervision = g.FirstOrDefault().EstadoSupervision
                          }).Union(
                          from r in _context.Reinsercion
                          join e in _context.Externados on Int32.Parse(r.IdTabla) equals e.Idexternados
                          where r.Tabla == "externados" || r.Tabla == "EXTERNADOS"
                          select new ReinsercionMCYSCPLCCURSVM
                          {
                              IdReinsercion = r.IdReinsercion,
                              IdTabla = r.IdTabla,
                              Nombre = string.Concat(e.APaterno, " ", e.AMaterno, " ", e.Nombre),
                              Causapenal = e.CausaPenal,
                              Delito = e.Delito,
                              EstadoVinculacion = r.Estado,
                              NomTabla = "Externados",
                              EstadoSupervision = "Sin supervision"
                          });





            var result = query.ToList();

            ViewData["CurrentFilter"] = searchString;
            ViewData["EstadoS"] = estadoSuper;
            ViewData["FiguraJ"] = figuraJudicial;

            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => (x.Nombre).Contains(searchString.ToUpper()) ||
                                              (x.IdTabla).ToString().Contains(searchString));
            }

            query.OrderBy(x => x.IdTabla);

            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(x => x.Nombre);
                    break;
                case "causa_penal_desc":
                    query = query.OrderByDescending(x => x.Nombre);
                    break;
                case "estado_cumplimiento_desc":
                    query = query.OrderByDescending(x => x.Nombre);
                    break;
                default:
                    query = query.OrderByDescending(x => x.Nombre);
                    break;
            }
            int pageSize = 15;
            return View(await PaginatedList<ReinsercionMCYSCPLCCURSVM>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        #endregion

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

        #region -Añadir Externado-
        public IActionResult AgregarExternado()
        {
            List<Estados> listaEstados = new List<Estados>();
            listaEstados = (from table in _context.Estados
                            select table).ToList();
            ViewBag.ListadoEstados = listaEstados;

            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearExternado(Externados externado)
        {
            // VERIFICACION DE DATOS 
            if (externado == null)
                return Json(new { success = false, message = "Datos de externado vacíos! método: CrearExternado" });
            else if (string.IsNullOrEmpty(externado.Nombre) || string.IsNullOrEmpty(externado.APaterno) || string.IsNullOrEmpty(externado.AMaterno))
                return Json(new { success = false, message = "Nombre(s) o apellidos de la persona vacios!" });
            else if (string.IsNullOrEmpty(externado.FechaNacimiento))
                return Json(new { success = false, message = "Fecha de nacimiento vacia!" });
            else if (externado.Edad == 0)
                return Json(new { success = false, message = "Verifica la edad!" });
            else if (externado.Sexo.Equals("Seleccione una opción"))
                return Json(new { success = false, message = "Selecciona un genero!" });
            else if (externado.LnEstado.Equals("0"))
                return Json(new { success = false, message = "Elige un estado de nacimiento!" });
            else if (string.IsNullOrEmpty(externado.ClaveUnicaScorpio) || string.IsNullOrEmpty(externado.Curp))
                return Json(new { success = false, message = "CURP vacia!" });
            else if (string.IsNullOrEmpty(externado.CausaPenal))
                return Json(new { success = false, message = "Causa penal vacia!" });
            else if (string.IsNullOrEmpty(externado.Delito))
                return Json(new { success = false, message = "Delito vacio!" });
            try
            {
                if (string.IsNullOrEmpty(externado.Observaciones))
                    externado.Observaciones = "NA";
                _context.Externados.Add(externado);
                await _context.SaveChangesAsync();
                int IdUltimoRegistro = await _context.Externados.OrderByDescending(e => e.Idexternados).Select(e => e.Idexternados).FirstOrDefaultAsync();
                string IdGenerado = IdUltimoRegistro.ToString();
                string NombreTabla = "externados";
                string NombreCompleto = await _context.Externados.OrderByDescending(e => e.Idexternados).Select(e => e.Nombre + e.APaterno + e.AMaterno).FirstOrDefaultAsync();
                return Json(new { success = true, message = "Externado creado exitosamente!", idTabla = IdGenerado, tabla = NombreTabla, nombrePersona = NombreCompleto });
            }
            catch (DbUpdateException ex)
            {
                return Json(new { error = true, message = "ERROR" + ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "ERROR GENERAL: " + ex.Message });
            }
        }

        #endregion

        #region - Crear Reinsercion -

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

            List<Persona> listap = new List<Persona>();
            List<Personacl> listapcl = new List<Personacl>();

            if (ModelState.IsValid)
            {
                //Sacar info de su tabla

                List<object> miLista = new List<object>(); // Esta es una función hipotética que obtiene los datos

                var persona = _context.Persona
                                   .SingleOrDefault(m => m.IdPersona == Int32.Parse(reinsercion.IdTabla));

                var personacl = _context.Personacl
                                  .SingleOrDefault(m => m.IdPersonaCl == Int32.Parse(reinsercion.IdTabla));


                _context.Add(reinsercion);
                await _context.SaveChangesAsync();


                foreach (var rol in roles)
                {
                    if (rol != "Vinculacion")
                    {
                        await _hubContext.Clients.Group("nuevaCanalizacion").SendAsync("sendMessage", persona.IdPersona + " " + persona.NombreCompleto);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reinsercion);
        }

        [HttpPost]
        public async Task<JsonResult> CrearReinsercionPorSupervisor([FromBody] Reinsercion reinsercion)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            if (ModelState.IsValid)
            {
                foreach (var rol in roles)
                {
                    if (rol != "Vinculacion")
                    {
                        await _hubContext.Clients.Group("nuevaCanalizacion").SendAsync("sendMessage", "Hay una nueva canalizacion");
                    }
                }

                int idReinsercionObtenido = await ObtenerIdReinsercionAsync(reinsercion.IdTabla, reinsercion.Tabla);

                if (idReinsercionObtenido == 0)
                    idReinsercionObtenido = await CrearIdReinsercionAsync(reinsercion);



                return Json(new { success = true, responseText = "Datos creado exitosamente! ", viewUrl = Url.Action("FichaCanalizacion", "Reinsercion", new { idReinsercion = idReinsercionObtenido }) });
            }
            return Json(new { success = false, responseText = "Error en la validación de datos" });
        }

        #region - ID Reinsercion -
        public async Task<int> ObtenerIdReinsercionAsync(string idTabla, string nombreTabla)
        {
            var idRegistroReinsercion = await _context.Reinsercion
                .Where(c => c.IdTabla == idTabla && c.Tabla == nombreTabla)
                .Select(c => c.IdReinsercion)
                .FirstOrDefaultAsync();

            return idRegistroReinsercion;
        }

        [HttpPost]
        public async Task<int> CrearIdReinsercionAsync(Reinsercion reinsercion)
        {
            _context.Add(reinsercion);
            await _context.SaveChangesAsync();
            int nuevoIdReinsercion = await ObtenerIdReinsercionAsync(reinsercion.IdTabla, reinsercion.Tabla);

            return nuevoIdReinsercion;
        }
        #endregion

        #endregion

        #region -FichaCanalización-

        public IActionResult FichaCanalizacion(int idReinsercion, int? id)
        {
            if (idReinsercion == 0 && (id == null || id == 0))
            {
                return BadRequest("Id Reinsercion vacio!");
            }


            if (idReinsercion == 0)
                ViewBag.idReinsercion = id;
            else
                ViewBag.idReinsercion = idReinsercion;

            int idX = ViewBag.idReinsercion;
            var existeIdReinsercion = _context.Reinsercion.Where(m => m.IdReinsercion == idX).FirstOrDefault();
            if (existeIdReinsercion == null)
            {
                return BadRequest("Id Reinsercion no existe en tabla reinsercion!");
            }
            var grupos = _context.Grupo.ToList();
            ViewBag.Grupos = grupos;

            var terapeutas = _context.Terapeutas.ToList();
            ViewBag.Terapeutas = terapeutas;

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CrearFichaCanalizacion([FromBody] DatosFichaCanalizacion datosFichaCanalizacion)
        {
            await Task.Delay(1000);
            var viewUrl = string.Empty;
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            if (datosFichaCanalizacion == null || datosFichaCanalizacion.Datos == null)
                return Json(new { success = false, responseText = "Error en la validación de datos" });
            try
            {
                int idRegistroCanalizacion = await ObtenerIdCanalizacionAsync(datosFichaCanalizacion.IdReinsercion);

                if (idRegistroCanalizacion == 0)
                {
                    int idNuevoRegistroCanalizacion = await CrearRegistroCanalizacionAsync(datosFichaCanalizacion.IdReinsercion);

                    if (datosFichaCanalizacion.TipoCanalizacion.Equals("TERAPIA"))
                        await CrearTerapiaAsync(idNuevoRegistroCanalizacion, datosFichaCanalizacion.Datos);

                    else if (datosFichaCanalizacion.TipoCanalizacion.Equals("EJESREINSERCION") || datosFichaCanalizacion.TipoCanalizacion.Equals("JORNADA"))
                        await CrearEjesReinsercionAsync(idNuevoRegistroCanalizacion, datosFichaCanalizacion.Datos);
                }
                else
                {
                    if (datosFichaCanalizacion.TipoCanalizacion.Equals("TERAPIA"))
                        await CrearTerapiaAsync(idRegistroCanalizacion, datosFichaCanalizacion.Datos);

                    else if (datosFichaCanalizacion.TipoCanalizacion.Equals("EJESREINSERCION") || datosFichaCanalizacion.TipoCanalizacion.Equals("JORNADA"))
                        await CrearEjesReinsercionAsync(idRegistroCanalizacion, datosFichaCanalizacion.Datos);
                }
                bool borrar = false;

                foreach (var rol in roles)
                {
                    if (rol == "Vinculacion")
                    {

                        borrar = true;

                        //viewUrl = Url.Action("EjesReinsercion", "Reinsercion", new { id = datosFichaCanalizacion.IdReinsercion });

                        //string viewUrl = string.Empty;
                        switch (datosFichaCanalizacion.TipoCanalizacion)
                        {
                            case "EJESREINSERCION":
                                viewUrl = Url.Action("EjesReinsercion/" + datosFichaCanalizacion.IdReinsercion, "Reinsercion");
                                break;
                            case "TERAPIA":
                                viewUrl = Url.Action("Terapias/" + datosFichaCanalizacion.IdReinsercion, "Reinsercion");
                                break;
                            case "JORNADA":
                                viewUrl = Url.Action("Jornadas/" + datosFichaCanalizacion.IdReinsercion, "Reinsercion");
                                break;
                            default:
                                viewUrl = Url.Action("Reinsercion", "Reinsercion");
                                break;
                        }

                        return Json(new { success = true, responseText = "Ficha creada exitosamente!", viewUrl = viewUrl, id = datosFichaCanalizacion.IdReinsercion });

                        //return Json(new { success = true, responseText = "Ficha creada exitosamente!", viewUrl});
                    }
                    else if (rol.ToUpper().Contains("Cl"))
                    {
                        viewUrl = Url.Action("Index", "Personacls");
                    }
                    else if (rol.ToUpper().Contains("MCSCP"))
                    {
                        viewUrl = Url.Action("Index", "Personas");
                    }

                }


                return Json(new { success = true, responseText = "Ficha creada exitosamente!", viewUrl = viewUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = $"Error al crear la ficha: {ex.InnerException}" });
            }
        }

        #region - ID Canalizacion -
        public async Task<int> ObtenerIdCanalizacionAsync(int idReinsercion)
        {

            var idRegistroCanalizacion = await _context.Canalizacion
                .Where(c => c.ReincercionIdReincercion == idReinsercion)
                .Select(c => c.IdCanalizacion)
                .FirstOrDefaultAsync();

            return idRegistroCanalizacion;
        }

        [HttpPost]
        public async Task<int> CrearRegistroCanalizacionAsync(int idReinsercion)
        {
            Canalizacion canalizacion = new Canalizacion();
            canalizacion.ReincercionIdReincercion = idReinsercion;

            _context.Add(canalizacion);
            await _context.SaveChangesAsync();

            return await ObtenerIdCanalizacionAsync(idReinsercion);

        }
        #endregion

        #region -Creacion de ficha por tipo- 

        [HttpPost]
        public async Task CrearTerapiaAsync(int idCanalizacion, object DatosTerapia)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    var DatosTerapiaDeserializados = JsonConvert.DeserializeObject<TerapiaModel>(DatosTerapia.ToString());

                    foreach (var terapia in DatosTerapiaDeserializados.TiposTerapiaSeleccionados)
                    {
                        Terapia FichaTerapia = new Terapia();

                        if (terapia.Equals("OTRO"))
                            FichaTerapia.Tipo = mg.normaliza(DatosTerapiaDeserializados.EspecificarOtraTerapia);
                        else
                            FichaTerapia.Tipo = terapia;

                        FichaTerapia.Terapeuta = mg.normaliza(DatosTerapiaDeserializados.Terapeuta);
                        FichaTerapia.FechaCanalizacion = DateTime.Now;
                        //PARA CALCULAR EL TIEMPO DE TERAPIA DEPENDIENDO DE LA FECHA DE INICIO Y FECHA DE TERMINO

                        double diasDiferencia = (DatosTerapiaDeserializados.FechaTermino - DatosTerapiaDeserializados.FechaInicio).TotalDays;
                        if (DatosTerapiaDeserializados.PeriodicidadTerapia.Equals("SEMANAL"))
                        {
                            double semanas = Math.Round(diasDiferencia / 7, 2);
                            FichaTerapia.TiempoTerapia = semanas.ToString() + " SEMANAS";
                        }
                        else if (DatosTerapiaDeserializados.PeriodicidadTerapia.Equals("MENSUAL"))
                        {
                            double meses = Math.Round(diasDiferencia / 30, 2);
                            FichaTerapia.TiempoTerapia = meses.ToString() + " MESES";
                        }
                        FichaTerapia.FechaInicio = DatosTerapiaDeserializados.FechaInicio;
                        FichaTerapia.FechaTermino = DatosTerapiaDeserializados.FechaTermino;


                        FichaTerapia.FechaTerapia = DatosTerapiaDeserializados.FechaTerapia;

                        FichaTerapia.PeriodicidadTerapia = mg.normaliza(DatosTerapiaDeserializados.PeriodicidadTerapia);
                        FichaTerapia.Estado = mg.normaliza(DatosTerapiaDeserializados.Estado);

                        if (DatosTerapiaDeserializados.Observaciones.Equals(""))
                            FichaTerapia.Observaciones = "NA";
                        else
                            FichaTerapia.Observaciones = mg.normaliza(DatosTerapiaDeserializados.Observaciones);

                        FichaTerapia.CanalizacionIdCanalizacion = idCanalizacion;
                        FichaTerapia.GrupoIdGrupo = DatosTerapiaDeserializados.GrupoId;

                        _context.Terapia.Add(FichaTerapia);
                        _context.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error al crear los registros de EjesReinsercion", ex);
                }
            }

        }

        [HttpPost]
        public async Task CrearEjesReinsercionAsync(int idCanalizacion, object DatosEjesReinsercion)
        {
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var DatosEjesDeserializados = JsonConvert.DeserializeObject<EjesReinsercionModel>(DatosEjesReinsercion.ToString());

                    foreach (var eje in DatosEjesDeserializados.EjesSeleccionados)
                    {
                        Ejesreinsercion ficha = new Ejesreinsercion();

                        if (eje.Equals("OTRO"))
                            ficha.Tipo = mg.normaliza(DatosEjesDeserializados.EspecificarOtroEje);
                        else
                            ficha.Tipo = eje;
                        ficha.FechaCanalizacion = DateTime.Now;
                        ficha.Lugar = mg.normaliza(DatosEjesDeserializados.Lugar);
                        ficha.Area = mg.normaliza(DatosEjesDeserializados.Area);
                        ficha.FechaLimite = DatosEjesDeserializados.FechaLimite;
                        ficha.FechaProgramada = DatosEjesDeserializados.FechaProgramada;
                        ficha.HorasJornada = mg.normaliza(DatosEjesDeserializados.HorasJornadas);
                        ficha.NoHoraJornada = DatosEjesDeserializados.NoHoraJornada;
                        if (DatosEjesDeserializados.Observaciones.Equals(""))
                            ficha.Observaciones = "NA";
                        else
                            ficha.Observaciones = mg.normaliza(DatosEjesDeserializados.Observaciones);

                        ficha.Estado = mg.normaliza(DatosEjesDeserializados.Estado);
                        ficha.CanalizacionIdCanalizacion = idCanalizacion;

                        _context.Ejesreinsercion.Add(ficha);
                        _context.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error al crear los registros de EjesReinsercion", ex);
                }
            }
        }
        #endregion

        #endregion

        #region - Ver canalizaciones -

        public async Task<IActionResult> VerCanalizaciones()
        {
            return View();
        }



        #endregion

        #region - Ver y Agregar Terapias - 

        public async Task<IActionResult> Terapias(int? id)
        {
            var grupos = _context.Grupo.ToList();
            ViewBag.Grupos = grupos;

            var terapeutas = _context.Terapeutas.ToList();
            ViewBag.Terapeutas = terapeutas;

            ViewBag.idReinsercion = id;

            var terapias = await (from t in _context.Terapia
                                  join c in _context.Canalizacion on t.CanalizacionIdCanalizacion equals c.IdCanalizacion
                                  join r in _context.Reinsercion on c.ReincercionIdReincercion equals r.IdReinsercion
                                  join g in _context.Grupo on t.GrupoIdGrupo equals g.IdGrupo
                                  where r.IdReinsercion == id
                                  join a in _context.Asistencia on t.IdTerapia equals a.TerapiaIdTerapia into gj
                                  from suba in gj.DefaultIfEmpty()
                                  select new TerapiaAsistenciaViewModal
                                  {
                                      // DATOS TERAPIA
                                      IdTerapia = t.IdTerapia,
                                      Tipo = t.Tipo,
                                      Terapeuta = t.Terapeuta,
                                      FechaCanalizacion = t.FechaCanalizacion,
                                      TiempoTerapia = t.TiempoTerapia,
                                      FechaInicioTerapia = t.FechaInicio,
                                      FechaTerminoTerapia = t.FechaTermino,
                                      FechaTerapia = t.FechaTerapia,
                                      PeriodiciadTerapia = t.PeriodicidadTerapia,
                                      Estado = t.Estado,
                                      Observaciones = t.Observaciones,
                                      CanalizacionIdCanalizacion = t.CanalizacionIdCanalizacion,

                                      // DATOS GRUPO
                                      IdGrupo = g.IdGrupo,
                                      NombreGrupo = g.NombreGrupo,
                                      Dia = g.Dia,
                                      HorarioGrupo = g.Horario,

                                      // DATOS ASISTENCIA
                                      IdAsistencia = suba != null ? suba.IdAsistencia.ToString() : "SIN REGISTRO",
                                      FechaAsistencia = suba != null ? suba.FechaAsistencia.ToString() : "SIN ASISTENCIA REGISTRADA",
                                      ObservacionesAsistencia = suba != null ? suba.Observaciones : "SIN ASISTENCIA REGISTRADA",
                                      Asistio = suba != null ? suba.Asistio.ToString() : "SIN ASISTENCIA REGISTRADA",
                                      TerapiaIdTerapia = suba != null ? suba.TerapiaIdTerapia : t.IdTerapia
                                  }).ToListAsync();

            return View(terapias);
        }



        [HttpPost]
        public async Task<JsonResult> ActualizarTerapia(int idTerapia, string valor, string NombreCampo, Terapia terapia)
        {

            terapia = await _context.Terapia.SingleOrDefaultAsync(c => c.IdTerapia == idTerapia);

            if (terapia == null)
            {
                return Json(new { success = false, message = "IdTerapia no encontrado." });
            }
            else
            {
                switch (NombreCampo)
                {
                    case "Terapeuta":
                        terapia.Terapeuta = mg.normaliza(valor);
                        break;
                    case "FechaCanalizacion":
                        DateTime? fechaCanalizacion = ConvertirAFecha(valor);
                        if (fechaCanalizacion.HasValue)
                            terapia.FechaCanalizacion = fechaCanalizacion.Value;
                        else
                            return Json(new { success = false, message = "Formato de fecha no válido para FechaCanalizacion." });
                        break;

                    case "TiempoTerapia":
                        terapia.TiempoTerapia = CalcularTiempoTotalTerapia(terapia.FechaInicio, terapia.FechaTermino, terapia.PeriodicidadTerapia);
                        break;

                    case "FechaInicioTerapia":
                        DateTime? fechaInicio = ConvertirAFecha(valor);
                        if (fechaInicio.HasValue)
                        {
                            terapia.FechaInicio = fechaInicio.Value;
                            terapia.TiempoTerapia = CalcularTiempoTotalTerapia(terapia.FechaInicio, terapia.FechaTermino, terapia.PeriodicidadTerapia);
                        }
                        else
                            return Json(new { success = false, message = "Formato de fecha no válido para FechaInicioTerapia." });
                        break;

                    case "FechaTerminoTerapia":
                        DateTime? fechaTermino = ConvertirAFecha(valor);
                        if (fechaTermino.HasValue)
                        {
                            terapia.FechaTermino = fechaTermino.Value;
                            terapia.TiempoTerapia = CalcularTiempoTotalTerapia(terapia.FechaInicio, terapia.FechaTermino, terapia.PeriodicidadTerapia);
                        }
                        else
                            return Json(new { success = false, message = "Formato de fecha no válido para FechaTerminoTerapia." });
                        break;

                    case "FechaAsistencia":
                        DateTime? fechaAsistencia = ConvertirAFecha(valor);
                        if (fechaAsistencia.HasValue)
                            terapia.FechaTerapia = fechaAsistencia.Value;
                        else
                            return Json(new { success = false, message = "Formato de fecha no válido para FechaAsistencia." });
                        break;

                    case "PeriodicidadTerapia":
                        terapia.PeriodicidadTerapia = mg.normaliza(valor);
                        terapia.TiempoTerapia = CalcularTiempoTotalTerapia(terapia.FechaInicio, terapia.FechaTermino, terapia.PeriodicidadTerapia);
                        break;

                    case "Estado":
                        terapia.Estado = mg.normaliza(valor);
                        break;

                    case "Observaciones":
                        terapia.Observaciones = mg.normaliza(valor);
                        break;

                    case "Grupo":
                        terapia.GrupoIdGrupo = Int32.Parse(valor);
                        break;

                    default:
                        return Json(new { success = false, message = "Campo no válido." });
                }

                _context.Terapia.Update(terapia);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Terapia actualizada correctamente." });
            }
        }

        private DateTime? ConvertirAFecha(string valor)
        {
            DateTime fechaConvertida;
            if (DateTime.TryParse(valor, out fechaConvertida))
            {
                return fechaConvertida;
            }
            return null;
        }


        private string CalcularTiempoTotalTerapia(DateTime? FechaInicio, DateTime? FechaTermino, string Periodicidad)
        {
            TimeSpan diferencia = FechaTermino.Value - FechaInicio.Value;
            double diasDiferencia = diferencia.TotalDays;
            string tiempoTotal = "";

            if (Periodicidad.Equals("SEMANAL"))
            {
                // Convertir días a semanas (considerando 7 días por semana)
                double semanas = Math.Round(diasDiferencia / 7, 2);
                tiempoTotal = semanas.ToString() + " SEMANAS";
            }
            else if (Periodicidad.Equals("MENSUAL"))
            {
                // Calcular la diferencia en meses
                double meses = Math.Round(diasDiferencia / 30, 2);
                tiempoTotal = meses.ToString() + " MESES";
            }
            return tiempoTotal;
        }




        public async Task<IActionResult> ModalAgregarTerapia(int? idReinsercion)
        {

            var grupos = await _context.Grupo.ToListAsync();
            ViewBag.Grupos = grupos;

            var terapeutas = await _context.Terapeutas.ToListAsync();
            ViewBag.Terapeutas = terapeutas;

            ViewBag.idReinsercion = idReinsercion;
            ViewBag.idCanalizacion = await _context.Canalizacion.Where(c => c.ReincercionIdReincercion == idReinsercion).Select(c => c.IdCanalizacion).FirstOrDefaultAsync();

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ModalCrearTerapia(int idReinsercion, [FromBody] Terapia terapia)
        {
            if (idReinsercion == 0)
                return Json(new { success = false, message = "IdReinsercion vacio" });

            if (terapia == null || idReinsercion == 0)
                return Json(new { success = false, message = "Datos de terapia vacia" });

            if (terapia.Tipo.Equals("") || terapia.Tipo == null)
                return Json(new { success = false, message = "Tipo de terapia no seleccionado" });

            if (terapia.FechaTermino < terapia.FechaInicio)
                return Json(new { success = false, message = "La fecha de termino es menor que la fecha de inicio" });

            try
            {
                terapia.FechaCanalizacion = DateTime.Now;
                terapia.TiempoTerapia = CalcularTiempoTotalTerapia(terapia.FechaInicio, terapia.FechaTermino, terapia.PeriodicidadTerapia);
                _context.Terapia.Add(terapia);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Terapia agregada con exito!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al guardar la terapia: " + ex.Message });
            }
        }



        [HttpPost]
        public async Task<JsonResult> EliminarTerapia(int idTerapia)
        {
            if (idTerapia == 0)
                return Json(new { success = false, message = "id Terapia no recibido!" });


            var tieneAsistencias = await _context.Asistencia.Where(a => a.TerapiaIdTerapia == idTerapia).ToListAsync();
            if (tieneAsistencias.Count > 0)
            {
                return Json(new { success = false, message = "No se puede eliminar Terapia, Cuenta con asistencias registradas!" });

            }
            else
            {
                var terapia = await _context.Terapia.SingleOrDefaultAsync(m => m.IdTerapia == idTerapia);
                _context.Terapia.Remove(terapia);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Terapia eliminada con exito!" });

            }

        }


        #endregion

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

        private bool ReinsercionExists(int id)
        {
            return _context.Reinsercion.Any(e => e.IdReinsercion == id);
        }

        #region -Menu Reinsercion-
        public async Task<IActionResult> MenuReinsercion()
        {
            DateTime fechaActual = DateTime.Now;
            DateTime unMesAnterior = DateTime.Now.AddDays(-30);
            ViewBag.AlertasCount = await (from persona in _context.Persona
                                          join supervicionMC in _context.Supervision on persona.IdPersona equals supervicionMC.PersonaIdPersona
                                          join cierreDeCasoMC in _context.Cierredecaso on supervicionMC.IdSupervision equals cierreDeCasoMC.SupervisionIdSupervision
                                          where cierreDeCasoMC.SeCerroCaso.Equals("SI") &&
                                          (cierreDeCasoMC.FechaAprobacion >= unMesAnterior && cierreDeCasoMC.FechaAprobacion <= fechaActual)
                                          select new
                                          {
                                              IdTabla = persona.IdPersona
                                          }).Union(
                               from personaCL in _context.Personacl
                               join supervisionCL in _context.Supervisioncl on personaCL.IdPersonaCl equals supervisionCL.PersonaclIdPersonacl
                               join cierreDeCasoCL in _context.Cierredecasocl on supervisionCL.IdSupervisioncl equals cierreDeCasoCL.SupervisionclIdSupervisioncl
                               where cierreDeCasoCL.SeCerroCaso.Equals("SI") &&
                               (cierreDeCasoCL.FechaAprobacion >= unMesAnterior && cierreDeCasoCL.FechaAprobacion <= fechaActual)
                               select new
                               {
                                   IdTabla = personaCL.IdPersonaCl
                               }).CountAsync();
            return View();
        }

        #endregion

        #region -Menu Supervision-
        public async Task<IActionResult> Menusupervision(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            //var user = await userManager.FindByNameAsync(User.Identity.Name);
            //#region -Solicitud Atendida Archivo prestamo Digital-
            //var warningRespuesta = from a in _context.Archivoprestamodigital
            //                       where a.EstadoPrestamo == 1 && user.ToString().ToUpper() == a.Usuario.ToUpper()
            //                       select a;
            //ViewBag.WarningsUser = warningRespuesta.Count();
            //#endregion


            var Reinsercion = await _context.Reinsercion.SingleOrDefaultAsync(m => m.IdReinsercion == id);
            ViewBag.IdReinsercion = Reinsercion.IdReinsercion;

            if (Reinsercion == null)
            {
                return NotFound();
            }



            List<Reinsercion> reinsercionVM = _context.Reinsercion.ToList();
            List<Canalizacion> canalizacionVM = _context.Canalizacion.ToList();
            List<Terapia> terapialVM = _context.Terapia.ToList();
            List<Ejesreinsercion> ejesreinsercionVM = _context.Ejesreinsercion.ToList();
            List<Oficioscanalizacion> oficioscanalizacionVM = _context.Oficioscanalizacion.ToList();

            List<Persona> personaVM = _context.Persona.ToList();
            List<Personacl> personacls = _context.Personacl.ToList();

            //List<Jornadas> JornadasVM = _context.Jornadas.ToList();

            #region -Jointables-
            var ES = from reinsercion in reinsercionVM

                     where reinsercion.IdReinsercion == id
                     select new
                     {
                         reinsercionVM = reinsercion
                     };


            ViewData["reincercion"] = from reinsercion in reinsercionVM
                                      join canalizacion in canalizacionVM on reinsercion.IdReinsercion equals canalizacion.ReincercionIdReincercion
                                      where reinsercion.IdReinsercion == id
                                      select new ReinsercionVM
                                      {
                                          reinsercionVM = reinsercion,
                                          canalizacionVM = canalizacion

                                      };

            var ESh = from reinsercion in reinsercionVM
                      join canalizacion in canalizacionVM on reinsercion.IdReinsercion equals canalizacion.IdCanalizacion
                      join terapia in terapialVM on reinsercion.IdReinsercion equals terapia.CanalizacionIdCanalizacion
                      join ejesreincercion in ejesreinsercionVM on reinsercion.IdReinsercion equals ejesreincercion.CanalizacionIdCanalizacion
                      join oficioscanalizacion in oficioscanalizacionVM on reinsercion.IdReinsercion equals oficioscanalizacion.CanalizacionIdCanalizacion
                      where reinsercion.IdReinsercion == id
                      select new ReinsercionVM
                      {
                          reinsercionVM = reinsercion,
                          canalizacionVM = canalizacion,
                          terapiaVM = terapia,
                          ejesreinsercionVM = ejesreincercion,
                          oficioscanalizacionVM = oficioscanalizacion
                      };

            ViewBag.TieneAlgunaCanalizacion = await _context.Canalizacion.Where(m => m.ReincercionIdReincercion == id).Select(m => m.IdCanalizacion).FirstOrDefaultAsync();
            #endregion
            return View();
        }
        #endregion

        #region -Borrar Registro de Reinsercion-

        [HttpPost]
        public async Task<JsonResult> BorrarRegistroReinsercion(int idReinsercion)
        {
            if (idReinsercion == 0)
            {
                return Json(new { success = false, message = "Error, idReinsercion vacio, metodo: BorrarRegistroReinsercion, vista:Reinsercion" });

            }

            // EL idcanalizacion solo puede existir si ya se creo una ficha de canalizacion
            var TieneCanalizaciones = await _context.Canalizacion.Where(m => m.ReincercionIdReincercion == idReinsercion).Select(m => m.IdCanalizacion).CountAsync();

            if (TieneCanalizaciones > 0)
            {
                return Json(new { success = false, message = "Error, no se puede borrar el registro por que este ya cuenta con una canalizacion!" });
            }
            var reinsercion = await _context.Reinsercion.SingleOrDefaultAsync(m => m.IdReinsercion == idReinsercion);

            _context.Reinsercion.Remove(reinsercion);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Borrado con exito!" });
        }

        #endregion

        #region -EjesReincercion-
        public async Task<IActionResult> EjesReinsercion(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }



            var reincercion = (from canalizacion in _context.Canalizacion
                               where canalizacion.ReincercionIdReincercion == id
                               select new
                               {
                                   canalizacion.IdCanalizacion,
                               }).FirstOrDefault();

            ViewBag.idReinsercion = id;
            ViewBag.IdCanalizacion = reincercion.IdCanalizacion;


            var datosreincercion = from r in _context.Reinsercion
                                   join c in _context.Canalizacion on r.IdReinsercion equals c.ReincercionIdReincercion
                                   join er in _context.Ejesreinsercion on c.IdCanalizacion equals er.CanalizacionIdCanalizacion
                                   where r.IdReinsercion == id && er.Tipo != "JORNADA"
                                   select new ReinsercionVM
                                   {
                                       ejesreinsercionVM = er,
                                       canalizacionVM = c,
                                       reinsercionVM = r,
                                       Monitoreos = _context.Monitoreo.Where(mo => mo.IdEjeReinsercion == er.IdejesReinsercion).ToList()

                                   };

            ViewData["EjesReinsercion"] = datosreincercion;


            ViewBag.listaEjes = from e in _context.Ejesreinsercion
                                group e by e.Tipo into grupo
                                select grupo.Key;

            ViewBag.listaLugar = from e in _context.Ejesreinsercion
                                 group e by e.Lugar into grupo
                                 select grupo.Key;


            ViewBag.listaEstadoRe = listaEstadoRe;

            return View();
        }
        public async Task<IActionResult> AddEjes(int? id, int idReinsercion)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.listaEjes = from e in _context.Ejesreinsercion
                                group e by e.Tipo into grupo
                                select grupo.Key;

            ViewBag.listaLugar = from e in _context.Ejesreinsercion
                                 group e by e.Lugar into grupo
                                 select grupo.Key;

            ViewBag.idCanalizacion = id;
            ViewBag.idReinsercion = idReinsercion;


            return View(id);
        }
        #endregion

        #region -Editar Ejes de reinaercion-
        [HttpPost]
        public ActionResult EditEjes(int idEje, string Campo, string Valor)
        {
            var ejes = _context.Ejesreinsercion.Find(idEje);


            switch (Campo)
            {
                case "FechaCanalizacion":

                    ejes.FechaCanalizacion = DateTime.Parse(Valor);
                    break;
                case "Lugar":
                    ejes.Lugar = Valor;
                    break;
                case "Estado":
                    ejes.Estado = Valor;
                    break;
                case "Observaciones":
                    ejes.Observaciones = mg.normaliza(Valor);
                    break;
                default:
                    break;
            }

            try
            {
                _context.SaveChanges();
                return Json(new { success = true });

            }
            catch (Exception ex)
            {
                return Json(new { success = true, error = ex });
            }



        }
        #endregion

        #region -Borrar Eje Reinsercion-
        public async Task<JsonResult> BorrarEje(int id)
        {
            var borrar = false;

            var reincercion = (from reinsercion in _context.Reinsercion
                               join canalizacion in _context.Canalizacion on reinsercion.IdReinsercion equals canalizacion.ReincercionIdReincercion
                               join ejesreinsercion in _context.Ejesreinsercion on canalizacion.IdCanalizacion equals ejesreinsercion.CanalizacionIdCanalizacion
                               where ejesreinsercion.IdejesReinsercion == id
                               select new
                               {
                                   reinsercion.IdReinsercion,
                               }).FirstOrDefault();
            string viewUrl = string.Empty;

            viewUrl = Url.Action("EjesReinsercion/" + reincercion.IdReinsercion, "Reinsercion");
            var TieneMonitoreos = await _context.Monitoreo.Where(m => m.IdEjeReinsercion == id).ToListAsync();
            if (TieneMonitoreos.Count() > 0)
            {

                return Json(new { borrar, message = "El id Cuenta con monitoreos! no se puede borrar" });
            }

            try
            {
                var ejesreinsercion = _context.Ejesreinsercion.SingleOrDefault(m => m.IdejesReinsercion == id);
                _context.Ejesreinsercion.Remove(ejesreinsercion);
                _context.SaveChanges();
                borrar = true;
                return Json(new { borrar, viewUrl = viewUrl, id = reincercion.IdReinsercion });

            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                return Json(new { borrar, error, viewUrl = viewUrl, id = reincercion.IdReinsercion });
            }
        }
        #endregion

        public JsonResult GetEstadosPorServicio(string tipoServicio)
        {
            var ListaDinamicaEstados = new Dictionary<string, List<string>>
            {
                { "Terapia", new List<string> { "Activo", "Alta(Concluido)", "Baja", "Espera", "Termino" } },
                { "Jornada", new List<string> { "Activo", "Cancelado", "Concluido", "Cumplió", "En espera" } },
                { "Educativa", new List<string> { "Activo", "Cancelado", "Certificado", "En espera" } },
                { "Laboral", new List<string> { "Activo", "Cancelado", "Concluido", "Contratado", "En espera" } },
                { "Extraordinarios", new List<string> { "Activo", "Cancelado", "Concluido", "En espera", "Satisfactorio" } },
                { "Antidoping", new List<string> { "Concluido", "En trámite", "No realizado" } }
            };

            if (ListaDinamicaEstados.ContainsKey(tipoServicio))
            {
                return Json(ListaDinamicaEstados[tipoServicio]);
            }

            return Json(new List<string>());
        }

        #region -OficiosCanalizacion-
        public IActionResult OficiosCanalizacion(int? id, int idReinsercion)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.idCanalizacion = id;

            ViewData["OficiosCanalizacion"] = from o in _context.Oficioscanalizacion
                                              where o.CanalizacionIdCanalizacion == id
                                              select new ReinsercionVM
                                              {
                                                  oficioscanalizacionVM = o
                                              };


            ViewData["ParaOficios"] = (from c in _context.Canalizacion
                                       join t in _context.Terapia on c.IdCanalizacion equals t.CanalizacionIdCanalizacion into terapiaGroup
                                       from t in terapiaGroup.DefaultIfEmpty()
                                       join e in _context.Ejesreinsercion on c.IdCanalizacion equals e.CanalizacionIdCanalizacion into ejesGroup
                                       from e in ejesGroup.DefaultIfEmpty()
                                       where c.IdCanalizacion == id
                                       select new ReinsercionVM
                                       {
                                           canalizacionVM = c,
                                           terapiaVM = t,
                                           ejesreinsercionVM = e
                                       }).ToList();



            ViewBag.listaServicios = (from c in _context.Canalizacion
                                      join t in _context.Terapia on c.IdCanalizacion equals t.CanalizacionIdCanalizacion
                                      where c.IdCanalizacion == id
                                      select new
                                      {
                                          Id = t.IdTerapia,
                                          Tipo = t.Tipo,
                                          tabla = "Terapia"
                                      })
                        .Union(
                            from c in _context.Canalizacion
                            join e in _context.Ejesreinsercion on c.IdCanalizacion equals e.CanalizacionIdCanalizacion
                            where c.IdCanalizacion == id
                            select new
                            {
                                Id = e.IdejesReinsercion,
                                Tipo = e.Tipo,
                                tabla = "Otro"
                            }
                        ).ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OficiosCanalizacion([Bind("IdoficiosCanalizacion,TipoArchivo,FechaArchivo,RutaArchivo,Observaciones,CanalizacionIdCanalizacion")] Oficioscanalizacion oficioscanalizacion)
        {

            oficioscanalizacion.TipoArchivo = mg.normaliza(oficioscanalizacion.TipoArchivo);
            oficioscanalizacion.FechaArchivo = DateTime.Now;
            oficioscanalizacion.Observaciones = mg.normaliza(oficioscanalizacion.Observaciones);
            oficioscanalizacion.CanalizacionIdCanalizacion = oficioscanalizacion.CanalizacionIdCanalizacion;


            return View(oficioscanalizacion);
        }
        #endregion

        #region -Psicologia- 

        public IActionResult MenuPsicologia()
        {
            return View();
        }


        public async Task<IActionResult> Psicologia(DateTime FechaTerapia)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            FechaTerapia = FechaTerapia == default(DateTime) ? DateTime.Now.Date : FechaTerapia;

            ViewData["FechaTerapia"] = FechaTerapia.ToString("yyyy-MM-dd");


            ViewData["grupo"] = from g in _context.Grupo
                                join tp in _context.Terapeutas on Int32.Parse(g.Idterapeuta) equals tp.IdTerapeutas
                                where tp.Username == user.UserName.ToString()
                                select new ReinsercionVM
                                {
                                    grupoVM = g,
                                    terapeutasVM = tp
                                };

            //((item.personaVM.Genero == ("M")) ? "hombre.png" : "mujer.png");

            ViewData["Psicologia"] = (from e in _context.Externados
                                      join r in _context.Reinsercion on e.Idexternados equals Int32.Parse(r.IdTabla) into pr
                                      from r in pr.DefaultIfEmpty()
                                      join c in _context.Canalizacion on r.IdReinsercion equals c.ReincercionIdReincercion into cr
                                      from c in cr.DefaultIfEmpty()
                                      join t in _context.Terapia on c.IdCanalizacion equals t.CanalizacionIdCanalizacion
                                      join g in _context.Grupo on t.GrupoIdGrupo equals g.IdGrupo
                                      join tp in _context.Terapeutas on Int32.Parse(g.Idterapeuta) equals tp.IdTerapeutas
                                      join a in _context.Asistencia on t.IdTerapia equals a.TerapiaIdTerapia into at
                                      from a in at.DefaultIfEmpty()
                                      where tp.Username == user.UserName.ToString() && r.Tabla == "externados" && t.FechaTerapia == FechaTerapia
                                      orderby e.Nombre + " " + e.APaterno + " " + e.AMaterno
                                      select new TerapiaAsistenciaViewModal
                                      {
                                          idpersona = e.Idexternados,
                                          nombre = e.Nombre + " " + e.APaterno + " " + e.AMaterno,
                                          cp = e.CausaPenal,
                                          FechaCanalizacion = t.FechaCanalizacion,
                                          Tipo = t.Tipo,
                                          IdGrupo = g.IdGrupo,
                                          tabla = "Externados",
                                          rutaFoto = e.Sexo == "M" ? "Fotos/hombre.png" : "Fotos/mujer.png"
                                      }).Union(from p in _context.Persona
                                               join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona into sg
                                               from s in sg.DefaultIfEmpty()
                                               join cp in _context.Causapenal on s.CausaPenalIdCausaPenal equals cp.IdCausaPenal
                                               join r in _context.Reinsercion on p.IdPersona equals Int32.Parse(r.IdTabla) into pr
                                               from r in pr.DefaultIfEmpty()
                                               join c in _context.Canalizacion on r.IdReinsercion equals c.ReincercionIdReincercion into cr
                                               from c in cr.DefaultIfEmpty()
                                               join t in _context.Terapia on c.IdCanalizacion equals t.CanalizacionIdCanalizacion
                                               join g in _context.Grupo on t.GrupoIdGrupo equals g.IdGrupo
                                               join tp in _context.Terapeutas on Int32.Parse(g.Idterapeuta) equals tp.IdTerapeutas
                                               join a in _context.Asistencia on t.IdTerapia equals a.TerapiaIdTerapia into at
                                               from a in at.DefaultIfEmpty()
                                               where tp.Username == user.UserName.ToString() && r.Tabla == "persona" && t.FechaTerapia == FechaTerapia
                                               orderby p.Nombre + " " + p.Paterno + " " + p.Materno
                                               select new TerapiaAsistenciaViewModal
                                               {
                                                   idpersona = p.IdPersona,
                                                   nombre = p.NombreCompleto,
                                                   cp = cp.CausaPenal,
                                                   FechaCanalizacion = t.FechaCanalizacion,
                                                   Tipo = t.Tipo,
                                                   IdGrupo = g.IdGrupo,
                                                   tabla = "MCYSCP",
                                                   rutaFoto = "Fotos/" + p.rutaFoto
                                               }).Union(from p in _context.Personacl
                                                        join s in _context.Supervisioncl on p.IdPersonaCl equals s.PersonaclIdPersonacl into sg
                                                        from s in sg.DefaultIfEmpty()
                                                        join cp in _context.Causapenalcl on s.IdSupervisioncl equals cp.IdCausaPenalcl
                                                        join r in _context.Reinsercion on p.IdPersonaCl equals Int32.Parse(r.IdTabla) into pr
                                                        from r in pr.DefaultIfEmpty()
                                                        join c in _context.Canalizacion on r.IdReinsercion equals c.ReincercionIdReincercion into cr
                                                        from c in cr.DefaultIfEmpty()
                                                        join t in _context.Terapia on c.IdCanalizacion equals t.CanalizacionIdCanalizacion
                                                        join g in _context.Grupo on t.GrupoIdGrupo equals g.IdGrupo
                                                        join tp in _context.Terapeutas on Int32.Parse(g.Idterapeuta) equals tp.IdTerapeutas
                                                        join a in _context.Asistencia on t.IdTerapia equals a.TerapiaIdTerapia into at
                                                        from a in at.DefaultIfEmpty()
                                                        where tp.Username == user.UserName.ToString() && r.Tabla == "personacl" && t.FechaTerapia == FechaTerapia
                                                        orderby p.Nombre + " " + p.Paterno + " " + p.Materno
                                                        select new TerapiaAsistenciaViewModal
                                                        {
                                                            idpersona = p.IdPersonaCl,
                                                            nombre = p.NombreCompleto,
                                                            cp = cp.CausaPenal,
                                                            FechaCanalizacion = t.FechaCanalizacion,
                                                            Tipo = t.Tipo,
                                                            IdGrupo = g.IdGrupo,
                                                            tabla = "Libertad Condicionada",
                                                            rutaFoto = "Fotoscl/" + p.RutaFoto
                                                        });


            return View();
        }

        public async Task<IActionResult> gruposAsistencia(string Dia, string IdPsicologo)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            var Terapeutas = await _context.Terapeutas.ToListAsync();

            var gruposTerapeutas = from g in _context.Grupo
                                   join tp in _context.Terapeutas on Int32.Parse(g.Idterapeuta) equals tp.IdTerapeutas
                                   select new ReinsercionVM
                                   {
                                       grupoVM = g,
                                       terapeutasVM = tp
                                   };

            var NumeroAsistentes = from g in _context.Grupo 
                                   join t in _context.Terapia on g.IdGrupo equals t.GrupoIdGrupo into GrupoTerapia
                                   from gt in GrupoTerapia.DefaultIfEmpty()  
                                   where gt.Estado.Equals("ACTIVO") || gt.IdTerapia ==0
                                   group gt by g.IdGrupo into grouped
                                   select new
                                   {
                                       IdGrupo = grouped.Key,
                                       NumeroAsistentes = grouped.Count(tg => tg != null && tg.Estado == "ACTIVO")
                                   };




            if (!string.IsNullOrEmpty(Dia))
            {
                if (Dia.Equals("TODOS"))
                    gruposTerapeutas = gruposTerapeutas;
                else
                    gruposTerapeutas = gruposTerapeutas.Where(m => m.grupoVM.Dia.Equals(Dia));


            }

            if (!string.IsNullOrEmpty(IdPsicologo))
            {
                if (IdPsicologo.Equals("TODOS"))
                    gruposTerapeutas = gruposTerapeutas;
                else
                    gruposTerapeutas = gruposTerapeutas.Where(m => m.terapeutasVM.IdTerapeutas.ToString() == IdPsicologo);


            }
            if (string.IsNullOrEmpty(Dia))
                ViewBag.DiaSeleccionado = "TODOS";
            else
                ViewBag.DiaSeleccionado = Dia;




            if (string.IsNullOrEmpty(IdPsicologo) || IdPsicologo.Equals("TODOS"))
            {
                ViewBag.IdPsicologoSeleccionado = "TODOS";
                ViewBag.NombrePsicologoSeleccionado = "TODOS";
            }
            else
            {
                ViewBag.IdPsicologoSeleccionado = IdPsicologo;
                ViewBag.NombrePsicologoSeleccionado = Terapeutas.Where(m => m.IdTerapeutas.ToString() == IdPsicologo)
                                                       .Select(m => m.Nombre +" "+ m.Paterno + " "+ m.Materno).FirstOrDefault();
            }



            ViewData["Terapeutas"] = Terapeutas;
            ViewData["grupos"] = gruposTerapeutas;
            ViewData["NumeroAsistentes"] = NumeroAsistentes.ToList();


            return View();
        }

        public IActionResult ListaDia(int idgrupo,string NombreGrupo)
        {

            ViewBag.NombreGRupo = NombreGrupo;




            ViewData["Listadia"] = (from e in _context.Externados
                                    join r in _context.Reinsercion on e.Idexternados equals Int32.Parse(r.IdTabla) into pr
                                    from r in pr.DefaultIfEmpty()
                                    join c in _context.Canalizacion on r.IdReinsercion equals c.ReincercionIdReincercion into cr
                                    from c in cr.DefaultIfEmpty()
                                    join t in _context.Terapia on c.IdCanalizacion equals t.CanalizacionIdCanalizacion
                                    join g in _context.Grupo on t.GrupoIdGrupo equals g.IdGrupo
                                    join tp in _context.Terapeutas on Int32.Parse(g.Idterapeuta) equals tp.IdTerapeutas
                                    join a in _context.Asistencia on t.IdTerapia equals a.TerapiaIdTerapia into at
                                    from a in at.DefaultIfEmpty()
                                    where g.IdGrupo == idgrupo && t.Estado.Equals("ACTIVO")/*&& t.FechaTerapia == DateTime.Parse("2024-08-30 00:00:00")*/
                                    orderby e.Nombre + " " + e.APaterno + " " + e.AMaterno
                                    select new TerapiaAsistenciaViewModal
                                    {
                                        idpersona = e.Idexternados,
                                        nombre = e.Nombre + " " + e.APaterno + " " + e.AMaterno,
                                        cp = e.CausaPenal,
                                        FechaCanalizacion = t.FechaCanalizacion,
                                        IdTerapia = t.IdTerapia,
                                        Tipo = t.Tipo,
                                        IdGrupo = g.IdGrupo,
                                        tabla = "Externados",
                                        rutaFoto = e.Sexo == "M" ? "Fotos/hombre.png" : "Fotos/mujer.png"
                                    }).Union(from p in _context.Persona
                                             join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona into sg
                                             from s in sg.DefaultIfEmpty()
                                             join cp in _context.Causapenal on s.CausaPenalIdCausaPenal equals cp.IdCausaPenal
                                             join r in _context.Reinsercion on p.IdPersona equals Int32.Parse(r.IdTabla) into pr
                                             from r in pr.DefaultIfEmpty()
                                             join c in _context.Canalizacion on r.IdReinsercion equals c.ReincercionIdReincercion into cr
                                             from c in cr.DefaultIfEmpty()
                                             join t in _context.Terapia on c.IdCanalizacion equals t.CanalizacionIdCanalizacion
                                             join g in _context.Grupo on t.GrupoIdGrupo equals g.IdGrupo
                                             join tp in _context.Terapeutas on Int32.Parse(g.Idterapeuta) equals tp.IdTerapeutas
                                             join a in _context.Asistencia on t.IdTerapia equals a.TerapiaIdTerapia into at
                                             from a in at.DefaultIfEmpty()
                                             where g.IdGrupo == idgrupo && t.Estado.Equals("ACTIVO")/*&& t.FechaTerapia == DateTime.Parse("2024-08-30 00:00:00")*/
                                             orderby p.Nombre + " " + p.Paterno + " " + p.Materno
                                             select new TerapiaAsistenciaViewModal
                                             {
                                                 idpersona = p.IdPersona,
                                                 nombre = p.NombreCompleto,
                                                 cp = cp.CausaPenal,
                                                 FechaCanalizacion = t.FechaCanalizacion,
                                                 IdTerapia = t.IdTerapia,
                                                 Tipo = t.Tipo,
                                                 IdGrupo = g.IdGrupo,
                                                 tabla = "MCYSCP",
                                                 rutaFoto = "Fotos/" + p.rutaFoto
                                             }).Union(from p in _context.Personacl
                                                      join s in _context.Supervisioncl on p.IdPersonaCl equals s.PersonaclIdPersonacl into sg
                                                      from s in sg.DefaultIfEmpty()
                                                      join cp in _context.Causapenalcl on s.IdSupervisioncl equals cp.IdCausaPenalcl
                                                      join r in _context.Reinsercion on p.IdPersonaCl equals Int32.Parse(r.IdTabla) into pr
                                                      from r in pr.DefaultIfEmpty()
                                                      join c in _context.Canalizacion on r.IdReinsercion equals c.ReincercionIdReincercion into cr
                                                      from c in cr.DefaultIfEmpty()
                                                      join t in _context.Terapia on c.IdCanalizacion equals t.CanalizacionIdCanalizacion
                                                      join g in _context.Grupo on t.GrupoIdGrupo equals g.IdGrupo
                                                      join tp in _context.Terapeutas on Int32.Parse(g.Idterapeuta) equals tp.IdTerapeutas
                                                      join a in _context.Asistencia on t.IdTerapia equals a.TerapiaIdTerapia into at
                                                      from a in at.DefaultIfEmpty()
                                                      where g.IdGrupo == idgrupo && t.Estado.Equals("ACTIVO")/*&& t.FechaTerapia == DateTime.Parse("2024-08-30 00:00:00")*/
                                                      orderby p.Nombre + " " + p.Paterno + " " + p.Materno
                                                      select new TerapiaAsistenciaViewModal
                                                      {
                                                          idpersona = p.IdPersonaCl,
                                                          nombre = p.NombreCompleto,
                                                          cp = cp.CausaPenal,
                                                          FechaCanalizacion = t.FechaCanalizacion,
                                                          IdTerapia = t.IdTerapia,
                                                          Tipo = t.Tipo,
                                                          IdGrupo = g.IdGrupo,
                                                          tabla = "Libertad Condicionada",
                                                          rutaFoto = "Fotoscl/" + p.RutaFoto
                                                      });

            return View();
        }


        [HttpPost]
        public async Task<JsonResult> Asistencia(string arra)
        {


            return Json(new { success = true, responseText = Convert.ToString(""), idPersonas = Convert.ToString(2) });
        }
        #endregion

        #region ListaSeugunCaso

        [HttpGet]
        public IActionResult ListaPorTipo(string tabla)
        {
            if (ListaTipoInforme.ContainsKey(tabla))
            {
                var lista = ListaTipoInforme[tabla];
                return Json(lista);
            }
            else
            {
                return Json(new List<string>());
            }
        }
        #endregion

        #region Monitoreo

        [HttpPost]
        public async Task<JsonResult> CrearMonitoreo([FromBody] Monitoreo monitoreo)
        {
            if (monitoreo == null)
                return Json(new { success = false, message = "Datos de monitoreo nulos!" });
            if (monitoreo.IdEjeReinsercion == 0)
                return Json(new { success = false, message = "idEjesReinsercion vacio!" });
            monitoreo.Fecha = DateTime.Now;
            await _context.Monitoreo.AddAsync(monitoreo);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Monitoreo agregado con exito!" });
        }
        #endregion




        //#region ListaSeugunCaso2

        //[HttpGet]
        //public IActionResult ListaPorEstados(string tipo)
        //{
        //    if (ListaDinamicaEstados.ContainsKey(tipo))
        //    {
        //        var lista = ListaDinamicaEstados[tipo];
        //        return Json(lista);
        //    }
        //    else
        //    {
        //        return Json(new List<string>());
        //    }
        //}
        //#endregion

        #region -Jornadas -
        public async Task<IActionResult> Jornadas(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            if (id == 0)
            {
                return BadRequest("id no valido!");
            }

            int idCanalizacion = await _context.Canalizacion.Where(m => m.ReincercionIdReincercion == id).Select(m => m.IdCanalizacion).FirstAsync();

            var jornadas = await (from j in _context.Ejesreinsercion
                                  where j.CanalizacionIdCanalizacion == idCanalizacion && j.Tipo.Equals("JORNADA")
                                  select j).ToListAsync();

            var monitoreos = await (from mo in _context.Monitoreo
                                    join j in jornadas on mo.IdEjeReinsercion equals j.IdejesReinsercion
                                    select mo).ToListAsync();

            var jornadasMonitoreos = jornadas.Select(j => new JornadasMonitoreoViewModel
            {
                Jornadas = j,
                Monitoreos = monitoreos.Where(mo => mo.IdEjeReinsercion == j.IdejesReinsercion).ToList()
            }).ToList();

            ViewBag.idCanalizacion = idCanalizacion;
            ViewBag.idReinsercion = id;

            return View(jornadasMonitoreos);
        }

        [HttpPost]
        public async Task<JsonResult> CrearJornada([FromBody] Ejesreinsercion ejesReinsercion)
        {
            if (ejesReinsercion == null)
                return Json(new { success = false, message = "Datos de la jornada vacíos!" });

            if (string.IsNullOrEmpty(ejesReinsercion.Tipo))
                return Json(new { success = false, message = "Error en tipo!" });
            if (ejesReinsercion.CanalizacionIdCanalizacion == 0)
                return Json(new { success = false, message = "Error en idCanalizacion!" });

            if (string.IsNullOrEmpty(ejesReinsercion.Estado))
                return Json(new { success = false, message = "Selecciona el estado de la jornada!" });

            if (string.IsNullOrEmpty(ejesReinsercion.HorasJornada))
                return Json(new { success = false, message = "Selecciona horas o jornadas!" });

            if (string.IsNullOrEmpty(ejesReinsercion.NoHoraJornada))
                return Json(new { success = false, message = "Especifica la cantidad de horas o jornadas!" });

            if (string.IsNullOrEmpty(ejesReinsercion.Lugar))
                return Json(new { success = false, message = "Lugar vacío!" });

            if (string.IsNullOrEmpty(ejesReinsercion.Area))
                return Json(new { success = false, message = "Área vacía!" });

            if (string.IsNullOrEmpty(ejesReinsercion.Estado))
                return Json(new { success = false, message = "Estado vacío!" });

            if (ejesReinsercion.FechaProgramada == null || ejesReinsercion.FechaProgramada == DateTime.MinValue)
                return Json(new { success = false, message = "Fecha Programada vacía!" });

            try
            {
                ejesReinsercion.FechaCanalizacion = DateTime.Now;
                if (string.IsNullOrEmpty(ejesReinsercion.Observaciones))
                    ejesReinsercion.Observaciones = "NA";
                await _context.Ejesreinsercion.AddAsync(ejesReinsercion);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Jornada creada con éxito!" });
            }
            catch (DbUpdateException ex)
            {
                // Log error (ejemplo: _logger.LogError(ex, "Error al actualizar la base de datos"))
                return Json(new { success = false, message = "Hubo un error al guardar la jornada. Comunicarse con el administrador del sistema." });
            }
            catch (Exception ex)
            {
                // Log error (ejemplo: _logger.LogError(ex, "Error general"))
                return Json(new { success = false, message = "Ocurrió un error inesperado. Comunicarse con el administrador del sistema" });
            }
        }
        [HttpPost]
        public async Task<JsonResult> ActualizarJornada(int idJornada, string valor, string NombreCampo, Ejesreinsercion jornada)
        {

            if (idJornada == 0)
                return Json(new { success = false, message = "IdJornada vacio." });

            jornada = await _context.Ejesreinsercion.SingleOrDefaultAsync(c => c.IdejesReinsercion == idJornada);

            if (jornada == null)
                return Json(new { success = false, message = "IdJornada no encontrado." });

            if (string.IsNullOrEmpty(NombreCampo))
                return Json(new { success = false, message = "Nombre del campo vacio." });

            if (string.IsNullOrEmpty(valor))
                return Json(new { success = false, message = "valor del campo vacio." });

            else
            {
                switch (NombreCampo)
                {
                    case "Lugar":
                        jornada.Lugar = mg.normaliza(valor);
                        break;
                    case "Area":
                        jornada.Area = mg.normaliza(valor);
                        break;

                    case "FechaCanalizacion":
                        DateTime? fechaCanalizacion = ConvertirAFecha(valor);
                        if (fechaCanalizacion.HasValue)
                            jornada.FechaCanalizacion = fechaCanalizacion.Value;
                        else
                            return Json(new { success = false, message = "Formato de fecha no válido para FechaCanalizacion." });
                        break;

                    case "FechaProgramada":
                        DateTime? fechaProgramada = ConvertirAFecha(valor);
                        if (fechaProgramada.HasValue)
                            jornada.FechaProgramada = fechaProgramada.Value;
                        else
                            return Json(new { success = false, message = "Formato de fecha no válido para FechaInicioTerapia." });
                        break;

                    case "FechaLimite":
                        DateTime? fechaLimite = ConvertirAFecha(valor);
                        if (fechaLimite.HasValue)
                            jornada.FechaLimite = fechaLimite.Value;
                        else
                            return Json(new { success = false, message = "Formato de fecha no válido para FechaTerminoTerapia." });
                        break;

                    case "Estado":
                        jornada.Estado = mg.normaliza(valor);
                        break;

                    case "HorasJornada":
                        jornada.HorasJornada = mg.normaliza(valor);
                        break;

                    case "NoHoraJornada":
                        jornada.NoHoraJornada = mg.normaliza(valor);
                        break;

                    case "Observaciones":
                        jornada.Observaciones = mg.normaliza(valor);
                        break;

                    default:
                        return Json(new { success = false, message = "Campo no válido." });
                }

                _context.Ejesreinsercion.Update(jornada);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Jornada actualizada correctamente." });
            }
        }
        #endregion


        #region -Alertas-
        public async Task<IActionResult> AlertasReinsercion(int? page, string searchString, string selectSearch)
        {
            if (!string.IsNullOrEmpty(selectSearch) && selectSearch.Equals("TODOS"))
                searchString = null;

            ViewBag.CurrentSelectSearch = selectSearch;
            ViewBag.CurrentSearchString = searchString;
            DateTime fechaActual = DateTime.Now;
            DateTime unMesAnterior = DateTime.Now.AddDays(-30);

            if (searchString != null && (page == 0 || page == null))
            {
                page = 1;
            }


            // LA QUERY VA A CRECER CONFORME SE VAYAN AGREGANDO MAS ALERTAS
            var query = (from persona in _context.Persona
                         join supervicionMC in _context.Supervision on persona.IdPersona equals supervicionMC.PersonaIdPersona
                         join cierreDeCasoMC in _context.Cierredecaso on supervicionMC.IdSupervision equals cierreDeCasoMC.SupervisionIdSupervision
                         where cierreDeCasoMC.SeCerroCaso.Equals("SI") &&
                         (cierreDeCasoMC.FechaAprobacion >= unMesAnterior && cierreDeCasoMC.FechaAprobacion <= fechaActual)
                         select new AlertasReinsercionViewModel
                         {
                             IdTabla = persona.IdPersona,
                             Nombre = $"{persona.Nombre} {persona.Paterno} {persona.Materno}",
                             Area = "MC Y SCP",
                             TipoAlerta = "Caso cerrado",
                             CierreCasoMC = cierreDeCasoMC,
                             FechaAprobacion = cierreDeCasoMC.FechaAprobacion
                         }).Union(
                       from personaCL in _context.Personacl
                       join supervisionCL in _context.Supervisioncl on personaCL.IdPersonaCl equals supervisionCL.PersonaclIdPersonacl
                       join cierreDeCasoCL in _context.Cierredecasocl on supervisionCL.IdSupervisioncl equals cierreDeCasoCL.SupervisionclIdSupervisioncl
                       where cierreDeCasoCL.SeCerroCaso.Equals("SI") &&
                       (cierreDeCasoCL.FechaAprobacion >= unMesAnterior && cierreDeCasoCL.FechaAprobacion <= fechaActual)
                       select new AlertasReinsercionViewModel
                       {
                           IdTabla = personaCL.IdPersonaCl,
                           Nombre = $"{personaCL.Nombre} {personaCL.Paterno} {personaCL.Materno}",
                           Area = "CL",
                           TipoAlerta = "Caso cerrado",
                           CierreCasoCL = cierreDeCasoCL,
                           FechaAprobacion = cierreDeCasoCL.FechaAprobacion
                       });



            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.Nombre.Contains(searchString.ToUpper()));

            }
            switch (selectSearch)
            {
                case "TODOS":
                    query = query.OrderByDescending(m => m.Area == "CL").ThenBy(m => m.FechaAprobacion);
                    break;
                case "MCYSCP":
                    query = query.Where(m => m.Area == "MC Y SCP");
                    break;
                case "CL":
                    query = query.Where(m => m.Area == "CL");
                    break;
                case "CASOCERRADO":
                    query = query.Where(m =>
                        (m.Area == "MC Y SCP" && m.CierreCasoMC != null && m.CierreCasoMC.SeCerroCaso.Equals("SI")) ||
                        (m.Area == "CL" && m.CierreCasoCL != null && m.CierreCasoCL.SeCerroCaso.Equals("SI"))
                    );
                    break;
                default:
                    query = query.OrderByDescending(m => m.Area == "CL").ThenBy(m => m.IdTabla);
                    break;
            }
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var paginatedList = await PaginatedList<AlertasReinsercionViewModel>.CreateAsync(query.AsNoTracking(), pageNumber, pageSize);

            return View(paginatedList);
        }
        #endregion

    }
}
