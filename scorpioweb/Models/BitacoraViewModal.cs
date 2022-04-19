using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace scorpioweb.Models
{
    public class BitacoraViewModal 
    {
        public Bitacora bitacoraVM { get; set; }
        public Fraccionesimpuestas fraccionesimpuestasVM { get; set; }
        public Supervision supervisionVM { get; set; }

    }
}
