using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Collections;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
using System.ComponentModel;

namespace Zenit
{
    public class DataClasses
    {
        static string MyConn="";
        public static void ConnOpen(string cnn)
        {
            if (!string.IsNullOrEmpty(cnn))
            {
                MyConn = cnn;

            }
            else
            {
                MyConn = cnn = System.Configuration.ConfigurationSettings.AppSettings["dbConnection"].ToString();
            }
        }
        public static int Insert<T>(T item)
        {
            int ret = 0;
            string TableName = item.GetType().Name;
            OrderedDictionary objList = fillPorperties(item);
            if (objList.Count > 0)
            {
                String[] myKeys = new String[objList.Count];
                object[] myValues = new object[objList.Count];
                objList.Keys.CopyTo(myKeys, 0);
                objList.Values.CopyTo(myValues, 0);
                string list = string.Join(",", myKeys);
                string paramaters = string.Join(",@", myKeys).Insert(0, "@");
                string query = "insert into " + TableName + " (" + list + ") values (" + paramaters + ") select @@Identity";
                tools t = new tools(MyConn);
                t.Baglanti_Ac();
                SqlCommand cmd = new SqlCommand(query, t.cnn);
                for (int i = 0; i < objList.Count; i++)
                {
                    cmd.Parameters.AddWithValue("@" + myKeys[i], myValues[i]);
                }
                int.TryParse(cmd.ExecuteScalar().ToString(), out ret);
                t.Baglanti_Kapat(cmd);
            }
            return ret;

        }
        public static int Update<T>(T item)
        {
            int ret = 0;
            string TableName = item.GetType().Name;

            OrderedDictionary objList = fillPorperties(item);
            if (objList.Count > 0)
            {
                String[] myKeys = new String[objList.Count];
                object[] myValues = new object[objList.Count];
                objList.Keys.CopyTo(myKeys, 0);
                objList.Values.CopyTo(myValues, 0);

                string command = string.Empty;

                for (int i = 1; i < objList.Count; i++)
                {
                    command += myKeys[i] + " = @" + myKeys[i] + ",";
                }

                string query = "update " + TableName + " set  " + command.TrimEnd(',') + "  where " + myKeys[0] + "=" + myValues[0];
                tools t = new tools(MyConn);
                t.Baglanti_Ac();
                SqlCommand cmd = new SqlCommand(query, t.cnn);

                for (int i = 0; i < objList.Count; i++)
                {
                    cmd.Parameters.AddWithValue("@" + myKeys[i], myValues[i]);
                }

                ret = cmd.ExecuteNonQuery();
                t.Baglanti_Kapat(cmd);

            }

            return ret;
        }
        public static bool Delete<T>(int ID)
        {
            bool ret = false;
            try
            {
                string TableName = typeof(T).Name;
                string query = "declare @columnName nvarchar(MAX) declare @query nvarchar(MAX)set @columnName=( SELECT Top(1)[name]  FROM syscolumns WHERE id = (SELECT id FROM sysobjects WHERE type = 'U' AND [Name] = '" + TableName + "') ORDER BY [colid] asc) exec( 'delete from " + TableName + " where ' + @columnName + '= " + ID + "')";
                tools t = new tools(MyConn);
                t.Baglanti_Ac();
                SqlCommand cmd = new SqlCommand(query, t.cnn);
                cmd.ExecuteNonQuery();
                t.Baglanti_Kapat(cmd);
                ret = true;
            }
            catch (Exception)
            {


            }

            return ret;
        }
        /// <summary>
        /// and yada or kullanılmalıdır.
        /// </summary>
        /// <typeparam name="type"></typeparam>
        /// <param name="type">and yada or kullanılmalıdır.</param>
        /// <returns></returns>
        public static List<T> Search<T>(T item,string type)
        {
            string TableName = item.GetType().Name;
            List<T> List = new List<T>();
            OrderedDictionary objList = fillPorperties(item);
            object[] myValues = new object[objList.Count];
            String[] myKeys = new String[objList.Count];
            objList.Values.CopyTo(myValues, 0);
            objList.Keys.CopyTo(myKeys, 0);
            string command = string.Empty;
            for (int i = 0; i < objList.Count; i++)
            {
                string qq = "";
                if (myValues[i].GetType() == typeof(string))
                    qq = "like'%'+@" + myKeys[i] + "+'%'";
                else if (myValues[i].GetType() == typeof(int))
                    qq = "=@" + myKeys[i];
                else if (myValues[i].GetType() == typeof(bool))
                    qq = "=@" + myKeys[i];
                else if (myValues[i].GetType() == typeof(DateTime))
                    qq = "1=1";

                command += "(" + myKeys[i] + " " + qq + " or " + myKeys[i] + " is null ) "+type+" ";
            }
            if(type=="or")
         command=   command.TrimEnd().TrimEnd('r').TrimEnd('o');
            else  if (type == "and")
                command = command.TrimEnd().TrimEnd('d').TrimEnd('n').TrimEnd('a');
            string query = "select * from " + TableName + " where ";
            query = query + " " + command;
            tools t = new tools(MyConn);
            t.Baglanti_Ac();
            SqlCommand cmd = new SqlCommand(query, t.cnn);
            for (int i = 0; i < objList.Count; i++)
            {
                cmd.Parameters.AddWithValue("@" + myKeys[i], myValues[i]);

            }
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = cmd;
            DataTable dt = new DataTable();
            adp.Fill(dt);
            t.Baglanti_Kapat();
            List = DataClasses.DtFilltoList<T>(dt);
            return List;



        }



