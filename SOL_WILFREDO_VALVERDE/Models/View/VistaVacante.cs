using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOL_WILFREDO_VALVERDE.Models.View
{
    public class VistaVacante
    {
        public int id_vacante { get; set; }
        public int id_curso { get; set; }
        public string nombrecurso { get; set; }
        public int id_seccion { get; set; }
        public string nombreseccion { get; set; }

        public int creditos { get; set; }
        public int cantidad_vacante { get; set; }
    }
}