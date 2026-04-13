using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Solicitudpresentacionperiodica
    {
        public int IdsolicitudPresentacionPeriodica { get; set; }
        public string Usuario { get; set; }
        public string Area { get; set; }
        public int? Idpersona { get; set; }
        public DateTime? FechaSubsanada { get; set; }
        public string Motivo { get; set; }
        public sbyte? Aprobada { get; set; }
        public sbyte? Subida { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
