using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SOL_WILFREDO_VALVERDE.Models;
using SOL_WILFREDO_VALVERDE.Models.View;


namespace SOL_WILFREDO_VALVERDE.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            List<VistaMatricula> lst;
            List<VistaCurso> lstC;
            using (Entities db = new Entities())
            {

                lstC = (from d in db.CURSO
                        select new VistaCurso
                        {
                            id_curso = (Int32)d.ID_CURSO,
                            nombre = d.NOMBRE,
                            creditos = (Int32)d.CREDITOS,
                            estado = (Int16)d.ESTADO
                        }).ToList();


                lst = (from d in db.MATRICULA
                       join a in db.ALUMNO on d.ID_ALUMNO equals a.ID_ALUMNO
                       join v in db.VACANTE on d.ID_VACANTE equals v.ID_VACANTE
                       join c in db.CURSO on v.ID_CURSO equals c.ID_CURSO into vacantecurso
                       from vc in vacantecurso.DefaultIfEmpty()
                       join s in db.SECCION on v.ID_SECCION equals s.ID_SECCION
                       join m in db.MODALIDAD on d.ID_MODALIDAD equals m.ID_MODALIDAD
                       select new VistaMatricula
                       {
                           id_matricula = (Int32)d.ID_MATRICULA,
                           id_alumno = (Int32)d.ID_ALUMNO,
                           nombres = a.NOMBRES,
                           apellidos = a.APELLIDOS,
                           id_modalidad = (Int32)d.ID_MODALIDAD,
                           id_curso = (Int32)v.ID_CURSO,
                           nombrecurso = vc.NOMBRE,
                           seccion = s.NOMBRE,
                           modalidad = m.NOMBRE,
                           creditos = (Int32)vc.CREDITOS,
                           fecha_matricula = d.FECHA_MATRICULA,
                           fecha_anulacion = d.FECHA_ANULACION,

                       }).OrderBy(x => x.id_matricula).ToList();


            }
            return View(lst);
        }

        [HttpPost]
        public JsonResult ListarVacantes()
        {
            List<VistaVacante> lstC;
            using (Entities db = new Entities())
            {
                lstC = (from d in db.VACANTE 
                        join c in db.CURSO on d.ID_CURSO equals c.ID_CURSO into vacantecurso
                        from vc in vacantecurso.DefaultIfEmpty()
                        join s in db.SECCION on d.ID_SECCION equals s.ID_SECCION
                        select new VistaVacante
                        {
                            id_vacante = (Int32)d.ID_VACANTE,
                            id_curso = (vc.ID_CURSO != null) ? (Int32)vc.ID_CURSO : 0,
                            nombrecurso = vc.NOMBRE,
                            creditos = (vc.CREDITOS != null)? (Int32)vc.CREDITOS:0,
                            cantidad_vacante = (Int32)d.CANTIDAD_VACANTE,
                            nombreseccion = s.NOMBRE
                        }).ToList();
            }
                            
            return Json(new { data = lstC }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BuscarNombre(string dni)
        {
            var pdni = dni;
            List<VistaAlumno> lstC;
            using (Entities db = new Entities())
            {
                lstC = (from d in db.ALUMNO where d.DNI.Equals(pdni)
                        select new VistaAlumno
                        {
                            id_alumno = (Int32)d.ID_ALUMNO,
                            nombres = d.NOMBRES,
                            apellidos = d.APELLIDOS
                        }).ToList();
            }

            VistaAlumno alumno = new VistaAlumno();
            foreach (var v in lstC)
            {
                alumno.id_alumno = v.id_alumno;
                alumno.nombres = v.nombres + ' ' + v.apellidos;
            }

            return Json(new { Nombre = alumno.nombres, IdAlumno = alumno.id_alumno }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Matricula()
        {
            List<VistaModalidad> lstC;
            using (Entities db = new Entities())
            {
                
                lstC = (from d in db.MODALIDAD
                        select new VistaModalidad
                    {
                        id_modalidad = (Int32)d.ID_MODALIDAD,
                        nombre = d.NOMBRE,                        
                        estado = (Int16)d.ESTADO
                    }).ToList();
            }


            var lista = lstC;
            VistaModalidad objUsuario = new VistaModalidad()
            {
                id_modalidad = 0,
                nombre = "Seleccionar"
            };
            lista.Insert(0, objUsuario);
            ViewBag.ListaModalidad = lista;

            return View();
        }

        [HttpPost]
        public JsonResult GuardarMatricula(string objeto, int id_alumno, int id_modalidad)
        {
            
            string mensaje = string.Empty;
            bool operacion_exitosa = true;

            VistaMatricula objDatos = new VistaMatricula();
            
            List<VistaMatricula> ObjOrderList = JsonConvert.DeserializeObject<List<VistaMatricula>>(objeto);
            var NIDMatricula = 0;

            if(validarLista(ObjOrderList,id_alumno))
            {

                    using (Entities db = new Entities())
                    {
                        int i = 0;
                        foreach (VistaMatricula lst in ObjOrderList)
                        {

                            if (validarObjeto(lst, id_alumno))
                            {
                                i++;
                                NIDMatricula = Convert.ToInt32(db.MATRICULA.Any() ? db.MATRICULA.Select(p => p.ID_MATRICULA).Max() + i : 1);
                                List<MATRICULA> listam = new List<MATRICULA>();
                                MATRICULA objMatricula = new MATRICULA();
                                objMatricula.ID_MATRICULA = NIDMatricula;
                                objMatricula.ID_ALUMNO = id_alumno;
                                objMatricula.ID_MODALIDAD = id_modalidad;
                                objMatricula.ID_VACANTE = lst.id_vacante;
                                objMatricula.FECHA_MATRICULA = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                                objMatricula.FECHA_ANULACION = null;
                                db.MATRICULA.Add(objMatricula);

                                VACANTE obj = new VACANTE();

                                VACANTE c = (from x in db.VACANTE
                                                where x.ID_VACANTE == lst.id_vacante
                                                select x).First();
                                c.CANTIDAD_VACANTE = c.CANTIDAD_VACANTE - 1;
                            }
                            else
                            {
                                operacion_exitosa = false;
                                mensaje = "No se realizó la matricula del alumno";
                            }
                        
                        }
                        db.SaveChanges();
                    }
            }else
            {
                operacion_exitosa = false;
                mensaje = "La cantidad de cursos superan el limite de créditos";
            }

            return Json(new { operacionExitosa = operacion_exitosa, idGenerado = NIDMatricula, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }


        private bool validarLista(List<VistaMatricula> listaMatricula, int id_alumno)
        {
            bool valor = true;
            int? cantidadCreditos = 0;
            int? sumaCreditos = 0;
            int? creditoslista = 0;
            using (Entities db = new Entities())
            {

                cantidadCreditos = ((from m in db.MATRICULA
                                     join v in db.VACANTE on m.ID_VACANTE equals v.ID_VACANTE
                                     join c in db.CURSO on v.ID_CURSO equals c.ID_CURSO
                                     where m.ID_ALUMNO == id_alumno && m.FECHA_ANULACION == null
                                     select c).Sum(d => d.CREDITOS)) == null
                                ? 0 :
                                (Int32)(from m in db.MATRICULA
                                        join v in db.VACANTE on m.ID_VACANTE equals v.ID_VACANTE
                                        join c in db.CURSO on v.ID_CURSO equals c.ID_CURSO
                                        where m.ID_ALUMNO == id_alumno
                                        select c).Sum(d => d.CREDITOS);
            }
            
            foreach (VistaMatricula lst in listaMatricula)
            {                                
                using (Entities db = new Entities())
                {

                    creditoslista = ((from v in db.VACANTE 
                                 join c in db.CURSO on v.ID_CURSO equals c.ID_CURSO
                                 where v.ID_VACANTE == lst.id_vacante
                                 && c.ID_CURSO == lst.id_curso
                                 select c).Sum((d => d.CREDITOS))) == null
                                ? 0 :
                                (Int32)((from v in db.VACANTE
                                         join c in db.CURSO on v.ID_CURSO equals c.ID_CURSO
                                         where v.ID_VACANTE == lst.id_vacante
                                         && c.ID_CURSO == lst.id_curso
                                         select c).Sum((d => d.CREDITOS)));
                }

                sumaCreditos = sumaCreditos + creditoslista;
             }

            if (cantidadCreditos + sumaCreditos > 5)
                return false;
            
            return valor;
        }

        private bool validarObjeto(VistaMatricula lst, int id_alumno)
        {
            bool valor = true;

            if (lst.id_vacante <= 0)
                return false;

            List<VistaAlumno> lstC;
            int? cantidadCursos = 0;
            int? cantidadCreditos = 0;
            int? creditos = 0;
            using (Entities db = new Entities())
            {

                cantidadCursos = (from m in db.MATRICULA
                                  join v in db.VACANTE on m.ID_VACANTE equals v.ID_VACANTE
                                  join c in db.CURSO on v.ID_CURSO equals c.ID_CURSO
                                  where m.ID_ALUMNO == id_alumno                                  
                                  && m.FECHA_ANULACION == null
                                  && c.ID_CURSO == lst.id_curso
                                  select c).Count();               
            }

            if ((cantidadCreditos + creditos) > 5)
                return false;

            if (cantidadCursos > 0)
                return false;


            return valor;

        }
    }
}