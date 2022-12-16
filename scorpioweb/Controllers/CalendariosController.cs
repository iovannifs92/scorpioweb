using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Newtonsoft.Json.Serialization;


namespace scorpioweb.Controllers
{
    [Authorize]
    public class CalendariosController : Controller
    {
        #region -Variables Globales-
        private readonly penas2Context _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        private List<SelectListItem> listaNoSi = new List<SelectListItem>
        {
            new SelectListItem{ Text="No", Value="NO"},
            new SelectListItem{ Text="Si", Value="SI"}
        };
        private List<SelectListItem> prioridad = new List<SelectListItem>

        {
            new SelectListItem{ Text="BAJA", Value="BAJA"},
            new SelectListItem{ Text="MEDIA", Value="MEDIA"},
            new SelectListItem{ Text="ALTA", Value="ALTA"}
        };
        private List<SelectListItem> frecuencia = new List<SelectListItem>
        {
            new SelectListItem{ Text="Diariamente", Value="DIARIAMENTE"},
            new SelectListItem{ Text="Semanalmente", Value="SEMANALMENTE"},
            new SelectListItem{ Text="Quincenalmente", Value="QUINCENALMENTE"},
            new SelectListItem{ Text="Mensualmente", Value="MENSUALMENTE"}
        };
        #endregion

        #region -Constructor-
        public CalendariosController(penas2Context context,
            RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
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

        public string colorPrioridad(string prioridad)
        {
            string color = "";
            switch (prioridad)
            {
                case "BAJA":
                    color = "#2CAD1E";
                    break;
                case "MEDIA":
                    color = "#E0CB26";
                    break;
                case "ALTA":
                    color = "#E00101";
                    break;
            }
            return color;
        }
        #endregion

        #region -getEventosMCySCP-
        public async Task<object> getEventosMCySCP()
        {
            #region -Variables de usuario-
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            bool flagCoordinador = false;
            string usuario = user.ToString();

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP")
                {
                    flagCoordinador = true;
                }
            }
            #endregion

            #region -Query para supervisiones-
            var supervisiones = from s in _context.Supervision
                                join p in _context.Persona on s.PersonaIdPersona equals p.IdPersona
                                join pl in _context.Planeacionestrategica on s.IdSupervision equals pl.SupervisionIdSupervision
                                where s.EstadoSupervision == "VIGENTE"
                                select new
                                {
                                    IdSupervision = s.IdSupervision,
                                    Nombre = p.Paterno + " " + p.Materno + " " + p.Nombre,
                                    FechaInforme = pl.FechaInforme,
                                    FechaProximoContacto = pl.FechaProximoContacto,
                                    Supervisor = p.Supervisor,
                                    EstadoSupervision = s.EstadoSupervision
                                };
            #endregion

            #region -Eventos del calendario-
            var calendario = from s in _context.Supervision
                             join t in _context.Calendario on s.IdSupervision equals t.SupervisionIdSupervision
                             join p in _context.Persona on s.PersonaIdPersona equals p.IdPersona
                             select new
                             {
                                 Idcalendario = t.Idcalendario,
                                 FechaEvento = t.FechaEvento,
                                 Mensaje = p.Paterno + " " + p.Materno + " " + p.Nombre + " --- " + t.Mensaje + (!flagCoordinador ? "" : (" --- " + p.Supervisor)),
                                 Color = colorPrioridad(t.Prioridad),
                                 IdSupervision = s.IdSupervision,
                                 Tipo = 1,
                                 Supervisor = p.Supervisor
                             };
            #endregion

            #region -Informes de supervisión-
            var informes = (from s in supervisiones
                            select new
                            {
                                Idcalendario = 0,
                                FechaEvento = s.FechaInforme,
                                Mensaje = s.Nombre + " --- INFORME SUPERVISIÓN" + (!flagCoordinador ? "" : (" --- " + s.Supervisor)),
                                Color = "#E00101",
                                IdSupervision = s.IdSupervision,
                                Tipo = 2,
                                Supervisor = s.Supervisor
                            });
            #endregion

            #region -Próximo contacto-
            var proximoContacto = (from s in supervisiones
                                   where s.FechaProximoContacto != null
                                   select new
                                   {
                                       Idcalendario = 0,
                                       FechaEvento = s.FechaProximoContacto,
                                       Mensaje = s.Nombre + " --- PRÓXIMO CONTACTO" + (!flagCoordinador ? "" : (" --- " + s.Supervisor)),
                                       Color = "#2CAD1E",
                                       IdSupervision = s.IdSupervision,
                                       Tipo = 3,
                                       Supervisor = s.Supervisor
                                   });
            #endregion

            #region -Presentaciones Periódicas-
            var presentaciones = from pp in _context.Presentacionperiodica
                                 join r in _context.Registrohuella on pp.RegistroidHuella equals r.IdregistroHuella
                                 join p in _context.Persona on r.PersonaIdPersona equals p.IdPersona
                                 where pp.FechaFirma > DateTime.Now.AddDays(-7)
                                 select new
                                 {
                                     Idcalendario = 0,
                                     FechaEvento = pp.FechaFirma,
                                     Mensaje = p.Paterno + " " + p.Materno + " " + p.Nombre + " --- PRESENTACIÓN PERIÓDICA",
                                     Color = "#E00101",
                                     IdSupervision = 0,
                                     Tipo = 4,
                                     Supervisor = p.Supervisor
                                 };
            #endregion

            if (!flagCoordinador)
            {
                calendario = calendario.Where(p => p.Supervisor == usuario);
                informes = informes.Where(p => p.Supervisor == usuario);
                proximoContacto = proximoContacto.Where(p => p.Supervisor == usuario);
            }

            var tasks = calendario.Union(informes).Union(proximoContacto).Union(presentaciones);
            return tasks;
        }
        #endregion

