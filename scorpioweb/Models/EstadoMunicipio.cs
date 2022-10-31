using Microsoft.AspNetCore.Mvc;

namespace scorpioweb.Models
{
    public class EstadoMunicipio
    {
        public Estados estadosVM { get; set; }
        public Municipios municipiosVM { get; set; }
        public Contactos contactosVM { get; set; }
    }
}
