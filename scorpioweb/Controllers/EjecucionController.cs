using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;
using scorpioweb.Class;
using SautinSoft.Document;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using SautinSoft.Document.MailMerging;
using DocumentFormat.OpenXml.Drawing.Charts;
using Org.BouncyCastle.Crypto;
using System.Globalization;
using Microsoft.AspNetCore.Rewrite.Internal;
using DocumentFormat.OpenXml.Wordprocessing;

namespace scorpioweb.Controllers
{
    [Authorize]
    public class EjecucionController : Controller
    {

        #region -Variables Globales-
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly penas2Context _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        #endregion

        #region -Metodos Generales-
        MetodosGenerales mg = new MetodosGenerales();
        #endregion


        public EjecucionController(penas2Context context, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,
                                  RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

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


            //List<Archivoprestamo> queryArchivoHistorial = (from a in _context.Archivoprestamo
            //                                               group a by a.ArcchivoIdArchivo into grp
            //                                               select grp.OrderByDescending(a => a.IdArchivoPrestamo).FirstOrDefault()).ToList();

            var filter = from e in _context.Ejecucion
                         select new EjecucionCP
                         {
                             ejecucionVM = e
                         };



            ViewData["CurrentFilter"] = searchString;
            ViewData["EstadoS"] = estadoSuper;
            ViewData["FiguraJ"] = figuraJudicial;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = filter.Where(acp => (acp.ejecucionVM.Paterno + " " + acp.ejecucionVM.Materno + " " + acp.ejecucionVM.Nombre).Contains(searchString.ToUpper()) ||
                                             (acp.ejecucionVM.Nombre + " " + acp.ejecucionVM.Materno + " " + acp.ejecucionVM.Paterno).Contains(searchString.ToUpper()) ||
                                             (acp.ejecucionVM.Materno + " " + acp.ejecucionVM.Paterno + " " + acp.ejecucionVM.Nombre).Contains(searchString.ToUpper()) ||
                                             (acp.ejecucionVM.Yo).Contains(searchString.ToUpper()) ||
                                             (acp.ejecucionVM.IdEjecucion.ToString()).Contains(searchString.ToUpper()) ||
                                             (acp.ejecucionVM.Ce.ToString()).Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(acp => acp.ejecucionVM.Paterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(acp => acp.ejecucionVM.Materno);
                    break;
                case "estado_cumplimiento_desc":
                    filter = filter.OrderByDescending(acp => acp.ejecucionVM.Nombre);
                    break;

            }

            filter = filter.OrderByDescending(spcp => spcp.ejecucionVM.IdEjecucion);


            int pageSize = 10;
            return View(await PaginatedList<EjecucionCP>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        #endregion

        // GET: Ejecucion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ejecucion = await _context.Ejecucion
                .SingleOrDefaultAsync(m => m.IdEjecucion == id);
            if (ejecucion == null)
            {
                return NotFound();
            }

            return View(ejecucion);
        }
        #region -Create Ejecucion-
        // GET: Ejecucion/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.centrosPenitenciarios = _context.Centrospenitenciarios.Select(Centrospenitenciarios => Centrospenitenciarios.Nombrecentro).ToList();
            ViewBag.directorio = _context.Directoriojueces.Select(Directoriojueces => Directoriojueces.Area).ToList();

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.User = user.ToString();

            List<string> Liatajuzgado = new List<string>();
            Liatajuzgado.Add("NA");
            Liatajuzgado.Add("JUZGADO 1");
            Liatajuzgado.Add("JUZGADO 2");
            Liatajuzgado.Add("JUZGADO 3");
            Liatajuzgado.Add("JUZGADO 4");

            ViewBag.Liatajuzgado = Liatajuzgado;

            return View();
        }

        // POST: Ejecucion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEjecucion,Paterno,Materno,Nombre,Yo,Ce,Juzgado,TieneceAcumuladas,CeAcumuladas,Usuario,LugarInternamiento")] Ejecucion ejecucion)
        {
            if (ModelState.IsValid)
            {
                ejecucion.Paterno = mg.removeSpaces(mg.normaliza(ejecucion.Paterno));
                ejecucion.Materno = mg.removeSpaces(mg.normaliza(ejecucion.Materno));
                ejecucion.Nombre = mg.removeSpaces(mg.normaliza(ejecucion.Nombre));
                ejecucion.Yo = mg.removeSpaces(mg.normaliza(ejecucion.Yo));
                ejecucion.Ce = mg.removeSpaces(mg.normaliza(ejecucion.Ce));
                ejecucion.Juzgado = mg.removeSpaces(mg.normaliza(ejecucion.Juzgado));
                ejecucion.TieneceAcumuladas = mg.removeSpaces(mg.normaliza(ejecucion.TieneceAcumuladas));
                ejecucion.CeAcumuladas = mg.removeSpaces(mg.normaliza(ejecucion.CeAcumuladas));
                ejecucion.Usuario = mg.removeSpaces(mg.normaliza(ejecucion.Usuario));
                ejecucion.LugarInternamiento = mg.removeSpaces(mg.normaliza(ejecucion.LugarInternamiento));

                _context.Add(ejecucion);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                return RedirectToAction(nameof(Index));
            }
            return View(ejecucion);
        }
        #endregion

        #region -Edit Ejecucion-
        // GET: Ejecucion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var ejecucion = await _context.Ejecucion.SingleOrDefaultAsync(m => m.IdEjecucion == id);

            ViewBag.centrosPenitenciarios = _context.Centrospenitenciarios.Select(Centrospenitenciarios => Centrospenitenciarios.Nombrecentro).ToList();
            ViewBag.directorio = _context.Directoriojueces.Select(Directoriojueces => Directoriojueces.Area).ToList();

            #region -Sacar Usuario-
            List<string> ListaUserEjecucion = new List<string>();
            foreach (var u in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(u, "Ejecucion"))
                {
                    ListaUserEjecucion.Add(u.ToString());
                }
                if (await userManager.IsInRoleAsync(u, "Coordinador Ejecucion"))
                {
                    ListaUserEjecucion.Add(u.ToString());
                }
            }


            var tagged = ListaUserEjecucion.Select((item, i) => new { Item = item, Index = (int?)i });
            var index = (from pair in tagged
                         where pair.Item == ejecucion.Usuario.ToLower()
                         select pair.Item).FirstOrDefault();

            ViewBag.ListaEjecucion = ListaUserEjecucion.Where(r => ListaUserEjecucion.Any(f => !r.EndsWith("\u0040nortedgepms.com")));
            ViewBag.ListaEjecucionEdit = index;
            #endregion
            #region juzgado          
            List<string> LiataJzgado = new List<string>();
            LiataJzgado.Add("NA");
            LiataJzgado.Add("JUZGADO 1");
            LiataJzgado.Add("JUZGADO 2");
            LiataJzgado.Add("JUZGADO 3");
            LiataJzgado.Add("JUZGADO 4");


            ViewBag.LiataJuzgado = LiataJzgado;

            var turno = LiataJzgado.Select((item, i) => new { Item = item, Index = (int?)i });
            var selectturno = (from pair in turno
                               where pair.Item == ejecucion.Juzgado
                               select pair.Item).FirstOrDefault();

            ViewBag.LiataJuzgadoEdit = selectturno;

            #endregion

            if (id == null)
            {
                return NotFound();
            }


            if (ejecucion == null)
            {
                return NotFound();
            }

            ViewBag.acumuladas = ejecucion.TieneceAcumuladas;
            return View(ejecucion);
        }

        // POST: Ejecucion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEjecucion,Paterno,Materno,Nombre,Yo,Ce,TieneceAcumuladas,CeAcumuladas,Usuario,Juzgado,LugarInternamiento")] Ejecucion ejecucion)
        {
            if (id != ejecucion.IdEjecucion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ejecucion.Paterno = mg.removeSpaces(mg.normaliza(ejecucion.Paterno));
                    ejecucion.Materno = mg.removeSpaces(mg.normaliza(ejecucion.Materno));
                    ejecucion.Nombre = mg.removeSpaces(mg.normaliza(ejecucion.Nombre));
                    ejecucion.Yo = mg.removeSpaces(mg.normaliza(ejecucion.Yo));
                    ejecucion.Ce = mg.removeSpaces(mg.normaliza(ejecucion.Ce));
                    ejecucion.TieneceAcumuladas = mg.removeSpaces(mg.normaliza(ejecucion.TieneceAcumuladas));
                    ejecucion.CeAcumuladas = mg.removeSpaces(mg.normaliza(ejecucion.CeAcumuladas));
                    ejecucion.Juzgado = mg.removeSpaces(mg.normaliza(ejecucion.Juzgado));
                    ejecucion.Usuario = mg.removeSpaces(mg.normaliza(ejecucion.Usuario));
                    ejecucion.LugarInternamiento = mg.removeSpaces(mg.normaliza(ejecucion.LugarInternamiento));

                    var oldEjecucion = await _context.Ejecucion.FindAsync(id);
                    _context.Entry(oldEjecucion).CurrentValues.SetValues(ejecucion);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EjecucionExists(ejecucion.IdEjecucion))
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
            return View(ejecucion);
        }
        #endregion

