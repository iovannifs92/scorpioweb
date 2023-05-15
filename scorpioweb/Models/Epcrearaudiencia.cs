using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Epcrearaudiencia
    {
        public int IdepcrearAudiencia { get; set; }
        public DateTime? FechaAudiencia { get; set; }
        public string Usuario { get; set; }
        public DateTime? FechaNotificacion { get; set; }
        public string Juzgado { get; set; }
        public string Ce { get; set; }
        public string PaternoS { get; set; }
        public string MaternoS { get; set; }
        public string NombreS { get; set; }
        public int? EjecucionIdEjecucion { get; set; }
    }
}
