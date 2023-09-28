using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Familiaresforaneoscl
    {
        public int IdFamiliaresForaneosCl { get; set; }
        public string Nombre { get; set; }
        public int? Edad { get; set; }
        public string Sexo { get; set; }
        public string Relacion { get; set; }
        public string TiempoConocerlo { get; set; }
        public string Pais { get; set; }
        public string Estado { get; set; }
        public string Telefono { get; set; }
        public string FrecuenciaContacto { get; set; }
        public string EnteradoProceso { get; set; }
        public string PuedeEnterarse { get; set; }
        public string Observaciones { get; set; }
        public int PersonaClIdPersonaCl { get; set; }
    }
}