        /// <summary>
        /// Verdiğiniz Class a göre tüm datayi List tipinde döner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ID">0 verirseniz hepsini getirir</param>
        /// <returns></returns>
        public static List<T> Select<T>(int ID)
        {
            List<T> List = new List<T>();
            try
            {
                string query = "";
                string TableName = typeof(T).Name;
                if (ID == 0)
                    query = "select * from " + TableName;
                else
                {
                    query="declare @columnName nvarchar(MAX) declare @query nvarchar(MAX)set @columnName=( SELECT Top(1)[name]  FROM syscolumns WHERE id = (SELECT id FROM sysobjects WHERE type = 'U' AND [Name] = '" + TableName + "') ORDER BY [colid] asc) exec( 'select * from " + TableName + " where ' + @columnName + '= " + ID + "')";
                    //query = "declare @columnName nvarchar(MAX) declare @query nvarchar(MAX)set  @columnName=(SELECT Top(1)[name] AS [Column Name] FROM syscolumns WHERE id = (SELECT id FROM sysobjects WHERE type = 'U' AND [Name] = '" + TableName + "')) exec( 'select * from " + TableName + " where ' + @columnName + '= " + ID + "')"; 
                }
                DataTable dt = DatatableData(query);
                List = DataClasses.DtFilltoList<T>(dt);
            }
            catch (Exception)
            {


            }
            return List;
        }
        public static List<T> Select<T>(int ID, bool random)
        {
            List<T> List = new List<T>();
            try
            {
                string query = "";
                string TableName = typeof(T).Name;
                if (ID == 0)
                {
                    if (random)
                        query = "select * from " + TableName + " ORDER BY NEWID()";
                    else
                        query = "select * from " + TableName;
                }
                else
                    query = "declare @columnName nvarchar(MAX) declare @query nvarchar(MAX)set  @columnName=(SELECT Top(1)[name] AS [Column Name] FROM syscolumns WHERE id = (SELECT id FROM sysobjects WHERE type = 'U' AND [Name] = '" + TableName + "')) exec( 'select * from " + TableName + " where ' + @columnName + '= " + ID + "')";
                DataTable dt = DatatableData(query);
                List = DataClasses.DtFilltoList<T>(dt);
            }
            catch (Exception)
            {


            }
            return List;
        }


