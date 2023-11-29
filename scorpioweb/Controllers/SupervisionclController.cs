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

namespace scorpioweb.Controllers
{
    public class SupervisionclController : Controller
    {
        #region -Constructor-
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public SupervisionclController(penas2Context context, IHostingEnvironment hostingEnvironment, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
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
                if (rol == "AdminMCSCP" || rol == "AdminMCSCP")
                {
                    flagCoordinador = true;
                }
            }

            var query = from persona in _context.Persona
                        join supervision in _context.Supervision on persona.IdPersona equals supervision.PersonaIdPersona
                        where supervision.IdSupervision == id
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
        private readonly List<SelectListItem> listaEstadosSupervision = new List<SelectListItem>
        {
            new SelectListItem{ Text = "", Value = "" },
            new SelectListItem{ Text = "Concluido", Value = "CONCLUIDO" },
            new SelectListItem{ Text = "Vigente", Value = "VIGENTE" },
            new SelectListItem{ Text = "En espera de respuesta", Value = "EN ESPERA DE RESPUESTA" }
        };
        private readonly List<SelectListItem> listaBeneficios = new List<SelectListItem>
        {
            new SelectListItem{ Text = "", Value = "" },
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
        private List<SelectListItem> listaFracciones = new List<SelectListItem>
        {
            new SelectListItem{ Text="XIV", Value="XIV"}
        };

        private List<SelectListItem> listaCumplimiento = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Cumplimiento", Value = "CUMPLIMIENTO" },
            new SelectListItem{ Text = "Incumplimiento", Value = "INCUMPLIMIENTO" },
        };
        private List<SelectListItem> listaCierreCaso = new List<SelectListItem>
        {
            new SelectListItem{ Text = "NA", Value = "NA" },
            new SelectListItem{ Text = "Sobreseimiento por acuerdo reparatorio", Value = "SOBRESEIMIENTO POR ACUERDO REPARATORIO" },
            new SelectListItem{ Text = "Sobreseimiento por suspensión condicional del proceso", Value = "SOBRESEIMIENTO POR SUSPENSION CONDICIONAL DEL PROCESO" },
            new SelectListItem{ Text = "Sobreseimiento por perdón", Value = "SOBRESEIMIENTO POR PERDON" },
            new SelectListItem{ Text = "Sobreseimiento por muerte del imputado", Value = "SOBRESEIMIENTO POR MUERTE DEL IMPUTADO" },
            new SelectListItem{ Text = "Sobreseimiento por extinción de la acción penal", Value = "SOBRESEIMIENTO POR EXTINCIÓN DE LA ACCION PENAL" },
            new SelectListItem{ Text = "Sobreseimiento por prescripción", Value = "SOBRESEIMIENTO POR PRESCRIPCION" },
            new SelectListItem{ Text = "Criterio de oportunidad", Value = "CRITERIO DE OPORTUNIDAD" },
            new SelectListItem{ Text = "Sentencia condenatoria en procedimiento abreviado", Value = "SENTENCIA CONDENATORIA EN PROCEDIMIENTO ABREVIADO" },
            new SelectListItem{ Text = "Sentencia absolutoria en procedimiento abreviado", Value = "SENTENCIA ABSOLUTORIA EN PROCEDIMIENTO ABREVIADO" },
            new SelectListItem{ Text = "Sentencia condenatoria en juicio oral", Value = "SENTENCIA CONDENATORIA EN JUICIO ORAL" },
            new SelectListItem{ Text = "Sentencia absolutoria en juicio oral", Value = "SENTENCIA ABSOLUTORIA EN JUICIO ORAL" },
            new SelectListItem{ Text = "No vinculación a proceso", Value = "NO VINCULACIÓN A PROCESO" },
            new SelectListItem{ Text = "Beneficio", Value = "BENEFICIO" },
            new SelectListItem{ Text = "Prision Preventiva", Value = "PRISION PREVENTIVA" },
            new SelectListItem{ Text = "No vinculación a proceso", Value = "NO VINCULACION A PROCESO" },
            new SelectListItem{ Text = "Por declinación", Value = "POR DECLINACION" }
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
            new SelectListItem{ Text = "Resolucio", Value = "RESOLUCION" },
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
           int? pageNumber
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
            //List<Beneficios> fraccionesimpuestasVM = _context.Beneficios.ToList();

            //List<Fraccionesimpuestas> queryFracciones = (from f in fraccionesimpuestasVM
            //                                             group f by f.SupervisionIdSupervision into grp
            //                                             select grp.OrderByDescending(f => f.IdFracciones).FirstOrDefault()).ToList();

            //List<Supervision> querySupervisionSinFraccion = (from s in _context.Supervision
            //                                                 join f in _context.Fraccionesimpuestas on s.IdSupervision equals f.SupervisionIdSupervision into SupervisionFracciones
            //                                                 from sf in SupervisionFracciones.DefaultIfEmpty()
            //                                                 select new Supervision
            //                                                 {
            //                                                 }).ToList();


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
                         select new SupervisionPyCPCL
                         {
                             personaVM = p,
                             supervisionVM = s,
                             causapenalVM = cp,
                             planeacionestrategicaVM = pe,
                             cierredecasoVM = cc
                         };

            if (supervisor == false)
            {
                filter = filter.Where(p => p.personaVM.Supervisor == User.Identity.Name);
            }


            ViewData["CurrentFilter"] = searchString;
            ViewData["EstadoS"] = estadoSuper;
            ViewData["FiguraJ"] = figuraJudicial;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = filter.Where(cl => (cl.personaVM.Paterno + " " + cl.personaVM.Materno + " " + cl.personaVM.Nombre).Contains(searchString) ||
                                              (cl.personaVM.Nombre + " " + cl.personaVM.Paterno + " " + cl.personaVM.Materno).Contains(searchString) ||
                                              cl.supervisionVM.EstadoSupervision.Contains(searchString) ||
                                              cl.causapenalVM.CausaPenal.Contains(searchString) ||
                                              cl.personaVM.Supervisor.Contains(searchString) ||
                                              (cl.personaVM.IdPersonaCl.ToString()).Contains(searchString)
                                              );
            }

