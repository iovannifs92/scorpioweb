using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Directoriojueces
    {
        public int IdDirectorioJueces { get; set; }
        public string Instancia { get; set; }
        public string Nombre { get; set; }
        public string Cargo { get; set; }
        public string Area { get; set; }
        public string TelefonoDirecto { get; set; }
        public string Conmutador { get; set; }
        public int Extension { get; set; }
        public string DistritoJudicial { get; set; }
        public string Edificio { get; set; }
        public string Localizacion { get; set; }
    }
}
