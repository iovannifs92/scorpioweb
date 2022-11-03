using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Mensajesistema
    {
        public int IdMensajeSistema { get; set; }
        public string Mensaje { get; set; }
        public string Usuario { get; set; }
        public string Activo { get; set; }
        public string Colectivo { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string Link { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorNuevo { get; set; }
        public string Tabla { get; set; }
        public int? IdentificadorTabla { get; set; }
        public string Nombre { get; set; }
        public int? PersonaIdPersona { get; set; }
    }
}
