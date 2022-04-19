using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace scorpioweb.Models
{
    public class PlaneacionWarningViewModel
    {
        public Persona personaVM { get; set; }
        public Supervision supervisionVM { get; set; }
        public Causapenal causapenalVM { get; set; }
        public Planeacionestrategica planeacionestrategicaVM { get; set; }
        public Fraccionesimpuestas fraccionesimpuestasVM { get; set; }
        public Municipios municipiosVM { get; set; }
        public Estados estadosVM { get; set; }
        public Archivointernomcscp archivointernomcscpVM { get; set; }
        public string tipoAdvertencia { get; set; }
        public string figuraJudicial { get; set; }
    }
}
