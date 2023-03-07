using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Historialbusquedageneral
    {
        public int IdHistorialBusquedaGeneral { get; set; }
        public DateTime? Fecha { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
    }
}
