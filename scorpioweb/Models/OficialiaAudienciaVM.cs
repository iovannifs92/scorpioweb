using System;

namespace scorpioweb.Models
{
    public class OficialiaAudienciaVM
    {
        public int Id { get; set; }
        public string Nomcom { get; set; }
        public DateTime? FechaAudiencia { get; set; }
        public string QuienAsistira { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public string Juzgado { get; set; }
        public string CarpetaEjecucion { get; set; }
        public string Area { get; set; }
        public string tipoAdvertencia { get; set; }
    }
}
