﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;
using Microsoft.AspNetCore.Http;
using SautinSoft.Document;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using scorpioweb.Class;
using F23.StringSimilarity;

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
            new SelectListItem{ Text = "Imposibilidad de Arraigo", Value = "IMPOSIBILIDAD DE ARRAIGO" }
        };

        private List<SelectListItem> listaRealizoEntrevista = new List<SelectListItem>
        {
            new SelectListItem{ Text="NA", Value="NA"},
            new SelectListItem{ Text = "Si", Value = "SI" },
            new SelectListItem{ Text = "Negativa", Value = "NEGATIVA" }
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

            var query = (from p in _context.Persona
                         join d in _context.Domicilio on p.IdPersona equals d.PersonaIdPersona into domicilioJoin
                         from d in domicilioJoin.DefaultIfEmpty()
                         join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona into supervisionJoin
                         from s in supervisionJoin.DefaultIfEmpty()
                         join cp in _context.Causapenal on s.CausaPenalIdCausaPenal equals cp.IdCausaPenal into causapenalJoin
                         from cp in causapenalJoin.DefaultIfEmpty()
                         group cp.CausaPenal by new { p.IdPersona, p.Paterno, p.Materno, p.Nombre, p.Edad, d.Calle, d.No, d.NombreCf, p.Curp } into g
                         select new
                         {
                             id = g.Key.IdPersona,
                             nomcom = g.Key.Paterno + " " + g.Key.Materno + " " + g.Key.Nombre,
                             NomTabla = "MCYSCP",
                             datoExtra = $"CURP: {g.Key.Curp} Edad: {g.Key.Edad};\n Domicilio: {g.Key.Calle}, {g.Key.No}, {g.Key.NombreCf};\n Causa(s) penal(es): {string.Join(", ", g)};\n"
                         }).Union
                         (from a in _context.Archivo
                          join ar in _context.Archivoregistro on a.IdArchivo equals ar.ArchivoIdArchivo
                          group new { ar.CausaPenal, ar.CarpetaEjecucion } by new { a.IdArchivo, a.Paterno, a.Materno, a.Nombre } into g
                          select new
                          {
                              id = g.Key.IdArchivo,
                              nomcom = g.Key.Paterno + " " + g.Key.Materno + " " + g.Key.Nombre,
                              NomTabla = "Archivo",
                              datoExtra = $"Causa(s) Penal(es): {string.Join(", ", g.Select(x => x.CausaPenal))};\n Carpeta de Ejecucion: {string.Join(", ", g.Select(x => x.CarpetaEjecucion))};\n"
                          }).Union
                            (from e in _context.Ejecucion
                             join epcp in _context.Epcausapenal on e.IdEjecucion equals epcp.EjecucionIdEjecucion into epcausapenalJoin
                             from epcp in epcausapenalJoin.DefaultIfEmpty()
                             group epcp.Causapenal by new { e.IdEjecucion, e.Paterno, e.Materno, e.Nombre, e.Ce } into g
                             select new
                             {
                                 id = g.Key.IdEjecucion,
                                 nomcom = g.Key.Paterno + " " + g.Key.Materno + " " + g.Key.Nombre,
                                 NomTabla = "Ejecucion",
                                 datoExtra = $"Carpeta de Ejecucion: {g.Key.Ce};\n Causa(s) Penal(es): {string.Join(", ", g)};\n"
                             }).Union
                                    (from sp in _context.Serviciospreviosjuicio
                                     select new
                                     {
                                         id = sp.IdserviciosPreviosJuicio,
                                         nomcom = sp.Paterno + " " + sp.Materno + " " + sp.Nombre,
                                         NomTabla = "Servicios Previos",
                                         datoExtra = $"Edad: {sp.Edad};\n Domicilio: {sp.Calle}, {sp.Colonia};\n"
                                     }).Union
                                    (from pp in _context.Prisionespreventivas
                                     select new
                                     {
                                         id = pp.Idprisionespreventivas,
                                         nomcom = pp.Paterno + " " + pp.Materno + " " + pp.Nombre,
                                         NomTabla = "Prision Preventiva",
                                         datoExtra = $"Causa penal: {pp.CausaPenal};\n"
                                     }).Union
                                    (from pp in _context.Oficialia
                                     select new
                                     {
                                         id = pp.IdOficialia,
                                         nomcom = pp.Paterno + " " + pp.Materno + " " + pp.Nombre,
                                         NomTabla = "Oficialia",
                                         datoExtra = $"Causa penal: {pp.CausaPenal};\n Carpeta de Ejecucion: {pp.CarpetaEjecucion};\n"
                                     });

            var result = query.ToList();

            int idpersona = 0;
            string nomCom = "";
            string tabla = "";
            string datoextra = "";
            var cosine = new Cosine(2);
            double r = 0;
            var list = new List<Tuple<string, string, int, string, double>>();

            List<string> listaNombre = new List<string>();


            foreach (var q in query)
            {
                r = cosine.Similarity(q.nomcom, nombreCompleto);
                if (r >= 0.87)
                {
                    nomCom = q.nomcom;
                    idpersona = q.id;
                    tabla = q.NomTabla;
                    datoextra = q.datoExtra;
                    list.Add(new Tuple<string, string, int, string, double>(nomCom, tabla, idpersona, datoextra, r));
                    simi = true;
                }
            }
            List<object> elementos = new List<object>();
            if (list.Count() != 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    double porcentaje = item.Item5 * 100;
                    int porcentajeFloor = (int)Math.Floor(porcentaje);
                    string nombreP = item.Item1;
                    string tablaP = item.Item2;
                    int idPersona = item.Item3;
                    string datoextraP = item.Item4;

                    elementos.Add(new { Id = idPersona, Nombre = nombreP, Tabla = tablaP, Dato = datoextraP });
                }
                return Json(new { success = true, lista = elementos });
            }
            return Json(new { success = false, lista = elementos });
        }
        #endregion



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
        public async Task<IActionResult> Create(IFormFile evidencia, [Bind("Nombre,Paterno,Materno,Sexo,Edad,Calle,Colonia,Domicilio,ClaveUnicaScorpio,Telefono,Papa,Mama,Ubicacion,Delito,UnidadInvestigacion,FechaDetencion,Situacion,RealizoEntrevista,TipoDetenido,Aer,Tamizaje,Rcomparesencia,Rvictima,Robstaculizacion,Recomendacion,Antecedentes,AntecedentesDatos,Observaciones, FechaNacimiento, LnMunicipio, LnEstado")] Serviciospreviosjuicio serviciospreviosjuicio)
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
            serviciospreviosjuicio.FechaNacimiento = serviciospreviosjuicio.FechaNacimiento;
            serviciospreviosjuicio.ClaveUnicaScorpio = serviciospreviosjuicio.ClaveUnicaScorpio;

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
                string file_name = idAER +"_"+ serviciospreviosjuicio.Paterno + "_" + serviciospreviosjuicio.Materno + "_" + serviciospreviosjuicio.Nombre + Path.GetExtension(evidencia.FileName);
                file_name = mg.replaceSlashes(file_name);
                serviciospreviosjuicio.RutaAer = file_name;
                var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "AER");
                var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                await evidencia.CopyToAsync(stream);
                stream.Close();
            }
            #endregion

            _context.Add(serviciospreviosjuicio);
            await _context.SaveChangesAsync();
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
            ViewBag.idUnidad= mg.BuscaId(ListaUnidadInvestigación, serviciospreviosjuicio.UnidadInvestigacion);

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
        public async Task<IActionResult> Edit(IFormFile evidencia, [Bind("IdserviciosPreviosJuicio,Nombre,Paterno,Materno,Sexo,Edad,Calle,Colonia,Domicilio, ClaveUnicaScorpio ,Telefono,Papa,Mama,Ubicacion,Delito,UnidadInvestigacion,FechaDetencion,Situacion,RealizoEntrevista,TipoDetenido,Aer,Tamizaje,Rcomparesencia,Rvictima,Robstaculizacion,Recomendacion,Antecedentes,AntecedentesDatos,Observaciones,serviciospreviosjuicioIdserviciospreviosjuicio,FechaCaptura,Usuario,, FechaNacimiento, LnMunicipio, LnEstado")] Serviciospreviosjuicio serviciospreviosjuicio)
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
                    serviciospreviosjuicio.FechaNacimiento = serviciospreviosjuicio.FechaNacimiento;


                    if (!(serviciospreviosjuicio.Paterno == null && serviciospreviosjuicio.Materno == null && serviciospreviosjuicio.FechaNacimiento == null && serviciospreviosjuicio.Sexo == null && serviciospreviosjuicio.LnEstado == null && serviciospreviosjuicio.Nombre == null))
                    {
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

    }
}
