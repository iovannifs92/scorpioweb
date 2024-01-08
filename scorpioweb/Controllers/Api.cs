using F23.StringSimilarity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Prng;
using QRCoder;
using SautinSoft.Document;

using SautinSoft.Document.MailMerging;
using scorpioweb.Class;
using scorpioweb.Models;
using System;
using System.Collections.Generic;
using System.Data;
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

        #region -Metodos Generales-
        MetodosGenerales mg = new MetodosGenerales();
        #endregion
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
                nacionalidad = ((tableAdolescente[0].adolescentesVM.Nacionalidad == "") ? " " : (tableAdolescente[0].adolescentesVM.Nacionalidad == null) ? " " : tableAdolescente[0].adolescentesVM.Nacionalidad),
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
                direcciondomiciliosecundario = ((tableAdolescente[0].adolescentesVM.DireccionDomsec == "") ? " " : (tableAdolescente[0].adolescentesVM.DireccionDomsec == null) ? " " : tableAdolescente[0].adolescentesVM.DireccionDomsec),
                //residenciahabitual = tableAdolescente[0].adolescentesVM.,
                #endregion
                #region -Estudia-
                estudia = tableAdolescente[0].adolescentesVM.Estudia,
                institucionestudios = ((tableAdolescente[0].adolescentesVM.InstitucionEst == "") ? " " : (tableAdolescente[0].adolescentesVM.InstitucionEst == null) ? " " : tableAdolescente[0].adolescentesVM.InstitucionEst),
                direccionescuela = ((tableAdolescente[0].adolescentesVM.DireccionEst == "") ? " " : (tableAdolescente[0].adolescentesVM.DireccionEst == null) ? " " : tableAdolescente[0].adolescentesVM.DireccionEst),
                gradoestudios = tableAdolescente[0].adolescentesVM.UltimoGradoEstudios,
                // horarioescuela = tableAdolescente[0].adolescentesVM.,
                // telefonoescuela = tableAdolescente[0].adolescentesVM.,
                #endregion
                #region -Trabaja-
                trabaja = tableAdolescente[0].adolescentesVM.Trabaja,
                lugartrabajo = ((tableAdolescente[0].adolescentesVM.LugarTrab == "") ? " " : (tableAdolescente[0].adolescentesVM.LugarTrab == null) ? " " : tableAdolescente[0].adolescentesVM.LugarTrab),
                direcciontrabajo = ((tableAdolescente[0].adolescentesVM.DireccionTrab == "") ? " " : (tableAdolescente[0].adolescentesVM.DireccionTrab == null) ? " " : tableAdolescente[0].adolescentesVM.DireccionTrab),
                puesto = ((tableAdolescente[0].adolescentesVM.Ocupacion == "") ? " " : (tableAdolescente[0].adolescentesVM.Ocupacion == null) ? " " : tableAdolescente[0].adolescentesVM.Ocupacion),
                salario = ((tableAdolescente[0].adolescentesVM.Salario == "") ? " " : (tableAdolescente[0].adolescentesVM.Salario == null) ? " " : tableAdolescente[0].adolescentesVM.Salario),
                tiempotrabajando = tableAdolescente[0].adolescentesVM.TiempoTrab,
                //tipoocupacion = tableAdolescente[0].adolescentesVM.TiempoTrab,
                #endregion 
                #region -ActividadaSocial-
                tipoactividad = tableAdolescente[0].adolescentesVM.GrupoPert,
                tiempoactividad = tableAdolescente[0].adolescentesVM.TiempoGrupo,
                lugaractividad = ((tableAdolescente[0].adolescentesVM.LugarActividad == "") ? " " : (tableAdolescente[0].adolescentesVM.LugarActividad == null) ? " " : tableAdolescente[0].adolescentesVM.LugarActividad),
                #endregion
                #region -abandonoEstado-
                viajahabitualmente = ((tableAdolescente[0].adolescentesVM.Viajahabitualmente == "") ? " " : (tableAdolescente[0].adolescentesVM.Viajahabitualmente == null) ? " " : tableAdolescente[0].adolescentesVM.Viajahabitualmente),
                tiempoviajes = tableAdolescente[0].adolescentesVM.Tiempoviajes,
                documentacion = tableAdolescente[0].adolescentesVM.Documentacion,
                visa = tableAdolescente[0].adolescentesVM.Visa,
                lugaresviaje = ((tableAdolescente[0].adolescentesVM.Lugaresviaje == "") ? " " : (tableAdolescente[0].adolescentesVM.Lugaresviaje == null) ? " " : tableAdolescente[0].adolescentesVM.Lugaresviaje),
                motivoviajes = ((tableAdolescente[0].adolescentesVM.Motivoviajes == "") ? " " : (tableAdolescente[0].adolescentesVM.Motivoviajes == null) ? " " : tableAdolescente[0].adolescentesVM.Motivoviajes),
                pasaporte = tableAdolescente[0].adolescentesVM.Pasaporte,
                familiaresestado = tableAdolescente[0].adolescentesVM.FamEst,
                frecuenciafamiliarestado = tableAdolescente[0].adolescentesVM.FreqFamEst,
                familiarespais = tableAdolescente[0].adolescentesVM.FamPais,
                frecuenciafamiliarpais = tableAdolescente[0].adolescentesVM.FreqFamPais,
                #endregion
                #region -Salud-
                enfermedadfisca = ((tableAdolescente[0].adolescentesVM.Enfermedad == "") ? " " : (tableAdolescente[0].adolescentesVM.Enfermedad == null) ? " " : tableAdolescente[0].adolescentesVM.Enfermedad),
                enfermedadmental = ((tableAdolescente[0].adolescentesVM.TipoEnfermedad == "") ? " " : (tableAdolescente[0].adolescentesVM.TipoEnfermedad == null) ? " " : tableAdolescente[0].adolescentesVM.TipoEnfermedad),
                nombreenfermedad = ((tableAdolescente[0].adolescentesVM.NombreEnfermedad == "") ? " " : (tableAdolescente[0].adolescentesVM.NombreEnfermedad == null) ? " " : tableAdolescente[0].adolescentesVM.NombreEnfermedad),
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
                relaciontestigo = tableAdolescente[0].adolescentesVM.RelacionTest,
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
                #region -DATOS VERIFICACION-
                quienVerifica= tableAdolescente[0].adolescentesVM.QuienVerifica,
                parentescoVerifica = tableAdolescente[0].adolescentesVM.ParentescoVefifica,
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

            if (tableAdolescente[0].adolescentesVM.RelacionLugar == "Radica")
            {
                var observacionRadica = "";
            }

            dc.MailMerge.ClearOptions = MailMergeClearOptions.RemoveEmptyRanges;
            dc.MailMerge.Execute(datosAdolecentes);
            dc.MailMerge.Execute(dataR, "riesgodetectado");
            dc.MailMerge.Execute(dataO, "factores");
            dc.Save(resultPath);

            //Response.Redirect("https://localhost:44359/Documentos/fichatecnica.docx");
            Response.Redirect("http://10.6.60.190/Documentos/fichatecnica.docx");
            #endregion

        }



        public void queryPrueba(string[] datosQuery)
        {

        }

        #region -testSimilitud-
        public JsonResult testSimilitud(string nombre, string paterno, string materno)
        {
            bool simi = false;
            var nombreCompleto = mg.normaliza(paterno) + " " + mg.normaliza(materno) + " " + mg.normaliza(nombre);
            var query = (from p in _context.Persona
                         join d in _context.Domicilio on p.IdPersona equals d.PersonaIdPersona into domicilioJoin
                         from d in domicilioJoin.DefaultIfEmpty()
                         join s in _context.Supervision on p.IdPersona equals s.PersonaIdPersona into supervisionJoin
                         from s in supervisionJoin.DefaultIfEmpty()
                         join eu in _context.Expedienteunico on p.ClaveUnicaScorpio equals eu.ClaveUnicaScorpio into ExpedienteunicoJoin
                         from eu in ExpedienteunicoJoin.DefaultIfEmpty()
                         join cp in _context.Causapenal on s.CausaPenalIdCausaPenal equals cp.IdCausaPenal into causapenalJoin
                         from cp in causapenalJoin.DefaultIfEmpty()
                         group cp.CausaPenal by new { p.IdPersona, p.Paterno, p.Materno, p.Nombre, p.Edad, d.Calle, d.No, d.NombreCf, cp.CausaPenal, p.ClaveUnicaScorpio, claveunica = eu.ClaveUnicaScorpio } into g
                         select new
                         {
                             id = g.Key.IdPersona,
                             nomcom = g.Key.Paterno + " " + g.Key.Materno + " " + g.Key.Nombre,
                             NomTabla = "MCYSCP",
                             datoExtra = $"CLAVE UNICA SCORPIO: {g.Key.ClaveUnicaScorpio}; Edad: {g.Key.Edad};\n Domicilio: {g.Key.Calle}, {g.Key.No}, {g.Key.NombreCf};\n Causa(s) penal(es): {string.Join(", ", g.Key.CausaPenal)};\n",
                             claveUnica = g.Key.claveunica
                         }).Union
                         (from a in _context.Archivo
                          join ar in _context.Archivoregistro on a.IdArchivo equals ar.ArchivoIdArchivo
                          join eu in _context.Expedienteunico on a.ClaveUnicaScorpio equals eu.ClaveUnicaScorpio into ExpedienteunicoJoin
                          from eu in ExpedienteunicoJoin.DefaultIfEmpty()
                          group new { ar.CausaPenal, ar.CarpetaEjecucion } by new { a.IdArchivo, a.Paterno, a.Materno, a.Nombre, a.ClaveUnicaScorpio, claveunica = eu.ClaveUnicaScorpio } into g
                          select new
                          {
                              id = g.Key.IdArchivo,
                              nomcom = g.Key.Paterno + " " + g.Key.Materno + " " + g.Key.Nombre,
                              NomTabla = "Archivo",
                              datoExtra = $"CLAVE UNICA SCORPIO: {g.Key.ClaveUnicaScorpio};  Causa(s) Penal(es): {string.Join(", ", g.Select(x => x.CausaPenal))};\n Carpeta de Ejecucion: {string.Join(", ", g.Select(x => x.CarpetaEjecucion))};\n",
                              claveUnica = g.Key.claveunica
                          }).Union
                            (from e in _context.Ejecucion
                             join epcp in _context.Epcausapenal on e.IdEjecucion equals epcp.EjecucionIdEjecucion into epcausapenalJoin
                             from epcp in epcausapenalJoin.DefaultIfEmpty()
                             join eu in _context.Expedienteunico on e.ClaveUnicaScorpio equals eu.ClaveUnicaScorpio into ExpedienteunicoJoin
                             from eu in ExpedienteunicoJoin.DefaultIfEmpty()
                             group epcp.Causapenal by new { e.IdEjecucion, e.Paterno, e.Materno, e.Nombre, e.Ce, e.ClaveUnicaScorpio, epcp.Causapenal, claveunica = eu.ClaveUnicaScorpio } into g
                             select new
                             {
                                 id = g.Key.IdEjecucion,
                                 nomcom = g.Key.Paterno + " " + g.Key.Materno + " " + g.Key.Nombre,
                                 NomTabla = "Ejecucion",
                                 datoExtra = $"CLAVE UNICA SCORPIO: {g.Key.ClaveUnicaScorpio};Carpeta de Ejecucion: {g.Key.Ce};\n Causa(s) Penal(es): {string.Join(", ", g.Key.Causapenal)};\n",
                                 claveUnica = g.Key.claveunica
                             }).Union
                                    (from sp in _context.Serviciospreviosjuicio
                                     where sp.Paterno.Contains(paterno) && sp.Materno.Contains(materno)
                                     join eu in _context.Expedienteunico on sp.ClaveUnicaScorpio equals eu.ClaveUnicaScorpio into ExpedienteunicoJoin
                                     from eu in ExpedienteunicoJoin.DefaultIfEmpty()
                                     select new
                                     {
                                         id = sp.IdserviciosPreviosJuicio,
                                         nomcom = sp.Paterno + " " + sp.Materno + " " + sp.Nombre,
                                         NomTabla = "ServiciosPrevios",
                                         datoExtra = $"CLAVE UNICA SCORPIO: {sp.ClaveUnicaScorpio};  Edad: {sp.Edad};\n Domicilio: {sp.Calle}, {sp.Colonia};\n",
                                         claveUnica = eu.ClaveUnicaScorpio
                                     }).Union
                                        (from pp in _context.Prisionespreventivas
                                         join eu in _context.Expedienteunico on pp.ClaveUnicaScorpio equals eu.ClaveUnicaScorpio into ExpedienteunicoJoin
                                         from eu in ExpedienteunicoJoin.DefaultIfEmpty()
                                         select new
                                         {
                                             id = pp.Idprisionespreventivas,
                                             nomcom = pp.Paterno + " " + pp.Materno + " " + pp.Nombre,
                                             NomTabla = "PrisionPreventiva",
                                             datoExtra = $"CLAVE UNICA SCORPIO: {pp.ClaveUnicaScorpio}; Causa penal: {pp.CausaPenal};\n",
                                             claveUnica = eu.ClaveUnicaScorpio
                                         }).Union
                                            (from pp in _context.Oficialia
                                             join eu in _context.Expedienteunico on pp.ClaveUnicaScorpio equals eu.ClaveUnicaScorpio into ExpedienteunicoJoin
                                             from eu in ExpedienteunicoJoin.DefaultIfEmpty()
                                             select new
                                             {
                                                 id = pp.IdOficialia,
                                                 nomcom = pp.Paterno + " " + pp.Materno + " " + pp.Nombre,
                                                 NomTabla = "Oficialia",
                                                 datoExtra = $"CLAVE UNICA SCORPIO: {pp.ClaveUnicaScorpio}; Causa penal: {pp.CausaPenal};\n Carpeta de Ejecucion: {pp.CarpetaEjecucion};\n",
                                                 claveUnica = eu.ClaveUnicaScorpio
                                             }).Union
                                                 (from p in _context.Personacl
                                                  join d in _context.Domiciliocl on p.IdPersonaCl equals d.IdDomiciliocl into domicilioJoin
                                                  from d in domicilioJoin.DefaultIfEmpty()
                                                  join s in _context.Supervisioncl on p.IdPersonaCl equals s.PersonaclIdPersonacl into supervisionJoin
                                                  from s in supervisionJoin.DefaultIfEmpty()
                                                  join eu in _context.Expedienteunico on p.ClaveUnicaScorpio equals eu.ClaveUnicaScorpio into ExpedienteunicoJoin
                                                  from eu in ExpedienteunicoJoin.DefaultIfEmpty()
                                                  join cp in _context.Causapenalcl on s.CausaPenalclIdCausaPenalcl equals cp.IdCausaPenalcl into causapenalJoin
                                                  from cp in causapenalJoin.DefaultIfEmpty()
                                                  group cp.CausaPenal by new { p.IdPersonaCl, p.Paterno, p.Materno, p.Nombre, p.Edad, d.Calle, d.No, d.NombreCf, p.ClaveUnicaScorpio, cp.CausaPenal, claveunica = eu.ClaveUnicaScorpio} into g
                                                  select new
                                                  {
                                                      id = g.Key.IdPersonaCl,
                                                      nomcom = g.Key.Paterno + " " + g.Key.Materno + " " + g.Key.Nombre,
                                                      NomTabla = "LibertadCondicionada",
                                                      datoExtra = $"CLAVE UNICA SCORPIO: {g.Key.ClaveUnicaScorpio}; Edad: {g.Key.Edad};\n Domicilio: {g.Key.Calle}, {g.Key.No}, {g.Key.NombreCf};\n Causa(s) penal(es): {string.Join(", ", g.Key.CausaPenal)};\n",
                                                      claveUnica = g.Key.claveunica
                                                  });

            var result = query.OrderBy(o => o.claveUnica);

            int idpersona = 0;
            string nomCom = "";
            string tabla = "";
            string datoextra = "";
            string clave = "";
            var cosine = new Cosine(2);
            double r = 0;
            var list = new List<Tuple<string, string, int, string, double, string>>();

            List<string> listaNombre = new List<string>();


            foreach (var q in result)
            {
                r = cosine.Similarity(q.nomcom, nombreCompleto);
                if (r >= 0.87)
                {
                    nomCom = q.nomcom;
                    idpersona = q.id;
                    tabla = q.NomTabla;
                    datoextra = q.datoExtra;
                    clave = q.claveUnica;
                    list.Add(new Tuple<string, string, int, string, double, string>(nomCom, tabla, idpersona, datoextra, r, clave));
                    simi = true;
                }
            }
            List<object> elementos = new List<object>();
            if (list.Count() != 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    double porcentaje = item.Item5 * 100;
                    int porcentajeFloor = (int)Math.Floor(porcentaje);
                    string nombreP = item.Item1;
                    string tablaP = item.Item2;
                    int idPersona = item.Item3;
                    string datoextraP = item.Item4;
                    string claveunica = item.Item6;
                    elementos.Add(new { Id = idPersona, Nombre = nombreP, Tabla = tablaP, Dato = datoextraP, Clave = claveunica });
                }
               
                return Json(new { success = true, lista = elementos });
            }
            return Json(new { success = false, lista = elementos });
        }


        public JsonResult Savcardatos(int id, string tabla)
        {
            List<object> listaDatos = new List<object>();

            switch (tabla)
            {
                case "MCYSCP":
                    var query = (from p in _context.Persona
                                 join d in _context.Domicilio on p.IdPersona equals d.PersonaIdPersona into domicilioJoin
                                 from d in domicilioJoin.DefaultIfEmpty()
                                 join e in _context.Estudios on p.IdPersona equals e.PersonaIdPersona into estudiosJoin
                                 from e in estudiosJoin.DefaultIfEmpty()
                                 join t in _context.Trabajo on p.IdPersona equals t.PersonaIdPersona into trabajoJoin
                                 from t in trabajoJoin.DefaultIfEmpty()
                                 join a in _context.Actividadsocial on p.IdPersona equals a.PersonaIdPersona into actividadessocialesJoin
                                 from a in actividadessocialesJoin.DefaultIfEmpty()
                                 join ae in _context.Abandonoestado on p.IdPersona equals ae.PersonaIdPersona into abandonoestadoJoin
                                 from ae in abandonoestadoJoin.DefaultIfEmpty()
                                 join s in _context.Saludfisica on p.IdPersona equals s.PersonaIdPersona into saludfisicaJoin
                                 from s in saludfisicaJoin.DefaultIfEmpty()
                                 where p.IdPersona == id
                                 select new
                                 {
                                     p,
                                     d,
                                     e,
                                     t,
                                     a,
                                     ae,
                                     s
                                 }).ToList();
                    listaDatos.AddRange(query);
                    break;
                case "Archivo":
                    var queryArchivo = (from p in _context.Archivo
                                   where p.IdArchivo == id
                                   select new
                                   {
                                       p
                                   }).ToList();
                    listaDatos.AddRange(queryArchivo);
                    break;
                case "Ejecucion":
                    var queryEjecucion = (from p in _context.Ejecucion
                                   where p.IdEjecucion == id
                                   select new
                                   {
                                       p
                                   }).ToList();
                    listaDatos.AddRange(queryEjecucion);
                    break;
                case "ServiciosPrevios":
                    var querySP = (from p in _context.Serviciospreviosjuicio
                                   where p.IdserviciosPreviosJuicio == id
                                   select new
                                   {
                                       p
                                   }).ToList();
                    listaDatos.AddRange(querySP);
                    break;
                case "PrisionPreventiva":
                    var queryPP = (from p in _context.Prisionespreventivas
                                   where p.Idprisionespreventivas == id
                                   select new
                                   {
                                       p
                                   }).ToList();
                    listaDatos.AddRange(queryPP);
                    break;
                case "Oficialia":
                    var queryO = (from p in _context.Oficialia
                                   where p.IdOficialia == id
                                   select new
                                   {
                                       p
                                   }).ToList();
                    listaDatos.AddRange(queryO);
                    break;
                case "LibertadCondicionada":
                    var queryCL = (from p in _context.Personacl
                                   join d in _context.Domiciliocl on p.IdPersonaCl equals d.PersonaclIdPersonacl into domicilioJoin
                                   from d in domicilioJoin.DefaultIfEmpty()
                                   join e in _context.Estudioscl on p.IdPersonaCl equals e.PersonaClIdPersonaCl into estudiosJoin
                                   from e in estudiosJoin.DefaultIfEmpty()
                                   join t in _context.Trabajocl on p.IdPersonaCl equals t.PersonaClIdPersonaCl into trabajoJoin
                                   from t in trabajoJoin.DefaultIfEmpty()
                                   join a in _context.Actividadsocialcl on p.IdPersonaCl equals a.PersonaClIdPersonaCl into actividadessocialesJoin
                                   from a in actividadessocialesJoin.DefaultIfEmpty()
                                   join ae in _context.Abandonoestadocl on p.IdPersonaCl equals ae.PersonaclIdPersonacl into abandonoestadoJoin
                                   from ae in abandonoestadoJoin.DefaultIfEmpty()
                                   join s in _context.Saludfisicacl on p.IdPersonaCl equals s.PersonaClIdPersonaCl into saludfisicaJoin
                                   from s in saludfisicaJoin.DefaultIfEmpty()
                                   where p.IdPersonaCl == id
                                   select new
                                   {
                                       p,
                                       d,
                                       e,
                                       t,
                                       a,
                                       ae,
                                       s
                                   }).ToList();
                    listaDatos.AddRange(queryCL);
                    break;
            }
            return Json(new { success = false, lista = listaDatos });
        }
        #endregion

        #region -SACAR CURP-
        public JsonResult cursJson(string paterno, string materno, DateTime? fnacimiento, string genero, string lnestado, string nombre)
        {
            var curs = mg.sacaCurs(paterno, materno, fnacimiento, genero, lnestado, nombre);
            return Json(new { success = true, responseText = Convert.ToString(0), curs = curs }); ;
        }
        #endregion
    }
}
