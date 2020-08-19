using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Revocacion
    {
        public int IdRevocacion { get; set; }
        public string Revocado { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string MotivoRevocacion { get; set; }
        public int SupervisionIdSupervision { get; set; }
    }
}
