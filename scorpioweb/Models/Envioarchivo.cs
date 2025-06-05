using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Envioarchivo
    {
        public int IdenvioArchivo { get; set; }
        public string Nombre { get; set; }
        public string Apaterno { get; set; }
        public string Amaterno { get; set; }
        public string Causapenal { get; set; }
        public string Delito { get; set; }
        public string TipoDocumento { get; set; }
        public string SituacionJuridico { get; set; }
        public sbyte? Recibido { get; set; }
        public sbyte? Revisado { get; set; }
        public int? IdArchvo { get; set; }
        public string Observaciones { get; set; }
        public string Area { get; set; }
        public string Usuario { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaRecibido { get; set; }
        public DateTime? FechaRevisado { get; set; }
        public string QuienRecibe { get; set; }
        public string QuienRevisa { get; set; }
    }
}
