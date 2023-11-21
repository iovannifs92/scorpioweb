using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
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
        // GET: Supervisioncl/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Supervisioncl/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSupervisioncl,Inicio,Termino,EstadoSupervision,PersonaclIdPersonacl,EstadoCumplimiento,CausaPenalclIdCausaPenalcl,Tta")] Supervisioncl supervisioncl)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supervisioncl);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supervisioncl);
        }


        #region Edicines 
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

            ViewData["fracciones"] = from b in beneficios
                                     where b.SupervisionclIdSupervisioncl == id
                                     orderby b.SupervisionclIdSupervisioncl
                                     select b;

            ViewBag.IdSupervisionclGuardar = id;

            ViewBag.listaFracciones = listaFracciones;
            //ViewBag.listaFiguraJudicial = listaFiguraJudicial;
            ViewBag.listaCumplimiento = listaCumplimiento;


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFraccionesimpuestas(int id, [Bind("IdFracciones,Tipo,Autoridad,FechaInicio,FechaTermino,Estado,Evidencia,FiguraJudicial,SupervisionIdSupervision")] Fraccionesimpuestas fraccionesimpuestas)
        {
            if (id != fraccionesimpuestas.SupervisionIdSupervision)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    fraccionesimpuestas.Autoridad = mg.normaliza(fraccionesimpuestas.Autoridad);

                    var oldFraccionesimpuestas = await _context.Fraccionesimpuestas.FindAsync(fraccionesimpuestas.IdFracciones, fraccionesimpuestas.SupervisionIdSupervision);
                    _context.Entry(oldFraccionesimpuestas).CurrentValues.SetValues(fraccionesimpuestas);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    _context.Update(fraccionesimpuestas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionclExists(fraccionesimpuestas.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("EditFraccionesimpuestas/" + fraccionesimpuestas.SupervisionIdSupervision, "Supervisiones");
            }
            return View(fraccionesimpuestas);
        }
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

            ViewBag.listaPeriodicidas = listaPeridodicidad;
            ViewBag.idEstadoCumplimiento = mg.BuscaId(listaCumplimiento, planeacionestrategicacl.PeriodicidadFirma);
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
        #endregion

        // GET: Supervisioncl/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervisioncl = await _context.Supervisioncl
                .SingleOrDefaultAsync(m => m.IdSupervisioncl == id);
            if (supervisioncl == null)
            {
                return NotFound();
            }

            return View(supervisioncl);
        }

        // POST: Supervisioncl/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supervisioncl = await _context.Supervisioncl.SingleOrDefaultAsync(m => m.IdSupervisioncl == id);
            _context.Supervisioncl.Remove(supervisioncl);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #region -VERIFICAR EXISTE-
        private bool SupervisionclExists(int id)
        {
            return _context.Supervisioncl.Any(e => e.IdSupervisioncl == id);
        }   private bool PlaneacionExists(int id)
        {
            return _context.Planeacionestrategicacl.Any(e => e.IdPlaneacionEstrategicacl == id);
        }
        #endregion
    }
}