        #region -Borrar Ejecucion-
        public JsonResult antesdeleteCE(Ejecucion ejecucion, Causapenal causapenal, string id)
        {
            var borrar = false;
            var idEp = Int32.Parse(id);

            var antesdel = from epcp in _context.Epcausapenal
                           where epcp.EjecucionIdEjecucion == idEp
                           select epcp;

            var sinProceso1 = from ep in _context.Ejecucion
                              join apa in _context.Epamparo on ep.IdEjecucion equals apa.EjecucionIdEjecucion
                              where ep.IdEjecucion == idEp
                              select apa;

            var sinProceso2 = from ep in _context.Ejecucion
                              join apa in _context.Epatencionf on ep.IdEjecucion equals apa.EjecucionIdEjecucion
                              where ep.IdEjecucion == idEp
                              select apa;

            var sinProceso3 = from ep in _context.Ejecucion
                              join apa in _context.Epaudiencia on ep.IdEjecucion equals apa.EjecucionIdEjecucion
                              where ep.IdEjecucion == idEp
                              select apa;


            if (antesdel.Any() || sinProceso1.Any() || sinProceso2.Any() || sinProceso3.Any())
            {
                return Json(new { success = true, responseText = Url.Action("Index", "Ejecucion"), borrar = borrar });
            }
            else
            {
                borrar = true;
                return Json(new { success = true, responseText = Url.Action("Index", "Ejecucion"), borrar = borrar });
            }
        }
        public async Task<JsonResult> deleteEjecucion(Ejecucion ejecucion, Historialeliminacion historialeliminacion, string[] datoEjecucion)
        {
            var borrar = false;
            var id = Int32.Parse(datoEjecucion[0]);
            var razon = mg.normaliza(datoEjecucion[1]);
            var user = mg.normaliza(datoEjecucion[2]);

            var query = (from s in _context.Ejecucion
                         where s.IdEjecucion == id
                         select s).FirstOrDefault();

            var queryP = (from s in _context.Supervision
                          join p in _context.Persona on s.PersonaIdPersona equals p.IdPersona
                          where s.IdSupervision == id
                          select p).FirstOrDefault();

            try
            {
                borrar = true;
                historialeliminacion.Id = id;
                historialeliminacion.Descripcion = "IDEJECUCION= " + query.IdEjecucion + " NOMBRE= " + query.Paterno + " " + query.Materno + " " + query.Nombre + " CE= " + query.Ce;
                historialeliminacion.Tipo = "EJECUCION";
                historialeliminacion.Razon = mg.normaliza(razon);
                historialeliminacion.Usuario = mg.normaliza(user);
                historialeliminacion.Fecha = DateTime.Now;
                historialeliminacion.Supervisor = mg.normaliza(query.Usuario);
                _context.Add(historialeliminacion);
                _context.SaveChanges();


                var ejecuciondel = _context.Ejecucion.SingleOrDefault(m => m.IdEjecucion == id);
                _context.Ejecucion.Remove(ejecuciondel);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);


                return Json(new { success = true, responseText = Url.Action("Index", "Ejecucion"), borrar = borrar });

            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("Index", "Ejecucion"), borrar = borrar });
            }

            //return Json(new { success = true, idPersonas = Convert.ToString(id) });
        }
        #endregion

        #region -EpCausa Penal-      
        public async Task<IActionResult> EpCausaPenal(int id)
        {
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();
            ViewBag.directorio = _context.Directoriojueces.Select(Directoriojueces => Directoriojueces.Area).ToList();

            ViewBag.idEjecucion = id;


            var snEpCausapenal = await _context.Epcausapenal.Where(m => m.EjecucionIdEjecucion == id).ToListAsync();
            if (snEpCausapenal.Count != 0)
            {
                return RedirectToAction("EditEpCausapenal", new { id });
            }


            List<string> Liatajuzgado = new List<string>();
            Liatajuzgado.Add("NA");
            Liatajuzgado.Add("JUZGADO 1");
            Liatajuzgado.Add("JUZGADO 2");
            Liatajuzgado.Add("JUZGADO 3");
            Liatajuzgado.Add("JUZGADO 4");

            ViewBag.Liatajuzgado = Liatajuzgado;
            return View();
        }
        public async Task<IActionResult> CreateEpCausapenal2(int id)
        {
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();
            ViewBag.directorio = _context.Directoriojueces.Select(Directoriojueces => Directoriojueces.Area).ToList();


            ViewBag.idEjecucion = id;
            List<string> Liatajuzgado = new List<string>();
            Liatajuzgado.Add("NA");
            Liatajuzgado.Add("JUZGADO 1");
            Liatajuzgado.Add("JUZGADO 2");
            Liatajuzgado.Add("JUZGADO 3");

            ViewBag.Liatajuzgado = Liatajuzgado;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EpCausaPenal(int id, [Bind("Causapenal,Delito,Clasificaciondelito,JuzgadoOrigen,FechaSentencia,Multa,Reparacion,Firmeza,Penaanos,Penameses,Penadias,Apartir,EjecucionIdEjecucion")] Epcausapenal epcausapenal)
        {
            if (ModelState.IsValid)
            {

                epcausapenal.Causapenal = mg.removeSpaces(mg.normaliza(epcausapenal.Causapenal));
                epcausapenal.Delito = mg.removeSpaces(mg.normaliza(epcausapenal.Delito));
                epcausapenal.Clasificaciondelito = mg.removeSpaces(mg.normaliza(epcausapenal.Clasificaciondelito));
                epcausapenal.JuzgadoOrigen = mg.removeSpaces(mg.normaliza(epcausapenal.JuzgadoOrigen));
                epcausapenal.FechaSentencia = epcausapenal.FechaSentencia;
                epcausapenal.Multa = mg.removeSpaces(mg.normaliza(epcausapenal.Multa));
                epcausapenal.Reparacion = mg.removeSpaces(mg.normaliza(epcausapenal.Reparacion));
                epcausapenal.Firmeza = epcausapenal.Firmeza;
                epcausapenal.Penaanos = epcausapenal.Penaanos;
                epcausapenal.Penameses = epcausapenal.Penameses;
                epcausapenal.Penadias = epcausapenal.Penadias;
                epcausapenal.Apartir = epcausapenal.Apartir;
                epcausapenal.EstadodeCausa = 0;
                epcausapenal.EjecucionIdEjecucion = epcausapenal.EjecucionIdEjecucion;


                _context.Add(epcausapenal);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                return RedirectToAction(nameof(Index));
            }
            return View(epcausapenal);
        }
        #endregion

        #region -Editar EP Causa Penal-
        public async Task<IActionResult> EditEPCausaPenal(int id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.User = user.ToString();
            ViewBag.Admin = false;
            ViewBag.Masteradmin = false;
            ViewBag.Archivo = false;


            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();
            ViewBag.directorio = _context.Directoriojueces.Select(Directoriojueces => Directoriojueces.Area).ToList();



            ViewData["tieneEPCP"] = from e in _context.Ejecucion
                                    join epcp in _context.Epcausapenal on e.IdEjecucion equals epcp.EjecucionIdEjecucion
                                    where e.IdEjecucion == id
                                    orderby epcp.EstadodeCausa
                                    select new EjecucionCP
                                    {
                                        ejecucionVM = e,
                                        epcausapenalVM = epcp,
                                    };

            #region -JUZGADO-

            List<string> Liatajuzgado = new List<string>();
            Liatajuzgado.Add("NA");
            Liatajuzgado.Add("JUZGADO 1");
            Liatajuzgado.Add("JUZGADO 2");
            Liatajuzgado.Add("JUZGADO 3");
            Liatajuzgado.Add("JUZGADO 4");

            ViewBag.Liatajuzgado = Liatajuzgado;

            #endregion


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEPCausaPenal(int id, [Bind("Idepcausapenal, Causapenal, Delito, Clasificaciondelito, JuzgadoOrigen, FechaSentencia, Multa, Reparacion, Firmeza, Penaanos, Penameses, Penadias, Apartir, EjecucionIdEjecucion")] Epcausapenal epcausapenal)
        {


            if (id != epcausapenal.Idepcausapenal)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    epcausapenal.Causapenal = mg.removeSpaces(mg.normaliza(epcausapenal.Causapenal));
                    epcausapenal.Delito = mg.removeSpaces(mg.normaliza(epcausapenal.Delito));
                    epcausapenal.Clasificaciondelito = mg.removeSpaces(mg.normaliza(epcausapenal.Clasificaciondelito));
                    epcausapenal.JuzgadoOrigen = mg.removeSpaces(mg.normaliza(epcausapenal.JuzgadoOrigen));
                    epcausapenal.FechaSentencia = epcausapenal.FechaSentencia;
                    epcausapenal.Multa = mg.removeSpaces(mg.normaliza(epcausapenal.Multa));
                    epcausapenal.Reparacion = mg.removeSpaces(mg.normaliza(epcausapenal.Reparacion));
                    epcausapenal.Firmeza = epcausapenal.Firmeza;
                    epcausapenal.Penaanos = epcausapenal.Penaanos;
                    epcausapenal.Penameses = epcausapenal.Penameses;
                    epcausapenal.Penadias = epcausapenal.Penadias;
                    epcausapenal.Apartir = epcausapenal.Apartir;
                    epcausapenal.EjecucionIdEjecucion = epcausapenal.EjecucionIdEjecucion;

                    _context.Update(epcausapenal);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EpcausapenalExists(epcausapenal.Idepcausapenal))
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
            return View(epcausapenal);
        }

        #endregion

        #region -Borrar Causa Penal-
        public JsonResult antesdeleteCP(Epcausapenal epcausapenal, Causapenal causapenal, int id, int idep)
        {
            var borrar = false;


            var antesdel = from epcp in _context.Epcausapenal
                           where epcp.Idepcausapenal == idep
                           select epcp;


            var tieneinstancia = from epi in _context.Epinstancia
                                 join epcp in _context.Epcausapenal on epi.EpcausapenalIdepcausapenal equals epcp.Idepcausapenal
                                 where epcp.Idepcausapenal == idep
                                 select epi;

            var tieneTermino = from ept in _context.Eptermino
                               join epcp in _context.Epcausapenal on ept.EpcausapenalIdepcausapenal equals epcp.Idepcausapenal
                               where epcp.Idepcausapenal == idep
                               select ept;


            if (tieneinstancia.Any() || tieneTermino.Any())
            {
                return Json(new { success = true, responseText = Url.Action("EditEpCausapenal", "Ejecucion"), borrar = borrar });
            }
            else
            {
                borrar = true;
                var epcausapenalDel = _context.Epcausapenal.SingleOrDefault(m => m.Idepcausapenal == id);
                
                _context.Epcausapenal.Remove(epcausapenalDel);
                _context.SaveChanges();
                _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                return Json(new { success = true, responseText = Url.Action("EditEpCausapenal", "Ejecucion"), borrar = borrar });
            }

        }
        public async Task<JsonResult> deleteEpCp(Epcausapenal epcausapenal, Historialeliminacion historialeliminacion, string[] datoEjecucion)
        {
            var borrar = false;
            var id = Int32.Parse(datoEjecucion[0]);
            var idep = Int32.Parse(datoEjecucion[1]);
            var razon = mg.normaliza(datoEjecucion[2]);
            var user = mg.normaliza(datoEjecucion[3]);

            var query = (from s in _context.Ejecucion
                         where s.IdEjecucion == idep
                         select s).FirstOrDefault();


            try
            {
                borrar = true;
                historialeliminacion.Id = id;
                historialeliminacion.Descripcion = "IDEJECUCION= " + query.IdEjecucion + " NOMBRE= " + query.Paterno + " " + query.Materno + " " + query.Nombre + " CE= " + query.Ce;
                historialeliminacion.Tipo = "EJECUCION";
                historialeliminacion.Razon = mg.normaliza(razon);
                historialeliminacion.Usuario = mg.normaliza(user);
                historialeliminacion.Fecha = DateTime.Now;
                historialeliminacion.Supervisor = mg.normaliza(query.Usuario);
                _context.Add(historialeliminacion);
                _context.SaveChanges();


                var epcausapenalDel = _context.Epcausapenal.SingleOrDefault(m => m.Idepcausapenal == id);
                _context.Epcausapenal.Remove(epcausapenalDel);
                _context.SaveChanges();
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                return Json(new { success = true, responseText = Url.Action("Index", "Ejecucion"), borrar = borrar });

            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("Index", "Ejecucion"), borrar = borrar });
            }

            return Json(new { success = true, idPersonas = Convert.ToString(id) });
        }

        #endregion

        #region -MenuProcesos-
        public async Task<IActionResult> MenuProcesos(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.User = user.ToString();

            var ejecucion = await _context.Ejecucion.SingleOrDefaultAsync(m => m.IdEjecucion == id);
            if (ejecucion == null)
            {
                return NotFound();
            }

            ViewData["joinTablesEjecucion"] = from ep in _context.Ejecucion
                                                  //join epAf in _context.Epatencionf on ep.IdEjecucion equals epAf.EjecucionIdEjecucion
                                                  //join epA in _context.Epamparo on ep.IdEjecucion equals epA.EjecucionIdEjecucion
                                                  //join epAu in _context.Epaudiencia on ep.IdEjecucion equals epAu.EjecucionIdEjecucion
                                              where ep.IdEjecucion == id
                                              select new EjecucionAFAA
                                              {
                                                  ejecucionVM = ep,
                                                  //epamparoVM = epA,
                                                  //epatencionfVM = epAf,
                                                  //epaudienciaVM = epAu
                                              };


            ViewData["joinTableEpInstancia"] = from ep in _context.Ejecucion
                                               join epcp in _context.Epcausapenal on ep.IdEjecucion equals epcp.EjecucionIdEjecucion
                                               join epi in _context.Epinstancia on epcp.Idepcausapenal equals epi.EpcausapenalIdepcausapenal
                                               where ep.IdEjecucion == id
                                               select new EjecucionCP
                                               {
                                                   ejecucionVM = ep,
                                                   epcausapenalVM = epcp,
                                                   epinstanciaVM = epi
                                               };

            ViewData["joinTablesTemino"] = from ep in _context.Ejecucion
                                           join epcp in _context.Epcausapenal on ep.IdEjecucion equals epcp.EjecucionIdEjecucion
                                           join ept in _context.Eptermino on epcp.Idepcausapenal equals ept.EpcausapenalIdepcausapenal
                                           where ep.IdEjecucion == id
                                           select new EjecucionCP
                                           {
                                               ejecucionVM = ep,
                                               epcausapenalVM = epcp,
                                               epterminoVM = ept
                                           };

            return View();
        }
        #endregion

        #region -EpAtencionF-
        public async Task<IActionResult> EpAtencionF(int? id, string Nombre, string Materno, string Paterno, string Ce, string nombree)
        {
            string NombreCompleto = $"{Paterno} {Materno} {Nombre}";
            NombreCompleto = (NombreCompleto == null || NombreCompleto.Trim() == "") ? nombree : NombreCompleto;                     
            // ViewBag.nombre = nombre;
            // ViewBag.cp = cp;
            // ViewBag.idpersona = idpersona;
            // ViewBag.supervisor = supervisor;
            ViewBag.EjecucionIdEjecucion = id;
            ViewBag.Nombre = NombreCompleto;
            ViewBag.Ce = Ce;
          
            List<Ejecucion> ejecucions = _context.Ejecucion.ToList();
            List<Epcausapenal> epcausapenals = _context.Epcausapenal.ToList();
            List<Epatencionf> epatencionfs = _context.Epatencionf.ToList();

            List<string> ListaUserEjecucion = new List<string>();
            foreach (var u in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(u, "Ejecucion"))
                {
                    ListaUserEjecucion.Add(u.ToString());
                }
                if (await userManager.IsInRoleAsync(u, "Coordinador Ejecucion"))
                {
                    ListaUserEjecucion.Add(u.ToString());
                }
            }

            ViewBag.QuienAtiende = ListaUserEjecucion.Where(r => ListaUserEjecucion.Any(f => !r.EndsWith("\u0040nortedgepms.com")));



            ViewData["AtencionFEP"] = from ep in ejecucions
                                      join epa in epatencionfs on ep.IdEjecucion equals epa.EjecucionIdEjecucion
                                      where ep.IdEjecucion == id
                                      select new EjecucionAFAA
                                      {
                                          epatencionfVM = epa
                                      };

            return View();
        }

        public async Task<IActionResult> CrearEpAtencionF(Epatencionf epatencionf, string[] datosAtencionF)
        {
            epatencionf.EjecucionIdEjecucion = Int32.Parse(datosAtencionF[0]);
            epatencionf.Turno = datosAtencionF[1];
            epatencionf.QuienAtiende = mg.removeSpaces(mg.normaliza(datosAtencionF[2]));
            epatencionf.Inicio = mg.validateDatetime(datosAtencionF[3]);
            epatencionf.Termino = mg.validateDatetime(datosAtencionF[4]);
            epatencionf.Observaciones = mg.removeSpaces(mg.normaliza(datosAtencionF[5]));


            _context.Add(epatencionf);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            return View();
        }


        public async Task<IActionResult> EditEpAtencionF(int? id,string Nombre,string Ce)
        {
            string charsToRemove = "?id=null";
            Ce = Ce.Replace(charsToRemove,"");                           
            ViewBag.Nombre = Nombre;
            ViewBag.Ce = Ce;
           

            if (id == null)
            {
                return NotFound();
            }
            var epatencionf = await _context.Epatencionf.SingleOrDefaultAsync(m => m.IdepAtencionF == id);
           
            #region Turno          
            List<string> LiataTurno = new List<string>();
            LiataTurno.Add("NA");
            LiataTurno.Add("JUZGADO 1");
            LiataTurno.Add("JUZGADO 2");
            LiataTurno.Add("JUZGADO 3");
            LiataTurno.Add("TURNO");

            ViewBag.LiataTurno = LiataTurno;

            var turno = LiataTurno.Select((item, i) => new { Item = item, Index = (int?)i });
            var selectturno = (from pair in turno
                               where pair.Item == epatencionf.Turno
                               select pair.Item).FirstOrDefault();

            ViewBag.Selectturno = selectturno;

            #endregion


            #region -Lista Quien atiende-
            List<string> ListaUserEjecucion = new List<string>();
            foreach (var u in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(u, "Ejecucion"))
                {
                    ListaUserEjecucion.Add(u.ToString().ToUpper());
                }
                if (await userManager.IsInRoleAsync(u, "Coordinador Ejecucion"))
                {
                    ListaUserEjecucion.Add(u.ToString().ToUpper());
                }
            }

            var tagged = ListaUserEjecucion.Select((item, i) => new { Item = item, Index = (int?)i });
            var index = (from pair in tagged
                         where pair.Item == epatencionf.QuienAtiende
                         select pair.Item).FirstOrDefault();

            ViewBag.QuienAtiende = ListaUserEjecucion.Where(r => ListaUserEjecucion.Any(f => !r.EndsWith("\u0040nortedgepms.com")));
            ViewBag.QuienAtiendeEdit = index;
            #endregion

            if (epatencionf == null)
            {
                return NotFound();
            }
            return View(epatencionf);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEpAtencionF(int id, [Bind("IdepAtencionF, Turno ,QuienAtiende, Inicio, Termino, Observaciones, GrupoAutoayuda, Observaciones, EjecucionIdEjecucion")] Epatencionf epatencionf, string Nombre, string Ce)
        {
    
            if (id != epatencionf.IdepAtencionF)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    epatencionf.Turno = mg.removeSpaces(mg.normaliza(epatencionf.Turno));
                    epatencionf.QuienAtiende = mg.removeSpaces(mg.normaliza(epatencionf.QuienAtiende));
                    epatencionf.Inicio = epatencionf.Inicio;
                    epatencionf.Termino = epatencionf.Termino;
                    epatencionf.Observaciones = mg.removeSpaces(mg.normaliza(epatencionf.Observaciones));
                    epatencionf.EjecucionIdEjecucion = epatencionf.EjecucionIdEjecucion;
                    epatencionf.IdepAtencionF = id;

                    _context.Update(epatencionf);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EpAtencionfExists(epatencionf.IdepAtencionF))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var idejecucion = (from e in _context.Ejecucion
                                   join epa in _context.Epatencionf on e.IdEjecucion equals epa.EjecucionIdEjecucion
                                   where epa.IdepAtencionF == id
                                   select e.IdEjecucion).FirstOrDefault();


                return RedirectToAction("EpAtencionF", new { id = idejecucion, nombree = Nombre,ce=Ce});
            }
            return View(epatencionf);
        }


        public async Task<JsonResult> DeleteAtencionF(Epatencionf epatencionf, Historialeliminacion historialeliminacion, int dato)
        {
            var borrar = false;

            var query = (from epa in _context.Epatencionf
                         where epa.IdepAtencionF == dato
                         select epa).FirstOrDefault();
            try
            {
                borrar = true;

                var epa = _context.Epatencionf.FirstOrDefault(m => m.IdepAtencionF == dato);
                _context.Epatencionf.Remove(epa);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);


                return Json(new { success = true, responseText = Url.Action("EpAtencionf", "Ejecucion"), borrar = borrar });
            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("EpAtencionf", "Ejecucion"), borrar = borrar });
            }
        }


        #endregion

        #region -EpAudiencia-
        public async Task<IActionResult> EpAudiencia(int? id, string Nombre, string Materno, string Paterno, string Ce,string nombree)
        {
            string NombreCompleto = $"{Paterno} {Materno} {Nombre}";
            NombreCompleto = (NombreCompleto == null || NombreCompleto.Trim() == "") ? nombree : NombreCompleto;
            // ViewBag.nombre = nombre;
            // ViewBag.cp = cp;
            // ViewBag.idpersona = idpersona;
            // ViewBag.supervisor = supervisor;
            ViewBag.EjecucionIdEjecucion = id;
            ViewBag.Nombre = NombreCompleto;
            ViewBag.Ce = Ce;

            List<Ejecucion> ejecucions = _context.Ejecucion.ToList();
            List<Epcausapenal> epcausapenals = _context.Epcausapenal.ToList();
            List<Epaudiencia> epaudiencias = _context.Epaudiencia.ToList();

            ViewData["AudienciaEP"] = from ep in ejecucions
                                      join epa in epaudiencias on ep.IdEjecucion equals epa.EjecucionIdEjecucion
                                      where ep.IdEjecucion == id
                                      select new EjecucionAFAA
                                      {
                                          epaudienciaVM = epa
                                      };


            var listaCausapenal = (from e in epcausapenals
                                   where e.EjecucionIdEjecucion == id
                                   select e.Causapenal).ToList();


            ViewBag.listaepcp = listaCausapenal;


            return View();
        }

        public async Task<IActionResult> CrearAudiencia(Epaudiencia epaudiencia, string[] datosAudiencia)
        {
            epaudiencia.EjecucionIdEjecucion = Int32.Parse(datosAudiencia[0]);
            epaudiencia.Causapenal = datosAudiencia[1];
            epaudiencia.Beneficio = mg.removeSpaces(mg.normaliza(datosAudiencia[2]));
            epaudiencia.InicioFirma = mg.validateDatetime(datosAudiencia[3]);
            epaudiencia.FinFirma = mg.validateDatetime(datosAudiencia[4]);
            epaudiencia.FechatrabajoIe = mg.validateDatetime(datosAudiencia[5]);
            epaudiencia.GrupoAutoayuda = mg.removeSpaces(mg.normaliza(datosAudiencia[6]));
            epaudiencia.Observaciones = mg.removeSpaces(mg.normaliza(datosAudiencia[7]));

            _context.Add(epaudiencia);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            return View();
        }

        public async Task<IActionResult> EditEpAudiencia(int? id, string Nombre, string Ce)
        {
            string charsToRemove = "?id=null";
            Ce = Ce.Replace(charsToRemove, "");
            ViewBag.Nombre = Nombre;
            ViewBag.Ce = Ce;

            if (id == null)
            {
                return NotFound();
            }

            var epaudiencia = await _context.Epaudiencia.SingleOrDefaultAsync(m => m.Idepaudiencia == id);
            #region Causapenal 
            List<Epcausapenal> epcausapenals = _context.Epcausapenal.ToList();
            var listaCausapenal = (from e in epcausapenals
                                   where e.EjecucionIdEjecucion == id
                                   select e.Causapenal).ToList();

            ViewBag.listaepcp = listaCausapenal;

            var turno = listaCausapenal.Select((item, i) => new { Item = item, Index = (int?)i });
            var selectcp = (from pair in turno
                            where pair.Item == epaudiencia.Causapenal
                            select pair.Item).FirstOrDefault();

            ViewBag.Selectcp = selectcp;
            #endregion
            if (epaudiencia == null)
            {
                return NotFound();
            }
            return View(epaudiencia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEpAudiencia(int id, [Bind("Idepaudiencia, Causapenal ,Beneficio, InicioFirma, FinFirma, FechatrabajoIe, GrupoAutoayuda, Observaciones, EjecucionIdEjecucion")] Epaudiencia epaudiencia,string Nombre, string Ce)
        {

            if (id != epaudiencia.Idepaudiencia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    epaudiencia.Causapenal = epaudiencia.Causapenal;
                    epaudiencia.Beneficio = epaudiencia.Beneficio;
                    epaudiencia.InicioFirma = epaudiencia.InicioFirma;
                    epaudiencia.FinFirma = epaudiencia.FinFirma;
                    epaudiencia.FechatrabajoIe = epaudiencia.FechatrabajoIe;
                    epaudiencia.GrupoAutoayuda = mg.removeSpaces(mg.normaliza(epaudiencia.GrupoAutoayuda));
                    epaudiencia.Observaciones = mg.removeSpaces(mg.normaliza(epaudiencia.Observaciones));
                    epaudiencia.EjecucionIdEjecucion = epaudiencia.EjecucionIdEjecucion;
                    epaudiencia.Idepaudiencia = id;

                    _context.Update(epaudiencia);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EpAudienciaExists(epaudiencia.Idepaudiencia))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var idejecucion = (from e in _context.Ejecucion
                                   join epa in _context.Epaudiencia on e.IdEjecucion equals epa.EjecucionIdEjecucion
                                   where epa.Idepaudiencia == id
                                   select e.IdEjecucion).FirstOrDefault();


                return RedirectToAction("EpAudiencia", new { id = idejecucion, nombree = Nombre, ce = Ce });
            }
            return View(epaudiencia);
        }

        public async Task<JsonResult> DeleteAudiencia(Epaudiencia epaudiencia, Historialeliminacion historialeliminacion, int dato)
        {
            var borrar = false;

            var query = (from epa in _context.Epaudiencia
                         where epa.Idepaudiencia == dato
                         select epa).FirstOrDefault();
            try
            {
                borrar = true;

                var epa = _context.Epaudiencia.FirstOrDefault(m => m.Idepaudiencia == dato);
                _context.Epaudiencia.Remove(epa);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                return Json(new { success = true, responseText = Url.Action("EpAudiencia", "Ejecucion"), borrar = borrar });
            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("EpAudiencia", "Ejecucion"), borrar = borrar });
            }
        }


        #endregion

        #region -EpAmparo-
        public async Task<IActionResult> EpAmparo(int? id, string Nombre, string Materno, string Paterno, string Ce,string nombree)
        {
            string NombreCompleto = $"{Paterno} {Materno} {Nombre}";
            NombreCompleto = (NombreCompleto == null || NombreCompleto.Trim() == "") ? nombree : NombreCompleto;
            // ViewBag.nombre = nombre;
            // ViewBag.cp = cp;
            // ViewBag.idpersona = idpersona;
            // ViewBag.supervisor = supervisor;
            ViewBag.EjecucionIdEjecucion = id;
            ViewBag.Nombre = NombreCompleto;
            ViewBag.Ce = Ce;
            List<Ejecucion> ejecucions = _context.Ejecucion.ToList();
            List<Epcausapenal> epcausapenals = _context.Epcausapenal.ToList();
            List<Epamparo> epamparos = _context.Epamparo.ToList();


            ViewData["AmparoEP"] = from ep in ejecucions
                                   join epa in epamparos on ep.IdEjecucion equals epa.EjecucionIdEjecucion
                                   where ep.IdEjecucion == id
                                   select new EjecucionAFAA
                                   {
                                       epamparoVM = epa
                                   };

            return View();
        }

        public async Task<IActionResult> CrearEpAmparo(Epamparo epamparo, string[] datosAmparo)
        {
            epamparo.EjecucionIdEjecucion = Int32.Parse(datosAmparo[0]);
            epamparo.Fecha = mg.validateDatetime(datosAmparo[1]);
            epamparo.Toca = mg.removeSpaces(mg.normaliza(datosAmparo[2]));
            epamparo.Observaciones = mg.removeSpaces(mg.normaliza(datosAmparo[3]));

            _context.Add(epamparo);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            return View();
        }

        public async Task<IActionResult> EditEpAmparo(int? id, string Nombre, string Ce)
        {
            string charsToRemove = "?id=null";
            Ce = Ce.Replace(charsToRemove, "");
            ViewBag.Nombre = Nombre;
            ViewBag.Ce = Ce;

            if (id == null)
            {
                return NotFound();
            }

            var epamparo = await _context.Epamparo.SingleOrDefaultAsync(m => m.Idepamparo == id);
            if (epamparo == null)
            {
                return NotFound();
            }
            return View(epamparo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEpAmparo(int id, [Bind("Idepamparo,Fecha ,Toca, Observaciones, EjecucionIdEjecucion")] Epamparo epamparo, string Nombre, string Ce)
        {

            if (id != epamparo.Idepamparo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    epamparo.Fecha = epamparo.Fecha;
                    epamparo.Toca = mg.removeSpaces(mg.normaliza(epamparo.Toca));
                    epamparo.Observaciones = mg.removeSpaces(mg.normaliza(epamparo.Observaciones));
                    epamparo.EjecucionIdEjecucion = epamparo.EjecucionIdEjecucion;
                    epamparo.Idepamparo = id;

                    _context.Update(epamparo);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EpAmparoExists(epamparo.Idepamparo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var idejecucion = (from e in _context.Ejecucion
                                   join epa in _context.Epamparo on e.IdEjecucion equals epa.EjecucionIdEjecucion
                                   where epa.Idepamparo == id
                                   select e.IdEjecucion).FirstOrDefault();


                return RedirectToAction("EpAmparo", new { id = idejecucion , nombree = Nombre, ce = Ce });
            }
            return View(epamparo);
        }

        public async Task<JsonResult> DeleteAmparo(Historialeliminacion historialeliminacion, int dato)
        {
            var borrar = false;

            var query = (from epi in _context.Epamparo
                         where epi.Idepamparo == dato
                         select epi).FirstOrDefault();
            try
            {
                borrar = true;

                var epa = _context.Epamparo.FirstOrDefault(m => m.Idepamparo == dato);
                _context.Epamparo.Remove(epa);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                return Json(new { success = true, responseText = Url.Action("EpAmparo", "Ejecucion"), borrar = borrar });
            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("EpAmparo", "Ejecucion"), borrar = borrar });
            }
        }

        #endregion

        #region -EPCP Instancia-
        public async Task<IActionResult> CrearEpCpInstancia(Epinstancia epinstancia, string[] datosInstancia)
        {
            epinstancia.EpcausapenalIdepcausapenal = Int32.Parse(datosInstancia[0]);
            epinstancia.Fecha = mg.validateDatetime(datosInstancia[1]);
            epinstancia.Multa = mg.removeSpaces(mg.normaliza(datosInstancia[2]));
            epinstancia.Reparacion = mg.removeSpaces(mg.normaliza(datosInstancia[3]));
            epinstancia.Firmeza = mg.validateDatetime(datosInstancia[4]);
            epinstancia.Penaanos = Int32.Parse(datosInstancia[5]);
            epinstancia.Penameses = Int32.Parse(datosInstancia[6]);
            epinstancia.Penadias = Int32.Parse(datosInstancia[7]);


            _context.Add(epinstancia);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
            return View();

        }

        // GET: Ejecucion/Edit/5
        public async Task<IActionResult> EditEpInstancia(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var epinstancia = await _context.Epinstancia.SingleOrDefaultAsync(m => m.Idepinstancia == id);
            if (epinstancia == null)
            {
                return NotFound();
            }
            return View(epinstancia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEpInstancia(int id, [Bind("Idepinstancia, Fecha, Multa, Reparacion, Firmeza, Penaanos, Penameses, Penadias, EpcausapenalIdepcausapenal")] Epinstancia epinstancia)
        {

            if (id != epinstancia.Idepinstancia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    epinstancia.Fecha = epinstancia.Fecha;
                    epinstancia.Multa = mg.removeSpaces(mg.normaliza(epinstancia.Multa));
                    epinstancia.Reparacion = mg.removeSpaces(mg.normaliza(epinstancia.Reparacion));
                    epinstancia.Firmeza = epinstancia.Firmeza;
                    epinstancia.Penaanos = epinstancia.Penaanos;
                    epinstancia.Penameses = epinstancia.Penameses;
                    epinstancia.Penadias = epinstancia.Penadias;
                    epinstancia.Idepinstancia = id;
                    epinstancia.EpcausapenalIdepcausapenal = epinstancia.EpcausapenalIdepcausapenal;

                    _context.Update(epinstancia);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EpinstancialExists(epinstancia.Idepinstancia))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var idejecucion = (from e in _context.Ejecucion
                                   join epcp in _context.Epcausapenal on e.IdEjecucion equals epcp.EjecucionIdEjecucion
                                   where epcp.Idepcausapenal == epinstancia.EpcausapenalIdepcausapenal
                                   select epcp.EjecucionIdEjecucion).FirstOrDefault();


                return RedirectToAction("MenuProcesos", new { id = idejecucion });
            }
            return View(epinstancia);
        }

        public async Task<JsonResult> DeleteEpInstancia(Historialeliminacion historialeliminacion, string[] datoInicio)
        {
            var borrar = false;
            var id = Int32.Parse(datoInicio[0]);
            var razon = mg.normaliza(datoInicio[1]);
            var user = mg.normaliza(datoInicio[2]);



            var queryhitorial = (from epi in _context.Epinstancia
                                 join epcp in _context.Epcausapenal on epi.EpcausapenalIdepcausapenal equals epcp.Idepcausapenal
                                 join ep in _context.Ejecucion on epcp.EjecucionIdEjecucion equals ep.IdEjecucion
                                 where epi.Idepinstancia == id
                                 select new EjecucionCP
                                 {
                                     epcausapenalVM = epcp,
                                     epinstanciaVM = epi,
                                     ejecucionVM = ep
                                 }).FirstOrDefault();

            var epa = _context.Eptermino.FirstOrDefault(m => m.Ideptermino == id);
            try
            {
                borrar = true;

                historialeliminacion.Id = id;
                historialeliminacion.Descripcion = "IDCAUSAPENAL= " + queryhitorial.epcausapenalVM.Idepcausapenal + " NOMBRE= " + queryhitorial.ejecucionVM.Paterno + " " + queryhitorial.ejecucionVM.Paterno + " " + queryhitorial.ejecucionVM.Paterno + " CP= " + queryhitorial.epcausapenalVM.Causapenal + " RAZON DE TERMINO= " + queryhitorial.epterminoVM.Formaconclucion;
                historialeliminacion.Tipo = "EPTERMINO";
                historialeliminacion.Razon = mg.normaliza(razon);
                historialeliminacion.Usuario = mg.normaliza(user);
                historialeliminacion.Fecha = DateTime.Now;
                historialeliminacion.Supervisor = mg.normaliza(queryhitorial.ejecucionVM.Usuario);
                _context.Add(historialeliminacion);
                _context.SaveChanges();


                _context.Eptermino.Remove(epa);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                var idejecucion = (from e in _context.Ejecucion
                                   join epcp in _context.Epcausapenal on e.IdEjecucion equals epcp.EjecucionIdEjecucion
                                   where epcp.Idepcausapenal == epa.EpcausapenalIdepcausapenal
                                   select epcp.EjecucionIdEjecucion).FirstOrDefault();


                return Json(new { success = true, responseText = Url.Action("MenuProcesos/" + idejecucion, "Ejecucion"), borrar = borrar });
            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                var idejecucion = (from e in _context.Ejecucion
                                   join epcp in _context.Epcausapenal on e.IdEjecucion equals epcp.EjecucionIdEjecucion
                                   where epcp.Idepcausapenal == epa.EpcausapenalIdepcausapenal
                                   select epcp.EjecucionIdEjecucion).FirstOrDefault();
                return Json(new { success = true, responseText = Url.Action("MenuProcesos/" + idejecucion, "Ejecucion"), borrar = borrar });
            }
        }


        #endregion

        #region -EPCP Termino-

        public async Task<IActionResult> CrearEpCpTermino(Eptermino eptermino, string idCausapenal, DateTime Fecha, string Formaconclucion, IList<IFormFile> files)
        {

            var idejecucion = (from cp in _context.Epcausapenal
                               join e in _context.Ejecucion on cp.EjecucionIdEjecucion equals e.IdEjecucion
                               where cp.Idepcausapenal == Int32.Parse(idCausapenal)
                               select cp.EjecucionIdEjecucion).FirstOrDefault();



            var idtermino = ((from tab1 in _context.Eptermino
                              select (int?)tab1.Ideptermino).Max() ?? 0) + 1;

            var path = "";
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string file_name = idtermino + "_" + idCausapenal + "_" + idejecucion + Path.GetExtension(formFile.FileName);
                    eptermino.Urldocumento = file_name;
                    var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "EvidenciaTermino");
                    var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                    await formFile.CopyToAsync(stream);
                    stream.Close();
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                }
            }

            if (ModelState.ErrorCount <= 1)
            {
                eptermino.EpcausapenalIdepcausapenal = Int32.Parse(idCausapenal);
                eptermino.Fecha = Fecha;
                eptermino.Formaconclucion = mg.normaliza(Formaconclucion);

                _context.Add(eptermino);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                eptermino = await _context.Eptermino.OrderByDescending(b => b.Ideptermino).FirstOrDefaultAsync();

                return RedirectToAction("Index", "Ejecucion");

            }
            return View(eptermino);
        }


        public async Task<IActionResult> EditEpTermino(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var eptermino = await _context.Eptermino.SingleOrDefaultAsync(m => m.Ideptermino == id);
            if (eptermino == null)
            {
                return NotFound();
            }
            return View(eptermino);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEpTermino(int id, [Bind("Ideptermino, Fecha, Formaconclucion")] Eptermino eptermino)
        {

            if (id != eptermino.Ideptermino)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    eptermino.Fecha = eptermino.Fecha;
                    eptermino.Formaconclucion = mg.removeSpaces(mg.normaliza(eptermino.Formaconclucion));
                    eptermino.Ideptermino = id;

                    _context.Update(eptermino);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EpterminoExists(eptermino.Ideptermino))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MenuProcesos));
            }
            return View(eptermino);
        }

        public async Task<JsonResult> DeleteEpTermino(Historialeliminacion historialeliminacion, string[] datoTermino)
        {
            var borrar = false;
            var id = Int32.Parse(datoTermino[0]);
            var razon = mg.normaliza(datoTermino[1]);
            var user = mg.normaliza(datoTermino[2]);



            var queryhitorial = (from ept in _context.Eptermino
                                 join epcp in _context.Epcausapenal on ept.EpcausapenalIdepcausapenal equals epcp.Idepcausapenal
                                 join ep in _context.Ejecucion on epcp.EjecucionIdEjecucion equals ep.IdEjecucion
                                 where ept.Ideptermino == id
                                 select new EjecucionCP
                                 {
                                     epcausapenalVM = epcp,
                                     epterminoVM = ept,
                                     ejecucionVM = ep
                                 }).FirstOrDefault();

            var epa = _context.Eptermino.FirstOrDefault(m => m.Ideptermino == id);
            try
            {
                borrar = true;

                historialeliminacion.Id = id;
                historialeliminacion.Descripcion = "IDCAUSAPENAL= " + queryhitorial.epcausapenalVM.Idepcausapenal + " NOMBRE= " + queryhitorial.ejecucionVM.Paterno + " " + queryhitorial.ejecucionVM.Paterno + " " + queryhitorial.ejecucionVM.Paterno + " CP= " + queryhitorial.epcausapenalVM.Causapenal + " RAZON DE TERMINO= " + queryhitorial.epterminoVM.Formaconclucion;
                historialeliminacion.Tipo = "EPTERMINO";
                historialeliminacion.Razon = mg.normaliza(razon);
                historialeliminacion.Usuario = mg.normaliza(user);
                historialeliminacion.Fecha = DateTime.Now;
                historialeliminacion.Supervisor = mg.normaliza(queryhitorial.ejecucionVM.Usuario);
                _context.Add(historialeliminacion);
                _context.SaveChanges();


                _context.Eptermino.Remove(epa);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                var idejecucion = (from e in _context.Ejecucion
                                   join epcp in _context.Epcausapenal on e.IdEjecucion equals epcp.EjecucionIdEjecucion
                                   where epcp.Idepcausapenal == epa.EpcausapenalIdepcausapenal
                                   select epcp.EjecucionIdEjecucion).FirstOrDefault();


                return Json(new { success = true, responseText = Url.Action("MenuProcesos/" + idejecucion, "Ejecucion"), borrar = borrar });
            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                var idejecucion = (from e in _context.Ejecucion
                                   join epcp in _context.Epcausapenal on e.IdEjecucion equals epcp.EjecucionIdEjecucion
                                   where epcp.Idepcausapenal == epa.EpcausapenalIdepcausapenal
                                   select epcp.EjecucionIdEjecucion).FirstOrDefault();
                return Json(new { success = true, responseText = Url.Action("MenuProcesos/" + idejecucion, "Ejecucion"), borrar = borrar });
            }
        }

        #endregion

        #region -CrearAudiencia-
        public async Task<IActionResult> listaEpCrearAudiencia(
           string sortOrder,
           string currentFilter,
           string searchString,
           string estadoSuper,
           string figuraJudicial,
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

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.User = user.ToString();
            ViewBag.Admin = false;
            ViewBag.Masteradmin = false;
            ViewBag.Archivo = false;


            //List<Archivoprestamo> queryArchivoHistorial = (from a in _context.Archivoprestamo
            //                                               group a by a.ArcchivoIdArchivo into grp
            //                                               select grp.OrderByDescending(a => a.IdArchivoPrestamo).FirstOrDefault()).ToList();

            var filter = from o in _context.Oficialia
                         where o.AsuntoOficio == "AUDIENCIA" && o.UsuarioTurnar == "uriel.ortega@dgepms.com"
                         select o;

            ViewData["CurrentFilter"] = searchString;
            ViewData["EstadoS"] = estadoSuper;
            ViewData["FiguraJ"] = figuraJudicial;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = filter.Where(acp => (acp.Paterno + " " + acp.Materno + " " + acp.Nombre).Contains(searchString.ToUpper()) ||
                                             (acp.Nombre + " " + acp.Paterno + " " + acp.Materno).Contains(searchString.ToUpper()) ||
                                             (acp.CarpetaEjecucion).Contains(searchString.ToUpper()) ||
                                             (acp.IdCarpetaEjecucion.ToString()).Contains(searchString));
            }


            filter = filter.OrderByDescending(o => o.FechaTermino);

            //a.personaVM.Paterno + " " + a.personaVM.Materno + " " + a.personaVM.Nombre).Contains(SearchString.ToUpper()) ||
            //                                  (a.personaVM.Nombre + " " + a.personaVM.Paterno + " " + a.personaVM.Materno).Contains(SearchString.ToUpper()) ||


            //switch (sortOrder)
            //{
            //    case "name_desc":
            //        filter = filter.OrderByDescending(acp => acp.ejecucionVM.Paterno);
            //        break;
            //    case "causa_penal_desc":
            //        filter = filter.OrderByDescending(acp => acp.ejecucionVM.Materno);
            //        break;
            //    case "estado_cumplimiento_desc":
            //        filter = filter.OrderByDescending(acp => acp.ejecucionVM.Nombre);
            //        break;

            //}
            int pageSize = 10;
            return View(await PaginatedList<Oficialia>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> CreateEpCrearAudiencia()
        {
            ViewBag.centrosPenitenciarios = _context.Centrospenitenciarios.Select(Centrospenitenciarios => Centrospenitenciarios.Nombrecentro).ToList();

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.User = user.ToString();

            #region -Lista Ejecucion-
            List<string> ListaUserEjecucion = new List<string>();
            foreach (var u in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(u, "Ejecucion"))
                {
                    ListaUserEjecucion.Add(u.ToString());
                }
                if (await userManager.IsInRoleAsync(u, "Coordinador Ejecucion"))
                {
                    ListaUserEjecucion.Add(u.ToString());
                }
            }

            ViewBag.LiataTurno = ViewBag.ListaGeneral = ListaUserEjecucion.Where(r => ListaUserEjecucion.Any(f => !r.EndsWith("\u0040nortedgepms.com")));
            #endregion

            #region -Lista Juzgado-
            List<string> Liatajuzgado = new List<string>();
            Liatajuzgado.Add("NA");
            Liatajuzgado.Add("JUZGADO 1");
            Liatajuzgado.Add("JUZGADO 2");
            Liatajuzgado.Add("JUZGADO 3");
            Liatajuzgado.Add("TURNO");

            ViewBag.Liatajuzgado = Liatajuzgado;
            #endregion

            #region -lista de ce-
            List<string> ListaCE = new List<string>();
            var ce = (from ep in _context.Ejecucion
                      select new EjecucionCP
                      {
                          ejecucionVM = ep,
                      }).ToList();

            ViewBag.lista = ce;
            #endregion

            return View();
        }
        // POST: Ejecucion/CreateEpCrearAudiencia
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateEpCrearAudiencia([Bind("IdOficialia,Capturista,Recibe,MetodoNotificacion,NumOficio,FechaRecepcion,FechaEmision,Expide,ReferenteImputado,Sexo,Paterno,Materno,Nombre,IdCarpetaEjecucion,CarpetaEjecucion,Juzgado,QuienAsistira,IdCausaPenal,CausaPenal,DelitoTipo,AutoVinculacion,ExisteVictima,NombreVictima,DireccionVictima,AsuntoOficio,TieneTermino,FechaTermino,UsuarioTurnar,Entregado,FechaSeguimiento,RutaArchivo,Seguimiento,Observaciones")] Oficialia oficialiaAudiencia)
        //{

        //    oficialiaAudiencia.Capturista = oficialiaAudiencia.Capturista;
        //    oficialiaAudiencia.Recibe = oficialiaAudiencia.Recibe;
        //    oficialiaAudiencia.MetodoNotificacion = oficialiaAudiencia.MetodoNotificacion;
        //    oficialiaAudiencia.NumOficio = oficialiaAudiencia.NumOficio;
        //    oficialiaAudiencia.FechaEmision = oficialiaAudiencia.FechaEmision;
        //    oficialiaAudiencia.Expide = oficialiaAudiencia.Expide;
        //    oficialiaAudiencia.ReferenteImputado = oficialiaAudiencia.ReferenteImputado;
        //    oficialiaAudiencia.Sexo = oficialiaAudiencia.Sexo;
        //    oficialiaAudiencia.IdCausaPenal = oficialiaAudiencia.IdCausaPenal;
        //    oficialiaAudiencia.CausaPenal = oficialiaAudiencia.CausaPenal;
        //    oficialiaAudiencia.DelitoTipo = oficialiaAudiencia.DelitoTipo;
        //    oficialiaAudiencia.AutoVinculacion = oficialiaAudiencia.AutoVinculacion;
        //    oficialiaAudiencia.ExisteVictima = oficialiaAudiencia.ExisteVictima;
        //    oficialiaAudiencia.DireccionVictima = oficialiaAudiencia.DireccionVictima;
        //    oficialiaAudiencia.AsuntoOficio = oficialiaAudiencia.AsuntoOficio;
        //    oficialiaAudiencia.TieneTermino = oficialiaAudiencia.TieneTermino;
        //    oficialiaAudiencia.UsuarioTurnar = oficialiaAudiencia.UsuarioTurnar;
        //    oficialiaAudiencia.Entregado = oficialiaAudiencia.Entregado;
        //    oficialiaAudiencia.Observaciones = oficialiaAudiencia.Observaciones;
        //    oficialiaAudiencia.FechaSeguimiento = oficialiaAudiencia.FechaSeguimiento;
        //    oficialiaAudiencia.RutaArchivo = oficialiaAudiencia.RutaArchivo;
        //    oficialiaAudiencia.Seguimiento = oficialiaAudiencia.Seguimiento;


        //    oficialiaAudiencia.FechaTermino = oficialiaAudiencia.FechaTermino;
        //    oficialiaAudiencia.FechaRecepcion = oficialiaAudiencia.FechaRecepcion;
        //    oficialiaAudiencia.Juzgado = mg.removeSpaces(mg.normaliza(oficialiaAudiencia.Juzgado));
        //    oficialiaAudiencia.QuienAsistira = mg.removeSpaces(mg.normaliza(oficialiaAudiencia.QuienAsistira));
        //    oficialiaAudiencia.CarpetaEjecucion = mg.removeSpaces(mg.normaliza(oficialiaAudiencia.CarpetaEjecucion));
        //    oficialiaAudiencia.Paterno = mg.removeSpaces(mg.normaliza(oficialiaAudiencia.Paterno));
        //    oficialiaAudiencia.Materno = mg.removeSpaces(mg.normaliza(oficialiaAudiencia.Materno));
        //    oficialiaAudiencia.Nombre = mg.removeSpaces(mg.normaliza(oficialiaAudiencia.Nombre));
        //    oficialiaAudiencia.IdOficialia = id;
        //    oficialiaAudiencia.IdCarpetaEjecucion = oficialiaAudiencia.IdCarpetaEjecucion;

        //    _context.Add(epcrearaudiencia);
        //    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
        //    return RedirectToAction(nameof(listaEpCrearAudiencia));

        //    return View(epcrearaudiencia);
        //}

        public async Task<IActionResult> EditEpCreateAudiencia(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var audienciaOficailia = await _context.Oficialia.SingleOrDefaultAsync(m => m.IdOficialia == id);


            #region -Lista Quien atiende-
            List<string> ListaUser = new List<string>();
            foreach (var u in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(u, "Ejecucion"))
                {
                    ListaUser.Add(u.ToString().ToUpper());
                }
                if (await userManager.IsInRoleAsync(u, "Coordinador Ejecucion"))
                {
                    ListaUser.Add(u.ToString().ToUpper());
                }
            }

            var tagged = ListaUser.Select((item, i) => new { Item = item, Index = (int?)i });
            //var index = (from pair in tagged
                         //where pair.Item == audienciaOficailia.QuienAsistira.ToUpper()
                         //select pair.Item).FirstOrDefault();
            var index = tagged.FirstOrDefault(pair => pair.Item == audienciaOficailia.QuienAsistira?.ToUpper())?.Item;
            //var index = tagged.FirstOrDefault(pair => pair.Item == audienciaOficailia.QuienAsistira.ToUpper())?.Item ?;

            //_context.Oficialia.FirstOrDefault(table => table.IdOficialia == (Convert.ToInt32(datosidAudiencia[i])))?.Juzgado ?? string.Empty,

            ViewBag.ListaUser = ListaUser.Where(r => ListaUser.Any(f => !r.EndsWith("\u0040nortedgepms.com")));
            ViewBag.UserEdit = index;
            #endregion



            #region -JUZGADO-

            List<string> Liatajuzgado = new List<string>();
            Liatajuzgado.Add("NA");
            Liatajuzgado.Add("JUZGADO 1");
            Liatajuzgado.Add("JUZGADO 2");
            Liatajuzgado.Add("JUZGADO 3");

            ViewBag.Liatajuzgado = Liatajuzgado;

            var turno = Liatajuzgado.Select((item, i) => new { Item = item, Index = (int?)i });
            var selectturno = (from pair in turno
                               where pair.Item == audienciaOficailia.Juzgado
                               select pair.Item).FirstOrDefault();

            ViewBag.Selectturno = selectturno;

            #endregion

            if (audienciaOficailia == null)
            {
                return NotFound();
            }
            return View(audienciaOficailia);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEpCreateAudiencia(int id, [Bind("IdOficialia,Capturista,Recibe,MetodoNotificacion,NumOficio,FechaRecepcion,FechaEmision,Expide,ReferenteImputado,Sexo,Paterno,Materno,Nombre,IdCarpetaEjecucion,CarpetaEjecucion,Juzgado,QuienAsistira,IdCausaPenal,CausaPenal,DelitoTipo,AutoVinculacion,ExisteVictima,NombreVictima,DireccionVictima,AsuntoOficio,TieneTermino,FechaTermino,UsuarioTurnar,Entregado,FechaSeguimiento,RutaArchivo,Seguimiento,Observaciones")] Oficialia oficialiaAudiencia)
        {

            if (id != oficialiaAudiencia.IdOficialia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try 
                {
                    oficialiaAudiencia.Capturista = oficialiaAudiencia.Capturista;
                    oficialiaAudiencia.Recibe = oficialiaAudiencia.Recibe;
                    oficialiaAudiencia.MetodoNotificacion = oficialiaAudiencia.MetodoNotificacion;
                    oficialiaAudiencia.NumOficio = oficialiaAudiencia.NumOficio;
                    oficialiaAudiencia.FechaEmision = oficialiaAudiencia.FechaEmision;
                    oficialiaAudiencia.Expide = oficialiaAudiencia.Expide;
                    oficialiaAudiencia.ReferenteImputado = oficialiaAudiencia.ReferenteImputado;
                    oficialiaAudiencia.Sexo = oficialiaAudiencia.Sexo;
                    oficialiaAudiencia.IdCausaPenal = oficialiaAudiencia.IdCausaPenal;
                    oficialiaAudiencia.CausaPenal = oficialiaAudiencia.CausaPenal;
                    oficialiaAudiencia.DelitoTipo = oficialiaAudiencia.DelitoTipo;
                    oficialiaAudiencia.AutoVinculacion = oficialiaAudiencia.AutoVinculacion;
                    oficialiaAudiencia.ExisteVictima = oficialiaAudiencia.ExisteVictima;
                    oficialiaAudiencia.DireccionVictima = oficialiaAudiencia.DireccionVictima;
                    oficialiaAudiencia.AsuntoOficio = oficialiaAudiencia.AsuntoOficio;
                    oficialiaAudiencia.TieneTermino = oficialiaAudiencia.TieneTermino;
                    oficialiaAudiencia.UsuarioTurnar = oficialiaAudiencia.UsuarioTurnar;
                    oficialiaAudiencia.Entregado = oficialiaAudiencia.Entregado;  
                    oficialiaAudiencia.Observaciones = oficialiaAudiencia.Observaciones;  
                    oficialiaAudiencia.FechaSeguimiento = oficialiaAudiencia.FechaSeguimiento;  
                    oficialiaAudiencia.RutaArchivo = oficialiaAudiencia.RutaArchivo;  
                    oficialiaAudiencia.Seguimiento = oficialiaAudiencia.Seguimiento;  
                    oficialiaAudiencia.FechaTermino = oficialiaAudiencia.FechaTermino;
                    oficialiaAudiencia.FechaRecepcion = oficialiaAudiencia.FechaRecepcion;
                    oficialiaAudiencia.Juzgado = mg.removeSpaces(mg.normaliza(oficialiaAudiencia.Juzgado));
                    oficialiaAudiencia.QuienAsistira = mg.removeSpaces(mg.normaliza(oficialiaAudiencia.QuienAsistira));
                    oficialiaAudiencia.CarpetaEjecucion = mg.removeSpaces(mg.normaliza(oficialiaAudiencia.CarpetaEjecucion));
                    oficialiaAudiencia.Paterno = mg.removeSpaces(mg.normaliza(oficialiaAudiencia.Paterno));
                    oficialiaAudiencia.Materno = mg.removeSpaces(mg.normaliza(oficialiaAudiencia.Materno));
                    oficialiaAudiencia.Nombre = mg.removeSpaces(mg.normaliza(oficialiaAudiencia.Nombre));
                    oficialiaAudiencia.IdOficialia = id;
                    oficialiaAudiencia.IdCarpetaEjecucion = oficialiaAudiencia.IdCarpetaEjecucion;

                    _context.Update(oficialiaAudiencia);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!oficialiaAudienciaExists(oficialiaAudiencia.IdOficialia))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(listaEpCrearAudiencia));
            }
            return View(oficialiaAudiencia);
        }


        public async Task<JsonResult> DeleteCrearAudiencia(Oficialia epcrearaudiencia, Historialeliminacion historialeliminacion, int dato)
        {
            var borrar = false;
            try
            {
                borrar = true;

                var epa = _context.Oficialia.FirstOrDefault(m => m.IdOficialia == dato);
                _context.Oficialia.Remove(epa);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                return Json(new { success = true, responseText = Url.Action("listaEpCrearAudiencia", "Ejecucion"), borrar = borrar });
            }
            catch (Exception ex)
            {
                var error = ex;
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("listaEpCrearAudiencia", "Ejecucion"), borrar = borrar });
            }
        }

        #endregion
        #region -Imprimir Audiencias Selecionadas -
        public void ImprimirAudiencias(string[] datosidAudiencia)
        {
            if (datosidAudiencia.Length == 0)
            {
                return;
            }

            DateTime now = DateTime.Now;
            int weekNumber = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            DateTime startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
            DateTime endOfWeek = startOfWeek.AddDays(4);


            string templatePath = this._hostingEnvironment.WebRootPath + "\\Documentos\\templateAudiencia.docx";
            string resultPath = this._hostingEnvironment.WebRootPath + "\\Documentos\\AudienciasEP.docx";

            DocumentCore dc = DocumentCore.Load(templatePath);

            var dataSource = new
            {
                fechaElavoracion = DateTime.Now.ToString("dd/MM/yyyy"),
                semana = startOfWeek.ToString("dd/MM/yyyy") + " Al " + endOfWeek.ToString("dd/MM/yyyy"),
                Audiencia = new object[datosidAudiencia.Length]
            };

            for (int i = 0; i < datosidAudiencia.Length; i++)
            {
                dataSource.Audiencia[i] = new
                {
                    IdepcrearAudiencia = datosidAudiencia[i],
                    FechaAudiencia = (_context.Oficialia.FirstOrDefault(table => table.IdOficialia == (Convert.ToInt32(datosidAudiencia[i])))?.FechaTermino ?? DateTime.MinValue).ToString(),

                    FechaNotificacion = (_context.Oficialia.FirstOrDefault(table => table.IdOficialia == (Convert.ToInt32(datosidAudiencia[i])))?.FechaRecepcion ?? DateTime.MinValue).ToString(),

                    Juzgado = _context.Oficialia.FirstOrDefault(table => table.IdOficialia == (Convert.ToInt32(datosidAudiencia[i])))?.Juzgado ?? string.Empty,

                    Usuario = _context.Oficialia.FirstOrDefault(table => table.IdOficialia == (Convert.ToInt32(datosidAudiencia[i])))?.QuienAsistira ?? string.Empty,

                    Ce = _context.Oficialia.FirstOrDefault(table => table.IdOficialia == (Convert.ToInt32(datosidAudiencia[i])))?.CarpetaEjecucion ?? string.Empty,

                    Sentenciado = (_context.Oficialia.FirstOrDefault(table => table.IdOficialia == (Convert.ToInt32(datosidAudiencia[i])))?.Paterno + " " +
                                   _context.Oficialia.FirstOrDefault(table => table.IdOficialia == (Convert.ToInt32(datosidAudiencia[i])))?.Materno + " " +
                                   _context.Oficialia.FirstOrDefault(table => table.IdOficialia == (Convert.ToInt32(datosidAudiencia[i])))?.Nombre) ?? string.Empty

            };
            }

            dc.MailMerge.ClearOptions = MailMergeClearOptions.RemoveEmptyRanges;
            dc.MailMerge.Execute(dataSource);
            dc.Save(resultPath);

        }
        #endregion

        public async Task<IActionResult> WarningEjecucion()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.showSupervisor = false;

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

        public async Task<IActionResult> filtra(string currentFilter)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            Boolean flagCoordinador = false, flagMaster = false, flagOperativo = false, flagDirector = false;
            string usuario = user.ToString();
            DateTime fechaAudiencia = (DateTime.Today).AddDays(7);
            DateTime terminoAlerta = (DateTime.Today).AddDays(-1);
            DateTime fechaControl = (DateTime.Today).AddDays(3);
            DateTime fechaInformeCoordinador = (DateTime.Today).AddDays(60);
            DateTime fechaHoy = DateTime.Today;

            if (User.Identity.Name == "jazmin.flores@dgepms.com") {
                flagDirector = true;
            }
            if (currentFilter == null)
            {
                ViewBag.Filtro = "TODOS";
            }
            else
            {
                ViewBag.Filtro = currentFilter;
            }

            foreach (var rol in roles)
            {
                if (rol == "Coordinador Ejecucion")
                {
                    flagCoordinador = true;
                }
            }
            foreach (var rol in roles)
            {
                if (rol == "Masteradmin")
                {
                    flagMaster = true;
                }
            }
            
            foreach (var rol in roles)
            {
                if (rol == "Operativo")
                {
                    flagOperativo = true;
                }
            }
            foreach (var rol in roles)
            {
                if (rol == "Director")
                {
                    flagDirector = true;
                }
            }

            List<Ejecucion> ejecucionVM = _context.Ejecucion.ToList();
            List <Oficialia>  oficialiaVM  = _context.Oficialia.ToList();


            var ViewDataAlertasVari = Enumerable.Empty<EjecucionWarningViewModel>();
            switch (currentFilter)
            {
                case "TODOS":
                    ViewDataAlertasVari = from o in oficialiaVM
                                          where  o.AsuntoOficio == "AUDIENCIA" && o.UsuarioTurnar.ToLower() == "uriel.ortega@dgepms.com" && o.FechaTermino != null && o.FechaTermino < fechaAudiencia && o.FechaTermino > terminoAlerta
                                          orderby o.FechaTermino
                                          select new EjecucionWarningViewModel
                                          {
                                              oficialiaVM = o,
                                              tipoAdvertencia = "Audiencia"
                                          };
                    break;
                case "AUDIENCIAS":
                    ViewDataAlertasVari = from o in oficialiaVM
                                          where o.AsuntoOficio == "AUDIENCIA" && o.UsuarioTurnar.ToLower() == "uriel.ortega@dgepms.com" && o.FechaTermino != null && o.FechaTermino < fechaAudiencia && o.FechaTermino > terminoAlerta
                                          orderby o.FechaTermino
                                          select new EjecucionWarningViewModel
                                          {
                                              oficialiaVM = o,
                                              tipoAdvertencia = "Audiencia"
                                          };
                    break;
            }
            var warnings = Enumerable.Empty<EjecucionWarningViewModel>();

            ViewData["alertas"] = ViewDataAlertasVari;

            return Json(new
            {
                success = true,
                user = usuario,
                admin = flagCoordinador || flagMaster || flagDirector || flagOperativo,
                //ViewData["alertas"] se usa como variable de esta funcion y no sirve como ViewData
                query = ViewData["alertas"]
            });
        }

        private bool EjecucionExists(int id)
        {
            return _context.Ejecucion.Any(e => e.IdEjecucion == id);
        }
        private bool EpcausapenalExists(int id)
        {
            return _context.Epcausapenal.Any(e => e.Idepcausapenal == id);
        }
        private bool EpinstancialExists(int id)
        {
            return _context.Epinstancia.Any(e => e.Idepinstancia == id);
        }
        private bool EpterminoExists(int id)
        {
            return _context.Eptermino.Any(e => e.Ideptermino == id);
        }
        private bool EpAmparoExists(int id)
        {
            return _context.Epamparo.Any(e => e.Idepamparo == id);
        }
        private bool EpAudienciaExists(int id)
        {
            return _context.Epaudiencia.Any(e => e.Idepaudiencia == id);
        }
        private bool EpAtencionfExists(int id)
        {
            return _context.Epatencionf.Any(e => e.IdepAtencionF == id);
        }
        private bool oficialiaAudienciaExists(int id)
        {
            return _context.Oficialia.Any(e => e.IdOficialia == id);
        }


        #region -MenuEjecucion-
        public IActionResult MenuEjecucion()
        {
            DateTime fechaAudiencia = (DateTime.Today).AddDays(7);
            DateTime terminoAlerta = (DateTime.Today).AddDays(-1);
            List<Oficialia> oficialiaVM = _context.Oficialia.ToList();

            var warningPlaneacion = from o in oficialiaVM
                                    where o.AsuntoOficio == "AUDIENCIA" && o.UsuarioTurnar.ToLower() == "uriel.ortega@dgepms.com" && o.FechaTermino != null && o.FechaTermino < fechaAudiencia && o.FechaTermino > terminoAlerta
                                    select new EjecucionWarningViewModel
                                    {
                                        oficialiaVM = o,
                                        tipoAdvertencia = "Audiencia"
                                    };

            ViewBag.Warnings = warningPlaneacion.Count();

            return View(); 
        }
        #endregion
    }
}
