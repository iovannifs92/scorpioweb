using System.ComponentModel.DataAnnotations;

namespace scorpioweb.Models
{
    public class ArchivoControlPrestamo
    {
        public Archivo archivoVM { get; set; }
        public Archivoprestamo archivoprestamoVM { get; set; }
        public Archivoregistro archivoregistroVM { get; set; } 
        public Areas areasVM { get; set; }
        public Archivoprestamodigital archivoprestamodigitalVM { get; set; }

 }
}
