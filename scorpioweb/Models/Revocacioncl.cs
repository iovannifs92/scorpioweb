using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Revocacioncl
    {
        public int IdRevocacioncl { get; set; }
        public string Revocado { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string MotivoRevocacion { get; set; }
        public int SupervisionclIdSupervisioncl { get; set; }
    }
}
