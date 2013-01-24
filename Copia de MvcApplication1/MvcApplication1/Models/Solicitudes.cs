using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;

namespace MvcApplication1.Models
{
    public class Solicitudes
    {
        public int ID { get; set; }
        public Usuarios UsuarioCreador { get; set; }
        public Usuarios UsuarioTecnico { get; set; }
        public string Descripcion { get; set; }
        public string Solucion { get; set; }
        public string Prioridad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime UltimaModificacion { get; set; }
        public Categorias categoria { get; set; }
        public SubCategorias subcategoria { get; set; }
        public Estados estado { get; set; }
        public Solicitudes() 
        {
            categoria = new Categorias();
            subcategoria = new SubCategorias();
            estado = new Estados();
        }
        public bool NuevaSolicitud() 
        {
            Conexion con = new Conexion();
            SqlDataReader solicituddata = con.NuevaSolicitud(UsuarioCreador.NombreUsuario, Descripcion, categoria.ID, Prioridad, subcategoria.ID);
            solicituddata.Read();
            this.UsuarioCreador.InicioSesion(this.UsuarioCreador.NombreUsuario);
            this.categoria.ID = Convert.ToInt32(solicituddata["CategoriaID"]);
            this.categoria.Nombre = Convert.ToString(solicituddata["Categoria"]);
            this.subcategoria.ID = Convert.ToInt32(solicituddata["SubCategoriaID"]);
            this.subcategoria.Nombre = Convert.ToString(solicituddata["SubCategoria"]);
            this.estado.ID = Convert.ToInt32(solicituddata["EstadoID"]);
            this.estado.Nombre = Convert.ToString(solicituddata["Estado"]);
            this.ID = Convert.ToInt32(solicituddata["SolicitudID"]);
            this.Descripcion = Convert.ToString(solicituddata["Descripcion"]);
            this.Prioridad = Convert.ToString(solicituddata["Prioridad"]);
            this.FechaCreacion = Convert.ToDateTime(solicituddata["FechaCreacion"]);
            this.UltimaModificacion = Convert.ToDateTime(solicituddata["UltimaModificacion"]);
            con.Close();
            return true;
        }
        public bool CargarSolicitud() 
        {
            Conexion con = new Conexion();
            SqlDataReader solicituddata = con.GetSolicitudById(this.ID);
            solicituddata.Read();
            this.UsuarioCreador = new Usuarios();
            this.UsuarioTecnico = new Usuarios();
            this.UsuarioCreador.InicioSesion(Convert.ToString(solicituddata["NombreUsuario"]));
            this.UsuarioTecnico.InicioSesion(Convert.ToString(solicituddata["UsuarioTecnico"]));
            this.categoria.ID = Convert.ToInt32(solicituddata["CategoriaID"]);
            this.categoria.Nombre = Convert.ToString(solicituddata["Categoria"]);
            this.subcategoria.ID = Convert.ToInt32(solicituddata["SubCategoriaID"]);
            this.subcategoria.Nombre = Convert.ToString(solicituddata["SubCategoria"]);
            this.estado.ID = Convert.ToInt32(solicituddata["EstadoID"]);
            this.estado.Nombre = Convert.ToString(solicituddata["Estado"]);
            this.ID = Convert.ToInt32(solicituddata["SolicitudID"]);
            this.Descripcion = Convert.ToString(solicituddata["Descripcion"]);
            this.Solucion = Convert.ToString(solicituddata["Solucion"]);
            this.Prioridad = Convert.ToString(solicituddata["Prioridad"]);
            this.FechaCreacion = Convert.ToDateTime(solicituddata["FechaCreacion"]);
            this.UltimaModificacion = Convert.ToDateTime(solicituddata["UltimaModificacion"]);
            con.Close();
            return true;
            
        }
        public bool Asignar(string supervisor, int tecnico) 
        {
            Conexion con = new Conexion();
            con.AsignarTecnico(this.ID, tecnico, supervisor);
            //Enviar mensaje a usuario tecnico
            SqlDataReader usuario = con.GetUsuarioById(tecnico);
            usuario.Read();
            string mensaje = String.Format("Se le ha asignado la nueva solicitud {0}. Favor trabajarla lo mas breve posible", this.ID);
            new Mensajes().EnviarMensaje(usuario["CorreoElectronico"].ToString(), "Solicitud asignada", mensaje);
            con.Close();
            return true;
        }
        public List<Solicitudes> GetSolicitudesByAsignado(string nombreusuario) 
        {
            Conexion con = new Conexion();
            SqlDataReader data = con.GetSolicitudesByAsignado(nombreusuario);
            List<Solicitudes> solicitudes = new List<Solicitudes>();
            while (data.Read()) 
            {
                Solicitudes solicitud = new Solicitudes();
                solicitud.ID = Convert.ToInt32(data["ID"].ToString());
                solicitud.CargarSolicitud();
                solicitudes.Add(solicitud);
            }
            con.Close();
            return solicitudes;
        }
        public List<Solicitudes> GetSolicitudesNoAsignadas() 
        {
            Conexion con = new Conexion();
            SqlDataReader data = con.GetSolicitudesNoAsignadas();
            List<Solicitudes> solicitudes = new List<Solicitudes>();
            while (data.Read())
            {
                Solicitudes solicitud = new Solicitudes();
                solicitud.ID = Convert.ToInt32(data["ID"].ToString());
                solicitud.CargarSolicitud();
                solicitudes.Add(solicitud);
            }
            con.Close();
            return solicitudes;
        }
        public List<Solicitudes> GetHistorialSolicitudes(int solicitudid) 
        {
            Conexion con = new Conexion();
            SqlDataReader data = con.GetHistorialSolicitud(solicitudid);
            List<Solicitudes> solicitudes = new List<Solicitudes>();
            while (data.Read())
            {
                Solicitudes solicitud = new Solicitudes();
                solicitud.FechaCreacion = Convert.ToDateTime(data["FechaTiempo"]);
                solicitud.UsuarioCreador = new Usuarios();
                solicitud.UsuarioCreador.Nombre = data["Usuario"].ToString();
                solicitud.UsuarioTecnico = new Usuarios();
                solicitud.UsuarioTecnico.Nombre = data["Tecnico"].ToString();
                solicitud.estado.Nombre = data["Estado"].ToString();
                solicitud.Descripcion = data["Descripcion"].ToString();
                solicitud.Solucion = data["Solucion"].ToString();
                solicitud.Prioridad = data["Prioridad"].ToString();
                solicitudes.Add(solicitud);
            }
            con.Close();
            return solicitudes; 
        }
        public List<Solicitudes> GetSolicitudesFiltradas(string categoria, string prioridad, string estado, string departamento, string creador, string tecnico, string fechadesde, string fechahasta, string fechaModificacionDesde, string fechaModificacionHasta)
        {
            Conexion con = new Conexion();
            DataTableReader data = con.Consulta(categoria, prioridad, estado, departamento, creador, tecnico, fechadesde, fechahasta, fechaModificacionDesde, fechaModificacionHasta);
            List<Solicitudes> solicitudes = new List<Solicitudes>();
            while (data.Read())
            {
                Solicitudes solicitud = new Solicitudes();
                solicitud.ID = Convert.ToInt32(data["ID"].ToString());
                solicitud.CargarSolicitud();
                solicitudes.Add(solicitud);
            }
            con.Close();
            return solicitudes;
        }
        public List<Solicitudes> GetSolicitudesByCreador(string nombreusuario)
        {
            Conexion con = new Conexion();
            SqlDataReader data = con.GetSolicitudesByCreador(nombreusuario);
            List<Solicitudes> solicitudes = new List<Solicitudes>();
            while (data.Read())
            {
                Solicitudes solicitud = new Solicitudes();
                solicitud.ID = Convert.ToInt32(data["ID"].ToString());
                solicitud.CargarSolicitud();
                solicitudes.Add(solicitud);
            }
            con.Close();
            return solicitudes;
        }

    }
    public class Categorias 
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public List<Categorias> GetAllCategorias() {
            Conexion con = new Conexion();
            List<Categorias> cats = new List<Categorias>();
            SqlDataReader categorias = con.GetAllCategorias();
            while (categorias.Read()) 
            {
                Categorias cat = new Categorias();
                cat.ID = Convert.ToInt32(categorias["ID"]);
                cat.Nombre = categorias["Nombre"].ToString();
                cats.Add(cat);
            }
            con.Close();
            return cats;
        }
    }
    public class SubCategorias 
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public List<SubCategorias> GetSubCategoriasByCategoriaId(int id) 
        {
            Conexion con = new Conexion();
            List<SubCategorias> subcats = new List<SubCategorias>();
            SqlDataReader subcategorias = con.GetSubCategoriasByCategoriaId(id);
            while (subcategorias.Read())
            {
                SubCategorias cat = new SubCategorias();
                cat.ID = Convert.ToInt32(subcategorias["ID"]);
                cat.Nombre = subcategorias["Nombre"].ToString();
                subcats.Add(cat);
            }
            con.Close();
            return subcats;
        }
    }
    public class Estados 
    {
        public int ID { get; set; } 
        public int EstadoOrigenID { get; set; }
        public string Nombre { get; set; }
    }
}