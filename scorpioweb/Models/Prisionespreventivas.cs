using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Prisionespreventivas
    {
        public int Idprisionespreventivas { get; set; }
        public int? NumeroControl { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombre { get; set; }
        public string Genero { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public string CausaPenal { get; set; }
        public string Delito { get; set; }
        public string Capturista { get; set; }
        public string Observaciones { get; set; }
        public string RutaArchivo { get; set; }

        public string NombreCompleto
        {
            get
            {
                return this.Paterno + " " + this.Materno + " " + this.Nombre;
            }
        }
    }
}
