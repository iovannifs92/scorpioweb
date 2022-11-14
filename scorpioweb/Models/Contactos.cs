using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Contactos
    {
        public int Idcontactos { get; set; }
        public string Categoria { get; set; }
        public string Lugar { get; set; }
        public string Dependencia { get; set; }
        public string Titular { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Extencion { get; set; }
        public sbyte? Destacado { get; set; }
    }
}
