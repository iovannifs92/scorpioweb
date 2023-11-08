using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Expedienteunico
    {
        public int IdexpedienteUnico { get; set; }
        public string Persona { get; set; }
        public string Ejecucion { get; set; }
        public string Archivo { get; set; }
        public string Vinculacion { get; set; }
        public string Serviciospreviosjuicio { get; set; }
        public string Personacl { get; set; }
        public string Adolecentes { get; set; }
        public string Prisionespreventivas { get; set; }
        public string Oficialia { get; set; }
        public string ClaveUnicaScorpio { get; set; }
    }
}
