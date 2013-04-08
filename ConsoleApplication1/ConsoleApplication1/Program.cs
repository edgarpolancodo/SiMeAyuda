using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Conexion con = new Conexion(); Conexion con2 = new Conexion();
            SqlDataReader solicitudes = con.GetSolicitudesAbiertas();
            SqlDataReader super = con2.GetAllUsuariosSupervisores();
            List<String> supervisores = new List<string>();
            while(super.Read()){
                supervisores.Add(super["CorreoElectronico"].ToString());
            }
            con2.Close();
            while (solicitudes.Read()) 
            {
                if (solicitudes["Nombre"].ToString() == "Validado")
                {
                    foreach (string sup in supervisores)
                    {
                        new Mensajes().EnviarMensaje(sup.ToString(), "Solicitud pendiente para cerrar", String.Format("Esta pendiente para cerrar la solicitud {0}. Favor de proceder y cerrarla.", solicitudes["ID"].ToString()));
                    }
                }
                else if (solicitudes["Nombre"].ToString() == "Resuelto") 
                {
                    new Mensajes().EnviarMensaje(solicitudes["CorreoElectronico"].ToString(), "Solicitud pendiente para validar", String.Format("Esta pendiente para validar la solicitud <b>{0}</b> con el asunto de {1}. En caso de que su solicitud haya sido resuelta favor especificar su nivel de satisfaccion haciendo click:<br/><a href='http://dtic017/MesaDeAyuda/Solicitud/Validado/{0}/5'>Muy satisfecho</a>, <a href='http://dtic017/MesaDeAyuda/Solicitud/Validado/{0}/4'> Satisfecho</a>, <a href='http://dtic017/MesaDeAyuda/Solicitud/Validado/{0}/3'> Indiferente</a>, <a href='http://dtic017/MesaDeAyuda/Solicitud/Validado/{0}/2'> Insatisfecho</a>, <a href='http://dtic017/MesaDeAyuda/Solicitud/Validado/{0}/1'> Muy Insatisfecho</a><br />En caso de que su solicitud no se haya resuelto <a href='http://dtic017/MesaDeAyuda/Solicitud/NoValido/{0}'>click aqui</a><br />Nota: Para ingresar al sistema debe de estar dentro de la red alambrica de MEPYD. No podrá ingresar a traves de Wi-Fi", solicitudes["ID"].ToString(), solicitudes["Descripcion"].ToString()));
                }
                else if (solicitudes["Nombre"].ToString() == "En desarrollo")
                {
                    new Mensajes().EnviarMensaje(solicitudes["CorreoElectronicoTecnico"].ToString(), "Solicitud aún en desarrollo", String.Format("Esta pendiente para resolver la solicitud {0}. Esto es solo una notificación, cuando termine con la solicitud favor marcarla como Resuelta.", solicitudes["ID"].ToString()));
                }
                else if (solicitudes["Nombre"].ToString() == "Asignado")
                {
                    new Mensajes().EnviarMensaje(solicitudes["CorreoElectronicoTecnico"].ToString(), "Solicitud pendiente de trabajar", String.Format("Esta pendiente para trabajar la solicitud {0}. Favor de proceder y marcar el estado En Desarrollo.", solicitudes["ID"].ToString()));
                }
                else if (solicitudes["Nombre"].ToString() == "Ingresado")
                {
                    foreach (string sup in supervisores)
                    {
                        new Mensajes().EnviarMensaje(sup, "Solicitud pendiente para asignar", String.Format("Esta pendiente para asignar a usuario tecnico la solicitud {0}. Favor de proceder y asignarlo a usuario tecnico.", solicitudes["ID"].ToString()));
                    }
                }
            }
        }
    }
}
