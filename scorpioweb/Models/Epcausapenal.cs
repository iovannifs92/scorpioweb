using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Epcausapenal
    {
        public int Idepcausapenal { get; set; }
        public string Causapenal { get; set; }
        public string Delito { get; set; }
        public string Clasificaciondelito { get; set; }
        public string JuzgadoOrigen { get; set; }
        public DateTime? FechaSentencia { get; set; }
        public string Multa { get; set; }
        public string Reparacion { get; set; }
        public DateTime? Firmeza { get; set; }
        public int? Penaanos { get; set; }
        public int? Penameses { get; set; }
        public int? Penadias { get; set; }
        public DateTime? Apartir { get; set; }
        public sbyte? EstadodeCausa { get; set; }
        public int? EjecucionIdEjecucion { get; set; }
    }
}
