﻿using System;
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
            if (str == null)
            {
                return "";
            }
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


        // GET: Archivo

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


            //var filter = from a in _context.Archivo
            //         join ap in _context.Archivoprestamo on a.IdArchivo equals ap.ArcchivoIdArchivo into tmp
            //         from left in tmp.DefaultIfEmpty()
            //         select new ArchivoControlPrestamo
            //         {

            //         };

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
                    filter = filter.OrderByDescending(spcp => spcp.archivoVM.Paterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(spcp => spcp.archivoVM.Materno);
                    break;
                case "estado_cumplimiento_desc":
                    filter = filter.OrderByDescending(spcp => spcp.archivoVM.Nombre);
                    break;
                
            }

            filter = filter.OrderByDescending(spcp => spcp.archivoVM.IdArchivo);


            int pageSize = 10;
            return View(await PaginatedList<ArchivoControlPrestamo>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Archivo/Details/5
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

        // GET: Archivo/Create
        public IActionResult Create()
        {
            return View();
        }

        #region -CREATE PERSONA ARCHIVO
        // POST: Archivo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        public JsonResult Createadd(int id, string nombre, string ap, string am, Archivo archivo)
        {
            bool create = false;
            var idExiste = (from a in _context.Archivo
                            where a.IdArchivo == id
                            select a.IdArchivo);

            if(!idExiste.Any())
            {
                try
                {
                    create = true;
                    archivo.IdArchivo = id;
                    archivo.Paterno = removeSpaces(normaliza(ap));
                    archivo.Materno = removeSpaces(normaliza(am));
                    archivo.Nombre = removeSpaces(normaliza(nombre));

                    _context.Add(archivo);
                    _context.SaveChanges();

                    return Json(new { success = true, responseText = Url.Action("Index", "Archivo"), create });
                }catch(Exception ex)
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

        //[HttpPost] 
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("IdArchivo,Paterno,Materno,Nombre,Urldocumento,ExpedienteUnicoIdExpedienteUnico")] Archivo archivo, string Paterno)
        //{
        //    var idExiste = (from a in _context.Archivo
        //                    where a.IdArchivo == archivo.IdArchivo
        //                    select a.IdArchivo).First();

        //    if (idExiste == archivo.IdArchivo)
        //    {
        //        return RedirectToAction("Index");

        //    }

        //    if (ModelState.IsValid)
        //    {
        //        archivo.Paterno = removeSpaces(normaliza(archivo.Paterno));
        //        archivo.Materno = removeSpaces(normaliza(archivo.Materno));
        //        archivo.Nombre = removeSpaces(normaliza(archivo.Nombre));


        //        _context.Add(archivo);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(archivo);
        //}

        // GET: Archivo/Edit/5
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

        // POST: Archivo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                archivo.Paterno = removeSpaces(normaliza(archivo.Paterno));
                archivo.Materno = removeSpaces(normaliza(archivo.Materno));
                archivo.Nombre = removeSpaces(normaliza(archivo.Nombre));
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

        // GET: Archivo/Delete/5
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

        // POST: Archivo/Delete/5
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

            return Json(new { success = true, responseText = Convert.ToString(stadoc)});
        }
        public JsonResult deletesuper(Archivo archivo, Historialeliminacion historialeliminacion, string[] datosuper)
        {
            var borrar = false;
            var id = Int32.Parse(datosuper[0]);
            var razon = normaliza(datosuper[1]);
            var user = normaliza(datosuper[2]);

            var query = (from a in _context.Archivo
                         where a.IdArchivo == id
                         select a).FirstOrDefault();

            try
            {
                borrar = true;
                historialeliminacion.Id = id;
                historialeliminacion.Descripcion = "IDARCHIVO= " + query.IdArchivo + "NOMBREL= " + query.Paterno + " " + query.Materno + " " + query.Materno;
                historialeliminacion.Tipo = "HISTORIALARCHIVO";
                historialeliminacion.Razon = normaliza(razon);
                historialeliminacion.Usuario = normaliza(user);
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

        private bool ArchivoExists(int id)
        {
            return _context.Archivo.Any(e => e.IdArchivo == id);
        }

        public IActionResult ArchivoMenu()
        {
            return View();
        }
        #region -PRESTAMO-
        // GET: Archivo/Create
        public async Task<IActionResult> CreatePrestamo(int id, Archivoprestamo archivoprestamo, Archivo archivo, Areas areas )
        {
            ViewBag.idArchivo = id;

            if (id == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> CreatePrestamo([Bind("Entrega,Recibe,Area,FechaInicial,FechaRenovacion,Estatus,Renovaciones,Urlvale,ArcchivoIdArchivo")] Archivoprestamo archivoprestamo, int archivoIdArchivo)
        {
            if (ModelState.IsValid)
            {
                var sacarnomEntrega = (from a in _context.Areas
                                   where a.IdArea == int.Parse(archivoprestamo.Entrega)
                                   select a.UserName).First(); 

                var sacarnomRecibe= (from a in _context.Areas
                                   where a.IdArea == int.Parse(archivoprestamo.Recibe)
                                   select a.UserName).First();   
                
                var sacarnomArea = (from a in _context.Areas
                                   where a.IdArea == int.Parse(archivoprestamo.Recibe)
                                   select a.Area).First(); 

                archivoprestamo.Entrega = normaliza(sacarnomEntrega.ToString());
                archivoprestamo.Recibe = normaliza(sacarnomRecibe.ToString());
                archivoprestamo.Area = normaliza(sacarnomArea.ToString());
                archivoprestamo.FechaInicial = archivoprestamo.FechaInicial;
                archivoprestamo.FechaRenovacion = archivoprestamo.FechaRenovacion;
                archivoprestamo.Estatus = "PRESTADO";
                archivoprestamo.Renovaciones = archivoprestamo.Renovaciones;
                archivoprestamo.Urlvale = archivoprestamo.Urlvale;
                archivoprestamo.ArcchivoIdArchivo = archivoIdArchivo;

                _context.Add(archivoprestamo);
                await _context.SaveChangesAsync();
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
            ViewBag.idEstatus = BuscaId(ListaEstatus, archivoprestamo.Estatus);

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

                archivoprestamo.Entrega = normaliza(sacarnomEntrega.ToString());
                archivoprestamo.Recibe = normaliza(sacarnomRecibe.ToString());
                archivoprestamo.Area = normaliza(sacarnomArea.ToString());
                archivoprestamo.FechaInicial = archivoprestamo.FechaInicial;
                archivoprestamo.FechaRenovacion = archivoprestamo.FechaRenovacion;
                archivoprestamo.Estatus = archivoprestamo.Estatus;
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
                    entrega =true;
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

        #region -ARCHIVO FISICO-
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

        // POST: Archivo/CreateArchivo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArchivo([Bind("CausaPenal,Delito,Situacion,Sentencia,FechaAcuerdo,Observaciones,CarpetaEjecucion,Envia,ArcchivoIdArchivo")] Archivoregistro archivoregistro, int archivoIdArchivo)
        {
            if (ModelState.IsValid)
            {
                var sacarnomEnvia = (from a in _context.Areas
                                       where a.IdArea == int.Parse(archivoregistro.Envia)
                                       select a.UserName).First();

                
                archivoregistro.CausaPenal = normaliza(archivoregistro.CausaPenal.ToString());
                archivoregistro.Delito = normaliza(archivoregistro.Delito.ToString());
                archivoregistro.Situacion = normaliza(archivoregistro.Situacion.ToString());
                archivoregistro.Sentencia = archivoregistro.Sentencia;
                archivoregistro.FechaAcuerdo = archivoregistro.FechaAcuerdo;
                archivoregistro.Observaciones = archivoregistro.Observaciones;
                archivoregistro.CarpetaEjecucion = archivoregistro.CarpetaEjecucion;
                archivoregistro.Envia = sacarnomEnvia.ToString();
                archivoregistro.ArchivoIdArchivo = archivoIdArchivo;

                _context.Add(archivoregistro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(archivoregistro);
        }

        public async Task<IActionResult> EditArchivo(int id)
        {

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
                                            archivoVM = a
                                        };

            return View();  
        }

        // POST: Archivo/EditArchivo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArchivo(int id, [Bind("IdArchivoRegistro,CausaPenal,Delito,Situacion,Sentencia,FechaAcuerdo,Observaciones,CarpetaEjecucion,Envia,ArcchivoIdArchivoo")] Archivoregistro archivoregistro, int archivoIdArchivo)
        {

            if (id != archivoregistro.IdArchivoRegistro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                var sacarnomEntrega = (from a in _context.Areas
                                       where a.IdArea == int.Parse(archivoregistro.Envia)
                                       select a.UserName).First();


                archivoregistro.Envia = sacarnomEntrega.ToString();
                archivoregistro.ArchivoIdArchivo = archivoIdArchivo;
                try
                {
                    _context.Update(archivoregistro);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(ArchivoControl));
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

        #region -ARCHIVO FISICO-
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


        public JsonResult GetAreaUser(int AreaId)
        {
            TempData["message"] = DateTime.Now;
            List<Areas> areas = new List<Areas>();

            areas = (from Areas in _context.Areas
                              where Areas.IdArea == AreaId
                     select Areas).ToList();

            return Json(new SelectList(areas, "IdArea", "Area"));
        }

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
                    filter = filter.OrderByDescending(spcp => spcp.archivoVM.Nombre);
                    break;
            }
            int pageSize = 100;

            //var queryable = query2.AsQueryable();
            return View(await PaginatedList<ArchivoControlPrestamo>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        #endregion
    }
}
