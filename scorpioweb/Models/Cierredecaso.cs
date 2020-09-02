using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Cierredecaso
    {
        public int IdCierreDeCaso { get; set; }
        public string SeCerroCaso { get; set; }
        public string ComoConcluyo { get; set; }
        public string NoArchivo { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string Autorizo { get; set; }
        public int SupervisionIdSupervision { get; set; }
    }
}
