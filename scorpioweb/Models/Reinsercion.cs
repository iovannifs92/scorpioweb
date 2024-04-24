using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Reinsercion
    {
        public int IdReinsercion { get; set; }
        public string IdTabla { get; set; }
        public string Tabla { get; set; }
        public string Lugar { get; set; }
        public string Estado { get; set; }
    }
}
