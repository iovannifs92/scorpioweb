using System.ComponentModel.DataAnnotations;

namespace scorpioweb.Models
{
    public class BuscadorGeneral
    {

        [Key]
        public int Id { get; set; }
        public string NombreCompletoPersona { get; set; }
        public string Tabla { get; set; }
    }
}
