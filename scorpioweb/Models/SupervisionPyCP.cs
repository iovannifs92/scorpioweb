using System.ComponentModel.DataAnnotations;

namespace scorpioweb.Models
{
    public class SupervisionPyCP
    {
        [Key]
        public int IdSupervisionPyCP { get; set; }
        public Supervision supervisionVM{ get; set; }
        public Persona personaVM { get; set; }
        public Causapenal causapenalVM { get; set; }

    }
}
