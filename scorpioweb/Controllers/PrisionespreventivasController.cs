﻿using System;
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

namespace scorpioweb.Controllers
{
    public class PrisionespreventivasController : Controller
    {
        private readonly penas2Context _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        #region -Metodos Generales-
        public string normaliza(string normalizar)
        {
            if (!String.IsNullOrEmpty(normalizar))
            {
                normalizar = normalizar.ToUpper();
            }
            return normalizar;
        }

        String replaceSlashes(string path)
        {
            String cleaned = "";

            for (int i = 0; i < path.Length; i++)
                if (path[i] == '/')
                    cleaned += '-';
                else
                    cleaned += path[i];
            return cleaned;
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
                    personas = personas.Where(p => (removeSpaces(p.Paterno) + " " + removeSpaces(p.Materno) + " " + removeSpaces(p.Nombre)).Contains(normaliza(searchString)) ||
                                                   (removeSpaces(p.Nombre) + " " + removeSpaces(p.Paterno) + " " + removeSpaces(p.Materno)).Contains(normaliza(searchString)) ||
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
        public async Task<IActionResult> Create(IFormFile archivo, [Bind("Idprisionespreventivas,NumeroControl,Paterno,Materno,Nombre,Genero,FechaRecepcion,CausaPenal,Delito,Capturista,Observaciones")] Prisionespreventivas prisionespreventivas)
        {
            string currentUser = User.Identity.Name;

            prisionespreventivas.Paterno = normaliza(prisionespreventivas.Paterno);
            prisionespreventivas.Materno = normaliza(prisionespreventivas.Materno);
            prisionespreventivas.Nombre = normaliza(prisionespreventivas.Nombre);
            prisionespreventivas.CausaPenal = normaliza(prisionespreventivas.CausaPenal);
            prisionespreventivas.Observaciones = normaliza(prisionespreventivas.Observaciones);
            prisionespreventivas.Capturista = currentUser;

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
                file_name = replaceSlashes(file_name);
                prisionespreventivas.RutaArchivo = file_name;
                var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "PP");
                var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                await archivo.CopyToAsync(stream);
                stream.Close();
            }
            #endregion

            _context.Add(prisionespreventivas);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Prisionespreventivas/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            ViewBag.catalogo = _context.Catalogodelitos.Select(Catalogodelitos => Catalogodelitos.Delito).ToList();

            ViewBag.listaSexo = listaSexo;
            ViewBag.idGenero = prisionespreventivas.Genero;
            ViewBag.delito = prisionespreventivas.Delito;

            return View(prisionespreventivas);
        }

        // POST: Prisionespreventivas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IFormFile archivo, [Bind("Idprisionespreventivas,NumeroControl,Paterno,Materno,Nombre,Genero,FechaRecepcion,CausaPenal,Delito,Capturista,Observaciones")] Prisionespreventivas prisionespreventivas)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    prisionespreventivas.Paterno = normaliza(prisionespreventivas.Paterno);
                    prisionespreventivas.Materno = normaliza(prisionespreventivas.Materno);
                    prisionespreventivas.Nombre = normaliza(prisionespreventivas.Nombre);
                    prisionespreventivas.CausaPenal = normaliza(prisionespreventivas.CausaPenal);
                    prisionespreventivas.Observaciones = normaliza(prisionespreventivas.Observaciones);
                    var oldPrisionespreventivas = await _context.Prisionespreventivas.FindAsync(prisionespreventivas.Idprisionespreventivas);
                    #region -EditarArchivo-
                    if (archivo == null)
                    {
                        prisionespreventivas.RutaArchivo = oldPrisionespreventivas.RutaArchivo;
                    }
                    else
                    {
                        string file_name = prisionespreventivas.Idprisionespreventivas + "_" + prisionespreventivas.Paterno + "_" + prisionespreventivas.Materno + "_" + prisionespreventivas.Nombre + Path.GetExtension(archivo.FileName);
                        file_name = replaceSlashes(file_name);
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

                    _context.Entry(oldPrisionespreventivas).CurrentValues.SetValues(prisionespreventivas);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
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
