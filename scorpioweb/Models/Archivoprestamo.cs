using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Archivoprestamo
    {
        public int IdArchivoPrestamo { get; set; }
        public string Entrega { get; set; }
        public string Recibe { get; set; }
        public string Area { get; set; }
        public DateTime? FechaInicial { get; set; }
        public DateTime? FechaRenovacion { get; set; }
        public string Estatus { get; set; }
        public string Renovaciones { get; set; }
        public string Urlvale { get; set; }
        public int? ArcchivoIdArchivo { get; set; }
    }
}
