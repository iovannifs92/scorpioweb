using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Bitacora
    {
        public int IdBitacora { get; set; }
        public DateTime? Fecha { get; set; }
        public string TipoPersona { get; set; }
        public string Texto { get; set; }
        public string TipoVisita { get; set; }
        public string RutaEvidencia { get; set; }
        public int SupervisionIdSupervision { get; set; }
        public int? FracionesImpuestasIdFracionesImpuestas { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? OficialiaIdOficialia { get; set; }
    }
}
