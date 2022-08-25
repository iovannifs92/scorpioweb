using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Municipios
    {
        public int? Id { get; set; }
        public string Municipio { get; set; }
        public int EstadosId { get; set; }
    }
}
