using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;
using scorpioweb.Class;
using System.Security.Policy;

namespace scorpioweb.Models
{
    [Authorize]
    public class ArchivoController : Controller
    {

        #region -Variables Globales-
        private readonly penas2Context _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        #endregion

        #region -Metodos Generales-
        MetodosGenerales mg = new MetodosGenerales();
        #endregion

        #region -Constructor-
        public ArchivoController(penas2Context context, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,
                                  RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;

        }
        #endregion

        #region -Index-
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

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.User = user.ToString();
            ViewBag.Admin = false;
            ViewBag.Masteradmin = false;
            ViewBag.Archivo = false;


            List<Archivoprestamo> queryArchivoHistorial = (from a in _context.Archivoprestamo
                                                           group a by a.ArcchivoIdArchivo into grp
                                                           select grp.OrderByDescending(a => a.IdArchivoPrestamo).FirstOrDefault()).ToList();

            var filter = from a in _context.Archivo
                         join ap in queryArchivoHistorial on a.IdArchivo equals ap.ArcchivoIdArchivo into tmp
                         from left in tmp.DefaultIfEmpty()
                         select new ArchivoControlPrestamo
                         {
                             archivoVM = a,
                             archivoprestamoVM = left,
                         };



            ViewData["CurrentFilter"] = searchString;
            ViewData["EstadoS"] = estadoSuper;
            ViewData["FiguraJ"] = figuraJudicial;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = filter.Where(acp => (acp.archivoVM.Paterno + " " + acp.archivoVM.Materno + " " + acp.archivoVM.Nombre).Contains(searchString) ||
                                             (acp.archivoVM.Nombre + " " + acp.archivoVM.Materno + " " + acp.archivoVM.Paterno).Contains(searchString) ||
                                             (acp.archivoVM.Materno + " " + acp.archivoVM.Paterno + " " + acp.archivoVM.Nombre).Contains(searchString) ||
                                             (acp.archivoVM.IdArchivo.ToString()).Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(acp => acp.archivoVM.Paterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(acp => acp.archivoVM.Materno);
                    break;
                case "estado_cumplimiento_desc":
                    filter = filter.OrderByDescending(acp => acp.archivoVM.Nombre);
                    break;

            }

            filter = filter.OrderByDescending(spcp => spcp.archivoVM.IdArchivo);


            int pageSize = 10;
            return View(await PaginatedList<ArchivoControlPrestamo>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        #endregion

        #region -Buscar en registros-
        public async Task<JsonResult> BuscarAR(string var_buscar)
        {
            var listaNombres = _context.BuscarArchivoRegistros
                              .FromSql("CALL spBuscadorArchivoregistro('" + var_buscar + "' )")
                              .ToList();

            return Json(new { success = true, responseText = Convert.ToString(0), busqueda = listaNombres });
        }
        #endregion

        #region -Details-
        public async Task<IActionResult> Details(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var archivo = await _context.Archivo
                    .SingleOrDefaultAsync(m => m.IdArchivo == id);
                if (archivo == null)
                {
                    return NotFound();
                }

                return View(archivo);
            }
            #endregion

        #region -Create-
        public IActionResult Create()
        {
            return View();
        }
        #endregion

        #region -Create Persona Archivo-
        public JsonResult Createadd(int id, string nombre, string ap, string am,string yo, Archivo archivo)
        {
            bool create = false;
            var idExiste = (from a in _context.Archivo
                            where a.IdArchivo == id
                            select a.IdArchivo);

            if (!idExiste.Any())
            {
                try
                {
                    create = true;
                    archivo.IdArchivo = id;
                    archivo.Paterno = mg.removeSpaces(mg.normaliza(ap));
                    archivo.Materno = mg.removeSpaces(mg.normaliza(am));
                    archivo.Nombre = mg.removeSpaces(mg.normaliza(nombre));
                    archivo.Yo = mg.removeSpaces(mg.normaliza(yo));

                    _context.Add(archivo);
                    _context.SaveChanges();

                    return Json(new { success = true, responseText = Url.Action("Index", "Archivo"), create });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, responseText = Url.Action("Index", "Archivo"), create });
                }
            }
            else
            {
                return Json(new { success = false, responseText = Url.Action("Index", "Archivo"), create });
            }

            var stadoc = (from a in _context.Archivo
                          where a.IdArchivo == id
                          select a.IdArchivo).FirstOrDefault();

            return Json(new { success = false });
        }
        #endregion

