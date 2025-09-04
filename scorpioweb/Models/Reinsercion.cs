using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Reinsercion
    {
        public int IdReinsercion { get; set; }
        public string IdTabla { get; set; }
        public string Tabla { get; set; }
        public string Lugar { get; set; }
        public string Estado { get; set; }

        //// Campos Extras 
        //public string Delito { get; set; }
        //public string CausaPenal { get; set; }
        //public string TipoServ { get; set; }
        //public string TipoTerapia { get; set; }
        //public string TipoVinculacion { get; set; }
        //public string PeriodoTerapia { get; set; }
        //public string CURP { get; set; }
        //public string NumJornadas { get; set; }
        //public string FechaConclusion { get; set; }
        //public string NivelEstudios { get; set; }
        //public string LugarNcimiento { get; set; }
        //public string FechaNacimiento { get; set; }


    }
}
