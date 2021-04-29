using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Proceso
    {
        public int IdProceso { get; set; }
        public string CausaPenal { get; set; }
        public string Coimputados { get; set; }
        public string CoimputadosSupervision { get; set; }
        public int? NoCoimputadosSupervision { get; set; }
        public string RelacionSupervisado { get; set; }
        public string RelacionLugar { get; set; }
        public string EspecificaRelacion { get; set; }
        public string Funcionario { get; set; }
        public string EspecificaFuncionario { get; set; }
        public string Observaciones { get; set; }
        public int PersonaIdPersona { get; set; }
        







    }
}
