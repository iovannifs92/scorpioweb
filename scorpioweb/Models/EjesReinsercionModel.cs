﻿using System.Collections.Generic;
using System;

namespace scorpioweb.Models
{
    public class EjesReinsercionModel
    {
        public List<string> EjesSeleccionados { get; set; }
        public string EspecificarOtroEje { get; set; }
        public string Observaciones { get; set; }
        public string Lugar { get; set; }
        public string Area { get; set; }
        public DateTime? FechaProgramada { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaCanalizacion { get; set; }
        public DateTime? FechaLimite { get; set; }
        public string HorasJornadas {  get; set; }
        public string NoHoraJornada { get; set; }  
        public int CanalizacionId { get; set; }
    }
}
