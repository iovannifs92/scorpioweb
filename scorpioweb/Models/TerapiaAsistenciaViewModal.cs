using System;
using System.Collections.Generic;

namespace scorpioweb.Models
{
    public class TerapiaAsistenciaViewModal
    {
        //DATOS DE TERAPIA
        public int IdTerapia { get; set; }
        public string Tipo { get; set; }
        public string Terapeuta { get; set; }
        public DateTime? FechaCanalizacion { get; set; }
        public string TiempoTerapia { get; set; }
        public DateTime? FechaInicioTerapia { get; set; }
        public DateTime? FechaTerminoTerapia { get; set; }

        public DateTime? FechaTerapia { get; set; }
        public string PeriodiciadTerapia { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public int CanalizacionIdCanalizacion { get; set; }

        //DATOS DE GRUPO
        public int IdGrupo { get; set; }
        public string NombreGrupo { get; set; }
        public string Dia {  get; set; }
        public string HorarioGrupo { get; set; }

        //DATOS DE ASISTENCIA


        public string IdAsistencia { get; set; }
        public string FechaAsistencia { get; set; }
        public string ObservacionesAsistencia { get; set; }
        public string Asistio { get; set; }
        public int TerapiaIdTerapia { get; set; }

        //DATOS PERSONA 
        public int idpersona { get; set; }
        public string nombre { get; set; }
        public string cp { get; set; }
        public string tabla { get; set; }  
        public string rutaFoto { get; set; }  




    }


}
