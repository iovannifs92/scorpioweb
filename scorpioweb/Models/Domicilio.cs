using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Domicilio
    {
        public int IdDomicilio { get; set; }
        public string TipoDomicilio { get; set; }
        public string Calle { get; set; }
        public string No { get; set; }
        public string TipoUbicacion { get; set; }
        public string NombreCf { get; set; }
        public string Pais { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        public string Temporalidad { get; set; }
        public string ResidenciaHabitual { get; set; }
        public string Cp { get; set; }
        public string Referencias { get; set; }
        public string Horario { get; set; }
        public string DomcilioSecundario { get; set; }
        public string Observaciones { get; set; }
        public int PersonaIdPersona { get; set; }
        public string Zona { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
    }
}
