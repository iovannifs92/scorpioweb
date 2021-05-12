using System;
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

namespace scorpioweb.Controllers
{
    [Authorize]
    public class SupervisionesController : Controller
    {
        #region -Constructor-
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public SupervisionesController(penas2Context context, IHostingEnvironment hostingEnvironment,
                        RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        #endregion

        #region -Variables globales-
        private readonly penas2Context _context;
        public static List<string> datosSupervision = new List<string>();

        private List<SelectListItem> listaNaSiNo = new List<SelectListItem>

        {
            new SelectListItem{ Text="NA", Value="NA"},
            new SelectListItem{ Text="Si", Value="SI"},
            new SelectListItem{ Text="No", Value="NO"}
        };

        private List<SelectListItem> listaSiNoNa = new List<SelectListItem>
        {
            new SelectListItem{ Text="Si", Value="SI"},
            new SelectListItem{ Text="No", Value="NO"},
            new SelectListItem{ Text="NA", Value="NA"}
        };
        private List<SelectListItem> listaFracciones = new List<SelectListItem>
        {
            new SelectListItem{ Text="I", Value="I"},
            new SelectListItem{ Text="II", Value="II"},
            new SelectListItem{ Text="III", Value="III"},
            new SelectListItem{ Text="IV", Value="IV"},
            new SelectListItem{ Text="V", Value="V"},
            new SelectListItem{ Text="VI", Value="VI"},
            new SelectListItem{ Text="VII", Value="VII"},
            new SelectListItem{ Text="VIII", Value="VIII"},
            new SelectListItem{ Text="IX", Value="IX"},
            new SelectListItem{ Text="X", Value="X"},
            new SelectListItem{ Text="XI", Value="XI"},
            new SelectListItem{ Text="XII", Value="XII"},
            new SelectListItem{ Text="XIII", Value="XIII"},
            new SelectListItem{ Text="XIV", Value="XIV"}
        };

        private List<SelectListItem> listaCumplimiento = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Cumplimiento", Value = "CUMPLIMIENTO" },
            new SelectListItem{ Text = "Cumplimiento Parcial", Value = "CUMPLIMIENTO PARCIAL" },
            new SelectListItem{ Text = "Incumplimiento Total", Value = "INCUMPLIMIENTO TOTAL" },
        };

        private List<SelectListItem> listaFiguraJudicial = new List<SelectListItem>
        {
            new SelectListItem{ Text = "MC", Value = "MC" },
            new SelectListItem{ Text = "SCP", Value = "SCP" },
        };


        public string normaliza(string normalizar)
        {
            if (!String.IsNullOrEmpty(normalizar))
            {
                normalizar = normalizar.ToUpper();
            }
            return normalizar;
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


        #endregion

        #region -Metodos Generales-

        #region -Crea QR-
        public void creaQR(int? id)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("https://localhost:44359/Personas/Details/" + id, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            System.IO.FileStream fs = System.IO.File.Open(this._hostingEnvironment.WebRootPath + "\\images\\QR.jpg", FileMode.Create);
            qrCodeImage.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        #endregion
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
        #endregion

        #region -CrearDocumento-
        public void crearDocumento(int id)
        {
            List<Fraccionesimpuestas> fraccionesImpuestas = _context.Fraccionesimpuestas.ToList();

            ViewData["fracciones"] = from fracciones in fraccionesImpuestas
                                     where fracciones.SupervisionIdSupervision == id
                                     orderby fracciones.IdFracciones
                                     select fracciones;
            string templatePath = "wwwroot/Documentos/template.docx";
            string resultPath = "wwwroot/Documentos/template.docx";

            DocumentCore dc = DocumentCore.Load(templatePath);

            var dataSource = new[] { new { Nombre = "IOVANNI FERNANDEZ SANCHEZ", Fecha = DateTime.Now.ToString("dd MMMMM yyyy") } };

            var dataSource2 = new
            {
                f1 = "FRACCION I",
                f2 = "FRACCION II",
                f3 = "FRACCION III",
                f4 = "FRACCION IV",
                f5 = "FRACCION V",
                f6 = "FRACCION VI",
                f7 = "FRACCION VII"
            };


            dc.MailMerge.Execute(dataSource);

            dc.MailMerge.ClearOptions = SautinSoft.Document.MailMerging.MailMergeClearOptions.RemoveEmptyTableRows;
            dc.MailMerge.Execute(dataSource2, "Fracciones");
            dc.Save(resultPath);

            Response.Redirect("https://localhost:44359/Documentos/" + "template.docx");


        }
        #endregion


        public ActionResult guardarSupervision(string[] datosS)
        {
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosS.Length; i++)
            {
                datosSupervision = new List<String> { datosS[i], currentUser };
            }

            return Json(new { success = true, responseText = "Datos Guardados con éxito,\n Presione Botón para guardar los cambios" });

        }

        #region -Index-
        public async Task<IActionResult> Index()
        {
            return View(await _context.Supervision.ToListAsync());
        }
        #endregion

        #region -Details-
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision
                .SingleOrDefaultAsync(m => m.IdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            return View(supervision);
        }

        #endregion

        #region -Create-
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSupervision,Inicio,Termino,EstadoSupervision,PersonaIdPersona,EstadoCumplimiento,CausaPenalIdCausaPenal")] Supervision supervision, SupervisionPyCP datosSupervisionDB)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supervision);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            string currentUser = User.Identity.Name;

            #region -idSuper-
            int idSuper = ((from table in _context.Supervision
                            select table.IdSupervision).Max()) + 1;
            supervision.IdSupervision = idSuper;
            #endregion




            return View(supervision);
        }

