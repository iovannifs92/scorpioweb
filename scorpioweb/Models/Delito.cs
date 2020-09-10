using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Delito
    {
        public int IdDelito { get; set; }
        public string Tipo { get; set; }
        public string Modalidad { get; set; }
        public string EspecificarDelito { get; set; }
        public int CausaPenalIdCausaPenal { get; set; }
    }
}
