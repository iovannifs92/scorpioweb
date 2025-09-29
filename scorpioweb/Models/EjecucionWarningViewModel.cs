namespace scorpioweb.Models
{
    public class EjecucionWarningViewModel
    {
        public Ejecucion ejecucionVM { get; set; }
        public Oficialia oficialiaVM { get; set; }
        public Audienciaep audienciaep { get; set; }
        
        public string tipoAdvertencia { get; set; }
        public string Area { get; set; }
    }
}
