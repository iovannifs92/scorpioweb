using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Ejesreinsercion
    {
        public int IdejesReinsercion { get; set; }
        public string Tipo { get; set; }
        public DateTime? FechaCanalizacion { get; set; }
        public string Lugar { get; set; }
        public string Observaciones { get; set; }
        public string Estado { get; set; }
        public int CanalizacionIdCanalizacion { get; set; }
    }
}
