namespace scorpioweb.Models
{
    public class PersonaclsViewModal
    {
        public Personacl personaclVM { get; set; }
        public Domiciliocl domicilioclVM { get; set; }
        public Estudioscl estudiosclVM { get; set; }
        public Estados estadosVMPersona { get; set; }
        public Municipios municipiosVMPersona { get; set; }
        public Estados estadosVMDomicilio { get; set; }
        public Municipios municipiosVMDomicilio { get; set; }
        public Domiciliosecundariocl domicilioSecundarioclVM { get; set; }
        public Estados estadosVMDomicilioSec { get; set; }
        public Municipios municipiosVMDomicilioSec { get; set; }
        public Consumosustanciascl consumoSustanciasclVM { get; set; }
        public Trabajocl trabajoclVM { get; set; }
        public Actividadsocialcl actividadSocialclVM { get; set; }
        public Abandonoestadocl abandonoEstadoclVM { get; set; }
        public Saludfisicacl saludFisicaclVM { get; set; }
        public Familiaresforaneoscl familiaresForaneosclVM { get; set; }
        public Asientofamiliarcl asientoFamiliarclVM { get; set; }
        //public Delito delitoDM { get; set; }
        public string CasoEspecial { get; set; }
    }
}
