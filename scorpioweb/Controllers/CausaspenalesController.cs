using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;

namespace scorpioweb.Controllers
{
    public class CausaspenalesController : Controller
    {
        private readonly penas2Context _context;

        public CausaspenalesController(penas2Context context)
        {
            _context = context;
        }

        // GET: Causaspenales
        public async Task<IActionResult> Index()
        {
            return View(await _context.Causapenal.ToListAsync());
        }

        // GET: Causaspenales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var causapenal = await _context.Causapenal
                .SingleOrDefaultAsync(m => m.IdCausaPenal == id);
            if (causapenal == null)
            {
                return NotFound();
            }

            return View(causapenal);
        }

        // GET: Causaspenales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Causaspenales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Causapenal causapenal, string cnpp, string juez, string cambio, string cp)
        {
            if (ModelState.IsValid)
            {
                causapenal.Cnpp = cnpp;
                causapenal.Juez = juez;
                causapenal.Cambio = cambio;
                causapenal.CausaPenal = cp;
                _context.Add(causapenal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(causapenal);
        }

        // GET: Causaspenales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var causapenal = await _context.Causapenal.SingleOrDefaultAsync(m => m.IdCausaPenal == id);
            if (causapenal == null)
            {
                return NotFound();
            }
            return View(causapenal);
        }

        // POST: Causaspenales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCausaPenal,Cnpp,Juez,Cambio,CausaPenal")] Causapenal causa)
        {
            if (id != causa.IdCausaPenal)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(causa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CausapenalExists(causa.IdCausaPenal))
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
            return View(causa);
        }

        // GET: Causaspenales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var causapenal = await _context.Causapenal
                .SingleOrDefaultAsync(m => m.IdCausaPenal == id);
            if (causapenal == null)
            {
                return NotFound();
            }

            return View(causapenal);
        }

