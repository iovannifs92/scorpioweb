using System.Collections.Generic;
using System;

namespace scorpioweb.Models
{
    public class TerapiaModel
    {
        public List<string> TiposTerapiaSeleccionados { get; set; }
        public string EspecificarOtraTerapia { get; set; }
        public string Terapeuta { get; set; }
        public DateTime FechaCanalizacion { get; set; }
        public string TiempoTerapia { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaTermino { get; set; }
        public DateTime FechaTerapia { get; set; }
        public string PeriodicidadTerapia { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public int CanalizacionId { get; set; }
        public int GrupoId { get; set; }
     

        
    }
}
