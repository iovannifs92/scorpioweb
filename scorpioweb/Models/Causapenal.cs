using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace scorpioweb.Models
{
    public partial class Causapenal
    {
        public int IdCausaPenal { get; set; }
        public string Cnpp { get; set; }
        public string Juez { get; set; }
        public string Cambio { get; set; }
        public string Distrito { get; set; }
        public string CausaPenal { get; set; }
        //public string CurrentFilter { get; set;}

        //public async Task OnGetAsync(string sortOrder, string searchString)
        //{
        //    Juez = sortOrder == "LName_Asc_Sort" ? " LName_Desc_Sort" : " LName_Asc_Sort";
        //    CausaPenal = sortOrder == "FName_Asc_Sort" ? " FName_Desc_Sort" : " FName_Asc_Sort";
        //    CurrentFilter = searchString;
        //}
    }
}
