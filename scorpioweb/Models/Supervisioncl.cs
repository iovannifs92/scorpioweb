using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Supervisioncl
    {
        public int IdSupervisioncl { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Termino { get; set; }
        public string EstadoSupervision { get; set; }
        public int PersonaclIdPersonacl { get; set; }
        public string EstadoCumplimiento { get; set; }
        public int CausaPenalclIdCausaPenalcl { get; set; }
        public string Tta { get; set; }
    }
}
