using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Archivo
    {
        public int IdArchivo { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombre { get; set; }
        public string Urldocumento { get; set; }
        public int? ExpedienteUnicoIdExpedienteUnico { get; set; }
        public string Yo { get; set; }
    }
}
