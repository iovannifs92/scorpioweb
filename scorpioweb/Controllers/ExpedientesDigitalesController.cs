using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using scorpioweb.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Syncfusion.EJ2.PdfViewer;

namespace scorpioweb.Controllers
{
    [Authorize]
    public class ExpedientesDigitalesController : Controller
    {
        #region -Constructor-
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly penas2Context _context;
        public ExpedientesDigitalesController(penas2Context context, IHostingEnvironment hostingEnvironment, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        #endregion

        #region -Index-
        public async Task<IActionResult> Index(int idArchivoRegistro, string urlArchivo, int tipo)
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            var roles = await userManager.GetRolesAsync(user);
            bool flagRol = false;
            string usuario = user.ToString().ToUpper();
            DateTime date = DateTime.Now;
            int permiso = 0;
            ViewBag.Permiso = true;
            string cp = "";


            foreach (var rol in roles)
            {
                if (rol.ToString() == "Masteradmin" || rol.ToString() == "Archivo")
                {
                    flagRol = true;
                }
            }

            if (flagRol != true && tipo == 1)
            {
                var query = (from ad in _context.Archivo
                             join ed in _context.Archivoprestamodigital on ad.IdArchivo equals ed.ArchivoIdArchivo
                             where ad.IdArchivo == idArchivoRegistro && ed.Usuario == usuario && date < ed.FechaCierre
                             select ad);
                cp = query.Select(p => p.Paterno + " " + p.Materno + " " + p.Nombre).FirstOrDefault();
                permiso = query.Count();
            }

            if (flagRol != true && tipo == 2)
            {
                var query = (from ad in _context.Archivoregistro
                             join ed in _context.Archivoprestamodigital on ad.ArchivoIdArchivo equals ed.ArchivoIdArchivo
                             where ad.IdArchivoRegistro == idArchivoRegistro && ed.Usuario == usuario && date < ed.FechaCierre
                             select ad);
                cp = "C. P. " + query.Select(p => p.CausaPenal).FirstOrDefault();
                permiso = query.Count();
            }


            if (permiso > 0 || flagRol)
            {
                ViewBag.Nombre = urlArchivo;
            }
            else
            {
                ViewBag.Permiso = false;
            }

            ViewBag.CP = cp;
            return View();
        } 
        #endregion

        #region -Load-
        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/Load")]
        public IActionResult Load([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            MemoryStream stream = new MemoryStream();
            object jsonResult = new object();
            if (jsonObject != null && jsonObject.ContainsKey("document"))
            {
                if (bool.Parse(jsonObject["isFileName"]))
                {
                    string documentPath = GetDocumentPath(jsonObject["document"]);
                    if (!string.IsNullOrEmpty(documentPath))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(documentPath);
                        stream = new MemoryStream(bytes);
                    }
                    else
                    {
                        return this.Content(jsonObject["document"] + " is not found");
                    }
                }
                else
                {
                    byte[] bytes = Convert.FromBase64String(jsonObject["document"]);
                    stream = new MemoryStream(bytes);
                }
            }
            jsonResult = pdfviewer.Load(stream, jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }
        #endregion

        #region -RenderPdfPages-
        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/RenderPdfPages")]
        public IActionResult RenderPdfPages([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();

            object jsonResult = pdfviewer.GetPage(jsonObject);

            return Content(JsonConvert.SerializeObject(jsonResult));
        }
        #endregion

        #region -Unload-
        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/Unload")]
        public IActionResult Unload([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            pdfviewer.ClearCache(jsonObject);
            return this.Content("Document cache is cleared");
        }
        #endregion

        #region -RenderThumbnailImages-
        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/RenderThumbnailImages")]
        public IActionResult RenderThumbnailImages([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object result = pdfviewer.GetThumbnailImages(jsonObject);
            return Content(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region -Bookmarks-
        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/Bookmarks")]
        public IActionResult Bookmarks([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object jsonResult = pdfviewer.GetBookmarks(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }
        #endregion

        #region -PrintImages-
        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/Print")]
        public IActionResult PrintImages([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object pageImage = pdfviewer.GetPrintImage(jsonObject);
            return Content(JsonConvert.SerializeObject(pageImage));
        }
        #endregion

        #region -RenderAnnotationComments-
        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/RenderAnnotationComments")]
        public IActionResult RenderAnnotationComments([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            object jsonResult = pdfviewer.GetAnnotationComments(jsonObject);
            return Content(JsonConvert.SerializeObject(jsonResult));
        }
        #endregion

        #region -Download-
        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/Download")]
        public IActionResult Download([FromBody] Dictionary<string, string> jsonObject)
        {
            PdfRenderer pdfviewer = new PdfRenderer();
            string documentBase = pdfviewer.GetDocumentAsBase64(jsonObject);
            return Content(documentBase);
        }
        #endregion

        #region -GetDocumentPath-
        private string GetDocumentPath(string document)
        {
            string documentPath = string.Empty;
            if (!System.IO.File.Exists(document))
            {
                string basePath = _hostingEnvironment.WebRootPath;
                string dataPath = string.Empty;
                dataPath = basePath + @"/Expedientes/";
                if (System.IO.File.Exists(dataPath + document))
                    documentPath = dataPath + document;
            }
            else
            {
                documentPath = document;
            }
            return documentPath;
        } 
        #endregion

    }
}
