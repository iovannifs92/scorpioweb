﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SautinSoft.Document;
using SautinSoft.Document.Drawing;
using QRCoder;
using System.Drawing;
using Size = SautinSoft.Document.Drawing.Size;
using System.Security.Claims;
using System.Data;
using Google.DataTable.Net.Wrapper.Extension;
using Google.DataTable.Net.Wrapper;
using F23.StringSimilarity;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using SautinSoft.Document.MailMerging;
using DocumentFormat.OpenXml.Office.Word;
using System.Data.SqlClient;
using scorpioweb.Data;
using DocumentFormat.OpenXml.EMMA;
using scorpioweb.Class;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using Microsoft.AspNetCore.Rewrite.Internal;
using ZXing.OneD;
using System.ComponentModel.DataAnnotations;
using Syncfusion.EJ2.Navigations;
using Syncfusion.EJ2.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MySql.Data.MySqlClient;
using System.Data.Common;



namespace scorpioweb.Controllers
{
    [Authorize]
    public class PersonasController : Controller
    {

        #region -Variables Globales-
        //To get content root path of the project
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly penas2Context _context;
        public static int contadorSustancia;
        public static int contadorFamiliares;
        public static int contadorReferencias;
        public static List<List<string>> datosSustancias = new List<List<string>>();
        public static List<List<string>> datosSustanciasEditadas = new List<List<string>>();
        public static List<List<string>> datosFamiliares = new List<List<string>>();
        public static List<List<string>> datosFamiliaresEditados = new List<List<string>>();
        public static List<List<string>> datosReferencias = new List<List<string>>();
        public static List<List<string>> datosReferenciasEditadas = new List<List<string>>();
        public static List<List<string>> datosFamiliaresExtranjero = new List<List<string>>();
        public static List<List<string>> datosDomiciolioSecundario = new List<List<string>>();
        public static int idPersona;
        public static List<Consumosustancias> consumosustancias;
        public static List<Asientofamiliar> familiares;
        public static List<Asientofamiliar> referenciaspersonales;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        private List<SelectListItem> listaJuzgados = new List<SelectListItem>
        {
            new SelectListItem { Text = "NA", Value = "NA" },
            new SelectListItem { Text = "JUZGADO 1", Value = "JUZGADO 1" },
            new SelectListItem { Text = "JUZGADO 2", Value = "JUZGADO 2" },
            new SelectListItem { Text = "JUZGADO 3", Value = "JUZGADO 3" }
        };
        private List<SelectListItem> listaNoSi = new List<SelectListItem>
        {
            new SelectListItem{ Text="No", Value="NO"},
            new SelectListItem{ Text="Si", Value="SI"}
        };
        private List<SelectListItem> listaNoSiNA = new List<SelectListItem>
        {
            new SelectListItem{ Text="NA", Value="NA"},
            new SelectListItem{ Text="No", Value="NO"},
            new SelectListItem{ Text="Si", Value="SI"}
        };

        private List<SelectListItem> listaSiNo = new List<SelectListItem>
        {
            new SelectListItem{ Text="Si", Value="SI"},
            new SelectListItem{ Text="No", Value="NO"}
        };

        private List<SelectListItem> listaZonas = new List<SelectListItem>
        {
            new SelectListItem{ Text="Sin zona asignada", Value="SIN ZONA ASIGNADA"},
            new SelectListItem{ Text="Zona 1", Value="ZONA 1"},
            new SelectListItem{ Text="Zona 2", Value="ZONA 2"},
            new SelectListItem{ Text="Zona 3", Value="ZONA 3"},
            new SelectListItem{ Text="Zona 4", Value="ZONA 4"},
            new SelectListItem{ Text="Zona 5", Value="ZONA 5"},
            new SelectListItem{ Text="Zona 6", Value="ZONA 6"},
            new SelectListItem{ Text="Zona 7", Value="ZONA 7"}
        };


        private List<SelectListItem> listaUbicacionExpediente = new List<SelectListItem>
        {
            new SelectListItem{ Text="NA", Value="NA"},
            new SelectListItem{ Text="MCSCP1-1", Value="MCSCP1-1"},
            new SelectListItem{ Text="MCSCP1-2", Value="MCSCP1-2"},
            new SelectListItem{ Text="MCSCP1-3", Value="MCSCP1-3"},
            new SelectListItem{ Text="MCSCP1-4", Value="MCSCP1-4"},
            new SelectListItem{ Text="MCSCP2-1", Value="MCSCP2-1"},
            new SelectListItem{ Text="MCSCP2-2", Value="MCSCP2-2"},
            new SelectListItem{ Text="MCSCP2-3", Value="MCSCP2-3"},
            new SelectListItem{ Text="MCSCP2-4", Value="MCSCP2-4"},
            new SelectListItem{ Text="MCSCP3-1", Value="MCSCP3-1"},
            new SelectListItem{ Text="MCSCP3-2", Value="MCSCP3-2"},
            new SelectListItem{ Text="MCSCP3-3", Value="MCSCP3-3"},
            new SelectListItem{ Text="MCSCP3-4", Value="MCSCP3-4"},
            new SelectListItem{ Text="MCSCP4-1", Value="MCSCP4-1"},
            new SelectListItem{ Text="MCSCP4-2", Value="MCSCP4-2"},
            new SelectListItem{ Text="MCSCP4-3", Value="MCSCP4-3"},
            new SelectListItem{ Text="MCSCP4-4", Value="MCSCP4-4"},
            new SelectListItem{ Text="MCSCP5-1", Value="MCSCP5-1"},
            new SelectListItem{ Text="MCSCP5-2", Value="MCSCP5-2"},
            new SelectListItem{ Text="MCSCP5-3", Value="MCSCP5-3"},
            new SelectListItem{ Text="MCSCP5-4", Value="MCSCP5-4"},
            new SelectListItem{ Text="MCSCP6-1", Value="MCSCP6-1"},
            new SelectListItem{ Text="MCSCP6-2", Value="MCSCP6-2"},
            new SelectListItem{ Text="MCSCP6-3", Value="MCSCP6-3"},
            new SelectListItem{ Text="MCSCP6-4", Value="MCSCP6-4"},
            new SelectListItem{ Text="MCSCP7-1", Value="MCSCP7-1"},
            new SelectListItem{ Text="MCSCP7-2", Value="MCSCP7-2"},
            new SelectListItem{ Text="MCSCP7-3", Value="MCSCP7-3"},
            new SelectListItem{ Text="MCSCP7-4", Value="MCSCP7-4"},
            new SelectListItem{ Text="PRESTAMO", Value="PRESTAMO"},
            new SelectListItem{ Text="ARCHIVO", Value="ARCHIVO"}
        };

        public async Task<List<string>> ObtenerListaSupervisoresmcyscocl()
        {
            List<string> listasupervisores = new List<string>();

            foreach (var u in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(u, "SupervisorLC"))
                {
                    listasupervisores.Add(u.ToString());
                }
                if (await userManager.IsInRoleAsync(u, "AdminLC"))
                {
                    listasupervisores.Add(u.ToString());
                }
                if (await userManager.IsInRoleAsync(u, "AdminMCSCP"))
                {
                    listasupervisores.Add(u.ToString());
                }
                if (await userManager.IsInRoleAsync(u, "SupervisorMCSCP"))
                {
                    listasupervisores.Add(u.ToString());
                }
            }

            return listasupervisores.Where(r => listasupervisores.Any(f => !r.EndsWith("\u0040nortedgepms.com"))).ToList();
        }
        #endregion

        #region -Constructor-


        public PersonasController(penas2Context context, IHostingEnvironment hostingEnvironment,
                                  RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager
            )
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;

