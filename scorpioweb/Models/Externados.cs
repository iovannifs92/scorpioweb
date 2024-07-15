using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Externados
    {
        public int Idexternados { get; set; }
        public string Nombre { get; set; }
        public string APaterno { get; set; }
        public string AMaterno { get; set; }
        public int? Telefono { get; set; }
        public string LnEstado { get; set; }
        public string FechaNacimiento { get; set; }
        public string Sexo { get; set; }
        public string CausaPenal { get; set; }
        public string Delito { get; set; }
        public string Observaciones { get; set; }
        public int? Edad { get; set; }
        public string ClaveUnicaScorpio { get; set; }
        public string Curp { get; set; }
    }
}
