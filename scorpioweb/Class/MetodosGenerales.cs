using F23.StringSimilarity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using scorpioweb.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace scorpioweb.Class
{
    public class MetodosGenerales
    {
        public string normaliza(string normalizar)
        {
            if (!String.IsNullOrEmpty(normalizar))
            {
                normalizar = removeSpaces(normalizar.ToUpper());
            }
            else
            {
                normalizar = "NA";
            }
            return normalizar;
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


        public DateTime validateDatetime(string value)
        {
            try
            {
                return DateTime.Parse(value, new System.Globalization.CultureInfo("pt-BR"));
            }
            catch
            {
                return DateTime.ParseExact("1900/01/01", "yyyy/MM/dd", CultureInfo.InvariantCulture);
            }
        }


        public String BuscaId(List<SelectListItem> lista, String texto)
        {
            foreach (var item in lista)
            {
                if (normaliza(item.Value) == normaliza(texto))
                {
                    return item.Value;
                }
            }
            return "";
        }


        public string Dinero(string normaliza)
        {
            if (!String.IsNullOrEmpty(normaliza))
            {
                try
                {
                    normaliza = ((Convert.ToInt32(normaliza) / 100)).ToString("C", CultureInfo.CurrentCulture);
                }
                catch (System.FormatException e)
                {
                    return normaliza;
                }
            }
            else
            {
                normaliza = "NA";
            }
            return normaliza;
        }

        public String replaceSlashes(string path)
        {
            String cleaned = "";

            for (int i = 0; i < path.Length; i++)
                if (path[i] == '/')
                    cleaned += '-';
                else
                    cleaned += path[i];
            return cleaned;
        }

        Dictionary<string, string> reemplazoDiccionario = new Dictionary<string, string>
        {
            { "BACA", "BXCA" }, { "LOCO", "LXCO" },
            { "BAKA", "BXKA" }, { "LOKA", "LXKA" },
            { "BUEI", "BXEI" }, { "LOKO", "LXKO" },
            { "BUEY", "BXEY" }, { "MAME", "MXME" },
            { "CACA", "CXCA" }, { "MAMO", "MXMO" },
            { "CACO", "CXCO" }, { "MEAR", "MXAR" },
            { "CAGA", "CXGA" }, { "MEAS", "MXAS" },
            { "CAGO", "CXGO" }, { "MEON", "MXON" },
            { "CAKA", "CXKA" }, { "MIAR", "MXAR" },
            { "CAKO", "CXKO" }, { "MION", "MXON" },
            { "COGE", "CXGE" }, { "MOCO", "MXCO" },
            { "COGI", "CXGI" }, { "MOKO", "MXKO" },
            { "COJA", "CXJA" }, { "MULA", "MXLA" },
            { "COJE", "CXJE" }, { "MULO", "MXLO" },
            { "COJI", "CXJI" }, { "NACA", "NXCA" },
            { "COJO", "CXJO" }, { "NACO", "NXCO" },
            { "COLA", "CXLA" }, { "PEDA", "PXDA" },
            { "CULO", "CXLO" }, { "PEDO", "PXDO" },
            { "FALO", "FXLO" }, { "PENE", "PXNE" },
            { "FETO", "FXTO" }, { "PIPI", "PXPI" },
            { "GETA", "GXTA" }, { "PITO", "PXTO" },
            { "GUEI", "GXEI" }, { "POPO", "PXPO" },
            { "GUEY", "GXEY" }, { "PUTA", "PXTA" },
            { "JETA", "JXTA" }, { "PUTO", "PXTO" },
            { "JOTO", "JXTO" }, { "QULO", "QXLO" },
            { "KACA", "KXCA" }, { "RATA", "RXTA" },
            { "KACO", "KXCO" }, { "ROBA", "RXBA" },
            { "KAGA", "KXGA" }, { "ROBE", "RXBE" },
            { "KAGO", "KXGO" }, { "ROBO", "RXBO" },
            { "KAKA", "KXKA" }, { "RUIN", "RXIN" },
            { "KAKO", "KXKO" }, { "SENO", "SXNO" },
            { "KOGE", "KXGE" }, { "TETA", "TXTA" },
            { "KOGI", "KXGI" }, { "VACA", "VXCA" },
            { "KOJA", "KXJA" }, { "VAGA", "VXGA" },
            { "KOJE", "KXJE" }, { "VAGO", "VXGO" },
            { "KOJI", "KXJI" }, { "VAKA", "VXKA" },
            { "KOJO", "KXJO" }, { "VUEI", "VXEI" },
            { "KOLA", "KXLA" }, { "VUEY", "VXEY" },
            { "KULO", "KXLO" }, { "WUEI", "WXEI" },
            { "LILO", "LXLO" }, { "WUEY", "WXEY" },
            { "LOCA", "LXCA" }
        // Agrega más palabras y sus reemplazos aquí
        };
        //Anexo 2 Catalogo de Palabras Inconvenientes
        public String anexoDosCurp(string palabra)
        {
            string[] palabras = palabra.Split(' ');

            List<string> palabrasModificadas = new List<string>();

            foreach (string palabrab in palabras)
            {
                if (reemplazoDiccionario.ContainsKey(palabrab))
                {
                    palabrasModificadas.Add(reemplazoDiccionario[palabrab]);
                }
                else
                {
                    palabrasModificadas.Add(palabrab);
                }
            }

            string resultado = string.Join(" ", palabrasModificadas);
            return resultado;
        }

        public String sacaCurs(string paterno, string materno, DateTime? fnacimiento, string genero, string lnestado, string nombre)
        {
            int i;
            StringBuilder curs = new StringBuilder("*****");
            string paternoN = normaliza(paterno);
            string maternoN = normaliza(materno);
            string generoN = normaliza(genero);
            string nombreN = normaliza(nombre);

            if ("Ñ".IndexOf(paternoN[0]) == 0)
            {
                //for (int a = 0; a < paternoN.Length; a++)
                //{
                var letras = paternoN[0].ToString().Replace('Ñ', 'X');
                curs[0] = letras[0];
                //}
            }
            else
            {
                curs[0] = paternoN[0];
            }

            for (i = 0; i < paternoN.Length; i++)
            {
                if ("AEIOU".IndexOf(paternoN[i]) >= 0)
                {
                    break;
                }
            }
            if (i < paterno.Length)
            {
                curs[1] = paterno[i];
            }
            curs[2] = maternoN[0];
            //curs[3] = nombreN[0];
            string[] nombres = nombreN.Split(' '); // Divide el nombre en partes separadas por espacios
            if (nombres.Length > 1) // Si hay al menos dos partes en el nombre
            {
                if (!(nombreN.Substring(0, 5) == "MARIA" || nombreN == "MA" || nombreN == "MA." || nombreN.Substring(0, 4) == "JOSE" || nombreN == "J" || nombreN == "J."))
                {
                    curs[3] = nombres[0][0];
                }
                else
                {
                    curs[3] = nombres[1][0]; // Usa la primera letra del segundo nombre
                }
            }
            else
            {
                curs[3] = nombreN[0]; // Si solo hay un nombre, usa la segunda letra del primer nombre
            }

            //switch (nombres.Length)
            //{
            //    case 1:
            //        curs[3] = nombreN[0];
            //        break;
            //    case 2:
            //        if (!(nombreN == "MARIA" || nombreN == "MA" || nombreN == "MA." || nombreN == "JOSE" || nombreN == "J" || nombreN == "J."))
            //        {
            //            curs[3] = nombres[0][0];
            //        }
            //        else
            //        {
            //            curs[3] = nombres[1][0]; // Usa la primera letra del segundo nombre
            //        }
            //        break;
            //    case 50:
            //        curs[3] = nombreN[0];
            //        break;
            //}
            //for (int a = 1; a < nombreN.Length; a++)
            //{
            //    if ("AEIOU".IndexOf(maternoN[a]) == -1)
            //    {
            //        break;
            //    }
            //}


            var anexoDos = anexoDosCurp(curs.ToString().Substring(0, 4));
            curs.Insert(0, anexoDos);
            curs.Insert(4, fnacimiento.Value.ToString("yyMMdd"));

            if (generoN == "M")
                curs[10] = 'H';
            else if (generoN == "F")
                curs[10] = 'M';
            else if (generoN == "N")
                curs[10] = 'X';
            //https://es.wikipedia.org/wiki/Plantilla:Abreviaciones_de_los_estados_de_M%C3%A9xico
            string[] abreviacionesEstados = { "**", "AG", "BC", "BS", "CM", "CO", "CL", "CS", "CH", "CX", "DG", "GT", "GR", "HG", "JC", "EM", "MI", "MO", "NA", "NL", "OA", "PU", "QT", "QR", "SL", "SI", "SO", "TB", "TM", "TL", "VE", "YU", "ZA", "EX" };
            curs.Insert(11, abreviacionesEstados[Int32.Parse(lnestado)]);
            for (i = 1; i < paternoN.Length; i++)
            {
                if ("AEIOU".IndexOf(paternoN[i]) == -1)
                {
                    break;
                }
            }
            if (i < paternoN.Length)
            {
                var letra = paternoN[i].ToString();
                if (letra == "Ñ")
                {
                    curs[13] = Convert.ToChar("X");
                }
                else
                {
                    curs[13] = paternoN[i];
                }
            }
            for (i = 1; i < maternoN.Length; i++)
            {
                if ("AEIOU".IndexOf(maternoN[i]) == -1)
                {
                    break;
                }
            }
            if (i < maternoN.Length)
            {
                var letra = maternoN[i].ToString();
                if (letra == "Ñ")
                {
                    curs[14] = Convert.ToChar("X");
                }
                else
                {
                    curs[14] = maternoN[i];
                }
            }
            for (i = 1; i < nombreN.Length; i++)
            {
                if ("AEIOU".IndexOf(nombreN[i]) == -1)
                {
                    break;
                }
            }
            if (i < nombreN.Length)
            {
                var letra = nombreN[i].ToString();
                if (letra == "Ñ")
                {
                    curs[15] = Convert.ToChar("X");
                }
                else
                {
                    curs[15] = nombreN[i];
                }
            }
            if (Int32.Parse(fnacimiento.Value.ToString("yyyy")) < 2000)
            {
                curs[16] = '0';
            }
            else
            {
                curs[16] = 'A';
            }
            return curs.ToString().ToUpper();
        }
        public string RemoveWhiteSpaces(string str)
        {
            return Regex.Replace(str, @"\s+", String.Empty);
        }

        Dictionary<string, string> remplazoBases = new Dictionary<string, string>
        {
             { "MCYSCP", "persona" },
            { "Archivo", "archivo" },
            { "Ejecucion", "ejecucion" },
            { "ServiciosPrevios", "serviciospreviosjuicio" },
            { "PrisionPreventiva", "prisionespreventivas" },
            { "Vinculacion", "vinculacion" },
            { "Oficialia", "oficialia" },
            { "LibertadCondicionada", "personacl" },
            { "Adolecentes", "adolecentess" }
        };

        public String cambioAbase(string palabra)
        {
            string[] palabras = palabra.Split(' ');

            List<string> palabrasModificadas = new List<string>();

            foreach (string palabrab in palabras)
            {
                if (remplazoBases.ContainsKey(palabrab))
                {
                    palabrasModificadas.Add(remplazoBases[palabrab]);
                }
                else
                {
                    palabrasModificadas.Add(palabrab);
                }
            }

            string resultado = string.Join(" ", palabrasModificadas);

            return resultado;
        }

        public int CalcularEdad(DateTime fechaNacimiento)
        {
            DateTime fechaActual = DateTime.Today;
            int edad = fechaActual.Year - fechaNacimiento.Year;

            // Ajusta la edad si la fecha de nacimiento no ha ocurrido todavía este año
            if (fechaNacimiento > fechaActual.AddYears(-edad))
            {
                edad--;
            }

            return edad;
        }

        //public async Task expedienteUnico(
        //   string var_tablanueva,
        //   string var_tablaSelect,
        //   int var_idnuevo,
        //   int var_idSelect,
        //   string var_curs,
        //   string CURSUsada,
        //   string datosArray)
        //{
        //    string var_tablaCurs = "ClaveUnicaScorpio";

        //    if (var_tablaSelect != null)
        //    {
        //        var_tablanueva = cambioAbase(RemoveWhiteSpaces(var_tablanueva));
        //        var_tablaSelect = cambioAbase(RemoveWhiteSpaces(var_tablaSelect));
        //        var_idnuevo = var_idnuevo;
        //        var_idSelect = var_idSelect;
        //        if (CURSUsada != null)
        //        {
        //            var_curs = CURSUsada;
        //        }

        //        string query = $"CALL spInsertExpedienteUnico('{var_tablanueva}', '{var_tablaSelect}', '{var_tablaCurs}', {var_idnuevo}, {var_idSelect},  '{var_curs}');";
        //        _context.Database.ExecuteSqlCommand(query);

        //        if (datosArray != null)
        //        {
        //            List<Dictionary<string, string>> listaObjetos = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(datosArray);

        //            // Proyectar la lista para obtener solo los valores de id y tabla
        //            var resultados = listaObjetos.Select(obj => new {
        //                id = obj["id"],
        //                tabla = obj["tabla"]
        //            });

        //            foreach (var resultado in resultados)
        //            {
        //                var_tablanueva = mg.cambioAbase(RemoveWhiteSpaces("MCYSCP"));
        //                var_tablaSelect = mg.cambioAbase(mRemoveWhiteSpaces(resultado.tabla));
        //                var_tablaCurs = "ClaveUnicaScorpio";
        //                var_idnuevo = var_idnuevo;
        //                var_idSelect = Int32.Parse(resultado.id);

        //                string query2 = $"CALL spInsertExpedienteUnico('{var_tablanueva}', '{var_tablaSelect}', '{var_tablaCurs}', {var_idnuevo}, {var_idSelect},  '{var_curs}');";
        //                _context.Database.ExecuteSqlCommand(query2);
        //            }
        //        }

        //    }
        //    //else
        //    //{
        //    //    if (CURSUsada != null)
        //    //    {
        //    //        var unica = (from eu in _context.Expedienteunico
        //    //                     where eu.ClaveUnicaScorpio == CURSUsada
        //    //                     select eu.IdexpedienteUnico).FirstOrDefault();
        //    //        if (unica == 0)
        //    //        {
        //    //            expedienteunico.ClaveUnicaScorpio = CURSUsada;
        //    //            expedienteunico.Persona = var_idnuevo.ToString();
        //    //            _context.Add(expedienteunico);
        //    //        }
        //    //        else
        //    //        {
        //    //            var query = (from a in _context.Expedienteunico
        //    //                         where a.IdexpedienteUnico == unica
        //    //                         select a).FirstOrDefault();

        //    //            query.Persona = var_idnuevo.ToString();
        //    //            _context.SaveChanges();
        //    //        }
        //    //        if (datosArray != null)
        //    //        {
        //    //            List<Dictionary<string, string>> listaObjetos = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(datosArray);

        //    //            // Proyectar la lista para obtener solo los valores de id y tabla
        //    //            var resultados = listaObjetos.Select(obj => new {
        //    //                id = obj["id"],
        //    //                tabla = obj["tabla"]
        //    //            });

        //    //            foreach (var resultado in resultados)
        //    //            {
        //    //                var_tablanueva = mg.cambioAbase(mg.RemoveWhiteSpaces("MCYSCP"));
        //    //                var_tablaSelect = mg.cambioAbase(mg.RemoveWhiteSpaces(resultado.tabla));
        //    //                var_tablaCurs = "ClaveUnicaScorpio";
        //    //                var_idnuevo = var_idnuevo;
        //    //                var_idSelect = Int32.Parse(resultado.id);

        //    //                string query2 = $"CALL spInsertExpedienteUnico('{var_tablanueva}', '{var_tablaSelect}', '{var_tablaCurs}', {var_idnuevo}, {var_idSelect},  '{var_curs}');";
        //    //                _context.Database.ExecuteSqlCommand(query2);
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //}

    }
}
