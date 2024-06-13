using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using Newtonsoft.Json;
using scorpioweb.Class;
using scorpioweb.Models;
using Syncfusion.EJ2.Linq;


namespace scorpioweb.Controllers
{
    [Authorize]
    public class ReinsercionController : Controller
    {
        private readonly penas2Context _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHubContext<HubNotificacion> _hubContext;
        private readonly RoleManager<IdentityRole> roleManager;
        MetodosGenerales mg = new MetodosGenerales();
        public ReinsercionController(penas2Context context, IHostingEnvironment hostingEnvironment,
                                  RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IHubContext<HubNotificacion> hubContext)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
            _hubContext = hubContext;

        }

        #region - index-
        // GET: Reinsercion
        public async Task<IActionResult> Index(
            string sortOrder,
           string currentFilter,
           string searchString,
           string estadoSuper,
           string figuraJudicial,
           int? pageNumber)
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

            var query = (from t1 in (
                        from p in _context.Persona
                        select new ReinsercionMCYSCPLCCURSVM
                        {
                            IdTabla = p.IdPersona,
                            Paterno = p.Paterno,
                            Materno = p.Materno,
                            Nombre = p.Nombre,
                            ClaveUnica = p.ClaveUnicaScorpio,
                            NomTabla = "MCYSCP"
                        }).Concat(
                            from pcl in _context.Personacl
                            select new ReinsercionMCYSCPLCCURSVM
                            {
                                IdTabla = pcl.IdPersonaCl,
                                Paterno = pcl.Paterno,
                                Materno = pcl.Materno,
                                Nombre = pcl.Nombre,
                                ClaveUnica = pcl.ClaveUnicaScorpio,
                                NomTabla = "CL"
                            }
                        )
                         join ex in _context.Expedienteunico on t1.ClaveUnica equals ex.ClaveUnicaScorpio
                         group t1 by new { t1.Paterno, t1.Materno, t1.Nombre, t1.ClaveUnica, t1.NomTabla } into g
                         where g.Key.ClaveUnica != null
                         select new ReinsercionMCYSCPLCCURSVM
                         {
                             IdTabla = g.Max(x => x.IdTabla),
                             Paterno = g.Max(x => x.Paterno),
                             Materno = g.Max(x => x.Materno),
                             Nombre = g.Max(x => x.Nombre),
                             ClaveUnica = g.Max(x => x.ClaveUnica),
                             NomTabla = g.Key.NomTabla

                         }
                    );

            var tamano = query.Count();

            // Ejecutar la consulta


