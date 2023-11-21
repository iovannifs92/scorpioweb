using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Presentacionperiodicacl
    {
        public int IdpresentacionPeriodicacl { get; set; }
        public DateTime? FechaFirma { get; set; }
        public string ComentarioFirma { get; set; }
        public int? IdregistroHuellacl { get; set; }
    }
}
