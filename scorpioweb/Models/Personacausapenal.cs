using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Personacausapenal
    {
        public int IdPersonaCausapenal { get; set; }
        public int CausaPenalIdCausaPenal { get; set; }
        public int PersonaIdPersona { get; set; }
    }
}
