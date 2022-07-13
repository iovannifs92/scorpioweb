using System;
using System.Collections.Generic;

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
        public DateTime? Fechacreacion { get; set; }
        public string Usuario { get; set; }
        public string CausaPenalCompleta
        {
            get
            {
                return this.CausaPenal + ", Distrito " + this.Distrito + ", " + this.Juez;
            }
        }
    }
}