        #region -Edit-
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivo = await _context.Archivo.SingleOrDefaultAsync(m => m.IdArchivo == id);
            if (archivo == null)
            {
                return NotFound();
            }
            return View(archivo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdArchivo,Paterno,Materno,Nombre,Urldocumento,ExpedienteUnicoIdExpedienteUnico")] Archivo archivo)
        {
            if (id != archivo.IdArchivo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                archivo.Paterno = mg.removeSpaces(mg.normaliza(archivo.Paterno));
                archivo.Materno = mg.removeSpaces(mg.normaliza(archivo.Materno));
                archivo.Nombre = mg.removeSpaces(mg.normaliza(archivo.Nombre));
                try
                {
                    _context.Update(archivo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArchivoExists(archivo.IdArchivo))
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
            return View(archivo);
        }
        #endregion

        #region -Delete-
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivo = await _context.Archivo
                .SingleOrDefaultAsync(m => m.IdArchivo == id);
            if (archivo == null)
            {
                return NotFound();
            }

            return View(archivo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var archivo = await _context.Archivo.SingleOrDefaultAsync(m => m.IdArchivo == id);
            _context.Archivo.Remove(archivo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public JsonResult antesdelete(int id)
        {
            var borrar = false;

            var antesdel = from a in _context.Archivo
                           join ar in _context.Archivoregistro on a.IdArchivo equals ar.ArchivoIdArchivo
                           where a.IdArchivo == id
                           select a;

            if (antesdel.Any())
            {
                return Json(new { success = true, responseText = Url.Action("Index", "Archivo"), borrar = borrar });
            }
            else
            {
                borrar = true;
                return Json(new { success = true, responseText = Url.Action("Index", "Archivo"), borrar = borrar });
            }
            var stadoc = (from a in _context.Archivo
                          where a.IdArchivo == id
                          select a.IdArchivo).FirstOrDefault();

            return Json(new { success = true, responseText = Convert.ToString(stadoc) });
        }

        public JsonResult deletesuper(Archivo archivo, Historialeliminacion historialeliminacion, string[] datosuper)
        {
            var borrar = false;
            var id = Int32.Parse(datosuper[0]);
            var razon = mg.normaliza(datosuper[1]);
            var user = mg.normaliza(datosuper[2]);

            var query = (from a in _context.Archivo
                         where a.IdArchivo == id
                         select a).FirstOrDefault();

            try
            {
                borrar = true;
                historialeliminacion.Id = id;
                historialeliminacion.Descripcion = "IDARCHIVO= " + query.IdArchivo + "NOMBREL= " + query.Paterno + " " + query.Materno + " " + query.Materno;
                historialeliminacion.Tipo = "HISTORIALARCHIVO";
                historialeliminacion.Razon = mg.normaliza(razon);
                historialeliminacion.Usuario = mg.normaliza(user);
                historialeliminacion.Fecha = DateTime.Now;
                historialeliminacion.Supervisor = "NA";
                _context.Add(historialeliminacion);
                _context.SaveChanges();

                _context.Database.ExecuteSqlCommand("CALL spBorrarRegistroArchivo(" + id + ")");
                return Json(new { success = true, responseText = Url.Action("index", "Personas"), borrar = borrar });
            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("index", "Archivo"), borrar = borrar });
            }

            var stadoc = (from a in _context.Archivo
                          where a.IdArchivo == id
                          select a.IdArchivo).FirstOrDefault();

            return Json(new { success = true, responseText = Convert.ToString(stadoc) });
        }
        #endregion

        #region -ArchivoExists-
        private bool ArchivoExists(int id)
        {
            return _context.Archivo.Any(e => e.IdArchivo == id);
        }
        #endregion

        #region -ArchivoMenu-
        public IActionResult ArchivoMenu()
        {
            return View();
        } 
        #endregion

        #region -Create Archivo-
        public async Task<IActionResult> CreateArchivo(int id)
        {
            ViewBag.idArchivo = id;
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

            List<Areas> listaGeneral = new List<Areas>();
            listaGeneral = (from table in _context.Areas
                            select table).ToList();
            ViewBag.ListaGeneral = listaGeneral;

            var snArchivos = await _context.Archivoregistro.Where(m => m.ArchivoIdArchivo == id).ToListAsync();
            if (snArchivos.Count != 0)
            {
                return RedirectToAction("EditArchivo", new {id});
            }   

            return View();
        }
        public async Task<IActionResult> CreateArchivo2(int id)
        {
            ViewBag.idArchivo = id;
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

            List<Areas> listaGeneral = new List<Areas>();
            listaGeneral = (from table in _context.Areas
                            select table).ToList();
            ViewBag.ListaGeneral = listaGeneral;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArchivo([Bind("CausaPenal,Delito,Situacion,Sentencia,FechaAcuerdo,Observaciones,CarpetaEjecucion,Envia,Reasignacion,ArcchivoIdArchivo")] Archivoregistro archivoregistro,Archivo archivo,  int archivoIdArchivo, IFormFile archivoFile)
        {

            if (ModelState.IsValid)
            {

                int idArchivo = ((from table in _context.Archivoregistro
                                   select table.IdArchivoRegistro).Max() + 1);

                var sacarnomEnvia = (from a in _context.Areas
                                     where a.IdArea == int.Parse(archivoregistro.Envia)
                                     select a.UserName).First();

                archivoregistro.CausaPenal = mg.normaliza(archivoregistro.CausaPenal.ToString());
                archivoregistro.Delito = mg.normaliza(archivoregistro.Delito.ToString());
                archivoregistro.Situacion = mg.normaliza(archivoregistro.Situacion.ToString());
                archivoregistro.Sentencia = mg.normaliza(archivoregistro.Sentencia);
                archivoregistro.FechaAcuerdo = archivoregistro.FechaAcuerdo;
                archivoregistro.Observaciones = mg.normaliza(archivoregistro.Observaciones);
                archivoregistro.CarpetaEjecucion = mg.normaliza(archivoregistro.CarpetaEjecucion);
                archivoregistro.Envia = mg.normaliza(sacarnomEnvia.ToString());
                archivoregistro.ArchivoIdArchivo = archivoIdArchivo;
                //archivoregistro.Reasignacion = 
           

                if (archivoFile == null)
                {

                }
                else
                {
                    var nombre = (from a in _context.Archivo
                                 where a.IdArchivo == archivoIdArchivo
                                 select a).ToString();

                    string file_name = archivoregistro.ArchivoIdArchivo + "_" + idArchivo + Path.GetExtension(archivoFile.FileName); ;
                    archivoregistro.Urldocumento = file_name;
                    var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Expedientes");

                    if (System.IO.File.Exists(Path.Combine(uploads, file_name)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, file_name));
                    }

                    var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                    await archivoFile.CopyToAsync(stream);
                    stream.Close();
                }

                _context.Add(archivoregistro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(archivoregistro);
        }

        public async Task<IActionResult> EditArchivo(int id)
        {

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.User = user.ToString();
            ViewBag.Admin = false;
            ViewBag.Masteradmin = false;
            ViewBag.Archivo = false;


            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

            List<Areas> listaGeneral = new List<Areas>();
            listaGeneral = (from table in _context.Areas
                            where !table.Area.EndsWith("\u0040nortedgepms.com")
                            select new Areas{
                                IdArea = table.IdArea,
                                UserName = table.UserName
                            }).ToList();

            ViewBag.ListaGeneral = listaGeneral;

            ViewData["tienearchivo"] = from a in _context.Archivo
                                       join ar in _context.Archivoregistro on a.IdArchivo equals ar.ArchivoIdArchivo
                                       where a.IdArchivo == id
                                       select new ArchivoControlPrestamo
                                       {
                                           archivoregistroVM = ar,
                                           archivoVM = a,
                                        };

            return View();  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArchivo(int id, [Bind("IdArchivoRegistro,CausaPenal,Delito,Situacion,Sentencia,FechaAcuerdo,Observaciones,CarpetaEjecucion,Envia, Otro, ArcchivoIdArchivoo")] Archivoregistro archivoregistro, int archivoIdArchivo, IFormFile archivoFile)
        {

            if (id != archivoregistro.IdArchivoRegistro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                var oldArchivo = await _context.Archivoregistro.FindAsync(archivoregistro.IdArchivoRegistro);
    
                archivoregistro.Envia = archivoregistro.Envia != null ? mg.normaliza(archivoregistro.Envia.ToString()) : "";
                archivoregistro.ArchivoIdArchivo = archivoIdArchivo;
                archivoregistro.CausaPenal = mg.normaliza(archivoregistro.CausaPenal.ToString());
                archivoregistro.Delito = mg.normaliza(archivoregistro.Delito.ToString());
                archivoregistro.Situacion = mg.normaliza(archivoregistro.Situacion.ToString());
                archivoregistro.Sentencia = mg.normaliza(archivoregistro.Sentencia);
                archivoregistro.FechaAcuerdo = archivoregistro.FechaAcuerdo;
                archivoregistro.Observaciones = mg.normaliza(archivoregistro.Observaciones);
                archivoregistro.CarpetaEjecucion = mg.normaliza(archivoregistro.CarpetaEjecucion);
                archivoregistro.Otro = mg.normaliza(archivoregistro.Otro);


                if (archivoFile == null)
                {
                    archivoregistro.Urldocumento = oldArchivo.Urldocumento;
                }
                else
                {
              
                    string file_name = oldArchivo.ArchivoIdArchivo + "_" + oldArchivo.IdArchivoRegistro + Path.GetExtension(archivoFile.FileName); ;
                    archivoregistro.Urldocumento = file_name;
                    var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Expedientes");

                    if (System.IO.File.Exists(Path.Combine(uploads, file_name)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, file_name));
                    }
                    var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                    await archivoFile.CopyToAsync(stream);
                    stream.Close();
                }

                try
                {
                    _context.Entry(oldArchivo).CurrentValues.SetValues(archivoregistro);

                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArchivoExists(archivoregistro.IdArchivoRegistro))
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
            return View(archivoregistro);
        }

        public async Task<IActionResult> DeleteArchivo(int? id)
        {
            var Archivoregistro = await _context.Archivoregistro.SingleOrDefaultAsync(m => m.IdArchivoRegistro == id);
            


            _context.Archivoregistro.Remove(Archivoregistro);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index" , "Archivo");

        }

        #endregion 

        #region -Prestamo-
        public async Task<IActionResult> CreatePrestamo(int id, Archivoprestamo archivoprestamo, Archivo archivo, Areas areas)
        {
            ViewBag.idArchivo = id;

            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.User = user.ToString();

            List<Archivo> archivos = _context.Archivo.ToList();
            List<Archivoprestamo> archivoprestamos = _context.Archivoprestamo.ToList();

            List<Areas> listausuarios = new List<Areas>();
            listausuarios = (from table in _context.Areas
                             where table.Area == "ARCHIVO"
                             select table).ToList();
            ViewBag.ListaUsuarios = listausuarios;


            List<Areas> listaGeneral = new List<Areas>();
            listaGeneral = (from table in _context.Areas
                            where !table.Area.EndsWith("\u0040nortedgepms.com")
                            select table).ToList();
            ViewBag.ListaGeneral = listaGeneral;

            return View();
        }

        // POST: Archivo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePrestamo([Bind("Entrega,Recibe,Area,FechaInicial,FechaRenovacion,Estatus,Renovaciones,Urlvale,ArcchivoIdArchivo")] Archivoprestamo archivoprestamo, int archivoIdArchivo, int optradio, Archivoprestamodigital archivoprestamodigital)
        {
            if (ModelState.IsValid)
            {

                if (optradio == 1)
                {
                    try
                    {
                        var sacarnomRecibe = (from a in _context.Areas
                                              where a.IdArea == int.Parse(archivoprestamo.Recibe)
                                              select a.UserName).First();

                        var sacarnomArea = (from a in _context.Areas
                                            where a.IdArea == int.Parse(archivoprestamo.Recibe)
                                            select a.Area).First();

                        archivoprestamo.Entrega = mg.normaliza(archivoprestamo.Entrega);
                        archivoprestamo.Recibe = mg.normaliza(sacarnomRecibe.ToString());
                        archivoprestamo.Area = mg.normaliza(sacarnomArea.ToString());
                        archivoprestamo.FechaInicial = DateTime.Now;
                        archivoprestamo.FechaRenovacion = DateTime.Now.AddMonths(1);
                        archivoprestamo.Estatus = "PRESTADO";
                        archivoprestamo.Renovaciones = archivoprestamo.Renovaciones;
                        archivoprestamo.Urlvale = archivoprestamo.Urlvale;
                        archivoprestamo.ArcchivoIdArchivo = archivoIdArchivo;

                        _context.Add(archivoprestamo);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, responseText = "Error al guardar " + ex });
                    }

                }
                else
                {

                    try
                    {
                        var sacarnomRecibe = (from a in _context.Areas
                                              where a.IdArea == int.Parse(archivoprestamo.Recibe)
                                              select a.UserName).First();

                        var sacarnomArea = (from a in _context.Areas
                                            where a.IdArea == int.Parse(archivoprestamo.Recibe)
                                            select a.Area).First();

                        archivoprestamodigital.ArchivoIdArchivo = archivoIdArchivo;
                        archivoprestamodigital.Usuario = mg.normaliza(sacarnomRecibe.ToString());
                        archivoprestamodigital.UsuarioOtorgaPermiso = mg.normaliza(archivoprestamo.Entrega);
                        archivoprestamodigital.FechaPrestamo = DateTime.Now;
                        archivoprestamodigital.FechaCierre = DateTime.Now.AddDays(7);
                        _context.Add(archivoprestamodigital);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, responseText = "Error al guardar " + ex });
                    }

                }

                return RedirectToAction(nameof(Index));
            }
            return View(archivoprestamo);
        }

        public async Task<IActionResult> EditPrestamo(int id, int IdPrestamo)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivoprestamo = await _context.Archivoprestamo.SingleOrDefaultAsync(m => m.IdArchivoPrestamo == id);

            var Entrega = (from sa in _context.Areas
                           where sa.UserName == archivoprestamo.Entrega.ToLower()
                           select sa.IdArea).First().ToString();

            ViewBag.idArchivo = archivoprestamo.ArcchivoIdArchivo;
            List<Areas> listausuarios = new List<Areas>();
            listausuarios = (from table in _context.Areas
                             where table.Area == "ARCHIVO"
                             select table).ToList();
            ViewBag.ListaUsuarios = listausuarios;
            ViewBag.userEntrega = Entrega;

            var Recibe = (from sa in _context.Areas
                          where sa.UserName == archivoprestamo.Recibe.ToLower()
                          select sa.IdArea).First().ToString();

            ViewBag.idArchivo = archivoprestamo.ArcchivoIdArchivo;
            List<Areas> listaGeneral = new List<Areas>();
            listaGeneral = (from table in _context.Areas
                            where !table.Area.EndsWith("\u0040nortedgepms.com")
                            select table).ToList();
            ViewBag.listaGeneral = listaGeneral;
            ViewBag.userGeneral = Recibe;

            List<SelectListItem> ListaEstatus;
            ListaEstatus = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Prestado", Value = "PRESTADO" },
                new SelectListItem{ Text = "Entregado", Value = "ENTREGADO" },
            };
            ViewBag.ListaEstatus = ListaEstatus;
            ViewBag.idEstatus = mg.BuscaId(ListaEstatus, archivoprestamo.Estatus);

            if (archivoprestamo == null)
            {
                return NotFound();
            }
            return View(archivoprestamo);
        }
        // POST: Archivo/EditPrestamo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPrestamo(int id, [Bind("IdArchivoPrestamo,Entrega,Recibe,Area,FechaInicial,FechaRenovacion,Estatus,Renovaciones,Urlvale,ArcchivoIdArchivo")] Archivoprestamo archivoprestamo, int archivoIdArchivo)
        {

            if (id != archivoprestamo.IdArchivoPrestamo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                var sacarnomEntrega = (from a in _context.Areas
                                       where a.IdArea == int.Parse(archivoprestamo.Entrega)
                                       select a.UserName).First();

                var sacarnomRecibe = (from a in _context.Areas
                                      where a.IdArea == int.Parse(archivoprestamo.Recibe)
                                      select a.UserName).First();

                var sacarnomArea = (from a in _context.Areas
                                    where a.IdArea == int.Parse(archivoprestamo.Recibe)
                                    select a.Area).First();

                archivoprestamo.Entrega = mg.normaliza(sacarnomEntrega.ToString());
                archivoprestamo.Recibe = mg.normaliza(sacarnomRecibe.ToString());
                archivoprestamo.Area = mg.normaliza(sacarnomArea.ToString());
                archivoprestamo.FechaInicial = archivoprestamo.FechaInicial;
                archivoprestamo.FechaRenovacion = archivoprestamo.FechaRenovacion;
                archivoprestamo.Estatus = mg.normaliza(archivoprestamo.Estatus);
                archivoprestamo.Renovaciones = archivoprestamo.Renovaciones;
                archivoprestamo.ArcchivoIdArchivo = archivoprestamo.ArcchivoIdArchivo;
                try
                {
                    _context.Update(archivoprestamo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArchivoExists(archivoprestamo.IdArchivoPrestamo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ArchivoControl));
            }
            return View(archivoprestamo);
        }

        public JsonResult Entrega(int id, Archivoprestamo archivoprestamo)
        {
            bool entrega = false;
            var empty = (from ap in _context.Archivoprestamo
                         where ap.IdArchivoPrestamo == id
                         select ap);

            try
            {
                if (empty.Any())
                {
                    entrega = true;
                    var query = (from ap in _context.Archivoprestamo
                                 where ap.IdArchivoPrestamo == id
                                 select ap).FirstOrDefault();
                    query.Estatus = "ENTREGADO";
                    _context.SaveChanges();

                }
            }
            catch
            {

                return Json(new { success = true, responseText = Convert.ToString(empty), idPersonas = Convert.ToString(id), entrega });
            }

            return Json(new { success = true, responseText = Convert.ToString(empty), idPersonas = Convert.ToString(id), entrega });
        }

        public JsonResult BorrarPrestado(int id)
        {
            var borrar = false;

            var empty = (from ap in _context.Archivoprestamo
                         where ap.IdArchivoPrestamo == id
                         select ap);

            try
            {
                if (empty.Any())
                {
                    borrar = true;
                    var query = (from ap in _context.Archivoprestamo
                                 where ap.IdArchivoPrestamo == id
                                 select ap).FirstOrDefault();
                    query.Estatus = "BORRADO";
                    _context.SaveChanges();

                }
            }
            catch
            {

                return Json(new { success = true, responseText = Convert.ToString(empty), idPersonas = Convert.ToString(id), borrar });
            }

            return Json(new { success = true, responseText = Convert.ToString(empty), idPersonas = Convert.ToString(id), borrar });
        }


        #endregion

        #region -ArchivoControl-
        public async Task<IActionResult> ArchivoControl(
           string sortOrder,
           string currentFilter,
           string SearchString,
           string estadoSuper,
           int? pageNumber
           )
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CausaPenalSortParm"] = String.IsNullOrEmpty(sortOrder) ? "causa_penal_desc" : "";
            ViewData["EstadoCumplimientoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "estado_cumplimiento_desc" : "";

            if (SearchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                SearchString = currentFilter;
            }

            var filter = from a in _context.Archivo
                         join ap in _context.Archivoprestamo on a.IdArchivo equals ap.ArcchivoIdArchivo
                         where ap.Estatus == "PRESTADO"
                         select new ArchivoControlPrestamo
                         {
                             archivoVM = a,
                             archivoprestamoVM = ap
                         };

            ViewData["CurrentFilter"] = SearchString;
            ViewData["EstadoS"] = estadoSuper;

            if (!String.IsNullOrEmpty(SearchString))
            {
                filter = filter.Where(a => (a.archivoVM.Paterno + " " + a.archivoVM.Materno + " " + a.archivoVM.Nombre).Contains(SearchString) ||
                                              (a.archivoVM.Nombre + " " + a.archivoVM.Paterno + " " + a.archivoVM.Materno).Contains(SearchString) ||
                                              (a.archivoVM.IdArchivo.ToString()).Contains(SearchString)
                                              );

            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(a => a.archivoVM.Nombre);
                    break;
                case "ap_ase":
                    filter = filter.OrderBy(a => a.archivoVM.Paterno);
                    break;
                case "am_ase":
                    filter = filter.OrderByDescending(a => a.archivoVM.Paterno);
                    break;
                default:
                    filter = filter.OrderByDescending(spcp => spcp.archivoVM.Nombre);
                    break;
            }
            int pageSize = 100;

            //var queryable = query2.AsQueryable();
            return View(await PaginatedList<ArchivoControlPrestamo>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        #endregion

        #region -GetAreaUser-
        public JsonResult GetAreaUser(int AreaId)
        {
            TempData["message"] = DateTime.Now;
            List<Areas> areas = new List<Areas>();

            areas = (from Areas in _context.Areas
                     where Areas.IdArea == AreaId
                     select Areas).ToList();

            return Json(new SelectList(areas, "IdArea", "Area"));
        } 
        #endregion

        #region -Historial de Archivo-
        public async Task<IActionResult> ArchivoHistorial(
           string sortOrder,
           string currentFilter,
           string SearchString,
           string estadoSuper,
           int? pageNumber
           )
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CausaPenalSortParm"] = String.IsNullOrEmpty(sortOrder) ? "causa_penal_desc" : "";
            ViewData["EstadoCumplimientoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "estado_cumplimiento_desc" : "";

            if (SearchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                SearchString = currentFilter;
            }

            var filter = from a in _context.Archivo
                         join ap in _context.Archivoprestamo on a.IdArchivo equals ap.ArcchivoIdArchivo
                         where ap.Estatus == "ENTREGADO"
                         select new ArchivoControlPrestamo
                         {
                              archivoVM= a,
                             archivoprestamoVM = ap
                         };

            ViewData["CurrentFilter"] = SearchString;
            ViewData["EstadoS"] = estadoSuper;

            if (!String.IsNullOrEmpty(SearchString))
            {
                filter = filter.Where(a => (a.archivoVM.Paterno + " " + a.archivoVM.Materno + " " + a.archivoVM.Nombre).Contains(SearchString) ||
                                              (a.archivoVM.Nombre + " " + a.archivoVM.Paterno + " " + a.archivoVM.Materno).Contains(SearchString) ||
                                              (a.archivoVM.IdArchivo.ToString()).Contains(SearchString)
                                              );

            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(a => a.archivoVM.Nombre);
                    break;
                case "ap_ase":
                    filter = filter.OrderBy(a => a.archivoVM.Paterno);
                    break;
                case "am_ase":
                    filter = filter.OrderByDescending(a => a.archivoVM.Paterno);
                    break;
                default:
                    filter = filter.OrderByDescending(a => a.archivoVM.Nombre);
                    break;
            }
            int pageSize = 100;

            //var queryable = query2.AsQueryable();
            return View(await PaginatedList<ArchivoControlPrestamo>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        #endregion

        #region -Prestamo Digital-
        public async Task<IActionResult> ArchivoPrestamoDigital(
           string sortOrder,
           string currentFilter,
           string SearchString,
           string estadoSuper,
           int? pageNumber
           )
        {

            ViewData["CurrentSort"] = sortOrder; 
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CausaPenalSortParm"] = String.IsNullOrEmpty(sortOrder) ? "causa_penal_desc" : "";
            ViewData["EstadoCumplimientoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "estado_cumplimiento_desc" : "";

            if (SearchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                SearchString = currentFilter;
            }

            IQueryable<ArchivoControlPrestamo> filter;

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.User = user.ToString();
            ViewBag.Admin = false;
            ViewBag.Masteradmin = false;
            ViewBag.Archivo = false;

            DateTime date = DateTime.Now;


            if(roles.ToString() == "Masteradmin" || roles.ToString() == "Archivo"){
                filter = from a in _context.Archivo
                             join ap in _context.Archivoprestamodigital on a.IdArchivo equals ap.ArchivoIdArchivo
                             select new ArchivoControlPrestamo
                             {
                                 archivoVM = a,
                                 archivoprestamodigitalVM = ap
                             };
            }
            else
            {
                filter = from a in _context.Archivo
                         join ap in _context.Archivoprestamodigital on a.IdArchivo equals ap.ArchivoIdArchivo
                         where date < ap.FechaCierre
                         select new ArchivoControlPrestamo
                         {
                             archivoVM = a,
                             archivoprestamodigitalVM = ap
                         };
            }

            

            ViewData["CurrentFilter"] = SearchString;
            ViewData["EstadoS"] = estadoSuper;

            if (!String.IsNullOrEmpty(SearchString))
            {
                filter = filter.Where(a => (a.archivoVM.Paterno + " " + a.archivoVM.Materno + " " + a.archivoVM.Nombre).Contains(SearchString) ||
                                              (a.archivoVM.Nombre + " " + a.archivoVM.Paterno + " " + a.archivoVM.Materno).Contains(SearchString) ||
                                              (a.archivoVM.IdArchivo.ToString()).Contains(SearchString)
                                              );

            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(a => a.archivoVM.Nombre);
                    break;
                case "ap_ase":
                    filter = filter.OrderBy(a => a.archivoVM.Paterno);
                    break;
                case "am_ase":
                    filter = filter.OrderByDescending(a => a.archivoVM.Paterno);
                    break;
                default:
                    filter = filter.OrderByDescending(a => a.archivoVM.Nombre);
                    break;
            }
            int pageSize = 100;

            //var queryable = query2.AsQueryable();
            return View(await PaginatedList<ArchivoControlPrestamo>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        public async Task<IActionResult> ArchivoCausas(int id)
        {

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.User = user.ToString();
            ViewBag.Admin = false;
            ViewBag.Masteradmin = false;
            ViewBag.Archivo = false;


            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

            List<Areas> listaGeneral = new List<Areas>();
            listaGeneral = (from table in _context.Areas
                            where !table.Area.EndsWith("\u0040nortedgepms.com")
                            select new Areas
                            {
                                IdArea = table.IdArea,
                                UserName = table.UserName
                            }).ToList();

            ViewBag.ListaGeneral = listaGeneral;

            ViewData["tienearchivo"] = from a in _context.Archivo
                                       join ar in _context.Archivoregistro on a.IdArchivo equals ar.ArchivoIdArchivo
                                       join area in _context.Areas on ar.Envia equals area.UserName
                                       where a.IdArchivo == id
                                       select new ArchivoControlPrestamo
                                       {
                                           archivoregistroVM = ar,
                                           archivoVM = a,
                                           areasVM = area,
                                       };

            return View();
        }

        #endregion

        #region -Reasignar-
        public JsonResult Bucaid(Archivo archivo, Archivoregistro archivoregistro, string datoidArchivo)
        {
            var update = false;
            var id = Int32.Parse(datoidArchivo);

            var buscaid = from a in _context.Archivo
                           where a.IdArchivo == id
                           select a;

            if (!buscaid.Any())
            {
                return Json(new { success = true, responseText = Url.Action("EditArchivo", "Archivo"), update = update });
            }
            else
            {
                update = true;

                var nombre = (from a in _context.Archivo
                             where a.IdArchivo == id
                             select new Archivo
                             {
                                 Nombre = a.Nombre,
                                 Paterno = a.Paterno,
                                 Materno = a.Materno
                             }).ToList();

                return Json(new { success = true, responseText = Url.Action("EditArchivo", "Archivo"), update = update, nombre = nombre }); ;
            }
            var stadoc = (from s in _context.Archivo
                          where s.IdArchivo == id
                          select s.IdArchivo).FirstOrDefault();

            return Json(new { success = true, responseText = Convert.ToString(stadoc), idSupervision = Convert.ToString(id) });
        }
        public JsonResult Reasignar(Archivoregistro archivoregistro, string datoidArchivo, string datoidArchivoregistro)
        {
            var update = false;
            int idd = Int32.Parse(datoidArchivoregistro);
            int archivoId = Int32.Parse(datoidArchivo);

            var aregistro = (from ar in _context.Archivoregistro
                            where ar.IdArchivoRegistro == idd
                            select new Archivoregistro
                            {
                                CausaPenal = ar.CausaPenal,
                                Delito = ar.Delito,
                                Sentencia = ar.Sentencia,
                                Situacion = ar.Situacion,
                                FechaAcuerdo = ar.FechaAcuerdo,
                                CarpetaEjecucion = ar.CarpetaEjecucion,
                                Observaciones = ar.Observaciones,
                                Envia = ar.Envia,
                                Urldocumento = ar.Urldocumento,
                                Otro = ar.Otro
                            }).ToList();

            archivoregistro.IdArchivoRegistro = idd;
            archivoregistro.ArchivoIdArchivo = archivoId;
            archivoregistro.CausaPenal = aregistro[0].CausaPenal; 
            archivoregistro.Delito = aregistro[0].Delito; 
            archivoregistro.Sentencia = aregistro[0].Sentencia; 
            archivoregistro.Situacion = aregistro[0].Situacion; 
            archivoregistro.FechaAcuerdo = aregistro[0].FechaAcuerdo; 
            archivoregistro.CarpetaEjecucion = aregistro[0].CarpetaEjecucion; 
            archivoregistro.Observaciones = aregistro[0].Observaciones; 
            archivoregistro.Envia = aregistro[0].Envia; 
            archivoregistro.Urldocumento = aregistro[0].Urldocumento; 
            archivoregistro.Otro = aregistro[0].Otro; 


            try
            {
                update = true;
                _context.Update(archivoregistro);
                _context.SaveChanges();
                return Json(new { success = true, responseText = Url.Action("EditArchivo", "Archivo"), update = update });
            }
            catch (Exception ex)
            {
                var error = ex;
                return Json(new { success = true, responseText = Url.Action("EditArchivo", "Archivo"), update = update, error = error });
            }

            var stadoc = (from c in _context.Archivoregistro
                          where c.IdArchivoRegistro == idd
                          select c.IdArchivoRegistro).FirstOrDefault();

            return Json(new { success = true, responseText = Convert.ToString(stadoc)});
        }
        #endregion

    }
}
