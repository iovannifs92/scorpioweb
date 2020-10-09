using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Supervision
    {
        public int IdSupervision { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Termino { get; set; }
        public string EstadoSupervision { get; set; }
        public int PersonaIdPersona { get; set; }
        public string EstadoCumplimiento { get; set; }
        public int CausaPenalIdCausaPenal { get; set; }
        public Persona personaVM { get; set; }

    }
}
