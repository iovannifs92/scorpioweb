using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Ejecucion
    {
        public int IdEjecucion { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombre { get; set; }
        public string Yo { get; set; }
        public string Ce { get; set; }
        public string Usuario { get; set; }
        public string LugarInternamiento { get; set; }
        public string Juzgado { get; set; }
        public string CeAcumuladas { get; set; }
        public string TieneceAcumuladas { get; set; }
    }
}
