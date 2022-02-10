using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Archivointernomcscp
    {
        public int IdarchivoInternoMcscp { get; set; }
        public int? PersonaIdPersona { get; set; }
        public DateTime? Fecha { get; set; }
        public string Usuario { get; set; }
        public string NuevaUbiacion { get; set; }
        public string CausaPenal { get; set; }
    }
}
