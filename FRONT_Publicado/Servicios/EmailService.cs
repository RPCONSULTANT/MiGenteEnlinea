using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MiGente_Web.Data;

namespace MiGente_Web.Servicios
{
    public class EmailService
    {
        migenteEntities db = new migenteEntities();
        public Config_Correo Config_Correo()
        {
            return db.Config_Correo.FirstOrDefault();
        }

    }
}