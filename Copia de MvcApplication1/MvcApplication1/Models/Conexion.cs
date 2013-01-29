using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;

namespace MvcApplication1.Models
{
    public class Conexion
    {
        SqlConnection conexion;
        public Conexion() {
            conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conexion.Open();
        }
        public void Close() {
            conexion.Close();
        }

        public SqlDataReader LoginUsuario(string nombreusuario)
        {
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcLoginUsuario";
            comando.Parameters.Add(new SqlParameter("@nombreusuario", nombreusuario));
            SqlDataReader resultado = comando.ExecuteReader();
            comando.Dispose();
            return resultado;
        }
        public SqlDataReader NuevoUsuario(string nombre, string nombreusuario, string correo, string departamento) {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcNuevoUsuario";
            comando.Parameters.Add(new SqlParameter("@nombre", nombre));
            comando.Parameters.Add(new SqlParameter("@nombreusuario", nombreusuario));
            comando.Parameters.Add(new SqlParameter("@correo", correo));
            comando.Parameters.Add(new SqlParameter("@departamento", departamento));
            comando.Parameters.Add(new SqlParameter("@rol", '1'));
            SqlDataReader resultado = comando.ExecuteReader();
            
            return resultado;
        }
        public SqlDataReader NuevaSolicitud(string usuario, string descripcion, int categoria, string prioridad, int subcategoria) 
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcNuevaSolicitud";
            comando.Parameters.Add(new SqlParameter("@Usuario", usuario));
            comando.Parameters.Add(new SqlParameter("@categoriaID", categoria));
            comando.Parameters.Add(new SqlParameter("@prioridad", prioridad));
            comando.Parameters.Add(new SqlParameter("@descripcion", descripcion));
            comando.Parameters.Add(new SqlParameter("@subcategoriaID", subcategoria));
            SqlDataReader resultado = comando.ExecuteReader();
            
            return resultado;
        }
        public SqlDataReader GetSolicitudById(int Id)
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGetSolicitudById";
            comando.Parameters.Add(new SqlParameter("@id", Id));
            SqlDataReader resultado = comando.ExecuteReader();
            
