using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Reasignacionsupervisor
    {
        public int Id { get; set; }
        public string PersonaIdpersona { get; set; }
        public DateTime? FechaReasignacion { get; set; }
        public string SAntuguo { get; set; }
        public string SNuevo { get; set; }
        public string QuienRealizo { get; set; }
        public string MotivoRealizo { get; set; }
    }
}
