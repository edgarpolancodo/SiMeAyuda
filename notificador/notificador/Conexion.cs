using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;

namespace WindowsFormsApplication1
{
    class Conexion
    {
        SqlConnection conexion;
        public Conexion()
        {
            conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            bool conectado_ = false;
            while (!conectado_)
            {
                try
                {
                    conexion.Open();
                    conectado_ = true;
                }
                catch (Exception ex)
                {
                    System.Threading.Thread.Sleep(300000);
                    Console.WriteLine(ex.Message.ToString());
                    Console.Write(ex.StackTrace.ToString());
                }
            }
        }
        public void Close()
        {
            conexion.Close();
        }
        public SqlDataReader GetNotificacion(string usuario) 
        {
            SqlCommand comando = new SqlCommand();
            comando.CommandType = System.Data.CommandType.StoredProcedure;
            comando.Connection = conexion;
            comando.CommandText = "ProcGetSolicitudesByUsuarioTecnicoForNotificar";
            comando.Parameters.Add(new SqlParameter("@usuario", usuario));
            SqlDataReader resultado = comando.ExecuteReader();
            return resultado;
        }
    }
}
