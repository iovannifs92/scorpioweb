using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Suspensionseguimiento
    {
        public int IdSuspensionSeguimiento { get; set; }
        public string Suspendido { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string MotivoSuspension { get; set; }
        public int SupervisionIdSupervision { get; set; }
    }
}
