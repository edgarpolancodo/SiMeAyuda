using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.DirectoryServices;
using MvcApplication1.Models;
using System.Data;

namespace MvcApplication1.Controllers
{
    public class SolicitudController : Controller
    {
        //
        // GET: /Solicitud/

        public ActionResult Index()
        {
            if (Session["nombreusuario"] != null)
            {
                ViewBag.Message = "Creando solicitud";
                
                List<Categorias> cat = new Categorias().GetAllCategorias();
                ViewData["categorias"] = cat;
            }
            else 
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(Solicitudes model) 
        {
            if (Session["nombreusuario"] != null)
            {
                model.categoria = new Categorias();
                model.subcategoria = new SubCategorias();
                model.estado = new Estados();
                model.categoria.ID = Convert.ToInt32(Request["Categoria"].ToString());
                model.Prioridad = Request["Prioridad"].ToString();
                model.subcategoria.ID = Convert.ToInt32(Request["SubCategoria"].ToString());
                model.UsuarioCreador = new Usuarios();

                if (Session["rol"].ToString() == "Supervisor" && Request["solicitante"] != "")
                {
                    Usuarios usuario = new Usuarios();
                    if (!usuario.InicioSesion(Request["solicitante"]))
                    {
                        string correoinstitucional = System.Configuration.ConfigurationManager.AppSettings["CorreoInstitucional"];
                        DirectoryEntry deRoot = new DirectoryEntry("LDAP://SEEPYD.local:3268/dc=seepyd,dc=local", "epolanco", "Inicio02");
                        DirectorySearcher dsFindUser = new DirectorySearcher(deRoot);
                        dsFindUser.SearchScope = SearchScope.Subtree;
                        dsFindUser.PropertiesToLoad.Add("sn"); // surname = last name
                        dsFindUser.PropertiesToLoad.Add("givenName"); // first name
                        dsFindUser.PropertiesToLoad.Add("mail"); // correo
                        dsFindUser.PropertiesToLoad.Add("sAMAccountName"); // nombre de usuario
                        dsFindUser.Filter = string.Format("(&(objectCategory=Person)(anr={0}))", Request["solicitante"]);
                        SearchResult result = dsFindUser.FindOne();
                        if (result != null)
                        {
                            string nombre = result.Properties["givenName"][0].ToString() + " " + result.Properties["sn"][0].ToString();
                            string departamento = result.Path.Split(',')[1].Remove(0, 3);
                            string correo = "";
                            if (result.Properties["mail"].Count != 0)
                            {
                                correo = result.Properties["mail"][0].ToString();
                            }
                            else
                            {
                                correo = result.Properties["sAMAccountName"][0].ToString() + correoinstitucional;
                            }
                            usuario.NuevoUsuario(result.Properties["sAMAccountName"][0].ToString(), nombre, departamento, correo);
                            model.UsuarioCreador = usuario;
                        }
                        else 
                        {
                            List<Categorias> cat = new Categorias().GetAllCategorias();
                            ViewData["categorias"] = cat;
                            ViewBag.Message = "Creando solicitud - Nombre de usuario no encontrado";
                            return View();
                        }
                    }
                    else 
                    {
                        model.UsuarioCreador = usuario;
                    }
                }
                else
                {
                    model.UsuarioCreador.InicioSesion(Session["nombreusuario"].ToString());
                }
                if (model.NuevaSolicitud())
                {
                    ViewBag.SolicitudID = model.ID;
                    //Enviar mensaje a usuario solicitante
                    string mensaje = String.Format("Se ha registrado la creación de su solicitud {0}. En lo mas adelante se le estará asistiendo.", model.ID);
                    new Mensajes().EnviarMensaje(Session["correousuario"].ToString(), "Solicitud registrada", mensaje);
                    //Enviar mensaje a usuarios supervisores
                    List<Usuarios> supervisores = new Usuarios().GetAllUsuariosSupervisores();
                    foreach (Usuarios supervisor in supervisores) 
                    {
                        mensaje = String.Format("Se ha registrado la nueva solicitud {0}. Favor asignarlo a un usuario tecnico", model.ID);
                        new Mensajes().EnviarMensaje(supervisor.CorreoElectronico, "Solicitud creada", mensaje);
                    }
                    return View("Creado");
                }
                List<Categorias> cata = new Categorias().GetAllCategorias();
                ViewData["categorias"] = cata;
                return View();
            }
            else 
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult ListaAsignar()
        {
            if (Session["nombreusuario"] != null)
            {
                if (Session["rol"].ToString() == "Supervisor")
                {

                    List<Solicitudes> solicitudes = new Solicitudes().GetSolicitudesNoAsignadas();
                    ViewData["solicitudes"] = solicitudes;

                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else 
            {
                return RedirectToAction("Index", "Home");
            }
            return View("AAsignar");
        }
        public ActionResult Asignar(string id)
        {
            if (Session["nombreusuario"] != null)
            {
                if (Session["rol"].ToString() == "Supervisor")
                {

                    Solicitudes solicitud = new Solicitudes();
                    solicitud.ID = Convert.ToInt32(id);
                    solicitud.CargarSolicitud();
                    ViewBag.Solicitud = solicitud;
                    //Usuarios tecnicos
                    ViewData["Tecnicos"] = new Usuarios().GetAllTecnicos(Convert.ToInt32(id));

                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else 
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Asignado() 
        {
            if (Session["nombreusuario"] != null)
            {
                if (Session["rol"].ToString() == "Supervisor")
                {

                    int tecnicoid = Convert.ToInt32(Request["tecnico"]);
                    string solicitudid = Request["solicitudid"];
                    Solicitudes solicitud = new Solicitudes();
                    solicitud.ID = Convert.ToInt32(solicitudid);
                    solicitud.UsuarioTecnico = new Usuarios();
                    solicitud.UsuarioTecnico.ID = Convert.ToInt32(solicitudid);
                    solicitud.Asignar(Session["nombreusuario"].ToString(), tecnicoid);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Ver(string id) 
        {
            if (Session["nombreusuario"] != null)
            {
                Solicitudes solicitud = new Solicitudes();
                solicitud.ID = Convert.ToInt32(id);
                solicitud.CargarSolicitud();
                ViewBag.Solicitud = solicitud;
                //Usuarios tecnicos
                //ViewBag.Tecnicos = new Conexion().GetAllTecnicos();
                //Estados dependientes de estado actual
                ViewBag.estados = new Conexion().GetEstadosByOrigen(solicitud.estado.ID, Session["nombreusuario"].ToString());
                List<Comentarios> comentarios = new Comentarios().GetComentariosBySolicitudId(Convert.ToInt32(id));
                ViewData["comentarios"] = comentarios;
                if (Session["rol"].ToString() == "Supervisor")
                {
                    ViewData["tecnicos"] = new Usuarios().GetAllTecnicos();
                }
            }
            else {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Ver()
        {
            if (Session["nombreusuario"] != null)
            {
                Solicitudes solicitud = new Solicitudes();
                solicitud.ID = Convert.ToInt32(Request["solicitudid"]);
                solicitud.CargarSolicitud();
                string PrioridadFinal = null, DescripcionFinal = null, SolucionFinal = null, EstadoFinal = null, usuarioTecnico = null;
                bool estadocambiado = false;
                if (solicitud.Prioridad != Request["Prioridad"])
                {
                    PrioridadFinal = Request["Prioridad"].ToString();
                }
                if (solicitud.estado.ID.ToString() != Request["Estado"])
                {
                    EstadoFinal = Request["Estado"].ToString();
                    estadocambiado = true;
                }
                if (solicitud.Descripcion != Request["Descripcion"] && Request["Descripcion"] != "")
                {
                    DescripcionFinal = Request["Descripcion"].ToString();
                }
                if (solicitud.Solucion != Request["Solucion"] && Request["Solucion"] != "")
                {
                    SolucionFinal = Request["Solucion"].ToString();
                }
                if (solicitud.UsuarioTecnico.ID.ToString() != Request["usuarioTecnico"] && Request["usuarioTecnico"] != "")
                {
                    usuarioTecnico = Request["usuarioTecnico"];
                }
                Conexion con = new Conexion();
                con.ModificarSolicitud(solicitud.ID, PrioridadFinal, EstadoFinal, DescripcionFinal, SolucionFinal, Session["nombreusuario"].ToString(), usuarioTecnico, Request["satisfaccion"]);
                con.Close();
                if (estadocambiado) 
                {
                    if (Request["Estado"] == "4") 
                    {
                        //Enviar mensaje a usuario solicitante
                        string mensaje = String.Format("Se ha registrado su solicitud {0} como resuelta. Favor dirigirse al sistema de mesa de ayuda y marcar el estado como validado o no validado. En caso de validado favor llenar el cuestionario de satisfaccion.", solicitud.ID);
                        new Mensajes().EnviarMensaje(solicitud.UsuarioCreador.CorreoElectronico, "Solicitud marcada como resuelta", mensaje);
                    }
                    else if (Request["Estado"] == "8")
                    {
                        //Enviar mensaje a usuaris supervisores
                        Conexion con2 = new Conexion();
                        SqlDataReader super = con2.GetAllUsuariosSupervisores();
                        List<String> supervisores = new List<string>();
                        while (super.Read())
                        {
                            supervisores.Add(super["CorreoElectronico"].ToString());
                        }
                        con2.Close();
                        string mensaje = String.Format("Se ha registrado la solicitud {0} como No Valida por el usuario solicitante. Favor de resolver situación.", solicitud.ID);
                        foreach (string sup in supervisores)
                        {
                            new Mensajes().EnviarMensaje(sup.ToString(), "Solicitud marcada como No Valida", mensaje);
                        }
                    }
                }
                return RedirectToAction("Ver", "Solicitud", solicitud.ID.ToString());
            }
            else 
            {
                return RedirectToAction("Index", "Home");
            }
            
        }
        public ActionResult Comentario() 
        {
            if (Session["nombreusuario"] != null)
            {
            Comentarios comentario = new Comentarios();
            comentario.solicitudid = Convert.ToInt32(Request["solicitudid"].ToString());
            comentario.usuario.InicioSesion(Session["nombreusuario"].ToString());
            comentario.Texto = Request["comentario"];
            comentario.NuevoComentario();
            return RedirectToAction("Ver/"+comentario.solicitudid.ToString(), "Solicitud");
            }else{
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Asignadas() 
        {
            if (Session["nombreusuario"] != null)
            {
                List<Solicitudes> solicitudes = new Solicitudes().GetSolicitudesByAsignado(Session["nombreusuario"].ToString());
                ViewData["solicitudes"] = solicitudes;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult Consultar() 
        {
            if (Session["nombreusuario"] != null)
            {
                SqlDataReader categorias = new Conexion().GetAllCategorias(), departamentos = new Conexion().GetAllDepartamentos(), estados = new Conexion().GetAllEstados();
                ViewBag.categorias = categorias;
                ViewBag.departamentos = departamentos;
                ViewBag.estados = estados;
                ViewData["Tecnicos"] = new Usuarios().GetAllTecnicos();
                ViewData["Solicitantes"] = new Usuarios().GetAllUsuariosSolicitantes();
            }
            else 
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Consultado() 
        {
            if (Session["nombreusuario"] != null)
            {
                string categoria = null, prioridad = null, estado = null, departamento = null, creador = null, tecnico = null, fechaDesde = null, fechaHasta = null, fechaModificacionDesde = null, fechaModificacionHasta = null, pagina = null;
                if (Request["Categoria"] != "")
                {
                    categoria = Request["Categoria"];
                }
                if (Request["Prioridad"] != "")
                {
                    prioridad = Request["Prioridad"];
                }
                if (Request["Estado"] != "")
                {
                    estado = Request["Estado"];
                }
                if (Request["Departamento"] != "")
                {
                    departamento = Request["Departamento"];
                }
                if (Request["UsuarioCreador"] != "")
                {
                    creador = Request["UsuarioCreador"];
                }
                if (Request["UsuarioTecnico"] != "")
                {
                    tecnico = Request["UsuarioTecnico"];
                }
                if (Request["FechaDesde"] != "")
                {
                    fechaDesde = Request["FechaDesde"];
                }
                if (Request["FechaHasta"] != "")
                {
                    fechaHasta = Request["FechaHasta"];
                }
                if (Request["FechaModificacionDesde"] != "")
                {
                    fechaModificacionDesde = Request["FechaModificacionDesde"];
                }
                if (Request["FechaModificacionHasta"] != "")
                {
                    fechaModificacionHasta = Request["FechaModificacionHasta"];
                }
                if (Request["pagina"] != "" && Request["pagina"] != null)
                {
                    pagina = Request["pagina"];
                }
                else 
                {
                    pagina = "1";
                }
                Conexion con = new Conexion();
                DataSet ds = con.Consulta(categoria, prioridad, estado, departamento, creador, tecnico, fechaDesde, fechaHasta, fechaModificacionDesde, fechaModificacionHasta, pagina);
                DataTableReader paginas = ds.Tables[2].CreateDataReader();
                paginas.Read();
                List<Solicitudes> solicitudesFiltradas = new Solicitudes().GetSolicitudesFiltradas(ds.Tables[1].CreateDataReader());
                ViewData["solicitudesFiltradas"] = solicitudesFiltradas;
                ViewBag.paginas = paginas["Paginas"].ToString();
                ViewBag.campos = Request;
                con.Close();
            }
            else {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public ActionResult HistorialSolicitud(string id) 
        {
            if (Session["nombreusuario"] != null)
            {
                List<Solicitudes> historiaSolicitud = new Solicitudes().GetHistorialSolicitudes(Convert.ToInt32(id));
                ViewData["historia"] = historiaSolicitud;
            }
            else {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public ActionResult Adjuntos(string id)
        {
            if (Session["nombreusuario"] != null)
            {
                ViewBag.solicitudid = id;
                ViewData["adjuntos"] = new Adjuntos().CargarAdjuntos(Convert.ToInt32(id));
            }
            else 
            {
                return RedirectToAction("Index", "Home");
            }
            return View();

        }
        [HttpPost]
        public ActionResult AdjuntosSubidos(HttpPostedFileBase[] archivos)
        {
            if (Session["nombreusuario"] != null && archivos.Length > 0)
            {
                Conexion con = new Conexion();
                if (!System.IO.Directory.Exists(String.Format(@"{0}{1}", System.Configuration.ConfigurationManager.AppSettings["UbicacionAdjuntos"], Request["solicitudid"])))
                {
                    System.IO.Directory.CreateDirectory(String.Format(@"{0}{1}", System.Configuration.ConfigurationManager.AppSettings["UbicacionAdjuntos"], Request["solicitudid"]));
                }
                else
                {
                    if (!System.IO.Directory.Exists(String.Format(@"{0}{1}\{2}", System.Configuration.ConfigurationManager.AppSettings["UbicacionAdjuntos"], Request["solicitudid"], Session["nombreusuario"].ToString())))
                    {
                        System.IO.Directory.CreateDirectory(String.Format(@"{0}{1}\{2}", System.Configuration.ConfigurationManager.AppSettings["UbicacionAdjuntos"], Request["solicitudid"], Session["nombreusuario"].ToString()));
                    }
                }
                if (!System.IO.Directory.Exists(String.Format(@"{0}{1}\{2}", System.Configuration.ConfigurationManager.AppSettings["UbicacionAdjuntos"], Request["solicitudid"], Session["nombreusuario"].ToString())))
                {
                    System.IO.Directory.CreateDirectory(String.Format(@"{0}{1}\{2}", System.Configuration.ConfigurationManager.AppSettings["UbicacionAdjuntos"], Request["solicitudid"], Session["nombreusuario"].ToString()));
                }
                foreach (HttpPostedFileBase arch in archivos)
                {
                    if (arch != null)
                    {
                        arch.SaveAs(String.Format(@"{0}\{1}\{2}\{3}", System.Configuration.ConfigurationManager.AppSettings["UbicacionAdjuntos"], Request["solicitudid"], Session["nombreusuario"].ToString(), arch.FileName));
                        con.GuardarArchivos(Session["nombreusuario"].ToString(), Convert.ToInt32(Request["solicitudid"]), String.Format(@"{0}/{1}/{2}", Request["solicitudid"], Session["nombreusuario"].ToString(), arch.FileName));
                    }
                }
                con.Close();
                return RedirectToAction("Adjuntos/" + Request["solicitudid"]);
            }
            else 
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult CreadasPorMi() {
            if (Session["nombreusuario"] != null)
            {
                List<Solicitudes> solicitudesFiltradas = new Solicitudes().GetSolicitudesByCreador(Session["nombreusuario"].ToString());
                ViewData["solicitudesFiltradas"] = solicitudesFiltradas;
            }
            else 
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        public ActionResult Reportes() 
        {
            if (Session["nombreusuario"] != null)
            {
                if (Session["rol"].ToString() == "Supervisor")
                {
                    SqlDataReader categorias = new Conexion().GetAllCategorias(), departamentos = new Conexion().GetAllDepartamentos(), estados = new Conexion().GetAllEstados();
                    ViewBag.categorias = categorias;
                    ViewBag.departamentos = departamentos;
                    ViewBag.estados = estados;
                    ViewData["Tecnicos"] = new Usuarios().GetAllTecnicos();
                    ViewData["Solicitantes"] = new Usuarios().GetAllUsuariosSolicitantes();
                }
                else 
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else 
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Reportado()
        {
            if (Session["nombreusuario"] != null)
            {
                if (Session["rol"].ToString() == "Supervisor")
                {
                    string query = "&NombreUsuario=" + Session["nombreusuario"].ToString();
                    if (Request["Categoria"] != "")
                    {
                        query += "&Categoria=" + Request["Categoria"];
                    }
                    else
                    {
                        query += "&Categoria:isnull=true";
                    }
                    if (Request["Prioridad"] != "")
                    {
                        query += "&Prioridad=" + Request["Prioridad"];
                    }
                    else
                    {
                        query += "&Prioridad:isnull=true";
                    }
                    if (Request["Estado"] != "")
                    {
                        query += "&Estado=" + Request["Estado"];
                    }
                    else
                    {
                        query += "&Estado:isnull=true";
                    }
                    if (Request["Departamento"] != "")
                    {
                        query += "&Departamento=" + Request["Departamento"];
                    }
                    else
                    {
                        query += "&Departamento:isnull=true";
                    }
                    if (Request["UsuarioCreador"] != "")
                    {
                        query += "&UsuarioCreador=" + Request["UsuarioCreador"];
                    }
                    else
                    {
                        query += "&UsuarioCreador:isnull=true";
                    }
                    if (Request["UsuarioTecnico"] != "")
                    {
                        query += "&UsuarioTecnico=" + Request["UsuarioTecnico"];
                    }
                    else
                    {
                        query += "&UsuarioTecnico:isnull=true";
                    }
                    if (Request["FechaDesde"] != "")
                    {
                        query += "&CreacionDesde=" + Request["FechaDesde"];
                    }
                    else 
                    {
                        query += "&CreacionDesde:isnull=true";
                    }
                    if (Request["FechaHasta"] != "")
                    {
                        query += "&CreacionHasta=" + Request["FechaHasta"];
                    }
                    else 
                    {
                        query += "&CreacionHasta:isnull=true";
                    }
                    if (Request["FechaModificacionDesde"] != "")
                    {
                        query += "&ModificacionDesde=" + Request["FechaModificacionDesde"];
                    }
                    else 
                    {
                        query += "&ModificacionDesde:isnull=true";
                    }
                    if (Request["FechaModificacionHasta"] != "")
                    {
                        query += "&ModificacionHasta=" + Request["FechaModificacionHasta"];
                    }
                    else 
                    {
                        query += "&ModificacionHasta:isnull=true";
                    }
                    return Redirect(System.Configuration.ConfigurationManager.AppSettings["ReportingServerURL"] + "&rs:Command=Render" + query + "&rs:Format=" + Request["Formato"]);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }else
            {
                return RedirectToAction("Index", "Home");
            }
            }
        public ActionResult SubCategorias(string id) 
        {
            List<SubCategorias> subc = new SubCategorias().GetSubCategoriasByCategoriaId(Convert.ToInt32(id));
            ViewData["subcategorias"] = subc;
            return View();
        }
    }
}
