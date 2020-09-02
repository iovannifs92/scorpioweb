using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Archivo
    {
        public int IdArchivo { get; set; }
        public int? NoArchivo { get; set; }
        public string Nombre { get; set; }
        public string CausaPenal { get; set; }
        public string Delito { get; set; }
        public string Sentencia { get; set; }
        public string Situacion { get; set; }
        public DateTime? FechaAcuerdo { get; set; }
        public string CarpetaEjecucion { get; set; }
        public string Observaciones { get; set; }
        public string Envia { get; set; }
        public string Prestado { get; set; }
        public string AreaPrestamo { get; set; }
        public DateTime? FechaPrestamo { get; set; }
    }
}
