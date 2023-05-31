namespace scorpioweb.Models
{
    public class EjecucionWarningViewModel
    {
        public Ejecucion ejecucionVM { get; set; }
        public Oficialia oficialiaVM { get; set; }
        public string NombreCompleto
        {
            get
            {
                return oficialiaVM.Paterno + " " + oficialiaVM.Materno + " " + oficialiaVM.Nombre;
            }
        }
        public string tipoAdvertencia { get; set; }
    }
}
