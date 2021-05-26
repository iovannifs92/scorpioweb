using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Catalogodelitos
    {
        public int IdCatalogoDelitos { get; set; }
        public string Codigo { get; set; }
        public string Delito { get; set; }
        public int? BienJuridicoIdBienJuridico { get; set; }
    }
}
