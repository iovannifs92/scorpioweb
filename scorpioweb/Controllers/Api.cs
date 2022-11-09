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
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly penas2Context _context;

        public Api(penas2Context context, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }


        public void Imprimir(string id)
        {
            var persona = _context.Adolescentes
               .SingleOrDefault(m => m.Folio == id);

            #region -joinTables-
            var tableAdolescente = (from a in _context.Adolescentes
                                    join aad in _context.AerAdolescentesDetalles on a.Folio equals aad.Folio
                                    where a.Folio == id
                                    select new
                                    {
                                        adolescentesVM = a,
                                        aerAdolescentesDetallesVM = aad
                                    }).ToList();

            IEnumerable<FroAdolescentes> dataO = from fro in _context.Fros
                                                 join froA in _context.FroAdolescentes on fro.IdDescFro equals froA.IdDesc
                                                 where fro.EntrevistaFolio == id && froA.Tipo == "O"
                                                 orderby fro.IdDescFro
                                                 select new FroAdolescentes
                                                 {
                                                     Descripcion = froA.Descripcion
                                                 };
            IEnumerable<FroAdolescentes> dataR = from fro in _context.Fros
                                                 join froA in _context.FroAdolescentes on fro.IdDescFro equals froA.IdDesc
                                                 where fro.EntrevistaFolio == id && froA.Tipo == "R"
                                                 orderby fro.IdDescFro
                                                 select new FroAdolescentes
                                                 {
                                                     Descripcion = froA.Descripcion
                                                 };

            #endregion

            #region -Generar Reporte-
            string templatePath = this._hostingEnvironment.WebRootPath + "\\Documentos\\templateFichaTecnica.docx";
            string resultPath = this._hostingEnvironment.WebRootPath + "\\Documentos\\fichatecnica.docx";

            DocumentCore dc = DocumentCore.Load(templatePath);

            var datosAdolecentes = new[] { new {
                #region -Datos Generales-
                nombre = tableAdolescente[0].adolescentesVM.Nombre,
                genero= tableAdolescente[0].adolescentesVM.Genero,
                nacionalidad = tableAdolescente[0].adolescentesVM.Nacionalidad,
                fnacimiento =(Convert.ToDateTime(tableAdolescente[0].adolescentesVM.FechaNacimiento)).ToString("dd MMMM yyyy"),
                estadocivil = tableAdolescente[0].adolescentesVM.Estadocivil,
                duracionestadocivil = tableAdolescente[0].adolescentesVM.Duracionestadocivil,
                hablaidioma = tableAdolescente[0].adolescentesVM.HablaEsp,
                //leerescribir = tableAdolescente[0].adolescentesVM.,
                //telefono = tableAdolescente[0].adolescentesVM.numeotelefono,
                hijos = tableAdolescente[0].adolescentesVM.DependientesEco,
                lnacimiento = tableAdolescente[0].adolescentesVM.MunicipioNac +" " + tableAdolescente[0].adolescentesVM.EstadoNac,
                edad = tableAdolescente[0].adolescentesVM.Edad,
                relaciondependientes = tableAdolescente[0].adolescentesVM.RelacionDep,              
                //especifiqueidioma = tableAdolescente[0].adolescentesVM.,
                //celular = tableAdolescente[0].adolescentesVM.celular,
                //cuantoshijos = tableAdolescente[0].adolescentesVM.celular,
                #endregion
                #region -Responsables-
                nombrepadre = tableAdolescente[0].adolescentesVM.NombrePadre,
                domiciliopadre = tableAdolescente[0].adolescentesVM.DomicilioPadre,
                nombremadre = tableAdolescente[0].adolescentesVM.NombreMadre,
                domiciliomadre = tableAdolescente[0].adolescentesVM.DomicilioMadre,
                nombretutor = tableAdolescente[0].adolescentesVM.NombreTutor,
                domiciliotutor = tableAdolescente[0].adolescentesVM.DomicilioTutor,
                relaciontutor = tableAdolescente[0].adolescentesVM.TutorRelacion,
                tiempoviviendofamilia = tableAdolescente[0].adolescentesVM.TiempoFam,
                personasvive = tableAdolescente[0].adolescentesVM.PersonasVive,
                #endregion
                #region -Domicilio-
                tipopropiedad = tableAdolescente[0].adolescentesVM.TipoPropiedad,
                direccion = tableAdolescente[0].adolescentesVM.DomicilioAct,
                tiempoendomicilio = tableAdolescente[0].adolescentesVM.TiempoRad,
                domiciliosecundario = tableAdolescente[0].adolescentesVM.DomicilioSec,
                direcciondomiciliosecundario = tableAdolescente[0].adolescentesVM.DireccionDomsec,
                //residenciahabitual = tableAdolescente[0].adolescentesVM.,
                #endregion
                #region -Estudia-
                estudia = tableAdolescente[0].adolescentesVM.Estudia,
                institucionestudios = tableAdolescente[0].adolescentesVM.InstitucionEst,
                direccionescuela = tableAdolescente[0].adolescentesVM.DireccionEst,
                gradoestudios = tableAdolescente[0].adolescentesVM.UltimoGradoEstudios,
                // horarioescuela = tableAdolescente[0].adolescentesVM.,
                // telefonoescuela = tableAdolescente[0].adolescentesVM.,
                #endregion
                #region -Trabaja-
                trabaja = tableAdolescente[0].adolescentesVM.Trabaja,
                lugartrabajo = tableAdolescente[0].adolescentesVM.LugarTrab,
                direcciontrabajo = tableAdolescente[0].adolescentesVM.DireccionTrab,
                puesto = tableAdolescente[0].adolescentesVM.Ocupacion,
                salario = tableAdolescente[0].adolescentesVM.Salario,
                tiempotrabajando = tableAdolescente[0].adolescentesVM.TiempoTrab,
                //tipoocupacion = tableAdolescente[0].adolescentesVM.TiempoTrab,
                #endregion
                #region -ActividadaSocial-
                tipoactividad = tableAdolescente[0].adolescentesVM.GrupoPert,
                tiempoactividad = tableAdolescente[0].adolescentesVM.TiempoGrupo,
                lugaractividad = tableAdolescente[0].adolescentesVM.LugarActividad,
                #endregion
                #region -abandonoEstado-
                viajahabitualmente = tableAdolescente[0].adolescentesVM.Viajahabitualmente,
                tiempoviajes = tableAdolescente[0].adolescentesVM.Tiempoviajes,
                documentacion = tableAdolescente[0].adolescentesVM.Documentacion,
                visa = tableAdolescente[0].adolescentesVM.Visa,
                lugaresviaje = tableAdolescente[0].adolescentesVM.Lugaresviaje,
                motivoviajes = tableAdolescente[0].adolescentesVM.Motivoviajes,
                pasaporte = tableAdolescente[0].adolescentesVM.Pasaporte,
                familiaresestado = tableAdolescente[0].adolescentesVM.FamEst,
                frecuenciafamiliarestado = tableAdolescente[0].adolescentesVM.FreqFamEst,
                familiarespais = tableAdolescente[0].adolescentesVM.FamPais,
                frecuenciafamiliarpais = tableAdolescente[0].adolescentesVM.FreqFamPais,
                #endregion
                #region -Salud-
                enfermedadfisca = tableAdolescente[0].adolescentesVM.Enfermedad,
                enfermedadmental = tableAdolescente[0].adolescentesVM.TipoEnfermedad,
                nombreenfermedad = tableAdolescente[0].adolescentesVM.NombreEnfermedad,
                consumosustancias = tableAdolescente[0].adolescentesVM.ConsumeDrog,
                tratamiento = tableAdolescente[0].adolescentesVM.Tratamiento,
                tipodroga = tableAdolescente[0].adolescentesVM.DrogasCon,
                drogasfrecuencia = tableAdolescente[0].adolescentesVM.FreqDrog,
                #endregion
                #region -Sometimiento-
                cumplimientoProcesos = tableAdolescente[0].adolescentesVM.CumplimientoProcesos,
                cooperacion = tableAdolescente[0].adolescentesVM.Cooperacion,
                amenaza = tableAdolescente[0].adolescentesVM.AmenazasTca,
                #endregion
                #region -victima-
                //nombrevictima = tableAdolescente[0].adolescentesVM.Nombrevictima,
                domiciliovictima = tableAdolescente[0].adolescentesVM.VictimaDir,
                relacionvictima = tableAdolescente[0].adolescentesVM.RelacionVict,
                delitovictima = tableAdolescente[0].adolescentesVM.DelitoVict,
                amenazavictima = tableAdolescente[0].adolescentesVM.AmenazasVictima,
                #endregion
                #region -testigos-
                testigos = tableAdolescente[0].adolescentesVM.Testigos,
                //domiciliotestigo = tableAdolescente[0].adolescentesVM.AmenazasTca,
                relaciontestigo = tableAdolescente[0].adolescentesVM.TutorRelacion,
                amenazatestigo = tableAdolescente[0].adolescentesVM.Amenazatestigo,
                #endregion
                #region -Co-detenidos-
                codetenido = tableAdolescente[0].adolescentesVM.CoDetenidios,
                //domiciliocodetenido = tableAdolescente[0].adolescentesVM.AmenazasTca,
                relacioncodetenido = tableAdolescente[0].adolescentesVM.RelacionDet,
                amenazadetenido = tableAdolescente[0].adolescentesVM.Amenazadetenido,
                #endregion
                #region -Lugarechos-
                //ubicacion = tableAdolescente[0].adolescentesVM.ubicacion,
                relacionlugar = tableAdolescente[0].adolescentesVM.RelacionLugar,
                //amenazatestigo = tableAdolescente[0].adolescentesVM.AmenazasTca,
                #endregion
                #region -RIESGOS DETECTADOS-
                
                #endregion
                #region -FACTORES DE ESTABILIDAD DETECTADOS-
                #endregion
                #region -RIESGOSFINALES-
                riesgosustracion = tableAdolescente[0].aerAdolescentesDetallesVM.RiesgoSustraccion,
                riesgovictima = tableAdolescente[0].aerAdolescentesDetallesVM.RiesgoVictima,
                riesgoobstaculizacion = tableAdolescente[0].aerAdolescentesDetallesVM.RiesgoObstaculizacion,
                riesgototal = tableAdolescente[0].aerAdolescentesDetallesVM.RiesgoTotal,
                #endregion
            }
            };

            dc.MailMerge.ClearOptions = MailMergeClearOptions.RemoveEmptyRanges;
            dc.MailMerge.Execute(datosAdolecentes);
            dc.MailMerge.Execute(dataR, "riesgodetectado");
            dc.MailMerge.Execute(dataO, "factores");
            dc.Save(resultPath);

            //Response.Redirect("https://localhost:44359/Documentos/fichatecnica.docx");
            Response.Redirect("http://10.6.60.190/Documentos/fichatecnica.docx");
            #endregion

        }
    }
}
