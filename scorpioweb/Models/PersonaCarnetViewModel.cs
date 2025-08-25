using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace scorpioweb.Models
{
    public class PersonaCarnetViewModel
    {
        public IFormFile Foto { get; set; }
        public int IdPersona { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string RutaFoto { get; set; }
        public string Institucion { get; set; }
        public string AreaPersona { get; set; }
        public string Tutor {  get; set; }
        public bool EsAdolescente { get; set; }
        public List<FamiliarViewModel> Familiares { get; set; }
        public List<FamiliarForaneoViewModel> FamiliaresForaneos { get; set; }
    }

    public class FamiliarViewModel
    {
        public string NombreFamiliar { get; set; }
        public string TelefonoFamiliar { get; set; }
        public string RelacionFamliar { get; set; }
    }

    public class FamiliarForaneoViewModel
    {
        public string NombreFamiliarForaneo { get; set; }
        public string TelefonoFamliarForaneo { get; set; }
        public string RelacionFamiliarForaneo { get; set; }
    }
}
