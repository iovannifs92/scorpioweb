using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Monitoreo
    {
        public int Idmonitoreo { get; set; }
        public string Comentario { get; set; }
        public DateTime? Fecha { get; set; }
        public string MetodoVerificacion { get; set; }
        public int? IdEjeReinsercion { get; set; }
    }
}
