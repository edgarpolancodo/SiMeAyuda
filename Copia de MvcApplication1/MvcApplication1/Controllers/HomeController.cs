using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.DirectoryServices;
using MvcApplication1.Models;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Bienvenido nuevamente";
            string correoinstitucional = System.Configuration.ConfigurationManager.AppSettings["CorreoInstitucional"];
            string nombreusuario = User.Identity.Name.Split('\\')[1].ToString();
            Usuarios usuario = new Usuarios();
            if (!usuario.InicioSesion(nombreusuario)) 
            {
                DirectoryEntry deRoot = new DirectoryEntry(System.Configuration.ConfigurationManager.AppSettings["ActiveDirectoryURL"], System.Configuration.ConfigurationManager.AppSettings["ActiveDirectoryUser"], System.Configuration.ConfigurationManager.AppSettings["ActiveDirectoryPassword"]);
                        
                DirectorySearcher dsFindUser = new DirectorySearcher(deRoot);
                dsFindUser.SearchScope = SearchScope.Subtree;

                dsFindUser.PropertiesToLoad.Add("cn"); // nombre completo
                //dsFindUser.PropertiesToLoad.Add("givenName"); // first name
                dsFindUser.PropertiesToLoad.Add("mail"); // correo
                dsFindUser.Filter = string.Format("(&(objectCategory=Person)(anr={0}))", nombreusuario);
                SearchResult result = dsFindUser.FindOne();
                ViewBag.Nombre = result.Properties["cn"][0].ToString();
                string departamento = result.Path.Split(',')[1].Remove(0, 3);
                string correo = "";
                if (result.Properties["mail"].Count != 0)
                {
                    correo = result.Properties["mail"][0].ToString();
                }
                else 
                {
                    correo = nombreusuario + correoinstitucional;
                }
                usuario.NuevoUsuario(nombreusuario, ViewBag.Nombre, departamento, correo);
                string mensaje = "Bienvenido al Sistema de Mesa de Ayuda del Ministerio de Economia, Planificación y Desarrollo\n Por esta vía se le enviará notificaciones de sus solicitudes.";
                new Mensajes().EnviarMensaje(correo, "Bienvenido al SiMeAyuda", mensaje);
                ViewBag.Message = "Bienvenido por primera vez";
            }
            Session.Timeout = 20;
            Session["nombre"] = usuario.Nombre;
            Session["nombreusuario"] = usuario.NombreUsuario;
            Session["correousuario"] = usuario.CorreoElectronico;
            Session["rol"] = usuario.rol.Nombre;
            return View();
        }

        public ActionResult About()
        {
            if (Session["nombreusuario"] != null)
            {
                return View();
            }
            else {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
