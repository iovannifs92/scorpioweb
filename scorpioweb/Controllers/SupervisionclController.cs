using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Class;
using scorpioweb.Models;
using SautinSoft.Document.MailMerging;
using SautinSoft.Document;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DocumentFormat.OpenXml.Spreadsheet;
using Org.BouncyCastle.Crypto;
using Microsoft.AspNetCore.SignalR;
using scorpioweb.Migrations.ApplicationDb;

namespace scorpioweb.Controllers
{
    public class SupervisionclController : Controller
    {
        #region -Constructor-
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IHubContext<HubNotificacion> _hubContext;
        public SupervisionclController(penas2Context context, IHostingEnvironment hostingEnvironment, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IHubContext<HubNotificacion> hubContext)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
            _hubContext = hubContext;
        }
        #endregion  
        private readonly penas2Context _context;
        #region -Metodos Generlaes -
        MetodosGenerales mg = new MetodosGenerales();

        #region -PermisosEdicion-
        public async Task<IActionResult> PermisosEdicion(int? id)
        {
            #region -PermisosEdicion-
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            bool flagCoordinador = false;
            bool flagUser = false;

            foreach (var rol in roles)
            {
                if (rol == "SupervisorLC" || rol == "AdminLC" || rol == "Masteradmin")
                {
                    flagCoordinador = true;
                }
            }

            var query = from persona in _context.Personacl
                        join supervision in _context.Supervisioncl on persona.IdPersonaCl equals supervision.PersonaclIdPersonacl
                        where supervision.IdSupervisioncl == id
                        select new
                        {
                            supervisor = persona.Supervisor
                        };

            foreach (var s in query)
            {
                if (s.supervisor == user.ToString())
                {
                    flagUser = true;
                }
            }

            if (flagCoordinador || flagUser)
            {
                ViewBag.usuario = true;
            }
            else
            {
                ViewBag.usuario = false;
            }

            return null;
            #endregion
        }
        #endregion
        #endregion


        #region -Variables globales-

        private readonly List<SelectListItem> listaNaSiNo = new List<SelectListItem>

        {
            new SelectListItem{ Text="NA", Value="NA"},
            new SelectListItem{ Text="Si", Value="SI"},
            new SelectListItem{ Text="No", Value="NO"}
        };