        public static List<T> SelectWhere<T>(T item)
        {
            string TableName = item.GetType().Name;
            List<T> List = new List<T>();
            OrderedDictionary objList = fillPorperties(item);
            object[] myValues = new object[objList.Count];
            String[] myKeys = new String[objList.Count];
            objList.Values.CopyTo(myValues, 0);
            objList.Keys.CopyTo(myKeys, 0);
            string command = string.Empty;
            for (int i = 0; i < objList.Count; i++)
            {
                string qq = "";
                if (myValues[i].GetType() == typeof(string))
                    qq = "=@" + myKeys[i] + "";
                else if (myValues[i].GetType() == typeof(int))
                    qq = "=@" + myKeys[i];
                else if (myValues[i].GetType() == typeof(bool))
                    qq = "=@" + myKeys[i];
                else if (myValues[i].GetType() == typeof(DateTime))
                    qq = "=@" + myKeys[i] + "";

                command += "" + myKeys[i] + " " + qq +" and ";
            }
            command += "1=1";
            string query = "select * from " + TableName + " where ";
            query = query + " " + command;
           
            tools t = new tools(MyConn);
            t.Baglanti_Ac();
            SqlCommand cmd = new SqlCommand(query, t.cnn);
            for (int i = 0; i < objList.Count; i++)
            {
                cmd.Parameters.AddWithValue("@" + myKeys[i], myValues[i]);

            }
            SqlDataAdapter adp = new SqlDataAdapter();
            adp.SelectCommand = cmd;
            DataTable dt = new DataTable();
            adp.Fill(dt);
            t.Baglanti_Kapat(cmd);
            List = DataClasses.DtFilltoList<T>(dt);
            return List;

        }



