using System;
using System.Collections.Generic;


namespace scorpioweb.Models
{
    public class OficialiaReporte
    {
        public int IdOficialia { get; set; }
        public string NumOficio { get; set; }
        public string FechaRecepcion { get; set; }
        public string FechaEmision { get; set; }
        public string Expide { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombre { get; set; }
        public string CarpetaEjecucion { get; set; }
        public string CausaPenal { get; set; }
        public string AsuntoOficio { get; set; }
        public string Observaciones { get; set; }
    }
}
