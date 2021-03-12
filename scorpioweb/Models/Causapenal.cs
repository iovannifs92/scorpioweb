using EntityFrameworkCore.Triggers;
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
        public string Distrito { get; set ; }
        public string CausaPenal { get; set; }

        //static Causapenal()
        //{
        //    Triggers<Causapenal>.Updated += e =>
        //    {
        //        int a = 1;                
        //    };
        //}

    }
}
