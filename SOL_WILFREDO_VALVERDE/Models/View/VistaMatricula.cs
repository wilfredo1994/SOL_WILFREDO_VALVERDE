using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOL_WILFREDO_VALVERDE.Models.View
{
    public class VistaMatricula
    {
        public int id_matricula { get; set; }
        public int id_alumno { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public int id_modalidad { get; set; }
        public int id_vacante { get; set; }

        public int id_curso { get; set; }
        public string nombrecurso { get; set; }
        public string seccion { get; set; }
        public string matricula { get; set; }
        public int creditos { get; set; }
        public string modalidad { get; set; }
        public string fecha_matricula { get; set; }
        public string fecha_anulacion { get; set; }
    }
}