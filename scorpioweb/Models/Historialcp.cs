using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Historialcp
    {
        public int Idhistorialcp { get; set; }
        public string CpAnterior { get; set; }
        public string Distrito { get; set; }
        public int? CausapenalIdCausapenal { get; set; }
    }
}
