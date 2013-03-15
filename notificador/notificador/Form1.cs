using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.DirectoryServices;
using System.Security.Principal;
using System.Data.SqlClient;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            linkLabel1.Links.Add(0, linkLabel1.Text.Length, System.Configuration.ConfigurationManager.AppSettings["SiMeAyudaURL"].ToString());
            notificacion();
            backgroundWorker1.RunWorkerAsync();
            
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
        private void notificacion() 
        {
            string usuario = "";
            if (WindowsIdentity.GetCurrent().Name.Split('\\')[0] == "SEEPYD" || WindowsIdentity.GetCurrent().Name.Split('\\')[0] == "ECONOMIA")
            {
                usuario = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
            }
            else 
            {
                usuario = System.Configuration.ConfigurationManager.AppSettings["usuario"].ToString();
            }
            Conexion con = new Conexion();
            SqlDataReader data = con.GetNotificacion(usuario);
            if (data.Read())
            {
                label4.Text = data["Nombre"].ToString();
                label5.Text = data["Asignadas"].ToString();
                label6.Text = data["Pendientes"].ToString();
                if (Convert.ToInt32(data["Asignadas"]) > 0)
                {
                    MessageBox.Show("Tiene solicitude(s) asignada(s). Favor de trabajar","Notificador de SiMeAyuda");
                    //this.Show();
                }
                notifyIcon1.Text = String.Format("Tiene {0} solicitudes asignadas\nTiene {1} solicitudes pendientes", data["Asignadas"].ToString(), data["Pendientes"].ToString());
                //data.Read();
                toolStripStatusLabel1.Text = "Actualizado el: " + DateTime.Now.ToString();
                con.Close();
            }
            else 
            {
                toolStripStatusLabel1.Text = "No hay registro con su usuario.";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true) 
            {
                System.Threading.Thread.Sleep(300000);
                notificacion();
            }
        }
    }
}
