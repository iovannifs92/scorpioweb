using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Canalizacion
    {
        public int IdCanalizacion { get; set; }
        public string Tipo { get; set; }
        public int ReincercionIdReincercion { get; set; }
    }
}
