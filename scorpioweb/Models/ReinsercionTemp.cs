using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class ReinsercionTemp
    {
        public int IdTemp { get; set; }
        public int IdReinsercion { get; set; }
        public string Delito { get; set; }
        public string CausaPenal { get; set; }
        public string TipoServ { get; set; }
        public string TipoTerapia { get; set; }
        public string TipoVinculacion { get; set; }
        public string PeriodoTerapia { get; set; }
        public string Curp { get; set; }
        public int? NumJornadas { get; set; }
        public DateTime? FechaConclusion { get; set; }
        public string NivelEstudios { get; set; }
        public string LugarNacimiento { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}
