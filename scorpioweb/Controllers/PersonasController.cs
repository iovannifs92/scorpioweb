using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SautinSoft.Document;
using SautinSoft.Document.Drawing;
using QRCoder;
using System.Drawing;
using Size = SautinSoft.Document.Drawing.Size;
using System.Security.Claims;
using System.Data;
using Google.DataTable.Net.Wrapper.Extension;
using Google.DataTable.Net.Wrapper;
using MySql.Data.MySqlClient;

using System.Threading;
using Newtonsoft.Json.Linq;

namespace scorpioweb.Controllers
{
    [Authorize]
    public class PersonasController : Controller
    {
        //To get content root path of the project
        private readonly IHostingEnvironment _hostingEnvironment;

        #region -Variables Globales-
        private readonly penas2Context _context;
        public static int contadorSustancia;
        public static int contadorFamiliares;
        public static int contadorReferencias;
        public static List<List<string>> datosSustancias = new List<List<string>>();
        public static List<List<string>> datosSustanciasEditadas = new List<List<string>>();
        public static List<List<string>> datosFamiliares = new List<List<string>>();
        public static List<List<string>> datosFamiliaresEditados = new List<List<string>>();
        public static List<List<string>> datosReferencias = new List<List<string>>();
        public static List<List<string>> datosReferenciasEditadas = new List<List<string>>();
        public static List<List<string>> datosFamiliaresExtranjero = new List<List<string>>();
        public static List<List<string>> datosDomiciolioSecundario = new List<List<string>>();
        public static int idPersona;
        public static List<Consumosustancias> consumosustancias;
        public static List<Asientofamiliar> familiares;
        public static List<Asientofamiliar> referenciaspersonales;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private List<SelectListItem> listaNoSi = new List<SelectListItem>
        {
            new SelectListItem{ Text="No", Value="NO"},
            new SelectListItem{ Text="Si", Value="SI"}
        };
        private List<SelectListItem> listaNoSiNA = new List<SelectListItem>
        {
            new SelectListItem{ Text="NA", Value="NA"},
            new SelectListItem{ Text="No", Value="NO"},
            new SelectListItem{ Text="Si", Value="SI"}
        };

        private List<SelectListItem> listaSiNo = new List<SelectListItem>
        {
            new SelectListItem{ Text="Si", Value="SI"},
            new SelectListItem{ Text="No", Value="NO"}
        };

        private List<SelectListItem> listaZonas = new List<SelectListItem>
        {
            new SelectListItem{ Text="NA", Value="NA"},
            new SelectListItem{ Text="Zona 1", Value="ZONA 1"},
            new SelectListItem{ Text="Zona 2", Value="ZONA 2"},
            new SelectListItem{ Text="Zona 3", Value="ZONA 3"},
            new SelectListItem{ Text="Zona 4", Value="ZONA 4"},
            new SelectListItem{ Text="Zona 5", Value="ZONA 5"},
            new SelectListItem{ Text="Zona 6", Value="ZONA 6"},
            new SelectListItem{ Text="Zona 7", Value="ZONA 7"}
        };


        private List<SelectListItem> listaUbicacionExpediente = new List<SelectListItem>
        {
            new SelectListItem{ Text="NA", Value="NA"},
            new SelectListItem{ Text="MCSCP1-1", Value="MCSCP1-1"},
            new SelectListItem{ Text="MCSCP1-2", Value="MCSCP1-2"},
            new SelectListItem{ Text="MCSCP1-3", Value="MCSCP1-3"},
            new SelectListItem{ Text="MCSCP1-4", Value="MCSCP1-4"},
            new SelectListItem{ Text="MCSCP2-1", Value="MCSCP2-1"},
            new SelectListItem{ Text="MCSCP2-2", Value="MCSCP2-2"},
            new SelectListItem{ Text="MCSCP2-3", Value="MCSCP2-3"},
            new SelectListItem{ Text="MCSCP2-4", Value="MCSCP2-4"},
            new SelectListItem{ Text="MCSCP3-1", Value="MCSCP3-1"},
            new SelectListItem{ Text="MCSCP3-2", Value="MCSCP3-2"},
            new SelectListItem{ Text="MCSCP3-3", Value="MCSCP3-3"},
            new SelectListItem{ Text="MCSCP3-4", Value="MCSCP3-4"},
            new SelectListItem{ Text="MCSCP4-1", Value="MCSCP4-1"},
            new SelectListItem{ Text="MCSCP4-2", Value="MCSCP4-2"},
            new SelectListItem{ Text="MCSCP4-3", Value="MCSCP4-3"},
            new SelectListItem{ Text="MCSCP4-4", Value="MCSCP4-4"},
            new SelectListItem{ Text="MCSCP5-1", Value="MCSCP5-1"},
            new SelectListItem{ Text="MCSCP5-2", Value="MCSCP5-2"},
            new SelectListItem{ Text="MCSCP5-3", Value="MCSCP5-3"},
            new SelectListItem{ Text="MCSCP5-4", Value="MCSCP5-4"},
            new SelectListItem{ Text="MCSCP6-1", Value="MCSCP6-1"},
            new SelectListItem{ Text="MCSCP6-2", Value="MCSCP6-2"},
            new SelectListItem{ Text="MCSCP6-3", Value="MCSCP6-3"},
            new SelectListItem{ Text="MCSCP6-4", Value="MCSCP6-4"},
            new SelectListItem{ Text="MCSCP7-1", Value="MCSCP7-1"},
            new SelectListItem{ Text="MCSCP7-2", Value="MCSCP7-2"},
            new SelectListItem{ Text="MCSCP7-3", Value="MCSCP7-3"},
            new SelectListItem{ Text="MCSCP7-4", Value="MCSCP7-4"},
            new SelectListItem{ Text="PRESTAMO", Value="PRESTAMO"},
            new SelectListItem{ Text="ARCHIVO", Value="ARCHIVO"}
        };

        #endregion

        #region -Constructor-
        public PersonasController(penas2Context context, IHostingEnvironment hostingEnvironment,
                                  RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
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
                normalizar = "S-D";
            }
            return normalizar;
        }

        public string removeSpaces(string str)
        {
            if (str == null)
            {
                return "";
            }
            while (str.Length > 0 && str[0] == ' ')
            {
                str = str.Substring(1);
            }
            while (str.Length > 0 && str[str.Length - 1] == ' ')
            {
                str = str.Substring(0, str.Length - 1);
            }
            return str;
        }

        public static DateTime validateDatetime(string value)
        {
            try
            {
                return DateTime.Parse(value, new System.Globalization.CultureInfo("pt-BR"));
            }
            catch
            {
                return DateTime.ParseExact("1900/01/01", "yyyy/MM/dd", CultureInfo.InvariantCulture);
            }
        }

        String replaceSlashes(string path)
        {
            String cleaned = "";

            for (int i = 0; i < path.Length; i++)
                if (path[i] == '/')
                    cleaned += '-';
                else
                    cleaned += path[i];
            return cleaned;
        }
        #endregion

        #region -Index-
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            //para ver si la  persona tiene o no huella registrada
            var queryhayhuella = from r in _context.Registrohuella
                                 join p in _context.Presentacionperiodica on r.IdregistroHuella equals p.RegistroidHuella
                                 group r by r.PersonaIdPersona into grup
                                 select new
                                 {
                                     grup.Key,
                                     Count = grup.Count()
                                 };

