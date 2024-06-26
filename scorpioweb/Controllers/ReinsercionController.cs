﻿using System;
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

            var fechaActualizacionLimite = new DateTime(2023, 12, 30, 0, 0, 0);

            var query = (from persona in _context.Persona
                        //join domicilio in _context.Domicilio on persona.IdPersona equals domicilio.PersonaIdPersona
                        //join estudios in _context.Estudios on persona.IdPersona equals estudios.PersonaIdPersona
                        //join trabajo in _context.Trabajo on persona.IdPersona equals trabajo.PersonaIdPersona
                        //join actividadsocial in _context.Actividadsocial on persona.IdPersona equals actividadsocial.PersonaIdPersona
                        //join saludfisica in _context.Saludfisica on persona.IdPersona equals saludfisica.PersonaIdPersona
                        join supervision in _context.Supervision on persona.IdPersona equals supervision.PersonaIdPersona into s_join
                        from s in s_join.DefaultIfEmpty()
                        join causapenal in _context.Causapenal on s.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal into cp_join
                        from cp in cp_join.DefaultIfEmpty()
                        join delito in _context.Delito on cp.IdCausaPenal equals delito.CausaPenalIdCausaPenal into dd_join
                        from dd in dd_join.DefaultIfEmpty()
                        where (s.EstadoSupervision == null || s.EstadoSupervision != "CONCLUIDO") && persona.Supervisor.Contains("@dgepms.com") && persona.UltimaActualización >= fechaActualizacionLimite
                        group new ReinsercionMCYSCPLCCURSVM
                        {
                            IdTabla = persona.IdPersona.ToString(),
                            Nombre = persona.Paterno + " " + persona.Materno + " " + persona.Nombre,
                            Causapenal = cp.CausaPenal,
                            Delito = dd.Tipo,
                            NomTabla = "MCySCP",
                            //EstadoSupervision = s.EstadoSupervision,
                            //ClaveUnica = persona.ClaveUnicaScorpio,
                            Supervisor = persona.Supervisor
                        } by new { persona.IdPersona, persona.Paterno, persona.Materno, persona.Nombre } into g
                        select g.FirstOrDefault()
                    ).Union(from personacl in _context.Personacl
                            //join domiciliocl in _context.Domiciliocl on personacl.IdPersonaCl equals domiciliocl.PersonaclIdPersonacl
                            //join estudioscl in _context.Estudioscl on personacl.IdPersonaCl equals estudioscl.PersonaClIdPersonaCl
                            //join trabajocl in _context.Trabajocl on personacl.IdPersonaCl equals trabajocl.PersonaClIdPersonaCl
                            //join actividadsocialcl in _context.Actividadsocialcl on personacl.IdPersonaCl equals actividadsocialcl.PersonaClIdPersonaCl
                            //join saludfisicacl in _context.Saludfisicacl on personacl.IdPersonaCl equals saludfisicacl.PersonaClIdPersonaCl
                            join supervisioncl in _context.Supervisioncl on personacl.IdPersonaCl equals supervisioncl.PersonaclIdPersonacl into scl_join
                            from scl in scl_join.DefaultIfEmpty()
                            join causapenalcl in _context.Causapenalcl on scl.CausaPenalclIdCausaPenalcl equals causapenalcl.IdCausaPenalcl into cpcl_join
                            from cpcl in cpcl_join.DefaultIfEmpty()
                            join delitocl in _context.Delitocl on cpcl.IdCausaPenalcl equals delitocl.CausaPenalclIdCausaPenalcl into dcl_join
                            from dcl in dcl_join.DefaultIfEmpty()
                            join expedienteunico in _context.Expedienteunico on personacl.IdPersonaCl.ToString() equals expedienteunico.Persona into ex_join
                            from ex in ex_join.DefaultIfEmpty()
                            join ejecucion in _context.Ejecucion on ex.Ejecucion equals ejecucion.IdEjecucion.ToString() into ep_join
                            from ep in ep_join.DefaultIfEmpty()
                            join epcausapenal in _context.Epcausapenal on ep.IdEjecucion equals epcausapenal.EjecucionIdEjecucion into epcp_join
                            from epcp in epcp_join.DefaultIfEmpty()
                            where (scl.EstadoSupervision == null || scl.EstadoSupervision != "CONCLUIDO") && personacl.Supervisor.Contains("@dgepms.com")
                            group new ReinsercionMCYSCPLCCURSVM
                            {
                                IdTabla = personacl.IdPersonaCl.ToString(),
                                Nombre = personacl.Paterno + " " + personacl.Materno + " " + personacl.Nombre,
                                Causapenal = epcp.Causapenal ?? cpcl.CausaPenal,
                                Delito = epcp.Delito ?? dcl.Tipo,
                                NomTabla = "Libertad Condicionada",
                                //EstadoSupervision = scl.EstadoSupervision,
                                //ClaveUnica = personacl.ClaveUnicaScorpio,
                                Supervisor = personacl.Supervisor
                            } by new { personacl.IdPersonaCl, personacl.Paterno, personacl.Materno, personacl.Nombre } into g
                            select g.FirstOrDefault()
                    ).Where(t1 => !_context.Reinsercion.Any(r => (r.Tabla == "persona" && r.IdTabla == t1.IdTabla.ToString())))
                    .Select(t1 => new ReinsercionMCYSCPLCCURSVM
                    {
                        IdTabla = t1.IdTabla,
                        Nombre =  t1.Nombre,
                        Causapenal = t1.Causapenal,
                        Delito = t1.Delito,
                        NomTabla = t1.NomTabla,
                        //EstadoSupervision = t1.EstadoSupervision,
                        //ClaveUnica = t1.ClaveUnica,
                        Supervisor = t1.Supervisor
                    });

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

            List<Canalizacion> queryFracciones = (from f in _context.Canalizacion
                                                        group f by f.ReincercionIdReincercion into grp
                                                        select grp.OrderByDescending(f => f.IdCanalizacion).FirstOrDefault()).ToList();

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
                         join cana in queryFracciones on r.IdReinsercion equals cana.ReincercionIdReincercion
                         where r.Tabla == "personacl"
                         group new { r.IdReinsercion, r.IdTabla, p.IdPersonaCl, p.Paterno, p.Materno, p.Nombre, r.Estado, scl.EstadoSupervision, ep.Ce, cpcl.CausaPenal, dcl.Tipo, epcp.Delito } by new { r.IdReinsercion, r.IdTabla } into g
                         select new ReinsercionMCYSCPLCCURSVM
                         {
                             IdReinsercion = g.Key.IdReinsercion,
                             IdTabla = g.Key.IdTabla,
                             Nombre = string.Concat(g.FirstOrDefault().Paterno, " ", g.FirstOrDefault().Materno, " ", g.FirstOrDefault().Nombre),
                             Causapenal = g.FirstOrDefault().CausaPenal ?? g.FirstOrDefault().CausaPenal,
                             Delito = g.FirstOrDefault().Delito ?? g.FirstOrDefault().Tipo,
                             EstadoVinculacion = g.FirstOrDefault().Estado,
                             NomTabla = "Libertad Condicionada",
                             EstadoSupervision = g.FirstOrDefault().EstadoSupervision
                         }).Union(
                          from r in _context.Reinsercion
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
                          where r.Tabla == "persona"
                          group new { r.IdReinsercion, r.IdTabla, p.IdPersona, p.Paterno, p.Materno, p.Nombre, r.Estado, scl.EstadoSupervision, ep.Ce, cpcl.CausaPenal, dcl.Tipo, epcp.Delito } by new { r.IdReinsercion, r.IdTabla } into g
                          select new ReinsercionMCYSCPLCCURSVM
                          {
                              IdReinsercion = g.Key.IdReinsercion,
                              IdTabla = g.Key.IdTabla,
                              Nombre = string.Concat(g.FirstOrDefault().Paterno, " ", g.FirstOrDefault().Materno, " ", g.FirstOrDefault().Nombre),
                              Causapenal = g.FirstOrDefault().CausaPenal,
                              Delito = g.FirstOrDefault().Tipo,
                              EstadoVinculacion = g.FirstOrDefault().Estado,
                              NomTabla = "MCYSCP",
                              EstadoSupervision = g.FirstOrDefault().EstadoSupervision
                          });


           var result = query.ToList();

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

        #region -Menu Supervision-
        public async Task<IActionResult> Menusupervision(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            //var user = await userManager.FindByNameAsync(User.Identity.Name);
            //#region -Solicitud Atendida Archivo prestamo Digital-
            //var warningRespuesta = from a in _context.Archivoprestamodigital
            //                       where a.EstadoPrestamo == 1 && user.ToString().ToUpper() == a.Usuario.ToUpper()
            //                       select a;
            //ViewBag.WarningsUser = warningRespuesta.Count();
            //#endregion


            var Reinsercion = await _context.Reinsercion.SingleOrDefaultAsync(m => m.IdReinsercion == id);
            if (Reinsercion == null)
            {
                return NotFound();
            }


            List<Reinsercion> reinsercionVM = _context.Reinsercion.ToList();
            List<Canalizacion> canalizacionVM = _context.Canalizacion.ToList();
            List<Terapia> terapialVM = _context.Terapia.ToList();
            List<Ejesreinsercion> ejesreinsercionVM = _context.Ejesreinsercion.ToList();
            List<Oficioscanalizacion> oficioscanalizacionVM = _context.Oficioscanalizacion.ToList();

            List<Persona> personaVM = _context.Persona.ToList();
            List<Personacl> personacls = _context.Personacl.ToList();

            //List<Jornadas> JornadasVM = _context.Jornadas.ToList();

            #region -Jointables-
            var ES = from reinsercion in reinsercionVM
                                               
                                                where reinsercion.IdReinsercion == id
                                                select new 
                                                {
                                                    reinsercionVM = reinsercion
                                                };


            ViewData["reincercion"] = from reinsercion in reinsercionVM
                                      where reinsercion.IdReinsercion == id
                                      select new ReinsercionVM
                                      {
                                          reinsercionVM = reinsercion
                                      };

            var ESh = from reinsercion in reinsercionVM
                                                join canalizacion in canalizacionVM on reinsercion.IdReinsercion equals canalizacion.IdCanalizacion
                                                join terapia in terapialVM on reinsercion.IdReinsercion equals terapia.CanalizacionIdCanalizacion
                                                join ejesreincercion in ejesreinsercionVM on reinsercion.IdReinsercion equals ejesreincercion.CanalizacionIdCanalizacion
                                                join oficioscanalizacion in oficioscanalizacionVM on reinsercion.IdReinsercion equals oficioscanalizacion.CanalizacionIdCanalizacion
                                                where reinsercion.IdReinsercion == id
                                                select new ReinsercionVM
                                                {
                                                    reinsercionVM = reinsercion,
                                                    canalizacionVM = canalizacion,
                                                    terapiaVM = terapia,
                                                    ejesreinsercionVM = ejesreincercion,
                                                    oficioscanalizacionVM = oficioscanalizacion
                                                };
            #endregion

            return View();
        }
        #endregion




    }
}
