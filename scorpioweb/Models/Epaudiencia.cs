using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Epaudiencia
    {
        public int Idepaudiencia { get; set; }
        public string Causapenal { get; set; }
        public string Beneficio { get; set; }
        public DateTime? InicioFirma { get; set; }
        public DateTime? FinFirma { get; set; }
        public DateTime? FechatrabajoIe { get; set; }
        public string GrupoAutoayuda { get; set; }
        public string Observaciones { get; set; }
        public int? EjecucionIdEjecucion { get; set; }
    }
}
