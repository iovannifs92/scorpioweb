using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office.CoverPageProps;

namespace scorpioweb.Models
{
    public class PlaneacionclWarningViewModel
    {

        public Personacl personaclVM { get; set; }
        public Supervisioncl supervisionclVM { get; set; }
        public Causapenalcl causapenalclVM { get; set; }
        public Planeacionestrategicacl planeacionestrategicaclVM { get; set; }
        public Beneficios beneficiosclVM { get; set; }
        public Municipios municipiosVM { get; set; }
        public Estados estadosVM { get; set; }
        
        public string tipoAdvertencia { get; set; }
        public string figuraJudicial { get; set; }
        public DateTime? fechaCmbio { get; set; }
        public string nivelAlerta { get; set; }
    }
}
