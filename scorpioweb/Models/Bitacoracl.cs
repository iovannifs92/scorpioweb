using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Bitacoracl
    {
        public int IdBitacoracl { get; set; }
        public DateTime? Fecha { get; set; }
        public string TipoPersona { get; set; }
        public string Texto { get; set; }
        public string TipoVisita { get; set; }
        public string RutaEvidencia { get; set; }
        public int SupervisionclIdSupervisioncl { get; set; }
        public int? BeneficiosclIdBeneficioscl { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int? OficialiaIdOficialia { get; set; }
    }
}
