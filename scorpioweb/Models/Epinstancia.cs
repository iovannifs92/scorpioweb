using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Epinstancia
    {
        public int Idepinstancia { get; set; }
        public DateTime? Fecha { get; set; }
        public string Multa { get; set; }
        public string Reparacion { get; set; }
        public DateTime? Firmeza { get; set; }
        public int? Penaanos { get; set; }
        public int? Penameses { get; set; }
        public int? Penadias { get; set; }
        public int? EpcausapenalIdepcausapenal { get; set; }
    }
}
