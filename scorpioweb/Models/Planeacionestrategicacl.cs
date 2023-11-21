using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Planeacionestrategicacl
    {
        public int IdPlaneacionEstrategicacl { get; set; }
        public string PlanSupervision { get; set; }
        public string MotivoNoPlaneacion { get; set; }
        public DateTime? VisitaVerificacion { get; set; }
        public DateTime? InformeInicial { get; set; }
        public DateTime? InformeSeguimiento { get; set; }
        public DateTime? InformeFinal { get; set; }
        public DateTime? FechaUltimoContacto { get; set; }
        public DateTime? FechaProximoContacto { get; set; }
        public string DiaFirma { get; set; }
        public string PeriodicidadFirma { get; set; }
        public int SupervisionclIdSupervisioncl { get; set; }
    }
}
