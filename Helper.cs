using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;

/// <summary>
/// Summary description for Helper
/// </summary>
/// 
namespace Zenit
{
    public class Helper
    {
        private static string passPhrase = "5iFrepr@se";           //' can be any string
        private static string saltValue = "s@1tMartiz";            //' can be any string
        private static string hashAlgorithm = "SHA1";              //' can be "MD5"
        private static int passwordIterations = 2;              //' can be any number
        private static string initVector = "@0A1b4C5E6f3G2h9";     //' must be 16 bytes
        private static int keySize = 256;                       //' can be 192 or 128
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tarih"></param>
        /// <param name="tip">tr >> dd/mm/yyyy , en >> mm/dd/yyyy</param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static string TarihForSql(string tarih, string tip, char splitter)
        {
            if (tarih.Split(splitter).Length == 3)
            {
                int gun, ay, yil;
                string sgun, say, syil;

                try
                {
                    gun = int.Parse(tarih.Split(splitter)[0]);
                    if (gun < 10) sgun = "0" + gun.ToString();
                    else sgun = gun.ToString();
                }
                catch { sgun = tarih.Split(splitter)[0]; }
                try
                {
                    ay = int.Parse(tarih.Split(splitter)[1]);
                    if (ay < 10) say = "0" + ay.ToString();
                    else say = ay.ToString();
                }
                catch { say = tarih.Split(splitter)[1]; }
                try
                {
                    yil = int.Parse(tarih.Split(splitter)[2]);
                    if (yil < 10) syil = "0" + yil.ToString();
                    else syil = yil.ToString();
                }
                catch { syil = tarih.Split(splitter)[2]; }

                switch (tip)
                {
                    case "tr":
                        tarih = syil + "-" + say + "-" + sgun;
                        break;
                    case "en":
                        tarih = syil + "-" + sgun + "-" + say;
                        break;
                }
            }
            else
                tarih = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();

            return tarih;
        }

        public Helper()
        {
            //
            // TODO: Add constructor logic here
            //
            System.Web.UI.Page p = HttpContext.Current.Handler as System.Web.UI.Page;
        }

        public static string TarihFormat()
        {
            string gun = DateTime.Now.Day.ToString();
            string ay = DateTime.Now.Month.ToString();
            string yil = DateTime.Now.Year.ToString();
            string saat = DateTime.Now.ToShortTimeString();

            return gun + "/" + ay + "/" + yil + " " + saat;
        }
        //ADMIN FUNCTIONS