        #region -getEventosOficialia-
        public async Task<object> getEventosOficialia()
        {
            #region -Variables de usuario-
            /*var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            bool flagCoordinador = false;
            string usuario = user.ToString();

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP")
                {
                    flagCoordinador = true;
                }
            }*/
            #endregion

            #region -Query para terminos-
            var terminos = from o in _context.Oficialia
                                where o.TieneTermino =="SI"
                                select new
                                {
                                    Idcalendario = o.IdOficialia,
                                    FechaEvento = o.FechaTermino,
                                    Mensaje = o.PaternoMaternoNombre + " ---- " +o.AsuntoOficio,
                                    Color = "#E00101",
                                    IdSupervision = 0,
                                    Tipo = 1,
                                    Supervisor = o.Recibe
                                };
            #endregion
            return terminos;
        }
        #endregion

        #region -Initialize events-
        public async Task<IActionResult> getCalendarTasks(int origen) /*Origen: 1=MCySPC, 2=Oficialia*/
        {   
            var tasks = await(origen == 1 ? getEventosMCySCP() : getEventosOficialia());

            return Json(tasks, new Newtonsoft.Json.JsonSerializerSettings());
        }
        #endregion

        #region -Index-
        public async Task<IActionResult> Index(int origen) /*Origen: 1=MCySPC, 2=Oficialia*/
        {
            ViewBag.origen = origen;
            return View(await _context.Calendario.ToListAsync());
        } 
        #endregion

        #region -Create-
        public IActionResult Create(int id)
        {
            ViewBag.idSupervision = id;
            ViewBag.prioridad = prioridad;
            ViewBag.repite = listaNoSi;
            ViewBag.frecuencia = frecuencia;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mensaje,Repite,Frecuencia,Repeticiones,FechaEvento,Prioridad,Tipo,SupervisionIdSupervision")] Calendario calendario, int Repeticiones)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            string usuario = user.ToString();

            if (ModelState.IsValid)
            {
                calendario.Usuario = usuario;
                calendario.FechaCreacion = DateTime.Now;
                calendario.Mensaje = normaliza(calendario.Mensaje);
                calendario.Tipo = normaliza(calendario.Tipo);
                if(calendario.Repite == "SI")
                {
                    for (int i = 0; i < Repeticiones; i++)
                    {
                        _context.Add(calendario);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                        switch (calendario.Frecuencia)
                        {
                            case "DIARIAMENTE":
                                calendario.FechaEvento = ((DateTime)calendario.FechaEvento).AddDays(1);
                                break;
                            case "SEMANALMENTE":
                                calendario.FechaEvento = ((DateTime)calendario.FechaEvento).AddDays(7);
                                break;
                            case "QUINCENALMENTE":
                                calendario.FechaEvento = ((DateTime)calendario.FechaEvento).AddDays(15);
                                break;
                            case "MENSUALMENTE":
                                calendario.FechaEvento = ((DateTime)calendario.FechaEvento).AddMonths(1);
                                break;
                        }
                    }
                }
                else 
                {
                    _context.Add(calendario);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index), new { origen = 1});
            }
            return View(calendario);
        } 
        #endregion

        #region -Edit-
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calendario = await _context.Calendario.SingleOrDefaultAsync(m => m.Idcalendario == id);
            if (calendario == null)
            {
                return NotFound();
            }

            var nombre = (from s in _context.Supervision
                         join p in _context.Persona on s.PersonaIdPersona equals p.IdPersona
                         join cp in _context.Causapenal on s.CausaPenalIdCausaPenal equals cp.IdCausaPenal
                         where s.IdSupervision == calendario.SupervisionIdSupervision
                         select new
                         {
                             nombreCompleto = p.Paterno + " " + p.Materno + " " + p.Nombre,
                             causaPenal = cp.CausaPenal
                         }).Single();

            ViewBag.datosGenerales = nombre.nombreCompleto + " " + nombre.causaPenal;
            ViewBag.supervision = calendario.SupervisionIdSupervision;

            ViewBag.listaPrioridad = prioridad;
            ViewBag.idPrioridad = BuscaId(prioridad, calendario.Prioridad);

            ViewBag.idCalendario = id;

            return View(calendario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Idcalendario,Mensaje,FechaEvento,Prioridad,Usuario,Tipo,FechaCreacion,SupervisionIdSupervision")] Calendario calendario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    calendario.Mensaje = normaliza(calendario.Mensaje);
                    calendario.Tipo = normaliza(calendario.Tipo);
                    var oldCalendario = await _context.Calendario.FindAsync(calendario.Idcalendario);

                    _context.Entry(oldCalendario).CurrentValues.SetValues(calendario);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalendarioExists(calendario.Idcalendario))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { origen = 1 });
            }
            return View(calendario);
        }
        #endregion

        #region -EditOficialia-
        public async Task<IActionResult> EditOficialia(int? id)
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

            return View(oficialia);

        }
        #endregion

        #region -Delete-
        public async Task<IActionResult> EventDelete(int? id)
        {
            var calendario = await _context.Calendario.SingleOrDefaultAsync(m => m.Idcalendario == id);
            _context.Calendario.Remove(calendario);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
            return RedirectToAction(nameof(Index));
        }

        private bool CalendarioExists(int id)
        {
            return _context.Calendario.Any(e => e.Idcalendario == id);
        } 
        #endregion
    }
}
