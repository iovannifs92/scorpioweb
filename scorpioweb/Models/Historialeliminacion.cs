using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Historialeliminacion
    {
        public int IdhistorialEliminacion { get; set; }
        public int? Id { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public string Razon { get; set; }
        public string Usuario { get; set; }
        public string Supervisor { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
