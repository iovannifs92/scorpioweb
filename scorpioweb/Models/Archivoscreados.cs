using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Archivoscreados
    {
        public int IdarchivosCreados { get; set; }
        public string Area { get; set; }
        public string Tipo { get; set; }
        public string Usuario { get; set; }
        public DateTime? Fecha { get; set; }
        public string Idpersona { get; set; }
        public string Idsupervision { get; set; }
    }
}
