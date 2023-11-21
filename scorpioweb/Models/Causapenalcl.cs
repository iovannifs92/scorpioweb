using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Causapenalcl
    {
        public int IdCausaPenalcl { get; set; }
        public string Cnpp { get; set; }
        public string Juez { get; set; }
        public string Cambio { get; set; }
        public string Distrito { get; set; }
        public string CausaPenal { get; set; }
        public DateTime? Fechacreacion { get; set; }
        public string Usuario { get; set; }
    }
}
