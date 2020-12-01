using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;
using SautinSoft.Document;
using Microsoft.AspNetCore.Hosting;
using SautinSoft.Document.Drawing;
using QRCoder;
using System.Drawing;
using Size = SautinSoft.Document.Drawing.Size;
using System.IO;

namespace scorpioweb.Controllers
{
    public class SupervisionesController : Controller
    {
        #region -Constructor-
        private readonly IHostingEnvironment _hostingEnvironment;
        public SupervisionesController(penas2Context context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
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

        private List<SelectListItem> listaCumplimiento=new List<SelectListItem>
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

            Response.Redirect("https://localhost:44359/Documentos/"+"template.docx");


        }
        #endregion


        public ActionResult guardarSupervision(string[] datosS)
        {
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosS.Length; i++)
            {
                datosSupervision = new List<String> { datosS[i], currentUser };
            }

            return Json(new { success = true, responseText = "Datos Guardados con éxito,\n Presione Boton para guaradar los cambios" });

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
                            select table).Count()) + 1;
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
                    _context.Update(supervision);
                    await _context.SaveChangesAsync();
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
                    _context.Update(fraccionesimpuestas);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction("Supervision/"+fraccionesimpuestas.SupervisionIdSupervision,"Supervisiones");
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

        #region -PersonaSupervicion-
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

            var filter = from p in _context.Persona
                         join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona
                         join cp in _context.Causapenal on s.CausaPenalIdCausaPenal equals cp.IdCausaPenal
                         where p.Supervisor == User.Identity.Name
                         select new SupervisionPyCP
                         {
                             personaVM = p,
                             supervisionVM = s,
                             causapenalVM = cp
                         };

            ViewData["CurrentFilter"] = searchString;
            ViewData["EstadoS"] = estadoSuper;


            if (!String.IsNullOrEmpty(searchString))
            {
                filter = filter.Where(spcp => spcp.personaVM.Paterno.Contains(searchString) ||
                                              spcp.personaVM.Materno.Contains(searchString) ||
                                              spcp.personaVM.Nombre.Contains(searchString) ||
                                              spcp.supervisionVM.EstadoSupervision.Contains(searchString) ||
                                              spcp.causapenalVM.CausaPenal.Contains(searchString) ||
                                              spcp.supervisionVM.EstadoSupervision.Contains(estadoSuper)
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
                    _context.Update(supervision);
                    await _context.SaveChangesAsync();
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
                                          join  personatable in personaVM on supervisiontable.PersonaIdPersona equals personatable.IdPersona
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

            var supervision = await _context.Aer.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            ViewBag.listaCuentaEvaluacion = listaNaSiNo;
            ViewBag.idCuentaEvaluacion = BuscaId(listaNaSiNo, supervision.CuentaEvaluacion);



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
                    _context.Update(aer);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(PersonaSupervision));
            }
            return View(aer);
        }
        #endregion

        #region -EditCambiodeobligaciones-
        public async Task<IActionResult>  EditCambiodeobligaciones(int? id, string nombre, string cp)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;

            var supervision = await _context.Cambiodeobligaciones.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            ViewBag.listaSediocambio = listaNaSiNo;
            ViewBag.idSediocambio = BuscaId(listaNaSiNo, supervision.SeDioCambio);

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
                    _context.Update(cambiodeobligaciones);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(PersonaSupervision));
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

            var supervision = await _context.Cierredecaso.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }


            ViewBag.listaSeCerroCaso = listaNaSiNo;
            ViewBag.idSeCerroCaso = BuscaId(listaNaSiNo, supervision.SeCerroCaso);
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
                    _context.Update(cierredecaso);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(PersonaSupervision));
            }
            return View(cierredecaso);
        }
        #endregion

        #region -Fraccionesimpuestas-
        public IActionResult EditFraccionesimpuestas(int? id, string nombre)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;

            List<Fraccionesimpuestas> fraccionesImpuestas = _context.Fraccionesimpuestas.ToList();

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
                    _context.Update(fraccionesimpuestas);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(PersonaSupervision));
            }
            return View(fraccionesimpuestas);
        }

        public async Task<IActionResult> CrearFracciones(Fraccionesimpuestas fraccionesImpuestas, string[] datosFracciones)
        {
            for (int i=0; i<14;i++)
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
                    await _context.SaveChangesAsync();
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

            var supervision = await _context.Planeacionestrategica.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }


            ViewBag.listaPlanSupervision = listaNaSiNo;
            ViewBag.idPlanSupervision = BuscaId(listaNaSiNo, supervision.PlanSupervision);


            #region Liata de supervision
            List<SelectListItem> ListaPeriodicidadFirma;
            ListaPeriodicidadFirma = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Diaria", Value = "DIARIA" },
                new SelectListItem{ Text = "Semanal", Value = "SEMANAL" },
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
                    _context.Update(planeacionestrategica);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(PersonaSupervision));
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
            var supervision = await _context.Revocacion.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            ViewBag.listaRevocado = listaNaSiNo;
            ViewBag.idRevocado = BuscaId(listaNaSiNo, supervision.Revocado);




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
                    _context.Update(revocacion);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(PersonaSupervision));
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

            var supervision = await _context.Suspensionseguimiento.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            ViewBag.listaSuspendido = listaNaSiNo;
            ViewBag.idSuspendido = BuscaId(listaNaSiNo, supervision.Suspendido);


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
                    _context.Update(suspensionseguimiento);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(PersonaSupervision));
            }
            return View();
        }
        #endregion

        #region -Graficos-
        #endregion

    }
}
