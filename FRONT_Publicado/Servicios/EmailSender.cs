using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.IO;
using MiGente_Web.Data;
using System.Web.Services.Description;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace MiGente_Web.Servicios
{
    public class EmailSender : EmailService
    {
        public void SendEmailRegistro(string name, string to, string subject, string url)
        {

            try
            {
                //obtener configuracion de correo
                var config = Config_Correo();

                // Leer el contenido HTML del archivo
                var curPath = AppDomain.CurrentDomain.BaseDirectory;
                StreamReader reader = new StreamReader(curPath + "/MailTemplates/confirmacionRegistro.html");

                string readFile = reader.ReadToEnd();
                string htmlBody = "";
                htmlBody = readFile;
                htmlBody = htmlBody.Replace("#linkActivar#", "http://"+url);
                htmlBody = htmlBody.Replace("#nombre#", name);


                MailMessage Msg = new MailMessage(new MailAddress(config.email, "Mi Gente en Linea"), new MailAddress(to));

                // Subject of e-mail
                Msg.Subject = subject;
                Msg.Body = htmlBody;
                Msg.IsBodyHtml = true;


                SmtpClient a = new SmtpClient();


                a.Host = config.servidor;
                a.Credentials = new NetworkCredential(config.email, config.pass);
                a.EnableSsl = true;

                a.Port = 587;
                //try
                //{
                //System.Threading.Thread.Sleep(3000);

                ServicePointManager.ServerCertificateValidationCallback =
    delegate (object s, X509Certificate certificate,
    X509Chain chain, SslPolicyErrors sslPolicyErrors)
    { return true; };

                a.Send(Msg);
                Msg.Dispose();



            }

            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo electrónico: " + ex.Message);
            }
        }

        public void SendEmailCompra(string name, string to, string subject, string plan, decimal monto, string numero)
        {

            try
            {
                //obtener configuracion de correo
                var config = Config_Correo();

                // Leer el contenido HTML del archivo
                var curPath = AppDomain.CurrentDomain.BaseDirectory;
                StreamReader reader = new StreamReader(curPath + "/MailTemplates/checkout.html");

                string readFile = reader.ReadToEnd();
                string htmlBody = "";
                htmlBody = readFile;
                htmlBody = htmlBody.Replace("#nombre#", name);
                htmlBody = htmlBody.Replace("#plan#", plan);
                htmlBody = htmlBody.Replace("#precio#", monto.ToString("N2"));
                htmlBody = htmlBody.Replace("#fecha#", DateTime.Now.Date.ToString("dd/MMM/yyyy"));
                htmlBody = htmlBody.Replace("#transaccion#", numero);



                MailMessage Msg = new MailMessage(new MailAddress(config.email, "Mi Gente en Linea"), new MailAddress(to));

                // Subject of e-mail
                Msg.Subject = subject;
                Msg.Body = htmlBody;
                Msg.IsBodyHtml = true;


                SmtpClient a = new SmtpClient();


                a.Host = config.servidor;
                a.Credentials = new NetworkCredential(config.email, config.pass);
                a.EnableSsl = true;

                a.Port = 587;
                //try
                //{
                //System.Threading.Thread.Sleep(3000);

                ServicePointManager.ServerCertificateValidationCallback =
    delegate (object s, X509Certificate certificate,
    X509Chain chain, SslPolicyErrors sslPolicyErrors)
    { return true; };

                a.Send(Msg);
                Msg.Dispose();



            }

            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo electrónico: " + ex.Message);
            }
        }

        public void SendEmailReset(string name, string to, string subject, string url)
        {

            try
            {
                //obtener configuracion de correo
                var config = Config_Correo();

                // Leer el contenido HTML del archivo
                var curPath = AppDomain.CurrentDomain.BaseDirectory;
                StreamReader reader = new StreamReader(curPath + "/MailTemplates/recuperarPass.html");

                string readFile = reader.ReadToEnd();
                string htmlBody = "";
                htmlBody = readFile;
                htmlBody = htmlBody.Replace("#linkActivar#", "http://" + url);
                htmlBody = htmlBody.Replace("#nombre#", name);


                MailMessage Msg = new MailMessage(new MailAddress(config.email, "Mi Gente en Linea"), new MailAddress(to));

                // Subject of e-mail
                Msg.Subject = subject;
                Msg.Body = htmlBody;
                Msg.IsBodyHtml = true;


                SmtpClient a = new SmtpClient();


                a.Host = config.servidor;
                a.Credentials = new NetworkCredential(config.email, config.pass);
                a.EnableSsl = true;

                a.Port = 587;
                //try
                //{
                //System.Threading.Thread.Sleep(3000);

                ServicePointManager.ServerCertificateValidationCallback =
    delegate (object s, X509Certificate certificate,
    X509Chain chain, SslPolicyErrors sslPolicyErrors)
    { return true; };

                a.Send(Msg);
                Msg.Dispose();



            }

            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo electrónico: " + ex.Message);
            }
        }

    }
}