using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Planeacion
    {
        public int IdPlaneacion { get; set; }
        public string Informe { get; set; }
        public DateTime? FechaInforme { get; set; }
        public string Observaciones { get; set; }
        public int ReincercionIdReincercion { get; set; }
    }
}
