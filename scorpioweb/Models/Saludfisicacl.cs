using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Saludfisicacl
    {
        public int IdSaludFisicaCl { get; set; }
        public string Enfermedad { get; set; }
        public string EspecifiqueEnfermedad { get; set; }
        public string EmbarazoLactancia { get; set; }
        public string Tiempo { get; set; }
        public string Tratamiento { get; set; }
        public string Discapacidad { get; set; }
        public string EspecifiqueDiscapacidad { get; set; }
        public string ServicioMedico { get; set; }
        public string EspecifiqueServicioMedico { get; set; }
        public string InstitucionServicioMedico { get; set; }
        public string Observaciones { get; set; }
        public int PersonaClIdPersonaCl { get; set; }
    }
}
