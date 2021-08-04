using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public partial class Oficialia
    {
        public int IdOficialia { get; set; }
        public string Capturista { get; set; }
        public string Recibe { get; set; }
        public string MetodoNotificacion { get; set; }
        public string NumOficio { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string Expide { get; set; }
        public string ReferenteImputado { get; set; }
        public string Sexo { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombre { get; set; }
        public string CarpetaEjecucion { get; set; }
        public int? IdCausaPenal { get; set; }
        public string CausaPenal { get; set; }
        public string ExisteVictima { get; set; }
        public string NombreVictima { get; set; }
        public string DireccionVictima { get; set; }
        public string AsuntoOficio { get; set; }
        public string TieneTermino { get; set; }
        public DateTime? FechaTermino { get; set; }
        public string UsuarioTurnar { get; set; }
        public sbyte? Entregado { get; set; }
        public string Observaciones { get; set; }
        public string PaternoMaternoNombre
        {
            get
            {
                string nombreCompleto = "";
                if (this.Paterno != "S-D")
                {
                    if (nombreCompleto != "")
                    {
                        nombreCompleto += " ";
                    }
                    nombreCompleto += this.Paterno;
                }
                if (this.Materno != "S-D")
                {
                    if (nombreCompleto != "")
                    {
                        nombreCompleto += " ";
                    }
                    nombreCompleto += this.Materno;
                }
                if (this.Nombre != "S-D")
                {
                    if (nombreCompleto != "")
                    {
                        nombreCompleto += " ";
                    }
                    nombreCompleto += this.Nombre;
                }
                return nombreCompleto;
            }
        }
        public string NombrePaternoMaterno
        {
            get
            {
                string nombreCompleto = "";
                if (this.Nombre != "S-D")
                {
                    if (nombreCompleto != "")
                    {
                        nombreCompleto += " ";
                    }
                    nombreCompleto += this.Nombre;
                }
                if (this.Paterno != "S-D")
                {
                    if (nombreCompleto != "")
                    {
                        nombreCompleto += " ";
                    }
                    nombreCompleto += this.Paterno;
                }
                if (this.Materno != "S-D")
                {
                    if (nombreCompleto != "")
                    {
                        nombreCompleto += " ";
                    }
                    nombreCompleto += this.Materno;
                }
                return nombreCompleto;
            }
        }
    }
}