            this.roleManager = roleManager;
            this.userManager = userManager;

        }
        #endregion





        #region -Metodos Generales-
        MetodosGenerales mg = new MetodosGenerales();
        #endregion

        #region -contarFalsos-
        //public void contarFalsos()
        //{
        //    var personas = from p in _context.Persona
        //                   select new
        //                   {
        //                       paterno = p.Paterno,
        //                       materno = p.Materno,
        //                       fnacimiento = p.Fnacimiento,
        //                       genero = p.Genero,
        //                       lnestado = p.Lnestado,
        //                       nombre = p.Nombre,
        //                       nomcom = p.Paterno + " " + p.Materno + " " + p.Nombre,
        //                       id = p.IdPersona
        //                   };
        //    var listaPersonas = personas.ToList();
        //    var personasCount = personas.Count();
        //    int cut = (int)(0.8 * personasCount);
        //    var cosine = new Cosine(2);
        //    string[] CURP = new string[personasCount];
        //    for (int i = 0; i < personasCount; i++)
        //    {
        //        CURP[i] = curp(listaPersonas[i].paterno, listaPersonas[i].materno, listaPersonas[i].fnacimiento, listaPersonas[i].genero, listaPersonas[i].lnestado, listaPersonas[i].nombre);
        //    }
        //    int falsosNegativos = 0;
        //    int falsosPositivos = 0;
        //    int falsosPositivosTotal = 0;
        //    for (int i = cut; i < personasCount; i++)
        //    {
        //        string nombreCompleto = listaPersonas[i].nomcom;
        //        double mx = 0;
        //        int mxId = 0;
        //        for (int j = 0; j < cut; j++)
        //        {
        //            double r = cosine.Similarity(listaPersonas[j].nomcom, nombreCompleto);
        //            if (r > mx)
        //            {
        //                mx = r;
        //                mxId = j;
        //            }
        //        }
        //        if (mx < 0.87 && CURP[mxId] == CURP[i])
        //        {
        //            falsosNegativos++;
        //        }
        //        if (mx >= 0.87 && CURP[mxId] != CURP[i])
        //        {
        //            if (CURP[i].IndexOf("*") == -1 && CURP[mxId].IndexOf("*") == -1)
        //            {
        //                Debug.WriteLine(nombreCompleto + ", " + listaPersonas[mxId].nomcom + ", " + CURP[i] + ", " + CURP[mxId] + ", " + listaPersonas[i].id + ", " + listaPersonas[mxId].id + ", " + mx + ", curp: " + cosine.Similarity(CURP[i], CURP[mxId]));
        //                falsosPositivos++;
        //            }
        //            falsosPositivosTotal++;
        //        }
        //    }
        //    Debug.WriteLine("falsos Negativos " + falsosNegativos);
        //    Debug.WriteLine("falsos Positivos " + falsosPositivos);
        //    Debug.WriteLine("falsos Positivos Total " + falsosPositivosTotal);
        //}
        #endregion

        #region -Pruebas-
        public ActionResult Pruebas()
        {
            return View();
        }
        #endregion

        #region -Index viejo-
        //public async Task<IActionResult> Index(
        //    string sortOrder,
        //    string currentFilter,
        //    string searchString, 
        //    int? pageNumber)
        //{
        //    //para ver si la  persona tiene o no huella registrada
        //    var queryhayhuella = from r in _context.Registrohuella
        //                         join p in _context.Presentacionperiodica on r.IdregistroHuella equals p.RegistroidHuella
        //                         group r by r.PersonaIdPersona into grup
        //                         select new
        //                         {
        //                             grup.Key,
        //                             Count = grup.Count()
        //                         };

        //    foreach (var personaHuella in queryhayhuella)
        //    {
        //        if (personaHuella.Count >= 1)
        //        {
        //            ViewBag.personaIdPersona = personaHuella.Key;
        //        };
        //    }
        //    #region -ListaUsuarios-            
        //    var user = await userManager.FindByNameAsync(User.Identity.Name);
        //    ViewBag.user = user;
        //    var roles = await userManager.GetRolesAsync(user);
        //    ViewBag.Admin = false;
        //    ViewBag.Masteradmin = false;
        //    ViewBag.Archivo = false;
        //    ViewBag.Serviciosprevios = false;

        //    foreach (var rol in roles)
        //    {
        //        if (rol == "AdminMCSCP")
        //        {
        //            ViewBag.Admin = true;
        //        }
        //    }
        //    foreach (var rol in roles)
        //    {
        //        if (rol == "Masteradmin")
        //        {
        //            ViewBag.Masteradmin = true;
        //        }
        //    }
        //    foreach (var rol in roles)
        //    {
        //        if (rol == "ArchivoMCSCP")
        //        {
        //            ViewBag.Archivo = true;
        //        }
        //    }

        //    foreach (var rol in roles)
        //    {
        //        if (rol == "Servicios previos")
        //        {
        //            ViewBag.Serviciosprevios = true;
        //        }
        //    }


        //    List<string> rolUsuario = new List<string>();

        //    for (int i = 0; i < roles.Count; i++)
        //    {
        //        rolUsuario.Add(roles[i]);
        //    }

        //    ViewBag.RolesUsuario = rolUsuario;
        //    String users = user.ToString();
        //    ViewBag.RolesUsuarios = users;
        //    List<string> ListaUsuariosAdminMCSCP = new List<string>();
        //    if (users == "isabel.almora@dgepms.com")
        //    {
        //        ListaUsuariosAdminMCSCP.Add("Seleccione");
        //        ListaUsuariosAdminMCSCP.Add("Expediente Concluido para Razón de Archivo");
        //        foreach (var u in userManager.Users)
        //        {
        //            if (await userManager.IsInRoleAsync(u, "SupervisorMCSCP"))
        //            {
        //                ListaUsuariosAdminMCSCP.Add(u.ToString());
        //            }
        //        }
        //    }
        //    ViewBag.ListadoUsuariosAdminMCSCP = ListaUsuariosAdminMCSCP;

        //    List<String> ListaUsuarios = new List<String>();

        //    ListaUsuarios.Add("Sin Registro");
        //    ListaUsuarios.Add("Archivo Interno");
        //    ListaUsuarios.Add("Archivo General");
        //    ListaUsuarios.Add("No Ubicado");
        //    ListaUsuarios.Add("Dirección");
        //    ListaUsuarios.Add("Coordinación Operativa");
        //    ListaUsuarios.Add("Coordinación MC y SCP");
        //    ListaUsuarios.Add("Expediente Concluido para Razón de Archivo");
        //    foreach (var u in userManager.Users)
        //    {
        //        if (await userManager.IsInRoleAsync(u, "SupervisorMCSCP"))
        //        {
        //            ListaUsuarios.Add(u.ToString());
        //        }
        //    }
        //    ViewBag.ListadoUsuarios = ListaUsuarios;
        //    #endregion

        //    ViewData["CurrentSort"] = sortOrder;
        //    ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        //    ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";


        //    if (searchString != null)
        //    {
        //        pageNumber = 1;
        //    }
        //    else
        //    {
        //        searchString = currentFilter;
        //    }

        //    var usu = await userManager.FindByNameAsync(User.Identity.Name);

        //    #region -Solicitud Atendida Archivo prestamo Digital-
        //    var warningRespuesta = from a in _context.Archivoprestamodigital
        //                           where a.EstadoPrestamo == 1 && usu.ToString().ToUpper() == a.Usuario.ToUpper()
        //                           select a;
        //    ViewBag.WarningsUser = warningRespuesta.Count();
        //    #endregion

        //    #region -PROXIMO SUPERVISOR-
        //    //// Obtén el último supervisor
        //    //var ultimoSupervisor = _context.Persona
        //    //                        .Where(p => p.Supervisor != null && p.Supervisor != "" && p.Supervisor != "enrique.martinez@dgepms.com" && !p.Supervisor.EndsWith("@nortedgepms.com"))
        //    //                        .OrderByDescending(p => p.IdPersona)
        //    //                        .Select(p => p.Supervisor)
        //    //                        .FirstOrDefault();
        //    //// Obtén la lista de usuarios y ordénala alfabéticamente
        //    //List<SelectListItem> ListaSuper = new List<SelectListItem>();
        //    //int j = 0;
        //    //foreach (var usuarios in userManager.Users)
        //    //{
        //    //    if (await userManager.IsInRoleAsync(usuarios, "SupervisorMCSCP"))
        //    //    {
        //    //        ListaSuper.Add(new SelectListItem
        //    //        {
        //    //            Text = usuarios.ToString(),
        //    //            Value = j.ToString()
        //    //        });
        //    //        j++;
        //    //    }
        //    //}

        //    //// Ordena la lista alfabéticamente por el texto (nombre de usuario)
        //    //ListaSuper = ListaSuper
        //    //            .Where(item => item.Text.EndsWith("@dgepms.com") && item.Text != "diana.renteria@dgepms.com" && item.Text != "enrique.martinez@dgepms.com")
        //    //            .OrderBy(item => item.Text)
        //    //            .ToList();

        //    //var siguienteSupervisor = ListaSuper.FirstOrDefault(item => string.Compare(item.Text, ultimoSupervisor, true) > 0);

        //    //if (siguienteSupervisor != null)
        //    //{
        //    //    var valorSiguienteSupervisor = siguienteSupervisor.Text;
        //    //    ViewBag.sigueinteSuperviosor = valorSiguienteSupervisor;
        //    //}
        //    //else
        //    //{
        //    //    ViewBag.sigueinteSuperviosor = ListaSuper.OrderBy(p => p.Text).Select(p => p.Text).FirstOrDefault();
        //    //}
        //    #endregion

        //    ViewData["CurrentFilter"] = searchString;

        //    var personas = from p in _context.Persona
        //                   where p.Supervisor != null
        //                   select p;


        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        foreach (var item in searchString.Split(new char[] { ' ' },
        //            StringSplitOptions.RemoveEmptyEntries))
        //        {
        //            personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(searchString) ||
        //                                           (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(searchString) ||
        //                                           p.Supervisor.Contains(searchString));
        //        }
        //    }

        //    switch (sortOrder)
        //    {
        //        case "name_desc":
        //            personas = personas.OrderByDescending(p => p.IdPersona);
        //            break;
        //        default:
        //            personas = personas.OrderByDescending(p => p.IdPersona);
        //            break;
        //    }

        //    int pageSize = 10;
        //    ViewBag.totalPages = (personas.Count() + pageSize - 1) / pageSize;

        //    //Response.Headers.Add("Refresh", "5");
        //    return View(await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        //    //return Json(await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        //}
        #endregion

        #region -Index nuevo-

        public async Task<IActionResult> Index()
        {

            var nomsuper = User.Identity.Name.ToString();
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);


            #region -Sacar USUARIOS ADMIN-


            ViewBag.user = user;
            ViewBag.Admin = false;

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP")
                {
                    ViewBag.Admin = true;
                }
            }
            #endregion

            #region -Solicitud Atendida Archivo prestamo Digital-
            var warningRespuesta = from a in _context.Archivoprestamodigital
                                   where a.EstadoPrestamo == 1 && user.ToString().ToUpper() == a.Usuario.ToUpper()
                                   select a;
            ViewBag.WarningsUser = warningRespuesta.Count();
            #endregion  

            ViewBag.RolesUsuarios = nomsuper;

            return View("Index", await _context.Persona.ToListAsync());
        }



        public async Task<IActionResult> GetPrueba(string sortOrder, string currentFilter, string Search, int? pageNumber, bool usuario)
        {
            #region -ListaUsuarios-            

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var nomsuper = User.Identity.Name.ToString();
            var roles = await userManager.GetRolesAsync(user);
            bool super = false;
            bool admin = false;
            bool invitado = true;
            bool bitacora = false;

            foreach (var rol in roles)
            {
                if (rol == "SupervisorMCSCP")
                {
                    super = true;
                }
            }
            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP" || rol == "Masteradmin")
                {
                    admin = true;
                }
            }

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP" || rol == "Masteradmin" || rol == "SupervisorMCSCP")
                {
                    // SE VUELVE FALSO SI EL USUARIO TIENE UN ROL DE MC Y SCP 
                    invitado = false;
                    break;

                }
            }

            foreach (var rol in roles)
            {
                if (rol == "Servicios previos" || rol == "Operativo")
                {
                    bitacora = true;
                }
            }

            String users = user.ToString();
            ViewBag.RolesUsuarios = users;
            #endregion
            List<Persona> listaSupervisados = new List<Persona>();
            listaSupervisados = (from table in _context.Persona
                                 select table).ToList();
            listaSupervisados.Insert(0, new Persona { IdPersona = 0, Supervisor = "Selecciona" });
            ViewBag.listaSupervisados = listaSupervisados;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (Search != null)
            {
                pageNumber = pageNumber;
            }
            else
            {
                Search = currentFilter;
            }

            ViewData["CurrentFilter"] = Search;



            var personas = from p in _context.Persona
                           where p.Supervisor != null
                           select p;

            if (!String.IsNullOrEmpty(Search))
            {
                foreach (var item in Search.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(Search) ||
                                                   (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(Search) ||
                                                   p.Supervisor.Contains(Search) || (p.IdPersona.ToString()).Contains(Search));

                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
                default:
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
            }

            personas.OrderByDescending(p => p.IdPersona);

            //PARA VER SUPERVISADOS DEL USUARIO CON SESION INICIADA
            if (usuario == true)
            {
                personas = personas.Where(p => p.Supervisor == nomsuper);
            };

            int pageSize = 10;
            // Response.Headers.Add("Refresh", "5");
            return Json(new
            {
                page = await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize),
                totalPages = (personas.Count() + pageSize - 1) / pageSize,
                admin,
                super,
                invitado,
                bitacora,
                nomsuper = nomsuper
            });
        }
        Persona persona1;
        List<Persona> personas1;

        public async Task<ActionResult> Personas()
        {
            Tuple<List<Persona>, Persona> tuple;

            tuple = new Tuple<List<Persona>, Persona>(personas1, persona1);

            return View("PersonasDetails", tuple);
        }
        #endregion

        #region -ListadoSupervisor-
        public async Task<IActionResult> ListadoSupervisor(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            String users = user.ToString();
            ViewBag.RolesUsuario = users;


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
                foreach (var item in searchString.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(searchString) ||
                                                   (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(searchString) ||
                                                   (p.IdPersona.ToString()).Contains(searchString)
                                                   );
                }
            }


            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
                default:
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
            }
            int pageSize = 10;
            return Json(new
            {
                page = await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize),
                totalPages = (personas.Count() + pageSize - 1) / pageSize
            });
        }
        #endregion

        #region -Colaboraciones-
        public IActionResult Colaboraciones()
        {
            var user = User.Identity.Name;


            #region -Solicitud Atendida Archivo prestamo Digital-
            var warningRespuesta = from a in _context.Archivoprestamodigital
                                   where a.EstadoPrestamo == 1 && user.ToString().ToUpper() == a.Usuario.ToUpper()
                                   select a;
            ViewBag.WarningsUser = warningRespuesta.Count();
            ViewBag.Usuario = user;

            #endregion

            var colaboraciones = (from persona in _context.Persona
                                  join domicilio in _context.Domicilio on persona.IdPersona equals domicilio.PersonaIdPersona
                                  join municipio in _context.Municipios on int.Parse(domicilio.Municipio) equals municipio.Id
                                  join supervision in _context.Supervision on persona.IdPersona equals supervision.PersonaIdPersona
                                  where supervision.EstadoSupervision == "VIGENTE" && supervision.Tta == "SI"
                                  select new PersonaViewModel
                                  {
                                      personaVM = persona,
                                      municipiosVMDomicilio = municipio,
                                      CasoEspecial = "TTA"
                                  }).Union
                            (from persona in _context.Persona
                             join domicilio in _context.Domicilio on persona.IdPersona equals domicilio.PersonaIdPersona
                             join municipio in _context.Municipios on int.Parse(domicilio.Municipio) equals municipio.Id
                             join supervision in _context.Supervision on persona.IdPersona equals supervision.PersonaIdPersona
                             join fraccion in _context.Fraccionesimpuestas on supervision.IdSupervision equals fraccion.SupervisionIdSupervision
                             where fraccion.Tipo == "XIII" && supervision.EstadoSupervision == "VIGENTE" && fraccion.FiguraJudicial == "MC"
                             select new PersonaViewModel
                             {
                                 personaVM = persona,
                                 municipiosVMDomicilio = municipio,
                                 CasoEspecial = "Resguardo Domiciliario"
                             }).Union
                           (from persona in _context.Persona
                            join domicilio in _context.Domicilio on persona.IdPersona equals domicilio.PersonaIdPersona
                            join municipio in _context.Municipios on int.Parse(domicilio.Municipio) equals municipio.Id
                            where persona.Colaboracion == "SI"
                            select new PersonaViewModel
                            {
                                personaVM = persona,
                                municipiosVMDomicilio = municipio,
                                CasoEspecial = "Colaboración"
                            });
            return View(colaboraciones);


        }
        #endregion

        #region -MenuMCSCP-
        public async Task<IActionResult> MenuMCSCP()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            string usuario = user.ToString();
            ViewBag.Usuario = usuario;
            DateTime fechaInforme = (DateTime.Today).AddDays(5);
            DateTime fechaControl = (DateTime.Today).AddDays(3);
            DateTime fechaInformeCoordinador = (DateTime.Today).AddDays(60);
            DateTime fechahoy = DateTime.Today;
            var fechaProcesal = DateTime.Now.AddMonths(-6);
            Boolean flagMaster = false;

            #region -Solicitud Atendida Archivo prestamo Digital-
            var warningRespuesta = from a in _context.Archivoprestamodigital
                                   where a.EstadoPrestamo == 1 && user.ToString().ToUpper() == a.Usuario.ToUpper()
                                   select a;
            ViewBag.WarningsUser = warningRespuesta.Count();
            #endregion

            #region -ASIGNAR SUPERVISOR-
            //// Obtén el último supervisor
            //var ultimoSupervisor = _context.Persona
            //                        .Where(p => p.Supervisor != null && p.Supervisor != "" && p.Supervisor != "enrique.martinez@dgepms.com" && !p.Supervisor.EndsWith("@nortedgepms.com"))
            //                        .OrderByDescending(p => p.IdPersona)
            //                        .Select(p => p.Supervisor)
            //                        .FirstOrDefault();
            //// Obtén la lista de usuarios y ordénala alfabéticamente
            //List<SelectListItem> ListaSuper = new List<SelectListItem>();
            //int j = 0;
            //foreach (var users in userManager.Users)
            //{
            //    if (await userManager.IsInRoleAsync(users, "SupervisorMCSCP"))
            //    {
            //        ListaSuper.Add(new SelectListItem
            //        {
            //            Text = users.ToString(),
            //            Value = j.ToString()
            //        });
            //        j++;
            //    }
            //}

            //// Ordena la lista alfabéticamente por el texto (nombre de usuario)
            //ListaSuper = ListaSuper
            //            .Where(item => item.Text.EndsWith("@dgepms.com") && item.Text != "diana.renteria@dgepms.com" && item.Text != "enrique.martinez@dgepms.com")
            //            .OrderBy(item => item.Text)
            //            .ToList();
            //ViewBag.ListadoUsuarios = ListaSuper;

            //var siguienteSupervisor = ListaSuper.FirstOrDefault(item => string.Compare(item.Text, ultimoSupervisor, true) > 0);

            //if (siguienteSupervisor != null)
            //{
            //    var valorSiguienteSupervisor = siguienteSupervisor.Text;
            //    ViewBag.sigueinteSuperviosor = valorSiguienteSupervisor;
            //}
            //else
            //{
            //    ViewBag.sigueinteSuperviosor = ListaSuper.OrderBy(p => p.Text).Select(p => p.Text).FirstOrDefault();
            //}
            #endregion

            ViewBag.Warnings = 0;

            foreach (var rol in roles)
            {
                if (rol == "Masteradmin")
                {
                    flagMaster = true;
                }
                if (rol == "CE Resguardos")
                {
                    flagMaster = true;
                }
            }

            #region -To List databases-

            List<Persona> personaVM = _context.Persona.ToList();
            List<Supervision> supervisionVM = _context.Supervision.ToList();
            List<Causapenal> causapenalVM = _context.Causapenal.ToList();
            List<Domicilio> domicilioVM = _context.Domicilio.ToList();
            List<Municipios> municipiosVM = _context.Municipios.ToList();
            List<Estados> estadosVM = _context.Estados.ToList();
            List<Planeacionestrategica> planeacionestrategicaVM = _context.Planeacionestrategica.ToList();
            List<Fraccionesimpuestas> fraccionesimpuestasVM = _context.Fraccionesimpuestas.ToList();
            List<Archivointernomcscp> archivointernomcscpsVM = _context.Archivointernomcscp.ToList();
            List<Personacausapenal> personacausapenalsVM = _context.Personacausapenal.ToList();


            List<Fraccionesimpuestas> queryFracciones = (from f in fraccionesimpuestasVM
                                                         group f by f.SupervisionIdSupervision into grp
                                                         select grp.OrderByDescending(f => f.IdFracciones).FirstOrDefault()).ToList();

            List<Archivointernomcscp> queryHistorialArchivoadmin = (from a in _context.Archivointernomcscp
                                                                    group a by a.PersonaIdPersona into grp
                                                                    select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();

            var listaaudit = (from a in _context.Audit
                              join s in _context.Supervision on int.Parse(Regex.Replace(a.PrimaryKey, @"[^0-9]", "")) equals s.IdSupervision
                              where a.TableName == "Supervision" && a.NewValues.Contains("en espera de respuesta")
                              group a by int.Parse(Regex.Replace(a.PrimaryKey, @"[^0-9]", "")) into grp
                              select grp.OrderByDescending(a => a.Id).FirstOrDefault());

            #endregion

            #region -Jointables-
            var sinResolucion = from p in personaVM
                                join d in domicilioVM on p.IdPersona equals d.PersonaIdPersona
                                join e in estadosVM on int.Parse(d.Estado) equals e.Id
                                join m in municipiosVM on int.Parse(d.Municipio) equals m.Id
                                where p.TieneResolucion == "NO"
                                select new PlaneacionWarningViewModel
                                {
                                    personaVM = p,
                                    estadosVM = e,
                                    municipiosVM = m,
                                    tipoAdvertencia = "Sin Resolución"
                                };

            var sinResolucion2 = from p in personaVM
                                 join d in domicilioVM on p.IdPersona equals d.PersonaIdPersona
                                 join e in estadosVM on int.Parse(d.Estado) equals e.Id
                                 join m in municipiosVM on int.Parse(d.Municipio) equals m.Id
                                 where p.TieneResolucion == "NO" && p.Supervisor == usuario
                                 select new PlaneacionWarningViewModel
                                 {
                                     personaVM = p,
                                     estadosVM = e,
                                     municipiosVM = m,
                                     tipoAdvertencia = "Sin Resolución"
                                 };

            #region -Expediente Físico en Resguardo-
            //var archivoadmin = from ha in queryHistorialArchivoadmin
            //                   join ai in archivointernomcscpsVM on ha.IdarchivoInternoMcscp equals ai.IdarchivoInternoMcscp
            //                   join p in personaVM on ha.PersonaIdPersona equals p.IdPersona
            //                   join domicilio in domicilioVM on p.IdPersona equals domicilio.PersonaIdPersona
            //                   join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
            //                   where p.UbicacionExpediente != "ARCHIVO INTERNO" && p.UbicacionExpediente != "ARCHIVO GENERAL" &&
            //                   p.UbicacionExpediente != "NO UBICADO" && p.UbicacionExpediente != "SIN REGISTRO" && p.UbicacionExpediente != "NA" && p.UbicacionExpediente != null
            //                   select new PlaneacionWarningViewModel
            //                   {
            //                       municipiosVM = municipio,
            //                       personaVM = p,
            //                       archivointernomcscpVM = ai,
            //                       tipoAdvertencia = "Expediente físico en resguardo"
            //                   }; 
            #endregion

            var leftJoin = from persona in personaVM
                           join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona into tmp
                           from sinsupervision in tmp.DefaultIfEmpty()
                           select new PlaneacionWarningViewModel
                           {
                               personaVM = persona,
                               supervisionVM = sinsupervision,
                               tipoAdvertencia = "Sin supervisión"
                           };
            var where = from ss in leftJoin
                        where ss.supervisionVM == null
                        select new PlaneacionWarningViewModel
                        {
                            personaVM = ss.personaVM,
                            supervisionVM = ss.supervisionVM,
                            tipoAdvertencia = "Sin supervisión"
                        };
            var where2 = from ss in leftJoin
                         where ss.personaVM.Supervisor == usuario && ss.supervisionVM == null
                         select new PlaneacionWarningViewModel
                         {
                             personaVM = ss.personaVM,
                             supervisionVM = ss.supervisionVM,
                             tipoAdvertencia = "Sin supervisión"
                         };

            var personasConSupervision = from persona in personaVM
                                         join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                         join fracciones in fraccionesimpuestasVM on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                         select new Supervision
                                         {
                                             IdSupervision = supervision.IdSupervision
                                         };
            List<int> idSupervisiones = personasConSupervision.Select(x => x.IdSupervision).Distinct().ToList();
            var joins = from persona in personaVM
                        join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                        join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                        join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                        join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                        select new PlaneacionWarningViewModel
                        {
                            personaVM = persona,
                            supervisionVM = supervision,
                            municipiosVM = municipio,
                            causapenalVM = causapenal,
                            tipoAdvertencia = "Sin figura judicial"
                        };

            var table = from persona in personaVM
                        join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                        join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                        join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                        join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                        join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                        join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                        select new PlaneacionWarningViewModel
                        {
                            personaVM = persona,
                            supervisionVM = supervision,
                            causapenalVM = causapenal,
                            planeacionestrategicaVM = planeacion,
                            fraccionesimpuestasVM = fracciones,
                            figuraJudicial = fracciones.FiguraJudicial,
                            municipiosVM = municipio
                        };

            var tableAudiot = from persona in personaVM
                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                              join audit in listaaudit on supervision.IdSupervision equals int.Parse(Regex.Replace(audit.PrimaryKey, @"[^0-9]", ""))
                              select new PlaneacionWarningViewModel
                              {
                                  personaVM = persona,
                                  supervisionVM = supervision,
                                  causapenalVM = causapenal,
                                  planeacionestrategicaVM = planeacion,
                                  auditVM = audit,
                                  fechaCmbio = audit.DateTime,
                                  municipiosVM = municipio
                              };

            if (usuario == "isabel.almora@dgepms.com" || usuario == "janeth@nortedgepms.com" || flagMaster == true)
            {
                var warningPlaneacion = (where).Union
                                        (sinResolucion).Union
                                        //(archivoadmin).Union
                                        (joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionVM.IdSupervision) && s.supervisionVM.EstadoSupervision == "VIGENTE")).Union
                                        (from persona in personaVM
                                         join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                         join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                         where persona.Colaboracion == "SI"
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = persona,
                                             municipiosVM = municipio,
                                             tipoAdvertencia = "Pendiente de asignación - colaboración"
                                         }).Union
                                        (from t in table
                                         where t.planeacionestrategicaVM.FechaInforme != null && t.planeacionestrategicaVM.FechaInforme < fechaInformeCoordinador && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.fraccionesimpuestasVM.FiguraJudicial == "SCP"
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = t.personaVM,
                                             supervisionVM = t.supervisionVM,
                                             causapenalVM = t.causapenalVM,
                                             planeacionestrategicaVM = t.planeacionestrategicaVM,
                                             fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                             figuraJudicial = t.figuraJudicial,
                                             tipoAdvertencia = "Informe fuera de tiempo"
                                         }).Union
                                        (from t in table
                                         where t.planeacionestrategicaVM.FechaInforme != null && t.planeacionestrategicaVM.FechaInforme < fechaControl && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.fraccionesimpuestasVM.FiguraJudicial == "MC"
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = t.personaVM,
                                             supervisionVM = t.supervisionVM,
                                             causapenalVM = t.causapenalVM,
                                             planeacionestrategicaVM = t.planeacionestrategicaVM,
                                             fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                             figuraJudicial = t.figuraJudicial,
                                             tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                         }).Union
                                        (from t in table
                                         where t.planeacionestrategicaVM.FechaInforme == null && t.supervisionVM.EstadoSupervision == "VIGENTE"
                                         orderby t.fraccionesimpuestasVM.FiguraJudicial
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = t.personaVM,
                                             supervisionVM = t.supervisionVM,
                                             causapenalVM = t.causapenalVM,
                                             planeacionestrategicaVM = t.planeacionestrategicaVM,
                                             fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                             figuraJudicial = t.figuraJudicial,
                                             tipoAdvertencia = "Sin fecha de informe"
                                         }).Union
                                        (from persona in personaVM
                                         join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                         join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                         join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                         where planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = persona,
                                             supervisionVM = supervision,
                                             causapenalVM = causapenal,
                                             planeacionestrategicaVM = planeacion,
                                             tipoAdvertencia = "Sin periodicidad de firma"
                                         }).Union
                                        (from t in table
                                         where t.personaVM.Supervisor != null && t.personaVM.Supervisor.EndsWith("\u0040dgepms.com") && t.planeacionestrategicaVM.FechaProximoContacto != null && t.planeacionestrategicaVM.FechaProximoContacto < fechahoy && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.planeacionestrategicaVM.PeriodicidadFirma != "NO APLICA"
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = t.personaVM,
                                             supervisionVM = t.supervisionVM,
                                             causapenalVM = t.causapenalVM,
                                             planeacionestrategicaVM = t.planeacionestrategicaVM,
                                             fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                             figuraJudicial = t.figuraJudicial,
                                             tipoAdvertencia = "Se paso el tiempo de la firma"
                                         }).Union
                                        (from t in tableAudiot
                                         where t.personaVM.Supervisor != null && t.auditVM.DateTime < fechaProcesal && t.supervisionVM.EstadoSupervision != "CONCLUIDO" && t.supervisionVM.EstadoSupervision == "EN ESPERA DE RESPUESTA"
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = t.personaVM,
                                             supervisionVM = t.supervisionVM,
                                             causapenalVM = t.causapenalVM,
                                             planeacionestrategicaVM = t.planeacionestrategicaVM,
                                             fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                             tipoAdvertencia = "Estado Procesal"
                                         });
                var warnings = Enumerable.Empty<PlaneacionWarningViewModel>();
                if (usuario == "janeth@nortedgepms.com" || flagMaster == true)
                {
                    var filteredWarnings = from pwvm in warningPlaneacion
                                           where pwvm.personaVM.Supervisor != null && pwvm.personaVM.Supervisor.EndsWith("\u0040nortedgepms.com")
                                           select pwvm;
                    warnings = warnings.Union(filteredWarnings);


                }
                if (usuario == "isabel.almora@dgepms.com" || flagMaster == true)
                {
                    var filteredWarnings = from pwvm in warningPlaneacion
                                           where pwvm.personaVM.Supervisor != null && pwvm.personaVM.Supervisor.EndsWith("\u0040dgepms.com")
                                           select pwvm;
                    warnings = warnings.Union(filteredWarnings);
                }
                ViewBag.Warnings = warnings.Count();
            }
            else
            {
                List<Archivointernomcscp> queryHistorialArchivo = (from a in _context.Archivointernomcscp
                                                                   group a by a.PersonaIdPersona into grp
                                                                   select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();



                #region -Expediente Físico en Resguardo-
                //var archivo = from ha in queryHistorialArchivoadmin
                //              join ai in archivointernomcscpsVM on ha.IdarchivoInternoMcscp equals ai.IdarchivoInternoMcscp
                //              join p in personaVM on ha.PersonaIdPersona equals p.IdPersona
                //              join domicilio in domicilioVM on p.IdPersona equals domicilio.PersonaIdPersona
                //              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                //              where p.UbicacionExpediente != "ARCHIVO INTERNO" && p.UbicacionExpediente != "ARCHIVO GENERAL" &&
                //              p.UbicacionExpediente != "NO UBICADO" && p.UbicacionExpediente != "SIN REGISTRO" && p.UbicacionExpediente != "NA" && p.UbicacionExpediente != null && p.UbicacionExpediente == usuario.ToUpper()
                //              select new PlaneacionWarningViewModel
                //              {
                //                  municipiosVM = municipio,
                //                  personaVM = p,
                //                  archivointernomcscpVM = ai,
                //                  tipoAdvertencia = "Expediente físico en resguardo"
                //              }; 
                #endregion

                var warningPlaneacion =
                                        (where2).Union
                                        (sinResolucion2).Union
                                        //(archivo).Union
                                        (joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionVM.IdSupervision) && s.personaVM.Supervisor == usuario && s.supervisionVM.EstadoSupervision == "VIGENTE")).Union
                                        (from t in table
                                         where t.personaVM.Supervisor == usuario && t.planeacionestrategicaVM.FechaInforme != null && t.planeacionestrategicaVM.FechaInforme < fechaInformeCoordinador && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.fraccionesimpuestasVM.FiguraJudicial == "SCP"
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = t.personaVM,
                                             supervisionVM = t.supervisionVM,
                                             causapenalVM = t.causapenalVM,
                                             planeacionestrategicaVM = t.planeacionestrategicaVM,
                                             fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                             figuraJudicial = t.figuraJudicial,
                                             tipoAdvertencia = "Informe fuera de tiempo"
                                         }).Union
                                        (from t in table
                                         where t.personaVM.Supervisor == usuario && t.planeacionestrategicaVM.FechaInforme != null && t.planeacionestrategicaVM.FechaInforme < fechaControl && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.fraccionesimpuestasVM.FiguraJudicial == "MC"
                                         select new PlaneacionWarningViewModel
                                         {
                                             personaVM = t.personaVM,
                                             supervisionVM = t.supervisionVM,
                                             causapenalVM = t.causapenalVM,
                                             planeacionestrategicaVM = t.planeacionestrategicaVM,
                                             fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                             figuraJudicial = t.figuraJudicial,
                                             tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                         }).Union
                                    (from t in table
                                     where t.personaVM.Supervisor == usuario && t.planeacionestrategicaVM.FechaInforme == null && t.supervisionVM.EstadoSupervision == "VIGENTE"
                                     orderby t.fraccionesimpuestasVM.FiguraJudicial
                                     select new PlaneacionWarningViewModel
                                     {
                                         personaVM = t.personaVM,
                                         supervisionVM = t.supervisionVM,
                                         causapenalVM = t.causapenalVM,
                                         planeacionestrategicaVM = t.planeacionestrategicaVM,
                                         fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                         figuraJudicial = t.figuraJudicial,
                                         tipoAdvertencia = "Sin fecha de informe"
                                     }).Union
                                    (from persona in personaVM
                                     join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                     join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                     join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                     where persona.Supervisor == usuario && planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                     select new PlaneacionWarningViewModel
                                     {
                                         personaVM = persona,
                                         supervisionVM = supervision,
                                         causapenalVM = causapenal,
                                         planeacionestrategicaVM = planeacion,
                                         tipoAdvertencia = "Sin periodicidad de firma"
                                     }).Union
                                    (from t in table
                                     where t.personaVM.Supervisor != null && t.personaVM.Supervisor.EndsWith("\u0040dgepms.com") && t.personaVM.Supervisor == usuario && t.planeacionestrategicaVM.FechaProximoContacto != null && t.planeacionestrategicaVM.FechaProximoContacto < fechahoy && t.supervisionVM.EstadoSupervision == "VIGENTE" && (t.planeacionestrategicaVM.PeriodicidadFirma == null || t.planeacionestrategicaVM.PeriodicidadFirma != "NO APLICA")
                                     select new PlaneacionWarningViewModel
                                     {
                                         personaVM = t.personaVM,
                                         supervisionVM = t.supervisionVM,
                                         causapenalVM = t.causapenalVM,
                                         planeacionestrategicaVM = t.planeacionestrategicaVM,
                                         fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                         tipoAdvertencia = "Se paso el tiempo de la firma"
                                     }).Union
                                    (from t in tableAudiot
                                     where t.personaVM.Supervisor == usuario && t.personaVM.Supervisor != null && t.auditVM.DateTime < fechaProcesal && t.supervisionVM.EstadoSupervision != "CONCLUIDO" && t.supervisionVM.EstadoSupervision == "EN ESPERA DE RESPUESTA"
                                     select new PlaneacionWarningViewModel
                                     {
                                         personaVM = t.personaVM,
                                         supervisionVM = t.supervisionVM,
                                         causapenalVM = t.causapenalVM,
                                         planeacionestrategicaVM = t.planeacionestrategicaVM,
                                         fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                         tipoAdvertencia = "Estado Procesal"
                                     });
                ViewBag.Warnings = warningPlaneacion.Count();
            }
            #endregion

            ViewBag.MensajesAdmin = (from mensaje in _context.Mensajesistema
                                     where mensaje.Activo == "1"
                                     select mensaje).Count();

            ViewBag.MensajesUsuario = (from mensaje in _context.Mensajesistema
                                       where mensaje.Activo == "1" && mensaje.Usuario == usuario || mensaje.Colectivo == "1"
                                       select mensaje).Count();

            List<string> rolUsuario = new List<string>();

            for (int i = 0; i < roles.Count; i++)
            {
                rolUsuario.Add(roles[i]);
            }

            ViewBag.RolesUsuario = rolUsuario;
            return View();
        }



        #endregion

        #region -AsignaSupervision-

        public async Task<IActionResult> AsignacionSupervision()
        {
            var usu = await userManager.FindByNameAsync(User.Identity.Name);

            #region -Solicitud Atendida Archivo prestamo Digital-
            var warningRespuesta = from a in _context.Archivoprestamodigital
                                   where a.EstadoPrestamo == 1 && usu.ToString().ToUpper() == a.Usuario.ToUpper()
                                   select a;
            ViewBag.WarningsUser = warningRespuesta.Count();
            #endregion


            List<SelectListItem> ListaUsuarios = new List<SelectListItem>();
            int i = 0;
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, "SupervisorMCSCP"))
                {
                    ListaUsuarios.Add(new SelectListItem
                    {
                        Text = user.ToString(),
                        Value = i.ToString()
                    });
                    i++;
                }
            }
            ViewBag.ListadoUsuarios = ListaUsuarios;

            var supervisoresScorpio = from s in _context.Supervision
                                      join p in _context.Persona on s.PersonaIdPersona equals p.IdPersona
                                      where s.EstadoSupervision == "VIGENTE"
                                      group p by p.Supervisor into grup
                                      select new
                                      {
                                          grup.Key,
                                          Count = grup.Count()
                                      };

            var supervisoresBD = from c in _context.Controlsupervisiones
                                 select new
                                 {
                                     c.Supervisor,
                                     c.Supervisados
                                 };

            var result = (from s in supervisoresScorpio
                          join b in supervisoresBD on s.Key equals b.Supervisor
                          select new
                          {
                              b.Supervisor,
                              Supervisados = s.Count + b.Supervisados
                          }).ToList();

            var recomendar = (((from r in result
                                orderby r.Supervisados ascending
                                select new
                                {
                                    r.Supervisor
                                }).Take(1))).ToArray();

            string recomendacion = (recomendar[0].Supervisor).ToString();

            ViewBag.Recomendacion = recomendacion;



            return View(await _context.Persona.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupervisor(Persona persona)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            int id = persona.IdPersona;
            string supervisor = persona.Supervisor;

            var personaUpdate = await _context.Persona
                .FirstOrDefaultAsync(p => p.IdPersona == id);

            personaUpdate.Supervisor = supervisor;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {

                if (!PersonaExists(persona.IdPersona))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("MenuMCSCP");
        }

        #endregion

        #region -Reasignacion-
        public async Task<IActionResult> Reasignacion(
           string sortOrder,
           string currentFilter,
           string searchString,
           int? pageNumber)
        {

            List<SelectListItem> ListaUsuarios = new List<SelectListItem>();
            int i = 0;
            foreach (var usuario in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(usuario, "SupervisorMCSCP"))
                {
                    ListaUsuarios.Add(new SelectListItem
                    {
                        Text = usuario.ToString(),
                        Value = i.ToString()
                    });
                    i++;
                }
            }
            ViewBag.ListadoUsuarios = ListaUsuarios;

            var queryhayhuella = from r in _context.Registrohuella
                                 join p in _context.Presentacionperiodica on r.IdregistroHuella equals p.RegistroidHuella
                                 group r by r.PersonaIdPersona into grup
                                 select new
                                 {
                                     grup.Key,
                                     Count = grup.Count()
                                 };

            foreach (var personaHuella in queryhayhuella)
            {
                if (personaHuella.Count >= 1)
                {

                    ViewBag.personaIdPersona = personaHuella.Key;
                };
            }


            #region -ListaUsuarios-        
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.Admin = false;
            ViewBag.Masteradmin = false;
            ViewBag.Archivo = false;

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP")
                {
                    ViewBag.Admin = true;
                }
            }
            foreach (var rol in roles)
            {
                if (rol == "Masteradmin")
                {
                    ViewBag.Masteradmin = true;
                }
            }
            foreach (var rol in roles)
            {
                if (rol == "ArchivoMCSCP")
                {
                    ViewBag.Archivo = true;
                }
            }
            #endregion

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
                           where p.Supervisor != null
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                foreach (var item in searchString.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(searchString) ||
                                                   (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(searchString) ||
                                                   p.Supervisor.Contains(searchString) || (p.IdPersona.ToString()).Contains(searchString));

                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
                default:
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
            }

            int pageSize = 10;
            // Response.Headers.Add("Refresh", "5");
            return View(await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSupervisorReasignacion(Persona persona, Reasignacionsupervisor reasignacionsupervisor)
        {
            int id = persona.IdPersona;
            string idS = (persona.IdPersona).ToString();
            string supervisor = persona.Supervisor;
            string motivo = reasignacionsupervisor.MotivoRealizo;
            string currentUser = User.Identity.Name;

            var sA = from p in _context.Persona
                     where p.IdPersona == id
                     select new
                     {
                         p.Supervisor
                     };

            reasignacionsupervisor.PersonaIdpersona = (persona.IdPersona).ToString();
            reasignacionsupervisor.MotivoRealizo = reasignacionsupervisor.MotivoRealizo;
            reasignacionsupervisor.FechaReasignacion = DateTime.Now;
            reasignacionsupervisor.SAntiguo = sA.FirstOrDefault().Supervisor.ToString();
            reasignacionsupervisor.QuienRealizo = currentUser;
            reasignacionsupervisor.SNuevo = supervisor;
            _context.Add(reasignacionsupervisor);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);

            var personaUpdate = await _context.Persona
                .FirstOrDefaultAsync(p => p.IdPersona == id);
            personaUpdate.Supervisor = supervisor;
            var user = await userManager.FindByNameAsync(User.Identity.Name);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {


                if (!PersonaExists(persona.IdPersona))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction("Reasignacion");
        }

        #endregion

        #region -Detalles-

        // GET: Personas/Details/5
        public async Task<IActionResult> Details(int? id, string Vinculacion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona
                .SingleOrDefaultAsync(m => m.IdPersona == id);

            var domicilioEM = await _context.Domicilio.SingleOrDefaultAsync(d => d.PersonaIdPersona == id);


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

            #endregion

            #region -Jointables-
            ViewData["joinTables"] = from personaTable in personaVM
                                     join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                     join estudios in estudiosVM on persona.IdPersona equals estudios.PersonaIdPersona
                                     join trabajo in trabajoVM on persona.IdPersona equals trabajo.PersonaIdPersona
                                     join actividaSocial in actividadSocialVM on persona.IdPersona equals actividaSocial.PersonaIdPersona
                                     join abandonoEstado in abandonoEstadoVM on persona.IdPersona equals abandonoEstado.PersonaIdPersona
                                     join saludFisica in saludFisicaVM on persona.IdPersona equals saludFisica.PersonaIdPersona
                                     //join nacimientoEstado in estados on (Int32.Parse(persona.Lnestado)) equals nacimientoEstado.Id
                                     //join nacimientoMunicipio in municipios on (Int32.Parse(persona.Lnmunicipio)) equals nacimientoMunicipio.Id
                                     join domicilioEstado in estados on (Int32.Parse(domicilio.Estado)) equals domicilioEstado.Id
                                     join domicilioMunicipio in municipios on (Int32.Parse(domicilio.Municipio)) equals domicilioMunicipio.Id
                                     where personaTable.IdPersona == id
                                     select new PersonaViewModel
                                     {
                                         personaVM = personaTable,
                                         domicilioVM = domicilio,
                                         estudiosVM = estudios,
                                         trabajoVM = trabajo,
                                         actividadSocialVM = actividaSocial,
                                         abandonoEstadoVM = abandonoEstado,
                                         saludFisicaVM = saludFisica,
                                         //estadosVMPersona=nacimientoEstado,
                                         //municipiosVMPersona=nacimientoMunicipio,  
                                         estadosVMDomicilio = domicilioEstado,
                                         municipiosVMDomicilio = domicilioMunicipio
                                     };

            #endregion

            #region Sacar el nombre de estdo y municipio (NACIMIENTO)
            var LNE = from e in _context.Estados
                      join p in _context.Persona on e.Id equals int.Parse(p.Lnestado)
                      where p.IdPersona == id
                      select new
                      {
                          e.Estado
                      };

            string selectem1 = LNE.FirstOrDefault().Estado.ToString();
            ViewBag.lnestado = selectem1.ToUpper();

            var LNM = (from p in _context.Persona
                       join m in _context.Municipios on p.Lnmunicipio equals m.Id.ToString()
                       where p.IdPersona == id
                       select new
                       {
                           m.Municipio
                       });

            string selectem2 = LNM.FirstOrDefault().Municipio.ToString();
            ViewBag.lnmunicipio = selectem2.ToUpper();
            #endregion

            #region Sacar el nombre de estdo y municipio (DOMICILIO)
            var E = (from d in _context.Domicilio
                     join m in _context.Estados on int.Parse(d.Estado) equals m.Id
                     join p in _context.Persona on d.PersonaIdPersona equals id
                     select new
                     {
                         m.Estado
                     });

            string selectem3 = E.FirstOrDefault().Estado.ToString();
            ViewBag.estado = selectem3.ToUpper();

            var M = (from d in _context.Domicilio
                     join m in _context.Municipios on int.Parse(d.Municipio) equals m.Id
                     join p in _context.Persona on d.PersonaIdPersona equals id
                     select new
                     {
                         m.Municipio,
                         m.Id
                     });

            string selectem4 = M.FirstOrDefault().Municipio.ToString();
            ViewBag.municipio = selectem4.ToUpper();
            #endregion

            #region Lnmunicipio
            int Lnestado;
            bool success = Int32.TryParse(persona.Lnestado, out Lnestado);
            List<Municipios> listaMunicipios = new List<Municipios>();
            if (success)
            {
                listaMunicipios = (from table in _context.Municipios
                                   where table.EstadosId == Lnestado
                                   select table).ToList();
            }

            listaMunicipios.Insert(0, new Municipios { Id = null, Municipio = "Selecciona" });

            ViewBag.ListadoMunicipios = listaMunicipios;
            ViewBag.idMunicipio = persona.Lnmunicipio;
            #endregion

            #region -JoinTables null-
            ViewData["joinTablesDomSec"] = from personaTable in personaVM
                                           join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                           join domicilioSec in domicilioSecundarioVM.DefaultIfEmpty() on domicilio.IdDomicilio equals domicilioSec.IdDomicilio
                                           where personaTable.IdPersona == id
                                           select new PersonaViewModel
                                           {
                                               domicilioSecundarioVM = domicilioSec
                                           };

            ViewData["joinTablesConsumoSustancias"] = from personaTable in personaVM
                                                      join sustancias in consumoSustanciasVM on persona.IdPersona equals sustancias.PersonaIdPersona
                                                      where personaTable.IdPersona == id
                                                      select new PersonaViewModel
                                                      {
                                                          consumoSustanciasVM = sustancias
                                                      };

            ViewData["joinTablesFamiliaresForaneos"] = from personaTable in personaVM
                                                       join familiarForaneo in familiaresForaneosVM on persona.IdPersona equals familiarForaneo.PersonaIdPersona
                                                       where personaTable.IdPersona == id
                                                       select new PersonaViewModel
                                                       {
                                                           familiaresForaneosVM = familiarForaneo
                                                       };

            ViewData["joinTablesFamiliares"] = from personaTable in personaVM
                                               join familiar in asientoFamiliarVM on persona.IdPersona equals familiar.PersonaIdPersona
                                               where personaTable.IdPersona == id && familiar.Tipo == "FAMILIAR"
                                               select new PersonaViewModel
                                               {
                                                   asientoFamiliarVM = familiar
                                               };

            ViewData["joinTablesReferencia"] = from personaTable in personaVM
                                               join referencia in asientoFamiliarVM on persona.IdPersona equals referencia.PersonaIdPersona
                                               where personaTable.IdPersona == id && referencia.Tipo == "REFERENCIA"
                                               select new PersonaViewModel
                                               {
                                                   asientoFamiliarVM = referencia
                                               };


            ViewBag.Referencia = ((ViewData["joinTablesReferencia"] as IEnumerable<scorpioweb.Models.PersonaViewModel>).Count()).ToString();

            ViewBag.Familiar = ((ViewData["joinTablesFamiliares"] as IEnumerable<scorpioweb.Models.PersonaViewModel>).Count()).ToString();
            #endregion


            if (persona == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);


            //foreach (var rol in roles)
            //{
            //    if(rol == "AdminMCSCP" || rol == "SupervisorMCSCP" || rol == "AuxiliarMCSCP" || rol == "ArchivoMCSCP")
            //    {
            //        ViewBag.Layout = "/Views/Shared/_Layout.cshtml";
            //        break;
            //    }
            //    if (rol == "Vinculacion")
            //    {
            //        ViewBag.ClaseParaDiv = "col-md-9";
            //        break;
            //    }
            //}
            if (!string.IsNullOrEmpty(Vinculacion) && Vinculacion.Contains("1"))
                ViewBag.ClaseParaDiv = "col-md-9";
            else
            {
                ViewBag.Layout = "/Views/Shared/_Layout.cshtml";
            }



            return View();
        }



        #endregion

        #region -Entrevista de encuadre insertar-

        #region -porBORRAR-
        public ActionResult guardarSustancia(string[] datosConsumo)
        {
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosConsumo.Length; i++)
            {
                datosSustancias.Add(new List<String> { datosConsumo[i], currentUser });
            }

            return Json(new { success = true, responseText = "Datos Guardados con éxito" });

        }
        public ActionResult agregarSustancias()
        {
            //por si no se vacian las listas despues de guardar el modal
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosSustancias.Count; i++)
            {
                if (datosSustancias[i][1] == currentUser)
                {
                    datosSustancias.RemoveAt(i);
                    i--;
                }
            }

            return Json(new { success = true });
        }
        public ActionResult guardarFamiliar(string[] datosFamiliar, int tipoGuardado)
        {
            string currentUser = User.Identity.Name;
            if (tipoGuardado == 1)
            {
                for (int i = 0; i < datosFamiliar.Length; i++)
                {
                    datosFamiliares.Add(new List<String> { datosFamiliar[i], currentUser });
                }
            }
            else if (tipoGuardado == 2)
            {
                for (int i = 0; i < datosFamiliar.Length; i++)
                {
                    datosReferencias.Add(new List<String> { datosFamiliar[i], currentUser });
                }
            }


            return Json(new { success = true, responseText = "Datos Guardados con éxito" });

        }
        public ActionResult agregarAsientoFamiliar(int tipo)
        {
            string currentUser = User.Identity.Name;
            if (tipo == 1)
            {
                for (int i = 0; i < datosFamiliares.Count; i++)
                {
                    if (datosFamiliares[i][1] == currentUser)
                    {
                        datosFamiliares.RemoveAt(i);
                        i--;
                    }
                }
            }
            else if (tipo == 2)
            {
                for (int i = 0; i < datosReferencias.Count; i++)
                {
                    if (datosReferencias[i][1] == currentUser)
                    {
                        datosReferencias.RemoveAt(i);
                        i--;
                    }
                }
            }
            return Json(new { success = true });
        }
        public ActionResult guardarDomcililiosecudario(string[] datosDS)
        {
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosDS.Length; i++)
            {
                datosDomiciolioSecundario.Add(new List<String> { datosDS[i], currentUser });
            }

            return Json(new { success = true, responseText = "Datos Guardados con éxito" });

        }
        public ActionResult guardarFamiliarExtranjero(string[] datosFE)
        {
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosFE.Length; i++)
            {
                datosFamiliaresExtranjero.Add(new List<String> { datosFE[i], currentUser });
            }

            return Json(new { success = true, responseText = "Datos Guardados con éxito" });

        }
        #endregion

        public ActionResult agregarAgregardomiciliosecuendario()
        {
            datosDomiciolioSecundario = new List<List<string>>();

            return Json(new { success = true });
        }

        public ActionResult agregarFamiliaresExtranjeros()
        {
            string currentUser = User.Identity.Name;
            for (int i = 0; i < datosFamiliaresExtranjero.Count; i++)
            {
                if (datosFamiliaresExtranjero[i][1] == currentUser)
                {
                    datosFamiliaresExtranjero.RemoveAt(i);
                    i--;
                }
            }

            return Json(new { success = true });
        }

        #region -Estados y Municipios-
        public JsonResult GetMunicipio(int EstadoId)
        {
            TempData["message"] = DateTime.Now;
            List<Municipios> municipiosList = new List<Municipios>();

            municipiosList = (from Municipios in _context.Municipios
                              where Municipios.EstadosId == EstadoId
                              select Municipios).ToList();

            return Json(new SelectList(municipiosList, "Id", "Municipio"));
        }
        public JsonResult GetMunicipioN(int EstadoId)
        {
            TempData["message"] = DateTime.Now;
            List<Municipios> municipiosList = _context.Municipios
                .Where(m => m.EstadosId == EstadoId)
                .ToList();

            return Json(municipiosList);
        }
        public JsonResult GetMunicipioED(int EstadoId)
        {
            TempData["message"] = DateTime.Now;
            List<Municipios> municipiosList = new List<Municipios>();

            if (EstadoId != 0)
            {
                municipiosList = (from Municipios in _context.Municipios
                                  where Municipios.EstadosId == EstadoId
                                  select Municipios).ToList();
            }
            else
            {
                municipiosList.Insert(0, new Municipios { Id = 0, Municipio = "Selecciona" });
            }
            municipiosList.Insert(0, new Municipios { Id = 0, Municipio = "Selecciona" });
            return Json(new SelectList(municipiosList, "Id", "Municipio"));
        }

        //public JsonResult GetZona()
        //{
        //    List<Zonas> zonasList = new List<Zonas>();
        //    zonasList = (from Zonas in _context.Zonas
        //                 select Zonas).ToList();
        //    return Json(new
        //    {
        //        success = true,
        //        zonas = zonasList
        //    });
        //}

        public string generaEstado(string id)
        {
            string estado = "";

            if (id == "0")
            {
                estado = "SIN ESTADO";
            }
            else
            {
                if (!String.IsNullOrEmpty(id))
                {
                    List<Estados> estados = _context.Estados.ToList();
                    estado = (estados.FirstOrDefault(x => x.Id == Int32.Parse(id)).Estado).ToUpper();
                }
            }
            return estado;
        }


        public string generaMunicipio(string id)
        {
            string municipio = "";

            if (id == "0")
            {
                municipio = "SIN MUNICIPIO";
            }
            else
            {
                if (!String.IsNullOrEmpty(id))
                {
                    List<Municipios> municipios = _context.Municipios.ToList();
                    municipio = (municipios.FirstOrDefault(x => x.Id == Int32.Parse(id)).Municipio).ToUpper();
                }
            }
            return municipio;
        }
        #endregion

        #region -CREATE-
        // GET: Personas/Create
        [Authorize(Roles = "AdminMCSCP, SupervisorMCSCP, Masteradmin, Asistente, AuxiliarMCSCP, AdminLC, SupervisorLC")]
        public async Task<IActionResult> Create(Estados Estados)
        {
            ViewBag.centrosPenitenciarios = _context.Centrospenitenciarios.Select(Centrospenitenciarios => Centrospenitenciarios.Nombrecentro).ToList();

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            ViewBag.UserMCYSCP = false;
            ViewBag.UserCL = false;
            ViewBag.user = user;

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP" || rol == "SupervisorMCSCP")
                {
                    ViewBag.UserMCYSCP = true;
                }
                if (rol == "AdminLC" || rol == "SupervisorLC")
                {
                    ViewBag.UserCL = true;
                }
            }

            #region -ASIGNAR SUPERVISOR-
            //// Obtén el último supervisor
            //var ultimoSupervisor = _context.Persona
            //                        .Where(p => p.Supervisor != null && p.Supervisor != "" && p.Supervisor != "enrique.martinez@dgepms.com" && !p.Supervisor.EndsWith("@nortedgepms.com"))
            //                        .OrderByDescending(p => p.IdPersona)
            //                        .Select(p => p.Supervisor)
            //                        .FirstOrDefault();
            //// Obtén la lista de usuarios y ordénala alfabéticamente
            //List<SelectListItem> ListaSuper = new List<SelectListItem>();
            //int j = 0;
            //foreach (var users in userManager.Users)
            //{
            //    if (await userManager.IsInRoleAsync(users, "SupervisorMCSCP"))
            //    {
            //        ListaSuper.Add(new SelectListItem
            //        {
            //            Text = users.ToString(),
            //            Value = j.ToString()
            //        });
            //        j++;
            //    }
            //}

            //// Ordena la lista alfabéticamente por el texto (nombre de usuario)
            //ListaSuper = ListaSuper
            //            .Where(item => item.Text.EndsWith("@dgepms.com") && item.Text != "diana.renteria@dgepms.com" && item.Text != "enrique.martinez@dgepms.com")
            //            .OrderBy(item => item.Text)
            //            .ToList();
            //ViewBag.ListadoUsuarios = ListaSuper;

            //var siguienteSupervisor = ListaSuper.FirstOrDefault(item => string.Compare(item.Text, ultimoSupervisor, true) > 0);

            //if (siguienteSupervisor != null)
            //{
            //    var valorSiguienteSupervisor = siguienteSupervisor.Text;
            //    ViewBag.sigueinteSuperviosor = valorSiguienteSupervisor;
            //}
            //else
            //{
            //    ViewBag.sigueinteSuperviosor = ListaSuper.OrderBy(p => p.Text).Select(p => p.Text).FirstOrDefault();
            //}
            #endregion

            List<Estados> listaEstados = new List<Estados>();
            listaEstados = (from table in _context.Estados
                            select table).ToList();
            ViewBag.ListadoEstados = listaEstados;

            List<Municipios> listaMunicipiosD = new List<Municipios>();
            listaMunicipiosD = (from table in _context.Municipios
                                where table.EstadosId == 10
                                select table).ToList();

            listaMunicipiosD.Insert(0, new Municipios { Id = null, Municipio = "Sin municipio" });
            ViewBag.ListaMunicipios = listaMunicipiosD;

            var colonias = from p in _context.Zonas
                           orderby p.Colonia
                           select p;
            ViewBag.colonias = colonias.ToList();

            ViewBag.coloniaDGEP = "Zona Centro";
            ViewBag.calleDGEP = "Calle Miguel de Cervantes Saavedra";
            ViewBag.noDGEP = "502";

            ViewBag.listaJuzgados = listaJuzgados;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Persona persona, Domicilio domicilio, Estudios estudios, Trabajo trabajo, Actividadsocial actividadsocial, Abandonoestado abandonoEstado, Saludfisica saludfisica, Domiciliosecundario domiciliosecundario, Consumosustancias consumosustanciasBD, Asientofamiliar asientoFamiliar, Familiaresforaneos familiaresForaneos,
                                                Personacl personacl, Domiciliocl domiciliocl, Estudioscl estudioscl, Trabajocl trabajocl, Actividadsocialcl actividadsocialcl, Abandonoestadocl abandonoEstadocl, Saludfisicacl saludfisicacl, Domiciliosecundariocl domiciliosecundariocl, Consumosustanciascl consumosustanciascl, Asientofamiliarcl asientoFamiliarcl, Familiaresforaneoscl familiaresForaneoscl, Expedienteunico expedienteunico,
            string resolucion, string centropenitenciario, string ce, string Juzgado, string sinocentropenitenciario, string nombre, string paterno, string materno, string nombrePadre, string nombreMadre, string alias, string sexo, int edad, DateTime fNacimiento, string lnPais,
            string lnEstado, string CURS, string CURSUsada, string tabla, int idselecionado, string tablanueva, string lnMunicipio, string lnLocalidad, string estadoCivil, string duracion, string otroIdioma, string comIndigena, string comLGBTTTIQ, string especifiqueIdioma,
            string leerEscribir, string traductor, string especifiqueTraductor, string telefonoFijo, string celular, string hijos, int nHijos, int nPersonasVive,
            string propiedades, string CURP, string consumoSustancias, string TratamientoAdicciones, string CualTratamientoAdicciones, string CuandoConsume, string familiares, string referenciasPersonales, string ubicacionExpediente, string colaboracion,
            string tipoDomicilio, string calle, string no, string nombreCF, string paisD, string estadoD, string municipioD, string temporalidad, string zona,
            string residenciaHabitual, int cp, string referencias, string horario, string observaciones, string lat, string lng, string cuentaDomicilioSecundario,
            /*string motivoDS, string tipoDomicilioDS, string calleDS, string noDS, string nombreCFDS, string paisDDS, string estadoDDS, string municipioDDS, string temporalidadDS,*/
            string residenciaHabitualDS, string cpDS, string referenciasDS, string horarioDS, string observacionesDS,
            string estudia, string gradoEstudios, string institucionE, string horarioE, string direccionE, string telefonoE, string observacionesE, string CursoAcademico, string CualCursoAcademico, string DeseaConcluirEstudios,
            string trabaja, string tipoOcupacion, string puesto, string empleadorJefe, string enteradoProceso, string sePuedeEnterar, string tiempoTrabajando, string PropuestaLaboral, string CualPropuesta, string Capacitacion, string CualCapacitacion, string AntesdeCentro, string TrabajoCentro, string CualTrabajoCentro,
            string salario, string temporalidadSalario, string direccionT, string horarioT, string telefonoT, string observacionesT,
            string tipoActividad, string horarioAS, string lugarAS, string telefonoAS, string sePuedeEnterarAS, string referenciaAS, string observacionesAS, string ActividadesDepCulCentro, string CualActividadesDepCulCentro, string DeseaDepCul, string CualDeseaDepCul,
            string vividoFuera, string lugaresVivido, string tiempoVivido, string motivoVivido, string viajaHabitual, string lugaresViaje, string tiempoViaje,
            string motivoViaje, string documentaciónSalirPais, string pasaporte, string visa, string familiaresFuera,
            string enfermedad, string especifiqueEnfermedad, string embarazoLactancia, string tiempoEmbarazo, string tratamiento, string discapacidad, string especifiqueDiscapacidad,
            string servicioMedico, string especifiqueServicioMedico, string institucionServicioMedico, string observacionesSalud, string capturista,
            IFormFile fotografia, string arraySustancias, string arrayFamiliarReferencia, string arrayDomSec, string arrayFamExtranjero, string inputAutocomplete, string idsSeleccionados)
        {

            string currentUser = User.Identity.Name;
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            bool esMCSCP = false;
            bool esCL = false;

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP" || rol == "SupervisorMCSCP" || rol == "AuxiliarMCSCP" || rol == "ArchivoMCSCP")
                {
                    esMCSCP = true;
                }
                if (rol == "AdminLC" || rol == "SupervisorLC")
                {
                    esCL = true;
                }
            }

            if (ModelState.ErrorCount <= 1)
            {
                if (esMCSCP)
                {
                    #region -Persona-
                    persona.Centropenitenciario = mg.removeSpaces(mg.normaliza(centropenitenciario));
                    persona.Sinocentropenitenciario = sinocentropenitenciario;

                    persona.TieneResolucion = mg.normaliza(resolucion);
                    persona.Nombre = mg.removeSpaces(mg.normaliza(nombre));
                    persona.Paterno = mg.removeSpaces(mg.normaliza(paterno));
                    persona.Materno = mg.removeSpaces(mg.normaliza(materno));
                    persona.NombrePadre = mg.normaliza(nombrePadre);
                    persona.NombreMadre = mg.normaliza(nombreMadre);
                    persona.Alias = mg.normaliza(alias);
                    persona.Genero = mg.normaliza(sexo);
                    persona.Edad = edad;
                    persona.Fnacimiento = fNacimiento;
                    persona.Lnpais = lnPais;
                    persona.Lnestado = string.IsNullOrWhiteSpace(lnEstado) ? "0" : mg.normaliza(lnEstado);
                    persona.Lnmunicipio = string.IsNullOrWhiteSpace(lnMunicipio) ? "0" : mg.normaliza(lnMunicipio);
                    persona.Lnlocalidad =string.IsNullOrWhiteSpace(lnLocalidad)? "NA" : mg.normaliza(lnLocalidad);
                    persona.EstadoCivil = estadoCivil;
                    persona.Duracion = duracion;
                    persona.OtroIdioma = mg.normaliza(otroIdioma);
                    persona.ComIndigena = mg.normaliza(comIndigena);
                    persona.ComLgbtttiq = mg.normaliza(comLGBTTTIQ);
                    persona.EspecifiqueIdioma = mg.normaliza(especifiqueIdioma);
                    persona.LeerEscribir = mg.normaliza(leerEscribir);
                    persona.Traductor = mg.normaliza(traductor);
                    persona.EspecifiqueTraductor = mg.normaliza(especifiqueTraductor);
                    persona.TelefonoFijo = telefonoFijo;
                    persona.Celular = celular;
                    persona.Hijos = mg.normaliza(hijos);
                    persona.Nhijos = nHijos;
                    persona.NpersonasVive = nPersonasVive;
                    persona.Propiedades = mg.normaliza(propiedades);
                    persona.Curp = mg.normaliza(CURP);
                    persona.ConsumoSustancias = mg.normaliza(consumoSustancias);
                    persona.Familiares = mg.normaliza(familiares);
                    persona.ReferenciasPersonales = mg.normaliza(referenciasPersonales);
                    persona.UbicacionExpediente = mg.normaliza(ubicacionExpediente);
                    persona.UltimaActualización = DateTime.Now;
                    persona.Capturista = currentUser;
                    persona.Candado = 0;
                    persona.MotivoCandado = "NA";
                    if (CURSUsada != null)
                    {
                        persona.ClaveUnicaScorpio = CURSUsada;
                    }
                    else
                    {
                        persona.ClaveUnicaScorpio = CURS;
                    }


                    var estado = (from e in _context.Estados
                                  where e.Id.ToString() == estadoD
                                  select e.Estado).FirstOrDefault().ToString();
                    var municipio = (from m in _context.Municipios
                                     where m.Id.ToString() == municipioD
                                     select m.Municipio).FirstOrDefault().ToString();
                    persona.Colaboracion = "NO";
                    if (persona.Capturista.EndsWith("\u0040dgepms.com") && estado == "Durango" && (municipio == "Gómez Palacio" || municipio == "Lerdo"))
                    {
                        persona.Colaboracion = "SI";
                        persona.Supervisor = "janeth@nortedgepms.com";
                    }
                    if (persona.Capturista.EndsWith("\u0040nortedgepms.com") && estado == "Durango" && municipio == "Durango")
                    {
                        persona.Colaboracion = "SI";
                        persona.Supervisor = "isabel.almora@dgepms.com";
                    }
                    #endregion

                    #region -Domicilio-
                    domicilio.TipoDomicilio = tipoDomicilio;
                    domicilio.Calle = mg.normaliza(calle);
                    domicilio.No = String.IsNullOrEmpty(no) ? no : no.ToUpper();
                    domicilio.Pais = paisD;
                    domicilio.Estado = estadoD;
                    domicilio.Municipio = municipioD;
                    domicilio.Temporalidad = temporalidad;
                    domicilio.ResidenciaHabitual = mg.normaliza(residenciaHabitual);
                    domicilio.Cp = cp;
                    domicilio.Referencias = mg.normaliza(referencias);
                    domicilio.DomcilioSecundario = cuentaDomicilioSecundario;
                    domicilio.Horario = mg.normaliza(horario);
                    domicilio.Observaciones = mg.normaliza(observaciones);
                    domicilio.Lat = lat;
                    domicilio.Lng = lng;

                    domicilio.NombreCf = mg.normaliza(inputAutocomplete);

                    List<Zonas> zonasList = new List<Zonas>();
                    zonasList = (from Zonas in _context.Zonas
                                 select Zonas).ToList();

                    domicilio.Zona = "SIN ZONA ASIGNADA";
                    int matches = 0;
                    for (int i = 0; i < zonasList.Count; i++)
                    {
                        if (zonasList[i].Colonia.ToUpper() == domicilio.NombreCf)
                        {
                            matches++;
                        }
                    }
                    for (int i = 0; i < zonasList.Count; i++)
                    {
                        if (zonasList[i].Colonia.ToUpper() == domicilio.NombreCf && (matches <= 1 || zonasList[i].Cp == domicilio.Cp))
                        {
                            domicilio.Zona = zonasList[i].Zona.ToUpper();
                        }
                    }
                    #endregion

                    #region -Domicilio Secundario-   
                    /*domiciliosecundario.Motivo = motivoDS;
                    domiciliosecundario.TipoDomicilio = tipoDomicilioDS;
                    domiciliosecundario.Calle = mg.normaliza(calleDS);
                    domiciliosecundario.No = mg.normaliza(noDS);
                    domiciliosecundario.NombreCf = mg.normaliza(nombreCFDS);
                    domiciliosecundario.Pais = paisDDS;
                    domiciliosecundario.Estado = estadoDDS;
                    domiciliosecundario.Municipio = municipioDDS;
                    domiciliosecundario.Temporalidad = temporalidadDS;
                    domiciliosecundario.ResidenciaHabitual = mg.normaliza(residenciaHabitualDS);
                    domiciliosecundario.Cp = mg.normaliza(cpDS);
                    domiciliosecundario.Referencias = mg.normaliza(referenciasDS);
                    domiciliosecundario.Horario = mg.normaliza(horarioDS);
                    domiciliosecundario.Observaciones = mg.normaliza(observacionesDS);*/
                    #endregion

                    #region -Estudios-
                    estudios.Estudia = estudia;
                    estudios.GradoEstudios = gradoEstudios;
                    estudios.InstitucionE = mg.normaliza(institucionE);
                    estudios.Horario = mg.normaliza(horarioE);
                    estudios.Direccion = mg.normaliza(direccionE);
                    estudios.Telefono = telefonoE;
                    estudios.Observaciones = mg.normaliza(observacionesE);
                    #endregion

                    #region -Trabajo-
                    trabajo.Trabaja = trabaja;
                    trabajo.TipoOcupacion = tipoOcupacion;
                    trabajo.Puesto = mg.normaliza(puesto);
                    trabajo.EmpledorJefe = mg.normaliza(empleadorJefe);
                    trabajo.EnteradoProceso = enteradoProceso;
                    trabajo.SePuedeEnterar = sePuedeEnterar;
                    trabajo.TiempoTrabajano = tiempoTrabajando;
                    trabajo.Salario = mg.normaliza(salario);
                    trabajo.Direccion = mg.normaliza(direccionT);
                    trabajo.Horario = mg.normaliza(horarioT);
                    trabajo.Telefono = telefonoT;
                    trabajo.Observaciones = mg.normaliza(observacionesT);
                    #endregion

                    #region -ActividadSocial-
                    actividadsocial.TipoActividad = mg.normaliza(tipoActividad);
                    actividadsocial.Horario = mg.normaliza(horarioAS);
                    actividadsocial.Lugar = mg.normaliza(lugarAS);
                    actividadsocial.Telefono = telefonoAS;
                    actividadsocial.SePuedeEnterar = sePuedeEnterarAS;
                    actividadsocial.Referencia = mg.normaliza(referenciaAS);
                    actividadsocial.Observaciones = mg.normaliza(observacionesAS);
                    #endregion

                    #region -AbandonoEstado-
                    abandonoEstado.VividoFuera = vividoFuera;
                    abandonoEstado.LugaresVivido = mg.normaliza(lugaresVivido);
                    abandonoEstado.TiempoVivido = mg.normaliza(tiempoVivido);
                    abandonoEstado.MotivoVivido = mg.normaliza(motivoVivido);
                    abandonoEstado.ViajaHabitual = viajaHabitual;
                    abandonoEstado.LugaresViaje = mg.normaliza(lugaresViaje);
                    abandonoEstado.TiempoViaje = mg.normaliza(tiempoViaje);
                    abandonoEstado.MotivoViaje = mg.normaliza(motivoViaje);
                    abandonoEstado.DocumentacionSalirPais = documentaciónSalirPais;
                    abandonoEstado.Pasaporte = pasaporte;
                    abandonoEstado.Visa = visa;
                    abandonoEstado.FamiliaresFuera = familiaresFuera;
                    //abandonoEstado.Cuantos = cuantosFamiliares;
                    #endregion

                    #region -Salud-
                    saludfisica.Enfermedad = enfermedad;
                    saludfisica.EspecifiqueEnfermedad = mg.normaliza(especifiqueEnfermedad);
                    saludfisica.EmbarazoLactancia = embarazoLactancia;
                    saludfisica.Tiempo = mg.normaliza(tiempoEmbarazo);
                    saludfisica.Tratamiento = mg.normaliza(tratamiento);
                    saludfisica.Discapacidad = discapacidad;
                    saludfisica.EspecifiqueDiscapacidad = mg.normaliza(especifiqueDiscapacidad);
                    saludfisica.ServicioMedico = servicioMedico;
                    saludfisica.EspecifiqueServicioMedico = especifiqueServicioMedico;
                    saludfisica.InstitucionServicioMedico = institucionServicioMedico;
                    saludfisica.Observaciones = mg.normaliza(observacionesSalud);
                    #endregion

                    #region -IdDomicilio-  
                    int idDomicilio = ((from table in _context.Domicilio
                                        select table.IdDomicilio).Max()) + 1;
                    domicilio.IdDomicilio = idDomicilio;
                    //domiciliosecundario.IdDomicilio = idDomicilio;
                    #endregion

                    #region -IdPersona-
                    int idPersona = ((from table in _context.Persona
                                      select table.IdPersona).Max()) + 1;


                    persona.IdPersona = idPersona;
                    domicilio.PersonaIdPersona = idPersona;
                    estudios.PersonaIdPersona = idPersona;
                    trabajo.PersonaIdPersona = idPersona;
                    actividadsocial.PersonaIdPersona = idPersona;
                    abandonoEstado.PersonaIdPersona = idPersona;
                    saludfisica.PersonaIdPersona = idPersona;

                    #endregion

                    #region -ConsumoSustancias-
                    if (arraySustancias != null)
                    {
                        JArray sustancias = JArray.Parse(arraySustancias);

                        for (int i = 0; i < sustancias.Count; i = i + 5)
                        {
                            consumosustanciasBD.Sustancia = sustancias[i].ToString();
                            consumosustanciasBD.Frecuencia = sustancias[i + 1].ToString();
                            consumosustanciasBD.Cantidad = mg.normaliza(sustancias[i + 2].ToString());
                            consumosustanciasBD.UltimoConsumo = mg.validateDatetime(sustancias[i + 3].ToString());
                            consumosustanciasBD.Observaciones = mg.normaliza(sustancias[i + 4].ToString());
                            consumosustanciasBD.PersonaIdPersona = idPersona;
                            _context.Add(consumosustanciasBD);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                        }
                    }
                    /*for (int i = 0; i < datosSustancias.Count; i++)
                    {
                        if (datosSustancias[i][1] == currentUser)
                        {
                            datosSustancias.RemoveAt(i);
                            i--;
                        }
                    }*/
                    #endregion

                    #region -FamiliarReferencia-
                    if (arrayFamiliarReferencia != null)
                    {
                        JArray familiarReferencia = JArray.Parse(arrayFamiliarReferencia);
                        for (int i = 0; i < familiarReferencia.Count; i = i + 14)
                        {
                            asientoFamiliar.Nombre = mg.normaliza(familiarReferencia[i].ToString());
                            asientoFamiliar.Relacion = familiarReferencia[i + 1].ToString();
                            try
                            {
                                asientoFamiliar.Edad = Int32.Parse(familiarReferencia[i + 2].ToString());
                            }
                            catch
                            {
                                asientoFamiliar.Edad = 0;
                            }
                            asientoFamiliar.Sexo = familiarReferencia[i + 3].ToString();
                            asientoFamiliar.Dependencia = familiarReferencia[i + 4].ToString();
                            asientoFamiliar.DependenciaExplica = mg.normaliza(familiarReferencia[i + 5].ToString());
                            asientoFamiliar.VivenJuntos = familiarReferencia[i + 6].ToString();
                            asientoFamiliar.Domicilio = mg.normaliza(familiarReferencia[i + 7].ToString());
                            asientoFamiliar.Telefono = familiarReferencia[i + 8].ToString();
                            asientoFamiliar.HorarioLocalizacion = mg.normaliza(familiarReferencia[i + 9].ToString());
                            asientoFamiliar.EnteradoProceso = familiarReferencia[i + 10].ToString();
                            asientoFamiliar.PuedeEnterarse = familiarReferencia[i + 11].ToString();
                            asientoFamiliar.Observaciones = mg.normaliza(familiarReferencia[i + 12].ToString());
                            asientoFamiliar.Tipo = familiarReferencia[i + 13].ToString();
                            asientoFamiliar.PersonaIdPersona = idPersona;
                            _context.Add(asientoFamiliar);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);

                        }
                    }
                    /*for (int i = 0; i < datosFamiliares.Count; i++)
                    {
                        if (datosFamiliares[i][1] == currentUser)
                        {
                            datosFamiliares.RemoveAt(i);
                            i--;
                        }
                    }*/
                    #endregion

                    #region -Domicilio Secundario-
                    if (arrayDomSec != null)
                    {
                        JArray domSec = JArray.Parse(arrayDomSec);
                        for (int i = 0; i < domSec.Count; i = i + 14)
                        {
                            domiciliosecundario.Motivo = mg.normaliza(domSec[i].ToString());
                            domiciliosecundario.TipoDomicilio = domSec[i + 1].ToString();
                            domiciliosecundario.Calle = mg.normaliza(domSec[i + 2].ToString());
                            domiciliosecundario.No = mg.normaliza(domSec[i + 3].ToString());
                            domiciliosecundario.NombreCf = mg.normaliza(domSec[i + 4].ToString());
                            domiciliosecundario.Pais = domSec[i + 5].ToString();
                            domiciliosecundario.Estado = mg.normaliza(domSec[i + 6].ToString());
                            domiciliosecundario.Municipio = domSec[i + 7].ToString();
                            domiciliosecundario.Temporalidad = domSec[i + 8].ToString();
                            domiciliosecundario.ResidenciaHabitual = domSec[i + 9].ToString();
                            domiciliosecundario.Cp = domSec[i + 10].ToString();
                            domiciliosecundario.Referencias = mg.normaliza(domSec[i + 11].ToString());
                            domiciliosecundario.Horario = mg.normaliza(domSec[i + 12].ToString());
                            domiciliosecundario.Observaciones = mg.normaliza(domSec[i + 13].ToString());
                            domiciliosecundario.IdDomicilio = idDomicilio;
                            _context.Add(domiciliosecundario);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                        }
                    }


                    /*for (int i = 0; i < datosDomiciolioSecundario.Count; i++)
                    {
                        if (datosDomiciolioSecundario[i][1] == currentUser)
                        {
                            datosDomiciolioSecundario.RemoveAt(i);
                            i--;
                        }
                    }*/
                    #endregion

                    #region -Familiares Extranjero-
                    if (arrayFamExtranjero != null)
                    {
                        JArray famExtranjero = JArray.Parse(arrayFamExtranjero);
                        for (int i = 0; i < famExtranjero.Count; i = i + 12)
                        {
                            familiaresForaneos.Nombre = mg.normaliza(famExtranjero[i].ToString());
                            familiaresForaneos.Relacion = famExtranjero[i + 1].ToString();
                            try
                            {
                                familiaresForaneos.Edad = Int32.Parse(famExtranjero[i + 2].ToString());
                            }
                            catch
                            {
                                familiaresForaneos.Edad = 0;
                            }
                            familiaresForaneos.Sexo = famExtranjero[i + 3].ToString();
                            familiaresForaneos.TiempoConocerlo = famExtranjero[i + 4].ToString();
                            familiaresForaneos.Pais = famExtranjero[i + 5].ToString();
                            familiaresForaneos.Estado = mg.normaliza(famExtranjero[i + 6].ToString());
                            familiaresForaneos.Telefono = famExtranjero[i + 7].ToString();
                            familiaresForaneos.FrecuenciaContacto = famExtranjero[i + 8].ToString();
                            familiaresForaneos.EnteradoProceso = famExtranjero[i + 9].ToString();
                            familiaresForaneos.PuedeEnterarse = famExtranjero[i + 10].ToString();
                            familiaresForaneos.Observaciones = mg.normaliza(famExtranjero[i + 11].ToString());
                            familiaresForaneos.PersonaIdPersona = idPersona;
                            _context.Add(familiaresForaneos);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                        }
                    }
                    /*for (int i = 0; i < datosFamiliaresExtranjero.Count; i++)
                    {
                        if (datosFamiliaresExtranjero[i][1] == currentUser)
                        {
                            datosFamiliaresExtranjero.RemoveAt(i);
                            i--;
                        }
                    }*/
                    #endregion

                    #region -Guardar Foto-
                    if (fotografia != null)
                    {
                        string file_name = persona.IdPersona + "_" + persona.Paterno + "_" + persona.Nombre + ".jpg";
                        file_name = mg.replaceSlashes(file_name);
                        persona.rutaFoto = file_name;
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Fotos");
                        var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                        await fotografia.CopyToAsync(stream);
                        stream.Close();
                    }
                    #endregion

                    // SE USABA PARA ASIGNAR UN SUPERVISADADO DE MANERA AUTOMATICA (DESDE EL PRINCIPIO PENSE QUE NO SERIA RENTBLE )
                    //#region -ASIGNAR SUPERVISOR-
                    ////Obtén el último supervisor
                    //if (User.Identity.Name.EndsWith("@dgepms.com"))
                    //{
                    //    var ultimoSupervisor = _context.Persona
                    //                        .Where(p => p.Supervisor != null && p.Supervisor != "" && p.Supervisor != "enrique.martinez@dgepms.com" && !p.Supervisor.EndsWith("@nortedgepms.com"))
                    //                        .OrderByDescending(p => p.IdPersona)
                    //                        .Select(p => p.Supervisor)
                    //                        .FirstOrDefault();

                    //    // Obtén la lista de usuarios y ordénala alfabéticamente
                    //    List<SelectListItem> ListaSuper = new List<SelectListItem>();
                    //    int j = 0;
                    //    foreach (var users in userManager.Users)
                    //    {
                    //        if (await userManager.IsInRoleAsync(users, "SupervisorMCSCP"))
                    //        {
                    //            ListaSuper.Add(new SelectListItem
                    //            {
                    //                Text = users.ToString(),
                    //                Value = j.ToString()
                    //            });
                    //            j++;
                    //        }
                    //    }

                    //    // Ordena la lista alfabéticamente por el texto (nombre de usuario)
                    //    ListaSuper = ListaSuper
                    //                 .Where(item => item.Text.EndsWith("@dgepms.com") && item.Text != "diana.renteria@dgepms.com" && item.Text != "enrique.martinez@dgepms.com").OrderBy(item => item.Text)
                    //                .ToList();
                    //    ViewBag.ListadoUsuarios = ListaSuper;

                    //    var siguienteSupervisor = ListaSuper.FirstOrDefault(item => string.Compare(item.Text, ultimoSupervisor, true) > 0);

                    //    if (siguienteSupervisor != null)
                    //    {
                    //        if (municipioD != "283" && municipioD != "282" && municipioD != "303")
                    //        {
                    //            persona.Supervisor = "enrique.martinez@dgepms.com";
                    //        }
                    //        else
                    //        {
                    //            var valorSiguienteSupervisor = siguienteSupervisor.Text;
                    //            persona.Supervisor = valorSiguienteSupervisor;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (municipioD != "283" && municipioD != "282" && municipioD != "303")
                    //        {
                    //            persona.Supervisor = "enrique.martinez@dgepms.com";
                    //        }
                    //        else
                    //        {
                    //            persona.Supervisor = ListaSuper.OrderBy(p => p.Text).Select(p => p.Text).FirstOrDefault();
                    //        }
                    //    }
                    //}
                    //#endregion

                    #region -Expediente Unico-
                    //TODO QUEDO EN API =)
                    #endregion

                    #region -Añadir a contexto-

                    _context.Add(persona);
                    _context.Add(domicilio);
                    // _context.Add(domiciliosecundario);
                    _context.Add(estudios);
                    _context.Add(trabajo);
                    _context.Add(actividadsocial);
                    _context.Add(abandonoEstado);
                    _context.Add(saludfisica);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);

                    return RedirectToAction("RegistroConfirmation/" + persona.IdPersona, "Personas");
                    #endregion

                }
                if (esCL = true)
                {

                    #region -Persona-
                    personacl.Centropenitenciario = mg.removeSpaces(mg.normaliza(centropenitenciario));
                    personacl.Sinocentropenitenciario = sinocentropenitenciario;
                    personacl.Juzgado = Juzgado;
                    personacl.TieneResolucion = mg.normaliza(resolucion);
                    personacl.Nombre = mg.removeSpaces(mg.normaliza(nombre));
                    personacl.Paterno = mg.removeSpaces(mg.normaliza(paterno));
                    personacl.Materno = mg.removeSpaces(mg.normaliza(materno));
                    personacl.NombrePadre = mg.normaliza(nombrePadre);
                    personacl.NombreMadre = mg.normaliza(nombreMadre);
                    personacl.Alias = mg.normaliza(alias);
                    personacl.Genero = mg.normaliza(sexo);
                    personacl.Edad = edad;
                    personacl.Fnacimiento = fNacimiento;
                    personacl.Lnpais = lnPais;
                    personacl.Lnestado = string.IsNullOrWhiteSpace(lnEstado) ? "0" : mg.normaliza(lnEstado);
                    personacl.Lnmunicipio = string.IsNullOrWhiteSpace(lnMunicipio) ? "0" : mg.normaliza(lnMunicipio);
                    personacl.Lnlocalidad = string.IsNullOrWhiteSpace(lnLocalidad) ? "NA" : mg.normaliza(lnLocalidad);
                    personacl.EstadoCivil = estadoCivil;
                    personacl.Duracion = mg.normaliza(duracion);
                    personacl.OtroIdioma = mg.normaliza(otroIdioma);
                    personacl.ComIndigena = mg.normaliza(comIndigena);
                    personacl.ComLgbtttiq = mg.normaliza(comLGBTTTIQ);
                    personacl.EspecifiqueIdioma = mg.normaliza(especifiqueIdioma);
                    personacl.LeerEscribir = mg.normaliza(leerEscribir);
                    personacl.Traductor = mg.normaliza(traductor);
                    personacl.EspecifiqueTraductor = mg.normaliza(especifiqueTraductor);
                    personacl.TelefonoFijo = telefonoFijo;
                    personacl.Celular = celular;
                    personacl.Hijos = mg.normaliza(hijos);
                    personacl.Nhijos = nHijos;
                    personacl.NpersonasVive = nPersonasVive;
                    personacl.Propiedades = mg.normaliza(propiedades);
                    personacl.Curp = mg.normaliza(CURP);
                    personacl.ConsumoSustancias = mg.normaliza(consumoSustancias);
                    personacl.TratamientoAdicciones = mg.normaliza(TratamientoAdicciones);
                    personacl.CualTratamientoAdicciones = mg.normaliza(CualTratamientoAdicciones);
                    personacl.Familiares = mg.normaliza(familiares);
                    personacl.ReferenciasPersonales = mg.normaliza(referenciasPersonales);
                    personacl.UbicacionExpediente = mg.normaliza(ubicacionExpediente);
                    personacl.UltimaActualización = DateTime.Now;
                    personacl.Capturista = currentUser;
                    personacl.Candado = 0;
                    personacl.MotivoCandado = "NA";
                    personacl.Centropenitenciario = mg.normaliza(centropenitenciario);
                    personacl.CuandoConsume = mg.normaliza(CuandoConsume);
                    personacl.Sinocentropenitenciario = sinocentropenitenciario;
                    personacl.Colaboracion = mg.normaliza(colaboracion);
                    if (CURSUsada != null)
                    {
                        personacl.ClaveUnicaScorpio = CURSUsada;
                    }
                    else
                    {
                        personacl.ClaveUnicaScorpio = CURS;
                    }
                    personacl.Ruta = 0;
                    personacl.Ce = ce;

                    var estado = (from e in _context.Estados
                                  where e.Id.ToString() == estadoD
                                  select e.Estado).FirstOrDefault().ToString();
                    var municipio = (from m in _context.Municipios
                                     where m.Id.ToString() == municipioD
                                     select m.Municipio).FirstOrDefault().ToString();

                    //if (personacl.Capturista.EndsWith("\u0040dgepms.com") && estado == "Durango" && (municipio == "Gómez Palacio" || municipio == "Lerdo"))
                    //{
                    //    personacl.Colaboracion = "SI";
                    //    personacl.Supervisor = "janeth@nortedgepms.com";
                    //}
                    //if (personacl.Capturista.EndsWith("\u0040nortedgepms.com") && estado == "Durango" && municipio == "Durango")
                    //{
                    //    personacl.Colaboracion = "SI";
                    //    personacl.Supervisor = "isabel.almora@dgepms.com";
                    //}
                    #endregion

                    #region -Domicilio-
                    domiciliocl.TipoDomicilio = tipoDomicilio;
                    domiciliocl.Calle = mg.normaliza(calle);
                    domiciliocl.No = String.IsNullOrEmpty(no) ? no : no.ToUpper();
                    domiciliocl.Pais = paisD;
                    domiciliocl.Estado = estadoD;
                    domiciliocl.Municipio = municipioD;
                    domiciliocl.Temporalidad = temporalidad;
                    domiciliocl.ResidenciaHabitual = mg.normaliza(residenciaHabitual);
                    domiciliocl.Cp = cp;
                    domiciliocl.Referencias = mg.normaliza(referencias);
                    domiciliocl.DomcilioSecundario = cuentaDomicilioSecundario;
                    domiciliocl.Horario = mg.normaliza(horario);
                    domiciliocl.Observaciones = mg.normaliza(observaciones);
                    domiciliocl.NombreCf = mg.normaliza(inputAutocomplete);

                    List<Zonas> zonasList = new List<Zonas>();
                    zonasList = (from Zonas in _context.Zonas
                                 select Zonas).ToList();

                    domiciliocl.Zona = "SIN ZONA ASIGNADA";
                    int matches = 0;
                    for (int i = 0; i < zonasList.Count; i++)
                    {
                        if (zonasList[i].Colonia.ToUpper() == domiciliocl.NombreCf)
                        {
                            matches++;
                        }
                    }
                    for (int i = 0; i < zonasList.Count; i++)
                    {
                        if (zonasList[i].Colonia.ToUpper() == domiciliocl.NombreCf && (matches <= 1 || zonasList[i].Cp == domiciliocl.Cp))
                        {
                            domiciliocl.Zona = zonasList[i].Zona.ToUpper();
                        }
                    }
                    #endregion

                    #region -Domicilio Secundario-   
                    /*domiciliosecundariocl.Motivo = motivoDS;
                    domiciliosecundariocl.TipoDomicilio = tipoDomicilioDS;
                    domiciliosecundariocl.Calle = mg.normaliza(calleDS);
                    domiciliosecundariocl.No = mg.normaliza(noDS);
                    domiciliosecundariocl.NombreCf = mg.normaliza(nombreCFDS);
                    domiciliosecundariocl.Pais = paisDDS;
                    domiciliosecundariocl.Estado = estadoDDS;
                    domiciliosecundariocl.Municipio = municipioDDS;
                    domiciliosecundariocl.Temporalidad = temporalidadDS;
                    domiciliosecundariocl.ResidenciaHabitual = mg.normaliza(residenciaHabitualDS);
                    domiciliosecundariocl.Cp = mg.normaliza(cpDS);
                    domiciliosecundariocl.Referencias = mg.normaliza(referenciasDS);
                    domiciliosecundariocl.Horario = mg.normaliza(horarioDS);
                    domiciliosecundariocl.Observaciones = mg.normaliza(observacionesDS);*/
                    #endregion

                    #region -Estudios-
                    estudioscl.Estudia = estudia;
                    estudioscl.GradoEstudios = gradoEstudios;
                    estudioscl.InstitucionE = mg.normaliza(institucionE);
                    estudioscl.Horario = mg.normaliza(horarioE);
                    estudioscl.Direccion = mg.normaliza(direccionE);
                    estudioscl.Telefono = telefonoE;
                    estudioscl.Observaciones = mg.normaliza(observacionesE);
                    estudioscl.CualCursoAcademico = mg.normaliza(CualCursoAcademico);
                    estudioscl.DeseaConcluirEstudios = mg.normaliza(DeseaConcluirEstudios);
                    estudioscl.CursoAcademico = mg.normaliza(CursoAcademico);

                    #endregion

                    #region -Trabajo-
                    trabajocl.Trabaja = trabaja;
                    trabajocl.TipoOcupacion = tipoOcupacion;
                    trabajocl.Puesto = mg.normaliza(puesto);
                    trabajocl.EmpledorJefe = mg.normaliza(empleadorJefe);
                    trabajocl.EnteradoProceso = enteradoProceso;
                    trabajocl.SePuedeEnterar = sePuedeEnterar;
                    trabajocl.TiempoTrabajano = tiempoTrabajando;
                    trabajocl.Salario = mg.normaliza(salario);
                    trabajocl.Direccion = mg.normaliza(direccionT);
                    trabajocl.Horario = mg.normaliza(horarioT);
                    trabajocl.Telefono = telefonoT;
                    trabajocl.Observaciones = mg.normaliza(observacionesT);
                    trabajocl.PropuestaLaboral = mg.normaliza(PropuestaLaboral);
                    trabajocl.CualPropuesta = mg.normaliza(CualPropuesta);
                    trabajocl.Capacitacion = mg.normaliza(Capacitacion);
                    trabajocl.CualCapacitacion = mg.normaliza(CualCapacitacion);
                    trabajocl.AntesdeCentro = mg.normaliza(AntesdeCentro);
                    trabajocl.TrabajoCentro = mg.normaliza(TrabajoCentro);
                    trabajocl.CualTrabajoCentro = mg.normaliza(CualTrabajoCentro);
                    #endregion

                    #region -ActividadSocial-
                    actividadsocialcl.TipoActividad = mg.normaliza(tipoActividad);
                    actividadsocialcl.Horario = mg.normaliza(horarioAS);
                    actividadsocialcl.Lugar = mg.normaliza(lugarAS);
                    actividadsocialcl.Telefono = telefonoAS;
                    actividadsocialcl.SePuedeEnterar = sePuedeEnterarAS;
                    actividadsocialcl.Referencia = mg.normaliza(referenciaAS);
                    actividadsocialcl.Observaciones = mg.normaliza(observacionesAS);
                    actividadsocialcl.ActividadesDepCulCentro = mg.normaliza(ActividadesDepCulCentro);
                    actividadsocialcl.CualActividadesDepCulCentro = mg.normaliza(CualActividadesDepCulCentro);
                    actividadsocialcl.DeseaDepCul = mg.normaliza(DeseaDepCul);
                    actividadsocialcl.CualDeseaDepCul = mg.normaliza(CualDeseaDepCul);
                    #endregion

                    #region -AbandonoEstado-
                    abandonoEstadocl.VividoFuera = vividoFuera;
                    abandonoEstadocl.LugaresVivido = mg.normaliza(lugaresVivido);
                    abandonoEstadocl.TiempoVivido = mg.normaliza(tiempoVivido);
                    abandonoEstadocl.MotivoVivido = mg.normaliza(motivoVivido);
                    abandonoEstadocl.ViajaHabitual = viajaHabitual;
                    abandonoEstadocl.LugaresViaje = mg.normaliza(lugaresViaje);
                    abandonoEstadocl.TiempoViaje = mg.normaliza(tiempoViaje);
                    abandonoEstadocl.MotivoViaje = mg.normaliza(motivoViaje);
                    abandonoEstadocl.DocumentacionSalirPais = documentaciónSalirPais;
                    abandonoEstadocl.Pasaporte = pasaporte;
                    abandonoEstadocl.Visa = visa;
                    abandonoEstadocl.FamiliaresFuera = familiaresFuera;
                    #endregion

                    #region -Salud-
                    saludfisicacl.Enfermedad = enfermedad;
                    saludfisicacl.EspecifiqueEnfermedad = mg.normaliza(especifiqueEnfermedad);
                    saludfisicacl.EmbarazoLactancia = embarazoLactancia;
                    saludfisicacl.Tiempo = mg.normaliza(tiempoEmbarazo);
                    saludfisicacl.Tratamiento = mg.normaliza(tratamiento);
                    saludfisicacl.Discapacidad = discapacidad;
                    saludfisicacl.EspecifiqueDiscapacidad = mg.normaliza(especifiqueDiscapacidad);
                    saludfisicacl.ServicioMedico = servicioMedico;
                    saludfisicacl.EspecifiqueServicioMedico = especifiqueServicioMedico;
                    saludfisicacl.InstitucionServicioMedico = institucionServicioMedico;
                    saludfisicacl.Observaciones = mg.normaliza(observacionesSalud);
                    #endregion

                    #region -IdDomicilio-  
                    int idDomiciliocl = ((from table in _context.Domiciliocl
                                          select table.IdDomiciliocl).Max()) + 1;
                    // int idDomiciliocl = _context.Domiciliocl.Select(item => (int?)item.IdDomicilioCl).Max() ?? 0 + 1;
                    domiciliocl.IdDomiciliocl = idDomiciliocl;
                    //domiciliosecundario.IdDomicilio = idDomicilio;
                    #endregion

                    #region -IdPersona-
                    //int idPersonacl = _context.Personacl.Select(item => (int?)item.IdPersonaCl).Max() ?? 0 + 1;
                    int idPersonacl = ((from table in _context.Personacl
                                        select table.IdPersonaCl).Max()) + 1;

                    personacl.IdPersonaCl = idPersonacl;
                    domiciliocl.PersonaclIdPersonacl = idPersonacl;
                    estudioscl.PersonaClIdPersonaCl = idPersonacl;
                    trabajocl.PersonaClIdPersonaCl = idPersonacl;
                    actividadsocialcl.PersonaClIdPersonaCl = idPersonacl;
                    abandonoEstadocl.PersonaclIdPersonacl = idPersonacl;
                    saludfisicacl.PersonaClIdPersonaCl = idPersonacl;

                    #endregion

                    #region -ConsumoSustancias-
                    if (arraySustancias != null)
                    {
                        JArray sustancias = JArray.Parse(arraySustancias);

                        for (int i = 0; i < sustancias.Count; i = i + 5)
                        {
                            consumosustanciascl.Sustancia = sustancias[i].ToString();
                            consumosustanciascl.Frecuencia = sustancias[i + 1].ToString();
                            consumosustanciascl.Cantidad = mg.normaliza(sustancias[i + 2].ToString());
                            consumosustanciascl.UltimoConsumo = mg.validateDatetime(sustancias[i + 3].ToString());
                            consumosustanciascl.Observaciones = mg.normaliza(sustancias[i + 4].ToString());
                            consumosustanciascl.PersonaClIdPersonaCl = idPersonacl;
                            _context.Add(consumosustanciascl);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                        }
                    }
                    /*for (int i = 0; i < datosSustancias.Count; i++)
                    {
                        if (datosSustancias[i][1] == currentUser)
                        {
                            datosSustancias.RemoveAt(i);
                            i--;
                        }
                    }*/
                    #endregion

                    #region -FamiliarReferencia-
                    if (arrayFamiliarReferencia != null)
                    {
                        JArray familiarReferencia = JArray.Parse(arrayFamiliarReferencia);
                        for (int i = 0; i < familiarReferencia.Count; i = i + 14)
                        {
                            asientoFamiliarcl.Nombre = mg.normaliza(familiarReferencia[i].ToString());
                            asientoFamiliarcl.Relacion = familiarReferencia[i + 1].ToString();
                            try
                            {
                                asientoFamiliarcl.Edad = Int32.Parse(familiarReferencia[i + 2].ToString());
                            }
                            catch
                            {
                                asientoFamiliarcl.Edad = 0;
                            }
                            asientoFamiliarcl.Sexo = familiarReferencia[i + 3].ToString();
                            asientoFamiliarcl.Dependencia = familiarReferencia[i + 4].ToString();
                            asientoFamiliarcl.DependenciaExplica = mg.normaliza(familiarReferencia[i + 5].ToString());
                            asientoFamiliarcl.VivenJuntos = familiarReferencia[i + 6].ToString();
                            asientoFamiliarcl.Domicilio = mg.normaliza(familiarReferencia[i + 7].ToString());
                            asientoFamiliarcl.Telefono = familiarReferencia[i + 8].ToString();
                            asientoFamiliarcl.HorarioLocalizacion = mg.normaliza(familiarReferencia[i + 9].ToString());
                            asientoFamiliarcl.EnteradoProceso = familiarReferencia[i + 10].ToString();
                            asientoFamiliarcl.PuedeEnterarse = familiarReferencia[i + 11].ToString();
                            asientoFamiliarcl.Observaciones = mg.normaliza(familiarReferencia[i + 12].ToString());
                            asientoFamiliarcl.Tipo = familiarReferencia[i + 13].ToString();
                            asientoFamiliarcl.PersonaClIdPersonaCl = idPersonacl;
                            _context.Add(asientoFamiliarcl);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);

                        }
                    }
                    /*for (int i = 0; i < datosFamiliares.Count; i++)
                    {
                        if (datosFamiliares[i][1] == currentUser)
                        {
                            datosFamiliares.RemoveAt(i);
                            i--;
                        }
                    }*/
                    #endregion

                    #region -Domicilio Secundario-
                    if (arrayDomSec != null)
                    {
                        JArray domSec = JArray.Parse(arrayDomSec);
                        for (int i = 0; i < domSec.Count; i = i + 14)
                        {
                            domiciliosecundariocl.Motivo = mg.normaliza(domSec[i].ToString());
                            domiciliosecundariocl.TipoDomicilio = domSec[i + 1].ToString();
                            domiciliosecundariocl.Calle = mg.normaliza(domSec[i + 2].ToString());
                            domiciliosecundariocl.No = mg.normaliza(domSec[i + 3].ToString());
                            domiciliosecundariocl.NombreCf = mg.normaliza(domSec[i + 4].ToString());
                            domiciliosecundariocl.Pais = domSec[i + 5].ToString();
                            domiciliosecundariocl.Estado = mg.normaliza(domSec[i + 6].ToString());
                            domiciliosecundariocl.Municipio = domSec[i + 7].ToString();
                            domiciliosecundariocl.Temporalidad = domSec[i + 8].ToString();
                            domiciliosecundariocl.ResidenciaHabitual = domSec[i + 9].ToString();
                            domiciliosecundariocl.Cp = domSec[i + 10].ToString();
                            domiciliosecundariocl.Referencias = mg.normaliza(domSec[i + 11].ToString());
                            domiciliosecundariocl.Horario = mg.normaliza(domSec[i + 12].ToString());
                            domiciliosecundariocl.Observaciones = mg.normaliza(domSec[i + 13].ToString());
                            domiciliosecundariocl.IdDomicilioCl = idDomiciliocl;
                            _context.Add(domiciliosecundariocl);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                        }
                    }


                    /*for (int i = 0; i < datosDomiciolioSecundario.Count; i++)
                    {
                        if (datosDomiciolioSecundario[i][1] == currentUser)
                        {
                            datosDomiciolioSecundario.RemoveAt(i);
                            i--;
                        }
                    }*/
                    #endregion

                    #region -Familiares Extranjero-
                    if (arrayFamExtranjero != null)
                    {
                        JArray famExtranjero = JArray.Parse(arrayFamExtranjero);
                        for (int i = 0; i < famExtranjero.Count; i = i + 12)
                        {
                            familiaresForaneoscl.Nombre = mg.normaliza(famExtranjero[i].ToString());
                            familiaresForaneoscl.Relacion = famExtranjero[i + 1].ToString();
                            try
                            {
                                familiaresForaneoscl.Edad = Int32.Parse(famExtranjero[i + 2].ToString());
                            }
                            catch
                            {
                                familiaresForaneoscl.Edad = 0;
                            }
                            familiaresForaneoscl.Sexo = famExtranjero[i + 3].ToString();
                            familiaresForaneoscl.TiempoConocerlo = famExtranjero[i + 4].ToString();
                            familiaresForaneoscl.Pais = famExtranjero[i + 5].ToString();
                            familiaresForaneoscl.Estado = mg.normaliza(famExtranjero[i + 6].ToString());
                            familiaresForaneoscl.Telefono = famExtranjero[i + 7].ToString();
                            familiaresForaneoscl.FrecuenciaContacto = famExtranjero[i + 8].ToString();
                            familiaresForaneoscl.EnteradoProceso = famExtranjero[i + 9].ToString();
                            familiaresForaneoscl.PuedeEnterarse = famExtranjero[i + 10].ToString();
                            familiaresForaneoscl.Observaciones = mg.normaliza(famExtranjero[i + 11].ToString());
                            familiaresForaneoscl.PersonaClIdPersonaCl = idPersonacl;
                            _context.Add(familiaresForaneoscl);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                        }
                    }
                    /*for (int i = 0; i < datosFamiliaresExtranjero.Count; i++)
                    {
                        if (datosFamiliaresExtranjero[i][1] == currentUser)
                        {
                            datosFamiliaresExtranjero.RemoveAt(i);
                            i--;
                        }
                    }*/
                    #endregion

                    #region -Guardar Foto-
                    if (fotografia != null)
                    {
                        string file_name = personacl.IdPersonaCl + "_" + personacl.Paterno + "_" + personacl.Nombre + ".jpg";
                        file_name = mg.replaceSlashes(file_name);
                        personacl.RutaFoto = file_name;
                        var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Fotoscl");
                        var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
                        await fotografia.CopyToAsync(stream);
                        stream.Close();
                    }
                    #endregion

                    #region -Expediente Unico-
                    //TODO QUEDO EN API =)
                    #endregion

                    #region -Añadir a contexto-

                    _context.Add(personacl); //Sirve
                    _context.Add(domiciliocl);
                    // _context.Add(domiciliosecundario);
                    _context.Add(estudioscl);
                    _context.Add(trabajocl); //Sirve
                    _context.Add(actividadsocialcl);
                    _context.Add(abandonoEstadocl);
                    _context.Add(saludfisicacl); //Sirve 
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);



                    return RedirectToAction("RegistroConfirmation/" + personacl.IdPersonaCl, "Personas");
                    #endregion
                }
            }
            return View();
        }
        #endregion

        #region -RegistroConfirmation-
        public async Task<IActionResult> RegistroConfirmation(int? id)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            bool escl = false;
            bool esmcyscp = false;

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP" || rol == "SupervisorMCSCP" || rol == "AuxiliarMCSCP" || rol == "ArchivoMCSCP")
                {
                    var persona = await _context.Persona.SingleOrDefaultAsync(m => m.IdPersona == id);
                    if (persona == null)
                    {
                        ViewBag.nombreRegistrado = null;
                    }
                    else
                    {
                        ViewBag.ControllerName = "Personas";
                        ViewBag.NombreTabla = "persona";
                        ViewBag.nombreRegistrado = persona.NombreCompleto;
                        ViewBag.idRegistrado = persona.IdPersona;
                        ViewBag.esmcyscp = esmcyscp = true;
                    }
                }
                else
                {
                    var persona = await _context.Personacl.SingleOrDefaultAsync(m => m.IdPersonaCl == id);
                    if (persona == null)
                    {
                        ViewBag.nombreRegistrado = null;
                    }
                    else
                    {
                        ViewBag.ControllerName = "Personascls";
                        ViewBag.NombreTabla = "personacl";
                        ViewBag.nombreRegistrado = persona.NombreCompleto;
                        ViewBag.idRegistrado = persona.IdPersonaCl;
                        ViewBag.escl = escl = true;
                    }
                }
            }
            return View();
        }

        #endregion       

        #endregion

        #region -Entrevista-
        public ActionResult Entrevista()
        {
            var personas = from p in _context.Persona
                           orderby p.Paterno
                           select p;

            ViewBag.personas = personas.ToList();

            idPersona = 0;

            return View();
        }

        [HttpPost, ActionName("Entrevista")]
        [ValidateAntiForgeryToken]
        public IActionResult EntrevistaPost()
        {
            if (ModelState.IsValid)
            {
                var persona = _context.Persona
                    .SingleOrDefault(m => m.IdPersona == idPersona);

                if (persona.Supervisor == null)
                {
                    return RedirectToAction("SinSupervisor");
                }
                else
                {
                    return RedirectToAction("MenuEdicion", "Personas", new { @id = idPersona });
                }
            }
            return View();
        }
        #endregion

        #region -Seleccionada-
        public ActionResult Seleccionada(string[] datosPersona)
        {
            idPersona = Int32.Parse(datosPersona[0]);
            return Json(new { success = true, responseText = "Persona seleccionada" });
        }
        #endregion

        #region -Reportes-
        public ActionResult ReportePersona()
        {
            return View();
        }



        #region -Crea QR-
        public void creaQR(int? id)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("10.6.60.190/Personas/Details/" + id, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            System.IO.FileStream fs = System.IO.File.Open(this._hostingEnvironment.WebRootPath + "\\images\\QR.jpg", FileMode.Create);
            qrCodeImage.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
            fs.Close();
        }
        #endregion

        public void Imprimir(int? id)
        {
            var persona = _context.Persona
               .SingleOrDefault(m => m.IdPersona == id);

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

            #endregion

            #region -Jointables-
            List<PersonaViewModel> vistaPersona = (from personaTable in personaVM
                                                   join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                                   join estudios in estudiosVM on persona.IdPersona equals estudios.PersonaIdPersona
                                                   join trabajo in trabajoVM on persona.IdPersona equals trabajo.PersonaIdPersona
                                                   join actividaSocial in actividadSocialVM on persona.IdPersona equals actividaSocial.PersonaIdPersona
                                                   join abandonoEstado in abandonoEstadoVM on persona.IdPersona equals abandonoEstado.PersonaIdPersona
                                                   join saludFisica in saludFisicaVM on persona.IdPersona equals saludFisica.PersonaIdPersona
                                                   join nacimientoEstado in estados on (Int32.Parse(persona.Lnestado)) equals nacimientoEstado.Id
                                                   join nacimientoMunicipio in municipios on (Int32.Parse(persona.Lnmunicipio)) equals nacimientoMunicipio.Id
                                                   join domicilioEstado in estados on (Int32.Parse(domicilio.Estado)) equals domicilioEstado.Id
                                                   join domicilioMunicipio in municipios on (Int32.Parse(domicilio.Municipio)) equals domicilioMunicipio.Id
                                                   where personaTable.IdPersona == id
                                                   select new PersonaViewModel
                                                   {
                                                       personaVM = personaTable,
                                                       domicilioVM = domicilio,
                                                       estudiosVM = estudios,
                                                       trabajoVM = trabajo,
                                                       actividadSocialVM = actividaSocial,
                                                       abandonoEstadoVM = abandonoEstado,
                                                       saludFisicaVM = saludFisica,
                                                       estadosVMPersona = nacimientoEstado,
                                                       municipiosVMPersona = nacimientoMunicipio,
                                                       estadosVMDomicilio = domicilioEstado,
                                                       municipiosVMDomicilio = domicilioMunicipio
                                                   }).ToList();
            #endregion

            creaQR(id);

            #region -GeneraDocumento-
            string templatePath = this._hostingEnvironment.WebRootPath + "\\Documentos\\templateEntrevista.docx";
            string resultPath = this._hostingEnvironment.WebRootPath + "\\Documentos\\entrevista.docx";
            string rutaFoto = ((vistaPersona[0].personaVM.Genero == ("M")) ? "hombre.png" : "mujer.png");
            if (vistaPersona[0].personaVM.rutaFoto != null)
            {
                rutaFoto = vistaPersona[0].personaVM.rutaFoto;
            }
            string picPath = this._hostingEnvironment.WebRootPath + "\\Fotos\\" + rutaFoto;

            DocumentCore dc = DocumentCore.Load(templatePath);

            string lnacimientoCleaned = "";
            if (vistaPersona[0].personaVM.Lnlocalidad != "NA")
            {
                if (lnacimientoCleaned != "")
                {
                    lnacimientoCleaned += ",";
                }
                lnacimientoCleaned += vistaPersona[0].personaVM.Lnlocalidad;
            }
            if (vistaPersona[0].municipiosVMPersona.Municipio != "Sin municipio")
            {
                if (lnacimientoCleaned != "")
                {
                    lnacimientoCleaned += ",";
                }
                lnacimientoCleaned += vistaPersona[0].municipiosVMPersona.Municipio.ToUpper();
            }
            if (vistaPersona[0].estadosVMPersona.Estado != "Sin estado")
            {
                if (lnacimientoCleaned != "")
                {
                    lnacimientoCleaned += ",";
                }
                lnacimientoCleaned += vistaPersona[0].estadosVMPersona.Estado.ToUpper();
            }
            if (lnacimientoCleaned != "")
            {
                lnacimientoCleaned += ",";
            }
            lnacimientoCleaned += vistaPersona[0].personaVM.Lnpais;
            var dataSource = new[] { new {
                nombre = vistaPersona[0].personaVM.Paterno+" "+ vistaPersona[0].personaVM.Materno +" "+ vistaPersona[0].personaVM.Nombre,
                nombrepadre = vistaPersona[0].personaVM.NombrePadre,
                nombremadre = vistaPersona[0].personaVM.NombreMadre,
                genero = vistaPersona[0].personaVM.Genero,
                lnacimiento = lnacimientoCleaned,
                fnacimiento =(Convert.ToDateTime(vistaPersona[0].personaVM.Fnacimiento)).ToString("dd MMMM yyyy"),
                edad=vistaPersona[0].personaVM.Edad,
                estadocivil=vistaPersona[0].personaVM.EstadoCivil,
                duracionestadocivil=vistaPersona[0].personaVM.Duracion,
                hablaidioma=vistaPersona[0].personaVM.OtroIdioma,
                especifiqueidioma=vistaPersona[0].personaVM.EspecifiqueIdioma,
                leerescribir=vistaPersona[0].personaVM.LeerEscribir,
                traductor=vistaPersona[0].personaVM.Traductor,
                especifiquetraductor=vistaPersona[0].personaVM.EspecifiqueTraductor,
                telefono=vistaPersona[0].personaVM.TelefonoFijo,
                celular=vistaPersona[0].personaVM.Celular,
                hijos=vistaPersona[0].personaVM.Hijos,
                cuantoshijos=vistaPersona[0].personaVM.Nhijos,
                personasvive=vistaPersona[0].personaVM.NpersonasVive,
                otraspropiedades=vistaPersona[0].personaVM.Propiedades,
                curp=vistaPersona[0].personaVM.Curp,
                consumosustancias=vistaPersona[0].personaVM.ConsumoSustancias,
                familiares=vistaPersona[0].personaVM.Familiares,
                referenciasPersonales=vistaPersona[0].personaVM.ReferenciasPersonales,
                tipopropiedad=vistaPersona[0].domicilioVM.TipoDomicilio,
                direccion=vistaPersona[0].domicilioVM.Calle+" "+vistaPersona[0].domicilioVM.No+", "+vistaPersona[0].domicilioVM.NombreCf+" CP "+vistaPersona[0].domicilioVM.Cp+", "+vistaPersona[0].estadosVMDomicilio.Estado+", "+vistaPersona[0].municipiosVMDomicilio.Municipio+", "+vistaPersona[0].domicilioVM.Pais,
                tiempoendomicilio=vistaPersona[0].domicilioVM.Temporalidad,
                residenciahabitual=vistaPersona[0].domicilioVM.ResidenciaHabitual,
                referenciasdomicilio=vistaPersona[0].domicilioVM.Referencias,
                horariodomicilio=vistaPersona[0].domicilioVM.Horario,
                observacionesdomicilio=vistaPersona[0].domicilioVM.Observaciones,
                domiciliosecundario=vistaPersona[0].domicilioVM.DomcilioSecundario,
                estudia=vistaPersona[0].estudiosVM.Estudia,
                gradoestudios=vistaPersona[0].estudiosVM.GradoEstudios,
                institucionestudios=vistaPersona[0].estudiosVM.InstitucionE,
                horarioescuela=vistaPersona[0].estudiosVM.Horario,
                direccionescuela=vistaPersona[0].estudiosVM.Direccion,
                telefonoescuela=vistaPersona[0].estudiosVM.Telefono,
                observacionesescolaridad=vistaPersona[0].estudiosVM.Observaciones,
                trabaja=vistaPersona[0].trabajoVM.Trabaja,
                tipoocupacion=vistaPersona[0].trabajoVM.TipoOcupacion,
                puesto=vistaPersona[0].trabajoVM.Puesto,
                empleador=vistaPersona[0].trabajoVM.EmpledorJefe,
                enteradoprocesotrabajo=vistaPersona[0].trabajoVM.EnteradoProceso,
                sepuedeenterartrabajo=vistaPersona[0].trabajoVM.SePuedeEnterar,
                tiempotrabajando=vistaPersona[0].trabajoVM.TiempoTrabajano,
                salario= mg.Dinero(vistaPersona[0].trabajoVM.Salario),
                temporalidadpago=vistaPersona[0].trabajoVM.TemporalidadSalario,
                direcciontrabajo=vistaPersona[0].trabajoVM.Direccion,
                horariotrabajo=vistaPersona[0].trabajoVM.Horario,
                telefonotrabajo=vistaPersona[0].trabajoVM.Telefono,
                observacionestrabajo=vistaPersona[0].trabajoVM.Observaciones,
                tipoactividad=vistaPersona[0].actividadSocialVM.TipoActividad,
                horarioactividad=vistaPersona[0].actividadSocialVM.Horario,
                lugaractividad=vistaPersona[0].actividadSocialVM.Lugar,
                telefonoactividad=vistaPersona[0].actividadSocialVM.Telefono,
                sepuedeenteraractividad=vistaPersona[0].actividadSocialVM.SePuedeEnterar,
                referenciaactividad=vistaPersona[0].actividadSocialVM.Referencia,
                observacionesactividad=vistaPersona[0].actividadSocialVM.Observaciones,
                vividofuera=vistaPersona[0].abandonoEstadoVM.VividoFuera,
                lugaresvivido=vistaPersona[0].abandonoEstadoVM.LugaresVivido,
                temporalidadviajes=vistaPersona[0].abandonoEstadoVM.TiempoVivido,
                motivovivido=vistaPersona[0].abandonoEstadoVM.MotivoVivido,
                viajahabitualmente=vistaPersona[0].abandonoEstadoVM.ViajaHabitual,
                lugaresviaje=vistaPersona[0].abandonoEstadoVM.LugaresViaje,
                tiempoviajes=vistaPersona[0].abandonoEstadoVM.TiempoViaje,
                motivoviajes=vistaPersona[0].abandonoEstadoVM.MotivoViaje,
                documentacion=vistaPersona[0].abandonoEstadoVM.DocumentacionSalirPais,
                pasaporte=vistaPersona[0].abandonoEstadoVM.Pasaporte,
                visa=vistaPersona[0].abandonoEstadoVM.Visa,
                familiaresfuera=vistaPersona[0].abandonoEstadoVM.FamiliaresFuera,
                enfermedades=vistaPersona[0].saludFisicaVM.Enfermedad,
                especenfermedad=vistaPersona[0].saludFisicaVM.EspecifiqueEnfermedad,
                tratamientomedico=vistaPersona[0].saludFisicaVM.Tratamiento,
                discapacidad=vistaPersona[0].saludFisicaVM.Discapacidad,
                especdiscapacidad=vistaPersona[0].saludFisicaVM.EspecifiqueDiscapacidad,
                serviciomedico=vistaPersona[0].saludFisicaVM.ServicioMedico,
                tiposervicio=vistaPersona[0].saludFisicaVM.EspecifiqueServicioMedico,
                institucionsalud=vistaPersona[0].saludFisicaVM.InstitucionServicioMedico,
                observacionessalud=vistaPersona[0].saludFisicaVM.Observaciones

            } };


            dc.MailMerge.FieldMerging += (sender, e) =>
            {
                if (e.FieldName == "foto")
                {
                    e.Inlines.Clear();
                    e.Inlines.Add(new Picture(dc, picPath) { Layout = new InlineLayout(new Size(100, 100)) });
                    e.Cancel = false;
                }
                if (e.FieldName == "QR")
                {
                    e.Inlines.Clear();
                    e.Inlines.Add(new Picture(dc, this._hostingEnvironment.WebRootPath + "\\images\\QR.jpg") { Layout = new InlineLayout(new Size(100, 100)) });
                    e.Cancel = false;
                }
            };

            dc.MailMerge.Execute(dataSource);


            dc.Save(resultPath);

            //Response.Redirect("https://localhost:44359/Documentos/entrevista.docx");
            Response.Redirect("http://10.6.60.190/Documentos/entrevista.docx");
            #endregion

        }
        #endregion

        #region -Procesos-
        public async Task<IActionResult> Procesos(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var supervisiones = await _context.Supervision.Where(m => m.PersonaIdPersona == id).ToListAsync();
            if (supervisiones.Count == 0)
            {

                return RedirectToAction("SinSupervision");
            }


            List<Fraccionesimpuestas> fraccionesimpuestasVM = _context.Fraccionesimpuestas.ToList();


            List<Fraccionesimpuestas> queryFracciones = (from f in fraccionesimpuestasVM
                                                         group f by f.SupervisionIdSupervision into grp
                                                         select grp.OrderByDescending(f => f.IdFracciones).FirstOrDefault()).ToList();


            var queryCausas = from c in _context.Causapenal
                              join s in _context.Supervision on c.IdCausaPenal equals s.CausaPenalIdCausaPenal
                              join p in _context.Persona on s.PersonaIdPersona equals p.IdPersona
                              join pe in _context.Planeacionestrategica on s.IdSupervision equals pe.SupervisionIdSupervision
                              join f in queryFracciones on s.IdSupervision equals f.SupervisionIdSupervision into tmp
                              from sinfracciones in tmp.DefaultIfEmpty()
                              where p.IdPersona == id
                              select new Procesos
                              {
                                  supervisionVM = s,
                                  causapenalVM = c,
                                  personaVM = p,
                                  planeacionestrategicaVM = pe,
                                  fraccionesimpuestasVM = ((sinfracciones == null) ? null : sinfracciones)
                              };

            ViewData["joinTbalasProceso1"] = queryCausas.ToList();

            return View();
        }
        #endregion

        #region -Presentaciones periodicas-
        public async Task<IActionResult> PresentacionPeriodicaPersona(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.Invitado = true;
            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP" || rol == "Masteradmin" || rol == "SupervisorMCSCP")
                {
                    ViewBag.Invitado = false;
                    break;
                }
            }


            List<PresentacionPeriodicaPersona> lists = new List<PresentacionPeriodicaPersona>();

            var queripersonasis = from p in _context.Persona
                                  join rh in _context.Registrohuella on p.IdPersona equals rh.PersonaIdPersona
                                  join pp in _context.Presentacionperiodica on rh.IdregistroHuella equals pp.RegistroidHuella
                                  where p.IdPersona == id
                                  select new PresentacionPeriodicaPersona
                                  {
                                      presentacionperiodicaVM = pp,
                                      registrohuellaVM = rh,
                                      personaVM = p
                                  };
            var maxfra = queripersonasis.OrderByDescending(u => u.presentacionperiodicaVM.FechaFirma);

            if (queripersonasis.Count() == 0)
            {
                return RedirectToAction("PresentacionPeriodicaConfirmation/" + "Personas");
            }
            else
            {
                ViewData["joinTablasPresentacion"] = maxfra;
                return View();
            }
        }

        public async Task<IActionResult> PresentacionPeriodicaConfirmation()
        {
            return View();
        }

        private bool PresentacionExists(int id)
        {
            return _context.Presentacionperiodica.Any(e => e.IdpresentacionPeriodica == id);
        }
        #endregion

        #region -EditarComentario-
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditComentario(int id, int idpersona, [Bind("IdpresentacionPeriodica,FechaFirma,ComentarioFirma,RegistroidHuella")] Presentacionperiodica presentacionperiodica)
        {
            id = presentacionperiodica.IdpresentacionPeriodica;
            presentacionperiodica.RegistroidHuella = presentacionperiodica.RegistroidHuella;
            presentacionperiodica.ComentarioFirma = presentacionperiodica.ComentarioFirma != null ? presentacionperiodica.ComentarioFirma.ToUpper() : "NA";
            presentacionperiodica.FechaFirma = presentacionperiodica.FechaFirma;

            //_context.SaveChanges();
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var oldDomicilio = await _context.Presentacionperiodica.FindAsync(presentacionperiodica.IdpresentacionPeriodica);
            _context.Entry(oldDomicilio).CurrentValues.SetValues(presentacionperiodica);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PresentacionExists(presentacionperiodica.IdpresentacionPeriodica))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
    
            return RedirectToAction("PresentacionPeriodicaPersona/" + idpersona);
        }
        #endregion

        #region -SinSupervision-
        public async Task<IActionResult> SinSupervision()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            bool invitado = true;

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP" || rol == "Masteradmin" || rol == "SupervisorMCSCP")
                {
                    // SE VUELVE FALSO SI EL USUARIO TIENE UN ROL DE MC Y SCP 
                    invitado = false;
                    break;

                }
            }


            ViewBag.EsInvitado = invitado;

            return View();
        }
        #endregion

        #region -SinSupervisor-
        public async Task<IActionResult> SinSupervisor()
        {
            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.IdPersona == idPersona);
            if (persona == null)
            {
                ViewBag.nombre = null;
                ViewBag.capturista = null;
            }
            else
            {
                ViewBag.nombre = persona.NombreCompleto;
                ViewBag.capturista = persona.Capturista;
            }
            return View();
        }
        #endregion

        #region -Edicion-        

        public async Task<IActionResult> MenuEdicion(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByNameAsync(User.Identity.Name);

            #region -Solicitud Atendida Archivo prestamo Digital-
            var warningRespuesta = from a in _context.Archivoprestamodigital
                                   where a.EstadoPrestamo == 1 && user.ToString().ToUpper() == a.Usuario.ToUpper()
                                   select a;
            ViewBag.WarningsUser = warningRespuesta.Count();
            #endregion


            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.IdPersona == id);
            if (persona == null)
            {
                return NotFound();
            }

            string rutaFoto = ((persona.Genero == ("M")) ? "hombre.png" : "mujer.png");
            if (persona.rutaFoto != null)
            {
                rutaFoto = persona.rutaFoto;
            }
            ViewBag.rutaFoto = rutaFoto;

            return View(persona);
        }

        public async Task<IActionResult> EditFoto(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.IdPersona == id);
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFoto([Bind("IdPersona")] Persona persona, IFormFile fotoEditada)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                #region -Guardar Foto-
                var file_name = (from a in _context.Persona
                                 where a.IdPersona == persona.IdPersona
                                 select a.rutaFoto).FirstOrDefault();
                if (file_name == null || file_name == "NA")
                {
                    var query = (from a in _context.Persona
                                 where a.IdPersona == persona.IdPersona
                                 select a).FirstOrDefault();
                    file_name = query.IdPersona + "_" + query.Paterno + "_" + query.Nombre + ".jpg";
                    query.rutaFoto = file_name;
                    try
                    {
                        var oldFoto = await _context.Persona.FindAsync(query.IdPersona);
                        _context.Entry(oldFoto).CurrentValues.SetValues(query);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                        //_context.Update(query);
                        //await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {

                       
                        if (!PersonaExists(query.IdPersona))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
               
                }
                var uploads = Path.Combine(this._hostingEnvironment.WebRootPath, "Fotos");
                var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create, FileAccess.ReadWrite);
                #endregion

                fotoEditada.CopyTo(stream);
                stream.Close();
                return RedirectToAction("MenuEdicion/" + persona.IdPersona, "Personas");
            }
            return View(persona);
        }

        String BuscaId(List<SelectListItem> lista, String texto)
        {
            foreach (var item in lista)
            {
                if (mg.normaliza(item.Value) == mg.normaliza(texto))
                {
                    return item.Value;
                }
            }
            return "";
        }

        #region -Edita Datos Generales-
        // GET: Personas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            ViewBag.centrosPenitenciarios = _context.Centrospenitenciarios.Select(Centrospenitenciarios => Centrospenitenciarios.Nombrecentro).ToList();

            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.IdPersona == id);
            List<string> consumo = new List<string>();
            consumosustancias = await _context.Consumosustancias.Where(m => m.PersonaIdPersona == id).ToListAsync();
            for (int i = 0; i < consumosustancias.Count; i++)
            {
                consumo.Add(consumosustancias[i].Sustancia?.ToString());
                consumo.Add(consumosustancias[i].Frecuencia?.ToString());
                consumo.Add(consumosustancias[i].Cantidad?.ToString());
                consumo.Add(consumosustancias[i].UltimoConsumo?.ToString("yyyy-MM-ddTHH:mm:ss"));
                consumo.Add(consumosustancias[i].Observaciones?.ToString());
                consumo.Add(consumosustancias[i].IdConsumoSustancias.ToString());
            }
            List<string> asientofamiliares = new List<string>();
            familiares = await _context.Asientofamiliar.Where(m => m.PersonaIdPersona == id && m.Tipo == "FAMILIAR").ToListAsync();
            for (int i = 0; i < familiares.Count; i++)
            {
                asientofamiliares.Add(familiares[i].Nombre?.ToString());
                asientofamiliares.Add(familiares[i].Relacion?.ToString());
                asientofamiliares.Add(familiares[i].Edad?.ToString());
                asientofamiliares.Add(familiares[i].Sexo?.ToString());
                asientofamiliares.Add(familiares[i].Dependencia?.ToString());
                asientofamiliares.Add(familiares[i].DependenciaExplica?.ToString());
                asientofamiliares.Add(familiares[i].VivenJuntos?.ToString());
                asientofamiliares.Add(familiares[i].Domicilio?.ToString());
                asientofamiliares.Add(familiares[i].Telefono?.ToString());
                asientofamiliares.Add(familiares[i].HorarioLocalizacion?.ToString());
                asientofamiliares.Add(familiares[i].EnteradoProceso?.ToString());
                asientofamiliares.Add(familiares[i].PuedeEnterarse?.ToString());
                asientofamiliares.Add(familiares[i].Observaciones?.ToString());
                asientofamiliares.Add(familiares[i].IdAsientoFamiliar.ToString());
            }
            List<string> asientoreferencias = new List<string>();
            referenciaspersonales = await _context.Asientofamiliar.Where(m => m.PersonaIdPersona == id && m.Tipo == "REFERENCIA").ToListAsync();
            for (int i = 0; i < referenciaspersonales.Count; i++)
            {
                asientoreferencias.Add(referenciaspersonales[i].Nombre?.ToString());
                asientoreferencias.Add(referenciaspersonales[i].Relacion?.ToString());
                asientoreferencias.Add(referenciaspersonales[i].Edad?.ToString());
                asientoreferencias.Add(referenciaspersonales[i].Sexo?.ToString());
                asientoreferencias.Add(referenciaspersonales[i].Dependencia?.ToString());
                asientoreferencias.Add(referenciaspersonales[i].DependenciaExplica?.ToString());
                asientoreferencias.Add(referenciaspersonales[i].VivenJuntos?.ToString());
                asientoreferencias.Add(referenciaspersonales[i].Domicilio?.ToString());
                asientoreferencias.Add(referenciaspersonales[i].Telefono?.ToString());
                asientoreferencias.Add(referenciaspersonales[i].HorarioLocalizacion?.ToString());
                asientoreferencias.Add(referenciaspersonales[i].EnteradoProceso?.ToString());
                asientoreferencias.Add(referenciaspersonales[i].PuedeEnterarse?.ToString());
                asientoreferencias.Add(referenciaspersonales[i].Observaciones?.ToString());
                asientoreferencias.Add(referenciaspersonales[i].IdAsientoFamiliar.ToString());
            }
            if (persona == null)
            {
                return NotFound();
            }

            #region PAIS          
            List<SelectListItem> ListaPais;
            ListaPais = new List<SelectListItem>
            {
              new SelectListItem{ Text="México", Value="MEXICO"},
              new SelectListItem{ Text="Estados Unidos", Value="ESTADOS UNIDOS"},
              new SelectListItem{ Text="Canada", Value="CANADA"},
              new SelectListItem{ Text="Colombia", Value="COLOMBIA"},
              new SelectListItem{ Text="El Salvador", Value="EL SALVADOR"},
              new SelectListItem{ Text="Guatemala", Value="GUATEMALA"},
              new SelectListItem{ Text="Chile", Value="CHILE"},
              new SelectListItem{ Text="Argentina", Value="ARGENTINA"},
              new SelectListItem{ Text="Brasil", Value="BRASIL"},
              new SelectListItem{ Text="Venezuela", Value="VENEZUELA"},
              new SelectListItem{ Text="Puerto Rico", Value="PUERTO RICO"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.listaLnpais = ListaPais;

            foreach (var item in ListaPais)
            {
                if (item.Value == persona.Lnpais)
                {
                    ViewBag.idLnpais = item.Value;
                    break;
                }
            }
            #endregion

            #region Lnestado
            List<Estados> listaEstados = new List<Estados>();
            listaEstados = (from table in _context.Estados
                            select table).ToList();

            ViewBag.ListadoEstados = listaEstados;

            ViewBag.idEstado = persona.Lnestado;
            #endregion

            #region Lnmunicipio
            int Lnestado;
            bool success = Int32.TryParse(persona.Lnestado, out Lnestado);
            List<Municipios> listaMunicipios = new List<Municipios>();
            if (success)
            {
                listaMunicipios = (from table in _context.Municipios
                                   where table.EstadosId == Lnestado
                                   select table).ToList();
            }

            listaMunicipios.Insert(0, new Municipios { Id = 0, Municipio = "Selecciona" });

            ViewBag.ListadoMunicipios = listaMunicipios;
            ViewBag.idMunicipio = persona.Lnmunicipio;
            #endregion

            #region EstadoCivil
            List<SelectListItem> ListaEstadoCivil;
            ListaEstadoCivil = new List<SelectListItem>
            {
              new SelectListItem{ Text="Soltero (a)", Value="SOLTERO (A)"},
              new SelectListItem{ Text="Casado (a)", Value="CASADO (A)"},
              new SelectListItem{ Text="Union libre", Value="UNION LIBRE"},
              new SelectListItem{ Text="Viudo (a)", Value="VIUDO (A)"},
              new SelectListItem{ Text="Divorciado (a)", Value="DIVORCIADO (A)"}
            };

            ViewBag.listaEstadoCivil = ListaEstadoCivil;
            ViewBag.idEstadoCivil = BuscaId(ListaEstadoCivil, persona.EstadoCivil);
            #endregion

            #region GENERO          
            List<SelectListItem> ListaGenero;
            ListaGenero = new List<SelectListItem>
            {
              new SelectListItem{ Text="Masculino", Value="M"},
              new SelectListItem{ Text="Femenino", Value="F"},
              new SelectListItem{ Text="Prefiero no decirlo", Value="N"},
            };

            ViewBag.listaGenero = ListaGenero;
            ViewBag.idGenero = BuscaId(ListaGenero, persona.Genero);

            #endregion

            #region -viewbags y listas-

            ViewBag.listaOtroIdioma = listaNoSi;
            ViewBag.idOtroIdioma = BuscaId(listaNoSi, persona.OtroIdioma);

            ViewBag.listaLeerEscribir = listaSiNo;
            ViewBag.idLeerEscribir = BuscaId(listaSiNo, persona.LeerEscribir);

            ViewBag.listaTraductor = listaNoSi;
            ViewBag.idTraductor = BuscaId(listaNoSi, persona.Traductor);

            ViewBag.listaHijos = listaNoSi;
            ViewBag.idHijos = BuscaId(listaNoSi, persona.Hijos);

            ViewBag.listaPropiedades = listaNoSi;
            ViewBag.idPropiedades = BuscaId(listaNoSi, persona.Propiedades);

            ViewBag.listaResolucion = listaNoSi;
            ViewBag.idResolucion = BuscaId(listaNoSi, persona.TieneResolucion);

            ViewBag.listaComindigena = listaNoSi;
            ViewBag.idComindigena = BuscaId(listaNoSi, persona.ComIndigena);

            ViewBag.listaComlgbtttiq = listaNoSi;
            ViewBag.idComlgbtttiq = BuscaId(listaNoSi, persona.ComLgbtttiq);

            ViewBag.listaSinoCentroPenitenciario = listaNoSi;
            ViewBag.idSinoCentroPenitenciario = BuscaId(listaNoSi, persona.Sinocentropenitenciario);

            ViewBag.idCentroPenitenciario = persona.Centropenitenciario;
            ViewBag.pais = persona.Lnpais;

            ViewBag.idioma = persona.OtroIdioma;
            ViewBag.EspecifiqueIdioma = persona.EspecifiqueIdioma;

            ViewBag.traductor = persona.Traductor;
            ViewBag.EspecifiqueTraductor = persona.EspecifiqueTraductor;

            ViewBag.TieneHijos = persona.Hijos;
            ViewBag.NumeroHijos = persona.Nhijos;

            #endregion

            #region -botones edicion sustancias, familiares y referencias-

            string familiarTipo = "", referenciaTipo = "", SI = "SI", NO = "NO";

            //PARA SABER SI LA PERSONA TIENE REGISTRO DE SUSTANCIAS
            int NumeroConsumoSustancias = _context.Consumosustancias.Where(a => a.PersonaIdPersona == id).Count();
            if (NumeroConsumoSustancias >= 1)
            {
                ViewBag.ConsumoSustancias = BuscaId(listaNoSi, SI);
            }
            else
            {
                ViewBag.ConsumoSustancias = BuscaId(listaNoSi, NO);
            }

            //CUENTA SI LA PERSONA TIENE FAMILIARES O REFERENCIAS EN ASIENTO FAMILIAR
            int NumeroFamiliares = _context.Asientofamiliar.Where(a => a.PersonaIdPersona == id).Count();
            //ALMACENA EL TIPO DE ASIENTO FAMILIAR YA SEA REFERENCIA O FAMILIAR
            List<string> TiposAsiento = _context.Asientofamiliar.Where(a => a.PersonaIdPersona == id).Select(a => a.Tipo).ToList();

            //SI LA PERSONA CUENTA CON REFERENCIA O FAMILIAR SE LLENAN VARIABLES
            foreach (string tipoAsiento in TiposAsiento)
            {
                if (tipoAsiento == "FAMILIAR")
                {
                    familiarTipo = tipoAsiento;
                }
                else if (tipoAsiento == "REFERENCIA")
                {
                    referenciaTipo = tipoAsiento;
                }
            }
            //SI ENCONTRO LLENA VIEWBAGS PARA BOTONES DE EDICION DE FAMILIAR y REFERENCIA
            if (NumeroFamiliares >= 1)
            {
                switch (familiarTipo)
                {
                    case "FAMILIAR":
                        ViewBag.idFamiliares = BuscaId(listaNoSi, SI);
                        break;
                    default:
                        ViewBag.idFamiliares = BuscaId(listaSiNo, NO);
                        break;
                }
                switch (referenciaTipo)
                {
                    case "REFERENCIA":
                        ViewBag.idReferenciasPersonales = BuscaId(listaSiNo, SI);
                        break;
                    default:
                        ViewBag.idReferenciasPersonales = BuscaId(listaSiNo, NO);
                        break;
                }
            }
            else
            {
                // SI NO ENCONTRO REGISTRO EN ASIENTO FAMILIAR BOTONES DE EDICION NO DISPNIBLES
                ViewBag.idFamiliares = BuscaId(listaSiNo, NO);
                ViewBag.idReferenciasPersonales = BuscaId(listaSiNo, NO);

            }
            #endregion

            #region Consume sustancias

            ViewBag.listaConsumoSustancias = listaNoSi;

            contadorSustancia = 0;

            List<SelectListItem> ListaSustancia;
            ListaSustancia = new List<SelectListItem>
            {
                new SelectListItem{ Text="Alcohol", Value="ALCOHOL"},
                new SelectListItem{ Text="Marihuana", Value="MARIHUANA"},
                new SelectListItem{ Text="Cocaína", Value="COCAINA"},
                new SelectListItem{ Text="Heroína", Value="HEROINA"},
                new SelectListItem{ Text="PVC", Value="PVC"},
                new SelectListItem{ Text="Solventes", Value="SOLVENTES"},
                new SelectListItem{ Text="Fármacos", Value="FARMACOS"},
                new SelectListItem{ Text="Cemento", Value="CEMENTO"},
                new SelectListItem{ Text="Crack", Value="CRACK"},
                new SelectListItem{ Text="Ácidos", Value="ACIDOS"},
                new SelectListItem{ Text="Tabaco", Value="TABACO"},
                new SelectListItem{ Text="Metanfetaminas", Value="METANFETAMINAS"},
                new SelectListItem{ Text="Otro", Value="OTRO"},
            };
            ViewBag.listaSustancia = ListaSustancia;

            List<SelectListItem> ListaFrecuencia;
            ListaFrecuencia = new List<SelectListItem>
            {
                new SelectListItem{ Text="Diario", Value="DIARIO"},
                new SelectListItem{ Text="Semanal", Value="SEMANAL"},
                new SelectListItem{ Text="Quincenal", Value="QUINCENAL"},
                new SelectListItem{ Text="Mensual", Value="MENSUAL"},
                new SelectListItem{ Text="Bimestral", Value="BIMESTRAL"},
                new SelectListItem{ Text="Trimestral", Value="TRIMESTRAL"},
                new SelectListItem{ Text="Semestral", Value="SEMESTRAL"},
                new SelectListItem{ Text="Anual", Value="ANUAL"},
                new SelectListItem{ Text="Ocasionalmente", Value="OCASIONALMENTE"},
            };
            ViewBag.listaFrecuencia = ListaFrecuencia;

            if (consumosustancias.Count > 0)
            {
                ViewBag.idSustancia = BuscaId(ListaSustancia, consumosustancias[contadorSustancia].Sustancia);
                ViewBag.idFrecuencia = BuscaId(ListaFrecuencia, consumosustancias[contadorSustancia].Frecuencia);
                ViewBag.cantidad = consumosustancias[contadorSustancia].Cantidad;
                ViewBag.ultimoConsumo = consumosustancias[contadorSustancia].UltimoConsumo;
                ViewBag.observaciones = consumosustancias[contadorSustancia].Observaciones;
                ViewBag.idConsumoSustancias = consumosustancias[contadorSustancia].IdConsumoSustancias;
                contadorSustancia++;
            }
            else
            {
                ViewBag.idSustancia = "ALCOHOL";
                ViewBag.idFrecuencia = "DIARIO";
                ViewBag.cantidad = null;
                ViewBag.ultimoConsumo = DateTime.ParseExact("01/01/1990", "MM/dd/yyyy", CultureInfo.InvariantCulture);
                ViewBag.observaciones = null;
            }

            ViewBag.ListaConsumo = consumo;
            ViewBag.ListaAsientoFamiliares = asientofamiliares;
            ViewBag.ListaAsientoReferencias = asientoreferencias;
            #endregion

            #region Familiares

            ViewBag.listaFamiliares = listaSiNo;

            contadorFamiliares = 0;

            List<SelectListItem> ListaRelacion;
            ListaRelacion = new List<SelectListItem>
            {
                new SelectListItem { Text = "Pápa", Value = "PAPA" },
                new SelectListItem { Text = "Máma", Value = "MAMA" },
                new SelectListItem{ Text="Esposo(a)", Value="ESPOSO (A)"},
                new SelectListItem{ Text="Hermano(a)", Value="HERMANO (A)"},
                new SelectListItem{ Text="Hijo(a)", Value="HIJO (A)"},
                new SelectListItem{ Text="Abuelo(a)", Value="ABUELO (A)"},
                new SelectListItem{ Text="Familiar 1 nivel", Value="FAMILIAR 1 NIVEL"},
                new SelectListItem{ Text="Amigo", Value="AMIGO"},
                new SelectListItem{ Text="Conocido", Value="CONOCIDO"},
                new SelectListItem{ Text="Otro", Value="OTRO"},
            };
            ViewBag.listaRelacion = ListaRelacion;

            List<SelectListItem> ListaSexo;
            ListaSexo = new List<SelectListItem>
            {
                new SelectListItem{ Text="Masculino", Value="M"},
                new SelectListItem{ Text="Femenino", Value="F"},
                new SelectListItem{ Text="Prefiero no decirlo", Value="N"},
            };
            ViewBag.listaSexo = ListaSexo;

            ViewBag.listaDependencia = listaNoSi;
            ViewBag.listaVivenJuntos = listaSiNo;
            ViewBag.listaEnteradoProceso = listaSiNo;
            ViewBag.listaPuedeEnterarse = listaNoSiNA;

            if (familiares.Count > 0)
            {
                ViewBag.nombreF = familiares[contadorFamiliares].Nombre;
                ViewBag.idRelacionF = BuscaId(ListaRelacion, familiares[contadorFamiliares].Relacion);
                ViewBag.edadF = familiares[contadorFamiliares].Edad;
                ViewBag.idSexoF = BuscaId(ListaSexo, familiares[contadorFamiliares].Sexo); ;
                ViewBag.idDependenciaF = BuscaId(listaNoSi, familiares[contadorFamiliares].Dependencia);
                ViewBag.dependenciaExplicaF = familiares[contadorFamiliares].DependenciaExplica;
                ViewBag.idVivenJuntosF = BuscaId(listaSiNo, familiares[contadorFamiliares].VivenJuntos);
                ViewBag.domicilioF = familiares[contadorFamiliares].Domicilio;
                ViewBag.telefonoF = familiares[contadorFamiliares].Telefono;
                ViewBag.horarioLocalizacionF = familiares[contadorFamiliares].HorarioLocalizacion;
                ViewBag.idEnteradoProcesoF = BuscaId(listaSiNo, familiares[contadorFamiliares].EnteradoProceso);
                ViewBag.idPuedeEnterarseF = BuscaId(listaNoSiNA, familiares[contadorFamiliares].PuedeEnterarse);
                ViewBag.AFobservacionesF = familiares[contadorFamiliares].Observaciones;
                ViewBag.tipoF = familiares[contadorFamiliares].Tipo;
                ViewBag.idAsientoFamiliarF = familiares[contadorFamiliares].IdAsientoFamiliar;
                contadorFamiliares++;
            }
            else
            {
                ViewBag.nombreF = null;
                ViewBag.idRelacionF = "MAMA";
                ViewBag.edadF = 0;
                ViewBag.idSexoF = "M";
                ViewBag.idDependenciaF = "NO";
                ViewBag.dependenciaExplicaF = null;
                ViewBag.idVivenJuntosF = "SI";
                ViewBag.domicilioF = null;
                ViewBag.telefonoF = null;
                ViewBag.horarioLocalizacionF = null;
                ViewBag.idEnteradoProcesoF = "SI";
                ViewBag.idPuedeEnterarseF = "NA";
                ViewBag.AFobservacionesF = null;
            }
            #endregion

            #region Referencias

            ViewBag.listaReferenciasPersonales = listaSiNo;

            contadorReferencias = 0;

            if (referenciaspersonales.Count > 0)
            {
                ViewBag.nombreR = referenciaspersonales[contadorReferencias].Nombre;
                ViewBag.idRelacionR = BuscaId(ListaRelacion, referenciaspersonales[contadorReferencias].Relacion);
                ViewBag.edadR = referenciaspersonales[contadorReferencias].Edad;
                ViewBag.idSexoR = BuscaId(ListaSexo, referenciaspersonales[contadorReferencias].Sexo); ;
                ViewBag.idDependenciaR = BuscaId(listaNoSi, referenciaspersonales[contadorReferencias].Dependencia);
                ViewBag.dependenciaExplicaR = referenciaspersonales[contadorReferencias].DependenciaExplica;
                ViewBag.idVivenJuntosR = BuscaId(listaSiNo, referenciaspersonales[contadorReferencias].VivenJuntos);
                ViewBag.domicilioR = referenciaspersonales[contadorReferencias].Domicilio;
                ViewBag.telefonoR = referenciaspersonales[contadorReferencias].Telefono;
                ViewBag.horarioLocalizacionR = referenciaspersonales[contadorReferencias].HorarioLocalizacion;
                ViewBag.idEnteradoProcesoR = BuscaId(listaSiNo, referenciaspersonales[contadorReferencias].EnteradoProceso);
                ViewBag.idPuedeEnterarseR = BuscaId(listaNoSiNA, referenciaspersonales[contadorReferencias].PuedeEnterarse);
                ViewBag.AFobservacionesR = referenciaspersonales[contadorReferencias].Observaciones;
                ViewBag.tipoR = referenciaspersonales[contadorReferencias].Tipo;
                ViewBag.idAsientoFamiliarR = referenciaspersonales[contadorReferencias].IdAsientoFamiliar;
                contadorReferencias++;
            }
            else
            {
                ViewBag.nombreR = null;
                ViewBag.idRelacionR = "MAMA";
                ViewBag.edadR = 0;
                ViewBag.idSexoR = "M";
                ViewBag.idDependenciaR = "NO";
                ViewBag.dependenciaExplicaR = null;
                ViewBag.idVivenJuntosR = "SI";
                ViewBag.domicilioR = null;
                ViewBag.telefonoR = null;
                ViewBag.horarioLocalizacionR = null;
                ViewBag.idEnteradoProcesoR = "SI";
                ViewBag.idPuedeEnterarseR = "NA";
                ViewBag.AFobservacionesR = null;
            }
            #endregion

            return View(persona);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPersona,TieneResolucion,Nombre,Paterno,Materno,NombrePadre,NombreMadre,Alias,Genero,Edad,Fnacimiento,Lnpais,Lnestado,Lnmunicipio,Lnlocalidad,EstadoCivil,Duracion,OtroIdioma,EspecifiqueIdioma,ComIndigena,ComLgbtttiq,DatosGeneralescol,LeerEscribir,Traductor,EspecifiqueTraductor,TelefonoFijo,Celular,Hijos,Nhijos,NpersonasVive,Propiedades,Curp,ConsumoSustancias,Familiares,ReferenciasPersonales,UltimaActualización,Supervisor,rutaFoto,Capturista,MotivoCandado,Candado,UbicacionExpediente")] Persona persona, Expedienteunico expedienteunico, string arraySustancias, string arraySustanciasEditadas, string arrayFamiliarReferencia, string arrayFamiliaresEditados, string arrayReferenciasEditadas, string centropenitenciario, string sinocentropenitenciario)
        {

            Domiciliosecundario domiciliosecundario = new Domiciliosecundario();
            Familiaresforaneos familiaresForaneos = new Familiaresforaneos();
            string currentUser = User.Identity.Name;

            if (id != persona.IdPersona)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                persona.Centropenitenciario = mg.removeSpaces(mg.normaliza(centropenitenciario));
                persona.Sinocentropenitenciario = sinocentropenitenciario;
                persona.TieneResolucion = mg.removeSpaces(mg.normaliza(persona.TieneResolucion));
                persona.Paterno = mg.removeSpaces(mg.normaliza(persona.Paterno));
                persona.Materno = mg.removeSpaces(mg.normaliza(persona.Materno));
                persona.Nombre = mg.removeSpaces(mg.normaliza(persona.Nombre));
                persona.NombrePadre = mg.normaliza(persona.NombrePadre);
                persona.NombreMadre = mg.normaliza(persona.NombreMadre);
                persona.Alias = mg.normaliza(persona.Alias);
                persona.Lnlocalidad = mg.normaliza(persona.Lnlocalidad);
                persona.Duracion = mg.normaliza(persona.Duracion);
                persona.DatosGeneralescol = mg.normaliza(persona.DatosGeneralescol);
                persona.EspecifiqueIdioma = mg.normaliza(persona.EspecifiqueIdioma);
                persona.EspecifiqueTraductor = mg.normaliza(persona.EspecifiqueTraductor);
                persona.ComIndigena = mg.normaliza(persona.ComIndigena);
                persona.ComLgbtttiq = mg.normaliza(persona.ComLgbtttiq);
                if (!(persona.Paterno == null && persona.Materno == null && persona.Nombre == null && persona.Genero == null && persona.Fnacimiento == null && persona.Lnestado == null))
                {
                    if (persona.Lnpais != "MEXICO")
                    {
                        persona.Lnestado = "33";
                    }
                    var curs = mg.sacaCurs(persona.Paterno, persona.Materno, persona.Fnacimiento, persona.Genero, persona.Lnestado, persona.Nombre);
                    persona.ClaveUnicaScorpio = curs;
                    persona.Curp = curs + "*";
                }
                persona.ConsumoSustancias = mg.normaliza(persona.ConsumoSustancias);
                persona.Familiares = mg.normaliza(persona.Familiares);
                persona.ReferenciasPersonales = mg.normaliza(persona.ReferenciasPersonales);
                persona.rutaFoto = mg.normaliza(persona.rutaFoto);
                persona.Capturista = persona.Capturista;
                persona.UbicacionExpediente = mg.normaliza(persona.UbicacionExpediente);
                if (persona.Candado == null) { persona.Candado = 0; }
                persona.Candado = persona.Candado;
                persona.MotivoCandado = mg.normaliza(persona.MotivoCandado);

                #region - sustancias agregadas -

                int idConsumoSustancias = ((from table in _context.Consumosustancias
                                            select table.IdConsumoSustancias).Max());
                if (arraySustancias != null)
                {
                    JArray sustancias = JArray.Parse(arraySustancias);

                    for (int i = 0; i < sustancias.Count; i = i + 5)
                    {
                        Consumosustancias consumosustanciasBD = new Consumosustancias();
                        persona.ConsumoSustancias = "SI";
                        consumosustanciasBD.Sustancia = sustancias[i].ToString();
                        consumosustanciasBD.Frecuencia = sustancias[i + 1].ToString();
                        consumosustanciasBD.Cantidad = mg.normaliza(sustancias[i + 2].ToString());
                        consumosustanciasBD.UltimoConsumo = mg.validateDatetime(sustancias[i + 3].ToString());
                        consumosustanciasBD.Observaciones = mg.normaliza(sustancias[i + 4].ToString());
                        consumosustanciasBD.PersonaIdPersona = id;
                        consumosustanciasBD.IdConsumoSustancias = ++idConsumoSustancias;
                        _context.Add(consumosustanciasBD);
                        await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                    }
                }
                #endregion

                #region - Sustancias editadas -

                if (arraySustanciasEditadas != null)
                {
                    JArray sustancias = JArray.Parse(arraySustanciasEditadas);

                    for (int i = 0; i < sustancias.Count; i = i + 6)
                    {
                        int idConsumo = Int32.Parse(sustancias[i + 5].ToString());
                        if (idConsumo < 0)
                        {
                            idConsumo = -idConsumo;
                            var sustancia = await _context.Consumosustancias.SingleOrDefaultAsync(m => m.IdConsumoSustancias == idConsumo);
                            _context.Consumosustancias.Remove(sustancia);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                        }
                        else
                        {
                            Consumosustancias consumosustanciasBD = new Consumosustancias();

                            consumosustanciasBD.Sustancia = sustancias[i].ToString();
                            consumosustanciasBD.Frecuencia = sustancias[i + 1].ToString();
                            consumosustanciasBD.Cantidad = mg.normaliza(sustancias[i + 2].ToString());
                            consumosustanciasBD.UltimoConsumo = mg.validateDatetime(sustancias[i + 3].ToString());
                            consumosustanciasBD.Observaciones = mg.normaliza(sustancias[i + 4].ToString());
                            consumosustanciasBD.IdConsumoSustancias = idConsumo;
                            consumosustanciasBD.PersonaIdPersona = id;

                            try
                            {
                                var oldconsumosustanciasBD = await _context.Consumosustancias.FindAsync(consumosustanciasBD.IdConsumoSustancias);
                                _context.Entry(oldconsumosustanciasBD).CurrentValues.SetValues(consumosustanciasBD);
                                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                            }
                            catch (DbUpdateConcurrencyException ex)
                            {

                               
                                if (!PersonaExists(consumosustanciasBD.PersonaIdPersona))
                                {
                                    return NotFound();
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region -Familiares, referencias agregados-

                int idAsientoFamiliar = ((from table in _context.Asientofamiliar
                                          select table.IdAsientoFamiliar).Max());
                if (arrayFamiliarReferencia != null)
                {
                    JArray familiarReferencia = JArray.Parse(arrayFamiliarReferencia);
                    for (int i = 0; i < familiarReferencia.Count; i = i + 14)
                    {
                        Asientofamiliar asientoFamiliar = new Asientofamiliar();
                        try
                        {
                            asientoFamiliar.IdAsientoFamiliar = ++idAsientoFamiliar;
                            asientoFamiliar.Nombre = mg.normaliza(familiarReferencia[i].ToString());
                            asientoFamiliar.Relacion = familiarReferencia[i + 1].ToString();
                            asientoFamiliar.Edad = Int32.Parse(familiarReferencia[i + 2].ToString());
                            asientoFamiliar.Sexo = familiarReferencia[i + 3].ToString();
                            asientoFamiliar.Dependencia = familiarReferencia[i + 4].ToString();
                            asientoFamiliar.DependenciaExplica = mg.normaliza(familiarReferencia[i + 5].ToString());
                            asientoFamiliar.VivenJuntos = familiarReferencia[i + 6].ToString();
                            asientoFamiliar.Domicilio = mg.normaliza(familiarReferencia[i + 7].ToString());
                            asientoFamiliar.Telefono = familiarReferencia[i + 8].ToString();
                            asientoFamiliar.HorarioLocalizacion = mg.normaliza(familiarReferencia[i + 9].ToString());
                            asientoFamiliar.EnteradoProceso = familiarReferencia[i + 10].ToString();
                            asientoFamiliar.PuedeEnterarse = familiarReferencia[i + 11].ToString();
                            asientoFamiliar.Observaciones = mg.normaliza(familiarReferencia[i + 12].ToString());
                            asientoFamiliar.Tipo = familiarReferencia[i + 13].ToString();
                            if (asientoFamiliar.Tipo.Equals("FAMILIAR"))
                            {
                                persona.Familiares = "SI";
                            }
                            else if (asientoFamiliar.Tipo.Equals("REFERENCIA"))
                            {
                                persona.ReferenciasPersonales = "SI";
                            }
                            asientoFamiliar.PersonaIdPersona = id;

                            _context.Add(asientoFamiliar);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                        }
                        catch (DbUpdateConcurrencyException ex)
                        {

                          
                            if (!PersonaExists(asientoFamiliar.PersonaIdPersona))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                      
                    }
                }
                #endregion

                #region -Familiares editados-
                if (arrayFamiliaresEditados != null)
                {
                    JArray familiarReferencia = JArray.Parse(arrayFamiliaresEditados);

                    for (int i = 0; i < familiarReferencia.Count; i = i + 14)
                    {
                        int idAsiento = Int32.Parse(familiarReferencia[i + 13].ToString());
                        if (idAsiento < 0)
                        {
                            idAsiento = -idAsiento;
                            var asiento = await _context.Asientofamiliar.SingleOrDefaultAsync(m => m.IdAsientoFamiliar == idAsiento);
                            _context.Asientofamiliar.Remove(asiento);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                        }
                        else
                        {
                            Asientofamiliar asientoFamiliar = new Asientofamiliar();

                            try
                            {
                                asientoFamiliar.IdAsientoFamiliar = Int32.Parse(familiarReferencia[i + 13].ToString());
                                asientoFamiliar.Nombre = mg.normaliza(familiarReferencia[i].ToString());
                                asientoFamiliar.Relacion = familiarReferencia[i + 1].ToString();
                                asientoFamiliar.Edad = Int32.Parse(familiarReferencia[i + 2].ToString());
                                asientoFamiliar.Sexo = familiarReferencia[i + 3].ToString();
                                asientoFamiliar.Dependencia = familiarReferencia[i + 4].ToString();
                                asientoFamiliar.DependenciaExplica = mg.normaliza(familiarReferencia[i + 5].ToString());
                                asientoFamiliar.VivenJuntos = familiarReferencia[i + 6].ToString();
                                asientoFamiliar.Domicilio = mg.normaliza(familiarReferencia[i + 7].ToString());
                                asientoFamiliar.Telefono = familiarReferencia[i + 8].ToString();
                                asientoFamiliar.HorarioLocalizacion = mg.normaliza(familiarReferencia[i + 9].ToString());
                                asientoFamiliar.EnteradoProceso = familiarReferencia[i + 10].ToString();
                                asientoFamiliar.PuedeEnterarse = familiarReferencia[i + 11].ToString();
                                asientoFamiliar.Observaciones = mg.normaliza(familiarReferencia[i + 12].ToString());
                                asientoFamiliar.Tipo = "FAMILIAR";
                                asientoFamiliar.PersonaIdPersona = id;

                                var oldAsientofamiliar = await _context.Asientofamiliar.FindAsync(asientoFamiliar.IdAsientoFamiliar);

                                _context.Entry(oldAsientofamiliar).CurrentValues.SetValues(asientoFamiliar);
                                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                            }
                            catch (DbUpdateConcurrencyException ex)
                            {

                                if (!PersonaExists(asientoFamiliar.PersonaIdPersona))
                                {
                                    return NotFound();
                                }
                                else
                                {
                                    throw;
                                }
                            }
                         
                        }
                    }
                }
                #endregion

                #region -Referencias editadas-

                if (arrayReferenciasEditadas != null)
                {
                    JArray familiarReferencia = JArray.Parse(arrayReferenciasEditadas);

                    for (int i = 0; i < familiarReferencia.Count; i = i + 14)
                    {
                        int idAsiento = Int32.Parse(familiarReferencia[i + 13].ToString());
                        if (idAsiento < 0)
                        {
                            idAsiento = -idAsiento;
                            var asiento = await _context.Asientofamiliar.SingleOrDefaultAsync(m => m.IdAsientoFamiliar == idAsiento);
                            _context.Asientofamiliar.Remove(asiento);
                            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                        }
                        else
                        {
                            Asientofamiliar asientoFamiliar = new Asientofamiliar();

                            try
                            {
                                asientoFamiliar.IdAsientoFamiliar = Int32.Parse(familiarReferencia[i + 13].ToString());
                                asientoFamiliar.Nombre = mg.normaliza(familiarReferencia[i].ToString());
                                asientoFamiliar.Relacion = familiarReferencia[i + 1].ToString();
                                asientoFamiliar.Edad = Int32.Parse(familiarReferencia[i + 2].ToString());
                                asientoFamiliar.Sexo = familiarReferencia[i + 3].ToString();
                                asientoFamiliar.Dependencia = familiarReferencia[i + 4].ToString();
                                asientoFamiliar.DependenciaExplica = mg.normaliza(familiarReferencia[i + 5].ToString());
                                asientoFamiliar.VivenJuntos = familiarReferencia[i + 6].ToString();
                                asientoFamiliar.Domicilio = mg.normaliza(familiarReferencia[i + 7].ToString());
                                asientoFamiliar.Telefono = familiarReferencia[i + 8].ToString();
                                asientoFamiliar.HorarioLocalizacion = mg.normaliza(familiarReferencia[i + 9].ToString());
                                asientoFamiliar.EnteradoProceso = familiarReferencia[i + 10].ToString();
                                asientoFamiliar.PuedeEnterarse = familiarReferencia[i + 11].ToString();
                                asientoFamiliar.Observaciones = mg.normaliza(familiarReferencia[i + 12].ToString());
                                asientoFamiliar.Tipo = "REFERENCIA";
                                asientoFamiliar.PersonaIdPersona = id;

                                var oldAsientofamiliar = await _context.Asientofamiliar.FindAsync(asientoFamiliar.IdAsientoFamiliar);

                                _context.Entry(oldAsientofamiliar).CurrentValues.SetValues(asientoFamiliar);
                                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value, 1);
                            }
                            catch (DbUpdateConcurrencyException ex)
                            {
                                if (!PersonaExists(asientoFamiliar.PersonaIdPersona))
                                {
                                    return NotFound();
                                }
                                else
                                {
                                    throw;
                                }
                            }
                         
                        }
                    }
                }
                #endregion

                try
                {

                    //#region -Expediente Unico-
                    //expedienteunico.ClaveUnicaScorpio = persona.ClaveUnicaScorpio;
                    //expedienteunico.Persona = idPersona.ToString();
                    //_context.Add(expedienteunico);
                    //#endregion 

                    var oldPersona = await _context.Persona.FindAsync(id);
                    _context.Entry(oldPersona).CurrentValues.SetValues(persona);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!PersonaExists(persona.IdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            
                return RedirectToAction("MenuEdicion/" + persona.IdPersona, "Personas");
            }
            return View(persona);
        }

        public async Task<IActionResult> actualizarUbicacion(Archivointernomcscp archivointernomcscp, string ubicacion, int idPersona, string usuario)
        {
            var persona = (from a in _context.Persona
                           where a.IdPersona == idPersona
                           select a).FirstOrDefault();
            persona.UbicacionExpediente = ubicacion;
            var oldPersona = await _context.Persona.FindAsync(idPersona);
            _context.Entry(oldPersona).CurrentValues.SetValues(persona);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);


            var utimoidarchivo = (from a in _context.Archivointernomcscp
                                  group a by a.PersonaIdPersona into grp
                                  select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();

            var filter = (from p in _context.Persona
                          join a in utimoidarchivo on p.IdPersona equals a.PersonaIdPersona
                          where a.NuevaUbicacion != null && a.PersonaIdPersona == persona.IdPersona
                          select a).FirstOrDefault();

            filter.Usuario = archivointernomcscp.Usuario.ToUpper();
            _context.SaveChanges();



            return Json(new { success = true });
        }
        #endregion

        #region -Edita Domicilio-
        public async Task<IActionResult> EditDomicilio(string nombre, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.idPersona = id;
            ViewData["Nombre"] = nombre;
            var domicilio = await _context.Domicilio.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);

            #region TIPODOMICILIO          
            List<SelectListItem> LiatatDomicilio;
            LiatatDomicilio = new List<SelectListItem>
            {
              new SelectListItem{ Text="Rentada", Value="RENTADA"},
              new SelectListItem{ Text="Prestada", Value="PRESTADA"},
              new SelectListItem{ Text="Propia", Value="PROPIA"},
              new SelectListItem{ Text="Familiar", Value="FAMILIAR"},
              new SelectListItem{ Text="Situación de calle", Value="SITUACION DE CALLE"},
              new SelectListItem{ Text="Irregular", Value="IRREGULAR"},
            };

            ViewBag.listatDomicilio = LiatatDomicilio;

            foreach (var item in LiatatDomicilio)
            {
                if (item.Value == domicilio.TipoDomicilio)
                {
                    ViewBag.idtDomicilio = item.Value;
                    break;
                }
            }
            #endregion


            #region PAIS          
            List<SelectListItem> ListaPaisD;
            ListaPaisD = new List<SelectListItem>
            {
              new SelectListItem{ Text="México", Value="MEXICO"},
              new SelectListItem{ Text="Estados Unidos", Value="ESTADOS UNIDOS"},
              new SelectListItem{ Text="Canada", Value="CANADA"},
              new SelectListItem{ Text="Colombia", Value="COLOMBIA"},
              new SelectListItem{ Text="El Salvador", Value="EL SALVADOR"},
              new SelectListItem{ Text="Guatemala", Value="GUATEMALA"},
              new SelectListItem{ Text="Chile", Value="CHILE"},
              new SelectListItem{ Text="Argentina", Value="ARGENTINA"},
              new SelectListItem{ Text="Brasil", Value="BRASIL"},
              new SelectListItem{ Text="Venezuela", Value="VENEZUELA"},
              new SelectListItem{ Text="Puerto Rico", Value="PUERTO RICO"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListaPaisD = ListaPaisD;

            foreach (var item in ListaPaisD)
            {
                if (item.Value == domicilio.Pais)
                {
                    ViewBag.idPaisD = item.Value;
                    break;
                }
            }
            #endregion

            #region Destado
            List<Estados> listaEstadosD = new List<Estados>();
            listaEstadosD = (from table in _context.Estados
                             select table).ToList();

            ViewBag.ListaEstadoD = listaEstadosD;
            ViewBag.idEstadoD = domicilio.Estado;
            #endregion

            #region Lnmunicipio
            int estadoD;
            bool success = Int32.TryParse(domicilio.Estado, out estadoD);
            List<Municipios> listaMunicipiosD = new List<Municipios>();
            if (success)
            {
                listaMunicipiosD = (from table in _context.Municipios
                                    where table.EstadosId == estadoD
                                    select table).ToList();
            }

            listaMunicipiosD.Insert(0, new Municipios { Id = 0, Municipio = "Sin municipio" });
            ViewBag.ListaMunicipioD = listaMunicipiosD;
            ViewBag.idMunicipioD = domicilio.Municipio;
            ViewBag.MunicipioD = "Sin municipio";
            for (int i = 0; i < listaMunicipiosD.Count; i++)
            {
                if (listaMunicipiosD[i].Id.ToString() == domicilio.Municipio)
                {
                    ViewBag.MunicipioD = listaMunicipiosD[i].Municipio;
                }
            }
            #endregion

            #region TemporalidadDomicilio
            List<SelectListItem> ListaDomicilioT;
            ListaDomicilioT = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Más de 10 años", Value = "MAS DE 10 AÑOS" },
                new SelectListItem{ Text = "Entre 5 y 10 años", Value = "ENTRE 5 Y 10 AÑOS" },
                new SelectListItem{ Text = "Entre 2 y 5 años", Value = "ENTRE 2 Y 5 AÑOS" },
                new SelectListItem{ Text = "Entre 6 meses y 2 años", Value = "ENTRE 6 MESES Y 2 AÑOS" },
                new SelectListItem{ Text = "Menos de 6 meses", Value = "MENOS DE 6 MESES" },
            };

            ViewBag.ListaTemporalidad = ListaDomicilioT;
            ViewBag.idTemporalidadD = BuscaId(ListaDomicilioT, domicilio.Temporalidad);
            #endregion

            ViewBag.listaResidenciaHabitual = listaSiNo;
            ViewBag.idResidenciaHabitual = BuscaId(listaSiNo, domicilio.ResidenciaHabitual);

            ViewBag.listacuentaDomicilioSecundario = listaNoSi;
            ViewBag.idcuentaDomicilioSecundario = BuscaId(listaNoSi, domicilio.DomcilioSecundario);

            ViewBag.listaZona = listaZonas;
            ViewBag.zona = BuscaId(listaZonas, domicilio.Zona);

            ViewBag.pais = domicilio.Pais;

            ViewBag.domi = domicilio.DomcilioSecundario;

            var colonias = from p in _context.Zonas
                           orderby p.Colonia
                           select p;
            ViewBag.colonias = colonias.ToList();
            ViewBag.colonia = domicilio.NombreCf;

            var colonia = domicilio.NombreCf;


            List<Zonas> zonasList = new List<Zonas>();
            zonasList = (from Zonas in _context.Zonas
                         select Zonas).ToList();
            ViewBag.idZona = 1;//first selected by default
            for (int i = 0; i < zonasList.Count; i++)
            {
                if (zonasList[i].Colonia.ToString().ToUpper() == domicilio.NombreCf.ToUpper())
                {
                    ViewBag.idZona = zonasList[i].Idzonas;
                }
            }

            if (domicilio == null)
            {
                return NotFound();
            }
            return View(domicilio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDomicilio(int id, [Bind("IdDomicilio,TipoDomicilio,Calle,No,TipoUbicacion,NombreCf,Pais,Estado,Municipio,Temporalidad,ResidenciaHabitual,Cp,Referencias,Horario,DomcilioSecundario,Observaciones,Zona,Lat,Lng,PersonaIdPersona,Zona")] Domicilio domicilio, string inputAutocomplete)
        {
            if (id != domicilio.PersonaIdPersona)
            {
                return NotFound();
            }

            domicilio.Calle = mg.normaliza(domicilio.Calle);
            domicilio.No = String.IsNullOrEmpty(domicilio.No) ? domicilio.No : domicilio.No.ToUpper();
            //domicilio.Cp = domicilio.Cp;
            domicilio.Referencias = mg.normaliza(domicilio.Referencias);
            domicilio.Horario = mg.normaliza(domicilio.Horario);
            domicilio.Observaciones = mg.normaliza(domicilio.Observaciones);
            //domicilio.Lat = domicilio.Lat;
            //domicilio.Lng = domicilio.Lng;

            domicilio.NombreCf = mg.normaliza(inputAutocomplete);

            List<Zonas> zonasList = new List<Zonas>();
            zonasList = (from Zonas in _context.Zonas
                         select Zonas).ToList();

            domicilio.Zona = "SIN ZONA ASIGNADA";
            int matches = 0;
            for (int i = 0; i < zonasList.Count; i++)
            {
                if (zonasList[i].Colonia.ToUpper() == domicilio.NombreCf)
                {
                    matches++;
                }
            }
            for (int i = 0; i < zonasList.Count; i++)
            {
                if (zonasList[i].Colonia.ToUpper() == domicilio.NombreCf && (matches <= 1 || zonasList[i].Cp == domicilio.Cp))
                {
                    domicilio.Zona = zonasList[i].Zona.ToUpper();
                }
            }

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                try
                {
                    var oldDomicilio = await _context.Domicilio.FindAsync(domicilio.IdDomicilio);
                    _context.Entry(oldDomicilio).CurrentValues.SetValues(domicilio);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(domicilio);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!PersonaExists(domicilio.PersonaIdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
              
                return RedirectToAction("MenuEdicion/" + domicilio.PersonaIdPersona, "Personas");
            }
            return View(domicilio);
        }
        #endregion

        #region Edit Dmicilio Secundario

        public async Task<IActionResult> EditDomSecundario2(int? id, string nombre, string idPersona)
        {
            int index = idPersona.IndexOf("?");
            if (index >= 0)
                idPersona = idPersona.Substring(0, index);

            ViewBag.idPersona = idPersona;
            ViewBag.nombre = nombre;

            if (id == null)
            {
                return NotFound();
            }

            var domisecu = await _context.Domicilio.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);

            #region -To List databases-
            List<Persona> personaVM = _context.Persona.ToList();
            List<Domicilio> domicilioVM = _context.Domicilio.ToList();
            List<Domiciliosecundario> domiciliosecundarioVM = _context.Domiciliosecundario.ToList();
            #endregion

            #region -Jointables-
            ViewData["joinTablesDomcilioSec"] = from personaTable in personaVM
                                                join domicilio in domicilioVM on personaTable.IdPersona equals domicilio.IdDomicilio
                                                join domicilioSec in domiciliosecundarioVM on domicilio.IdDomicilio equals domicilioSec.IdDomicilio
                                                where personaTable.IdPersona == id
                                                select new PersonaViewModel
                                                {
                                                    domicilioSecundarioVM = domicilioSec
                                                };
            #endregion

            #region TIPODOMICILIO          
            List<SelectListItem> LiatatDomicilio;
            LiatatDomicilio = new List<SelectListItem>
            {
              new SelectListItem{ Text="Rentada", Value="RENTADA"},
              new SelectListItem{ Text="Prestada", Value="PRESTADA"},
              new SelectListItem{ Text="Propia", Value="PROPIA"},
              new SelectListItem{ Text="Familiar", Value="FAMILIAR"},
              new SelectListItem{ Text="Situación de calle", Value="SITUACION DE CALLE"},
              new SelectListItem{ Text="Irregular", Value="IRREGULAR"},
            };

            ViewBag.listatDomicilio = LiatatDomicilio;
            ViewBag.idtDomicilio = BuscaId(LiatatDomicilio, domisecu.TipoDomicilio);
            #endregion

            #region PAIS          
            List<SelectListItem> ListaPaisD;
            ListaPaisD = new List<SelectListItem>
            {
              new SelectListItem{ Text="México", Value="MEXICO"},
              new SelectListItem{ Text="Estados Unidos", Value="ESTADOS UNIDOS"},
              new SelectListItem{ Text="Canada", Value="CANADA"},
              new SelectListItem{ Text="Colombia", Value="COLOMBIA"},
              new SelectListItem{ Text="El Salvador", Value="EL SALVADOR"},
              new SelectListItem{ Text="Guatemala", Value="GUATEMALA"},
              new SelectListItem{ Text="Chile", Value="CHILE"},
              new SelectListItem{ Text="Argentina", Value="ARGENTINA"},
              new SelectListItem{ Text="Brasil", Value="BRASIL"},
              new SelectListItem{ Text="Venezuela", Value="VENEZUELA"},
              new SelectListItem{ Text="Puerto Rico", Value="PUERTO RICO"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListaPaisED = ListaPaisD;
            ViewBag.idPaisED = BuscaId(ListaPaisD, domisecu.Pais);

            ViewBag.ListaPaisM = ListaPaisD;
            ViewBag.idPaisM = BuscaId(ListaPaisD, domisecu.Pais);
            #endregion

            #region Destado
            List<Estados> listaEstadosD = new List<Estados>();
            listaEstadosD = (from table in _context.Estados
                             select table).ToList();

            List<Domiciliosecundario> listadomiciliosecundarios = new List<Domiciliosecundario>();
            listadomiciliosecundarios = (from table in _context.Domiciliosecundario
                                         select table).ToList();


            listaEstadosD.Insert(0, new Estados { Id = 0, Estado = "Selecciona" });
            ViewBag.ListaEstadoED = listaEstadosD;
            ViewBag.idEstadoED = domisecu.Estado;

            ViewBag.ListaEstadoM = listaEstadosD;
            ViewBag.idEstadoM = domisecu.Estado;
            #endregion

            #region Lnmunicipio
            int estadoD;
            bool success = Int32.TryParse(domisecu.Estado, out estadoD);
            List<Municipios> listaMunicipiosD = new List<Municipios>();
            if (success)
            {
                listaMunicipiosD = (from table in _context.Municipios
                                    where table.EstadosId == estadoD
                                    select table).ToList();
            }

            ViewBag.ListaMunicipioED = listaMunicipiosD;
            ViewBag.idMunicipioED = domisecu.Municipio;

            ViewBag.ListaMunicipioM = listaMunicipiosD;
            ViewBag.idMunicipioM = domisecu.Municipio;

            ViewBag.Pais = domisecu.Pais;
            #endregion

            #region TemporalidadDomicilio
            List<SelectListItem> ListaDomicilioT;
            ListaDomicilioT = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Más de 10 años", Value = "MAS DE 10 AÑOS" },
                new SelectListItem{ Text = "Entre 5 y 10 años", Value = "ENTRE 5 Y 10 AÑOS" },
                new SelectListItem{ Text = "Entre 2 y 5 años", Value = "ENTRE 2 Y 5 AÑOS" },
                new SelectListItem{ Text = "Entre 6 meses y 2 año", Value = "ENTRE 6 MESES Y 2 AÑO" },
                new SelectListItem{ Text = "Menos de 6 meses", Value = "MENOS DE 6 MESES" },
            };

            ViewBag.ListaTemporalidad = ListaDomicilioT;
            ViewBag.idTemporalidad = BuscaId(ListaDomicilioT, domisecu.Temporalidad);



            ViewBag.listaResidenciaHabitual = listaSiNo;
            ViewBag.idResidenciaHabitual = BuscaId(listaSiNo, domisecu.ResidenciaHabitual);

            #endregion
            if (domisecu == null)
            {
                return NotFound();
            }

            return View(domisecu);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDomSecundario([Bind("IdDomicilioSecundario,IdDomicilio,TipoDomicilio,Calle,No,TipoUbicacion,NombreCf,Pais,Estado,Municipio,Temporalidad,ResidenciaHabitual,Cp,Referencias,Horario,Motivo,Observaciones")] Domiciliosecundario domiciliosecundario, string nombre, string idPersona)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                try
                {

                    domiciliosecundario.TipoUbicacion = mg.normaliza(domiciliosecundario.TipoUbicacion);
                    domiciliosecundario.Calle = mg.normaliza(domiciliosecundario.Calle);
                    domiciliosecundario.No = mg.normaliza(domiciliosecundario.No);
                    domiciliosecundario.NombreCf = mg.normaliza(domiciliosecundario.NombreCf);
                    domiciliosecundario.Cp = mg.normaliza(domiciliosecundario.Cp);
                    domiciliosecundario.Referencias = mg.normaliza(domiciliosecundario.Referencias);
                    domiciliosecundario.Horario = mg.normaliza(domiciliosecundario.Horario);
                    domiciliosecundario.Motivo = mg.normaliza(domiciliosecundario.Motivo);
                    domiciliosecundario.Observaciones = mg.normaliza(domiciliosecundario.Observaciones);


                    var oldDomicilio = await _context.Domiciliosecundario.FindAsync(domiciliosecundario.IdDomicilioSecundario);
                    _context.Entry(oldDomicilio).CurrentValues.SetValues(domiciliosecundario);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(domiciliosecundario);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                   
                    //if (!DomiciliosecundarioExists(domiciliosecundario.IdDomicilioSecundario))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }
                
                return RedirectToAction("EditDomicilio/" + idPersona, "Personas", new { nombre = nombre });
            }
            return View();
        }

        public async Task<IActionResult> DeleteConfirmedDom(int? id, int idpersona)
        {
            var domseundario = await _context.Domiciliosecundario.SingleOrDefaultAsync(m => m.IdDomicilioSecundario == id);
            _context.Domiciliosecundario.Remove(domseundario);
            await _context.SaveChangesAsync();

            var empty = (from ds in _context.Domiciliosecundario
                         where ds.IdDomicilio == domseundario.IdDomicilio
                         select ds);

            if (!empty.Any())
            {
                var query = (from a in _context.Domicilio
                             where a.IdDomicilio == domseundario.IdDomicilio
                             select a).FirstOrDefault();
                query.DomcilioSecundario = "NO";
                _context.SaveChanges();
            }

            return RedirectToAction("EditDomicilio/" + idpersona, "Personas");
        }

        public async Task<IActionResult> CrearDomicilioSecundario(Domiciliosecundario domiciliosecundario, string[] datosDomicilio)
        {
            domiciliosecundario.IdDomicilio = Int32.Parse(datosDomicilio[0]);
            domiciliosecundario.TipoDomicilio = mg.normaliza(datosDomicilio[1]);
            domiciliosecundario.Calle = mg.normaliza(datosDomicilio[2]);
            domiciliosecundario.No = mg.normaliza(datosDomicilio[3]);
            domiciliosecundario.TipoUbicacion = mg.normaliza(datosDomicilio[4]);
            domiciliosecundario.NombreCf = mg.normaliza(datosDomicilio[5]);
            domiciliosecundario.Pais = datosDomicilio[6];
            domiciliosecundario.Estado = datosDomicilio[7];
            domiciliosecundario.Municipio = datosDomicilio[8];
            domiciliosecundario.Temporalidad = mg.normaliza(datosDomicilio[9]);
            domiciliosecundario.ResidenciaHabitual = mg.normaliza(datosDomicilio[10]);
            domiciliosecundario.Cp = mg.normaliza(datosDomicilio[11]);
            domiciliosecundario.Referencias = mg.normaliza(datosDomicilio[12]);
            domiciliosecundario.Motivo = mg.normaliza(datosDomicilio[13]);
            domiciliosecundario.Horario = mg.normaliza(datosDomicilio[14]);
            domiciliosecundario.Observaciones = mg.normaliza(datosDomicilio[15]);


            var query = (from a in _context.Domicilio
                         where a.IdDomicilio == domiciliosecundario.IdDomicilio
                         select a).FirstOrDefault();
            query.DomcilioSecundario = "SI";
            _context.SaveChanges();

            var query2 = (from p in _context.Persona
                          join d in _context.Domicilio on p.IdPersona equals d.IdDomicilio
                          join ds in _context.Domiciliosecundario on d.IdDomicilio equals ds.IdDomicilio
                          where ds.IdDomicilioSecundario == domiciliosecundario.IdDomicilio
                          select p);



            _context.Add(domiciliosecundario);
            await _context.SaveChangesAsync();

            return RedirectToAction("EditDomicilio/" + query.PersonaIdPersona, "Personas");
        }
        #endregion

        #region -Edita Escolaridad-
        public async Task<IActionResult> EditEscolaridad(string nombre, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Nombre"] = nombre;
            var estudios = await _context.Estudios.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);
            if (estudios == null)
            {
                return NotFound();
            }

            ViewBag.estudia = estudios.Estudia;
            ViewBag.listaEstudia = listaNoSi;
            ViewBag.idEstudia = BuscaId(listaNoSi, estudios.Estudia);

            #region GradoEstudios
            List<SelectListItem> ListaGradoEstudios;
            ListaGradoEstudios = new List<SelectListItem>
            {
              new SelectListItem{ Text="Primaria", Value="PRIMARIA"},
              new SelectListItem{ Text="Secundaria", Value="SECUNDARIA"},
              new SelectListItem{ Text="Bachillerato", Value="BACHILLERATO"},
              new SelectListItem{ Text="TSU", Value="TSU"},
              new SelectListItem{ Text="Licenciatura", Value="LICENCIATURA"},
              new SelectListItem{ Text="Maestría", Value="MAESTRÍA"},
              new SelectListItem{ Text="Doctorado", Value="DOCTORADO"}
            };

            ViewBag.listaGradoEstudios = ListaGradoEstudios;
            ViewBag.idGradoEstudios = BuscaId(ListaGradoEstudios, estudios.GradoEstudios);
            #endregion


            List<Estudios> estudiosVM = _context.Estudios.ToList();
            List<Persona> personaVM = _context.Persona.ToList();


            ViewData["joinTablesPersonaEstudia"] =
                                     from personaTable in personaVM
                                     join estudiosTabla in estudiosVM on personaTable.IdPersona equals estudiosTabla.PersonaIdPersona
                                     where personaTable.IdPersona == idPersona
                                     select new PersonaViewModel
                                     {
                                         personaVM = personaTable,
                                         estudiosVM = estudiosTabla

                                     };

            if ((ViewData["joinTablesPersonaEstudia"] as IEnumerable<scorpioweb.Models.PersonaViewModel>).Count() == 0)
            {
                ViewBag.RA = false;
            }
            else
            {
                ViewBag.RA = true;
            }


            return View(estudios);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEscolaridad(int id, [Bind("IdEstudios,Estudia,GradoEstudios,InstitucionE,Horario,Direccion,Telefono,Observaciones,PersonaIdPersona")] Estudios estudios)
        {
            if (id != estudios.PersonaIdPersona)
            {
                return NotFound();
            }

            estudios.InstitucionE = mg.normaliza(estudios.InstitucionE);
            estudios.Horario = mg.normaliza(estudios.Horario);
            estudios.Direccion = mg.normaliza(estudios.Direccion);
            estudios.Observaciones = mg.normaliza(estudios.Observaciones);



            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                try
                {
                    var oldEstudios = await _context.Estudios.FindAsync(estudios.IdEstudios);
                    _context.Entry(oldEstudios).CurrentValues.SetValues(estudios);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(estudios);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                   
                    if (!PersonaExists(estudios.PersonaIdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            
                return RedirectToAction("MenuEdicion/" + estudios.PersonaIdPersona, "Personas");
            }
            return View(estudios);
        }
        #endregion

        #region -Edita Trabajo-
        public async Task<IActionResult> EditTrabajo(string nombre, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Nombre"] = nombre;
            var trabajo = await _context.Trabajo.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);
            if (trabajo == null)
            {
                return NotFound();
            }

            ViewBag.listaTrabaja = listaSiNo;
            ViewBag.idTrabaja = BuscaId(listaSiNo, trabajo.Trabaja);

            #region TipoOcupacion
            List<SelectListItem> ListaTipoOcupacion;
            ListaTipoOcupacion = new List<SelectListItem>
            {
                new SelectListItem{ Text = "NA", Value = "NA" },
                new SelectListItem{ Text = "Funcionarios, directores y jefes", Value = "FUNCIONARIO, DIRECTOR Y JEFE" },
                new SelectListItem{ Text = "Profesionistas y técnicos", Value = "PROFESIONISTA O TÉCNICO" },
                new SelectListItem{ Text = "Trabajadores auxiliares en actividades administrativas", Value = "TRABAJADORES AUXILIARES EN ACTIVIDADES ADMINISTRATIVAS" },
                new SelectListItem{ Text = "Comerciantes, empleados en ventas y agentes de ventas", Value = "COMERCIANTES, EMPLEADOS DE VENTA Y AGENTES DE VENTAS" },
                new SelectListItem{ Text = "Trabajadores en servicios personales y vigilancia", Value = "TRABAJADORES EN SERVICIOS PERSONALES Y VIGILANCIA" },
                new SelectListItem{ Text = "Trabajadores en actividades agrícolas, ganaderas, forestales, caza y pesca", Value = "TRABAJADORES EN ACTIVIDADES AGRICOLAS, GANADERAS, FORESTALES, CAZA Y PESCA" },
                new SelectListItem{ Text = "Trabajadores artesanales", Value = "TRABAJADORES ARTESANALES" },
                new SelectListItem{ Text = "Operadores de maquinaria industrial, ensambladores, choferes y conductores de transporte", Value = "OPERADORES DE MAQUINARIA INDUSTRIAL, ENSAMBLADORES, CHOFERES Y CONDUCTORES DE TRANSPORTE" },
                new SelectListItem{ Text = "Trabajadores en actividades elementales y de apoyo", Value = "TRABAJADORES EN ACTIVIDADES ELEMENTALES Y DE APOYO" }
            };

            ViewBag.listaTipoOcupacion = ListaTipoOcupacion;
            ViewBag.idTipoOcupacion = BuscaId(ListaTipoOcupacion, trabajo.TipoOcupacion);
            #endregion

            ViewBag.listaEnteradoProceso = listaNoSiNA;
            ViewBag.idEnteradoProceso = BuscaId(listaNoSiNA, trabajo.EnteradoProceso);

            ViewBag.listasePuedeEnterarT = listaNoSiNA;
            ViewBag.idsePuedeEnterart = BuscaId(listaNoSiNA, trabajo.SePuedeEnterar);
            ViewBag.trabaja = trabajo.Trabaja;

            #region TiempoTrabajando
            List<SelectListItem> ListaTiempoTrabajando;
            ListaTiempoTrabajando = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Más de 10 años", Value = "MAS DE 10 AÑOS" },
                new SelectListItem{ Text = "Entre 2 y 4 años", Value = "ENTRE 2 Y 4 AÑOS" },
                new SelectListItem{ Text = "Entre 5 y 9 años", Value = "ENTRE 5 Y 9 AÑOS" },
                new SelectListItem{ Text = "Más de un año menos de 2", Value = "MAS DE UN AÑO Y MENOS DE 2" },
                new SelectListItem{ Text = "Entre 6 meses y 1 año", Value = "ENTRE 6 MESES Y 1 AÑO" },
                new SelectListItem{ Text = "Menos de 6 meses", Value = "MENOS DE 6 MESES" },
                new SelectListItem{ Text = "Tiene menos de 3 meses sin actividad laboral", Value = "SIN TRABAJO DURANTE MENOS DE 3 MESES" },
                new SelectListItem{ Text = "Tiene más de 3 meses sin actividad laboral", Value = "SIN TRABAJO DURANTE MAS DE 3 MESES" }
            };

            ViewBag.listaTiempoTrabajando = ListaTiempoTrabajando;
            ViewBag.idTiempoTrabajando = BuscaId(ListaTiempoTrabajando, trabajo.TiempoTrabajano);
            #endregion

            #region TemporalidadSalario
            List<SelectListItem> ListaTemporalidadSalario;
            ListaTemporalidadSalario = new List<SelectListItem>
            {
                new SelectListItem{ Text = "NA", Value = "NA" },
                new SelectListItem{ Text = "Diario", Value = "DIARIO" },
                new SelectListItem{ Text = "Semanal", Value = "SEMANAL" },
                new SelectListItem{ Text = "Quincenal", Value = "QUINCENAL" },
                new SelectListItem{ Text = "Mensual", Value = "MENSUAL" }
            };

            ViewBag.listaTemporalidadSalario = ListaTemporalidadSalario;
            ViewBag.idTemporalidadSalario = BuscaId(ListaTemporalidadSalario, trabajo.TemporalidadSalario);
            #endregion

            return View(trabajo);
        }

        // POST: Trabajoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrabajo(int id, [Bind("IdTrabajo,Trabaja,TipoOcupacion,Puesto,EmpledorJefe,EnteradoProceso,SePuedeEnterar,TiempoTrabajano,Salario,TemporalidadSalario,Direccion,Horario,Telefono,Observaciones,PersonaIdPersona")] Trabajo trabajo)
        {
            if (id != trabajo.PersonaIdPersona)
            {
                return NotFound();
            }

            trabajo.Puesto = mg.normaliza(trabajo.Puesto);
            trabajo.EmpledorJefe = mg.normaliza(trabajo.EmpledorJefe);
            trabajo.Salario = mg.normaliza(trabajo.Salario);
            trabajo.TemporalidadSalario = mg.normaliza(trabajo.TemporalidadSalario);
            trabajo.Direccion = mg.normaliza(trabajo.Direccion);
            trabajo.Horario = mg.normaliza(trabajo.Horario);
            trabajo.Observaciones = mg.normaliza(trabajo.Observaciones);

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                try
                {
                    var oldTrabajo = await _context.Trabajo.FindAsync(trabajo.IdTrabajo);
                    _context.Entry(oldTrabajo).CurrentValues.SetValues(trabajo);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(trabajo);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!PersonaExists(trabajo.PersonaIdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
               
                return RedirectToAction("MenuEdicion/" + trabajo.PersonaIdPersona, "Personas");
            }
            return View(trabajo);
        }
        #endregion

        #region -Edita Actividades Sociales-
        public async Task<IActionResult> EditActividadesSociales(string nombre, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Nombre"] = nombre;
            var actividadsocial = await _context.Actividadsocial.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);

            ViewBag.listasePuedeEnterarASr = listaNoSiNA;
            ViewBag.idsePuedeEnterarAS = BuscaId(listaNoSiNA, actividadsocial.SePuedeEnterar);

            if (actividadsocial == null)
            {
                return NotFound();
            }
            return View(actividadsocial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditActividadesSociales(int id, [Bind("IdActividadSocial,TipoActividad,Horario,Lugar,Telefono,SePuedeEnterar,Referencia,Observaciones,PersonaIdPersona")] Actividadsocial actividadsocial)
        {
            if (id != actividadsocial.PersonaIdPersona)
            {
                return NotFound();
            }

            actividadsocial.TipoActividad = mg.normaliza(actividadsocial.TipoActividad);
            actividadsocial.Horario = mg.normaliza(actividadsocial.Horario);
            actividadsocial.Lugar = mg.normaliza(actividadsocial.Lugar);
            actividadsocial.Referencia = mg.normaliza(actividadsocial.Referencia);
            actividadsocial.Observaciones = mg.normaliza(actividadsocial.Observaciones);



            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                try
                {

                    var oldActividadsocial = await _context.Actividadsocial.FindAsync(actividadsocial.IdActividadSocial);
                    _context.Entry(oldActividadsocial).CurrentValues.SetValues(actividadsocial);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(actividadsocial);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {

                     if (!PersonaExists(actividadsocial.PersonaIdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
              
                return RedirectToAction("MenuEdicion/" + actividadsocial.PersonaIdPersona, "Personas");
            }
            return View(actividadsocial);
        }
        #endregion

        #region -Edita Abandono Estado-
        public async Task<IActionResult> EditAbandonoEstado(string nombre, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Nombre"] = nombre;
            var abandonoestado = await _context.Abandonoestado.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);

            ViewBag.listaVividoFuera = listaNoSi;
            ViewBag.idVividoFuera = BuscaId(listaNoSi, abandonoestado.VividoFuera);

            ViewBag.listaViajaHabitual = listaNoSi;
            ViewBag.idViajaHabitual = BuscaId(listaNoSi, abandonoestado.ViajaHabitual);

            ViewBag.listaDocumentacionSalirPais = listaNoSi;
            ViewBag.idDocumentacionSalirPais = BuscaId(listaNoSi, abandonoestado.DocumentacionSalirPais);

            ViewBag.listaPasaporte = listaNoSi;
            ViewBag.idPasaporte = BuscaId(listaNoSi, abandonoestado.Pasaporte);

            ViewBag.listaVisa = listaNoSi;
            ViewBag.idVisa = BuscaId(listaNoSi, abandonoestado.Visa);

            ViewBag.listaFamiliaresFuera = listaNoSi;
            ViewBag.idFamiliaresFuera = BuscaId(listaNoSi, abandonoestado.FamiliaresFuera);

            ViewBag.vfuera = abandonoestado.VividoFuera;
            ViewBag.vlugar = abandonoestado.ViajaHabitual;
            ViewBag.document = abandonoestado.DocumentacionSalirPais;
            ViewBag.Abandono = abandonoestado.FamiliaresFuera;



            if (abandonoestado == null)
            {
                return NotFound();
            }
            return View(abandonoestado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAbandonoEstado(int id, [Bind("IdAbandonoEstado,VividoFuera,LugaresVivido,TiempoVivido,MotivoVivido,ViajaHabitual,LugaresViaje,TiempoViaje,MotivoViaje,DocumentacionSalirPais,Pasaporte,Visa,FamiliaresFuera,Cuantos,PersonaIdPersona")] Abandonoestado abandonoestado, string arrayFamExtranjero, string arrayFamExtranjerosEditados)
        {
            if (id != abandonoestado.PersonaIdPersona)
            {
                return NotFound();
            }

            abandonoestado.LugaresVivido = mg.normaliza(abandonoestado.LugaresVivido);
            abandonoestado.TiempoVivido = mg.normaliza(abandonoestado.TiempoVivido);
            abandonoestado.MotivoVivido = mg.normaliza(abandonoestado.MotivoVivido);
            abandonoestado.LugaresViaje = mg.normaliza(abandonoestado.LugaresViaje);
            abandonoestado.TiempoViaje = mg.normaliza(abandonoestado.TiempoViaje);
            abandonoestado.MotivoViaje = mg.normaliza(abandonoestado.MotivoViaje);




            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                try
                {
                    var oldAbandonoestado = await _context.Abandonoestado.FindAsync(abandonoestado.IdAbandonoEstado);
                    _context.Entry(oldAbandonoestado).CurrentValues.SetValues(abandonoestado);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException ex)
                {

                    
                    if (!PersonaExists(abandonoestado.PersonaIdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            
                return RedirectToAction("MenuEdicion/" + abandonoestado.PersonaIdPersona, "Personas");
            }
            return View(abandonoestado);
        }
        #endregion

        #region -EditFamiliaresForaneos-
        public async Task<IActionResult> EditFamiliaresForaneos(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var familiaresforaneos = await _context.Familiaresforaneos.Where(m => m.PersonaIdPersona == id).FirstOrDefaultAsync();
            ViewBag.idFamiliarF = familiaresforaneos.PersonaIdPersona;

            #region GENERO          
            List<SelectListItem> ListaGenero;
            ListaGenero = new List<SelectListItem>
            {
              new SelectListItem{ Text="Masculino", Value="M"},
              new SelectListItem{ Text="Femenino", Value="F"},
              new SelectListItem{ Text="Prefiero no decirlo", Value="N"},
            };
            ViewBag.listaGenero = ListaGenero;
            ViewBag.idGenero = BuscaId(ListaGenero, familiaresforaneos.Sexo);
            #endregion

            #region Relacion
            List<SelectListItem> ListaRelacion;
            ListaRelacion = new List<SelectListItem>
            {
              new SelectListItem{ Text="Máma", Value="MAMA"},
              new SelectListItem{ Text="Pápa", Value="PAPA"},
              new SelectListItem{ Text="Esposo (a)", Value="ESPOSO (A)"},
              new SelectListItem{ Text="Hermano (a)", Value="HERMAN0 (A)"},
              new SelectListItem{ Text="Hijo (a)", Value="HIJO (A)"},
              new SelectListItem{ Text="Abelo (a)", Value="ABUELO (A)"},
              new SelectListItem{ Text="Familiar 1 Nivel", Value="FAMILIAR 1 NIVEL"},
              new SelectListItem{ Text="Amigo", Value="AMIGO"},
              new SelectListItem{ Text="Conocido", Value="CONOCIDO (A)"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };
            ViewBag.listaRelacion = ListaRelacion;
            ViewBag.idRelacion = BuscaId(ListaRelacion, familiaresforaneos.Relacion);
            #endregion

            #region Tiempo de conocerlo
            List<SelectListItem> ListaTiempo;
            ListaTiempo = new List<SelectListItem>
            {
              new SelectListItem{ Text="Menos de un año", Value="MENOS DE 1 AÑO"},
              new SelectListItem{ Text="Entre 1 y 2 años", Value="ENTRE 1 Y 2 AÑOS"},
              new SelectListItem{ Text="Entre 2 y 5 años(a)", Value="ENTRE 2 Y 5 AÑOS"},
              new SelectListItem{ Text="Más de 5 años", Value="MÁS DE 5 AÑOS"},
              new SelectListItem{ Text="Toda la vida", Value="TODA LA VIDA"},
            };
            ViewBag.listTiempo = ListaTiempo;
            ViewBag.idTiempo = BuscaId(ListaTiempo, familiaresforaneos.TiempoConocerlo);
            #endregion

            #region PAIS          
            List<SelectListItem> ListaPaisD;
            ListaPaisD = new List<SelectListItem>
            {
              new SelectListItem{ Text="México", Value="MEXICO"},
              new SelectListItem{ Text="Estados Unidos", Value="ESTADOS UNIDOS"},
              new SelectListItem{ Text="Canada", Value="CANADA"},
              new SelectListItem{ Text="Colombia", Value="COLOMBIA"},
              new SelectListItem{ Text="El Salvador", Value="EL SALVADOR"},
              new SelectListItem{ Text="Guatemala", Value="GUATEMALA"},
              new SelectListItem{ Text="Chile", Value="CHILE"},
              new SelectListItem{ Text="Argentina", Value="ARGENTINA"},
              new SelectListItem{ Text="Brasil", Value="BRASIL"},
              new SelectListItem{ Text="Venezuela", Value="VENEZUELA"},
              new SelectListItem{ Text="Puerto Rico", Value="PUERTO RICO"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListaPais = ListaPaisD;
            ViewBag.idPais = BuscaId(ListaPaisD, familiaresforaneos.Pais);
            #endregion

            #region Destado
            List<Estados> listaEstado = new List<Estados>();
            listaEstado = (from table in _context.Estados
                           select table).ToList();

            listaEstado.Insert(0, new Estados { Id = 0, Estado = "Selecciona" });
            ViewBag.ListaEstado = listaEstado;
            ViewBag.idEstado = familiaresforaneos.Estado;
            #endregion

            #region Frecuencia de contacto
            List<SelectListItem> ListaFrecuencia;
            ListaFrecuencia = new List<SelectListItem>
            {
              new SelectListItem{ Text="Diariamente", Value="DIARIAMENTE"},
              new SelectListItem{ Text="Una vez a la semana", Value="UNA VEZ A LA SEMANA"},
              new SelectListItem{ Text="Una vez cada 15 días", Value="UNA VEZ CADA 15 DIAS"},
              new SelectListItem{ Text="Una vez al mes", Value="UNA VEZ AL MES"},
              new SelectListItem{ Text=" Una vez al año", Value="UNA VEZ AL AÑO"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListFrecuencia = ListaFrecuencia;
            ViewBag.idFrecuencia = BuscaId(ListaFrecuencia, familiaresforaneos.Pais);
            #endregion

            ViewBag.listaProseso = listaNoSi;
            ViewBag.idProseso = BuscaId(listaNoSi, familiaresforaneos.EnteradoProceso);

            ViewBag.listaEnterar = listaNoSiNA;
            ViewBag.idEnterar = BuscaId(listaNoSiNA, familiaresforaneos.PuedeEnterarse);
            ViewBag.Pais = familiaresforaneos.Pais;

            if (familiaresforaneos == null)
            {
                return NotFound();
            }
            return View(familiaresforaneos);
        }

        public async Task<IActionResult> EditFamiliaresForaneos2(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var familiaresforaneos = await _context.Familiaresforaneos.FirstAsync(m => m.PersonaIdPersona == id);

            #region -To List databases-
            List<Persona> personaVM = _context.Persona.ToList();
            List<Familiaresforaneos> familiaresforaneosVM = _context.Familiaresforaneos.ToList();

            #endregion

            #region -Jointables-
            ViewData["joinTableFamiliarF"] = from personaTable in personaVM
                                             join familiarf in familiaresforaneosVM on personaTable.IdPersona equals familiarf.PersonaIdPersona
                                             where familiarf.PersonaIdPersona == id
                                             select new PersonaViewModel
                                             {
                                                 familiaresForaneosVM = familiarf
                                             };
            #endregion


            #region GENERO          
            List<SelectListItem> ListaGenero;
            ListaGenero = new List<SelectListItem>
            {
              new SelectListItem{ Text="Masculino", Value="M"},
              new SelectListItem{ Text="Femenino", Value="F"},
              new SelectListItem{ Text="Prefiero no decirlo", Value="N"},
            };
            ViewBag.listaGenero = ListaGenero;

            #endregion


            #region Relacion
            List<SelectListItem> ListaRelacion;
            ListaRelacion = new List<SelectListItem>
            {
              new SelectListItem{ Text="Máma", Value="MAMA"},
              new SelectListItem{ Text="Pápa", Value="PAPA"},
              new SelectListItem{ Text="Esposo (a)", Value="ESPOSO (A)"},
              new SelectListItem{ Text="Hermano (a)", Value="HERMAN0 (A)"},
              new SelectListItem{ Text="Hijo (a)", Value="HIJO (A)"},
              new SelectListItem{ Text="Abelo (a)", Value="ABUELO (A)"},
              new SelectListItem{ Text="Familiar 1 Nivel", Value="FAMILIAR 1 NIVEL"},
              new SelectListItem{ Text="Amigo", Value="AMIGO"},
              new SelectListItem{ Text="Conocido", Value="CONOCIDO (A)"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };
            ViewBag.listaRelacion = ListaRelacion;

            #endregion

            #region Tiempo de conocerlo
            List<SelectListItem> ListaTiempo;
            ListaTiempo = new List<SelectListItem>
            {
              new SelectListItem{ Text="Menos de un año", Value="MENOS DE 1 AÑO"},
              new SelectListItem{ Text="Entre 1 y 2 años", Value="ENTRE 1 Y 2 AÑOS"},
              new SelectListItem{ Text="Entre 2 y 5 años(a)", Value="ENTRE 2 Y 5 AÑOS"},
              new SelectListItem{ Text="Más de 5 años", Value="MÁS DE 5 AÑOS"},
              new SelectListItem{ Text="Toda la vida", Value="TODA LA VIDA"},
            };
            ViewBag.listTiempo = ListaTiempo;

            #endregion

            #region PAIS          
            List<SelectListItem> ListaPaisD;
            ListaPaisD = new List<SelectListItem>
            {
              new SelectListItem{ Text="México", Value="MEXICO"},
              new SelectListItem{ Text="Estados Unidos", Value="ESTADOS UNIDOS"},
              new SelectListItem{ Text="Canada", Value="CANADA"},
              new SelectListItem{ Text="Colombia", Value="COLOMBIA"},
              new SelectListItem{ Text="El Salvador", Value="EL SALVADOR"},
              new SelectListItem{ Text="Guatemala", Value="GUATEMALA"},
              new SelectListItem{ Text="Chile", Value="CHILE"},
              new SelectListItem{ Text="Argentina", Value="ARGENTINA"},
              new SelectListItem{ Text="Brasil", Value="BRASIL"},
              new SelectListItem{ Text="Venezuela", Value="VENEZUELA"},
              new SelectListItem{ Text="Puerto Rico", Value="PUERTO RICO"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListaPais = ListaPaisD;

            #endregion

            #region Destado
            List<Estados> listaEstado = new List<Estados>();
            listaEstado = (from table in _context.Estados
                           select table).ToList();

            listaEstado.Insert(0, new Estados { Id = 0, Estado = "Selecciona" });
            ViewBag.ListaEstado = listaEstado;

            #endregion
            #region Frecuencia de contacto
            List<SelectListItem> ListaFrecuencia;
            ListaFrecuencia = new List<SelectListItem>
            {
              new SelectListItem{ Text="Diariamente", Value="DIARIAMENTE"},
              new SelectListItem{ Text="Una vez a la semana", Value="UNA VEZ A LA SEMANA"},
              new SelectListItem{ Text="Una vez cada 15 días", Value="UNA VEZ CADA 15 DIAS"},
              new SelectListItem{ Text="Una vez al mes", Value="UNA VEZ AL MES"},
              new SelectListItem{ Text=" Una vez al año", Value="UNA VEZ AL AÑO"},
              new SelectListItem{ Text="Otro", Value="OTRO"},
            };

            ViewBag.ListFrecuencia = ListaFrecuencia;

            #endregion

            ViewBag.listaProseso = listaNoSi;

            ViewBag.listaEnterar = listaNoSiNA;

            if (familiaresforaneos == null)
            {
                return NotFound();
            }

            return View(familiaresforaneos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFamiliaresForaneos(int id, [Bind("IdFamiliaresForaneos,Nombre,Edad,Sexo,Relacion,TiempoConocerlo,Pais,Estado,Telefono,FrecuenciaContacto,EnteradoProceso,PuedeEnterarse,Observaciones,PersonaIdPersona")] Familiaresforaneos familiaresforaneos)
        {

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                try
                {

                    familiaresforaneos.Nombre = mg.normaliza(familiaresforaneos.Nombre);
                    familiaresforaneos.Estado = mg.normaliza(familiaresforaneos.Estado);
                    familiaresforaneos.Observaciones = mg.normaliza(familiaresforaneos.Observaciones);


                    var oldFamiliaresforaneos = await _context.Familiaresforaneos.FindAsync(familiaresforaneos.IdFamiliaresForaneos);
                    _context.Entry(oldFamiliaresforaneos).CurrentValues.SetValues(familiaresforaneos);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(familiaresforaneos);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                  
                }
              
                return RedirectToAction("EditAbandonoEstado/" + familiaresforaneos.PersonaIdPersona, "Personas");
            }
            return View(familiaresforaneos);
        }

        public async Task<IActionResult> DeleteConfirmedFamiiarF(int? id)
        {
            var familiarf = await _context.Familiaresforaneos.SingleOrDefaultAsync(m => m.IdFamiliaresForaneos == id);
            _context.Familiaresforaneos.Remove(familiarf);
            await _context.SaveChangesAsync();

            var empty = (from ff in _context.Familiaresforaneos
                         where ff.PersonaIdPersona == familiarf.PersonaIdPersona
                         select ff);

            if (!empty.Any())
            {
                var query = (from a in _context.Abandonoestado
                             where a.PersonaIdPersona == familiarf.PersonaIdPersona
                             select a).FirstOrDefault();
                query.FamiliaresFuera = "NO";
                _context.SaveChanges();
            }




            return RedirectToAction("EditAbandonoEstado/" + familiarf.PersonaIdPersona, "Personas");
        }

        public async Task<IActionResult> CrearFamiliarforaneo(Familiaresforaneos familiaresforaneos, string[] datosFamiliarF)
        {

            familiaresforaneos.PersonaIdPersona = Int32.Parse(datosFamiliarF[0]);
            familiaresforaneos.Nombre = mg.normaliza(datosFamiliarF[1]);
            try
            {
                familiaresforaneos.Edad = Int32.Parse(datosFamiliarF[2]);
            }
            catch
            {
                familiaresforaneos.Edad = 0;
            }
            familiaresforaneos.Sexo = datosFamiliarF[3];
            familiaresforaneos.Relacion = datosFamiliarF[4];
            familiaresforaneos.TiempoConocerlo = datosFamiliarF[5];
            familiaresforaneos.Pais = datosFamiliarF[6];
            familiaresforaneos.Estado = mg.normaliza(datosFamiliarF[7]);
            familiaresforaneos.Telefono = datosFamiliarF[8];
            familiaresforaneos.FrecuenciaContacto = datosFamiliarF[9];
            familiaresforaneos.EnteradoProceso = datosFamiliarF[10];
            familiaresforaneos.PuedeEnterarse = datosFamiliarF[11];
            familiaresforaneos.Observaciones = mg.normaliza(datosFamiliarF[12]);

            var query = (from a in _context.Abandonoestado
                         where a.PersonaIdPersona == familiaresforaneos.PersonaIdPersona
                         select a).FirstOrDefault();
            query.FamiliaresFuera = "SI";
            _context.SaveChanges();

            _context.Add(familiaresforaneos);
            await _context.SaveChangesAsync();


            return View();
        }
        #endregion

        #region -Editar Salud-
        public async Task<IActionResult> EditSalud(string nombre, string genero, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewData["Nombre"] = nombre;
            ViewData["Genero"] = genero;
            var saludfisica = await _context.Saludfisica.SingleOrDefaultAsync(m => m.PersonaIdPersona == id);


            ViewBag.listaEnfermedad = listaNoSi;
            ViewBag.idEnfermedad = BuscaId(listaNoSi, saludfisica.Enfermedad);

            ViewBag.listaEmbarazoLactancia = listaNoSi;
            ViewBag.idEmbarazoLactancia = BuscaId(listaNoSi, saludfisica.EmbarazoLactancia);

            ViewBag.listaDiscapacidad = listaNoSi;
            ViewBag.idDiscapacidad = BuscaId(listaNoSi, saludfisica.Discapacidad);

            ViewBag.listaServicioMedico = listaNoSi;
            ViewBag.idServicioMedico = BuscaId(listaNoSi, saludfisica.ServicioMedico);

            ViewBag.enfermedad = saludfisica.Enfermedad;
            ViewBag.especial = saludfisica.Discapacidad;
            ViewBag.smedico = saludfisica.ServicioMedico;

            #region EspecifiqueServicioMedico
            List<SelectListItem> ListaEspecifiqueServicioMedico;
            ListaEspecifiqueServicioMedico = new List<SelectListItem>
            {
                new SelectListItem{ Text = "NA", Value = "NA" },
                new SelectListItem{ Text = "Derecho habiente", Value = "DERECHO HABIENTE" },
                new SelectListItem{ Text = "Seguro Médico", Value = "SEGURO MEDICO" }
            };

            ViewBag.listaEspecifiqueServicioMedico = ListaEspecifiqueServicioMedico;
            ViewBag.idEspecifiqueServicioMedico = BuscaId(ListaEspecifiqueServicioMedico, saludfisica.EspecifiqueServicioMedico);
            #endregion

            #region InstitucionServicioMedico
            List<SelectListItem> ListaInstitucionServicioMedico;
            ListaInstitucionServicioMedico = new List<SelectListItem>
            {
                new SelectListItem{ Text = "NA", Value = "NA" },
                new SelectListItem{ Text = "IMSS", Value = "IMSS" },
                new SelectListItem{ Text = "ISSSTE", Value = "ISSSTE" },
                new SelectListItem{ Text = "Seguro Popular", Value = "SEGURO POPULAR" },
                new SelectListItem{ Text = "Militar", Value = "MILITAR" },
                new SelectListItem{ Text = "Otro", Value = "OTRO" }
            };

            ViewBag.listaInstitucionServicioMedico = ListaInstitucionServicioMedico;
            ViewBag.idInstitucionServicioMedico = BuscaId(ListaInstitucionServicioMedico, saludfisica.InstitucionServicioMedico);
            #endregion



            if (saludfisica == null)
            {
                return NotFound();
            }
            return View(saludfisica);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSalud(int id, [Bind("IdSaludFisica,Enfermedad,EspecifiqueEnfermedad,EmbarazoLactancia,Tiempo,Tratamiento,Discapacidad,EspecifiqueDiscapacidad,ServicioMedico,EspecifiqueServicioMedico,InstitucionServicioMedico,Observaciones,PersonaIdPersona")] Saludfisica saludfisica)
        {
            if (id != saludfisica.PersonaIdPersona)
            {
                return NotFound();
            }

            saludfisica.EspecifiqueEnfermedad = mg.normaliza(saludfisica.EspecifiqueEnfermedad);
            saludfisica.Tratamiento = mg.normaliza(saludfisica.Tratamiento);
            saludfisica.EspecifiqueDiscapacidad = mg.normaliza(saludfisica.EspecifiqueDiscapacidad);
            saludfisica.Observaciones = mg.normaliza(saludfisica.Observaciones);
            saludfisica.Tiempo = mg.normaliza(saludfisica.Tiempo);



            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                try
                {
                    var oldSaludfisica = await _context.Saludfisica.FindAsync(saludfisica.IdSaludFisica);
                    _context.Entry(oldSaludfisica).CurrentValues.SetValues(saludfisica);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                    //_context.Update(saludfisica);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {

                   
                    if (!PersonaExists(saludfisica.PersonaIdPersona))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
               
                return RedirectToAction("MenuEdicion/" + saludfisica.PersonaIdPersona, "Personas");
            }
            return View(saludfisica);
        }
        #endregion

        #region -WarningSupervisor-
        public async Task<IActionResult> WarningSupervisor()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.showSupervisor = false;

            #region -Solicitud Atendida Archivo prestamo Digital-
            var warningRespuesta = from a in _context.Archivoprestamodigital
                                   where a.EstadoPrestamo == 1 && user.ToString().ToUpper() == a.Usuario.ToUpper()
                                   select a;
            ViewBag.WarningsUser = warningRespuesta.Count();
            #endregion



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
            Boolean flagCoordinador = false, flagMaster = false;
            string usuario = user.ToString();
            DateTime fechaInforme = (DateTime.Today).AddDays(5);
            DateTime fechaControl = (DateTime.Today).AddDays(3);
            DateTime fechaInformeCoordinador = (DateTime.Today).AddDays(60);
            DateTime fechaHoy = DateTime.Today;
            var fechaProcesal = DateTime.Now.AddMonths(-6);

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
                if (rol == "AdminMCSCP")
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
                if (rol == "CE Resguardos")
                {
                    flagMaster = true;
                }
            }

            #region -To List databases-

            List<Persona> personaVM = _context.Persona.ToList();
            List<Supervision> supervisionVM = _context.Supervision.ToList();
            List<Causapenal> causapenalVM = _context.Causapenal.ToList();
            List<Planeacionestrategica> planeacionestrategicaVM = _context.Planeacionestrategica.ToList();
            List<Fraccionesimpuestas> fraccionesimpuestasVM = _context.Fraccionesimpuestas.ToList();
            List<Domicilio> domicilioVM = _context.Domicilio.ToList();
            List<Municipios> municipiosVM = _context.Municipios.ToList();
            List<Estados> estadosVM = _context.Estados.ToList();
            List<Archivointernomcscp> archivointernomcscpsVM = _context.Archivointernomcscp.ToList();
            List<Personacausapenal> personacausapenalsVM = _context.Personacausapenal.ToList();



            List<Fraccionesimpuestas> queryFracciones = (from f in fraccionesimpuestasVM
                                                         group f by f.SupervisionIdSupervision into grp
                                                         select grp.OrderByDescending(f => f.IdFracciones).FirstOrDefault()).ToList();


            List<Archivointernomcscp> queryHistorialArchivoadmin = (from a in _context.Archivointernomcscp
                                                                    group a by a.PersonaIdPersona into grp
                                                                    select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();


            var listaaudit = (from a in _context.Audit
                              join s in _context.Supervision on int.Parse(Regex.Replace(a.PrimaryKey, @"[^0-9]", "")) equals s.IdSupervision
                              where a.TableName == "Supervision" && a.NewValues.Contains("en espera de respuesta")
                              group a by int.Parse(Regex.Replace(a.PrimaryKey, @"[^0-9]", "")) into grp
                              select grp.OrderByDescending(a => a.Id).FirstOrDefault());
            #endregion

            #region -Jointables-
            var sinResolucion = from p in personaVM
                                join d in domicilioVM on p.IdPersona equals d.PersonaIdPersona
                                join e in estadosVM on int.Parse(d.Estado) equals e.Id
                                join m in municipiosVM on int.Parse(d.Municipio) equals m.Id
                                where p.TieneResolucion == "NO"
                                select new PlaneacionWarningViewModel
                                {
                                    personaVM = p,
                                    estadosVM = e,
                                    municipiosVM = m,
                                    tipoAdvertencia = "Sin Resolución"
                                };

            var sinResolucion2 = from p in personaVM
                                 join d in domicilioVM on p.IdPersona equals d.PersonaIdPersona
                                 join e in estadosVM on int.Parse(d.Estado) equals e.Id
                                 join m in municipiosVM on int.Parse(d.Municipio) equals m.Id
                                 where p.TieneResolucion == "NO" && p.Supervisor == usuario
                                 select new PlaneacionWarningViewModel
                                 {
                                     personaVM = p,
                                     estadosVM = e,
                                     municipiosVM = m,
                                     tipoAdvertencia = "Sin Resolución"
                                 };


            #region -Expediente Físico en Resguardo-
            //var archivoadmin = from ha in queryHistorialArchivoadmin
            //                   join ai in archivointernomcscpsVM on ha.IdarchivoInternoMcscp equals ai.IdarchivoInternoMcscp
            //                   join p in personaVM on ha.PersonaIdPersona equals p.IdPersona
            //                   join domicilio in domicilioVM on p.IdPersona equals domicilio.PersonaIdPersona
            //                   join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
            //                   where p.UbicacionExpediente != "ARCHIVO INTERNO" && p.UbicacionExpediente != "ARCHIVO GENERAL" &&
            //                   p.UbicacionExpediente != "NO UBICADO" && p.UbicacionExpediente != "SIN REGISTRO" && p.UbicacionExpediente != "NA" && p.UbicacionExpediente != null
            //                   select new PlaneacionWarningViewModel
            //                   {
            //                       municipiosVM = municipio,
            //                       personaVM = p,
            //                       archivointernomcscpVM = ai,
            //                       tipoAdvertencia = "Expediente físico en resguardo"
            //                   }; 
            #endregion




            var leftJoin = from persona in personaVM
                           join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                           join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                           join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona into tmp
                           from sinsupervision in tmp.DefaultIfEmpty()
                           select new PlaneacionWarningViewModel
                           {
                               personaVM = persona,
                               supervisionVM = sinsupervision,
                               municipiosVM = municipio,
                               tipoAdvertencia = "Sin supervisión"
                           };
            var where = from ss in leftJoin
                        where ss.supervisionVM == null
                        select new PlaneacionWarningViewModel
                        {
                            personaVM = ss.personaVM,
                            supervisionVM = ss.supervisionVM,
                            municipiosVM = ss.municipiosVM,
                            tipoAdvertencia = "Sin supervisión"
                        };

            var where2 = from ss in leftJoin
                         where ss.personaVM.Supervisor == usuario && ss.supervisionVM == null
                         select new PlaneacionWarningViewModel
                         {
                             personaVM = ss.personaVM,
                             supervisionVM = ss.supervisionVM,
                             municipiosVM = ss.municipiosVM,
                             tipoAdvertencia = "Sin supervisión"
                         };

            var personasConSupervision = from persona in personaVM
                                         join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                         join fracciones in fraccionesimpuestasVM on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                                         select new Supervision
                                         {
                                             IdSupervision = supervision.IdSupervision
                                         };
            List<int> idSupervisiones = personasConSupervision.Select(x => x.IdSupervision).Distinct().ToList();
            var joins = from persona in personaVM
                        join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                        join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                        join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                        join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                        select new PlaneacionWarningViewModel
                        {
                            personaVM = persona,
                            supervisionVM = supervision,
                            municipiosVM = municipio,
                            causapenalVM = causapenal,
                            tipoAdvertencia = "Sin figura judicial"
                        };

            var table = from persona in personaVM
                        join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                        join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                        join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                        join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                        join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                        join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                        select new PlaneacionWarningViewModel
                        {
                            personaVM = persona,
                            supervisionVM = supervision,
                            causapenalVM = causapenal,
                            planeacionestrategicaVM = planeacion,
                            fraccionesimpuestasVM = fracciones,
                            figuraJudicial = fracciones.FiguraJudicial,
                            municipiosVM = municipio
                        };


            var tableAudiot = from persona in personaVM
                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                              join fracciones in queryFracciones on supervision.IdSupervision equals fracciones.SupervisionIdSupervision
                              join audit in listaaudit on supervision.IdSupervision equals int.Parse(Regex.Replace(audit.PrimaryKey, @"[^0-9]", ""))
                              select new PlaneacionWarningViewModel
                              {
                                  personaVM = persona,
                                  supervisionVM = supervision,
                                  causapenalVM = causapenal,
                                  planeacionestrategicaVM = planeacion,
                                  figuraJudicial = fracciones.FiguraJudicial,
                                  auditVM = audit,
                                  fechaCmbio = audit.DateTime,
                                  municipiosVM = municipio
                              };

            if (usuario == "isabel.almora@dgepms.com" || usuario == "janeth@nortedgepms.com" || flagMaster == true)
            {
                var ViewDataAlertasVari = Enumerable.Empty<PlaneacionWarningViewModel>();
                switch (currentFilter)
                {
                    case "TODOS":
                        ViewDataAlertasVari = (sinResolucion).Union
                                              (from persona in personaVM
                                               join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                               join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                               where persona.Colaboracion == "SI"
                                               select new PlaneacionWarningViewModel
                                               {
                                                   personaVM = persona,
                                                   municipiosVM = municipio,
                                                   tipoAdvertencia = "Pendiente de asignación - colaboración"
                                               }).Union
                                               (from t in table
                                                where t.planeacionestrategicaVM.FechaInforme != null && t.planeacionestrategicaVM.FechaInforme < fechaInformeCoordinador && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.fraccionesimpuestasVM.FiguraJudicial == "SCP"
                                                orderby t.planeacionestrategicaVM.FechaInforme
                                                select new PlaneacionWarningViewModel
                                                {
                                                    personaVM = t.personaVM,
                                                    supervisionVM = t.supervisionVM,
                                                    municipiosVM = t.municipiosVM,
                                                    causapenalVM = t.causapenalVM,
                                                    planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                    fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                    figuraJudicial = t.fraccionesimpuestasVM.FiguraJudicial,
                                                    tipoAdvertencia = "Informe fuera de tiempo"
                                                }).Union
                                                (from t in table
                                                 where t.planeacionestrategicaVM.FechaInforme != null && t.planeacionestrategicaVM.FechaInforme < fechaControl && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.fraccionesimpuestasVM.FiguraJudicial == "MC"
                                                 select new PlaneacionWarningViewModel
                                                 {
                                                     personaVM = t.personaVM,
                                                     supervisionVM = t.supervisionVM,
                                                     municipiosVM = t.municipiosVM,
                                                     causapenalVM = t.causapenalVM,
                                                     planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                     fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                     figuraJudicial = t.fraccionesimpuestasVM.FiguraJudicial,
                                                     tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                                 }).Union
                                            (from t in table
                                             where t.planeacionestrategicaVM.FechaInforme == null && t.supervisionVM.EstadoSupervision == "VIGENTE"
                                             orderby t.fraccionesimpuestasVM.FiguraJudicial
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = t.personaVM,
                                                 supervisionVM = t.supervisionVM,
                                                 municipiosVM = t.municipiosVM,
                                                 causapenalVM = t.causapenalVM,
                                                 planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                 fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                 figuraJudicial = t.fraccionesimpuestasVM.FiguraJudicial,
                                                 tipoAdvertencia = "Sin fecha de informe"
                                             }).Union
                                            (from persona in personaVM
                                             join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                             join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                             join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                             join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                             join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                             where planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = persona,
                                                 supervisionVM = supervision,
                                                 municipiosVM = municipio,
                                                 causapenalVM = causapenal,
                                                 planeacionestrategicaVM = planeacion,
                                                 tipoAdvertencia = "Sin periodicidad de firma"
                                             }).Union
                                            (from t in table
                                             where t.personaVM.Supervisor != null && t.personaVM.Supervisor != null && t.personaVM.Supervisor.EndsWith("\u0040dgepms.com") && t.planeacionestrategicaVM.FechaProximoContacto != null && t.planeacionestrategicaVM.FechaProximoContacto < fechaHoy && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.planeacionestrategicaVM.PeriodicidadFirma != "NO APLICA"
                                             orderby t.planeacionestrategicaVM.FechaProximoContacto descending
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = t.personaVM,
                                                 supervisionVM = t.supervisionVM,
                                                 municipiosVM = t.municipiosVM,
                                                 causapenalVM = t.causapenalVM,
                                                 planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                 fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                 figuraJudicial = t.fraccionesimpuestasVM.FiguraJudicial,
                                                 tipoAdvertencia = "Se paso el tiempo de la firma"
                                             }).Union
                                            (where).Union
                                            //(archivoadmin).Union
                                            (joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionVM.IdSupervision) && s.supervisionVM.EstadoSupervision == "VIGENTE")).Union
                                            (from t in tableAudiot
                                             where t.personaVM.Supervisor != null && t.auditVM.DateTime < fechaProcesal && t.supervisionVM.EstadoSupervision != "CONCLUIDO" && t.supervisionVM.EstadoSupervision == "EN ESPERA DE RESPUESTA"
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = t.personaVM,
                                                 municipiosVM = t.municipiosVM,
                                                 supervisionVM = t.supervisionVM,
                                                 causapenalVM = t.causapenalVM,
                                                 planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                 fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                 tipoAdvertencia = "Estado Procesal",
                                                 auditVM = t.auditVM
                                             });
                        break;
                    case "SIN RESOLUCION":
                        ViewDataAlertasVari = sinResolucion;
                        break;
                    //case "EXPEDIENTE FISICO EN RESGUARDO":
                    //    ViewDataAlertasVari = archivoadmin;
                    //    break;
                    case "INFORME FUERA DE TIEMPO":
                        ViewDataAlertasVari = from t in table
                                              where t.planeacionestrategicaVM.FechaInforme != null && t.planeacionestrategicaVM.FechaInforme < fechaInformeCoordinador && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.fraccionesimpuestasVM.FiguraJudicial == "SCP"
                                              orderby t.planeacionestrategicaVM.FechaInforme
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = t.personaVM,
                                                  supervisionVM = t.supervisionVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalVM = t.causapenalVM,
                                                  planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                  fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                  figuraJudicial = t.fraccionesimpuestasVM.FiguraJudicial,
                                                  tipoAdvertencia = "Informe fuera de tiempo"
                                              };
                        break;
                    case "CONTROL DE SUPERVISION A 3 DIAS O MENOS":
                        ViewDataAlertasVari = from t in table
                                              where t.planeacionestrategicaVM.FechaInforme != null && t.planeacionestrategicaVM.FechaInforme < fechaControl && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.fraccionesimpuestasVM.FiguraJudicial == "MC"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = t.personaVM,
                                                  supervisionVM = t.supervisionVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalVM = t.causapenalVM,
                                                  planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                  fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                  figuraJudicial = t.fraccionesimpuestasVM.FiguraJudicial,
                                                  tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                              };
                        break;
                    case "SIN FECHA DE INFORME":
                        ViewDataAlertasVari = from t in table
                                              where t.planeacionestrategicaVM.FechaInforme == null && t.supervisionVM.EstadoSupervision == "VIGENTE"
                                              orderby t.fraccionesimpuestasVM.FiguraJudicial
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = t.personaVM,
                                                  supervisionVM = t.supervisionVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalVM = t.causapenalVM,
                                                  planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                  fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                  figuraJudicial = t.fraccionesimpuestasVM.FiguraJudicial,
                                                  tipoAdvertencia = "Sin fecha de informe"
                                              };
                        break;
                    case "SIN PERIODICIDAD DE FIRMA":
                        ViewDataAlertasVari = from persona in personaVM
                                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                              where planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  supervisionVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalVM = causapenal,
                                                  planeacionestrategicaVM = planeacion,
                                                  tipoAdvertencia = "Sin periodicidad de firma"
                                              };
                        break;
                    case "SIN SUPERVISION":
                        ViewDataAlertasVari = where;
                        break;
                    case "SIN FIGURA JUDICIAL":
                        ViewDataAlertasVari = joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionVM.IdSupervision) && s.supervisionVM.EstadoSupervision == "VIGENTE");
                        break;
                    case "SE PASO EL TIEMPO DE LA FIRMA":
                        ViewDataAlertasVari = from t in table
                                              where t.personaVM.Supervisor != null && t.personaVM.Supervisor != null && t.personaVM.Supervisor.EndsWith("\u0040dgepms.com") && t.planeacionestrategicaVM.FechaProximoContacto != null && t.planeacionestrategicaVM.FechaProximoContacto < fechaHoy && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.planeacionestrategicaVM.PeriodicidadFirma != "NO APLICA"
                                              orderby t.planeacionestrategicaVM.FechaProximoContacto descending
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = t.personaVM,
                                                  supervisionVM = t.supervisionVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalVM = t.causapenalVM,
                                                  planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                  fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                  figuraJudicial = t.fraccionesimpuestasVM.FiguraJudicial,
                                                  tipoAdvertencia = "Se paso el tiempo de la firma"
                                              };
                        break;
                    case "PENDIENTE DE ASIGNACION - COLABORACION":
                        ViewDataAlertasVari = from persona in personaVM
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              where persona.Colaboracion == "SI"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  municipiosVM = municipio,
                                                  tipoAdvertencia = "Pendiente de asignación - colaboración"
                                              };
                        break;
                    case "ESTADO PROCESAL":
                        ViewDataAlertasVari = from t in tableAudiot
                                              where t.personaVM.Supervisor != null && t.auditVM.DateTime < fechaProcesal && t.supervisionVM.EstadoSupervision != "CONCLUIDO" && t.supervisionVM.EstadoSupervision == "EN ESPERA DE RESPUESTA"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = t.personaVM,
                                                  municipiosVM = t.municipiosVM,
                                                  supervisionVM = t.supervisionVM,
                                                  causapenalVM = t.causapenalVM,
                                                  planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                  fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                  tipoAdvertencia = "Estado Procesal",
                                                  auditVM = t.auditVM
                                              };
                        break;
                }

                var warnings = Enumerable.Empty<PlaneacionWarningViewModel>();
                if (usuario == "janeth@nortedgepms.com" || flagMaster == true)
                {
                    var filteredWarnings = from pwvm in ViewDataAlertasVari
                                           where pwvm.personaVM.Supervisor != null && pwvm.personaVM.Supervisor.EndsWith("\u0040nortedgepms.com")
                                           select pwvm;
                    warnings = warnings.Union(filteredWarnings);
                }
                if (usuario == "isabel.almora@dgepms.com" || flagMaster == true)
                {
                    var filteredWarnings = from pwvm in ViewDataAlertasVari
                                           where pwvm.personaVM.Supervisor != null && pwvm.personaVM.Supervisor.EndsWith("\u0040dgepms.com")
                                           select pwvm;
                    warnings = warnings.Union(filteredWarnings);
                }
                ViewData["alertas"] = warnings;
            }
            else
            {
                List<Archivointernomcscp> queryHistorialArchivo = (from a in _context.Archivointernomcscp
                                                                   group a by a.PersonaIdPersona into grp
                                                                   select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();

                #region -Expediente Físico en Resguardo-
                //var archivo = from ha in queryHistorialArchivoadmin
                //              join ai in archivointernomcscpsVM on ha.IdarchivoInternoMcscp equals ai.IdarchivoInternoMcscp
                //              join p in personaVM on ha.PersonaIdPersona equals p.IdPersona
                //              join domicilio in domicilioVM on p.IdPersona equals domicilio.PersonaIdPersona
                //              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                //              where p.UbicacionExpediente != "ARCHIVO INTERNO" && p.UbicacionExpediente != "ARCHIVO GENERAL" &&
                //              p.UbicacionExpediente != "NO UBICADO" && p.UbicacionExpediente != "SIN REGISTRO" && p.UbicacionExpediente != "NA" && p.UbicacionExpediente != null && p.UbicacionExpediente == usuario.ToUpper()
                //              select new PlaneacionWarningViewModel
                //              {
                //                  municipiosVM = municipio,
                //                  personaVM = p,
                //                  archivointernomcscpVM = ai,
                //                  tipoAdvertencia = "Expediente físico en resguardo"
                //              }; 
                #endregion

                switch (currentFilter)
                {
                    case "TODOS":
                        ViewData["alertas"] = (where2).Union
                                              (sinResolucion2).Union
                                              //(archivo).Union
                                              (joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionVM.IdSupervision) && s.personaVM.Supervisor == usuario && s.supervisionVM.EstadoSupervision == "VIGENTE")).Union
                                              (from t in table
                                               where t.personaVM.Supervisor == usuario && t.planeacionestrategicaVM.FechaInforme != null && t.planeacionestrategicaVM.FechaInforme < fechaInformeCoordinador && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.fraccionesimpuestasVM.FiguraJudicial == "SCP"
                                               orderby t.planeacionestrategicaVM.FechaInforme
                                               select new PlaneacionWarningViewModel
                                               {
                                                   personaVM = t.personaVM,
                                                   supervisionVM = t.supervisionVM,
                                                   municipiosVM = t.municipiosVM,
                                                   causapenalVM = t.causapenalVM,
                                                   planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                   fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                   figuraJudicial = t.figuraJudicial,
                                                   tipoAdvertencia = "Informe fuera de tiempo"
                                               }).Union
                                               (from t in table
                                                where t.personaVM.Supervisor == usuario && t.planeacionestrategicaVM.FechaInforme != null && t.planeacionestrategicaVM.FechaInforme < fechaControl && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.fraccionesimpuestasVM.FiguraJudicial == "MC"
                                                select new PlaneacionWarningViewModel
                                                {
                                                    personaVM = t.personaVM,
                                                    supervisionVM = t.supervisionVM,
                                                    municipiosVM = t.municipiosVM,
                                                    causapenalVM = t.causapenalVM,
                                                    planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                    fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                    figuraJudicial = t.figuraJudicial,
                                                    tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                                }).Union
                                            (from t in table
                                             where t.personaVM.Supervisor == usuario && t.planeacionestrategicaVM.FechaInforme == null && t.supervisionVM.EstadoSupervision == "VIGENTE"
                                             orderby t.fraccionesimpuestasVM.FiguraJudicial
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = t.personaVM,
                                                 supervisionVM = t.supervisionVM,
                                                 municipiosVM = t.municipiosVM,
                                                 causapenalVM = t.causapenalVM,
                                                 planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                 fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                 figuraJudicial = t.figuraJudicial,
                                                 tipoAdvertencia = "Sin fecha de informe"
                                             }).Union
                                            (from persona in personaVM
                                             join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                             join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                             join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                             join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                             join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                             where persona.Supervisor == usuario && planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = persona,
                                                 supervisionVM = supervision,
                                                 municipiosVM = municipio,
                                                 causapenalVM = causapenal,
                                                 planeacionestrategicaVM = planeacion,
                                                 tipoAdvertencia = "Sin periodicidad de firma"
                                             }).Union
                                            (from t in table
                                             where t.personaVM.Supervisor != null && t.personaVM.Supervisor.EndsWith("\u0040dgepms.com") && t.personaVM.Supervisor == usuario && t.planeacionestrategicaVM.FechaProximoContacto != null && t.planeacionestrategicaVM.FechaProximoContacto < fechaHoy && t.supervisionVM.EstadoSupervision == "VIGENTE" && (t.planeacionestrategicaVM.PeriodicidadFirma == null || t.planeacionestrategicaVM.PeriodicidadFirma != "NO APLICA")
                                             orderby t.planeacionestrategicaVM.FechaProximoContacto descending
                                             select new PlaneacionWarningViewModel
                                             {
                                                 personaVM = t.personaVM,
                                                 supervisionVM = t.supervisionVM,
                                                 fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                 figuraJudicial = t.fraccionesimpuestasVM.FiguraJudicial,
                                                 municipiosVM = t.municipiosVM,
                                                 causapenalVM = t.causapenalVM,
                                                 planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                 tipoAdvertencia = "Se paso el tiempo de la firma"
                                             }).Union
                                             (from t in tableAudiot
                                              where t.personaVM.Supervisor == usuario && t.personaVM.Supervisor != null && t.auditVM.DateTime < fechaProcesal && t.supervisionVM.EstadoSupervision != "CONCLUIDO" && t.supervisionVM.EstadoSupervision == "EN ESPERA DE RESPUESTA"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = t.personaVM,
                                                  municipiosVM = t.municipiosVM,
                                                  supervisionVM = t.supervisionVM,
                                                  causapenalVM = t.causapenalVM,
                                                  planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                  fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                  tipoAdvertencia = "Estado Procesal",
                                                  auditVM = t.auditVM
                                              });
                        break;
                    //case "EXPEDIENTE FISICO EN RESGUARDO":
                    //    ViewData["alertas"] = archivo;
                    //    break;
                    case "SIN RESOLUCION":
                        ViewData["alertas"] = sinResolucion2;
                        break;
                    case "INFORME FUERA DE TIEMPO":
                        ViewData["alertas"] = from t in table
                                              where t.personaVM.Supervisor == usuario && t.planeacionestrategicaVM.FechaInforme != null && t.planeacionestrategicaVM.FechaInforme < fechaInformeCoordinador && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.fraccionesimpuestasVM.FiguraJudicial == "SCP"
                                              orderby t.planeacionestrategicaVM.FechaInforme
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = t.personaVM,
                                                  supervisionVM = t.supervisionVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalVM = t.causapenalVM,
                                                  planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                  fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                  figuraJudicial = t.figuraJudicial,
                                                  tipoAdvertencia = "Informe fuera de tiempo"
                                              };
                        break;
                    case "CONTROL DE SUPERVISION A 3 DIAS O MENOS":
                        ViewData["alertas"] = from t in table
                                              where t.personaVM.Supervisor == usuario && t.planeacionestrategicaVM.FechaInforme != null && t.planeacionestrategicaVM.FechaInforme < fechaControl && t.supervisionVM.EstadoSupervision == "VIGENTE" && t.fraccionesimpuestasVM.FiguraJudicial == "MC"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = t.personaVM,
                                                  supervisionVM = t.supervisionVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalVM = t.causapenalVM,
                                                  planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                  fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                  figuraJudicial = t.figuraJudicial,
                                                  tipoAdvertencia = "Control de supervisión a 3 días o menos"
                                              };
                        break;
                    case "SIN FECHA DE INFORME":
                        ViewData["alertas"] = from t in table
                                              where t.personaVM.Supervisor == usuario && t.planeacionestrategicaVM.FechaInforme == null && t.supervisionVM.EstadoSupervision == "VIGENTE"
                                              orderby t.fraccionesimpuestasVM.FiguraJudicial
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = t.personaVM,
                                                  supervisionVM = t.supervisionVM,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalVM = t.causapenalVM,
                                                  planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                  fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                  figuraJudicial = t.figuraJudicial,
                                                  tipoAdvertencia = "Sin fecha de informe"
                                              };
                        break;
                    case "SIN PERIODICIDAD DE FIRMA":
                        ViewData["alertas"] = from persona in personaVM
                                              join supervision in supervisionVM on persona.IdPersona equals supervision.PersonaIdPersona
                                              join domicilio in domicilioVM on persona.IdPersona equals domicilio.PersonaIdPersona
                                              join municipio in municipiosVM on int.Parse(domicilio.Municipio) equals municipio.Id
                                              join causapenal in causapenalVM on supervision.CausaPenalIdCausaPenal equals causapenal.IdCausaPenal
                                              join planeacion in planeacionestrategicaVM on supervision.IdSupervision equals planeacion.SupervisionIdSupervision
                                              where persona.Supervisor == usuario && planeacion.PeriodicidadFirma == null && supervision.EstadoSupervision == "VIGENTE"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = persona,
                                                  supervisionVM = supervision,
                                                  municipiosVM = municipio,
                                                  causapenalVM = causapenal,
                                                  planeacionestrategicaVM = planeacion,
                                                  tipoAdvertencia = "Sin periodicidad de firma"
                                              };
                        break;
                    case "SIN SUPERVISION":
                        ViewData["alertas"] = where2;
                        break;
                    case "SIN FIGURA JUDICIAL":
                        ViewData["alertas"] = joins.Where(s => !idSupervisiones.Any(x => x == s.supervisionVM.IdSupervision) && s.personaVM.Supervisor == usuario && s.supervisionVM.EstadoSupervision == "VIGENTE");
                        break;
                    case "SE PASO EL TIEMPO DE LA FIRMA":
                        ViewData["alertas"] = from t in table
                                              where t.personaVM.Supervisor != null && t.personaVM.Supervisor.EndsWith("\u0040dgepms.com") && t.personaVM.Supervisor == usuario && t.planeacionestrategicaVM.FechaProximoContacto != null && t.planeacionestrategicaVM.FechaProximoContacto < fechaHoy && t.supervisionVM.EstadoSupervision == "VIGENTE" && (t.planeacionestrategicaVM.PeriodicidadFirma == null || t.planeacionestrategicaVM.PeriodicidadFirma != "NO APLICA")
                                              orderby t.planeacionestrategicaVM.FechaProximoContacto descending
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = t.personaVM,
                                                  supervisionVM = t.supervisionVM,
                                                  fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                  figuraJudicial = t.fraccionesimpuestasVM.FiguraJudicial,
                                                  municipiosVM = t.municipiosVM,
                                                  causapenalVM = t.causapenalVM,
                                                  planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                  tipoAdvertencia = "Se paso el tiempo de la firma"
                                              };
                        break;
                    case "ESTADO PROCESAL":
                        ViewData["alertas"] = from t in tableAudiot
                                              where t.personaVM.Supervisor == usuario && t.personaVM.Supervisor != null && t.auditVM.DateTime < fechaProcesal && t.supervisionVM.EstadoSupervision != "CONCLUIDO" && t.supervisionVM.EstadoSupervision == "EN ESPERA DE RESPUESTA"
                                              select new PlaneacionWarningViewModel
                                              {
                                                  personaVM = t.personaVM,
                                                  municipiosVM = t.municipiosVM,
                                                  supervisionVM = t.supervisionVM,
                                                  causapenalVM = t.causapenalVM,
                                                  planeacionestrategicaVM = t.planeacionestrategicaVM,
                                                  fraccionesimpuestasVM = t.fraccionesimpuestasVM,
                                                  tipoAdvertencia = "Estado Procesal",
                                                  auditVM = t.auditVM
                                              };
                        break;
                }
            }
            #endregion
            return Json(new
            {
                success = true,
                user = usuario,
                admin = flagCoordinador || flagMaster,
                //ViewData["alertas"] se usa como variable de esta funcion y no sirve como ViewData
                query = ViewData["alertas"]
            });
        }
        #endregion

        #endregion

        #region -Borrar-
        // GET: Personas/Delete/5
        public JsonResult antesdelete(Persona persona, Historialeliminacion historialeliminacion, string[] datoPersona)
        {
            var borrar = false;
            var idpersona = Int32.Parse(datoPersona[0]);

            var query = (from p in _context.Persona
                         where p.IdPersona == idpersona
                         select p).FirstOrDefault();


            var antesDel = from s in _context.Supervision
                           where s.PersonaIdPersona == idpersona
                           select s;

            var antesDel2 = from pp in _context.Presentacionperiodica
                            join rh in _context.Registrohuella on pp.RegistroidHuella equals rh.IdregistroHuella
                            where rh.PersonaIdPersona == idpersona
                            select pp;

            var nom = query.NombreCompleto;

            if (antesDel.Any() || antesDel2.Any())
            {
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("Index", "Personas"), borrar = borrar, nombre = nom });
            }
            var stadoc = (from p in _context.Persona
                          where p.IdPersona == persona.IdPersona
                          select p.IdPersona).FirstOrDefault();

            return Json(new { success = true, responseText = Convert.ToString(stadoc), idPersonas = Convert.ToString(persona.IdPersona) });

        }


        public JsonResult deletePersona(Persona persona, Historialeliminacion historialeliminacion, string[] datoPersona)
        {
            var borrar = false;
            var idpersona = Int32.Parse(datoPersona[0]);
            var razon = mg.normaliza(datoPersona[1]);
            var user = mg.normaliza(datoPersona[2]);

            var query = (from p in _context.Persona
                         where p.IdPersona == idpersona
                         select p).FirstOrDefault();

            try
            {
                borrar = true;
                historialeliminacion.Id = idpersona;
                historialeliminacion.Descripcion = query.Paterno + " " + query.Materno + " " + query.Nombre;
                historialeliminacion.Tipo = "PERSONA";
                historialeliminacion.Razon = mg.normaliza(razon);
                historialeliminacion.Usuario = user;
                historialeliminacion.Fecha = DateTime.Now;
                historialeliminacion.Supervisor = mg.normaliza(query.Supervisor);
                _context.Add(historialeliminacion);
                _context.SaveChanges();
                _context.Database.ExecuteSqlCommand("CALL spBorrarPersona(" + idpersona + ")");
                return Json(new { success = true, responseText = Url.Action("Index", "Personas"), borrar = borrar });
            }
            catch (DbException ex)
            {
                
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("Index", "Personas"), borrar = borrar, error = ex });
            }
            catch (DbUpdateException ex)
            {
             
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("Index", "Personas"), borrar = borrar, error = ex });
            }
            catch (Exception ex)
            {

                
                borrar = false;
                return Json(new { success = true, responseText = Url.Action("Index", "Personas"), borrar = borrar, error = ex });
            }
            var stadoc = (from p in _context.Persona
                          where p.IdPersona == persona.IdPersona
                          select p.IdPersona).FirstOrDefault();

            return Json(new { success = true, responseText = Convert.ToString(stadoc), idPersonas = Convert.ToString(persona.IdPersona) });

        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var persona = await _context.Persona.SingleOrDefaultAsync(m => m.IdPersona == id);
            _context.Persona.Remove(persona);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> deleteSustancia()
        {
            var sustancia = await _context.Consumosustancias.SingleOrDefaultAsync(m => m.IdConsumoSustancias == consumosustancias[contadorSustancia - 1].IdConsumoSustancias);
            _context.Consumosustancias.Remove(sustancia);
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (contadorSustancia == consumosustancias.Count)
            {
                if (datosSustancias.Count + datosSustanciasEditadas.Count == 0)
                {
                    return Json(new { success = true, responseText = "Ya No" });
                }
                else
                {
                    return Json(new { success = true, responseText = "Datos Guardados con éxito" });
                }
            }
            else
            {
                return Json(new
                {
                    success = true,
                    responseText = "Siguiente",
                    sustancia = consumosustancias[contadorSustancia].Sustancia,
                    frecuencia = consumosustancias[contadorSustancia].Frecuencia,
                    cantidad = consumosustancias[contadorSustancia].Cantidad,
                    ultimoConsumo = consumosustancias[contadorSustancia].UltimoConsumo,
                    observacionesConsumo = consumosustancias[contadorSustancia].Observaciones,
                    idConsumoSustancias = consumosustancias[contadorSustancia++].IdConsumoSustancias
                });
            }
        }

        public async Task<IActionResult> deleteFamiliar(int tipoGuardado)
        {
            if (tipoGuardado == 1)
            {
                var asientoFamiliar = await _context.Asientofamiliar.SingleOrDefaultAsync(m => m.IdAsientoFamiliar == familiares[contadorFamiliares - 1].IdAsientoFamiliar);
                _context.Asientofamiliar.Remove(asientoFamiliar);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (contadorFamiliares == familiares.Count)
                {
                    if (datosFamiliares.Count + datosFamiliaresEditados.Count == 0)
                    {
                        return Json(new { success = true, responseText = "Ya No" });
                    }
                    else
                    {
                        return Json(new { success = true, responseText = "Datos Guardados con éxito" });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        responseText = "Siguiente",
                        nombre = familiares[contadorFamiliares].Nombre,
                        relacion = familiares[contadorFamiliares].Relacion,
                        edad = familiares[contadorFamiliares].Edad,
                        sexo = familiares[contadorFamiliares].Sexo,
                        dependencia = familiares[contadorFamiliares].Dependencia,
                        explicaDependencia = familiares[contadorFamiliares].DependenciaExplica,
                        vivenJuntos = familiares[contadorFamiliares].VivenJuntos,
                        direccion = familiares[contadorFamiliares].Domicilio,
                        telefono = familiares[contadorFamiliares].Telefono,
                        horarioLocalizacion = familiares[contadorFamiliares].HorarioLocalizacion,
                        enteradoProceso = familiares[contadorFamiliares].EnteradoProceso,
                        puedeEnterarse = familiares[contadorFamiliares].PuedeEnterarse,
                        observaciones = familiares[contadorFamiliares].Observaciones,
                        idAsientoFamiliar = familiares[contadorFamiliares++].IdAsientoFamiliar
                    });
                }
            }
            else
            {
                var asientoFamiliar = await _context.Asientofamiliar.SingleOrDefaultAsync(m => m.IdAsientoFamiliar == referenciaspersonales[contadorReferencias - 1].IdAsientoFamiliar);
                _context.Asientofamiliar.Remove(asientoFamiliar);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (contadorReferencias == referenciaspersonales.Count)
                {
                    if (datosReferencias.Count + datosReferenciasEditadas.Count == 0)
                    {
                        return Json(new { success = true, responseText = "Ya No" });
                    }
                    else
                    {
                        return Json(new { success = true, responseText = "Datos Guardados con éxito" });
                    }
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        responseText = "Siguiente",
                        nombre = referenciaspersonales[contadorReferencias].Nombre,
                        relacion = referenciaspersonales[contadorReferencias].Relacion,
                        edad = referenciaspersonales[contadorReferencias].Edad,
                        sexo = referenciaspersonales[contadorReferencias].Sexo,
                        dependencia = referenciaspersonales[contadorReferencias].Dependencia,
                        explicaDependencia = referenciaspersonales[contadorReferencias].DependenciaExplica,
                        vivenJuntos = referenciaspersonales[contadorReferencias].VivenJuntos,
                        direccion = referenciaspersonales[contadorReferencias].Domicilio,
                        telefono = referenciaspersonales[contadorReferencias].Telefono,
                        horarioLocalizacion = referenciaspersonales[contadorReferencias].HorarioLocalizacion,
                        enteradoProceso = referenciaspersonales[contadorReferencias].EnteradoProceso,
                        puedeEnterarse = referenciaspersonales[contadorReferencias].PuedeEnterarse,
                        observaciones = referenciaspersonales[contadorReferencias].Observaciones,
                        idAsientoFamiliar = referenciaspersonales[contadorReferencias++].IdAsientoFamiliar
                    });
                }
            }
        }
        #endregion

        #region -BitmapToBytes-
        private static MemoryStream BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream;
            }
        }
        #endregion

        #region -PersonaExists-
        private bool PersonaExists(int id)
        {
            return _context.Persona.Any(e => e.IdPersona == id);
        }
        #endregion

        #region -obtenerDatos-
        public ActionResult OnGetChartData()
        {

            var supervisoresScorpio = from p in _context.Persona
                                      group p by p.Supervisor into grup
                                      select new
                                      {
                                          grup.Key,
                                          Count = grup.Count()
                                      }
                          ;

            var supervisoresBD = from c in _context.Controlsupervisiones
                                 select new
                                 {
                                     c.Supervisor,
                                     c.Supervisados
                                 };

            var result = (from s in supervisoresScorpio
                          join b in supervisoresBD on s.Key equals b.Supervisor
                          select new
                          {
                              Supervisor = ((b.Supervisor).ToString()).Substring(0, ((b.Supervisor).ToString()).IndexOf("@")),
                              Supervisados = s.Count + b.Supervisados
                          }).ToList();

            var json = result.ToGoogleDataTable()
            .NewColumn(new Column(ColumnType.String, "Supervisor"), x => x.Supervisor)
            .NewColumn(new Column(ColumnType.Number, "Supervisiones"), x => x.Supervisados)
            .Build()
            .GetJson();

            return Content(json);
        }
        #endregion

        #region -Actualizar Candado All-
        public JsonResult LoockCandado(Persona persona, string[] datoCandado)
        //public async Task<IActionResult> LoockCandado(Persona persona, string[] datoCandado)
        {
            persona.Candado = Convert.ToSByte(datoCandado[0] == "true");
            persona.IdPersona = Int32.Parse(datoCandado[1]);
            persona.MotivoCandado = mg.normaliza(datoCandado[2]);

            var empty = (from p in _context.Persona
                         where p.IdPersona == persona.IdPersona
                         select p);

            if (empty.Any())
            {
                var query = (from p in _context.Persona
                             where p.IdPersona == persona.IdPersona
                             select p).FirstOrDefault();
                query.Candado = persona.Candado;
                query.MotivoCandado = persona.MotivoCandado;
                _context.SaveChanges();
            }
            var stadoc = (from p in _context.Persona
                          where p.IdPersona == persona.IdPersona
                          select p.Candado).FirstOrDefault();
            //return View();

            return Json(new { success = true, responseText = Convert.ToString(stadoc), idPersonas = Convert.ToString(persona.IdPersona) });
        }
        public JsonResult getEstadodeCanadado(int id)
        {
            //IEnumerable<Persona> shops = _context.Persona;
            //return Json(shops.Select(u => new { u.Candado, u.IdPersona }).Where(u => u.IdPersona == id));

            var stadoc = (from p in _context.Persona
                          where p.IdPersona == id
                          select p.Candado);

            return Json(stadoc);
        }
        #endregion

        #region -JsonA-ll-
        public async Task<IActionResult> Get(string sortOrder,
            string currentFilter,
            string Search,
            int? pageNumber)
        {
            #region -ListaUsuarios-            
            //var user = await userManager.FindByNameAsync(User.Identity.Name);
            //var roles = await userManager.GetRolesAsync(user);

            //List<string> rolUsuario = new List<string>();

            //for (int i = 0; i < roles.Count; i++)
            //{
            //    rolUsuario.Add(roles[i]);
            //}

            //ViewBag.RolesUsuario = rolUsuario[1];

            //String users = user.ToString();
            //ViewBag.RolesUsuarios = users;

            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            ViewBag.Admin = false;
            ViewBag.Masteradmin = false;
            ViewBag.Archivo = false;

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP")
                {
                    ViewBag.Admin = true;
                }
            }
            foreach (var rol in roles)
            {
                if (rol == "Masteradmin")
                {
                    ViewBag.Masteradmin = true;
                }
            }
            foreach (var rol in roles)
            {
                if (rol == "ArchivoMCSCP")
                {
                    ViewBag.Archivo = true;
                }
            }

            String users = user.ToString();
            ViewBag.RolesUsuarios = users;
            #endregion
            List<Persona> listaSupervisados = new List<Persona>();
            listaSupervisados = (from table in _context.Persona
                                 select table).ToList();
            listaSupervisados.Insert(0, new Persona { IdPersona = 0, Supervisor = "Selecciona" });
            ViewBag.listaSupervisados = listaSupervisados;




            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";



            if (Search != null)
            {
                pageNumber = 1;
            }
            else
            {
                Search = currentFilter;
            }
            ViewData["CurrentFilter"] = Search;

            var personas = from p in _context.Persona
                           where p.Supervisor != null
                           select p;


            if (!String.IsNullOrEmpty(Search))
            {
                foreach (var item in Search.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(Search) ||
                                                   (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(Search) ||
                                                   p.Supervisor.Contains(Search) || (p.IdPersona.ToString()).Contains(Search));

                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
                default:
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
            }

            int pageSize = 10;
            // Response.Headers.Add("Refresh", "5");
            return Json(new
            {
                page = await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize),
                totalPages = (personas.Count() + pageSize - 1) / pageSize
            });

            //return Json(new
            //{
            //    success = true,
            //    responseText = "Siguiente",
            //    sustancia = consumosustancias[contadorSustancia].Sustancia,
            //    frecuencia = consumosustancias[contadorSustancia].Frecuencia,
            //    cantidad = consumosustancias[contadorSustancia].Cantidad,
            //    ultimoConsumo = consumosustancias[contadorSustancia].UltimoConsumo,
            //    observacionesConsumo = consumosustancias[contadorSustancia].Observaciones,
            //    idConsumoSustancias = consumosustancias[contadorSustancia++].IdConsumoSustancias
            //});

        }

        public async Task<IActionResult> GetBusqueda(string searchValue,
            string sortOrder,
            string currentFilter,
            int? pageNumber)
        {
            //List<Persona> persona = new List<Persona>();
            //persona = _context.Persona.Where(x => (x.Nombre +" "+x.Paterno+ " " + x.Materno).Contains(searchValue)|| searchValue == null).ToList();
            //return Json(persona);

            if (searchValue != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchValue = currentFilter;
            }
            ViewData["CurrentFilter"] = searchValue;

            var personas = from p in _context.Persona
                           where p.Supervisor != null
                           select p;
            if (!String.IsNullOrEmpty(searchValue))
            {
                foreach (var item in searchValue.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    personas = personas.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(searchValue) ||
                                                    (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(searchValue) ||
                                                    p.Supervisor.Contains(searchValue) || (p.IdPersona.ToString()).Contains(searchValue));

                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    personas = personas.OrderByDescending(p => p.IdPersona);
                    break;
                default:
                    personas = personas.OrderBy(p => p.IdPersona);
                    break;
            }

            int pageSize = 10;

            return Json(await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        #endregion

        #region -Listado Supervisados -
        public IActionResult SupervisadosList(Persona persona)
        {
            //datosSustancias.Clear();            
            List<Persona> listaSupervisados = new List<Persona>();
            listaSupervisados = (from table in _context.Persona
                                 select table).ToList();
            listaSupervisados.Insert(0, new Persona { IdPersona = 0, Supervisor = "Selecciona" });
            ViewBag.listaSupervisados = listaSupervisados;
            return View();
        }


        #endregion

        #region -ArchivoInternoMCSCP-
        public async Task<IActionResult> ArchivoPrestamo(
           string sortOrder,
           string currentFilter,
           string SearchString,
           string estadoSuper,
           int? pageNumber
           )
        {
            var usu = await userManager.FindByNameAsync(User.Identity.Name);
            String users = usu.ToString();
            ViewBag.user = users;

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CausaPenalSortParm"] = String.IsNullOrEmpty(sortOrder) ? "causa_penal_desc" : "";
            ViewData["EstadoCumplimientoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "estado_cumplimiento_desc" : "";

            if (SearchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                SearchString = currentFilter;
            }


            List<Archivointernomcscp> queryHistorialArchivo = (from a in _context.Archivointernomcscp
                                                               group a by a.PersonaIdPersona into grp
                                                               select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();

            var filter = from p in _context.Persona
                         join a in queryHistorialArchivo on p.IdPersona equals a.PersonaIdPersona
                         where a.NuevaUbicacion != "NO UBICADO" && a.NuevaUbicacion != "ARCHIVO INTERNO" && a.NuevaUbicacion != "SIN REGISTRO" && a.NuevaUbicacion != null
                         select new ArchivoPersona
                         {
                             archivointernomcscpVM = a,
                             personaVM = p,
                         };

            var count = filter.Count();

            ViewData["CurrentFilter"] = SearchString;
            ViewData["EstadoS"] = estadoSuper;

            if (!String.IsNullOrEmpty(SearchString))
            {
                filter = filter.Where(a => (a.personaVM.Paterno + " " + a.personaVM.Materno + " " + a.personaVM.Nombre).Contains(SearchString.ToUpper()) ||
                                              (a.personaVM.Nombre + " " + a.personaVM.Paterno + " " + a.personaVM.Materno).Contains(SearchString.ToUpper()) ||
                                              (a.personaVM.IdPersona.ToString()).Contains(SearchString)
                                              );

            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(a => a.personaVM.Paterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.CausaPenal);
                    break;
                case "fechaa_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.Fecha);
                    break;
                default:
                    filter = filter.OrderBy(spcp => spcp.personaVM.Paterno);
                    break;
            }


            List<SelectListItem> ListaUbicacion = new List<SelectListItem>();
            int ii = 0;
            ListaUbicacion.Add(new SelectListItem { Text = "Sin Registro", Value = "Sin Registro" });
            ListaUbicacion.Add(new SelectListItem { Text = "Archivo Interno", Value = "Archivo Interno" });
            ListaUbicacion.Add(new SelectListItem { Text = "Archivo General", Value = "Archivo General" });
            ListaUbicacion.Add(new SelectListItem { Text = "No Ubicado", Value = "No Ubicado" });
            ListaUbicacion.Add(new SelectListItem { Text = "Dirección", Value = "Dirección" });
            ListaUbicacion.Add(new SelectListItem { Text = "Coordinación Operativa", Value = "Coordinación Operativa" });
            ListaUbicacion.Add(new SelectListItem { Text = "Coordinación MC y SCP", Value = "Coordinación MC y SCP" });
            ListaUbicacion.Add(new SelectListItem { Text = "Expediente Concluido para Razón de Archivo", Value = "Expediente Concluido para Razón de Archivo" });

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, "SupervisorMCSCP"))
                {

                    ListaUbicacion.Add(new SelectListItem
                    {
                        Text = user.ToString(),
                        Value = ii.ToString()
                    });
                }
            }

            ViewBag.ListaUbicacion = ListaUbicacion;

            int pageSize = 10;

            //var queryable = query2.AsQueryable();
            return View(await PaginatedList<ArchivoPersona>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> RazondeArchivo(
           string sortOrder,
           string currentFilter,
           string SearchString,
           string estadoSuper,
           int? pageNumber
           )
        {
            var usuario = await userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.user = usuario;
            String users = usuario.ToString();

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CausaPenalSortParm"] = String.IsNullOrEmpty(sortOrder) ? "causa_penal_desc" : "";
            ViewData["EstadoCumplimientoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "estado_cumplimiento_desc" : "";

            if (SearchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                SearchString = currentFilter;
            }


            List<Archivointernomcscp> queryHistorialArchivo = (from a in _context.Archivointernomcscp
                                                               group a by a.PersonaIdPersona into grp
                                                               select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();

            var filter = from p in _context.Persona
                         join a in queryHistorialArchivo on p.IdPersona equals a.PersonaIdPersona
                         where a.NuevaUbicacion == "EXPEDIENTE CONCLUIDO PARA RAZÓN DE ARCHIVO" && a.NuevaUbicacion != null
                         select new ArchivoPersona
                         {
                             archivointernomcscpVM = a,
                             personaVM = p,
                         };

            var count = filter.Count();

            ViewData["CurrentFilter"] = SearchString;
            ViewData["EstadoS"] = estadoSuper;

            if (!String.IsNullOrEmpty(SearchString))
            {
                filter = filter.Where(a => (a.personaVM.Paterno + " " + a.personaVM.Materno + " " + a.personaVM.Nombre).Contains(SearchString.ToUpper()) ||
                                              (a.personaVM.Nombre + " " + a.personaVM.Paterno + " " + a.personaVM.Materno).Contains(SearchString.ToUpper()) ||
                                              (a.personaVM.IdPersona.ToString()).Contains(SearchString)
                                              );

            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(a => a.personaVM.Paterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.CausaPenal);
                    break;
                case "fechaa_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.Fecha);
                    break;
                default:
                    filter = filter.OrderBy(spcp => spcp.personaVM.Paterno);
                    break;
            }

            List<SelectListItem> ListaUbicacion = new List<SelectListItem>();
            int ii = 0;

            if (users == "isabel.almora@dgepms.com")
            {
                ListaUbicacion.Add(new SelectListItem { Text = "Seleccione", Value = "Seleccione" });
                ListaUbicacion.Add(new SelectListItem { Text = "Expediente Concluido para Razón de Archivo", Value = "Expediente Concluido para Razón de Archivo" });
                foreach (var user in userManager.Users)
                {
                    if (await userManager.IsInRoleAsync(user, "SupervisorMCSCP"))
                    {

                        ListaUbicacion.Add(new SelectListItem
                        {
                            Text = user.ToString(),
                            Value = ii.ToString()
                        });
                    }
                }
            }
            else if (users == "claudia.armendariz@dgepms.com")
            {
                ListaUbicacion.Add(new SelectListItem { Text = "Seleccione", Value = "Seleccione" });
                ListaUbicacion.Add(new SelectListItem { Text = "Archivo General", Value = "Archivo General" });
            }
            ViewBag.ListaUbicacion = ListaUbicacion;

            int pageSize = 10;

            //var queryable = query2.AsQueryable();
            return View(await PaginatedList<ArchivoPersona>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        #region -Update Ubicación archivo y causa penal-
        public JsonResult UpdateUyCP(Archivointernomcscp archivointernomcscp, Persona persona, string cambioCP, string idArchivo, string usuario, string cambioUE, string idpersona, string archivoid)
        {
            //#region -Actualizar causa penal-
            //if (idArchivo != null)
            //{
            //    archivointernomcscp.CausaPenal = cambioCP;
            //    archivointernomcscp.IdarchivoInternoMcscp = Int32.Parse(idArchivo);
            //}
            //#endregion
            //var empty = (from a in _context.Archivointernomcscp
            //             where a.IdarchivoInternoMcscp == archivointernomcscp.IdarchivoInternoMcscp
            //             select a);

            //if (empty.Any())
            //{
            //    var query = (from a in _context.Archivointernomcscp
            //                 where a.IdarchivoInternoMcscp == archivointernomcscp.IdarchivoInternoMcscp
            //                 select a).FirstOrDefault();
            //    query.CausaPenal = archivointernomcscp.CausaPenal;
            //    _context.SaveChanges();
            //}

            #region -Actualizar Ubicacion-
            if (idpersona != null)
            {
                archivointernomcscp.IdarchivoInternoMcscp = Int32.Parse(archivoid);
                persona.IdPersona = Int32.Parse(idpersona);
                persona.UbicacionExpediente = mg.normaliza(cambioUE);
                archivointernomcscp.Usuario = mg.normaliza(usuario);
            }
            #endregion

            var emptypersona = (from p in _context.Persona
                                where p.IdPersona == persona.IdPersona
                                select p);

            if (emptypersona.Any())
            {
                var query = (from p in _context.Persona
                             where p.IdPersona == persona.IdPersona
                             select p).FirstOrDefault();
                query.UbicacionExpediente = persona.UbicacionExpediente;
                _context.SaveChanges();

                var utimoidarchivo = (from a in _context.Archivointernomcscp
                                      group a by a.PersonaIdPersona into grp
                                      select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();

                var filter = (from p in _context.Persona
                              join a in utimoidarchivo on p.IdPersona equals a.PersonaIdPersona
                              where a.NuevaUbicacion != null && a.PersonaIdPersona == persona.IdPersona
                              select a).FirstOrDefault();

                filter.Usuario = archivointernomcscp.Usuario;
                _context.SaveChanges();
            }

            var cp = (from a in _context.Persona
                      where a.IdPersona == persona.IdPersona
                      select a.UbicacionExpediente).FirstOrDefault();


            //return View();

            return Json(new { success = true, responseText = Convert.ToString(cp), idPersonas = Convert.ToString(archivointernomcscp.IdarchivoInternoMcscp) });
        }
        #endregion -Update Ubicación archivo y causa penal-

        #endregion

        #region -ArchivoHistorial-
        public async Task<IActionResult> ArchivoHistorial(
           string sortOrder,
           string currentFilter,
           string SearchString,
           string estadoSuper,
           int? pageNumber
           )
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CausaPenalSortParm"] = String.IsNullOrEmpty(sortOrder) ? "causa_penal_desc" : "";
            ViewData["EstadoCumplimientoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "estado_cumplimiento_desc" : "";

            if (SearchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                SearchString = currentFilter;
            }


            //List<Archivointernomcscp> queryHistorialArchivo = (from a in _context.Archivointernomcscp
            //                                                   group a by a.PersonaIdPersona into grp
            //                                                   select grp.OrderByDescending(a => a.IdarchivoInternoMcscp).FirstOrDefault()).ToList();

            //var filter = from p in _context.Persona
            //             join a in queryHistorialArchivo on p.IdPersona equals a.PersonaIdPersona
            //             select new ArchivoPersona
            //             {
            //                 archivointernomcscpVM = a,
            //                 personaVM = p,
            //             };

            var filter = from p in _context.Persona
                         join a in _context.Archivointernomcscp on p.IdPersona equals a.PersonaIdPersona
                         where a.NuevaUbicacion != null
                         select new ArchivoPersona
                         {
                             archivointernomcscpVM = a,
                             personaVM = p
                         };

            ViewData["CurrentFilter"] = SearchString;
            ViewData["EstadoS"] = estadoSuper;

            if (!String.IsNullOrEmpty(SearchString))
            {
                filter = filter.Where(a => (a.personaVM.Paterno + " " + a.personaVM.Materno + " " + a.personaVM.Nombre).Contains(SearchString) ||
                                              (a.personaVM.Nombre + " " + a.personaVM.Paterno + " " + a.personaVM.Materno).Contains(SearchString) ||
                                              (a.personaVM.IdPersona.ToString()).Contains(SearchString)
                                              );

            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(a => a.personaVM.Paterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.CausaPenal);
                    break;
                case "fechaa_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.Fecha);
                    break;
                default:
                    filter = filter.OrderByDescending(spcp => spcp.archivointernomcscpVM.Fecha);
                    break;
            }
            int pageSize = 100;

            //var queryable = query2.AsQueryable();
            return View(await PaginatedList<ArchivoPersona>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        #endregion

        #region -ArchivoNoUbicado-
        public async Task<IActionResult> ArchivoNoUbicado(
           string sortOrder,
           string currentFilter,
           string SearchString,
           string estadoSuper,
           int? pageNumber
           )
        {

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CausaPenalSortParm"] = String.IsNullOrEmpty(sortOrder) ? "causa_penal_desc" : "";
            ViewData["EstadoCumplimientoSortParm"] = String.IsNullOrEmpty(sortOrder) ? "estado_cumplimiento_desc" : "";

            if (SearchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                SearchString = currentFilter;
            }

            var filter = from p in _context.Persona
                         join a in _context.Archivointernomcscp on p.IdPersona equals a.PersonaIdPersona
                         where a.NuevaUbicacion == "No Ubicado"
                         select new ArchivoPersona
                         {
                             archivointernomcscpVM = a,
                             personaVM = p
                         };

            ViewData["CurrentFilter"] = SearchString;
            ViewData["EstadoS"] = estadoSuper;

            if (!String.IsNullOrEmpty(SearchString))
            {
                filter = filter.Where(a => (a.personaVM.Paterno + " " + a.personaVM.Materno + " " + a.personaVM.Nombre).Contains(SearchString) ||
                                              (a.personaVM.Nombre + " " + a.personaVM.Paterno + " " + a.personaVM.Materno).Contains(SearchString) ||
                                              (a.personaVM.IdPersona.ToString()).Contains(SearchString)
                                              );

            }

            switch (sortOrder)
            {
                case "name_desc":
                    filter = filter.OrderByDescending(a => a.personaVM.Paterno);
                    break;
                case "causa_penal_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.CausaPenal);
                    break;
                case "fechaa_desc":
                    filter = filter.OrderByDescending(a => a.archivointernomcscpVM.Fecha);
                    break;
                default:
                    filter = filter.OrderBy(spcp => spcp.personaVM.Paterno);
                    break;
            }
            int pageSize = 10;

            //var queryable = query2.AsQueryable();
            return View(await PaginatedList<ArchivoPersona>.CreateAsync(filter.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        #endregion

        #region -MenuArchivoMCySCP-
        public IActionResult MenuArchivoMCySCP()
        {
            return View();
        }
        #endregion

        #region -Contactos-
        public IActionResult Contacto(string SearchString, string sortOrder)
        {
            ViewBag.PageLoaded = true;
            var listaEstado = from table in _context.Estados
                              orderby table.Estado
                              where table.Id != 0
                              select new EstadoMunicipio
                              {
                                  estadosVM = table,

                              };

            ViewData["ListaEstados"] = listaEstado;


            ViewData["CurrentSort"] = sortOrder;
            ViewData["destacados"] = String.IsNullOrEmpty(sortOrder) ? "destacados" : "";
            ViewData["dependencia"] = sortOrder == "dependencia" ? "date_desc" : "dependencia";




            var Contactos = from c in _context.Contactos
                            select c;




            if (!String.IsNullOrEmpty(SearchString))
            {
                foreach (var item in SearchString.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    Contactos = Contactos.Where(a => (a.Lugar).Contains(SearchString.ToUpper()) ||
                                              (a.Telefono).Contains(SearchString.ToUpper()) ||
                                              (a.Dependencia).Contains(SearchString.ToUpper()) ||
                                              (a.Correo).Contains(SearchString.ToUpper())
                                              );

                }
            }

            switch (sortOrder)
            {
                case "destacados":
                    Contactos = Contactos.OrderByDescending(a => a.Destacado);
                    break;
                case "dependencia":
                    Contactos = Contactos.OrderByDescending(a => a.Dependencia);
                    break;
                default:
                    Contactos = Contactos.OrderBy(spcp => spcp.Lugar);
                    break;
            }

            ViewData["ListaContactos"] = Contactos;

            return View();
        }
        #endregion

        #region -AddContacto-
        public IActionResult AddContacto()
        {
            List<Estados> listaEstadosD = new List<Estados>();
            listaEstadosD = (from table in _context.Estados
                             select table).ToList();

            ViewBag.ListaEstadoD = listaEstadosD;


            return View();
        }

        public JsonResult CreateContactos(Contactos contactos, string Lugar, string Dependencia, string Titular, string Telefono, string Extencion, string Correo, string[] datosAtencionF)
        {
            var user = userManager.FindByNameAsync(User.Identity.Name);
            try
            {
                contactos.Lugar = Lugar.ToUpper();
                contactos.Dependencia = Dependencia.ToUpper();
                contactos.Titular = Titular.ToUpper();
                contactos.Correo = Correo.ToUpper();
                contactos.Telefono = Telefono.ToUpper();
                contactos.Extencion = Extencion.ToUpper();
                contactos.Destacado = 0;

                _context.Add(contactos);
                _context.SaveChanges();

            }
            catch (DbUpdateConcurrencyException ex)
            {
               
                return Json(new { success = false, responseText = ex });
            }
            catch (DbUpdateException ex)
            {
              
                return Json(new { success = false, responseText = ex });
            }
            catch (Exception ex)
            {
               
                return Json(new { success = false, responseText = ex });
            }

            return Json(new { success = true, responseText = Convert.ToString(0), Contacto = Convert.ToString(contactos.Idcontactos) });
        }
        #endregion

        #region -EditContacto-
        public async Task<IActionResult> EditContacto(int? id)

        {
            if (id == null)
            {
                return NotFound();
            }

            var contacto = await _context.Contactos.SingleOrDefaultAsync(m => m.Idcontactos == id);
            if (contacto == null)
            {
                return NotFound();
            }

            ViewBag.Categoria = contacto.Categoria;
            ViewBag.Destacado = contacto.Destacado;
            ViewBag.idContacto = contacto.Idcontactos;


            return View(contacto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditContacto(/*int id,*/ [Bind("Idcontactos,Categoria,Lugar,Dependencia,Titular,Correo,Telefono,Extencion,Destacado")] Contactos contactos, int Estado, int Municipio)
        {

            //var contacto = _context.Contactos
            //   .SingleOrDefault(m => m.Idcontactos == id);

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                try
                {


                    contactos.Lugar = contactos.Lugar == null ? "NA" : contactos.Lugar.ToUpper();
                    contactos.Dependencia = contactos.Dependencia == null ? "NA" : contactos.Dependencia.ToUpper();
                    contactos.Titular = contactos.Titular == null ? "NA" : contactos.Titular.ToUpper();
                    contactos.Correo = contactos.Correo == null ? "NA" : contactos.Correo.ToUpper();
                    contactos.Telefono = contactos.Telefono == null ? "NA" : contactos.Telefono.ToUpper();
                    contactos.Extencion = contactos.Extencion /*== null ? "NA" : contacto.Extencion.ToUpper()*/;
                    contactos.Destacado = contactos.Destacado;

                    _context.Update(contactos);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException ex)
                {
                
                    if (!ContactoExists(contactos.Idcontactos))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex)
                {
                    
                }
                catch (Exception ex)
                {
                    
                }
            }
            //return RedirectToAction("EditContacto/" + contactos.Idcontactos, "Personas");
            return RedirectToAction("Contacto/");
        }
        #endregion

        #region -Delete Contacto-

        public async Task<IActionResult> DeleteContacto(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var contacto = await _context.Contactos.SingleOrDefaultAsync(m => m.Idcontactos == id);
            if (contacto == null)
            {
                return NotFound();
            }
            _context.Contactos.Remove(contacto);
            await _context.SaveChangesAsync();

            return RedirectToAction("Contacto", "Personas");
        }
        #endregion

        #region -Imprimir Contactos-
        public JsonResult imprimirContactos(Contactos contactos)
        {
            //List<Contactos> lista = new List<Contactos>();
            //#region -Define contenido de variables-
            //for (int i = 0; i < datosidContacto.Length; i++)
            //{
            //    var Contactos = (from a in _context.Contactos
            //                                where a.Idcontactos == (Convert.ToInt32(datosidContacto[i]))
            //                            select a).ToList();


            //}
            //#endregion


            IEnumerable<Contactos> dataContactos = from c in _context.Contactos
                                                   select new Contactos
                                                   {
                                                       Lugar = c.Lugar,
                                                       Dependencia = c.Dependencia,
                                                       Titular = c.Titular,
                                                       Correo = c.Correo,
                                                       Telefono = c.Telefono,
                                                       Extencion = c.Extencion
                                                   };

            string templatePath = this._hostingEnvironment.WebRootPath + "\\Documentos\\templateContactos.docx";
            string resultPath = this._hostingEnvironment.WebRootPath + "\\Documentos\\ContactosDGEPMS.docx";

            DocumentCore dc = DocumentCore.Load(templatePath);

            dc.MailMerge.ClearOptions = MailMergeClearOptions.RemoveEmptyRanges;


            dc.MailMerge.Execute(dataContactos, "Contactos");
            dc.Save(resultPath);


            return Json(new { success = true, responseText = Convert.ToString(0), Contacto = Convert.ToString(contactos.Idcontactos) });

            //Response.Redirect("https://localhost:44359/Documentos/ContactosDGEPMS.docx");
            //Response.Redirect("http://10.6.60.190/Documentos/ContactosDGEPMS.docx");

        }
        #endregion

        #region -Destacados-

        public JsonResult Destacado(Contactos contactos, Persona persona, string[] datoContacto)
        {
            contactos.Destacado = Convert.ToSByte(datoContacto[0] == "true");
            contactos.Idcontactos = Int32.Parse(datoContacto[1]);

            var empty = (from c in _context.Contactos
                         where c.Idcontactos == contactos.Idcontactos
                         select c);
            if (empty.Any())
            {
                var query = (from c in _context.Contactos
                             where c.Idcontactos == contactos.Idcontactos
                             select c).FirstOrDefault();
                query.Destacado = contactos.Destacado;
                _context.SaveChanges();
            }
            var stadoc = (from c in _context.Contactos
                          where c.Idcontactos == contactos.Idcontactos
                          select c.Destacado).FirstOrDefault();

            return Json(new { success = true, responseText = Convert.ToString(stadoc), idPersonas = Convert.ToString(persona.IdPersona) });

        }



        #endregion

        #region -ContactoExists-
        private bool ContactoExists(int id)
        {
            return _context.Contactos.Any(e => e.Idcontactos == id);
        }
        #endregion

        #region -MensajesSistema-
        public async Task<IActionResult> MensajesSistema()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            string usuario = user.ToString();

            ViewBag.Admin = false;

            foreach (var rol in roles)
            {
                if (rol == "Masteradmin")
                {
                    ViewBag.Admin = true;
                }
                if (rol == "AdminMCSCP")
                {
                    ViewBag.Admin = true;
                }
            }

            var mensajes = from mensaje in _context.Mensajesistema
                           select mensaje;

            if (ViewBag.Admin == false)
            {
                mensajes = from mensaje in _context.Mensajesistema
                           where mensaje.Usuario == usuario || mensaje.Colectivo == "1"
                           select mensaje;
            }

            return View(mensajes);
        }
        #endregion

        #region -MensajeVisto-
        public async Task<IActionResult> MensajeVisto(int id)
        {
            var mensajes = await _context.Mensajesistema.SingleOrDefaultAsync(m => m.IdMensajeSistema == id);
            mensajes.Activo = "0";
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
            return Json(new { success = true });
        }
        #endregion

        #region -BuscadorGeneral-
        public async Task<JsonResult> buscadorGeneral(string var_paterno, string var_materno, string var_nombre, string rolUser, Historialbusquedageneral historialbusquedageneral)
        {

            bool var_flag = false;
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            foreach (var rol in roles)
            {
                if (rol == "Masteradmin" || rol == "AdminMCSCP" || rol == "SupervisorMCSCP" || rol == "Director" || rol == "Oficialia" || rol == "Coordinador" || rol == "AdminLC" || rol == "SupervisorLC")
                {
                    var_flag = true;
                }
            }

            List<BuscadorGeneral> listaNombres = new List<BuscadorGeneral>();


            listaNombres = _context.BuscadorGenerals
                                            .FromSql("CALL spBuscadorGeneralNombres('" + var_paterno + "', '" + var_materno + "', '" + var_nombre + "', " + var_flag + " )")
                                            .ToList();

            if (listaNombres.Count > 0)
            {
                historialbusquedageneral.Fecha = DateTime.Now;
                historialbusquedageneral.Paterno = mg.normaliza(var_paterno);
                historialbusquedageneral.Materno = mg.normaliza(var_materno);
                historialbusquedageneral.Nombre = mg.normaliza(var_nombre);
                historialbusquedageneral.Usuario = mg.normaliza(user.ToString());
                _context.Add(historialbusquedageneral);
                _context.SaveChanges();

            }

            return Json(new { success = true, responseText = Convert.ToString(0), busqueda = listaNombres });
        }



        public async Task<JsonResult> buscadorGeneralConcat(string var_nombre, string rolUser, Historialbusquedageneral historialbusquedageneral)
        {

            bool var_flag = false;
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);

            foreach (var rol in roles)
            {
                if (rol == "Masteradmin" || rol == "AdminMCSCP" || rol == "SupervisorMCSCP" || rol == "Director" || rol == "Oficialia" || rol == "Coordinador")
                {
                    var_flag = true;
                }
            }

            List<BuscadorGeneralConcat> listaNombresConcat = new List<BuscadorGeneralConcat>();


            listaNombresConcat = _context.BuscadorGeneralConcats
                                .FromSql("CALL spBuscadorGeneralNombresConcat('" + var_nombre + "', " + var_flag + ")")
                                .ToList();

            if (listaNombresConcat.Count > 0)
            {
                historialbusquedageneral.Fecha = DateTime.Now;
                historialbusquedageneral.Paterno = "NA";
                historialbusquedageneral.Materno = "NA";
                historialbusquedageneral.Nombre = mg.normaliza(var_nombre);
                historialbusquedageneral.Usuario = mg.normaliza(user.ToString());
                _context.Add(historialbusquedageneral);
                _context.SaveChanges();
            }

            return Json(new { success = true, responseText = Convert.ToString(0), busqueda = listaNombresConcat });

        }
        #endregion

        #region-VistaBitacora-
        public IActionResult VistaBitacora(int id)
        {
            ViewData["Bitacora"] =
                                from p in _context.Persona
                                join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona
                                join b in _context.Bitacora on s.IdSupervision equals b.SupervisionIdSupervision
                                join c in _context.Causapenal on s.CausaPenalIdCausaPenal equals c.IdCausaPenal
                                where p.IdPersona == id
                                select new PersonaBitacora
                                {

                                    personaVM = p,
                                    supervisionVM = s,
                                    bitacoraVM = b,
                                    causapenalVM = c

                                };
            return View();
        }
        #endregion

        #region -Libronegro-
        public async Task<IActionResult> libronegro(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            string currentUser = User.Identity.Name;
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            bool esMCSCP = false;
            bool esCL = false;

            String users = user.ToString();
            ViewBag.RolesUsuario = users;

            foreach (var rol in roles)
            {
                if (rol == "AdminMCSCP" || rol == "SupervisorMCSCP" || rol == "AuxiliarMCSCP" || rol == "ArchivoMCSCP")
                {
                    esMCSCP = true;
                }
                if (rol == "AdminLC" || rol == "SupervisorLC")
                {
                    esCL = true;
                }
            }

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            List<string> supervisores = await ObtenerListaSupervisoresmcyscocl();
            ViewBag.listaSupervisores = supervisores;



            ViewData["CurrentFilter"] = searchString;

            var libronegro = from p in _context.Libronegro
                             select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                foreach (var item in searchString.Split(new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    libronegro = libronegro.Where(p => (p.Paterno + " " + p.Materno + " " + p.Nombre).Contains(searchString) ||
                                                   (p.Nombre + " " + p.Paterno + " " + p.Materno).Contains(searchString) ||
                                                   (p.Cp).Contains(searchString) ||
                                                   (p.Direccion).Contains(searchString)
                                                   );
                }
            }

            switch (sortOrder)
            {
                case "name_desc":
                    libronegro = libronegro.OrderByDescending(p => p.Idlibronegro);
                    break;
                default:
                    libronegro = libronegro.OrderByDescending(p => p.Idlibronegro);
                    break;
            }

            //if (esMCSCP = true)
            //{
            //    libronegro.Where(l => l.Area == "MCYSCP");
            //}
            //if (esCL = true)
            //{
            //    libronegro.Where(l => l.Area == "CL");
            //}


            int pageSize = 10;
            return View(await PaginatedList<Libronegro>.CreateAsync(libronegro.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        public async Task<IActionResult> libronegrocreate()
        {
            List<string> supervisores = await ObtenerListaSupervisoresmcyscocl();

            ViewBag.listaSupervisores = supervisores;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> libronegrocreate([Bind("Materno,Paterno,Nombre,Cp,Telefono,Direccion,F1,F2,F3,F4,Supervisor")] Libronegro libronegro)
        {
            if (ModelState.IsValid)
            {
                //int idArchivo = ((from table in _context.Archivoregistro
                //                  select table.IdArchivoRegistro).Max() + 1);


                var user = await userManager.FindByNameAsync(User.Identity.Name);
                var roles = await userManager.GetRolesAsync(user);

                foreach (var rol in roles)
                {
                    if (rol == "AdminMCSCP" || rol == "SupervisorMCSCP" || rol == "AuxiliarMCSCP" || rol == "ArchivoMCSCP")
                    {
                        libronegro.Area = "MCYSCP";
                    }
                    if (rol == "AdminLC" || rol == "SupervisorLC")
                    {
                        libronegro.Area = "CL";
                    }
                }


                libronegro.Paterno = mg.normaliza(libronegro.Paterno);
                libronegro.Materno = mg.normaliza(libronegro.Materno);
                libronegro.Nombre = mg.normaliza(libronegro.Nombre);
                libronegro.Cp = mg.normaliza(libronegro.Cp);
                libronegro.Telefono = mg.normaliza(libronegro.Telefono);
                libronegro.Direccion = mg.normaliza(libronegro.Direccion);
                libronegro.F1 = mg.normaliza(libronegro.F1);
                libronegro.F2 = mg.normaliza(libronegro.F2);
                libronegro.F3 = mg.normaliza(libronegro.F3);
                libronegro.F4 = mg.normaliza(libronegro.F4);
                libronegro.Proceso = libronegro.Proceso;
                libronegro.Supervisor = libronegro.Supervisor;
                libronegro.FechaCaptura = DateTime.Now;

                _context.Add(libronegro);
                await _context.SaveChangesAsync();

                return RedirectToAction("libronegro", "Personas");

            }


            return View(libronegro);
        }

        public async Task<IActionResult> libronegroedit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libronegro = await _context.Libronegro.SingleOrDefaultAsync(m => m.Idlibronegro == id);
            if (libronegro == null)
            {
                return NotFound();
            }

            List<string> supervisores = await ObtenerListaSupervisoresmcyscocl();

            ViewBag.listaSupervisores = supervisores;

            return View(libronegro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> libronegroedit(int id, [Bind("Idlibronegro,Nombre,Paterno,Materno,Cp,Telefono,Direccion,F1,F2,F3,F4,Area,Proceso,Supervisor ")] Libronegro libronegro)
        {

            if (id != libronegro.Idlibronegro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);
                libronegro.Area = mg.normaliza(libronegro.Area);
                libronegro.Paterno = mg.normaliza(libronegro.Paterno);
                libronegro.Materno = mg.normaliza(libronegro.Materno);
                libronegro.Nombre = mg.normaliza(libronegro.Nombre);
                libronegro.Cp = mg.normaliza(libronegro.Cp);
                libronegro.Telefono = mg.normaliza(libronegro.Telefono);
                libronegro.Direccion = mg.normaliza(libronegro.Direccion);
                libronegro.F1 = mg.normaliza(libronegro.F1);
                libronegro.F2 = mg.normaliza(libronegro.F2);
                libronegro.F3 = mg.normaliza(libronegro.F3);
                libronegro.F4 = mg.normaliza(libronegro.F4);
                libronegro.Proceso = libronegro.Proceso;
                libronegro.Supervisor = libronegro.Supervisor;

                try
                {
                    var oldlibronegro = await _context.Libronegro.FindAsync(id);
                    _context.Entry(oldlibronegro).CurrentValues.SetValues(libronegro);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                   
                }
              
                return RedirectToAction("libronegro", "Personas");
            }
            return View(libronegro);
        }
        // POST: Libronegro/Delete/
        public async Task<IActionResult> Deletelibro(int id)
        {
            var libronegro = await _context.Libronegro.SingleOrDefaultAsync(m => m.Idlibronegro == id);
            _context.Libronegro.Remove(libronegro);
            await _context.SaveChangesAsync();
            return RedirectToAction("libronegro", "Personas");
        }

        public void Toggle(int id)
        {
            var libronegro = (from ln in _context.Libronegro
                              where ln.Idlibronegro == id
                              select ln).FirstOrDefault();
            if (libronegro.Proceso == 0 || libronegro.Proceso == null)
            {
                libronegro.Proceso = 1;
            }
            else
            {
                libronegro.Proceso = 0;
            }
            _context.SaveChanges();
        }
        public void cambioS(int id, string value)
        {
            var libronegro = (from ln in _context.Libronegro
                              where ln.Idlibronegro == id
                              select ln).FirstOrDefault();
            libronegro.Supervisor = value;

            _context.SaveChanges();
        }

        #endregion


    }

}