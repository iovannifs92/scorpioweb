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


        // GET: Personas
        public async Task<IActionResult> Index(
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
           
            String users = user.ToString();
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
                    personas = personas.Where(p => p.Paterno.StartsWith(searchString)
                                        || p.Materno.StartsWith(searchString)
                                        || p.Nombre.StartsWith(searchString));
                }
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
                    personas = personas.Where(p => p.Paterno.StartsWith(searchString)
                                        || p.Materno.StartsWith(searchString)
                                        || p.Nombre.StartsWith(searchString));
                }
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

        public async Task<IActionResult> MenuMCSCP()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            List<string> rolUsuario = new List<string>();

            for (int i = 0; i < roles.Count; i++)
            {
                rolUsuario.Add(roles[i]);
            }

            ViewBag.RolesUsuario = rolUsuario;
            return View();
        }

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

        public ActionResult guardarSustancia(string[] datosConsumo)
        {
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosConsumo.Length; i++)
            {
                datosSustancias.Add(new List<String> { datosConsumo[i], currentUser });
            }

            return Json(new { success = true, responseText = "Datos Guardados con éxito" });

        }

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
                    observacionesConsumo = consumosustancias[contadorSustancia++].Observaciones
                });
            }
        }

        public ActionResult editarSustancias()
        {
            contadorSustancia = 1;//por cargar la 1er sustancia
            datosSustanciasEditadas = new List<List<string>>();//por si no se vacian las listas despues de guardar

            return Json(new { success = true });
        }

        public ActionResult agregarSustancias()
        {
            datosSustancias = new List<List<string>>();//por si no se vacian las listas despues de guardar el modal

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
                        observaciones = familiares[contadorFamiliares++].Observaciones
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
                        observaciones = referenciaspersonales[contadorReferencias++].Observaciones
                    });
                }
            }
        }

        public ActionResult editarFamiliares(int tipoGuardado)
        {
            if (tipoGuardado == 1)
            {
                contadorFamiliares = 1;
                datosFamiliaresEditados = new List<List<string>>();
            }
            else if (tipoGuardado == 2)
            {
                contadorReferencias = 1;
                datosReferenciasEditadas = new List<List<string>>();
            }
            return Json(new { success = true });
        }

        public ActionResult agregarAsientoFamiliar(int tipo)
        {
            if (tipo == 1)
            {
                datosFamiliares = new List<List<string>>();
            }
            else if (tipo == 2)
            {
                datosReferenciasEditadas = new List<List<string>>();
            }
            return Json(new { success = true });
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

        public ActionResult agregarFamiliaresExtranjeros()
        {
            datosFamiliaresExtranjero = new List<List<string>>();

            return Json(new { success = true });
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Persona persona, Domicilio domicilio, Estudios estudios, Trabajo trabajo, Actividadsocial actividadsocial, Abandonoestado abandonoEstado, Saludfisica saludfisica, Domiciliosecundario domiciliosecundario, Consumosustancias consumosustanciasBD, Asientofamiliar asientoFamiliar, Familiaresforaneos familiaresForaneos,
            string nombre, string paterno, string materno, string alias, string sexo, int edad, DateTime fNacimiento, string lnPais,
            string lnEstado, string lnMunicipio, string lnLocalidad, string estadoCivil, string duracion, string otroIdioma, string especifiqueIdioma,
            string leerEscribir, string traductor, string especifiqueTraductor, string telefonoFijo, string celular, string hijos, int nHijos, int nPersonasVive,
            string propiedades, string CURP, string consumoSustancias, string familiares, string referenciasPersonales,
            string tipoDomicilio, string calle, string no, string nombreCF, string paisD, string estadoD, string municipioD, string temporalidad,
            string residenciaHabitual, string cp, string referencias, string horario, string observaciones, string cuentaDomicilioSecundario,
            string motivoDS, string tipoDomicilioDS, string calleDS, string noDS, string nombreCFDS, string paisDDS, string estadoDDS, string municipioDDS, string temporalidadDS,
            string residenciaHabitualDS, string cpDS, string referenciasDS, string horarioDS, string observacionesDS,
            string estudia, string gradoEstudios, string institucionE, string horarioE, string direccionE, string telefonoE, string observacionesE,
            string trabaja, string tipoOcupacion, string puesto, string empleadorJefe, string enteradoProceso, string sePuedeEnterar, string tiempoTrabajando,
            string salario, string temporalidadSalario, string direccionT, string horarioT, string telefonoT, string observacionesT,
            string tipoActividad, string horarioAS, string lugarAS, string telefonoAS, string sePuedeEnterarAS, string referenciaAS, string observacionesAS,
            string vividoFuera, string lugaresVivido, string tiempoVivido, string motivoVivido, string viajaHabitual, string lugaresViaje, string tiempoViaje,
            string motivoViaje, string documentaciónSalirPais, string pasaporte, string visa, string familiaresFuera,
            string enfermedad, string especifiqueEnfermedad, string embarazoLactancia, string tiempoEmbarazo, string tratamiento, string discapacidad, string especifiqueDiscapacidad,
            string servicioMedico, string especifiqueServicioMedico, string institucionServicioMedico, string observacionesSalud, string capturista,
            IFormFile fotografia)
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
                persona.UltimaActualización = DateTime.Now;
                persona.Capturista = currentUser;
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
                domicilio.Referencias = normaliza(referencias);
                domicilio.DomcilioSecundario = cuentaDomicilioSecundario;
                domicilio.Horario = normaliza(horario);
                domicilio.Observaciones = normaliza(observaciones);
                #endregion

                #region -Domicilio Secundario-
                domiciliosecundario.Motivo = motivoDS;
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
                domiciliosecundario.Observaciones = normaliza(observacionesDS);
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
                domiciliosecundario.IdDomicilio = idDomicilio;
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
                for (int i = 0; i < datosSustancias.Count; i = i + 5)
                {
                    if (datosSustancias[i][1] == currentUser)
                    { /*Revisar el cambio de variable "iovanni" por a variable de usuario*/
                        consumosustanciasBD.Sustancia = datosSustancias[i][0];
                        consumosustanciasBD.Frecuencia = datosSustancias[i + 1][0];
                        consumosustanciasBD.Cantidad = normaliza(datosSustancias[i + 2][0]);
                        consumosustanciasBD.UltimoConsumo = validateDatetime(datosSustancias[i + 3][0]);
                        consumosustanciasBD.Observaciones = normaliza(datosSustancias[i + 4][0]);
                        consumosustanciasBD.PersonaIdPersona = idPersona;
                        _context.Add(consumosustanciasBD);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
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
                for (int i = 0; i < datosFamiliares.Count; i = i + 13)
                {
                    if (datosFamiliares[i][1] == currentUser)
                    {
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
                        asientoFamiliar.PersonaIdPersona = idPersona;
                        _context.Add(asientoFamiliar);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
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
                for (int i = 0; i < datosReferencias.Count; i = i + 13)
                {
                    if (datosReferencias[i][1] == currentUser)
                    {
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
                }
                #endregion

                #region -Familiares Extranjero-
                for (int i = 0; i < datosFamiliaresExtranjero.Count; i = i + 12)
                {
                    if (datosFamiliaresExtranjero[i][1] == currentUser)
                    {
                        familiaresForaneos.Nombre = normaliza(datosFamiliaresExtranjero[i][0]);
                        familiaresForaneos.Relacion = datosFamiliaresExtranjero[i + 1][0];
                        familiaresForaneos.Edad = Int32.Parse(datosFamiliaresExtranjero[i + 2][0]);
                        familiaresForaneos.Sexo = datosFamiliaresExtranjero[i + 3][0];
                        familiaresForaneos.TiempoConocerlo = datosFamiliaresExtranjero[i + 4][0];
                        familiaresForaneos.Pais = datosFamiliaresExtranjero[i + 5][0];
                        familiaresForaneos.Estado = normaliza(datosFamiliaresExtranjero[i + 6][0]);
                        familiaresForaneos.Telefono = datosFamiliaresExtranjero[i + 7][0];
                        familiaresForaneos.FrecuenciaContacto = datosFamiliaresExtranjero[i + 8][0];
                        familiaresForaneos.EnteradoProceso = datosFamiliaresExtranjero[i + 9][0];
                        familiaresForaneos.PuedeEnterarse = datosFamiliaresExtranjero[i + 10][0];
                        familiaresForaneos.Observaciones = normaliza(datosFamiliaresExtranjero[i + 11][0]);
                        familiaresForaneos.PersonaIdPersona = idPersona;
                        _context.Add(familiaresForaneos);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                    }
                }

                for (int i = 0; i < datosFamiliaresExtranjero.Count; i++)
                {
                    if (datosFamiliaresExtranjero[i][1] == currentUser)
                    {
                        datosFamiliaresExtranjero.RemoveAt(i);
                        i--;
                    }
                }
                #endregion

                #region -Guardar Foto-
                if(fotografia != null)
                {
                  string file_name = persona.IdPersona + "_" + persona.Paterno + "_" + persona.Nombre + ".jpg";
                  persona.rutaFoto = file_name;
                  var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Fotos");
                  var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                  await fotografia.CopyToAsync(stream);
                }
                #endregion

                #region -Añadir a contexto-
                _context.Add(persona);
                _context.Add(domicilio);
                _context.Add(domiciliosecundario);
                _context.Add(estudios);
                _context.Add(trabajo);
                _context.Add(actividadsocial);
                _context.Add(abandonoEstado);
                _context.Add(saludfisica);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                return RedirectToAction(nameof(Index));
                #endregion
            }
            return RedirectToAction("ListadoSupervisor", "Personas");
        }

        #endregion

        #region -Entrevista-
        public ActionResult Entrevista()
        {
            var personas = from p in _context.Persona
                           where p.Supervisor != null
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
            string currentUser = User.Identity.Name;
            if (ModelState.IsValid)
            {
                return RedirectToAction("MenuEdicion", "Personas", new { @id = idPersona });
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
            List<Causapenal> causaPenalVM = _context.Causapenal.ToList();
            List<Persona> personaVM = _context.Persona.ToList();
            #region -Jointables-
            ViewData["joinTablesSupervision"] = from supervisiontable in SupervisionVM
                                                join personatable in personaVM on supervisiontable.PersonaIdPersona equals personatable.IdPersona
                                                join causapenaltable in causaPenalVM on supervisiontable.CausaPenalIdCausaPenal equals causapenaltable.IdCausaPenal
                                                where personatable.IdPersona == id

                                                select new SupervisionPyCP
                                                {
                                                    causapenalVM = causapenaltable,
                                                    supervisionVM = supervisiontable,
                                                    personaVM = personatable
                                                };
            #endregion

            return View();
        }

        public ActionResult SinSupervision()
        {
            return View();
        }

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
            ViewBag.pais = persona.Lnpais;
            ViewBag.idioma = persona.OtroIdioma;
            ViewBag.traductor = persona.Traductor;
            ViewBag.Hijos = persona.Hijos;
            ViewBag.conSustancia = persona.ConsumoSustancias;

            #region Consume sustancias
            ViewBag.listaConsumoSustancias = listaNoSi;
            ViewBag.idConsumoSustancias = BuscaId(listaNoSi, persona.ConsumoSustancias);

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
        public async Task<IActionResult> Edit(int id, [Bind("IdPersona,Nombre,Paterno,Materno,Alias,Genero,Edad,Fnacimiento,Lnpais,Lnestado,Lnmunicipio,Lnlocalidad,EstadoCivil,Duracion,OtroIdioma,EspecifiqueIdioma,DatosGeneralescol,LeerEscribir,Traductor,EspecifiqueTraductor,TelefonoFijo,Celular,Hijos,Nhijos,NpersonasVive,Propiedades,Curp,ConsumoSustancias,Familiares,ReferenciasPersonales,UltimaActualización,Supervisor,rutaFoto,Capturista")] Persona persona)
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

                #region -ConsumoSustancias-
                //Sustancias editadas
                for (int i = 0; i < datosSustanciasEditadas.Count; i = i + 5)
                {
                    if (datosSustanciasEditadas[i][1] == currentUser)
                    {
                        Consumosustancias consumosustanciasBD = new Consumosustancias();

                        consumosustanciasBD.IdConsumoSustancias = consumosustancias[i / 5].IdConsumoSustancias;
                        consumosustanciasBD.Sustancia = datosSustanciasEditadas[i][0];
                        consumosustanciasBD.Frecuencia = datosSustanciasEditadas[i + 1][0];
                        consumosustanciasBD.Cantidad = normaliza(datosSustanciasEditadas[i + 2][0]);
                        consumosustanciasBD.UltimoConsumo = validateDatetime(datosSustanciasEditadas[i + 3][0]);
                        consumosustanciasBD.Observaciones = normaliza(datosSustanciasEditadas[i + 4][0]);
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
                        await _context.SaveChangesAsync(null,1);
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
                for (int i = 0; i < datosFamiliaresEditados.Count; i = i + 13)
                {
                    if (datosFamiliaresEditados[i][1] == currentUser)
                    {
                        Asientofamiliar asientoFamiliar = new Asientofamiliar();

                        asientoFamiliar.IdAsientoFamiliar = familiares[i / 13].IdAsientoFamiliar;
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
                for (int i = 0; i < datosReferenciasEditadas.Count; i = i + 13)
                {
                    if (datosReferenciasEditadas[i][1] == currentUser)
                    {
                        Asientofamiliar asientoFamiliar = new Asientofamiliar();

                        asientoFamiliar.IdAsientoFamiliar = referenciaspersonales[i / 13].IdAsientoFamiliar;
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
        public async Task<IActionResult> EditDomicilio(int id, [Bind("IdDomicilio,TipoDomicilio,Calle,No,TipoUbicacion,NombreCf,Pais,Estado,Municipio,Temporalidad,ResidenciaHabitual,Cp,Referencias,Horario,DomcilioSecundario,Observaciones,PersonaIdPersona")] Domicilio domicilio)
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
            ViewBag.listaEstudia = listaSiNo;
            ViewBag.idEstudia = BuscaId(listaSiNo, estudios.Estudia);

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
            //ViewBag.Delitos = ((ViewData["joinTablesCausaDelito"] as IEnumerable<scorpioweb.Models.CausaDelitoViewModel>).Count()).ToString();




            if ((ViewData["joinTablesPersonaEstudia"] as IEnumerable<scorpioweb.Models.PersonaViewModel>).Count() == 0)
            {
                ViewBag.RA = false;
            }
            else
            {
                ViewBag.RA = true;
            }

            //List<SelectListItem> ListaTrueFalse;
            //ListaTrueFalse = new List<SelectListItem>
            //{
            //  new SelectListItem{ Text="SI", Value="True"},
            //  new SelectListItem{ Text="NO", Value="Flse"}
            //};
            //ViewBag.listaTr = ListaTrueFalse;
            //ViewBag.idGradoEstudios = BuscaId(ListaTrueFalse, ViewBag.RA);

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

        public async Task<IActionResult> DeleteSustancia(int? id)
        {
            var sustancia = await _context.Consumosustancias.SingleOrDefaultAsync(m => m.IdConsumoSustancias == id);
            _context.Consumosustancias.Remove(sustancia);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            return RedirectToAction(nameof(Index));//return RedirectToAction("Supervision/" + fraccionesimpuestas.SupervisionIdSupervision, "Supervisiones");
        }
        #endregion

        private static MemoryStream BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream;
            }
        }

        private bool PersonaExists(int id)
        {
            return _context.Persona.Any(e => e.IdPersona == id);
        }
    }
}