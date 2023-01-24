using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Archivoregistro
    {
        public int IdArchivoRegistro { get; set; }
        public string CausaPenal { get; set; }
        public string Delito { get; set; }
        public string Sentencia { get; set; }
        public string Situacion { get; set; }
        public DateTime? FechaAcuerdo { get; set; }
        public string CarpetaEjecucion { get; set; }
        public string Observaciones { get; set; }
        public string Envia { get; set; }
        public int? ArchivoIdArchivo { get; set; }
    }
}
