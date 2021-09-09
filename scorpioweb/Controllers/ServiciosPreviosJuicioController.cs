using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;
using Microsoft.AspNetCore.Http;
using SautinSoft.Document;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace scorpioweb.Controllers
{
    [Authorize]
    public class ServiciosPreviosJuicioController : Controller
    {
        #region -Constructor-
        private readonly penas2Context _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public ServiciosPreviosJuicioController(penas2Context context, IHostingEnvironment hostingEnvironment,
                        RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        #endregion

        #region -Listas-
        private List<SelectListItem> listaNaSiNo = new List<SelectListItem>

        {
            new SelectListItem{ Text="NA", Value="NA"},
            new SelectListItem{ Text="Si", Value="SI"},
            new SelectListItem{ Text="No", Value="NO"}
        };
        
        private List<SelectListItem> listaSexo = new List<SelectListItem>
        {
            new SelectListItem{ Text="Masculino", Value="M"},
            new SelectListItem{ Text="Femenino", Value="F"}
        };

        private List<SelectListItem> listaDomicilio = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Verdadero", Value = "VERDADERO" },
            new SelectListItem{ Text = "Falso", Value = "FALSO" }
        };

        private List<SelectListItem> listaUbicacion = new List<SelectListItem>
        {
            new SelectListItem{ Text = "C. Operativa", Value = "C. OPERATIVA" },
            new SelectListItem{ Text = "Archivo", Value = "ARCHIVO" },
            new SelectListItem{ Text = "C. Ejecución de Penas", Value = "C. EJECUCION PENAS" },
            new SelectListItem{ Text = "Adolescentes", Value = "ADOLESCENTES" }
        };

        private List<SelectListItem> ListaUnidadInvestigación = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Centro de Operaciones Estratégicas", Value = "CENTRO DE OPERACIONES ESTRATEGICAS" },
            new SelectListItem{ Text = "M. P. Fóraneos", Value = "M. P. FORANEOS" },
            new SelectListItem{ Text = "P. G. R.", Value = "P. G. R." },
            new SelectListItem{ Text = "P. G. R. Gómez Palacio , Dgo.", Value = "P. G. R. GOMEZ PALACIO, DGO" },
            new SelectListItem{ Text = "Fiscalía", Value = "FISCALIA" },
        };

        private List<SelectListItem> listaSituacionJuridica = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Detenido", Value = "DETENIDO" },
            new SelectListItem{ Text = "Imposibilidad de Arraigo", Value = "IMPOSIBILIDAD DE ARRAIGO" }
        };

        private List<SelectListItem> listaRealizoEntrevista = new List<SelectListItem>
        {
            new SelectListItem{ Text="NA", Value="NA"},
            new SelectListItem{ Text = "Si", Value = "SI" },
            new SelectListItem{ Text = "Negativa", Value = "NEGATIVA" }
        };

        private List<SelectListItem> listaTipoDetenido = new List<SelectListItem>
        {
            new SelectListItem{ Text = "NA", Value = "NA" },
            new SelectListItem{ Text = "Adulto", Value = "ADULTO" },
            new SelectListItem{ Text = "Adolescente", Value = "ADOLESCENTE" }
        };

        private List<SelectListItem> listaAER = new List<SelectListItem>
        {
            new SelectListItem{ Text = "NA", Value = "NA" },
            new SelectListItem{ Text = "Alto", Value = "ALTO" },
            new SelectListItem{ Text = "Medio", Value = "MEDIO" },
            new SelectListItem{ Text = "Bajo", Value = "BAJO" }
        };

        private List<SelectListItem> listaRiesgo = new List<SelectListItem>
        {
            new SelectListItem{ Text = "NA", Value = "NA" },
            new SelectListItem{ Text = "Alto", Value = "ALTO" },
            new SelectListItem{ Text = "Bajo", Value = "BAJO" }
        };

        private List<SelectListItem> listaRecomendación = new List<SelectListItem>
        {
            new SelectListItem{ Text = "NA", Value = "NA" },
            new SelectListItem{ Text = "Medida en Libertad sin Condiciones", Value = "MEDIDA EN LIBERTAD SIN CONDICIONES" },
            new SelectListItem{ Text = "Medida Cautelar en Libertad", Value = "MEDIDA CAUTELAR EN LIBERTAD" },
            new SelectListItem{ Text = "Internamiento Preventivo", Value = "INTERNAMIENTO PREVENTIVO" }
        };

        #endregion

        #region -Metodos Generales-
        public string normaliza(string normalizar)
        {
            if (!String.IsNullOrEmpty(normalizar))
            {
                normalizar = normalizar.ToUpper();
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
        #endregion


        #region -Index-
        public async Task<IActionResult> Index(
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

            var personas = from p in _context.Serviciospreviosjuicio
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                foreach (var item in searchString.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(searchString) ||
                                                   (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(searchString));

                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.Paterno);
                    break;
                default:
                    personas = personas.OrderBy(p => p.Paterno);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Serviciospreviosjuicio>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        #endregion

        #region -Details-
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviciospreviosjuicio = await _context.Serviciospreviosjuicio
                .SingleOrDefaultAsync(m => m.IdserviciosPreviosJuicio == id);
            if (serviciospreviosjuicio == null)
            {
                return NotFound();
            }

            return View(serviciospreviosjuicio);
        }
        #endregion

        #region -Create-
        public IActionResult Create()
        {
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();
            #region -Listas-
            ViewBag.listaSexo = listaSexo;

            ViewBag.listaDomicilio = listaDomicilio;

            ViewBag.listaUbicacion = listaUbicacion;

            ViewBag.listaUnidadI = ListaUnidadInvestigación;

            ViewBag.listaSituacionJuridica = listaSituacionJuridica;

            ViewBag.listaRealizoE = listaRealizoEntrevista;

            ViewBag.listaTipoDetenido = listaTipoDetenido;

            ViewBag.listaAER = listaAER;

            ViewBag.listaTamizaje = listaNaSiNo;

            ViewBag.listaComparesencia = listaRiesgo;

            ViewBag.listaVictima = listaRiesgo;

            ViewBag.listaObstaculizar = listaRiesgo;

            ViewBag.listaRecomendacion = listaRecomendación;

            ViewBag.listaAntecedentes = listaNaSiNo;
            #endregion

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile evidencia, [Bind("Nombre,Paterno,Materno,Sexo,Edad,Calle,Colonia,Domicilio,Telefono,Papa,Mama,Ubicacion,Delito,UnidadInvestigacion,FechaDetencion,Situacion,RealizoEntrevista,TipoDetenido,Aer,Tamizaje,Rcomparesencia,Rvictima,Robstaculizacion,Recomendacion,Antecedentes,AntecedentesDatos,Observaciones")] Serviciospreviosjuicio serviciospreviosjuicio)
        {
            int idAER = 0;
            serviciospreviosjuicio.Nombre = normaliza(serviciospreviosjuicio.Nombre);
            serviciospreviosjuicio.Paterno = normaliza(serviciospreviosjuicio.Paterno);
            serviciospreviosjuicio.Materno = normaliza(serviciospreviosjuicio.Materno);
            serviciospreviosjuicio.Calle = normaliza(serviciospreviosjuicio.Calle);
            serviciospreviosjuicio.Colonia = normaliza(serviciospreviosjuicio.Colonia);
            serviciospreviosjuicio.Papa = normaliza(serviciospreviosjuicio.Papa);
            serviciospreviosjuicio.Mama = normaliza(serviciospreviosjuicio.Mama);
            serviciospreviosjuicio.AntecedentesDatos = normaliza(serviciospreviosjuicio.AntecedentesDatos);
            serviciospreviosjuicio.Observaciones = normaliza(serviciospreviosjuicio.Observaciones);

            int cont = (from table in _context.Serviciospreviosjuicio
                        select table.IdserviciosPreviosJuicio).Count();
            if (cont != 0)
            {
                idAER = ((from table in _context.Serviciospreviosjuicio
                              select table.IdserviciosPreviosJuicio).Max()) + 1;
            }
            else
            {
                idAER = 1;
            }
            

            serviciospreviosjuicio.IdserviciosPreviosJuicio = idAER;

            #region -Guardar archivo-
            if (evidencia != null)
            {
                string file_name = idAER +"_"+ serviciospreviosjuicio.Paterno + "_" + serviciospreviosjuicio.Materno + "_" + serviciospreviosjuicio.Nombre + Path.GetExtension(evidencia.FileName);
                serviciospreviosjuicio.RutaAer = file_name;
                var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "AER");
                var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                await evidencia.CopyToAsync(stream);
                stream.Close();
            }
            #endregion

            _context.Add(serviciospreviosjuicio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region -Edit-
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();
            var persona = await _context.Serviciospreviosjuicio.SingleOrDefaultAsync(m => m.IdserviciosPreviosJuicio == id);

            #region -Listas-
            ViewBag.listaSexo = listaSexo;
            ViewBag.idGenero = BuscaId(listaSexo, persona.Sexo);

            ViewBag.listaDomicilio = listaDomicilio;
            ViewBag.idDomicilio = BuscaId(listaDomicilio, persona.Domicilio);

            ViewBag.listaUbicacion = listaUbicacion;
            ViewBag.idUbicacion = BuscaId(listaUbicacion, persona.Ubicacion);

            ViewBag.listaUnidadI = ListaUnidadInvestigación;
            ViewBag.idUnidad= BuscaId(ListaUnidadInvestigación, persona.UnidadInvestigacion);

            ViewBag.listaSituacionJuridica = listaSituacionJuridica;
            ViewBag.idSituacionJuridica = BuscaId(listaSituacionJuridica, persona.Situacion);

            ViewBag.listaRealizoE = listaRealizoEntrevista;
            ViewBag.idRealizoE = BuscaId(listaRealizoEntrevista, persona.RealizoEntrevista);

            ViewBag.listaTipoDetenido = listaTipoDetenido;
            ViewBag.idlTipoDetenido = BuscaId(listaTipoDetenido, persona.TipoDetenido);

            ViewBag.listaAER = listaAER;
            ViewBag.idAER = BuscaId(listaAER, persona.Aer);

            ViewBag.listaTamizaje = listaNaSiNo;
            ViewBag.idTamizaje = BuscaId(listaNaSiNo, persona.Tamizaje);

            ViewBag.listaComparesencia = listaRiesgo;
            ViewBag.idComparesencia = BuscaId(listaRiesgo, persona.Rcomparesencia);

            ViewBag.listaVictima = listaRiesgo;
            ViewBag.idVictima = BuscaId(listaRiesgo, persona.Rvictima);

            ViewBag.listaObstaculizar = listaRiesgo;
            ViewBag.idObstaculizar = BuscaId(listaRiesgo, persona.Robstaculizacion);

            ViewBag.listaRecomendacion = listaRecomendación;
            ViewBag.idRecomendacion = BuscaId(listaRecomendación, persona.Recomendacion);

            ViewBag.listaAntecedentes = listaNaSiNo;
            ViewBag.idAntecedentes = BuscaId(listaNaSiNo, persona.Antecedentes);
            #endregion

            ViewBag.TipoDetenido = persona.TipoDetenido;
            ViewBag.RealizoEntrevista = persona.RealizoEntrevista;
            ViewBag.TieneAntecedentes = persona.Antecedentes;

            var serviciospreviosjuicio = await _context.Serviciospreviosjuicio.SingleOrDefaultAsync(m => m.IdserviciosPreviosJuicio == id);
            if (serviciospreviosjuicio == null)
            {
                return NotFound();
            }
            return View(serviciospreviosjuicio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile evidencia, [Bind("IdserviciosPreviosJuicio,Nombre,Paterno,Materno,Sexo,Edad,Calle,Colonia,Domicilio,Telefono,Papa,Mama,Ubicacion,Delito,UnidadInvestigacion,FechaDetencion,Situacion,RealizoEntrevista,TipoDetenido,Aer,Tamizaje,Rcomparesencia,Rvictima,Robstaculizacion,Recomendacion,Antecedentes,AntecedentesDatos,Observaciones,PersonaIdPersona")] Serviciospreviosjuicio serviciospreviosjuicio)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    serviciospreviosjuicio.Nombre = normaliza(serviciospreviosjuicio.Nombre);
                    serviciospreviosjuicio.Paterno = normaliza(serviciospreviosjuicio.Paterno);
                    serviciospreviosjuicio.Materno = normaliza(serviciospreviosjuicio.Materno);
                    serviciospreviosjuicio.Calle = normaliza(serviciospreviosjuicio.Calle);
                    serviciospreviosjuicio.Colonia = normaliza(serviciospreviosjuicio.Colonia);
                    serviciospreviosjuicio.Papa = normaliza(serviciospreviosjuicio.Papa);
                    serviciospreviosjuicio.Mama = normaliza(serviciospreviosjuicio.Mama);
                    serviciospreviosjuicio.AntecedentesDatos = normaliza(serviciospreviosjuicio.AntecedentesDatos);
                    serviciospreviosjuicio.Observaciones = normaliza(serviciospreviosjuicio.Observaciones);
                    var oldServiciospreviosjuicio = await _context.Serviciospreviosjuicio.FindAsync(serviciospreviosjuicio.IdserviciosPreviosJuicio);
                    #region -EditarArchivo-
                    if (evidencia == null)
                    {
                        serviciospreviosjuicio.RutaAer = oldServiciospreviosjuicio.RutaAer;
                    }
                    else
                    {
                        string file_name = serviciospreviosjuicio.IdserviciosPreviosJuicio + "_" + serviciospreviosjuicio.Paterno + "_" + serviciospreviosjuicio.Materno + "_" + serviciospreviosjuicio.Nombre + Path.GetExtension(evidencia.FileName);
                        serviciospreviosjuicio.RutaAer = file_name;
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "AER");

                        if (System.IO.File.Exists(Path.Combine(uploads, file_name)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, file_name));
                        }

                        var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                        await evidencia.CopyToAsync(stream);
                        stream.Close();
                    }
                    #endregion


                    _context.Entry(oldServiciospreviosjuicio).CurrentValues.SetValues(serviciospreviosjuicio);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                    //_context.Update(serviciospreviosjuicio);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiciospreviosjuicioExists(serviciospreviosjuicio.IdserviciosPreviosJuicio))
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
            return View(serviciospreviosjuicio);
        }
        #endregion

        #region -Delete-
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviciospreviosjuicio = await _context.Serviciospreviosjuicio
                .SingleOrDefaultAsync(m => m.IdserviciosPreviosJuicio == id);
            if (serviciospreviosjuicio == null)
            {
                return NotFound();
            }

            return View(serviciospreviosjuicio);
        }

        // POST: ServiciosPreviosJuicio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviciospreviosjuicio = await _context.Serviciospreviosjuicio.SingleOrDefaultAsync(m => m.IdserviciosPreviosJuicio == id);
            _context.Serviciospreviosjuicio.Remove(serviciospreviosjuicio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region -ServiciospreviosjuicioExists-
        private bool ServiciospreviosjuicioExists(int id)
        {
            return _context.Serviciospreviosjuicio.Any(e => e.IdserviciosPreviosJuicio == id);
        }
        #endregion

    }
}
