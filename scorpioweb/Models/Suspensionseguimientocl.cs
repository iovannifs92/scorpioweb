using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Suspensionseguimientocl
    {
        public int IdSuspensionSeguimientocl { get; set; }
        public string Suspendido { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string MotivoSuspension { get; set; }
        public int SupervisionclIdSupervisioncl { get; set; }
    }
}
