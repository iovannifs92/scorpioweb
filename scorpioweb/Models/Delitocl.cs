using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Delitocl
    {
        public int IdDelitocl { get; set; }
        public string Tipo { get; set; }
        public string Modalidad { get; set; }
        public string EspecificarDelito { get; set; }
        public int CausaPenalclIdCausaPenalcl { get; set; }
    }
}
