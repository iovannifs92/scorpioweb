using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using SautinSoft.Document;
using SautinSoft.Document.MailMerging;
using scorpioweb.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace scorpioweb.Controllers
{
    [Authorize]
    public class SupervisionesController : Controller
    {
        #region -Constructor-
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public SupervisionesController(penas2Context context, IHostingEnvironment hostingEnvironment, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
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
        private List<SelectListItem> listaSiNo = new List<SelectListItem>
        {
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

        private List<SelectListItem> listaCumplimiento = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Cumplimiento", Value = "CUMPLIMIENTO" },
            new SelectListItem{ Text = "Incumplimiento", Value = "INCUMPLIMIENTO" },
        };

        private List<SelectListItem> listaFiguraJudicial = new List<SelectListItem>
        {
            new SelectListItem{ Text = "MC", Value = "MC" },
            new SelectListItem{ Text = "SCP", Value = "SCP" },
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
            new SelectListItem{ Text = "Cambio de SCP a MC", Value = "CAMBIO DE SCP A MC" }
        };

        public string normaliza(string normalizar)
        {
            if (!String.IsNullOrEmpty(normalizar))
            {
                normalizar = normalizar.ToUpper();
            }
            else
            {
                normalizar = "NA";
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

        #region -guardarSupervision-
        public ActionResult guardarSupervision(string[] datosS)
        {
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosS.Length; i++)
            {
                datosSupervision = new List<String> { datosS[i], currentUser };
            }

            return Json(new { success = true, responseText = "Datos Guardados con éxito,\n Presione Botón para guardar los cambios" });

        }
        #endregion

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
        public async Task<IActionResult> Edit(int? id, string nombre, string cp, string idpersona)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;

            await PermisosEdicion(id);

            var supervision = await _context.Supervision.SingleOrDefaultAsync(m => m.IdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            #region Estado Supervisión
            List<SelectListItem> ListaEstadoS;
            ListaEstadoS = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Concluido", Value = "CONCLUIDO" },
                new SelectListItem{ Text = "Vigente", Value = "VIGENTE" },
                new SelectListItem{ Text = "En espera de respuesta", Value = "EN ESPERA DE RESPUESTA" },
                new SelectListItem{ Text = "Informado Diversa Causa", Value = "INFORMADO DIVERSA CAUSA" },
                new SelectListItem{ Text = "Prisión Preventiva Por Diversa Causa", Value = "PRISIÓN PREVENTIVA POR DIVERSA CAUSA" },
                new SelectListItem{ Text = "Informado Incumplimiento MC", Value = "INFORMADO INCUMPLIMIENTO MC" },
                new SelectListItem{ Text = "Sustraido", Value = "SUSTRAIDO" },
                new SelectListItem{ Text = "Suspendido", Value = "SUSPENDIDO" }
            };

            ViewBag.listaEstadoSupervision = ListaEstadoS;
            ViewBag.idEstadoSupervision = BuscaId(ListaEstadoS, supervision.EstadoSupervision);
            #endregion

            #region Estado Cumplimiento
            List<SelectListItem> ListaEstadoC;
            ListaEstadoC = new List<SelectListItem>
            {
                new SelectListItem{ Text = "", Value = "NA" },
                new SelectListItem{ Text = "Cumpliendo", Value = "CUMPLIENDO" },
                new SelectListItem{ Text = "Cumplimiento Parcial", Value = "CUMPLIMIENTO PARCIAL" },
                new SelectListItem{ Text = "Incumplimiento Total", Value = "INCUMPLIMIENTO TOTAL" },
            };

            ViewBag.listaEstadoCumplimiento = ListaEstadoC;
            ViewBag.idEstadoCumplimiento = BuscaId(ListaEstadoC, supervision.EstadoCumplimiento);
            #endregion

            ViewBag.listaTTA = listaSiNo;
            ViewBag.idTTA = BuscaId(listaSiNo, supervision.Tta);

            return View(supervision);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSupervision,Inicio,Termino,EstadoSupervision,PersonaIdPersona,EstadoCumplimiento,CausaPenalIdCausaPenal, Tta")] Supervision supervision)
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

        public async Task<IActionResult> AddOrEdit(string nombre, string cp, int id, string idpersona)
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
        public async Task<IActionResult> AddOrEdit([Bind("IdFracciones,Tipo,Autoridad,FechaInicio,FechaTermino,Estado,Evidencia,FiguraJudicial,SupervisionIdSupervision")] Fraccionesimpuestas fraccionesimpuestas, string nombre, string cp, string idpersona)
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
                return RedirectToAction("EditFraccionesimpuestas/" + fraccionesimpuestas.SupervisionIdSupervision, "Supervisiones", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona });
            }
            return View(fraccionesimpuestas);
        }


        public async Task<IActionResult> DeleteFraccion(int? id, string nombre, string cp, string idpersona)
        {
            var fraccionesimpuestas = await _context.Fraccionesimpuestas.SingleOrDefaultAsync(m => m.IdFracciones == id);
            var oldfraccionesimpuestas = await _context.Fraccionesimpuestas.FindAsync(fraccionesimpuestas.IdFracciones, fraccionesimpuestas.SupervisionIdSupervision);
            _context.Entry(oldfraccionesimpuestas).CurrentValues.SetValues(fraccionesimpuestas);

            _context.Fraccionesimpuestas.Remove(fraccionesimpuestas);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditFraccionesimpuestas/" + fraccionesimpuestas.SupervisionIdSupervision, "Supervisiones", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona });
        }


        #region -Acciones de supervision-
        public async Task<IActionResult> AddAccionSupervision(string nombre, string cp, int id, string idpersona, string[] datosBitacora, string supervisor, int idcp)
        {
            int index = cp.IndexOf("?");
            if (index >= 0)
                cp = cp.Substring(0, index);


            ViewBag.cp = cp;
            ViewBag.nombre = nombre;
            ViewBag.idpersona = idpersona;
            ViewBag.supervisor = supervisor;
            ViewBag.idcp = idcp;

            if (id == null)
            {
                return NotFound();
            }

            List<Bitacora> bitacora = _context.Bitacora.ToList();
            List<Fraccionesimpuestas> fraccionesImpuestas = _context.Fraccionesimpuestas.ToList();
            List<Supervision> supervision = _context.Supervision.ToList();
            int SupervisionIdSupervision = 0;
            var idsupervision = datosBitacora[0];
            if (idsupervision != null)
            {
                SupervisionIdSupervision = Int32.Parse(idsupervision);
            }

            var snbitacora = await _context.Bitacora.Where(m => m.FracionesImpuestasIdFracionesImpuestas == id).ToListAsync();
            if (snbitacora.Count == 0)
            {
                return RedirectToAction("CreateBitacora2", new { id, SupervisionIdSupervision, @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @supervisor = supervisor, @idcp=idcp });
            }


            #region -Select idOficialia
            List<Bitacora> bitacorasvm = _context.Bitacora.ToList();

            var leftjoin = from o in _context.Oficialia
                           join p in _context.Persona on o.UsuarioTurnar equals p.Supervisor
                           join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona
                           join b in bitacorasvm on o.IdOficialia equals b.OficialiaIdOficialia into temp
                           from bo in temp.DefaultIfEmpty()
                           select new ListaOficialiaBitacoraViewModel
                           {
                               oficialiavm = o,
                               supervisionvm = s,
                               personavm = p,
                               bitacoravm = bo
                           };

            var wheres = (from bn in leftjoin
                          where bn.oficialiavm.UsuarioTurnar == supervisor
                          group bn by bn.oficialiavm.IdOficialia into grp
                          select grp.OrderBy(bn => bn.oficialiavm.IdOficialia).FirstOrDefault()).ToList();


            var selects = (from wh in wheres
                           select wh.oficialiavm.IdOficialia).ToList();

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
                                         where Bitacora.FracionesImpuestasIdFracionesImpuestas == id
                                         select new BitacoraViewModal
                                         {
                                             bitacoraVM = Bitacora
                                         };





            ViewData["tienebitacora"] = from s in supervision
                                        join b in bitacora on s.IdSupervision equals b.SupervisionIdSupervision
                                        join fi in fraccionesImpuestas on b.FracionesImpuestasIdFracionesImpuestas equals fi.IdFracciones
                                        where s.IdSupervision == id
                                        select new BitacoraViewModal
                                        {
                                            bitacoraVM = b,
                                            supervisionVM = s,
                                            fraccionesimpuestasVM = fi
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

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddAccionSupervision([Bind("IdBitacora,Fecha,TipoPersona,Texto,TipoVisita,RutaEvidencia,OficialiaIdOficialia,SupervisionIdSupervision,FracionesImpuestasIdFracionesImpuestas ")] Bitacora bitacora, IFormFile evidencia, string nombre, string cp, string idpersona, string supervisor, string idcp)
        {
            bitacora.Texto = normaliza(bitacora.Texto);
            bitacora.OficialiaIdOficialia = bitacora.OficialiaIdOficialia;

            var supervision = _context.Supervision
               .SingleOrDefault(m => m.IdSupervision == bitacora.SupervisionIdSupervision);

            if (ModelState.IsValid)
            {
                try
                {
                    bitacora.Texto = normaliza(bitacora.Texto);


                    var oldBitacora = await _context.Bitacora.FindAsync(bitacora.IdBitacora, bitacora.SupervisionIdSupervision);

                    if (evidencia == null)
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
                return RedirectToAction("EditFraccionesimpuestas/" + bitacora.SupervisionIdSupervision, "Supervisiones", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @supervisor = supervisor, @idcp = idcp });
            }
            return View();
        }

        public async Task<IActionResult> CrearAccionSuper(Bitacora bitacora, string[] datosBitacora)
        {
            bitacora.SupervisionIdSupervision = Int32.Parse(datosBitacora[0]);
            bitacora.FracionesImpuestasIdFracionesImpuestas = Int32.Parse(datosBitacora[1]);
            bitacora.Fecha = validateDatetime(datosBitacora[2]);
            bitacora.TipoPersona = datosBitacora[3];
            bitacora.Texto = normaliza(datosBitacora[4]);
            bitacora.TipoVisita = datosBitacora[5];
            bitacora.RutaEvidencia = datosBitacora[6];


            var supervision = _context.Supervision
               .SingleOrDefault(m => m.IdSupervision == bitacora.SupervisionIdSupervision);

            _context.Add(bitacora);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditFraccionesimpuestas/" + bitacora.SupervisionIdSupervision, "Supervisiones");
        }
        public async Task<IActionResult> DeleteRegistro2(int? id, string nombre, string cp, string idpersona,string supervisor, string idcp)
        {
            var Bitacora = await _context.Bitacora.SingleOrDefaultAsync(m => m.IdBitacora == id);
            var oldBitacora = await _context.Bitacora.FindAsync(Bitacora.IdBitacora, Bitacora.SupervisionIdSupervision);
            _context.Entry(oldBitacora).CurrentValues.SetValues(Bitacora);


            _context.Bitacora.Remove(Bitacora);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditFraccionesimpuestas/" + Bitacora.SupervisionIdSupervision, "Supervisiones", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, idcp = idcp, @supervisor = supervisor });

        }

        #endregion

        #endregion

        #region -Delete-

        public JsonResult antesdelete(Supervision supervision, Fraccionesimpuestas fraccionesimpuestas, string[] datosuper)
        {
            var borrar = false;
            var id = Int32.Parse(datosuper[0]);

            var antesdel = from s in _context.Supervision
                           join fi in _context.Fraccionesimpuestas on s.IdSupervision equals fi.SupervisionIdSupervision
                           where s.IdSupervision == id
                           select s;

            if (antesdel.Any())
            {
                return Json(new { success = true, responseText = Url.Action("ListadeCausas", "Causaspenales"), borrar = borrar });
            }
            else
            {
                borrar = true;
                return Json(new { success = true, responseText = Url.Action("ListadeCausas", "Causaspenales"), borrar = borrar });
            }
            var stadoc = (from s in _context.Supervision
                          where s.IdSupervision == id
                          select s.IdSupervision).FirstOrDefault();

            return Json(new { success = true, responseText = Convert.ToString(stadoc), idSupervision = Convert.ToString(id) });
        }
        public JsonResult deletesuper(Supervision supervision, Historialeliminacion historialeliminacion, string[] datosuper)
        {
            var borrar = false;
            var id = Int32.Parse(datosuper[0]);
            var razon = normaliza(datosuper[1]);
            var user = normaliza(datosuper[2]);

            var query = (from s in _context.Supervision
                         join p in _context.Persona on s.PersonaIdPersona equals p.IdPersona
                         where s.IdSupervision == id
                         select s).FirstOrDefault();
            var queryP = (from s in _context.Supervision
                         join p in _context.Persona on s.PersonaIdPersona equals p.IdPersona
                         where s.IdSupervision == id
                         select p).FirstOrDefault();

            try
            {
                borrar = true;
                historialeliminacion.Id = id;
                historialeliminacion.Descripcion = "IDPERSONA= "+ query.PersonaIdPersona + " IDCAUSAPENAL= " +query.CausaPenalIdCausaPenal+" IDSUPERVISIÓN= "+query.PersonaIdPersona;
                historialeliminacion.Tipo = "SUPERVISIÓN";
                historialeliminacion.Razon = normaliza(razon);
                historialeliminacion.Usuario = normaliza(user);
                historialeliminacion.Fecha = DateTime.Now;
                historialeliminacion.Supervisor = normaliza(queryP.Supervisor);
                _context.Add(historialeliminacion);
                _context.SaveChanges();

                _context.Database.ExecuteSqlCommand("CALL spBorrarSupervision(" + id + ")");
                return Json(new { success = true, responseText = Url.Action("index", "Personas"), borrar = borrar });

            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("index", "Personas"), borrar = borrar });
            }

            var stadoc = (from c in _context.Causapenal
                          where c.IdCausaPenal == id
                          select c.IdCausaPenal).FirstOrDefault();

            return Json(new { success = true, responseText = Convert.ToString(stadoc), idPersonas = Convert.ToString(id) });
        }




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

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP")
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
            foreach (var rol in roles)
            {
                if (rol == "ArchivoMCSCP")
                {
                    ViewBag.Archivo = true;
                }
            }

            List<Fraccionesimpuestas> fraccionesimpuestasVM = _context.Fraccionesimpuestas.ToList();

            List<Fraccionesimpuestas> queryFracciones = (from f in fraccionesimpuestasVM
                                                         group f by f.SupervisionIdSupervision into grp
                                                         select grp.OrderByDescending(f => f.IdFracciones).FirstOrDefault()).ToList();

            List<Supervision> querySupervisionSinFraccion = (from s in _context.Supervision
                                                             join f in _context.Fraccionesimpuestas on s.IdSupervision equals f.SupervisionIdSupervision into SupervisionFracciones
                                                             from sf in SupervisionFracciones.DefaultIfEmpty()
                                                             select new Supervision
                                                             {
                                                             }).ToList();


            List<Cierredecaso> queryFile = (from c in _context.Cierredecaso
                                            join s in _context.Supervision on c.SupervisionIdSupervision equals s.IdSupervision
                                            select new Cierredecaso { }).ToList();


            if (queryFile != null)
            {
                ViewBag.FileCierre = queryFile;
            }


            #region Estado Supervisión
            List<SelectListItem> ListaEstadoS;
            ListaEstadoS = new List<SelectListItem>
            {
                new SelectListItem{ Text = "", Value = "" },
                new SelectListItem{ Text = "Concluido", Value = "CONCLUIDO" },
                new SelectListItem{ Text = "Vigente", Value = "VIGENTE" },
                new SelectListItem{ Text = "En espera de respuesta", Value = "EN ESPERA DE RESPUESTA" },
                new SelectListItem{ Text = "Informado Diversa Causa", Value = "INFORMADO DIVERSA CAUSA" },
                new SelectListItem{ Text = "Prisión Preventiva Por Diversa Causa", Value = "PRISIÓN PREVENTIVA POR DIVERSA CAUSA" },
                new SelectListItem{ Text = "Informado Incumplimiento MC", Value = "INFORMADO INCUMPLIMIENTO MC" },
                new SelectListItem{ Text = "Sustraido", Value = "SUSTRAIDO" },
                new SelectListItem{ Text = "Suspendido", Value = "SUSPENDIDO" }
            };
            ViewBag.listaEstadoSupervision = ListaEstadoS;

            List<SelectListItem> ListaFiltroEstadoS;
            ListaFiltroEstadoS = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Todos", Value = "TODOS" },
                new SelectListItem{ Text = "Concluido", Value = "CONCLUIDO" },
                new SelectListItem{ Text = "Vigente", Value = "VIGENTE" },
                new SelectListItem{ Text = "En espera de respuesta", Value = "EN ESPERA DE RESPUESTA" },
                new SelectListItem{ Text = "Informado Diversa Causa", Value = "INFORMADO DIVERSA CAUSA" },
                new SelectListItem{ Text = "Prisión Preventiva Por Diversa Causa", Value = "PRISIÓN PREVENTIVA POR DIVERSA CAUSA" },
                new SelectListItem{ Text = "Informado Incumplimiento MC", Value = "INFORMADO INCUMPLIMIENTO MC" },
                new SelectListItem{ Text = "Sustraido", Value = "SUSTRAIDO" },
                new SelectListItem{ Text = "Suspendido", Value = "SUSPENDIDO" }
            };
            ViewBag.listaFiltroEstadoSupervision = ListaFiltroEstadoS;
            #endregion

            List<SelectListItem> listaFiguraJ = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Todos", Value = "TODOS" },
                new SelectListItem{ Text = "Sin Figura Judicial", Value = "SIN FIGURA JUDICIAL" },
                new SelectListItem{ Text = "MC", Value = "MC" },
                new SelectListItem{ Text = "SCP", Value = "SCP" },
            };
            ViewBag.listaFiguraJudicial = listaFiguraJ;

            var filter = from p in _context.Persona
                         join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona
                         join cp in _context.Causapenal on s.CausaPenalIdCausaPenal equals cp.IdCausaPenal
                         join pe in _context.Planeacionestrategica on s.IdSupervision equals pe.SupervisionIdSupervision
                         join c in _context.Cierredecaso on s.IdSupervision equals c.SupervisionIdSupervision
                         join fracciones in queryFracciones on s.IdSupervision equals fracciones.SupervisionIdSupervision
                         into PersonaSupervisionCausaPenal
                         from fraccion in PersonaSupervisionCausaPenal.DefaultIfEmpty()
                         select new SupervisionPyCP
                         {
                             cierredecasoVM = c,
                             personaVM = p, 
                             supervisionVM = s,
                             causapenalVM = cp,
                             planeacionestrategicaVM = pe,
                             fraccionesimpuestasVM = fraccion,
                             tiempoSupervision = (s.Termino != null && s.Inicio != null) ? ((int)(s.Termino - s.Inicio).Value.TotalDays) : 0
                         };

            if (supervisor == false)
            {
                filter = from p in _context.Persona
                         join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona
                         join cp in _context.Causapenal on s.CausaPenalIdCausaPenal equals cp.IdCausaPenal
                         join pe in _context.Planeacionestrategica on s.IdSupervision equals pe.SupervisionIdSupervision
                         join c in _context.Cierredecaso on s.IdSupervision equals c.SupervisionIdSupervision
                         join fracciones in queryFracciones on s.IdSupervision equals fracciones.SupervisionIdSupervision
                         into PersonaSupervisionCausaPenal
                         from fraccion in PersonaSupervisionCausaPenal.DefaultIfEmpty()
                         where p.Supervisor == User.Identity.Name
                         select new SupervisionPyCP
                         {
                             cierredecasoVM = c,
                             personaVM = p,
                             supervisionVM = s,
                             causapenalVM = cp,
                             planeacionestrategicaVM = pe,
                             fraccionesimpuestasVM = fraccion,
                             tiempoSupervision = (s.Termino != null && s.Inicio != null) ? ((int)(s.Termino - s.Inicio).Value.TotalDays) : 0
                         };
            }


            ViewData["CurrentFilter"] = searchString;
            ViewData["EstadoS"] = estadoSuper;
            ViewData["FiguraJ"] = figuraJudicial;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = filter.Where(spcp => (spcp.personaVM.Paterno + " " + spcp.personaVM.Materno + " " + spcp.personaVM.Nombre).Contains(searchString) ||
                                              (spcp.personaVM.Nombre + " " + spcp.personaVM.Paterno + " " + spcp.personaVM.Materno).Contains(searchString) ||
                                              spcp.supervisionVM.EstadoSupervision.Contains(searchString) ||
                                              spcp.causapenalVM.CausaPenal.Contains(searchString) ||
                                              spcp.personaVM.Supervisor.Contains(searchString) ||
                                              (spcp.personaVM.IdPersona.ToString()).Contains(searchString)
                                              );
            }

            if (estadoSuper != null && estadoSuper != "Todos")
            {
                filter = filter.Where(spcp => spcp.supervisionVM.EstadoSupervision == estadoSuper);
            }

            if (figuraJudicial != null && figuraJudicial != "Todos")
            {
                if (figuraJudicial == "Sin Figura Judicial")
                {
                    filter = filter.Where(spcp => spcp.fraccionesimpuestasVM.FiguraJudicial == null);
                }
                else
                {
                    filter = filter.Where(spcp => spcp.fraccionesimpuestasVM.FiguraJudicial == figuraJudicial);
                }
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
            //Vigente al principio, Concluido al final
            filter = filter.OrderByDescending(spcp => spcp.supervisionVM.EstadoSupervision);

            //var personas = _context.Persona
            //    .FromSql("CALL informeSemanal")
            //    .ToList();


            int pageSize = 10;
            return View(await PaginatedList<SupervisionPyCP>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        #region -Update Persona supervision-
        public JsonResult UpdatePersonasupervision(Supervision supervision, Planeacionestrategica planeacionestrategica, string superid, string campo, string planeacionid, string estados, DateTime fecha, DateTime intermedio)
        {

            #region -Actualizar fechas en supervision-
            if (superid != null)
            {
                supervision.Termino = fecha;
                supervision.Inicio = fecha;
                var camps = campo;
                supervision.IdSupervision = Int32.Parse(superid);
                supervision.EstadoSupervision = normaliza(estados);
            }

            var empty = (from s in _context.Supervision
                         where s.IdSupervision == supervision.IdSupervision
                         select s);

            if (empty.Any())
            {
                if (campo == "Inicio")
                {
                    var query = (from s in _context.Supervision
                                 where s.IdSupervision == supervision.IdSupervision
                                 select s).FirstOrDefault();
                    query.Inicio = supervision.Inicio;
                    _context.SaveChanges();
                }
                if (campo == "Termino")
                {
                    var query = (from s in _context.Supervision
                                 where s.IdSupervision == supervision.IdSupervision
                                 select s).FirstOrDefault();
                    query.Termino = supervision.Termino;
                    _context.SaveChanges();
                }
            }
            #endregion

            #region -actualizacion de fecha en planeacion estrategica-
            if (planeacionid != null)
            {
                planeacionestrategica.FechaInforme = intermedio;
                planeacionestrategica.IdPlaneacionEstrategica = Int32.Parse(planeacionid);

            }

            var emptype = (from pe in _context.Planeacionestrategica
                           where pe.IdPlaneacionEstrategica == planeacionestrategica.IdPlaneacionEstrategica
                           select pe);
            if (emptype.Any())
            {
                var query = (from pe in _context.Planeacionestrategica
                             where pe.IdPlaneacionEstrategica == planeacionestrategica.IdPlaneacionEstrategica
                             select pe).FirstOrDefault();
                query.FechaInforme = planeacionestrategica.FechaInforme;
                _context.SaveChanges();
            }
            #endregion

            #region -actualizacion de estado de supervision-
            var empty2 = (from s in _context.Supervision
                          where s.IdSupervision == supervision.IdSupervision
                          select s);
            if (empty2.Any())
            {
                var query = (from s in _context.Supervision
                             where s.IdSupervision == supervision.IdSupervision
                             select s).FirstOrDefault();
                query.EstadoSupervision = supervision.EstadoSupervision;
                _context.SaveChanges();
            }

            #endregion
            var cp = (from s in _context.Supervision
                      where s.IdSupervision == supervision.IdSupervision
                      select s.Inicio).FirstOrDefault();


            return Json(new { success = true, responseText = Convert.ToString(cp), idPersonas = Convert.ToString(supervision.IdSupervision) });
        }
        #endregion -Update Update Persona supervision-
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EdicionMenuSuper(Supervision supervision, Planeacionestrategica planeacionestrategica)
        {
            int idSuper = supervision.IdSupervision;
            int idPlaneacion = planeacionestrategica.IdPlaneacionEstrategica;
            var fInicio = supervision.Inicio;
            var fTermino = supervision.Termino;
            var fInforme = planeacionestrategica.FechaInforme;
            string estadoS = supervision.EstadoSupervision;


            supervision.Inicio = fInicio;
            supervision.Termino = fTermino;
            supervision.EstadoSupervision = estadoS;
            planeacionestrategica.FechaInforme = fInforme;

            var fInicioUpdate = (from a in _context.Supervision
                                 where a.IdSupervision == idSuper
                                 select a).FirstOrDefault();
            fInicioUpdate.Inicio = fInicio;
            _context.SaveChanges();

            var fTerminoUpdate = (from a in _context.Supervision
                                  where a.IdSupervision == idSuper
                                  select a).FirstOrDefault();
            fTerminoUpdate.Termino = fTermino;
            _context.SaveChanges();


            var estadoSUpdate = (from a in _context.Supervision
                                 where a.IdSupervision == idSuper
                                 select a).FirstOrDefault();
            estadoSUpdate.EstadoSupervision = estadoS;
            _context.SaveChanges();


            var fInformeUpdate = (from a in _context.Planeacionestrategica
                                  where a.IdPlaneacionEstrategica == idPlaneacion
                                  select a).FirstOrDefault();
            fInformeUpdate.FechaInforme = fInforme;
            _context.SaveChanges();


            try
            {
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
            return RedirectToAction("PersonaSupervision");
        }


        #endregion

        #region -Aer-
        public async Task<IActionResult> EditAer(int? id, string nombre, string cp, string idpersona)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;

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
                    aer.EvaluadorCaso = normaliza(aer.EvaluadorCaso);

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
        public async Task<IActionResult> EditCambiodeobligaciones(int? id, string nombre, string cp, string idpersona, string cambio)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;

            await PermisosEdicion(id);

            var supervision = await _context.Cambiodeobligaciones.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (supervision == null)
            {
                return NotFound();
            }

            ViewBag.listaSediocambio = listaNaSiNo;
            ViewBag.idSediocambio = BuscaId(listaNaSiNo, supervision.SeDioCambio);
            ViewBag.cambio = supervision.SeDioCambio;

            ViewBag.listaMotivoAprobacion = listaMotivoAprobacion;
            ViewBag.idMotivoAprobacion = BuscaId(listaMotivoAprobacion, supervision.MotivoAprobacion);

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
                    cambiodeobligaciones.MotivoAprobacion = normaliza(cambiodeobligaciones.MotivoAprobacion);
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

            var cierre = await _context.Cierredecaso.SingleOrDefaultAsync(m => m.SupervisionIdSupervision == id);
            if (cierre == null)
            {
                return NotFound();
            }

            ViewBag.CierreCaso = listaCierreCaso;
            ViewBag.idCierreCaso = BuscaId(listaCierreCaso, cierre.ComoConcluyo);
            ViewBag.listaSeCerroCaso = listaNaSiNo;
            ViewBag.idSeCerroCaso = BuscaId(listaNaSiNo, cierre.SeCerroCaso);
            ViewBag.cierre = cierre.SeCerroCaso;
            #region Autorizo
            List<SelectListItem> ListaAutorizo;
            ListaAutorizo = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Director", Value = "DIRECTOR" },
                new SelectListItem{ Text = "Coordinador", Value = "COORDINADOR" }
                };

            ViewBag.listaAutorizo = ListaAutorizo;
            ViewBag.idAutorizo = BuscaId(ListaAutorizo, cierre.Autorizo);
            #endregion

            ViewBag.Achivocierre = cierre.RutaArchivo;



            return View(cierre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCierredecaso(int id, [Bind("IdCierreDeCaso,SeCerroCaso,ComoConcluyo,NoArchivo,FechaAprobacion,Autorizo,RuataArchivo,SupervisionIdSupervision")] Cierredecaso cierredecaso, IFormFile archivo)
        {

            var supervision = _context.Supervision
               .SingleOrDefault(m => m.IdSupervision == cierredecaso.SupervisionIdSupervision);

            if (id != cierredecaso.SupervisionIdSupervision)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {

                    cierredecaso.ComoConcluyo = normaliza(cierredecaso.ComoConcluyo);
                    var oldcierredecaso = await _context.Cierredecaso.FindAsync(cierredecaso.IdCierreDeCaso, cierredecaso.SupervisionIdSupervision);

                    if (archivo == null)
                    {
                        cierredecaso.RutaArchivo = oldcierredecaso.RutaArchivo;
                    }
                    else
                    {
                        string file_name = cierredecaso.IdCierreDeCaso + "_" + cierredecaso.SupervisionIdSupervision + "_" + supervision.PersonaIdPersona + Path.GetExtension(archivo.FileName);
                        cierredecaso.RutaArchivo = file_name;
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Cierredecaso");

                        if (System.IO.File.Exists(Path.Combine(uploads, file_name)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, file_name));
                        }

                        var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                        await archivo.CopyToAsync(stream);
                        stream.Close();
                    }

                    _context.Entry(oldcierredecaso).CurrentValues.SetValues(cierredecaso);
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
        public async Task<IActionResult> EditFraccionesimpuestas(int? id, string nombre, string cp, string idpersona, string supervisor, int idcp)
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
            var snbitacora = await _context.Bitacora.Where(m => m.SupervisionIdSupervision == id).ToListAsync();

            List<Fraccionesimpuestas> fraccionesImpuestas = _context.Fraccionesimpuestas.ToList();
            //List<Bitacora> bitacora = _context.Bitacora.ToList();
            List<Supervision> supervision = _context.Supervision.ToList();
            List<Persona> personas = _context.Persona.ToList();

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
                    fraccionesimpuestas.Autoridad = normaliza(fraccionesimpuestas.Autoridad);

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
                return RedirectToAction("EditFraccionesimpuestas/" + fraccionesimpuestas.SupervisionIdSupervision, "Supervisiones");
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
        public async Task<IActionResult> EditPlaneacionestrategica(int? id, string nombre, string cp, string idpersona)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;

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
                new SelectListItem{ Text = "", Value = "" },
                new SelectListItem{ Text = "Diaria", Value = "DIARIA" },
                new SelectListItem{ Text = "Semanal", Value = "SEMANAL" },
                new SelectListItem{ Text = "Quincenal", Value = "QUINCENAL" },
                new SelectListItem{ Text = "Mensual", Value = "MENSUAL" },
                new SelectListItem{ Text = "Bimestral", Value = "BIMESTRAL" },
                new SelectListItem{ Text = "Trimestral", Value = "TRIMESTRAL" },
                new SelectListItem{ Text = "Semestral", Value = "SEMESTRAL" },
                new SelectListItem{ Text = "Anual", Value = "ANUAL", },
                new SelectListItem{ Text = "No aplica", Value = "NO APLICA"}
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
                    planeacionestrategica.MotivoNoPlaneacion = normaliza(planeacionestrategica.MotivoNoPlaneacion);

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
                    revocacion.MotivoRevocacion = normaliza(revocacion.MotivoRevocacion);
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
                    suspensionseguimiento.MotivoSuspension = normaliza(suspensionseguimiento.MotivoSuspension);
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
        public async Task<IActionResult> ListaVictima(int? id, string cp, string nombre, string idpersona )
        {

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;


            var supervision = _context.Supervision
            .SingleOrDefault(m => m.IdSupervision == id);

            var persona = _context.Persona
           .SingleOrDefault(m => m.IdPersona == supervision.PersonaIdPersona);
            var cpp = _context.Causapenal
           .SingleOrDefault(m => m.IdCausaPenal == supervision.CausaPenalIdCausaPenal);

      

            await PermisosEdicion(id);


            List<Victima> victimas = _context.Victima.ToList();

            ViewData["Victima"] = from table in victimas
                                  where table.SupervisionIdSupervision == id
                                  orderby table.IdVictima
                                  select table;

            ViewBag.IdSupervisionGuardar = id;


            return View();
        }
        #endregion


        #region -Create Victima-
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
        public async Task<IActionResult> CreateVictima(Victima victima, string IdVictima, string NombreV, string Edad, string Telefono, string ConoceDetenido, string TipoRelacion,
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
                var cpp = _context.Causapenal
               .SingleOrDefault(m => m.IdCausaPenal == supervision.CausaPenalIdCausaPenal);

                //ViewBag.Npersona = persona.NombreCompleto;
                //ViewBag.cp = cpp.CausaPenal;

                int idVictima = ((from table in _context.Victima
                                  select table.IdVictima).Max()) + 1;

                victima.IdVictima = idVictima;
                _context.Add(victima);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListaVictima/" + victima.SupervisionIdSupervision, "Supervisiones", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona });
                //return RedirectToAction("ListaVictima/" + victima.SupervisionIdSupervision, "Supervisiones");
            }
            return View(victima);
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

            var Victima = await _context.Victima.SingleOrDefaultAsync(m => m.IdVictima == id);
            if (Victima == null)
            {
                return NotFound();
            }

            #endregion

            #region ConoceDetenido
            List<SelectListItem> ListaConoceDetenido;
            ListaConoceDetenido = new List<SelectListItem>
            {
              new SelectListItem{ Text="NA", Value="NA"},
              new SelectListItem{ Text="Si", Value="SI"},
              new SelectListItem{ Text="No", Value="NO"},
            };
            ViewBag.ConoceDetenido = ListaConoceDetenido;
            ViewBag.idConoceDetenido = BuscaId(ListaConoceDetenido, Victima.ConoceDetenido);
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
        public async Task<IActionResult> Editvictima(int id, [Bind("IdVictima,NombreV,Edad,Telefono,ConoceDetenido,TipoRelacion,TiempoConocerlo,ViveSupervisado,Direccion,Victimacol,SupervisionIdSupervision, Observaciones")] Victima victima, string nombre, string cp, string idpersona)
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
                return RedirectToAction("ListaVictima/" + victima.SupervisionIdSupervision, "Supervisiones", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona });
            }
            return View();
        }
        public async Task<IActionResult> DeleteVictima(int? id, string nombre, string cp, string idpersona)
        {
            var Victima = await _context.Victima.SingleOrDefaultAsync(m => m.IdVictima == id);
            _context.Victima.Remove(Victima);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListaVictima/" + Victima.SupervisionIdSupervision, "Supervisiones", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona });

        }
        public async Task<IActionResult> VerVictima(int? id, string nombre, string cp, string idpersona)
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

        #region -Bitacora-
        public async Task<IActionResult> ListaBitacora(int? id, string nombre, string cp, string idpersona, string idcp, string supervisor)
        {
            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;
            ViewBag.supervisor = supervisor;
            ViewBag.idcp = idcp;




            var supervision = _context.Supervision
            .SingleOrDefault(m => m.IdSupervision == id);

            var persona = _context.Persona
           .SingleOrDefault(m => m.IdPersona == supervision.PersonaIdPersona);
            var cpp = _context.Causapenal
           .SingleOrDefault(m => m.IdCausaPenal == supervision.CausaPenalIdCausaPenal);
            //ViewBag.nombre = persona.NombreCompleto;
            //ViewBag.cp = cp.CausaPenal;

            //var oficialia = (from _cp in _context.Causapenal
            //                 join o in _context.Oficialia on _cp.IdCausaPenal equals o.IdCausaPenal
            //                 join b in _context.Bitacora on o.IdOficialia equals b.IdOficialia
            //                 where _cp.IdCausaPenal == cpp.IdCausaPenal);

            await PermisosEdicion(id);


            List<Bitacora> bitacora = _context.Bitacora.ToList();
            List<Fraccionesimpuestas> fraccionesimpuestas = _context.Fraccionesimpuestas.ToList();

            ViewData["BitacoraFracciones"] = from b in bitacora
                                             join fi in fraccionesimpuestas on b.FracionesImpuestasIdFracionesImpuestas equals fi.IdFracciones into tmp
                                             from fleft in tmp.DefaultIfEmpty()
                                             where b.SupervisionIdSupervision == id
                                             orderby b.Fecha descending
                                             select new BitacoraViewModal
                                             {
                                                 bitacoraVM = b,
                                                 fraccionesimpuestasVM = fleft
                                             };

            ViewBag.IdSupervisionGuardar = id;


            return View();
        }
        #region -Create Bitacora-
        public IActionResult CreateBitacora(string nombre, string cp, int id,string supervisor, int idcp, int idpersona,  string[] datosBitacora)
        {
            int index = cp.IndexOf("?");
            if (index >= 0)
                cp = cp.Substring(0, index);

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;
            ViewBag.idcp = idcp;
            ViewBag.supervisor = supervisor;
            #region -Select idOficialia

            List<Bitacora> bitacorasvm = _context.Bitacora.ToList();

            var leftjoin = from o in _context.Oficialia
                           join p in _context.Persona on o.UsuarioTurnar equals p.Supervisor
                           join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona
                           join b in bitacorasvm on o.IdOficialia equals b.OficialiaIdOficialia into temp
                           from bo in temp.DefaultIfEmpty()
                           select new ListaOficialiaBitacoraViewModel
                           {
                               oficialiavm = o,
                               supervisionvm = s,                      
                               personavm = p,
                               bitacoravm = bo
                           };
            var wheres = (from bn in leftjoin
                        where bn.oficialiavm.UsuarioTurnar == supervisor && bn.bitacoravm == null
                        group bn by bn.oficialiavm.IdOficialia into grp
                        select grp.OrderBy(bn => bn.oficialiavm.IdOficialia).FirstOrDefault()).ToList();

            var select = (from wh in wheres
                         select wh.oficialiavm.IdOficialia).ToList();

            ViewBag.expoficialia = select;
            #endregion

            ViewBag.IdSupervisionGuardar = id;
            var idfraciones = datosBitacora[0];

            if (idfraciones != null)
            {
                ViewBag.idFracciones = Int32.Parse(idfraciones);
            }

            return View();
        }
        public IActionResult CreateBitacora2(string nombre, string cp, int id, int SupervisionIdSupervision, string idpersona, string supervisor, int idcp)
        {
            int index = cp.IndexOf("?");
            if (index >= 0)
                cp = cp.Substring(0, index);

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;
            ViewBag.supervisor = supervisor;
            ViewBag.idcp = idcp;

            ViewBag.FracionesImpuestasIdFracionesImpuestas = id;
            ViewBag.SupervisionIdSupervision = SupervisionIdSupervision;

            #region -Select idOficialia

            List<Bitacora> bitacorasvm = _context.Bitacora.ToList();

            var leftjoin = from o in _context.Oficialia
                           join p in _context.Persona on o.UsuarioTurnar equals p.Supervisor
                           join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona
                           join b in bitacorasvm on o.IdOficialia equals b.OficialiaIdOficialia into temp
                           from bo in temp.DefaultIfEmpty()
                           select new ListaOficialiaBitacoraViewModel
                           {
                               oficialiavm = o,
                               supervisionvm = s,
                               personavm = p,
                               bitacoravm = bo
                           };
            var wheres = (from bn in leftjoin
                          where bn.oficialiavm.UsuarioTurnar == supervisor && bn.bitacoravm == null
                          group bn by bn.oficialiavm.IdOficialia into grp
                          select grp.OrderBy(bn => bn.oficialiavm.IdOficialia).FirstOrDefault()).ToList();

            var select = (from wh in wheres
                          select wh.oficialiavm.IdOficialia).ToList();

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
                id=int.Parse(numero);
            }
            catch (Exception e)
            {

            }

            return id;
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBitacora(Bitacora bitacora, string IdBitacora, DateTime Fecha, string tipoPersona, string idoficialia,
            string tipoVisita, string Texto, string SupervisionIdSupervision, string FracionesImpuestasIdFracionesImpuestas, IFormFile evidencia, string nombre, string cp, string idpersona, string idOficialia, string supervisor, string idcp)
        {
            string currentUser = User.Identity.Name;
            if (ModelState.ErrorCount <= 1)
            {
                if (FracionesImpuestasIdFracionesImpuestas != null)
                {
                    bitacora.FracionesImpuestasIdFracionesImpuestas = Int32.Parse(FracionesImpuestasIdFracionesImpuestas);
                }
                bitacora.Fecha = Fecha;
                bitacora.TipoPersona = normaliza(tipoPersona);
                bitacora.TipoVisita = normaliza(tipoVisita);
                bitacora.Texto = normaliza(Texto);
                bitacora.OficialiaIdOficialia = idOficialia != null ? siNumero(idOficialia) : 0;
                bitacora.FechaRegistro = DateTime.Now;

                var supervision = _context.Supervision
               .SingleOrDefault(m => m.IdSupervision == bitacora.SupervisionIdSupervision);

                _context.Add(bitacora);
                await _context.SaveChangesAsync();

                bitacora = await _context.Bitacora.OrderByDescending(b => b.IdBitacora).FirstOrDefaultAsync();

                #region -Guardar archivo-
                if (evidencia != null)
                {
                    string file_name = bitacora.IdBitacora + "" + bitacora.SupervisionIdSupervision + "" + supervision.PersonaIdPersona + Path.GetExtension(evidencia.FileName);
                    bitacora.RutaEvidencia = file_name;
                    var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Evidencia");
                    var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                    await evidencia.CopyToAsync(stream);
                    stream.Close();
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                }
                #endregion

                if (bitacora.FracionesImpuestasIdFracionesImpuestas != null)
                {
                    return RedirectToAction("EditFraccionesimpuestas/" + bitacora.SupervisionIdSupervision, "Supervisiones", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @supervisor = supervisor, @idcp = idcp });
                }
                else
                {
                    return RedirectToAction("ListaBitacora/" + bitacora.SupervisionIdSupervision, "Supervisiones", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @supervisor = supervisor, @idcp = idcp });
                }
            }
            return View(bitacora);
        }

        #endregion
        public async Task<IActionResult> EditBitacora(int id, string nombre, string cp, int idpersona, string supervisor, int idcp)
        {
            int index = cp.IndexOf("?");
            if (index >= 0)
                cp = cp.Substring(0, index);

            ViewBag.nombre = nombre;
            ViewBag.cp = cp;
            ViewBag.idpersona = idpersona;
            ViewBag.supervisor = supervisor;
            ViewBag.idcp = idcp;

            if (id == null)
            {
                return NotFound();
            }
            var bitacora = await _context.Bitacora.SingleOrDefaultAsync(m => m.IdBitacora == id);


            #region -Select idOficialia

            List<Bitacora> bitacorasvm = _context.Bitacora.ToList();

            var leftjoin = from o in _context.Oficialia
                           join p in _context.Persona on o.UsuarioTurnar equals p.Supervisor
                           join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona
                           join b in bitacorasvm on o.IdOficialia equals b.OficialiaIdOficialia into temp
                           from bo in temp.DefaultIfEmpty()
                           select new ListaOficialiaBitacoraViewModel
                           {
                               oficialiavm = o,
                               supervisionvm = s,
                               personavm = p,
                               bitacoravm = bo
                           };
            var wheres = (from bn in leftjoin
                          where bn.oficialiavm.UsuarioTurnar == supervisor 
                          group bn by bn.oficialiavm.IdOficialia into grp
                          select grp.OrderBy(bn => bn.oficialiavm.IdOficialia).FirstOrDefault()).ToList();

            var selects = (from wh in wheres
                          select wh.oficialiavm.IdOficialia).ToList();

            List<SelectListItem> ListaOficios = new List<SelectListItem>();
            ListaOficios = new List<SelectListItem>
            {
              new SelectListItem{ Text="NA", Value="0"},
            };
            foreach (var select in selects)
            {
                ListaOficios.Add(
                 new SelectListItem{Text = select.ToString(),Value = select.ToString()}
                );
                
            }
            ViewBag.expoficialia = ListaOficios;
            ViewBag.idexpoficialia = BuscaId(ListaOficios, bitacora.OficialiaIdOficialia.ToString());
            #endregion



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
              new SelectListItem{ Text="Informe de Supervisión", Value="INFORME"},
              new SelectListItem{ Text="Correo Electrónico", Value="CORREO ELECTRONICO"},
              new SelectListItem{ Text="Citatorio", Value="CITATORIO"},
              new SelectListItem{ Text="Visita Domiciliar", Value="VISITA DOMICILIAR"},
              new SelectListItem{ Text="Notificación a Víctima", Value="NOTIFICACION A VICTIMA"},
            };
            ViewBag.TipoVisita = ListaTipoVisita;
            ViewBag.idTipoVisita = BuscaId(ListaTipoVisita, bitacora.TipoVisita);
            #endregion

            ViewBag.RutaEvidencia = bitacora.RutaEvidencia;

            return View(bitacora);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBitacora([Bind("IdBitacora,Fecha,TipoPersona,Texto,TipoVisita,RutaEvidencia,OficialiaIdOficialia,SupervisionIdSupervision")] Bitacora bitacora, IFormFile evidencia, string nombre, string cp, string idpersona, string supervisor, string idcp)
        {
            bitacora.Texto = normaliza(bitacora.Texto);
            bitacora.OficialiaIdOficialia = bitacora.OficialiaIdOficialia;

            var supervision = _context.Supervision
               .SingleOrDefault(m => m.IdSupervision == bitacora.SupervisionIdSupervision);

            if (ModelState.IsValid)
            {
                try
                {
                    var oldBitacora = await _context.Bitacora.FindAsync(bitacora.IdBitacora, bitacora.SupervisionIdSupervision);

                    if (evidencia == null)
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
                return RedirectToAction("ListaBitacora/" + bitacora.SupervisionIdSupervision, "Supervisiones", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona = idpersona, @idcp = idcp, @supervisor = supervisor });
                
            }
            return View(bitacora);
        }
        public async Task<IActionResult> DeleteRegistro(int? id, string nombre, string cp, string idpersona, string idcp, string supervisor)
        {
            var Bitacora = await _context.Bitacora.SingleOrDefaultAsync(m => m.IdBitacora == id);
            var oldBitacora = await _context.Bitacora.FindAsync(Bitacora.IdBitacora, Bitacora.SupervisionIdSupervision);
            _context.Entry(oldBitacora).CurrentValues.SetValues(Bitacora);

            _context.Bitacora.Remove(Bitacora);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListaBitacora/" + Bitacora.SupervisionIdSupervision, "Supervisiones", new { @nombre = Regex.Replace(nombre.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", ""), @cp = cp, @idpersona= idpersona, @idcp=idcp, @supervisor = supervisor });
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

        #region -Obtener Nombre Supervisor-
        public static string nombreSupervisor(string idSistema)
        {
            string nombre = "";

            switch (idSistema)
            {
                case "ana.valles@dgepms.com":
                    nombre = "LIC. ANA ISABEL VALLES FLORES";
                    break;
                case "carmen.gonzalez@dgepms.com":
                    nombre = "OF. MARÍA DEL CARMEN GONZALEZ";
                    break;
                case "LIC. MAYRA YUDITH GONZÁLEZ MARTÍNEZ":
                    nombre = "";
                    break;
                case "yoshman.mendez@dgepms.com":
                    nombre = "LIC. YOSHMAN MENDEZ HERNANDEZ";
                    break;
                case "isabel.almora@dgepms.com":
                    nombre = "LIC. ISABEL ALMORA DE LA CRUZ";
                    break;
                case "alejandra.jimenez@dgepms.com":
                    nombre = "LIC. ALEJANDRA JIMÉNEZ GALLEGOS";
                    break;
                case "amor.davalos@dgepms.com":
                    nombre = "LIC. AMOR DÁVALOS NÁJERA";
                    break;
                case "iovannifs92@gmail.com":
                    nombre = "IOVANNI FERNÁNDEZ SÁNCHEZ";
                    break;
                default:
                    nombre = "SIN REGISTRO";
                    break;
            }

            return nombre;
        }
        #endregion

        #region -Imprimir Reporte Supervision-
        public void imprimirReporteSupervision(string[] datosidFraccion)
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
            #region -F7-
            string No7 = string.Empty;
            string TextoFraccion7 = string.Empty;
            string Estatus7 = string.Empty;
            string Actividades7 = string.Empty;
            #endregion
            #region -F8-
            string No8 = string.Empty;
            string TextoFraccion8 = string.Empty;
            string Estatus8 = string.Empty;
            string Actividades8 = string.Empty;
            #endregion
            #region -F9-
            string No9 = string.Empty;
            string TextoFraccion9 = string.Empty;
            string Estatus9 = string.Empty;
            string Actividades9 = string.Empty;
            #endregion
            #region -F10-
            string No10 = string.Empty;
            string TextoFraccion10 = string.Empty;
            string Estatus10 = string.Empty;
            string Actividades10 = string.Empty;
            #endregion
            #region -F11-
            string No11 = string.Empty;
            string TextoFraccion11 = string.Empty;
            string Estatus11 = string.Empty;
            string Actividades11 = string.Empty;
            #endregion
            #region -F12-
            string No12 = string.Empty;
            string TextoFraccion12 = string.Empty;
            string Estatus12 = string.Empty;
            string Actividades12 = string.Empty;
            #endregion
            #region -F13-
            string No13 = string.Empty;
            string TextoFraccion13 = string.Empty;
            string Estatus13 = string.Empty;
            string Actividades13 = string.Empty;
            #endregion
            #region -F14-
            string No14 = string.Empty;
            string TextoFraccion14 = string.Empty;
            string Estatus14 = string.Empty;
            string Actividades14 = string.Empty;
            #endregion
            #endregion

            #region -Consultas y llenado de variables temporales-
            int idSupervision = (from table in _context.Fraccionesimpuestas
                                where table.IdFracciones == (Convert.ToInt32(datosidFraccion[datosidFraccion.Length - 1]))
                                select table.SupervisionIdSupervision).FirstOrDefault(); //Obtener IdSupervision

            var tipo = from table in _context.Fraccionesimpuestas
                       where table.IdFracciones == (Convert.ToInt32(datosidFraccion[datosidFraccion.Length - 1]))
                       select new
                       {
                           FechaImposicion=table.FechaInicio,
                           FiguraJudicial=table.FiguraJudicial
                       };

            int idCP = (from table in _context.Supervision
                        where table.IdSupervision == idSupervision
                        select table.CausaPenalIdCausaPenal).FirstOrDefault();

            int idPersona = (from table in _context.Supervision
                            where table.IdSupervision == idSupervision
                            select table.PersonaIdPersona).FirstOrDefault();

            var persona = from table in _context.Persona
                         where table.IdPersona == idPersona
                         select new
                         {
                             Paterno=table.Paterno,
                             Materno=table.Materno,
                             Nombre=table.Nombre,
                             Supervisor=table.Supervisor
                         };

            var causapenal = from table in _context.Causapenal
                              where table.IdCausaPenal == idCP
                              select new
                              {
                                  CausaPenal= table.CausaPenal,
                                  Juez=table.Juez,
                                  Distrito=table.Distrito
                              };

            var delitos = from table in _context.Delito
                          where table.CausaPenalIdCausaPenal == idCP
                          select new
                          {
                              Delito = table.Tipo
                          };

            var presentacion = from registro in _context.Registrohuella
                                 join p in _context.Presentacionperiodica on registro.IdregistroHuella equals p.RegistroidHuella
                                 where registro.PersonaIdPersona == idPersona
                                 select new
                                 {
                                     fechaFirma = p.FechaFirma
                                 };


            string inicio = "";

            try
            {
                inicio = ((from table in _context.Supervision
                           where table.IdSupervision == idSupervision
                           select table.Inicio).FirstOrDefault()).Value.ToString("dd MMMM yyyy");
            }
            catch(System.InvalidOperationException e)
            {
                inicio = "xxxxxxxxxxxxxxxx-Sin fecha de inicio en Supervisión-xxxxxxxxxxxxxxxxxx";
            }
            
            string cp = "";
            string juez = "";
            string fechaImposicion = "";
            string figuraJudicial = "";
            string nombre = "";
            string delito = "";
            string supervisor = "";
            string distrito = "";
            string presentaciones = "";
            string tipoInforme = "C";

            foreach (var p in persona)
            {
                nombre = p.Paterno + " " + p.Materno + " " + p.Nombre;
                supervisor = nombreSupervisor(p.Supervisor);
            }

            foreach(var c in causapenal)
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
                figuraJudicial = (t.FiguraJudicial == "MC") ? "MEDIDAS CAUTELARES" : "SUSPENSIÓN CONDICIONAL DEL PROCESO";
            }

            foreach (var p in presentacion)
            {
                presentaciones += p.fechaFirma.Value.ToString("dd MMMM yyyy") + " \n";
            }
            #endregion

            #region -string tipo de Fracciones-
            string tipo1 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "RESIDIR EN UN LUGAR DETERMINADO." : "LA PRESENTACIÓN PERIÓDICA ANTE EL JUEZ O ANTE AUTORIDAD DISTINTA QUE AQUÉL DESIGNE.";
            string tipo2 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "FRECUENTAR O DEJAR DE FRECUENTAR DETERMINADOS LUGARES O PERSONAS." : "LA EXHIBICIÓN DE UNA GARANTÍA ECONÓMICA.";
            string tipo3 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "ABSTENERSE DE CONSUMIR DROGAS O ESTUPEFACIENTES O DE ABUSAR DE LAS BEBIDAS ALCOHÓLICAS." : "EL EMBARGO DE BIENES.";
            string tipo4 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "PARTICIPAR EN PROGRAMAS ESPECIALES PARA LA PREVENCIÓN Y EL TRATAMIENTO DE ADICCIONES." : "LA INMOVILIZACIÓN DE CUENTAS Y DEMÁS VALORES QUE SE ENCUENTREN DENTRO DEL SISTEMA FINANCIERO.";
            string tipo5 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "APRENDER UNA PROFESIÓN U OFICIO O SEGUIR CURSOS DE CAPACITACIÓN EN EL LUGAR O LA INSTITUCIÓN QUE DETERMINE EL JUEZ DE CONTROL." : "LA PROHIBICIÓN DE SALIR SIN AUTORIZACIÓN DEL PAÍS, DE LA LOCALIDAD EN LA CUAL RESIDE O DEL ÁMBITO TERRITORIAL QUE FIJE EL JUEZ.";
            string tipo6 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "PRESTAR SERVICIO SOCIAL A FAVOR DEL ESTADO O DE INSTITUCIONES DE BENEFICENCIA PÚBLICA." : "EL SOMETIMIENTO AL CUIDADO O VIGILANCIA DE UNA PERSONA O INSTITUCIÓN DETERMINADA O INTERNAMIENTO A INSTITUCIÓN DETERMINADA.";
            string tipo7 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "SOMETERSE A TRATAMIENTO MÉDICO O PSICOLÓGICO, DE PREFERENCIA EN INSTITUCIONES PÚBLICAS." : "LA PROHIBICIÓN DE CONCURRIR A DETERMINADAS REUNIONES O ACERCARSE O CIERTOS LUGARES.";
            string tipo8 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "TENER UN TRABAJO O EMPLEO, O ADQUIRIR, EN EL PLAZO QUE EL JUEZ DE CONTROL DETERMINE, UN OFICIO, ARTE, INDUSTRIA O PROFESIÓN, SI NO TIENE MEDIOS PROPIOS DE SUBSISTENCIA." : "LA PROHIBICIÓN DE CONVIVIR, ACERCARSE O COMUNICARSE CON DETERMINADAS PERSONAS, CON LAS VÍCTIMAS U OFENDIDOS O TESTIGOS, SIEMPRE QUE NO SE AFECTE EL DERECHO DE DEFENSA.";
            string tipo9 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "SOMETERSE A LA VIGILANCIA QUE DETERMINE EL JUEZ DE CONTROL." : "LA SEPARACIÓN INMEDIATA DEL DOMICILIO.";
            string tipo10 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "NO POSEER NI PORTAR ARMAS." : "LA SUSPENSIÓN TEMPORAL EN EL EJERCICIO DEL CARGO CUANDO SE LE ATRIBUYE UN DELITO COMETIDO POR SERVIDORES PÚBLICOS.";
            string tipo11 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "NO CONDUCIR VEHÍCULOS." : "LA SUSPENSIÓN TEMPORAL EN EL EJERCICIO DE UNA DETERMINADA ACTIVIDAD PROFESIONAL O LABORAL.";
            string tipo12 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "ABSTENERSE DE VIAJAR AL EXTRANJERO." : "LA COLOCACIÓN DE LOCALIZADORES ELECTRÓNICOS.";
            string tipo13 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "CUMPLIR CON LOS DEBERES DE DEUDOR ALIMENTARIO." : "EL RESGUARDO EN SU PROPIO DOMICILIO CON LAS MODALIDADES QUE EL JUEZ DISPONGA.";
            string tipo14 = (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO") ? "CUALQUIER OTRA CONDICIÓN QUE, A JUICIO DEL JUEZ DE CONTROL, LOGRE UNA EFECTIVA TUTELA DE LOS DERECHOS DE LA VÍCTIMA." : "LA PRISIÓN PREVENTIVA.";
            #endregion

            #region -Define contenido de variables-
            for (int i=0; i< datosidFraccion.Length; i++)
            {
                string tipoF = (from table in _context.Fraccionesimpuestas
                            where table.IdFracciones == (Convert.ToInt32(datosidFraccion[i]))
                            select table.Tipo).FirstOrDefault();

                string estatusF= (from table in _context.Fraccionesimpuestas
                                  where table.IdFracciones == (Convert.ToInt32(datosidFraccion[i]))
                                  select table.Estado).FirstOrDefault();

                if (estatusF == "INCUMPLIMIENTO")
                {
                    tipoInforme = "I";
                }

                var actividades = from fracc in _context.Fraccionesimpuestas
                                  join bitacora in _context.Bitacora on fracc.IdFracciones equals bitacora.FracionesImpuestasIdFracionesImpuestas
                                  where fracc.IdFracciones == (Convert.ToInt32(datosidFraccion[i]))
                                  select new
                                  {
                                      actividades=bitacora.Texto,
                                      fecha=bitacora.Fecha
                                  };

                switch (tipoF)
                {
                    case "I":
                        No1 = "I";
                        TextoFraccion1 = tipo1;
                        Estatus1 = estatusF;
                        if(figuraJudicial== "MEDIDAS CAUTELARES")
                        {
                            if (presentaciones != "")
                            {
                                Actividades1 = "CON FECHA " + inicio + " COMPARECE EL SUPERVISADO(A) ANTE LAS INSTALACIONES DE LA DIRECCIÓN GENERAL DE " +
                                "EJECUCIÓN DE PENAS, MEDIDAS DE SEGURIDAD, SUPERVISIÓN DE MEDIDAS CAUTELARES Y DE LA SUSPENSIÓN CONDICIONAL DEL " +
                                "PROCESO AL CUAL SE LE NOTIFICAN SUS OBLIGACIONES PROCESALES, ASÍ MISMO SE TIENE REGISTRO DE LAS SIGUIENTES PRESENTACIONES PERIÓDICAS \n" +
                                presentaciones;
                            }
                            foreach (var act in actividades)
                            {
                                Actividades1 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                            }

                        }
                        else
                        {
                            foreach (var act in actividades)
                            {
                                Actividades1 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                            }
                        }                        
                        break;
                    case "II":
                        No2 = "II";
                        TextoFraccion2 = tipo2;
                        Estatus2 = estatusF;
                        foreach (var act in actividades)
                        {
                            Actividades2 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                        }
                        break;
                    case "III":
                        No3 = "III";
                        TextoFraccion3 = tipo3;
                        Estatus3 = estatusF;
                        foreach (var act in actividades)
                        {
                            Actividades3 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                        }
                        break;
                    case "IV":
                        No4 = "IV";
                        TextoFraccion4 = tipo4;
                        Estatus4 = estatusF;
                        foreach (var act in actividades)
                        {
                            Actividades4 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                        }
                        break;
                    case "V":
                        No5 = "V";
                        TextoFraccion5 = tipo5;
                        Estatus5 = estatusF;
                        foreach (var act in actividades)
                        {
                            Actividades5 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                        }
                        break;
                    case "VI":
                        No6 = "VI";
                        TextoFraccion6 = tipo6;
                        Estatus6 = estatusF;
                        foreach (var act in actividades)
                        {
                            Actividades6 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                        }
                        break;
                    case "VII":
                        No7 = "VII";
                        TextoFraccion7 = tipo7;
                        Estatus7 = estatusF;
                        foreach (var act in actividades)
                        {
                            Actividades7 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                        }
                        break;
                    case "VIII":
                        No8 = "VIII";
                        TextoFraccion8 = tipo8;
                        Estatus8 = estatusF;
                        foreach (var act in actividades)
                        {
                            Actividades8 += "Con fecha " + act.fecha.Value.ToString("dd MMMM yyyy") + " " + act.actividades + " \n";
                        }
                        break;
                    case "IX":
                        No9 = "IX";
                        TextoFraccion9 = tipo9;
                        Estatus9 = estatusF;
                        if (figuraJudicial == "SUSPENSIÓN CONDICIONAL DEL PROCESO")
                        {
                            if (presentaciones != "")
                            {
                                Actividades9 = "CON FECHA " + inicio + " COMPARECE EL SUPERVISADO(A) ANTE LAS INSTALACIONES DE LA DIRECCIÓN GENERAL DE " +
                                "EJECUCIÓN DE PENAS, MEDIDAS DE SEGURIDAD, SUPERVISIÓN DE MEDIDAS CAUTELARES Y DE LA SUSPENSIÓN CONDICIONAL DEL " +
                                "PROCESO AL CUAL SE LE NOTIFICAN SUS OBLIGACIONES PROCESALES, ASÍ MISMO SE TIENE REGISTRO DE LAS SIGUIENTES PRESENTACIONES PERIÓDICAS \n" +
                                presentaciones;
                            }
                            foreach (var act in actividades)
                            {
                                Actividades9 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                            }
                        }
                        else
                        {
                            foreach (var act in actividades)
                            {
                                Actividades9 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                            }
                        }                        
                        break;
                    case "X":
                        No10 = "X";
                        TextoFraccion10 = tipo10;
                        Estatus10 = estatusF;
                        foreach (var act in actividades)
                        {
                            Actividades10 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                        }
                        break;
                    case "XI":
                        No11 = "XI";
                        TextoFraccion11 = tipo11;
                        Estatus11 = estatusF;
                        foreach (var act in actividades)
                        {
                            Actividades11 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                        }
                        break;
                    case "XII":
                        No12 = "XII";
                        TextoFraccion12 = tipo12;
                        Estatus12 = estatusF;
                        foreach (var act in actividades)
                        {
                            Actividades12 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                        }
                        break;
                    case "XIII":
                        No13 = "XIII";
                        TextoFraccion13 = tipo13;
                        Estatus13 = estatusF;
                        foreach (var act in actividades)
                        {
                            Actividades13 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                        }
                        break;
                    case "XIV":
                        No14 = "XIV";
                        TextoFraccion14 = tipo14;
                        Estatus14 = estatusF;
                        foreach (var act in actividades)
                        {
                            Actividades14 += "CON FECHA " + act.fecha.Value.ToString("dd MMMM yyyy").ToUpper() + " " + act.actividades + " \n";
                        }
                        break;
                }

            }
            #endregion

            string templatePath = this._hostingEnvironment.WebRootPath + "\\Documentos\\templateSCP.docx";
            string resultPath = this._hostingEnvironment.WebRootPath + "\\Documentos\\reporteSupervision.docx";


            DocumentCore dc = DocumentCore.Load(templatePath);

            var dataSource = new
            {
                Fecha = DateTime.Now.ToString("dd MMMM yyyy").ToUpper(),
                CP = cp,                
                idPer = idPersona,
                CI = tipoInforme,
                Juez = juez,
                FechaImposicion = fechaImposicion,
                FiguraJudicial = figuraJudicial,
                Nombre = nombre,
                Delito=delito,
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
                    new
                    {
                        No=No7,
                        TextoFraccion=TextoFraccion7,
                        Estatus=Estatus7,
                        Actividades=Actividades7
                    },
                    new
                    {
                        No=No8,
                        TextoFraccion=TextoFraccion8,
                        Estatus=Estatus8,
                        Actividades=Actividades8
                    },
                    new
                    {
                        No=No9,
                        TextoFraccion=TextoFraccion9,
                        Estatus=Estatus9,
                        Actividades=Actividades9
                    },
                    new
                    {
                        No=No10,
                        TextoFraccion=TextoFraccion10,
                        Estatus=Estatus10,
                        Actividades=Actividades10
                    },
                    new
                    {
                        No=No11,
                        TextoFraccion=TextoFraccion11,
                        Estatus=Estatus11,
                        Actividades=Actividades11
                    },
                    new
                    {
                        No=No12,
                        TextoFraccion=TextoFraccion12,
                        Estatus=Estatus12,
                        Actividades=Actividades12
                    },
                    new
                    {
                        No=No13,
                        TextoFraccion=TextoFraccion13,
                        Estatus=Estatus13,
                        Actividades=Actividades13
                    },
                    new
                    {
                        No=No14,
                        TextoFraccion=TextoFraccion14,
                        Estatus=Estatus14,
                        Actividades=Actividades14
                    },
                }
            };

            dc.MailMerge.ClearOptions = MailMergeClearOptions.RemoveEmptyRanges;
            dc.MailMerge.Execute(dataSource);
            dc.Save(resultPath);
            //Response.Redirect("https://localhost:44359/Documentos/reporteSupervisionSCP.docx");
        }
        #endregion

        #region -Graficos-   
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

        private static MemoryStream BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream;
            }
        }

        #endregion

        #region -Actualizar Candado-
        public JsonResult LoockCandado(Persona persona, string[] datoCandado)
        //public async Task<IActionResult> LoockCandado(Persona persona, string[] datoCandado)
        {
            persona.Candado = Convert.ToSByte(datoCandado[0] == "true");
            persona.IdPersona = Int32.Parse(datoCandado[1]);
            persona.MotivoCandado = normaliza(datoCandado[2]);

            var empty = (from p in _context.Persona
                         where p.IdPersona == persona.IdPersona
                         select p);

            if (empty.Any())
            {
                var query = (from p in _context.Persona
                             where p.IdPersona == persona.IdPersona
                             select p).FirstOrDefault();
                query.Candado = persona.Candado;
                query.MotivoCandado = persona.MotivoCandado;
                _context.SaveChanges();
            }
            var stadoc = (from p in _context.Persona
                          where p.IdPersona == persona.IdPersona
                          select p.Candado).FirstOrDefault();
            //return View();

            return Json(new { success = true, responseText = Convert.ToString(stadoc), idPersonas = Convert.ToString(persona.IdPersona) });
        }
        public JsonResult getEstadodeCanadado(int id)
        {
            //IEnumerable<Persona> shops = _context.Persona;
            //return Json(shops.Select(u => new { u.Candado, u.IdPersona }).Where(u => u.IdPersona == id));

            var stadoc = (from p in _context.Persona
                          where p.IdPersona == id
                          select p.Candado);

            return Json(stadoc);
        }
        #endregion

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

        #region -Archivos-
        public IActionResult Archivos()
        {
            return View();
        }
        #endregion
    }
}