using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Serviciospreviosjuicio
    {
        public int IdserviciosPreviosJuicio { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Sexo { get; set; }
        public string Edad { get; set; }
        public string Calle { get; set; }
        public string Colonia { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
        public string Papa { get; set; }
        public string Mama { get; set; }
        public string Ubicacion { get; set; }
        public string Delito { get; set; }
        public string UnidadInvestigacion { get; set; }
        public DateTime? FechaDetencion { get; set; }
        public string Situacion { get; set; }
        public string RealizoEntrevista { get; set; }
        public string TipoDetenido { get; set; }
        public string Aer { get; set; }
        public string Tamizaje { get; set; }
        public string Rcomparesencia { get; set; }
        public string Rvictima { get; set; }
        public string Robstaculizacion { get; set; }
        public string Recomendacion { get; set; }
        public string Antecedentes { get; set; }
        public string AntecedentesDatos { get; set; }
        public string Observaciones { get; set; }
        public int? PersonaIdPersona { get; set; }
        public string RutaAer { get; set; }
        public string Usuario { get; set; }
        public DateTime? FechaCaptura { get; set; }
        public string LnPais { get; set; }
        public string LnEstado { get; set; }
        public string LnMunicipio { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string ClaveUnicaScorpio { get; set; }
        public string NombreCompleto
        {
            get
            {
                return this.Paterno + " " + this.Materno + " " + this.Nombre;
            }
        }
    }
}
