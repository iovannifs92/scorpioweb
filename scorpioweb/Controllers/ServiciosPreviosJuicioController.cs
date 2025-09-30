using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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
using Newtonsoft.Json;
using QRCoder;
using SautinSoft.Document;
using scorpioweb.Class;
using scorpioweb.Models;

namespace scorpioweb.Controllers
{

    public class ServiciosPreviosJuicioController : Controller
    {
        #region -Constructor-
        private readonly penas2Context _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public ServiciosPreviosJuicioController(penas2Context context, IHostingEnvironment hostingEnvironment,
                        RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        #endregion

        #region -Listas-
        private List<SelectListItem> listaNaSiNo = new List<SelectListItem>

        {
            new SelectListItem{ Text="NA", Value="NA"},
            new SelectListItem{ Text="Si", Value="SI"},
            new SelectListItem{ Text="No", Value="NO"}
        };

        private List<SelectListItem> listaSexo = new List<SelectListItem>
        {
            new SelectListItem{ Text="Masculino", Value="M"},
            new SelectListItem{ Text="Femenino", Value="F"}
        };

        private List<SelectListItem> listaDomicilio = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Verdadero", Value = "VERDADERO" },
            new SelectListItem{ Text = "Falso", Value = "FALSO" }
        };

        private List<SelectListItem> listaUbicacion = new List<SelectListItem>
        {
            new SelectListItem{ Text = "C. Operativa", Value = "C. OPERATIVA" },
            new SelectListItem{ Text = "C. Región Norte", Value = "C. REGIÓN NORTE" },
            new SelectListItem{ Text = "Archivo", Value = "ARCHIVO" },
            new SelectListItem{ Text = "C. Ejecución de Penas", Value = "C. EJECUCION PENAS" },
            new SelectListItem{ Text = "Adolescentes", Value = "ADOLESCENTES" }
        };

        private List<SelectListItem> ListaUnidadInvestigación = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Centro de Operaciones Estratégicas", Value = "CENTRO DE OPERACIONES ESTRATEGICAS" },
            new SelectListItem{ Text = "M. P. Fóraneos", Value = "M. P. FORANEOS" },
            new SelectListItem{ Text = "F. G. R.", Value = "F. G. R." },
            new SelectListItem{ Text = "F. G. R. Gómez Palacio , Dgo.", Value = "F. G. R. GOMEZ PALACIO, DGO" },
            new SelectListItem{ Text = "Fiscalía", Value = "FISCALIA" },
            new SelectListItem{ Text = "Vicefiscalía", Value = "VICEFISCALIA" }
        };

