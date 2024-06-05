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
using F23.StringSimilarity;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SautinSoft.Document.MailMerging;
using DocumentFormat.OpenXml.Office.Word;
using System.Data.SqlClient;
using scorpioweb.Data;
using DocumentFormat.OpenXml.EMMA;
using scorpioweb.Class;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using Microsoft.AspNetCore.Rewrite.Internal;
using ZXing.OneD;
using System.ComponentModel.DataAnnotations;
using Syncfusion.EJ2.Navigations;
using Syncfusion.EJ2.Linq;
using Microsoft.AspNetCore.SignalR;


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
        private List<SelectListItem> listaJuzgados = new List<SelectListItem>
        {
            new SelectListItem { Text = "NA", Value = "NA" },
            new SelectListItem { Text = "JUZGADO 1", Value = "JUZGADO 1" },
            new SelectListItem { Text = "JUZGADO 2", Value = "JUZGADO 2" },
            new SelectListItem { Text = "JUZGADO 3", Value = "JUZGADO 3" }
        };
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
        #endregion

        #region -PersonasCL-

        #region -Menu CL-
        public async Task<IActionResult> Menucl()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            Boolean flagCoordinador = false, flagMaster = false;
            string usuario = user.ToString();
            DateTime fechaInforme = (DateTime.Today).AddDays(5);
            DateTime fechaControl = (DateTime.Today).AddDays(3);
            DateTime fechaInformeCoordinador = (DateTime.Today).AddDays(30);
            DateTime fechaHoy = DateTime.Today;
            var fechaProcesal = DateTime.Now.AddMonths(-6);
            ViewBag.Warnings = 0;


            foreach (var rol in roles)
            {
                if (rol == "Masteradmin")
                {
                    flagMaster = true;
                }
            }


            #region -To List databases-

            List<Personacl> personaclVM = _context.Personacl.ToList();
            List<Supervisioncl> supervisionclVM = _context.Supervisioncl.ToList();
            List<Causapenalcl> causapenalclVM = _context.Causapenalcl.ToList();
            List<Planeacionestrategicacl> planeacionestrategicaclVM = _context.Planeacionestrategicacl.ToList();
            List<Beneficios> beneficiosclVM = _context.Beneficios.ToList();
            List<Domiciliocl> domicilioclVM = _context.Domiciliocl.ToList();
            List<Municipios> municipiosVM = _context.Municipios.ToList();
            List<Estados> estadosVM = _context.Estados.ToList();
            List<Personaclcausapenalcl> personacausapenalclVM = _context.Personaclcausapenalcl.ToList();



            List<Beneficios> queryBeneficios = (from b in beneficiosclVM
                                                group b by b.SupervisionclIdSupervisioncl into grp
                                                select grp.OrderByDescending(b => b.IdBeneficios).FirstOrDefault()).ToList();



            //var listaaudit = (from a in _context.Audit
            //                  join s in _context.Supervision on int.Parse(Regex.Replace(a.PrimaryKey, @"[^0-9]", "")) equals s.IdSupervision
            //                  where a.TableName == "Supervision" && a.NewValues.Contains("en espera de respuesta")
            //                  group a by int.Parse(Regex.Replace(a.PrimaryKey, @"[^0-9]", "")) into grp
            //                  select grp.OrderByDescending(a => a.Id).FirstOrDefault());
            #endregion

            #region -Jointables-

            var sinResolucion = from p in personaclVM
                                join d in domicilioclVM on p.IdPersonaCl equals d.PersonaclIdPersonacl
                                join e in estadosVM on int.Parse(d.Estado) equals e.Id
                                join m in municipiosVM on int.Parse(d.Municipio) equals m.Id
                                where p.TieneResolucion == "NO" || p.TieneResolucion == "NA" || p.TieneResolucion is null
                                select new PlaneacionclWarningViewModel
                                {
                                    personaclVM = p,
                                    estadosVM = e,
                                    municipiosVM = m,
                                    tipoAdvertencia = "Sin Resolución"
                                };
            var sinResolucion2 = from p in personaclVM
                                 join d in domicilioclVM on p.IdPersonaCl equals d.PersonaclIdPersonacl
                                 join e in estadosVM on int.Parse(d.Estado) equals e.Id
                                 join m in municipiosVM on int.Parse(d.Municipio) equals m.Id
                                 where (p.TieneResolucion == "NO" || p.TieneResolucion == "NA" || p.TieneResolucion is null) && p.Supervisor == usuario
                                 select new PlaneacionclWarningViewModel
                                 {
                                     personaclVM = p,
                                     estadosVM = e,
                                     municipiosVM = m,
                                     tipoAdvertencia = "Sin Resolución"
                                 };


            var leftJoin = from persona in personaclVM
                           join domicilio in domicilioclVM on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                           join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                           join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl into tmp
                           from sinsupervision in tmp.DefaultIfEmpty()
                           select new PlaneacionclWarningViewModel
                           {
                               personaclVM = persona,
                               supervisionclVM = sinsupervision,
                               municipiosVM = municipio,
                               tipoAdvertencia = "Sin supervisión"
                           };

            var where = from ss in leftJoin
                        where ss.supervisionclVM == null
                        select new PlaneacionclWarningViewModel
                        {
                            personaclVM = ss.personaclVM,
                            supervisionclVM = ss.supervisionclVM,
                            municipiosVM = ss.municipiosVM,
                            tipoAdvertencia = "Sin supervisión"
                        };

            var where2 = from ss in leftJoin
                         where ss.personaclVM.Supervisor == usuario && ss.supervisionclVM == null
                         select new PlaneacionclWarningViewModel
                         {
                             personaclVM = ss.personaclVM,
                             supervisionclVM = ss.supervisionclVM,
                             municipiosVM = ss.municipiosVM,
                             tipoAdvertencia = "Sin supervisión"
                         };

            var personasConSupervision = from persona in personaclVM
                                         join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                                         join Beneficios in beneficiosclVM on supervision.IdSupervisioncl equals Beneficios.SupervisionclIdSupervisioncl
                                         select new Supervisioncl
                                         {
                                             IdSupervisioncl = supervision.IdSupervisioncl
                                         };


            List<int> idSupervisiones = personasConSupervision.Select(x => x.IdSupervisioncl).Distinct().ToList();

            var joins = from persona in personaclVM
                        join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                        join domicilio in domicilioclVM on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                        join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                        join causapenal in causapenalclVM on supervision.CausaPenalclIdCausaPenalcl equals causapenal.IdCausaPenalcl
                        select new PlaneacionclWarningViewModel
                        {
                            personaclVM = persona,
                            supervisionclVM = supervision,
                            municipiosVM = municipio,
                            causapenalclVM = causapenal,
                            tipoAdvertencia = "Sin figura judicial"
                        };

            var table = from persona in personaclVM
                        join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                        join domicilio in domicilioclVM on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                        join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                        join causapenal in causapenalclVM on supervision.CausaPenalclIdCausaPenalcl equals causapenal.IdCausaPenalcl
                        join planeacion in planeacionestrategicaclVM on supervision.IdSupervisioncl equals planeacion.SupervisionclIdSupervisioncl
                        join Beneficios in queryBeneficios on supervision.IdSupervisioncl equals Beneficios.SupervisionclIdSupervisioncl
                        select new PlaneacionclWarningViewModel
                        {
                            personaclVM = persona,
                            supervisionclVM = supervision,
                            causapenalclVM = causapenal,
                            planeacionestrategicaclVM = planeacion,
                            beneficiosclVM = Beneficios,
                            figuraJudicial = Beneficios.FiguraJudicial,
                            municipiosVM = municipio
                        };

            if (usuario == "andrea.valdez@dgepms.com" || flagMaster == true)
            {
                var warningPlaneacion = (where).Union
                                               (sinResolucion).Union
                                               (joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionclVM.IdSupervisioncl) && s.supervisionclVM.EstadoSupervision == "VIGENTE")).Union
                                               //(from persona in personaVM
                                               // join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                               // join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                               // where persona.Colaboracion == "SI"
                                               // select new PlaneacionWarningViewModel
                                               // {
                                               //     personaVM = persona,
                                               //     municipiosVM = municipio,
                                               //     tipoAdvertencia = "Pendiente de asignación - colaboración"
                                               // }).Union
                                               (from t in table
                                                where t.planeacionestrategicaclVM.InformeInicial != null && t.planeacionestrategicaclVM.InformeInicial < fechaInformeCoordinador && t.supervisionclVM.EstadoSupervision == "VIGENTE"
                                                orderby t.planeacionestrategicaclVM.InformeInicial
                                                select new PlaneacionclWarningViewModel
                                                {
                                                    personaclVM = t.personaclVM,
                                                    supervisionclVM = t.supervisionclVM,
                                                    municipiosVM = t.municipiosVM,
                                                    causapenalclVM = t.causapenalclVM,
                                                    planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                    beneficiosclVM = t.beneficiosclVM,
                                                    figuraJudicial = t.figuraJudicial,
                                                    tipoAdvertencia = "Informe fuera de tiempo"
                                                }).Union
                                                (from t in table
                                                 where t.planeacionestrategicaclVM.InformeInicial != null && t.planeacionestrategicaclVM.InformeInicial < fechaControl && t.supervisionclVM.EstadoSupervision == "VIGENTE" && t.beneficiosclVM.FiguraJudicial != "SUSTITUCIÓN DE LA PENA"
                                                 select new PlaneacionclWarningViewModel
                                                 {
                                                     personaclVM = t.personaclVM,
                                                     supervisionclVM = t.supervisionclVM,
                                                     municipiosVM = t.municipiosVM,
                                                     causapenalclVM = t.causapenalclVM,
                                                     planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                     beneficiosclVM = t.beneficiosclVM,
                                                     figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                     tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                                 }).Union
                                                (from t in table
                                                 where t.planeacionestrategicaclVM.InformeInicial == null && t.supervisionclVM.EstadoSupervision == "VIGENTE"
                                                 orderby t.beneficiosclVM.FiguraJudicial
                                                 select new PlaneacionclWarningViewModel
                                                 {
                                                     personaclVM = t.personaclVM,
                                                     supervisionclVM = t.supervisionclVM,
                                                     municipiosVM = t.municipiosVM,
                                                     causapenalclVM = t.causapenalclVM,
                                                     planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                     beneficiosclVM = t.beneficiosclVM,
                                                     figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                     tipoAdvertencia = "Sin fecha de informe"
                                                 }).Union
                                                (from persona in personaclVM
                                                 join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                                                 join domicilio in domicilioclVM on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                                                 join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                                 join causapenal in causapenalclVM on supervision.CausaPenalclIdCausaPenalcl equals causapenal.IdCausaPenalcl
                                                 join planeacion in planeacionestrategicaclVM on supervision.IdSupervisioncl equals planeacion.SupervisionclIdSupervisioncl
                                                 where planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                                 select new PlaneacionclWarningViewModel
                                                 {
                                                     personaclVM = persona,
                                                     supervisionclVM = supervision,
                                                     municipiosVM = municipio,
                                                     causapenalclVM = causapenal,
                                                     planeacionestrategicaclVM = planeacion,
                                                     tipoAdvertencia = "Sin periodicidad de firma"
                                                 }).Union
                                                (from t in table
                                                 where t.personaclVM.Supervisor != null && t.personaclVM.Supervisor != null && t.personaclVM.Supervisor.EndsWith("\u0040dgepms.com") && t.planeacionestrategicaclVM.FechaProximoContacto != null && t.planeacionestrategicaclVM.FechaProximoContacto < fechaHoy && t.supervisionclVM.EstadoSupervision == "VIGENTE" && t.planeacionestrategicaclVM.PeriodicidadFirma != "NO APLICA"
                                                 orderby t.planeacionestrategicaclVM.FechaProximoContacto descending
                                                 select new PlaneacionclWarningViewModel
                                                 {
                                                     personaclVM = t.personaclVM,
                                                     supervisionclVM = t.supervisionclVM,
                                                     municipiosVM = t.municipiosVM,
                                                     causapenalclVM = t.causapenalclVM,
                                                     planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                     beneficiosclVM = t.beneficiosclVM,
                                                     figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                     tipoAdvertencia = "Se paso el tiempo de la firma"
                                                 });//.Union
                                                    //(where).Union
                                                    //(archivoadmin).Union
                                                    //(joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionVM.IdSupervision) && s.supervisionVM.EstadoSupervision == "VIGENTE")).Union
                                                    //(from t in tableAudiot
                                                    // where t.personaVM.Supervisor != null && t.auditVM.DateTime < fechaProcesal && t.supervisionVM.EstadoSupervision != "CONCLUIDO"
                                                    // select new PlaneacionWarningViewModel
                                                    // {
                                                    //     personaVM = t.personaVM,
                                                    //     municipiosVM = t.municipiosVM,
                                                    //     supervisionVM = t.supervisionVM,
                                                    //     causapenalVM = t.causapenalVM,
                                                    //     planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                    //     fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                    //     tipoAdvertencia = "Estado Procesal",
                                                    //     auditVM = t.auditVM
                                                    // });
                var warnings = Enumerable.Empty<PlaneacionclWarningViewModel>();
                if (usuario == "andrea.valdez@dgepms.com" || flagMaster == true)
                {
                    var filteredWarnings = from pwvm in warningPlaneacion
                                           where pwvm.personaclVM.Supervisor != null && pwvm.personaclVM.Supervisor.EndsWith("\u0040dgepms.com")
                                           select pwvm;
                    warnings = warnings.Union(filteredWarnings);
                }

                ViewBag.Warnings = warnings.Count();
            }
            else
            {
                var warningPlaneacion = (where2).Union
                                        (sinResolucion2).Union
                                        //(archivo).Union
                                        (joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionclVM.IdSupervisioncl) && s.personaclVM.Supervisor == usuario && s.supervisionclVM.EstadoSupervision == "VIGENTE")).Union
                                          (from t in table
                                           where t.planeacionestrategicaclVM.InformeInicial != null && t.planeacionestrategicaclVM.InformeInicial < fechaInformeCoordinador && t.supervisionclVM.EstadoSupervision == "VIGENTE" && usuario == t.personaclVM.Supervisor
                                           orderby t.planeacionestrategicaclVM.InformeInicial
                                           select new PlaneacionclWarningViewModel
                                           {
                                               personaclVM = t.personaclVM,
                                               supervisionclVM = t.supervisionclVM,
                                               municipiosVM = t.municipiosVM,
                                               causapenalclVM = t.causapenalclVM,
                                               planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                               beneficiosclVM = t.beneficiosclVM,
                                               figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                               tipoAdvertencia = "Informe fuera de tiempo"
                                           }).Union
                                          (from t in table
                                           where t.planeacionestrategicaclVM.InformeInicial != null && t.planeacionestrategicaclVM.InformeInicial < fechaControl && t.supervisionclVM.EstadoSupervision == "VIGENTE" && t.beneficiosclVM.FiguraJudicial != "SUSTITUCIÓN DE LA PENA" && usuario == t.personaclVM.Supervisor
                                           select new PlaneacionclWarningViewModel
                                           {
                                               personaclVM = t.personaclVM,
                                               supervisionclVM = t.supervisionclVM,
                                               municipiosVM = t.municipiosVM,
                                               causapenalclVM = t.causapenalclVM,
                                               planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                               beneficiosclVM = t.beneficiosclVM,
                                               figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                               tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                           }).Union
                                          (from t in table
                                           where t.planeacionestrategicaclVM.InformeInicial == null && t.supervisionclVM.EstadoSupervision == "VIGENTE" && usuario == t.personaclVM.Supervisor
                                           orderby t.beneficiosclVM.FiguraJudicial
                                           select new PlaneacionclWarningViewModel
                                           {
                                               personaclVM = t.personaclVM,
                                               supervisionclVM = t.supervisionclVM,
                                               municipiosVM = t.municipiosVM,
                                               causapenalclVM = t.causapenalclVM,
                                               planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                               beneficiosclVM = t.beneficiosclVM,
                                               figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                               tipoAdvertencia = "Sin fecha de informe"
                                           }).Union
                                          (from persona in personaclVM
                                           join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                                           join domicilio in domicilioclVM on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                                           join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                           join causapenal in causapenalclVM on supervision.CausaPenalclIdCausaPenalcl equals causapenal.IdCausaPenalcl
                                           join planeacion in planeacionestrategicaclVM on supervision.IdSupervisioncl equals planeacion.SupervisionclIdSupervisioncl
                                           where planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE" && usuario == persona.Supervisor
                                           select new PlaneacionclWarningViewModel
                                           {
                                               personaclVM = persona,
                                               supervisionclVM = supervision,
                                               municipiosVM = municipio,
                                               causapenalclVM = causapenal,
                                               planeacionestrategicaclVM = planeacion,
                                               tipoAdvertencia = "Sin periodicidad de firma"
                                           }).Union
                                          (from t in table
                                           where t.personaclVM.Supervisor != null && t.personaclVM.Supervisor != null && t.personaclVM.Supervisor.EndsWith("\u0040dgepms.com") && t.planeacionestrategicaclVM.FechaProximoContacto != null && t.planeacionestrategicaclVM.FechaProximoContacto < fechaHoy && t.supervisionclVM.EstadoSupervision == "VIGENTE" && t.planeacionestrategicaclVM.PeriodicidadFirma != "NO APLICA" && usuario == t.personaclVM.Supervisor
                                           orderby t.planeacionestrategicaclVM.FechaProximoContacto descending
                                           select new PlaneacionclWarningViewModel
                                           {
                                               personaclVM = t.personaclVM,
                                               supervisionclVM = t.supervisionclVM,
                                               municipiosVM = t.municipiosVM,
                                               causapenalclVM = t.causapenalclVM,
                                               planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                               beneficiosclVM = t.beneficiosclVM,
                                               figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                               tipoAdvertencia = "Se paso el tiempo de la firma"
                                           });
                //(from t in tableAudiot
                // where t.personaVM.Supervisor == usuario && t.personaVM.Supervisor != null && t.auditVM.DateTime < fechaProcesal && t.supervisionVM.EstadoSupervision != "CONCLUIDO"
                // select new PlaneacionWarningViewModel
                // {
                //     personaVM = t.personaVM,
                //     municipiosVM = t.municipiosVM,
                //     supervisionVM = t.supervisionVM,
                //     causapenalVM = t.causapenalVM,
                //     planeacionestrategicaVM = t.planeacionestrategicaVM,
                //     fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                //     tipoAdvertencia = "Estado Procesal",
                //     auditVM = t.auditVM
                // });
                ViewBag.Warnings = warningPlaneacion.Count();
            }

            #endregion

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
        #endregion

        #region -Index-
        // GET: Personacls
        public async Task<IActionResult> Index()
        {


            var nomsuper = User.Identity.Name.ToString();

            #region -Sacar USUARIOS ADMIN-
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.user = user;
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.Admin = false;
            foreach (var rol in roles)
            {
                if (rol == "AdminLC")
                {
                    ViewBag.Admin = true;
                }
            }
            #endregion



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
            bool invitado = true;

            foreach (var rol in roles)
            {
                if (rol == "SupervisorLC")
                {
                    super = true;
                }
            }
            foreach (var rol in roles)
            {
                if (rol == "AdminLC" || rol == "Masteradmin")
                {
                    admin = true;
                }
            }

            foreach (var rol in roles)
            {
                if (rol == "AdminLC" || rol == "Masteradmin" || rol == "SupervisorLC")
                {
                    // SE VUELVE FALSO SI EL USUARIO TIENE UN ROL DE CONDICIONES EN LIBERTAD 
                    invitado = false;
                    break;

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
                           select p;

            var canalizadoIds = (from p in personas
                                 join re in _context.Reinsercion on p.IdPersonaCl equals Int32.Parse(re.IdTabla)
                                 join c in _context.Canalizacion on re.IdReinsercion equals c.ReincercionIdReincercion
                                 where re.Tabla == "personacl"
                                 select p.IdPersonaCl).ToList();




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

            if (usuario == true)
            {
                personas = personas.Where(p => p.Supervisor == nomsuper);
            };


            if (admin == true)
            {
                personas = personas.Where(p => p.Supervisor != null);
            };


            int pageSize = 10;
            // Response.Headers.Add("Refresh", "5");
            return Json(new
            {
                page = await PaginatedList<Personacl>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize),
                totalPages = (personas.Count() + pageSize - 1) / pageSize,
                admin,
                super,
                invitado,
                nomsuper = nomsuper,
                canalizados = canalizadoIds // Añadimos los IDs canalizados aquí
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

        #region -Detalles-
        // GET: Personacls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var TieneRegistroPsicologia = await _context.Psicologiavincu.SingleOrDefaultAsync(u => u.PersonaClIdPersonaCl == id);
            if (TieneRegistroPsicologia != null)
            {
                ViewBag.PsicologiaVinculacion = TieneRegistroPsicologia;
            }
            else
            {
                Psicologiavincu vinculacion = new Psicologiavincu();
                ViewBag.PsicologiaVinculacion = vinculacion;
            }


            var personacl = await _context.Personacl
                .SingleOrDefaultAsync(m => m.IdPersonaCl == id);

            var domicilioEM = await _context.Domiciliocl.SingleOrDefaultAsync(d => d.PersonaclIdPersonacl == id);

            #region -To List databases-

            List<Personacl> personaclVM = _context.Personacl.ToList();
            List<Domiciliocl> domicilioclVM = _context.Domiciliocl.ToList();
            List<Estudioscl> estudiosclVM = _context.Estudioscl.ToList();
            List<Estados> estados = _context.Estados.ToList();
            List<Municipios> municipios = _context.Municipios.ToList();
            List<Domiciliosecundariocl> domicilioSecundarioclVM = _context.Domiciliosecundariocl.ToList();
            List<Consumosustanciascl> consumoSustanciasclVM = _context.Consumosustanciascl.ToList();
            List<Trabajocl> trabajoclVM = _context.Trabajocl.ToList();
            List<Actividadsocialcl> actividadSocialclVM = _context.Actividadsocialcl.ToList();
            List<Abandonoestadocl> abandonoEstadoclVM = _context.Abandonoestadocl.ToList();
            List<Saludfisicacl> saludFisicaclVM = _context.Saludfisicacl.ToList();
            List<Familiaresforaneoscl> familiaresForaneosclVM = _context.Familiaresforaneoscl.ToList();
            List<Asientofamiliarcl> asientoFamiliarclVM = _context.Asientofamiliarcl.ToList();

            #endregion


            #region -Jointables-

            ViewData["joinTables"] = from PersonaCLTable in personaclVM
                                     join DomicilioCL in domicilioclVM on personacl.IdPersonaCl equals DomicilioCL.PersonaclIdPersonacl
                                     join EstudiosCL in estudiosclVM on personacl.IdPersonaCl equals EstudiosCL.PersonaClIdPersonaCl
                                     join TrabajoCL in trabajoclVM on personacl.IdPersonaCl equals TrabajoCL.PersonaClIdPersonaCl
                                     join ActividaSocialCL in actividadSocialclVM on personacl.IdPersonaCl equals ActividaSocialCL.PersonaClIdPersonaCl
                                     join AbandonoEstadoCL in abandonoEstadoclVM on personacl.IdPersonaCl equals AbandonoEstadoCL.PersonaclIdPersonacl
                                     join SaludFisicaCL in saludFisicaclVM on personacl.IdPersonaCl equals SaludFisicaCL.PersonaClIdPersonaCl
                                     //join nacimientoEstado in estados on (Int32.Parse(persona.Lnestado)) equals nacimientoEstado.Id
                                     //join nacimientoMunicipio in municipios on (Int32.Parse(persona.Lnmunicipio)) equals nacimientoMunicipio.Id
                                     join domicilioEstado in estados on (Int32.Parse(DomicilioCL.Estado)) equals domicilioEstado.Id
                                     join domicilioMunicipio in municipios on (Int32.Parse(DomicilioCL.Municipio)) equals domicilioMunicipio.Id
                                     where PersonaCLTable.IdPersonaCl == id
                                     select new PersonaclsViewModal
                                     {
                                         personaclVM = PersonaCLTable,
                                         domicilioclVM = DomicilioCL,
                                         estudiosclVM = EstudiosCL,
                                         trabajoclVM = TrabajoCL,
                                         actividadSocialclVM = ActividaSocialCL,
                                         abandonoEstadoclVM = AbandonoEstadoCL,
                                         saludFisicaclVM = SaludFisicaCL,
                                         //estadosVMPersona=nacimientoEstado,
                                         //municipiosVMPersona=nacimientoMunicipio,  
                                         estadosVMDomicilio = domicilioEstado,
                                         municipiosVMDomicilio = domicilioMunicipio
                                     };

            #endregion


            #region Sacar el nombre de estdo y municipio (NACIMIENTO)

            var LNE = (from e in _context.Estados
                       join p in _context.Personacl on e.Id equals int.Parse(p.Lnestado)
                       where p.IdPersonaCl == id
                       select new
                       {
                           e.Estado
                       });

            string selectem1 = LNE.FirstOrDefault().Estado.ToString();
            ViewBag.lnestado = selectem1.ToUpper();

            var LNM = (from p in _context.Personacl
                       join d in _context.Domiciliocl on p.IdPersonaCl equals d.PersonaclIdPersonacl
                       join m in _context.Municipios on p.Lnmunicipio equals m.Id.ToString()
                       where p.IdPersonaCl == id
                       select new
                       {
                           m.Municipio
                       });



            string selectem2 = LNM.FirstOrDefault().Municipio.ToString();
            ViewBag.lnmunicipio = selectem2.ToUpper();

            #endregion

            #region Sacar el nombre de estdo y municipio (DOMICILIO)

            var E = (from d in _context.Domiciliocl
                     join m in _context.Estados on int.Parse(d.Estado) equals m.Id
                     join p in _context.Personacl on d.PersonaclIdPersonacl equals p.IdPersonaCl
                     where p.IdPersonaCl == id
                     select new
                     {
                         m.Estado
                     });

            string selectem3 = E.FirstOrDefault().Estado.ToString();
            ViewBag.estado = selectem3.ToUpper();

            var M = (from d in _context.Domiciliocl
                     join m in _context.Municipios on int.Parse(d.Municipio) equals m.Id
                     join p in _context.Personacl on d.PersonaclIdPersonacl equals p.IdPersonaCl
                     where p.IdPersonaCl == id
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
            bool success = Int32.TryParse(personacl.Lnestado, out Lnestado);
            List<Municipios> listaMunicipios = new List<Municipios>();
            if (success)
            {
                listaMunicipios = (from table in _context.Municipios
                                   where table.EstadosId == Lnestado
                                   select table).ToList();
            }

            listaMunicipios.Insert(0, new Municipios { Id = null, Municipio = "Selecciona" });

            ViewBag.ListadoMunicipios = listaMunicipios;
            ViewBag.idMunicipio = personacl.Lnmunicipio;

            #endregion

            #region -JoinTables null-

            ViewData["joinTablesDomSec"] = from PersonaCLTable in personaclVM
                                           join DomicilioCL in domicilioclVM on personacl.IdPersonaCl equals DomicilioCL.PersonaclIdPersonacl
                                           join DomicilioSecCL in domicilioSecundarioclVM.DefaultIfEmpty() on DomicilioCL.IdDomiciliocl equals DomicilioSecCL.IdDomicilioCl
                                           where PersonaCLTable.IdPersonaCl == id
                                           select new PersonaclsViewModal
                                           {
                                               domicilioSecundarioclVM = DomicilioSecCL
                                           };

            ViewData["joinTablesConsumoSustancias"] = from PersonaCLTable in personaclVM
                                                      join SustanciasCL in consumoSustanciasclVM on personacl.IdPersonaCl equals SustanciasCL.PersonaClIdPersonaCl
                                                      where PersonaCLTable.IdPersonaCl == id
                                                      select new PersonaclsViewModal
                                                      {
                                                          consumoSustanciasclVM = SustanciasCL
                                                      };

            ViewData["joinTablesFamiliaresForaneos"] = from PersonaCLTable in personaclVM
                                                       join FamiliarForaneoCL in familiaresForaneosclVM on personacl.IdPersonaCl equals FamiliarForaneoCL.PersonaClIdPersonaCl
                                                       where PersonaCLTable.IdPersonaCl == id
                                                       select new PersonaclsViewModal
                                                       {
                                                           familiaresForaneosclVM = FamiliarForaneoCL
                                                       };

            ViewData["joinTablesFamiliares"] = from PersonaCLTable in personaclVM
                                               join FamiliarCL in asientoFamiliarclVM on personacl.IdPersonaCl equals FamiliarCL.PersonaClIdPersonaCl
                                               where PersonaCLTable.IdPersonaCl == id && FamiliarCL.Tipo == "FAMILIAR"
                                               select new PersonaclsViewModal
                                               {
                                                   asientoFamiliarclVM = FamiliarCL
                                               };

            ViewData["joinTablesReferencia"] = from PersonaCLTable in personaclVM
                                               join ReferenciaCL in asientoFamiliarclVM on personacl.IdPersonaCl equals ReferenciaCL.PersonaClIdPersonaCl
                                               where PersonaCLTable.IdPersonaCl == id && ReferenciaCL.Tipo == "REFERENCIA"
                                               select new PersonaclsViewModal
                                               {
                                                   asientoFamiliarclVM = ReferenciaCL
                                               };


            ViewBag.Referencia = ((ViewData["joinTablesReferencia"] as IEnumerable<scorpioweb.Models.PersonaclsViewModal>).Count()).ToString();

            ViewBag.Familiar = ((ViewData["joinTablesFamiliares"] as IEnumerable<scorpioweb.Models.PersonaclsViewModal>).Count()).ToString();

            #endregion

            if (personacl == null)
            {
                return NotFound();
            }

            return View(/*personacl*/);
        }
        #endregion

        #region -Psicologia Vinculacion-
        [HttpPost]
        public async Task<JsonResult> PsicologiaVinculacion(string valor, string NombreCampo, int IdPersonaCL, Psicologiavincu Vinculacion)
        {
            if (String.IsNullOrEmpty(valor))
            {
                return Json(new { success = false, message = "El valor está vacío o nulo." });
            }
            else
            {
                var TieneRegistro = await _context.Psicologiavincu.SingleOrDefaultAsync(u => u.PersonaClIdPersonaCl == IdPersonaCL);

                if (TieneRegistro != null)
                {


                    switch (NombreCampo)
                    {
                        case "RelacionPH":
                            TieneRegistro.RParejahijo = mg.normaliza(valor);

                            break;
                        case "RelacionMPH":
                            TieneRegistro.RPadremadre = mg.normaliza(valor);

                            break;

                        case "SienteEmocionalmente":
                            TieneRegistro.Emocionalmente = mg.normaliza(valor);

                            break;

                        case "selectApoyoPsicologico":
                            TieneRegistro.ApoyoPsicologica = mg.normaliza(valor);

                            break;

                        case "selectINE":
                            TieneRegistro.IneVigente = mg.normaliza(valor);
                            if (valor.Equals("SI"))
                                TieneRegistro.ApoyoTramite = "NO";
                            break;

                        case "INE":
                            TieneRegistro.ApoyoTramite = mg.normaliza(valor);

                            break;

                        case "selectAsesoria":
                            TieneRegistro.AsesoriaJuri = mg.normaliza(valor);
                            if (valor.Equals("NO"))
                                TieneRegistro.CualAsesoriaJuri = "NA";
                            break;

                        case "Asesoria":
                            TieneRegistro.CualAsesoriaJuri = mg.normaliza(valor);

                            break;

                    }


                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                }
                else
                {
                    switch (NombreCampo)
                    {
                        case "RelacionPH":
                            Vinculacion.RParejahijo = mg.normaliza(valor);

                            break;
                        case "RelacionMPH":
                            Vinculacion.RPadremadre = mg.normaliza(valor);

                            break;

                        case "SienteEmocionalmente":
                            Vinculacion.Emocionalmente = mg.normaliza(valor);

                            break;

                        case "selectApoyoPsicologico":
                            Vinculacion.ApoyoPsicologica = mg.normaliza(valor);

                            break;

                        case "selectINE":
                            Vinculacion.IneVigente = mg.normaliza(valor);

                            break;

                        case "INE":
                            Vinculacion.ApoyoTramite = mg.normaliza(valor);

                            break;

                        case "selectAsesoria":
                            Vinculacion.AsesoriaJuri = mg.normaliza(valor);

                            break;

                        case "Asesoria":
                            Vinculacion.CualAsesoriaJuri = mg.normaliza(valor);

                            break;

                    }
                    Vinculacion.PersonaClIdPersonaCl = IdPersonaCL;
                    _context.Psicologiavincu.Add(Vinculacion);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }

            }
            return Json(new { success = true, message = "¡Operación exitosa!" });
        }


        #endregion

        #region -AsignaSupervision-
        public async Task<IActionResult> AsignacionSupervision()
        {
            var usu = await userManager.FindByNameAsync(User.Identity.Name);

            #region -Solicitud Atendida Archivo prestamo Digital-

            var warningRespuesta = from a in _context.Archivoprestamodigital
                                   where a.EstadoPrestamo == 1 && usu.ToString().ToUpper() == a.Usuario.ToUpper()
                                   select a;
            ViewBag.WarningsUser = warningRespuesta.Count();
            #endregion
            List<SelectListItem> ListaUsuarios = new List<SelectListItem>();
            int i = 0;
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, "AdminLC"))
                {
                    ListaUsuarios.Add(new SelectListItem
                    {
                        Text = user.ToString(),
                        Value = i.ToString()
                    });
                    i++;
                }
                if (await userManager.IsInRoleAsync(user, "SupervisorLC"))
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
            return View(await _context.Personacl.ToListAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupervisor(Personacl personacl)
        {
            try
            {

                int Id = personacl.IdPersonaCl;
                string supervisor = personacl.Supervisor;
                var personaUpdate = await _context.Personacl.FirstOrDefaultAsync(p => p.IdPersonaCl == Id);
                personaUpdate.Supervisor = supervisor;
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
            return RedirectToAction("Menucl");
        }


        #endregion

        #region -Reasignacion-
        public async Task<IActionResult> Reasignacioncl(
           string sortOrder,
           string currentFilter,
           string searchString,
           int? pageNumber)
        {

            List<SelectListItem> ListaUsuarios = new List<SelectListItem>();
            int i = 0;
            foreach (var usuario in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(usuario, "SupervisorLC"))
                {
                    ListaUsuarios.Add(new SelectListItem
                    {
                        Text = usuario.ToString(),
                        Value = i.ToString()
                    });
                    i++;
                }
            }
            ViewBag.ListadoUsuarios = ListaUsuarios;

            //var queryhayhuella = from r in _context.Registrohuella
            //                     join p in _context.Presentacionperiodica on r.IdregistroHuella equals p.RegistroidHuella
            //                     group r by r.PersonaIdPersona into grup
            //                     select new
            //                     {
            //                         grup.Key,
            //                         Count = grup.Count()
            //                     };

            //foreach (var personaHuella in queryhayhuella)
            //{
            //    if (personaHuella.Count >= 1)
            //    {

            //        ViewBag.personaIdPersona = personaHuella.Key;
            //    };
            //}


            //#region -ListaUsuarios-        
            //var user = await userManager.FindByNameAsync(User.Identity.Name);
            //var roles = await userManager.GetRolesAsync(user);
            //ViewBag.Admin = false;
            //ViewBag.Masteradmin = false;
            //ViewBag.Archivo = false;

            //foreach (var rol in roles)
            //{
            //    if (rol == "AdminMCSCP")
            //    {
            //        ViewBag.Admin = true;
            //    }
            //}
            //foreach (var rol in roles)
            //{
            //    if (rol == "Masteradmin")
            //    {
            //        ViewBag.Masteradmin = true;
            //    }
            //}
            //foreach (var rol in roles)
            //{
            //    if (rol == "ArchivoMCSCP")
            //    {
            //        ViewBag.Archivo = true;
            //    }
            //}
            //#endregion

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

            var personas = from p in _context.Personacl
                           where p.Supervisor != null
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                foreach (var item in searchString.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(searchString) ||
                                                   (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(searchString) ||
                                                   p.Supervisor.Contains(searchString) || (p.IdPersonaCl.ToString()).Contains(searchString));

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

            int pageSize = 10;
            // Response.Headers.Add("Refresh", "5");
            return View(await PaginatedList<Personacl>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupervisorReasignacion(Personacl persona, Reasignacionsupervisor reasignacionsupervisor)
        {
            int id = persona.IdPersonaCl;
            string idS = (persona.IdPersonaCl).ToString();
            string supervisor = persona.Supervisor;
            string motivo = reasignacionsupervisor.MotivoRealizo;
            string currentUser = User.Identity.Name;

            var sA = from p in _context.Personacl
                     where p.IdPersonaCl == id
                     select new
                     {
                         p.Supervisor
                     };

            reasignacionsupervisor.PersonaIdpersona = (persona.IdPersonaCl).ToString();
            reasignacionsupervisor.MotivoRealizo = reasignacionsupervisor.MotivoRealizo;
            reasignacionsupervisor.FechaReasignacion = DateTime.Now;
            reasignacionsupervisor.SAntiguo = sA.FirstOrDefault().Supervisor.ToString();
            reasignacionsupervisor.QuienRealizo = currentUser;
            reasignacionsupervisor.SNuevo = supervisor;
            _context.Add(reasignacionsupervisor);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);

            var personaUpdate = await _context.Personacl
                .FirstOrDefaultAsync(p => p.IdPersonaCl == id);
            personaUpdate.Supervisor = supervisor;



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonaclExists(persona.IdPersonaCl))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Reasignacioncl");
        }

        #endregion

        #region -Colaboraciones-
        public async Task<IActionResult> Colaboraciones()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            ViewBag.UserAdmin = false;
            ViewBag.user = user;

            foreach (var rol in roles)
            {
                if (rol == "AdminLC" || rol == "Masteradmin")
                {
                    ViewBag.UserAdmin = true;
                }
                if (rol == "SupervisorLC")
                {
                    ViewBag.UserCL = false;
                }
            }





            #region -Solicitud Atendida Archivo prestamo Digital-
            var warningRespuesta = from a in _context.Archivoprestamodigital
                                   where a.EstadoPrestamo == 1 && user.ToString().ToUpper() == a.Usuario.ToUpper()
                                   select a;
            ViewBag.WarningsUser = warningRespuesta.Count();
            #endregion



            var colaboraciones = (from persona in _context.Personacl
                                  join domicilio in _context.Domiciliocl on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                                  join municipio in _context.Municipios on int.Parse(domicilio.Municipio) equals municipio.Id
                                  join supervision in _context.Supervisioncl on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                                  join beneficios in _context.Beneficios on supervision.IdSupervisioncl equals beneficios.SupervisionclIdSupervisioncl
                                  where /*beneficios.Tipo == "XIII" &&*/ supervision.EstadoSupervision == "VIGENTE" && beneficios.FiguraJudicial == "MEDIDA DE SEGURIDAD" && beneficios.FiguraJudicial == "SUSTITUCIÓN DE LA PRISION PREVENTIVA POR PRISION DOMICILIARIA"
                                  select new PersonaclsViewModal
                                  {
                                      personaclVM = persona,
                                      municipiosVMDomicilio = municipio,
                                      CasoEspecial = "Prision Domiciliaria"
                                  }).Union
                                (from persona in _context.Personacl
                                 join domicilio in _context.Domiciliocl on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                                 join municipio in _context.Municipios on int.Parse(domicilio.Municipio) equals municipio.Id
                                 where persona.Colaboracion == "SI"
                                 select new PersonaclsViewModal
                                 {
                                     personaclVM = persona,
                                     municipiosVMDomicilio = municipio,
                                     CasoEspecial = "Colaboración con " + persona.UbicacionExpediente
                                 });

            if (ViewBag.UserCL == false)
            {
                colaboraciones = colaboraciones.Where(p => p.personaclVM.Supervisor == user.ToString());
            }

            return View(colaboraciones);
        }
        #endregion

        #region -SinSupervision-
        public async Task<IActionResult> SinSupervision()
        {

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            bool invitado = true;

            foreach (var rol in roles)
            {
                if (rol == "AdminLC" || rol == "Masteradmin" || rol == "SupervisorLC")
                {
                    // SE VUELVE FALSO SI EL USUARIO TIENE UN ROL DE CONDICIONES EN LIBERTAD 
                    invitado = false;
                    break;

                }
            }

            ViewBag.EsInvitado = invitado;

            return View();
        }
        #endregion

        #region -obtenerDatos-
        public ActionResult OnGetChartData()
        {
            var resultados = from p in _context.Personacl
                             where p.Supervisor != null && p.Supervisor != ""
                             group p by p.Supervisor into grupo
                             select new
                             {
                                 Supervisor = grupo.Key,
                                 NumeroSupervisados = grupo.Count()
                             };
            var json = resultados.ToGoogleDataTable()
            .NewColumn(new Column(ColumnType.String, "Supervisor"), x => x.Supervisor)
            .NewColumn(new Column(ColumnType.Number, "Supervisiones"), x => x.NumeroSupervisados)
            .Build()
            .GetJson();

            return Content(json);
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
                if (rol == "AdminLC" || rol == "SupervisorLC")
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
                consumocl.Add(consumosustanciascl[i].UltimoConsumo?.ToString("yyyy-MM-ddTHH:mm:ss"));
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

            #region listaJuzgado          
            ViewBag.listaJuzgados = listaJuzgados;
            ViewBag.idJuzgados = BuscaId(listaJuzgados, personacl.Juzgado);

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

            #region -viewbags y listas-

            ViewBag.listaOtroIdioma = listaNoSi;
            ViewBag.idOtroIdioma = BuscaId(listaNoSi, personacl.OtroIdioma);

            ViewBag.listaColaboracion = listaNoSi;
            ViewBag.idColaboracion = BuscaId(listaNoSi, personacl.Colaboracion);

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

            ViewBag.listaTratamiento = listaNoSi;
            ViewBag.idTratamiento = BuscaId(listaNoSi, personacl.TratamientoAdicciones);

            ViewBag.listaSinoCentroPenitenciario = listaNoSi;
            ViewBag.idSinoCentroPenitenciario = BuscaId(listaNoSi, personacl.Sinocentropenitenciario);

            ViewBag.idCentroPenitenciario = personacl.Centropenitenciario;
            ViewBag.pais = personacl.Lnpais;

            ViewBag.idioma = personacl.OtroIdioma;
            ViewBag.EspecifiqueIdioma = personacl.EspecifiqueIdioma;


            ViewBag.traductor = personacl.Traductor;
            ViewBag.EspecifiqueTraductor = personacl.EspecifiqueTraductor;

            ViewBag.TieneHijos = personacl.Hijos;
            ViewBag.NumeroHijos = personacl.Nhijos;


            #endregion

            #region -botones edicion sustancias, familiares y referencias-

            string familiarTipo = "", referenciaTipo = "", SI = "SI", NO = "NO";

            //PARA SABER SI LA PERSONA TIENE REGISTRO DE SUSTANCIAS
            int NumeroConsumoSustancias = _context.Consumosustanciascl.Where(a => a.PersonaClIdPersonaCl == id).Count();
            if (NumeroConsumoSustancias >= 1)
            {
                ViewBag.ConsumoSustancias = BuscaId(listaNoSi, SI);
            }
            else
            {
                ViewBag.ConsumoSustancias = BuscaId(listaNoSi, NO);
            }

            //CUENTA SI LA PERSONA TIENE FAMILIARES O REFERENCIAS EN ASIENTO FAMILIAR
            int NumeroFamiliares = _context.Asientofamiliarcl.Where(a => a.PersonaClIdPersonaCl == id).Count();
            //ALMACENA EL TIPO DE ASIENTO FAMILIAR YA SEA REFERENCIA O FAMILIAR
            List<string> TiposAsiento = _context.Asientofamiliarcl.Where(a => a.PersonaClIdPersonaCl == id).Select(a => a.Tipo).ToList();

            //SI LA PERSONA CUENTA CON REFERENCIA O FAMILIAR SE LLENAN VARIABLES
            foreach (string tipoAsiento in TiposAsiento)
            {
                if (tipoAsiento == "FAMILIAR")
                {
                    familiarTipo = tipoAsiento;
                }
                else if (tipoAsiento == "REFERENCIA")
                {
                    referenciaTipo = tipoAsiento;
                }
            }
            //SI ENCONTRO LLENA VIEWBAGS PARA BOTONES DE EDICION DE FAMILIAR y REFERENCIA
            if (NumeroFamiliares >= 1)
            {
                switch (familiarTipo)
                {
                    case "FAMILIAR":
                        ViewBag.idFamiliares = BuscaId(listaNoSi, SI);
                        break;
                    default:
                        ViewBag.idFamiliares = BuscaId(listaSiNo, NO);
                        break;
                }
                switch (referenciaTipo)
                {
                    case "REFERENCIA":
                        ViewBag.idReferenciasPersonales = BuscaId(listaSiNo, SI);
                        break;
                    default:
                        ViewBag.idReferenciasPersonales = BuscaId(listaSiNo, NO);
                        break;
                }
            }
            else
            {
                // SI NO ENCONTRO REGISTRO EN ASIENTO FAMILIAR BOTONES DE EDICION NO DISPNIBLES
                ViewBag.idFamiliares = BuscaId(listaSiNo, NO);
                ViewBag.idReferenciasPersonales = BuscaId(listaSiNo, NO);

            }
            #endregion

            #region Consume sustancias

            ViewBag.listaConsumoSustancias = listaNoSi;


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
                ViewBag.ultimoConsumo = DateTime.ParseExact("01/01/1990", "MM/dd/yyyy", CultureInfo.InvariantCulture);
                ViewBag.observaciones = null;
            }

            ViewBag.ListaConsumo = consumocl;
            ViewBag.ListaAsientoFamiliares = asientofamiliarescl;
            ViewBag.ListaAsientoReferencias = asientoreferenciascl;
            #endregion

            #region Familiares

            ViewBag.listaFamiliares = listaSiNo;

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
        public async Task<IActionResult> Edit(int id, [Bind("IdPersonaCl,Nombre,Paterno,Materno,NombrePadre,NombreMadre,Alias,Genero,Edad,Fnacimiento,Lnpais,Lnestado,Lnmunicipio,Lnlocalidad,EstadoCivil,Duracion,OtroIdioma,EspecifiqueIdioma,DatosGeneralescol,LeerEscribir,Traductor,EspecifiqueTraductor,TelefonoFijo,Celular,Hijos,Nhijos,NpersonasVive,Propiedades,Curp,ConsumoSustancias,TratamientoAdicciones,CualTratamientoAdicciones,UltimaActualización,Supervisor,RutaFoto,Familiares,ReferenciasPersonales,Capturista,Candado,MotivoCandado,Colaboracion,UbicacionExpediente,ComLgbtttiq,ComIndigena,TieneResolucion,Juzgado,Ce")] Personacl personacl, string arraySustancias, string arraySustanciasEditadas, string arrayFamiliarReferencia, string arrayFamiliaresEditados, string arrayReferenciasEditadas, string centropenitenciario, string sinocentropenitenciario)
        {
            if (id != personacl.IdPersonaCl)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    personacl.Ce = mg.removeSpaces(mg.normaliza(personacl.Ce));
                    personacl.Juzgado = mg.removeSpaces(mg.normaliza(personacl.Juzgado));
                    personacl.Nombre = mg.removeSpaces(mg.normaliza(personacl.Nombre));
                    personacl.Paterno = mg.removeSpaces(mg.normaliza(personacl.Paterno));
                    personacl.Materno = mg.removeSpaces(mg.normaliza(personacl.Materno));
                    personacl.NombrePadre = mg.normaliza(personacl.NombrePadre);
                    personacl.NombreMadre = mg.normaliza(personacl.NombreMadre);
                    personacl.Alias = mg.normaliza(personacl.Alias);
                    personacl.Lnlocalidad = mg.normaliza(personacl.Lnlocalidad);
                    personacl.Duracion = mg.normaliza(personacl.Duracion);
                    personacl.DatosGeneralescol = mg.normaliza(personacl.DatosGeneralescol);
                    personacl.EspecifiqueIdioma = mg.normaliza(personacl.EspecifiqueIdioma);
                    personacl.EspecifiqueTraductor = mg.normaliza(personacl.EspecifiqueTraductor);
                    personacl.ComIndigena = mg.normaliza(personacl.ComIndigena);
                    personacl.ComLgbtttiq = mg.normaliza(personacl.ComLgbtttiq);
                    personacl.Colaboracion = mg.normaliza(personacl.Colaboracion);
                    personacl.UbicacionExpediente = mg.normaliza(personacl.UbicacionExpediente);
                    if (!(personacl.Paterno == null && personacl.Materno == null && personacl.Nombre == null && personacl.Genero == null && personacl.Fnacimiento == null && personacl.Lnestado == null))
                    {
                        var curs = mg.sacaCurs(personacl.Paterno, personacl.Materno, personacl.Fnacimiento, personacl.Genero, personacl.Lnestado, personacl.Nombre);
                        personacl.ClaveUnicaScorpio = curs;
                        personacl.Curp = curs + "*";
                    }
                    personacl.Curp = mg.normaliza(personacl.Curp);
                    personacl.Centropenitenciario = mg.removeSpaces(mg.normaliza(centropenitenciario));
                    personacl.Sinocentropenitenciario = sinocentropenitenciario;
                    personacl.ConsumoSustancias = mg.normaliza(personacl.ConsumoSustancias);
                    personacl.Familiares = mg.normaliza(personacl.Familiares);
                    personacl.ReferenciasPersonales = mg.normaliza(personacl.ReferenciasPersonales);
                    personacl.RutaFoto = mg.normaliza(personacl.RutaFoto);
                    personacl.Capturista = personacl.Capturista;
                    if (personacl.Candado == null) { personacl.Candado = 0; }
                    personacl.Candado = personacl.Candado;
                    personacl.MotivoCandado = mg.normaliza(personacl.MotivoCandado);
                    personacl.TratamientoAdicciones = mg.normaliza(personacl.TratamientoAdicciones);
                    personacl.CualTratamientoAdicciones = mg.normaliza(personacl.CualTratamientoAdicciones);

                    #region -sustancias agregadas -

                    int idConsumoSustancias = ((from table in _context.Consumosustanciascl
                                                select table.IdConsumoSustanciasCl).Max());
                    if (arraySustancias != null)
                    {
                        JArray sustancias = JArray.Parse(arraySustancias);

                        for (int i = 0; i < sustancias.Count; i = i + 5)
                        {
                            Consumosustanciascl consumosustanciasCLBD = new Consumosustanciascl();
                            personacl.ConsumoSustancias = "SI";
                            consumosustanciasCLBD.IdConsumoSustanciasCl = ++idConsumoSustancias;
                            consumosustanciasCLBD.Sustancia = sustancias[i].ToString();
                            consumosustanciasCLBD.Frecuencia = sustancias[i + 1].ToString();
                            consumosustanciasCLBD.Cantidad = mg.normaliza(sustancias[i + 2].ToString());
                            consumosustanciasCLBD.UltimoConsumo = mg.validateDatetime(sustancias[i + 3].ToString());
                            consumosustanciasCLBD.Observaciones = mg.normaliza(sustancias[i + 4].ToString());
                            consumosustanciasCLBD.PersonaClIdPersonaCl = id;

                            _context.Add(consumosustanciasCLBD);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                        }
                    }
                    #endregion

                    #region -Sustancias editadas -

                    if (arraySustanciasEditadas != null)
                    {
                        JArray sustancias = JArray.Parse(arraySustanciasEditadas);

                        for (int i = 0; i < sustancias.Count; i = i + 6)
                        {
                            int idConsumo = Int32.Parse(sustancias[i + 5].ToString());

                            if (idConsumo < 0)
                            {
                                idConsumo = -idConsumo;
                                var sustancia = await _context.Consumosustanciascl.SingleOrDefaultAsync(m => m.IdConsumoSustanciasCl == idConsumo);
                                _context.Consumosustanciascl.Remove(sustancia);
                                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                            }
                            else
                            {
                                Consumosustanciascl consumosustanciasCLBD = new Consumosustanciascl();
                                try
                                {
                                    consumosustanciasCLBD.IdConsumoSustanciasCl = idConsumo;
                                    consumosustanciasCLBD.Sustancia = sustancias[i].ToString();
                                    consumosustanciasCLBD.Frecuencia = sustancias[i + 1].ToString();
                                    consumosustanciasCLBD.Cantidad = mg.normaliza(sustancias[i + 2].ToString());
                                    consumosustanciasCLBD.UltimoConsumo = mg.validateDatetime(sustancias[i + 3].ToString());
                                    consumosustanciasCLBD.Observaciones = mg.normaliza(sustancias[i + 4].ToString());
                                    consumosustanciasCLBD.PersonaClIdPersonaCl = id;

                                    var oldconsumosustanciasCLBD = await _context.Consumosustanciascl.FindAsync(consumosustanciasCLBD.IdConsumoSustanciasCl);
                                    _context.Entry(oldconsumosustanciasCLBD).CurrentValues.SetValues(consumosustanciasCLBD);
                                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    if (!PersonaclExists(consumosustanciasCLBD.PersonaClIdPersonaCl))
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
                    }
                    #endregion

                    #region -Familiares, referencias agregados-

                    int idAsientoFamiliar = ((from table in _context.Asientofamiliarcl
                                              select table.IdAsientoFamiliarCl).Max());
                    if (arrayFamiliarReferencia != null)
                    {
                        JArray familiarReferencia = JArray.Parse(arrayFamiliarReferencia);
                        for (int i = 0; i < familiarReferencia.Count; i = i + 14)
                        {
                            Asientofamiliarcl asientoFamiliarCL = new Asientofamiliarcl();
                            try
                            {
                                asientoFamiliarCL.IdAsientoFamiliarCl = ++idAsientoFamiliar;
                                asientoFamiliarCL.Nombre = mg.normaliza(familiarReferencia[i].ToString());
                                asientoFamiliarCL.Relacion = familiarReferencia[i + 1].ToString();
                                asientoFamiliarCL.Edad = Int32.Parse(familiarReferencia[i + 2].ToString());
                                asientoFamiliarCL.Sexo = familiarReferencia[i + 3].ToString();
                                asientoFamiliarCL.Dependencia = familiarReferencia[i + 4].ToString();
                                asientoFamiliarCL.DependenciaExplica = mg.normaliza(familiarReferencia[i + 5].ToString());
                                asientoFamiliarCL.VivenJuntos = familiarReferencia[i + 6].ToString();
                                asientoFamiliarCL.Domicilio = mg.normaliza(familiarReferencia[i + 7].ToString());
                                asientoFamiliarCL.Telefono = familiarReferencia[i + 8].ToString();
                                asientoFamiliarCL.HorarioLocalizacion = mg.normaliza(familiarReferencia[i + 9].ToString());
                                asientoFamiliarCL.EnteradoProceso = familiarReferencia[i + 10].ToString();
                                asientoFamiliarCL.PuedeEnterarse = familiarReferencia[i + 11].ToString();
                                asientoFamiliarCL.Observaciones = mg.normaliza(familiarReferencia[i + 12].ToString());
                                asientoFamiliarCL.Tipo = familiarReferencia[i + 13].ToString();
                                if (asientoFamiliarCL.Tipo.Equals("FAMILIAR"))
                                {
                                    personacl.Familiares = "SI";
                                }
                                else if (asientoFamiliarCL.Tipo.Equals("REFERENCIA"))
                                {
                                    personacl.ReferenciasPersonales = "SI";
                                }
                                asientoFamiliarCL.PersonaClIdPersonaCl = id;

                                _context.Add(asientoFamiliarCL);
                                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                if (!PersonaclExists(asientoFamiliarCL.PersonaClIdPersonaCl))
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
                    #endregion

                    #region -Familiares editados-

                    if (arrayFamiliaresEditados != null)
                    {
                        JArray familiarReferencia = JArray.Parse(arrayFamiliaresEditados);

                        for (int i = 0; i < familiarReferencia.Count; i = i + 14)
                        {
                            int idAsiento = Int32.Parse(familiarReferencia[i + 13].ToString());
                            if (idAsiento < 0)
                            {
                                idAsiento = -idAsiento;

                                var asiento = await _context.Asientofamiliarcl.SingleOrDefaultAsync(m => m.IdAsientoFamiliarCl == idAsiento);
                                _context.Asientofamiliarcl.Remove(asiento);
                                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                            }
                            else
                            {
                                Asientofamiliarcl asientoFamiliarCL = new Asientofamiliarcl();
                                try
                                {
                                    asientoFamiliarCL.IdAsientoFamiliarCl = Int32.Parse(familiarReferencia[i + 13].ToString());
                                    asientoFamiliarCL.Nombre = mg.normaliza(familiarReferencia[i].ToString());
                                    asientoFamiliarCL.Relacion = familiarReferencia[i + 1].ToString();
                                    asientoFamiliarCL.Edad = Int32.Parse(familiarReferencia[i + 2].ToString());
                                    asientoFamiliarCL.Sexo = familiarReferencia[i + 3].ToString();
                                    asientoFamiliarCL.Dependencia = familiarReferencia[i + 4].ToString();
                                    asientoFamiliarCL.DependenciaExplica = mg.normaliza(familiarReferencia[i + 5].ToString());
                                    asientoFamiliarCL.VivenJuntos = familiarReferencia[i + 6].ToString();
                                    asientoFamiliarCL.Domicilio = mg.normaliza(familiarReferencia[i + 7].ToString());
                                    asientoFamiliarCL.Telefono = familiarReferencia[i + 8].ToString();
                                    asientoFamiliarCL.HorarioLocalizacion = mg.normaliza(familiarReferencia[i + 9].ToString());
                                    asientoFamiliarCL.EnteradoProceso = familiarReferencia[i + 10].ToString();
                                    asientoFamiliarCL.PuedeEnterarse = familiarReferencia[i + 11].ToString();
                                    asientoFamiliarCL.Observaciones = mg.normaliza(familiarReferencia[i + 12].ToString());
                                    asientoFamiliarCL.Tipo = "FAMILIAR";
                                    asientoFamiliarCL.PersonaClIdPersonaCl = id;

                                    var oldAsientofamiliar = await _context.Asientofamiliarcl.FindAsync(asientoFamiliarCL.IdAsientoFamiliarCl);

                                    _context.Entry(oldAsientofamiliar).CurrentValues.SetValues(asientoFamiliarCL);
                                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    if (!PersonaclExists(asientoFamiliarCL.PersonaClIdPersonaCl))
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
                    }
                    #endregion

                    #region -Referencias editadas-

                    if (arrayReferenciasEditadas != null)
                    {
                        JArray familiarReferencia = JArray.Parse(arrayReferenciasEditadas);

                        for (int i = 0; i < familiarReferencia.Count; i = i + 14)
                        {
                            int idAsiento = Int32.Parse(familiarReferencia[i + 13].ToString());
                            if (idAsiento < 0)
                            {
                                idAsiento = -idAsiento;
                                var asiento = await _context.Asientofamiliarcl.SingleOrDefaultAsync(m => m.IdAsientoFamiliarCl == idAsiento);
                                _context.Asientofamiliarcl.Remove(asiento);
                                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                            }
                            else
                            {
                                Asientofamiliarcl asientoFamiliarCL = new Asientofamiliarcl();
                                try
                                {
                                    asientoFamiliarCL.IdAsientoFamiliarCl = Int32.Parse(familiarReferencia[i + 13].ToString());
                                    asientoFamiliarCL.Nombre = mg.normaliza(familiarReferencia[i].ToString());
                                    asientoFamiliarCL.Relacion = familiarReferencia[i + 1].ToString();
                                    asientoFamiliarCL.Edad = Int32.Parse(familiarReferencia[i + 2].ToString());
                                    asientoFamiliarCL.Sexo = familiarReferencia[i + 3].ToString();
                                    asientoFamiliarCL.Dependencia = familiarReferencia[i + 4].ToString();
                                    asientoFamiliarCL.DependenciaExplica = mg.normaliza(familiarReferencia[i + 5].ToString());
                                    asientoFamiliarCL.VivenJuntos = familiarReferencia[i + 6].ToString();
                                    asientoFamiliarCL.Domicilio = mg.normaliza(familiarReferencia[i + 7].ToString());
                                    asientoFamiliarCL.Telefono = familiarReferencia[i + 8].ToString();
                                    asientoFamiliarCL.HorarioLocalizacion = mg.normaliza(familiarReferencia[i + 9].ToString());
                                    asientoFamiliarCL.EnteradoProceso = familiarReferencia[i + 10].ToString();
                                    asientoFamiliarCL.PuedeEnterarse = familiarReferencia[i + 11].ToString();
                                    asientoFamiliarCL.Observaciones = mg.normaliza(familiarReferencia[i + 12].ToString());
                                    asientoFamiliarCL.Tipo = "REFERENCIA";
                                    asientoFamiliarCL.PersonaClIdPersonaCl = id;

                                    var oldAsientofamiliar = await _context.Asientofamiliar.FindAsync(asientoFamiliarCL.IdAsientoFamiliarCl);
                                    _context.Entry(oldAsientofamiliar).CurrentValues.SetValues(asientoFamiliarCL);
                                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                                }
                                catch (DbUpdateConcurrencyException)
                                {
                                    if (!PersonaclExists(asientoFamiliarCL.PersonaClIdPersonaCl))
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
                    }
                    #endregion

                    var oldPersona = await _context.Personacl.FindAsync(id);
                    _context.Entry(oldPersona).CurrentValues.SetValues(personacl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
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
                return RedirectToAction("MenuEdicion/" + id/*, "Personas"*/);
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
            ViewBag.nombre = nombre;
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
        public async Task<IActionResult> EditDomicilio(int id, [Bind("IdDomiciliocl,TipoDomicilio,Calle,No,TipoUbicacion,NombreCf,Pais,Estado,Municipio,Temporalidad,ResidenciaHabitual,Cp,Referencias,Horario,DomcilioSecundario,Observaciones,Zona,Lat,Lng,PersonaclIdPersonacl,Zona")] Domiciliocl domiciliocl, string inputAutocomplete, string nombre, int idPersona)
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
                    var oldDomicilio = await _context.Domiciliocl.FindAsync(domiciliocl.IdDomiciliocl);
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
                return RedirectToAction("MenuEdicion/" + domiciliocl.PersonaclIdPersonacl, "Personacls", new { nombre });
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

            var domisecu = await _context.Domiciliocl.SingleOrDefaultAsync(m => m.IdDomiciliocl == id);
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

            ViewBag.ListaPaisM = ListaPaisD;
            #endregion

            #region Destado
            List<Estados> listaEstadosD = new List<Estados>();
            listaEstadosD = (from table in _context.Estados
                             select table).ToList();

            List<Domiciliosecundariocl> listadomiciliosecundarios = new List<Domiciliosecundariocl>();
            listadomiciliosecundarios = (from table in _context.Domiciliosecundariocl
                                         select table).ToList();


            listaEstadosD.Insert(0, new Estados { Id = 0, Estado = "Selecciona" });
            ViewBag.ListaEstadoED = listaEstadosD;

            ViewBag.ListaEstadoM = listaEstadosD;
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
            ViewBag.ListaMunicipioM = listaMunicipiosD;
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

            ViewBag.listaResidenciaHabitual = listaSiNo;
            #endregion
            if (domisecu == null)
            {
                return NotFound();
            }

            return View(domisecu);
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


                    var oldDomicilio = await _context.Domiciliosecundariocl.FindAsync(domiciliosecundariocl.IdDomicilioSecundarioCl);
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
                return RedirectToAction("EditDomicilio/" + idPersona, "Personacls", new { nombre });
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

            return RedirectToAction("EditDomicilio/" + idpersona, "Personacls");
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

            var centro = await _context.Personacl.SingleOrDefaultAsync(m => m.IdPersonaCl == id);

            ViewBag.siNoCentro = centro.Sinocentropenitenciario;

            var estudioscl = await _context.Estudioscl.SingleOrDefaultAsync(m => m.PersonaClIdPersonaCl == id);
            if (estudioscl == null)
            {
                return NotFound();
            }

            ViewBag.estudia = estudioscl.Estudia;
            ViewBag.estudiaq = estudioscl.CualCursoAcademico;

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


            #region -PARA PERSONAS CENTRO-
            ViewBag.listaCursoAcademico = listaNoSi;
            ViewBag.idCursoAcademico = BuscaId(listaNoSi, estudioscl.CursoAcademico);

            ViewBag.listaDeseaConcluirEstudios = listaNoSi;
            ViewBag.idDeseaConcluirEstudios = BuscaId(listaNoSi, estudioscl.DeseaConcluirEstudios);
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
        public async Task<IActionResult> EditEscolaridad(int id, [Bind("IdEstudiosCl,Estudia,GradoEstudios,InstitucionE,Horario,Direccion,Telefono,CursoAcademico,CualCursoAcademico,DeseaConcluirEstudios,Observaciones,PersonaClIdPersonaCl")] Estudioscl estudioscl)
        {
            if (id != estudioscl.PersonaClIdPersonaCl)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    estudioscl.Estudia = mg.normaliza(estudioscl.Estudia);
                    estudioscl.CursoAcademico = mg.normaliza(estudioscl.CursoAcademico);
                    estudioscl.CualCursoAcademico = mg.normaliza(estudioscl.CualCursoAcademico);
                    estudioscl.DeseaConcluirEstudios = mg.normaliza(estudioscl.DeseaConcluirEstudios);

                    estudioscl.InstitucionE = estudioscl.Estudia.Equals("NO") ? "NA" : mg.normaliza(estudioscl.InstitucionE);
                    estudioscl.Horario = estudioscl.Estudia.Equals("NO") ? "NA" : mg.normaliza(estudioscl.Horario);
                    estudioscl.Direccion = estudioscl.Estudia.Equals("NO") ? "NA" : mg.normaliza(estudioscl.Direccion);
                    estudioscl.Telefono = estudioscl.Estudia.Equals("NO") ? "0" : mg.normaliza(estudioscl.Telefono);

                    estudioscl.Observaciones = mg.normaliza(estudioscl.Observaciones);

                    var oldEstudios = await _context.Estudioscl.FindAsync(estudioscl.IdEstudiosCl);

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
                return RedirectToAction("MenuEdicion/" + id);
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

            var centro = await _context.Personacl.SingleOrDefaultAsync(m => m.IdPersonaCl == id);

            ViewBag.siNoCentro = centro.Sinocentropenitenciario;


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

            ViewBag.listaPropuestaLaboral = listaNoSi;
            ViewBag.idPropuestaLaboral = BuscaId(listaNoSi, trabajocl.PropuestaLaboral);

            ViewBag.listaTrabajoCentro = listaNoSi;
            ViewBag.idTrabajoCentro = BuscaId(listaNoSi, trabajocl.TrabajoCentro);

            ViewBag.listaCapacitacion = listaNoSi;
            ViewBag.idCapacitacion = BuscaId(listaNoSi, trabajocl.Capacitacion);

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
        public async Task<IActionResult> EditTrabajo(int id, [Bind("IdTrabajoCl,Trabaja,TipoOcupacion,Puesto,EmpledorJefe,EnteradoProceso,SePuedeEnterar,TiempoTrabajano,Salario,TemporalidadSalario,Direccion,Horario,Telefono,ConocimientoHabilidad,PropuestaLaboral,CualPropuesta,AntesdeCentro,TrabajoCentro,CualTrabajoCentro,Capacitacion,CualCapacitacion,Observaciones,PersonaClIdPersonaCl")] Trabajocl trabajocl)
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
            trabajocl.ConocimientoHabilidad = mg.normaliza(trabajocl.ConocimientoHabilidad);
            trabajocl.PropuestaLaboral = mg.normaliza(trabajocl.PropuestaLaboral);
            trabajocl.CualPropuesta = mg.normaliza(trabajocl.CualPropuesta);
            trabajocl.AntesdeCentro = mg.normaliza(trabajocl.AntesdeCentro);
            trabajocl.TrabajoCentro = mg.normaliza(trabajocl.TrabajoCentro);
            trabajocl.CualTrabajoCentro = mg.normaliza(trabajocl.CualTrabajoCentro);
            trabajocl.Capacitacion = mg.normaliza(trabajocl.Capacitacion);
            trabajocl.CualCapacitacion = mg.normaliza(trabajocl.CualCapacitacion);
            trabajocl.Observaciones = mg.normaliza(trabajocl.Observaciones);

            if (ModelState.IsValid)
            {
                try
                {
                    var oldTrabajo = await _context.Trabajocl.FindAsync(trabajocl.IdTrabajoCl);
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
                return RedirectToAction("MenuEdicion/" + trabajocl.PersonaClIdPersonaCl, "Personacls");
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

            var centro = await _context.Personacl.SingleOrDefaultAsync(m => m.IdPersonaCl == id);
            ViewBag.siNoCentro = centro.Sinocentropenitenciario;

            ViewBag.listasePuedeEnterarASr = listaNoSiNA;
            ViewBag.idsePuedeEnterarAS = BuscaId(listaNoSiNA, actividadsocialcl.SePuedeEnterar);

            ViewBag.listaActividadesDepCulCentro = listaNoSiNA;
            ViewBag.idActividadesDepCulCentro = BuscaId(listaNoSiNA, actividadsocialcl.ActividadesDepCulCentro);

            ViewBag.listaDeseaDepCul = listaNoSiNA;
            ViewBag.idDeseaDepCul = BuscaId(listaNoSiNA, actividadsocialcl.DeseaDepCul);

            if (actividadsocialcl == null)
            {
                return NotFound();
            }
            return View(actividadsocialcl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditActividadesSociales(int id, [Bind("IdActividadSocialCl,TipoActividad,Horario,Lugar,Telefono,SePuedeEnterar,Referencia,Observaciones,DeseaDepCul,ActividadesDepCulCentro,CualActividadesDepCulCentro,CualDeseaDepCul,PersonaClIdPersonaCl")] Actividadsocialcl actividadsocialcl)
        {
            if (id != actividadsocialcl.PersonaClIdPersonaCl)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {

                    actividadsocialcl.TipoActividad = mg.normaliza(actividadsocialcl.TipoActividad);
                    actividadsocialcl.Horario = mg.normaliza(actividadsocialcl.Horario);
                    actividadsocialcl.Lugar = mg.normaliza(actividadsocialcl.Lugar);
                    actividadsocialcl.Referencia = mg.normaliza(actividadsocialcl.Referencia);
                    actividadsocialcl.Observaciones = mg.normaliza(actividadsocialcl.Observaciones);

                    actividadsocialcl.DeseaDepCul = mg.normaliza(actividadsocialcl.DeseaDepCul);
                    actividadsocialcl.ActividadesDepCulCentro = mg.normaliza(actividadsocialcl.ActividadesDepCulCentro);
                    actividadsocialcl.CualActividadesDepCulCentro = mg.normaliza(actividadsocialcl.CualActividadesDepCulCentro);
                    actividadsocialcl.CualDeseaDepCul = mg.normaliza(actividadsocialcl.CualDeseaDepCul);

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
                return RedirectToAction("MenuEdicion/" + id);
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
                return RedirectToAction("MenuEdicion/" + abandonoestadocl.PersonaclIdPersonacl, "Personacls");
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


                    var oldFamiliaresforaneos = await _context.Familiaresforaneoscl.FindAsync(familiaresforaneoscl.IdFamiliaresForaneosCl);
                    _context.Entry(oldFamiliaresforaneos).CurrentValues.SetValues(familiaresforaneoscl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(familiaresforaneos);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction("EditAbandonoEstado/" + familiaresforaneoscl.PersonaClIdPersonaCl, "Personacls");
            }
            return View(familiaresforaneoscl);
        }

        public async Task<IActionResult> DeleteConfirmedFamiiarF(int? id)
        {
            var familiarfcl = await _context.Familiaresforaneoscl.SingleOrDefaultAsync(m => m.IdFamiliaresForaneosCl == id);
            _context.Familiaresforaneoscl.Remove(familiarfcl);
            await _context.SaveChangesAsync();

            var empty = (from ff in _context.Familiaresforaneoscl
                         where ff.PersonaClIdPersonaCl == familiarfcl.PersonaClIdPersonaCl
                         select ff);

            if (!empty.Any())
            {
                var query = (from a in _context.Abandonoestadocl
                             where a.PersonaclIdPersonacl == familiarfcl.PersonaClIdPersonaCl
                             select a).FirstOrDefault();
                query.FamiliaresFuera = "NO";
                _context.SaveChanges();
            }




            return RedirectToAction("EditAbandonoEstado/" + familiarfcl.PersonaClIdPersonaCl, "Personacls");
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
            if (id != saludfisicacl.PersonaClIdPersonaCl)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    saludfisicacl.Enfermedad = mg.normaliza(saludfisicacl.Enfermedad);
                    saludfisicacl.EspecifiqueEnfermedad = saludfisicacl.Enfermedad.Equals("NO") ? "NA" : mg.normaliza(saludfisicacl.EspecifiqueEnfermedad);
                    saludfisicacl.Tratamiento = saludfisicacl.Enfermedad.Equals("NO") ? "NA" : mg.normaliza(saludfisicacl.Tratamiento);

                    saludfisicacl.EmbarazoLactancia = mg.normaliza(saludfisicacl.EmbarazoLactancia);
                    saludfisicacl.Tiempo = saludfisicacl.EmbarazoLactancia.Equals("NO") ? "NA" : mg.normaliza(saludfisicacl.Tiempo);

                    saludfisicacl.Discapacidad = mg.normaliza(saludfisicacl.Discapacidad);
                    saludfisicacl.EspecifiqueDiscapacidad = saludfisicacl.Discapacidad.Equals("NO") ? "NA" : mg.normaliza(saludfisicacl.EspecifiqueDiscapacidad);

                    saludfisicacl.ServicioMedico = mg.normaliza(saludfisicacl.ServicioMedico);
                    saludfisicacl.EspecifiqueServicioMedico = saludfisicacl.ServicioMedico.Equals("NO") ? "NA" : mg.normaliza(saludfisicacl.EspecifiqueServicioMedico);
                    saludfisicacl.InstitucionServicioMedico = saludfisicacl.ServicioMedico.Equals("NO") ? "NA" : mg.normaliza(saludfisicacl.InstitucionServicioMedico);

                    saludfisicacl.Observaciones = mg.normaliza(saludfisicacl.Observaciones);



                    var oldSaludfisicacl = await _context.Saludfisicacl.FindAsync(saludfisicacl.IdSaludFisicaCl);

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
                return RedirectToAction("MenuEdicion/" + saludfisicacl.PersonaClIdPersonaCl, "Personacls");
            }
            return View(saludfisicacl);
        }
        #endregion

        #region -Edit foto -
        public async Task<IActionResult> EditFoto(int? id, string ruta)
        {
            if (id == null)
            {
                return NotFound();
            }
            string ruta2 = ruta == null ? "" : ruta;
            string[] partes = ruta2.Split('?');
            string nuevaRuta = partes[0];
            ViewBag.ruta = nuevaRuta;



            var personacl = await _context.Personacl.SingleOrDefaultAsync(m => m.IdPersonaCl == id);
            if (personacl == null)
            {
                return NotFound();
            }

            return View(personacl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFoto([Bind("IdPersonaCl")] Personacl personacl, IFormFile fotoEditada, string ruta)
        {
            if (ModelState.IsValid)
            {

                #region -Guardar Foto-
                var file_name = (from a in _context.Personacl
                                 where a.IdPersonaCl == personacl.IdPersonaCl
                                 select a.RutaFoto).FirstOrDefault();
                if (file_name == null || file_name == "NA")
                {
                    var query = (from a in _context.Personacl
                                 where a.IdPersonaCl == personacl.IdPersonaCl
                                 select a).FirstOrDefault();
                    file_name = query.IdPersonaCl + "_" + query.Paterno + "_" + query.Nombre + ".jpg";
                    query.RutaFoto = file_name;
                    try
                    {
                        var oldFoto = await _context.Persona.FindAsync(query.IdPersonaCl);
                        _context.Entry(oldFoto).CurrentValues.SetValues(query);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                        //_context.Update(query);
                        //await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PersonaclExists(query.IdPersonaCl))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Fotoscl");
                var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create, FileAccess.ReadWrite);
                #endregion

                fotoEditada.CopyTo(stream);
                stream.Close();
                string ruta2 = ruta == null ? "" : ruta;
                if (ruta2.Equals("detalles"))
                {
                    return RedirectToAction("Details/" + personacl.IdPersonaCl, "Personacls");
                }
                else
                {
                    return RedirectToAction("MenuEdicion/" + personacl.IdPersonaCl, "Personacls");
                }

            }
            return View(personacl);
        }
        #endregion

        #region -WarningSupervisor-
        public async Task<IActionResult> WarningSupervisor()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.showSupervisor = false;

            #region -Solicitud Atendida Archivo prestamo Digital-
            var warningRespuesta = from a in _context.Archivoprestamodigital
                                   where a.EstadoPrestamo == 1 && user.ToString().ToUpper() == a.Usuario.ToUpper()
                                   select a;
            ViewBag.WarningsUser = warningRespuesta.Count();
            #endregion



            foreach (var rol in roles)
            {
                if (rol == "AdminLC" || rol == "Masteradmin")
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
            DateTime fechaInforme = (DateTime.Today).AddDays(5);
            DateTime fechaControl = (DateTime.Today).AddDays(3);
            DateTime fechaInformeCoordinador = (DateTime.Today).AddDays(60);
            DateTime fechaHoy = DateTime.Today;
            var fechaProcesal = DateTime.Now.AddMonths(-6);

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
                if (rol == "AdminLC")
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

            List<Personacl> personaclVM = _context.Personacl.ToList();
            List<Supervisioncl> supervisionclVM = _context.Supervisioncl.ToList();
            List<Causapenalcl> causapenalclVM = _context.Causapenalcl.ToList();
            List<Planeacionestrategicacl> planeacionestrategicaclVM = _context.Planeacionestrategicacl.ToList();
            List<Beneficios> beneficiosclVM = _context.Beneficios.ToList();
            List<Domiciliocl> domicilioclVM = _context.Domiciliocl.ToList();
            List<Municipios> municipiosVM = _context.Municipios.ToList();
            List<Estados> estadosVM = _context.Estados.ToList();
            List<Personaclcausapenalcl> personacausapenalclVM = _context.Personaclcausapenalcl.ToList();



            List<Beneficios> queryBeneficios = (from b in beneficiosclVM
                                                group b by b.SupervisionclIdSupervisioncl into grp
                                                select grp.OrderByDescending(b => b.IdBeneficios).FirstOrDefault()).ToList();



            //var listaaudit = (from a in _context.Audit
            //                  join s in _context.Supervision on int.Parse(Regex.Replace(a.PrimaryKey, @"[^0-9]", "")) equals s.IdSupervision
            //                  where a.TableName == "Supervision" && a.NewValues.Contains("en espera de respuesta")
            //                  group a by int.Parse(Regex.Replace(a.PrimaryKey, @"[^0-9]", "")) into grp
            //                  select grp.OrderByDescending(a => a.Id).FirstOrDefault());
            #endregion

            #region -Jointables-

            var sinResolucion = from p in personaclVM
                                join d in domicilioclVM on p.IdPersonaCl equals d.PersonaclIdPersonacl
                                join e in estadosVM on int.Parse(d.Estado) equals e.Id
                                join m in municipiosVM on int.Parse(d.Municipio) equals m.Id
                                where p.TieneResolucion == "NO" || p.TieneResolucion == "NA" || p.TieneResolucion is null
                                select new PlaneacionclWarningViewModel
                                {
                                    personaclVM = p,
                                    estadosVM = e,
                                    municipiosVM = m,
                                    tipoAdvertencia = "Sin Resolución"
                                };
            var sinResolucion2 = from p in personaclVM
                                 join d in domicilioclVM on p.IdPersonaCl equals d.PersonaclIdPersonacl
                                 join e in estadosVM on int.Parse(d.Estado) equals e.Id
                                 join m in municipiosVM on int.Parse(d.Municipio) equals m.Id
                                 where (p.TieneResolucion == "NO" || p.TieneResolucion == "NA" || p.TieneResolucion is null) && p.Supervisor == usuario
                                 select new PlaneacionclWarningViewModel
                                 {
                                     personaclVM = p,
                                     estadosVM = e,
                                     municipiosVM = m,
                                     tipoAdvertencia = "Sin Resolución"
                                 };


            var leftJoin = from persona in personaclVM
                           join domicilio in domicilioclVM on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                           join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                           join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl into tmp
                           from sinsupervision in tmp.DefaultIfEmpty()
                           select new PlaneacionclWarningViewModel
                           {
                               personaclVM = persona,
                               supervisionclVM = sinsupervision,
                               municipiosVM = municipio,
                               tipoAdvertencia = "Sin supervisión"
                           };

            var where = from ss in leftJoin
                        where ss.supervisionclVM == null
                        select new PlaneacionclWarningViewModel
                        {
                            personaclVM = ss.personaclVM,
                            supervisionclVM = ss.supervisionclVM,
                            municipiosVM = ss.municipiosVM,
                            tipoAdvertencia = "Sin supervisión"
                        };

            var where2 = from ss in leftJoin
                         where ss.personaclVM.Supervisor == usuario && ss.supervisionclVM == null
                         select new PlaneacionclWarningViewModel
                         {
                             personaclVM = ss.personaclVM,
                             supervisionclVM = ss.supervisionclVM,
                             municipiosVM = ss.municipiosVM,
                             tipoAdvertencia = "Sin supervisión"
                         };

            var personasConSupervision = from persona in personaclVM
                                         join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                                         join Beneficios in beneficiosclVM on supervision.IdSupervisioncl equals Beneficios.SupervisionclIdSupervisioncl
                                         select new Supervisioncl
                                         {
                                             IdSupervisioncl = supervision.IdSupervisioncl
                                         };


            List<int> idSupervisiones = personasConSupervision.Select(x => x.IdSupervisioncl).Distinct().ToList();

            var joins = from persona in personaclVM
                        join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                        join domicilio in domicilioclVM on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                        join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                        join causapenal in causapenalclVM on supervision.CausaPenalclIdCausaPenalcl equals causapenal.IdCausaPenalcl
                        select new PlaneacionclWarningViewModel
                        {
                            personaclVM = persona,
                            supervisionclVM = supervision,
                            municipiosVM = municipio,
                            causapenalclVM = causapenal,
                            tipoAdvertencia = "Sin figura judicial"
                        };

            var table = from persona in personaclVM
                        join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                        join domicilio in domicilioclVM on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                        join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                        join causapenal in causapenalclVM on supervision.CausaPenalclIdCausaPenalcl equals causapenal.IdCausaPenalcl
                        join planeacion in planeacionestrategicaclVM on supervision.IdSupervisioncl equals planeacion.SupervisionclIdSupervisioncl
                        join Beneficios in queryBeneficios on supervision.IdSupervisioncl equals Beneficios.SupervisionclIdSupervisioncl
                        select new PlaneacionclWarningViewModel
                        {
                            personaclVM = persona,
                            supervisionclVM = supervision,
                            causapenalclVM = causapenal,
                            planeacionestrategicaclVM = planeacion,
                            beneficiosclVM = Beneficios,
                            figuraJudicial = Beneficios.FiguraJudicial,
                            municipiosVM = municipio
                        };


            //var tableAudiot = from persona in personaVM
            //                  join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
            //                  join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
            //                  join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
            //                  join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
            //                  join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
            //                  join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
            //                  join audit in listaaudit on supervision.IdSupervision equals int.Parse(Regex.Replace(audit.PrimaryKey, @"[^0-9]", ""))
            //                  select new PlaneacionWarningViewModel
            //                  {
            //                      personaVM = persona,
            //                      supervisionVM = supervision,
            //                      causapenalVM = causapenal,
            //                      planeacionestrategicaVM = planeacion,
            //                      figuraJudicial = fracciones.FiguraJudicial,
            //                      auditVM = audit,
            //                      fechaCmbio = audit.DateTime,
            //                      municipiosVM = municipio
            //                  };

            if (usuario == "andrea.valdez@dgepms.com"/* || usuario == "janeth@nortedgepms.com"*/ || flagMaster == true)
            {
                var ViewDataAlertasVari = Enumerable.Empty<PlaneacionclWarningViewModel>();

                switch (currentFilter)
                {
                    case "TODOS":
                        ViewDataAlertasVari = (where).Union
                                               (sinResolucion).Union
                                               (joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionclVM.IdSupervisioncl) && s.supervisionclVM.EstadoSupervision == "VIGENTE")).Union
                                               //(from persona in personaVM
                                               // join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                               // join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                               // where persona.Colaboracion == "SI"
                                               // select new PlaneacionWarningViewModel
                                               // {
                                               //     personaVM = persona,
                                               //     municipiosVM = municipio,
                                               //     tipoAdvertencia = "Pendiente de asignación - colaboración"
                                               // }).Union
                                               (from t in table
                                                where t.planeacionestrategicaclVM.InformeInicial != null && t.planeacionestrategicaclVM.InformeInicial < fechaInformeCoordinador && t.supervisionclVM.EstadoSupervision == "VIGENTE" && t.beneficiosclVM.FiguraJudicial != "SUSTITUCIÓN DE LA PENA"
                                                orderby t.planeacionestrategicaclVM.InformeInicial
                                                select new PlaneacionclWarningViewModel
                                                {
                                                    personaclVM = t.personaclVM,
                                                    supervisionclVM = t.supervisionclVM,
                                                    municipiosVM = t.municipiosVM,
                                                    causapenalclVM = t.causapenalclVM,
                                                    planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                    beneficiosclVM = t.beneficiosclVM,
                                                    figuraJudicial = t.figuraJudicial,
                                                    tipoAdvertencia = "Informe fuera de tiempo"
                                                }).Union
                                                (from t in table
                                                 where t.planeacionestrategicaclVM.InformeInicial != null && t.planeacionestrategicaclVM.InformeInicial < fechaControl && t.supervisionclVM.EstadoSupervision == "VIGENTE" && t.beneficiosclVM.FiguraJudicial != "SUSTITUCIÓN DE LA PENA"
                                                 select new PlaneacionclWarningViewModel
                                                 {
                                                     personaclVM = t.personaclVM,
                                                     supervisionclVM = t.supervisionclVM,
                                                     municipiosVM = t.municipiosVM,
                                                     causapenalclVM = t.causapenalclVM,
                                                     planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                     beneficiosclVM = t.beneficiosclVM,
                                                     figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                     tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                                 }).Union
                                                (from t in table
                                                 where t.planeacionestrategicaclVM.InformeInicial == null && t.supervisionclVM.EstadoSupervision == "VIGENTE"
                                                 orderby t.beneficiosclVM.FiguraJudicial
                                                 select new PlaneacionclWarningViewModel
                                                 {
                                                     personaclVM = t.personaclVM,
                                                     supervisionclVM = t.supervisionclVM,
                                                     municipiosVM = t.municipiosVM,
                                                     causapenalclVM = t.causapenalclVM,
                                                     planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                     beneficiosclVM = t.beneficiosclVM,
                                                     figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                     tipoAdvertencia = "Sin fecha de informe"
                                                 }).Union
                                                (from persona in personaclVM
                                                 join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                                                 join domicilio in domicilioclVM on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                                                 join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                                 join causapenal in causapenalclVM on supervision.CausaPenalclIdCausaPenalcl equals causapenal.IdCausaPenalcl
                                                 join planeacion in planeacionestrategicaclVM on supervision.IdSupervisioncl equals planeacion.SupervisionclIdSupervisioncl
                                                 where planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                                 select new PlaneacionclWarningViewModel
                                                 {
                                                     personaclVM = persona,
                                                     supervisionclVM = supervision,
                                                     municipiosVM = municipio,
                                                     causapenalclVM = causapenal,
                                                     planeacionestrategicaclVM = planeacion,
                                                     tipoAdvertencia = "Sin periodicidad de firma"
                                                 }).Union
                                                (from t in table
                                                 where t.personaclVM.Supervisor != null && t.personaclVM.Supervisor != null && t.personaclVM.Supervisor.EndsWith("\u0040dgepms.com") && t.planeacionestrategicaclVM.FechaProximoContacto != null && t.planeacionestrategicaclVM.FechaProximoContacto < fechaHoy && t.supervisionclVM.EstadoSupervision == "VIGENTE" && t.planeacionestrategicaclVM.PeriodicidadFirma != "NO APLICA"
                                                 orderby t.planeacionestrategicaclVM.FechaProximoContacto descending
                                                 select new PlaneacionclWarningViewModel
                                                 {
                                                     personaclVM = t.personaclVM,
                                                     supervisionclVM = t.supervisionclVM,
                                                     municipiosVM = t.municipiosVM,
                                                     causapenalclVM = t.causapenalclVM,
                                                     planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                     beneficiosclVM = t.beneficiosclVM,
                                                     figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                     tipoAdvertencia = "Se paso el tiempo de la firma"
                                                 });//.Union
                                                    //(where).Union
                                                    //(archivoadmin).Union
                                                    //(joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionVM.IdSupervision) && s.supervisionVM.EstadoSupervision == "VIGENTE")).Union
                                                    //(from t in tableAudiot
                                                    // where t.personaVM.Supervisor != null && t.auditVM.DateTime < fechaProcesal && t.supervisionVM.EstadoSupervision != "CONCLUIDO"
                                                    // select new PlaneacionWarningViewModel
                                                    // {
                                                    //     personaVM = t.personaVM,
                                                    //     municipiosVM = t.municipiosVM,
                                                    //     supervisionVM = t.supervisionVM,
                                                    //     causapenalVM = t.causapenalVM,
                                                    //     planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                    //     fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                    //     tipoAdvertencia = "Estado Procesal",
                                                    //     auditVM = t.auditVM
                                                    // });
                        break;
                    case "SIN RESOLUCION":
                        ViewDataAlertasVari = sinResolucion;
                        break;
                    case "INFORME FUERA DE TIEMPO":
                        ViewDataAlertasVari = from t in table
                                              where t.planeacionestrategicaclVM.InformeInicial != null && t.planeacionestrategicaclVM.InformeInicial < fechaInformeCoordinador && t.supervisionclVM.EstadoSupervision == "VIGENTE"
                                              orderby t.planeacionestrategicaclVM.InformeInicial
                                              select new PlaneacionclWarningViewModel
                                              {
                                                  personaclVM = t.personaclVM,
                                                  supervisionclVM = t.supervisionclVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalclVM = t.causapenalclVM,
                                                  planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                  beneficiosclVM = t.beneficiosclVM,
                                                  figuraJudicial = t.figuraJudicial,
                                                  tipoAdvertencia = "Informe fuera de tiempo"
                                              };
                        break;
                    case "CONTROL DE SUPERVISION A 3 DIAS O MENOS":
                        ViewDataAlertasVari = from t in table
                                              where t.planeacionestrategicaclVM.InformeInicial != null && t.planeacionestrategicaclVM.InformeInicial < fechaControl && t.supervisionclVM.EstadoSupervision == "VIGENTE" && t.beneficiosclVM.FiguraJudicial != "SUSTITUCIÓN DE LA PENA"
                                              select new PlaneacionclWarningViewModel
                                              {
                                                  personaclVM = t.personaclVM,
                                                  supervisionclVM = t.supervisionclVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalclVM = t.causapenalclVM,
                                                  planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                  beneficiosclVM = t.beneficiosclVM,
                                                  figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                  tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                              };
                        break;
                    case "SIN FECHA DE INFORME":
                        ViewDataAlertasVari = from t in table
                                              where t.planeacionestrategicaclVM.InformeInicial == null && t.supervisionclVM.EstadoSupervision == "VIGENTE"
                                              orderby t.beneficiosclVM.FiguraJudicial
                                              select new PlaneacionclWarningViewModel
                                              {
                                                  personaclVM = t.personaclVM,
                                                  supervisionclVM = t.supervisionclVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalclVM = t.causapenalclVM,
                                                  planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                  beneficiosclVM = t.beneficiosclVM,
                                                  figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                  tipoAdvertencia = "Sin fecha de informe"
                                              };
                        break;
                    case "SIN PERIODICIDAD DE FIRMA":
                        ViewDataAlertasVari = from persona in personaclVM
                                              join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                                              join domicilio in domicilioclVM on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join causapenal in causapenalclVM on supervision.CausaPenalclIdCausaPenalcl equals causapenal.IdCausaPenalcl
                                              join planeacion in planeacionestrategicaclVM on supervision.IdSupervisioncl equals planeacion.SupervisionclIdSupervisioncl
                                              where planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                              select new PlaneacionclWarningViewModel
                                              {
                                                  personaclVM = persona,
                                                  supervisionclVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalclVM = causapenal,
                                                  planeacionestrategicaclVM = planeacion,
                                                  tipoAdvertencia = "Sin periodicidad de firma"
                                              };
                        break;
                    case "SIN SUPERVISION":
                        ViewDataAlertasVari = where;
                        break;
                    case "SIN FIGURA JUDICIAL":
                        ViewDataAlertasVari = joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionclVM.IdSupervisioncl) && s.supervisionclVM.EstadoSupervision == "VIGENTE");
                        break;
                    case "SE PASO EL TIEMPO DE LA FIRMA":
                        ViewDataAlertasVari = from t in table
                                              where t.personaclVM.Supervisor != null && t.personaclVM.Supervisor != null && t.personaclVM.Supervisor.EndsWith("\u0040dgepms.com") && t.planeacionestrategicaclVM.FechaProximoContacto != null && t.planeacionestrategicaclVM.FechaProximoContacto < fechaHoy && t.supervisionclVM.EstadoSupervision == "VIGENTE" && t.planeacionestrategicaclVM.PeriodicidadFirma != "NO APLICA"
                                              orderby t.planeacionestrategicaclVM.FechaProximoContacto descending
                                              select new PlaneacionclWarningViewModel
                                              {
                                                  personaclVM = t.personaclVM,
                                                  supervisionclVM = t.supervisionclVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalclVM = t.causapenalclVM,
                                                  planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                  beneficiosclVM = t.beneficiosclVM,
                                                  figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                  tipoAdvertencia = "Se paso el tiempo de la firma"
                                              };
                        break;
                    case "PENDIENTE DE ASIGNACION - COLABORACION":
                        ViewDataAlertasVari = from persona in personaclVM
                                              join domicilio in domicilioclVM on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              where persona.Colaboracion == "SI"
                                              select new PlaneacionclWarningViewModel
                                              {
                                                  personaclVM = persona,
                                                  municipiosVM = municipio,
                                                  tipoAdvertencia = "Pendiente de asignación - colaboración"
                                              };
                        break;
                        //case "ESTADO PROCESAL":
                        //    ViewDataAlertasVari = from t in tableAudiot
                        //                          where t.personaVM.Supervisor != null && t.auditVM.DateTime < fechaProcesal && t.supervisionVM.EstadoSupervision != "CONCLUIDO"
                        //                          select new PlaneacionWarningViewModel
                        //                          {
                        //                              personaVM = t.personaVM,
                        //                              municipiosVM = t.municipiosVM,
                        //                              supervisionVM = t.supervisionVM,
                        //                              causapenalVM = t.causapenalVM,
                        //                              planeacionestrategicaVM = t.planeacionestrategicaVM,
                        //                              fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                        //                              tipoAdvertencia = "Estado Procesal",
                        //                              auditVM = t.auditVM
                        //                          };
                        //    break;
                }

                var warnings = Enumerable.Empty<PlaneacionclWarningViewModel>();
                //if (usuario == "janeth@nortedgepms.com" || flagMaster == true)
                //{
                //    var filteredWarnings = from pwvm in ViewDataAlertasVari
                //                           where pwvm.personaVM.Supervisor != null && pwvm.personaVM.Supervisor.EndsWith("\u0040nortedgepms.com")
                //                           select pwvm;
                //    warnings = warnings.Union(filteredWarnings);
                //}
                if (usuario == "andrea.valdez@dgepms.com" || flagMaster == true)
                {
                    var filteredWarnings = from pwvm in ViewDataAlertasVari
                                           where pwvm.personaclVM.Supervisor != null && pwvm.personaclVM.Supervisor.EndsWith("\u0040dgepms.com")
                                           select pwvm;
                    warnings = warnings.Union(filteredWarnings);
                }
                ViewData["alertas"] = warnings;
            }
            else
            {
                //List<Archivointernomcscp> queryHistorialArchivo = (from a in _context.Archivointernomcscp
                //                                                   group a by a.PersonaIdPersona into grp
                //                                                   select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();

                switch (currentFilter)
                {
                    case "TODOS":
                        ViewData["alertas"] = (where2).Union
                                              (sinResolucion2).Union
                                              //(archivo).Union
                                              (joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionclVM.IdSupervisioncl) && s.personaclVM.Supervisor == usuario && s.supervisionclVM.EstadoSupervision == "VIGENTE")).Union
                                                (from t in table
                                                 where t.personaclVM.Supervisor == usuario && t.planeacionestrategicaclVM.InformeInicial != null && t.planeacionestrategicaclVM.InformeInicial < fechaInformeCoordinador && t.supervisionclVM.EstadoSupervision == "VIGENTE"
                                                 orderby t.planeacionestrategicaclVM.InformeInicial
                                                 select new PlaneacionclWarningViewModel
                                                 {
                                                     personaclVM = t.personaclVM,
                                                     supervisionclVM = t.supervisionclVM,
                                                     municipiosVM = t.municipiosVM,
                                                     causapenalclVM = t.causapenalclVM,
                                                     planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                     beneficiosclVM = t.beneficiosclVM,
                                                     figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                     tipoAdvertencia = "Informe fuera de tiempo"
                                                 }).Union
                                                (from t in table
                                                 where t.personaclVM.Supervisor == usuario && t.planeacionestrategicaclVM.InformeInicial != null && t.planeacionestrategicaclVM.InformeInicial < fechaControl && t.supervisionclVM.EstadoSupervision == "VIGENTE" && t.beneficiosclVM.FiguraJudicial != "SUSTITUCIÓN DE LA PENA"
                                                 select new PlaneacionclWarningViewModel
                                                 {
                                                     personaclVM = t.personaclVM,
                                                     supervisionclVM = t.supervisionclVM,
                                                     municipiosVM = t.municipiosVM,
                                                     causapenalclVM = t.causapenalclVM,
                                                     planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                     beneficiosclVM = t.beneficiosclVM,
                                                     figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                     tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                                 }).Union
                                                (from t in table
                                                 where t.personaclVM.Supervisor == usuario && t.planeacionestrategicaclVM.InformeInicial == null && t.supervisionclVM.EstadoSupervision == "VIGENTE"
                                                 orderby t.beneficiosclVM.FiguraJudicial
                                                 select new PlaneacionclWarningViewModel
                                                 {
                                                     personaclVM = t.personaclVM,
                                                     supervisionclVM = t.supervisionclVM,
                                                     municipiosVM = t.municipiosVM,
                                                     causapenalclVM = t.causapenalclVM,
                                                     planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                     beneficiosclVM = t.beneficiosclVM,
                                                     figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                     tipoAdvertencia = "Sin fecha de informe"
                                                 }).Union
                                                (from persona in personaclVM
                                                 join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                                                 join domicilio in domicilioclVM on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                                                 join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                                 join causapenal in causapenalclVM on supervision.CausaPenalclIdCausaPenalcl equals causapenal.IdCausaPenalcl
                                                 join planeacion in planeacionestrategicaclVM on supervision.IdSupervisioncl equals planeacion.SupervisionclIdSupervisioncl
                                                 where persona.Supervisor == usuario && planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                                 select new PlaneacionclWarningViewModel
                                                 {
                                                     personaclVM = persona,
                                                     supervisionclVM = supervision,
                                                     municipiosVM = municipio,
                                                     causapenalclVM = causapenal,
                                                     planeacionestrategicaclVM = planeacion,
                                                     tipoAdvertencia = "Sin periodicidad de firma"
                                                 }).Union
                                                (from t in table
                                                 where t.personaclVM.Supervisor == usuario && t.personaclVM.Supervisor != null && t.personaclVM.Supervisor != null && t.personaclVM.Supervisor.EndsWith("\u0040dgepms.com") && t.planeacionestrategicaclVM.FechaProximoContacto != null && t.planeacionestrategicaclVM.FechaProximoContacto < fechaHoy && t.supervisionclVM.EstadoSupervision == "VIGENTE" && t.planeacionestrategicaclVM.PeriodicidadFirma != "NO APLICA"
                                                 orderby t.planeacionestrategicaclVM.FechaProximoContacto descending
                                                 select new PlaneacionclWarningViewModel
                                                 {
                                                     personaclVM = t.personaclVM,
                                                     supervisionclVM = t.supervisionclVM,
                                                     municipiosVM = t.municipiosVM,
                                                     causapenalclVM = t.causapenalclVM,
                                                     planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                     beneficiosclVM = t.beneficiosclVM,
                                                     figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                     tipoAdvertencia = "Se paso el tiempo de la firma"
                                                 });
                        //(from t in tableAudiot
                        // where t.personaVM.Supervisor == usuario && t.personaVM.Supervisor != null && t.auditVM.DateTime < fechaProcesal && t.supervisionVM.EstadoSupervision != "CONCLUIDO"
                        // select new PlaneacionWarningViewModel
                        // {
                        //     personaVM = t.personaVM,
                        //     municipiosVM = t.municipiosVM,
                        //     supervisionVM = t.supervisionVM,
                        //     causapenalVM = t.causapenalVM,
                        //     planeacionestrategicaVM = t.planeacionestrategicaVM,
                        //     fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                        //     tipoAdvertencia = "Estado Procesal",
                        //     auditVM = t.auditVM
                        // });
                        break;
                    //case "EXPEDIENTE FISICO EN RESGUARDO":
                    //    ViewData["alertas"] = archivo;
                    //    break;
                    case "SIN RESOLUCION":
                        ViewData["alertas"] = sinResolucion2;
                        break;
                    case "INFORME FUERA DE TIEMPO":
                        ViewData["alertas"] = from t in table
                                              where t.personaclVM.Supervisor == usuario && t.planeacionestrategicaclVM.InformeInicial != null && t.planeacionestrategicaclVM.InformeInicial < fechaInformeCoordinador && t.supervisionclVM.EstadoSupervision == "VIGENTE" && t.beneficiosclVM.FiguraJudicial != "SUSTITUCIÓN DE LA PENA"
                                              orderby t.planeacionestrategicaclVM.InformeInicial
                                              select new PlaneacionclWarningViewModel
                                              {
                                                  personaclVM = t.personaclVM,
                                                  supervisionclVM = t.supervisionclVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalclVM = t.causapenalclVM,
                                                  planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                  beneficiosclVM = t.beneficiosclVM,
                                                  figuraJudicial = t.figuraJudicial,
                                                  tipoAdvertencia = "Informe fuera de tiempo"
                                              };
                        break;
                    case "CONTROL DE SUPERVISION A 3 DIAS O MENOS":
                        ViewData["alertas"] = from t in table
                                              where t.personaclVM.Supervisor == usuario && t.planeacionestrategicaclVM.InformeInicial != null && t.planeacionestrategicaclVM.InformeInicial < fechaControl && t.supervisionclVM.EstadoSupervision == "VIGENTE" && t.beneficiosclVM.FiguraJudicial != "SUSTITUCIÓN DE LA PENA"
                                              select new PlaneacionclWarningViewModel
                                              {
                                                  personaclVM = t.personaclVM,
                                                  supervisionclVM = t.supervisionclVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalclVM = t.causapenalclVM,
                                                  planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                  beneficiosclVM = t.beneficiosclVM,
                                                  figuraJudicial = t.figuraJudicial,
                                                  tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                              };
                        break;
                    case "SIN FECHA DE INFORME":
                        ViewData["alertas"] = from t in table
                                              where t.personaclVM.Supervisor == usuario && t.planeacionestrategicaclVM.InformeInicial == null && t.supervisionclVM.EstadoSupervision == "VIGENTE"
                                              orderby t.beneficiosclVM.FiguraJudicial
                                              select new PlaneacionclWarningViewModel
                                              {
                                                  personaclVM = t.personaclVM,
                                                  supervisionclVM = t.supervisionclVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalclVM = t.causapenalclVM,
                                                  planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                  beneficiosclVM = t.beneficiosclVM,
                                                  figuraJudicial = t.figuraJudicial,
                                                  tipoAdvertencia = "Sin fecha de informe"
                                              };
                        break;
                    case "SIN PERIODICIDAD DE FIRMA":
                        ViewData["alertas"] = from persona in personaclVM
                                              join supervision in supervisionclVM on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                                              join domicilio in domicilioclVM on persona.IdPersonaCl equals domicilio.PersonaclIdPersonacl
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join causapenal in causapenalclVM on supervision.CausaPenalclIdCausaPenalcl equals causapenal.IdCausaPenalcl
                                              join planeacion in planeacionestrategicaclVM on supervision.IdSupervisioncl equals planeacion.SupervisionclIdSupervisioncl
                                              where persona.Supervisor == usuario && planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                              select new PlaneacionclWarningViewModel
                                              {
                                                  personaclVM = persona,
                                                  supervisionclVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalclVM = causapenal,
                                                  planeacionestrategicaclVM = planeacion,
                                                  tipoAdvertencia = "Sin periodicidad de firma"
                                              };
                        break;
                    case "SIN SUPERVISION":
                        ViewData["alertas"] = where2;
                        break;
                    case "SIN FIGURA JUDICIAL":
                        ViewData["alertas"] = joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionclVM.IdSupervisioncl) && s.personaclVM.Supervisor == usuario && s.supervisionclVM.EstadoSupervision == "VIGENTE");
                        break;
                    case "SE PASO EL TIEMPO DE LA FIRMA":
                        ViewData["alertas"] = from t in table
                                              where t.personaclVM.Supervisor != null && t.personaclVM.Supervisor.EndsWith("\u0040dgepms.com") && t.personaclVM.Supervisor == usuario && t.planeacionestrategicaclVM.FechaProximoContacto != null && t.planeacionestrategicaclVM.FechaProximoContacto < fechaHoy && t.supervisionclVM.EstadoSupervision == "VIGENTE" && (t.planeacionestrategicaclVM.PeriodicidadFirma == null || t.planeacionestrategicaclVM.PeriodicidadFirma != "NO APLICA")
                                              orderby t.planeacionestrategicaclVM.FechaProximoContacto descending
                                              select new PlaneacionclWarningViewModel
                                              {
                                                  personaclVM = t.personaclVM,
                                                  supervisionclVM = t.supervisionclVM,
                                                  beneficiosclVM = t.beneficiosclVM,
                                                  figuraJudicial = t.beneficiosclVM.FiguraJudicial,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalclVM = t.causapenalclVM,
                                                  planeacionestrategicaclVM = t.planeacionestrategicaclVM,
                                                  tipoAdvertencia = "Se paso el tiempo de la firma"
                                              };
                        break;
                        //case "ESTADO PROCESAL":
                        //    ViewData["alertas"] = from t in tableAudiot
                        //                          where t.personaVM.Supervisor == usuario && t.personaVM.Supervisor != null && t.auditVM.DateTime < fechaProcesal && t.supervisionVM.EstadoSupervision != "CONCLUIDO"
                        //                          select new PlaneacionWarningViewModel
                        //                          {
                        //                              personaVM = t.personaVM,
                        //                              municipiosVM = t.municipiosVM,
                        //                              supervisionVM = t.supervisionVM,
                        //                              causapenalVM = t.causapenalVM,
                        //                              planeacionestrategicaVM = t.planeacionestrategicaVM,
                        //                              fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                        //                              tipoAdvertencia = "Estado Procesal",
                        //                              auditVM = t.auditVM
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

        #region -Delete-
        public JsonResult antesdelete(Personacl personacl, Historialeliminacion historialeliminacion, string[] datoPersona)
        {
            var borrar = false;
            var idpersona = Int32.Parse(datoPersona[0]);

            var query = (from p in _context.Personacl
                         where p.IdPersonaCl == idpersona
                         select p).FirstOrDefault();


            var antesDel = from s in _context.Supervisioncl
                           where s.PersonaclIdPersonacl == idpersona
                           select s;

            var antesDel2 = from pp in _context.Presentacionperiodicacl
                            join rh in _context.Registrohuellacl on pp.IdregistroHuellacl equals rh.IdregistroHuellacl
                            where rh.PersonaclIdPersonacl == idpersona
                            select pp;

            var nom = query.NombreCompleto;

            if (antesDel.Any() || antesDel2.Any())
            {
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("index", "Personacls"), borrar = borrar, nombre = nom });
            }
            var stadoc = (from p in _context.Personacl
                          where p.IdPersonaCl == personacl.IdPersonaCl
                          select p.IdPersonaCl).FirstOrDefault();

            return Json(new { success = true, responseText = Convert.ToString(stadoc), idPersonas = Convert.ToString(personacl.IdPersonaCl) });

        }

        public JsonResult deletePersona(Personacl personacl, Historialeliminacion historialeliminacion, string[] datoPersona)
        {
            var borrar = false;
            var idpersona = Int32.Parse(datoPersona[0]);
            var razon = mg.normaliza(datoPersona[1]);
            var user = mg.normaliza(datoPersona[2]);

            var query = (from p in _context.Personacl
                         where p.IdPersonaCl == idpersona
                         select p).FirstOrDefault();
            try
            {
                borrar = true;
                historialeliminacion.Id = idpersona;
                historialeliminacion.Descripcion = query.Paterno + " " + query.Materno + " " + query.Nombre;
                historialeliminacion.Tipo = "PERSONACL";
                historialeliminacion.Razon = mg.normaliza(razon);
                historialeliminacion.Usuario = user;
                historialeliminacion.Fecha = DateTime.Now;
                historialeliminacion.Supervisor = mg.normaliza(query.Supervisor);
                _context.Add(historialeliminacion);
                _context.SaveChanges();
                _context.Database.ExecuteSqlCommand("CALL spBorrarPersonacl(" + idpersona + ")");
                return Json(new { success = true, responseText = Url.Action("index", "Personacls"), borrar = borrar });
            }
            catch (Exception ex)
            {
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("index", "Personacls"), borrar = borrar, error = ex });
            }
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var persona = await _context.Personacl.SingleOrDefaultAsync(m => m.IdPersonaCl == id);
            _context.Personacl.Remove(persona);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region -Presentaciones periodicas-
        public async Task<IActionResult> PresentacionPeriodicaPersona(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.Invitado = true;
            foreach (var rol in roles)
            {
                if (rol == "AdminLC" || rol == "Masteradmin" || rol == "SupervisorLC")
                {
                    ViewBag.Invitado = false;
                    break;
                }
            }

            List<Presentacionperiodicacl> lists = new List<Presentacionperiodicacl>();

            var queripersonasis = from p in _context.Personacl
                                  join rh in _context.Registrohuellacl on p.IdPersonaCl equals rh.PersonaclIdPersonacl
                                  join pp in _context.Presentacionperiodicacl on rh.IdregistroHuellacl equals pp.IdregistroHuellacl
                                  where p.IdPersonaCl == id
                                  select new PresentacionPeriodicaclPersonacl
                                  {
                                      presentacionperiodicaVM = pp,
                                      registrohuellaVM = rh,
                                      personaVM = p
                                  };

            var maxfra = queripersonasis.OrderByDescending(u => u.presentacionperiodicaVM.FechaFirma);

            if (queripersonasis.Count() == 0)
            {
                return RedirectToAction("PresentacionPeriodicaConfirmation/" + "Personascl");
            }
            else
            {
                ViewData["joinTablasPresentacion"] = maxfra;
                return View();
            }
        }

        public async Task<IActionResult> PresentacionPeriodicaConfirmation()
        {
            return View();
        }

        private bool PresentacionExists(int id)
        {
            return _context.Presentacionperiodicacl.Any(e => e.IdpresentacionPeriodicacl == id);
        }

        #region -EditarComentario-
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditComentario(int id, int idpersonacl, [Bind("IdpresentacionPeriodicacl,FechaFirma,ComentarioFirma,IdregistroHuellacl")] Presentacionperiodicacl presentacionperiodicacl)
        {
            id = presentacionperiodicacl.IdpresentacionPeriodicacl;
            presentacionperiodicacl.IdregistroHuellacl = presentacionperiodicacl.IdregistroHuellacl;
            presentacionperiodicacl.ComentarioFirma = presentacionperiodicacl.ComentarioFirma != null ? presentacionperiodicacl.ComentarioFirma.ToUpper() : "NA";
            presentacionperiodicacl.FechaFirma = presentacionperiodicacl.FechaFirma;

            var oldDomicilio = await _context.Presentacionperiodicacl.FindAsync(presentacionperiodicacl.IdpresentacionPeriodicacl);
            _context.Entry(oldDomicilio).CurrentValues.SetValues(presentacionperiodicacl);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PresentacionExists(presentacionperiodicacl.IdpresentacionPeriodicacl))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("PresentacionPeriodicaPersona/" + idpersonacl);
        }
        #endregion

        #endregion

        #region -Procesos-
        public async Task<IActionResult> Procesos(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervisiones = await _context.Supervisioncl.Where(m => m.PersonaclIdPersonacl == id).ToListAsync();
            if (supervisiones.Count == 0)
            {
                return RedirectToAction("SinSupervision");
            }


            List<Beneficios> beneficiosVM = _context.Beneficios.ToList();


            List<Beneficios> queryBeneficios = (from b in beneficiosVM
                                                group b by b.SupervisionclIdSupervisioncl into grp
                                                select grp.OrderByDescending(b => b.IdBeneficios).FirstOrDefault()).ToList();


            var queryCausas = from c in _context.Causapenalcl
                              join s in _context.Supervisioncl on c.IdCausaPenalcl equals s.CausaPenalclIdCausaPenalcl
                              join p in _context.Personacl on s.PersonaclIdPersonacl equals p.IdPersonaCl
                              join pe in _context.Planeacionestrategicacl on s.IdSupervisioncl equals pe.SupervisionclIdSupervisioncl
                              join f in queryBeneficios on s.IdSupervisioncl equals f.SupervisionclIdSupervisioncl into tmp
                              from sinfracciones in tmp.DefaultIfEmpty()
                              where p.IdPersonaCl == id
                              select new Procesoscl
                              {
                                  supervisionVM = s,
                                  causapenalVM = c,
                                  personaVM = p,
                                  planeacionestrategicaVM = pe,
                                  beneficiosVM = ((sinfracciones == null) ? null : sinfracciones)
                              };

            ViewData["joinTbalasProceso1"] = queryCausas.ToList();

            return View();
        }
        #endregion

        #region -Reportes-
        public ActionResult ReportePersona()
        {
            return View();
        }
        #region -Crea QR-
        public void creaQR(int? id)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("10.6.60.190/Personacls/Details/" + id, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            System.IO.FileStream fs = System.IO.File.Open(this._hostingEnvironment.WebRootPath + "\\images\\QR.jpg", FileMode.Create);
            qrCodeImage.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
            fs.Close();
        }
        #endregion

        public void Imprimir(int? id)
        {
            var personacl = _context.Personacl
               .SingleOrDefault(m => m.IdPersonaCl == id);


            #region -To List databases-

            List<Personacl> personaclVM = _context.Personacl.ToList();
            List<Domiciliocl> domicilioclVM = _context.Domiciliocl.ToList();
            List<Estudioscl> estudiosclVM = _context.Estudioscl.ToList();
            List<Estados> estados = _context.Estados.ToList();
            List<Municipios> municipios = _context.Municipios.ToList();
            List<Domiciliosecundariocl> domicilioSecundarioclVM = _context.Domiciliosecundariocl.ToList();
            List<Consumosustanciascl> consumoSustanciasclVM = _context.Consumosustanciascl.ToList();
            List<Trabajocl> trabajoclVM = _context.Trabajocl.ToList();
            List<Actividadsocialcl> actividadSocialclVM = _context.Actividadsocialcl.ToList();
            List<Abandonoestadocl> abandonoEstadoclVM = _context.Abandonoestadocl.ToList();
            List<Saludfisicacl> saludFisicaclVM = _context.Saludfisicacl.ToList();
            List<Familiaresforaneoscl> familiaresForaneosclVM = _context.Familiaresforaneoscl.ToList();
            List<Asientofamiliarcl> asientoFamiliarclVM = _context.Asientofamiliarcl.ToList();

            #endregion

            #region -Jointables-
            List<PersonaclsViewModal> vistaPersona = (from PersonaTableCL in personaclVM
                                                      join DomicilioCL in domicilioclVM on personacl.IdPersonaCl equals DomicilioCL.PersonaclIdPersonacl
                                                      join EstudiosCL in estudiosclVM on personacl.IdPersonaCl equals EstudiosCL.PersonaClIdPersonaCl
                                                      join TrabajoCL in trabajoclVM on personacl.IdPersonaCl equals TrabajoCL.PersonaClIdPersonaCl
                                                      join ActividaSocialCL in actividadSocialclVM on personacl.IdPersonaCl equals ActividaSocialCL.PersonaClIdPersonaCl
                                                      join AbandonoEstadoCL in abandonoEstadoclVM on personacl.IdPersonaCl equals AbandonoEstadoCL.PersonaclIdPersonacl
                                                      join SaludFisicaCL in saludFisicaclVM on personacl.IdPersonaCl equals SaludFisicaCL.PersonaClIdPersonaCl
                                                      join NacimientoEstadoCL in estados on (Int32.Parse(personacl.Lnestado)) equals NacimientoEstadoCL.Id
                                                      join NacimientoMunicipioCL in municipios on (Int32.Parse(personacl.Lnmunicipio)) equals NacimientoMunicipioCL.Id
                                                      join DomicilioEstadoCL in estados on (Int32.Parse(DomicilioCL.Estado)) equals DomicilioEstadoCL.Id
                                                      join DomicilioMunicipioCL in municipios on (Int32.Parse(DomicilioCL.Municipio)) equals DomicilioMunicipioCL.Id
                                                      where PersonaTableCL.IdPersonaCl == id
                                                      select new PersonaclsViewModal
                                                      {
                                                          personaclVM = PersonaTableCL,
                                                          domicilioclVM = DomicilioCL,
                                                          estudiosclVM = EstudiosCL,
                                                          trabajoclVM = TrabajoCL,
                                                          actividadSocialclVM = ActividaSocialCL,
                                                          abandonoEstadoclVM = AbandonoEstadoCL,
                                                          saludFisicaclVM = SaludFisicaCL,
                                                          estadosVMPersona = NacimientoEstadoCL,
                                                          municipiosVMPersona = NacimientoMunicipioCL,
                                                          estadosVMDomicilio = DomicilioEstadoCL,
                                                          municipiosVMDomicilio = DomicilioMunicipioCL
                                                      }).ToList();
            #endregion
            creaQR(id);

            #region -GeneraDocumento-
            string templatePath = this._hostingEnvironment.WebRootPath + "\\Documentos\\templateEntrevista.docx";
            string resultPath = this._hostingEnvironment.WebRootPath + "\\Documentos\\entrevista.docx";
            string rutaFoto = ((vistaPersona[0].personaclVM.Genero == ("M")) ? "hombre.png" : "mujer.png");
            if (vistaPersona[0].personaclVM.RutaFoto != null)
            {
                rutaFoto = vistaPersona[0].personaclVM.RutaFoto;
            }
            string picPath = this._hostingEnvironment.WebRootPath + "\\Fotoscl\\" + rutaFoto;

            DocumentCore dc = DocumentCore.Load(templatePath);

            string lnacimientoCleaned = "";
            if (vistaPersona[0].personaclVM.Lnlocalidad != "NA")
            {
                if (lnacimientoCleaned != "")
                {
                    lnacimientoCleaned += ",";
                }
                lnacimientoCleaned += vistaPersona[0].personaclVM.Lnlocalidad;
            }
            if (vistaPersona[0].municipiosVMPersona.Municipio != "Sin municipio")
            {
                if (lnacimientoCleaned != "")
                {
                    lnacimientoCleaned += ",";
                }
                lnacimientoCleaned += vistaPersona[0].municipiosVMPersona.Municipio.ToUpper();
            }
            if (vistaPersona[0].estadosVMPersona.Estado != "Sin estado")
            {
                if (lnacimientoCleaned != "")
                {
                    lnacimientoCleaned += ",";
                }
                lnacimientoCleaned += vistaPersona[0].estadosVMPersona.Estado.ToUpper();
            }
            if (lnacimientoCleaned != "")
            {
                lnacimientoCleaned += ",";
            }
            lnacimientoCleaned += vistaPersona[0].personaclVM.Lnpais;
            var dataSource = new[] { new {
                nombre = vistaPersona[0].personaclVM.Paterno+" "+ vistaPersona[0].personaclVM.Materno +" "+ vistaPersona[0].personaclVM.Nombre,
                nombrepadre = vistaPersona[0].personaclVM.NombrePadre,
                nombremadre = vistaPersona[0].personaclVM.NombreMadre,
                genero = vistaPersona[0].personaclVM.Genero,
                lnacimiento = lnacimientoCleaned,
                fnacimiento =(Convert.ToDateTime(vistaPersona[0].personaclVM.Fnacimiento)).ToString("dd MMMM yyyy"),
                edad=vistaPersona[0].personaclVM.Edad,
                estadocivil=vistaPersona[0].personaclVM.EstadoCivil,
                duracionestadocivil=vistaPersona[0].personaclVM.Duracion,
                hablaidioma=vistaPersona[0].personaclVM.OtroIdioma,
                especifiqueidioma=vistaPersona[0].personaclVM.EspecifiqueIdioma,
                leerescribir=vistaPersona[0].personaclVM.LeerEscribir,
                traductor=vistaPersona[0].personaclVM.Traductor,
                especifiquetraductor=vistaPersona[0].personaclVM.EspecifiqueTraductor,
                telefono=vistaPersona[0].personaclVM.TelefonoFijo,
                celular=vistaPersona[0].personaclVM.Celular,
                hijos=vistaPersona[0].personaclVM.Hijos,
                cuantoshijos=vistaPersona[0].personaclVM.Nhijos,
                personasvive=vistaPersona[0].personaclVM.NpersonasVive,
                otraspropiedades=vistaPersona[0].personaclVM.Propiedades,
                curp=vistaPersona[0].personaclVM.Curp,
                consumosustancias=vistaPersona[0].personaclVM.ConsumoSustancias,
                familiares=vistaPersona[0].personaclVM.Familiares,
                referenciasPersonales=vistaPersona[0].personaclVM.ReferenciasPersonales,
                tipopropiedad=vistaPersona[0].domicilioclVM.TipoDomicilio,
                direccion=vistaPersona[0].domicilioclVM.Calle+" "+vistaPersona[0].domicilioclVM.No+", "+vistaPersona[0].domicilioclVM.NombreCf+" CP "+vistaPersona[0].domicilioclVM.Cp+", "+vistaPersona[0].estadosVMDomicilio.Estado+", "+vistaPersona[0].municipiosVMDomicilio.Municipio+", "+vistaPersona[0].domicilioclVM.Pais,
                tiempoendomicilio=vistaPersona[0].domicilioclVM.Temporalidad,
                residenciahabitual=vistaPersona[0].domicilioclVM.ResidenciaHabitual,
                referenciasdomicilio=vistaPersona[0].domicilioclVM.Referencias,
                horariodomicilio=vistaPersona[0].domicilioclVM.Horario,
                observacionesdomicilio=vistaPersona[0].domicilioclVM.Observaciones,
                domiciliosecundario=vistaPersona[0].domicilioclVM.DomcilioSecundario,
                estudia=vistaPersona[0].estudiosclVM.Estudia,
                gradoestudios=vistaPersona[0].estudiosclVM.GradoEstudios,
                institucionestudios=vistaPersona[0].estudiosclVM.InstitucionE,
                horarioescuela=vistaPersona[0].estudiosclVM.Horario,
                direccionescuela=vistaPersona[0].estudiosclVM.Direccion,
                telefonoescuela=vistaPersona[0].estudiosclVM.Telefono,
                observacionesescolaridad=vistaPersona[0].estudiosclVM.Observaciones,
                trabaja=vistaPersona[0].trabajoclVM.Trabaja,
                tipoocupacion=vistaPersona[0].trabajoclVM.TipoOcupacion,
                puesto=vistaPersona[0].trabajoclVM.Puesto,
                empleador=vistaPersona[0].trabajoclVM.EmpledorJefe,
                enteradoprocesotrabajo=vistaPersona[0].trabajoclVM.EnteradoProceso,
                sepuedeenterartrabajo=vistaPersona[0].trabajoclVM.SePuedeEnterar,
                tiempotrabajando=vistaPersona[0].trabajoclVM.TiempoTrabajano,
                salario= mg.Dinero(vistaPersona[0].trabajoclVM.Salario),
                temporalidadpago=vistaPersona[0].trabajoclVM.TemporalidadSalario,
                direcciontrabajo=vistaPersona[0].trabajoclVM.Direccion,
                horariotrabajo=vistaPersona[0].trabajoclVM.Horario,
                telefonotrabajo=vistaPersona[0].trabajoclVM.Telefono,
                observacionestrabajo=vistaPersona[0].trabajoclVM.Observaciones,
                tipoactividad=vistaPersona[0].actividadSocialclVM.TipoActividad,
                horarioactividad=vistaPersona[0].actividadSocialclVM.Horario,
                lugaractividad=vistaPersona[0].actividadSocialclVM.Lugar,
                telefonoactividad=vistaPersona[0].actividadSocialclVM.Telefono,
                sepuedeenteraractividad=vistaPersona[0].actividadSocialclVM.SePuedeEnterar,
                referenciaactividad=vistaPersona[0].actividadSocialclVM.Referencia,
                observacionesactividad=vistaPersona[0].actividadSocialclVM.Observaciones,
                vividofuera=vistaPersona[0].abandonoEstadoclVM.VividoFuera,
                lugaresvivido=vistaPersona[0].abandonoEstadoclVM.LugaresVivido,
                temporalidadviajes=vistaPersona[0].abandonoEstadoclVM.TiempoVivido,
                motivovivido=vistaPersona[0].abandonoEstadoclVM.MotivoVivido,
                viajahabitualmente=vistaPersona[0].abandonoEstadoclVM.ViajaHabitual,
                lugaresviaje=vistaPersona[0].abandonoEstadoclVM.LugaresViaje,
                tiempoviajes=vistaPersona[0].abandonoEstadoclVM.TiempoViaje,
                motivoviajes=vistaPersona[0].abandonoEstadoclVM.MotivoViaje,
                documentacion=vistaPersona[0].abandonoEstadoclVM.DocumentacionSalirPais,
                pasaporte=vistaPersona[0].abandonoEstadoclVM.Pasaporte,
                visa=vistaPersona[0].abandonoEstadoclVM.Visa,
                familiaresfuera=vistaPersona[0].abandonoEstadoclVM.FamiliaresFuera,
                enfermedades=vistaPersona[0].saludFisicaclVM.Enfermedad,
                especenfermedad=vistaPersona[0].saludFisicaclVM.EspecifiqueEnfermedad,
                tratamientomedico=vistaPersona[0].saludFisicaclVM.Tratamiento,
                discapacidad=vistaPersona[0].saludFisicaclVM.Discapacidad,
                especdiscapacidad=vistaPersona[0].saludFisicaclVM.EspecifiqueDiscapacidad,
                serviciomedico=vistaPersona[0].saludFisicaclVM.ServicioMedico,
                tiposervicio=vistaPersona[0].saludFisicaclVM.EspecifiqueServicioMedico,
                institucionsalud=vistaPersona[0].saludFisicaclVM.InstitucionServicioMedico,
                observacionessalud=vistaPersona[0].saludFisicaclVM.Observaciones

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

        #endregion

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
