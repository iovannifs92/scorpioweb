using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Enviocorrespondencia
    {
        public int IdenvioCorrespondencia { get; set; }
        public string Area { get; set; }
        public string Nombre { get; set; }
        public string Apaterno { get; set; }
        public string Amaterno { get; set; }
        public string NoOficio { get; set; }
        public string FiguraJudicial { get; set; }
        public string Genero { get; set; }
        public string Asunto { get; set; }
        public string Autoridad { get; set; }
        public string Observaciones { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public sbyte? Entregado { get; set; }
        public sbyte? Recibido { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public DateTime? FechaRecibido { get; set; }
        public string QuienEntrega { get; set; }
        public string QuienRecibe { get; set; }
    }
}
