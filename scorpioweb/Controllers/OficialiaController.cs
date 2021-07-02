using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using scorpioweb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace scorpioweb.Controllers
{
    public class OficialiaController : Controller
    {
        #region -Variables Globales-
        private readonly penas2Context _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public static List<List<string>> datosDelitos = new List<List<string>>();
        #endregion

        #region -Constructor-
        public OficialiaController(penas2Context context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        #endregion

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
            string currentUser = User.Identity.Name;
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
                                    select table.IdOficialia).Count()) + 1;
                }
                oficialia.IdOficialia = idOficialia;

                oficialia.Capturista = capturista;
                oficialia.Recibe = recibe;
                oficialia.MetodoNotificacion = metodoNotificacion;
                oficialia.NumOficio = numOficio;
                oficialia.Expide = expide;
                oficialia.ReferenteImputado = referenteImputado;
                oficialia.Sexo = sexo;
                oficialia.Paterno = paterno;
                oficialia.Materno = materno;
                oficialia.Nombre = nombre;
                oficialia.CarpetaEjecucion = carpetaEjecucion;
                oficialia.ExisteVictima = existeVictima;
                oficialia.NombreVictima = nombreVictima;
                oficialia.DireccionVictima = direccionVictima;
                oficialia.AsuntoOficio = asuntoOficio;
                oficialia.TieneTermino = tieneTermino;
                oficialia.UsuarioTurnar = usuarioTurnar;
                oficialia.Observaciones = observaciones;
                oficialia.IdCausaPenal = idCausaPenal;
                oficialia.Entregado = Entregado;
                oficialia.FechaRecepcion = fechaRecepcion;
                oficialia.FechaEmision = fechaEmision;
                oficialia.FechaTermino = fechaTermino;
                _context.Add(oficialia);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                return RedirectToAction(nameof(Index));
            }
            return View(oficialia);
        }
    }
}