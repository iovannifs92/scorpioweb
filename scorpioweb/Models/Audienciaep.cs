using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Audienciaep
    {
        public int IdaudienciaEp { get; set; }
        public DateTime? FechaNotificacion { get; set; }
        public string Sentenciado { get; set; }
        public DateTime? FechaAudiencia { get; set; }
        public string Juzgado { get; set; }
        public string CarpetaEjecucion { get; set; }
        public string Usuario { get; set; }
        public int? IdejecucionEjecucion { get; set; }
    }
}
