using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Persona
    {
        public int IdPersona { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Alias { get; set; }
        public string Genero { get; set; }
        public int? Edad { get; set; }
        public DateTime? Fnacimiento { get; set; }
        public string Lnpais { get; set; }
        public string Lnestado { get; set; }
        public string Lnmunicipio { get; set; }
        public string Lnlocalidad { get; set; }
        public string EstadoCivil { get; set; }
        public string Duracion { get; set; }
        public string OtroIdioma { get; set; }
        public string EspecifiqueIdioma { get; set; }
        public string DatosGeneralescol { get; set; }
        public string LeerEscribir { get; set; }
        public string Traductor { get; set; }
        public string EspecifiqueTraductor { get; set; }
        public string TelefonoFijo { get; set; }
        public string Celular { get; set; }
        public string Hijos { get; set; }
        public int? Nhijos { get; set; }
        public int? NpersonasVive { get; set; }
        public string Propiedades { get; set; }
        public string Curp { get; set; }
        public string ConsumoSustancias { get; set; }
        public DateTime? UltimaActualización { get; set; }
        public string Supervisor { get; set; }
        public string rutaFoto { get; set; }
        public string Familiares { get; set; }
        public string ReferenciasPersonales { get; set; }
        public string Capturista { get; set; }
        public sbyte? Candado { get; set; }
        public string MotivoCandado { get; set; }
        public string UbicacionExpediente { get; set; }
        public string Colaboracion { get; set; }
        public string ComLgbtttiq { get; set; }
        public string ComIndigena { get; set; }
        public string TieneResolucion { get; set; }
        public string NombreCompleto
        {
            get
            {
                return this.Paterno + " " + this.Materno + " " + this.Nombre;
            }
        }
    }
}