        private List<SelectListItem> listaSituacionJuridica = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Detenido", Value = "DETENIDO" },
            new SelectListItem{ Text = "No Detenido", Value = "NO DETENIDO" },
            new SelectListItem{ Text = "Imposibilidad de Arraigo", Value = "IMPOSIBILIDAD DE ARRAIGO" },
            new SelectListItem{ Text = "Prision Preventiva", Value = "PRISION PREVENTIVA" },
            new SelectListItem{ Text = "Sentenciado", Value = "SENTENCIADO" },
            new SelectListItem{ Text = "Resguardo Domiciliario", Value = "RESGUARDO DOMICILIARIO" }
        };

        private List<SelectListItem> listaRealizoEntrevista = new List<SelectListItem>
        {
            new SelectListItem{ Text="NA", Value="NA"},
            new SelectListItem{ Text = "Si", Value = "SI" },
            new SelectListItem{ Text = "Negativa", Value = "NEGATIVA" },
            new SelectListItem{ Text = "No Fue Localizado", Value = "NO FUE LOCALIZADO" }
        };

        private List<SelectListItem> listaTipoDetenido = new List<SelectListItem>
        {
            new SelectListItem{ Text = "NA", Value = "NA" },
            new SelectListItem{ Text = "Adulto", Value = "ADULTO" },
            new SelectListItem{ Text = "Adolescente", Value = "ADOLESCENTE" }
        };

        private List<SelectListItem> listaAER = new List<SelectListItem>
        {
            new SelectListItem{ Text = "NA", Value = "NA" },
            new SelectListItem{ Text = "Alto", Value = "ALTO" },
            new SelectListItem{ Text = "Medio", Value = "MEDIO" },
            new SelectListItem{ Text = "Bajo", Value = "BAJO" }
        };

        private List<SelectListItem> listaRiesgo = new List<SelectListItem>
        {
            new SelectListItem{ Text = "NA", Value = "NA" },
            new SelectListItem{ Text = "Alto", Value = "ALTO" },
            new SelectListItem{ Text = "Bajo", Value = "BAJO" }
        };

        private List<SelectListItem> listaRecomendación = new List<SelectListItem>
        {
            new SelectListItem{ Text = "NA", Value = "NA" },
            new SelectListItem{ Text = "Medida en Libertad sin Condiciones", Value = "MEDIDA EN LIBERTAD SIN CONDICIONES" },
            new SelectListItem{ Text = "Medida Cautelar en Libertad", Value = "MEDIDA CAUTELAR EN LIBERTAD" },
            new SelectListItem{ Text = "Internamiento Preventivo", Value = "INTERNAMIENTO PREVENTIVO" }
        };

        #endregion

        #region -Metodos Generales-
        MetodosGenerales mg = new MetodosGenerales();

        #region -Estados y Municipios-
        public JsonResult GetMunicipio(int EstadoId)
        {
            TempData["message"] = DateTime.Now;
            List<Municipios> municipiosList = new List<Municipios>();

            municipiosList = (from Municipios in _context.Municipios
                              where Municipios.EstadosId == EstadoId
                              select Municipios).ToList();

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

        //public JsonResult GetZona()
        //{
        //    List<Zonas> zonasList = new List<Zonas>();
        //    zonasList = (from Zonas in _context.Zonas
        //                 select Zonas).ToList();
        //    return Json(new
        //    {
        //        success = true,
        //        zonas = zonasList
        //    });
        //}

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
        #endregion

        public async Task<IActionResult> MenuSPJ()
        {

            return View();
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

            var user = await userManager.FindByNameAsync(User.Identity.Name);

            #region -Solicitud Atendida Archivo prestamo Digital-
            var warningRespuesta = from a in _context.Archivoprestamodigital
                                   where a.EstadoPrestamo == 1 && user.ToString().ToUpper() == a.Usuario.ToUpper()
                                   select a;
            ViewBag.WarningsUser = warningRespuesta.Count();
            #endregion


            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            ViewData["CurrentFilter"] = searchString;

            var serviciospreviosjuicios = from p in _context.Serviciospreviosjuicio
                                          select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                foreach (var item in searchString.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    serviciospreviosjuicios = serviciospreviosjuicios.Where(p => (mg.removeSpaces(p.Paterno) + " " + mg.removeSpaces(p.Materno) + " " + mg.removeSpaces(p.Nombre)).Contains(mg.normaliza(searchString)) ||
                                                   (mg.removeSpaces(p.Nombre) + " " + mg.removeSpaces(p.Paterno) + " " + mg.removeSpaces(p.Materno)).Contains(mg.normaliza(searchString)));
                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    serviciospreviosjuicios = serviciospreviosjuicios.OrderByDescending(p => p.Paterno);
                    break;
                default:
                    serviciospreviosjuicios = serviciospreviosjuicios.OrderBy(p => p.Paterno);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Serviciospreviosjuicio>.CreateAsync(serviciospreviosjuicios.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        #endregion

        #region -Details-
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviciospreviosjuicio = await _context.Serviciospreviosjuicio
                .SingleOrDefaultAsync(m => m.IdserviciosPreviosJuicio == id);
            if (serviciospreviosjuicio == null)
            {
                return NotFound();
            }

            return View(serviciospreviosjuicio);
        }
        #endregion


        #region -Create-
        public async Task<IActionResult> Create()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            ViewBag.UserMCYSCP = false;
            ViewBag.user = user;

            foreach (var rol in roles)
            {
                if (rol == "Servicios previos" || rol == "Operativo")
                {
                    ViewBag.Serviciosprevios = true;
                }
            }


            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();
            #region -Listas-
            ViewBag.listaSexo = listaSexo;

            ViewBag.listaDomicilio = listaDomicilio;

            ViewBag.listaUbicacion = listaUbicacion;

            ViewBag.listaUnidadI = ListaUnidadInvestigación;

            ViewBag.listaSituacionJuridica = listaSituacionJuridica;

            ViewBag.listaRealizoE = listaRealizoEntrevista;

            ViewBag.listaTipoDetenido = listaTipoDetenido;

            ViewBag.listaAER = listaAER;

            ViewBag.listaTamizaje = listaNaSiNo;

            ViewBag.listaComparesencia = listaRiesgo;

            ViewBag.listaVictima = listaRiesgo;

            ViewBag.listaObstaculizar = listaRiesgo;

            ViewBag.listaRecomendacion = listaRecomendación;

            ViewBag.listaAntecedentes = listaNaSiNo;
            #endregion

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

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile evidencia, [Bind("Nombre,Paterno,Materno,Sexo,Edad,Calle,Colonia,Domicilio,ClaveUnicaScorpio,Telefono,Papa,Mama,Ubicacion,Delito,UnidadInvestigacion,FechaDetencion,Situacion,RealizoEntrevista,TipoDetenido,Aer,Tamizaje,Rcomparesencia,Rvictima,Robstaculizacion,Recomendacion,Antecedentes,AntecedentesDatos,Observaciones, FechaNacimiento, LnMunicipio, LnEstado,LnPais")] Serviciospreviosjuicio serviciospreviosjuicio, Expedienteunico expedienteunico, string tabla, string idselecionado, string CURS, string CURSUsada, string datosArray)
        {
            int idAER = 0;
            serviciospreviosjuicio.Nombre = mg.normaliza(serviciospreviosjuicio.Nombre);
            serviciospreviosjuicio.Paterno = mg.normaliza(serviciospreviosjuicio.Paterno);
            serviciospreviosjuicio.Materno = mg.normaliza(serviciospreviosjuicio.Materno);
            serviciospreviosjuicio.Sexo = mg.normaliza(serviciospreviosjuicio.Sexo);
            serviciospreviosjuicio.Edad = serviciospreviosjuicio.Edad;
            serviciospreviosjuicio.Calle = mg.normaliza(serviciospreviosjuicio.Calle);
            serviciospreviosjuicio.Colonia = mg.normaliza(serviciospreviosjuicio.Colonia);
            serviciospreviosjuicio.Domicilio = mg.normaliza(serviciospreviosjuicio.Domicilio);
            serviciospreviosjuicio.Telefono = serviciospreviosjuicio.Telefono;
            serviciospreviosjuicio.Papa = mg.normaliza(serviciospreviosjuicio.Papa);
            serviciospreviosjuicio.Mama = mg.normaliza(serviciospreviosjuicio.Mama);
            serviciospreviosjuicio.Ubicacion = mg.normaliza(serviciospreviosjuicio.Ubicacion);
            serviciospreviosjuicio.Delito = mg.normaliza(serviciospreviosjuicio.Delito);
            serviciospreviosjuicio.UnidadInvestigacion = mg.normaliza(serviciospreviosjuicio.UnidadInvestigacion);
            serviciospreviosjuicio.FechaDetencion = serviciospreviosjuicio.FechaDetencion;
            serviciospreviosjuicio.Situacion = serviciospreviosjuicio.Situacion;
            serviciospreviosjuicio.RealizoEntrevista = mg.normaliza(serviciospreviosjuicio.RealizoEntrevista);
            serviciospreviosjuicio.TipoDetenido = mg.normaliza(serviciospreviosjuicio.TipoDetenido);
            serviciospreviosjuicio.Aer = mg.normaliza(serviciospreviosjuicio.Aer);
            serviciospreviosjuicio.Tamizaje = mg.normaliza(serviciospreviosjuicio.Tamizaje);
            serviciospreviosjuicio.Rcomparesencia = mg.normaliza(serviciospreviosjuicio.Rcomparesencia);
            serviciospreviosjuicio.Rvictima = mg.normaliza(serviciospreviosjuicio.Rvictima);
            serviciospreviosjuicio.Robstaculizacion = mg.normaliza(serviciospreviosjuicio.Robstaculizacion);
            serviciospreviosjuicio.Recomendacion = mg.normaliza(serviciospreviosjuicio.Recomendacion);
            serviciospreviosjuicio.Antecedentes = mg.normaliza(serviciospreviosjuicio.Antecedentes);
            serviciospreviosjuicio.AntecedentesDatos = mg.normaliza(serviciospreviosjuicio.AntecedentesDatos);
            serviciospreviosjuicio.Observaciones = mg.normaliza(serviciospreviosjuicio.Observaciones);
            serviciospreviosjuicio.Usuario = User.Identity.Name.ToUpper();
            serviciospreviosjuicio.FechaCaptura = DateTime.Now;
            serviciospreviosjuicio.LnEstado = serviciospreviosjuicio.LnEstado;
            serviciospreviosjuicio.LnMunicipio = serviciospreviosjuicio.LnMunicipio;
            serviciospreviosjuicio.LnPais = serviciospreviosjuicio.LnPais;
            serviciospreviosjuicio.FechaNacimiento = serviciospreviosjuicio.FechaNacimiento;
            if (CURSUsada != null)
            {
                serviciospreviosjuicio.ClaveUnicaScorpio = CURSUsada;
            }
            else
            {
                serviciospreviosjuicio.ClaveUnicaScorpio = CURS;
            }

            int cont = (from table in _context.Serviciospreviosjuicio
                        select table.IdserviciosPreviosJuicio).Count();
            if (cont != 0)
            {
                idAER = ((from table in _context.Serviciospreviosjuicio
                          select table.IdserviciosPreviosJuicio).Max()) + 1;
            }
            else
            {
                idAER = 1;
            }


            serviciospreviosjuicio.IdserviciosPreviosJuicio = idAER;

            #region -Guardar archivo-
            if (evidencia != null)
            {
                string file_name = idAER + "_" + serviciospreviosjuicio.Paterno + "_" + serviciospreviosjuicio.Materno + "_" + serviciospreviosjuicio.Nombre + Path.GetExtension(evidencia.FileName);
                file_name = mg.replaceSlashes(file_name);
                serviciospreviosjuicio.RutaAer = file_name;
                var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "AER");
                var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                await evidencia.CopyToAsync(stream);
                stream.Close();
            }
            #endregion

            #region -Expediente Unico-
            // TODO ESTA EN 
            #endregion

            _context.Add(serviciospreviosjuicio);

            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region -Edit-
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();
            var serviciospreviosjuicio = await _context.Serviciospreviosjuicio.SingleOrDefaultAsync(m => m.IdserviciosPreviosJuicio == id);

            #region -Listas-

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

            ViewBag.ListadoPais = ListaPais;
            ViewBag.idPais = mg.BuscaId(ListaPais, serviciospreviosjuicio.LnPais);

            #region Lnestado
            List<Estados> listaEstados = new List<Estados>();
            listaEstados = (from table in _context.Estados
                            select table).ToList();

            ViewBag.ListadoEstados = listaEstados;

            ViewBag.idEstado = serviciospreviosjuicio.LnEstado;
            #endregion

            #region Lnmunicipio
            int Lnestado;
            bool success = Int32.TryParse(serviciospreviosjuicio.LnEstado, out Lnestado);
            List<Municipios> listaMunicipios = new List<Municipios>();
            if (success)
            {
                listaMunicipios = (from table in _context.Municipios
                                   where table.EstadosId == Lnestado
                                   select table).ToList();
            }

            listaMunicipios.Insert(0, new Municipios { Id = 0, Municipio = "Selecciona" });

            ViewBag.ListadoMunicipios = listaMunicipios;
            ViewBag.idMunicipio = serviciospreviosjuicio.LnEstado;
            #endregion

            ViewBag.listaSexo = listaSexo;
            ViewBag.idGenero = mg.BuscaId(listaSexo, serviciospreviosjuicio.Sexo);

            ViewBag.listaDomicilio = listaDomicilio;
            ViewBag.idDomicilio = mg.BuscaId(listaDomicilio, serviciospreviosjuicio.Domicilio);

            ViewBag.listaUbicacion = listaUbicacion;
            ViewBag.idUbicacion = mg.BuscaId(listaUbicacion, serviciospreviosjuicio.Ubicacion);

            ViewBag.listaUnidadI = ListaUnidadInvestigación;
            ViewBag.idUnidad = mg.BuscaId(ListaUnidadInvestigación, serviciospreviosjuicio.UnidadInvestigacion);

            ViewBag.listaSituacionJuridica = listaSituacionJuridica;
            ViewBag.idSituacionJuridica = mg.BuscaId(listaSituacionJuridica, serviciospreviosjuicio.Situacion);

            ViewBag.listaRealizoE = listaRealizoEntrevista;
            ViewBag.idRealizoE = mg.BuscaId(listaRealizoEntrevista, serviciospreviosjuicio.RealizoEntrevista);

            ViewBag.listaTipoDetenido = listaTipoDetenido;
            ViewBag.idlTipoDetenido = mg.BuscaId(listaTipoDetenido, serviciospreviosjuicio.TipoDetenido);

            ViewBag.listaAER = listaAER;
            ViewBag.idAER = mg.BuscaId(listaAER, serviciospreviosjuicio.Aer);

            ViewBag.listaTamizaje = listaNaSiNo;
            ViewBag.idTamizaje = mg.BuscaId(listaNaSiNo, serviciospreviosjuicio.Tamizaje);

            ViewBag.listaComparesencia = listaRiesgo;
            ViewBag.idComparesencia = mg.BuscaId(listaRiesgo, serviciospreviosjuicio.Rcomparesencia);

            ViewBag.listaVictima = listaRiesgo;
            ViewBag.idVictima = mg.BuscaId(listaRiesgo, serviciospreviosjuicio.Rvictima);

            ViewBag.listaObstaculizar = listaRiesgo;
            ViewBag.idObstaculizar = mg.BuscaId(listaRiesgo, serviciospreviosjuicio.Robstaculizacion);

            ViewBag.listaRecomendacion = listaRecomendación;
            ViewBag.idRecomendacion = mg.BuscaId(listaRecomendación, serviciospreviosjuicio.Recomendacion);

            ViewBag.listaAntecedentes = listaNaSiNo;
            ViewBag.idAntecedentes = mg.BuscaId(listaNaSiNo, serviciospreviosjuicio.Antecedentes);
            #endregion

            ViewBag.TipoDetenido = serviciospreviosjuicio.TipoDetenido;
            ViewBag.RealizoEntrevista = serviciospreviosjuicio.RealizoEntrevista;
            ViewBag.TieneAntecedentes = serviciospreviosjuicio.Antecedentes;

            var serviciospreviosjuici = await _context.Serviciospreviosjuicio.SingleOrDefaultAsync(m => m.IdserviciosPreviosJuicio == id);
            if (serviciospreviosjuici == null)
            {
                return NotFound();
            }
            return View(serviciospreviosjuicio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile evidencia, [Bind("IdserviciosPreviosJuicio,Nombre,Paterno,Materno,Sexo,Edad,Calle,Colonia,Domicilio, ClaveUnicaScorpio ,Telefono,Papa,Mama,Ubicacion,Delito,UnidadInvestigacion,FechaDetencion,Situacion,RealizoEntrevista,TipoDetenido,Aer,Tamizaje,Rcomparesencia,Rvictima,Robstaculizacion,Recomendacion,Antecedentes,AntecedentesDatos,Observaciones,serviciospreviosjuicioIdserviciospreviosjuicio,FechaCaptura,Usuario,ClaveUnicaScorpio, FechaNacimiento, LnMunicipio, LnEstado,LnPais")] Serviciospreviosjuicio serviciospreviosjuicio)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    serviciospreviosjuicio.Nombre = mg.normaliza(serviciospreviosjuicio.Nombre);
                    serviciospreviosjuicio.Paterno = mg.normaliza(serviciospreviosjuicio.Paterno);
                    serviciospreviosjuicio.Materno = mg.normaliza(serviciospreviosjuicio.Materno);
                    serviciospreviosjuicio.Sexo = mg.normaliza(serviciospreviosjuicio.Sexo);
                    serviciospreviosjuicio.Edad = mg.normaliza(serviciospreviosjuicio.Edad);
                    serviciospreviosjuicio.Calle = mg.normaliza(serviciospreviosjuicio.Calle);
                    serviciospreviosjuicio.Colonia = mg.normaliza(serviciospreviosjuicio.Colonia);
                    serviciospreviosjuicio.Domicilio = mg.normaliza(serviciospreviosjuicio.Domicilio);
                    serviciospreviosjuicio.Telefono = mg.normaliza(serviciospreviosjuicio.Telefono);
                    serviciospreviosjuicio.Papa = mg.normaliza(serviciospreviosjuicio.Papa);
                    serviciospreviosjuicio.Mama = mg.normaliza(serviciospreviosjuicio.Mama);
                    serviciospreviosjuicio.Ubicacion = mg.normaliza(serviciospreviosjuicio.Ubicacion);
                    serviciospreviosjuicio.Delito = mg.normaliza(serviciospreviosjuicio.Delito);
                    serviciospreviosjuicio.UnidadInvestigacion = mg.normaliza(serviciospreviosjuicio.UnidadInvestigacion);
                    serviciospreviosjuicio.FechaDetencion = serviciospreviosjuicio.FechaDetencion;
                    serviciospreviosjuicio.Situacion = mg.normaliza(serviciospreviosjuicio.Situacion);
                    serviciospreviosjuicio.RealizoEntrevista = mg.normaliza(serviciospreviosjuicio.RealizoEntrevista);
                    serviciospreviosjuicio.TipoDetenido = mg.normaliza(serviciospreviosjuicio.TipoDetenido);
                    serviciospreviosjuicio.Aer = mg.normaliza(serviciospreviosjuicio.Aer);
                    serviciospreviosjuicio.Tamizaje = mg.normaliza(serviciospreviosjuicio.Tamizaje);
                    serviciospreviosjuicio.Rcomparesencia = mg.normaliza(serviciospreviosjuicio.Rcomparesencia);
                    serviciospreviosjuicio.Rvictima = mg.normaliza(serviciospreviosjuicio.Rvictima);
                    serviciospreviosjuicio.Robstaculizacion = mg.normaliza(serviciospreviosjuicio.Robstaculizacion);
                    serviciospreviosjuicio.Recomendacion = mg.normaliza(serviciospreviosjuicio.Recomendacion);
                    serviciospreviosjuicio.AntecedentesDatos = mg.normaliza(serviciospreviosjuicio.AntecedentesDatos);
                    serviciospreviosjuicio.Observaciones = mg.normaliza(serviciospreviosjuicio.Observaciones);
                    serviciospreviosjuicio.Usuario = serviciospreviosjuicio.Usuario;
                    serviciospreviosjuicio.FechaCaptura = serviciospreviosjuicio.FechaCaptura;
                    serviciospreviosjuicio.LnEstado = serviciospreviosjuicio.LnEstado;
                    serviciospreviosjuicio.LnMunicipio = serviciospreviosjuicio.LnMunicipio;
                    serviciospreviosjuicio.LnPais = serviciospreviosjuicio.LnPais;
                    serviciospreviosjuicio.FechaNacimiento = serviciospreviosjuicio.FechaNacimiento;
                    serviciospreviosjuicio.ClaveUnicaScorpio = serviciospreviosjuicio.ClaveUnicaScorpio;


                    if (!(serviciospreviosjuicio.Paterno == null && serviciospreviosjuicio.Materno == null && serviciospreviosjuicio.FechaNacimiento == null && serviciospreviosjuicio.Sexo == null && serviciospreviosjuicio.LnEstado == null && serviciospreviosjuicio.Nombre == null))
                    {
                        if (serviciospreviosjuicio.LnPais != "MEXICO")
                        {
                            serviciospreviosjuicio.LnEstado = "33";
                        }
                        var curs = mg.sacaCurs(serviciospreviosjuicio.Paterno, serviciospreviosjuicio.Materno, serviciospreviosjuicio.FechaNacimiento, serviciospreviosjuicio.Sexo, serviciospreviosjuicio.LnEstado, serviciospreviosjuicio.Nombre);
                        serviciospreviosjuicio.ClaveUnicaScorpio = curs;
                    }

                    var oldServiciospreviosjuicio = await _context.Serviciospreviosjuicio.FindAsync(serviciospreviosjuicio.IdserviciosPreviosJuicio);
                    #region -EditarArchivo-
                    if (evidencia == null)
                    {
                        serviciospreviosjuicio.RutaAer = oldServiciospreviosjuicio.RutaAer;
                    }
                    else
                    {
                        string file_name = serviciospreviosjuicio.IdserviciosPreviosJuicio + "_" + serviciospreviosjuicio.Paterno + "_" + serviciospreviosjuicio.Materno + "_" + serviciospreviosjuicio.Nombre + Path.GetExtension(evidencia.FileName);
                        file_name = mg.replaceSlashes(file_name);
                        serviciospreviosjuicio.RutaAer = file_name;
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "AER");

                        if (System.IO.File.Exists(Path.Combine(uploads, file_name)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, file_name));
                        }

                        var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                        await evidencia.CopyToAsync(stream);
                        stream.Close();
                    }
                    #endregion


                    _context.Entry(oldServiciospreviosjuicio).CurrentValues.SetValues(serviciospreviosjuicio);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                    //_context.Update(serviciospreviosjuicio);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiciospreviosjuicioExists(serviciospreviosjuicio.IdserviciosPreviosJuicio))
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
            return View(serviciospreviosjuicio);
        }
        #endregion

        #region -Delete-
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviciospreviosjuicio = await _context.Serviciospreviosjuicio
                .SingleOrDefaultAsync(m => m.IdserviciosPreviosJuicio == id);
            if (serviciospreviosjuicio == null)
            {
                return NotFound();
            }

            return View(serviciospreviosjuicio);
        }

        // POST: ServiciosPreviosJuicio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            #region -Editar en expediente unico-
            try
            {
                var ideu = (from eu in _context.Expedienteunico
                            where Int64.Parse(eu.Serviciospreviosjuicio) == id
                            select eu.IdexpedienteUnico).FirstOrDefault();

                var query = (from s in _context.Expedienteunico
                             where s.IdexpedienteUnico == ideu
                             select s).FirstOrDefault();
                query.Serviciospreviosjuicio = null;
                _context.SaveChanges();

            }
            catch (Exception ex)
            {

            }
            #endregion

            var serviciospreviosjuicio = await _context.Serviciospreviosjuicio.SingleOrDefaultAsync(m => m.IdserviciosPreviosJuicio == id);
            _context.Serviciospreviosjuicio.Remove(serviciospreviosjuicio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region -ServiciospreviosjuicioExists-
        private bool ServiciospreviosjuicioExists(int id)
        {
            return _context.Serviciospreviosjuicio.Any(e => e.IdserviciosPreviosJuicio == id);
        }
        #endregion

        #region -Envio a correspondencia - 
        public async Task<IActionResult> Correspondencia()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            int count = 0; // SI AL FINAL ESTA VARIABLE VALE 2, EL USUARIO TIENE LOS DOS ROLES Y VA A PODER LAS ACCIONES DE ENVIAR Y RECIBIR
            ViewBag.Area = mg.areaSegunRol(roles);
            ViewBag.BotonId = true;
            ViewBag.EnviarRecibir = false;
            switch (ViewBag.Area)
            {
                case "MCySCP": ViewBag.Area = "MCYSCP";//esto se modifica para que al momento de buscar por id no halla problema en el metodo sacar datos de la api
                break; 
              
                case "LC":ViewBag.Area = "Libertad Condicionada";//esto se modifica para que al momento de buscar por id no halla problema en el metodo sacar datos de la api  
                    break;
                
                case "UESPA":ViewBag.BotonId = false;//NO ES NECESARIO POR QUE NO HAY BD DE ADOLESCENTES
                    break;
                
                case "Direccion":ViewBag.BotonId = false;//NO ES NECESARIO POR QUE NO HAY BD DE DIRECCION
                    break;
                
                case "Vinculacion":ViewBag.BotonId = false;//NO ES NECESARIO POR QUE NO HAY BD DE VINCULACION
                    break;
                
                case "Ejecución de Penas": ViewBag.Area = "Ejecucion";
                    break;
                
                case "Servicios previos": ViewBag.Area = "Servicios Previos";
                    break;
                
                case "Servicios legales":
                    ViewBag.BotonId = false;
                    break;
                
                case "Sistemas":
                    ViewBag.BotonId = false;//NO ES NECESARIO POR QUE NO HAY BD DE SISTEMAS
                    break;
                
                case "Oficialia":
                    ViewBag.BotonId = false;
                    break;// SOLO PARA QUE NO SE VAYA AL DEFAULT
                
                case "Coordinacion Operativa":
                    ViewBag.BotonId = false;
                    break;// SOLO PARA QUE NO SE VAYA AL DEFAULT
                
                default:
                    ViewBag.Area = ViewBag.Area ?? "SIN AREA";
                    ViewBag.BotonId = false;
                    break;
            }
            foreach(var rol in roles)
            {
                if (rol.ToString().Equals("EnviarCorrespondencia") || rol.ToString().Equals("RecibirCorrespondencia"))
                    count++;
            }
            if (count == 2)
                ViewBag.EnviarRecibir = true;// EL USUARIO PUEDE ENVIAR Y RECIBIR DOCUMENTOS

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EnviarCorrespondencia(Enviocorrespondencia datos)
        {
            if (datos == null)
                return BadRequest(new { success = false, message = "No se recibieron datos" });

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            datos.Nombre = mg.normaliza(datos.Nombre);
            datos.Apaterno = mg.normaliza(datos.Apaterno);
            datos.Amaterno = mg.normaliza(datos.Amaterno);
            datos.Nombre = mg.normaliza(datos.Nombre);
            datos.QuienEntrega = user.UserName; 
            datos.FechaRegistro = DateTime.Now;
            datos.Entregado = 0;
            datos.Recibido = 0;
            datos.FiguraJudicial = mg.normaliza(datos.FiguraJudicial) ?? "NA";
            datos.Area = mg.normaliza(datos.Area) ?? "NA";
            datos.Asunto = mg.normaliza(datos.Asunto) ?? "NA";
            datos.Autoridad = mg.normaliza(datos.Autoridad) ?? "NA";
            datos.Observaciones = datos.Observaciones ?? "NA";

            try
            {
                // Assuming _context is your DbContext injected in the controller
                await _context.Enviocorrespondencia.AddAsync(datos);
                await _context.SaveChangesAsync();

                return Json(new { success = true, datos });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> VerCorrespondenciaEnviada(string area)
        {
            if (area == "SIN AREA")
                return BadRequest("SIN AREA");

            List<Enviocorrespondencia> correspondencia = new List<Enviocorrespondencia>();
            switch (area)
            {
                case "MCYSCP":
                    correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "MCYSCP").OrderByDescending(m => m.FechaRegistro).ToListAsync();
                    break;
                case "Libertad Condicionada":
                    correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "LIBERTAD CONDICIONADA").OrderByDescending(m => m.FechaRegistro).ToListAsync();
                    break;
                case "Direccion":
                    correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "DIRECCION").OrderByDescending(m => m.FechaRegistro).ToListAsync();
                    break;
                case "UESPA":
                    correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "UESPA").OrderByDescending(m => m.FechaRegistro).ToListAsync();
                    break;
                case "Vinculacion":
                    correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "VINCULACION").OrderByDescending(m => m.FechaRegistro).ToListAsync();
                    break;
                case "Ejecucion":
                    correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "EJECUCION").OrderByDescending(m => m.FechaRegistro).ToListAsync();
                    break;
                case "Servicios Previos":
                    correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "SERVICIOS PREVIOS").OrderByDescending(m => m.FechaRegistro).ToListAsync();
                    break;
                case "Servicios legales":
                    correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "SERVICIOS LEGALES").OrderByDescending(m => m.FechaRegistro).ToListAsync();
                    break;
                case "Sistemas":
                    correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "SISTEMAS").OrderByDescending(m => m.FechaRegistro).ToListAsync();
                    break;
                case "Oficialia":
                    correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "OFICIALIA").OrderByDescending(m => m.FechaRegistro).ToListAsync();
                    break;
                case "Coordinacion Operativa":
                    correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "OPERATIVO").OrderByDescending(m => m.FechaRegistro).ToListAsync();
                    break;
                default:
                    return BadRequest("SIN AREA");
            }

            return PartialView("_VerCorrespondenciaEnviada",correspondencia);
        }

            [HttpGet]
        public async Task<IActionResult> FiltrarCorrespondencia(int? page, string searchString, string selectSearch)
        {

            if (string.IsNullOrEmpty(selectSearch) && string.IsNullOrEmpty(searchString))
                return BadRequest();

            List<Enviocorrespondencia> correspondencia = new List<Enviocorrespondencia>();
            var officialAreas = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { 
                { "UESPA", "UESPA" }, 
                { "Ejecución de Penas", "EJECUCION" },
                { "LC", "LIBERTAD CONDICIONADA" },
                { "MCySCP", "MCYSCP" },
                { "Servicios legales", "SERVICIOS LEGALES" }, 
                { "Servicios previos", "SERVICIOS PREVIOS" }, 
                { "Sistemas", "SISTEMAS" }, { "Oficialia", "OFICIALIA" }, 
                { "Coordinacion Operativa", "COORDINACION OPERATIVA" },
                { "Direccion", "DIRECCION" }, 
                { "Vinculacion", "VINCULACION" } 
            };

            // Get all users and their roles
            var users = await userManager.Users.ToListAsync();
            var userAreas = new Dictionary<string, string>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);

                // Skip users who only have "usuario" as their single role
                if (roles.Count == 1 && roles.Contains("usuario", StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                var internalArea = mg.areaSegunRol(roles); // your existing method

                // Map internal area to official display area
                if (officialAreas.ContainsKey(internalArea))
                {
                    userAreas[user.UserName] = officialAreas[internalArea];
                }
                else
                {
                    userAreas[user.UserName] = "Sin área asignada";
                }
            }

            ViewBag.UserAreas = userAreas;

            // SE ENTRA A ESTE SWITCH CON EL DATO QUE VIENE DEL SELECT EN LA VISTA CORRESPONDENCIA
            //SOLO VA A ENTRAR AQUI SI EL SELECTSEARCH NO ESTA VACIO Y EL SEARCHSTRING ESTA VACIO
            if (!string.IsNullOrEmpty(selectSearch) && string.IsNullOrEmpty(searchString))
            {
                //PONER AQUI EL VIEW BAG ASEGURA QUE CUANDO SE ACTUALIZE LA PAGINA, SOLO EL FILTRO SELECCIONADO SE QUEDE Y NO EL DE NOMBRE SI ES QUE ESCRIBIO ALGO 
                ViewBag.CurrentSelectSearch = selectSearch;
                selectSearch = selectSearch.ToUpper();
                switch (selectSearch)
                {

                    case "UESPA":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "UESPA").OrderBy(m => m.Entregado).ToListAsync();
                        break;

                    case "DIRECCION":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "DIRECCION").OrderBy(m => m.Entregado).ToListAsync();
                        break;

                    case "EJECUCION":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "EJECUCION").OrderBy(m => m.Entregado).ToListAsync();
                        break;

                    case "LIBERTAD CONDICIONADA":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "LIBERTAD CONDICIONADA").OrderBy(m => m.Entregado).ToListAsync();
                        break;

                    case "MCYSCP":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "MCYSCP").OrderBy(m => m.Entregado).ToListAsync();
                        break;

                    case "COORDINACION OPERATIVA":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "OPERATIVO").OrderBy(m => m.Entregado).ToListAsync();
                        break;

                    case "SERVICIOS LEGALES":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "SERVICIOS LEGALES").OrderBy(m => m.Entregado).ToListAsync();
                        break;

                    case "SERVICIOS PREVIOS":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "SERVICIOS PREVIOS").OrderBy(m => m.Entregado).ToListAsync();
                        break;

                    case "SISTEMAS":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "SISTEMAS").OrderBy(m => m.Entregado).ToListAsync();
                        break;

                    case "OFICIALIA":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "OFICIALIA").OrderBy(m => m.Entregado).ToListAsync();
                        break;
                    case "VINCULACION":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Area == "VINCULACION").OrderBy(m => m.Entregado).ToListAsync();
                        break;

                    case "OFICIO NO ENTREGADO-ACUSE NO RECIBIDO":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Entregado == 0 && m.Recibido == 0).OrderBy(m => m.Entregado).ToListAsync();
                        break;

                    case "OFICIO ENTREGADO-ACUSE NO RECIBIDO":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Entregado == 1 && m.Recibido == 0).OrderBy(m => m.Recibido).ToListAsync();
                        break;

                    case "OFICIO ENTREGADO-ACUSE RECIBIDO":
                        correspondencia = await _context.Enviocorrespondencia.Where(m => m.Entregado == 1 && m.Recibido == 1).OrderBy(m => m.Entregado).ToListAsync();
                        break;
                  
                    case "TODOS":
                        correspondencia = await _context.Enviocorrespondencia.OrderBy(m => m.Entregado).ToListAsync();
                        break;

                    default:
                        return BadRequest();
                }
            }
            //SOLO VA A ENTRAR AQUI SI EL SELECTSEARCH ESTA VACIO Y EL SEARCHSTRING NO ESTA VACIO
            else if (string.IsNullOrEmpty(selectSearch) && !string.IsNullOrEmpty(searchString))
            {
                //PONER AQUI EL VIEW BAG ASEGURA QUE CUANDO SE ACTUALIZE LA PAGINA, SOLO EL FILTRO SELECCIONADO SE QUEDE Y NO EL DE NOMBRE SI ES QUE ESCRIBIO ALGO 
                searchString = searchString.ToUpper();
                ViewBag.CurrentSearchString = searchString;
                correspondencia = await _context.Enviocorrespondencia
                    .Where(m => m.Nombre.Contains(searchString)
                             || m.Apaterno.Contains(searchString)
                             || m.Amaterno.Contains(searchString))
                    .OrderBy(m => m.Entregado)
                    .ToListAsync();
            }
            //SE RETORNA UNA VISTA PARCIAL PARA QUE NO SE ACTUALIZE TODA LA PAGINA, ADEMAS MANDAMOS EL MODELO PARA LA TABLA
            return PartialView("_RecibirCorrespondencia", correspondencia);
        }

        [HttpPost]
        public async Task<IActionResult> ModificarCorrespondencia(int id, string tipo,string usuario)
        {
            var correspondencia = await _context.Enviocorrespondencia.FindAsync(id);
            if (correspondencia == null)
                return Json(new { success = false, message = "Registro no encontrado" });

            switch (tipo)
            {
                case "entregado":
                    correspondencia.Entregado = 1;
                    correspondencia.FechaEntrega = DateTime.Now;
                    break;
                case "recibido":
                    correspondencia.Recibido = 1;
                    correspondencia.FechaRecibido = DateTime.Now;
                    break;
                case "select-usuario":
                    correspondencia.QuienRecibe = usuario;
                    break;
            }
            try
            {
                
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {

                return Json(new { success = false, message ="Error al actualizar el registro" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al actualizar el registro" });
            }


            return Json(new { success = true });
        }
        #endregion

    }
}
