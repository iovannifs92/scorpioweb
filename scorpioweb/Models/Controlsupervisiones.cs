using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Controlsupervisiones
    {
        public int IdControlSupervisiones { get; set; }
        public string Supervisor { get; set; }
        public int? Supervisados { get; set; }
    }
}
