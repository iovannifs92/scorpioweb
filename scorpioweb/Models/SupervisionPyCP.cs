
namespace scorpioweb.Models
{
    public class SupervisionPyCP
    {
        public Supervision supervisionVM{ get; set; }
        public Persona personaVM { get; set; }
        public Causapenal causapenalVM { get; set; }
        public Fraccionesimpuestas fraccionesimpuestasVM { get; set; }
        public Planeacionestrategica planeacionestrategicaVM { get; set; }
        public int tiempoSupervision { get; set; }
        public Cierredecaso cierredecasoVM { get; set; }
    }
}
