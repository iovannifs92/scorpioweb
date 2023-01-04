using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Calendario
    {
        public int Idcalendario { get; set; }
        public string Mensaje { get; set; }
        public DateTime? FechaEvento { get; set; }
        public string Prioridad { get; set; }
        public string Usuario { get; set; }
        public string Tipo { get; set; }
        public string Repite { get; set; }
        public string Frecuencia { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int? SupervisionIdSupervision { get; set; }
    }
}
