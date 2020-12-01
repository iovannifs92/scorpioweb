using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Fraccionesimpuestas
    {
        public int IdFracciones { get; set; }
        public string Tipo { get; set; }
        public string Autoridad { get; set; }        
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaTermino { get; set; }
        public string Estado { get; set; }
        public string Evidencia { get; set; }
        public string FiguraJudicial { get; set; }
        public int SupervisionIdSupervision { get; set; }
    }
}
