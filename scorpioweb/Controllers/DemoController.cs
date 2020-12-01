using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace scorpioweb.Controllers
{
    [Route ("demo")]
    public class DemoController : Controller
    {
        private IHostingEnvironment ihostingEnvironment;

        public DemoController(IHostingEnvironment ihostingEnvironment)
        {
            this.ihostingEnvironment = ihostingEnvironment;
        }

        /*[Route("index")]
        [Route("")]
        [Route("~/")]*/
        public IActionResult Index()
        {
            return View();
        }

        [Route("upload")]
        [HttpPost]
        public IActionResult Upload(IFormFile photo)
        {
            string file_name = "Nombre" + DateTime.Now.ToString("Mdyyyy")+".jpg";
            var uploads = Path.Combine(this.ihostingEnvironment.WebRootPath, "Fotos");
            var stream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create);
            photo.CopyToAsync(stream);
            ViewBag.photo = file_name;
            return View("Success");
        }
    }
}