            if (estadoSuper != null && estadoSuper != "Todos")
            {
                filter = filter.Where(cl => cl.supervisionVM.EstadoSupervision == estadoSuper);
            }

            if (figuraJudicial != null && figuraJudicial != "Todos")
            {
                if (figuraJudicial == "Sin Figura Judicial")
                {
                    filter = filter.Where(cl => cl.beneficiosVM.FiguraJudicial == null);
                }
                else
                {
                    filter = filter.Where(cl => cl.beneficiosVM.FiguraJudicial == figuraJudicial);
                }
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
        public async Task<IActionResult> Supervision(int? id)
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
            ViewBag.idTta = mg.BuscaId(listaNoSiNa,supervisioncl.Tta);
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
                return RedirectToAction(nameof(Index));
            }
            return View(supervisioncl);
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

            List<Beneficios> beneficios= _context.Beneficios.ToList();
            List<Bitacora> bitacora = _context.Bitacora.ToList();
            List<Supervision> supervision = _context.Supervision.ToList();
            List<Persona> personas = _context.Persona.ToList();
            List<Condicionescl> condicionescl = _context.Condicionescl.ToList();
            ViewBag.condicionescl = condicionescl;


            ViewData["Beneficios"] = from b in beneficios
                                     where b.SupervisionclIdSupervisioncl == id
                                     orderby b.SupervisionclIdSupervisioncl
                                     select b;

            ViewBag.IdSupervisionclGuardar = id;

