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
            var path = Path.Combine(this.ihostingEnvironment.WebRootPath, "images", photo.FileName);
            var stream = new FileStream(path, FileMode.Create);
            photo.CopyToAsync(stream);
            ViewBag.photo = photo.FileName;
            return View("Success");
        }
    }
}
