using System;
using System.ComponentModel.DataAnnotations;

namespace scorpioweb.Models
{
    public class HistorialPrestamos
    {
        [Key]
        public int idArchivo { get; set; }
        public string Ubicacion { get; set; }
        public string TipoPrestamo { get; set; }
        public string nombre { get; set; }
        public DateTime fechaprestamo { get; set; }
        public DateTime fechaentrega { get; set; }
        public string entrega { get; set; }
        public string usuario { get; set; }
    }
}
