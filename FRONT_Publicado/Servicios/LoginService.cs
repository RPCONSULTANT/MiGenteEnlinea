using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ClassLibrary_CSharp.Encryption;
using MiGente_Web.Data;
using MiGente_Web;
using System.Web.Security;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
using Newtonsoft.Json;

namespace MiGente_Web.Servicios
{
    public class LoginService
    {
        public static HttpCookie myCookie = new HttpCookie("login");

        public int login(string email, string pass)
        {
            
            using (var db = new migenteEntities()) 
            {
                Crypt crypt = new Crypt();
                crypt.Encrypt(pass);
                var result = db.AccederClientes(email).FirstOrDefault();
                if (result!=null)
                {
                    if (string.IsNullOrEmpty(result.password))
                    {
                        return 1;
                    }
                    if (crypt.Decrypt(result.password) != pass)
                    {
                        return 0;
                    }
                    else
                    {

                        //Variables de Sesion
                        FormsAuthentication.SetAuthCookie(result.usuario, false);

                     

                        myCookie["usuario"] = result.usuario;
                        myCookie["email"] = result.Email;
                        myCookie["userID"] = result.userID;
                        myCookie["nombre"] = result.Nombre + " "+ result.Apellido;
                        myCookie["tipo"] = result.Tipo.ToString();

                        if (string.IsNullOrEmpty(result.planID.ToString()))
                        {
                            myCookie["planID"] = "0";

                        }
                        else
                        {

                            myCookie["planID"] = result.planID.ToString();
                            myCookie["vencimientoPlan"] = Convert.ToDateTime(result.vencimiento).ToString("d");
                            myCookie["nomina"] = result.nomina.ToString();
                            myCookie["empleados"] = result.empleados.ToString();
                            myCookie["historico"] = result.historico.ToString();
                       



                        }

                        var vPerfil = obtenerPerfil(result.userID);
                        string vPerfilSerializado = JsonConvert.SerializeObject(vPerfil);
                        myCookie["vPerfil"] = vPerfilSerializado;


                        var now = DateTime.Now;
                        myCookie.Expires = now.AddDays(1d);
                   


                        return 2;
                    }

                }
                else
                {
                    return 3;
                }

              
            };

           
        }

        public VPerfiles obtenerPerfil(string userID)
        {
            using (var db = new migenteEntities())
            {
                return db.VPerfiles.Where(a => a.userID == userID).FirstOrDefault();

            }
        }
        public VPerfiles obtenerPerfilByEmail(string email)
        {
            using (var db = new migenteEntities())
            {
                
                var result = db.VPerfiles.Where(a => a.Email == email).FirstOrDefault();
                return result;
            }
        }
        public List<Credenciales> obtenerCredenciales(string userID)
        {
            using (var db = new migenteEntities())
            {
                return db.Credenciales.Where(a => a.userID == userID).ToList();

            }
        }
        public bool actualizarPerfil(perfilesInfo info, Perfiles perfil)
        {
            using (var db = new migenteEntities())
            {
                db.Entry(info).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
              
            }
            using (var db1 = new migenteEntities())
            {
                db1.Entry(perfil).State = System.Data.Entity.EntityState.Modified;
                db1.SaveChanges();
            
            }
            return true;
        }
        public bool actualizarPerfil1(Perfiles perfil)
        {
            using (var db = new migenteEntities())
            {
                db.Entry(perfil).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            using (var db1 = new migenteEntities())
            {
                db1.Entry(perfil).State = System.Data.Entity.EntityState.Modified;
                db1.SaveChanges();

            }
            return true;
        }
        public bool agregarPerfilInfo(perfilesInfo info)
        {
            using (var db = new migenteEntities())
            {
                db.perfilesInfo.Add(info);
                db.SaveChanges();

                return true;
            }
          
        }
        public Perfiles getPerfilByID(int perfilID)
        {
            using (var db = new migenteEntities())
            {
                return db.Perfiles.Where(x => x.perfilID == perfilID).FirstOrDefault();

            }
        }
        public bool validarCorreo(string correo)
        {


            using (var db = new migenteEntities())
            {
                var result = db.Perfiles.Where(x => x.Email == correo).FirstOrDefault();

                if (result != null)
                {
                    return true;
                }
            };

            return false;
        }
        public VPerfiles getPerfilInfo(Guid userID)
        {
            using (var db = new migenteEntities())
            {
                return db.VPerfiles.Where(x => x.userID == userID.ToString()).FirstOrDefault();


            }
        }

    
    }
}