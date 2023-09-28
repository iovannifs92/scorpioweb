using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Trabajocl
    {
        public int IdTrabajoCl { get; set; }
        public string Trabaja { get; set; }
        public string TipoOcupacion { get; set; }
        public string Puesto { get; set; }
        public string EmpledorJefe { get; set; }
        public string EnteradoProceso { get; set; }
        public string SePuedeEnterar { get; set; }
        public string TiempoTrabajano { get; set; }
        public string Salario { get; set; }
        public string TemporalidadSalario { get; set; }
        public string Direccion { get; set; }
        public string Horario { get; set; }
        public string Telefono { get; set; }
        public string Observaciones { get; set; }
        public int PersonaClIdPersonaCl { get; set; }
    }
}
