using System;

namespace scorpioweb.Models
{
    public class AlertasEjecucionViewModel
    {
        public int idTabla { get; set; }
        public string nombre { get; set; }
        public DateTime? fechaRecepcion { get; set; }
        public DateTime? fechaAlerta { get; set; }
        public string usuario { get; set; }
        public string juzgado { get; set; }
        public string carpetaEjecucion { get; set; }
        public string tipoAdvertencia { get; set; }
        public string mensajeAdvertencia { get; set; }


    }
}
