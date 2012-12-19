using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace MvcApplication1.Models
{
    public class Comentarios
    {
        public int ID;
        public string Texto;
        public int solicitudid;
        public Usuarios usuario;
        public DateTime tiempo;
        public Comentarios() {
            usuario = new Usuarios();
        }
        public bool NuevoComentario() 
        {
            Conexion con = new Conexion();
            this.ID = con.NuevoComentario(this.Texto, usuario.ID, solicitudid);
            con.Close();
            return true;
        }
        public List<Comentarios> GetComentariosBySolicitudId(int solicitud_id)
        {
            Conexion con = new Conexion();
            SqlDataReader datos = con.GetComentariosBySolicitudId(solicitud_id);
            List<Comentarios> comentarios = new List<Comentarios>();
            while (datos.Read()) 
            {
                Comentarios comentario = new Comentarios();
                comentario.Texto = datos["Texto"].ToString();
                comentario.tiempo = Convert.ToDateTime(datos["Tiempo"]);
                comentario.usuario = new Usuarios();
                comentario.usuario.InicioSesion(datos["NombreUsuario"].ToString());
                comentarios.Add(comentario);
            }
            con.Close();
            return comentarios;
        }
    }
    public class Adjuntos {
        public int ID;
        public int solicitudId;
        public Usuarios usuario;
        public string Archivo;
        public DateTime fechatiempo;
        public Adjuntos() 
        {
            this.usuario = new Usuarios();
        }
        public List<Adjuntos> CargarAdjuntos(int solicitud) 
        {
            Conexion con = new Conexion();
            List<Adjuntos> adjuntos = new List<Adjuntos>();
            SqlDataReader data = con.GetAdjuntosBySolicitud(solicitud);
            while (data.Read()) 
            {
                Adjuntos adjunto = new Adjuntos();
                adjunto.ID = Convert.ToInt32(data["ID"].ToString());
                adjunto.solicitudId = solicitud;
                adjunto.usuario = new Usuarios();
                adjunto.usuario.InicioSesion(data["NombreUsuario"].ToString());
                adjunto.Archivo = data["Archivo"].ToString();
                adjunto.fechatiempo = Convert.ToDateTime(data["FechaTiempo"]);
                adjuntos.Add(adjunto);
            }
            con.Close();
            return adjuntos;
        }
    }
}