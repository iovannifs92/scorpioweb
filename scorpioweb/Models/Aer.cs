using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Aer
    {
        public int IdAer { get; set; }
        public string CuentaEvaluacion { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string EvaluadorCaso { get; set; }
        public string RiesgoDetectado { get; set; }
        public string RiesgoSustraccion { get; set; }
        public string RiesgoObstaculizacion { get; set; }
        public string RiesgoVictima { get; set; }
        public int SupervisionIdSupervision { get; set; }
    }
}
