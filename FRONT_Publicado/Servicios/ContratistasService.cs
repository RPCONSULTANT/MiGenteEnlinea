using MiGente_Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using DevExpress.Utils.About;
using DevExpress.XtraTreeMap;
using System.Runtime.Remoting.Contexts;

namespace MiGente_Web.Servicios
{
    public class ContratistasService
    {
        public List<VContratistas> getTodasUltimos20()
        {
            using (migenteEntities db = new migenteEntities())
            {
                var result = db.VContratistas
      .OrderByDescending(a => a.contratistaID).Where(x => x.activo == true)
      .Take(20)
      .ToList();

                return result;
            };
        }
        public VContratistas getMiPerfil(string userID)
        {
            using (migenteEntities db = new migenteEntities())
            {
                var result = db.VContratistas.Where(x=>x.userID==userID).FirstOrDefault();

                return result;
            };
        }
        public List<Contratistas_Servicios> getServicios(int contratistaID)
        {
            using (migenteEntities db = new migenteEntities())
            {
                var result = db.Contratistas_Servicios.Where(x => x.contratistaID ==contratistaID).ToList();

                return result;
            };
        }
        public bool agregarServicio(Contratistas_Servicios servicio)
        {
            using (migenteEntities db = new migenteEntities())
            {
                db.Contratistas_Servicios.Add(servicio);
                db.SaveChanges();
                return true;
            }
        }
        public bool GuardarPerfil(Contratistas ct, string userID)
        {
            using (var db = new migenteEntities())
            {
                var ct_old = db.Contratistas.FirstOrDefault(e => e.userID == userID); // Aquí debes especificar cómo obtener el objeto que deseas modificar
                if (ct_old != null)
                {
                    ct_old.titulo = ct.titulo;
                    ct_old.sector = ct.sector;

                    ct_old.presentacion = ct.presentacion;
                    ct_old.email = ct.email;
                    ct_old.tipo = ct.tipo;
                    ct_old.identificacion = ct.identificacion;

                    ct_old.Nombre = ct.Nombre;
                    ct_old.Apellido = ct.Apellido;


                    ct_old.telefono1 = ct.telefono1;
                    ct_old.telefono2 = ct.telefono2;

                    ct_old.whatsapp1 = ct.whatsapp1;
                    ct_old.whatsapp2 = ct.whatsapp2;
                    ct_old.imagenURL = ct.imagenURL;
                    ct_old.provincia= ct.provincia;
                    db.SaveChanges();
                }

                return true;

            }
        }
        public bool ActivarPerfil(string userID)
        {
            using (var db = new migenteEntities())
            {
                var ct_old = db.Contratistas.FirstOrDefault(e => e.userID == userID); // Aquí debes especificar cómo obtener el objeto que deseas modificar
                if (ct_old != null)
                {
                    ct_old.activo = true;

                    db.SaveChanges();
                }
                return true;
            }
        }
        public bool DesactivarPerfil(string userID)
        {
            using (var db = new migenteEntities())
            {
                var ct_old = db.Contratistas.FirstOrDefault(e => e.userID == userID); // Aquí debes especificar cómo obtener el objeto que deseas modificar
                if (ct_old != null)
                {
                    ct_old.activo = false;

                    db.SaveChanges();
                }
                return true;
            }
        }
        public List<VContratistas> getConCriterio(string palabrasClave, string zona)
        {
            using (migenteEntities db = new migenteEntities())
            {
                if (zona == "Cualquier Ubicacion")
                {
                    var result = db.VContratistas
                    .OrderByDescending(a => a.contratistaID)
                    .Where(item => item.titulo.ToLower().Contains(palabrasClave.ToLower()) && item.activo == true)
                    .ToList();
                    return result;
                }
                else
                {
                    var result = db.VContratistas
                    .OrderByDescending(a => a.contratistaID)
                    .Where(item => item.titulo.ToLower().Contains(palabrasClave) && item.provincia.ToLower() == zona.ToLower() && item.activo == true)
                    .ToList();
                    return result;
                }

            }
        }


    }
}