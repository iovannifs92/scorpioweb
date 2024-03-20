using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Psicologiavincu
    {
        public int IdpsicologiaVincu { get; set; }
        public string RParejahijo { get; set; }
        public string RPadremadre { get; set; }
        public string Emocionalmente { get; set; }
        public string ApoyoPsicologica { get; set; }
        public string IneVigente { get; set; }
        public string ApoyoTramite { get; set; }
        public string AsesoriaJuri { get; set; }
        public string CualAsesoriaJuri { get; set; }
        public int PersonaClIdPersonaCl { get; set; }
    }
}
