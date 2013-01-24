using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace MvcApplication1.Models
{
    public class Usuarios
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public string NombreUsuario { get; set; }
        public string CorreoElectronico { get; set; }
        public Departamentos departamento { get; set; }
        public string Extension { get; set; }
        public Rol rol { get; set; }
        public DateTime UltimoAcceso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public Usuarios() 
        {
            this.departamento = new Departamentos();
            this.rol = new Rol();
        }
        public bool InicioSesion(string nombreusuario) 
        {
            Conexion con = new Conexion();
            SqlDataReader userdata = con.LoginUsuario(nombreusuario);
            if (userdata.Read())
            {
                this.ID = Convert.ToInt32(userdata["UsuarioID"]);
                this.Nombre = Convert.ToString(userdata["Nombre"]);
                this.NombreUsuario = Convert.ToString(userdata["NombreUsuario"]);
                this.CorreoElectronico = Convert.ToString(userdata["Correo"]);
                this.rol.ID = Convert.ToInt32(userdata["RolID"]);
                this.rol.Nombre = Convert.ToString(userdata["Rol"]);
                this.departamento.ID = Convert.ToInt32(userdata["DepartamentoID"]);
                this.departamento.Nombre = Convert.ToString(userdata["Departamento"]);
                con.Close();
                return true;
            }
            else 
            {
                con.Close();
                return false;
            }
        }
        public bool NuevoUsuario(string nombreusuario, string nombre, string departamento, string correo)
        {
            Conexion con = new Conexion();
            SqlDataReader userdata = con.NuevoUsuario(nombre, nombreusuario, correo, departamento);
            if (userdata.Read())
            {
                this.ID = Convert.ToInt32(userdata["UsuarioID"]);
                this.Nombre = Convert.ToString(userdata["Nombre"]);
                this.NombreUsuario = Convert.ToString(userdata["NombreUsuario"]);
                this.CorreoElectronico = Convert.ToString(userdata["Correo"]);
                this.rol.ID = Convert.ToInt32(userdata["RolID"]);
                this.rol.Nombre = Convert.ToString(userdata["Rol"]);
                this.departamento.ID = Convert.ToInt32(userdata["DepartamentoID"]);
                this.departamento.Nombre = Convert.ToString(userdata["Departamento"]);
                con.Close();
                return true;
            }
            else
            {
                con.Close();
                return false;
            }
        }
        public List<Usuarios> GetAllTecnicos() 
        {
            Conexion con = new Conexion();
            List<Usuarios> usuarios = new List<Usuarios>();
            SqlDataReader users = con.GetAllTecnicos();
            while (users.Read()) 
            {
                Usuarios usuario = new Usuarios();
                usuario.InicioSesion(users["NombreUsuario"].ToString());
                usuarios.Add(usuario);
            }
            con.Close();
            return usuarios;
        }
        public List<Usuarios> GetAllTecnicos(int solicitudid)
        {
            Conexion con = new Conexion();
            List<Usuarios> usuarios = new List<Usuarios>();
            SqlDataReader users = con.GetAllTecnicos(solicitudid);
            while (users.Read())
            {
                Usuarios usuario = new Usuarios();
                usuario.InicioSesion(users["NombreUsuario"].ToString());
                usuarios.Add(usuario);
            }
            con.Close();
            return usuarios;
        }
        public List<Usuarios> GetAllUsuariosSolicitantes()
        {
            Conexion con = new Conexion();
            List<Usuarios> usuarios = new List<Usuarios>();
            SqlDataReader users = con.GetAllUsuariosSolicitantes();
            while (users.Read())
            {
                Usuarios usuario = new Usuarios();
                usuario.InicioSesion(users["NombreUsuario"].ToString());
                usuarios.Add(usuario);
            }
            con.Close();
            return usuarios;
        }
        public List<Usuarios> GetAllUsuariosSupervisores()
        {
            Conexion con = new Conexion();
            List<Usuarios> usuarios = new List<Usuarios>();
            SqlDataReader users = con.GetAllUsuariosSupervisores();
            while (users.Read())
            {
                Usuarios usuario = new Usuarios();
                usuario.InicioSesion(users["NombreUsuario"].ToString());
                usuarios.Add(usuario);
            }
            con.Close();
            return usuarios;
        } 
    }
    public class Departamentos 
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
    }
    public class Rol {
        public int ID { get; set; }
        public string Nombre { get; set; }
    }
}