        #endregion

        #region -Edit-
        public async Task<IActionResult> Edit(int? id, string nombre, string cp)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;

            await PermisosEdicion(id);

            var supervision = await _context.Supervision.SingleOrDefaultAsync(m => m.IdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            #region Estado Suprvición
            List<SelectListItem> ListaEstadoS;
            ListaEstadoS = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Concluido", Value = "CONCLUIDO" },
                new SelectListItem{ Text = "Vigente", Value = "VIGENTE" },
                new SelectListItem{ Text = "Pendiente", Value = "PENDIENTE" },
                };

            ViewBag.listaEstadoSupervision = ListaEstadoS;
            ViewBag.idEstadoSupervision = BuscaId(ListaEstadoS, supervision.EstadoSupervision);
            #endregion



            #region Estado Cumplimiento
            List<SelectListItem> ListaEstadoC;
            ListaEstadoC = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Cumpliendo", Value = "CUMPLIENDO" },
                new SelectListItem{ Text = "Cumplimiento Parcial", Value = "CUMPLIMIENTO PARCIAL" },
                new SelectListItem{ Text = "Incumplimiento Total", Value = "INCUMPLIMIENTO TOTAL" },
            };

            ViewBag.listaEstadoCumplimiento = ListaEstadoC;
            ViewBag.idEstadoCumplimiento = BuscaId(ListaEstadoC, supervision.EstadoCumplimiento);
            #endregion

            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSupervision,Inicio,Termino,EstadoSupervision,PersonaIdPersona,EstadoCumplimiento,CausaPenalIdCausaPenal")] Supervision supervision)
        {
            if (id != supervision.IdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldSupervision = await _context.Supervision.FindAsync(id);
                    _context.Entry(oldSupervision).CurrentValues.SetValues(supervision);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(supervision);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(supervision.IdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + supervision.IdSupervision, "Supervisiones");
            }
            return View(supervision);
        }

        #endregion

        #region -menuOficios-
        public IActionResult GeneraOficios(int id)
        {
            ViewBag.idSupervision = id;

            return View();
        }

        public void tipoOficio(int id, int tipo)
        {
            var supervision = _context.Supervision
               .SingleOrDefault(m => m.IdSupervision == id);

            var persona = _context.Persona
                .SingleOrDefault(m => m.IdPersona == supervision.PersonaIdPersona);

            var causaPenal = _context.Causapenal
                .SingleOrDefault(m => m.IdCausaPenal == supervision.CausaPenalIdCausaPenal);

            List<Delito> delito = _context.Delito.Where(x => x.CausaPenalIdCausaPenal == causaPenal.IdCausaPenal).ToList();
            List<Fraccionesimpuestas> fraccionesImpuestas = _context.Fraccionesimpuestas.Where(x => x.SupervisionIdSupervision == supervision.IdSupervision).ToList();

            switch (tipo)
            {
                case 1:
                    break;
            }
        }
        #endregion

        #region -Editar y borrar fracciones-        

