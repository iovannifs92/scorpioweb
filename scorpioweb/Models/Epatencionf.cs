using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Epatencionf
    {
        public int IdepAtencionF { get; set; }
        public string Turno { get; set; }
        public string QuienAtiende { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Termino { get; set; }
        public string Observaciones { get; set; }
        public int? EjecucionIdEjecucion { get; set; }
    }
}