            foreach (var personaHuella in queryhayhuella)
            {
                if (personaHuella.Count >= 1)
                {
                    ViewBag.personaIdPersona = personaHuella.Key;
                };
            }
            #region -ListaUsuarios-            
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.Admin = false;
            ViewBag.Masteradmin = false;
            ViewBag.Archivo = false;

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP")
                {
                    ViewBag.Admin = true;
                }
            }
            foreach (var rol in roles)
            {
                if (rol == "Masteradmin")
                {
                    ViewBag.Masteradmin = true;
                }
            }
            foreach (var rol in roles)
            {
                if (rol == "ArchivoMCSCP")
                {
                    ViewBag.Archivo = true;
                }
            }

            List<string> rolUsuario = new List<string>();

            for (int i = 0; i < roles.Count; i++)
            {
                rolUsuario.Add(roles[i]);
            }


            ViewBag.RolesUsuario = rolUsuario;

            String users = user.ToString();
            ViewBag.RolesUsuarios = users;

            List<String> ListaUsuarios = new List<String>();
            ListaUsuarios.Add("Sin Registro");
            ListaUsuarios.Add("Archivo Interno");
            ListaUsuarios.Add("Archivo General");
            ListaUsuarios.Add("No Ubicado");
            ListaUsuarios.Add("Dirección");
            ListaUsuarios.Add("Coordinación Operativa");
            ListaUsuarios.Add("Coordinación MC y SCP");
            foreach (var u in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(u, "SupervisorMCSCP"))
                {
                    ListaUsuarios.Add(u.ToString());
                }
            }
            ViewBag.ListadoUsuarios = ListaUsuarios;
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

            var personas = from p in _context.Persona
                           where p.Supervisor != null
                           select p;


            if (!String.IsNullOrEmpty(searchString))
            {
                foreach (var item in searchString.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(searchString) ||
                                                   (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(searchString) ||
                                                   p.Supervisor.Contains(searchString));
                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
                default:
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
            }

            int pageSize = 10;
            ViewBag.totalPages = (personas.Count() + pageSize - 1) / pageSize;

            //Response.Headers.Add("Refresh", "5");
            return View(await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
            //return Json(await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        Persona persona1;
        List<Persona> personas1;

        public async Task<ActionResult> Personas()
        {
            Tuple<List<Persona>, Persona> tuple;

            tuple = new Tuple<List<Persona>, Persona>(personas1, persona1);



            return View("PersonasDetails", tuple);
        }
        #endregion

        #region -ListadoSupervisor-
        public async Task<IActionResult> ListadoSupervisor(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            String users = user.ToString();
            ViewBag.RolesUsuario = users;


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
                foreach (var item in searchString.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(searchString) ||
                                                   (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(searchString) ||
                                                   (p.IdPersona.ToString()).Contains(searchString)
                                                   );
                }
            }


            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
                default:
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
            }
            int pageSize = 10;
            return Json(new
            {
                page = await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize),
                totalPages = (personas.Count() + pageSize - 1) / pageSize
            });
        }
        #endregion

        #region -Colaboraciones-
        public async Task<IActionResult> Colaboraciones()
        {
            var colaboraciones = from persona in _context.Persona
                                 join domicilio in _context.Domicilio on persona.IdPersona equals domicilio.PersonaIdPersona
                                 join municipio in _context.Municipios on int.Parse(domicilio.Municipio) equals municipio.Id
                                 where persona.Colaboracion == "SI"
                                 select new PersonaViewModel
                                 {
                                     personaVM = persona,
                                     municipiosVMDomicilio = municipio
                                 };

            return View(colaboraciones);
        }
        #endregion

        #region -MenuMCSCP-
        public async Task<IActionResult> MenuMCSCP()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            string usuario = user.ToString();
            DateTime fechaInforme = (DateTime.Now).AddDays(5);
            DateTime fechaControl = (DateTime.Now).AddDays(3);
            DateTime fechaInformeCoordinador = (DateTime.Now).AddDays(30);
            Boolean flagMaster = false;
            //Double comparaFecha;
            ViewBag.Warnings = 0;

            foreach (var rol in roles)
            {
                if (rol == "Masteradmin")
                {
                    flagMaster = true;
                }
            }

            #region -To List databases-

            List<Persona> personaVM = _context.Persona.ToList();
            List<Supervision> supervisionVM = _context.Supervision.ToList();
            List<Causapenal> causapenalVM = _context.Causapenal.ToList();
            List<Domicilio> domicilioVM = _context.Domicilio.ToList();
            List<Municipios> municipiosVM = _context.Municipios.ToList();
            List<Planeacionestrategica> planeacionestrategicaVM = _context.Planeacionestrategica.ToList();
            List<Fraccionesimpuestas> fraccionesimpuestasVM = _context.Fraccionesimpuestas.ToList();
            List<Archivointernomcscp> archivointernomcscpsVM = _context.Archivointernomcscp.ToList();
            List<Personacausapenal> personacausapenalsVM = _context.Personacausapenal.ToList();
            List<Fraccionesimpuestas> queryFracciones = (from f in fraccionesimpuestasVM
                                                         group f by f.SupervisionIdSupervision into grp
                                                         select grp.OrderByDescending(f => f.IdFracciones).FirstOrDefault()).ToList();
            List<Archivointernomcscp> queryHistorialArchivoadmin = (from a in _context.Archivointernomcscp
                                                                    group a by a.PersonaIdPersona into grp
                                                                    select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();
            #endregion

            #region -Jointables-


            var archivoadmin = from ha in queryHistorialArchivoadmin
                               join ai in archivointernomcscpsVM on ha.IdarchivoInternoMcscp equals ai.IdarchivoInternoMcscp
                               join p in personaVM on ha.PersonaIdPersona equals p.IdPersona
                               join domicilio in domicilioVM on p.IdPersona equals domicilio.PersonaIdPersona
                               join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                               where p.UbicacionExpediente != "ARCHIVO INTERNO" && p.UbicacionExpediente != "ARCHIVO GENERAL" &&
                               p.UbicacionExpediente != "NO UBICADO" && p.UbicacionExpediente != "SIN REGISTRO" && p.UbicacionExpediente != "NA" && p.UbicacionExpediente != null
                               join supervision in supervisionVM on p.IdPersona equals supervision.PersonaIdPersona into tmp
                               from sinsuper in tmp.DefaultIfEmpty()
                               select new PlaneacionWarningViewModel
                               {
                                   municipiosVM = municipio,
                                   personaVM = p,
                                   archivointernomcscpVM = ai,
                                   tipoAdvertencia = "Expediente físico en resguardo"
                               };

            var leftJoin = from persona in personaVM
                           join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona into tmp
                           from sinsupervision in tmp.DefaultIfEmpty()
                           select new PlaneacionWarningViewModel
                           {
                               personaVM = persona,
                               supervisionVM = sinsupervision,
                               tipoAdvertencia = "Sin supervisión"
                           };
            var where = from ss in leftJoin
                        where ss.supervisionVM == null
                        select new PlaneacionWarningViewModel
                        {
                            personaVM = ss.personaVM,
                            supervisionVM = ss.supervisionVM,
                            tipoAdvertencia = "Sin supervisión"
                        };
            var where2 = from ss in leftJoin
                         where ss.personaVM.Supervisor == usuario && ss.supervisionVM == null
                         select new PlaneacionWarningViewModel
                         {
                             personaVM = ss.personaVM,
                             supervisionVM = ss.supervisionVM,
                             tipoAdvertencia = "Sin supervisión"
                         };

            if (usuario == "esmeralda.vargas@dgepms.com" || usuario == "janeth@nortedgepms.com" || flagMaster == true)
            {
                var warningPlaneacion = (where).Union
                                        (archivoadmin).Union
                                        (from persona in personaVM
                                         join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                         join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                         where persona.Colaboracion == "SI"
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = persona,
                                             municipiosVM = municipio,
                                             tipoAdvertencia = "Pendiente de asignación - colaboración"
                                         }).Union
                                        (from persona in personaVM
                                         join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                         join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                         join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                         join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                         where planeacion.FechaInforme != null && planeacion.FechaInforme < fechaInformeCoordinador && supervision.EstadoSupervision == "VIGENTE" && fracciones.FiguraJudicial == "SCP"
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = persona,
                                             supervisionVM = supervision,
                                             causapenalVM = causapenal,
                                             planeacionestrategicaVM = planeacion,
                                             fraccionesimpuestasVM = fracciones,
                                             figuraJudicial = fracciones.FiguraJudicial,
                                             tipoAdvertencia = "Informe fuera de tiempo"
                                         }).Union
                                        (from persona in personaVM
                                         join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                         join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                         join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                         join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                         where planeacion.FechaInforme != null && planeacion.FechaInforme < fechaControl && supervision.EstadoSupervision == "VIGENTE" && fracciones.FiguraJudicial == "MC"
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = persona,
                                             supervisionVM = supervision,
                                             causapenalVM = causapenal,
                                             planeacionestrategicaVM = planeacion,
                                             fraccionesimpuestasVM = fracciones,
                                             figuraJudicial = fracciones.FiguraJudicial,
                                             tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                         }).Union
                                    (from persona in personaVM
                                     join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                     join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                     join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                     join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                     where planeacion.FechaInforme == null && supervision.EstadoSupervision == "VIGENTE"
                                     orderby fracciones.FiguraJudicial
                                     select new PlaneacionWarningViewModel
                                     {
                                         personaVM = persona,
                                         supervisionVM = supervision,
                                         causapenalVM = causapenal,
                                         planeacionestrategicaVM = planeacion,
                                         fraccionesimpuestasVM = fracciones,
                                         figuraJudicial = fracciones.FiguraJudicial,
                                         tipoAdvertencia = "Sin fecha de informe"
                                     }).Union
                                    (from persona in personaVM
                                     join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                     join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                     join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                     where planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                     select new PlaneacionWarningViewModel
                                     {
                                         personaVM = persona,
                                         supervisionVM = supervision,
                                         causapenalVM = causapenal,
                                         planeacionestrategicaVM = planeacion,
                                         tipoAdvertencia = "Sin periodicidad de firma"
                                     }).Union
                                            (from persona in personaVM
                                             join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                             join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                             join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                             where persona.Supervisor != null && persona.Supervisor.EndsWith("\u0040dgepms.com") && planeacion.FechaProximoContacto != null && planeacion.FechaProximoContacto < fechaControl && supervision.EstadoSupervision == "VIGENTE"
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = persona,
                                                 supervisionVM = supervision,
                                                 causapenalVM = causapenal,
                                                 planeacionestrategicaVM = planeacion,
                                                 tipoAdvertencia = "Se paso el tiempo de la firma"
                                             })
                //.Union
                //(from persona in personaVM
                // join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                // join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                // join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                // where supervision.EstadoSupervision == null
                // select new PlaneacionWarningViewModel
                // {
                //     personaVM = persona,
                //     supervisionVM = supervision,
                //     causapenalVM = causapenal,
                //     planeacionestrategicaVM = planeacion,
                //     tipoAdvertencia = "Sin estado de supervisión"
                // });
                ;
                var warnings = Enumerable.Empty<PlaneacionWarningViewModel>();
                if (usuario == "janeth@nortedgepms.com" || flagMaster == true)
                {
                    var filteredWarnings = from pwvm in warningPlaneacion
                                           where pwvm.personaVM.Supervisor != null && pwvm.personaVM.Supervisor.EndsWith("\u0040nortedgepms.com")
                                           select pwvm;
                    warnings = warnings.Union(filteredWarnings);
                }
                if (usuario == "esmeralda.vargas@dgepms.com" || flagMaster == true)
                {
                    var filteredWarnings = from pwvm in warningPlaneacion
                                           where pwvm.personaVM.Supervisor != null && pwvm.personaVM.Supervisor.EndsWith("\u0040dgepms.com")
                                           select pwvm;
                    warnings = warnings.Union(filteredWarnings);
                }
                ViewBag.Warnings = warnings.Count();
            }
            else
            {
                List<Archivointernomcscp> queryHistorialArchivo = (from a in _context.Archivointernomcscp
                                                                   group a by a.PersonaIdPersona into grp
                                                                   select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();



                var archivo = from ha in queryHistorialArchivoadmin
                              join ai in archivointernomcscpsVM on ha.IdarchivoInternoMcscp equals ai.IdarchivoInternoMcscp
                              join p in personaVM on ha.PersonaIdPersona equals p.IdPersona
                              join domicilio in domicilioVM on p.IdPersona equals domicilio.PersonaIdPersona
                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                              where p.UbicacionExpediente == usuario.ToUpper() && p.UbicacionExpediente != null
                              join supervision in supervisionVM on p.IdPersona equals supervision.PersonaIdPersona into tmp
                              from sinsuper in tmp.DefaultIfEmpty()
                              select new PlaneacionWarningViewModel
                              {
                                  municipiosVM = municipio,
                                  personaVM = p,
                                  archivointernomcscpVM = ai,
                                  tipoAdvertencia = "Expediente físico en resguardo"
                              };

                var warningPlaneacion = where2.Union
                                        (archivo).Union
                                        (from persona in personaVM
                                         join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                         join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                         join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                         join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                         where persona.Supervisor == usuario && planeacion.FechaInforme != null && planeacion.FechaInforme < fechaInformeCoordinador && supervision.EstadoSupervision == "VIGENTE" && fracciones.FiguraJudicial == "SCP"
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = persona,
                                             supervisionVM = supervision,
                                             causapenalVM = causapenal,
                                             planeacionestrategicaVM = planeacion,
                                             fraccionesimpuestasVM = fracciones,
                                             figuraJudicial = fracciones.FiguraJudicial,
                                             tipoAdvertencia = "Informe fuera de tiempo"
                                         }).Union
                                        (from persona in personaVM
                                         join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                         join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                         join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                         join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                         where persona.Supervisor == usuario && planeacion.FechaInforme != null && planeacion.FechaInforme < fechaControl && supervision.EstadoSupervision == "VIGENTE" && fracciones.FiguraJudicial == "MC"
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = persona,
                                             supervisionVM = supervision,
                                             causapenalVM = causapenal,
                                             planeacionestrategicaVM = planeacion,
                                             fraccionesimpuestasVM = fracciones,
                                             figuraJudicial = fracciones.FiguraJudicial,
                                             tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                         }).Union
                                    (from persona in personaVM
                                     join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                     join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                     join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                     join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                     where persona.Supervisor == usuario && planeacion.FechaInforme == null && supervision.EstadoSupervision == "VIGENTE"
                                     orderby fracciones.FiguraJudicial
                                     select new PlaneacionWarningViewModel
                                     {
                                         personaVM = persona,
                                         supervisionVM = supervision,
                                         causapenalVM = causapenal,
                                         planeacionestrategicaVM = planeacion,
                                         fraccionesimpuestasVM = fracciones,
                                         figuraJudicial = fracciones.FiguraJudicial,
                                         tipoAdvertencia = "Sin fecha de informe"
                                     }).Union
                                    (from persona in personaVM
                                     join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                     join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                     join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                     where persona.Supervisor == usuario && planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                     select new PlaneacionWarningViewModel
                                     {
                                         personaVM = persona,
                                         supervisionVM = supervision,
                                         causapenalVM = causapenal,
                                         planeacionestrategicaVM = planeacion,
                                         tipoAdvertencia = "Sin periodicidad de firma"
                                     }).Union
                                    (from persona in personaVM
                                     join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                     join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                     join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                     where persona.Supervisor != null && persona.Supervisor.EndsWith("\u0040dgepms.com") && persona.Supervisor == usuario && planeacion.FechaProximoContacto != null && planeacion.FechaProximoContacto < fechaControl && supervision.EstadoSupervision == "VIGENTE"
                                     select new PlaneacionWarningViewModel
                                     {
                                         personaVM = persona,
                                         supervisionVM = supervision,
                                         causapenalVM = causapenal,
                                         planeacionestrategicaVM = planeacion,
                                         tipoAdvertencia = "Se paso el tiempo de la firma"
                                     })

                //.Union
                //(from persona in personaVM
                // join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                // join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                // join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                // where persona.Supervisor == usuario && supervision.EstadoSupervision == null
                // select new PlaneacionWarningViewModel
                // {
                //     personaVM = persona,
                //     supervisionVM = supervision,
                //     causapenalVM = causapenal,
                //     planeacionestrategicaVM = planeacion,
                //     tipoAdvertencia = "Sin estado de supervisión"
                // });
                ;
                ViewBag.Warnings = warningPlaneacion.Count();
            }
            #endregion

            List<string> rolUsuario = new List<string>();

            for (int i = 0; i < roles.Count; i++)
            {
                rolUsuario.Add(roles[i]);
            }

            ViewBag.RolesUsuario = rolUsuario;
            return View();
        }



        #endregion

        #region -AsignaSupervision-

        public async Task<IActionResult> AsignacionSupervision()
        {
            List<SelectListItem> ListaUsuarios = new List<SelectListItem>();
            int i = 0;
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, "SupervisorMCSCP"))
                {
                    ListaUsuarios.Add(new SelectListItem
                    {
                        Text = user.ToString(),
                        Value = i.ToString()
                    });
                    i++;
                }
            }
            ViewBag.ListadoUsuarios = ListaUsuarios;

            var supervisoresScorpio = from s in _context.Supervision
                                      join p in _context.Persona on s.PersonaIdPersona equals p.IdPersona
                                      where s.EstadoSupervision == "VIGENTE"
                                      group p by p.Supervisor into grup
                                      select new
                                      {
                                          grup.Key,
                                          Count = grup.Count()
                                      };

            var supervisoresBD = from c in _context.Controlsupervisiones
                                 select new
                                 {
                                     c.Supervisor,
                                     c.Supervisados
                                 };

            var result = (from s in supervisoresScorpio
                          join b in supervisoresBD on s.Key equals b.Supervisor
                          select new
                          {
                              b.Supervisor,
                              Supervisados = s.Count + b.Supervisados
                          }).ToList();

            var recomendar = (((from r in result
                                orderby r.Supervisados ascending
                                select new
                                {
                                    r.Supervisor
                                }).Take(1))).ToArray();

            string recomendacion = (recomendar[0].Supervisor).ToString();

            ViewBag.Recomendacion = recomendacion;
            return View(await _context.Persona.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupervisor(Persona persona)
        {
            int id = persona.IdPersona;
            string supervisor = persona.Supervisor;

            var personaUpdate = await _context.Persona
                .FirstOrDefaultAsync(p => p.IdPersona == id);

            personaUpdate.Supervisor = supervisor;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonaExists(persona.IdPersona))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("MenuMCSCP");
        }

        #endregion

        #region -Reasignacion-



        public async Task<IActionResult> Reasignacion(
           string sortOrder,
           string currentFilter,
           string searchString,
           int? pageNumber)
        {



            List<SelectListItem> ListaUsuarios = new List<SelectListItem>();
            int i = 0;
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, "SupervisorMCSCP"))
                {
                    ListaUsuarios.Add(new SelectListItem
                    {
                        Text = user.ToString(),
                        Value = i.ToString()
                    });
                    i++;
                }
            }
            ViewBag.ListadoUsuarios = ListaUsuarios;

            var queryhayhuella = from r in _context.Registrohuella
                                 join p in _context.Presentacionperiodica on r.IdregistroHuella equals p.RegistroidHuella
                                 group r by r.PersonaIdPersona into grup
                                 select new
                                 {
                                     grup.Key,
                                     Count = grup.Count()
                                 };

            foreach (var personaHuella in queryhayhuella)
            {
                if (personaHuella.Count >= 1)
                {

                    ViewBag.personaIdPersona = personaHuella.Key;
                };
            }


            #region -ListaUsuarios-            
            var usr = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(usr);

            List<string> rolUsuario = new List<string>();

            for (int e = 0; e < roles.Count; e++)
            {
                rolUsuario.Add(roles[e]);
            }

            ViewBag.RolesUsuario = rolUsuario;

            String users = usr.ToString();
            ViewBag.RolesUsuarios = users;
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

            var personas = from p in _context.Persona
                           where p.Supervisor != null
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                foreach (var item in searchString.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(searchString) ||
                                                   (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(searchString) ||
                                                   p.Supervisor.Contains(searchString) || (p.IdPersona.ToString()).Contains(searchString));

                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
                default:
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
            }

            int pageSize = 10;
            // Response.Headers.Add("Refresh", "5");
            return View(await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupervisorReasignacion(Persona persona, Reasignacionsupervisor reasignacionsupervisor)
        {
            int id = persona.IdPersona;
            string idS = (persona.IdPersona).ToString();
            string supervisor = persona.Supervisor;
            string motivo = reasignacionsupervisor.MotivoRealizo;
            string currentUser = User.Identity.Name;

            var sA = from p in _context.Persona
                     where p.IdPersona == id
                     select new
                     {
                         p.Supervisor
                     };

            reasignacionsupervisor.PersonaIdpersona = (persona.IdPersona).ToString();
            reasignacionsupervisor.MotivoRealizo = reasignacionsupervisor.MotivoRealizo;
            reasignacionsupervisor.FechaReasignacion = DateTime.Now;
            reasignacionsupervisor.SAntiguo = sA.FirstOrDefault().Supervisor.ToString();
            reasignacionsupervisor.QuienRealizo = currentUser;
            reasignacionsupervisor.SNuevo = supervisor;
            _context.Add(reasignacionsupervisor);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);

            var personaUpdate = await _context.Persona
                .FirstOrDefaultAsync(p => p.IdPersona == id);
            personaUpdate.Supervisor = supervisor;



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonaExists(persona.IdPersona))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Reasignacion");
        }

        #endregion

        #region -Detalles-

        // GET: Personas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona
                .SingleOrDefaultAsync(m => m.IdPersona == id);

            var domicilioEM = await _context.Domicilio.SingleOrDefaultAsync(d => d.PersonaIdPersona == id);

            #region -To List databases-

            List<Persona> personaVM = _context.Persona.ToList();
            List<Domicilio> domicilioVM = _context.Domicilio.ToList();
            List<Estudios> estudiosVM = _context.Estudios.ToList();
            List<Estados> estados = _context.Estados.ToList();
            List<Municipios> municipios = _context.Municipios.ToList();
            List<Domiciliosecundario> domicilioSecundarioVM = _context.Domiciliosecundario.ToList();
            List<Consumosustancias> consumoSustanciasVM = _context.Consumosustancias.ToList();
            List<Trabajo> trabajoVM = _context.Trabajo.ToList();
            List<Actividadsocial> actividadSocialVM = _context.Actividadsocial.ToList();
            List<Abandonoestado> abandonoEstadoVM = _context.Abandonoestado.ToList();
            List<Saludfisica> saludFisicaVM = _context.Saludfisica.ToList();
            List<Familiaresforaneos> familiaresForaneosVM = _context.Familiaresforaneos.ToList();
            List<Asientofamiliar> asientoFamiliarVM = _context.Asientofamiliar.ToList();

            #endregion

            #region -Jointables-
            ViewData["joinTables"] = from personaTable in personaVM
                                     join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                     join estudios in estudiosVM on persona.IdPersona equals estudios.PersonaIdPersona
                                     join trabajo in trabajoVM on persona.IdPersona equals trabajo.PersonaIdPersona
                                     join actividaSocial in actividadSocialVM on persona.IdPersona equals actividaSocial.PersonaIdPersona
                                     join abandonoEstado in abandonoEstadoVM on persona.IdPersona equals abandonoEstado.PersonaIdPersona
                                     join saludFisica in saludFisicaVM on persona.IdPersona equals saludFisica.PersonaIdPersona
                                     //join nacimientoEstado in estados on (Int32.Parse(persona.Lnestado)) equals nacimientoEstado.Id
                                     //join nacimientoMunicipio in municipios on (Int32.Parse(persona.Lnmunicipio)) equals nacimientoMunicipio.Id
                                     //join domicilioEstado in estados on (Int32.Parse(domicilio.Estado)) equals domicilioEstado.Id
                                     //join domicilioMunicipio in municipios on (Int32.Parse(domicilio.Municipio)) equals domicilioMunicipio.Id
                                     where personaTable.IdPersona == id
                                     select new PersonaViewModel
                                     {
                                         personaVM = personaTable,
                                         domicilioVM = domicilio,
                                         estudiosVM = estudios,
                                         trabajoVM = trabajo,
                                         actividadSocialVM = actividaSocial,
                                         abandonoEstadoVM = abandonoEstado,
                                         saludFisicaVM = saludFisica
                                         //estadosVMPersona=nacimientoEstado,
                                         //municipiosVMPersona=nacimientoMunicipio,  
                                         //estadosVMDomicilio = domicilioEstado,
                                         //municipiosVMDomicilio= domicilioMunicipio,
                                     };

            #endregion


            #region Sacar el nombre de estdo y municipio (NACIMIENTO)
            var LNE = (from e in _context.Estados
                       join p in _context.Persona on e.Id equals int.Parse(p.Lnestado)
                       where p.IdPersona == id
                       select new
                       {
                           e.Estado
                       });

            string selectem1 = LNE.FirstOrDefault().Estado.ToString();
            ViewBag.lnestado = selectem1.ToUpper();

            var LNM = (from m in _context.Municipios
                       join p in _context.Persona on m.Id equals int.Parse(p.Lnestado)
                       where p.IdPersona == id
                       select new
                       {
                           m.Municipio
                       });

            string selectem2 = LNM.FirstOrDefault().Municipio.ToString();
            ViewBag.lnmunicipio = selectem2.ToUpper();





            #endregion

            #region Sacar el nombre de estdo y municipio (DOMICILIO)
            var E = (from d in _context.Domicilio
                     join m in _context.Estados on int.Parse(d.Estado) equals m.Id
                     join p in _context.Persona on d.PersonaIdPersona equals id
                     select new
                     {
                         m.Estado
                     });

            string selectem3 = E.FirstOrDefault().Estado.ToString();
            ViewBag.estado = selectem3.ToUpper();

            var M = (from d in _context.Domicilio
                     join m in _context.Municipios on int.Parse(d.Municipio) equals m.Id
                     join p in _context.Persona on d.PersonaIdPersona equals id
                     select new
                     {
                         m.Municipio,
                         m.Id
                     });

            string selectem4 = M.FirstOrDefault().Municipio.ToString();
            ViewBag.municipio = selectem4.ToUpper();
            #endregion

            #region Lnmunicipio
            int Lnestado;
            bool success = Int32.TryParse(persona.Lnestado, out Lnestado);
            List<Municipios> listaMunicipios = new List<Municipios>();
            if (success)
            {
                listaMunicipios = (from table in _context.Municipios
                                   where table.EstadosId == Lnestado
                                   select table).ToList();
            }

            listaMunicipios.Insert(0, new Municipios { Id = 0, Municipio = "Selecciona" });

            ViewBag.ListadoMunicipios = listaMunicipios;
            ViewBag.idMunicipio = persona.Lnmunicipio;
            #endregion

            #region -JoinTables null-
            ViewData["joinTablesDomSec"] = from personaTable in personaVM
                                           join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                           join domicilioSec in domicilioSecundarioVM.DefaultIfEmpty() on domicilio.IdDomicilio equals domicilioSec.IdDomicilio
                                           where personaTable.IdPersona == id
                                           select new PersonaViewModel
                                           {
                                               domicilioSecundarioVM = domicilioSec
                                           };

            ViewData["joinTablesConsumoSustancias"] = from personaTable in personaVM
                                                      join sustancias in consumoSustanciasVM on persona.IdPersona equals sustancias.PersonaIdPersona
                                                      where personaTable.IdPersona == id
                                                      select new PersonaViewModel
                                                      {
                                                          consumoSustanciasVM = sustancias
                                                      };

            ViewData["joinTablesFamiliaresForaneos"] = from personaTable in personaVM
                                                       join familiarForaneo in familiaresForaneosVM on persona.IdPersona equals familiarForaneo.PersonaIdPersona
                                                       where personaTable.IdPersona == id
                                                       select new PersonaViewModel
                                                       {
                                                           familiaresForaneosVM = familiarForaneo
                                                       };

            ViewData["joinTablesFamiliares"] = from personaTable in personaVM
                                               join familiar in asientoFamiliarVM on persona.IdPersona equals familiar.PersonaIdPersona
                                               where personaTable.IdPersona == id && familiar.Tipo == "FAMILIAR"
                                               select new PersonaViewModel
                                               {
                                                   asientoFamiliarVM = familiar
                                               };

            ViewData["joinTablesReferencia"] = from personaTable in personaVM
                                               join referencia in asientoFamiliarVM on persona.IdPersona equals referencia.PersonaIdPersona
                                               where personaTable.IdPersona == id && referencia.Tipo == "REFERENCIA"
                                               select new PersonaViewModel
                                               {
                                                   asientoFamiliarVM = referencia
                                               };


            ViewBag.Referencia = ((ViewData["joinTablesReferencia"] as IEnumerable<scorpioweb.Models.PersonaViewModel>).Count()).ToString();

            ViewBag.Familiar = ((ViewData["joinTablesFamiliares"] as IEnumerable<scorpioweb.Models.PersonaViewModel>).Count()).ToString();
            #endregion


            if (persona == null)
            {
                return NotFound();
            }

            return View();
        }
        #endregion

        #region -Entrevista de encuadre insertar-

        #region -porBORRAR-
        public ActionResult guardarSustancia(string[] datosConsumo)
        {
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosConsumo.Length; i++)
            {
                datosSustancias.Add(new List<String> { datosConsumo[i], currentUser });
            }

            return Json(new { success = true, responseText = "Datos Guardados con éxito" });

        }
        public ActionResult agregarSustancias()
        {
            //por si no se vacian las listas despues de guardar el modal
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosSustancias.Count; i++)
            {
                if (datosSustancias[i][1] == currentUser)
                {
                    datosSustancias.RemoveAt(i);
                    i--;
                }
            }

            return Json(new { success = true });
        }
        public ActionResult guardarFamiliar(string[] datosFamiliar, int tipoGuardado)
        {
            string currentUser = User.Identity.Name;
            if (tipoGuardado == 1)
            {
                for (int i = 0; i < datosFamiliar.Length; i++)
                {
                    datosFamiliares.Add(new List<String> { datosFamiliar[i], currentUser });
                }
            }
            else if (tipoGuardado == 2)
            {
                for (int i = 0; i < datosFamiliar.Length; i++)
                {
                    datosReferencias.Add(new List<String> { datosFamiliar[i], currentUser });
                }
            }


            return Json(new { success = true, responseText = "Datos Guardados con éxito" });

        }
        public ActionResult agregarAsientoFamiliar(int tipo)
        {
            string currentUser = User.Identity.Name;
            if (tipo == 1)
            {
                for (int i = 0; i < datosFamiliares.Count; i++)
                {
                    if (datosFamiliares[i][1] == currentUser)
                    {
                        datosFamiliares.RemoveAt(i);
                        i--;
                    }
                }
            }
            else if (tipo == 2)
            {
                for (int i = 0; i < datosReferencias.Count; i++)
                {
                    if (datosReferencias[i][1] == currentUser)
                    {
                        datosReferencias.RemoveAt(i);
                        i--;
                    }
                }
            }
            return Json(new { success = true });
        }
        public ActionResult guardarDomcililiosecudario(string[] datosDS)
        {
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosDS.Length; i++)
            {
                datosDomiciolioSecundario.Add(new List<String> { datosDS[i], currentUser });
            }

            return Json(new { success = true, responseText = "Datos Guardados con éxito" });

        }
        public ActionResult guardarFamiliarExtranjero(string[] datosFE)
        {
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosFE.Length; i++)
            {
                datosFamiliaresExtranjero.Add(new List<String> { datosFE[i], currentUser });
            }

            return Json(new { success = true, responseText = "Datos Guardados con éxito" });

        }
        #endregion

        #region -EditaSustancias-
        public ActionResult siguienteSustancia(string[] datosConsumo)
        {
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosConsumo.Length; i++)
            {
                datosSustanciasEditadas.Add(new List<String> { datosConsumo[i], currentUser });
            }

            if (contadorSustancia == consumosustancias.Count)
            {
                return Json(new { success = true, responseText = "Datos Guardados con éxito" });
            }
            else
            {
                return Json(new
                {
                    success = true,
                    responseText = "Siguiente",
                    sustancia = consumosustancias[contadorSustancia].Sustancia,
                    frecuencia = consumosustancias[contadorSustancia].Frecuencia,
                    cantidad = consumosustancias[contadorSustancia].Cantidad,
                    ultimoConsumo = consumosustancias[contadorSustancia].UltimoConsumo,
                    observacionesConsumo = consumosustancias[contadorSustancia].Observaciones,
                    idConsumoSustancias = consumosustancias[contadorSustancia++].IdConsumoSustancias
                });
            }
        }

        public ActionResult editarSustancias()
        {
            contadorSustancia = 1;//por cargar la 1er sustancia

            //por si no se vacian las listas despues de guardar
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosSustanciasEditadas.Count; i++)
            {
                if (datosSustanciasEditadas[i][1] == currentUser)
                {
                    datosSustanciasEditadas.RemoveAt(i);
                    i--;
                }
            }

            return Json(new { success = true });
        }

        #endregion

        #region -EditaFamiliar-
        public ActionResult siguienteFamiliar(string[] datosFamiliar, int tipoGuardado)
        {
            string currentUser = User.Identity.Name;

            if (tipoGuardado == 1)
            {
                for (int i = 0; i < datosFamiliar.Length; i++)
                {
                    datosFamiliaresEditados.Add(new List<String> { datosFamiliar[i], currentUser });
                }

                if (contadorFamiliares == familiares.Count)
                {
                    return Json(new { success = true, responseText = "Datos Guardados con éxito" });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        responseText = "Siguiente",
                        nombre = familiares[contadorFamiliares].Nombre,
                        relacion = familiares[contadorFamiliares].Relacion,
                        edad = familiares[contadorFamiliares].Edad,
                        sexo = familiares[contadorFamiliares].Sexo,
                        dependencia = familiares[contadorFamiliares].Dependencia,
                        explicaDependencia = familiares[contadorFamiliares].DependenciaExplica,
                        vivenJuntos = familiares[contadorFamiliares].VivenJuntos,
                        direccion = familiares[contadorFamiliares].Domicilio,
                        telefono = familiares[contadorFamiliares].Telefono,
                        horarioLocalizacion = familiares[contadorFamiliares].HorarioLocalizacion,
                        enteradoProceso = familiares[contadorFamiliares].EnteradoProceso,
                        puedeEnterarse = familiares[contadorFamiliares].PuedeEnterarse,
                        observaciones = familiares[contadorFamiliares].Observaciones,
                        idAsientoFamiliar = familiares[contadorFamiliares++].IdAsientoFamiliar
                    });
                }
            }
            else
            {
                for (int i = 0; i < datosFamiliar.Length; i++)
                {
                    datosReferenciasEditadas.Add(new List<String> { datosFamiliar[i], currentUser });
                }

                if (contadorReferencias == referenciaspersonales.Count)
                {
                    return Json(new { success = true, responseText = "Datos Guardados con éxito" });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        responseText = "Siguiente",
                        nombre = referenciaspersonales[contadorReferencias].Nombre,
                        relacion = referenciaspersonales[contadorReferencias].Relacion,
                        edad = referenciaspersonales[contadorReferencias].Edad,
                        sexo = referenciaspersonales[contadorReferencias].Sexo,
                        dependencia = referenciaspersonales[contadorReferencias].Dependencia,
                        explicaDependencia = referenciaspersonales[contadorReferencias].DependenciaExplica,
                        vivenJuntos = referenciaspersonales[contadorReferencias].VivenJuntos,
                        direccion = referenciaspersonales[contadorReferencias].Domicilio,
                        telefono = referenciaspersonales[contadorReferencias].Telefono,
                        horarioLocalizacion = referenciaspersonales[contadorReferencias].HorarioLocalizacion,
                        enteradoProceso = referenciaspersonales[contadorReferencias].EnteradoProceso,
                        puedeEnterarse = referenciaspersonales[contadorReferencias].PuedeEnterarse,
                        observaciones = referenciaspersonales[contadorReferencias].Observaciones,
                        idAsientoFamiliar = referenciaspersonales[contadorReferencias++].IdAsientoFamiliar
                    });
                }
            }
        }

        public ActionResult editarFamiliares(int tipoGuardado)
        {
            string currentUser = User.Identity.Name;
            if (tipoGuardado == 1)
            {
                contadorFamiliares = 1;
                for (int i = 0; i < datosFamiliaresEditados.Count; i++)
                {
                    if (datosFamiliaresEditados[i][1] == currentUser)
                    {
                        datosFamiliaresEditados.RemoveAt(i);
                        i--;
                    }
                }
            }
            else if (tipoGuardado == 2)
            {
                contadorReferencias = 1;
                for (int i = 0; i < datosReferenciasEditadas.Count; i++)
                {
                    if (datosReferenciasEditadas[i][1] == currentUser)
                    {
                        datosReferenciasEditadas.RemoveAt(i);
                        i--;
                    }
                }
            }
            return Json(new { success = true });
        }
        #endregion

        public ActionResult agregarAgregardomiciliosecuendario()
        {
            datosDomiciolioSecundario = new List<List<string>>();

            return Json(new { success = true });
        }

        public ActionResult agregarFamiliaresExtranjeros()
        {
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosFamiliaresExtranjero.Count; i++)
            {
                if (datosFamiliaresExtranjero[i][1] == currentUser)
                {
                    datosFamiliaresExtranjero.RemoveAt(i);
                    i--;
                }
            }

            return Json(new { success = true });
        }

        #region -Estados y Municipios-
        public JsonResult GetMunicipio(int EstadoId)
        {
            TempData["message"] = DateTime.Now;
            List<Municipios> municipiosList = new List<Municipios>();

            if (EstadoId != 0)
            {

                municipiosList = (from Municipios in _context.Municipios
                                  where Municipios.EstadosId == EstadoId
                                  select Municipios).ToList();
            }
            else
            {
                municipiosList.Insert(0, new Municipios { Id = 0, Municipio = "Selecciona" });
            }

            return Json(new SelectList(municipiosList, "Id", "Municipio"));
        }

        public JsonResult GetMunicipioED(int EstadoId)
        {
            TempData["message"] = DateTime.Now;
            List<Municipios> municipiosList = new List<Municipios>();

            if (EstadoId != 0)
            {

                municipiosList = (from Municipios in _context.Municipios
                                  where Municipios.EstadosId == EstadoId
                                  select Municipios).ToList();
            }
            else
            {
                municipiosList.Insert(0, new Municipios { Id = 0, Municipio = "Selecciona" });
            }
            municipiosList.Insert(0, new Municipios { Id = 0, Municipio = "Selecciona" });
            return Json(new SelectList(municipiosList, "Id", "Municipio"));
        }

        public string generaEstado(string id)
        {
            string estado = "";

            if (id == "0")
            {
                estado = "SIN ESTADO";
            }
            else
            {
                if (!String.IsNullOrEmpty(id))
                {
                    List<Estados> estados = _context.Estados.ToList();
                    estado = (estados.FirstOrDefault(x => x.Id == Int32.Parse(id)).Estado).ToUpper();
                }
            }
            return estado;
        }


        public string generaMunicipio(string id)
        {
            string municipio = "";

            if (id == "0")
            {
                municipio = "SIN MUNICIPIO";
            }
            else
            {
                if (!String.IsNullOrEmpty(id))
                {
                    List<Municipios> municipios = _context.Municipios.ToList();
                    municipio = (municipios.FirstOrDefault(x => x.Id == Int32.Parse(id)).Municipio).ToUpper();
                }
            }
            return municipio;
        }
        #endregion

        #region -CREATE-
        // GET: Personas/Create
        [Authorize(Roles = "AdminMCSCP, SupervisorMCSCP, Masteradmin, Asistente, AuxiliarMCSCP ")]
        public IActionResult Create(Estados Estados)
        {
            //datosSustancias.Clear();            
            List<Estados> listaEstados = new List<Estados>();
            listaEstados = (from table in _context.Estados
                            select table).ToList();
            listaEstados.Insert(0, new Estados { Id = 0, Estado = "Selecciona" });
            ViewBag.ListadoEstados = listaEstados;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Persona persona, Domicilio domicilio, Estudios estudios, Trabajo trabajo, Actividadsocial actividadsocial, Abandonoestado abandonoEstado, Saludfisica saludfisica, Domiciliosecundario domiciliosecundario, Consumosustancias consumosustanciasBD, Asientofamiliar asientoFamiliar, Familiaresforaneos familiaresForaneos,
            string nombre, string paterno, string materno, string alias, string sexo, int edad, DateTime fNacimiento, string lnPais,
            string lnEstado, string lnMunicipio, string lnLocalidad, string estadoCivil, string duracion, string otroIdioma, string especifiqueIdioma,
            string leerEscribir, string traductor, string especifiqueTraductor, string telefonoFijo, string celular, string hijos, int nHijos, int nPersonasVive,
            string propiedades, string CURP, string consumoSustancias, string familiares, string referenciasPersonales, string ubicacionExpediente,
            string tipoDomicilio, string calle, string no, string nombreCF, string paisD, string estadoD, string municipioD, string temporalidad, string zona,
            string residenciaHabitual, string cp, string referencias, string horario, string observaciones, string cuentaDomicilioSecundario,
            /*string motivoDS, string tipoDomicilioDS, string calleDS, string noDS, string nombreCFDS, string paisDDS, string estadoDDS, string municipioDDS, string temporalidadDS,*/
            string residenciaHabitualDS, string cpDS, string referenciasDS, string horarioDS, string observacionesDS,
            string estudia, string gradoEstudios, string institucionE, string horarioE, string direccionE, string telefonoE, string observacionesE,
            string trabaja, string tipoOcupacion, string puesto, string empleadorJefe, string enteradoProceso, string sePuedeEnterar, string tiempoTrabajando,
            string salario, string temporalidadSalario, string direccionT, string horarioT, string telefonoT, string observacionesT,
            string tipoActividad, string horarioAS, string lugarAS, string telefonoAS, string sePuedeEnterarAS, string referenciaAS, string observacionesAS,
            string vividoFuera, string lugaresVivido, string tiempoVivido, string motivoVivido, string viajaHabitual, string lugaresViaje, string tiempoViaje,
            string motivoViaje, string documentaciónSalirPais, string pasaporte, string visa, string familiaresFuera,
            string enfermedad, string especifiqueEnfermedad, string embarazoLactancia, string tiempoEmbarazo, string tratamiento, string discapacidad, string especifiqueDiscapacidad,
            string servicioMedico, string especifiqueServicioMedico, string institucionServicioMedico, string observacionesSalud, string capturista,
            IFormFile fotografia, string arraySustancias, string arrayFamiliarReferencia, string arrayDomSec, string arrayFamExtranjero)
        {

            string currentUser = User.Identity.Name;


            if (ModelState.ErrorCount <= 1)
            {
                #region -Persona-            
                persona.Nombre = removeSpaces(normaliza(nombre));
                persona.Paterno = removeSpaces(normaliza(paterno));
                persona.Materno = removeSpaces(normaliza(materno));
                persona.Alias = normaliza(alias);
                persona.Genero = normaliza(sexo);
                persona.Edad = edad;
                persona.Fnacimiento = fNacimiento;
                persona.Lnpais = lnPais;
                persona.Lnestado = lnEstado;
                persona.Lnmunicipio = lnMunicipio;
                persona.Lnlocalidad = normaliza(lnLocalidad);
                persona.EstadoCivil = estadoCivil;
                persona.Duracion = duracion;
                persona.OtroIdioma = normaliza(otroIdioma);
                persona.EspecifiqueIdioma = normaliza(especifiqueIdioma);
                persona.LeerEscribir = normaliza(leerEscribir);
                persona.Traductor = normaliza(traductor);
                persona.EspecifiqueTraductor = normaliza(especifiqueTraductor);
                persona.TelefonoFijo = telefonoFijo;
                persona.Celular = celular;
                persona.Hijos = normaliza(hijos);
                persona.Nhijos = nHijos;
                persona.NpersonasVive = nPersonasVive;
                persona.Propiedades = normaliza(propiedades);
                persona.Curp = normaliza(CURP);
                persona.ConsumoSustancias = normaliza(consumoSustancias);
                persona.Familiares = normaliza(familiares);
                persona.ReferenciasPersonales = normaliza(referenciasPersonales);
                persona.UbicacionExpediente = normaliza(ubicacionExpediente);
                persona.UltimaActualización = DateTime.Now;
                persona.Capturista = currentUser;
                persona.Candado = 0;
                persona.MotivoCandado = "NA";

                var estado = (from e in _context.Estados
                              where e.Id.ToString() == estadoD
                              select e.Estado).FirstOrDefault().ToString();
                var municipio = (from m in _context.Municipios
                                 where m.Id.ToString() == municipioD
                                 select m.Municipio).FirstOrDefault().ToString();
                persona.Colaboracion = "NO";
                if (persona.Capturista.EndsWith("\u0040dgepms.com") && estado == "Durango" && (municipio == "Gómez Palacio" || municipio == "Lerdo"))
                {
                    persona.Colaboracion = "SI";
                    persona.Supervisor = "janeth@nortedgepms.com";
                }
                if (persona.Capturista.EndsWith("\u0040nortedgepms.com") && estado == "Durango" && municipio == "Durango")
                {
                    persona.Colaboracion = "SI";
                    persona.Supervisor = "esmeralda.vargas@dgepms.com";
                }
                #endregion

                #region -Domicilio-
                domicilio.TipoDomicilio = tipoDomicilio;
                domicilio.Calle = normaliza(calle);
                domicilio.No = normaliza(no);
                domicilio.NombreCf = normaliza(nombreCF);
                domicilio.Pais = paisD;
                domicilio.Estado = estadoD;
                domicilio.Municipio = municipioD;
                domicilio.Temporalidad = temporalidad;
                domicilio.ResidenciaHabitual = normaliza(residenciaHabitual);
                domicilio.Cp = normaliza(cp);
                domicilio.Zona = zona;
                domicilio.Referencias = normaliza(referencias);
                domicilio.DomcilioSecundario = cuentaDomicilioSecundario;
                domicilio.Horario = normaliza(horario);
                domicilio.Observaciones = normaliza(observaciones);
                #endregion

                #region -Domicilio Secundario-   
                /*domiciliosecundario.Motivo = motivoDS;
                domiciliosecundario.TipoDomicilio = tipoDomicilioDS;
                domiciliosecundario.Calle = normaliza(calleDS);
                domiciliosecundario.No = normaliza(noDS);
                domiciliosecundario.NombreCf = normaliza(nombreCFDS);
                domiciliosecundario.Pais = paisDDS;
                domiciliosecundario.Estado = estadoDDS;
                domiciliosecundario.Municipio = municipioDDS;
                domiciliosecundario.Temporalidad = temporalidadDS;
                domiciliosecundario.ResidenciaHabitual = normaliza(residenciaHabitualDS);
                domiciliosecundario.Cp = normaliza(cpDS);
                domiciliosecundario.Referencias = normaliza(referenciasDS);
                domiciliosecundario.Horario = normaliza(horarioDS);
                domiciliosecundario.Observaciones = normaliza(observacionesDS);*/
                #endregion

                #region -Estudios-
                estudios.Estudia = estudia;
                estudios.GradoEstudios = gradoEstudios;
                estudios.InstitucionE = normaliza(institucionE);
                estudios.Horario = normaliza(horarioE);
                estudios.Direccion = normaliza(direccionE);
                estudios.Telefono = telefonoE;
                estudios.Observaciones = normaliza(observacionesE);
                #endregion

                #region -Trabajo-
                trabajo.Trabaja = trabaja;
                trabajo.TipoOcupacion = tipoOcupacion;
                trabajo.Puesto = normaliza(puesto);
                trabajo.EmpledorJefe = normaliza(empleadorJefe);
                trabajo.EnteradoProceso = enteradoProceso;
                trabajo.SePuedeEnterar = sePuedeEnterar;
                trabajo.TiempoTrabajano = tiempoTrabajando;
                trabajo.Salario = normaliza(salario);
                trabajo.Direccion = normaliza(direccionT);
                trabajo.Horario = normaliza(horarioT);
                trabajo.Telefono = telefonoT;
                trabajo.Observaciones = normaliza(observacionesT);
                #endregion

                #region -ActividadSocial-
                actividadsocial.TipoActividad = normaliza(tipoActividad);
                actividadsocial.Horario = normaliza(horarioAS);
                actividadsocial.Lugar = normaliza(lugarAS);
                actividadsocial.Telefono = telefonoAS;
                actividadsocial.SePuedeEnterar = sePuedeEnterarAS;
                actividadsocial.Referencia = normaliza(referenciaAS);
                actividadsocial.Observaciones = normaliza(observacionesAS);
                #endregion

                #region -AbandonoEstado-
                abandonoEstado.VividoFuera = vividoFuera;
                abandonoEstado.LugaresVivido = normaliza(lugaresVivido);
                abandonoEstado.TiempoVivido = normaliza(tiempoVivido);
                abandonoEstado.MotivoVivido = normaliza(motivoVivido);
                abandonoEstado.ViajaHabitual = viajaHabitual;
                abandonoEstado.LugaresViaje = normaliza(lugaresViaje);
                abandonoEstado.TiempoViaje = normaliza(tiempoViaje);
                abandonoEstado.MotivoViaje = normaliza(motivoViaje);
                abandonoEstado.DocumentacionSalirPais = documentaciónSalirPais;
                abandonoEstado.Pasaporte = pasaporte;
                abandonoEstado.Visa = visa;
                abandonoEstado.FamiliaresFuera = familiaresFuera;
                //abandonoEstado.Cuantos = cuantosFamiliares;
                #endregion

                #region -Salud-
                saludfisica.Enfermedad = enfermedad;
                saludfisica.EspecifiqueEnfermedad = normaliza(especifiqueEnfermedad);
                saludfisica.EmbarazoLactancia = embarazoLactancia;
                saludfisica.Tiempo = normaliza(tiempoEmbarazo);
                saludfisica.Tratamiento = normaliza(tratamiento);
                saludfisica.Discapacidad = discapacidad;
                saludfisica.EspecifiqueDiscapacidad = normaliza(especifiqueDiscapacidad);
                saludfisica.ServicioMedico = servicioMedico;
                saludfisica.EspecifiqueServicioMedico = especifiqueServicioMedico;
                saludfisica.InstitucionServicioMedico = institucionServicioMedico;
                saludfisica.Observaciones = normaliza(observacionesSalud);
                #endregion

                #region -IdDomicilio-  
                int idDomicilio = ((from table in _context.Domicilio
                                    select table.IdDomicilio).Max()) + 1;
                domicilio.IdDomicilio = idDomicilio;
                //domiciliosecundario.IdDomicilio = idDomicilio;
                #endregion

                #region -IdPersona-
                int idPersona = ((from table in _context.Persona
                                  select table.IdPersona).Max()) + 1;


                persona.IdPersona = idPersona;
                domicilio.PersonaIdPersona = idPersona;
                estudios.PersonaIdPersona = idPersona;
                trabajo.PersonaIdPersona = idPersona;
                actividadsocial.PersonaIdPersona = idPersona;
                abandonoEstado.PersonaIdPersona = idPersona;
                saludfisica.PersonaIdPersona = idPersona;

                #endregion

                #region -ConsumoSustancias-
                if (arraySustancias != null)
                {
                    JArray sustancias = JArray.Parse(arraySustancias);

                    for (int i = 0; i < sustancias.Count; i = i + 5)
                    {
                        consumosustanciasBD.Sustancia = sustancias[i].ToString();
                        consumosustanciasBD.Frecuencia = sustancias[i + 1].ToString();
                        consumosustanciasBD.Cantidad = normaliza(sustancias[i + 2].ToString());
                        consumosustanciasBD.UltimoConsumo = validateDatetime(sustancias[i + 3].ToString());
                        consumosustanciasBD.Observaciones = normaliza(sustancias[i + 4].ToString());
                        consumosustanciasBD.PersonaIdPersona = idPersona;
                        _context.Add(consumosustanciasBD);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                    }
                }
                /*for (int i = 0; i < datosSustancias.Count; i++)
                {
                    if (datosSustancias[i][1] == currentUser)
                    {
                        datosSustancias.RemoveAt(i);
                        i--;
                    }
                }*/
                #endregion

                #region -FamiliarReferencia-
                if (arrayFamiliarReferencia != null)
                {
                    JArray familiarReferencia = JArray.Parse(arrayFamiliarReferencia);
                    for (int i = 0; i < familiarReferencia.Count; i = i + 14)
                    {
                        asientoFamiliar.Nombre = normaliza(familiarReferencia[i].ToString());
                        asientoFamiliar.Relacion = familiarReferencia[i + 1].ToString();
                        try
                        {
                            asientoFamiliar.Edad = Int32.Parse(familiarReferencia[i + 2].ToString());
                        }
                        catch
                        {
                            asientoFamiliar.Edad = 0;
                        }
                        asientoFamiliar.Sexo = familiarReferencia[i + 3].ToString();
                        asientoFamiliar.Dependencia = familiarReferencia[i + 4].ToString();
                        asientoFamiliar.DependenciaExplica = normaliza(familiarReferencia[i + 5].ToString());
                        asientoFamiliar.VivenJuntos = familiarReferencia[i + 6].ToString();
                        asientoFamiliar.Domicilio = normaliza(familiarReferencia[i + 7].ToString());
                        asientoFamiliar.Telefono = familiarReferencia[i + 8].ToString();
                        asientoFamiliar.HorarioLocalizacion = normaliza(familiarReferencia[i + 9].ToString());
                        asientoFamiliar.EnteradoProceso = familiarReferencia[i + 10].ToString();
                        asientoFamiliar.PuedeEnterarse = familiarReferencia[i + 11].ToString();
                        asientoFamiliar.Observaciones = normaliza(familiarReferencia[i + 12].ToString());
                        asientoFamiliar.Tipo = familiarReferencia[i + 13].ToString();
                        asientoFamiliar.PersonaIdPersona = idPersona;
                        _context.Add(asientoFamiliar);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);

                    }
                }
                /*for (int i = 0; i < datosFamiliares.Count; i++)
                {
                    if (datosFamiliares[i][1] == currentUser)
                    {
                        datosFamiliares.RemoveAt(i);
                        i--;
                    }
                }*/
                #endregion

                #region -Referencias-
                /*for (int i = 0; i < datosReferencias.Count; i = i + 13)
                {
                    if (datosReferencias[i][1] == currentUser)
                    {
                        asientoFamiliar.Nombre = normaliza(datosReferencias[i][0]);
                        asientoFamiliar.Relacion = datosReferencias[i + 1][0];
                        try
                        {
                            asientoFamiliar.Edad = Int32.Parse(datosReferencias[i + 2][0]);
                        }
                        catch
                        {
                            asientoFamiliar.Edad = 0;
                        }
                        asientoFamiliar.Sexo = datosReferencias[i + 3][0];
                        asientoFamiliar.Dependencia = datosReferencias[i + 4][0];
                        asientoFamiliar.DependenciaExplica = normaliza(datosReferencias[i + 5][0]);
                        asientoFamiliar.VivenJuntos = datosReferencias[i + 6][0];
                        asientoFamiliar.Domicilio = normaliza(datosReferencias[i + 7][0]);
                        asientoFamiliar.Telefono = datosReferencias[i + 8][0];
                        asientoFamiliar.HorarioLocalizacion = normaliza(datosReferencias[i + 9][0]);
                        asientoFamiliar.EnteradoProceso = datosReferencias[i + 10][0];
                        asientoFamiliar.PuedeEnterarse = datosReferencias[i + 11][0];
                        asientoFamiliar.Observaciones = normaliza(datosReferencias[i + 12][0]);
                        asientoFamiliar.Tipo = "REFERENCIA";
                        asientoFamiliar.PersonaIdPersona = idPersona;
                        _context.Add(asientoFamiliar);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                    }
                }

                for (int i = 0; i < datosReferencias.Count; i++)
                {
                    if (datosReferencias[i][1] == currentUser)
                    {
                        datosReferencias.RemoveAt(i);
                        i--;
                    }
                }*/
                #endregion

                #region -Domicilio Secundario-
                if (arrayDomSec != null)
                {
                    JArray domSec = JArray.Parse(arrayDomSec);
                    for (int i = 0; i < domSec.Count; i = i + 14)
                    {
                        domiciliosecundario.Motivo = normaliza(domSec[i].ToString());
                        domiciliosecundario.TipoDomicilio = domSec[i + 1].ToString();
                        domiciliosecundario.Calle = normaliza(domSec[i + 2].ToString());
                        domiciliosecundario.No = normaliza(domSec[i + 3].ToString());
                        domiciliosecundario.NombreCf = normaliza(domSec[i + 4].ToString());
                        domiciliosecundario.Pais = domSec[i + 5].ToString();
                        domiciliosecundario.Estado = normaliza(domSec[i + 6].ToString());
                        domiciliosecundario.Municipio = domSec[i + 7].ToString();
                        domiciliosecundario.Temporalidad = domSec[i + 8].ToString();
                        domiciliosecundario.ResidenciaHabitual = domSec[i + 9].ToString();
                        domiciliosecundario.Cp = domSec[i + 10].ToString();
                        domiciliosecundario.Referencias = normaliza(domSec[i + 11].ToString());
                        domiciliosecundario.Horario = normaliza(domSec[i + 12].ToString());
                        domiciliosecundario.Observaciones = normaliza(domSec[i + 13].ToString());
                        domiciliosecundario.IdDomicilio = idDomicilio;
                        _context.Add(domiciliosecundario);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                    }
                }


                /*for (int i = 0; i < datosDomiciolioSecundario.Count; i++)
                {
                    if (datosDomiciolioSecundario[i][1] == currentUser)
                    {
                        datosDomiciolioSecundario.RemoveAt(i);
                        i--;
                    }
                }*/
                #endregion

                #region -Familiares Extranjero-
                if (arrayFamExtranjero != null)
                {
                    JArray famExtranjero = JArray.Parse(arrayFamExtranjero);
                    for (int i = 0; i < famExtranjero.Count; i = i + 12)
                    {
                        familiaresForaneos.Nombre = normaliza(famExtranjero[i].ToString());
                        familiaresForaneos.Relacion = famExtranjero[i + 1].ToString();
                        familiaresForaneos.Edad = Int32.Parse(famExtranjero[i + 2].ToString());
                        familiaresForaneos.Sexo = famExtranjero[i + 3].ToString();
                        familiaresForaneos.TiempoConocerlo = famExtranjero[i + 4].ToString();
                        familiaresForaneos.Pais = famExtranjero[i + 5].ToString();
                        familiaresForaneos.Estado = normaliza(famExtranjero[i + 6].ToString());
                        familiaresForaneos.Telefono = famExtranjero[i + 7].ToString();
                        familiaresForaneos.FrecuenciaContacto = famExtranjero[i + 8].ToString();
                        familiaresForaneos.EnteradoProceso = famExtranjero[i + 9].ToString();
                        familiaresForaneos.PuedeEnterarse = famExtranjero[i + 10].ToString();
                        familiaresForaneos.Observaciones = normaliza(famExtranjero[i + 11].ToString());
                        familiaresForaneos.PersonaIdPersona = idPersona;
                        _context.Add(familiaresForaneos);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                    }
                }
                /*for (int i = 0; i < datosFamiliaresExtranjero.Count; i++)
                {
                    if (datosFamiliaresExtranjero[i][1] == currentUser)
                    {
                        datosFamiliaresExtranjero.RemoveAt(i);
                        i--;
                    }
                }*/
                #endregion

                #region -Guardar Foto-
                if (fotografia != null)
                {
                    string file_name = persona.IdPersona + "_" + persona.Paterno + "_" + persona.Nombre + ".jpg";
                    file_name = replaceSlashes(file_name);
                    persona.rutaFoto = file_name;
                    var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Fotos");
                    var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                    await fotografia.CopyToAsync(stream);
                    stream.Close();
                }
                #endregion

                #region -Añadir a contexto-
                _context.Add(persona);
                _context.Add(domicilio);
                // _context.Add(domiciliosecundario);
                _context.Add(estudios);
                _context.Add(trabajo);
                _context.Add(actividadsocial);
                _context.Add(abandonoEstado);
                _context.Add(saludfisica);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                return RedirectToAction("RegistroConfirmation/" + persona.IdPersona, "Personas");
                #endregion
            }
            return RedirectToAction("ListadoSupervisor", "Personas");
        }
        #endregion

        #region -RegistroConfirmation-
        public async Task<IActionResult> RegistroConfirmation(int? id)
        {
            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.IdPersona == id);
            if (persona == null)
            {
                ViewBag.nombreRegistrado = null;
            }
            else
            {
                ViewBag.nombreRegistrado = persona.NombreCompleto;
                ViewBag.idRegistrado = persona.IdPersona;
            }
            return View();
        }
        #endregion       

        #endregion

        #region -Entrevista-
        public ActionResult Entrevista()
        {
            var personas = from p in _context.Persona
                           orderby p.Paterno
                           select p;

            ViewBag.personas = personas.ToList();

            idPersona = 0;

            return View();
        }

        [HttpPost, ActionName("Entrevista")]
        [ValidateAntiForgeryToken]
        public IActionResult EntrevistaPost()
        {
            if (ModelState.IsValid)
            {
                var persona = _context.Persona
                    .SingleOrDefault(m => m.IdPersona == idPersona);

                if (persona.Supervisor == null)
                {
                    return RedirectToAction("SinSupervisor");
                }
                else
                {
                    return RedirectToAction("MenuEdicion", "Personas", new { @id = idPersona });
                }
            }
            return View();
        }
        #endregion

        #region -Seleccionada-
        public ActionResult Seleccionada(string[] datosPersona)
        {
            idPersona = Int32.Parse(datosPersona[0]);
            return Json(new { success = true, responseText = "Persona seleccionada" });
        }
        #endregion

        #region -Reportes-
        public ActionResult ReportePersona()
        {
            return View();
        }

        public string normalizaDinero(string normalizar)
        {
            if (!String.IsNullOrEmpty(normalizar))
            {
                try
                {
                    normalizar = ((Convert.ToInt32(normalizar) / 100)).ToString("C", CultureInfo.CurrentCulture);
                }
                catch (System.FormatException e)
                {
                    return normalizar;
                }
            }
            else
            {
                normalizar = "S-D";
            }
            return normalizar;
        }

        #region -Crea QR-
        public void creaQR(int? id)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("10.6.60.190/Personas/Details/" + id, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            System.IO.FileStream fs = System.IO.File.Open(this._hostingEnvironment.WebRootPath + "\\images\\QR.jpg", FileMode.Create);
            qrCodeImage.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
            fs.Close();
        }
        #endregion

        public void Imprimir(int? id)
        {
            var persona = _context.Persona
               .SingleOrDefault(m => m.IdPersona == id);

            #region -To List databases-

            List<Persona> personaVM = _context.Persona.ToList();
            List<Domicilio> domicilioVM = _context.Domicilio.ToList();
            List<Estudios> estudiosVM = _context.Estudios.ToList();
            List<Estados> estados = _context.Estados.ToList();
            List<Municipios> municipios = _context.Municipios.ToList();
            List<Domiciliosecundario> domicilioSecundarioVM = _context.Domiciliosecundario.ToList();
            List<Consumosustancias> consumoSustanciasVM = _context.Consumosustancias.ToList();
            List<Trabajo> trabajoVM = _context.Trabajo.ToList();
            List<Actividadsocial> actividadSocialVM = _context.Actividadsocial.ToList();
            List<Abandonoestado> abandonoEstadoVM = _context.Abandonoestado.ToList();
            List<Saludfisica> saludFisicaVM = _context.Saludfisica.ToList();
            List<Familiaresforaneos> familiaresForaneosVM = _context.Familiaresforaneos.ToList();
            List<Asientofamiliar> asientoFamiliarVM = _context.Asientofamiliar.ToList();

            #endregion

            #region -Jointables-
            List<PersonaViewModel> vistaPersona = (from personaTable in personaVM
                                                   join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                                   join estudios in estudiosVM on persona.IdPersona equals estudios.PersonaIdPersona
                                                   join trabajo in trabajoVM on persona.IdPersona equals trabajo.PersonaIdPersona
                                                   join actividaSocial in actividadSocialVM on persona.IdPersona equals actividaSocial.PersonaIdPersona
                                                   join abandonoEstado in abandonoEstadoVM on persona.IdPersona equals abandonoEstado.PersonaIdPersona
                                                   join saludFisica in saludFisicaVM on persona.IdPersona equals saludFisica.PersonaIdPersona
                                                   join nacimientoEstado in estados on (Int32.Parse(persona.Lnestado)) equals nacimientoEstado.Id
                                                   join nacimientoMunicipio in municipios on (Int32.Parse(persona.Lnmunicipio)) equals nacimientoMunicipio.Id
                                                   join domicilioEstado in estados on (Int32.Parse(domicilio.Estado)) equals domicilioEstado.Id
                                                   join domicilioMunicipio in municipios on (Int32.Parse(domicilio.Municipio)) equals domicilioMunicipio.Id
                                                   where personaTable.IdPersona == id
                                                   select new PersonaViewModel
                                                   {
                                                       personaVM = personaTable,
                                                       domicilioVM = domicilio,
                                                       estudiosVM = estudios,
                                                       trabajoVM = trabajo,
                                                       actividadSocialVM = actividaSocial,
                                                       abandonoEstadoVM = abandonoEstado,
                                                       saludFisicaVM = saludFisica,
                                                       estadosVMPersona = nacimientoEstado,
                                                       municipiosVMPersona = nacimientoMunicipio,
                                                       estadosVMDomicilio = domicilioEstado,
                                                       municipiosVMDomicilio = domicilioMunicipio
                                                   }).ToList();
            #endregion

            creaQR(id);

            #region -GeneraDocumento-
            string templatePath = this._hostingEnvironment.WebRootPath + "\\Documentos\\templateEntrevista.docx";
            string resultPath = this._hostingEnvironment.WebRootPath + "\\Documentos\\entrevista.docx";
            string rutaFoto = ((vistaPersona[0].personaVM.Genero == ("M")) ? "hombre.png" : "mujer.png");
            if (vistaPersona[0].personaVM.rutaFoto != null)
            {
                rutaFoto = vistaPersona[0].personaVM.rutaFoto;
            }
            string picPath = this._hostingEnvironment.WebRootPath + "\\Fotos\\" + rutaFoto;

            DocumentCore dc = DocumentCore.Load(templatePath);

            var dataSource = new[] { new {
                nombre = vistaPersona[0].personaVM.Paterno+" "+ vistaPersona[0].personaVM.Materno +" "+ vistaPersona[0].personaVM.Nombre,
                genero = vistaPersona[0].personaVM.Genero,
                lnacimiento=vistaPersona[0].personaVM.Lnlocalidad+", "+ vistaPersona[0].municipiosVMPersona.Municipio +", "+ vistaPersona[0].estadosVMPersona.Estado+", "+vistaPersona[0].personaVM.Lnpais,
                fnacimiento=(Convert.ToDateTime(vistaPersona[0].personaVM.Fnacimiento)).ToString("dd MMMM yyyy"),
                edad=vistaPersona[0].personaVM.Edad,
                estadocivil=vistaPersona[0].personaVM.EstadoCivil,
                duracionestadocivil=vistaPersona[0].personaVM.Duracion,
                hablaidioma=vistaPersona[0].personaVM.OtroIdioma,
                especifiqueidioma=vistaPersona[0].personaVM.EspecifiqueIdioma,
                leerescribir=vistaPersona[0].personaVM.LeerEscribir,
                traductor=vistaPersona[0].personaVM.Traductor,
                especifiquetraductor=vistaPersona[0].personaVM.EspecifiqueTraductor,
                telefono=vistaPersona[0].personaVM.TelefonoFijo,
                celular=vistaPersona[0].personaVM.Celular,
                hijos=vistaPersona[0].personaVM.Hijos,
                cuantoshijos=vistaPersona[0].personaVM.Nhijos,
                personasvive=vistaPersona[0].personaVM.NpersonasVive,
                otraspropiedades=vistaPersona[0].personaVM.Propiedades,
                curp=vistaPersona[0].personaVM.Curp,
                consumosustancias=vistaPersona[0].personaVM.ConsumoSustancias,
                familiares=vistaPersona[0].personaVM.Familiares,
                referenciasPersonales=vistaPersona[0].personaVM.ReferenciasPersonales,
                tipopropiedad=vistaPersona[0].domicilioVM.TipoDomicilio,
                direccion=vistaPersona[0].domicilioVM.Calle+" "+vistaPersona[0].domicilioVM.No+", "+vistaPersona[0].domicilioVM.NombreCf+" CP "+vistaPersona[0].domicilioVM.Cp+", "+vistaPersona[0].estadosVMDomicilio.Estado+", "+vistaPersona[0].municipiosVMDomicilio.Municipio+", "+vistaPersona[0].domicilioVM.Pais,
                tiempoendomicilio=vistaPersona[0].domicilioVM.Temporalidad,
                residenciahabitual=vistaPersona[0].domicilioVM.ResidenciaHabitual,
                referenciasdomicilio=vistaPersona[0].domicilioVM.Referencias,
                horariodomicilio=vistaPersona[0].domicilioVM.Horario,
                observacionesdomicilio=vistaPersona[0].domicilioVM.Observaciones,
                domiciliosecundario=vistaPersona[0].domicilioVM.DomcilioSecundario,
                estudia=vistaPersona[0].estudiosVM.Estudia,
                gradoestudios=vistaPersona[0].estudiosVM.GradoEstudios,
                institucionestudios=vistaPersona[0].estudiosVM.InstitucionE,
                horarioescuela=vistaPersona[0].estudiosVM.Horario,
                direccionescuela=vistaPersona[0].estudiosVM.Direccion,
                telefonoescuela=vistaPersona[0].estudiosVM.Telefono,
                observacionesescolaridad=vistaPersona[0].estudiosVM.Observaciones,
                trabaja=vistaPersona[0].trabajoVM.Trabaja,
                tipoocupacion=vistaPersona[0].trabajoVM.TipoOcupacion,
                puesto=vistaPersona[0].trabajoVM.Puesto,
                empleador=vistaPersona[0].trabajoVM.EmpledorJefe,
                enteradoprocesotrabajo=vistaPersona[0].trabajoVM.EnteradoProceso,
                sepuedeenterartrabajo=vistaPersona[0].trabajoVM.SePuedeEnterar,
                tiempotrabajando=vistaPersona[0].trabajoVM.TiempoTrabajano,
                salario= normalizaDinero(vistaPersona[0].trabajoVM.Salario),
                temporalidadpago=vistaPersona[0].trabajoVM.TemporalidadSalario,
                direcciontrabajo=vistaPersona[0].trabajoVM.Direccion,
                horariotrabajo=vistaPersona[0].trabajoVM.Horario,
                telefonotrabajo=vistaPersona[0].trabajoVM.Telefono,
                observacionestrabajo=vistaPersona[0].trabajoVM.Observaciones,
                tipoactividad=vistaPersona[0].actividadSocialVM.TipoActividad,
                horarioactividad=vistaPersona[0].actividadSocialVM.Horario,
                lugaractividad=vistaPersona[0].actividadSocialVM.Lugar,
                telefonoactividad=vistaPersona[0].actividadSocialVM.Telefono,
                sepuedeenteraractividad=vistaPersona[0].actividadSocialVM.SePuedeEnterar,
                referenciaactividad=vistaPersona[0].actividadSocialVM.Referencia,
                observacionesactividad=vistaPersona[0].actividadSocialVM.Observaciones,
                vividofuera=vistaPersona[0].abandonoEstadoVM.VividoFuera,
                lugaresvivido=vistaPersona[0].abandonoEstadoVM.LugaresVivido,
                temporalidadviajes=vistaPersona[0].abandonoEstadoVM.TiempoVivido,
                motivovivido=vistaPersona[0].abandonoEstadoVM.MotivoVivido,
                viajahabitualmente=vistaPersona[0].abandonoEstadoVM.ViajaHabitual,
                lugaresviaje=vistaPersona[0].abandonoEstadoVM.LugaresViaje,
                tiempoviajes=vistaPersona[0].abandonoEstadoVM.TiempoViaje,
                motivoviajes=vistaPersona[0].abandonoEstadoVM.MotivoViaje,
                documentacion=vistaPersona[0].abandonoEstadoVM.DocumentacionSalirPais,
                pasaporte=vistaPersona[0].abandonoEstadoVM.Pasaporte,
                visa=vistaPersona[0].abandonoEstadoVM.Visa,
                familiaresfuera=vistaPersona[0].abandonoEstadoVM.FamiliaresFuera,
                enfermedades=vistaPersona[0].saludFisicaVM.Enfermedad,
                especenfermedad=vistaPersona[0].saludFisicaVM.EspecifiqueEnfermedad,
                tratamientomedico=vistaPersona[0].saludFisicaVM.Tratamiento,
                discapacidad=vistaPersona[0].saludFisicaVM.Discapacidad,
                especdiscapacidad=vistaPersona[0].saludFisicaVM.EspecifiqueDiscapacidad,
                serviciomedico=vistaPersona[0].saludFisicaVM.ServicioMedico,
                tiposervicio=vistaPersona[0].saludFisicaVM.EspecifiqueServicioMedico,
                institucionsalud=vistaPersona[0].saludFisicaVM.InstitucionServicioMedico,
                observacionessalud=vistaPersona[0].saludFisicaVM.Observaciones

            } };


            dc.MailMerge.FieldMerging += (sender, e) =>
            {
                if (e.FieldName == "foto")
                {
                    e.Inlines.Clear();
                    e.Inlines.Add(new Picture(dc, picPath) { Layout = new InlineLayout(new Size(100, 100)) });
                    e.Cancel = false;
                }
                if (e.FieldName == "QR")
                {
                    e.Inlines.Clear();
                    e.Inlines.Add(new Picture(dc, this._hostingEnvironment.WebRootPath + "\\images\\QR.jpg") { Layout = new InlineLayout(new Size(100, 100)) });
                    e.Cancel = false;
                }
            };

            dc.MailMerge.Execute(dataSource);


            dc.Save(resultPath);

            //Response.Redirect("https://localhost:44359/Documentos/entrevista.docx");
            Response.Redirect("http://10.6.60.190/Documentos/entrevista.docx");
            #endregion

        }
        #endregion

        #region -Procesos-
        public async Task<IActionResult> Procesos(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervisiones = await _context.Supervision.Where(m => m.PersonaIdPersona == id).ToListAsync();
            if (supervisiones.Count == 0)
            {
                return RedirectToAction("SinSupervision");
            }

            List<Supervision> SupervisionVM = _context.Supervision.ToList();
            List<Fraccionesimpuestas> fraccionesimpuestasVM = _context.Fraccionesimpuestas.ToList();
            List<Planeacionestrategica> planeacionestrategicasVM = _context.Planeacionestrategica.ToList();
            List<Causapenal> causaPenalVM = _context.Causapenal.ToList();
            List<Persona> personaVM = _context.Persona.ToList();


            #region -JointablesProcesos-

            var queryPro = (from s in _context.Supervision
                            join p in _context.Persona on s.PersonaIdPersona equals p.IdPersona
                            join c in _context.Causapenal on s.CausaPenalIdCausaPenal equals c.IdCausaPenal
                            join f in _context.Fraccionesimpuestas on s.IdSupervision equals f.SupervisionIdSupervision
                            join pe in _context.Planeacionestrategica on s.IdSupervision equals pe.SupervisionIdSupervision
                            where s.PersonaIdPersona == id
                            group c by c.IdCausaPenal into grup
                            select grup
                          );


            var q = queryPro.ToList();


            List<Procesos> lists = new List<Procesos>();


            for (int i = 0; i < q.Count; i++)
            {

                var querya = from c in _context.Causapenal
                             join s in _context.Supervision on c.IdCausaPenal equals s.CausaPenalIdCausaPenal
                             join p in _context.Persona on s.PersonaIdPersona equals p.IdPersona
                             join f in _context.Fraccionesimpuestas on s.IdSupervision equals f.SupervisionIdSupervision
                             join pe in _context.Planeacionestrategica on s.IdSupervision equals pe.SupervisionIdSupervision
                             where p.IdPersona == id && q[i].Key.Equals(c.IdCausaPenal)
                             select new Procesos
                             {
                                 supervisionVM = s,
                                 causapenalVM = c,
                                 personaVM = p,
                                 fraccionesimpuestasVM = f,
                                 planeacionestrategicaVM = pe
                             };

                var maxfra = querya.OrderByDescending(u => u.fraccionesimpuestasVM.IdFracciones).FirstOrDefault();
                lists.Add(maxfra);

            }



            ViewData["joinTbalasProceso1"] = lists;

            #endregion
            return View();
        }
        #endregion

        #region -Presentaciones periodicas-
        public async Task<IActionResult> PresentacionPeriodicaPersona(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<PresentacionPeriodicaPersona> lists = new List<PresentacionPeriodicaPersona>();

            var queripersonasis = from p in _context.Persona
                                  join rh in _context.Registrohuella on p.IdPersona equals rh.PersonaIdPersona
                                  join pp in _context.Presentacionperiodica on rh.IdregistroHuella equals pp.RegistroidHuella
                                  where p.IdPersona == id
                                  select new PresentacionPeriodicaPersona
                                  {
                                      presentacionperiodicaVM = pp,
                                      registrohuellaVM = rh,
                                      personaVM = p
                                  };
            var maxfra = queripersonasis.OrderByDescending(u => u.presentacionperiodicaVM.FechaFirma);

            if (queripersonasis.Count() == 0)
            {
                return RedirectToAction("PresentacionPeriodicaConfirmation/" + "Personas");
            }
            else
            {
                ViewData["joinTablasPresentacion"] = maxfra;
                return View();
            }
        }
        #endregion

        public async Task<IActionResult> PresentacionPeriodicaConfirmation()
        {
            return View();
        }
        #region -EditarComentario-
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComentario(Presentacionperiodica presentacionperiodica, Persona persona)
        {
            int idPresentacion = presentacionperiodica.IdpresentacionPeriodica;
            int idPersona = persona.IdPersona;
            var comentario = presentacionperiodica.ComentarioFirma;
            if (comentario == null)
            {
                comentario = "";
            }

            var comentarioUpdate = (from a in _context.Presentacionperiodica
                                    where a.IdpresentacionPeriodica == idPresentacion
                                    select a).FirstOrDefault();
            comentarioUpdate.ComentarioFirma = comentario.ToUpper();
            _context.SaveChanges();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PresentacionExists(presentacionperiodica.IdpresentacionPeriodica))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("PresentacionPeriodicaPersona/" + idPersona);
        }
        #endregion
        private bool PresentacionExists(int id)
        {
            return _context.Presentacionperiodica.Any(e => e.IdpresentacionPeriodica == id);
        }

        #region -SinSupervision-
        public ActionResult SinSupervision()
        {
            return View();
        }
        #endregion

        #region -SinSupervisor-
        public async Task<IActionResult> SinSupervisor()
        {
            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.IdPersona == idPersona);
            if (persona == null)
            {
                ViewBag.nombre = null;
                ViewBag.capturista = null;
            }
            else
            {
                ViewBag.nombre = persona.NombreCompleto;
                ViewBag.capturista = persona.Capturista;
            }
            return View();
        }
        #endregion

        #region -Edicion-        

        public async Task<IActionResult> MenuEdicion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.IdPersona == id);
            if (persona == null)
            {
                return NotFound();
            }

            string rutaFoto = ((persona.Genero == ("M")) ? "hombre.png" : "mujer.png");
            if (persona.rutaFoto != null)
            {
                rutaFoto = persona.rutaFoto;
            }
            ViewBag.rutaFoto = rutaFoto;

            return View(persona);
        }

        public async Task<IActionResult> EditFoto(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.IdPersona == id);
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFoto([Bind("IdPersona")] Persona persona, IFormFile fotoEditada)
        {
            if (ModelState.IsValid)
            {
                #region -Guardar Foto-
                var file_name = (from a in _context.Persona
                                 where a.IdPersona == persona.IdPersona
                                 select a.rutaFoto).FirstOrDefault();
                if (file_name == null || file_name == "S-D")
                {
                    var query = (from a in _context.Persona
                                 where a.IdPersona == persona.IdPersona
                                 select a).FirstOrDefault();
                    file_name = query.IdPersona + "_" + query.Paterno + "_" + query.Nombre + ".jpg";
                    query.rutaFoto = file_name;
                    try
                    {
                        var oldFoto = await _context.Persona.FindAsync(query.IdPersona);
                        _context.Entry(oldFoto).CurrentValues.SetValues(query);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                        //_context.Update(query);
                        //await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PersonaExists(query.IdPersona))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Fotos");
                var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create, FileAccess.ReadWrite);
                #endregion

                fotoEditada.CopyTo(stream);
                stream.Close();
                return RedirectToAction("MenuEdicion/" + persona.IdPersona, "Personas");
            }
            return View(persona);
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

        #region -Edita Datos Generales-
        // GET: Personas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.IdPersona == id);
            consumosustancias = await _context.Consumosustancias.Where(m => m.PersonaIdPersona == id).ToListAsync();
            familiares = await _context.Asientofamiliar.Where(m => m.PersonaIdPersona == id && m.Tipo == "FAMILIAR").ToListAsync();
            referenciaspersonales = await _context.Asientofamiliar.Where(m => m.PersonaIdPersona == id && m.Tipo == "REFERENCIA").ToListAsync();
            if (persona == null)
            {
                return NotFound();
            }

            #region PAIS          
            List<SelectListItem> ListaPais;
            ListaPais = new List<SelectListItem>
            {
              new SelectListItem{ Text="México", Value="MEXICO"},
              new SelectListItem{ Text="Estados Unidos", Value="ESTADOS UNIDOS"},
              new SelectListItem{ Text="Canada", Value="CANADA"},
              new SelectListItem{ Text="Colombia", Value="COLOMBIA"},
              new SelectListItem{ Text="El Salvador", Value="EL SALVADOR"},
              new SelectListItem{ Text="Guatemala", Value="GUATEMALA"},
              new SelectListItem{ Text="Chile", Value="CHILE"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.listaLnpais = ListaPais;

            foreach (var item in ListaPais)
            {
                if (item.Value == persona.Lnpais)
                {
                    ViewBag.idLnpais = item.Value;
                    break;
                }
            }
            #endregion

            #region Lnestado
            List<Estados> listaEstados = new List<Estados>();
            listaEstados = (from table in _context.Estados
                            select table).ToList();

            listaEstados.Insert(0, new Estados { Id = 0, Estado = "Selecciona" });

            ViewBag.ListadoEstados = listaEstados;

            ViewBag.idEstado = persona.Lnestado;
            #endregion

            #region Lnmunicipio
            int Lnestado;
            bool success = Int32.TryParse(persona.Lnestado, out Lnestado);
            List<Municipios> listaMunicipios = new List<Municipios>();
            if (success)
            {
                listaMunicipios = (from table in _context.Municipios
                                   where table.EstadosId == Lnestado
                                   select table).ToList();
            }

            listaMunicipios.Insert(0, new Municipios { Id = 0, Municipio = "Selecciona" });

            ViewBag.ListadoMunicipios = listaMunicipios;
            ViewBag.idMunicipio = persona.Lnmunicipio;
            #endregion

            #region EstadoCivil
            List<SelectListItem> ListaEstadoCivil;
            ListaEstadoCivil = new List<SelectListItem>
            {
              new SelectListItem{ Text="Soltero (a)", Value="SOLTERO (A)"},
              new SelectListItem{ Text="Casado (a)", Value="CASADO (A)"},
              new SelectListItem{ Text="Union libre", Value="UNION LIBRE"},
              new SelectListItem{ Text="Viudo (a)", Value="VIUDO (A)"},
              new SelectListItem{ Text="Divorciado (a)", Value="DIVORCIADO (A)"}
            };

            ViewBag.listaEstadoCivil = ListaEstadoCivil;
            ViewBag.idEstadoCivil = BuscaId(ListaEstadoCivil, persona.EstadoCivil);
            #endregion

            #region GENERO          
            List<SelectListItem> ListaGenero;
            ListaGenero = new List<SelectListItem>
            {
              new SelectListItem{ Text="Masculino", Value="M"},
              new SelectListItem{ Text="Femenino", Value="F"},
              new SelectListItem{ Text="Prefiero no decirlo", Value="N"},
            };

            ViewBag.listaGenero = ListaGenero;
            ViewBag.idGenero = BuscaId(ListaGenero, persona.Genero);

            #endregion

            ViewBag.listaOtroIdioma = listaNoSi;
            ViewBag.idOtroIdioma = BuscaId(listaNoSi, persona.OtroIdioma);

            ViewBag.listaLeerEscribir = listaSiNo;
            ViewBag.idLeerEscribir = BuscaId(listaSiNo, persona.LeerEscribir);

            ViewBag.listaTraductor = listaNoSi;
            ViewBag.idTraductor = BuscaId(listaNoSi, persona.Traductor);

            ViewBag.listaHijos = listaNoSi;
            ViewBag.idHijos = BuscaId(listaNoSi, persona.Hijos);

            ViewBag.listaPropiedades = listaNoSi;
            ViewBag.idPropiedades = BuscaId(listaNoSi, persona.Propiedades);

            //ViewBag.listaUbicacionExp = listaUbicacionExpediente;
            //ViewBag.idUbicacionExp = BuscaId(listaUbicacionExpediente, persona.UbicacionExpediente);

            ViewBag.pais = persona.Lnpais;
            ViewBag.idioma = persona.OtroIdioma;
            ViewBag.traductor = persona.Traductor;
            ViewBag.Hijos = persona.Hijos;

            #region Consume sustancias
            ViewBag.listaConsumoSustancias = listaNoSi;
            ViewBag.ConsumoSustancias = BuscaId(listaNoSi, persona.ConsumoSustancias);

            contadorSustancia = 0;

            List<SelectListItem> ListaSustancia;
            ListaSustancia = new List<SelectListItem>
            {
                new SelectListItem{ Text="Alcohol", Value="ALCOHOL"},
                new SelectListItem{ Text="Marihuana", Value="MARIHUANA"},
                new SelectListItem{ Text="Cocaína", Value="COCAINA"},
                new SelectListItem{ Text="Heroína", Value="HEROINA"},
                new SelectListItem{ Text="PVC", Value="PVC"},
                new SelectListItem{ Text="Solventes", Value="SOLVENTES"},
                new SelectListItem{ Text="Fármacos", Value="FARMACOS"},
                new SelectListItem{ Text="Cemento", Value="CEMENTO"},
                new SelectListItem{ Text="Crack", Value="CRACK"},
                new SelectListItem{ Text="Ácidos", Value="ACIDOS"},
                new SelectListItem{ Text="Tabaco", Value="TABACO"},
                new SelectListItem{ Text="Metanfetaminas", Value="METANFETAMINAS"},
                new SelectListItem{ Text="Otro", Value="OTRO"},
            };
            ViewBag.listaSustancia = ListaSustancia;

            List<SelectListItem> ListaFrecuencia;
            ListaFrecuencia = new List<SelectListItem>
            {
                new SelectListItem{ Text="Diario", Value="DIARIO"},
                new SelectListItem{ Text="Semanal", Value="SEMANAL"},
                new SelectListItem{ Text="Quincenal", Value="QUINCENAL"},
                new SelectListItem{ Text="Mensual", Value="MENSUAL"},
                new SelectListItem{ Text="Bimestral", Value="BIMESTRAL"},
                new SelectListItem{ Text="Trimestral", Value="TRIMESTRAL"},
                new SelectListItem{ Text="Semestral", Value="SEMESTRAL"},
                new SelectListItem{ Text="Anual", Value="ANUAL"},
                new SelectListItem{ Text="Ocasionalmente", Value="OCASIONALMENTE"},
            };
            ViewBag.listaFrecuencia = ListaFrecuencia;








            if (consumosustancias.Count > 0)
            {
                ViewBag.idSustancia = BuscaId(ListaSustancia, consumosustancias[contadorSustancia].Sustancia);
                ViewBag.idFrecuencia = BuscaId(ListaFrecuencia, consumosustancias[contadorSustancia].Frecuencia);
                ViewBag.cantidad = consumosustancias[contadorSustancia].Cantidad;
                ViewBag.ultimoConsumo = consumosustancias[contadorSustancia].UltimoConsumo;
                ViewBag.observaciones = consumosustancias[contadorSustancia].Observaciones;
                ViewBag.idConsumoSustancias = consumosustancias[contadorSustancia].IdConsumoSustancias;
                contadorSustancia++;
            }
            else
            {
                ViewBag.idSustancia = "ALCOHOL";
                ViewBag.idFrecuencia = "DIARIO";
                ViewBag.cantidad = null;
                ViewBag.ultimoConsumo = null;
                ViewBag.observaciones = null;
            }
            #endregion

            #region Familiares
            ViewBag.listaFamiliares = listaSiNo;
            ViewBag.idFamiliares = BuscaId(listaSiNo, persona.Familiares);

            contadorFamiliares = 0;

            List<SelectListItem> ListaRelacion;
            ListaRelacion = new List<SelectListItem>
            {
                new SelectListItem{ Text="Máma", Value="MAMA"},
                new SelectListItem{ Text="Pápa", Value="PAPA"},
                new SelectListItem{ Text="Esposo(a)", Value="ESPOSO (A)"},
                new SelectListItem{ Text="Hermano(a)", Value="HERMANO (A)"},
                new SelectListItem{ Text="Hijo(a)", Value="HIJO (A)"},
                new SelectListItem{ Text="Abuelo(a)", Value="ABUELO (A)"},
                new SelectListItem{ Text="Familiar 1 nivel", Value="FAMILIAR 1 NIVEL"},
                new SelectListItem{ Text="Amigo", Value="AMIGO"},
                new SelectListItem{ Text="Conocido", Value="CONOCIDO"},
                new SelectListItem{ Text="Otro", Value="OTRO"},
            };
            ViewBag.listaRelacion = ListaRelacion;

            List<SelectListItem> ListaSexo;
            ListaSexo = new List<SelectListItem>
            {
                new SelectListItem{ Text="Masculino", Value="M"},
                new SelectListItem{ Text="Femenino", Value="F"},
                new SelectListItem{ Text="Prefiero no decirlo", Value="N"},
            };
            ViewBag.listaSexo = ListaSexo;

            ViewBag.listaDependencia = listaNoSi;
            ViewBag.listaVivenJuntos = listaSiNo;
            ViewBag.listaEnteradoProceso = listaSiNo;
            ViewBag.listaPuedeEnterarse = listaNoSiNA;

            if (familiares.Count > 0)
            {
                ViewBag.nombreF = familiares[contadorFamiliares].Nombre;
                ViewBag.idRelacionF = BuscaId(ListaRelacion, familiares[contadorFamiliares].Relacion);
                ViewBag.edadF = familiares[contadorFamiliares].Edad;
                ViewBag.idSexoF = BuscaId(ListaSexo, familiares[contadorFamiliares].Sexo); ;
                ViewBag.idDependenciaF = BuscaId(listaNoSi, familiares[contadorFamiliares].Dependencia);
                ViewBag.dependenciaExplicaF = familiares[contadorFamiliares].DependenciaExplica;
                ViewBag.idVivenJuntosF = BuscaId(listaSiNo, familiares[contadorFamiliares].VivenJuntos);
                ViewBag.domicilioF = familiares[contadorFamiliares].Domicilio;
                ViewBag.telefonoF = familiares[contadorFamiliares].Telefono;
                ViewBag.horarioLocalizacionF = familiares[contadorFamiliares].HorarioLocalizacion;
                ViewBag.idEnteradoProcesoF = BuscaId(listaSiNo, familiares[contadorFamiliares].EnteradoProceso);
                ViewBag.idPuedeEnterarseF = BuscaId(listaNoSiNA, familiares[contadorFamiliares].PuedeEnterarse);
                ViewBag.AFobservacionesF = familiares[contadorFamiliares].Observaciones;
                ViewBag.idAsientoFamiliarF = familiares[contadorFamiliares].IdAsientoFamiliar;
                contadorFamiliares++;
            }
            else
            {
                ViewBag.nombreF = null;
                ViewBag.idRelacionF = "MAMA";
                ViewBag.edadF = 0;
                ViewBag.idSexoF = "M";
                ViewBag.idDependenciaF = "NO";
                ViewBag.dependenciaExplicaF = null;
                ViewBag.idVivenJuntosF = "SI";
                ViewBag.domicilioF = null;
                ViewBag.telefonoF = null;
                ViewBag.horarioLocalizacionF = null;
                ViewBag.idEnteradoProcesoF = "SI";
                ViewBag.idPuedeEnterarseF = "NA";
                ViewBag.AFobservacionesF = null;
            }
            #endregion

            #region Referencias
            ViewBag.listaReferenciasPersonales = listaSiNo;
            ViewBag.idReferenciasPersonales = BuscaId(listaSiNo, persona.ReferenciasPersonales);

            contadorReferencias = 0;

            if (referenciaspersonales.Count > 0)
            {
                ViewBag.nombreR = referenciaspersonales[contadorReferencias].Nombre;
                ViewBag.idRelacionR = BuscaId(ListaRelacion, referenciaspersonales[contadorReferencias].Relacion);
                ViewBag.edadR = referenciaspersonales[contadorReferencias].Edad;
                ViewBag.idSexoR = BuscaId(ListaSexo, referenciaspersonales[contadorReferencias].Sexo); ;
                ViewBag.idDependenciaR = BuscaId(listaNoSi, referenciaspersonales[contadorReferencias].Dependencia);
                ViewBag.dependenciaExplicaR = referenciaspersonales[contadorReferencias].DependenciaExplica;
                ViewBag.idVivenJuntosR = BuscaId(listaSiNo, referenciaspersonales[contadorReferencias].VivenJuntos);
                ViewBag.domicilioR = referenciaspersonales[contadorReferencias].Domicilio;
                ViewBag.telefonoR = referenciaspersonales[contadorReferencias].Telefono;
                ViewBag.horarioLocalizacionR = referenciaspersonales[contadorReferencias].HorarioLocalizacion;
                ViewBag.idEnteradoProcesoR = BuscaId(listaSiNo, referenciaspersonales[contadorReferencias].EnteradoProceso);
                ViewBag.idPuedeEnterarseR = BuscaId(listaNoSiNA, referenciaspersonales[contadorReferencias].PuedeEnterarse);
                ViewBag.AFobservacionesR = referenciaspersonales[contadorReferencias].Observaciones;
                ViewBag.idAsientoFamiliarR = referenciaspersonales[contadorReferencias].IdAsientoFamiliar;
                contadorReferencias++;
            }
            else
            {
                ViewBag.nombreR = null;
                ViewBag.idRelacionR = "MAMA";
                ViewBag.edadR = 0;
                ViewBag.idSexoR = "M";
                ViewBag.idDependenciaR = "NO";
                ViewBag.dependenciaExplicaR = null;
                ViewBag.idVivenJuntosR = "SI";
                ViewBag.domicilioR = null;
                ViewBag.telefonoR = null;
                ViewBag.horarioLocalizacionR = null;
                ViewBag.idEnteradoProcesoR = "SI";
                ViewBag.idPuedeEnterarseR = "NA";
                ViewBag.AFobservacionesR = null;
            }
            #endregion

            return View(persona);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPersona,Nombre,Paterno,Materno,Alias,Genero,Edad,Fnacimiento,Lnpais,Lnestado,Lnmunicipio,Lnlocalidad,EstadoCivil,Duracion,OtroIdioma,EspecifiqueIdioma,DatosGeneralescol,LeerEscribir,Traductor,EspecifiqueTraductor,TelefonoFijo,Celular,Hijos,Nhijos,NpersonasVive,Propiedades,Curp,ConsumoSustancias,Familiares,ReferenciasPersonales,UltimaActualización,Supervisor,rutaFoto,Capturista,Candado, UbicacionExpediente")] Persona persona)
        {
            string currentUser = User.Identity.Name;

            if (id != persona.IdPersona)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                persona.Paterno = removeSpaces(normaliza(persona.Paterno));
                persona.Materno = removeSpaces(normaliza(persona.Materno));
                persona.Nombre = removeSpaces(normaliza(persona.Nombre));
                persona.Alias = normaliza(persona.Alias);
                persona.Lnlocalidad = normaliza(persona.Lnlocalidad);
                persona.Duracion = normaliza(persona.Duracion);
                persona.DatosGeneralescol = normaliza(persona.DatosGeneralescol);
                persona.EspecifiqueIdioma = normaliza(persona.EspecifiqueIdioma);
                persona.EspecifiqueTraductor = normaliza(persona.EspecifiqueTraductor);
                persona.Curp = normaliza(persona.Curp);
                persona.ConsumoSustancias = normaliza(persona.ConsumoSustancias);
                persona.Familiares = normaliza(persona.Familiares);
                persona.ReferenciasPersonales = normaliza(persona.ReferenciasPersonales);
                persona.rutaFoto = normaliza(persona.rutaFoto);
                persona.Capturista = persona.Capturista;
                persona.UbicacionExpediente = normaliza(persona.UbicacionExpediente);
                if (persona.Candado == null) { persona.Candado = 0; }
                persona.Candado = persona.Candado;
                #region -ConsumoSustancias-
                //Sustancias editadas
                for (int i = 0; i < datosSustanciasEditadas.Count; i = i + 6)
                {
                    if (datosSustanciasEditadas[i][1] == currentUser)
                    {
                        Consumosustancias consumosustanciasBD = new Consumosustancias();

                        consumosustanciasBD.Sustancia = datosSustanciasEditadas[i][0];
                        consumosustanciasBD.Frecuencia = datosSustanciasEditadas[i + 1][0];
                        consumosustanciasBD.Cantidad = normaliza(datosSustanciasEditadas[i + 2][0]);
                        consumosustanciasBD.UltimoConsumo = validateDatetime(datosSustanciasEditadas[i + 3][0]);
                        consumosustanciasBD.Observaciones = normaliza(datosSustanciasEditadas[i + 4][0]);
                        consumosustanciasBD.IdConsumoSustancias = Int32.Parse(datosSustanciasEditadas[i + 5][0]);
                        consumosustanciasBD.PersonaIdPersona = id;

                        try
                        {
                            var oldconsumosustanciasBD = await _context.Consumosustancias.FindAsync(consumosustanciasBD.IdConsumoSustancias);
                            _context.Entry(oldconsumosustanciasBD).CurrentValues.SetValues(consumosustanciasBD);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                            //_context.Update(consumosustanciasBD);
                            //await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!PersonaExists(consumosustanciasBD.PersonaIdPersona))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }


                for (int i = 0; i < datosSustanciasEditadas.Count; i++)
                {
                    if (datosSustanciasEditadas[i][1] == currentUser)
                    {
                        datosSustanciasEditadas.RemoveAt(i);
                        i--;
                    }
                }

                //Sustancias agregadas
                int idConsumoSustancias = ((from table in _context.Consumosustancias
                                            select table.IdConsumoSustancias).Max());
                for (int i = 0; i < datosSustancias.Count; i = i + 5)
                {
                    if (datosSustancias[i][1] == currentUser)
                    {
                        Consumosustancias consumosustanciasBD = new Consumosustancias();

                        consumosustanciasBD.Sustancia = datosSustancias[i][0];
                        consumosustanciasBD.Frecuencia = datosSustancias[i + 1][0];
                        consumosustanciasBD.Cantidad = normaliza(datosSustancias[i + 2][0]);
                        consumosustanciasBD.UltimoConsumo = validateDatetime(datosSustancias[i + 3][0]);
                        consumosustanciasBD.Observaciones = normaliza(datosSustancias[i + 4][0]);
                        consumosustanciasBD.PersonaIdPersona = id;
                        consumosustanciasBD.IdConsumoSustancias = ++idConsumoSustancias;
                        _context.Add(consumosustanciasBD);
                        await _context.SaveChangesAsync(null, 1);
                    }
                }

                for (int i = 0; i < datosSustancias.Count; i++)
                {
                    if (datosSustancias[i][1] == currentUser)
                    {
                        datosSustancias.RemoveAt(i);
                        i--;
                    }
                }
                #endregion

                #region -Familiares-
                int idAsientoFamiliar = ((from table in _context.Asientofamiliar
                                          select table.IdAsientoFamiliar).Max());
                //Familiares editados
                for (int i = 0; i < datosFamiliaresEditados.Count; i = i + 14)
                {
                    if (datosFamiliaresEditados[i][1] == currentUser)
                    {
                        Asientofamiliar asientoFamiliar = new Asientofamiliar();

                        asientoFamiliar.Nombre = normaliza(datosFamiliaresEditados[i][0]);
                        asientoFamiliar.Relacion = datosFamiliaresEditados[i + 1][0];
                        asientoFamiliar.Edad = Int32.Parse(datosFamiliaresEditados[i + 2][0]);
                        asientoFamiliar.Sexo = datosFamiliaresEditados[i + 3][0];
                        asientoFamiliar.Dependencia = datosFamiliaresEditados[i + 4][0];
                        asientoFamiliar.DependenciaExplica = normaliza(datosFamiliaresEditados[i + 5][0]);
                        asientoFamiliar.VivenJuntos = datosFamiliaresEditados[i + 6][0];
                        asientoFamiliar.Domicilio = normaliza(datosFamiliaresEditados[i + 7][0]);
                        asientoFamiliar.Telefono = datosFamiliaresEditados[i + 8][0];
                        asientoFamiliar.HorarioLocalizacion = normaliza(datosFamiliaresEditados[i + 9][0]);
                        asientoFamiliar.EnteradoProceso = datosFamiliaresEditados[i + 10][0];
                        asientoFamiliar.PuedeEnterarse = datosFamiliaresEditados[i + 11][0];
                        asientoFamiliar.Observaciones = normaliza(datosFamiliaresEditados[i + 12][0]);
                        asientoFamiliar.IdAsientoFamiliar = Int32.Parse(datosFamiliaresEditados[i + 13][0]);
                        asientoFamiliar.Tipo = "FAMILIAR";
                        asientoFamiliar.PersonaIdPersona = id;

                        try
                        {
                            var oldAsientofamiliar = await _context.Asientofamiliar.FindAsync(asientoFamiliar.IdAsientoFamiliar);
                            _context.Entry(oldAsientofamiliar).CurrentValues.SetValues(asientoFamiliar);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                            //_context.Update(asientoFamiliar);
                            //await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!PersonaExists(asientoFamiliar.PersonaIdPersona))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }


                for (int i = 0; i < datosFamiliaresEditados.Count; i++)
                {
                    if (datosFamiliaresEditados[i][1] == currentUser)
                    {
                        datosFamiliaresEditados.RemoveAt(i);
                        i--;
                    }
                }

                //Familiares agregados
                for (int i = 0; i < datosFamiliares.Count; i = i + 13)
                {
                    if (datosFamiliares[i][1] == currentUser)
                    {
                        Asientofamiliar asientoFamiliar = new Asientofamiliar();

                        asientoFamiliar.Nombre = normaliza(datosFamiliares[i][0]);
                        asientoFamiliar.Relacion = datosFamiliares[i + 1][0];
                        asientoFamiliar.Edad = Int32.Parse(datosFamiliares[i + 2][0]);
                        asientoFamiliar.Sexo = datosFamiliares[i + 3][0];
                        asientoFamiliar.Dependencia = datosFamiliares[i + 4][0];
                        asientoFamiliar.DependenciaExplica = normaliza(datosFamiliares[i + 5][0]);
                        asientoFamiliar.VivenJuntos = datosFamiliares[i + 6][0];
                        asientoFamiliar.Domicilio = normaliza(datosFamiliares[i + 7][0]);
                        asientoFamiliar.Telefono = datosFamiliares[i + 8][0];
                        asientoFamiliar.HorarioLocalizacion = normaliza(datosFamiliares[i + 9][0]);
                        asientoFamiliar.EnteradoProceso = datosFamiliares[i + 10][0];
                        asientoFamiliar.PuedeEnterarse = datosFamiliares[i + 11][0];
                        asientoFamiliar.Observaciones = normaliza(datosFamiliares[i + 12][0]);
                        asientoFamiliar.Tipo = "FAMILIAR";
                        asientoFamiliar.PersonaIdPersona = id;
                        asientoFamiliar.IdAsientoFamiliar = ++idAsientoFamiliar;
                        _context.Add(asientoFamiliar);
                        await _context.SaveChangesAsync(null, 1);
                    }
                }

                for (int i = 0; i < datosFamiliares.Count; i++)
                {
                    if (datosFamiliares[i][1] == currentUser)
                    {
                        datosFamiliares.RemoveAt(i);
                        i--;
                    }
                }
                #endregion

                #region -Referencias-
                //Referencias editadas
                for (int i = 0; i < datosReferenciasEditadas.Count; i = i + 14)
                {
                    if (datosReferenciasEditadas[i][1] == currentUser)
                    {
                        Asientofamiliar asientoFamiliar = new Asientofamiliar();

                        asientoFamiliar.Nombre = normaliza(datosReferenciasEditadas[i][0]);
                        asientoFamiliar.Relacion = datosReferenciasEditadas[i + 1][0];
                        asientoFamiliar.Edad = Int32.Parse(datosReferenciasEditadas[i + 2][0]);
                        asientoFamiliar.Sexo = datosReferenciasEditadas[i + 3][0];
                        asientoFamiliar.Dependencia = datosReferenciasEditadas[i + 4][0];
                        asientoFamiliar.DependenciaExplica = normaliza(datosReferenciasEditadas[i + 5][0]);
                        asientoFamiliar.VivenJuntos = datosReferenciasEditadas[i + 6][0];
                        asientoFamiliar.Domicilio = normaliza(datosReferenciasEditadas[i + 7][0]);
                        asientoFamiliar.Telefono = datosReferenciasEditadas[i + 8][0];
                        asientoFamiliar.HorarioLocalizacion = normaliza(datosReferenciasEditadas[i + 9][0]);
                        asientoFamiliar.EnteradoProceso = datosReferenciasEditadas[i + 10][0];
                        asientoFamiliar.PuedeEnterarse = datosReferenciasEditadas[i + 11][0];
                        asientoFamiliar.Observaciones = normaliza(datosReferenciasEditadas[i + 12][0]);
                        asientoFamiliar.IdAsientoFamiliar = Int32.Parse(datosReferenciasEditadas[i + 13][0]);
                        asientoFamiliar.Tipo = "REFERENCIA";
                        asientoFamiliar.PersonaIdPersona = id;

                        try
                        {
                            var oldAsientofamiliar = await _context.Asientofamiliar.FindAsync(asientoFamiliar.IdAsientoFamiliar);
                            _context.Entry(oldAsientofamiliar).CurrentValues.SetValues(asientoFamiliar);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                            //_context.Update(asientoFamiliar);
                            //await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!PersonaExists(asientoFamiliar.PersonaIdPersona))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }


                for (int i = 0; i < datosReferenciasEditadas.Count; i++)
                {
                    if (datosReferenciasEditadas[i][1] == currentUser)
                    {
                        datosReferenciasEditadas.RemoveAt(i);
                        i--;
                    }
                }

                //Referencias agregadas
                for (int i = 0; i < datosReferencias.Count; i = i + 13)
                {
                    if (datosReferencias[i][1] == currentUser)
                    {
                        Asientofamiliar asientoFamiliar = new Asientofamiliar();

                        asientoFamiliar.Nombre = normaliza(datosReferencias[i][0]);
                        asientoFamiliar.Relacion = datosReferencias[i + 1][0];
                        asientoFamiliar.Edad = Int32.Parse(datosReferencias[i + 2][0]);
                        asientoFamiliar.Sexo = datosReferencias[i + 3][0];
                        asientoFamiliar.Dependencia = datosReferencias[i + 4][0];
                        asientoFamiliar.DependenciaExplica = normaliza(datosReferencias[i + 5][0]);
                        asientoFamiliar.VivenJuntos = datosReferencias[i + 6][0];
                        asientoFamiliar.Domicilio = normaliza(datosReferencias[i + 7][0]);
                        asientoFamiliar.Telefono = datosReferencias[i + 8][0];
                        asientoFamiliar.HorarioLocalizacion = normaliza(datosReferencias[i + 9][0]);
                        asientoFamiliar.EnteradoProceso = datosReferencias[i + 10][0];
                        asientoFamiliar.PuedeEnterarse = datosReferencias[i + 11][0];
                        asientoFamiliar.Observaciones = normaliza(datosReferencias[i + 12][0]);
                        asientoFamiliar.Tipo = "REFERENCIA";
                        asientoFamiliar.PersonaIdPersona = id;
                        asientoFamiliar.IdAsientoFamiliar = ++idAsientoFamiliar;
                        _context.Add(asientoFamiliar);
                        await _context.SaveChangesAsync(null, 1);
                    }
                }

                for (int i = 0; i < datosReferencias.Count; i++)
                {
                    if (datosReferencias[i][1] == currentUser)
                    {
                        datosReferencias.RemoveAt(i);
                        i--;
                    }
                }
                #endregion

                try
                {
                    var oldPersona = await _context.Persona.FindAsync(id);
                    _context.Entry(oldPersona).CurrentValues.SetValues(persona);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(persona);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(persona.IdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MenuEdicion/" + persona.IdPersona, "Personas");
            }
            return View(persona);
        }

        public async Task<IActionResult> actualizarUbicacion(string ubicacion, int idPersona)
        {
            var persona = (from a in _context.Persona
                           where a.IdPersona == idPersona
                           select a).FirstOrDefault();
            persona.UbicacionExpediente = ubicacion;
            var oldPersona = await _context.Persona.FindAsync(idPersona);
            _context.Entry(oldPersona).CurrentValues.SetValues(persona);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            return Json(new { success = true });
        }
        #endregion

        #region -Edita Domicilio-
        public async Task<IActionResult> EditDomicilio(string nombre, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Nombre"] = nombre;
            var domicilio = await _context.Domicilio.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);

            #region TIPODOMICILIO          
            List<SelectListItem> LiatatDomicilio;
            LiatatDomicilio = new List<SelectListItem>
            {
              new SelectListItem{ Text="Rentada", Value="RENTADA"},
              new SelectListItem{ Text="Prestada", Value="PRESTADA"},
              new SelectListItem{ Text="Propia", Value="PROPIA"},
              new SelectListItem{ Text="Familiar", Value="FAMILIAR"},
              new SelectListItem{ Text="Situación de calle", Value="SITUACION DE CALLE"},
              new SelectListItem{ Text="Irregular", Value="IRREGULAR"},
            };

            ViewBag.listatDomicilio = LiatatDomicilio;

            foreach (var item in LiatatDomicilio)
            {
                if (item.Value == domicilio.TipoDomicilio)
                {
                    ViewBag.idtDomicilio = item.Value;
                    break;
                }
            }
            #endregion


            #region PAIS          
            List<SelectListItem> ListaPaisD;
            ListaPaisD = new List<SelectListItem>
            {
              new SelectListItem{ Text="México", Value="MEXICO"},
              new SelectListItem{ Text="Estados Unidos", Value="ESTADOS UNIDOS"},
              new SelectListItem{ Text="Canada", Value="CANADA"},
              new SelectListItem{ Text="Colombia", Value="COLOMBIA"},
              new SelectListItem{ Text="El Salvador", Value="EL SALVADOR"},
              new SelectListItem{ Text="Guatemala", Value="GUATEMALA"},
              new SelectListItem{ Text="Chile", Value="CHILE"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListaPaisD = ListaPaisD;

            foreach (var item in ListaPaisD)
            {
                if (item.Value == domicilio.Pais)
                {
                    ViewBag.idPaisD = item.Value;
                    break;
                }
            }
            #endregion

            #region Destado
            List<Estados> listaEstadosD = new List<Estados>();
            listaEstadosD = (from table in _context.Estados
                             select table).ToList();

            listaEstadosD.Insert(0, new Estados { Id = 0, Estado = "Selecciona" });
            ViewBag.ListaEstadoD = listaEstadosD;
            ViewBag.idEstadoD = domicilio.Estado;
            #endregion

            #region Lnmunicipio
            int estadoD;
            bool success = Int32.TryParse(domicilio.Estado, out estadoD);
            List<Municipios> listaMunicipiosD = new List<Municipios>();
            if (success)
            {
                listaMunicipiosD = (from table in _context.Municipios
                                    where table.EstadosId == estadoD
                                    select table).ToList();
            }

            listaMunicipiosD.Insert(0, new Municipios { Id = 0, Municipio = "Selecciona" });
            ViewBag.ListaMunicipioD = listaMunicipiosD;
            ViewBag.idMunicipioD = domicilio.Municipio;
            #endregion

            #region TemporalidadDomicilio
            List<SelectListItem> ListaDomicilioT;
            ListaDomicilioT = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Más de 10 años", Value = "MAS DE 10 AÑOS" },
                new SelectListItem{ Text = "Entre 5 y 10 años", Value = "ENTRE 5 Y 10 AÑOS" },
                new SelectListItem{ Text = "Entre 2 y 5 años", Value = "ENTRE 2 Y 5 AÑOS" },
                new SelectListItem{ Text = "Entre 6 meses y 2 años", Value = "ENTRE 6 MESES Y 2 AÑOS" },
                new SelectListItem{ Text = "Menos de 6 meses", Value = "MENOS DE 6 MESES" },
            };

            ViewBag.ListaTemporalidad = ListaDomicilioT;
            ViewBag.idTemporalidadD = BuscaId(ListaDomicilioT, domicilio.Temporalidad);
            #endregion


            ViewBag.listaResidenciaHabitual = listaSiNo;
            ViewBag.idResidenciaHabitual = BuscaId(listaSiNo, domicilio.ResidenciaHabitual);

            ViewBag.listacuentaDomicilioSecundario = listaNoSi;
            ViewBag.idcuentaDomicilioSecundario = BuscaId(listaNoSi, domicilio.DomcilioSecundario);

            ViewBag.listaZona = listaZonas;
            ViewBag.idZona = BuscaId(listaZonas, domicilio.Zona);

            ViewBag.pais = domicilio.Pais;
            ViewBag.domi = domicilio.DomcilioSecundario;

            if (domicilio == null)
            {
                return NotFound();
            }
            return View(domicilio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDomicilio(int id, [Bind("IdDomicilio,TipoDomicilio,Calle,No,TipoUbicacion,NombreCf,Pais,Estado,Municipio,Temporalidad,ResidenciaHabitual,Cp,Referencias,Horario,DomcilioSecundario,Observaciones,PersonaIdPersona, Zona")] Domicilio domicilio)
        {
            if (id != domicilio.PersonaIdPersona)
            {
                return NotFound();
            }

            domicilio.Calle = normaliza(domicilio.Calle);
            domicilio.No = normaliza(domicilio.No);
            domicilio.NombreCf = normaliza(domicilio.NombreCf);
            domicilio.Cp = normaliza(domicilio.Cp);
            domicilio.Referencias = normaliza(domicilio.Referencias);
            domicilio.Horario = normaliza(domicilio.Horario);
            domicilio.Observaciones = normaliza(domicilio.Observaciones);
            domicilio.Zona = domicilio.Zona;


            if (ModelState.IsValid)
            {
                try
                {
                    var oldDomicilio = await _context.Domicilio.FindAsync(domicilio.IdDomicilio);
                    _context.Entry(oldDomicilio).CurrentValues.SetValues(domicilio);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(domicilio);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(domicilio.PersonaIdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MenuEdicion/" + domicilio.PersonaIdPersona, "Personas");
            }
            return View(domicilio);
        }
        #endregion

        #region Edit Dmicilio Secundario

        public async Task<IActionResult> EditDomSecundario2(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var domisecu = await _context.Domicilio.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);

            #region -To List databases-
            List<Persona> personaVM = _context.Persona.ToList();
            List<Domicilio> domicilioVM = _context.Domicilio.ToList();
            List<Domiciliosecundario> domiciliosecundarioVM = _context.Domiciliosecundario.ToList();
            #endregion

            #region -Jointables-
            ViewData["joinTablesDomcilioSec"] = from personaTable in personaVM
                                                join domicilio in domicilioVM on personaTable.IdPersona equals domicilio.IdDomicilio
                                                join domicilioSec in domiciliosecundarioVM on domicilio.IdDomicilio equals domicilioSec.IdDomicilio
                                                where personaTable.IdPersona == id
                                                select new PersonaViewModel
                                                {
                                                    domicilioSecundarioVM = domicilioSec
                                                };
            #endregion

            #region TIPODOMICILIO          
            List<SelectListItem> LiatatDomicilio;
            LiatatDomicilio = new List<SelectListItem>
            {
              new SelectListItem{ Text="Rentada", Value="RENTADA"},
              new SelectListItem{ Text="Prestada", Value="PRESTADA"},
              new SelectListItem{ Text="Propia", Value="PROPIA"},
              new SelectListItem{ Text="Familiar", Value="FAMILIAR"},
              new SelectListItem{ Text="Situación de calle", Value="SITUACION DE CALLE"},
              new SelectListItem{ Text="Irregular", Value="IRREGULAR"},
            };

            ViewBag.listatDomicilio = LiatatDomicilio;
            ViewBag.idtDomicilio = BuscaId(LiatatDomicilio, domisecu.TipoDomicilio);
            #endregion

            #region PAIS          
            List<SelectListItem> ListaPaisD;
            ListaPaisD = new List<SelectListItem>
            {
              new SelectListItem{ Text="México", Value="MEXICO"},
              new SelectListItem{ Text="Estados Unidos", Value="ESTADOS UNIDOS"},
              new SelectListItem{ Text="Canada", Value="CANADA"},
              new SelectListItem{ Text="Colombia", Value="COLOMBIA"},
              new SelectListItem{ Text="El Salvador", Value="EL SALVADOR"},
              new SelectListItem{ Text="Guatemala", Value="GUATEMALA"},
              new SelectListItem{ Text="Chile", Value="CHILE"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListaPaisED = ListaPaisD;
            ViewBag.idPaisED = BuscaId(ListaPaisD, domisecu.Pais);

            ViewBag.ListaPaisM = ListaPaisD;
            ViewBag.idPaisM = BuscaId(ListaPaisD, domisecu.Pais);
            #endregion

            #region Destado
            List<Estados> listaEstadosD = new List<Estados>();
            listaEstadosD = (from table in _context.Estados
                             select table).ToList();

            List<Domiciliosecundario> listadomiciliosecundarios = new List<Domiciliosecundario>();
            listadomiciliosecundarios = (from table in _context.Domiciliosecundario
                                         select table).ToList();


            listaEstadosD.Insert(0, new Estados { Id = 0, Estado = "Selecciona" });
            ViewBag.ListaEstadoED = listaEstadosD;
            ViewBag.idEstadoED = domisecu.Estado;

            ViewBag.ListaEstadoM = listaEstadosD;
            ViewBag.idEstadoM = domisecu.Estado;
            #endregion

            #region Lnmunicipio
            int estadoD;
            bool success = Int32.TryParse(domisecu.Estado, out estadoD);
            List<Municipios> listaMunicipiosD = new List<Municipios>();
            if (success)
            {
                listaMunicipiosD = (from table in _context.Municipios
                                    where table.EstadosId == estadoD
                                    select table).ToList();
            }

            ViewBag.ListaMunicipioED = listaMunicipiosD;
            ViewBag.idMunicipioED = domisecu.Municipio;

            ViewBag.ListaMunicipioM = listaMunicipiosD;
            ViewBag.idMunicipioM = domisecu.Municipio;

            ViewBag.Pais = domisecu.Pais;
            #endregion


            #region TemporalidadDomicilio
            List<SelectListItem> ListaDomicilioT;
            ListaDomicilioT = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Más de 10 años", Value = "MAS DE 10 AÑOS" },
                new SelectListItem{ Text = "Entre 5 y 10 años", Value = "ENTRE 5 Y 10 AÑOS" },
                new SelectListItem{ Text = "Entre 2 y 5 años", Value = "ENTRE 2 Y 5 AÑOS" },
                new SelectListItem{ Text = "Entre 6 meses y 2 año", Value = "ENTRE 6 MESES Y 2 AÑO" },
                new SelectListItem{ Text = "Menos de 6 meses", Value = "MENOS DE 6 MESES" },
            };

            ViewBag.ListaTemporalidad = ListaDomicilioT;
            ViewBag.idTemporalidad = BuscaId(ListaDomicilioT, domisecu.Temporalidad);



            ViewBag.listaResidenciaHabitual = listaSiNo;
            ViewBag.idResidenciaHabitual = BuscaId(listaSiNo, domisecu.ResidenciaHabitual);

            #endregion
            if (domisecu == null)
            {
                return NotFound();
            }

            return View(domisecu);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDomSecundario([Bind("IdDomicilioSecundario,IdDomicilio,TipoDomicilio,Calle,No,TipoUbicacion,NombreCf,Pais,Estado,Municipio,Temporalidad,ResidenciaHabitual,Cp,Referencias,Horario,Motivo,Observaciones")] Domiciliosecundario domiciliosecundario, Domicilio domicilio)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var oldDomicilio = await _context.Domiciliosecundario.FindAsync(domiciliosecundario.IdDomicilioSecundario);
                    _context.Entry(oldDomicilio).CurrentValues.SetValues(domiciliosecundario);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(domiciliosecundario);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!DomiciliosecundarioExists(domiciliosecundario.IdDomicilioSecundario))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> DeleteConfirmedDom(int? id)
        {
            var domseundario = await _context.Domiciliosecundario.SingleOrDefaultAsync(m => m.IdDomicilioSecundario == id);
            _context.Domiciliosecundario.Remove(domseundario);
            await _context.SaveChangesAsync();

            var empty = (from ds in _context.Domiciliosecundario
                         where ds.IdDomicilio == domseundario.IdDomicilio
                         select ds);

            if (!empty.Any())
            {
                var query = (from a in _context.Domicilio
                             where a.IdDomicilio == domseundario.IdDomicilio
                             select a).FirstOrDefault();
                query.DomcilioSecundario = "NO";
                _context.SaveChanges();
            }

            return RedirectToAction("EditDomicilio/" + domseundario.IdDomicilio, "Personas");
        }

        public async Task<IActionResult> CrearDomicilioSecundario(Domiciliosecundario domiciliosecundario, string[] datosDomicilio)
        {
            domiciliosecundario.IdDomicilio = Int32.Parse(datosDomicilio[0]);
            domiciliosecundario.TipoDomicilio = datosDomicilio[1];
            domiciliosecundario.Calle = datosDomicilio[2];
            domiciliosecundario.No = datosDomicilio[3];
            domiciliosecundario.TipoUbicacion = datosDomicilio[4];
            domiciliosecundario.NombreCf = datosDomicilio[5];
            domiciliosecundario.Pais = datosDomicilio[6];
            domiciliosecundario.Estado = datosDomicilio[7];
            domiciliosecundario.Municipio = datosDomicilio[8];
            domiciliosecundario.Temporalidad = datosDomicilio[9];
            domiciliosecundario.ResidenciaHabitual = datosDomicilio[10];
            domiciliosecundario.Cp = datosDomicilio[11];
            domiciliosecundario.Referencias = datosDomicilio[12];
            domiciliosecundario.Motivo = datosDomicilio[13];
            domiciliosecundario.Horario = datosDomicilio[14];
            domiciliosecundario.Observaciones = datosDomicilio[15];


            var query = (from a in _context.Domicilio
                         where a.IdDomicilio == domiciliosecundario.IdDomicilio
                         select a).FirstOrDefault();
            query.DomcilioSecundario = "SI";
            _context.SaveChanges();



            _context.Add(domiciliosecundario);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditDomicilio/" + domiciliosecundario.IdDomicilio, "Personas");
        }
        #endregion

        #region -Edita Escolaridad-
        public async Task<IActionResult> EditEscolaridad(string nombre, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Nombre"] = nombre;
            var estudios = await _context.Estudios.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);
            if (estudios == null)
            {
                return NotFound();
            }

            ViewBag.estudia = estudios.Estudia;
            ViewBag.listaEstudia = listaNoSi;
            ViewBag.idEstudia = BuscaId(listaNoSi, estudios.Estudia);

            #region GradoEstudios
            List<SelectListItem> ListaGradoEstudios;
            ListaGradoEstudios = new List<SelectListItem>
            {
              new SelectListItem{ Text="Primaria", Value="PRIMARIA"},
              new SelectListItem{ Text="Secundaria", Value="SECUNDARIA"},
              new SelectListItem{ Text="Bachillerato", Value="BACHILLERATO"},
              new SelectListItem{ Text="TSU", Value="TSU"},
              new SelectListItem{ Text="Licenciatura", Value="LICENCIATURA"},
              new SelectListItem{ Text="Maestría", Value="MAESTRÍA"},
              new SelectListItem{ Text="Doctorado", Value="DOCTORADO"}
            };

            ViewBag.listaGradoEstudios = ListaGradoEstudios;
            ViewBag.idGradoEstudios = BuscaId(ListaGradoEstudios, estudios.GradoEstudios);
            #endregion


            List<Estudios> estudiosVM = _context.Estudios.ToList();
            List<Persona> personaVM = _context.Persona.ToList();


            ViewData["joinTablesPersonaEstudia"] =
                                     from personaTable in personaVM
                                     join estudiosTabla in estudiosVM on personaTable.IdPersona equals estudiosTabla.PersonaIdPersona
                                     where personaTable.IdPersona == idPersona
                                     select new PersonaViewModel
                                     {
                                         personaVM = personaTable,
                                         estudiosVM = estudiosTabla

                                     };

            if ((ViewData["joinTablesPersonaEstudia"] as IEnumerable<scorpioweb.Models.PersonaViewModel>).Count() == 0)
            {
                ViewBag.RA = false;
            }
            else
            {
                ViewBag.RA = true;
            }


            return View(estudios);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEscolaridad(int id, [Bind("IdEstudios,Estudia,GradoEstudios,InstitucionE,Horario,Direccion,Telefono,Observaciones,PersonaIdPersona")] Estudios estudios)
        {
            if (id != estudios.PersonaIdPersona)
            {
                return NotFound();
            }

            estudios.InstitucionE = normaliza(estudios.InstitucionE);
            estudios.Horario = normaliza(estudios.Horario);
            estudios.Direccion = normaliza(estudios.Direccion);
            estudios.Observaciones = normaliza(estudios.Observaciones);



            if (ModelState.IsValid)
            {
                try
                {
                    var oldEstudios = await _context.Estudios.FindAsync(estudios.IdEstudios);
                    _context.Entry(oldEstudios).CurrentValues.SetValues(estudios);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(estudios);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(estudios.PersonaIdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MenuEdicion/" + estudios.PersonaIdPersona, "Personas");
            }
            return View(estudios);
        }
        #endregion

        #region -Edita Trabajo-
        public async Task<IActionResult> EditTrabajo(string nombre, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Nombre"] = nombre;
            var trabajo = await _context.Trabajo.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);
            if (trabajo == null)
            {
                return NotFound();
            }

            ViewBag.listaTrabaja = listaSiNo;
            ViewBag.idTrabaja = BuscaId(listaSiNo, trabajo.Trabaja);

            #region TipoOcupacion
            List<SelectListItem> ListaTipoOcupacion;
            ListaTipoOcupacion = new List<SelectListItem>
            {
                new SelectListItem{ Text = "NA", Value = "NA" },
                new SelectListItem{ Text = "Funcionarios, directores y jefes", Value = "FUNCIONARIO, DIRECTOR Y JEFE" },
                new SelectListItem{ Text = "Profesionistas y técnicos", Value = "PROFESIONISTA O TÉCNICO" },
                new SelectListItem{ Text = "Trabajadores auxiliares en actividades administrativas", Value = "TRABAJADORES AUXILIARES EN ACTIVIDADES ADMINISTRATIVAS" },
                new SelectListItem{ Text = "Comerciantes, empleados en ventas y agentes de ventas", Value = "COMERCIANTES, EMPLEADOS DE VENTA Y AGENTES DE VENTAS" },
                new SelectListItem{ Text = "Trabajadores en servicios personales y vigilancia", Value = "TRABAJADORES EN SERVICIOS PERSONALES Y VIGILANCIA" },
                new SelectListItem{ Text = "Trabajadores en actividades agrícolas, ganaderas, forestales, caza y pesca", Value = "TRABAJADORES EN ACTIVIDADES AGRICOLAS, GANADERAS, FORESTALES, CAZA Y PESCA" },
                new SelectListItem{ Text = "Trabajadores artesanales", Value = "TRABAJADORES ARTESANALES" },
                new SelectListItem{ Text = "Operadores de maquinaria industrial, ensambladores, choferes y conductores de transporte", Value = "OPERADORES DE MAQUINARIA INDUSTRIAL, ENSAMBLADORES, CHOFERES Y CONDUCTORES DE TRANSPORTE" },
                new SelectListItem{ Text = "Trabajadores en actividades elementales y de apoyo", Value = "TRABAJADORES EN ACTIVIDADES ELEMENTALES Y DE APOYO" }
            };

            ViewBag.listaTipoOcupacion = ListaTipoOcupacion;
            ViewBag.idTipoOcupacion = BuscaId(ListaTipoOcupacion, trabajo.TipoOcupacion);
            #endregion

            ViewBag.listaEnteradoProceso = listaNoSiNA;
            ViewBag.idEnteradoProceso = BuscaId(listaNoSiNA, trabajo.EnteradoProceso);

            ViewBag.listasePuedeEnterarT = listaNoSiNA;
            ViewBag.idsePuedeEnterart = BuscaId(listaNoSiNA, trabajo.SePuedeEnterar);
            ViewBag.trabaja = trabajo.Trabaja;

            #region TiempoTrabajando
            List<SelectListItem> ListaTiempoTrabajando;
            ListaTiempoTrabajando = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Más de 10 años", Value = "MAS DE 10 AÑOS" },
                new SelectListItem{ Text = "Entre 2 y 4 años", Value = "ENTRE 2 Y 4 AÑOS" },
                new SelectListItem{ Text = "Entre 5 y 9 años", Value = "ENTRE 5 Y 9 AÑOS" },
                new SelectListItem{ Text = "Más de un año menos de 2", Value = "MAS DE UN AÑO Y MENOS DE 2" },
                new SelectListItem{ Text = "Entre 6 meses y 1 año", Value = "ENTRE 6 MESES Y 1 AÑO" },
                new SelectListItem{ Text = "Menos de 6 meses", Value = "MENOS DE 6 MESES" },
                new SelectListItem{ Text = "Tiene menos de 3 meses sin actividad laboral", Value = "SIN TRABAJO DURANTE MENOS DE 3 MESES" },
                new SelectListItem{ Text = "Tiene más de 3 meses sin actividad laboral", Value = "SIN TRABAJO DURANTE MAS DE 3 MESES" }
            };

            ViewBag.listaTiempoTrabajando = ListaTiempoTrabajando;
            ViewBag.idTiempoTrabajando = BuscaId(ListaTiempoTrabajando, trabajo.TiempoTrabajano);
            #endregion

            #region TemporalidadSalario
            List<SelectListItem> ListaTemporalidadSalario;
            ListaTemporalidadSalario = new List<SelectListItem>
            {
                new SelectListItem{ Text = "NA", Value = "NA" },
                new SelectListItem{ Text = "Diario", Value = "DIARIO" },
                new SelectListItem{ Text = "Semanal", Value = "SEMANAL" },
                new SelectListItem{ Text = "Quincenal", Value = "QUINCENAL" },
                new SelectListItem{ Text = "Mensual", Value = "MENSUAL" }
            };

            ViewBag.listaTemporalidadSalario = ListaTemporalidadSalario;
            ViewBag.idTemporalidadSalario = BuscaId(ListaTemporalidadSalario, trabajo.TemporalidadSalario);
            #endregion

            return View(trabajo);
        }

        // POST: Trabajoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrabajo(int id, [Bind("IdTrabajo,Trabaja,TipoOcupacion,Puesto,EmpledorJefe,EnteradoProceso,SePuedeEnterar,TiempoTrabajano,Salario,TemporalidadSalario,Direccion,Horario,Telefono,Observaciones,PersonaIdPersona")] Trabajo trabajo)
        {
            if (id != trabajo.PersonaIdPersona)
            {
                return NotFound();
            }

            trabajo.Puesto = normaliza(trabajo.Puesto);
            trabajo.EmpledorJefe = normaliza(trabajo.EmpledorJefe);
            trabajo.Salario = normaliza(trabajo.Salario);
            trabajo.Direccion = normaliza(trabajo.Direccion);
            trabajo.Horario = normaliza(trabajo.Horario);
            trabajo.Observaciones = normaliza(trabajo.Observaciones);

            if (ModelState.IsValid)
            {
                try
                {
                    var oldTrabajo = await _context.Trabajo.FindAsync(trabajo.IdTrabajo);
                    _context.Entry(oldTrabajo).CurrentValues.SetValues(trabajo);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(trabajo);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(trabajo.PersonaIdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MenuEdicion/" + trabajo.PersonaIdPersona, "Personas");
            }
            return View(trabajo);
        }
        #endregion

        #region -Edita Actividades Sociales-
        public async Task<IActionResult> EditActividadesSociales(string nombre, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Nombre"] = nombre;
            var actividadsocial = await _context.Actividadsocial.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);

            ViewBag.listasePuedeEnterarASr = listaNoSiNA;
            ViewBag.idsePuedeEnterarAS = BuscaId(listaNoSiNA, actividadsocial.SePuedeEnterar);

            if (actividadsocial == null)
            {
                return NotFound();
            }
            return View(actividadsocial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditActividadesSociales(int id, [Bind("IdActividadSocial,TipoActividad,Horario,Lugar,Telefono,SePuedeEnterar,Referencia,Observaciones,PersonaIdPersona")] Actividadsocial actividadsocial)
        {
            if (id != actividadsocial.PersonaIdPersona)
            {
                return NotFound();
            }

            actividadsocial.TipoActividad = normaliza(actividadsocial.TipoActividad);
            actividadsocial.Horario = normaliza(actividadsocial.Horario);
            actividadsocial.Lugar = normaliza(actividadsocial.Lugar);
            actividadsocial.Referencia = normaliza(actividadsocial.Referencia);
            actividadsocial.Observaciones = normaliza(actividadsocial.Observaciones);



            if (ModelState.IsValid)
            {
                try
                {
                    var oldActividadsocial = await _context.Actividadsocial.FindAsync(actividadsocial.IdActividadSocial);
                    _context.Entry(oldActividadsocial).CurrentValues.SetValues(actividadsocial);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(actividadsocial);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(actividadsocial.PersonaIdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MenuEdicion/" + actividadsocial.PersonaIdPersona, "Personas");
            }
            return View(actividadsocial);
        }
        #endregion

        #region -Edita Abandono Estado-
        public async Task<IActionResult> EditAbandonoEstado(string nombre, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Nombre"] = nombre;
            var abandonoestado = await _context.Abandonoestado.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);

            ViewBag.listaVividoFuera = listaNoSi;
            ViewBag.idVividoFuera = BuscaId(listaNoSi, abandonoestado.VividoFuera);

            ViewBag.listaViajaHabitual = listaNoSi;
            ViewBag.idViajaHabitual = BuscaId(listaNoSi, abandonoestado.ViajaHabitual);

            ViewBag.listaDocumentacionSalirPais = listaNoSi;
            ViewBag.idDocumentacionSalirPais = BuscaId(listaNoSi, abandonoestado.DocumentacionSalirPais);

            ViewBag.listaPasaporte = listaNoSi;
            ViewBag.idPasaporte = BuscaId(listaNoSi, abandonoestado.Pasaporte);

            ViewBag.listaVisa = listaNoSi;
            ViewBag.idVisa = BuscaId(listaNoSi, abandonoestado.Visa);

            ViewBag.listaFamiliaresFuera = listaNoSi;
            ViewBag.idFamiliaresFuera = BuscaId(listaNoSi, abandonoestado.FamiliaresFuera);

            ViewBag.vfuera = abandonoestado.VividoFuera;
            ViewBag.vlugar = abandonoestado.ViajaHabitual;
            ViewBag.document = abandonoestado.DocumentacionSalirPais;
            ViewBag.Abandono = abandonoestado.FamiliaresFuera;



            if (abandonoestado == null)
            {
                return NotFound();
            }
            return View(abandonoestado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAbandonoEstado(int id, [Bind("IdAbandonoEstado,VividoFuera,LugaresVivido,TiempoVivido,MotivoVivido,ViajaHabitual,LugaresViaje,TiempoViaje,MotivoViaje,DocumentacionSalirPais,Pasaporte,Visa,FamiliaresFuera,Cuantos,PersonaIdPersona")] Abandonoestado abandonoestado)
        {
            if (id != abandonoestado.PersonaIdPersona)
            {
                return NotFound();
            }

            abandonoestado.LugaresVivido = normaliza(abandonoestado.LugaresVivido);
            abandonoestado.TiempoVivido = normaliza(abandonoestado.TiempoVivido);
            abandonoestado.MotivoVivido = normaliza(abandonoestado.MotivoVivido);
            abandonoestado.LugaresViaje = normaliza(abandonoestado.LugaresViaje);
            abandonoestado.TiempoViaje = normaliza(abandonoestado.TiempoViaje);
            abandonoestado.MotivoViaje = normaliza(abandonoestado.MotivoViaje);




            if (ModelState.IsValid)
            {
                try
                {
                    var oldAbandonoestado = await _context.Abandonoestado.FindAsync(abandonoestado.IdAbandonoEstado);
                    _context.Entry(oldAbandonoestado).CurrentValues.SetValues(abandonoestado);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(abandonoestado.PersonaIdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MenuEdicion/" + abandonoestado.PersonaIdPersona, "Personas");
            }
            return View(abandonoestado);
        }
        #endregion

        #region -EditFamiliaresForaneos-
        public async Task<IActionResult> EditFamiliaresForaneos(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var familiaresforaneos = await _context.Familiaresforaneos.Where(m => m.PersonaIdPersona == id).FirstOrDefaultAsync();
            ViewBag.idFamiliarF = familiaresforaneos.PersonaIdPersona;

            #region GENERO          
            List<SelectListItem> ListaGenero;
            ListaGenero = new List<SelectListItem>
            {
              new SelectListItem{ Text="Masculino", Value="M"},
              new SelectListItem{ Text="Femenino", Value="F"},
              new SelectListItem{ Text="Prefiero no decirlo", Value="N"},
            };
            ViewBag.listaGenero = ListaGenero;
            ViewBag.idGenero = BuscaId(ListaGenero, familiaresforaneos.Sexo);
            #endregion

            #region Relacion
            List<SelectListItem> ListaRelacion;
            ListaRelacion = new List<SelectListItem>
            {
              new SelectListItem{ Text="Máma", Value="MAMA"},
              new SelectListItem{ Text="Pápa", Value="PAPA"},
              new SelectListItem{ Text="Esposo (a)", Value="ESPOSO (A)"},
              new SelectListItem{ Text="Hermano (a)", Value="HERMAN0 (A)"},
              new SelectListItem{ Text="Hijo (a)", Value="HIJO (A)"},
              new SelectListItem{ Text="Abelo (a)", Value="ABUELO (A)"},
              new SelectListItem{ Text="Familiar 1 Nivel", Value="FAMILIAR 1 NIVEL"},
              new SelectListItem{ Text="Amigo", Value="AMIGO"},
              new SelectListItem{ Text="Conocido", Value="CONOCIDO (A)"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };
            ViewBag.listaRelacion = ListaRelacion;
            ViewBag.idRelacion = BuscaId(ListaRelacion, familiaresforaneos.Relacion);
            #endregion

            #region Tiempo de conocerlo
            List<SelectListItem> ListaTiempo;
            ListaTiempo = new List<SelectListItem>
            {
              new SelectListItem{ Text="Menos de un año", Value="MENOS DE 1 AÑO"},
              new SelectListItem{ Text="Entre 1 y 2 años", Value="ENTRE 1 Y 2 AÑOS"},
              new SelectListItem{ Text="Entre 2 y 5 años(a)", Value="ENTRE 2 Y 5 AÑOS"},
              new SelectListItem{ Text="Más de 5 años", Value="MÁS DE 5 AÑOS"},
              new SelectListItem{ Text="Toda la vida", Value="TODA LA VIDA"},
            };
            ViewBag.listTiempo = ListaTiempo;
            ViewBag.idTiempo = BuscaId(ListaTiempo, familiaresforaneos.TiempoConocerlo);
            #endregion

            #region PAIS          
            List<SelectListItem> ListaPaisD;
            ListaPaisD = new List<SelectListItem>
            {
              new SelectListItem{ Text="México", Value="MEXICO"},
              new SelectListItem{ Text="Estados Unidos", Value="ESTADOS UNIDOS"},
              new SelectListItem{ Text="Canada", Value="CANADA"},
              new SelectListItem{ Text="Colombia", Value="COLOMBIA"},
              new SelectListItem{ Text="El Salvador", Value="EL SALVADOR"},
              new SelectListItem{ Text="Guatemala", Value="GUATEMALA"},
              new SelectListItem{ Text="Chile", Value="CHILE"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListaPais = ListaPaisD;
            ViewBag.idPais = BuscaId(ListaPaisD, familiaresforaneos.Pais);
            #endregion

            #region Destado
            List<Estados> listaEstado = new List<Estados>();
            listaEstado = (from table in _context.Estados
                           select table).ToList();

            listaEstado.Insert(0, new Estados { Id = 0, Estado = "Selecciona" });
            ViewBag.ListaEstado = listaEstado;
            ViewBag.idEstado = familiaresforaneos.Estado;
            #endregion

            #region Frecuencia de contacto
            List<SelectListItem> ListaFrecuencia;
            ListaFrecuencia = new List<SelectListItem>
            {
              new SelectListItem{ Text="Diariamente", Value="DIARIAMENTE"},
              new SelectListItem{ Text="Una vez a la semana", Value="UNA VEZ A LA SEMANA"},
              new SelectListItem{ Text="Una vez cada 15 días", Value="UNA VEZ CADA 15 DIAS"},
              new SelectListItem{ Text="Una vez al mes", Value="UNA VEZ AL MES"},
              new SelectListItem{ Text=" Una vez al año", Value="UNA VEZ AL AÑO"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListFrecuencia = ListaFrecuencia;
            ViewBag.idFrecuencia = BuscaId(ListaFrecuencia, familiaresforaneos.Pais);
            #endregion

            ViewBag.listaProseso = listaNoSi;
            ViewBag.idProseso = BuscaId(listaNoSi, familiaresforaneos.EnteradoProceso);

            ViewBag.listaEnterar = listaNoSiNA;
            ViewBag.idEnterar = BuscaId(listaNoSiNA, familiaresforaneos.PuedeEnterarse);
            ViewBag.Pais = familiaresforaneos.Pais;

            if (familiaresforaneos == null)
            {
                return NotFound();
            }
            return View(familiaresforaneos);
        }

        public async Task<IActionResult> EditFamiliaresForaneos2(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var familiaresforaneos = await _context.Familiaresforaneos.FirstAsync(m => m.PersonaIdPersona == id);

            #region -To List databases-
            List<Persona> personaVM = _context.Persona.ToList();
            List<Familiaresforaneos> familiaresforaneosVM = _context.Familiaresforaneos.ToList();

            #endregion

            #region -Jointables-
            ViewData["joinTableFamiliarF"] = from personaTable in personaVM
                                             join familiarf in familiaresforaneosVM on personaTable.IdPersona equals familiarf.PersonaIdPersona
                                             where familiarf.PersonaIdPersona == id
                                             select new PersonaViewModel
                                             {
                                                 familiaresForaneosVM = familiarf
                                             };
            #endregion


            #region GENERO          
            List<SelectListItem> ListaGenero;
            ListaGenero = new List<SelectListItem>
            {
              new SelectListItem{ Text="Masculino", Value="M"},
              new SelectListItem{ Text="Femenino", Value="F"},
              new SelectListItem{ Text="Prefiero no decirlo", Value="N"},
            };
            ViewBag.listaGenero = ListaGenero;

            #endregion


            #region Relacion
            List<SelectListItem> ListaRelacion;
            ListaRelacion = new List<SelectListItem>
            {
              new SelectListItem{ Text="Máma", Value="MAMA"},
              new SelectListItem{ Text="Pápa", Value="PAPA"},
              new SelectListItem{ Text="Esposo (a)", Value="ESPOSO (A)"},
              new SelectListItem{ Text="Hermano (a)", Value="HERMAN0 (A)"},
              new SelectListItem{ Text="Hijo (a)", Value="HIJO (A)"},
              new SelectListItem{ Text="Abelo (a)", Value="ABUELO (A)"},
              new SelectListItem{ Text="Familiar 1 Nivel", Value="FAMILIAR 1 NIVEL"},
              new SelectListItem{ Text="Amigo", Value="AMIGO"},
              new SelectListItem{ Text="Conocido", Value="CONOCIDO (A)"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };
            ViewBag.listaRelacion = ListaRelacion;

            #endregion

            #region Tiempo de conocerlo
            List<SelectListItem> ListaTiempo;
            ListaTiempo = new List<SelectListItem>
            {
              new SelectListItem{ Text="Menos de un año", Value="MENOS DE 1 AÑO"},
              new SelectListItem{ Text="Entre 1 y 2 años", Value="ENTRE 1 Y 2 AÑOS"},
              new SelectListItem{ Text="Entre 2 y 5 años(a)", Value="ENTRE 2 Y 5 AÑOS"},
              new SelectListItem{ Text="Más de 5 años", Value="MÁS DE 5 AÑOS"},
              new SelectListItem{ Text="Toda la vida", Value="TODA LA VIDA"},
            };
            ViewBag.listTiempo = ListaTiempo;

            #endregion

            #region PAIS          
            List<SelectListItem> ListaPaisD;
            ListaPaisD = new List<SelectListItem>
            {
              new SelectListItem{ Text="México", Value="MEXICO"},
              new SelectListItem{ Text="Estados Unidos", Value="ESTADOS UNIDOS"},
              new SelectListItem{ Text="Canada", Value="CANADA"},
              new SelectListItem{ Text="Colombia", Value="COLOMBIA"},
              new SelectListItem{ Text="El Salvador", Value="EL SALVADOR"},
              new SelectListItem{ Text="Guatemala", Value="GUATEMALA"},
              new SelectListItem{ Text="Chile", Value="CHILE"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListaPais = ListaPaisD;

            #endregion

            #region Destado
            List<Estados> listaEstado = new List<Estados>();
            listaEstado = (from table in _context.Estados
                           select table).ToList();

            listaEstado.Insert(0, new Estados { Id = 0, Estado = "Selecciona" });
            ViewBag.ListaEstado = listaEstado;

            #endregion
            #region Frecuencia de contacto
            List<SelectListItem> ListaFrecuencia;
            ListaFrecuencia = new List<SelectListItem>
            {
              new SelectListItem{ Text="Diariamente", Value="DIARIAMENTE"},
              new SelectListItem{ Text="Una vez a la semana", Value="UNA VEZ A LA SEMANA"},
              new SelectListItem{ Text="Una vez cada 15 días", Value="UNA VEZ CADA 15 DIAS"},
              new SelectListItem{ Text="Una vez al mes", Value="UNA VEZ AL MES"},
              new SelectListItem{ Text=" Una vez al año", Value="UNA VEZ AL AÑO"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListFrecuencia = ListaFrecuencia;

            #endregion

            ViewBag.listaProseso = listaNoSi;

            ViewBag.listaEnterar = listaNoSiNA;

            if (familiaresforaneos == null)
            {
                return NotFound();
            }

            return View(familiaresforaneos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFamiliaresForaneos(int id, [Bind("IdFamiliaresForaneos,Nombre,Edad,Sexo,Relacion,TiempoConocerlo,Pais,Estado,Telefono,FrecuenciaContacto,EnteradoProceso,PuedeEnterarse,Observaciones,PersonaIdPersona")] Familiaresforaneos familiaresforaneos)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var oldFamiliaresforaneos = await _context.Familiaresforaneos.FindAsync(familiaresforaneos.IdFamiliaresForaneos);
                    _context.Entry(oldFamiliaresforaneos).CurrentValues.SetValues(familiaresforaneos);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(familiaresforaneos);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction("EditAbandonoEstado/" + familiaresforaneos.PersonaIdPersona, "Personas");
            }
            return View(familiaresforaneos);
        }

        public async Task<IActionResult> DeleteConfirmedFamiiarF(int? id)
        {
            var familiarf = await _context.Familiaresforaneos.SingleOrDefaultAsync(m => m.IdFamiliaresForaneos == id);
            _context.Familiaresforaneos.Remove(familiarf);
            await _context.SaveChangesAsync();

            var empty = (from ff in _context.Familiaresforaneos
                         where ff.PersonaIdPersona == familiarf.PersonaIdPersona
                         select ff);

            if (!empty.Any())
            {
                var query = (from a in _context.Abandonoestado
                             where a.PersonaIdPersona == familiarf.PersonaIdPersona
                             select a).FirstOrDefault();
                query.FamiliaresFuera = "NO";
                _context.SaveChanges();
            }




            return RedirectToAction("EditAbandonoEstado/" + familiarf.PersonaIdPersona, "Personas");
        }

        public async Task<IActionResult> CrearFamiliarforaneo(Familiaresforaneos familiaresforaneos, string[] datosFamiliarF)
        {

            familiaresforaneos.PersonaIdPersona = Int32.Parse(datosFamiliarF[0]);
            familiaresforaneos.Nombre = datosFamiliarF[1];
            familiaresforaneos.Edad = Int32.Parse(datosFamiliarF[2]);
            familiaresforaneos.Sexo = datosFamiliarF[3];
            familiaresforaneos.Relacion = datosFamiliarF[4];
            familiaresforaneos.TiempoConocerlo = datosFamiliarF[5];
            familiaresforaneos.Pais = datosFamiliarF[6];
            familiaresforaneos.Estado = datosFamiliarF[7];
            familiaresforaneos.Telefono = datosFamiliarF[8];
            familiaresforaneos.FrecuenciaContacto = datosFamiliarF[9];
            familiaresforaneos.EnteradoProceso = datosFamiliarF[10];
            familiaresforaneos.PuedeEnterarse = datosFamiliarF[11];
            familiaresforaneos.Observaciones = datosFamiliarF[12];

            var query = (from a in _context.Abandonoestado
                         where a.PersonaIdPersona == familiaresforaneos.PersonaIdPersona
                         select a).FirstOrDefault();
            query.FamiliaresFuera = "SI";
            _context.SaveChanges();

            _context.Add(familiaresforaneos);
            await _context.SaveChangesAsync();


            return View();
        }
        #endregion

        #region -Editar Salud-
        public async Task<IActionResult> EditSalud(string nombre, string genero, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Nombre"] = nombre;
            ViewData["Genero"] = genero;
            var saludfisica = await _context.Saludfisica.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);


            ViewBag.listaEnfermedad = listaNoSi;
            ViewBag.idEnfermedad = BuscaId(listaNoSi, saludfisica.Enfermedad);

            ViewBag.listaEmbarazoLactancia = listaNoSi;
            ViewBag.idEmbarazoLactancia = BuscaId(listaNoSi, saludfisica.EmbarazoLactancia);

            ViewBag.listaDiscapacidad = listaNoSi;
            ViewBag.idDiscapacidad = BuscaId(listaNoSi, saludfisica.Discapacidad);

            ViewBag.listaServicioMedico = listaNoSi;
            ViewBag.idServicioMedico = BuscaId(listaNoSi, saludfisica.ServicioMedico);

            ViewBag.enfermedad = saludfisica.Enfermedad;
            ViewBag.especial = saludfisica.Discapacidad;
            ViewBag.smedico = saludfisica.ServicioMedico;

            #region EspecifiqueServicioMedico
            List<SelectListItem> ListaEspecifiqueServicioMedico;
            ListaEspecifiqueServicioMedico = new List<SelectListItem>
            {
                new SelectListItem{ Text = "NA", Value = "NA" },
                new SelectListItem{ Text = "Derecho habiente", Value = "DERECHO HABIENTE" },
                new SelectListItem{ Text = "Seguro Médico", Value = "SEGURO MEDICO" }
            };

            ViewBag.listaEspecifiqueServicioMedico = ListaEspecifiqueServicioMedico;
            ViewBag.idEspecifiqueServicioMedico = BuscaId(ListaEspecifiqueServicioMedico, saludfisica.EspecifiqueServicioMedico);
            #endregion

            #region InstitucionServicioMedico
            List<SelectListItem> ListaInstitucionServicioMedico;
            ListaInstitucionServicioMedico = new List<SelectListItem>
            {
                new SelectListItem{ Text = "NA", Value = "NA" },
                new SelectListItem{ Text = "IMSS", Value = "IMSS" },
                new SelectListItem{ Text = "ISSSTE", Value = "ISSSTE" },
                new SelectListItem{ Text = "Seguro Popular", Value = "SEGURO POPULAR" },
                new SelectListItem{ Text = "Militar", Value = "MILITAR" },
                new SelectListItem{ Text = "Otro", Value = "OTRO" }
            };

            ViewBag.listaInstitucionServicioMedico = ListaInstitucionServicioMedico;
            ViewBag.idInstitucionServicioMedico = BuscaId(ListaInstitucionServicioMedico, saludfisica.InstitucionServicioMedico);
            #endregion



            if (saludfisica == null)
            {
                return NotFound();
            }
            return View(saludfisica);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSalud(int id, [Bind("IdSaludFisica,Enfermedad,EspecifiqueEnfermedad,EmbarazoLactancia,Tiempo,Tratamiento,Discapacidad,EspecifiqueDiscapacidad,ServicioMedico,EspecifiqueServicioMedico,InstitucionServicioMedico,Observaciones,PersonaIdPersona")] Saludfisica saludfisica)
        {
            if (id != saludfisica.PersonaIdPersona)
            {
                return NotFound();
            }

            saludfisica.EspecifiqueEnfermedad = normaliza(saludfisica.EspecifiqueEnfermedad);
            saludfisica.Tratamiento = normaliza(saludfisica.Tratamiento);
            saludfisica.EspecifiqueDiscapacidad = normaliza(saludfisica.EspecifiqueDiscapacidad);
            saludfisica.Observaciones = normaliza(saludfisica.Observaciones);
            saludfisica.Tiempo = normaliza(saludfisica.Tiempo);



            if (ModelState.IsValid)
            {
                try
                {
                    var oldSaludfisica = await _context.Saludfisica.FindAsync(saludfisica.IdSaludFisica);
                    _context.Entry(oldSaludfisica).CurrentValues.SetValues(saludfisica);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(saludfisica);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(saludfisica.PersonaIdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MenuEdicion/" + saludfisica.PersonaIdPersona, "Personas");
            }
            return View(saludfisica);
        }
        #endregion

        #region -WarningSupervisor-
        public async Task<IActionResult> WarningSupervisor()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.showSupervisor = false;

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP" || rol == "Masteradmin")
                {
                    ViewBag.showSupervisor = true;
                }
            }
            ViewBag.norte = user.ToString().EndsWith("\u0040nortedgepms.com");

            return View();
        }

        public async Task<IActionResult> filtra(string currentFilter)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            Boolean flagCoordinador = false, flagMaster = false;
            string usuario = user.ToString();
            DateTime fechaInforme = (DateTime.Now).AddDays(5);
            DateTime fechaControl = (DateTime.Now).AddDays(3);
            DateTime fechaInformeCoordinador = (DateTime.Now).AddDays(30);
            DateTime fechaHoy = DateTime.Today;

            if (currentFilter == null)
            {
                ViewBag.Filtro = "TODOS";
            }
            else
            {
                ViewBag.Filtro = currentFilter;
            }

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP")
                {
                    flagCoordinador = true;
                }
            }
            foreach (var rol in roles)
            {
                if (rol == "Masteradmin")
                {
                    flagMaster = true;
                }
            }

            #region -To List databases-

            List<Persona> personaVM = _context.Persona.ToList();
            List<Supervision> supervisionVM = _context.Supervision.ToList();
            List<Causapenal> causapenalVM = _context.Causapenal.ToList();
            List<Planeacionestrategica> planeacionestrategicaVM = _context.Planeacionestrategica.ToList();
            List<Fraccionesimpuestas> fraccionesimpuestasVM = _context.Fraccionesimpuestas.ToList();
            List<Domicilio> domicilioVM = _context.Domicilio.ToList();
            List<Municipios> municipiosVM = _context.Municipios.ToList();
            List<Archivointernomcscp> archivointernomcscpsVM = _context.Archivointernomcscp.ToList();
            List<Personacausapenal> personacausapenalsVM = _context.Personacausapenal.ToList();

    

            List<Fraccionesimpuestas> queryFracciones = (from f in fraccionesimpuestasVM
                                                         group f by f.SupervisionIdSupervision into grp
                                                         select grp.OrderByDescending(f => f.IdFracciones).FirstOrDefault()).ToList();


            List<Archivointernomcscp> queryHistorialArchivoadmin = (from a in _context.Archivointernomcscp
                                                                    group a by a.PersonaIdPersona into grp
                                                                    select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();
            #endregion

            #region -Jointables-
            var archivoadmin = from ha in queryHistorialArchivoadmin
                               join ai in archivointernomcscpsVM on ha.IdarchivoInternoMcscp equals ai.IdarchivoInternoMcscp
                               join p in personaVM on ha.PersonaIdPersona equals p.IdPersona
                               join domicilio in domicilioVM on p.IdPersona equals domicilio.PersonaIdPersona
                               join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                               where p.UbicacionExpediente != "ARCHIVO INTERNO" && p.UbicacionExpediente != "ARCHIVO GENERAL" &&
                               p.UbicacionExpediente != "NO UBICADO" && p.UbicacionExpediente != "SIN REGISTRO" && p.UbicacionExpediente != "NA" && p.UbicacionExpediente != null
                               join supervision in supervisionVM on p.IdPersona equals supervision.PersonaIdPersona into tmp
                               from sinsuper in tmp.DefaultIfEmpty()
                               select new PlaneacionWarningViewModel
                               {
                                   municipiosVM = municipio,
                                   personaVM = p,
                                   archivointernomcscpVM = ai,
                                   tipoAdvertencia = "Expediente físico en resguardo"
                               };

            var leftJoin = from persona in personaVM
                           join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                           join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                           join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona into tmp
                           from sinsupervision in tmp.DefaultIfEmpty()
                           select new PlaneacionWarningViewModel
                           {
                               personaVM = persona,
                               supervisionVM = sinsupervision,
                               municipiosVM = municipio,
                               tipoAdvertencia = "Sin supervisión"
                           };
            var where = from ss in leftJoin
                        where ss.supervisionVM == null
                        select new PlaneacionWarningViewModel
                        {
                            personaVM = ss.personaVM,
                            supervisionVM = ss.supervisionVM,
                            municipiosVM = ss.municipiosVM,
                            tipoAdvertencia = "Sin supervisión"
                        };

            var where2 = from ss in leftJoin
                         where ss.personaVM.Supervisor == usuario && ss.supervisionVM == null
                         select new PlaneacionWarningViewModel
                         {
                             personaVM = ss.personaVM,
                             supervisionVM = ss.supervisionVM,
                             municipiosVM = ss.municipiosVM,
                             tipoAdvertencia = "Sin supervisión"
                         };

            if (usuario == "esmeralda.vargas@dgepms.com" || usuario == "janeth@nortedgepms.com" || flagMaster == true)
            {
                var ViewDataAlertasVari = Enumerable.Empty<PlaneacionWarningViewModel>();
                switch (currentFilter)
                {
                    case "TODOS":
                        ViewDataAlertasVari = (from persona in personaVM
                                               join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                               join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                               where persona.Colaboracion == "SI"
                                               select new PlaneacionWarningViewModel
                                               {
                                                   personaVM = persona,
                                                   municipiosVM = municipio,
                                                   tipoAdvertencia = "Pendiente de asignación - colaboración"
                                               }).Union
                                               (from persona in personaVM
                                                join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                                join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                                join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                                join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                                join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                                join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                                where planeacion.FechaInforme != null && planeacion.FechaInforme < fechaInformeCoordinador && supervision.EstadoSupervision == "VIGENTE" && fracciones.FiguraJudicial == "SCP"
                                                orderby planeacion.FechaInforme
                                                select new PlaneacionWarningViewModel
                                                {
                                                    personaVM = persona,
                                                    supervisionVM = supervision,
                                                    municipiosVM = municipio,
                                                    causapenalVM = causapenal,
                                                    planeacionestrategicaVM = planeacion,
                                                    fraccionesimpuestasVM = fracciones,
                                                    figuraJudicial = fracciones.FiguraJudicial,
                                                    tipoAdvertencia = "Informe fuera de tiempo"
                                                }).Union
                                                (from persona in personaVM
                                                 join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                                 join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                                 join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                                 join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                                 join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                                 join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                                 where planeacion.FechaInforme != null && planeacion.FechaInforme < fechaControl && supervision.EstadoSupervision == "VIGENTE" && fracciones.FiguraJudicial == "MC"
                                                 select new PlaneacionWarningViewModel
                                                 {
                                                     personaVM = persona,
                                                     supervisionVM = supervision,
                                                     municipiosVM = municipio,
                                                     causapenalVM = causapenal,
                                                     planeacionestrategicaVM = planeacion,
                                                     fraccionesimpuestasVM = fracciones,
                                                     figuraJudicial = fracciones.FiguraJudicial,
                                                     tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                                 }).Union
                                            (from persona in personaVM
                                             join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                             join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                             join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                             join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                             join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                             join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                             where planeacion.FechaInforme == null && supervision.EstadoSupervision == "VIGENTE"
                                             orderby fracciones.FiguraJudicial
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = persona,
                                                 supervisionVM = supervision,
                                                 municipiosVM = municipio,
                                                 causapenalVM = causapenal,
                                                 planeacionestrategicaVM = planeacion,
                                                 fraccionesimpuestasVM = fracciones,
                                                 figuraJudicial = fracciones.FiguraJudicial,
                                                 tipoAdvertencia = "Sin fecha de informe"
                                             }).Union
                                            (from persona in personaVM
                                             join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                             join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                             join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                             join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                             join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                             where planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = persona,
                                                 supervisionVM = supervision,
                                                 municipiosVM = municipio,
                                                 causapenalVM = causapenal,
                                                 planeacionestrategicaVM = planeacion,
                                                 tipoAdvertencia = "Sin periodicidad de firma"
                                             }).Union
                                            (from persona in personaVM
                                             join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                             join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                             join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                             join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                             join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                             where persona.Supervisor != null && persona.Supervisor.EndsWith("\u0040dgepms.com") && planeacion.FechaProximoContacto != null && planeacion.FechaProximoContacto < fechaControl && supervision.EstadoSupervision == "VIGENTE"
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = persona,
                                                 supervisionVM = supervision,
                                                 municipiosVM = municipio,
                                                 causapenalVM = causapenal,
                                                 planeacionestrategicaVM = planeacion,
                                                 tipoAdvertencia = "Se paso el tiempo de la firma"
                                             }).Union
                                            (where).Union
                                            (archivoadmin)
                                            ;
                        /*.Union
                        (from persona in personaVM
                         join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                         join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                         join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                         join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                         join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                         where supervision.EstadoSupervision == null
                         select new PlaneacionWarningViewModel
                         {
                             personaVM = persona,
                             supervisionVM = supervision,
                             municipiosVM = municipio,
                             causapenalVM = causapenal,
                             planeacionestrategicaVM = planeacion,
                             tipoAdvertencia = "Sin estado de supervisión"
                         })*/
                        break;
                    case "EXPEDIENTE FISICO EN RESGUARDO":
                        ViewDataAlertasVari = archivoadmin;
                        break;
                    case "INFORME FUERA DE TIEMPO":
                        ViewDataAlertasVari = from persona in personaVM
                                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                              join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                              where planeacion.FechaInforme != null && planeacion.FechaInforme < fechaInformeCoordinador && supervision.EstadoSupervision == "VIGENTE" && fracciones.FiguraJudicial == "SCP"
                                              orderby planeacion.FechaInforme
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  supervisionVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalVM = causapenal,
                                                  planeacionestrategicaVM = planeacion,
                                                  fraccionesimpuestasVM = fracciones,
                                                  figuraJudicial = fracciones.FiguraJudicial,
                                                  tipoAdvertencia = "Informe fuera de tiempo"
                                              };
                        break;
                    case "CONTROL DE SUPERVISION A 3 DIAS O MENOS":
                        ViewDataAlertasVari = from persona in personaVM
                                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                              join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                              where planeacion.FechaInforme != null && planeacion.FechaInforme < fechaControl && supervision.EstadoSupervision == "VIGENTE" && fracciones.FiguraJudicial == "MC"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  supervisionVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalVM = causapenal,
                                                  planeacionestrategicaVM = planeacion,
                                                  fraccionesimpuestasVM = fracciones,
                                                  figuraJudicial = fracciones.FiguraJudicial,
                                                  tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                              };
                        break;
                    case "SIN FECHA DE INFORME":
                        ViewDataAlertasVari = from persona in personaVM
                                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                              join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                              where planeacion.FechaInforme == null && supervision.EstadoSupervision == "VIGENTE"
                                              orderby fracciones.FiguraJudicial
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  supervisionVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalVM = causapenal,
                                                  planeacionestrategicaVM = planeacion,
                                                  fraccionesimpuestasVM = fracciones,
                                                  figuraJudicial = fracciones.FiguraJudicial,
                                                  tipoAdvertencia = "Sin fecha de informe"
                                              };
                        break;
                    case "SIN PERIODICIDAD DE FIRMA":
                        ViewDataAlertasVari = from persona in personaVM
                                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                              where planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  supervisionVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalVM = causapenal,
                                                  planeacionestrategicaVM = planeacion,
                                                  tipoAdvertencia = "Sin periodicidad de firma"
                                              };
                        break;
                    case "SIN SUPERVISION":
                        ViewDataAlertasVari = where;
                        break;
                    case "SE PASO EL TIEMPO DE LA FIRMA":
                        ViewDataAlertasVari = from persona in personaVM
                                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                              where persona.Supervisor != null && persona.Supervisor!=null && persona.Supervisor.EndsWith("\u0040dgepms.com") && planeacion.FechaProximoContacto != null && planeacion.FechaProximoContacto < fechaControl && supervision.EstadoSupervision == "VIGENTE"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  supervisionVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalVM = causapenal,
                                                  planeacionestrategicaVM = planeacion,
                                                  tipoAdvertencia = "Se paso el tiempo de la firma"
                                              };
                        break;
                    case "PENDIENTE DE ASIGNACION - COLABORACION":
                        ViewDataAlertasVari = from persona in personaVM
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              where persona.Colaboracion == "SI"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  municipiosVM = municipio,
                                                  tipoAdvertencia = "Pendiente de asignación - colaboración"
                                              };
                        break;
                        //case "SIN ESTADO DE SUPERVISION":
                        //    ViewData["alertas"] = from persona in personaVM
                        //                          join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                        //                          join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                        //                          join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                        //                          join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                        //                          join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                        //                          where supervision.EstadoSupervision == null
                        //                          select new PlaneacionWarningViewModel
                        //                          {
                        //                              personaVM = persona,
                        //                              supervisionVM = supervision,
                        //                              municipiosVM = municipio,
                        //                              causapenalVM = causapenal,
                        //                              planeacionestrategicaVM = planeacion,
                        //                              tipoAdvertencia = "Sin estado de supervisión"
                        //                          };
                        //    break;
                }

                var warnings = Enumerable.Empty<PlaneacionWarningViewModel>();
                if (usuario == "janeth@nortedgepms.com" || flagMaster == true)
                {
                    var filteredWarnings = from pwvm in ViewDataAlertasVari
                                           where pwvm.personaVM.Supervisor != null && pwvm.personaVM.Supervisor.EndsWith("\u0040nortedgepms.com")
                                           select pwvm;
                    warnings = warnings.Union(filteredWarnings);
                }
                if (usuario == "esmeralda.vargas@dgepms.com" || flagMaster == true)
                {
                    var filteredWarnings = from pwvm in ViewDataAlertasVari
                                           where pwvm.personaVM.Supervisor != null && pwvm.personaVM.Supervisor.EndsWith("\u0040dgepms.com")
                                           select pwvm;
                    warnings = warnings.Union(filteredWarnings);
                }
                ViewData["alertas"] =  warnings;
            }
            else
            {
                List<Archivointernomcscp> queryHistorialArchivo = (from a in _context.Archivointernomcscp
                                                                   group a by a.PersonaIdPersona into grp
                                                                   select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();

                var archivo = from ha in queryHistorialArchivoadmin
                              join ai in archivointernomcscpsVM on ha.IdarchivoInternoMcscp equals ai.IdarchivoInternoMcscp
                              join p in personaVM on ha.PersonaIdPersona equals p.IdPersona
                              join domicilio in domicilioVM on p.IdPersona equals domicilio.PersonaIdPersona
                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                              where p.UbicacionExpediente == usuario.ToUpper() && p.UbicacionExpediente != null
                              join supervision in supervisionVM on p.IdPersona equals supervision.PersonaIdPersona into tmp
                              from sinsuper in tmp.DefaultIfEmpty()
                              select new PlaneacionWarningViewModel
                              {
                                  municipiosVM = municipio,
                                  personaVM = p,
                                  archivointernomcscpVM = ai,
                                  tipoAdvertencia = "Expediente físico en resguardo"
                              };

                switch (currentFilter)
                {
                    case "TODOS":
                        ViewData["alertas"] = archivo.Union
                                              (from persona in personaVM
                                               join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                               join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                               join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                               join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                               join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                               join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                               where persona.Supervisor == usuario && planeacion.FechaInforme != null && planeacion.FechaInforme < fechaInformeCoordinador && supervision.EstadoSupervision == "VIGENTE" && fracciones.FiguraJudicial == "SCP"
                                               orderby planeacion.FechaInforme
                                               select new PlaneacionWarningViewModel
                                               {
                                                   personaVM = persona,
                                                   supervisionVM = supervision,
                                                   municipiosVM = municipio,
                                                   causapenalVM = causapenal,
                                                   planeacionestrategicaVM = planeacion,
                                                   fraccionesimpuestasVM = fracciones,
                                                   figuraJudicial = fracciones.FiguraJudicial,
                                                   tipoAdvertencia = "Informe fuera de tiempo"
                                               }).Union
                                               (from persona in personaVM
                                                join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                                join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                                join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                                join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                                join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                                join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                                where persona.Supervisor == usuario && planeacion.FechaInforme != null && planeacion.FechaInforme < fechaControl && supervision.EstadoSupervision == "VIGENTE" && fracciones.FiguraJudicial == "MC"
                                                select new PlaneacionWarningViewModel
                                                {
                                                    personaVM = persona,
                                                    supervisionVM = supervision,
                                                    municipiosVM = municipio,
                                                    causapenalVM = causapenal,
                                                    planeacionestrategicaVM = planeacion,
                                                    fraccionesimpuestasVM = fracciones,
                                                    figuraJudicial = fracciones.FiguraJudicial,
                                                    tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                                }).Union
                                            (from persona in personaVM
                                             join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                             join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                             join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                             join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                             join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                             join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                             where persona.Supervisor == usuario && planeacion.FechaInforme == null && supervision.EstadoSupervision == "VIGENTE"
                                             orderby fracciones.FiguraJudicial
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = persona,
                                                 supervisionVM = supervision,
                                                 municipiosVM = municipio,
                                                 causapenalVM = causapenal,
                                                 planeacionestrategicaVM = planeacion,
                                                 fraccionesimpuestasVM = fracciones,
                                                 figuraJudicial = fracciones.FiguraJudicial,
                                                 tipoAdvertencia = "Sin fecha de informe"
                                             }).Union
                                            (from persona in personaVM
                                             join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                             join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                             join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                             join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                             join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                             where persona.Supervisor == usuario && planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = persona,
                                                 supervisionVM = supervision,
                                                 municipiosVM = municipio,
                                                 causapenalVM = causapenal,
                                                 planeacionestrategicaVM = planeacion,
                                                 tipoAdvertencia = "Sin periodicidad de firma"
                                             }).Union
                                            (from persona in personaVM
                                             join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                             join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                             join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                             join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                             join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                             where persona.Supervisor != null && persona.Supervisor.EndsWith("\u0040dgepms.com") && persona.Supervisor == usuario && planeacion.FechaProximoContacto != null && planeacion.FechaProximoContacto < fechaControl && supervision.EstadoSupervision == "VIGENTE"
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = persona,
                                                 supervisionVM = supervision,
                                                 municipiosVM = municipio,
                                                 causapenalVM = causapenal,
                                                 planeacionestrategicaVM = planeacion,
                                                 tipoAdvertencia = "Se paso el tiempo de la firma"
                                             }).Union
                                            (where2);
                        /*.Union
                        (from persona in personaVM
                         join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                         join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                         join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                         join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                         join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                         where persona.Supervisor == usuario && supervision.EstadoSupervision == null
                         select new PlaneacionWarningViewModel
                         {
                             personaVM = persona,
                             supervisionVM = supervision,
                             municipiosVM = municipio,
                             causapenalVM = causapenal,
                             planeacionestrategicaVM = planeacion,
                             tipoAdvertencia = "Sin estado de supervisión"
                         })*/
                        break;
                    case "EXPEDIENTE FISICO EN RESGUARDO":
                        ViewData["alertas"] = archivo;
                        break;
                    case "INFORME FUERA DE TIEMPO":
                        ViewData["alertas"] = from persona in personaVM
                                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                              join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                              where persona.Supervisor == usuario && planeacion.FechaInforme != null && planeacion.FechaInforme < fechaInformeCoordinador && supervision.EstadoSupervision == "VIGENTE" && fracciones.FiguraJudicial == "SCP"
                                              orderby planeacion.FechaInforme
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  supervisionVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalVM = causapenal,
                                                  planeacionestrategicaVM = planeacion,
                                                  fraccionesimpuestasVM = fracciones,
                                                  figuraJudicial = fracciones.FiguraJudicial,
                                                  tipoAdvertencia = "Informe fuera de tiempo"
                                              };
                        break;
                    case "CONTROL DE SUPERVISION A 3 DIAS O MENOS":
                        ViewData["alertas"] = from persona in personaVM
                                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                              join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                              where persona.Supervisor == usuario && planeacion.FechaInforme != null && planeacion.FechaInforme < fechaControl && supervision.EstadoSupervision == "VIGENTE" && fracciones.FiguraJudicial == "MC"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  supervisionVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalVM = causapenal,
                                                  planeacionestrategicaVM = planeacion,
                                                  fraccionesimpuestasVM = fracciones,
                                                  figuraJudicial = fracciones.FiguraJudicial,
                                                  tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                              };
                        break;
                    case "SIN FECHA DE INFORME":
                        ViewData["alertas"] = from persona in personaVM
                                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                              join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                              where persona.Supervisor == usuario && planeacion.FechaInforme == null && supervision.EstadoSupervision == "VIGENTE"
                                              orderby fracciones.FiguraJudicial
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  supervisionVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalVM = causapenal,
                                                  planeacionestrategicaVM = planeacion,
                                                  fraccionesimpuestasVM = fracciones,
                                                  figuraJudicial = fracciones.FiguraJudicial,
                                                  tipoAdvertencia = "Sin fecha de informe"
                                              };
                        break;
                    case "SIN PERIODICIDAD DE FIRMA":
                        ViewData["alertas"] = from persona in personaVM
                                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                              where persona.Supervisor == usuario && planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  supervisionVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalVM = causapenal,
                                                  planeacionestrategicaVM = planeacion,
                                                  tipoAdvertencia = "Sin periodicidad de firma"
                                              };
                        break;
                    case "SIN SUPERVISION":
                        ViewData["alertas"] = where2;
                        break;
                    case "SE PASO EL TIEMPO DE LA FIRMA":
                        ViewData["alertas"] = from persona in personaVM
                                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                              where persona.Supervisor != null && persona.Supervisor.EndsWith("\u0040dgepms.com") && persona.Supervisor == usuario && planeacion.FechaProximoContacto != null && planeacion.FechaProximoContacto < fechaControl && supervision.EstadoSupervision == "VIGENTE"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  supervisionVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalVM = causapenal,
                                                  planeacionestrategicaVM = planeacion,
                                                  tipoAdvertencia = "Se paso el tiempo de la firma"
                                              };
                        break;
                        //case "SIN ESTADO DE SUPERVISION":
                        //    ViewData["alertas"] = from persona in personaVM
                        //                          join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                        //                          join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                        //                          join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                        //                          join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                        //                          join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                        //                          where persona.Supervisor == usuario && supervision.EstadoSupervision == null
                        //                          select new PlaneacionWarningViewModel
                        //                          {
                        //                              personaVM = persona,
                        //                              supervisionVM = supervision,
                        //                              municipiosVM = municipio,
                        //                              causapenalVM = causapenal,
                        //                              planeacionestrategicaVM = planeacion,
                        //                              tipoAdvertencia = "Sin estado de supervisión"
                        //                          };
                        //    break;
                }
            }
            #endregion
            return Json(new
            {
                success = true,
                user = usuario,
                admin = flagCoordinador || flagMaster,
                //ViewData["alertas"] se usa como variable de esta funcion y no sirve como ViewData
                query = ViewData["alertas"]
            });
        }
        #endregion

        #endregion

        #region -Borrar-
        // GET: Personas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona
                .SingleOrDefaultAsync(m => m.IdPersona == id);
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.IdPersona == id);
            _context.Persona.Remove(persona);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> deleteSustancia()
        {
            var sustancia = await _context.Consumosustancias.SingleOrDefaultAsync(m => m.IdConsumoSustancias == consumosustancias[contadorSustancia - 1].IdConsumoSustancias);
            _context.Consumosustancias.Remove(sustancia);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (contadorSustancia == consumosustancias.Count)
            {
                if (datosSustancias.Count + datosSustanciasEditadas.Count == 0)
                {
                    return Json(new { success = true, responseText = "Ya No" });
                }
                else
                {
                    return Json(new { success = true, responseText = "Datos Guardados con éxito" });
                }
            }
            else
            {
                return Json(new
                {
                    success = true,
                    responseText = "Siguiente",
                    sustancia = consumosustancias[contadorSustancia].Sustancia,
                    frecuencia = consumosustancias[contadorSustancia].Frecuencia,
                    cantidad = consumosustancias[contadorSustancia].Cantidad,
                    ultimoConsumo = consumosustancias[contadorSustancia].UltimoConsumo,
                    observacionesConsumo = consumosustancias[contadorSustancia].Observaciones,
                    idConsumoSustancias = consumosustancias[contadorSustancia++].IdConsumoSustancias
                });
            }
        }

        public async Task<IActionResult> deleteFamiliar(int tipoGuardado)
        {
            if (tipoGuardado == 1)
            {
                var asientoFamiliar = await _context.Asientofamiliar.SingleOrDefaultAsync(m => m.IdAsientoFamiliar == familiares[contadorFamiliares - 1].IdAsientoFamiliar);
                _context.Asientofamiliar.Remove(asientoFamiliar);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (contadorFamiliares == familiares.Count)
                {
                    if (datosFamiliares.Count + datosFamiliaresEditados.Count == 0)
                    {
                        return Json(new { success = true, responseText = "Ya No" });
                    }
                    else
                    {
                        return Json(new { success = true, responseText = "Datos Guardados con éxito" });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        responseText = "Siguiente",
                        nombre = familiares[contadorFamiliares].Nombre,
                        relacion = familiares[contadorFamiliares].Relacion,
                        edad = familiares[contadorFamiliares].Edad,
                        sexo = familiares[contadorFamiliares].Sexo,
                        dependencia = familiares[contadorFamiliares].Dependencia,
                        explicaDependencia = familiares[contadorFamiliares].DependenciaExplica,
                        vivenJuntos = familiares[contadorFamiliares].VivenJuntos,
                        direccion = familiares[contadorFamiliares].Domicilio,
                        telefono = familiares[contadorFamiliares].Telefono,
                        horarioLocalizacion = familiares[contadorFamiliares].HorarioLocalizacion,
                        enteradoProceso = familiares[contadorFamiliares].EnteradoProceso,
                        puedeEnterarse = familiares[contadorFamiliares].PuedeEnterarse,
                        observaciones = familiares[contadorFamiliares].Observaciones,
                        idAsientoFamiliar = familiares[contadorFamiliares++].IdAsientoFamiliar
                    });
                }
            }
            else
            {
                var asientoFamiliar = await _context.Asientofamiliar.SingleOrDefaultAsync(m => m.IdAsientoFamiliar == referenciaspersonales[contadorReferencias - 1].IdAsientoFamiliar);
                _context.Asientofamiliar.Remove(asientoFamiliar);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (contadorReferencias == referenciaspersonales.Count)
                {
                    if (datosReferencias.Count + datosReferenciasEditadas.Count == 0)
                    {
                        return Json(new { success = true, responseText = "Ya No" });
                    }
                    else
                    {
                        return Json(new { success = true, responseText = "Datos Guardados con éxito" });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        responseText = "Siguiente",
                        nombre = referenciaspersonales[contadorReferencias].Nombre,
                        relacion = referenciaspersonales[contadorReferencias].Relacion,
                        edad = referenciaspersonales[contadorReferencias].Edad,
                        sexo = referenciaspersonales[contadorReferencias].Sexo,
                        dependencia = referenciaspersonales[contadorReferencias].Dependencia,
                        explicaDependencia = referenciaspersonales[contadorReferencias].DependenciaExplica,
                        vivenJuntos = referenciaspersonales[contadorReferencias].VivenJuntos,
                        direccion = referenciaspersonales[contadorReferencias].Domicilio,
                        telefono = referenciaspersonales[contadorReferencias].Telefono,
                        horarioLocalizacion = referenciaspersonales[contadorReferencias].HorarioLocalizacion,
                        enteradoProceso = referenciaspersonales[contadorReferencias].EnteradoProceso,
                        puedeEnterarse = referenciaspersonales[contadorReferencias].PuedeEnterarse,
                        observaciones = referenciaspersonales[contadorReferencias].Observaciones,
                        idAsientoFamiliar = referenciaspersonales[contadorReferencias++].IdAsientoFamiliar
                    });
                }
            }
        }
        #endregion

        #region -BitmapToBytes-
        private static MemoryStream BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream;
            }
        }
        #endregion

        #region -PersonaExists-
        private bool PersonaExists(int id)
        {
            return _context.Persona.Any(e => e.IdPersona == id);
        }
        #endregion

        #region -obtenerDatos-
        public ActionResult OnGetChartData()
        {

            var supervisoresScorpio = from p in _context.Persona
                                      group p by p.Supervisor into grup
                                      select new
                                      {
                                          grup.Key,
                                          Count = grup.Count()
                                      }
                          ;

            var supervisoresBD = from c in _context.Controlsupervisiones
                                 select new
                                 {
                                     c.Supervisor,
                                     c.Supervisados
                                 };

            var result = (from s in supervisoresScorpio
                          join b in supervisoresBD on s.Key equals b.Supervisor
                          select new
                          {
                              Supervisor = ((b.Supervisor).ToString()).Substring(0, ((b.Supervisor).ToString()).IndexOf("@")),
                              Supervisados = s.Count + b.Supervisados
                          }).ToList();

            var json = result.ToGoogleDataTable()
            .NewColumn(new Column(ColumnType.String, "Supervisor"), x => x.Supervisor)
            .NewColumn(new Column(ColumnType.Number, "Supervisiones"), x => x.Supervisados)
            .Build()
            .GetJson();

            return Content(json);
        }
        #endregion

        #region -Actualizar Candado All-
        public JsonResult LoockCandado(Persona persona, string[] datoCandado)
        //public async Task<IActionResult> LoockCandado(Persona persona, string[] datoCandado)
        {
            persona.Candado = Convert.ToSByte(datoCandado[0] == "true");
            persona.IdPersona = Int32.Parse(datoCandado[1]);
            persona.MotivoCandado = datoCandado[2];

            var empty = (from p in _context.Persona
                         where p.IdPersona == persona.IdPersona
                         select p);

            if (empty.Any())
            {
                var query = (from p in _context.Persona
                             where p.IdPersona == persona.IdPersona
                             select p).FirstOrDefault();
                query.Candado = persona.Candado;
                query.MotivoCandado = persona.MotivoCandado;
                _context.SaveChanges();
            }
            var stadoc = (from p in _context.Persona
                          where p.IdPersona == persona.IdPersona
                          select p.Candado).FirstOrDefault();
            //return View();

            return Json(new { success = true, responseText = Convert.ToString(stadoc), idPersonas = Convert.ToString(persona.IdPersona) });
        }
        public JsonResult getEstadodeCanadado(int id)
        {
            //IEnumerable<Persona> shops = _context.Persona;
            //return Json(shops.Select(u => new { u.Candado, u.IdPersona }).Where(u => u.IdPersona == id));

            var stadoc = (from p in _context.Persona
                          where p.IdPersona == id
                          select p.Candado);

            return Json(stadoc);
        }
        #endregion

        #region -JsonA-ll-
        public async Task<IActionResult> Get(string sortOrder,
            string currentFilter,
            string Search,
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

            ViewBag.RolesUsuario = rolUsuario[1];

            String users = user.ToString();
            ViewBag.RolesUsuarios = users;
            #endregion


            //List<Persona> personas = _context.Persona.ToList();
            //var personas = _context.Persona;
            //var pagedData = PaginatedList<Persona>.CreateAsync(personas, pageIndex, pageSize);

            List<Persona> listaSupervisados = new List<Persona>();
            listaSupervisados = (from table in _context.Persona
                                 select table).ToList();
            listaSupervisados.Insert(0, new Persona { IdPersona = 0, Supervisor = "Selecciona" });
            ViewBag.listaSupervisados = listaSupervisados;




            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";



            if (Search != null)
            {
                pageNumber = 1;
            }
            else
            {
                Search = currentFilter;
            }
            ViewData["CurrentFilter"] = Search;

            var personas = from p in _context.Persona
                           where p.Supervisor != null
                           select p;


            if (!String.IsNullOrEmpty(Search))
            {
                foreach (var item in Search.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(Search) ||
                                                   (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(Search) ||
                                                   p.Supervisor.Contains(Search) || (p.IdPersona.ToString()).Contains(Search));

                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
                default:
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
            }

            int pageSize = 10;
            // Response.Headers.Add("Refresh", "5");
            return Json(new
            {
                page = await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize),
                totalPages = (personas.Count() + pageSize - 1) / pageSize
            });

            //return Json(new
            //{
            //    success = true,
            //    responseText = "Siguiente",
            //    sustancia = consumosustancias[contadorSustancia].Sustancia,
            //    frecuencia = consumosustancias[contadorSustancia].Frecuencia,
            //    cantidad = consumosustancias[contadorSustancia].Cantidad,
            //    ultimoConsumo = consumosustancias[contadorSustancia].UltimoConsumo,
            //    observacionesConsumo = consumosustancias[contadorSustancia].Observaciones,
            //    idConsumoSustancias = consumosustancias[contadorSustancia++].IdConsumoSustancias
            //});

        }

        public async Task<IActionResult> GetBusqueda(string searchValue,
            string sortOrder,
            string currentFilter,
            int? pageNumber)
        {
            //List<Persona> persona = new List<Persona>();
            //persona = _context.Persona.Where(x => (x.Nombre +" "+x.Paterno+ " " + x.Materno).Contains(searchValue)|| searchValue == null).ToList();
            //return Json(persona);

            if (searchValue != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchValue = currentFilter;
            }
            ViewData["CurrentFilter"] = searchValue;

            var personas = from p in _context.Persona
                           where p.Supervisor != null
                           select p;
            if (!String.IsNullOrEmpty(searchValue))
            {
                foreach (var item in searchValue.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(searchValue) ||
                                                    (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(searchValue) ||
                                                    p.Supervisor.Contains(searchValue) || (p.IdPersona.ToString()).Contains(searchValue));

                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
                default:
                    personas = personas.OrderBy(p => p.IdPersona);
                    break;
            }

            int pageSize = 10;

            return Json(await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        #endregion

        #region -Listado Supervisados -
        public IActionResult SupervisadosList(Persona persona)
        {
            //datosSustancias.Clear();            
            List<Persona> listaSupervisados = new List<Persona>();
            listaSupervisados = (from table in _context.Persona
                                 select table).ToList();
            listaSupervisados.Insert(0, new Persona { IdPersona = 0, Supervisor = "Selecciona" });
            ViewBag.listaSupervisados = listaSupervisados;
            return View();
        }


        #endregion

        #region -ArchivoInternoMCSCP-
        public async Task<IActionResult> ArchivoPrestamo(
           string sortOrder,
           string currentFilter,
           string SearchString,
           string estadoSuper,
           int? pageNumber
           )
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CausaPenalSortParm"] = String.IsNullOrEmpty(sortOrder) ? "causa_penal_desc" : "";
            ViewData["EstadoCumplimientoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "estado_cumplimiento_desc" : "";

            if (SearchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                SearchString = currentFilter;
            }


            List<Archivointernomcscp> queryHistorialArchivo = (from a in _context.Archivointernomcscp
                                                               group a by a.PersonaIdPersona into grp
                                                               select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();

            var filter = from p in _context.Persona
                         join a in queryHistorialArchivo on p.IdPersona equals a.PersonaIdPersona
                         where a.NuevaUbicacion != "NO UBICADO" && a.NuevaUbicacion != "ARCHIVO GENERAL" && a.NuevaUbicacion != "ARCHIVO INTERNO"  && a.NuevaUbicacion != "SIN REGISTRO" && a.NuevaUbicacion != null
                         select new ArchivoPersona
                         {
                             archivointernomcscpVM = a,
                             personaVM = p,
                         };

            var count = filter.Count();

            ViewData["CurrentFilter"] = SearchString;
            ViewData["EstadoS"] = estadoSuper;

            if (!String.IsNullOrEmpty(SearchString))
            {
                filter = filter.Where(a => (a.personaVM.Paterno + " " + a.personaVM.Materno + " " + a.personaVM.Nombre).Contains(SearchString.ToUpper()) ||
                                              (a.personaVM.Nombre + " " + a.personaVM.Paterno + " " + a.personaVM.Materno).Contains(SearchString.ToUpper()) ||
                                              (a.personaVM.IdPersona.ToString()).Contains(SearchString)
                                              );

            }





            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(a => a.personaVM.Paterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.CausaPenal);
                    break;
                case "fechaa_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.Fecha);
                    break;
                default:
                    filter = filter.OrderBy(spcp => spcp.personaVM.Paterno);
                    break;
            }


            List<SelectListItem> ListaUbicacion = new List<SelectListItem>();
            int ii = 0;
            ListaUbicacion.Add(new SelectListItem { Text = "Sin Registro", Value = "Sin Registro" });
            ListaUbicacion.Add(new SelectListItem { Text = "Archivo Interno", Value = "Archivo Interno" });
            ListaUbicacion.Add(new SelectListItem { Text = "Archivo General", Value = "Archivo General" });
            ListaUbicacion.Add(new SelectListItem { Text = "No Ubicado", Value = "No Ubicado" });
            ListaUbicacion.Add(new SelectListItem { Text = "Dirección", Value = "Dirección" });
            ListaUbicacion.Add(new SelectListItem { Text = "Coordinación Operativa", Value = "Coordinación Operativa" });
            ListaUbicacion.Add(new SelectListItem { Text = "Coordinación MC y SCP", Value = "Coordinación MC y SCP" });

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, "SupervisorMCSCP"))
                {

                    ListaUbicacion.Add(new SelectListItem
                    {
                        Text = user.ToString(),
                        Value = ii.ToString()
                    });
                }
            }

            ViewBag.ListaUbicacion = ListaUbicacion;

            int pageSize = 10;

            //var queryable = query2.AsQueryable();
            return View(await PaginatedList<ArchivoPersona>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        #region -Update Ubicación archivo y causa penal-
        public JsonResult UpdateUyCP(Archivointernomcscp archivointernomcscp, Persona persona, string cambioCP, string idArchivo, string cambioUE, string idpersona, string archivoid)
        //public async Task<IActionResult> LoockCandado(Persona persona, string[] datoCandado)
        {

            //#region -Actualizar causa penal-
            //if (idArchivo != null)
            //{
            //    archivointernomcscp.CausaPenal = cambioCP;
            //    archivointernomcscp.IdarchivoInternoMcscp = Int32.Parse(idArchivo);
            //}
            //#endregion
            //var empty = (from a in _context.Archivointernomcscp
            //             where a.IdarchivoInternoMcscp == archivointernomcscp.IdarchivoInternoMcscp
            //             select a);

            //if (empty.Any())
            //{
            //    var query = (from a in _context.Archivointernomcscp
            //                 where a.IdarchivoInternoMcscp == archivointernomcscp.IdarchivoInternoMcscp
            //                 select a).FirstOrDefault();
            //    query.CausaPenal = archivointernomcscp.CausaPenal;
            //    _context.SaveChanges();
            //}

            #region -Actualizar Ubicacion-
            if (idpersona != null)
            {
                archivointernomcscp.IdarchivoInternoMcscp = Int32.Parse(archivoid);
                persona.IdPersona = Int32.Parse(idpersona);
                persona.UbicacionExpediente = normaliza(cambioUE);
            }
            #endregion

            var emptypersona = (from p in _context.Persona
                                where p.IdPersona == persona.IdPersona
                                select p);

            if (emptypersona.Any())
            {
                var query = (from p in _context.Persona
                             where p.IdPersona == persona.IdPersona
                             select p).FirstOrDefault();
                query.UbicacionExpediente = persona.UbicacionExpediente;
                _context.SaveChanges();
            }

            var cp = (from a in _context.Persona
                      where a.IdPersona == persona.IdPersona
                      select a.UbicacionExpediente).FirstOrDefault();


            //return View();

            return Json(new { success = true, responseText = Convert.ToString(cp), idPersonas = Convert.ToString(archivointernomcscp.IdarchivoInternoMcscp) });
        }
        #endregion -Update Ubicación archivo y causa penal-

        #endregion

        #region -ArchivoHistorial-
        public async Task<IActionResult> ArchivoHistorial(
           string sortOrder,
           string currentFilter,
           string SearchString,
           string estadoSuper,
           int? pageNumber
           )
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CausaPenalSortParm"] = String.IsNullOrEmpty(sortOrder) ? "causa_penal_desc" : "";
            ViewData["EstadoCumplimientoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "estado_cumplimiento_desc" : "";

            if (SearchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                SearchString = currentFilter;
            }


            //List<Archivointernomcscp> queryHistorialArchivo = (from a in _context.Archivointernomcscp
            //                                                   group a by a.PersonaIdPersona into grp
            //                                                   select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();

            //var filter = from p in _context.Persona
            //             join a in queryHistorialArchivo on p.IdPersona equals a.PersonaIdPersona
            //             select new ArchivoPersona
            //             {
            //                 archivointernomcscpVM = a,
            //                 personaVM = p,
            //             };

            var filter = from p in _context.Persona
                         join a in _context.Archivointernomcscp on p.IdPersona equals a.PersonaIdPersona
                         where a.NuevaUbicacion != null
                         select new ArchivoPersona
                         {
                             archivointernomcscpVM = a,
                             personaVM = p
                         };

            ViewData["CurrentFilter"] = SearchString;
            ViewData["EstadoS"] = estadoSuper;

            if (!String.IsNullOrEmpty(SearchString))
            {
                filter = filter.Where(a => (a.personaVM.Paterno + " " + a.personaVM.Materno + " " + a.personaVM.Nombre).Contains(SearchString) ||
                                              (a.personaVM.Nombre + " " + a.personaVM.Paterno + " " + a.personaVM.Materno).Contains(SearchString) ||
                                              (a.personaVM.IdPersona.ToString()).Contains(SearchString)
                                              );

            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(a => a.personaVM.Paterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.CausaPenal);
                    break;
                case "fechaa_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.Fecha);
                    break;
                default:
                    filter = filter.OrderByDescending(spcp => spcp.archivointernomcscpVM.Fecha);
                    break;
            }
            int pageSize = 100;

            //var queryable = query2.AsQueryable();
            return View(await PaginatedList<ArchivoPersona>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        #endregion

        #region -ArchivoNoUbicado-
        public async Task<IActionResult> ArchivoNoUbicado(
           string sortOrder,
           string currentFilter,
           string SearchString,
           string estadoSuper,
           int? pageNumber
           )
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CausaPenalSortParm"] = String.IsNullOrEmpty(sortOrder) ? "causa_penal_desc" : "";
            ViewData["EstadoCumplimientoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "estado_cumplimiento_desc" : "";

            if (SearchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                SearchString = currentFilter;
            }

            var filter = from p in _context.Persona
                         join a in _context.Archivointernomcscp on p.IdPersona equals a.PersonaIdPersona
                         where a.NuevaUbicacion == "No Ubicado"
                         select new ArchivoPersona
                         {
                             archivointernomcscpVM = a,
                             personaVM = p
                         };

            ViewData["CurrentFilter"] = SearchString;
            ViewData["EstadoS"] = estadoSuper;

            if (!String.IsNullOrEmpty(SearchString))
            {
                filter = filter.Where(a => (a.personaVM.Paterno + " " + a.personaVM.Materno + " " + a.personaVM.Nombre).Contains(SearchString) ||
                                              (a.personaVM.Nombre + " " + a.personaVM.Paterno + " " + a.personaVM.Materno).Contains(SearchString) ||
                                              (a.personaVM.IdPersona.ToString()).Contains(SearchString)
                                              );

            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(a => a.personaVM.Paterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.CausaPenal);
                    break;
                case "fechaa_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.Fecha);
                    break;
                default:
                    filter = filter.OrderBy(spcp => spcp.personaVM.Paterno);
                    break;
            }
            int pageSize = 10;

            //var queryable = query2.AsQueryable();
            return View(await PaginatedList<ArchivoPersona>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        #endregion


        public IActionResult MenuArchivoMCySCP()
        {
            return View();
        }
    }
}