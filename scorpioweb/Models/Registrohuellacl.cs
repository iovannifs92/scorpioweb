using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Registrohuellacl
    {
        public int IdregistroHuellacl { get; set; }
        public byte[] FingerPrint { get; set; }
        public int? PersonaclIdPersonacl { get; set; }
        public string SupervisorH { get; set; }
        public DateTime? FechadeRegistro { get; set; }
    }
}
