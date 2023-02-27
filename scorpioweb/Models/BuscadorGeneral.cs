using System.ComponentModel.DataAnnotations;

namespace scorpioweb.Models
{
    public class BuscadorGeneral
    {

        [Key]
        public int Id { get; set; }
        public string paterno { get; set; }
        public string materno { get; set; }
        public string nombre { get; set; }
        public string Tabla { get; set; }
    }
}
