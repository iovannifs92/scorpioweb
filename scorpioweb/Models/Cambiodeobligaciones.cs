using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Cambiodeobligaciones
    {
        public int IdCambiodeObligaciones { get; set; }
        public string SeDioCambio { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string MotivoAprobacion { get; set; }
        public int SupervisionIdSupervision { get; set; }
    }
}
