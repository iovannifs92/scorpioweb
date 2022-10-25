using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using SautinSoft.Document;
using SautinSoft.Document.MailMerging;
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

namespace scorpioweb.Controllers
{
    public class Api : Controller
    {
        private readonly penas2Context _context;

        public Api(penas2Context context)
        {
            _context = context;
        }

        
        public JsonResult Imprimir(string folio)
        {
            var datos = "";
            try
            {
                var datosAdol = _context.Database.ExecuteSqlCommand("CALL spReporteAER(" + 19 + ")");
            }
            catch(Exception ex)
            {

            }




            //return View();
            return Json(datos);

        }
        public JsonResult LoockCandado(Persona persona, string[] datoCandado)
        //public async Task<IActionResult> LoockCandado(Persona persona, string[] datoCandado)
        {
            persona.Candado = Convert.ToSByte(datoCandado[0] == "true");
            persona.IdPersona = Int32.Parse(datoCandado[1]);
            persona.MotivoCandado = datoCandado[2];

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
    }
}
