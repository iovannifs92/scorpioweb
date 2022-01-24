using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Presentacionperiodica
    {
        public int IdpresentacionPeriodica { get; set; }
        public DateTime? FechaFirma { get; set; }
        public string ComentarioFirma { get; set; }
        public int? RegistroidHuella { get; set; }
    }
}
