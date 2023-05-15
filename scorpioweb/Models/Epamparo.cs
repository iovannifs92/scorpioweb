using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Epamparo
    {
        public int Idepamparo { get; set; }
        public DateTime? Fecha { get; set; }
        public string Toca { get; set; }
        public string Observaciones { get; set; }
        public int? EjecucionIdEjecucion { get; set; }
    }
}
