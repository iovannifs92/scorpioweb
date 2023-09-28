using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using F23.StringSimilarity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using scorpioweb.Class;

namespace scorpioweb.Models
{
    public class PersonaclsController : Controller
    {
        #region -Variables Globales-
        //To get content root path of the project
        private readonly IHostingEnvironment _hostingEnvironment;
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
        public static List<Consumosustanciascl> consumosustanciascl;
        public static List<Asientofamiliarcl> familiarescl;
        public static List<Asientofamiliarcl> referenciaspersonalescl;
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
            new SelectListItem{ Text="Sin zona asignada", Value="SIN ZONA ASIGNADA"},
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
        public PersonaclsController(penas2Context context, IHostingEnvironment hostingEnvironment,
                                  RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;

        }
        #endregion


        #region -Metodos Generales-
        MetodosGenerales mg = new MetodosGenerales();
        #region -Bsuacarid-
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
        #region -GetMunicipio-
        public JsonResult GetMunicipio(int EstadoId)
        {
            TempData["message"] = DateTime.Now;
            List<Municipios> municipiosList = new List<Municipios>();

            municipiosList = (from Municipios in _context.Municipios
                              where Municipios.EstadosId == EstadoId
                              select Municipios).ToList();

            return Json(new SelectList(municipiosList, "Id", "Municipio"));
        }
        #endregion
        #region -curp-
        //Curp sin contar homonimos a 17 caracteres
        public JsonResult cursJson(string paterno, string materno, DateTime? fnacimiento, string genero, string lnestado, string nombre)
        {
            var curs = mg.sacaCurs(paterno, materno, fnacimiento, genero, lnestado, nombre);
            return Json(new { success = true, responseText = Convert.ToString(0), curs = curs }); ;
        }
        #endregion
        #region -testSimilitud-
        public JsonResult testSimilitud(string nombre, string paterno, string materno)
        {
            bool simi = false;
            var nombreCompleto = mg.normaliza(paterno) + " " + mg.normaliza(materno) + " " + mg.normaliza(nombre);

            var query = from p in _context.Persona
                        select new
                        {
                            nomcom = p.Paterno + " " + p.Materno + " " + p.Nombre,
                            id = p.IdPersona
                        };

            int idpersona = 0;
            string nomCom = "";
            var cosine = new Cosine(2);
            double r = 0;
            var list = new List<Tuple<string, int, double>>();

            List<string> listaNombre = new List<string>();


            foreach (var q in query)
            {
                r = cosine.Similarity(q.nomcom, nombreCompleto);
                if (r >= 0.87)
                {
                    nomCom = q.nomcom;
                    idpersona = q.id;
                    list.Add(new Tuple<string, int, double>(nomCom, idpersona, r));
                    simi = true;
                }
            }
            if (list.Count() != 0)
            {
                var tupleWithMaxItem1 = list.OrderBy(x => x.Item3).Last();

                if (simi == true)
                {
                    double i = tupleWithMaxItem1.Item3 * 100;
                    int porcentaje = (int)Math.Floor(i);
                    string id = tupleWithMaxItem1.Item2.ToString();
                    return Json(new { success = true, responseText = Url.Action("MenuEdicion/" + id, "Personas"), porcentaje = porcentaje });
                }
            }

            return Json(new { success = false });
        }
        #endregion
        #endregion

        #region -PersonasCL-
        #region -Index-
        // GET: Personacls
        public async Task<IActionResult> Index()
        {
            var nomsuper = User.Identity.Name.ToString();

            ViewBag.RolesUsuarios = nomsuper;
            return View(await _context.Personacl.ToListAsync());
        }

        public async Task<IActionResult> Get(string sortOrder,
            string currentFilter,
            string Search,
            int? pageNumber,
            bool usuario)
        {
            #region -ListaUsuarios-            

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var nomsuper = User.Identity.Name.ToString();
            var roles = await userManager.GetRolesAsync(user);
            bool super = false;
            bool admin = false;

            foreach (var rol in roles)
            {
                if (rol == "AdminLC" || rol == "SupervisiorLC")
                {
                    super = true;
                }
            }
            foreach (var rol in roles)
            {
                if (rol == "Masteradmin")
                {
                    admin = true;
                }
            }

            String users = user.ToString();
            ViewBag.RolesUsuarios = users;
            #endregion
            List<Personacl> listaSupervisados = new List<Personacl>();
            listaSupervisados = (from table in _context.Personacl
                                 select table).ToList();
            listaSupervisados.Insert(0, new Personacl { IdPersonaCl = 0, Supervisor = "Selecciona" });
            ViewBag.listaSupervisados = listaSupervisados;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (Search != null)
            {
                pageNumber = pageNumber;
            }
            else
            {
                Search = currentFilter;
            }
            ViewData["CurrentFilter"] = Search;

            var personas = from p in _context.Personacl
                           where p.Supervisor != null
                           select p;

            if (!String.IsNullOrEmpty(Search))
            {
                foreach (var item in Search.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(Search) ||
                                                   (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(Search) ||
                                                   p.Supervisor.Contains(Search) || (p.IdPersonaCl.ToString()).Contains(Search));

                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.IdPersonaCl);
                    break;
                default:
                    personas = personas.OrderByDescending(p => p.IdPersonaCl);
                    break;
            }
            personas.OrderByDescending(p => p.IdPersonaCl);
            if (usuario == true) {
                personas = personas.Where(p => p.Supervisor == nomsuper);
            };

            int pageSize = 10;
            // Response.Headers.Add("Refresh", "5");
            return Json(new
            {
                page = await PaginatedList<Personacl>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize),
                totalPages = (personas.Count() + pageSize - 1) / pageSize,
                admin,
                super,
                nomsuper = nomsuper
            });
        }

        public JsonResult LoockCandado(Personacl personacl, string[] datoCandado)
        //public async Task<IActionResult> LoockCandado(Persona persona, string[] datoCandado)
        {
            personacl.Candado = Convert.ToSByte(datoCandado[0] == "true");
            personacl.IdPersonaCl = Int32.Parse(datoCandado[1]);
            personacl.MotivoCandado = mg.normaliza(datoCandado[2]);

            var empty = (from p in _context.Personacl
                         where p.IdPersonaCl == personacl.IdPersonaCl
                         select p);

            if (empty.Any())
            {
                var query = (from p in _context.Personacl
                             where p.IdPersonaCl == personacl.IdPersonaCl
                             select p).FirstOrDefault();
                query.Candado = personacl.Candado;
                query.MotivoCandado = personacl.MotivoCandado;
                _context.SaveChanges();
            }
            var stadoc = (from p in _context.Personacl
                          where personacl.IdPersonaCl == personacl.IdPersonaCl
                          select p.Candado).FirstOrDefault();
            //return View();

            return Json(new { success = true, responseText = Convert.ToString(stadoc), idPersonas = Convert.ToString(personacl.IdPersonaCl) });
        }
        #endregion

        #region -Details-
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
        #endregion

        #region -Create-
        // GET: Personacls/Create
        public async Task<IActionResult> Create(Estados Estados)
        {
            ViewBag.centrosPenitenciarios = _context.Centrospenitenciarios.Select(Centrospenitenciarios => Centrospenitenciarios.Nombrecentro).ToList();

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            ViewBag.UserMCYSCP = false;
            ViewBag.UserCL = false;
            ViewBag.user = user;

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP" || rol == "SupervisorMCSCP")
                {
                    ViewBag.UserMCYSCP = true;
                }
                if (rol == "AdminLC" || rol == "SupervisiorLC")
                {
                    ViewBag.UserCL = true;
                }
            }

