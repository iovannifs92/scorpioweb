using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace scorpioweb.Models
{
    public class ListaOficialiaBitacoraViewModel
    {
        public Oficialia oficialiavm { get; set; }
        public Supervision supervisionvm { get; set; }
        public Causapenal causapenalvm { get; set; }
        public Persona personavm { get; set; }
        public Bitacora bitacoravm { get; set; }
    }
}
