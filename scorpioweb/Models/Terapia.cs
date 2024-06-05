using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Terapia
    {
        public int IdTerapia { get; set; }
        public string Tipo { get; set; }
        public string Terapeuta { get; set; }
        public DateTime? FechaCanalizacion { get; set; }
        public string TiempoTerapia { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaTermino { get; set; }
        public DateTime? FechaTerapia { get; set; }
        public string PeriodicidadTerapia { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public int CanalizacionIdCanalizacion { get; set; }
        public int GrupoIdGrupo { get; set; }
    }
}