        /// <summary>
        ///  her tablonun instance i alındığında propertyler le aynı isimde ve tipte için Datatable oluşturuluyor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>       
        #region Paging
        /// <summary>
        /// Datanın pageSize a göre kaç sayfa olacağını verir 
        /// </summary>
        /// <param name="RowCount">Toplam Data Sayısı</param>
        /// <param name="pageSize">Sayfadaki item sayısı</param>
        /// <returns></returns>
        public static int GetNoPages(double RowCount, double rowLimit)
        {
            // double rowLimit = pageSize;

            try
            {
                double i = RowCount % rowLimit;

                if (i == 0)
                {
                    return (int)(Math.Round(RowCount / rowLimit));
                }
                else
                {
                    if (i > (rowLimit / 2))
                    {
                        return (int)(Math.Round(RowCount / rowLimit));
                    }
                    else
                    {
                        return (int)(Math.Round(RowCount / rowLimit) + 1);
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        /// <summary>
        /// Tablonun ilk colunu ID olmazk zorundadır.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowlimit"></param>
        /// <param name="SayfaLinkleri"></param>
        /// <returns></returns>
        public static List<T> SelectPaging<T>(double rowlimit, out string SayfaLinkleri, out string toplamitem)
        {
            string TableName = typeof(T).Name;
            string clas = "";
            List<T> List = new List<T>();
            int pageSize = 0;
            int.TryParse(HttpContext.Current.Request.QueryString["pager"], out pageSize);
            pageSize = pageSize == 0 ? 1 : pageSize;
            string query = "declare @columnName nvarchar(100)set @columnName='" + TableName + "'+'.'+(SELECT Top(1)[name] AS [Column Name] FROM syscolumns WHERE id = (SELECT id FROM sysobjects WHERE type = 'U' AND [Name] = '" + TableName + "'));WITH paging AS (select ROW_NUMBER() over (order by @columnName desc) as Row, * from ( select  top(10000)* from " + TableName + " order by ID desc )  as Table1 ) SELECT  (select count(*) from paging) TotalCount, * FROM paging WHERE Row between ((" + pageSize + " * " + rowlimit + ") +1- " + rowlimit + ") and (" + pageSize + "* " + rowlimit + ") order by ID desc";
            DataTable dt = DatatableData(query);
            List<T> ListCount = DataClasses.Select<T>(0);
            List = DtFilltoList<T>(dt);
            SayfaLinkleri = "";
            string sPagePath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            //System.IO.FileInfo oFileInfo = new System.IO.FileInfo(sPagePath);
            string[] dizi = sPagePath.Split('/');
            string sPageName = "";
            if(dizi.Length>0)
            sPageName=  dizi[dizi.Length - 1];
            int page = GetNoPages(ListCount.Count, rowlimit);
            toplamitem = ListCount.Count.ToString();
            if (page >= 5)
            {
                int pager = 0;
                int.TryParse(HttpContext.Current.Request.QueryString["pager"], out pager);
                if (pager > 0)
                {

                    if (pager >= 5)
                    {
                   
                        if (pager >= (page-5))
                        {
                            if (pager >= page)
                            {
                                SayfaLinkleri += "<a href='javascript:;' onclick=\"getList('" + sPageName + "?pager=" + 1 + "')\" class='" + clas + "'><<</a>";
                                for (int i = (pager - 5); i <= pager; i++)
                                {
                                    clas = HttpContext.Current.Request.QueryString["pager"] != null && HttpContext.Current.Request.QueryString["pager"] == i.ToString() ? "aktif_page" : "pasif_page";
                                    if (HttpContext.Current.Request.QueryString["pager"] == null)
                                    {
                                        if (i == 1)
                                            clas = "aktif_page";
                                    }
                                    SayfaLinkleri += "<a href='javascript:;' onclick=\"getList('" + sPageName + "?pager=" + i + "')\" class='" + clas + "'>" + i + "</a>";
                                }
                            }
                            else
                            {

                                SayfaLinkleri += "<a href='javascript:;' onclick=\"getList('" + sPageName + "?pager=" + 1 + "')\" class='" + clas + "'><<</a>";
                                for (int i = (pager - 4); i <= pager; i++)
                                {
                                    clas = HttpContext.Current.Request.QueryString["pager"] != null && HttpContext.Current.Request.QueryString["pager"] == i.ToString() ? "aktif_page" : "pasif_page";
                                    if (HttpContext.Current.Request.QueryString["pager"] == null)
                                    {
                                        if (i == 1) 


                                            clas = "aktif_page";
                                    }
                                    SayfaLinkleri += "<a href='javascript:;' onclick=\"getList('" + sPageName + "?pager=" + i + "')\" class='" + clas + "'>" + i + "</a>";

                                }
                                SayfaLinkleri += "<a href='javascript:;' onclick=\"getList('" + sPageName + "?pager=" + page + "')\" class='pasif_page'>>></a>";
                            
                            }
                           
                        }
                        else
                        {
                            SayfaLinkleri += "<a href='javascript:;' onclick=\"getList('" + sPageName + "?pager=" + 1 + "')\" class='" + clas + "'><<</a>";
                            for (int i = (pager - 2); i <= (pager + 2); i++)
                            {
                                clas = HttpContext.Current.Request.QueryString["pager"] != null && HttpContext.Current.Request.QueryString["pager"] == i.ToString() ? "aktif_page" : "pasif_page";
                                if (HttpContext.Current.Request.QueryString["pager"] == null)
                                {
                                    if (i == 1)
                                        clas = "aktif_page";
                                }
                                SayfaLinkleri += "<a href='javascript:;' onclick=\"getList('" + sPageName + "?pager=" + i + "')\" class='" + clas + "'>" + i + "</a>";

                            }
                            SayfaLinkleri += "<a>...</a><a href='javascript:;' onclick=\"getList('" + sPageName + "?pager=" + page + "')\" class='" + clas + "'>>></a>";
                        }
                       
                    }
                    else
                    {
                        for (int i = 1; i <= 5; i++)
                        {
                            clas = HttpContext.Current.Request.QueryString["pager"] != null && HttpContext.Current.Request.QueryString["pager"] == i.ToString() ? "aktif_page" : "pasif_page";
                            if (HttpContext.Current.Request.QueryString["pager"] == null)
                            {
                                if (i == 1)
                                    clas = "aktif_page";
                            }
                            SayfaLinkleri += "<a href='javascript:;' onclick=\"getList('" + sPageName + "?pager=" + i + "')\" class='" + clas + "'>" + i + "</a>";

                        }
                        SayfaLinkleri += "<a>...</a><a href='javascript:;' onclick=\"getList('" + sPageName + "?pager=" + page + "')\" class='" + clas + "'>>></a>";
                    }

                }
            }
            else
            {
                for (int i = 1; i <= page; i++)
                {
                    clas = HttpContext.Current.Request.QueryString["pager"] != null && HttpContext.Current.Request.QueryString["pager"] == i.ToString() ? "aktif_page" : "pasif_page";
                    if (HttpContext.Current.Request.QueryString["pager"] == null)
                    {
                        if (i == 1)
                            clas = "aktif_page";
                    }
                    SayfaLinkleri += "<a href='javascript:;' onclick=\"getList('" + sPageName + "?pager=" + i + "')\" class='" + clas + "'>" + i + "</a>";
                }

            }




            //for (int i = 1; i < page; i++)
            //{
            //    clas = HttpContext.Current.Request.QueryString["pager"] != null && HttpContext.Current.Request.QueryString["pager"] == i.ToString() ? "aktif_page" : "pasif_page";
            //    if (HttpContext.Current.Request.QueryString["pager"] == null)
            //    {
            //        if (i == 1)
            //            clas = "aktif_page";
            //    }
            //    SayfaLinkleri += "<a href='javascript:;' onclick=\"getList('" + sPageName + "?pager=" + i + "')\" class='" + clas + "'>" + i + "</a>";
            //}
            return List;
        }
        #endregion
        /// <summary>
        /// Query e göre DataTable tipinde tablo döner
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static DataTable DatatableData(string query)
        {
            DataTable dt = new DataTable();
            tools t = new tools(MyConn);
            t.Baglanti_Ac();
            SqlDataAdapter cmd = new SqlDataAdapter(query, t.cnn);
            cmd.Fill(dt);
            t.Baglanti_Kapat();
            return dt;
        }
        #region oopMethods
        public static List<T> DtFilltoList<T>(DataTable dt)
        {
            List<T> List = new List<T>();
            T item = Activator.CreateInstance<T>();
            ConstructorInfo constInfo = typeof(T).GetConstructor(new Type[] { typeof(DataRow) });
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                T entity = (T)constInfo.Invoke(new object[] { dt.Rows[i] });
                List.Add(entity);
            }
            return List;
        }
        public static DataTable CreateTable<T>()
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo tp in typeof(T).GetProperties())
            {

                Type t = tp.GetGetMethod().ReturnType;
                dt.Columns.Add(tp.Name, Nullable.GetUnderlyingType(t) ?? t);
            }
            return dt;
        }
        public static DataTable ConvertGenericListToDataTable<T>(IList<T> list)
        {
            DataTable dt = CreateTable<T>();
            Type entityType = typeof(T);

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);
            foreach (T item in list)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyDescriptor property in properties)
                {
                    dr[property.Name] = property.GetValue(item);
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        static OrderedDictionary fillPorperties<T>(T item)
        {
            OrderedDictionary objList = new OrderedDictionary();
            foreach (var tp in item.GetType().GetProperties())
            {
                if (tp.GetValue(item, null) != null)
                {
                    objList.Add(tp.Name, tp.GetValue(item, null));
                }
            }
            return objList;
        }
        public static object GetData<T>(object take)
        {

            T r;
            object test;
            //if (typeof(T) == typeof(int))
            //{
            //    if (!string.IsNullOrEmpty(take.ToString()))
            //        r = (T)take;
            //    else
            //    {
            //        test = 0;
            //        r = (T)test;
            //    }
            //}
            //else if (typeof(T) == typeof(string))
            //{
            //    if (!string.IsNullOrEmpty(take.ToString()))
            //        r = (T)take;
            //    else
            //    {
            //        test = null;
            //        r = (T)test;
            //    }

            //}
            //else if (typeof(T) == typeof(bool))
            //{
            //    if (!string.IsNullOrEmpty(take.ToString()))
            //        r = (T)take;
            //    else
            //    {
            //        test = false;
            //        r = (T)test;
            //    }

            //}
            //else if (typeof(T) == typeof(DateTime))
            //{
            //    if (!string.IsNullOrEmpty(take.ToString()))
            //        r = (T)take;
            //    else
            //    {
            //        test = DateTime.Now;
            //        r = (T)test;
            //    }

            //}
            //else
            //{
            //    test = null;
            //    r = (T)test;
            //}
            //return r;
            try
            {


                if (take != null)
                    r = (T)take;
                else
                {

                    r = Activator.CreateInstance<T>();
                    if (typeof(T) == typeof(DateTime))
                    {
                        test = DateTime.Now;
                        r = (T)test;
                    }
                }
                return r;

            }
            catch
            {
                return null;
            }


        }
        #endregion
    }


}