        public static bool CheckAdmin()
        {
            if (HttpContext.Current.Session["adminid"] == null)
                return false;
            else
                return true;
        }
        public static bool CheckAdminType()
        {
            if (HttpContext.Current.Session["admintip"] != null)
            {
                if (HttpContext.Current.Session["admintip"].ToString() == "adm")
                    return true;
                else if (HttpContext.Current.Session["admintip"].ToString() == "affil")
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public static bool OtelEditOk(string otelid)
        {
            if (HttpContext.Current.Session["adminid"].ToString() == otelid)
                return true;
            else
                return false;
        }
        public static double DateDiff(string howtocompare, System.DateTime startDate, System.DateTime endDate)
        {
            double diff = 0;
            try
            {
                System.TimeSpan TS = new
                    System.TimeSpan(startDate.Ticks - endDate.Ticks);
                #region converstion options
                switch (howtocompare.ToLower())
                {
                    case "m":
                        diff = Convert.ToDouble(TS.TotalMinutes);
                        break;
                    case "s":
                        diff = Convert.ToDouble(TS.TotalSeconds);
                        break;
                    case "t":
                        diff = Convert.ToDouble(TS.Ticks);
                        break;
                    case "mm":
                        diff = Convert.ToDouble(TS.TotalMilliseconds);
                        break;
                    case "yyyy":
                        diff = Convert.ToDouble(TS.TotalDays / 365);
                        break;
                    case "q":
                        diff = Convert.ToDouble((TS.TotalDays / 365) / 4);
                        break;
                    default:
                        //d
                        diff = Convert.ToDouble(TS.TotalDays);
                        break;
                }
                #endregion
            }
            catch
            {
                diff = -1;
            }
            return diff;
        }

        public static string SonrakiTarih(int yil, int ay, int gun)
        {
            gun++;

            if (SonrakiAyaGec(ay, gun, (yil % 4 == 0)))
            {
                gun = 1;
                ay++;
                if (ay > 12)
                {
                    ay = 1;
                    yil++;
                }
            }

            return gun.ToString() + "/" + ay.ToString() + "/" + yil.ToString();
        }
        public static bool SonrakiAyaGec(int ay, int gun, bool artikyil)
        {
            bool sonuc = false;
            switch (ay)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    if (gun > 31) sonuc = true;
                    break;
                case 2:
                    if (artikyil)
                    { if (gun > 29) sonuc = true; }
                    else
                    { if (gun > 28) sonuc = true; }
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    if (gun > 30) sonuc = true;
                    break;
            }
            return sonuc;
        }
        public static string TarihMDY(string tarih)
        {
            return tarih.Split('/')[1] + "/" + tarih.Split('/')[0] + "/" + tarih.Split('/')[2];
        }

        public static string Doviz(string tip)
        {
            switch (tip)
            {
                case "YTL":
                    tip = "Y";
                    break;
                case "USD":
                    tip = "$";
                    break;
                case "EUR":
                    tip = "€";
                    break;
            }
            return tip;
        }

        public static bool HttpsOn()
        {
            return (HttpContext.Current.Request.ServerVariables["HTTPS"] == "on");
        }
        //------------------------------------------------------------

        public static string SqlTemiz(string kelime)
        {
            if (kelime == null)
                return "";

            kelime = kelime.Replace("'", "");
            kelime = kelime.ToLower().Replace("delete", "");
            kelime = kelime.ToLower().Replace("insert", "");
            kelime = kelime.ToLower().Replace("update", "");
            kelime = kelime.ToLower().Replace("exec", "");
            kelime = kelime.ToLower().Replace("declare", "");
            kelime = kelime.ToLower().Replace("cursor", "");

            return kelime;
        }

        public static string FormatOku(string klasor, string dosya)
        {
            StreamReader tr;

            tr = new StreamReader(HttpContext.Current.Server.MapPath(klasor) + "\\" + dosya, System.Text.Encoding.Default);
            dosya = tr.ReadToEnd();
            tr.Close();
            return dosya;
        }

        public static void DropDol(bool temizle, string seciniz, string secinizval, string secili, DropDownList drp, DataView dv,
        string txt1, string val, string txt2)
        {
            ListItem item = null;
            try
            {
                if (temizle) drp.Items.Clear();

                if (seciniz != "")
                {
                    item = new ListItem();
                    item.Text = seciniz;
                    item.Value = secinizval;
                    drp.Items.Add(item);
                }
                if (dv != null && dv.Count > 0)
                {
                    for (int i = 0; i < dv.Count; i++)
                    {
                        item = new ListItem();
                        item.Text = dv[i][txt1].ToString() + (txt2 != "" ? " - " + dv[i][txt2].ToString() : "");
                        item.Value = dv[i][val].ToString();
                        drp.Items.Add(item);
                    }
                    item = null;
                    item = drp.Items.FindByValue(secili);
                    if (item != null)
                        item.Selected = true;
                }
            }
            catch { }
        }

        public static void SonucGoster(GridView gwMain, DataGrid dgMain, DataView dvGen, Label lblSonuc)
        {
            if (gwMain != null)
                lblSonuc.Text = (gwMain.Rows.Count == 0 ? "Kayýt bulunamadý." : "Toplam " + dvGen.Count.ToString() + " kayýt.");
            else if (dgMain != null)
                lblSonuc.Text = (dvGen.Count == 0 ? "Kayýt bulunamadý." : "Toplam " + dvGen.Count.ToString() + " kayýt.");

        }

        public static string KlasorAdi(string klasor)
        {
            klasor = klasor.ToLower()
                    .Replace(" ", "")
                    .Replace("ý", "i")
                    .Replace("ö", "o")
                    .Replace("ü", "u")
                    .Replace("þ", "s")
                    .Replace("ç", "c")
                    .Replace("ð", "g")
                    .Replace(".", "")
                    .Replace("/", "")
                    .Replace("\\", "")
                    .Replace(":", "")
                    .Replace("*", "")
                    .Replace("?", "")
                    .Replace("\"", "")
                    .Replace("<", "")
                    .Replace(">", "")
                    .Replace("|", "")
                    .Replace("´", "")
                    .Replace("'", "")
                    .Replace("&", "");

            return klasor;
        }

        public static string DosyaAdi(string dosya)
        {
            dosya = dosya.ToLower()
                    .Replace(" ", "")
                    .Replace("ý", "i")
                    .Replace("ö", "o")
                    .Replace("ü", "u")
                    .Replace("þ", "s")
                    .Replace("ç", "c")
                    .Replace("ð", "g")
                    .Replace("/", "")
                    .Replace("\\", "")
                    .Replace(":", "")
                    .Replace("*", "")
                    .Replace("?", "")
                    .Replace("\"", "")
                    .Replace("<", "")
                    .Replace(">", "")
                    .Replace("|", "")
                    .Replace("´", "")
                    .Replace("'", "")
                    .Replace("&", "")
                    .Replace(";", "")
                    .Replace("+", "");

            return dosya;
        }

        public static string GunKisaAd(string gun)
        {
            switch (gun)
            {
                case "Monday":
                    gun = "Mon";
                    break;
                case "Tuesday":
                    gun = "Tue";
                    break;
                case "Wednesday":
                    gun = "Wed";
                    break;
                case "Thursday":
                    gun = "Thu";
                    break;
                case "Friday":
                    gun = "Fri";
                    break;
                case "Saturday":
                    gun = "Sat";
                    break;
                case "Sunday":
                    gun = "Sun";
                    break;
            }
            return gun;
        }

        public static string AyAd(int ay, int gun, int yil, bool kisa, string dil, bool gunekle)
        {
            string[] kisaay = new string[] { "Oca", "Þub", "Mar", "Nis", "May", "Haz", "Tem", "Aðu", "Eyl", "Eki", "Kas", "Ara" };
            string[] uzunay = new string[] { "Ocak", "Þubat", "Mart", "Nisan", "Mayýs", "Haziran", "Temmuz", "Aðustos", "Eylül", "Ekim", "Kasým", "Aralýk" };
            string[] kisaayeng = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            string[] uzunayeng = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            string ek = " " + HidOnEk(gun) + gun.ToString() + ", " + yil;
            string don = kisa ? kisaay[ay - 1] : uzunay[ay - 1];

            //if(dil == "tr")
            //    don = kisa ? kisaay[ay - 1] : uzunay[ay - 1];
            if (dil == "eng")
                don = kisa ? kisaayeng[ay - 1] : uzunayeng[ay - 1];

            return don + (gunekle ? ek : "");
        }

        public static string YkrSifirEkle(string fiyat)
        {
            //string tf = fiyat;
            try
            {
                fiyat = String.Format("{0:#,##0.00}", double.Parse(fiyat));
            }
            catch
            { }
            return fiyat;
        }

        public static void PrepHidReqForm(HtmlInputHidden inpHid, string _name, string _value, Panel _pn)
        {
            inpHid = new HtmlInputHidden();
            inpHid.Name = _name;
            inpHid.Value = _value;
            _pn.Controls.Add(inpHid);
        }

        public static string HidOnEk(int sayi)
        {
            return sayi < 10 ? "0" : "";
        }

        //XML
        public static string[] TlKarsiligiFromMerkez()
        {
            string[] kurs = new string[4];

            if (HttpContext.Current.Session["usdal"] == null)
            {
                //'Dim kod, tur As String
                XmlTextReader rdr = new XmlTextReader("http://www.tcmb.gov.tr/kurlar/today.xml");
                XmlDocument MyXml = new XmlDocument();
                XmlNodeList MyNode;

                //'kod = kodu.ToString()
                //'tur = turu.ToString()
                //'Deðiþkenlerimizi stringe çeviriyoruz.

                try
                {
                    MyXml.Load(rdr);
                    //' internetteki xml belgesini uygulamamýza yüklüyoruz.
                    //MyNode = MyXml.SelectNodes("/Tarih_Date/Currency[@Kod ='USD']/ForexSelling");
                    //HttpContext.Current.Session["usdsat"] = MyNode.Item(0).InnerXml.ToString().Replace(".", ",");
                    //MyNode = MyXml.SelectNodes("/Tarih_Date/Currency[@Kod ='EUR']/ForexSelling");
                    //HttpContext.Current.Session["eursat"] = MyNode.Item(0).InnerXml.ToString().Replace(".", ",");
                    MyNode = MyXml.SelectNodes("/Tarih_Date/Currency[@Kod ='USD']/ForexBuying");
                    HttpContext.Current.Session["usdal"] = MyNode.Item(0).InnerXml.ToString().Replace(".", ",");
                    MyNode = MyXml.SelectNodes("/Tarih_Date/Currency[@Kod ='EUR']/ForexBuying");
                    HttpContext.Current.Session["eural"] = MyNode.Item(0).InnerXml.ToString().Replace(".", ",");
                }
                catch { }


            }
            kurs[0] = HttpContext.Current.Session["usdal"].ToString();
            kurs[1] = HttpContext.Current.Session["eural"].ToString();
            //kurs(1) = HttpContext.Current.Session("eursat");
            //kurs(2) = HttpContext.Current.Session("usdsat");


            return kurs;
            //'seçilen node un verisi stringe çevrilip geri döndürülür.

        }

        //captcha
        public static string GenerateRandomCode()
        {
            Random r = new Random();
            string s = "";
            for (int i = 0; i < 6; i++)
                s = String.Concat(s, r.Next(10).ToString());
            return s;
        }

        public static string KarakterSinirlama(string kelime, int karakter)
        {
            if (kelime.Length > karakter) return kelime.Substring(karakter);

            return kelime;
        }




        public static int ToInt(string _val, int _donus)
        {
            if (_val == null)
                return _donus;

            int tmp = 0;

            if (int.TryParse(_val, out tmp))
                return int.Parse(_val);

            return _donus;
        }
        public static double ToDouble(string _val, double _donus)
        {
            if (_val == null)
                return _donus;

            double tmp = 0;

            if (double.TryParse(_val, out tmp))
                return double.Parse(_val);

            return _donus;
        }

        public static string NullToStr(object _val, string _donus)
        {
            if (_val != null)
                return _val.ToString();

            return _donus;
        }
        public static double PercentFiyat(string _val, int _oran, double _donus)
        {
            if (_val == null)
                return _donus;

            if (double.TryParse(_val, out _donus))
                return double.Parse(_val) - (double.Parse(_val) * _oran / 100);

            return _donus;
        }
        public static string ParseUrl()
        {
            string _url = HttpContext.Current.Request.ServerVariables["URL"];

            return _url.Split('/')[_url.Split('/').Length - 2];

        }
        public static string TerTemizURL(string adi, params string[] _vals)
        {
            //_vals[0] > replacement for " "

            string strTitle = adi.ToString();
            #region Generate SEO Friendly URL based on Title
            //Trim Start and End Spaces.
            strTitle = strTitle.Trim();

            //Trim "-" Hyphen
            strTitle = strTitle.Trim('-');


            strTitle = strTitle.Replace("ç", "c"); strTitle = strTitle.Replace("ð", "g");
            strTitle = strTitle.Replace("ý", "i"); strTitle = strTitle.Replace("ö", "o");
            strTitle = strTitle.Replace("þ", "s"); strTitle = strTitle.Replace("ü", "u");
            strTitle = strTitle.Replace("\"", "-"); strTitle = strTitle.Replace("/", "-");
            strTitle = strTitle.Replace("(", ""); strTitle = strTitle.Replace(")", "");
            strTitle = strTitle.Replace("{", ""); strTitle = strTitle.Replace("}", "");
            strTitle = strTitle.Replace("%", ""); strTitle = strTitle.Replace("&", "");
            strTitle = strTitle.Replace("+", ""); strTitle = strTitle.Replace(".", "-");
            strTitle = strTitle.Replace("?", ""); strTitle = strTitle.Replace(",", "");
            strTitle = strTitle.Replace("Ý", "i"); strTitle = strTitle.Replace("’", "");
            strTitle = strTitle.Replace("'", "");
            strTitle = strTitle.Replace("I", "i");
            strTitle = strTitle.Replace("ž", "-");

            char[] chars = @"$%#@!*?;:~`+=()[]{}|\'<>,/^&"".".ToCharArray();
            strTitle = strTitle.Replace("c#", "C-Sharp");
            strTitle = strTitle.Replace("vb.net", "VB-Net");
            strTitle = strTitle.Replace("asp.net", "Asp-Net");

            //Replace . with - hyphen
            strTitle = strTitle.Replace(".", "-");

            //Replace Special-Characters
            for (int i = 0; i < chars.Length; i++)
            {
                string strChar = chars.GetValue(i).ToString();
                if (strTitle.Contains(strChar))
                {
                    strTitle = strTitle.Replace(strChar, string.Empty);
                }
            }
            strTitle = strTitle.ToLowerInvariant();
            //Replace all spaces with one "-" hyphen
            strTitle = strTitle.Replace(" ", _vals != null && _vals.Length > 0 ? _vals[0] : "-");

            //Replace multiple "-" hyphen with single "-" hyphen.
            strTitle = strTitle.Replace("--", "-");
            strTitle = strTitle.Replace("---", "-");
            strTitle = strTitle.Replace("----", "-");
            strTitle = strTitle.Replace("-----", "-");
            strTitle = strTitle.Replace("----", "-");
            strTitle = strTitle.Replace("---", "-");
            strTitle = strTitle.Replace("--", "-");


            strTitle = strTitle.Replace("ç", "c"); strTitle = strTitle.Replace("ð", "g");
            strTitle = strTitle.Replace("ý", "i"); strTitle = strTitle.Replace("ö", "o");
            strTitle = strTitle.Replace("þ", "s"); strTitle = strTitle.Replace("ü", "u");
            strTitle = strTitle.Replace("\"", "-"); strTitle = strTitle.Replace("/", "-");
            strTitle = strTitle.Replace("(", ""); strTitle = strTitle.Replace(")", "");
            strTitle = strTitle.Replace("{", ""); strTitle = strTitle.Replace("}", "");
            strTitle = strTitle.Replace("%", ""); strTitle = strTitle.Replace("&", "");
            strTitle = strTitle.Replace("+", ""); strTitle = strTitle.Replace(".", "-");
            strTitle = strTitle.Replace("?", ""); strTitle = strTitle.Replace(",", "");
            strTitle = strTitle.Replace("Ý", "i");
            strTitle = strTitle.Replace("I", "i");

            //Run the code again...
            //Trim Start and End Spaces.
            strTitle = strTitle.Trim();

            //Trim "-" Hyphen
            strTitle = strTitle.Trim('-');
            #endregion

            return strTitle;
        }
        public static string KarakterSinirlama(string kelime, int karakter, params string[] _vals)
        {
            if (kelime.Length > karakter) return kelime.Substring(0, karakter) + (_vals != null && _vals.Length > 0 ? _vals[0] : kelime);

            return kelime;
        }
    }
}