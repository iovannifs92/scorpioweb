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
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using scorpioweb.Migrations.ApplicationDb;
using Newtonsoft.Json;
using Syncfusion.EJ2.Linq;

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
        private readonly MetodosGenerales mg = new MetodosGenerales();
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

            var warningSolicitud = from a in _context.Archivo
                                   where a.Solucitud == 1
                                   select a;

            ViewBag.Warnings = warningSolicitud.Count();


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

            if (roles.ToString() == "Masteradmin" || roles.ToString() == "Archivo" || roles.ToString() == "Director")
            {
                ViewBag.Archivo = true;
            }


            List<Archivoprestamo> queryArchivoHistorial = (from a in _context.Archivoprestamo
                                                           group a by a.ArcchivoIdArchivo into grp
                                                           select grp.OrderByDescending(a => a.IdArchivoPrestamo).FirstOrDefault()).ToList();

            List<Archivoprestamodigital> archivoprestamodigitals = (from a in _context.Archivoprestamodigital
                                                                    group a by a.ArchivoIdArchivo into gtp
                                                                    select gtp.OrderByDescending(a => a.IdArchivoPrestamoDigital).FirstOrDefault()).ToList();

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
                                             (acp.archivoVM.Yo).Contains(searchString) ||
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

        #region -Busqueda en registros-
        public JsonResult BuscarAR(string var_buscar)
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
        public JsonResult Createadd(int id, string nombre, string ap, string am, string yo, string condicion, Archivo archivo, Expedienteunico expedienteunico, string tabla, string idselecionado, string CURS, string datosArray)
        {
            bool create = false;
            var idExiste = (from a in _context.Archivo
                            where a.IdArchivo == id
                            select a.IdArchivo);

            if (id != 0 && !idExiste.Any())
            {
                try
                {
                    create = true;
                    archivo.IdArchivo = id;
                    archivo.Paterno = mg.removeSpaces(mg.normaliza(ap));
                    archivo.Materno = mg.removeSpaces(mg.normaliza(am));
                    archivo.Nombre = mg.removeSpaces(mg.normaliza(nombre));
                    archivo.Yo = mg.removeSpaces(mg.normaliza(yo));
                    archivo.CondicionEspecial = mg.removeSpaces(mg.normaliza(condicion));


                    //#region -Expediente Unico-

                    //string var_tablanueva = "";
                    //string var_tablaSelect = "";
                    //string var_tablaCurs = "";
                    //int var_idnuevo = 0;
                    //int var_idSelect = 0;
                    //string var_curs = "";

                    //if (idselecionado != null && tabla != null)
                    //{
                    //    var_tablanueva = mg.cambioAbase(mg.RemoveWhiteSpaces("Archivo"));
                    //    var_tablaSelect = mg.cambioAbase(mg.RemoveWhiteSpaces(tabla));
                    //    var_tablaCurs = "ClaveUnicaScorpio";
                    //    var_idnuevo = id;
                    //    var_idSelect = Int32.Parse(idselecionado);
                    //    var_curs = CURS;
                    //    archivo.ClaveUnicaScorpio = CURS;

                    //    string query = $"CALL spInsertExpedienteUnico('{var_tablanueva}', '{var_tablaSelect}', '{var_tablaCurs}', {var_idnuevo}, {var_idSelect},  '{var_curs}');";
                    //    _context.Database.ExecuteSqlCommand(query);


                    //    if (datosArray != null)
                    //    {
                    //        List<Dictionary<string, string>> listaObjetos = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(datosArray);

                    //        // Proyectar la lista para obtener solo los valores de id y tabla
                    //        var resultados = listaObjetos.Select(obj => new {
                    //            id = obj["id"],
                    //            tabla = obj["tabla"]
                    //        });

                    //        foreach (var resultado in resultados)
                    //        {
                    //            var_tablanueva = mg.cambioAbase(mg.RemoveWhiteSpaces("Archivo"));
                    //            var_tablaSelect = mg.cambioAbase(mg.RemoveWhiteSpaces(resultado.tabla));
                    //            var_tablaCurs = "ClaveUnicaScorpio";
                    //            var_idnuevo = id;
                    //            var_idSelect = Int32.Parse(resultado.id);

                    //            string query2 = $"CALL spInsertExpedienteUnico('{var_tablanueva}', '{var_tablaSelect}', '{var_tablaCurs}', {var_idnuevo}, {var_idSelect},  '{var_curs}');";
                    //            _context.Database.ExecuteSqlCommand(query2);
                    //        }
                    //    }
                    //}
                    //#endregion

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
        public async Task<IActionResult> Edit(int id, [Bind("IdArchivo,Paterno,Materno,Nombre,Yo,Urldocumento, CondicionEspecial, ExpedienteUnicoIdExpedienteUnico, Solucitud, QuienSolicita,ClaveUnicaScorpio")] Archivo archivo)
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
                archivo.Yo = mg.removeSpaces(mg.normaliza(archivo.Yo));
                archivo.CondicionEspecial = mg.removeSpaces(mg.normaliza(archivo.CondicionEspecial));
                archivo.Solucitud = archivo.Solucitud;
                archivo.QuienSolicita = archivo.QuienSolicita;
                archivo.ClaveUnicaScorpio = archivo.ClaveUnicaScorpio;
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

            #region -Editar en expediente unico-
            try
            {
                var ideu = (from eu in _context.Expedienteunico
                            where Int64.Parse(eu.Archivo) == id
                            select eu.IdexpedienteUnico).FirstOrDefault();

                var query = (from s in _context.Expedienteunico
                             where s.IdexpedienteUnico == ideu
                             select s).FirstOrDefault();
                query.Archivo = null;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            #endregion

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
                return Json(new { success = true, responseText = Url.Action("Index", "Archivo"), borrar });
            }
            else
            {
                borrar = true;
                return Json(new { success = true, responseText = Url.Action("Index", "Archivo"), borrar });
            }
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

                #region -Editar en expediente unico-
                try
                {
                    var ideu = (from eu in _context.Expedienteunico
                                where Int64.Parse(eu.Archivo) == id
                                select eu.IdexpedienteUnico).FirstOrDefault();

                    var queryB = (from s in _context.Expedienteunico
                                  where s.IdexpedienteUnico == ideu
                                  select s).FirstOrDefault();
                    queryB.Archivo = null;
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {

                }
                #endregion

                _context.Database.ExecuteSqlCommand("CALL spBorrarRegistroArchivo(" + id + ")");
                return Json(new { success = true, responseText = Url.Action("index", "Personas"), borrar });
            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("index", "Archivo"), borrar });
            }
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
            var user = User.Identity.Name;
            var warningSolicitud = from a in _context.Archivo
                                   where a.Solucitud == 1
                                   select a;

            ViewBag.Warnings = warningSolicitud.Count();
            #region -Solicitud Atendida Archivo prestamo Digital-
            var warningRespuesta = from a in _context.Archivoprestamodigital
                                   where a.EstadoPrestamo == 1 && user.ToUpper() == a.Usuario.ToUpper()
                                   select a;
            ViewBag.WarningsUser = warningRespuesta.Count();
            #endregion

            return View();
        }
        #endregion

        #region -Create Archivo-
        public async Task<IActionResult> CreateArchivo(int id)
        {
            ViewBag.idArchivo = id;
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

            List<string> ListaCoordinadores = new List<string>();
            foreach (var u in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(u, "Oficialia"))
                {
                    ListaCoordinadores.Add(u.ToString());
                }
                if (await userManager.IsInRoleAsync(u, "Director"))
                {
                    ListaCoordinadores.Add(u.ToString());
                }
                if (await userManager.IsInRoleAsync(u, "Coordinador"))
                {
                    ListaCoordinadores.Add(u.ToString());
                }
            }

            ViewBag.ListaGeneral = ViewBag.ListaGeneral = ListaCoordinadores.Where(r => ListaCoordinadores.Any(f => !r.EndsWith("\u0040nortedgepms.com")));

            var snArchivos = await _context.Archivoregistro.Where(m => m.ArchivoIdArchivo == id).ToListAsync();
            if (snArchivos.Count != 0)
            {
                return RedirectToAction("EditArchivo", new { id });
            }

            return View();
        }
        public async Task<IActionResult> CreateArchivo2(int id)
        {
            ViewBag.idArchivo = id;
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

            List<string> ListaCoordinadores = new List<string>();
            foreach (var u in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(u, "Oficialia"))
                {
                    ListaCoordinadores.Add(u.ToString());
                }
                if (await userManager.IsInRoleAsync(u, "Director"))
                {
                    ListaCoordinadores.Add(u.ToString());
                }
                if (await userManager.IsInRoleAsync(u, "Coordinador"))
                {
                    ListaCoordinadores.Add(u.ToString());
                }
            }

            //!table.Area.EndsWith("\u0040nortedgepms.com")


            ViewBag.ListaGeneral = ListaCoordinadores.Where(r => ListaCoordinadores.Any(f => !r.EndsWith("\u0040nortedgepms.com")));

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArchivo([Bind("CausaPenal,Delito,Situacion,Sentencia,FechaAcuerdo,Observaciones,CarpetaEjecucion,Envia,Reasignacion,Otro,ArchivoIdArchivo")] Archivoregistro archivoregistro, Archivo archivo, int archivoIdArchivo, IFormFile archivoFile)
        {
            if (ModelState.IsValid)
            {
                int idArchivo = ((from table in _context.Archivoregistro
                                  select table.IdArchivoRegistro).Max() + 1);

                archivoregistro.CausaPenal = mg.normaliza(archivoregistro.CausaPenal);
                archivoregistro.Delito = mg.normaliza(archivoregistro.Delito);
                archivoregistro.Situacion = mg.normaliza(archivoregistro.Situacion);
                archivoregistro.Sentencia = mg.normaliza(archivoregistro.Sentencia);
                archivoregistro.FechaAcuerdo = archivoregistro.FechaAcuerdo;
                archivoregistro.Observaciones = mg.normaliza(archivoregistro.Observaciones);
                archivoregistro.CarpetaEjecucion = mg.normaliza(archivoregistro.CarpetaEjecucion);
                archivoregistro.Envia = mg.normaliza(archivoregistro.Envia);
                archivoregistro.ArchivoIdArchivo = archivoIdArchivo;
                archivoregistro.Otro = archivoregistro.Otro;

                if (archivoFile == null)
                {
                    _context.Add(archivoregistro);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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


                    _context.Add(archivoregistro);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }


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
                            select new Areas
                            {
                                IdArea = table.IdArea,
                                UserName = table.UserName
                            }).ToList();

            ViewBag.ListaGeneral = listaGeneral;

            ViewData["tienearchivo"] = (from a in _context.Archivo
                                       join ar in _context.Archivoregistro on a.IdArchivo equals ar.ArchivoIdArchivo
                                       where a.IdArchivo == id
                                       select new ArchivoControlPrestamo
                                       {
                                           archivoregistroVM = ar,
                                           archivoVM = a,
                                       }).OrderBy(x => x.archivoregistroVM.CausaPenal);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArchivo(int id, [Bind("IdArchivoRegistro,CausaPenal,Delito,Situacion,Sentencia,FechaAcuerdo,Observaciones,CarpetaEjecucion,Envia,Otro,ArcchivoIdArchivoo")] Archivoregistro archivoregistro, int archivoIdArchivo, IFormFile archivoFile)
        {

            if (id != archivoregistro.IdArchivoRegistro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                var oldArchivo = await _context.Archivoregistro.FindAsync(archivoregistro.IdArchivoRegistro);

                archivoregistro.Envia = mg.normaliza(archivoregistro.Envia);
                archivoregistro.ArchivoIdArchivo = archivoIdArchivo;
                archivoregistro.CausaPenal = mg.normaliza(archivoregistro.CausaPenal);
                archivoregistro.Delito = mg.normaliza(archivoregistro.Delito);
                archivoregistro.Situacion = mg.normaliza(archivoregistro.Situacion);
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

        public JsonResult DeleteArchivo(Archivoregistro archivoregistro, int dato)
        {
            var borrar = false;

            var query = (from c in _context.Archivoregistro
                         where c.IdArchivoRegistro == dato
                         select c).FirstOrDefault();
            try
            {
                borrar = true;

                var ar = _context.Archivoregistro.FirstOrDefault(m => m.IdArchivoRegistro == dato);
                _context.Archivoregistro.Remove(ar);
                _context.SaveChanges();

                return Json(new { success = true, responseText = Url.Action("EditArchivo", "Archivo"), borrar });
            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("EditArchivo", "Archivo"), borrar });
            }
        }
        #endregion 

        #region -Prestamo-
        public async Task<IActionResult> CreatePrestamo(int? id, Archivoprestamo archivoprestamo, Archivo archivo, Areas areas)
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
        public async Task<IActionResult> CreatePrestamo([Bind("Entrega,Recibe,Area,FechaInicial,FechaRenovacion,Estatus,Renovaciones,Urlvale,ArcchivoIdArchivo")] Archivoprestamo archivoprestamo, int archivoIdArchivo, string usuario, int optradio, Archivoprestamodigital archivoprestamodigital)
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
                        archivoprestamodigital.EstadoPrestamo = 1;
                        var query = (from a in _context.Archivo
                                     where a.IdArchivo == archivoIdArchivo
                                     select a).FirstOrDefault();
                        query.Solucitud = 0;
                        query.QuienSolicita = "";
                        _context.SaveChanges();

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

        public async Task<IActionResult> EditPrestamo(int? id, int IdPrestamo)
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
                    query.FechaRenovacion = DateTime.Now;
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
            var warningSolicitud = from a in _context.Archivo
                                   where a.Solucitud == 1
                                   select a;

            ViewBag.Warnings = warningSolicitud.Count();


            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "IdDesc" : "";
            ViewData["fechaPrestamoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "fecha_prestamo" : "";
            ViewData["fechaRenovacionSortParm"] = String.IsNullOrEmpty(sortOrder) ? "fecha_Renovacion" : "";

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
            if (roles.Contains("Masteradmin") || roles.Contains("Archivo"))
            {
                filter = from a in _context.Archivo
                         join ap in _context.Archivoprestamo on a.IdArchivo equals ap.ArcchivoIdArchivo
                         where ap.Estatus == "PRESTADO"
                         select new ArchivoControlPrestamo
                         {
                             archivoVM = a,
                             archivoprestamoVM = ap
                         };

            }
            else
            {

                filter = from a in _context.Archivo
                         join ap in _context.Archivoprestamo on a.IdArchivo equals ap.ArcchivoIdArchivo
                         where ap.Recibe == user.ToString() && ap.Estatus == "PRESTADO"
                         select new ArchivoControlPrestamo
                         {
                             archivoVM = a,
                             archivoprestamoVM = ap
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
                case "IdDesc":
                    filter = filter.OrderByDescending(a => a.archivoVM.IdArchivo);
                    break;
                case "name_desc":
                    filter = filter.OrderBy(a => a.archivoVM.Nombre);
                    break;
                case "fecha_prestamo":
                    filter = filter.OrderBy(a => a.archivoprestamoVM.FechaInicial);
                    break;
                case "fecha_Renovacion":
                    filter = filter.OrderByDescending(a => a.archivoprestamoVM.FechaRenovacion);
                    break;
                default:
                    filter = filter.OrderByDescending(spcp => spcp.archivoVM.Nombre);
                    break;
            }
            int pageSize = 10;

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

        public async Task<IActionResult> ArchivoHistorial()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.showSupervisor = false;

            var warningSolicitud = from a in _context.Archivo
                                   where a.Solucitud == 1
                                   select a;

            ViewBag.Warnings = warningSolicitud.Count();

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP" || rol == "Masteradmin")
                {
                    ViewBag.showSupervisor = true;
                }
            }
            ViewBag.norte = user.ToString().EndsWith("\u0040nortedgepms.com");

            return View();
        }
        public JsonResult HistorialPrestamo()
        {


            List<HistorialPrestamos> listaPrestamos = new List<HistorialPrestamos>();

            listaPrestamos = _context.HistorialPrestamos
                                            .FromSql("CALL spHistorialPrestamo()")
                                            .ToList();

            return Json(new { success = true, responseText = Convert.ToString(0), busqueda = listaPrestamos });
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


            var warningSolicitud = from a in _context.Archivo
                                   where a.Solucitud == 1
                                   select a;

            ViewBag.Warnings = warningSolicitud.Count();

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


            if (roles.Contains("Masteradmin") || roles.Contains("Archivo"))
            {
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
                                       where a.IdArchivo == id
                                       select new ArchivoControlPrestamo
                                       {
                                           archivoregistroVM = ar,
                                           archivoVM = a
                                       };

            return View();
        }

        #endregion

        #region -Reacignar-
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
        }
        #endregion
        #region -Solicitud de prestamo-
        public async Task<IActionResult> SolicitudesPrestamo(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            String users = user.ToString();
            ViewBag.RolesUsuario = users;


            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            ViewData["CurrentFilter"] = searchString;

            var paraPrestamo = from p in _context.Archivo
                               where p.Solucitud == 1
                               select p;

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    foreach (var item in searchString.Split(new char[] { ' ' },
            //        StringSplitOptions.RemoveEmptyEntries))
            //    {
            //        paraPrestamo = paraPrestamo.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(searchString) ||
            //                                       (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(searchString) ||
            //                                       (p.Idlibronegro.ToString()).Contains(searchString)
            //                                       );
            //    }
            //}


            //switch (sortOrder)
            //{
            //    case "name_desc":
            //        libronegro = libronegro.OrderByDescending(p => p.Idlibronegro);
            //        break;
            //    default:
            //        libronegro = libronegro.OrderByDescending(p => p.Idlibronegro);
            //        break;
            //}
            int pageSize = 100;
            return View(await PaginatedList<Archivo>.CreateAsync(paraPrestamo.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        public JsonResult solicitud(Archivo archivo, string[] datosoli)
        {
            archivo.Solucitud = Convert.ToSByte(datosoli[0] == "true");
            archivo.IdArchivo = Int32.Parse(datosoli[1]);
            archivo.QuienSolicita = mg.normaliza(datosoli[2]);

            var empty = (from a in _context.Archivo
                         where archivo.IdArchivo == archivo.IdArchivo
                         select a);

            if (empty.Any())
            {
                var query = (from a in _context.Archivo
                             where a.IdArchivo == archivo.IdArchivo
                             select a).FirstOrDefault();
                query.Solucitud = archivo.Solucitud;
                query.QuienSolicita = archivo.QuienSolicita;
                _context.SaveChanges();
            }
            var stadoc = (from p in _context.Archivo
                          where p.IdArchivo == archivo.IdArchivo
                          select p.Solucitud).FirstOrDefault();
            //return View();

            return Json(new { success = true, responseText = Convert.ToString(stadoc), idarchivo = Convert.ToString(archivo.IdArchivo) });
        }


        public JsonResult verDocumento(int datoarchivo)
        {
            bool borrar = false;

            var user = User.Identity.Name;

            try
            {
                borrar = true;
                var query = (from a in _context.Archivoprestamodigital
                             where a.IdArchivoPrestamoDigital == datoarchivo && user.ToString().ToUpper() == a.Usuario.ToUpper()
                             select a).FirstOrDefault();
                query.EstadoPrestamo = 0;
                _context.SaveChanges();

                return Json(new { success = true, responseText = Url.Action("ArchivoPrestamoDigital", "Archivo"), borrar = borrar });
            }
            catch (Exception ex)
            {
                var error = ex;
                return Json(new { success = true, responseText = Url.Action("ArchivoPrestamoDigital", "Archivo"), borrar = borrar });
            }

        }


        public JsonResult Prestar(Archivoprestamodigital archivoprestamodigital, int archivoIdArchivo, string usuario)
        {
            var sacarnomArea = (from a in _context.Areas
                                where a.UserName == usuario
                                select a.Area).First();

            archivoprestamodigital.ArchivoIdArchivo = archivoIdArchivo;
            archivoprestamodigital.Usuario = mg.normaliza(usuario);
            archivoprestamodigital.UsuarioOtorgaPermiso = mg.normaliza(User.Identity.Name);
            archivoprestamodigital.FechaPrestamo = DateTime.Now;
            archivoprestamodigital.FechaCierre = DateTime.Now.AddDays(7);
            archivoprestamodigital.EstadoPrestamo = 1;
            var query = (from a in _context.Archivo
                         where a.IdArchivo == archivoIdArchivo
                         select a).FirstOrDefault();
            query.Solucitud = 0;
            query.QuienSolicita = "";
            _context.SaveChanges();

            _context.Add(archivoprestamodigital);
            _context.SaveChanges();

            return Json(new { success = true, responseText = Url.Action("SolicitudPrestamo", "Archivo"), });
        }


        #endregion

        #region -EnvioArchivo-
        public async Task<IActionResult> Envioarchivo(
            string sortOrder,
            string currentFilter,
            string searchString,
            string estadoSuper,
            string figuraJudicial,
            int? pageNumber,
            string areaFiltro,
            string recibidoFiltro,
            string revisadoFiltro,
            DateTime? fechaDesde,
            DateTime? fechaHasta
            )
        {
            // Warnings
            var warningSolicitud = from a in _context.Archivo
                                   where a.Solucitud == 1
                                   select a;
            ViewBag.Warnings = warningSolicitud.Count();

            // Sorting
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CausaPenalSortParm"] = String.IsNullOrEmpty(sortOrder) ? "causa_penal_desc" : "";
            ViewData["EstadoCumplimientoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "estado_cumplimiento_desc" : "";
            ViewData["CurrentFilter"] = searchString;
            ViewData["areaFiltro"] = areaFiltro;
            ViewData["recibidoFiltro"] = recibidoFiltro?.ToString();
            ViewData["revisadoFiltro"] = revisadoFiltro?.ToString();

            // Paging & searchString persistence
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            // User & roles
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            ViewBag.User = user.ToString();
            ViewBag.Admin = false;
            ViewBag.Masteradmin = false;
            ViewBag.Archivo = roles.Any(r => r == "Masteradmin" || r == "Archivo" || r == "Director");

            #region -Sacar Roles-
            areaFiltro = mg.areaSegunRol(roles);
            #endregion

            List<SelectListItem> listaAreas;
            listaAreas = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Todos", Value = "" },
                new SelectListItem{ Text = "Ejecución de Penas", Value = "Ejecución de penas" },
                new SelectListItem{ Text = "LC", Value = "LC" },
                new SelectListItem{ Text = "MCySCP", Value = "MCySCP" },
                new SelectListItem{ Text = "Oficialia", Value = "Oficialia" },
                new SelectListItem{ Text = "Servicios Previos", Value = "Servicios previos" },
                new SelectListItem{ Text = "UESPÁ", Value = "UESPA" }
            };
            ViewBag.ListaAreas = listaAreas;

            List<SelectListItem> listaRecibido;
            listaRecibido = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Todos", Value = "" },
                new SelectListItem{ Text = "Si", Value = "1" },
                new SelectListItem{ Text = "No", Value = "0" }

            };
            ViewBag.listaFiltro1 = listaRecibido; 
            
            List<SelectListItem> listaRevisado;
            listaRevisado = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Todos", Value = "" },
                new SelectListItem{ Text = "Si", Value = "1" },
                new SelectListItem{ Text = "No", Value = "0" }

            };
            ViewBag.listaFiltro2 = listaRevisado;

            // Query base
            var filter = _context.Envioarchivo.AsQueryable();

            // Filtro por texto de búsqueda
            if (!String.IsNullOrEmpty(searchString))
            {

                foreach (var item in searchString.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    filter = filter.Where(ea =>
                                        ((ea.Apaterno ?? "") + " " + (ea.Amaterno ?? "") + " " + (ea.Nombre ?? "")).Contains(searchString) ||
                                        ((ea.Nombre ?? "") + " " + (ea.Apaterno ?? "") + " " + (ea.Amaterno ?? "")).Contains(searchString) ||
                                        (ea.Usuario ?? "").Contains(searchString) ||
                                        (ea.Causapenal ?? "").Contains(searchString) ||
                                        (ea.Area ?? "").Contains(searchString) ||
                                        (ea.IdArchvo ?? "").Contains(searchString)
                    );
                }

            }

            // Filtro por Fecha Desde
            if (fechaDesde.HasValue)
            {
                filter = filter.Where(e => e.FechaRecibido >= fechaDesde.Value);
            }

            // Filtro por Fecha Hasta
            if (fechaHasta.HasValue)
            {
                filter = filter.Where(e => e.FechaRecibido <= fechaHasta.Value);
            }
            ViewBag.fInicio = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.fFinal = fechaHasta?.ToString("yyyy-MM-dd");

            // Filtro por Área
            if (!String.IsNullOrWhiteSpace(areaFiltro) && areaFiltro != "Todos")
            {
                filter = filter.Where(ea => ea.Area == areaFiltro);
            }

            // Filtro por Recibido
            if (!string.IsNullOrEmpty(recibidoFiltro) && recibidoFiltro != "Todos")
            {
                filter = filter.Where(ea => ea.Recibido == (recibidoFiltro == "Si" ? 1 : 0));
            }

            // Filtro por Revisado
            if (!string.IsNullOrEmpty(revisadoFiltro) && revisadoFiltro != "Todos")
            {
                filter = filter.Where(ea => ea.Revisado == (revisadoFiltro == "Si" ? 1 : 0));
            }

            // Ordenamiento
            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(ea => ea.Apaterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(ea => ea.Apaterno);
                    break;
                case "estado_cumplimiento_desc":
                    filter = filter.OrderByDescending(ea => ea.Nombre);
                    break;
                default:
                    filter = filter.OrderByDescending(ea => ea.IdenvioArchivo);
                    break;
            }

            // Paging
            int pageSize = 20;
            var paged = await PaginatedList<Envioarchivo>.CreateAsync(
                filter.AsNoTracking(),
                pageNumber ?? 1,
                pageSize
            );

            return View(paged);
        }

        public void recibido(int id, string QuienRecibe)
        {
            var envioarchivo = (from ea in _context.Envioarchivo
                                where ea.IdenvioArchivo == id
                                select ea).FirstOrDefault();
            if (envioarchivo.Recibido == 0 || envioarchivo.Recibido == null)
            {
                envioarchivo.Recibido = 1;
            }
            else
            {
                envioarchivo.Recibido = 0;
            }

            envioarchivo.QuienRecibe = QuienRecibe;
            envioarchivo.FechaRecibido = DateTime.Now;
            _context.SaveChanges();
        }

        public void revisado(int id, string QuienRevisa)
        {
            var envioarchivo = (from en in _context.Envioarchivo
                                where en.IdenvioArchivo == id
                                select en).FirstOrDefault();
            if (envioarchivo.Revisado == 0 || envioarchivo.Revisado == null)
            {
                envioarchivo.Revisado = 1;
            }
            else
            {
                envioarchivo.Revisado = 0;
            }

            envioarchivo.QuienRevisa = QuienRevisa;
            envioarchivo.FechaRevisado = DateTime.Now;
            _context.SaveChanges();
        }

        [HttpPost]
        public void ActualizarIdArchivo(int id, string idarchivo)
        {
            var envioarchivo = (from en in _context.Envioarchivo
                                where en.IdenvioArchivo == id
                                select en).FirstOrDefault();

            if (envioarchivo != null)
            {
                envioarchivo.IdArchvo = idarchivo;
                _context.SaveChanges();
            }
        }

        [HttpPost]
        public void ActualizarObservaciones(int id, string observaciones)
        {
            var envioarchivo = (from en in _context.Envioarchivo
                                where en.IdenvioArchivo == id
                                select en).FirstOrDefault();

            if (envioarchivo != null)
            {
                envioarchivo.Observaciones = observaciones;
                _context.SaveChanges();
            }
        }

        // GET: Envioarchivo/Create
        public async Task<IActionResult> createEnvioArchivo()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.User = user.ToString();

            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

            var roleToArea = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Uespa", "UESPA" },
                { "Ejecucion", "Ejecución de Penas" },
                { "AuxiliarEjecucion", "Ejecución de Penas" },
                { "Coordinador Ejecucion", "Ejecución de Penas" },
                { "SupervisorLC", "LC" },
                { "AdminLC", "LC" },
                { "AdminMCSCP", "MCySCP" },
                { "SupervisorMCSCP", "MCySCP" },
                { "AuxiliarMCSCP", "MCySCP" },
                { "Servicios previos", "Servicios previos" },
                { "Oficialia", "Oficialia" },
            // ... puedes añadir más según tus reglas
            };

            string areaAsignada = roles
                .Where(role => roleToArea.ContainsKey(role))
                .Select(role => roleToArea[role])
                .FirstOrDefault() ?? "Sin área asignada";


            ViewBag.AreaAsignada = areaAsignada;


            if (areaAsignada == "Ejecución de Penas")
            {
                ViewBag.SituacionJuridico = _context.Eptermino
                    .Select(e => e.Formaconclucion)
                    .Where(e => !string.IsNullOrEmpty(e))
                    .Distinct()
                    .ToList();
            }
            else if (areaAsignada == "MCySCP")
            {
                ViewBag.SituacionJuridico = _context.Cierredecaso
                    .Select(c => c.ComoConcluyo)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .ToList();
            }
            else if (areaAsignada == "CL")
            {
                ViewBag.SituacionJuridico = _context.Cierredecasocl
                    .Select(c => c.ComoConcluyo)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .ToList();
            }     
            else if (areaAsignada == "Servicios previos")
            {
                ViewBag.SituacionJuridico = _context.Serviciospreviosjuicio
                    .Select(c => c.Situacion)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Distinct()
                    .ToList();
            }



            return View();
        }

        // POST: Envioarchivo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult createEnvioArchivo([Bind("Nombre,Apaterno,Amaterno,Causapenal,Delito,TipoDocumento,SituacionJuridico,Recibido,Revisado,IdArchvo,Observaciones,Area,Usuario,FechaRegistro")] Envioarchivo envioarchivo)
        {
            if (ModelState.IsValid)
            {
               
                _context.Envioarchivo.Add(envioarchivo);
                _context.SaveChanges();
                return RedirectToAction(nameof(createEnvioArchivo)); // O la acción de listado
            }
            return View(envioarchivo);
        }
        #endregion

        #region -editEnvioArchivo-
        public async Task<IActionResult> editEnvioArchivo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivo = await _context.Envioarchivo.SingleOrDefaultAsync(m => m.IdenvioArchivo == id);
            if (archivo == null)
            {
                return NotFound();
            }
            return View(archivo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> editEnvioArchivo(int id, [Bind("IdenvioArchivo,Nombre,Apaterno,Amaterno,Causapenal,Delito,TipoDocumento,SituacionJuridico,Recibido,Revisado,IdArchvo,Observaciones,Area,Usuario,FechaRegistro,FechaRecibido,FechaRevisado,QuienRecibe,QuienRevisa")] Envioarchivo envioarchivo, int IdenvioArchivo)
        {

            if (ModelState.IsValid)
            {
                envioarchivo.IdenvioArchivo = id;
                envioarchivo.Apaterno = mg.removeSpaces(mg.normaliza(envioarchivo.Apaterno));
                envioarchivo.Amaterno = mg.removeSpaces(mg.normaliza(envioarchivo.Amaterno));
                envioarchivo.Nombre = mg.removeSpaces(mg.normaliza(envioarchivo.Nombre));
                envioarchivo.Causapenal = envioarchivo.Causapenal;
                envioarchivo.Delito = mg.removeSpaces(mg.normaliza(envioarchivo.Delito));
                envioarchivo.TipoDocumento = envioarchivo.TipoDocumento;
                envioarchivo.SituacionJuridico = mg.removeSpaces(mg.normaliza(envioarchivo.SituacionJuridico));
                envioarchivo.Recibido = envioarchivo.Recibido;
                envioarchivo.Revisado = envioarchivo.Revisado;
                envioarchivo.IdArchvo = envioarchivo.IdArchvo;
                envioarchivo.Observaciones = envioarchivo.Observaciones;
                envioarchivo.Area = envioarchivo.Area;
                envioarchivo.Usuario = envioarchivo.Usuario;
                envioarchivo.FechaRegistro = envioarchivo.FechaRegistro;
                envioarchivo.FechaRevisado = envioarchivo.FechaRevisado;
                envioarchivo.FechaRecibido = envioarchivo.FechaRecibido;
                envioarchivo.QuienRecibe = envioarchivo.QuienRecibe;
                envioarchivo.QuienRevisa = envioarchivo.QuienRevisa;
                try
                {
                    _context.Update(envioarchivo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArchivoExists(envioarchivo.IdenvioArchivo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Envioarchivo));
            }
            return View(envioarchivo);
        }
        #endregion

        #region -Delete-
        public JsonResult DeleteEnvioArchivo(int id)
        {
            var borrar = false;
            var envioarchivo = (from en in _context.Envioarchivo where en.IdenvioArchivo == id select en).FirstOrDefault();
            try
            {
                if (envioarchivo != null)
                {
                    _context.Envioarchivo.Remove(envioarchivo);
                    _context.SaveChanges();
                    borrar = true;
                    return Json(new { success = true, responseText = Url.Action("EnvioArchivo", "Archivo"), borrar = borrar, mensaje = "Exito" });
                }
                
            }
            catch(Exception ex)
            {
                return Json(new { success = false, responseText = Url.Action("Envioarchivo", "Archivo"), borrar = borrar, mensaje = "Error; " + ex });
            }
            return Json(new { success = true, responseText = Url.Action("EnvioArchivo", "Archivo"), borrar = borrar, mensaje = "Exito" });
        }
        #endregion

    }
}

