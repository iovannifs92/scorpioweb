using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using scorpioweb.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace scorpioweb.Class
{
    public class MetodosGenerales
    {
        private readonly UserManager<ApplicationUser> userManager;
        public string normaliza(string normalizar)
        {
            if (!String.IsNullOrEmpty(normalizar))
            {
                normalizar = normalizar.ToUpper();
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


    }
}
