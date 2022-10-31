using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Contactos
    {
        public int Idcontactomunicipio { get; set; }
        public string Lugar { get; set; }
        public string Dependencia { get; set; }
        public string NombreTitular { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Extencion { get; set; }
        public string EstadoMunicipio { get; set; }

        
    }
}
