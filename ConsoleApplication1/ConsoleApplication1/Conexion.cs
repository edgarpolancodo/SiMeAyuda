using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;


namespace ConsoleApplication1
{
    class Conexion
    {
        SqlConnection conexion;
        public Conexion()
        {
            conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            conexion.Open();
        }
        public void Close()
        {
            conexion.Close();
        }
        public SqlDataReader GetSolicitudesAbiertas() 
        {
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGetSolicitudesAbiertas";
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
    }
}