            return resultado;
            
        }
        public SqlDataReader GetAllCategorias() 
        {
            //conexion.Open();
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.Text;
            comando.Connection = conexion;
            comando.CommandText = "select * from Categorias";
            SqlDataReader resultado = comando.ExecuteReader();
            return resultado;
        }
        public SqlDataReader GetSubCategoriasByCategoriaId(int id) 
        {
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.Text;
            comando.Connection = conexion;
            comando.CommandText = String.Format("select * from SubCategorias where CategoriaID='{0}'", id.ToString());
            SqlDataReader resultado = comando.ExecuteReader();
            return resultado;
        }
        public SqlDataReader GetAllTecnicos() 
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGetAllTecnicos";
            SqlDataReader resultado = comando.ExecuteReader();
            return resultado;
        }
        //Esta funcion busca los tecnicos basados en la subcategoria de la solicitud
        public SqlDataReader GetAllTecnicos(int solicitudid)
        {

            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGetAllTecnicosBySubCategoria";
            comando.Parameters.Add(new SqlParameter("@solicitudID", solicitudid));
            SqlDataReader resultado = comando.ExecuteReader();
            return resultado;
        }
        public bool AsignarTecnico(int solicitudid, int tecnico, string supervisor)
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcAsignarTecnico";
            comando.Parameters.Add(new SqlParameter("@solicitudID", solicitudid));
            comando.Parameters.Add(new SqlParameter("@tecnico", tecnico));
            comando.Parameters.Add(new SqlParameter("@supervisor", supervisor));
            comando.ExecuteNonQuery();
            
            return true;
        }
        public SqlDataReader GetAllEstados()
        {
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.Text;
            comando.Connection = conexion;
            comando.CommandText = String.Format("select * from Estados");
            //conexion.Close();
            return comando.ExecuteReader();
        }
        public SqlDataReader GetAllUsuariosSolicitantes()
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.Text;
            comando.Connection = conexion;
            comando.CommandText = String.Format("select * from Usuarios where RolID=(select ID from Roles where Nombre='Solicitante')");
            SqlDataReader resultado = comando.ExecuteReader();
            
            return resultado;
        }
        public SqlDataReader GetUsuarioById(int id) 
        {
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.Text;
            comando.Connection = conexion;
            comando.CommandText = String.Format("select * from Usuarios where ID={0}",id.ToString());
            SqlDataReader resultado = comando.ExecuteReader();

            return resultado;
        }
        public SqlDataReader GetAllUsuariosSupervisores()
        {

            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.Text;
            comando.Connection = conexion;
            comando.CommandText = String.Format("select * from Usuarios where RolID=(select ID from Roles where Nombre='Supervisor')");
            SqlDataReader resultado = comando.ExecuteReader();

            return resultado;
        }
        public SqlDataReader GetAllDepartamentos()
        {
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.Text;
            comando.Connection = conexion;
            comando.CommandText = String.Format("select * from Departamentos");
            SqlDataReader resultado = comando.ExecuteReader();
            return resultado;
        }
        public SqlDataReader GetEstadosByOrigen(int estado, string nombreusuario)
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGetEstadosByOrigen";
            comando.Parameters.Add(new SqlParameter("@id", estado));
            comando.Parameters.Add(new SqlParameter("@nombreusuario", nombreusuario));
            SqlDataReader resultado = comando.ExecuteReader();
            
            return resultado;
        }
        public bool ModificarSolicitud(int solicitudid, string prioridad, string estado, string descripcion, string solucion, string usuario, string usuarioTecnico, string satisfaccion) {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcModificarSolicitud";
            comando.Parameters.Add(new SqlParameter("@solicitudID", solicitudid));
            comando.Parameters.Add(new SqlParameter("@prioridad", prioridad));
            comando.Parameters.Add(new SqlParameter("@estado", estado));
            comando.Parameters.Add(new SqlParameter("@descripcion", descripcion));
            comando.Parameters.Add(new SqlParameter("@solucion", solucion));
            comando.Parameters.Add(new SqlParameter("@usuario", usuario));
            comando.Parameters.Add(new SqlParameter("@usuarioAsignado", usuarioTecnico));
            comando.Parameters.Add(new SqlParameter("@satisfaccion", satisfaccion));
            comando.ExecuteNonQuery();
            
            return true;
        }
        public int NuevoComentario(string Texto, int usuario, int solicitudid) 
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGuardarComentario";
            comando.Parameters.Add(new SqlParameter("@texto", Texto));
            comando.Parameters.Add(new SqlParameter("@solicitudid", solicitudid));
            comando.Parameters.Add(new SqlParameter("@usuarioid", usuario));
            
            return Convert.ToInt32(comando.ExecuteScalar().ToString());
            
        }
        public SqlDataReader GetComentariosBySolicitudId(int solicitudid)
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGetComentariosBySolicitudId";
            comando.Parameters.Add(new SqlParameter("@solicitudid", solicitudid));
            SqlDataReader resultado = comando.ExecuteReader();
            
            return resultado;

        }
        public SqlDataReader GetSolicitudesByAsignado(string nombreusuario)
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGetSolicitudesByAsignado";
            comando.Parameters.Add(new SqlParameter("@nombreusuario", nombreusuario));
            SqlDataReader resultado = comando.ExecuteReader();
            
            return resultado;

        }
        public SqlDataReader GetSolicitudesByCreador(string nombreusuario)
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGetSolicitudesByCreador";
            comando.Parameters.Add(new SqlParameter("@nombreusuario", nombreusuario));
            SqlDataReader resultado = comando.ExecuteReader();
            
            return resultado;

        }
        public SqlDataReader GetSolicitudesNoAsignadas() 
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGetSolicitudesNoAsignadas";
            SqlDataReader resultado = comando.ExecuteReader();
            
            return resultado;
        }
        public SqlDataReader GetHistorialSolicitud(int solicitudid) 
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGetHistorialSolicitud";
            comando.Parameters.Add(new SqlParameter("@solicitudid", solicitudid));
            SqlDataReader resultado = comando.ExecuteReader();
            
            return resultado;
        }
        public DataSet Consulta(string categoria, string prioridad, string estado, string departamento, string creador, string tecnico, string fechadesde, string fechahasta, string fechaModificacionDesde, string fechaModificacionHasta, string pagina) 
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcConsulta";
            comando.Parameters.Add(new SqlParameter("@categoria", categoria));
            comando.Parameters.Add(new SqlParameter("@prioridad", prioridad));
            comando.Parameters.Add(new SqlParameter("@estado", estado));
            comando.Parameters.Add(new SqlParameter("@departamento", departamento));
            comando.Parameters.Add(new SqlParameter("@creador", creador));
            comando.Parameters.Add(new SqlParameter("@tecnico", tecnico));
            comando.Parameters.Add(new SqlParameter("@fechaDesde", fechadesde));
            comando.Parameters.Add(new SqlParameter("@fechaHasta", fechahasta));
            comando.Parameters.Add(new SqlParameter("@fechaModificacionDesde", fechaModificacionDesde));
            comando.Parameters.Add(new SqlParameter("@fechaModificacionHasta", fechaModificacionHasta));
            comando.Parameters.Add(new SqlParameter("@pagina", pagina));
            SqlDataAdapter a = new SqlDataAdapter(comando);
            DataSet ds = new DataSet();
            a.Fill(ds);
            
            return ds;
        }
        public SqlDataReader GetAdjuntosBySolicitud(int solicitud)
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGetAdjuntosBySolicitud";
            comando.Parameters.Add(new SqlParameter("@id", solicitud));
            SqlDataReader resultado = comando.ExecuteReader();
            
            return resultado;

        }
        public bool GuardarArchivos(string usuario, int solicitudid, string archivo) 
        {
            
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGuardarArchivos";
            comando.Parameters.Add(new SqlParameter("@usuarioid", usuario));
            comando.Parameters.Add(new SqlParameter("@solicitudid", solicitudid));
            comando.Parameters.Add(new SqlParameter("@Archivo", archivo));
            comando.ExecuteNonQuery();
            
            return true;
        }
    }
}