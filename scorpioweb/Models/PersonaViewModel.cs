using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace scorpioweb.Models
{
    public class PersonaViewModel
    {
        public Persona personaVM { get; set; }
        public Domicilio domicilioVM { get; set; }
        public Estudios estudiosVM { get; set; }
        public Estados estadosVMPersona { get; set; }
        public Municipios municipiosVMPersona { get; set; }
        public Estados estadosVMDomicilio { get; set; }
        public Municipios municipiosVMDomicilio { get; set; }
        public Domiciliosecundario domicilioSecundarioVM { get; set; }
        public Estados estadosVMDomicilioSec { get; set; }
        public Municipios municipiosVMDomicilioSec { get; set; }
        public Consumosustancias consumoSustanciasVM { get; set; }
        public Trabajo trabajoVM { get; set; }
        public Actividadsocial actividadSocialVM { get; set; }
        public Abandonoestado abandonoEstadoVM { get; set; }
        public Saludfisica saludFisicaVM { get; set; }
        public Familiaresforaneos familiaresForaneosVM { get; set; }
        public Asientofamiliar asientoFamiliarVM { get; set; }
        public Delito delitoDM { get; set; }
        public string CasoEspecial { get; set; }
    }
}
