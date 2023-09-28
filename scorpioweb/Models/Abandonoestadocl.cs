using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Abandonoestadocl
    {
        public int IdAbandonoEstadocl { get; set; }
        public string VividoFuera { get; set; }
        public string LugaresVivido { get; set; }
        public string TiempoVivido { get; set; }
        public string MotivoVivido { get; set; }
        public string ViajaHabitual { get; set; }
        public string LugaresViaje { get; set; }
        public string TiempoViaje { get; set; }
        public string MotivoViaje { get; set; }
        public string DocumentacionSalirPais { get; set; }
        public string Pasaporte { get; set; }
        public string Visa { get; set; }
        public string FamiliaresFuera { get; set; }
        public int? Cuantos { get; set; }
        public int PersonaclIdPersonacl { get; set; }
    }
}
