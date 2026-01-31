using MiGente_Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace MiGente_Web.Servicios
{
    public class CalificacionesService
    {
        public List<VCalificaciones> getTodas()
        {
            using (migenteEntities db = new migenteEntities())
            {
                var result = db.VCalificaciones.ToList();
                return result;
            };
        }
        public List<VCalificaciones> getById(string id, string userID=null)
        {
            using (migenteEntities db = new migenteEntities())
            {
             
                if (userID!=null)
                {
                var result = db.VCalificaciones.Where(x => x.identificacion == id && x.userID ==userID)
                        .OrderByDescending(x=>x.calificacionID).ToList();
                    return result;

                }
                else
                {
                    var result = db.VCalificaciones.Where(x => x.identificacion == id).ToList();
                    return result;

                }

            };
        }
        public Calificaciones getCalificacionByID(int calificacionID)
        {
            using (migenteEntities db = new migenteEntities())
            {

                    var result = db.Calificaciones.Where(x => x.calificacionID == calificacionID)
                            .OrderByDescending(x => x.calificacionID).FirstOrDefault();
                    return result;
               

            };
        }
        public Calificaciones calificarPerfil(Calificaciones cal)
        {
            using (var db=new migenteEntities())
            {
                db.Calificaciones.Add(cal);
                db.SaveChanges();
                return cal;
            }
        }
    }
}