using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Asistencia
    {
        public int IdAsistencia { get; set; }
        public DateTime? FechaAsistencia { get; set; }
        public string Observaciones { get; set; }
        public sbyte? Asistio { get; set; }
        public int TerapiaIdTerapia { get; set; }
    }
}