        public async Task<IActionResult> AddOrEdit(int id)
        {
            if (id == 0)
            {
                return View();
            }

            var fraccionesimpuestas = await _context.Fraccionesimpuestas.SingleOrDefaultAsync(m => m.IdFracciones == id);
            if (fraccionesimpuestas == null)
            {
                return NotFound();
            }

            ViewBag.listaFracciones = listaFracciones;
            ViewBag.idFraccion = BuscaId(listaFracciones, fraccionesimpuestas.Tipo);

            ViewBag.listaCumplimiento = listaCumplimiento;
            ViewBag.idCumplimiento = BuscaId(listaCumplimiento, fraccionesimpuestas.Estado);

            ViewBag.listaFiguraJudicial = listaFiguraJudicial;
            ViewBag.idFiguraJudicial = BuscaId(listaFiguraJudicial, fraccionesimpuestas.FiguraJudicial);

            return View(fraccionesimpuestas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("IdFracciones,Tipo,Autoridad,FechaInicio,FechaTermino,Estado,Evidencia,FiguraJudicial,SupervisionIdSupervision")] Fraccionesimpuestas fraccionesimpuestas)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var oldFraccionesimpuestas = await _context.Fraccionesimpuestas.FindAsync(fraccionesimpuestas.IdFracciones, fraccionesimpuestas.SupervisionIdSupervision);
                    _context.Entry(oldFraccionesimpuestas).CurrentValues.SetValues(fraccionesimpuestas);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(fraccionesimpuestas);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(fraccionesimpuestas.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + fraccionesimpuestas.SupervisionIdSupervision, "Supervisiones");
            }
            return View(fraccionesimpuestas);
        }


        public async Task<IActionResult> DeleteFraccion(int? id)
        {
            var fraccionesimpuestas = await _context.Fraccionesimpuestas.SingleOrDefaultAsync(m => m.IdFracciones == id);
            _context.Fraccionesimpuestas.Remove(fraccionesimpuestas);
            await _context.SaveChangesAsync();

            return RedirectToAction("Supervision/" + fraccionesimpuestas.SupervisionIdSupervision, "Supervisiones");
        }
        #endregion

        #region -Delete-
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision
                .SingleOrDefaultAsync(m => m.IdSupervision == id);
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
            var supervision = await _context.Supervision.SingleOrDefaultAsync(m => m.IdSupervision == id);
            _context.Supervision.Remove(supervision);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region -SupervisionExists-
        private bool SupervisionExists(int id)
        {
            return _context.Supervision.Any(e => e.IdSupervision == id);
        }

        #endregion

        public static IEnumerable<Supervision> supervisions = new List<Supervision> {
                new Supervision {
                    IdSupervision = 1,
                    EstadoSupervision = "Red"
                },
                new Supervision {
                    IdSupervision = 2,
                    EstadoSupervision = "Blue"
                }
            };

        #region -MenuSupervision-
        public IActionResult MenuSupervision()
        {
            return View();
        }
        #endregion

        #region -PersonaSupervision-
        public async Task<IActionResult> PersonaSupervision(
           string sortOrder,
           string currentFilter,
           string searchString,
           string estadoSuper,
           int? pageNumber
           )

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

            bool supervisor = false;

            var usuario = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(usuario);

            List<string> rolUsuario = new List<string>();

            for (int i = 0; i < roles.Count; i++)
            {
                rolUsuario.Add(roles[i]);
                if(roles[i]== "SupervisorMCSCP")
                {
                    supervisor = true;
                }
            }

            var filter= from p in _context.Persona
                        join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona
                        join cp in _context.Causapenal on s.CausaPenalIdCausaPenal equals cp.IdCausaPenal
                        select new SupervisionPyCP
                        {
                            personaVM = p,
                            supervisionVM = s,
                            causapenalVM = cp
                        };

            if (supervisor)
            {
                filter = from p in _context.Persona
                             join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona
                             join cp in _context.Causapenal on s.CausaPenalIdCausaPenal equals cp.IdCausaPenal
                             where p.Supervisor == User.Identity.Name
                             select new SupervisionPyCP
                             {
                                 personaVM = p,
                                 supervisionVM = s,
                                 causapenalVM = cp
                             };
            }
            

            ViewData["CurrentFilter"] = searchString;
            ViewData["EstadoS"] = estadoSuper;


            if (!String.IsNullOrEmpty(searchString))
            {
                filter = filter.Where(spcp => (spcp.personaVM.Paterno + " " + spcp.personaVM.Materno + " " + spcp.personaVM.Nombre).Contains(searchString) ||
                                              (spcp.personaVM.Nombre + " " + spcp.personaVM.Paterno + " " + spcp.personaVM.Materno).Contains(searchString) ||
                                              spcp.supervisionVM.EstadoSupervision.Contains(searchString) ||
                                              spcp.causapenalVM.CausaPenal.Contains(searchString) ||
                                              spcp.personaVM.Supervisor.Contains(searchString)
                                              );
            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(spcp => spcp.personaVM.Paterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(spcp => spcp.causapenalVM.CausaPenal);
                    break;
                case "estado_cumplimiento_desc":
                    filter = filter.OrderByDescending(spcp => spcp.supervisionVM.EstadoCumplimiento);
                    break;
                default:
                    filter = filter.OrderBy(spcp => spcp.personaVM.Paterno);
                    break;
            }





            int pageSize = 10;
            return View(await PaginatedList<SupervisionPyCP>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PersonaSupervision(int id, [Bind("IdSupervision,Inicio,Termino,EstadoSupervision,PersonaIdPersona,EstadoCumplimiento,CausaPenalIdCausaPenal")] Supervision supervision)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var oldSupervision = await _context.Supervision.FindAsync(id);
                    _context.Entry(oldSupervision).CurrentValues.SetValues(supervision);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(supervision);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(supervision.IdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(PersonaSupervision));
            }
            return View(supervision);
        }
        #endregion

        #region -Supervision-
        public async Task<IActionResult> Supervision(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision.SingleOrDefaultAsync(m => m.IdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }


            List<Supervision> SupervisionVM = _context.Supervision.ToList();
            List<Causapenal> causaPenalVM = _context.Causapenal.ToList();
            List<Persona> personaVM = _context.Persona.ToList();
            #region -Jointables-
            ViewData["joinTablesSupervision"] = from supervisiontable in SupervisionVM
                                                join personatable in personaVM on supervisiontable.PersonaIdPersona equals personatable.IdPersona
                                                join causapenaltable in causaPenalVM on supervisiontable.CausaPenalIdCausaPenal equals causapenaltable.IdCausaPenal
                                                where supervisiontable.IdSupervision == id

                                                select new SupervisionPyCP
                                                {
                                                    causapenalVM = causapenaltable,
                                                    supervisionVM = supervisiontable,
                                                    personaVM = personatable
                                                };
            #endregion


            return View();
        }
        #endregion


        #region -Aer-
        public async Task<IActionResult> EditAer(int? id, string nombre, string cp)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;

            await PermisosEdicion(id);

            var supervision = await _context.Aer.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            ViewBag.listaCuentaEvaluacion = listaNaSiNo;
            ViewBag.idCuentaEvaluacion = BuscaId(listaNaSiNo, supervision.CuentaEvaluacion);
            ViewBag.eveluacion = supervision.CuentaEvaluacion;



            #region Riesgo
            List<SelectListItem> ListaEstadoC;
            ListaEstadoC = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Baja", Value = "BAJA" },
                new SelectListItem{ Text = "Media", Value = "MEDIA" },
                new SelectListItem{ Text = "Alta", Value = "ALTA" }
                };

            ViewBag.listaRiesgoDetectado = ListaEstadoC;
            ViewBag.idRiesgoDetectado = BuscaId(ListaEstadoC, supervision.RiesgoDetectado);

            ViewBag.listaRiesgoSustraccion = ListaEstadoC;
            ViewBag.idRiesgoSustraccion = BuscaId(ListaEstadoC, supervision.RiesgoSustraccion);

            ViewBag.listaRiesgoObstaculizacion = ListaEstadoC;
            ViewBag.idRiesgoObstaculizacion = BuscaId(ListaEstadoC, supervision.RiesgoObstaculizacion);

            ViewBag.listaRiesgoVictima = ListaEstadoC;
            ViewBag.idRiesgoVictima = BuscaId(ListaEstadoC, supervision.RiesgoVictima);
            #endregion



            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAer(int id, [Bind("IdAer,CuentaEvaluacion,FechaEntrega,EvaluadorCaso,RiesgoDetectado,RiesgoSustraccion,RiesgoObstaculizacion,RiesgoVictima,SupervisionIdSupervision")] Aer aer)
        {
            if (id != aer.SupervisionIdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldAer = await _context.Aer.FindAsync(aer.IdAer, aer.SupervisionIdSupervision);
                    _context.Entry(oldAer).CurrentValues.SetValues(aer);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(aer);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(aer.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + aer.SupervisionIdSupervision, "Supervisiones");
            }
            return View(aer);
        }
        #endregion

        #region -EditCambiodeobligaciones-
        public async Task<IActionResult> EditCambiodeobligaciones(int? id, string nombre, string cp, string cambio)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;

            await PermisosEdicion(id);

            var supervision = await _context.Cambiodeobligaciones.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            ViewBag.listaSediocambio = listaNaSiNo;
            ViewBag.idSediocambio = BuscaId(listaNaSiNo, supervision.SeDioCambio);
            ViewBag.cambio = supervision.SeDioCambio;

            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCambiodeobligaciones(int id, [Bind("IdCambiodeObligaciones,SeDioCambio,FechaAprobacion,MotivoAprobacion,SupervisionIdSupervision")] Cambiodeobligaciones cambiodeobligaciones)
        {
            if (id != cambiodeobligaciones.SupervisionIdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldCambiodeobligaciones = await _context.Cambiodeobligaciones.FindAsync(cambiodeobligaciones.IdCambiodeObligaciones, cambiodeobligaciones.SupervisionIdSupervision);
                    _context.Entry(oldCambiodeobligaciones).CurrentValues.SetValues(cambiodeobligaciones);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(cambiodeobligaciones);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(cambiodeobligaciones.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + cambiodeobligaciones.SupervisionIdSupervision, "Supervisiones");
            }
            return View(cambiodeobligaciones);
        }
        #endregion

        #region -EditCierredecaso-
        public async Task<IActionResult> EditCierredecaso(int? id, string nombre, string cp)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;

            await PermisosEdicion(id);

            var supervision = await _context.Cierredecaso.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }


            ViewBag.listaSeCerroCaso = listaNaSiNo;
            ViewBag.idSeCerroCaso = BuscaId(listaNaSiNo, supervision.SeCerroCaso);
            ViewBag.cierre = supervision.SeCerroCaso;
            #region Autorizo
            List<SelectListItem> ListaAutorizo;
            ListaAutorizo = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Director", Value = "DIRECTOR" },
                new SelectListItem{ Text = "Coordinador", Value = "COORDINADOR" }
                };

            ViewBag.listaAutorizo = ListaAutorizo;
            ViewBag.idAutorizo = BuscaId(ListaAutorizo, supervision.Autorizo);
            #endregion



            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCierredecaso(int id, [Bind("IdCierreDeCaso,SeCerroCaso,ComoConcluyo,NoArchivo,FechaAprobacion,Autorizo,SupervisionIdSupervision")] Cierredecaso cierredecaso)
        {
            if (id != cierredecaso.SupervisionIdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldCierredecaso = await _context.Cierredecaso.FindAsync(cierredecaso.IdCierreDeCaso, cierredecaso.SupervisionIdSupervision);
                    _context.Entry(oldCierredecaso).CurrentValues.SetValues(cierredecaso);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(cierredecaso);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(cierredecaso.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + cierredecaso.SupervisionIdSupervision, "Supervisiones");
            }
            return View(cierredecaso);
        }
        #endregion

        #region -Fraccionesimpuestas-
        public async Task<IActionResult> EditFraccionesimpuestas(int? id, string nombre, string cp)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;

            await PermisosEdicion(id);

            List < Fraccionesimpuestas > fraccionesImpuestas = _context.Fraccionesimpuestas.ToList();

            ViewData["fracciones"] = from fracciones in fraccionesImpuestas
                                     where fracciones.SupervisionIdSupervision == id
                                     orderby fracciones.IdFracciones
                                     select fracciones;

            ViewBag.IdSupervisionGuardar = id;

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
                    var oldFraccionesimpuestas = await _context.Fraccionesimpuestas.FindAsync(fraccionesimpuestas.IdFracciones, fraccionesimpuestas.SupervisionIdSupervision);
                    _context.Entry(oldFraccionesimpuestas).CurrentValues.SetValues(fraccionesimpuestas);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(fraccionesimpuestas);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(fraccionesimpuestas.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + fraccionesimpuestas.SupervisionIdSupervision, "Supervisiones");
            }
            return View(fraccionesimpuestas);
        }

        public async Task<IActionResult> CrearFracciones(Fraccionesimpuestas fraccionesImpuestas, string[] datosFracciones)
        {
            for (int i = 0; i < 14; i++)
            {
                if (bool.Parse(datosFracciones[i]) == true)
                {
                    fraccionesImpuestas.SupervisionIdSupervision = Int32.Parse(datosFracciones[14]);
                    fraccionesImpuestas.FiguraJudicial = datosFracciones[15];
                    fraccionesImpuestas.FechaInicio = validateDatetime(datosFracciones[16]);
                    fraccionesImpuestas.FechaTermino = validateDatetime(datosFracciones[17]);
                    fraccionesImpuestas.Estado = datosFracciones[18];
                    switch (i)
                    {
                        case 0:
                            fraccionesImpuestas.Tipo = "I";
                            break;
                        case 1:
                            fraccionesImpuestas.Tipo = "II";
                            break;
                        case 2:
                            fraccionesImpuestas.Tipo = "III";
                            break;
                        case 3:
                            fraccionesImpuestas.Tipo = "IV";
                            break;
                        case 4:
                            fraccionesImpuestas.Tipo = "V";
                            break;
                        case 5:
                            fraccionesImpuestas.Tipo = "VI";
                            break;
                        case 6:
                            fraccionesImpuestas.Tipo = "VII";
                            break;
                        case 7:
                            fraccionesImpuestas.Tipo = "VIII";
                            break;
                        case 8:
                            fraccionesImpuestas.Tipo = "IX";
                            break;
                        case 9:
                            fraccionesImpuestas.Tipo = "X";
                            break;
                        case 10:
                            fraccionesImpuestas.Tipo = "XI";
                            break;
                        case 11:
                            fraccionesImpuestas.Tipo = "XII";
                            break;
                        case 12:
                            fraccionesImpuestas.Tipo = "XIII";
                            break;
                        case 13:
                            fraccionesImpuestas.Tipo = "XIV";
                            break;
                    }
                    _context.Add(fraccionesImpuestas);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                }
            }
            return View();
            //return RedirectToAction(nameof(Index));
        }
        #endregion



        #region -EditPlaneacionestrategica-
        public async Task<IActionResult> EditPlaneacionestrategica(int? id, string nombre, string cp)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;

            await PermisosEdicion(id);

            var supervision = await _context.Planeacionestrategica.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }


            ViewBag.listaPlanSupervision = listaNaSiNo;
            ViewBag.idPlanSupervision = BuscaId(listaNaSiNo, supervision.PlanSupervision);
            ViewBag.plan = supervision.PlanSupervision;


            #region Liata de supervision
            List<SelectListItem> ListaPeriodicidadFirma;
            ListaPeriodicidadFirma = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Diaria", Value = "DIARIA" },
                new SelectListItem{ Text = "Semanal", Value = "SEMANAL" },
                new SelectListItem{ Text = "Quincenal", Value = "QUINCENAL" },
                new SelectListItem{ Text = "Mensual", Value = "MENSUAL" },
                new SelectListItem{ Text = "Bimestral", Value = "BIMESTRAL" },
                new SelectListItem{ Text = "Trimestral", Value = "TRIMESTRAL" },
                new SelectListItem{ Text = "Semestral", Value = "SEMESTRAL" },
                new SelectListItem{ Text = "Anual", Value = "ANUAL" },
                };

            ViewBag.listaPeriodicidadFirma = ListaPeriodicidadFirma;
            ViewBag.idPeriodicidadFirma = BuscaId(ListaPeriodicidadFirma, supervision.PeriodicidadFirma);
            #endregion


            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlaneacionestrategica(int id, [Bind("IdPlaneacionEstrategica,PlanSupervision,MotivoNoPlaneacion,FechaAprobacion,UltimoInforme,FechaInforme,FechaUltimoContacto,FechaProximoContacto,DiaFirma,PeriodicidadFirma,SupervisionIdSupervision")] Planeacionestrategica planeacionestrategica)
        {
            if (id != planeacionestrategica.SupervisionIdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldPlaneacionestrategica = await _context.Planeacionestrategica.FindAsync(planeacionestrategica.IdPlaneacionEstrategica, planeacionestrategica.SupervisionIdSupervision);
                    _context.Entry(oldPlaneacionestrategica).CurrentValues.SetValues(planeacionestrategica);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(planeacionestrategica);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(planeacionestrategica.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + planeacionestrategica.SupervisionIdSupervision, "Supervisiones");
            }
            return View(planeacionestrategica);
        }
        #endregion

        #region -Revocacion-


        public async Task<IActionResult> EditRevocacion(int? id, string nombre, string cp)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;

            await PermisosEdicion(id);

            var supervision = await _context.Revocacion.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            ViewBag.listaRevocado = listaNaSiNo;
            ViewBag.idRevocado = BuscaId(listaNaSiNo, supervision.Revocado);
            ViewBag.revocado = supervision.Revocado;




            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRevocacion(int id, [Bind("IdRevocacion,Revocado,FechaAprobacion,MotivoRevocacion,SupervisionIdSupervision")] Revocacion revocacion)
        {
            if (id != revocacion.SupervisionIdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldRevocacion = await _context.Revocacion.FindAsync(revocacion.IdRevocacion, revocacion.SupervisionIdSupervision);
                    _context.Entry(oldRevocacion).CurrentValues.SetValues(revocacion);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(revocacion);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(revocacion.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + revocacion.SupervisionIdSupervision, "Supervisiones");
            }
            return View(revocacion);
        }
        #endregion

        #region -EditSuspensionseguimiento-
        public async Task<IActionResult> EditSuspensionseguimiento(int? id, string nombre, string cp)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;

            await PermisosEdicion(id);

            var supervision = await _context.Suspensionseguimiento.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            ViewBag.listaSuspendido = listaNaSiNo;
            ViewBag.idSuspendido = BuscaId(listaNaSiNo, supervision.Suspendido);
            ViewBag.suspe = supervision.Suspendido;

            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSuspensionseguimiento(int id, [Bind("IdSuspensionSeguimiento,Suspendido,FechaAprobacion,MotivoSuspension,SupervisionIdSupervision")] Suspensionseguimiento suspensionseguimiento)
        {
            if (id != suspensionseguimiento.SupervisionIdSupervision)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldSuspensionseguimiento = await _context.Suspensionseguimiento.FindAsync(suspensionseguimiento.IdSuspensionSeguimiento, suspensionseguimiento.SupervisionIdSupervision);
                    _context.Entry(oldSuspensionseguimiento).CurrentValues.SetValues(suspensionseguimiento);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(suspensionseguimiento);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(suspensionseguimiento.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + suspensionseguimiento.SupervisionIdSupervision, "Supervisiones");
            }
            return View();
        }
        #endregion

        #region -EditVictima-

        #region -Lista Victima-
        public async Task<IActionResult> ListaVictima(int? id)
        {
            var supervision = _context.Supervision
            .SingleOrDefault(m => m.IdSupervision == id);

            var persona = _context.Persona
           .SingleOrDefault(m => m.IdPersona == supervision.PersonaIdPersona);
            var cp = _context.Causapenal
           .SingleOrDefault(m => m.IdCausaPenal == supervision.CausaPenalIdCausaPenal);

            ViewBag.nombre = persona.NombreCompleto;
            ViewBag.cp = cp.CausaPenal;

            await PermisosEdicion(id);


            List<Victima> victimas  = _context.Victima.ToList();

            ViewData["Victima"] = from table in victimas
                                   where table.SupervisionIdSupervision == id
                                   orderby table.IdVictima
                                   select table;

            ViewBag.IdSupervisionGuardar = id;


            return View();
        }
        #endregion
        #region -Create Victima-
        public IActionResult CreateVictima(int? id, string nombre, string cp)
        {
            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.IdSupervisionGuardar = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVictima(Victima victima, string IdVictima, string NombreV, string Edad, string Telefono, string ConoceDetenido, string TipoRelacion, 
            string TiempoConocerlo, string ViveSupervisado, string Direccion, string Victimacol, string Observaciones, string SupervisionIdSupervision)
        {
            string currentUser = User.Identity.Name;
            if (ModelState.ErrorCount <= 1)
            {
                victima.NombreV = normaliza(NombreV);
                victima.Edad = Edad;
                victima.Telefono = Telefono;
                victima.ConoceDetenido = normaliza(ConoceDetenido);
                victima.TipoRelacion = normaliza(TipoRelacion);
                victima.TiempoConocerlo = normaliza(TiempoConocerlo);
                victima.ViveSupervisado = normaliza(ViveSupervisado);
                victima.Direccion = normaliza(Direccion);
                victima.Victimacol = normaliza(Victimacol);
                victima.Observaciones = normaliza(Observaciones);


                var supervision = _context.Supervision
               .SingleOrDefault(m => m.IdSupervision == victima.SupervisionIdSupervision);


                var persona = _context.Persona
               .SingleOrDefault(m => m.IdPersona == supervision.PersonaIdPersona);
                var cp = _context.Causapenal
               .SingleOrDefault(m => m.IdCausaPenal == supervision.CausaPenalIdCausaPenal);

                ViewBag.Npersona = persona.NombreCompleto;
                ViewBag.cp = cp.CausaPenal;

                int idVictima = ((from table in _context.Victima
                                   select table.IdVictima).Max()) + 1;

                victima.IdVictima = idVictima;
                _context.Add(victima);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListaVictima/" + victima.SupervisionIdSupervision, "Supervisiones");
            }
            return View(victima);
        }
        #endregion

        public async Task<IActionResult> Editvictima(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }            

            var Victima = await _context.Victima.SingleOrDefaultAsync(m => m.IdVictima == id);
            if (Victima == null)
            {
                return NotFound();
            }

            ViewBag.ConoceDetenido = listaSiNoNa;
            ViewBag.idConoceDetenido = BuscaId(listaSiNoNa, Victima.ConoceDetenido);


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
            ViewBag.TipoRelacion = ListaRelacion;
            ViewBag.idTipoRelacion = BuscaId(ListaRelacion, Victima.TipoRelacion);
            #endregion


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
            ViewBag.idTiempoConocerlo = BuscaId(ListaTiempo, Victima.TiempoConocerlo);

            ViewBag.ViveSupervisado = listaNaSiNo;
            ViewBag.idViveSupervisado = BuscaId(listaNaSiNo, Victima.ViveSupervisado);


            return View(Victima);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editvictima(int id, [Bind("IdVictima,NombreV,Edad,Telefono,ConoceDetenido,TipoRelacion,TiempoConocerlo,ViveSupervisado,Direccion,Victimacol,SupervisionIdSupervision, Observaciones")] Victima victima)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var oldVictima = await _context.Victima.FindAsync(victima.IdVictima, victima.SupervisionIdSupervision);
                    _context.Entry(oldVictima).CurrentValues.SetValues(victima);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(victima.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Supervision/" + victima.SupervisionIdSupervision, "Supervisiones");
            }
            return View();
        }
        public async Task<IActionResult> DeleteVictima(int? id)
        {
            var Victima = await _context.Victima.SingleOrDefaultAsync(m => m.IdVictima == id);
            _context.Victima.Remove(Victima);
            await _context.SaveChangesAsync();

            return RedirectToAction("Supervision/" + Victima.SupervisionIdSupervision, "Supervisiones");
        }
        public async Task<IActionResult> VerVictima(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Victima = await _context.Victima
                .SingleOrDefaultAsync(m => m.IdVictima == id);
            if (Victima == null)
            {
                return NotFound();
            }
            return View(Victima);
        }
        #endregion

        #region --Bitacora--
        public async Task<IActionResult> ListaBitacora(int? id)
        {
            var supervision = _context.Supervision
            .SingleOrDefault(m => m.IdSupervision == id);

            var persona = _context.Persona
           .SingleOrDefault(m => m.IdPersona == supervision.PersonaIdPersona);
            var cp = _context.Causapenal
           .SingleOrDefault(m => m.IdCausaPenal == supervision.CausaPenalIdCausaPenal);

            ViewBag.nombre = persona.NombreCompleto;
            ViewBag.cp = cp.CausaPenal;

            await PermisosEdicion(id);


            List<Bitacora> bitacora = _context.Bitacora.ToList();

            ViewData["Bitacora"] = from table in bitacora
                                   where table.SupervisionIdSupervision == id
                                   orderby table.IdBitacora
                                   select table;

            ViewBag.IdSupervisionGuardar = id;


            return View();
        }
        #region -Create Bitacora-
        public IActionResult CreateBitacora(int? id, string nombre, string cp)
        {
            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.IdSupervisionGuardar = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBitacora(Bitacora bitacora, string IdBitacora, DateTime Fecha, string tipoPersona,
            string tipoVisita, string Texto, string SupervisionIdSupervision, IFormFile evidencia)
        {
            string currentUser = User.Identity.Name;
            if (ModelState.ErrorCount <= 1)
            {
                bitacora.Fecha = Fecha;
                bitacora.TipoPersona = normaliza(tipoPersona);
                bitacora.TipoVisita = normaliza(tipoVisita);
                bitacora.Texto = normaliza(Texto);


                var supervision = _context.Supervision
               .SingleOrDefault(m => m.IdSupervision == bitacora.SupervisionIdSupervision);


                var persona = _context.Persona
               .SingleOrDefault(m => m.IdPersona == supervision.PersonaIdPersona);
                var cp = _context.Causapenal
               .SingleOrDefault(m => m.IdCausaPenal == supervision.CausaPenalIdCausaPenal);

                ViewBag.Npersona = persona.NombreCompleto;
                ViewBag.cp = cp.CausaPenal;

                int idBitacora = ((from table in _context.Bitacora
                                   select table.IdBitacora).Max()) + 1;

                bitacora.IdBitacora = idBitacora;

                #region -Guardar archivo-
                string file_name = bitacora.IdBitacora + "_" + bitacora.SupervisionIdSupervision + "_" + supervision.PersonaIdPersona + Path.GetExtension(evidencia.FileName);
                bitacora.RutaEvidencia = file_name;
                var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Evidencia");
                var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                #endregion


                _context.Add(bitacora);
                await evidencia.CopyToAsync(stream);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListaBitacora/" + bitacora.SupervisionIdSupervision, "Supervisiones");
            }
            return View(bitacora);
        }
        #endregion
        public async Task<IActionResult> EditBitacora(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bitacora = await _context.Bitacora.SingleOrDefaultAsync(m => m.IdBitacora == id);
            if (bitacora == null)
            {
                return NotFound();
            }

            #region ListaTipoPersona
            List<SelectListItem> ListaTipoPersona;
            ListaTipoPersona = new List<SelectListItem>
            {
              new SelectListItem{ Text="Supervisado", Value="SUPERVISADO"},
              new SelectListItem{ Text="Víctima", Value="VICTIMA"},

            };
            ViewBag.TipoPersona = ListaTipoPersona;
            ViewBag.idTipoPersona = BuscaId(ListaTipoPersona, bitacora.TipoPersona);
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
            };
            ViewBag.TipoVisita = ListaTipoVisita;
            ViewBag.idTipoVisita = BuscaId(ListaTipoVisita, bitacora.TipoVisita);
            #endregion

            ViewBag.RutaEvidencia =  bitacora.RutaEvidencia;

            return View(bitacora);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBitacora([Bind("IdBitacora,Fecha,TipoPersona,Texto,TipoVisita,RutaEvidencia,SupervisionIdSupervision")] Bitacora bitacora, IFormFile evidencia)
        {
            bitacora.Texto = normaliza(bitacora.Texto);

            var supervision = _context.Supervision
               .SingleOrDefault(m => m.IdSupervision == bitacora.SupervisionIdSupervision);

            if (ModelState.IsValid)
            {
                try
                {
                    var oldBitacora = await _context.Bitacora.FindAsync(bitacora.IdBitacora, bitacora.SupervisionIdSupervision);

                    if(evidencia == null)
                    { 
                        bitacora.RutaEvidencia = oldBitacora.RutaEvidencia;
                    }
                    else
                    {
                        string file_name = bitacora.IdBitacora + "_" + bitacora.SupervisionIdSupervision + "_" + supervision.PersonaIdPersona + Path.GetExtension(evidencia.FileName);
                        bitacora.RutaEvidencia = file_name;
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Evidencia");

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
                    //_context.Update(bitacora);
                    //await evidencia.CopyToAsync(stream);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BitacoraExists(bitacora.SupervisionIdSupervision))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ListaBitacora/" + bitacora.SupervisionIdSupervision, "Supervisiones");
            }
            return View(bitacora);
        }
        public async Task<IActionResult> DeleteRegistro(int? id)
        {
            var Bitacora = await _context.Bitacora.SingleOrDefaultAsync(m => m.IdBitacora == id);
            _context.Bitacora.Remove(Bitacora);
            await _context.SaveChangesAsync();

            return RedirectToAction("Supervision/" + Bitacora.SupervisionIdSupervision, "Supervisiones");
        }
        
        private bool BitacoraExists(int id)
        {
            return _context.Bitacora.Any(e => e.IdBitacora == id);
        }

        private bool VictimaExists(int id)
        {
            return _context.Victima.Any(e => e.IdVictima == id);
        }

        #endregion



        #region -Graficos-   
        private static MemoryStream BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream;
            }
        }

        #endregion


        public IActionResult Archivos()
        {
            return View();
        }

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

    }
}
