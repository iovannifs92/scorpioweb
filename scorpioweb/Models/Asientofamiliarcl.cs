using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Asientofamiliarcl
    {
        public int IdAsientoFamiliarCl { get; set; }
        public string Nombre { get; set; }
        public string Relacion { get; set; }
        public int? Edad { get; set; }
        public string Sexo { get; set; }
        public string Dependencia { get; set; }
        public string DependenciaExplica { get; set; }
        public string VivenJuntos { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string HorarioLocalizacion { get; set; }
        public string EnteradoProceso { get; set; }
        public string PuedeEnterarse { get; set; }
        public string Tipo { get; set; }
        public string TiempoHabitando { get; set; }
        public string Observaciones { get; set; }
        public int PersonaClIdPersonaCl { get; set; }
    }
}
