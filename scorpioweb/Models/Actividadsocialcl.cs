using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Actividadsocialcl
    {
        public int IdActividadSocialCl { get; set; }
        public string TipoActividad { get; set; }
        public string Horario { get; set; }
        public string Lugar { get; set; }
        public string Telefono { get; set; }
        public string SePuedeEnterar { get; set; }
        public string Referencia { get; set; }
        public string Observaciones { get; set; }
        public int PersonaClIdPersonaCl { get; set; }
        public string ActividadesDepCulCentro { get; set; }
        public string CualActividadesDepCulCentro { get; set; }
        public string DeseaDepCul { get; set; }
        public string CualDeseaDepCul { get; set; }
    }
}
