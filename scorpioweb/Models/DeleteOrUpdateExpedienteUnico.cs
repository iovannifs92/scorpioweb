using System.ComponentModel.DataAnnotations;

namespace scorpioweb.Models
{
    public class DeleteOrUpdateExpedienteUnico
    {
        [Key]
        public int idExpedienteUnico { get; set; }
        public string Tabla { get; set; }
        public string ClaveUnicaScorpio { get; set; }
        public string idTabla { get; set; }
    }
}