            List<Estados> listaEstados = new List<Estados>();
            listaEstados = (from table in _context.Estados
                            select table).ToList();
            ViewBag.ListadoEstados = listaEstados;

            List<Municipios> listaMunicipiosD = new List<Municipios>();
            listaMunicipiosD = (from table in _context.Municipios
                                where table.EstadosId == 10
                                select table).ToList();

            listaMunicipiosD.Insert(0, new Municipios { Id = null, Municipio = "Sin municipio" });
            ViewBag.ListaMunicipios = listaMunicipiosD;

            var colonias = from p in _context.Zonas
                           orderby p.Colonia
                           select p;
            ViewBag.colonias = colonias.ToList();

            ViewBag.coloniaDGEP = "Zona Centro";
            ViewBag.calleDGEP = "Calle Miguel de Cervantes Saavedra";
            ViewBag.noDGEP = "502";
            return View();
        }


        // POST: Personacls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Personacl personacl)
        {
            string currentUser = User.Identity.Name;
            if (ModelState.IsValid)
            {

            }
            return View(personacl);
        }
        #endregion

        #region -Edit-
        // GET: Personacls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.centrosPenitenciarios = _context.Centrospenitenciarios.Select(Centrospenitenciarios => Centrospenitenciarios.Nombrecentro).ToList();

            if (id == null)
            {
                return NotFound();
            }

            var personacl = await _context.Personacl.SingleOrDefaultAsync(m => m.IdPersonaCl == id);
            if (personacl == null)
            {
                return NotFound();
            }

            List<string> consumocl = new List<string>();
            consumosustanciascl = await _context.Consumosustanciascl.Where(m => m.PersonaClIdPersonaCl == id).ToListAsync();
            for (int i = 0; i < consumosustanciascl.Count; i++)
            {
                consumocl.Add(consumosustanciascl[i].Sustancia?.ToString());
                consumocl.Add(consumosustanciascl[i].Frecuencia?.ToString());
                consumocl.Add(consumosustanciascl[i].Cantidad?.ToString());
                consumocl.Add(consumosustanciascl[i].UltimoConsumo?.ToString());
                consumocl.Add(consumosustanciascl[i].Observaciones?.ToString());
                consumocl.Add(consumosustanciascl[i].IdConsumoSustanciasCl.ToString());
            }
            List<string> asientofamiliarescl = new List<string>();
            familiarescl = await _context.Asientofamiliarcl.Where(m => m.PersonaClIdPersonaCl == id && m.Tipo == "FAMILIAR").ToListAsync();
            for (int i = 0; i < familiarescl.Count; i++)
            {
                asientofamiliarescl.Add(familiarescl[i].Nombre?.ToString());
                asientofamiliarescl.Add(familiarescl[i].Relacion?.ToString());
                asientofamiliarescl.Add(familiarescl[i].Edad?.ToString());
                asientofamiliarescl.Add(familiarescl[i].Sexo?.ToString());
                asientofamiliarescl.Add(familiarescl[i].Dependencia?.ToString());
                asientofamiliarescl.Add(familiarescl[i].DependenciaExplica?.ToString());
                asientofamiliarescl.Add(familiarescl[i].VivenJuntos?.ToString());
                asientofamiliarescl.Add(familiarescl[i].Domicilio?.ToString());
                asientofamiliarescl.Add(familiarescl[i].Telefono?.ToString());
                asientofamiliarescl.Add(familiarescl[i].HorarioLocalizacion?.ToString());
                asientofamiliarescl.Add(familiarescl[i].EnteradoProceso?.ToString());
                asientofamiliarescl.Add(familiarescl[i].PuedeEnterarse?.ToString());
                asientofamiliarescl.Add(familiarescl[i].Observaciones?.ToString());
                asientofamiliarescl.Add(familiarescl[i].IdAsientoFamiliarCl.ToString());
            }
            List<string> asientoreferenciascl = new List<string>();
            referenciaspersonalescl = await _context.Asientofamiliarcl.Where(m => m.PersonaClIdPersonaCl == id && m.Tipo == "REFERENCIA").ToListAsync();
            for (int i = 0; i < referenciaspersonalescl.Count; i++)
            {
                asientoreferenciascl.Add(referenciaspersonalescl[i].Nombre?.ToString());
                asientoreferenciascl.Add(referenciaspersonalescl[i].Relacion?.ToString());
                asientoreferenciascl.Add(referenciaspersonalescl[i].Edad?.ToString());
                asientoreferenciascl.Add(referenciaspersonalescl[i].Sexo?.ToString());
                asientoreferenciascl.Add(referenciaspersonalescl[i].Dependencia?.ToString());
                asientoreferenciascl.Add(referenciaspersonalescl[i].DependenciaExplica?.ToString());
                asientoreferenciascl.Add(referenciaspersonalescl[i].VivenJuntos?.ToString());
                asientoreferenciascl.Add(referenciaspersonalescl[i].Domicilio?.ToString());
                asientoreferenciascl.Add(referenciaspersonalescl[i].Telefono?.ToString());
                asientoreferenciascl.Add(referenciaspersonalescl[i].HorarioLocalizacion?.ToString());
                asientoreferenciascl.Add(referenciaspersonalescl[i].EnteradoProceso?.ToString());
                asientoreferenciascl.Add(referenciaspersonalescl[i].PuedeEnterarse?.ToString());
                asientoreferenciascl.Add(referenciaspersonalescl[i].Observaciones?.ToString());
                asientoreferenciascl.Add(referenciaspersonalescl[i].IdAsientoFamiliarCl.ToString());
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
              new SelectListItem{ Text="Argentina", Value="ARGENTINA"},
              new SelectListItem{ Text="Brasil", Value="BRASIL"},
              new SelectListItem{ Text="Venezuela", Value="VENEZUELA"},
              new SelectListItem{ Text="Puerto Rico", Value="PUERTO RICO"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.listaLnpais = ListaPais;

            foreach (var item in ListaPais)
            {
                if (item.Value == personacl.Lnpais)
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

            ViewBag.ListadoEstados = listaEstados;

            ViewBag.idEstado = personacl.Lnestado;
            #endregion

            #region Lnmunicipio
            int Lnestado;
            bool success = Int32.TryParse(personacl.Lnestado, out Lnestado);
            List<Municipios> listaMunicipios = new List<Municipios>();
            if (success)
            {
                listaMunicipios = (from table in _context.Municipios
                                   where table.EstadosId == Lnestado
                                   select table).ToList();
            }

            listaMunicipios.Insert(0, new Municipios { Id = 0, Municipio = "Selecciona" });

            ViewBag.ListadoMunicipios = listaMunicipios;
            ViewBag.idMunicipio = personacl.Lnmunicipio;
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
            ViewBag.idEstadoCivil = BuscaId(ListaEstadoCivil, personacl.EstadoCivil);
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
            ViewBag.idGenero = BuscaId(ListaGenero, personacl.Genero);

            #endregion

            ViewBag.listaOtroIdioma = listaNoSi;
            ViewBag.idOtroIdioma = BuscaId(listaNoSi, personacl.OtroIdioma);

            ViewBag.listaLeerEscribir = listaSiNo;
            ViewBag.idLeerEscribir = BuscaId(listaSiNo, personacl.LeerEscribir);

            ViewBag.listaTraductor = listaNoSi;
            ViewBag.idTraductor = BuscaId(listaNoSi, personacl.Traductor);

            ViewBag.listaHijos = listaNoSi;
            ViewBag.idHijos = BuscaId(listaNoSi, personacl.Hijos);

            ViewBag.listaPropiedades = listaNoSi;
            ViewBag.idPropiedades = BuscaId(listaNoSi, personacl.Propiedades);

            ViewBag.listaResolucion = listaNoSi;
            ViewBag.idResolucion = BuscaId(listaNoSi, personacl.TieneResolucion);

            ViewBag.listaComindigena = listaNoSi;
            ViewBag.idComindigena = BuscaId(listaNoSi, personacl.ComIndigena);

            ViewBag.listaComlgbtttiq = listaNoSi;
            ViewBag.idComlgbtttiq = BuscaId(listaNoSi, personacl.ComLgbtttiq);

            ViewBag.listaSinoCentroPenitenciario = listaNoSi;
            ViewBag.idSinoCentroPenitenciario = BuscaId(listaNoSi, personacl.Sinocentropenitenciario);

            ViewBag.idCentroPenitenciario = personacl.Centropenitenciario;
            ViewBag.pais = personacl.Lnpais;
            ViewBag.idioma = personacl.OtroIdioma;
            ViewBag.traductor = personacl.Traductor;
            ViewBag.Hijos = personacl.Hijos;



            #region Consume sustancias
            ViewBag.listaConsumoSustancias = listaNoSi;
            ViewBag.ConsumoSustancias = BuscaId(listaNoSi, personacl.ConsumoSustancias);

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

            if (consumosustanciascl.Count > 0)
            {
                ViewBag.idSustancia = BuscaId(ListaSustancia, consumosustanciascl[contadorSustancia].Sustancia);
                ViewBag.idFrecuencia = BuscaId(ListaFrecuencia, consumosustanciascl[contadorSustancia].Frecuencia);
                ViewBag.cantidad = consumosustanciascl[contadorSustancia].Cantidad;
                ViewBag.ultimoConsumo = consumosustanciascl[contadorSustancia].UltimoConsumo;
                ViewBag.observaciones = consumosustanciascl[contadorSustancia].Observaciones;
                ViewBag.idConsumoSustancias = consumosustanciascl[contadorSustancia].IdConsumoSustanciasCl;
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

            ViewBag.ListaConsumo = consumocl;
            ViewBag.ListaAsientoFamiliares = asientofamiliarescl;
            ViewBag.ListaAsientoReferencias = asientoreferenciascl;
            #endregion

            #region Familiares
            ViewBag.listaFamiliares = listaSiNo;
            ViewBag.idFamiliares = BuscaId(listaSiNo, personacl.Familiares);

            contadorFamiliares = 0;

            List<SelectListItem> ListaRelacion;
            ListaRelacion = new List<SelectListItem>
            {
                new SelectListItem { Text = "Pápa", Value = "PAPA" },
                new SelectListItem { Text = "Máma", Value = "MAMA" },
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

            if (familiarescl.Count > 0)
            {
                ViewBag.nombreF = familiarescl[contadorFamiliares].Nombre;
                ViewBag.idRelacionF = BuscaId(ListaRelacion, familiarescl[contadorFamiliares].Relacion);
                ViewBag.edadF = familiarescl[contadorFamiliares].Edad;
                ViewBag.idSexoF = BuscaId(ListaSexo, familiarescl[contadorFamiliares].Sexo); ;
                ViewBag.idDependenciaF = BuscaId(listaNoSi, familiarescl[contadorFamiliares].Dependencia);
                ViewBag.dependenciaExplicaF = familiarescl[contadorFamiliares].DependenciaExplica;
                ViewBag.idVivenJuntosF = BuscaId(listaSiNo, familiarescl[contadorFamiliares].VivenJuntos);
                ViewBag.domicilioF = familiarescl[contadorFamiliares].Domicilio;
                ViewBag.telefonoF = familiarescl[contadorFamiliares].Telefono;
                ViewBag.horarioLocalizacionF = familiarescl[contadorFamiliares].HorarioLocalizacion;
                ViewBag.idEnteradoProcesoF = BuscaId(listaSiNo, familiarescl[contadorFamiliares].EnteradoProceso);
                ViewBag.idPuedeEnterarseF = BuscaId(listaNoSiNA, familiarescl[contadorFamiliares].PuedeEnterarse);
                ViewBag.AFobservacionesF = familiarescl[contadorFamiliares].Observaciones;
                ViewBag.tipoF = familiarescl[contadorFamiliares].Tipo;
                ViewBag.idAsientoFamiliarF = familiarescl[contadorFamiliares].IdAsientoFamiliarCl;
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
            ViewBag.idReferenciasPersonales = BuscaId(listaSiNo, personacl.ReferenciasPersonales);

            contadorReferencias = 0;

            if (referenciaspersonalescl.Count > 0)
            {
                ViewBag.nombreR = referenciaspersonalescl[contadorReferencias].Nombre;
                ViewBag.idRelacionR = BuscaId(ListaRelacion, referenciaspersonalescl[contadorReferencias].Relacion);
                ViewBag.edadR = referenciaspersonalescl[contadorReferencias].Edad;
                ViewBag.idSexoR = BuscaId(ListaSexo, referenciaspersonalescl[contadorReferencias].Sexo); ;
                ViewBag.idDependenciaR = BuscaId(listaNoSi, referenciaspersonalescl[contadorReferencias].Dependencia);
                ViewBag.dependenciaExplicaR = referenciaspersonalescl[contadorReferencias].DependenciaExplica;
                ViewBag.idVivenJuntosR = BuscaId(listaSiNo, referenciaspersonalescl[contadorReferencias].VivenJuntos);
                ViewBag.domicilioR = referenciaspersonalescl[contadorReferencias].Domicilio;
                ViewBag.telefonoR = referenciaspersonalescl[contadorReferencias].Telefono;
                ViewBag.horarioLocalizacionR = referenciaspersonalescl[contadorReferencias].HorarioLocalizacion;
                ViewBag.idEnteradoProcesoR = BuscaId(listaSiNo, referenciaspersonalescl[contadorReferencias].EnteradoProceso);
                ViewBag.idPuedeEnterarseR = BuscaId(listaNoSiNA, referenciaspersonalescl[contadorReferencias].PuedeEnterarse);
                ViewBag.AFobservacionesR = referenciaspersonalescl[contadorReferencias].Observaciones;
                ViewBag.tipoR = referenciaspersonalescl[contadorReferencias].Tipo;
                ViewBag.idAsientoFamiliarR = referenciaspersonalescl[contadorReferencias].IdAsientoFamiliarCl;
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
        #endregion

        #region -Edita Domicilio-
        public async Task<IActionResult> EditDomicilio(string nombre, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.idPersona = id;
            ViewData["Nombre"] = nombre;
            var domiciliocl = await _context.Domiciliocl.SingleOrDefaultAsync(m => m.PersonaclIdPersonacl == id);

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
                if (item.Value == domiciliocl.TipoDomicilio)
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
              new SelectListItem{ Text="Argentina", Value="ARGENTINA"},
              new SelectListItem{ Text="Brasil", Value="BRASIL"},
              new SelectListItem{ Text="Venezuela", Value="VENEZUELA"},
              new SelectListItem{ Text="Puerto Rico", Value="PUERTO RICO"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListaPaisD = ListaPaisD;

            foreach (var item in ListaPaisD)
            {
                if (item.Value == domiciliocl.Pais)
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

            ViewBag.ListaEstadoD = listaEstadosD;
            ViewBag.idEstadoD = domiciliocl.Estado;
            #endregion

            #region Lnmunicipio
            int estadoD;
            bool success = Int32.TryParse(domiciliocl.Estado, out estadoD);
            List<Municipios> listaMunicipiosD = new List<Municipios>();
            if (success)
            {
                listaMunicipiosD = (from table in _context.Municipios
                                    where table.EstadosId == estadoD
                                    select table).ToList();
            }

            listaMunicipiosD.Insert(0, new Municipios { Id = 0, Municipio = "Sin municipio" });
            ViewBag.ListaMunicipioD = listaMunicipiosD;
            ViewBag.idMunicipioD = domiciliocl.Municipio;
            ViewBag.MunicipioD = "Sin municipio";
            for (int i = 0; i < listaMunicipiosD.Count; i++)
            {
                if (listaMunicipiosD[i].Id.ToString() == domiciliocl.Municipio)
                {
                    ViewBag.MunicipioD = listaMunicipiosD[i].Municipio;
                }
            }
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
            ViewBag.idTemporalidadD = BuscaId(ListaDomicilioT, domiciliocl.Temporalidad);
            #endregion

            ViewBag.listaResidenciaHabitual = listaSiNo;
            ViewBag.idResidenciaHabitual = BuscaId(listaSiNo, domiciliocl.ResidenciaHabitual);

            ViewBag.listacuentaDomicilioSecundario = listaNoSi;
            ViewBag.idcuentaDomicilioSecundario = BuscaId(listaNoSi, domiciliocl.DomcilioSecundario);

            ViewBag.listaZona = listaZonas;
            ViewBag.zona = BuscaId(listaZonas, domiciliocl.Zona);

            ViewBag.pais = domiciliocl.Pais;

            ViewBag.domi = domiciliocl.DomcilioSecundario;

            var colonias = from p in _context.Zonas
                           orderby p.Colonia
                           select p;
            ViewBag.colonias = colonias.ToList();
            ViewBag.colonia = domiciliocl.NombreCf;

            var colonia = domiciliocl.NombreCf;


            List<Zonas> zonasList = new List<Zonas>();
            zonasList = (from Zonas in _context.Zonas
                         select Zonas).ToList();
            ViewBag.idZona = 1;//first selected by default
            for (int i = 0; i < zonasList.Count; i++)
            {
                if (zonasList[i].Colonia.ToString().ToUpper() == domiciliocl.NombreCf.ToUpper())
                {
                    ViewBag.idZona = zonasList[i].Idzonas;
                }
            }

            if (domiciliocl == null)
            {
                return NotFound();
            }
            return View(domiciliocl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDomicilio(int id, [Bind("IdDomiciliocl,TipoDomicilio,Calle,No,TipoUbicacion,NombreCf,Pais,Estado,Municipio,Temporalidad,ResidenciaHabitual,Cp,Referencias,Horario,DomcilioSecundario,Observaciones,Zona,Lat,Lng,PersonaclIdPersonacl,Zona")] Domiciliocl domiciliocl, string inputAutocomplete)
        {
            if (id != domiciliocl.PersonaclIdPersonacl)
            {
                return NotFound();
            }

            domiciliocl.Calle = mg.normaliza(domiciliocl.Calle);
            domiciliocl.No = String.IsNullOrEmpty(domiciliocl.No) ? domiciliocl.No : domiciliocl.No.ToUpper();
            //domicilio.Cp = domicilio.Cp;
            domiciliocl.Referencias = mg.normaliza(domiciliocl.Referencias);
            domiciliocl.Horario = mg.normaliza(domiciliocl.Horario);
            domiciliocl.Observaciones = mg.normaliza(domiciliocl.Observaciones);
            //domicilio.Lat = domicilio.Lat;
            //domicilio.Lng = domicilio.Lng;

            domiciliocl.NombreCf = mg.normaliza(inputAutocomplete);

            List<Zonas> zonasList = new List<Zonas>();
            zonasList = (from Zonas in _context.Zonas
                         select Zonas).ToList();

            domiciliocl.Zona = "SIN ZONA ASIGNADA";
            int matches = 0;
            for (int i = 0; i < zonasList.Count; i++)
            {
                if (zonasList[i].Colonia.ToUpper() == domiciliocl.NombreCf)
                {
                    matches++;
                }
            }
            for (int i = 0; i < zonasList.Count; i++)
            {
                if (zonasList[i].Colonia.ToUpper() == domiciliocl.NombreCf && (matches <= 1 || zonasList[i].Cp == domiciliocl.Cp))
                {
                    domiciliocl.Zona = zonasList[i].Zona.ToUpper();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldDomicilio = await _context.Domicilio.FindAsync(domiciliocl.IdDomiciliocl);
                    _context.Entry(oldDomicilio).CurrentValues.SetValues(domiciliocl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(domicilio);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DomicilioclExists(domiciliocl.IdDomiciliocl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MenuEdicion/" + domiciliocl.PersonaclIdPersonacl, "Personascl");
            }
            return View(domiciliocl);
        }
        #endregion

        #region Edit Dmicilio Secundario

        public async Task<IActionResult> EditDomSecundario2(int? id, string nombre, string idPersona)
        {
            int index = idPersona.IndexOf("?");
            if (index >= 0)
                idPersona = idPersona.Substring(0, index);

            ViewBag.idPersona = idPersona;
            ViewBag.nombre = nombre;

            if (id == null)
            {
                return NotFound();
            }

            var domisecucl = await _context.Domiciliocl.SingleOrDefaultAsync(m => m.PersonaclIdPersonacl == id);

            #region -To List databases-
            List<Personacl> personaclVM = _context.Personacl.ToList();
            List<Domiciliocl> domicilioclVM = _context.Domiciliocl.ToList();
            List<Domiciliosecundariocl> domiciliosecundarioclVM = _context.Domiciliosecundariocl.ToList();
            #endregion

            #region -Jointables-
            ViewData["joinTablesDomcilioSec"] = from personaTable in personaclVM
                                                join domicilio in domicilioclVM on personaTable.IdPersonaCl equals domicilio.IdDomiciliocl
                                                join domicilioSec in domiciliosecundarioclVM on domicilio.IdDomiciliocl equals domicilioSec.IdDomicilioCl
                                                where personaTable.IdPersonaCl == id
                                                select new PersonaclsViewModal
                                                {
                                                    domicilioSecundarioclVM = domicilioSec
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
            ViewBag.idtDomicilio = BuscaId(LiatatDomicilio, domisecucl.TipoDomicilio);
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
              new SelectListItem{ Text="Argentina", Value="ARGENTINA"},
              new SelectListItem{ Text="Brasil", Value="BRASIL"},
              new SelectListItem{ Text="Venezuela", Value="VENEZUELA"},
              new SelectListItem{ Text="Puerto Rico", Value="PUERTO RICO"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListaPaisED = ListaPaisD;
            ViewBag.idPaisED = BuscaId(ListaPaisD, domisecucl.Pais);

            ViewBag.ListaPaisM = ListaPaisD;
            ViewBag.idPaisM = BuscaId(ListaPaisD, domisecucl.Pais);
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
            ViewBag.idEstadoED = domisecucl.Estado;

            ViewBag.ListaEstadoM = listaEstadosD;
            ViewBag.idEstadoM = domisecucl.Estado;
            #endregion

            #region Lnmunicipio
            int estadoD;
            bool success = Int32.TryParse(domisecucl.Estado, out estadoD);
            List<Municipios> listaMunicipiosD = new List<Municipios>();
            if (success)
            {
                listaMunicipiosD = (from table in _context.Municipios
                                    where table.EstadosId == estadoD
                                    select table).ToList();
            }

            ViewBag.ListaMunicipioED = listaMunicipiosD;
            ViewBag.idMunicipioED = domisecucl.Municipio;

            ViewBag.ListaMunicipioM = listaMunicipiosD;
            ViewBag.idMunicipioM = domisecucl.Municipio;

            ViewBag.Pais = domisecucl.Pais;
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
            ViewBag.idTemporalidad = BuscaId(ListaDomicilioT, domisecucl.Temporalidad);



            ViewBag.listaResidenciaHabitual = listaSiNo;
            ViewBag.idResidenciaHabitual = BuscaId(listaSiNo, domisecucl.ResidenciaHabitual);

            #endregion
            if (domisecucl == null)
            {
                return NotFound();
            }

            return View(domisecucl);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDomSecundario([Bind("IdDomicilioSecundarioCl,IdDomicilioCl,TipoDomicilio,Calle,No,TipoUbicacion,NombreCf,Pais,Estado,Municipio,Temporalidad,ResidenciaHabitual,Cp,Referencias,Horario,Motivo,Observaciones")] Domiciliosecundariocl domiciliosecundariocl, string nombre, string idPersona)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    domiciliosecundariocl.TipoUbicacion = mg.normaliza(domiciliosecundariocl.TipoUbicacion);
                    domiciliosecundariocl.Calle = mg.normaliza(domiciliosecundariocl.Calle);
                    domiciliosecundariocl.No = mg.normaliza(domiciliosecundariocl.No);
                    domiciliosecundariocl.NombreCf = mg.normaliza(domiciliosecundariocl.NombreCf);
                    domiciliosecundariocl.Cp = domiciliosecundariocl.Cp;
                    domiciliosecundariocl.Referencias = mg.normaliza(domiciliosecundariocl.Referencias);
                    domiciliosecundariocl.Horario = mg.normaliza(domiciliosecundariocl.Horario);
                    domiciliosecundariocl.Motivo = mg.normaliza(domiciliosecundariocl.Motivo);
                    domiciliosecundariocl.Observaciones = mg.normaliza(domiciliosecundariocl.Observaciones);


                    var oldDomicilio = await _context.Domiciliosecundario.FindAsync(domiciliosecundariocl.IdDomicilioSecundarioCl);
                    _context.Entry(oldDomicilio).CurrentValues.SetValues(domiciliosecundariocl);
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
                return RedirectToAction("EditDomicilio/" + idPersona, "Personascl", new { nombre = nombre });
            }
            return View();
        }

        public async Task<IActionResult> DeleteConfirmedDom(int? id, int idpersona)
        {
            var domseundariocl = await _context.Domiciliosecundariocl.SingleOrDefaultAsync(m => m.IdDomicilioSecundarioCl == id);
            _context.Domiciliosecundariocl.Remove(domseundariocl);
            await _context.SaveChangesAsync();

            var empty = (from ds in _context.Domiciliosecundariocl
                         where ds.IdDomicilioSecundarioCl == domseundariocl.IdDomicilioSecundarioCl
                         select ds);

            if (!empty.Any())
            {
                var query = (from a in _context.Domiciliocl
                             where a.IdDomiciliocl == domseundariocl.IdDomicilioSecundarioCl
                             select a).FirstOrDefault();
                query.DomcilioSecundario = "NO";
                _context.SaveChanges();
            }

            return RedirectToAction("EditDomicilio/" + idpersona, "Personascl");
        }

        public async Task<IActionResult> CrearDomicilioSecundario(Domiciliosecundariocl domiciliosecundariocl, string[] datosDomicilio)
        {
            domiciliosecundariocl.IdDomicilioCl = Int32.Parse(datosDomicilio[0]);
            domiciliosecundariocl.TipoDomicilio = mg.normaliza(datosDomicilio[1]);
            domiciliosecundariocl.Calle = mg.normaliza(datosDomicilio[2]);
            domiciliosecundariocl.No = mg.normaliza(datosDomicilio[3]);
            domiciliosecundariocl.TipoUbicacion = mg.normaliza(datosDomicilio[4]);
            domiciliosecundariocl.NombreCf = mg.normaliza(datosDomicilio[5]);
            domiciliosecundariocl.Pais = datosDomicilio[6];
            domiciliosecundariocl.Estado = datosDomicilio[7];
            domiciliosecundariocl.Municipio = datosDomicilio[8];
            domiciliosecundariocl.Temporalidad = mg.normaliza(datosDomicilio[9]);
            domiciliosecundariocl.ResidenciaHabitual = mg.normaliza(datosDomicilio[10]);
            domiciliosecundariocl.Cp = mg.normaliza(datosDomicilio[11]);
            domiciliosecundariocl.Referencias = mg.normaliza(datosDomicilio[12]);
            domiciliosecundariocl.Motivo = mg.normaliza(datosDomicilio[13]);
            domiciliosecundariocl.Horario = mg.normaliza(datosDomicilio[14]);
            domiciliosecundariocl.Observaciones = mg.normaliza(datosDomicilio[15]);


            var query = (from a in _context.Domiciliocl
                         where a.IdDomiciliocl == domiciliosecundariocl.IdDomicilioCl
                         select a).FirstOrDefault();
            query.DomcilioSecundario = "SI";
            _context.SaveChanges();

            var query2 = (from p in _context.Personacl
                          join d in _context.Domiciliocl on p.IdPersonaCl equals d.IdDomiciliocl
                          join ds in _context.Domiciliosecundariocl on d.IdDomiciliocl equals ds.IdDomicilioCl
                          where ds.IdDomicilioSecundarioCl == domiciliosecundariocl.IdDomicilioCl
                          select p);



            _context.Add(domiciliosecundariocl);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditDomicilio/" + query.PersonaclIdPersonacl, "Personascl");
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
            var estudioscl = await _context.Estudioscl.SingleOrDefaultAsync(m => m.PersonaClIdPersonaCl == id);
            if (estudioscl == null)
            {
                return NotFound();
            }

            ViewBag.estudia = estudioscl.Estudia;
            ViewBag.listaEstudia = listaNoSi;
            ViewBag.idEstudia = BuscaId(listaNoSi, estudioscl.Estudia);

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
            ViewBag.idGradoEstudios = BuscaId(ListaGradoEstudios, estudioscl.GradoEstudios);
            #endregion


            List<Estudioscl> estudiosclVM = _context.Estudioscl.ToList();
            List<Personacl> personaclVM = _context.Personacl.ToList();


            ViewData["joinTablesPersonaEstudia"] =
                                     from personaTable in personaclVM
                                     join estudiosTabla in estudiosclVM on personaTable.IdPersonaCl equals estudiosTabla.PersonaClIdPersonaCl
                                     where personaTable.IdPersonaCl == idPersona
                                     select new PersonaclsViewModal
                                     {
                                         personaclVM = personaTable,
                                         estudiosclVM = estudiosTabla

                                     };

            if ((ViewData["joinTablesPersonaEstudia"] as IEnumerable<scorpioweb.Models.PersonaclsViewModal>).Count() == 0)
            {
                ViewBag.RA = false;
            }
            else
            {
                ViewBag.RA = true;
            }


            return View(estudioscl);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEscolaridad(int id, [Bind("IdEstudiosCl,Estudia,GradoEstudios,InstitucionE,Horario,Direccion,Telefono,Observaciones,PersonaClIdPersonaCl")] Estudioscl estudioscl)
        {
            if (id != estudioscl.PersonaClIdPersonaCl)
            {
                return NotFound();
            }

            estudioscl.InstitucionE = mg.normaliza(estudioscl.InstitucionE);
            estudioscl.Horario = mg.normaliza(estudioscl.Horario);
            estudioscl.Direccion = mg.normaliza(estudioscl.Direccion);
            estudioscl.Observaciones = mg.normaliza(estudioscl.Observaciones);



            if (ModelState.IsValid)
            {
                try
                {
                    var oldEstudios = await _context.Estudios.FindAsync(estudioscl.IdEstudiosCl);
                    _context.Entry(oldEstudios).CurrentValues.SetValues(estudioscl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(estudios);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstudioclExists(estudioscl.IdEstudiosCl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MenuEdicion/" + estudioscl.PersonaClIdPersonaCl, "Personascl");
            }
            return View(estudioscl);
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
            var trabajocl = await _context.Trabajocl.SingleOrDefaultAsync(m => m.PersonaClIdPersonaCl == id);
            if (trabajocl == null)
            {
                return NotFound();
            }

            ViewBag.listaTrabaja = listaSiNo;
            ViewBag.idTrabaja = BuscaId(listaSiNo, trabajocl.Trabaja);

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
            ViewBag.idTipoOcupacion = BuscaId(ListaTipoOcupacion, trabajocl.TipoOcupacion);
            #endregion

            ViewBag.listaEnteradoProceso = listaNoSiNA;
            ViewBag.idEnteradoProceso = BuscaId(listaNoSiNA, trabajocl.EnteradoProceso);

            ViewBag.listasePuedeEnterarT = listaNoSiNA;
            ViewBag.idsePuedeEnterart = BuscaId(listaNoSiNA, trabajocl.SePuedeEnterar);
            ViewBag.trabaja = trabajocl.Trabaja;

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
            ViewBag.idTiempoTrabajando = BuscaId(ListaTiempoTrabajando, trabajocl.TiempoTrabajano);
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
            ViewBag.idTemporalidadSalario = BuscaId(ListaTemporalidadSalario, trabajocl.TemporalidadSalario);
            #endregion

            return View(trabajocl);
        }

        // POST: Trabajoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrabajo(int id, [Bind("IdTrabajoCl,Trabaja,TipoOcupacion,Puesto,EmpledorJefe,EnteradoProceso,SePuedeEnterar,TiempoTrabajano,Salario,TemporalidadSalario,Direccion,Horario,Telefono,Observaciones,PersonaClIdPersonaCl")] Trabajocl trabajocl)
        {
            if (id != trabajocl.PersonaClIdPersonaCl)
            {
                return NotFound();
            }

            trabajocl.Puesto = mg.normaliza(trabajocl.Puesto);
            trabajocl.EmpledorJefe = mg.normaliza(trabajocl.EmpledorJefe);
            trabajocl.Salario = mg.normaliza(trabajocl.Salario);
            trabajocl.TemporalidadSalario = mg.normaliza(trabajocl.TemporalidadSalario);
            trabajocl.Direccion = mg.normaliza(trabajocl.Direccion);
            trabajocl.Horario = mg.normaliza(trabajocl.Horario);
            trabajocl.Observaciones = mg.normaliza(trabajocl.Observaciones);

            if (ModelState.IsValid)
            {
                try
                {
                    var oldTrabajo = await _context.Trabajo.FindAsync(trabajocl.IdTrabajoCl);
                    _context.Entry(oldTrabajo).CurrentValues.SetValues(trabajocl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(trabajo);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrabajoclExists(trabajocl.IdTrabajoCl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MenuEdicion/" + trabajocl.PersonaClIdPersonaCl, "Personascl");
            }
            return View(trabajocl);
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
            var actividadsocialcl = await _context.Actividadsocialcl.SingleOrDefaultAsync(m => m.PersonaClIdPersonaCl == id);

            ViewBag.listasePuedeEnterarASr = listaNoSiNA;
            ViewBag.idsePuedeEnterarAS = BuscaId(listaNoSiNA, actividadsocialcl.SePuedeEnterar);

            if (actividadsocialcl == null)
            {
                return NotFound();
            }
            return View(actividadsocialcl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditActividadesSociales(int id, [Bind("IdActividadSocialCl,TipoActividad,Horario,Lugar,Telefono,SePuedeEnterar,Referencia,Observaciones,PersonaClIdPersonaCl")] Actividadsocialcl actividadsocialcl)
        {
            if (id != actividadsocialcl.PersonaClIdPersonaCl)
            {
                return NotFound();
            }

            actividadsocialcl.TipoActividad = mg.normaliza(actividadsocialcl.TipoActividad);
            actividadsocialcl.Horario = mg.normaliza(actividadsocialcl.Horario);
            actividadsocialcl.Lugar = mg.normaliza(actividadsocialcl.Lugar);
            actividadsocialcl.Referencia = mg.normaliza(actividadsocialcl.Referencia);
            actividadsocialcl.Observaciones = mg.normaliza(actividadsocialcl.Observaciones);



            if (ModelState.IsValid)
            {
                try
                {
                    var oldActividadsocialcl = await _context.Actividadsocialcl.FindAsync(actividadsocialcl.IdActividadSocialCl);
                    _context.Entry(oldActividadsocialcl).CurrentValues.SetValues(actividadsocialcl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(actividadsocial);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActividadSocialExists(actividadsocialcl.IdActividadSocialCl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MenuEdicion/" + actividadsocialcl.PersonaClIdPersonaCl, "Personascl");
            }
            return View(actividadsocialcl);
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
            var abandonoestadocl = await _context.Abandonoestadocl.SingleOrDefaultAsync(m => m.PersonaclIdPersonacl == id);

            ViewBag.listaVividoFuera = listaNoSi;
            ViewBag.idVividoFuera = BuscaId(listaNoSi, abandonoestadocl.VividoFuera);

            ViewBag.listaViajaHabitual = listaNoSi;
            ViewBag.idViajaHabitual = BuscaId(listaNoSi, abandonoestadocl.ViajaHabitual);

            ViewBag.listaDocumentacionSalirPais = listaNoSi;
            ViewBag.idDocumentacionSalirPais = BuscaId(listaNoSi, abandonoestadocl.DocumentacionSalirPais);

            ViewBag.listaPasaporte = listaNoSi;
            ViewBag.idPasaporte = BuscaId(listaNoSi, abandonoestadocl.Pasaporte);

            ViewBag.listaVisa = listaNoSi;
            ViewBag.idVisa = BuscaId(listaNoSi, abandonoestadocl.Visa);

            ViewBag.listaFamiliaresFuera = listaNoSi;
            ViewBag.idFamiliaresFuera = BuscaId(listaNoSi, abandonoestadocl.FamiliaresFuera);

            ViewBag.vfuera = abandonoestadocl.VividoFuera;
            ViewBag.vlugar = abandonoestadocl.ViajaHabitual;
            ViewBag.document = abandonoestadocl.DocumentacionSalirPais;
            ViewBag.Abandono = abandonoestadocl.FamiliaresFuera;



            if (abandonoestadocl == null)
            {
                return NotFound();
            }
            return View(abandonoestadocl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAbandonoEstado(int id, [Bind("IdAbandonoEstadocl,VividoFuera,LugaresVivido,TiempoVivido,MotivoVivido,ViajaHabitual,LugaresViaje,TiempoViaje,MotivoViaje,DocumentacionSalirPais,Pasaporte,Visa,FamiliaresFuera,Cuantos,PersonaclIdPersonacl")] Abandonoestadocl abandonoestadocl, string arrayFamExtranjero, string arrayFamExtranjerosEditados)
        {
            if (id != abandonoestadocl.PersonaclIdPersonacl)
            {
                return NotFound();
            }

            abandonoestadocl.LugaresVivido = mg.normaliza(abandonoestadocl.LugaresVivido);
            abandonoestadocl.TiempoVivido = mg.normaliza(abandonoestadocl.TiempoVivido);
            abandonoestadocl.MotivoVivido = mg.normaliza(abandonoestadocl.MotivoVivido);
            abandonoestadocl.LugaresViaje = mg.normaliza(abandonoestadocl.LugaresViaje);
            abandonoestadocl.TiempoViaje = mg.normaliza(abandonoestadocl.TiempoViaje);
            abandonoestadocl.MotivoViaje = mg.normaliza(abandonoestadocl.MotivoViaje);




            if (ModelState.IsValid)
            {
                try
                {
                    var oldAbandonoestadocl = await _context.Abandonoestadocl.FindAsync(abandonoestadocl.IdAbandonoEstadocl);
                    _context.Entry(oldAbandonoestadocl).CurrentValues.SetValues(abandonoestadocl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AbandonoestadoExists(abandonoestadocl.IdAbandonoEstadocl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MenuEdicion/" + abandonoestadocl.PersonaclIdPersonacl, "Personascl");
            }
            return View(abandonoestadocl);
        }
        #endregion

        #region -EditFamiliaresForaneos-
        public async Task<IActionResult> EditFamiliaresForaneos(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var familiaresforaneoscl = await _context.Familiaresforaneoscl.Where(m => m.IdFamiliaresForaneosCl == id).FirstOrDefaultAsync();
            ViewBag.idFamiliarF = familiaresforaneoscl.PersonaClIdPersonaCl;

            #region GENERO          
            List<SelectListItem> ListaGenero;
            ListaGenero = new List<SelectListItem>
            {
              new SelectListItem{ Text="Masculino", Value="M"},
              new SelectListItem{ Text="Femenino", Value="F"},
              new SelectListItem{ Text="Prefiero no decirlo", Value="N"},
            };
            ViewBag.listaGenero = ListaGenero;
            ViewBag.idGenero = BuscaId(ListaGenero, familiaresforaneoscl.Sexo);
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
            ViewBag.idRelacion = BuscaId(ListaRelacion, familiaresforaneoscl.Relacion);
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
            ViewBag.idTiempo = BuscaId(ListaTiempo, familiaresforaneoscl.TiempoConocerlo);
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
              new SelectListItem{ Text="Argentina", Value="ARGENTINA"},
              new SelectListItem{ Text="Brasil", Value="BRASIL"},
              new SelectListItem{ Text="Venezuela", Value="VENEZUELA"},
              new SelectListItem{ Text="Puerto Rico", Value="PUERTO RICO"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListaPais = ListaPaisD;
            ViewBag.idPais = BuscaId(ListaPaisD, familiaresforaneoscl.Pais);
            #endregion

            #region Destado
            List<Estados> listaEstado = new List<Estados>();
            listaEstado = (from table in _context.Estados
                           select table).ToList();

            listaEstado.Insert(0, new Estados { Id = 0, Estado = "Selecciona" });
            ViewBag.ListaEstado = listaEstado;
            ViewBag.idEstado = familiaresforaneoscl.Estado;
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
            ViewBag.idFrecuencia = BuscaId(ListaFrecuencia, familiaresforaneoscl.Pais);
            #endregion

            ViewBag.listaProseso = listaNoSi;
            ViewBag.idProseso = BuscaId(listaNoSi, familiaresforaneoscl.EnteradoProceso);

            ViewBag.listaEnterar = listaNoSiNA;
            ViewBag.idEnterar = BuscaId(listaNoSiNA, familiaresforaneoscl.PuedeEnterarse);
            ViewBag.Pais = familiaresforaneoscl.Pais;

            if (familiaresforaneoscl == null)
            {
                return NotFound();
            }
            return View(familiaresforaneoscl);
        }

        public async Task<IActionResult> EditFamiliaresForaneos2(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var familiaresforaneoscl = await _context.Familiaresforaneoscl.FirstAsync(m => m.PersonaClIdPersonaCl == id);

            #region -To List databases-
            List<Personacl> personaclVM = _context.Personacl.ToList();
            List<Familiaresforaneoscl> familiaresforaneosclVM = _context.Familiaresforaneoscl.ToList();

            #endregion

            #region -Jointables-
            ViewData["joinTableFamiliarF"] = from personaTable in personaclVM
                                             join familiarf in familiaresforaneosclVM on personaTable.IdPersonaCl equals familiarf.PersonaClIdPersonaCl
                                             where familiarf.PersonaClIdPersonaCl == id
                                             select new PersonaclsViewModal
                                             {
                                                 familiaresForaneosclVM = familiarf
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
              new SelectListItem{ Text="Argentina", Value="ARGENTINA"},
              new SelectListItem{ Text="Brasil", Value="BRASIL"},
              new SelectListItem{ Text="Venezuela", Value="VENEZUELA"},
              new SelectListItem{ Text="Puerto Rico", Value="PUERTO RICO"},
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

            if (familiaresforaneoscl == null)
            {
                return NotFound();
            }

            return View(familiaresforaneoscl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFamiliaresForaneos(int id, [Bind("IdFamiliaresForaneosCl,Nombre,Edad,Sexo,Relacion,TiempoConocerlo,Pais,Estado,Telefono,FrecuenciaContacto,EnteradoProceso,PuedeEnterarse,Observaciones,PersonaClIdPersonaCl")] Familiaresforaneoscl familiaresforaneoscl)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    familiaresforaneoscl.Nombre = mg.normaliza(familiaresforaneoscl.Nombre);
                    familiaresforaneoscl.Estado = mg.normaliza(familiaresforaneoscl.Estado);
                    familiaresforaneoscl.Observaciones = mg.normaliza(familiaresforaneoscl.Observaciones);


                    var oldFamiliaresforaneos = await _context.Familiaresforaneos.FindAsync(familiaresforaneoscl.IdFamiliaresForaneosCl);
                    _context.Entry(oldFamiliaresforaneos).CurrentValues.SetValues(familiaresforaneoscl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(familiaresforaneos);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction("EditAbandonoEstado/" + familiaresforaneoscl.PersonaClIdPersonaCl, "Personascl");
            }
            return View(familiaresforaneoscl);
        }

        public async Task<IActionResult> DeleteConfirmedFamiiarF(int? id)
        {
            var familiarfcl = await _context.Familiaresforaneoscl.SingleOrDefaultAsync(m => m.IdFamiliaresForaneosCl == id);
            _context.Familiaresforaneoscl.Remove(familiarfcl);
            await _context.SaveChangesAsync();

            var empty = (from ff in _context.Familiaresforaneos
                         where ff.PersonaIdPersona == familiarfcl.PersonaClIdPersonaCl
                         select ff);

            if (!empty.Any())
            {
                var query = (from a in _context.Abandonoestadocl
                             where a.PersonaclIdPersonacl == familiarfcl.PersonaClIdPersonaCl
                             select a).FirstOrDefault();
                query.FamiliaresFuera = "NO";
                _context.SaveChanges();
            }




            return RedirectToAction("EditAbandonoEstado/" + familiarfcl.PersonaClIdPersonaCl, "Personascl");
        }

        public async Task<IActionResult> CrearFamiliarforaneo(Familiaresforaneoscl familiaresforaneoscl, string[] datosFamiliarF)
        {

            familiaresforaneoscl.PersonaClIdPersonaCl = Int32.Parse(datosFamiliarF[0]);
            familiaresforaneoscl.Nombre = mg.normaliza(datosFamiliarF[1]);
            try
            {
                familiaresforaneoscl.Edad = Int32.Parse(datosFamiliarF[2]);
            }
            catch
            {
                familiaresforaneoscl.Edad = 0;
            }
            familiaresforaneoscl.Sexo = datosFamiliarF[3];
            familiaresforaneoscl.Relacion = datosFamiliarF[4];
            familiaresforaneoscl.TiempoConocerlo = datosFamiliarF[5];
            familiaresforaneoscl.Pais = datosFamiliarF[6];
            familiaresforaneoscl.Estado = mg.normaliza(datosFamiliarF[7]);
            familiaresforaneoscl.Telefono = datosFamiliarF[8];
            familiaresforaneoscl.FrecuenciaContacto = datosFamiliarF[9];
            familiaresforaneoscl.EnteradoProceso = datosFamiliarF[10];
            familiaresforaneoscl.PuedeEnterarse = datosFamiliarF[11];
            familiaresforaneoscl.Observaciones = mg.normaliza(datosFamiliarF[12]);

            var query = (from a in _context.Abandonoestado
                         where a.PersonaIdPersona == familiaresforaneoscl.PersonaClIdPersonaCl
                         select a).FirstOrDefault();
            query.FamiliaresFuera = "SI";
            _context.SaveChanges();

            _context.Add(familiaresforaneoscl);
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
            var saludfisicacl = await _context.Saludfisicacl.SingleOrDefaultAsync(m => m.PersonaClIdPersonaCl == id);


            ViewBag.listaEnfermedad = listaNoSi;
            ViewBag.idEnfermedad = BuscaId(listaNoSi, saludfisicacl.Enfermedad);

            ViewBag.listaEmbarazoLactancia = listaNoSi;
            ViewBag.idEmbarazoLactancia = BuscaId(listaNoSi, saludfisicacl.EmbarazoLactancia);

            ViewBag.listaDiscapacidad = listaNoSi;
            ViewBag.idDiscapacidad = BuscaId(listaNoSi, saludfisicacl.Discapacidad);

            ViewBag.listaServicioMedico = listaNoSi;
            ViewBag.idServicioMedico = BuscaId(listaNoSi, saludfisicacl.ServicioMedico);

            ViewBag.enfermedad = saludfisicacl.Enfermedad;
            ViewBag.especial = saludfisicacl.Discapacidad;
            ViewBag.smedico = saludfisicacl.ServicioMedico;

            #region EspecifiqueServicioMedico
            List<SelectListItem> ListaEspecifiqueServicioMedico;
            ListaEspecifiqueServicioMedico = new List<SelectListItem>
            {
                new SelectListItem{ Text = "NA", Value = "NA" },
                new SelectListItem{ Text = "Derecho habiente", Value = "DERECHO HABIENTE" },
                new SelectListItem{ Text = "Seguro Médico", Value = "SEGURO MEDICO" }
            };

            ViewBag.listaEspecifiqueServicioMedico = ListaEspecifiqueServicioMedico;
            ViewBag.idEspecifiqueServicioMedico = BuscaId(ListaEspecifiqueServicioMedico, saludfisicacl.EspecifiqueServicioMedico);
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
            ViewBag.idInstitucionServicioMedico = BuscaId(ListaInstitucionServicioMedico, saludfisicacl.InstitucionServicioMedico);
            #endregion



            if (saludfisicacl == null)
            {
                return NotFound();
            }
            return View(saludfisicacl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSalud(int id, [Bind("IdSaludFisicaCl,Enfermedad,EspecifiqueEnfermedad,EmbarazoLactancia,Tiempo,Tratamiento,Discapacidad,EspecifiqueDiscapacidad,ServicioMedico,EspecifiqueServicioMedico,InstitucionServicioMedico,Observaciones,PersonaClIdPersonaCl")] Saludfisicacl saludfisicacl)
        {
            if (id != saludfisicacl.IdSaludFisicaCl)
            {
                return NotFound();
            }

            saludfisicacl.EspecifiqueEnfermedad = mg.normaliza(saludfisicacl.EspecifiqueEnfermedad);
            saludfisicacl.Tratamiento = mg.normaliza(saludfisicacl.Tratamiento);
            saludfisicacl.EspecifiqueDiscapacidad = mg.normaliza(saludfisicacl.EspecifiqueDiscapacidad);
            saludfisicacl.Observaciones = mg.normaliza(saludfisicacl.Observaciones);
            saludfisicacl.Tiempo = mg.normaliza(saludfisicacl.Tiempo);



            if (ModelState.IsValid)
            {
                try
                {
                    var oldSaludfisicacl = await _context.Saludfisicacl.FindAsync(saludfisicacl.PersonaClIdPersonaCl);
                    _context.Entry(oldSaludfisicacl).CurrentValues.SetValues(saludfisicacl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(saludfisica);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaludfisicaclExists(saludfisicacl.IdSaludFisicaCl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MenuEdicion/" + saludfisicacl.PersonaClIdPersonaCl, "Personascl");
            }
            return View(saludfisicacl);
        }
        #endregion


        #region -Delete-
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
        #endregion
        #endregion


        public IActionResult Menucl()
        {
            return View();
        }
        public async Task<IActionResult> MenuEdicion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByNameAsync(User.Identity.Name);

            #region -Solicitud Atendida Archivo prestamo Digital-
            var warningRespuesta = from a in _context.Archivoprestamodigital
                                   where a.EstadoPrestamo == 1 && user.ToString().ToUpper() == a.Usuario.ToUpper()
                                   select a;
            ViewBag.WarningsUser = warningRespuesta.Count();
            #endregion


            var personacl = await _context.Personacl.SingleOrDefaultAsync(m => m.IdPersonaCl == id);
            if (personacl == null)
            {
                return NotFound();
            }

            string rutaFoto = ((personacl.Genero == ("M")) ? "hombre.png" : "mujer.png");
            if (personacl.RutaFoto != null)
            {
                rutaFoto = personacl.RutaFoto;
            }
            ViewBag.rutaFoto = rutaFoto;

            return View(personacl);
        }
        public async Task<IActionResult> EditFoto(int? id)
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

        #region -Existe-
        private bool PersonaclExists(int id)
        {
            return _context.Personacl.Any(e => e.IdPersonaCl == id);
        }
        private bool DomicilioclExists(int id)
        {
            return _context.Domiciliocl.Any(e => e.IdDomiciliocl == id);
        }
        private bool EstudioclExists(int id)
        {
            return _context.Estudioscl.Any(e => e.IdEstudiosCl == id);
        }
        private bool TrabajoclExists(int id)
        {
            return _context.Trabajocl.Any(e => e.IdTrabajoCl == id);
        } 
        private bool ActividadSocialExists(int id)
        {
            return _context.Actividadsocialcl.Any(e => e.IdActividadSocialCl == id);
        }
        private bool AbandonoestadoExists(int id)
        {
            return _context.Abandonoestadocl.Any(e => e.IdAbandonoEstadocl == id);
        }
                private bool SaludfisicaclExists(int id)
        {
            return _context.Saludfisicacl.Any(e => e.IdSaludFisicaCl == id);
        }

        #endregion




    }
}