        private readonly List<SelectListItem> listaEstadosSupervision = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Todos", Value = "" },
            new SelectListItem{ Text = "Concluido", Value = "CONCLUIDO" },
            new SelectListItem{ Text = "Vigente", Value = "VIGENTE" },
            new SelectListItem{ Text = "En espera de respuesta", Value = "EN ESPERA DE RESPUESTA" }
        };
        private readonly List<SelectListItem> listaBeneficios = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Todos", Value = "" },
            new SelectListItem{ Text = "LIBERTAD ABSOLUTA", Value = "LIBERTAD ABSOLUTA" },
            new SelectListItem{ Text = "LIBERTAD ANTICIPADA", Value = "LIBERTAD ANTICIPADA" },
            new SelectListItem{ Text = "LIBERTAD CONDICIONADA", Value = "LIBERTAD CONDICIONADA" },
            new SelectListItem{ Text = "MEDIDA DE SEGURIDAD", Value = "MEDIDA DE SEGURIDAD" },
            new SelectListItem{ Text = "SUSPENSION CONDICIONAL CONDENA", Value = "SUSPENSION CONDICIONAL CONDENA" },
            new SelectListItem{ Text = "SUSPENSION CONDICIONAL DE LA EJECUCION DE LA PENA", Value = "SUSPENSION CONDICIONAL DE LA EJECUCION DE LA PENA" },
            new SelectListItem{ Text = "SUSTITUCIÓN DE LA PENA", Value = "SUSTITUCIÓN DE LA PENA" },
            new SelectListItem{ Text = "SUSTITUCIÓN DE LA PRISION PREVENTIVA POR PRISION DOMICILIARIA", Value = "SUSTITUCIÓN DE LA PRISION PREVENTIVA POR PRISION DOMICILIARIA" },
            new SelectListItem{ Text = "SUSTITUCIÓN POR CONFINAMIENTO", Value = "SUSTITUCIÓN POR CONFINAMIENTO" },
            new SelectListItem{ Text = "TRATAMIENTO PRELIBERACIONAL PREVIO A LA LIBERTAD ABSOLUTA", Value = "TRATAMIENTO PRELIBERACIONAL PREVIO A LA LIBERTAD ABSOLUTA" }
        };

        private readonly List<SelectListItem> listaSiNoNa = new List<SelectListItem>
        {
            new SelectListItem{ Text="Si", Value="SI"},
            new SelectListItem{ Text="No", Value="NO"},
            new SelectListItem{ Text="NA", Value="NA"}
        };
        private List<SelectListItem> listaNoSiNa = new List<SelectListItem>
        {
            new SelectListItem{ Text="No", Value="NO"},
            new SelectListItem{ Text="Si", Value="SI"},
            new SelectListItem{ Text="NA", Value="NA"}
        };

        List<string> listaCondiciones = new List<string>()
        {
            "I",
            "II",
            "III",
            "IV",
            "V",
            "VI"
        };

        private List<SelectListItem> listaCumplimiento = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Cumplimiento", Value = "CUMPLIMIENTO" },
            new SelectListItem{ Text = "Incumplimiento", Value = "INCUMPLIMIENTO" },
        };
        private List<SelectListItem> listaCierreCaso = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Libertad Absoluta", Value = "LIBERTAD ABSOLUTA" },
            new SelectListItem{ Text = "Por declinación", Value = "POR DECLINACION" },
            new SelectListItem{ Text = "Beneficio", Value = "BENEFICIO" },
            new SelectListItem{ Text = "Orden de Aprhensión", Value = "ORDEN DE APREHENSIÓN" },
            new SelectListItem{ Text = "Resolición", Value = "RESOLUCION" },
            new SelectListItem{ Text = "Revocación", Value = "REVOCACION" },
            new SelectListItem{ Text = "Sobreseimiento por extinción de la acción penal", Value = "SOBRESEIMIENTO POR EXTINCIÓN DE LA ACCION PENAL" }
        };

        private List<SelectListItem> listaMotivoAprobacion = new List<SelectListItem>
        {
            new SelectListItem{ Text = "NA", Value = "NA" },
            new SelectListItem{ Text = "Cambio de MC a SCP", Value = "CAMBIO DE MC A SCP" },
            new SelectListItem{ Text = "Cambio de SCP a MC", Value = "CAMBIO DE SCP A MC" },
            new SelectListItem{ Text = "Cambio de MCPP a MC", Value = "CAMBIO DE MCPP A MC" },
            new SelectListItem{ Text = "Cambio de MC a MCPP", Value = "CAMBIO DE MC A MCPP" },
            new SelectListItem{ Text = "Cambio de SCP a MCPP", Value = "CAMBIO DE SCP A MCPP" }
        };
        private List<SelectListItem> listaPeridodicidad = new List<SelectListItem>
        {
            new SelectListItem{ Text = "", Value = "" },
            new SelectListItem{ Text = "Diaria", Value = "DIARIA" },
            new SelectListItem { Text = "Semanal", Value = "SEMANAL" },
            new SelectListItem { Text = "Quincenal", Value = "QUINCENAL" },
            new SelectListItem { Text = "Mensual", Value = "MENSUAL" },
            new SelectListItem { Text = "Bimestral", Value = "BIMESTRAL" },
            new SelectListItem { Text = "Trimestral", Value = "TRIMESTRAL" },
            new SelectListItem { Text = "Semestral", Value = "SEMESTRAL" },
            new SelectListItem { Text = "Anual", Value = "ANUAL", },
            new SelectListItem { Text = "No aplica", Value = "NO APLICA" }
        };
        private List<SelectListItem> listaBitacoras = new List<SelectListItem>
        {
            new SelectListItem{ Text = "", Value = "" },
            new SelectListItem{ Text = "Resolucion", Value = "RESOLUCION" },
            new SelectListItem { Text = "Notificacion", Value = "NOTIFICACION" },
            new SelectListItem { Text = "Verificacion", Value = "VERIFICACION" },
            new SelectListItem { Text = "Informe Inicial", Value = "INFORME INICIAL" },
            new SelectListItem { Text = "Informe de Supervision", Value = "INFORME DE SUPERVISION" },
            new SelectListItem { Text = "Visita Domiciliaria", Value = "VISITA DOMICILIARIA" },
            new SelectListItem { Text = "Oficio de Vigilancia", Value = "OFICIO DE VIGILANCIA" },
            new SelectListItem { Text = "Plan de Estrategia", Value = "PLAN DE ESTRATEGIA" }
        };
        private List<SelectListItem> listaPersona = new List<SelectListItem>
        {
             new SelectListItem{ Text="Supervisado", Value="SUPERVISADO"},
             new SelectListItem{ Text="Víctima", Value="VICTIMA"},
        };


        #endregion

        #region -PersonaSupervision-
        // GET: Supervisioncl
        public async Task<IActionResult> Index(
           string sortOrder,
           string currentFilter,
           string searchString,
           string estadoSuper,
           string figuraJudicial,
           int? pageNumber,
           DateTime FechaTermino1,
           DateTime FechaTermino2
           )

        {
            #region
            #endregion
    
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
            bool supervisor = false;

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.User = user.ToString();
            ViewBag.Admin = false;
            ViewBag.Masteradmin = false;
            ViewBag.Archivo = false;


            #region -Solicitud Atendida Archivo prestamo Digital-
            var warningRespuesta = from a in _context.Archivoprestamodigital
                                   where a.EstadoPrestamo == 1 && user.ToString().ToUpper() == a.Usuario.ToUpper()
                                   select a;
            ViewBag.WarningsUser = warningRespuesta.Count();
            #endregion


            foreach (var rol in roles)
            {
                if (rol == "AdminLC")
                {
                    ViewBag.Admin = true;
                    supervisor = true;
                }
            }
            foreach (var rol in roles)
            {
                if (rol == "Masteradmin")
                {
                    ViewBag.Masteradmin = true;
                    supervisor = true;
                }
            }
            List<Beneficios> BeneficiosVM = _context.Beneficios.ToList();

            List<Beneficios> queryBeneficios = (from b in BeneficiosVM
                                                group b by b.SupervisionclIdSupervisioncl into grp
                                                select grp.OrderByDescending(b => b.IdBeneficios).FirstOrDefault()).ToList();

            List<Supervisioncl> querySupervisionSinBenefico = (from s in _context.Supervisioncl
                                                               join b in _context.Beneficios on s.IdSupervisioncl equals b.SupervisionclIdSupervisioncl into SupervisionFracciones
                                                               from sf in SupervisionFracciones.DefaultIfEmpty()
                                                               select new Supervisioncl
                                                               {
                                                               }).ToList();


            List<Cierredecasocl> queryFile = (from c in _context.Cierredecasocl
                                              join s in _context.Supervisioncl on c.SupervisionclIdSupervisioncl equals s.IdSupervisioncl
                                              select new Cierredecasocl { }).ToList();


            if (queryFile != null)
            {
                ViewBag.FileCierre = queryFile;
            }


            #region Listas Supervision
            ViewBag.listaEstadoSupervision = listaEstadosSupervision;
            ViewBag.listaFiltroEstadoSupervision = listaEstadosSupervision;
            ViewBag.listaFiguraJudicial = listaBeneficios;
            #endregion
            var filter = from p in _context.Personacl
                         join s in _context.Supervisioncl on p.IdPersonaCl equals s.PersonaclIdPersonacl
                         join cp in _context.Causapenalcl on s.CausaPenalclIdCausaPenalcl equals cp.IdCausaPenalcl
                         join pe in _context.Planeacionestrategicacl on s.IdSupervisioncl equals pe.SupervisionclIdSupervisioncl
                         join cc in _context.Cierredecasocl on s.IdSupervisioncl equals cc.SupervisionclIdSupervisioncl
                         join beneficios in queryBeneficios on s.IdSupervisioncl equals beneficios.SupervisionclIdSupervisioncl
                         into PersonaSupervisionCausaPenal
                         from beneficios in PersonaSupervisionCausaPenal.DefaultIfEmpty()
                         select new SupervisionPyCPCL
                         {
                             personaVM = p,
                             supervisionVM = s,
                             causapenalVM = cp,
                             planeacionestrategicaVM = pe,
                             cierredecasoVM = cc,
                             beneficiosVM = beneficios
                         };


            if (supervisor == false)
            {
                filter = filter.Where(p => p.personaVM.Supervisor == User.Identity.Name);
            }


            ViewData["CurrentFilter"] = searchString;
            ViewData["EstadoS"] = estadoSuper;
            ViewData["FiguraJ"] = figuraJudicial;


            ViewData["FechaTermino1"] = FechaTermino1.ToString().Equals(default(DateTime).ToString()) ? null : FechaTermino1.ToString("yyyy-MM-dd");
            ViewData["FechaTermino2"] = FechaTermino2.ToString().Equals(default(DateTime).ToString()) ? null : FechaTermino2.ToString("yyyy-MM-dd");

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = filter.Where(cl => (cl.personaVM.Paterno + " " + cl.personaVM.Materno + " " + cl.personaVM.Nombre).Contains(searchString) ||
                                              (cl.personaVM.Nombre + " " + cl.personaVM.Paterno + " " + cl.personaVM.Materno).Contains(searchString) ||
                                              cl.supervisionVM.EstadoSupervision.Contains(searchString) ||
                                              cl.causapenalVM.CausaPenal.Contains(searchString) ||
                                              cl.personaVM.Supervisor.Contains(searchString) ||
                                              cl.personaVM.Ce.Contains(searchString) ||
                                              (cl.personaVM.IdPersonaCl.ToString()).Contains(searchString)
                                              );
            }

            if (estadoSuper != null && estadoSuper != "Todos")
            {
                filter = filter.Where(cl => cl.supervisionVM.EstadoSupervision == estadoSuper);
            }

            if (figuraJudicial != null && figuraJudicial != "Todos")
            {
                if (figuraJudicial == "Sin beneficio otorgado")
                {
                    filter = filter.Where(cl => cl.beneficiosVM.FiguraJudicial == null);
                }
                else
                {
                    filter = filter.Where(cl => cl.beneficiosVM.FiguraJudicial == figuraJudicial);
                }
            }

            if (FechaTermino1 != default(DateTime) && FechaTermino2 != default(DateTime))
            {
                filter = filter.Where(cl => cl.supervisionVM.Termino >= FechaTermino1 && cl.supervisionVM.Termino <= FechaTermino2);
            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(cl => cl.personaVM.Paterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(cl => cl.causapenalVM.CausaPenal);
                    break;
                case "estado_cumplimiento_desc":
                    filter = filter.OrderByDescending(cl => cl.supervisionVM.EstadoCumplimiento);
                    break;
                default:
                    filter = filter.OrderByDescending(cl => cl.personaVM.IdPersonaCl);
                    break;
            }
            //Vigente al principio, Concluido al final
            filter = filter.OrderByDescending(cl => cl.supervisionVM.EstadoSupervision);

            //var personas = _context.Persona
            //    .FromSql("CALL informeSemanal")
            //    .ToList();


            int pageSize = 10;
            return View(await PaginatedList<SupervisionPyCPCL>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        #endregion

        #region -Supervision-
        public async Task<IActionResult> Supervision(int? id, int idpersona)
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


            var supervision = await _context.Supervisioncl.SingleOrDefaultAsync(m => m.IdSupervisioncl == id);
            if (supervision == null)
            {
                return NotFound();
            }


            List<Supervisioncl> SupervisionVM = _context.Supervisioncl.ToList();
            List<Causapenalcl> causaPenalVM = _context.Causapenalcl.ToList();
            List<Personacl> personaVM = _context.Personacl.ToList();

            #region -Oficios Vinculacion-
            var hayOficio = (from p in _context.Personacl
                             join s in _context.Supervisioncl on p.IdPersonaCl equals s.PersonaclIdPersonacl
                             join r in _context.Reinsercion on p.IdPersonaCl equals Int32.Parse(r.IdTabla) into pr
                             from reinsercion in pr.DefaultIfEmpty()
                             join c in _context.Canalizacion on reinsercion.IdReinsercion equals c.ReincercionIdReincercion into rc
                             from canalizacion in rc.DefaultIfEmpty()
                             join o in _context.Oficioscanalizacion on canalizacion.IdCanalizacion equals o.CanalizacionIdCanalizacion into co
                             from oficios in co.DefaultIfEmpty()
                             where p.IdPersonaCl == idpersona && reinsercion.Tabla == "personacl"
                             select new
                             {
                                 Persona = p,
                                 Supervision = s,
                                 Reinsercion = reinsercion,
                                 Canalizacion = canalizacion,
                                 OficiosCanalizacion = oficios
                             }).ToList();

            ViewBag.hayoficio = hayOficio.Count();


            #endregion



            #region -Jointables-
            ViewData["joinTablesSupervision"] = from supervisiontable in SupervisionVM
                                                join personatable in personaVM on supervisiontable.PersonaclIdPersonacl equals personatable.IdPersonaCl
                                                join causapenaltable in causaPenalVM on supervisiontable.CausaPenalclIdCausaPenalcl equals causapenaltable.IdCausaPenalcl
                                                where supervisiontable.IdSupervisioncl == id
                                                select new SupervisionPyCPCL
                                                {
                                                    causapenalVM = causapenaltable,
                                                    supervisionVM = supervisiontable,
                                                    personaVM = personatable
                                                };
            #endregion


            return View();
        }
        #endregion

        #region Edits 

        #region EditSupervision
        // GET: Supervisioncl/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervisioncl = await _context.Supervisioncl.SingleOrDefaultAsync(m => m.IdSupervisioncl == id);
            if (supervisioncl == null)
            {
                return NotFound();
            }
            #region Listas 
            ViewBag.listaEstadosSupervision = listaEstadosSupervision;
            ViewBag.idEstadoSupervision = mg.BuscaId(listaEstadosSupervision, supervisioncl.EstadoSupervision);

            ViewBag.listaEstadoC = listaCumplimiento;
            ViewBag.idEstadoCumplimiento = mg.BuscaId(listaCumplimiento, supervisioncl.EstadoCumplimiento);

            ViewBag.listanosi = listaNoSiNa;
            ViewBag.idTta = mg.BuscaId(listaNoSiNa, supervisioncl.Tta);
            #endregion

            return View(supervisioncl);
        }

        // POST: Supervisioncl/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSupervisioncl,Inicio,Termino,EstadoSupervision,PersonaclIdPersonacl,EstadoCumplimiento,CausaPenalclIdCausaPenalcl,Tta")] Supervisioncl supervisioncl)
        {
            if (id != supervisioncl.IdSupervisioncl)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supervisioncl);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionclExists(supervisioncl.IdSupervisioncl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + supervisioncl.IdSupervisioncl, "Supervisioncl");
            }
            return RedirectToAction("Supervision/" + supervisioncl.IdSupervisioncl, "Supervisioncl");
        }
        #endregion

        #region --Edit Beneficioos-
        public async Task<IActionResult> EditBeneficios(int? id, string nombre, string cp, string idpersona, string supervisor, int idcp)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;
            ViewBag.supervisor = supervisor;
            ViewBag.idcp = idcp;

            await PermisosEdicion(id);
            //var snbitacora = await _context.Bitacora.Where(m => m.SupervisionIdSupervision == id).ToListAsync();

            List<Beneficios> beneficios = _context.Beneficios.ToList();
            List<Bitacoracl> bitacora = _context.Bitacoracl.ToList();
            List<Supervisioncl> supervision = _context.Supervisioncl.ToList();
            List<Personacl> personas = _context.Personacl.ToList();
            List<Condicionescl> condicionescl = _context.Condicionescl.ToList();
            ViewBag.condicionescl = condicionescl;


            ViewData["Beneficios"] = from b in beneficios
                                     where b.SupervisionclIdSupervisioncl == id
                                     orderby b.SupervisionclIdSupervisioncl
                                     select b;

            ViewData["IdSupervisionGuardar"] = id;
            ViewBag.IdSupervisionGuardar = id;


            ViewBag.listaBeneficios = listaBeneficios;
            ViewBag.listaCumplimiento = listaCumplimiento;


            return View();
        }
        #endregion
        public ActionResult addCondicion(string tipo, Condicionescl condicionescl)
        {
            condicionescl.Tipo = mg.removeSpaces(mg.normaliza(tipo));
            _context.Add(condicionescl);
            _context.SaveChanges();
            return Json(new { success = true });

        }
        public async Task<IActionResult> CrearCondicion(Beneficios beneficios, string[] datosBeneficios, string[] datosidCondiciones)
        {
            if (datosBeneficios[1] != "SUSPENSION CONDICIONAL CONDENA")
            {
                for (int i = 0; i < datosidCondiciones.Length; i++)
                {
                    beneficios.SupervisionclIdSupervisioncl = Int32.Parse(datosBeneficios[0]);
                    beneficios.FiguraJudicial = datosBeneficios[1];
                    beneficios.FechaInicio = mg.validateDatetime(datosBeneficios[2]);
                    beneficios.FechaTermino = mg.validateDatetime(datosBeneficios[3]);
                    beneficios.Estado = datosBeneficios[4];
                    beneficios.Tipo = datosidCondiciones[i];

                    _context.Add(beneficios);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);

                }
            }
            else
            {
                for (int i = 0; i < datosidCondiciones.Length ; i++)
                {
                    beneficios.SupervisionclIdSupervisioncl = Int32.Parse(datosBeneficios[0]);
                    beneficios.FiguraJudicial = datosBeneficios[1];
                    beneficios.FechaInicio = mg.validateDatetime(datosBeneficios[2]);
                    beneficios.FechaTermino = mg.validateDatetime(datosBeneficios[3]);
                    beneficios.Estado = datosBeneficios[4];
                    switch (Int32.Parse(datosidCondiciones[i]))
                    {
                        case 0:
                            beneficios.Tipo = "I";
                            break;
                        case 1:
                            beneficios.Tipo = "II";
                            break;
                        case 2:
                            beneficios.Tipo = "III";
                            break;
                        case 3:
                            beneficios.Tipo = "IV";
                            break;
                        case 4:
                            beneficios.Tipo = "V";
                            break;
                        case 5:
                            beneficios.Tipo = "VI";
                            break;
                    }
                    _context.Add(beneficios);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                }
            }
            
            return View();
            //return RedirectToAction(nameof(Index));
        }

        #region -Update Persona supervision-
        public async Task<JsonResult> UpdatePersonasupervision(Supervisioncl supervision, Planeacionestrategicacl planeacionestrategica, string superid, string personaid, string causapenalid, string campo, string planeacionid, string estados, DateTime fecha, DateTime intermedio)
        {

            #region -Actualizar fechas en supervision-
            if (superid != null)
            {
                var camps = campo;
                supervision.IdSupervisioncl = Int32.Parse(superid);
                supervision.EstadoSupervision = mg.normaliza(estados);
            }

            var empty = (from s in _context.Supervisioncl
                         where s.IdSupervisioncl == supervision.IdSupervisioncl
                         select s);

            if (empty.Any())
            {
                if (campo == "Inicio")
                {
                    supervision.Inicio = fecha;
                    var query = (from s in _context.Supervisioncl
                                 where s.IdSupervisioncl == supervision.IdSupervisioncl
                                 select s).FirstOrDefault();
                    query.Inicio = supervision.Inicio;
                    _context.SaveChanges();
                }
                if (campo == "Termino")
                {
                    supervision.Termino = fecha;
                    var query = (from s in _context.Supervisioncl
                                 where s.IdSupervisioncl == supervision.IdSupervisioncl
                                 select s).FirstOrDefault();
                    query.Termino = supervision.Termino;
                    _context.SaveChanges();
                }
            }
            #endregion

            #region -actualizacion de fecha en planeacion estrategica-
            if (planeacionid != null)
            {
                planeacionestrategica.InformeSeguimiento = intermedio;
                planeacionestrategica.IdPlaneacionEstrategicacl = Int32.Parse(planeacionid);

            }

            var emptype = (from pe in _context.Planeacionestrategicacl
                           where pe.IdPlaneacionEstrategicacl == planeacionestrategica.IdPlaneacionEstrategicacl
                           select pe);
            if (emptype.Any())
            {
                var query = (from pe in _context.Planeacionestrategicacl
                             where pe.IdPlaneacionEstrategicacl == planeacionestrategica.IdPlaneacionEstrategicacl
                             select pe).FirstOrDefault();
                query.InformeSeguimiento = planeacionestrategica.InformeSeguimiento;
                _context.SaveChanges();
            }
            #endregion

            #region -actualizacion de estado de supervision-

            var empty2 = (from s in _context.Supervisioncl
                          where s.IdSupervisioncl == supervision.IdSupervisioncl
                          select s);
            if (empty2.Any())
            {

                supervision.PersonaclIdPersonacl = Int32.Parse(personaid);
                supervision.CausaPenalclIdCausaPenalcl = Int32.Parse(causapenalid);
                var supervisions = await _context.Supervisioncl.SingleOrDefaultAsync(m => m.IdSupervisioncl == supervision.IdSupervisioncl);
                supervision.Inicio = supervisions.Inicio;
                supervision.Termino = supervisions.Termino;
                supervision.Tta = supervisions.Tta;

                var query = (from s in _context.Supervisioncl
                             where s.IdSupervisioncl == supervision.IdSupervisioncl
                             select s).FirstOrDefault();
                query.EstadoSupervision = supervision.EstadoSupervision;

                try
                {
                    var oldSupervision = await _context.Supervisioncl.FindAsync(supervision.IdSupervisioncl);
                    _context.Entry(oldSupervision).CurrentValues.SetValues(supervision);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (Exception ex)
                {
                    var innerException = ex.InnerException;
                }

            }

            #endregion
            var cp = (from s in _context.Supervisioncl
                      where s.IdSupervisioncl == supervision.IdSupervisioncl
                      select s.Inicio).FirstOrDefault();


            return Json(new { success = true, responseText = Convert.ToString(cp), idPersonas = Convert.ToString(supervision.IdSupervisioncl) });
        }
        #endregion -Update Update Persona supervision-

        #region -Editar y borrar beneficios-        

        public async Task<IActionResult> EditCondiciones(string nombre, string cp, int id, string idpersona)
        {
            int index = cp.IndexOf("?");
            if (index >= 0)
                cp = cp.Substring(0, index);

            if (id == 0)
            {
                return View();
            }

            ViewBag.cp = cp;
            ViewBag.nombre = nombre;
            ViewBag.idpersona = idpersona;


            var beneficios = await _context.Beneficios.SingleOrDefaultAsync(m => m.IdBeneficios == id);
            if (beneficios == null)
            {
                return NotFound();
            }

            //List<Condicionescl> listacondiciones = _context.Condicionescl.ToList();
            var listacondiciones = (from p in _context.Condicionescl
                                   select p.Tipo).ToList();


            ViewBag.Figura = beneficios.FiguraJudicial;   
                                   

            if (beneficios.FiguraJudicial != "SUSPENSION CONDICIONAL CONDENA")
            {
                ViewBag.listacondicionescl = listacondiciones;
                ViewBag.listacondicionescl2 = listaCondiciones;
            }
            else
            {
                ViewBag.listacondicionescl = listaCondiciones;
                ViewBag.listacondicionescl2 = listacondiciones;
            }
            ViewBag.idCondicionescl =  beneficios.Tipo;

            ViewBag.listaCumplimiento = listaCumplimiento;
            ViewBag.idCumplimiento = mg.BuscaId(listaCumplimiento, beneficios.Estado);

            ViewBag.listaBeneficios = listaBeneficios;
            ViewBag.idFiguraJudicial = mg.BuscaId(listaBeneficios, beneficios.FiguraJudicial);

            return View(beneficios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCondiciones([Bind("IdBeneficios,Tipo,Autoridad,FechaInicio,FechaTermino,Estado,Evidencia,FiguraJudicial,SupervisionclIdSupervisioncl")] Beneficios beneficios, string nombre, string cp, string idpersona, string Tipo2Value)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var oldBeneficios = await _context.Beneficios.FindAsync(beneficios.IdBeneficios, beneficios.SupervisionclIdSupervisioncl);
                    _context.Entry(oldBeneficios).CurrentValues.SetValues(beneficios);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(fraccionesimpuestas);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeneficioExists(beneficios.SupervisionclIdSupervisioncl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("EditBeneficios/" + beneficios.SupervisionclIdSupervisioncl, "Supervisioncl", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona });
            }
            return View(beneficios);
        }


        public async Task<IActionResult> DeleteBeneficio(int? id, string nombre, string cp, string idpersona)
        {
            var beneficios = await _context.Beneficios.SingleOrDefaultAsync(m => m.IdBeneficios == id);
            var oldbeneficios = await _context.Beneficios.FindAsync(beneficios.IdBeneficios, beneficios.SupervisionclIdSupervisioncl);
            _context.Entry(oldbeneficios).CurrentValues.SetValues(oldbeneficios);

            _context.Beneficios.Remove(beneficios);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditBeneficios/" + beneficios.SupervisionclIdSupervisioncl, "Supervisioncl", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona });
        }


        #region -Acciones de supervision-
        public async Task<IActionResult> AddAccionSupervision(string nombre, string cp, int? id, string idpersona, string[] datosBitacora, string supervisor, int idcp, string idSupervision)
        {

            int signo = idSupervision.IndexOf("?");
            if (signo != -1)
            {

                idSupervision = idSupervision.Substring(0, signo);
            }

            if (datosBitacora[0] == null || !datosBitacora[0].Equals(idSupervision))
            {

                datosBitacora[0] = idSupervision;
            }
            int index = cp.IndexOf("?");
            if (index >= 0)
                cp = cp.Substring(0, index);


            ViewBag.cp = cp;
            ViewBag.nombre = nombre;
            ViewBag.idpersona = idpersona;
            ViewBag.supervisor = supervisor;
            ViewBag.idcp = idcp;
            ViewBag.idfraccionesimpuestas = id;

            if (id == null)
            {
                return NotFound();
            }

            List<Bitacoracl> bitacora = _context.Bitacoracl.ToList();
            List<Beneficios> Beneficios = _context.Beneficios.ToList();
            List<Supervisioncl> supervision = _context.Supervisioncl.ToList();

            int SupervisionclIdSupervisioncl = 0;
            var idsupervision = datosBitacora[0];
            if (idsupervision != null)
            {
                SupervisionclIdSupervisioncl = Int32.Parse(idsupervision);
            }

            var snbitacora = await _context.Bitacoracl.Where(m => m.BeneficiosclIdBeneficioscl == id).ToListAsync();
            if (snbitacora.Count == 0)
            {

                return RedirectToAction("CreateBitacora2", new { id, SupervisionclIdSupervisioncl, @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @supervisor = supervisor, @idcp = idcp });
            }


            #region -Select idOficialia


            var leftjoin = from o in _context.Oficialia
                           join p in _context.Personacl on o.UsuarioTurnar equals p.Supervisor
                           join s in _context.Supervisioncl on p.IdPersonaCl equals s.PersonaclIdPersonacl
                           join b in bitacora on o.IdOficialia equals b.OficialiaIdOficialia into temp
                           from bo in temp.DefaultIfEmpty()
                           select new ListaOficialiaBitacoraViewModelCL
                           {
                               OficialiaVM = o,
                               SupervisionClVM = s,
                               PersonaClVM = p,
                               BitacoraClVM = bo
                           };

            var wheres = (from bn in leftjoin
                          where bn.OficialiaVM.UsuarioTurnar == supervisor
                          group bn by bn.OficialiaVM.IdOficialia into grp
                          select grp.OrderBy(bn => bn.OficialiaVM.IdOficialia).FirstOrDefault()).ToList();


            var selects = (from wh in wheres
                           select wh.OficialiaVM.IdOficialia).ToList();

            List<SelectListItem> ListaOficios = new List<SelectListItem>();
            ListaOficios = new List<SelectListItem>
            {
              new SelectListItem{ Text="NA", Value="0"},
            };
            foreach (var select in selects)
            {
                ListaOficios.Add(
                 new SelectListItem { Text = select.ToString(), Value = select.ToString() }
                );

            }
            ViewBag.expoficialia = ListaOficios;

            #endregion

            ViewData["tablaBiatacora"] = from Bitacora in bitacora
                                         where Bitacora.BeneficiosclIdBeneficioscl == id
                                         select new BitacoraclViewModal
                                         {
                                             bitacoraVM = Bitacora
                                         };





            ViewData["tienebitacora"] = from s in supervision
                                        join b in bitacora on s.IdSupervisioncl equals b.SupervisionclIdSupervisioncl
                                        join be in Beneficios on b.BeneficiosclIdBeneficioscl equals be.IdBeneficios
                                        where s.IdSupervisioncl == id
                                        select new BitacoraclViewModal
                                        {
                                            bitacoraVM = b,
                                            supervisionVM = s,
                                            beneficiosVM = be
                                        };



            #region ListaTipoPersona

            List<SelectListItem> ListaTipoPersona;
            ListaTipoPersona = new List<SelectListItem>
            {
              new SelectListItem{ Text="Supervisado", Value="SUPERVISADO"},
              new SelectListItem{ Text="Víctima", Value="VICTIMA"},

            };
            ViewBag.TipoPersona = ListaTipoPersona;
            #endregion

            #region ListaTipoVisita

            List<SelectListItem> ListaTipoVisita;
            ListaTipoVisita = new List<SelectListItem>
            {
              new SelectListItem{ Text="Presencial", Value="PRESENCIAL"},
              new SelectListItem{ Text="Firma Periódica", Value="FIRMA PERIODICA"},
              new SelectListItem{ Text="WhatsApp", Value="WHATSAPP"},
              new SelectListItem{ Text="Telefónica", Value="TELEFONICA"},
              new SelectListItem{ Text="Correo Electrónico", Value="CORREO ELECTRONICO"},
              new SelectListItem{ Text="Citatorio", Value="CITATORIO"},
              new SelectListItem{ Text="Visita Domiciliar", Value="VISITA DOMICILIAR"},
              new SelectListItem{ Text="Notificación a Víctima", Value="NOTIFICACION A VICTIMA"},
            };
            ViewBag.TipoVisita = ListaTipoVisita;
            #endregion


            return View("AddAccionSupervisionCL");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddAccionSupervision([Bind("IdBitacoracl,Fecha,TipoPersona,Texto,TipoVisita,RutaEvidencia,OficialiaIdOficialia,FechaRegistro,SupervisionclIdSupervisioncl,BeneficiosclIdBeneficioscl ")] Bitacoracl bitacora, IFormFile evidencia, string nombre, string cp, string idpersona, string supervisor, string idcp)
        {
            bitacora.Texto = mg.normaliza(bitacora.Texto);
            bitacora.OficialiaIdOficialia = bitacora.OficialiaIdOficialia;
            bitacora.FechaRegistro = bitacora.FechaRegistro;

            var supervision = _context.Supervisioncl
               .SingleOrDefault(m => m.IdSupervisioncl == bitacora.SupervisionclIdSupervisioncl);

            if (ModelState.IsValid)
            {
                try
                {
                    bitacora.Texto = mg.normaliza(bitacora.Texto);


                    var oldBitacora = await _context.Bitacoracl.FindAsync(bitacora.IdBitacoracl, bitacora.SupervisionclIdSupervisioncl);

                    if (evidencia == null)
                    {
                        bitacora.RutaEvidencia = oldBitacora.RutaEvidencia;
                    }
                    else
                    {
                        string file_name = bitacora.IdBitacoracl + "_" + bitacora.SupervisionclIdSupervisioncl + "_" + supervision.PersonaclIdPersonacl + Path.GetExtension(evidencia.FileName);
                        bitacora.RutaEvidencia = file_name;
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "EvidenciaCL");

                        if (System.IO.File.Exists(Path.Combine(uploads, file_name)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, file_name));
                        }

                        var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                        await evidencia.CopyToAsync(stream);
                        stream.Close();
                    }

                    _context.Entry(oldBitacora).CurrentValues.SetValues(bitacora);

                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeneficioExists(bitacora.SupervisionclIdSupervisioncl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("EditBeneficios/" + bitacora.SupervisionclIdSupervisioncl, "Supervisioncl", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @supervisor = supervisor, @idcp = idcp });
            }
            return View();
        }

        public async Task<IActionResult> CrearAccionSuper(Bitacoracl bitacora, string[] datosBitacora)
        {
            bitacora.SupervisionclIdSupervisioncl = Int32.Parse(datosBitacora[0]);
            bitacora.BeneficiosclIdBeneficioscl = Int32.Parse(datosBitacora[1]);
            bitacora.Fecha = mg.validateDatetime(datosBitacora[2]);
            bitacora.TipoPersona = datosBitacora[3];
            bitacora.Texto = mg.normaliza(datosBitacora[4]);
            bitacora.TipoVisita = datosBitacora[5];
            bitacora.RutaEvidencia = datosBitacora[6];


            var supervision = _context.Supervisioncl
               .SingleOrDefault(m => m.IdSupervisioncl == bitacora.SupervisionclIdSupervisioncl);

            _context.Add(bitacora);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditBeneficios/" + bitacora.SupervisionclIdSupervisioncl, "Supervisioncl");
        }
        public async Task<IActionResult> DeleteRegistro2(int? id, string nombre, string cp, string idpersona, string supervisor, string idcp)
        {
            var Bitacora = await _context.Bitacoracl.SingleOrDefaultAsync(m => m.IdBitacoracl == id);
            var oldBitacora = await _context.Bitacoracl.FindAsync(Bitacora.IdBitacoracl, Bitacora.SupervisionclIdSupervisioncl);
            _context.Entry(oldBitacora).CurrentValues.SetValues(Bitacora);


            _context.Bitacoracl.Remove(Bitacora);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditBeneficios/" + Bitacora.SupervisionclIdSupervisioncl, "Supervisioncl", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, idcp = idcp, @supervisor = supervisor });

        }

        #endregion

        #endregion

        #region EditPlaneacionEstrategica
        // GET: Supervisioncl/Edit/5
        public async Task<IActionResult> EditPlaneacionEstrategica(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planeacionestrategicacl = await _context.Planeacionestrategicacl.SingleOrDefaultAsync(m => m.SupervisionclIdSupervisioncl == id);
            //if (planeacionestrategicacl.EstadoInfInicial == null)
            //    planeacionestrategicacl.EstadoInfInicial = 0;
            if (planeacionestrategicacl == null)
            {
                return NotFound();
            }
            #region Listas 
            ViewBag.listaplaneacion = listaNoSiNa;
            ViewBag.idPlaneacion = mg.BuscaId(listaNoSiNa, planeacionestrategicacl.PlanSupervision);

            ViewBag.listaPeriodicida = listaPeridodicidad;
            ViewBag.idPeriodicida = mg.BuscaId(listaPeridodicidad, planeacionestrategicacl.PeriodicidadFirma);
            #endregion

            return View(planeacionestrategicacl);
        }

        // POST: Supervisioncl/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlaneacionEstrategica(int id, [Bind("IdPlaneacionEstrategicacl,PlanSupervision,MotivoNoPlaneacion,VisitaVerificacion,InformeInicial,InformeSeguimiento,InformeFinal,FechaUltimoContacto,EstadoInfInicial,FechaProximoContacto,DiaFirma,PeriodicidadFirma,CausaPenalclIdCausaPenalcl,SupervisionclIdSupervisioncl,Tta")] Planeacionestrategicacl planeacionestrategicacl)
        {
            if (id != planeacionestrategicacl.SupervisionclIdSupervisioncl)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(planeacionestrategicacl);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionclExists(planeacionestrategicacl.IdPlaneacionEstrategicacl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + planeacionestrategicacl.SupervisionclIdSupervisioncl, "Supervisioncl");
            }
            return View(planeacionestrategicacl);
        }
        #endregion

        #region -EditCambiodeobligaciones-
        public async Task<IActionResult> EditCambioObligaciones(int? id, string nombre, string cp, string idpersona, string cambio)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;

            await PermisosEdicion(id);

            var cambiodeobligaciones = await _context.Cambiodeobligacionescl.SingleOrDefaultAsync(m => m.SupervisionclIdSupervisioncl == id);
            if (cambiodeobligaciones == null)
            {
                return NotFound();
            }

            ViewBag.listaSediocambio = listaSiNoNa;
            ViewBag.idSediocambio = mg.BuscaId(listaSiNoNa, cambiodeobligaciones.SeDioCambio);
            ViewBag.cambio = cambiodeobligaciones.SeDioCambio;

            ViewBag.listaMotivoAprobacion = listaMotivoAprobacion;
            ViewBag.idMotivoAprobacion = mg.BuscaId(listaMotivoAprobacion, cambiodeobligaciones.MotivoAprobacion);

            return View(cambiodeobligaciones);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCambiodeobligaciones(int id, [Bind("IdCambiodeObligacionescl,SeDioCambio,FechaAprobacion,MotivoAprobacion,SupervisionclIdSupervisioncl")] Cambiodeobligacionescl cambiodeobligacionescl)
        {
            if (id != cambiodeobligacionescl.SupervisionclIdSupervisioncl)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    cambiodeobligacionescl.MotivoAprobacion = mg.normaliza(cambiodeobligacionescl.MotivoAprobacion);
                    var oldCambiodeobligaciones = await _context.Cambiodeobligacionescl.FindAsync(cambiodeobligacionescl.IdCambiodeObligacionescl, cambiodeobligacionescl.SupervisionclIdSupervisioncl);
                    _context.Entry(oldCambiodeobligaciones).CurrentValues.SetValues(cambiodeobligacionescl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(cambiodeobligaciones);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CambiodeObligacionesclExists(cambiodeobligacionescl.SupervisionclIdSupervisioncl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + cambiodeobligacionescl.SupervisionclIdSupervisioncl, "Supervisioncl");
            }
            return View(cambiodeobligacionescl);
        }
        #endregion

        #region -EditRevocacion-


        public async Task<IActionResult> EditRevocacion(int? id, string nombre, string cp, string idpersona)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;

            await PermisosEdicion(id);

            var SupervisionCL = await _context.Revocacioncl.SingleOrDefaultAsync(m => m.SupervisionclIdSupervisioncl == id);
            if (SupervisionCL == null)
            {
                return NotFound();
            }

            ViewBag.listaRevocado = listaNaSiNo;
            ViewBag.idRevocado = mg.BuscaId(listaNaSiNo, SupervisionCL.Revocado);
            ViewBag.revocado = SupervisionCL.Revocado;




            return View(SupervisionCL);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRevocacion(int id, [Bind("IdRevocacioncl,Revocado,FechaAprobacion,MotivoRevocacion,SupervisionclIdSupervisioncl")] Revocacioncl RevocacionCL)
        {
            if (id != RevocacionCL.SupervisionclIdSupervisioncl)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    RevocacionCL.MotivoRevocacion = (RevocacionCL.Revocado.Equals("NA") || RevocacionCL.Revocado.Equals("NO")) ? "NA" : mg.normaliza(RevocacionCL.MotivoRevocacion);
                    RevocacionCL.FechaAprobacion = (RevocacionCL.Revocado.Equals("NA") || RevocacionCL.Revocado.Equals("NO")) ? new DateTime(1999, 1, 1, 0, 0, 0) : RevocacionCL.FechaAprobacion;

                    var oldRevocacion = await _context.Revocacioncl.FindAsync(RevocacionCL.IdRevocacioncl, RevocacionCL.SupervisionclIdSupervisioncl);
                    _context.Entry(oldRevocacion).CurrentValues.SetValues(RevocacionCL);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(revocacion);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionclExists(RevocacionCL.SupervisionclIdSupervisioncl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + RevocacionCL.SupervisionclIdSupervisioncl, "Supervisioncl");
            }

            return View(RevocacionCL);
        }
        #endregion

        #region -EditSuspensionseguimiento-

        public async Task<IActionResult> EditSuspensionseguimiento(int? id, string nombre, string cp, string idpersona)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;

            await PermisosEdicion(id);

            var SupervisionCL = await _context.Suspensionseguimientocl.SingleOrDefaultAsync(m => m.SupervisionclIdSupervisioncl == id);
            if (SupervisionCL == null)
            {
                return NotFound();
            }

            ViewBag.listaSuspendido = listaNaSiNo;
            ViewBag.idSuspendido = mg.BuscaId(listaNaSiNo, SupervisionCL.Suspendido);
            ViewBag.suspe = SupervisionCL.Suspendido;

            return View(SupervisionCL);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSuspensionseguimiento(int id, [Bind("IdSuspensionSeguimientocl,Suspendido,FechaAprobacion,MotivoSuspension,SupervisionclIdSupervisioncl")] Suspensionseguimientocl SuspensionseguimientoCL)
        {
            if (id != SuspensionseguimientoCL.SupervisionclIdSupervisioncl)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    SuspensionseguimientoCL.MotivoSuspension = (SuspensionseguimientoCL.Suspendido.Equals("NO") || SuspensionseguimientoCL.Suspendido.Equals("NA")) ? "NA" : mg.normaliza(SuspensionseguimientoCL.MotivoSuspension);
                    SuspensionseguimientoCL.FechaAprobacion = (SuspensionseguimientoCL.Suspendido.Equals("NO") || SuspensionseguimientoCL.Suspendido.Equals("NA")) ? new DateTime(1999, 1, 1, 0, 0, 0) : SuspensionseguimientoCL.FechaAprobacion;


                    var oldSuspensionseguimiento = await _context.Suspensionseguimientocl.FindAsync(SuspensionseguimientoCL.IdSuspensionSeguimientocl, SuspensionseguimientoCL.SupervisionclIdSupervisioncl);
                    _context.Entry(oldSuspensionseguimiento).CurrentValues.SetValues(SuspensionseguimientoCL);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(suspensionseguimiento);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionclExists(SuspensionseguimientoCL.SupervisionclIdSupervisioncl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + SuspensionseguimientoCL.SupervisionclIdSupervisioncl, "Supervisioncl");
            }
            return View();
        }
        #endregion

        #region -EditCierredecaso-
        public async Task<IActionResult> EditCierreCaso(int? id, string nombre, string cp, string idpersona)
        {
            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;

            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;

            await PermisosEdicion(id);

            var cierre = await _context.Cierredecasocl.SingleOrDefaultAsync(m => m.SupervisionclIdSupervisioncl == id);
            if (cierre == null)
            {
                return NotFound();
            }

            ViewBag.CierreCaso = listaCierreCaso;
            ViewBag.idCierreCaso = mg.BuscaId(listaCierreCaso, cierre.ComoConcluyo);
            ViewBag.listaSeCerroCaso = listaSiNoNa;
            ViewBag.idSeCerroCaso = mg.BuscaId(listaSiNoNa, cierre.SeCerroCaso);
            ViewBag.cierre = cierre.SeCerroCaso;
            #region Autorizo
            List<SelectListItem> ListaAutorizo;
            ListaAutorizo = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Director", Value = "DIRECTOR" },
                new SelectListItem{ Text = "Coordinador", Value = "COORDINADOR" }
                };

            ViewBag.listaAutorizo = ListaAutorizo;
            ViewBag.idAutorizo = mg.BuscaId(ListaAutorizo, cierre.Autorizo);
            #endregion

            ViewBag.Achivocierre = cierre.RutaArchivo;



            return View(cierre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCierreCaso(int id, [Bind("IdCierreDeCasocl,SeCerroCaso,ComoConcluyo,NoArchivo,FechaAprobacion,Autorizo,RuataArchivo,SupervisionclIdSupervisioncl")] Cierredecasocl cierredecasocl, IFormFile archivo)
        {

            var supervision = _context.Supervisioncl
               .SingleOrDefault(m => m.IdSupervisioncl == cierredecasocl.SupervisionclIdSupervisioncl);

            var personacl = _context.Personacl
              .SingleOrDefault(m => m.IdPersonaCl == supervision.PersonaclIdPersonacl);

            if (id != cierredecasocl.SupervisionclIdSupervisioncl)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {

                    cierredecasocl.ComoConcluyo = (cierredecasocl.SeCerroCaso.Equals("NO") || cierredecasocl.SeCerroCaso.Equals("NA")) ? "NA" : mg.normaliza(cierredecasocl.ComoConcluyo);
                    cierredecasocl.NoArchivo = (cierredecasocl.SeCerroCaso.Equals("NO") || cierredecasocl.SeCerroCaso.Equals("NA")) ? "NA" : mg.normaliza(cierredecasocl.NoArchivo);
                    cierredecasocl.FechaAprobacion = (cierredecasocl.SeCerroCaso.Equals("NO") || cierredecasocl.SeCerroCaso.Equals("NA")) ? new DateTime(1999, 1, 1, 0, 0, 0) : cierredecasocl.FechaAprobacion;
                    cierredecasocl.Autorizo = (cierredecasocl.SeCerroCaso.Equals("NO") || cierredecasocl.SeCerroCaso.Equals("NA")) ? "NA" : mg.normaliza(cierredecasocl.Autorizo);

                    var oldcierredecaso = await _context.Cierredecasocl.FindAsync(cierredecasocl.IdCierreDeCasocl, cierredecasocl.SupervisionclIdSupervisioncl);

                    if (archivo == null)
                    {
                        cierredecasocl.RutaArchivo = oldcierredecaso.RutaArchivo;
                    }
                    else
                    {
                        string file_name = cierredecasocl.IdCierreDeCasocl + "_" + cierredecasocl.SupervisionclIdSupervisioncl + "_" + supervision.PersonaclIdPersonacl + Path.GetExtension(archivo.FileName);
                        cierredecasocl.RutaArchivo = file_name;
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "CierredeCasocl");

                        if (System.IO.File.Exists(Path.Combine(uploads, file_name)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, file_name));
                        }

                        var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                        await archivo.CopyToAsync(stream);
                        stream.Close();
                    }

                    var user = await userManager.FindByNameAsync(User.Identity.Name);
                    var roles = await userManager.GetRolesAsync(user);


                    _context.Entry(oldcierredecaso).CurrentValues.SetValues(cierredecasocl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                    #region -Existe en reincercion-
                    if (cierredecasocl.SeCerroCaso == "SI")
                    {
                        var hayOficio = (from p in _context.Personacl
                                         join s in _context.Supervisioncl on p.IdPersonaCl equals s.PersonaclIdPersonacl
                                         join r in _context.Reinsercion on p.IdPersonaCl equals Int32.Parse(r.IdTabla)
                                         where p.IdPersonaCl == personacl.IdPersonaCl && r.Tabla == "personacl"
                                         select new
                                         {
                                             Persona = p,
                                             Supervision = s,
                                             Reinsercion = r,
                                         });

                        if (hayOficio.ToList().Count() != 0)
                        {
                            await _hubContext.Clients.Group("seCerrocaso").SendAsync("alertCierreCaso", "Realizar cancelaciones correspondientes de " + personacl.NombreCompleto + " de Libertad Condicionada.");
                        }

                        //var datos = hayOficio.First();

                        //PENDIENTES 
                        //#region -Cambiar Estado Supervision Vinculacion-
                        //var query = (from s in _context.Reinsercion
                        //             where s.IdReinsercion == datos.Reinsercion.IdReinsercion
                        //             select s).FirstOrDefault();
                        //query.Estado = "CONCLUIDO";
                        //_context.SaveChanges();
                        //#endregion
                    }
                    #endregion
                    //_context.Update(cierredecaso);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CierredecasoclExists(cierredecasocl.SupervisionclIdSupervisioncl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + cierredecasocl.SupervisionclIdSupervisioncl, "Supervisioncl", new { idpersona = personacl.IdPersonaCl});
            }
            return View(cierredecasocl);
        }
        #endregion

        #region -Bitacora-
        public async Task<IActionResult> ListaBitacora(int? id, string nombre, string cp, string idpersona, string idcp, string supervisor)
        {
            if(id == null && string.IsNullOrEmpty(idpersona))
            {
                return NotFound();
            }

            if (id == null && !string.IsNullOrEmpty(idpersona))
            {
                //SOLICITUD PROVIENE DEL BOTON BITACORA EN EL INDEX
                var idSupervision = _context.Supervisioncl
                    .SingleOrDefault(m => m.PersonaclIdPersonacl == int.Parse(idpersona));
                if(idSupervision == null)
                {
                    return RedirectToAction("SinSupervision","Personacls");
                }
                id = idSupervision.IdSupervisioncl;
            }


            var supervision = _context.Supervisioncl
            .SingleOrDefault(m => m.IdSupervisioncl == id);

            var persona = _context.Personacl
           .SingleOrDefault(m => m.IdPersonaCl == supervision.PersonaclIdPersonacl);
            var cpp = _context.Causapenalcl
           .SingleOrDefault(m => m.IdCausaPenalcl == supervision.CausaPenalclIdCausaPenalcl);
            //ViewBag.nombre = persona.NombreCompleto;
            //ViewBag.cp = cp.CausaPenal;

            //var oficialia = (from _cp in _context.Causapenal
            //                 join o in _context.Oficialia on _cp.IdCausaPenal equals o.IdCausaPenal
            //                 join b in _context.Bitacora on o.IdOficialia equals b.IdOficialia
            //                 where _cp.IdCausaPenal == cpp.IdCausaPenal);

            
            
            await PermisosEdicion(id);


            List<Bitacoracl> bitacora = _context.Bitacoracl.ToList();
            List<Beneficios> beneficios = _context.Beneficios.ToList();

            ViewData["BitacoraBeneficios"] = from b in bitacora
                                             join be in beneficios on b.BeneficiosclIdBeneficioscl equals be.IdBeneficios into tmp
                                             from fleft in tmp.DefaultIfEmpty()
                                             where b.SupervisionclIdSupervisioncl == id
                                             orderby b.Fecha descending
                                             select new BitacoraclViewModal
                                             {
                                                 bitacoraVM = b,
                                                 beneficiosVM = fleft
                                             };

            ViewBag.IdSupervisionGuardar = id;
            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;
            ViewBag.supervisor = supervisor;
            ViewBag.idcp = idcp;

            return View();
        }
        #region -Create Bitacora-
        public IActionResult CreateBitacora(string nombre, string cp, int id, string supervisor, int idcp, int idpersona, int idfraccionesimpuestas)
        {
            int index = cp.IndexOf("?");
            if (index >= 0)
                cp = cp.Substring(0, index);

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;
            ViewBag.idcp = idcp;
            ViewBag.supervisor = supervisor;
            ViewBag.BeneficiosclIdBeneficioscl = idfraccionesimpuestas;

            ViewBag.listaBitacoras = listaBitacoras;

            var Beneficios = (from s in _context.Supervisioncl
                              join b in _context.Beneficios on s.IdSupervisioncl equals b.SupervisionclIdSupervisioncl
                              where b.SupervisionclIdSupervisioncl == id
                              select new BitacoraclViewModal
                              {
                                  beneficiosVM = b
                              });


            ViewBag.countFrac = null;

            if (idfraccionesimpuestas == 0)
            {
                ViewBag.countFrac = Beneficios.Count();
                ViewData["BeneficiosBitaccora"] = Beneficios;
            }

            ViewBag.IdSupervisionGuardar = id;

            return View();
        }
        public IActionResult CreateBitacora2(string nombre, string cp, int id, int SupervisionclIdSupervisioncl, string idpersona, string supervisor, int idcp)
        {
            int index = cp.IndexOf("?");
            if (index >= 0)
                cp = cp.Substring(0, index);

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;
            ViewBag.supervisor = supervisor;
            ViewBag.idcp = idcp;

            ViewBag.BeneficiosclIdBeneficioscl = id;
            ViewBag.IdSupervisionGuardar = SupervisionclIdSupervisioncl;

            #region -Select idOficialia

            List<Bitacoracl> bitacorasvm = _context.Bitacoracl.ToList();

            var leftjoin = from o in _context.Oficialia
                           join p in _context.Personacl on o.UsuarioTurnar equals p.Supervisor
                           join s in _context.Supervisioncl on p.IdPersonaCl equals s.PersonaclIdPersonacl
                           join b in bitacorasvm on o.IdOficialia equals b.OficialiaIdOficialia into temp
                           from bo in temp.DefaultIfEmpty()
                           select new ListaOficialiaBitacoraViewModelCL
                           {
                               OficialiaVM = o,
                               SupervisionClVM = s,
                               PersonaClVM = p,
                               BitacoraClVM = bo
                           };
            var wheres = (from bn in leftjoin
                          where bn.OficialiaVM.UsuarioTurnar == supervisor && bn.BitacoraClVM == null
                          group bn by bn.OficialiaVM.IdOficialia into grp
                          select grp.OrderBy(bn => bn.OficialiaVM.IdOficialia).FirstOrDefault()).ToList();

            var select = (from wh in wheres
                          select wh.OficialiaVM.IdOficialia).ToList();

            ViewBag.expoficialia = select;
            #endregion


            return View();
        }


        #region -siNumero-
        public static int siNumero(string numero)
        {
            int id = 0;

            try
            {
                id = int.Parse(numero);
            }
            catch (Exception e)
            {

            }

            return id;
        }
        #endregion


        public async Task<IActionResult> AgregarBitacora(Bitacoracl bitacoracl, string IdBitacoracl, DateTime Fecha, string tipoPersona,
        string tipoVisita, string Texto, string SupervisionclIdSupervisioncl, string BeneficiosclIdBeneficioscl, IList<IFormFile> files, string nombre, string cp, string idpersona, string idOficialia, string supervisor, string idcp, string[] datosidBeneficio)
        {

            int idbitacora = _context.Bitacoracl.Max(p => p.IdBitacoracl) + 1;

            string currentUser = User.Identity.Name;
            string file_name = null;

            if (files != null && files.Count > 0)
            {
                var uploadTasks = new List<Task>();

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        string fileExtension = Path.GetExtension(formFile.FileName);
                        file_name = $"{idbitacora}_{SupervisionclIdSupervisioncl}_{idpersona}{fileExtension}";

                        bitacoracl.RutaEvidencia = file_name;
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "EvidenciaCL");
                        var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);

                        uploadTasks.Add(formFile.CopyToAsync(stream));
                    }
                    break;
                }

                await Task.WhenAll(uploadTasks);
            }
            if (ModelState.ErrorCount <= 1)
            {

                for (int i = 0; i < datosidBeneficio.Length; i++)
                {

                    bitacoracl.Fecha = Fecha;
                    bitacoracl.TipoPersona = mg.normaliza(tipoPersona);
                    bitacoracl.TipoVisita = mg.normaliza(tipoVisita);
                    bitacoracl.Texto = mg.normaliza(Texto);
                    bitacoracl.OficialiaIdOficialia = idOficialia != null ? siNumero(idOficialia) : 0;
                    bitacoracl.FechaRegistro = DateTime.Now;
                    bitacoracl.BeneficiosclIdBeneficioscl = Int32.Parse(datosidBeneficio[i]);

                    var supervision = _context.Supervisioncl
                    .SingleOrDefault(m => m.IdSupervisioncl == bitacoracl.SupervisionclIdSupervisioncl);

                    _context.Add(bitacoracl);
                    _context.SaveChanges();

                }

                if (datosidBeneficio.Length == 0)
                {
                    if (BeneficiosclIdBeneficioscl != null)
                    {
                        bitacoracl.BeneficiosclIdBeneficioscl = Int32.Parse(BeneficiosclIdBeneficioscl);
                    }
                    bitacoracl.Fecha = Fecha;
                    bitacoracl.TipoPersona = mg.normaliza(tipoPersona);
                    bitacoracl.TipoVisita = mg.normaliza(tipoVisita);
                    bitacoracl.Texto = mg.normaliza(Texto);
                    bitacoracl.OficialiaIdOficialia = idOficialia != null ? siNumero(idOficialia) : 0;
                    bitacoracl.FechaRegistro = DateTime.Now;

                    var supervision = _context.Supervisioncl
                   .SingleOrDefault(m => m.IdSupervisioncl == bitacoracl.SupervisionclIdSupervisioncl);

                    _context.Add(bitacoracl);
                    await _context.SaveChangesAsync();

                    bitacoracl = await _context.Bitacoracl.OrderByDescending(b => b.IdBitacoracl).FirstOrDefaultAsync();



                }

                if (bitacoracl.BeneficiosclIdBeneficioscl != null)
                {
                    return RedirectToAction("EditBeneficios/" + bitacoracl.SupervisionclIdSupervisioncl, "Supervisioncl", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @supervisor = supervisor, @idcp = idcp });
                }
                else
                {
                    return RedirectToAction("ListaBitacora/" + bitacoracl.SupervisionclIdSupervisioncl, "Supervisioncl", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @supervisor = supervisor, @idcp = idcp });
                }

            }
            return View(bitacoracl);
        }


        #endregion
        public async Task<IActionResult> EditBitacora(int? id, string nombre, string cp, int idpersona, string supervisor, int idcp, int idSupervisioncl, int IdBeneficios)
        {
            //int index = cp.IndexOf("?");
            //if (index >= 0)
            //    cp = cp.Substring(0, index);

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;
            ViewBag.supervisor = supervisor;
            ViewBag.idcp = idcp;
            ViewBag.idSupervisioncl = idSupervisioncl;
            ViewBag.IdBeneficios = IdBeneficios;

            if (id == null)
            {
                return NotFound();
            }
            //PENDIENTE PARA QUE SALGAN LOS BENEFICISO EN LA EDICION DE BITACORAS 
            var bitacora = await _context.Bitacoracl.SingleOrDefaultAsync(m => m.IdBitacoracl == id);

            var Beneficios = (from s in _context.Supervisioncl
                              join b in _context.Beneficios on s.IdSupervisioncl equals b.SupervisionclIdSupervisioncl
                              where b.SupervisionclIdSupervisioncl == idSupervisioncl
                              select new BitacoraclViewModal
                              {
                                  beneficiosVM = b
                              });


            ViewBag.countFrac = null;

            if (idSupervisioncl == 0)
            {
                ViewBag.countFrac = Beneficios.Count();
                ViewData["BeneficiosBitaccora"] = Beneficios;
            }
            if (bitacora == null)
            {
                return NotFound();
            }

            #region ListaTipoPersona
            ViewBag.TipoPersona = listaPersona;
            ViewBag.idTipoPersona = mg.BuscaId(listaPersona, bitacora.TipoPersona);
            #endregion

            #region ListaTipoAccion
            ViewBag.TipoAccion = listaBitacoras;
            ViewBag.idTipoAccion = mg.BuscaId(listaBitacoras, bitacora.TipoVisita);
            #endregion

            ViewBag.RutaEvidencia = bitacora.RutaEvidencia;

            return View(bitacora);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBitacora([Bind("IdBitacoracl,Fecha,TipoPersona,Texto,TipoVisita,RutaEvidencia,OficialiaIdOficialia,FechaRegistro,BeneficiosclIdBeneficioscl,SupervisionclIdSupervisioncl")] Bitacoracl bitacoracl, IFormFile evidencia, string nombre, string cp, string idpersona, string supervisor, string idcp)
        {
            bitacoracl.Texto = mg.normaliza(bitacoracl.Texto);
            bitacoracl.OficialiaIdOficialia = bitacoracl.OficialiaIdOficialia;
            bitacoracl.BeneficiosclIdBeneficioscl = bitacoracl.BeneficiosclIdBeneficioscl;

            var supervision = _context.Supervision
               .SingleOrDefault(m => m.IdSupervision == bitacoracl.SupervisionclIdSupervisioncl);

            if (ModelState.IsValid)
            {
                try
                {
                    var oldBitacora = await _context.Bitacoracl.FindAsync(bitacoracl.IdBitacoracl, bitacoracl.SupervisionclIdSupervisioncl);

                    if (evidencia == null)
                    {
                        bitacoracl.RutaEvidencia = oldBitacora.RutaEvidencia;
                    }
                    else
                    {
                        string file_name = bitacoracl.IdBitacoracl + "_" + bitacoracl.SupervisionclIdSupervisioncl + "_" + supervision.PersonaIdPersona + Path.GetExtension(evidencia.FileName);
                        bitacoracl.RutaEvidencia = file_name;
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "EvidenciaCL");

                        if (System.IO.File.Exists(Path.Combine(uploads, file_name)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, file_name));
                        }

                        var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                        await evidencia.CopyToAsync(stream);
                        stream.Close();
                    }

                    _context.Entry(oldBitacora).CurrentValues.SetValues(bitacoracl);

                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(bitacora);
                    //await evidencia.CopyToAsync(stream);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BitacoraExists(bitacoracl.SupervisionclIdSupervisioncl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ListaBitacora/" + bitacoracl.SupervisionclIdSupervisioncl, "Supervisioncl", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @idcp = idcp, @supervisor = supervisor });

            }
            return View(bitacoracl);
        }
        public async Task<IActionResult> DeleteRegistro(int? id, string nombre, string cp, string idpersona, string idcp, string supervisor)
        {
            var Bitacora = await _context.Bitacoracl.SingleOrDefaultAsync(m => m.IdBitacoracl == id);
            var oldBitacora = await _context.Bitacoracl.FindAsync(Bitacora.IdBitacoracl, Bitacora.SupervisionclIdSupervisioncl);
            _context.Entry(oldBitacora).CurrentValues.SetValues(Bitacora);

            _context.Bitacoracl.Remove(Bitacora);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListaBitacora/" + Bitacora.SupervisionclIdSupervisioncl, "Supervisioncl", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @idcp = idcp, @supervisor = supervisor });
        }
        #endregion

        #region -Victima-
        public async Task<IActionResult> ListaVictima(int? id, string cp, string nombre, string idpersona)
        {
            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;

            var supervision = _context.Supervisioncl
            .SingleOrDefault(m => m.IdSupervisioncl == id);

            var persona = _context.Personacl
           .SingleOrDefault(m => m.IdPersonaCl == supervision.PersonaclIdPersonacl);
            var cpp = _context.Causapenalcl
           .SingleOrDefault(m => m.IdCausaPenalcl == supervision.CausaPenalclIdCausaPenalcl);



            await PermisosEdicion(id);


            List<Victimacl> victimas = _context.Victimacl.ToList();

            ViewData["Victima"] = from table in victimas
                                  where table.SupervisionclIdSupervisioncl == id
                                  orderby table.IdVictimacl
                                  select table;

            ViewBag.IdSupervisionGuardar = id;


            return View();
        }
        public IActionResult CreateVictima(int? id, string nombre, string cp, string idpersona)
        {

            int index = cp.IndexOf("?");
            if (index >= 0)
                cp = cp.Substring(0, index);


            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;

            ViewBag.IdSupervisionGuardar = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVictima(Victimacl victimacl, string IdVictima, string NombreV, string Edad, string Telefono, string ConoceDetenido, string TipoRelacion,
            string TiempoConocerlo, string ViveSupervisado, string Direccion, string Victimacol, string Observaciones, string SupervisionIdSupervision, string nombre, string cp, string idpersona)
        {

            int index = cp.IndexOf("?");
            if (index >= 0)
                cp = cp.Substring(0, index);


            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;

            string currentUser = User.Identity.Name;
            if (ModelState.ErrorCount <= 1)
            {
                victimacl.NombreV = mg.normaliza(NombreV);
                victimacl.Edad = Edad;
                victimacl.Telefono = Telefono;
                victimacl.ConoceDetenido = mg.normaliza(ConoceDetenido);
                victimacl.TipoRelacion = mg.normaliza(TipoRelacion);
                victimacl.TiempoConocerlo = mg.normaliza(TiempoConocerlo);
                victimacl.ViveSupervisado = mg.normaliza(ViveSupervisado);
                victimacl.Direccion = mg.normaliza(Direccion);
                victimacl.Victimacol = mg.normaliza(Victimacol);
                victimacl.Observaciones = mg.normaliza(Observaciones);


                var supervision = _context.Supervisioncl
               .SingleOrDefault(m => m.IdSupervisioncl == victimacl.SupervisionclIdSupervisioncl);


                var persona = _context.Personacl
               .SingleOrDefault(m => m.IdPersonaCl == supervision.PersonaclIdPersonacl);
                var cpp = _context.Causapenalcl
               .SingleOrDefault(m => m.IdCausaPenalcl == supervision.CausaPenalclIdCausaPenalcl);

                //ViewBag.Npersona = persona.NombreCompleto;
                //ViewBag.cp = cpp.CausaPenal;

                int idVictima = ((from table in _context.Victimacl
                                  select table.IdVictimacl).Max()) + 1;

                victimacl.IdVictimacl = idVictima;
                _context.Add(victimacl);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListaVictima/" + victimacl.SupervisionclIdSupervisioncl, "Supervisioncl", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona });
                //return RedirectToAction("ListaVictima/" + victima.SupervisionIdSupervision, "Supervisiones");
            }
            return View(victimacl);
        }

        public async Task<IActionResult> Editvictima(int? id, string nombre, string cp, string idpersona)
        {
            int index = cp.IndexOf("?");
            if (index >= 0)
                cp = cp.Substring(0, index);


            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;

            if (id == null)
            {
                return NotFound();
            }

            var Victima = await _context.Victimacl.SingleOrDefaultAsync(m => m.IdVictimacl == id);
            if (Victima == null)
            {
                return NotFound();
            }

            List<SelectListItem> ListaConoceDetenido;
            ListaConoceDetenido = new List<SelectListItem>
            {
              new SelectListItem{ Text="NA", Value="NA"},
              new SelectListItem{ Text="Si", Value="SI"},
              new SelectListItem{ Text="No", Value="NO"},
            };
            ViewBag.ConoceDetenido = ListaConoceDetenido;
            ViewBag.idConoceDetenido = mg.BuscaId(ListaConoceDetenido, Victima.ConoceDetenido);


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
            ViewBag.TipoRelacion = ListaRelacion;
            ViewBag.idTipoRelacion = mg.BuscaId(ListaRelacion, Victima.TipoRelacion);

            List<SelectListItem> ListaTiempo;
            ListaTiempo = new List<SelectListItem>
            {
              new SelectListItem{ Text="Menos de un año", Value="MENOS DE 1 AÑO"},
              new SelectListItem{ Text="Entre 1 y 2 años", Value="ENTRE 1 Y 2 AÑOS"},
              new SelectListItem{ Text="Entre 2 y 5 años(a)", Value="ENTRE 2 Y 5 AÑOS"},
              new SelectListItem{ Text="Más de 5 años", Value="MÁS DE 5 AÑOS"},
              new SelectListItem{ Text="Toda la vida", Value="TODA LA VIDA"},
            };
            ViewBag.TiempoConocerlo = ListaTiempo;
            ViewBag.idTiempoConocerlo = mg.BuscaId(ListaTiempo, Victima.TiempoConocerlo);

            ViewBag.ViveSupervisado = listaSiNoNa;
            ViewBag.idViveSupervisado = mg.BuscaId(listaSiNoNa, Victima.ViveSupervisado);


            return View(Victima);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editvictima(int id, [Bind("IdVictimacl,NombreV,Edad,Telefono,ConoceDetenido,TipoRelacion,TiempoConocerlo,ViveSupervisado,Direccion,Victimacol,SupervisionclIdSupervisioncl, Observaciones")] Victimacl victimacl, string nombre, string cp, string idpersona)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var oldVictima = await _context.Victimacl.FindAsync(victimacl.IdVictimacl, victimacl.SupervisionclIdSupervisioncl);
                    _context.Entry(oldVictima).CurrentValues.SetValues(victimacl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VictimaExists(victimacl.IdVictimacl))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ListaVictima/" + victimacl.SupervisionclIdSupervisioncl, "Supervisioncl", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona });
            }
            return View();
        }
        public async Task<IActionResult> DeleteVictima(int? id, string nombre, string cp, string idpersona)
        {
            var Victima = await _context.Victimacl.SingleOrDefaultAsync(m => m.IdVictimacl == id);
            _context.Victimacl.Remove(Victima);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListaVictima/" + Victima.SupervisionclIdSupervisioncl, "Supervisioncl", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona });

        }
        public async Task<IActionResult> VerVictima(int? id, string nombre, string cp, string idpersona)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Victima = await _context.Victimacl
                .SingleOrDefaultAsync(m => m.IdVictimacl == id);
            if (Victima == null)
            {
                return NotFound();
            }
            return View(Victima);
        }
        #endregion

        #endregion -Edits-fViewBag.usuario

        #region -Actualizar Candado-
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
                          where p.IdPersonaCl == personacl.IdPersonaCl
                          select p.Candado).FirstOrDefault();
            //return View();

            return Json(new { success = true, responseText = Convert.ToString(stadoc), idPersonas = Convert.ToString(personacl.IdPersonaCl) });
        }
        public JsonResult getEstadodeCanadado(int id)
        {
            //IEnumerable<Persona> shops = _context.Persona;
            //return Json(shops.Select(u => new { u.Candado, u.IdPersona }).Where(u => u.IdPersona == id));

            var stadoc = (from p in _context.Personacl
                          where p.IdPersonaCl == id
                          select p.Candado);

            return Json(stadoc);
        }
        #endregion

        #region -Delete-
        public JsonResult antesdelete(Supervisioncl supervisioncl, Beneficios beneficios, string[] datosuper)
        {
            var borrar = false;
            var id = Int32.Parse(datosuper[0]);

            var antesdel = from s in _context.Supervisioncl
                           join b in _context.Bitacoracl on s.IdSupervisioncl equals b.SupervisionclIdSupervisioncl
                           where s.IdSupervisioncl == id
                           select s;


            var antesdel2 = from s in _context.Supervisioncl
                            join b in _context.Beneficios on s.IdSupervisioncl equals b.SupervisionclIdSupervisioncl
                            where s.IdSupervisioncl == id
                            select s;

            if (antesdel.Any() || antesdel2.Any())
            {
                return Json(new { success = true, responseText = Url.Action("Index", "Supervisioncl"), borrar = borrar });
            }
            else
            {
                borrar = true;
                return Json(new { success = true, responseText = Url.Action("Index", "Supervisioncl"), borrar = borrar });
            }
        }
        public JsonResult deletesuper(Supervisioncl supervisioncl, Historialeliminacion historialeliminacion, string[] datosuper)
        {
            var borrar = false;
            var id = Int32.Parse(datosuper[0]);
            var razon = mg.normaliza(datosuper[1]);
            var user = mg.normaliza(datosuper[2]);

            var query = (from s in _context.Supervisioncl
                         join p in _context.Personacl on s.PersonaclIdPersonacl equals p.IdPersonaCl
                         where s.IdSupervisioncl == id
                         select s).FirstOrDefault();

            var queryP = (from s in _context.Supervisioncl
                          join p in _context.Personacl on s.PersonaclIdPersonacl equals p.IdPersonaCl
                          where s.IdSupervisioncl == id
                          select p).FirstOrDefault();

            try
            {
                borrar = true;
                historialeliminacion.Id = id;
                historialeliminacion.Descripcion = "IDPERSONA= " + query.PersonaclIdPersonacl + " IDCAUSAPENAL= " + query.CausaPenalclIdCausaPenalcl + " IDSUPERVISIÓN= " + query.IdSupervisioncl;
                historialeliminacion.Tipo = "SUPERVISIÓN";
                historialeliminacion.Razon = mg.normaliza(razon);
                historialeliminacion.Usuario = mg.normaliza(user);
                historialeliminacion.Fecha = DateTime.Now;
                historialeliminacion.Supervisor = mg.normaliza(queryP.Supervisor);
                _context.Add(historialeliminacion);
                _context.SaveChanges();

                _context.Database.ExecuteSqlCommand("CALL spBorrarSupervisioncl(" + id + ")");
                return Json(new { success = true, responseText = Url.Action("index", "Supervisioncl"), borrar = borrar });

            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("index", "Supervisioncl"), borrar = borrar });
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervisioncl
                .SingleOrDefaultAsync(m => m.IdSupervisioncl == id);
            if (supervision == null)
            {
                return NotFound();
            }

            return View(supervision);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supervision = await _context.Supervisioncl.SingleOrDefaultAsync(m => m.IdSupervisioncl == id);
            _context.Supervisioncl.Remove(supervision);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region -Obtener Nombre Supervisor-
        Dictionary<string, string> diccionarioSupervisores = new Dictionary<string, string>
            {
                {"victor.morales@dgepms.com", "VICTOR MANUEL MORALES PEREZ"},
                {"juanita.rojas@dgepms.com", "JUANITA ROJAS GARCIA"},
                {"carmen.gonzalez@dgepms.com", "OF. MARIA DEL CARMEN GONZALEZ"},
                {"ana.quinonez@dgepms.com", "LIC. ANA MARIA QUIÑONEZ"},
                {"teresita.medina@dgepms.com", "TERESITA MEDINA"},
                {"david.nevarez@dgepms.com", "DAVID IVAN NEVAREZ"},
                {"amor.davalos@dgepms.com", "LIC. AMOR DAVALOS NAJERA"},
                {"harled.ledesma@dgepms.com", "LIC. HARLE LEDESMA"},
                {"carmen.trujillo@dgepms.com", "LIC. CARMEN TRUJILLO"},
                {"andrea.valdez@dgepms.com", "LIC. ANDREA VALDEZ"}

            };
        #endregion

        #region -Imprimir Reporte Supervision-
        public void imprimirReporteSupervision(string[] datosidBeneficio)
        {
            #region -Definir variables-
            #region -F1-
            string No1 = string.Empty;
            string TextoFraccion1 = string.Empty;
            string Estatus1 = string.Empty;
            string Actividades1 = string.Empty;
            #endregion 
            #region -F2-
            string No2 = string.Empty;
            string TextoFraccion2 = string.Empty;
            string Estatus2 = string.Empty;
            string Actividades2 = string.Empty;
            #endregion 
            #region -F3-
            string No3 = string.Empty;
            string TextoFraccion3 = string.Empty;
            string Estatus3 = string.Empty;
            string Actividades3 = string.Empty;
            #endregion 
            #region -F4-
            string No4 = string.Empty;
            string TextoFraccion4 = string.Empty;
            string Estatus4 = string.Empty;
            string Actividades4 = string.Empty;
            #endregion 
            #region -F5-
            string No5 = string.Empty;
            string TextoFraccion5 = string.Empty;
            string Estatus5 = string.Empty;
            string Actividades5 = string.Empty;
            #endregion 
            #region -F6-
            string No6 = string.Empty;
            string TextoFraccion6 = string.Empty;
            string Estatus6 = string.Empty;
            string Actividades6 = string.Empty;
            #endregion 
            #endregion

            #region -Consultas y llenado de variables temporales-
            int idSupervision = (from table in _context.Beneficios
                                 where table.IdBeneficios == (Convert.ToInt32(datosidBeneficio[datosidBeneficio.Length - 1]))
                                 select table.SupervisionclIdSupervisioncl).FirstOrDefault(); //Obtener IdSupervision

            var tipo = from table in _context.Beneficios
                       where table.IdBeneficios == (Convert.ToInt32(datosidBeneficio[datosidBeneficio.Length - 1]))
                       select new
                       {
                           FechaImposicion = table.FechaInicio,
                           FiguraJudicial = table.FiguraJudicial
                       };



            int idCP = (from table in _context.Supervisioncl
                        where table.IdSupervisioncl == idSupervision
                        select table.CausaPenalclIdCausaPenalcl).FirstOrDefault();

            int idPersona = (from table in _context.Supervisioncl
                             where table.IdSupervisioncl == idSupervision
                             select table.PersonaclIdPersonacl).FirstOrDefault();

            var persona = from table in _context.Personacl
                          where table.IdPersonaCl == idPersona
                          select new
                          {
                              Paterno = table.Paterno,
                              Materno = table.Materno,
                              Nombre = table.Nombre,
                              Supervisor = table.Supervisor,
                              ce = table.Ce,
                              juzgado = table.Juzgado
                          };

            var causapenal = from table in _context.Causapenalcl
                             where table.IdCausaPenalcl == idCP
                             select new
                             {
                                 CausaPenal = table.CausaPenal,
                                 Juez = table.Juez,
                                 Distrito = table.Distrito
                             };

            var delitos = from table in _context.Delitocl
                          where table.CausaPenalclIdCausaPenalcl == idCP
                          select new
                          {
                              Delito = table.Tipo
                          };

            var presentacion = from registro in _context.Registrohuellacl
                               join p in _context.Presentacionperiodicacl on registro.IdregistroHuellacl equals p.IdregistroHuellacl
                               where registro.PersonaclIdPersonacl == idPersona
                               select new
                               {
                                   fechaFirma = p.FechaFirma
                               };


            string inicio = "";

            try
            {
                inicio = ((from table in _context.Supervisioncl
                           where table.IdSupervisioncl == idSupervision
                           select table.Inicio).FirstOrDefault()).Value.ToString("dd MMMM yyyy");
            }
            catch (System.InvalidOperationException e)
            {
                inicio = "xxxxxxxxxxxxxxxx-Sin fecha de inicio en Supervisión-xxxxxxxxxxxxxxxxxx";
            }


            string final = "";

            try
            {
                final = ((from table in _context.Supervisioncl
                          where table.IdSupervisioncl == idSupervision
                          select table.Termino).FirstOrDefault()).Value.ToString("dd MMMM yyyy");
            }
            catch (System.InvalidOperationException e)
            {
                final = "-Sin fecha de termino en Supervisión-";
            }

            string cp = "";
            string juzgado = "";
            string ce = "";
            string juez = "";
            string fechaImposicion = "";
            string figuraJudicial = "";
            string fechaFinal = "";
            string nombre = "";
            string delito = "";
            string supervisor = "";
            string distrito = "";
            string presentaciones = "";
            string tipoInforme = "C";
            string fechaAudienciars = "C";
            string varfj = "";



            Dictionary<string, string> diccionarioJuzgados = new Dictionary<string, string>
            {
                { "NA", "NA" },
                { "JUZGADO 1", "J1" },
                { "JUZGADO 2", "J2" },
                { "JUZGADO 3", "J3" }
            };

            foreach (var p in persona)
            {
                nombre = p.Paterno + " " + p.Materno + " " + p.Nombre;
                if (diccionarioSupervisores.ContainsKey(p.Supervisor))
                {
                    supervisor = diccionarioSupervisores[p.Supervisor];
                }
                else
                {
                    // Si no se encuentra en el diccionario, asignar el valor original
                    supervisor = p.Supervisor.ToString();
                }
                ce = p.ce;
                if (diccionarioJuzgados.ContainsKey(p.juzgado))
                {
                    juzgado = diccionarioJuzgados[p.juzgado];
                }
                else
                {
                    // Si no se encuentra en el diccionario, asignar el valor original
                    juzgado = p.juzgado.ToString();
                }


            }

            foreach (var c in causapenal)
            {
                cp = c.CausaPenal;
                juez = c.Juez;
                distrito = c.Distrito;
            }

            foreach (var d in delitos)
            {
                delito += d.Delito + " ";
            }

            foreach (var t in tipo)
            {
                fechaImposicion = t.FechaImposicion.Value.ToString("dd MMMM yyyy");
                figuraJudicial = t.FiguraJudicial;
            }

            foreach (var p in presentacion)
            {
                presentaciones += p.fechaFirma.Value.ToString("dd MMMM yyyy") + " \n";
            }

            #region -string tipo de Fracciones

            Dictionary<string, string> reglas = new Dictionary<string, string>
            {
                { "I", "OBSERVAR BUENA CONDUCTA DURANTE EL TÉRMINO DE SUSPENSIÓN" },
                { "II", "PRESENTARSE MENSUALMENTE ANTE LAS AUTORIDADES ENCARGADAS DE LA EJECUCIÓN DE PENAS Y MEDIDAS DE SEGURIDAD, LAS QUE LE OTORGARÁN EL SALVOCONDUCTO RESPECTIVO." },
                { "III", "QUEDAR SUJETO A LA VIGILANCIA DE LA AUTORIDAD" },
                { "IV", "PRESENTARSE ANTE LAS AUTORIDADES JUDICIALES O ANTES LAS ENCARGADAS DE LA EJECUCIÓN DE PENAS Y MEDIDAS DE SEGURIDAD CUANTAS VECES SEA REQUERIDO PARA ELLO." },
                { "V", "COMUNICAR A LAS AUTORIDADES DEL ÓRGANO EJECUTOR DE PENAS SUS CAMBIOS DE DOMICILIO." },
                { "VI", "NO AUSENTARSE DEL ESTADO SIN PREVIO PERMISO DE LAS AUTORIDADES ENCARGADAS DE LA EJECUCIÓN DE PENAS Y MEDIDAS DE SEGURIDAD." }
            };
 
           string ObtenerValor(Dictionary<string, string> diccionario, string clave)
            {
                if (diccionario.TryGetValue(clave, out string valor))
                {
                    return valor; // Devolver el valor del diccionario si la clave está presente
                }
                else
                {
                    return clave; // Devolver la clave (valor de entrada) si la clave no está presente
                }
            }
            #endregion

            #endregion

            #region -Define contenido de variables-
            string actividadesfor = string.Empty;
            List<object> fracciones = new List<object>();
            bool presentacionesbool = false;

            for (int i = 0; i < datosidBeneficio.Length; i++)
            {
                string estatusF = (from table in _context.Beneficios
                                   where table.IdBeneficios == (Convert.ToInt32(datosidBeneficio[i]))
                                   select table.Estado).FirstOrDefault();

                string tipoF = (from table in _context.Beneficios
                                   where table.IdBeneficios == (Convert.ToInt32(datosidBeneficio[i]))
                                   select table.FiguraJudicial).FirstOrDefault();

                string tipoB = (from table in _context.Beneficios
                                   where table.IdBeneficios == (Convert.ToInt32(datosidBeneficio[i]))
                                   select table.Tipo).FirstOrDefault();

                var actividades = from ben in _context.Beneficios
                                  join bitacora in _context.Bitacoracl on ben.IdBeneficios equals bitacora.BeneficiosclIdBeneficioscl
                                  where ben.IdBeneficios == (Convert.ToInt32(datosidBeneficio[i]))
                                  select new
                                  {
                                      actividades = bitacora.Texto,
                                      fecha = bitacora.Fecha
                                  };

                if (tipoF != "SUSPENSION CONDICIONAL CONDENA")
                {
                    if (tipoB == "PRESENTACIÓN PERIÓDICA")
                    {
                        if (presentaciones != "")
                        {
                            actividadesfor = "CON FECHA " + inicio + " COMPARECE EL SUPERVISADO(A) ANTE LAS INSTALACIONES DE LA DIRECCIÓN GENERAL DE " +
                            "EJECUCIÓN DE PENAS, MEDIDAS DE SEGURIDAD, SUPERVISIÓN DE MEDIDAS CAUTELARES Y DE LA SUSPENSIÓN CONDICIONAL DEL " +
                            "PROCESO AL CUAL SE LE NOTIFICAN SUS OBLIGACIONES PROCESALES, ASÍ MISMO SE TIENE REGISTRO DE LAS SIGUIENTES PRESENTACIONES PERIÓDICAS \n" +
                            presentaciones;
                        }
                        else
                        {
                            foreach (var a in actividades)
                            {
                                actividadesfor += "CON FECHA " + a.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + a.actividades + " \n";
                            }
                        }
                    }
                    else
                    {
                        foreach (var a in actividades)
                        {
                            actividadesfor += "CON FECHA " + a.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + a.actividades + " \n";
                        }
                    }
                }else{
                    switch (tipoB)
                    {
                        case "I":
                            No1 = "I";
                            TextoFraccion1 = ObtenerValor(reglas, tipoB);
                            Estatus1 = estatusF;
                            foreach (var act in actividades)
                            {
                                Actividades1 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                            }
                            break;
                        case "II":
                            No2 = "II";
                            TextoFraccion2 = ObtenerValor(reglas, tipoB);
                            Estatus2 = estatusF;
                            if (presentaciones != "")
                            {
                                Actividades2 = "CON FECHA " + inicio + " COMPARECE EL SUPERVISADO(A) ANTE LAS INSTALACIONES DE LA DIRECCIÓN GENERAL DE " +
                                "EJECUCIÓN DE PENAS, MEDIDAS DE SEGURIDAD, SUPERVISIÓN DE MEDIDAS CAUTELARES Y DE LA SUSPENSIÓN CONDICIONAL DEL " +
                                "PROCESO AL CUAL SE LE NOTIFICAN SUS OBLIGACIONES PROCESALES, ASÍ MISMO SE TIENE REGISTRO DE LAS SIGUIENTES PRESENTACIONES PERIÓDICAS \n" +
                                presentaciones;
                            }
                            else
                            {
                                foreach (var a in actividades)
                                {
                                    Actividades2 += "CON FECHA " + a.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + a.actividades + " \n";
                                }
                            }
                            break;
                        case "III":
                            No3 = "III";
                            TextoFraccion3 = ObtenerValor(reglas, tipoB);
                            Estatus3 = estatusF;
                            foreach (var act in actividades)
                            {
                                Actividades3 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                            }
                            break;
                        case "IV":
                            No4 = "IV";
                            TextoFraccion4 =ObtenerValor(reglas, tipoB);
                            Estatus4 = estatusF;
                            foreach (var act in actividades)
                            {
                                Actividades4 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                            }
                            break;
                        case "V":
                            No5 = "V";
                            TextoFraccion5 = ObtenerValor(reglas, tipoB);
                            Estatus5 = estatusF;
                            foreach (var act in actividades)
                            {
                                Actividades5 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                            }
                            break;
                        case "VI":
                            No6 = "VI";
                            TextoFraccion6 = ObtenerValor(reglas, tipoB);
                            Estatus6 = estatusF;
                            foreach (var act in actividades)
                            {
                                Actividades6 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                            }
                            break;
                    }
                }
                if(tipoF != "SUSPENSION CONDICIONAL CONDENA")
                {
                    var nuevaFraccion = new
                    {
                        No = 1 + i,
                        TextoFraccion = ObtenerValor(reglas, tipoB),
                        Estatus = estatusF,
                        Actividades = actividadesfor
                    };
                    actividadesfor = "";
                    fracciones.Add(nuevaFraccion);
                }
                
            }
            #endregion

            string templatePath = this._hostingEnvironment.WebRootPath + "\\Documentos\\templateCL.docx";
            string resultPath = this._hostingEnvironment.WebRootPath + "\\Documentos\\reporteSupervision.docx";


            DocumentCore dc = DocumentCore.Load(templatePath);

            if (figuraJudicial != "SUSPENSION CONDICIONAL CONDENA")
            {
                var dataSource = new
                {
                    Fecha = DateTime.Now.ToString("dd MMMM yyyy").ToUpper(),
                    fechaFinal = final,
                    //fechaAudienciars = fechaAudiencia,
                    CP = cp,
                    CE = ce,
                    Js = juzgado,
                    idPer = idPersona,
                    Juez = juez,
                    FechaImposicion = fechaImposicion,
                    FiguraJudicial = figuraJudicial,
                    Nombre = nombre,
                    Delito = delito,
                    Supervisor = supervisor,
                    Distrito = distrito,
                    Fraccion = fracciones.ToArray(),
                };
                dc.MailMerge.ClearOptions = MailMergeClearOptions.RemoveEmptyRanges;
                dc.MailMerge.Execute(dataSource);
               
            }
            else
            {
                var dataSource = new
                {
                    Fecha = DateTime.Now.ToString("dd MMMM yyyy").ToUpper(),
                    fechaFinal = final,
                    //fechaAudienciars = fechaAudiencia,
                    CP = cp,
                    CE = ce,
                    Js = juzgado,
                    idPer = idPersona,
                    Juez = juez,
                    FechaImposicion = fechaImposicion,
                    FiguraJudicial = figuraJudicial,
                    Nombre = nombre,
                    Delito = delito,
                    Supervisor = supervisor,
                    Distrito = distrito,
                    Fraccion = new object[]
                    {
                        new
                        {
                            No=No1,
                            TextoFraccion=TextoFraccion1,
                            Estatus=Estatus1,
                            Actividades=Actividades1
                        },
                        new
                        {
                            No=No2,
                            TextoFraccion=TextoFraccion2,
                            Estatus=Estatus2,
                            Actividades=Actividades2
                        },
                        new
                        {
                            No=No3,
                            TextoFraccion=TextoFraccion3,
                            Estatus=Estatus3,
                            Actividades=Actividades3
                        },
                        new
                        {
                            No=No4,
                            TextoFraccion=TextoFraccion4,
                            Estatus=Estatus4,
                            Actividades=Actividades4
                        },
                        new
                        {
                            No=No5,
                            TextoFraccion=TextoFraccion5,
                            Estatus=Estatus5,
                            Actividades=Actividades5
                        },
                        new
                        {
                            No=No6,
                            TextoFraccion=TextoFraccion6,
                            Estatus=Estatus6,
                            Actividades=Actividades6
                        },

                    },
                };
                dc.MailMerge.ClearOptions = MailMergeClearOptions.RemoveEmptyRanges;
                dc.MailMerge.Execute(dataSource);

            }
            dc.Save(resultPath);
            //Response.Redirect("https://localhost:44359/Documentos/reporteSupervision.docx");
        }
        #endregion
        #region -Vinculacion-

        public IActionResult Vinculacion(int idpersona)
        {
            ViewData["Oficios"] = (from p in _context.Personacl
                                   join s in _context.Supervisioncl on p.IdPersonaCl equals s.PersonaclIdPersonacl
                                   join r in _context.Reinsercion on p.IdPersonaCl equals Int32.Parse(r.IdTabla) into pr
                                   from reinsercion in pr.DefaultIfEmpty()
                                   join c in _context.Canalizacion on reinsercion.IdReinsercion equals c.ReincercionIdReincercion into rc
                                   from canalizacion in rc.DefaultIfEmpty()
                                   join o in _context.Oficioscanalizacion on canalizacion.IdCanalizacion equals o.CanalizacionIdCanalizacion
                                   where p.IdPersonaCl == idpersona && reinsercion.Tabla == "personacl"
                                   select new ReinsercionVM
                                   {
                                       personaclVM = p,
                                       SupervisionclVM = s,
                                       reinsercionVM = reinsercion,
                                       canalizacionVM = canalizacion,
                                       oficioscanalizacionVM = o
                                   }).ToList();

            //return Json(new
            //{
            //    success = true,

            //    query = ViewData["alertas"]
            //});
            return View();
        }
        #endregion




        #region -VERIFICAR EXISTE-
        private bool SupervisionclExists(int id)
        {
            return _context.Supervisioncl.Any(e => e.IdSupervisioncl == id);
        }
        private bool PlaneacionExists(int id)
        {
            return _context.Planeacionestrategicacl.Any(e => e.IdPlaneacionEstrategicacl == id);
        }
        private bool BeneficioExists(int id)
        {
            return _context.Planeacionestrategicacl.Any(e => e.IdPlaneacionEstrategicacl == id);
        }
        private bool CambiodeObligacionesclExists(int id)
        {
            return _context.Cambiodeobligacionescl.Any(e => e.IdCambiodeObligacionescl == id);
        }
        private bool CierredecasoclExists(int id)
        {
            return _context.Cierredecasocl.Any(e => e.IdCierreDeCasocl == id);
        }
        private bool VictimaExists(int id)
        {
            return _context.Victimacl.Any(e => e.IdVictimacl == id);
        }
        private bool BitacoraExists(int id)
        {
            return _context.Bitacoracl.Any(e => e.IdBitacoracl == id);
        }

        #endregion
    }
}
