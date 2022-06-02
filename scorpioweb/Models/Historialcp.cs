using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Historialcp
    {
        public int IdHistorialcp { get; set; }
        public string Cnpp { get; set; }
        public string Juez { get; set; }
        public string Cambio { get; set; }
        public string Distrito { get; set; }
        public string Causapenal { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? CausapenalIdCausapenal { get; set; }
    }
}
