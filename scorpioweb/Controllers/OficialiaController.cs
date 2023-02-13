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
using Microsoft.AspNetCore.Authorization;
using F23.StringSimilarity;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Spreadsheet;

namespace scorpioweb.Controllers
{
    [Authorize]
    public class OficialiaController : Controller
    {
        #region -Variables Globales-
        private readonly penas2Context _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public static List<List<string>> datosDelitos = new List<List<string>>();
        private readonly IHostingEnvironment _hostingEnvironment;
        public static List<SelectListItem> ListaRecibe = new List<SelectListItem>()
        {
            new SelectListItem{ Text="", Value="0"},
            new SelectListItem{ Text="karla.zapico@dgepms.com", Value="1"},
            new SelectListItem{ Text="renata.martinez@dgepms.com", Value="2"},
            new SelectListItem{ Text="yoena@dgepms.com", Value="3"}
        };
        public static List<SelectListItem> ListaCoordinadores = new List<SelectListItem>()
        {
            new SelectListItem{ Text="", Value="0"},
            new SelectListItem{ Text="alma.gaxiola@dgepms.com", Value="1"},
            new SelectListItem{ Text="esmeralda.vargas@dgepms.com", Value="2"},
            new SelectListItem{ Text="andrea.valdez@dgepms.com", Value="3"},
            new SelectListItem{ Text="iovannifs92@gmail.com", Value="4"},
            new SelectListItem{ Text="yuridia.quinones@dgepms.com", Value="5"},
            new SelectListItem{ Text="guadalupe.barcenas@dgepms.com", Value="6"},
            new SelectListItem{ Text="liliana.vargas@dgepms.com", Value="7"},
            new SelectListItem{ Text="uriel.ortega@dgepms.com", Value="8"},
            new SelectListItem{ Text="juan.castillo@dgepms.com", Value="9"},
            new SelectListItem{ Text="jazmin.flores@dgepms.com", Value="10"},
            new SelectListItem{ Text="janeth@nortedgepms.com", Value="11"},
            new SelectListItem{ Text="carmen.trujillo@dgepms.com", Value="12"},
            new SelectListItem{ Text="yoena@dgepms.com", Value="13"},
            new SelectListItem{ Text="enrique.martinez@dgepms.com", Value="14"}
        };
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
                normalizar = "NA";
            }
            return normalizar;
        }

        public string removeSpaces(string str)
        {
            while (str.Length > 0 && str[0] == ' ')
            {
                str = str.Substring(1);
            }
            while (str.Length > 0 && str[str.Length - 1] == ' ')
            {
                str = str.Substring(0, str.Length - 1);
            }
            return str;
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
                        delitoDB.Tipo = normaliza(datosDelitos[i][0]);
                        delitoDB.Modalidad = normaliza(datosDelitos[i + 1][0]);
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
                causapenal.Juez = normaliza(juez);
                causapenal.Distrito = distrito;
                causapenal.Cambio = cambio;
                causapenal.CausaPenal = normaliza(cp);
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
            ViewBag.recibe = ListaRecibe;
            ViewBag.coordinadores = ListaCoordinadores;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Captura(Oficialia oficialia, string recibe, string metodoNotificacion, string numOficio, string expide,
            string referenteImputado, string sexo, string paterno, string materno, string nombre, string carpetaEjecucion, string existeVictima,
            string nombreVictima, string direccionVictima, string asuntoOficio, string tieneTermino, string usuarioTurnar, string observaciones,
            int? idCausaPenal, string Entregado, DateTime? fechaRecepcion, DateTime? fechaEmision, DateTime? fechaTermino, string delitoTipo, string autoVinculacion, IFormFile archivo)
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
                oficialia.MetodoNotificacion = metodoNotificacion;
                oficialia.NumOficio = normaliza(numOficio);
                oficialia.Expide = normaliza(expide);
                oficialia.ReferenteImputado = referenteImputado;
                oficialia.Sexo = sexo;
                oficialia.Paterno = removeSpaces(normaliza(paterno));
                oficialia.Materno = removeSpaces(normaliza(materno));
                oficialia.Nombre = removeSpaces(normaliza(nombre));
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
                oficialia.DelitoTipo = normaliza(delitoTipo);
                oficialia.AutoVinculacion = autoVinculacion;
                oficialia.IdCausaPenal = idCausaPenal;
                oficialia.Seguimiento = "NO";
                if (usuarioTurnar == "Selecciona")
                {
                    oficialia.UsuarioTurnar = null;
                }
                else
                {
                    oficialia.UsuarioTurnar = usuarioTurnar;
                }
                if (recibe == "Selecciona")
                {
                    oficialia.Recibe = null;
                }
                else
                {
                    oficialia.Recibe = recibe;
                }
                var cp = await _context.Causapenal.SingleOrDefaultAsync(m => m.IdCausaPenal == idCausaPenal);
                if (cp != null)
                {
                    oficialia.CausaPenal = cp.CausaPenal;
                }
                #region -Guardar archivo-
                if (archivo != null)
                {
                    string file_name = "o_" + idOficialia + Path.GetExtension(archivo.FileName);
                    oficialia.RutaArchivo = file_name;
                    var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "EvidenciaOficialia");
                    var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                    await archivo.CopyToAsync(stream);
                    stream.Close();
                }
                #endregion
                _context.Add(oficialia);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                return RedirectToAction("EditRegistros", "Oficialia");
            }
            return View(oficialia);
        }

        public async Task<IActionResult> EditRegistros(
            string currentFilter,
            DateTime? inicial,
            DateTime? final,
            string UsuarioTurnar,
            string Capturista,
            int? pageNumber)
        {
            List<SelectListItem> ListaUsuariosOficialia = new List<SelectListItem>();
            int i = 0;
            ListaUsuariosOficialia.Add(new SelectListItem
            {
                Text = "todos",
                Value = i.ToString()
            });
            i++;
            var usersOficialia = userManager.Users.OrderBy(r => r.UserName);
            foreach (var u in usersOficialia)
            {
                if (await userManager.IsInRoleAsync(u, "Oficialia"))
                {
                    ListaUsuariosOficialia.Add(new SelectListItem
                    {
                        Text = u.ToString(),
                        Value = i.ToString()
                    });
                    i++;
                }
            }
            ViewBag.usuariosOficialia = ListaUsuariosOficialia;

            var supervisores = from o in _context.Oficialia
                               where o.UsuarioTurnar != null
                               orderby o.UsuarioTurnar
                               select o.UsuarioTurnar;
            supervisores = supervisores.Distinct();
            List<SelectListItem> ListaSupervisores = new List<SelectListItem>();
            int j = 0;
            ListaSupervisores.Add(new SelectListItem
            {
                Text = "todos",
                Value = j.ToString()
            });
            j++;
            foreach (var supervisor in supervisores)
            {
                ListaSupervisores.Add(new SelectListItem
                {
                    Text = supervisor.ToString(),
                    Value = j.ToString()
                });
                j++;
            }
            ViewBag.supervisores = ListaSupervisores;

            ViewBag.UsuarioTurnar = UsuarioTurnar;
            ViewBag.Capturista = Capturista;

            ViewData["CurrentFilter"] = currentFilter;
            if (inicial != null)
            {
                ViewData["inicial"] = Convert.ToDateTime(inicial).ToString("yyyy-MM-dd");
            }
            if (final != null)
            {
                ViewData["final"] = Convert.ToDateTime(final).ToString("yyyy-MM-dd");
            }

            var oficios = from oficialia in _context.Oficialia
                          select oficialia;

            if (inicial != null)
            {
                oficios = oficios.Where(o => o.FechaRecepcion != null && DateTime.Compare((DateTime)inicial.Value.Date, (DateTime)o.FechaRecepcion.Value.Date) <= 0);
            }

            if (final != null)
            {
                oficios = oficios.Where(o => o.FechaRecepcion != null && DateTime.Compare((DateTime)o.FechaRecepcion.Value.Date, (DateTime)final.Value.Date) <= 0);
            }

            if (UsuarioTurnar != null && UsuarioTurnar != "todos")
            {
                oficios = oficios.Where(o => o.UsuarioTurnar != null && o.UsuarioTurnar == UsuarioTurnar);
            }

            if (Capturista != null && Capturista != "todos")
            {
                oficios = oficios.Where(o => o.Capturista != null && o.Capturista == Capturista);
            }

            if (currentFilter != null)
            {
                foreach (var item in currentFilter.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    oficios = oficios.Where(o => (o.UsuarioTurnar != null && o.UsuarioTurnar.Contains(currentFilter.ToLower())) ||
                                             (o.Paterno + " " + o.Materno + " " + o.Nombre).Contains(currentFilter.ToUpper()) ||
                                             (o.Nombre + " " + o.Paterno + " " + o.Materno).Contains(currentFilter.ToUpper()) ||
                                             (o.CausaPenal != null && o.CausaPenal.Contains(currentFilter)) ||
                                             o.Observaciones.Contains(currentFilter));
                }
            }

            oficios = oficios.OrderByDescending(x => x.IdOficialia);

            int pageSize = 10;
            return View(await PaginatedList<Oficialia>.CreateAsync(oficios.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> Seguimiento(
            string currentFilter,
            DateTime? inicial,
            DateTime? final,
            int? pageNumber)
        {
            string currentUser = User.Identity.Name;
            var user = await userManager.FindByNameAsync(currentUser);
            var roles = await userManager.GetRolesAsync(user);
            Boolean flagMaster = false;
            foreach (var rol in roles)
            {
                if (rol == "Masteradmin" || rol == "Oficialia")
                {
                    flagMaster = true;
                }
            }

            ViewData["CurrentFilter"] = currentFilter;
            if (inicial != null)
            {
                ViewData["inicial"] = Convert.ToDateTime(inicial).ToString("yyyy-MM-dd");
            }
            if (final != null)
            {
                ViewData["final"] = Convert.ToDateTime(final).ToString("yyyy-MM-dd");
            }

            var oficios = from oficialia in _context.Oficialia
                          where oficialia.UsuarioTurnar == currentUser
                          select oficialia;
            if(flagMaster)
            {
                oficios = from oficialia in _context.Oficialia
                          select oficialia;
            }

            oficios = oficios.OrderBy(s => s.Seguimiento).ThenByDescending(s => s.IdOficialia);

            if (inicial != null)
            {
                oficios = oficios.Where(o => o.FechaRecepcion != null && DateTime.Compare((DateTime)inicial.Value.Date, (DateTime)o.FechaRecepcion.Value.Date) <= 0);
            }

            if (final != null)
            {
                oficios = oficios.Where(o => o.FechaRecepcion != null && DateTime.Compare((DateTime)o.FechaRecepcion.Value.Date, (DateTime)final.Value.Date) <= 0);
            }

            if (currentFilter != null)
            {
                foreach (var item in currentFilter.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    oficios = oficios.Where(o => (o.Paterno + " " + o.Materno + " " + o.Nombre).Contains(currentFilter.ToUpper()) ||
                                             (o.Nombre + " " + o.Paterno + " " + o.Materno).Contains(currentFilter.ToUpper()));
                }
            }

            int pageSize = 10;
            return View(await PaginatedList<Oficialia>.CreateAsync(oficios.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public void Toggle(int id)
        {
            var oficio = (from oficialia in _context.Oficialia
                          where oficialia.IdOficialia == id
                          select oficialia).FirstOrDefault();
            if (oficio.Seguimiento == "NO")
            {
                oficio.Seguimiento = "SI";
                oficio.FechaSeguimiento = DateTime.Now;
            }
            else
            {
                oficio.Seguimiento = "NO";
            }
            _context.SaveChanges();
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
            ViewBag.recibe = ListaRecibe;
            ViewBag.coordinadores = ListaCoordinadores;

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
        public async Task<IActionResult> Edit(int id, IFormFile archivo, [Bind("IdOficialia,Capturista,Recibe,MetodoNotificacion,NumOficio,FechaRecepcion,FechaEmision,Expide,ReferenteImputado,Sexo,Paterno,Materno,Nombre,CarpetaEjecucion,IdCausaPenal,CausaPenal,DelitoTipo,AutoVinculacion,ExisteVictima,NombreVictima,DireccionVictima,AsuntoOficio,TieneTermino,FechaTermino,UsuarioTurnar,Entregado,Observaciones")] Oficialia oficialia)
        {
            if (id != oficialia.IdOficialia)
            {
                return NotFound();
            }

            oficialia.NumOficio = normaliza(oficialia.NumOficio);
            oficialia.Expide = normaliza(oficialia.Expide);
            oficialia.Paterno = removeSpaces(normaliza(oficialia.Paterno));
            oficialia.Materno = removeSpaces(normaliza(oficialia.Materno));
            oficialia.Nombre = removeSpaces(normaliza(oficialia.Nombre));
            oficialia.CarpetaEjecucion = normaliza(oficialia.CarpetaEjecucion);
            oficialia.NombreVictima = normaliza(oficialia.NombreVictima);
            oficialia.DireccionVictima = normaliza(oficialia.DireccionVictima);
            oficialia.AsuntoOficio = normaliza(oficialia.AsuntoOficio);
            oficialia.Observaciones = normaliza(oficialia.Observaciones);
            oficialia.DelitoTipo = normaliza(oficialia.DelitoTipo);
            oficialia.AutoVinculacion = normaliza(oficialia.AutoVinculacion);
            var oldOficialia = await _context.Oficialia.FindAsync(oficialia.IdOficialia);
            if (oficialia.Recibe == "Selecciona")
            {
                oficialia.Recibe = null;
            }
            if (oficialia.UsuarioTurnar == "Selecciona")
            {
                oficialia.UsuarioTurnar = null;
            }

            var cp = await _context.Causapenal.SingleOrDefaultAsync(m => m.IdCausaPenal == oficialia.IdCausaPenal);
            if (cp != null)
            {
                oficialia.CausaPenal = cp.CausaPenal;
            }

            #region -EditarArchivo-
            if (archivo == null)
            {
                oficialia.RutaArchivo = oldOficialia.RutaArchivo;
            }
            else
            {
                string file_name = "o_" + id + Path.GetExtension(archivo.FileName);
                oficialia.RutaArchivo = file_name;
                var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "EvidenciaOficialia");
                if (System.IO.File.Exists(Path.Combine(uploads, file_name)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, file_name));
                }
                var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                await archivo.CopyToAsync(stream);
                stream.Close();
            }
            #endregion

            if (ModelState.IsValid)
            {
                try
                {
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

        public async Task<IActionResult> Reportes(
            string currentFilter,
            DateTime? inicial,
            DateTime? final,
            string UsuarioTurnar,
            string Capturista)
        {
            List<SelectListItem> ListaUsuariosOficialia = new List<SelectListItem>();
            int i = 0;
            ListaUsuariosOficialia.Add(new SelectListItem
            {
                Text = "todos",
                Value = i.ToString()
            });
            i++;
            var usersOficialia = userManager.Users.OrderBy(r => r.UserName);
            foreach (var u in usersOficialia)
            {
                if (await userManager.IsInRoleAsync(u, "Oficialia"))
                {
                    ListaUsuariosOficialia.Add(new SelectListItem
                    {
                        Text = u.ToString(),
                        Value = i.ToString()
                    });
                    i++;
                }
            }
            ViewBag.usuariosOficialia = ListaUsuariosOficialia;

            var supervisores = from o in _context.Oficialia
                               where o.UsuarioTurnar != null
                               orderby o.UsuarioTurnar
                               select o.UsuarioTurnar;
            supervisores = supervisores.Distinct();
            List <SelectListItem> ListaSupervisores = new List<SelectListItem>();
            int j = 0;
            ListaSupervisores.Add(new SelectListItem
            {
                Text = "todos",
                Value = j.ToString()
            });
            j++;
            foreach (var supervisor in supervisores)
            {
                ListaSupervisores.Add(new SelectListItem
                {
                    Text = supervisor.ToString(),
                    Value = j.ToString()
                });
                j++;
            }
            ViewBag.supervisores = ListaSupervisores;

            ViewBag.UsuarioTurnar = UsuarioTurnar;
            ViewBag.Capturista = Capturista;

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
                oficios = oficios.Where(o => o.FechaRecepcion != null && DateTime.Compare((DateTime)inicial.Value.Date, (DateTime)o.FechaRecepcion.Value.Date) <= 0);
            }

            if (final != null)
            if (final != null)
            {
                oficios = oficios.Where(o => o.FechaRecepcion != null && DateTime.Compare((DateTime)o.FechaRecepcion.Value.Date, (DateTime)final.Value.Date) <= 0);
            }

            if (UsuarioTurnar != null && UsuarioTurnar != "todos")
            {
                oficios = oficios.Where(o => o.UsuarioTurnar != null && o.UsuarioTurnar == UsuarioTurnar);
            }

            if (Capturista != null && Capturista != "todos")
            {
                oficios = oficios.Where(o => o.Capturista != null && o.Capturista == Capturista);
            }

            if (currentFilter != null)
            {
                foreach (var item in currentFilter.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    oficios = oficios.Where(o => (o.UsuarioTurnar != null && o.UsuarioTurnar.Contains(currentFilter.ToLower())) ||
                                             (o.Paterno + " " + o.Materno + " " + o.Nombre).Contains(currentFilter.ToUpper()) ||
                                             (o.Nombre + " " + o.Paterno + " " + o.Materno).Contains(currentFilter.ToUpper()) ||
                                             (o.CausaPenal != null && o.CausaPenal.Contains(currentFilter)));
                }
            }
            oficios = oficios.OrderByDescending(x => x.IdOficialia);

            var ids = from o in oficios
                      select o.IdOficialia;
            ViewBag.ids = ids.ToList();
            return View(await oficios.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Reportes(string ids,
            DateTime startDate,
            DateTime endDate,
            DateTime? inicial,
            DateTime? final,
            string supervisor,
            string capturer)
        {
            string[] idList = ids.Substring(1, ids.Length - 2).Split(',');

            string area;
            var record = _context.Areas.FirstOrDefault(a => a.UserName == supervisor);
            if (record == null)
            {
                area = "SIN AREA";
            }
            else
            {
                area = record.Area;
            }

            var oficios = from o in _context.Oficialia
                          select o;

            IEnumerable<OficialiaReporte> dataOficialia = from o in oficios
                                                          where idList.Contains(o.IdOficialia.ToString())
                                                          select new OficialiaReporte
                                                          {
                                                              IdOficialia = o.IdOficialia,
                                                              FechaRecepcion = (o.FechaRecepcion.Value).ToString("dd-MMMM-yyyy"),
                                                              FechaEmision = (o.FechaEmision.Value).ToString("dd-MMMM-yyyy"),
                                                              Expide = o.Expide,
                                                              AsuntoOficio = o.AsuntoOficio,
                                                              Paterno = o.Paterno,
                                                              Materno = o.Materno,
                                                              Nombre = o.Nombre,
                                                              CausaPenal = o.CausaPenal,
                                                              CarpetaEjecucion = o.CarpetaEjecucion,
                                                              Observaciones = o.Observaciones
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
                    FechaInicio=startDate.ToString("dd-MMMM-yyyy"),
                    FechaFin=endDate.ToString("dd-MMMM-yyyy"),
                    Entrega=supervisor,
                    Capturista=capturer,
                    Area=area
                }
            };

            dc.MailMerge.ClearOptions = MailMergeClearOptions.RemoveUnusedFields;
            dc.MailMerge.Execute(dataSource);
            dc.MailMerge.Execute(dataOficialia, "OficialiaReporte");


            dc.Save(resultPath);


            //Response.Redirect("https://localhost:44359/Documentos/reporteOficialia.docx");
            Response.Redirect("http://10.6.60.190/Documentos/reporteOficialia.docx");
            #endregion

            //return RedirectToAction("Reportes", "Oficialia");
        }

        public IActionResult CPList(string cp)
        {
            cp = normaliza(cp);

            var query = from c in _context.Causapenal
                        select c;

            int idCP = 0;
            string trialCP = "";
            var cosine = new Cosine(2);
            double r = 0;
            var list = new List<Tuple<double, Causapenal>>();

            foreach (var q in query)
            {
                r = cosine.Similarity(q.CausaPenal, cp);
                if (r >= 0.75)
                {
                    list.Add(new Tuple<double, Causapenal>(r, q));
                }
            }

            list = list.OrderByDescending(x => x.Item1).ToList();

            List<Causapenal> CPquery = _context.Causapenal.ToList();

            List<Causapenal> CPList = new List<Causapenal>();
            foreach (var c in list)
            {
                CPList.Add(c.Item2);
            }
            ViewData["CPList"] = CPList;

            return View();
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

        // GET: Oficialia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oficialia = await _context.Oficialia
                .SingleOrDefaultAsync(m => m.IdOficialia == id);
            if (oficialia == null)
            {
                return NotFound();
            }

            return View(oficialia);
        }

        // POST: Oficialia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var oficialia = await _context.Oficialia.SingleOrDefaultAsync(m => m.IdOficialia == id);
            _context.Oficialia.Remove(oficialia);
            await _context.SaveChangesAsync();
            return RedirectToAction("EditRegistros", "Oficialia");
        }

        #region -OficialiaExists-
        private bool OficialiaExists(int id)
        {
            return _context.Oficialia.Any(e => e.IdOficialia == id);
        }
        #endregion
    }
}