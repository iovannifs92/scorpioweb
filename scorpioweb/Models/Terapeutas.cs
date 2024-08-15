using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Terapeutas
    {
        public int IdTerapeutas { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string TipoTerapia { get; set; }
        public string Username { get; set; }
    }
}
