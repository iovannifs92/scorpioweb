using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Libronegro
    {
        public int Idlibronegro { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombre { get; set; }
        public string Cp { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string F1 { get; set; }
        public string F2 { get; set; }
        public string F3 { get; set; }
        public string F4 { get; set; }
        public string Area { get; set; }
        public string Supervisor { get; set; }
        public DateTime? FechaCaptura { get; set; }
        public sbyte? Proceso { get; set; }
    }
}
