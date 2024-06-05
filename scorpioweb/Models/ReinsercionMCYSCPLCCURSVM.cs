namespace scorpioweb.Models
{
    public class ReinsercionMCYSCPLCCURSVM
    {
        public int IdReinsercion { get; set; }
        public int IdTabla { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombre { get; set; }
        public string ClaveUnica { get; set; }
        public string NomTabla { get; set; }
        public string Causapenal { get; set; }
        public string Delito { get; set; }
        public string EstadoVinculacion { get; set; }
        public string EstadoSupervision { get; set; }
        public string NombreCompleto
        {
            get
            {
                return this.Paterno + " " + this.Materno + " " + this.Nombre;
            }
        }
    }
}
