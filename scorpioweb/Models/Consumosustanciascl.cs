using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Consumosustanciascl
    {
        public int IdConsumoSustanciasCl { get; set; }
        public string Sustancia { get; set; }
        public string Consume { get; set; }
        public string Frecuencia { get; set; }
        public string Cantidad { get; set; }
        public DateTime? UltimoConsumo { get; set; }
        public string Observaciones { get; set; }
        public int PersonaClIdPersonaCl { get; set; }
    }
}
