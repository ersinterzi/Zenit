using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace Zenit
{
    public class tools
    {
        //public static SqlConnection cnn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["strConn"].ToString());
        public tools()
        {
        }
        public tools(string MyConn)
        {
            if (!string.IsNullOrEmpty(MyConn))
            {

                cnn.ConnectionString = MyConn;
            }
        }
        //Dışardan Okumak için
        public SqlConnection cnn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["dbConnection"].ToString());

        //Şuan için direk erişim
        // public static SqlConnection cnn = new SqlConnection(@"User ID=serv_user;Password=smartis45;Initial Catalog=ElaResort;Data Source=delta3\sql08r2express");

        #region Baglanti Aç ve Kapat
        public void Baglanti_Ac()
        {
             

            if (cnn == null)
            {
                cnn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["dbConnection"].ToString());
            }
            if (cnn.State == System.Data.ConnectionState.Broken || cnn.State == System.Data.ConnectionState.Closed)
            {
                
                cnn.Open();
            }


        }

        public void Baglanti_Kapat(SqlCommand cmd)
        {
            cmd.Cancel();
            cmd.Dispose();
            cnn.Close();
            cnn.Dispose();



        }
        public void Baglanti_Kapat()
        {
            cnn.Close();
            cnn.Dispose();

        }
        #endregion


    }
}