using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Planeacionestrategica
    {
        public int IdPlaneacionEstrategica { get; set; }
        public string PlanSupervision { get; set; }
        public string MotivoNoPlaneacion { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string UltimoInforme { get; set; }
        public DateTime? FechaInforme { get; set; }
        public DateTime? FechaUltimoContacto { get; set; }
        public DateTime? FechaProximoContacto { get; set; }
        public DateTime? FechaAudienciaRs { get; set; }
        public string DiaFirma { get; set; }
        public string PeriodicidadFirma { get; set; }
        public int SupervisionIdSupervision { get; set; }
        public string Planeacionestrategicacol { get; set; }
    }
}