            ViewData["CurrentFilter"] = searchString;
            ViewData["EstadoS"] = estadoSuper;
            ViewData["FiguraJ"] = figuraJudicial;

            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => (x.Paterno + " " + x.Materno + " " + x.Nombre).Contains(searchString.ToUpper()) ||
                                              (x.Nombre + " " + x.Paterno + " " + x.Materno).Contains(searchString.ToUpper()) ||
                                              (x.IdTabla).ToString().Contains(searchString));
            }

            query.OrderBy(x => x.IdTabla);

            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(x => x.Paterno);
                    break;
                case "causa_penal_desc":
                    query = query.OrderByDescending(x => x.Materno);
                    break;
                case "estado_cumplimiento_desc":
                    query = query.OrderByDescending(x => x.Nombre);
                    break;
                default:
                    query = query.OrderByDescending(x => x.Materno);
                    break;
            }
            int pageSize = 15;
            return View(await PaginatedList<ReinsercionMCYSCPLCCURSVM>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Reinsercion
        public async Task<IActionResult> Reinsercion(
            string sortOrder,
           string currentFilter,
           string searchString,
           string estadoSuper,
           string figuraJudicial,
           int? pageNumber)
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

            var query = (from r in _context.Reinsercion
                         join p in _context.Personacl on r.IdTabla equals p.IdPersonaCl.ToString()
                         join d in _context.Domiciliocl on p.IdPersonaCl equals d.PersonaclIdPersonacl
                         join e in _context.Estudioscl on p.IdPersonaCl equals e.PersonaClIdPersonaCl
                         join t in _context.Trabajocl on p.IdPersonaCl equals t.PersonaClIdPersonaCl
                         join a in _context.Actividadsocialcl on p.IdPersonaCl equals a.PersonaClIdPersonaCl
                         join s in _context.Saludfisicacl on p.IdPersonaCl equals s.PersonaClIdPersonaCl
                         join pv in _context.Psicologiavincu on p.IdPersonaCl equals pv.PersonaClIdPersonaCl into pvGroup
                         from pv in pvGroup.DefaultIfEmpty()
                         join ex in _context.Expedienteunico on p.IdPersonaCl.ToString() equals ex.Personacl into exGroup
                         from ex in exGroup.DefaultIfEmpty()
                         join ep in _context.Ejecucion on ex.Ejecucion equals ep.IdEjecucion.ToString() into epGroup
                         from ep in epGroup.DefaultIfEmpty()
                         join epcp in _context.Epcausapenal on ep.IdEjecucion equals epcp.EjecucionIdEjecucion into epcpGroup
                         from epcp in epcpGroup.DefaultIfEmpty()
                         join scl in _context.Supervisioncl on p.IdPersonaCl equals scl.PersonaclIdPersonacl into sclGroup
                         from scl in sclGroup.DefaultIfEmpty()
                         join cpcl in _context.Causapenalcl on scl.CausaPenalclIdCausaPenalcl equals cpcl.IdCausaPenalcl into cpclGroup
                         from cpcl in cpclGroup.DefaultIfEmpty()
                         join dcl in _context.Delitocl on cpcl.IdCausaPenalcl equals dcl.CausaPenalclIdCausaPenalcl into dclGroup
                         from dcl in dclGroup.DefaultIfEmpty()
                         select new ReinsercionMCYSCPLCCURSVM
                         {
                             IdReinsercion = r.IdReinsercion,
                             IdTabla = p.IdPersonaCl,
                             Nombre = string.Concat(p.Paterno, " ", p.Materno, " ", p.Nombre),
                             Causapenal = ep.Ce ?? cpcl.CausaPenal,
                             Delito = epcp.Delito ?? dcl.Tipo,
                             EstadoVinculacion = r.Estado,
                             NomTabla = "Libertad Condicionada",
                             EstadoSupervision = scl.EstadoSupervision
                         }).Concat(from r in _context.Reinsercion
                        join p in _context.Persona on r.IdTabla equals p.IdPersona.ToString()
                        join d in _context.Domicilio on p.IdPersona equals d.PersonaIdPersona
                        join e in _context.Estudios on p.IdPersona equals e.PersonaIdPersona
                        join t in _context.Trabajo on p.IdPersona equals t.PersonaIdPersona
                        join a in _context.Actividadsocial on p.IdPersona equals a.PersonaIdPersona
                        join s in _context.Saludfisica on p.IdPersona equals s.PersonaIdPersona
                        join ex in _context.Expedienteunico on p.IdPersona.ToString() equals ex.Personacl into exGroup
                        from ex in exGroup.DefaultIfEmpty()
                        join ep in _context.Ejecucion on ex.Ejecucion equals ep.IdEjecucion.ToString() into epGroup
                        from ep in epGroup.DefaultIfEmpty()
                        join epcp in _context.Epcausapenal on ep.IdEjecucion equals epcp.EjecucionIdEjecucion into epcpGroup
                        from epcp in epcpGroup.DefaultIfEmpty()
                        join scl in _context.Supervision on p.IdPersona equals scl.PersonaIdPersona into sclGroup
                        from scl in sclGroup.DefaultIfEmpty()
                        join cpcl in _context.Causapenal on scl.CausaPenalIdCausaPenal equals cpcl.IdCausaPenal into cpclGroup
                        from cpcl in cpclGroup.DefaultIfEmpty()
                        join dcl in _context.Delitocl on cpcl.IdCausaPenal equals dcl.CausaPenalclIdCausaPenalcl into dclGroup
                        from dcl in dclGroup.DefaultIfEmpty()
                        select new ReinsercionMCYSCPLCCURSVM
                        {
                            IdReinsercion = r.IdReinsercion,
                            IdTabla = p.IdPersona,
                            Nombre = string.Concat(p.Paterno, " ", p.Materno, " ", p.Nombre),
                            Causapenal = ep.Ce ?? cpcl.CausaPenal,
                            Delito = epcp.Delito ?? dcl.Tipo,
                            EstadoVinculacion = r.Estado,
                            NomTabla = "MCYSCP",
                            EstadoSupervision = scl.EstadoSupervision
                        });

    
            var result = query.ToList();

            //Ejecutar la consulta


           ViewData["CurrentFilter"] = searchString;
            ViewData["EstadoS"] = estadoSuper;
            ViewData["FiguraJ"] = figuraJudicial;

            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => (x.Nombre).Contains(searchString.ToUpper()) ||
                                              (x.IdTabla).ToString().Contains(searchString));
            }

            query.OrderBy(x => x.IdTabla);

            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(x => x.Nombre);
                    break;
                case "causa_penal_desc":
                    query = query.OrderByDescending(x => x.Nombre);
                    break;
                case "estado_cumplimiento_desc":
                    query = query.OrderByDescending(x => x.Nombre);
                    break;
                default:
                    query = query.OrderByDescending(x => x.Nombre);
                    break;
            }
            int pageSize = 15;
            return View(await PaginatedList<ReinsercionMCYSCPLCCURSVM>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        #endregion
        // GET: Reinsercion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reinsercion = await _context.Reinsercion
                .SingleOrDefaultAsync(m => m.IdReinsercion == id);
            if (reinsercion == null)
            {
                return NotFound();
            }

            return View(reinsercion);
        }


        #region - Crear Reinsercion -

        // GET: Reinsercion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reinsercion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdReinsercion,IdTabla,Tabla,Lugar,Estado")] Reinsercion reinsercion)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            List<Persona> listap = new List<Persona>();
            List<Personacl> listapcl = new List<Personacl>();

            if (ModelState.IsValid)
            {
                //Sacar info de su tabla

                List<object> miLista = new List<object>(); // Esta es una función hipotética que obtiene los datos

                var persona = _context.Persona
                                   .SingleOrDefault(m => m.IdPersona == Int32.Parse(reinsercion.IdTabla));

                var personacl = _context.Personacl
                                  .SingleOrDefault(m => m.IdPersonaCl == Int32.Parse(reinsercion.IdTabla));


                _context.Add(reinsercion);
                await _context.SaveChangesAsync();


                foreach (var rol in roles)
                {
                    if (rol != "Vinculacion")
                    {
                        await _hubContext.Clients.Group("nuevaCanalizacion").SendAsync("sendMessage", persona.IdPersona + " " + persona.NombreCompleto);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reinsercion);
        }

        [HttpPost]
        public async Task<JsonResult> CrearReinsercionPorSupervisor([FromBody] Reinsercion reinsercion)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            if (ModelState.IsValid)
            {            
                foreach (var rol in roles)
                {
                    if (rol != "Vinculacion")
                    {
                        await _hubContext.Clients.Group("nuevaCanalizacion").SendAsync("sendMessage", "Hay una nueva canalizacion");
                    }
                }

                int idReinsercionObtenido = await ObtenerIdReinsercionAsync(reinsercion.IdTabla, reinsercion.Tabla);

                if (idReinsercionObtenido == 0)
                    idReinsercionObtenido = await CrearIdReinsercionAsync(reinsercion);



                return Json(new { success = true, responseText = "Datos creado exitosamente! ", viewUrl = Url.Action("FichaCanalizacion", "Reinsercion", new { idReinsercion = idReinsercionObtenido }) });
            }
            return Json(new { success = false, responseText = "Error en la validación de datos" });
        }

            #region - ID Reinsercion -
            public async Task<int> ObtenerIdReinsercionAsync(string idTabla, string nombreTabla)
            {
                var idRegistroReinsercion = await _context.Reinsercion
                    .Where(c => c.IdTabla == idTabla && c.Tabla == nombreTabla)
                    .Select(c => c.IdReinsercion)
                    .FirstOrDefaultAsync();

                return idRegistroReinsercion;
            }

            [HttpPost]
            public async Task<int> CrearIdReinsercionAsync(Reinsercion reinsercion)
            {
                _context.Add(reinsercion);
                await _context.SaveChangesAsync();
                int nuevoIdReinsercion = await ObtenerIdReinsercionAsync(reinsercion.IdTabla, reinsercion.Tabla);

                return nuevoIdReinsercion;
            }
            #endregion

        #endregion


        #region -FichaCanalización-

        public IActionResult FichaCanalizacion(int idReinsercion)
        {
            var grupos = _context.Grupo.ToList();
            ViewBag.Grupos = grupos;

            var terapeutas = _context.Terapeutas.ToList();
            ViewBag.Terapeutas = terapeutas;

            ViewBag.idReinsercion = idReinsercion;
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CrearFichaCanalizacion([FromBody] DatosFichaCanalizacion datosFichaCanalizacion)
        {
            if (datosFichaCanalizacion == null || datosFichaCanalizacion.Datos == null)
                return Json(new { success = false, responseText = "Error en la validación de datos" });
            try
            {
                int idRegistroCanalizacion = await ObtenerIdCanalizacionAsync(datosFichaCanalizacion.IdReinsercion);

                if (idRegistroCanalizacion == 0)
                {
                    int idNuevoRegistroCanalizacion = await CrearRegistroCanalizacionAsync(datosFichaCanalizacion.IdReinsercion);

                    if (datosFichaCanalizacion.TipoCanalizacion.Equals("TERAPIA"))
                        await CrearTerapiaAsync(idNuevoRegistroCanalizacion, datosFichaCanalizacion.Datos);

                    else if (datosFichaCanalizacion.TipoCanalizacion.Equals("EJESREINSERCION"))
                        await CrearEjesReinsercionAsync(idNuevoRegistroCanalizacion, datosFichaCanalizacion.Datos);
                }
                else
                {
                    if (datosFichaCanalizacion.TipoCanalizacion.Equals("TERAPIA"))
                        await CrearTerapiaAsync(idRegistroCanalizacion, datosFichaCanalizacion.Datos);

                    else if (datosFichaCanalizacion.TipoCanalizacion.Equals("EJESREINSERCION"))
                        await CrearEjesReinsercionAsync(idRegistroCanalizacion, datosFichaCanalizacion.Datos);
                } 
                return Json(new { success = true, responseText = "Ficha creada exitosamente!", viewUrl = Url.Action("Index", "Personacls")});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = $"Error al crear la ficha: {ex.Message}" });
            }
        }

            #region - ID Canalizacion -
            public async Task<int> ObtenerIdCanalizacionAsync(int idReinsercion)
            {

                var idRegistroCanalizacion = await _context.Canalizacion
                    .Where(c => c.ReincercionIdReincercion == idReinsercion)
                    .Select(c => c.IdCanalizacion)
                    .FirstOrDefaultAsync();

                return idRegistroCanalizacion;
            }

            [HttpPost]
            public async Task<int> CrearRegistroCanalizacionAsync(int idReinsercion)
            {
                Canalizacion canalizacion = new Canalizacion();
                canalizacion.ReincercionIdReincercion = idReinsercion;

                _context.Add(canalizacion);
                await _context.SaveChangesAsync();

                return await ObtenerIdCanalizacionAsync(idReinsercion);

            }
             #endregion

            #region -Creacion de ficha por tipo- 

            [HttpPost]
            public async Task CrearTerapiaAsync(int idCanalizacion, object DatosTerapia)
            {
                using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var DatosTerapiaDeserializados = JsonConvert.DeserializeObject<TerapiaModel>(DatosTerapia.ToString());

                        foreach (var terapia in DatosTerapiaDeserializados.TiposTerapiaSeleccionados)
                        {
                            Terapia FichaTerapia = new Terapia();

                            if (terapia.Equals("OTRO"))
                                FichaTerapia.Tipo = mg.normaliza(DatosTerapiaDeserializados.EspecificarOtraTerapia);
                            else
                                FichaTerapia.Tipo = terapia;

                            FichaTerapia.Terapeuta = mg.normaliza(DatosTerapiaDeserializados.Terapeuta);
                            FichaTerapia.FechaCanalizacion = DateTime.Now;
                            FichaTerapia.FechaInicio = DatosTerapiaDeserializados.FechaInicio;
                            FichaTerapia.TiempoTerapia = mg.normaliza(DatosTerapiaDeserializados.TiempoTerapia);
                            FichaTerapia.FechaInicio = DatosTerapiaDeserializados.FechaInicio;
                            FichaTerapia.FechaTermino = DatosTerapiaDeserializados.FechaTermino;
                            FichaTerapia.PeriodicidadTerapia = mg.normaliza(DatosTerapiaDeserializados.PeriodicidadTerapia);
                            FichaTerapia.Estado = mg.normaliza(DatosTerapiaDeserializados.Estado);

                            if (DatosTerapiaDeserializados.Observaciones.Equals(""))
                                FichaTerapia.Observaciones = "NA";
                            else
                                FichaTerapia.Observaciones = mg.normaliza(DatosTerapiaDeserializados.Observaciones);

                            FichaTerapia.CanalizacionIdCanalizacion = idCanalizacion;
                            FichaTerapia.GrupoIdGrupo = DatosTerapiaDeserializados.GrupoId;

                            _context.Terapia.Add(FichaTerapia);
                            _context.SaveChanges();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error al crear los registros de EjesReinsercion", ex);
                    }
                }

            }

            [HttpPost]
            public async Task CrearEjesReinsercionAsync(int idCanalizacion, object DatosEjesReinsercion)
            {
                using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var DatosEjesDeserializados = JsonConvert.DeserializeObject<EjesReinsercionModel>(DatosEjesReinsercion.ToString());

                        foreach (var eje in DatosEjesDeserializados.EjesSeleccionados)
                        {
                            Ejesreinsercion ficha = new Ejesreinsercion();

                            if (eje.Equals("OTRO"))
                                ficha.Tipo = mg.normaliza(DatosEjesDeserializados.EspecificarOtroEje);
                            else
                                ficha.Tipo = eje;
                            ficha.FechaCanalizacion = DateTime.Now;
                            ficha.Lugar = mg.normaliza(DatosEjesDeserializados.Lugar);

                            if (DatosEjesDeserializados.Observaciones.Equals(""))
                                ficha.Observaciones = "NA";
                            else
                                ficha.Observaciones = mg.normaliza(DatosEjesDeserializados.Observaciones);

                            ficha.Estado = mg.normaliza(DatosEjesDeserializados.Estado);
                            ficha.CanalizacionIdCanalizacion = idCanalizacion;

                            _context.Ejesreinsercion.Add(ficha);
                            _context.SaveChanges();
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error al crear los registros de EjesReinsercion", ex);
                    }
                }
            }
        #endregion

        #endregion


        #region - Ver canalizaciones -

        public async Task<IActionResult> VerCanalizaciones()
        {

            return View();
        }



        #endregion

        // GET: Reinsercion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reinsercion = await _context.Reinsercion.SingleOrDefaultAsync(m => m.IdReinsercion == id);
            if (reinsercion == null)
            {
                return NotFound();
            }
            return View(reinsercion);
        }

        // POST: Reinsercion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdReinsercion,IdTabla,Tabla,Lugar,Estado")] Reinsercion reinsercion)
        {
            if (id != reinsercion.IdReinsercion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reinsercion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReinsercionExists(reinsercion.IdReinsercion))
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
            return View(reinsercion);
        }

        // GET: Reinsercion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reinsercion = await _context.Reinsercion
                .SingleOrDefaultAsync(m => m.IdReinsercion == id);
            if (reinsercion == null)
            {
                return NotFound();
            }

            return View(reinsercion);
        }

        // POST: Reinsercion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reinsercion = await _context.Reinsercion.SingleOrDefaultAsync(m => m.IdReinsercion == id);
            _context.Reinsercion.Remove(reinsercion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReinsercionExists(int id)
        {
            return _context.Reinsercion.Any(e => e.IdReinsercion == id);
        }
        public IActionResult MenuReinsercion()
        {
            return View();
        }
    }
}
