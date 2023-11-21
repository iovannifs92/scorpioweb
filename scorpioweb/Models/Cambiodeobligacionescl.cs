using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Cambiodeobligacionescl
    {
        public int IdCambiodeObligacionescl { get; set; }
        public string SeDioCambio { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string MotivoAprobacion { get; set; }
        public int SupervisionclIdSupervisioncl { get; set; }
    }
}
