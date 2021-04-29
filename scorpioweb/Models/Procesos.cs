using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace scorpioweb.Models
{
    public partial class Procesos
    {
        public Supervision supervisionVM { get; set; }
        public Persona personaVM { get; set; }
        public Causapenal causapenalVM { get; set; }
        public Fraccionesimpuestas fraccionesimpuestasVM { get; set; }
        public Planeacionestrategica planeacionestrategicaVM { get; set; }
    }
}
