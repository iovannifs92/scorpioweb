using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Victima
    {
        public int IdVictima { get; set; }
        public string NombreV { get; set; }
        public string Edad { get; set; }
        public string Telefono { get; set; }
        public string ConoceDetenido { get; set; }
        public string TipoRelacion { get; set; }
        public string TiempoConocerlo { get; set; }
        public string ViveSupervisado { get; set; }
        public string Direccion { get; set; }
        public string Victimacol { get; set; }
        public int SupervisionIdSupervision { get; set; }
        public string Observaciones { get; set; }
    }
}