        // POST: Causaspenales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var causapenal = await _context.Causapenal.SingleOrDefaultAsync(m => m.IdCausaPenal == id);
            _context.Causapenal.Remove(causapenal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //GET: CausasPenales/AsignacionCP
        public async Task<IActionResult> AsignacionCP(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";


            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            ViewData["CurrentFilter"] = searchString;

            var personas = from p in _context.Persona
                           where p.Supervisor == User.Identity.Name
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                personas = personas.Where(p => p.Paterno.Contains(searchString)
                                        || p.Materno.Contains(searchString)
                                        || p.Nombre.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.Paterno);
                    break;
                case "Date":
                    personas = personas.OrderBy(p => p.UltimaActualización);
                    break;
                case "date_desc":
                    personas = personas.OrderByDescending(p => p.UltimaActualización);
                    break;
                default:
                    personas = personas.OrderBy(p => p.Paterno);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        #region -Detalles CP-

        // GET: Personas/Details/5
        public async Task<IActionResult> DetailsCP(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var persona = await _context.Persona
            //    .SingleOrDefaultAsync(m => m.IdPersona == id);

            //List<Causapenal> causaPenal = _context.Causapenal.ToList();

            //ViewData["joinTables"] = from personaTable in causaPenal
            //                         where personaTable.IdPersona == id
            //                         select new PersonaViewModel
            //                         {
            //                             causaPenal = personaTable
            //                         };

            //if (persona == null)
            //{
            //    return NotFound();
            //}

            //return View();

            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona
                .SingleOrDefaultAsync(m => m.IdPersona == id);

            #region -To List databases-

            List<Persona> personaVM = _context.Persona.ToList();
            List<Domicilio> domicilioVM = _context.Domicilio.ToList();
            List<Estudios> estudiosVM = _context.Estudios.ToList();
            List<Estados> estados = _context.Estados.ToList();
            List<Municipios> municipios = _context.Municipios.ToList();
            List<Domiciliosecundario> domicilioSecundarioVM = _context.Domiciliosecundario.ToList();
            List<Consumosustancias> consumoSustanciasVM = _context.Consumosustancias.ToList();
            List<Trabajo> trabajoVM = _context.Trabajo.ToList();
            List<Actividadsocial> actividadSocialVM = _context.Actividadsocial.ToList();
            List<Abandonoestado> abandonoEstadoVM = _context.Abandonoestado.ToList();
            List<Saludfisica> saludFisicaVM = _context.Saludfisica.ToList();
            List<Familiaresforaneos> familiaresForaneosVM = _context.Familiaresforaneos.ToList();
            List<Asientofamiliar> asientoFamiliarVM = _context.Asientofamiliar.ToList();
            List<Personacausapenal> personaCausaPenalVM = _context.Personacausapenal.ToList();
            List<Causapenal> causaPenalVM = _context.Causapenal.ToList();

            #endregion

            #region -Jointables-
            ViewData["joinTables"] = from personaTable in personaVM
                                     join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                     join estudios in estudiosVM on persona.IdPersona equals estudios.PersonaIdPersona
                                     join trabajo in trabajoVM on persona.IdPersona equals trabajo.PersonaIdPersona
                                     join actividaSocial in actividadSocialVM on persona.IdPersona equals actividaSocial.PersonaIdPersona
                                     join abandonoEstado in abandonoEstadoVM on persona.IdPersona equals abandonoEstado.PersonaIdPersona
                                     join saludFisica in saludFisicaVM on persona.IdPersona equals saludFisica.PersonaIdPersona
                                     join personaCausaPenal in personaCausaPenalVM on persona.IdPersona equals personaCausaPenal.PersonaIdPersona
                                     join causaPenal in causaPenalVM on personaCausaPenal.CausaPenalIdCausaPenal equals causaPenal.IdCausaPenal
                                     //join nacimientoEstado in estados on (Int32.Parse(persona.Lnestado)) equals nacimientoEstado.Id
                                     //join nacimientoMunicipio in municipios on (Int32.Parse(persona.Lnmunicipio)) equals nacimientoMunicipio.Id
                                     //join domicilioEstado in estados on (Int32.Parse(domicilio.Estado)) equals domicilioEstado.Id
                                     //join domicilioMunicipio in municipios on (Int32.Parse(domicilio.Municipio)) equals domicilioMunicipio.Id
                                     where personaTable.IdPersona == id
                                     select new PersonaCausaPenalViewModel
                                     {
                                         personaVM = personaTable,
                                         /*domicilioVM = domicilio,
                                         estudiosVM = estudios,
                                         trabajoVM = trabajo,
                                         actividadSocialVM = actividaSocial,
                                         abandonoEstadoVM = abandonoEstado,
                                         saludFisicaVM = saludFisica,*/
                                         causaPenalVM = causaPenal
                                         //estadosVMPersona=nacimientoEstado,
                                         //municipiosVMPersona=nacimientoMunicipio,
                                         //estadosVMDomicilio = domicilioEstado,
                                         //municipiosVMDomicilio= domicilioMunicipio,
                                     };

            #endregion

            //#region -JoinTables null-
            //ViewData["joinTablesDomSec"] = from personaTable in personaVM
            //                               join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
            //                               join domicilioSec in domicilioSecundarioVM.DefaultIfEmpty() on domicilio.IdDomicilio equals domicilioSec.IdDomicilio
            //                               where personaTable.IdPersona == id
            //                               select new PersonaViewModel
            //                               {
            //                                   domicilioSecundarioVM = domicilioSec
            //                               };

            //ViewData["joinTablesConsumoSustancias"] = from personaTable in personaVM
            //                                          join sustancias in consumoSustanciasVM on persona.IdPersona equals sustancias.PersonaIdPersona
            //                                          where personaTable.IdPersona == id
            //                                          select new PersonaViewModel
            //                                          {
            //                                              consumoSustanciasVM = sustancias
            //                                          };

            //ViewData["joinTablesFamiliaresForaneos"] = from personaTable in personaVM
            //                                           join familiarForaneo in familiaresForaneosVM on persona.IdPersona equals familiarForaneo.PersonaIdPersona
            //                                           where personaTable.IdPersona == id
            //                                           select new PersonaViewModel
            //                                           {
            //                                               familiaresForaneosVM = familiarForaneo
            //                                           };

            //ViewData["joinTablesFamiliares"] = from personaTable in personaVM
            //                                   join familiar in asientoFamiliarVM on persona.IdPersona equals familiar.PersonaIdPersona
            //                                   where personaTable.IdPersona == id && familiar.Tipo == "FAMILIAR"
            //                                   select new PersonaViewModel
            //                                   {
            //                                       asientoFamiliarVM = familiar
            //                                   };

            //ViewData["joinTablesReferencia"] = from personaTable in personaVM
            //                                   join referencia in asientoFamiliarVM on persona.IdPersona equals referencia.PersonaIdPersona
            //                                   where personaTable.IdPersona == id && referencia.Tipo == "REFERENCIA"
            //                                   select new PersonaViewModel
            //                                   {
            //                                       asientoFamiliarVM = referencia
            //                                   };


            //ViewBag.Referencia = ((ViewData["joinTablesReferencia"] as IEnumerable<scorpioweb.Models.PersonaViewModel>).Count()).ToString();

            //ViewBag.Familiar = ((ViewData["joinTablesFamiliares"] as IEnumerable<scorpioweb.Models.PersonaViewModel>).Count()).ToString();
            //#endregion


            if (persona == null)
            {
                return NotFound();
            }

            return View();

            //return View(await _context.Persona.ToListAsync());
        }
        #endregion

        // GET: Causaspenales
        public IActionResult ControlCP()
        {
            return View();
        }

        private bool CausapenalExists(int id)
        {
            return _context.Causapenal.Any(e => e.IdCausaPenal == id);
        }
    }
}
