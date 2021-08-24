using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using scorpioweb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using SautinSoft.Document;
using SautinSoft.Document.Drawing;
using Microsoft.AspNetCore.Hosting;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using SautinSoft.Document.MailMerging;
using System.Globalization;

namespace scorpioweb.Controllers
{
    public class OficialiaController : Controller
    {
        #region -Variables Globales-
        private readonly penas2Context _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public static List<List<string>> datosDelitos = new List<List<string>>();
        private readonly IHostingEnvironment _hostingEnvironment;
        #endregion

        #region -Constructor-
        public OficialiaController(penas2Context context, RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion

        public string normaliza(string normalizar)
        {
            if (!String.IsNullOrEmpty(normalizar))
            {
                normalizar = normalizar.ToUpper();
            }
            else
            {
                normalizar = "S-D";
            }
            return normalizar;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region -Modal Delito-
        public ActionResult GuardarDelito(string[] datosDelito)
        {
            string currentUser = User.Identity.Name;
            datosDelito.DefaultIfEmpty();
            for (int i = 0; i < datosDelito.Length; i++)
            {

                datosDelitos.Add(new List<String> { datosDelito[i], currentUser });
            }

            return Json(new { success = true, responseText = "Datos Guardados con éxito" });

        }
        #endregion

        public async Task<IActionResult> CreateCausaPenal(Causapenal causapenal, Delito delitoDB, string cnpp, string juez, string distrito, string cambio, string cp, string delitoM)
        {
            string currentUser = User.Identity.Name;
            if (ModelState.IsValid)
            {
                int idCausaPenal = ((from table in _context.Causapenal
                                     select table.IdCausaPenal).Max()) + 1;
                causapenal.IdCausaPenal = idCausaPenal;
                #region -Delitos-
                for (int i = 0; i < datosDelitos.Count; i = i + 3)
                {
                    if (datosDelitos[i][1] == currentUser)
                    {
                        delitoDB.Tipo = datosDelitos[i][0];
                        delitoDB.Modalidad = datosDelitos[i + 1][0];
                        delitoDB.EspecificarDelito = datosDelitos[i + 2][0];
                        delitoDB.CausaPenalIdCausaPenal = idCausaPenal;

                        _context.Add(delitoDB);
                        await _context.SaveChangesAsync(null, 1);
                    }
                }

                for (int i = 0; i < datosDelitos.Count; i++)
                {
                    if (datosDelitos[i][1] == currentUser)
                    {
                        datosDelitos.RemoveAt(i);
                        i--;
                    }
                }
                #endregion

                causapenal.Cnpp = cnpp;
                causapenal.Juez = juez;
                causapenal.Distrito = distrito;
                causapenal.Cambio = cambio;
                causapenal.CausaPenal = cp;
                _context.Add(causapenal);
                await _context.SaveChangesAsync(null, 1);
                return RedirectToAction(nameof(Index));
            }
            return View(causapenal);
        }

        public IActionResult Captura()
        {
            List<Causapenal> listaCP = new List<Causapenal>();
            List<Causapenal> CPVM = _context.Causapenal.ToList();
            listaCP = (
                from CPTable in CPVM
                select CPTable
                ).ToList();
            ViewBag.cp = listaCP;
            ViewBag.directorio = _context.Directoriojueces.Select(Directoriojueces => Directoriojueces.Area).ToList();
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();
            ViewBag.expide = _context.Expide.Select(Expide => Expide.Nombre).ToList();
            ViewBag.asunto = _context.Asuntooficio.Select(Asuntooficio => Asuntooficio.Asunto).ToList();
            List<SelectListItem> ListaUsuarios = new List<SelectListItem>();
            int i = 0;
            ListaUsuarios.Add(new SelectListItem
            {
                Text = "Selecciona",
                Value = i.ToString()
            });
            i++;
            foreach (var user in userManager.Users)
            {
                ListaUsuarios.Add(new SelectListItem
                {
                    Text = user.ToString(),
                    Value = i.ToString()
                });
                i++;
            }
            ViewBag.usuarios = ListaUsuarios;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Captura(Oficialia oficialia, string capturista, string recibe, string metodoNotificacion, string numOficio, string expide,
            string referenteImputado, string sexo, string paterno, string materno, string nombre, string carpetaEjecucion, string existeVictima,
            string nombreVictima, string direccionVictima, string asuntoOficio, string tieneTermino, string usuarioTurnar, string observaciones,
            int? idCausaPenal, sbyte? Entregado, DateTime? fechaRecepcion, DateTime? fechaEmision, DateTime? fechaTermino)
        {
            if (ModelState.IsValid)
            {
                int count = (from table in _context.Oficialia
                             select table.IdOficialia).Count();
                int idOficialia;
                if (count == 0)
                {
                    idOficialia = 1;
                }
                else
                {
                    idOficialia = ((from table in _context.Oficialia
                                    select table.IdOficialia).Max()) + 1;
                }
                oficialia.IdOficialia = idOficialia;

                oficialia.Capturista = User.Identity.Name;
                oficialia.Recibe = recibe;
                oficialia.MetodoNotificacion = metodoNotificacion;
                oficialia.NumOficio = normaliza(numOficio);
                oficialia.Expide = normaliza(expide);
                oficialia.ReferenteImputado = referenteImputado;
                oficialia.Sexo = sexo;
                oficialia.Paterno = normaliza(paterno);
                oficialia.Materno = normaliza(materno);
                oficialia.Nombre = normaliza(nombre);
                oficialia.CarpetaEjecucion = normaliza(carpetaEjecucion);
                oficialia.ExisteVictima = existeVictima;
                oficialia.NombreVictima = normaliza(nombreVictima);
                oficialia.DireccionVictima = normaliza(direccionVictima);
                oficialia.AsuntoOficio = normaliza(asuntoOficio);
                oficialia.TieneTermino = tieneTermino;
                oficialia.Observaciones = normaliza(observaciones);
                oficialia.Entregado = Entregado;
                oficialia.FechaRecepcion = fechaRecepcion;
                oficialia.FechaEmision = fechaEmision;
                oficialia.FechaTermino = fechaTermino;
                oficialia.IdCausaPenal = idCausaPenal;
                if (usuarioTurnar == "Selecciona")
                {
                    oficialia.UsuarioTurnar = null;
                }
                else
                {
                    oficialia.UsuarioTurnar = usuarioTurnar;
                }
                var cp = await _context.Causapenal.SingleOrDefaultAsync(m => m.IdCausaPenal == idCausaPenal);
                if (cp != null)
                {
                    oficialia.CausaPenal = cp.CausaPenal;
                }
                _context.Add(oficialia);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                return RedirectToAction(nameof(Index));
            }
            return View(oficialia);
        }

        public async Task<IActionResult> EditRegistros(
            string currentFilter,
            DateTime? inicial,
            DateTime? final,
            int? pageNumber)
        {
            List<SelectListItem> ListaUsuarios = new List<SelectListItem>();
            int j = 0;
            foreach (var usuario in userManager.Users)
            {
                ListaUsuarios.Add(new SelectListItem
                {
                    Text = usuario.ToString(),
                    Value = j.ToString()
                });
                j++;
            }
            ViewBag.usuarios = ListaUsuarios;

            #region -ListaUsuarios-            
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            List<string> rolUsuario = new List<string>();

            for (int i = 0; i < roles.Count; i++)
            {
                rolUsuario.Add(roles[i]);
            }

            ViewBag.RolesUsuario = rolUsuario;

            String users = user.ToString();
            ViewBag.RolesUsuarios = users;
            #endregion

            ViewData["CurrentFilter"] = currentFilter;
            if (inicial != null)
            {
                ViewData["inicial"] = Convert.ToDateTime(inicial).ToString("yyyy-MM-dd");
            }
            if (final != null)
            {
                ViewData["final"] = Convert.ToDateTime(final).ToString("yyyy-MM-dd");
            }

            var oficios = from o in _context.Oficialia
                          select o;

            if (inicial != null)
            {
                oficios = oficios.Where(o => o.FechaEmision != null && DateTime.Compare((DateTime)inicial, (DateTime)o.FechaEmision) <= 0);
            }

            if (final != null)
            {
                oficios = oficios.Where(o => o.FechaEmision != null && DateTime.Compare((DateTime)o.FechaEmision, (DateTime)final) <= 0);
            }

            if (currentFilter != null)
            {
                foreach (var item in currentFilter.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    oficios = oficios.Where(o => o.UsuarioTurnar.Contains(currentFilter) ||
                                             o.PaternoMaternoNombre.Contains(currentFilter.ToUpper()) ||
                                             o.NombrePaternoMaterno.Contains(currentFilter.ToUpper()));
                }
            }

            int pageSize = 10;
            return View(await PaginatedList<Oficialia>.CreateAsync(oficios.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> Edit(int? id, string titulo)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oficio = await _context.Oficialia.SingleOrDefaultAsync(m => m.IdOficialia == id);
            if (oficio == null)
            {
                return NotFound();
            }

            List<Causapenal> listaCP = new List<Causapenal>();
            List<Causapenal> CPVM = _context.Causapenal.ToList();
            listaCP = (
                from CPTable in CPVM
                select CPTable
                ).ToList();
            ViewBag.cp = listaCP;
            ViewBag.directorio = _context.Directoriojueces.Select(Directoriojueces => Directoriojueces.Area).ToList();
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();
            ViewBag.expide = _context.Expide.Select(Expide => Expide.Nombre).ToList();
            ViewBag.asunto = _context.Asuntooficio.Select(Asuntooficio => Asuntooficio.Asunto).ToList();
            List<SelectListItem> ListaUsuarios = new List<SelectListItem>();
            int i = 0;
            ListaUsuarios.Add(new SelectListItem
            {
                Text = "Selecciona",
                Value = i.ToString()
            });
            i++;
            foreach (var user in userManager.Users)
            {
                ListaUsuarios.Add(new SelectListItem
                {
                    Text = user.ToString(),
                    Value = i.ToString()
                });
                i++;
            }
            ViewBag.usuarios = ListaUsuarios;

            var causapenal = await _context.Causapenal.SingleOrDefaultAsync(m => m.IdCausaPenal == oficio.IdCausaPenal);
            if (causapenal != null)
            {
                ViewBag.val = causapenal.CausaPenal + ", Distrito " + causapenal.Distrito + ", " + causapenal.Juez;
            }
            ViewBag.titulo = titulo;
            return View(oficio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdOficialia,Capturista,Recibe,MetodoNotificacion,NumOficio,FechaRecepcion,FechaEmision,Expide,ReferenteImputado,Sexo,Paterno,Materno,Nombre,CarpetaEjecucion,IdCausaPenal,ExisteVictima,NombreVictima,DireccionVictima,AsuntoOficio,TieneTermino,FechaTermino,UsuarioTurnar,Entregado,Observaciones")] Oficialia oficialia)
        {
            if (id != oficialia.IdOficialia)
            {
                return NotFound();
            }

            oficialia.NumOficio = normaliza(oficialia.NumOficio);
            oficialia.Expide = normaliza(oficialia.Expide);
            oficialia.Paterno = normaliza(oficialia.Paterno);
            oficialia.Materno = normaliza(oficialia.Materno);
            oficialia.Nombre = normaliza(oficialia.Nombre);
            oficialia.CarpetaEjecucion = normaliza(oficialia.CarpetaEjecucion);
            oficialia.NombreVictima = normaliza(oficialia.NombreVictima);
            oficialia.DireccionVictima = normaliza(oficialia.DireccionVictima);
            oficialia.AsuntoOficio = normaliza(oficialia.AsuntoOficio);
            oficialia.Observaciones = normaliza(oficialia.Observaciones);
            if (oficialia.UsuarioTurnar == "Selecciona")
            {
                oficialia.UsuarioTurnar = null;
            }

            var cp = await _context.Causapenal.SingleOrDefaultAsync(m => m.IdCausaPenal == oficialia.IdCausaPenal);
            if (cp != null)
            {
                oficialia.CausaPenal = cp.CausaPenal;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var oldOficialia = await _context.Oficialia.FindAsync(oficialia.IdOficialia);
                    _context.Entry(oldOficialia).CurrentValues.SetValues(oficialia);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OficialiaExists(oficialia.IdOficialia))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("EditRegistros", "Oficialia");
            }
            return View(oficialia);
        }

        public async Task<IActionResult> Duplicate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oficialia = await _context.Oficialia.SingleOrDefaultAsync(m => m.IdOficialia == id);
            if (oficialia == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                int count = (from table in _context.Oficialia
                             select table.IdOficialia).Count();
                int idOficialia;
                if (count == 0)
                {
                    idOficialia = 1;
                }
                else
                {
                    idOficialia = ((from table in _context.Oficialia
                                    select table.IdOficialia).Max()) + 1;
                }
                oficialia.IdOficialia = idOficialia;

                _context.Add(oficialia);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                return RedirectToAction("Edit/" + idOficialia, "Oficialia", new { @titulo = "Registro Copiado" }); 
            }
            return View(oficialia);
        }


        public async Task<IActionResult> Reportes()
        {
            List<SelectListItem> ListaCapturista = new List<SelectListItem>();
            List<SelectListItem> ListaRecibe = new List<SelectListItem>();

            var c = from o in _context.Oficialia
                    group o by new { o.Capturista }
                    into grupo
                    select grupo.FirstOrDefault();

            foreach (var capturista in c)
            {
                ListaCapturista.Add(new SelectListItem
                {
                    Text = capturista.Capturista,
                    Value = capturista.Capturista
                });
            }

            var r = from o in _context.Oficialia
                    group o by new { o.Recibe }
                    into grupo
                    select grupo.FirstOrDefault();

            foreach (var recibe in r)
            {
                ListaRecibe.Add(new SelectListItem
                {
                    Text = recibe.Recibe,
                    Value = recibe.Recibe
                });
            }


            ViewBag.capturista = ListaCapturista;
            ViewBag.recibe = ListaRecibe;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Reportes(DateTime fechaInicio, DateTime fechaFin, string entrega, string captura)
        {

            IEnumerable<OficialiaReporte> dataOficialia = from o in _context.Oficialia.AsEnumerable()
                              where o.Capturista == captura
                              && o.Recibe == entrega
                              && (o.FechaRecepcion >= fechaInicio && o.FechaRecepcion <= fechaFin)
                              select new OficialiaReporte{
                                  FechaRecepcion = (o.FechaRecepcion.Value).ToString("dd-MMMM-yyyy"),
                                  FechaEmision= (o.FechaEmision.Value).ToString("dd-MMMM-yyyy"),
                                  Expide=o.Expide,
                                  AsuntoOficio=o.AsuntoOficio,
                                  Paterno=o.Paterno,
                                  Materno=o.Materno,
                                  Nombre=o.Nombre,
                                  CausaPenal=o.CausaPenal,
                                  CarpetaEjecucion=o.CarpetaEjecucion,
                                  Observaciones=o.Observaciones
                              };

            //item.FechaDetencion.Value.ToString("dd-MMMM-yyyy")

            //string xmlOficialia = SerializeObject<List<Oficialia>>(listaOficialia);

            
            #region -GeneraDocumento-
            DataSet ds = new DataSet();
            //ds.ReadXml(new StringReader(xmlOficialia));

            string templatePath = this._hostingEnvironment.WebRootPath + "\\Documentos\\templateOficialia.docx";
            string resultPath = this._hostingEnvironment.WebRootPath + "\\Documentos\\reporteOficialia.docx";

            DocumentCore dc = DocumentCore.Load(templatePath);

            var dataSource = new[]
            {
                new
                {
                    FechaInicio=fechaInicio.ToString("dd-MMMM-yyyy"),
                    FechaFin=fechaFin.ToString("dd-MMMM-yyyy"),
                    Entrega=entrega.Substring(0, (entrega.IndexOf("@"))),
                    Captura=captura.Substring(0, (captura.IndexOf("@")))
                }
            };

            dc.MailMerge.ClearOptions = MailMergeClearOptions.RemoveUnusedFields;
            dc.MailMerge.Execute(dataSource);
            dc.MailMerge.Execute(dataOficialia, "OficialiaReporte");
            

            dc.Save(resultPath);


            Response.Redirect("https://localhost:44359/Documentos/reporteOficialia.docx");
            //Response.Redirect("http://10.6.60.190/Documentos/reporteOficialia.docx");
            #endregion

            //return RedirectToAction("Reportes", "Oficialia");
        }

        #region -SerializeObject-
        public static string SerializeObject<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(true, true);
            settings.Indent = true;
            //settings.OmitXmlDeclaration = true;  

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, obj, ns);
                }

                return textWriter.ToString(); //This is the output as a string  
            }
        }
        #endregion

        #region -OficialiaExists-
        private bool OficialiaExists(int id)
        {
            return _context.Oficialia.Any(e => e.IdOficialia == id);
        }
        #endregion
    }
}