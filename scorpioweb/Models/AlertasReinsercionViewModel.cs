namespace scorpioweb.Models
{
    public class AlertasReinsercionViewModel
    {
       
        public int IdTabla { get; set; }
        public string Nombre { get; set; }
        public string Area { get; set; }
        public string TipoAlerta { get; set; }
        public Cierredecaso CierreCasoMC { get; set; }    
        public Cierredecasocl CierreCasoCL { get; set; }

    }
}
