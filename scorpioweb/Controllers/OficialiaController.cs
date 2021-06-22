using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using scorpioweb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace scorpioweb.Controllers
{
    public class OficialiaController : Controller
    {
        #region -Variables Globales-
        private readonly penas2Context _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
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

        public IActionResult Captura()
        {
            ViewBag.expide = _context.Expide.Select(Expide => Expide.Nombre).ToList();
            ViewBag.asunto = _context.Asuntooficio.Select(Asuntooficio => Asuntooficio.Asunto).ToList();
            ViewBag.cp = _context.Causapenal.Select(Causapenal => Causapenal.CausaPenal + ", Distrito " + Causapenal.Distrito + ", " + Causapenal.Juez).ToList();
            List<SelectListItem> ListaUsuarios = new List<SelectListItem>();
            int i = 0;
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
    }
}