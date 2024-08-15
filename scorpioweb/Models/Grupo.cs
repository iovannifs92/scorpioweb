using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Grupo
    {
        public int IdGrupo { get; set; }
        public string NombreGrupo { get; set; }
        public string Dia { get; set; }
        public string Horario { get; set; }
        public string Estado { get; set; }
        public string Idterapeuta { get; set; }
    }
}
