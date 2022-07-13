using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Historialeliminacion
    {
        public int IdhistorialEliminacion { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public int? Idpersona { get; set; }
        public string Razon { get; set; }
        public string Usuario { get; set; }
        public string Supervisor { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
