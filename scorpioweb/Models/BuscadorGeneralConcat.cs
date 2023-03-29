using System.ComponentModel.DataAnnotations;

namespace scorpioweb.Models
{
    public class BuscadorGeneralConcat
    {
        [Key]
        public int Id { get; set; }
        public string nombreCompleto { get; set; }
        public string Tabla { get; set; }
    }
}
