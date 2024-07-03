using System;

namespace scorpioweb.Models
{
    public class AsistenciasModel
    {
        public string IdAsistencia { get; set; }
        public string FechaAsistencia { get; set; }
        public string ObservacionesAsistencia { get; set; }
        public string Asistio { get; set; }
        public int TerapiaIdTerapia { get; set; }
    }
}