            ViewBag.listaFracciones = listaFracciones;
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
            return Json(new { success = true});

        }
        public async Task<IActionResult> CrearCondicion(Beneficios beneficios, string[] datosBeneficios,string[] datosidCondiciones)
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
            return View();
            //return RedirectToAction(nameof(Index));
        }
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

            List<Condicionescl> condicionescl = _context.Condicionescl.ToList();

            ViewBag.listacondicionescl = condicionescl.Select(c => c.Tipo );
            ViewBag.idCondicionescl =  beneficios.Tipo;

            ViewBag.listaCumplimiento = listaCumplimiento;
            ViewBag.idCumplimiento = mg.BuscaId(listaCumplimiento, beneficios.Estado);

            ViewBag.listaBeneficios = listaBeneficios;
            ViewBag.idFiguraJudicial = mg.BuscaId(listaBeneficios, beneficios.FiguraJudicial);

            return View(beneficios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCondiciones([Bind("IdBeneficios,Tipo,Autoridad,FechaInicio,FechaTermino,Estado,Evidencia,FiguraJudicial,SupervisionclIdSupervisioncl")] Beneficios beneficios, string nombre, string cp, string idpersona)
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
        //public async Task<IActionResult> AddAccionSupervision(string nombre, string cp, int? id, string idpersona, string[] datosBitacora, string supervisor, int idcp)
        //{
        //    int index = cp.IndexOf("?");
        //    if (index >= 0)
        //        cp = cp.Substring(0, index);


        //    ViewBag.cp = cp;
        //    ViewBag.nombre = nombre;
        //    ViewBag.idpersona = idpersona;
        //    ViewBag.supervisor = supervisor;
        //    ViewBag.idcp = idcp;
        //    ViewBag.idfraccionesimpuestas = id;

        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    List<Bitacora> bitacora = _context.Bitacora.ToList();
        //    List<Fraccionesimpuestas> fraccionesImpuestas = _context.Fraccionesimpuestas.ToList();
        //    List<Supervision> supervision = _context.Supervision.ToList();
        //    int SupervisionIdSupervision = 0;
        //    var idsupervision = datosBitacora[0];
        //    if (idsupervision != null)
        //    {
        //        SupervisionIdSupervision = Int32.Parse(idsupervision);
        //    }

        //    var snbitacora = await _context.Bitacora.Where(m => m.FracionesImpuestasIdFracionesImpuestas == id).ToListAsync();
        //    if (snbitacora.Count == 0)
        //    {
        //        return RedirectToAction("CreateBitacora2", new { id, SupervisionIdSupervision, @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @supervisor = supervisor, @idcp = idcp });
        //    }


        //    #region -Select idOficialia
        //    List<Bitacora> bitacorasvm = _context.Bitacora.ToList();

        //    var leftjoin = from o in _context.Oficialia
        //                   join p in _context.Persona on o.UsuarioTurnar equals p.Supervisor
        //                   join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona
        //                   join b in bitacorasvm on o.IdOficialia equals b.OficialiaIdOficialia into temp
        //                   from bo in temp.DefaultIfEmpty()
        //                   select new ListaOficialiaBitacoraViewModel
        //                   {
        //                       oficialiavm = o,
        //                       supervisionvm = s,
        //                       personavm = p,
        //                       bitacoravm = bo
        //                   };

        //    var wheres = (from bn in leftjoin
        //                  where bn.oficialiavm.UsuarioTurnar == supervisor
        //                  group bn by bn.oficialiavm.IdOficialia into grp
        //                  select grp.OrderBy(bn => bn.oficialiavm.IdOficialia).FirstOrDefault()).ToList();


        //    var selects = (from wh in wheres
        //                   select wh.oficialiavm.IdOficialia).ToList();

        //    List<SelectListItem> ListaOficios = new List<SelectListItem>();
        //    ListaOficios = new List<SelectListItem>
        //    {
        //      new SelectListItem{ Text="NA", Value="0"},
        //    };
        //    foreach (var select in selects)
        //    {
        //        ListaOficios.Add(
        //         new SelectListItem { Text = select.ToString(), Value = select.ToString() }
        //        );

        //    }
        //    ViewBag.expoficialia = ListaOficios;

        //    #endregion

        //    ViewData["tablaBiatacora"] = from Bitacora in bitacora
        //                                 where Bitacora.FracionesImpuestasIdFracionesImpuestas == id
        //                                 select new BitacoraViewModal
        //                                 {
        //                                     bitacoraVM = Bitacora
        //                                 };





        //    ViewData["tienebitacora"] = from s in supervision
        //                                join b in bitacora on s.IdSupervision equals b.SupervisionIdSupervision
        //                                join fi in fraccionesImpuestas on b.FracionesImpuestasIdFracionesImpuestas equals fi.IdFracciones
        //                                where s.IdSupervision == id
        //                                select new BitacoraViewModal
        //                                {
        //                                    bitacoraVM = b,
        //                                    supervisionVM = s,
        //                                    fraccionesimpuestasVM = fi
        //                                };



        //    #region ListaTipoPersona
        //    List<SelectListItem> ListaTipoPersona;
        //    ListaTipoPersona = new List<SelectListItem>
        //    {
        //      new SelectListItem{ Text="Supervisado", Value="SUPERVISADO"},
        //      new SelectListItem{ Text="Víctima", Value="VICTIMA"},

        //    };
        //    ViewBag.TipoPersona = ListaTipoPersona;
        //    #endregion

        //    #region ListaTipoVisita
        //    List<SelectListItem> ListaTipoVisita;
        //    ListaTipoVisita = new List<SelectListItem>
        //    {
        //      new SelectListItem{ Text="Presencial", Value="PRESENCIAL"},
        //      new SelectListItem{ Text="Firma Periódica", Value="FIRMA PERIODICA"},
        //      new SelectListItem{ Text="WhatsApp", Value="WHATSAPP"},
        //      new SelectListItem{ Text="Telefónica", Value="TELEFONICA"},
        //      new SelectListItem{ Text="Correo Electrónico", Value="CORREO ELECTRONICO"},
        //      new SelectListItem{ Text="Citatorio", Value="CITATORIO"},
        //      new SelectListItem{ Text="Visita Domiciliar", Value="VISITA DOMICILIAR"},
        //      new SelectListItem{ Text="Notificación a Víctima", Value="NOTIFICACION A VICTIMA"},
        //    };
        //    ViewBag.TipoVisita = ListaTipoVisita;
        //    #endregion

        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> EditAddAccionSupervision([Bind("IdBitacora,Fecha,TipoPersona,Texto,TipoVisita,RutaEvidencia,OficialiaIdOficialia,FechaRegistro,SupervisionIdSupervision,FracionesImpuestasIdFracionesImpuestas ")] Bitacora bitacora, IFormFile evidencia, string nombre, string cp, string idpersona, string supervisor, string idcp)
        //{
        //    bitacora.Texto = mg.normaliza(bitacora.Texto);
        //    bitacora.OficialiaIdOficialia = bitacora.OficialiaIdOficialia;
        //    bitacora.FechaRegistro = bitacora.FechaRegistro;

        //    var supervision = _context.Supervision
        //       .SingleOrDefault(m => m.IdSupervision == bitacora.SupervisionIdSupervision);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            bitacora.Texto = mg.normaliza(bitacora.Texto);


        //            var oldBitacora = await _context.Bitacora.FindAsync(bitacora.IdBitacora, bitacora.SupervisionIdSupervision);

        //            if (evidencia == null)
        //            {
        //                bitacora.RutaEvidencia = oldBitacora.RutaEvidencia;
        //            }
        //            else
        //            {
        //                string file_name = bitacora.IdBitacora + "_" + bitacora.SupervisionIdSupervision + "_" + supervision.PersonaIdPersona + Path.GetExtension(evidencia.FileName);
        //                bitacora.RutaEvidencia = file_name;
        //                var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Evidencia");

        //                if (System.IO.File.Exists(Path.Combine(uploads, file_name)))
        //                {
        //                    System.IO.File.Delete(Path.Combine(uploads, file_name));
        //                }

        //                var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
        //                await evidencia.CopyToAsync(stream);
        //                stream.Close();
        //            }

        //            _context.Entry(oldBitacora).CurrentValues.SetValues(bitacora);

        //            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
        //            _context.Update(bitacora);
        //            await evidencia.CopyToAsync(stream);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!BeneficioExists(bitacora.SupervisionIdSupervision))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction("EditFraccionesimpuestas/" + bitacora.SupervisionIdSupervision, "Supervisiones", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @supervisor = supervisor, @idcp = idcp });
        //    }
        //    return View();
        //}

        //public async Task<IActionResult> CrearAccionSuper(Bitacora bitacora, string[] datosBitacora)
        //{
        //    bitacora.SupervisionIdSupervision = Int32.Parse(datosBitacora[0]);
        //    bitacora.FracionesImpuestasIdFracionesImpuestas = Int32.Parse(datosBitacora[1]);
        //    bitacora.Fecha = mg.validateDatetime(datosBitacora[2]);
        //    bitacora.TipoPersona = datosBitacora[3];
        //    bitacora.Texto = mg.normaliza(datosBitacora[4]);
        //    bitacora.TipoVisita = datosBitacora[5];
        //    bitacora.RutaEvidencia = datosBitacora[6];


        //    var supervision = _context.Supervision
        //       .SingleOrDefault(m => m.IdSupervision == bitacora.SupervisionIdSupervision);

        //    _context.Add(bitacora);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("EditFraccionesimpuestas/" + bitacora.SupervisionIdSupervision, "Supervisiones");
        //}
        //public async Task<IActionResult> DeleteRegistro2(int? id, string nombre, string cp, string idpersona, string supervisor, string idcp)
        //{
        //    var Bitacora = await _context.Bitacora.SingleOrDefaultAsync(m => m.IdBitacora == id);
        //    var oldBitacora = await _context.Bitacora.FindAsync(Bitacora.IdBitacora, Bitacora.SupervisionIdSupervision);
        //    _context.Entry(oldBitacora).CurrentValues.SetValues(Bitacora);


        //    _context.Bitacora.Remove(Bitacora);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("EditFraccionesimpuestas/" + Bitacora.SupervisionIdSupervision, "Supervisiones", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, idcp = idcp, @supervisor = supervisor });

        //}

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
        public async Task<IActionResult> EditPlaneacionEstrategica(int id, [Bind("IdPlaneacionEstrategicacl,PlanSupervision,MotivoNoPlaneacion,VisitaVerificacion,InformeInicial,InformeSeguimiento,InformeFinal,FechaUltimoContacto,FechaProximoContacto,DiaFirma,PeriodicidadFirma,CausaPenalclIdCausaPenalcl,SupervisionclIdSupervisioncl,Tta")] Planeacionestrategicacl planeacionestrategicacl)
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

        #region -EditCierredecaso-
        public async Task<IActionResult> EditCierredecaso(int? id, string nombre, string cp, string idpersona)
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
        public async Task<IActionResult> EditCierredecaso(int id, [Bind("IdCierreDeCaso,SeCerroCaso,ComoConcluyo,NoArchivo,FechaAprobacion,Autorizo,RuataArchivo,SupervisionIdSupervision")] Cierredecasocl cierredecasocl, IFormFile archivo)
        {

            var supervision = _context.Supervisioncl
               .SingleOrDefault(m => m.IdSupervisioncl == cierredecasocl.SupervisionclIdSupervisioncl);

            if (id != cierredecasocl.SupervisionclIdSupervisioncl)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {

                    cierredecasocl.ComoConcluyo = mg.normaliza(cierredecasocl.ComoConcluyo);
                    var oldcierredecaso = await _context.Cierredecaso.FindAsync(cierredecasocl.IdCierreDeCasocl, cierredecasocl.SupervisionclIdSupervisioncl);

                    if (archivo == null)
                    {
                        cierredecasocl.RutaArchivo = oldcierredecaso.RutaArchivo;
                    }
                    else
                    {
                        string file_name = cierredecasocl.IdCierreDeCasocl + "_" + cierredecasocl.SupervisionclIdSupervisioncl + "_" + supervision.PersonaclIdPersonacl + Path.GetExtension(archivo.FileName);
                        cierredecasocl.RutaArchivo = file_name;
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Cierredecaso");

                        if (System.IO.File.Exists(Path.Combine(uploads, file_name)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, file_name));
                        }

                        var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                        await archivo.CopyToAsync(stream);
                        stream.Close();
                    }

                    _context.Entry(oldcierredecaso).CurrentValues.SetValues(cierredecasocl);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
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
                return RedirectToAction("Supervision/" + cierredecasocl.IdCierreDeCasocl, "Supervisioncl");
            }
            return View(cierredecasocl);
        }
        #endregion

        #region -Bitacora-
        public async Task<IActionResult> ListaBitacora(int? id, string nombre, string cp, string idpersona, string idcp, string supervisor)
        {
            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;
            ViewBag.supervisor = supervisor;
            ViewBag.idcp = idcp;

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
            ViewBag.idfraccionesimpuestas = idfraccionesimpuestas;

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
        //public IActionResult CreateBitacora2(string nombre, string cp, int id, int SupervisionIdSupervision, string idpersona, string supervisor, int idcp)
        //{
        //    int index = cp.IndexOf("?");
        //    if (index >= 0)
        //        cp = cp.Substring(0, index);

        //    ViewBag.nombre = nombre;
        //    ViewBag.cp = cp;
        //    ViewBag.idpersona = idpersona;
        //    ViewBag.supervisor = supervisor;
        //    ViewBag.idcp = idcp;

        //    ViewBag.FracionesImpuestasIdFracionesImpuestas = id;
        //    ViewBag.SupervisionIdSupervision = SupervisionIdSupervision;

        //    #region -Select idOficialia

        //    List<Bitacora> bitacorasvm = _context.Bitacora.ToList();

        //    var leftjoin = from o in _context.Oficialia
        //                   join p in _context.Persona on o.UsuarioTurnar equals p.Supervisor
        //                   join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona
        //                   join b in bitacorasvm on o.IdOficialia equals b.OficialiaIdOficialia into temp
        //                   from bo in temp.DefaultIfEmpty()
        //                   select new ListaOficialiaBitacoraViewModel
        //                   {
        //                       oficialiavm = o,
        //                       supervisionvm = s,
        //                       personavm = p,
        //                       bitacoravm = bo
        //                   };
        //    var wheres = (from bn in leftjoin
        //                  where bn.oficialiavm.UsuarioTurnar == supervisor && bn.bitacoravm == null
        //                  group bn by bn.oficialiavm.IdOficialia into grp
        //                  select grp.OrderBy(bn => bn.oficialiavm.IdOficialia).FirstOrDefault()).ToList();

        //    var select = (from wh in wheres
        //                  select wh.oficialiavm.IdOficialia).ToList();

        //    ViewBag.expoficialia = select;
        //    #endregion


        //    return View();
        //}


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

            if (files != null && files.Count > 0)
            {
                var uploadTasks = new List<Task>();

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        string fileExtension = Path.GetExtension(formFile.FileName);
                        string file_name = $"{idbitacora}_{SupervisionclIdSupervisioncl}_{idpersona}{fileExtension}";

                        bitacoracl.RutaEvidencia = file_name;
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "EvidenciaCL");
                        var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);

                        uploadTasks.Add(formFile.CopyToAsync(stream));
                    }
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
                    bitacoracl.RutaEvidencia = bitacoracl.RutaEvidencia;

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
                    return RedirectToAction("EditFraccionesimpuestas/" + bitacoracl.SupervisionclIdSupervisioncl, "Supervisioncl", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @supervisor = supervisor, @idcp = idcp });
                }
                else
                {
                    return RedirectToAction("ListaBitacora/" + bitacoracl.SupervisionclIdSupervisioncl, "Supervisioncl", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @supervisor = supervisor, @idcp = idcp });
                }

            }
            return View(bitacoracl);
        }


        #endregion
        public async Task<IActionResult> EditBitacora(int? id, string nombre, string cp, int idpersona, string supervisor, int idcp, int idSupervisioncl,int IdBeneficios)
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
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Evidenciacl");

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

        #endregion -Edits-

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
