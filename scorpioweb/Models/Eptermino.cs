using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Eptermino
    {
        public int Ideptermino { get; set; }
        public DateTime? Fecha { get; set; }
        public string Formaconclucion { get; set; }
        public string Urldocumento { get; set; }
        public int? EpcausapenalIdepcausapenal { get; set; }
    }
}
