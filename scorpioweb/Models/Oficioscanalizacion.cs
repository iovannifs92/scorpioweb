using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Oficioscanalizacion
    {
        public int IdoficiosCanalizacion { get; set; }
        public string TipoArchivo { get; set; }
        public DateTime? FechaArchivo { get; set; }
        public string Observaciones { get; set; }
        public string RutaArchivo { get; set; }
        public int CanalizacionIdCanalizacion { get; set; }
    }
}
