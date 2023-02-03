using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Archivoprestamodigital
    {
        public int IdArchivoPrestamoDigital { get; set; }
        public string Usuario { get; set; }
        public DateTime? FechaPrestamo { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string TiempoConsulta { get; set; }
        public string UsuarioOtorgaPermiso { get; set; }
        public int? ArchivoIdArchivo { get; set; }
    }
}
