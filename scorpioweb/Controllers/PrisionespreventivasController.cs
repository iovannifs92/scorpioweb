using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;
using System.Security.Claims;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using scorpioweb.Class;
using Newtonsoft.Json;

namespace scorpioweb.Controllers
{
    public class PrisionespreventivasController : Controller
    {
        private readonly penas2Context _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        #region -Metodos Generales-
        MetodosGenerales mg = new MetodosGenerales();
        #endregion

        public PrisionespreventivasController(penas2Context context, IHostingEnvironment hostingEnvironment,
                        RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        private List<SelectListItem> listaSexo = new List<SelectListItem>
        {
            new SelectListItem{ Text="Masculino", Value="M"},
            new SelectListItem{ Text="Femenino", Value="F"}
        };

        // GET: Prisionespreventivas
        public async Task<IActionResult> Index(
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var personas = from p in _context.Prisionespreventivas
                           select p;
            personas = personas.OrderByDescending(p => p.Idprisionespreventivas);

            if (!String.IsNullOrEmpty(searchString))
            {
                foreach (var item in searchString.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (mg.replaceSlashes(p.Paterno) + " " + mg.replaceSlashes(p.Materno) + " " + mg.replaceSlashes(p.Nombre)).Contains(mg.normaliza(searchString)) ||
                                                   (mg.replaceSlashes(p.Nombre) + " " + mg.replaceSlashes(p.Paterno) + " " + mg.replaceSlashes(p.Materno)).Contains(mg.normaliza(searchString)) ||
                                                   p.NumeroControl.ToString() == searchString);
                }
            }

            int pageSize = 10;
            return View(await PaginatedList<Prisionespreventivas>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Prisionespreventivas/Create
        public IActionResult Create()
        {
            ViewBag.listaSexo = listaSexo;
            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

            return View();
        }

        // POST: Prisionespreventivas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile archivo, [Bind("Idprisionespreventivas,NumeroControl,Paterno,Materno,Nombre,Genero,FechaRecepcion,CausaPenal,Delito,Capturista,Observaciones")] Prisionespreventivas prisionespreventivas, Expedienteunico expedienteunico, string tabla, string idselecionado, string CURS, string datosArray)
        {
            string currentUser = User.Identity.Name;

            prisionespreventivas.NumeroControl = prisionespreventivas.NumeroControl;
            prisionespreventivas.Paterno = mg.normaliza(prisionespreventivas.Paterno);
            prisionespreventivas.Materno = mg.normaliza(prisionespreventivas.Materno);
            prisionespreventivas.Nombre = mg.normaliza(prisionespreventivas.Nombre);
            prisionespreventivas.CausaPenal = mg.normaliza(prisionespreventivas.CausaPenal);
            prisionespreventivas.Observaciones = mg.normaliza(prisionespreventivas.Observaciones);
            prisionespreventivas.Capturista = currentUser;
            prisionespreventivas.ClaveUnicaScorpio = CURS;

            
            int id = 0;
            int cont = (from table in _context.Prisionespreventivas
                        select table.Idprisionespreventivas).Count();
            if (cont != 0)
            {
                id = ((from table in _context.Prisionespreventivas
                       select table.Idprisionespreventivas).Max()) + 1;
            }
            else
            {
                id = 1;
            }
            prisionespreventivas.Idprisionespreventivas = id;

            #region -Guardar archivo-
            if (archivo != null)
            {
                string file_name = id + "_" + prisionespreventivas.Paterno + "_" + prisionespreventivas.Materno + "_" + prisionespreventivas.Nombre + Path.GetExtension(archivo.FileName);
                file_name = mg.replaceSlashes(file_name);
                prisionespreventivas.RutaArchivo = file_name;
                var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "PP");
                var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                await archivo.CopyToAsync(stream);
                stream.Close();
            }
            #endregion

            #region -Expediente Unico-

            string var_tablanueva = "";
            string var_tablaSelect = "";
            string var_tablaCurs = "";
            int var_idnuevo = 0;
            int var_idSelect = 0;
            string var_curs = "";
            if (idselecionado != null && tabla != null)
            {
                var_tablanueva = mg.cambioAbase(mg.RemoveWhiteSpaces("PrisionPreventiva"));
                var_tablaSelect = mg.cambioAbase(mg.RemoveWhiteSpaces(tabla));
                var_tablaCurs = "ClaveUnicaScorpio";
                var_idnuevo = id;
                var_idSelect = Int32.Parse(idselecionado);
                var_curs = CURS;

                string query = $"CALL spInsertExpedienteUnico('{var_tablanueva}', '{var_tablaSelect}', '{var_tablaCurs}', {var_idnuevo}, {var_idSelect},  '{var_curs}');";
                _context.Database.ExecuteSqlCommand(query);

                if (datosArray != null)
                {
                    List<Dictionary<string, string>> listaObjetos = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(datosArray);

                    // Proyectar la lista para obtener solo los valores de id y tabla
                    var resultados = listaObjetos.Select(obj => new {
                        id = obj["id"],
                        tabla = obj["tabla"]
                    });

                    foreach (var resultado in resultados)
                    {
                        var_tablanueva = mg.cambioAbase(mg.RemoveWhiteSpaces("PrisionPreventiva"));
                        var_tablaSelect = mg.cambioAbase(mg.RemoveWhiteSpaces(resultado.tabla));
                        var_tablaCurs = "ClaveUnicaScorpio";
                        var_idnuevo = id;
                        var_idSelect = Int32.Parse(resultado.id);

                        string query2 = $"CALL spInsertExpedienteUnico('{var_tablanueva}', '{var_tablaSelect}', '{var_tablaCurs}', {var_idnuevo}, {var_idSelect},  '{var_curs}');";
                        _context.Database.ExecuteSqlCommand(query2);
                    }
                }


            }
            #endregion

            _context.Add(prisionespreventivas);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Prisionespreventivas/Edit/5
        public async Task<IActionResult> Edit(int? id, int? numeroControl)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prisionespreventivas = await _context.Prisionespreventivas.SingleOrDefaultAsync(m => m.Idprisionespreventivas == id);

            if (prisionespreventivas == null)
            {
                return NotFound();
            }

            // Set to negative for cloning, positive for edit
            if (numeroControl != null)
            {
                prisionespreventivas.Idprisionespreventivas = -(int)prisionespreventivas.Idprisionespreventivas;
            }

            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

            ViewBag.listaSexo = listaSexo;
            ViewBag.idGenero = prisionespreventivas.Genero;
            ViewBag.delito = prisionespreventivas.Delito;

            if (numeroControl == 0)
            {
                prisionespreventivas.NumeroControl = null;
                ViewBag.numeroControl = null;
            }
            else
            {
                ViewBag.numeroControl = prisionespreventivas.NumeroControl;
            }

            return View(prisionespreventivas);
        }

        // POST: Prisionespreventivas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile archivo, [Bind("Idprisionespreventivas,NumeroControl,Paterno,Materno,Nombre,Genero,FechaRecepcion,CausaPenal,Delito,Capturista,Observaciones,ClaveUnicaScorpio")] Prisionespreventivas prisionespreventivas)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool clone = false;
                    if(prisionespreventivas.Idprisionespreventivas < 0)
                    {
                        clone = true;
                        prisionespreventivas.Idprisionespreventivas = -prisionespreventivas.Idprisionespreventivas;
                    }


                    prisionespreventivas.Paterno = mg.normaliza(prisionespreventivas.Paterno);
                    prisionespreventivas.Materno = mg.normaliza(prisionespreventivas.Materno);
                    prisionespreventivas.Nombre = mg.normaliza(prisionespreventivas.Nombre);
                    prisionespreventivas.CausaPenal = mg.normaliza(prisionespreventivas.CausaPenal);
                    prisionespreventivas.Observaciones = mg.normaliza(prisionespreventivas.Observaciones);
                    prisionespreventivas.ClaveUnicaScorpio = prisionespreventivas.ClaveUnicaScorpio;
                    var oldPrisionespreventivas = await _context.Prisionespreventivas.FindAsync(prisionespreventivas.Idprisionespreventivas);

                    if (clone)
                    {
                        int count = (from table in _context.Prisionespreventivas
                                     select table.Idprisionespreventivas).Count();
                        int idPrisionesPreventivas;
                        if (count == 0)
                        {
                            idPrisionesPreventivas = 1;
                        }
                        else
                        {
                            idPrisionesPreventivas = ((from table in _context.Prisionespreventivas
                                                       select table.Idprisionespreventivas).Max()) + 1;
                        }
                        prisionespreventivas.Idprisionespreventivas = idPrisionesPreventivas;
                    }

                    #region -EditarArchivo-
                    if (archivo == null)
                    {
                        prisionespreventivas.RutaArchivo = oldPrisionespreventivas.RutaArchivo;
                    }
                    else
                    {
                        string file_name = prisionespreventivas.Idprisionespreventivas + "_" + prisionespreventivas.Paterno + "_" + prisionespreventivas.Materno + "_" + prisionespreventivas.Nombre + Path.GetExtension(archivo.FileName);
                        file_name = mg.replaceSlashes(file_name);
                        prisionespreventivas.RutaArchivo = file_name;
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "PP");

                        if (System.IO.File.Exists(Path.Combine(uploads, file_name)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, file_name));
                        }

                        var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                        await archivo.CopyToAsync(stream);
                        stream.Close();
                    }
                    #endregion

                    if (clone)
                    {
                        _context.Add(prisionespreventivas);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                    }
                    else
                    {
                        _context.Entry(oldPrisionespreventivas).CurrentValues.SetValues(prisionespreventivas);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrisionespreventivasExists(prisionespreventivas.Idprisionespreventivas))
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
            return View(prisionespreventivas);
        }

        public IActionResult Duplicate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Edit", new { id = id, numeroControl = 0 });
            }
            return View();
        }
        // GET: Prisionespreventivas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prisionespreventivas = await _context.Prisionespreventivas
                .SingleOrDefaultAsync(m => m.Idprisionespreventivas == id);
            if (prisionespreventivas == null)
            {
                return NotFound();
            }

            return View(prisionespreventivas);
        }

        // POST: Prisionespreventivas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var prisionespreventivas = await _context.Prisionespreventivas.SingleOrDefaultAsync(m => m.Idprisionespreventivas == id);
            _context.Prisionespreventivas.Remove(prisionespreventivas);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrisionespreventivasExists(int id)
        {
            return _context.Prisionespreventivas.Any(e => e.Idprisionespreventivas == id);
        }
    }
}
