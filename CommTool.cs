using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;


namespace JDB
{
    public class CommTool : System.Web.UI.Page
    {

        public string ver = "1.0.170608.1";

        public bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        public bool IsDate(string sdate)
        {
            DateTime dt;
            bool isDate = true;

            try
            {
                dt = DateTime.Parse(sdate);
            }
            catch
            {
                isDate = false;
            }
            return isDate;
        }

        public int Cint2(object s)
        {
            int tmp = 0;
            try
            {
                if (s != null)
                {
                    tmp = Convert.ToInt32(s);
                }
            }
            catch { }
            finally
            {
            }

            return tmp;
        }

        public int Cint2(object s, int defaultValue)
        {
            int tmp = defaultValue;
            try
            {
                if (s != null)
                {
                    tmp = Convert.ToInt32(s);
                }
            }
            catch { }
            finally
            {
            }

            return tmp;
        }

        public Int64 CInt64(object s, Int64 defaultvalue = 0)
        {
            Int64 tmp = defaultvalue;
            try
            {
                if (s != null)
                {
                    tmp = Int64.Parse(s.ToString());
                }
            }
            catch { }
            return tmp;
        }
        
        public string GetFullNum(int num, int size)
        {
            //補0
            string tmp = num.ToString();
            if (size >= tmp.Length)
            {
                while (!(tmp.Length == size))
                {
                    tmp = "0" + tmp;
                }
            }
            return tmp;
        }

        public string Config(string Key)
        {
            string Value = "";
            if (Key != null)
            {
                Value = ConfigurationSettings.AppSettings.Get(Key);
            }
            return Value;
        }

        public bool GetDDL(DropDownList DDL, string strDT)
        {
            DDL.SelectedIndex = -1;
            //解除全部
            if (strDT != null)
            {
                for (int i = 0; i <= DDL.Items.Count - 1; i++)
                {
                    if (DDL.Items[i].Value == Convert.ToString(strDT))
                    {
                        DDL.Items[i].Selected = true;
                        break; // TODO: might not be correct. Was : Exit For
                        //防止重複
                    }
                }
            }
            return true;
        }

        public bool GetDDLtext(DropDownList DDL, string strDT)
        {
            DDL.SelectedIndex = -1;
            //解除全部
            if (strDT != null)
            {
                for (int i = 0; i <= DDL.Items.Count - 1; i++)
                {
                    if (DDL.Items[i].Text.Trim() == strDT.Trim())
                    {
                        DDL.Items[i].Selected = true;
                        break; // TODO: might not be correct. Was : Exit For
                        //防止重複
                    }
                }
            }
            return true;
        }

        public bool GetDDL(RadioButtonList RBL, string strDT)
        {
            RBL.SelectedIndex = -1;
            if (strDT != null)
            {
                for (int i = 0; i <= RBL.Items.Count - 1; i++)
                {
                    if (RBL.Items[i].Value == Convert.ToString(strDT))
                    {
                        RBL.Items[i].Selected = true;
                        break; // TODO: might not be correct. Was : Exit For
                        //防止重複
                    }
                }
            }
            return true;
        }

        public bool GetDDLtext(RadioButtonList RBL, string strDT)
        {
            RBL.SelectedIndex = -1;
            if (strDT != null)
            {
                for (int i = 0; i <= RBL.Items.Count - 1; i++)
                {
                    if (RBL.Items[i].Text == Convert.ToString(strDT))
                    {
                        RBL.Items[i].Selected = true;
                        break; // TODO: might not be correct. Was : Exit For
                        //防止重複
                    }
                }
            }
            return true;
        }

        public bool GetDDL(CheckBoxList CBL, string strDT)
        {
            //CBL.SelectedIndex = -1  '複選型的 不用解除
            if (strDT != null)
            {
                for (int i = 0; i <= CBL.Items.Count - 1; i++)
                {
                    if (CBL.Items[i].Value == Convert.ToString(strDT))
                    {
                        CBL.Items[i].Selected = true;
                    }
                }
            }
            return true;
        }

        public bool GetDDLtext(CheckBoxList CBL, string strDT)
        {
            //CBL.SelectedIndex = -1
            if (strDT != null)
            {
                for (int i = 0; i <= CBL.Items.Count - 1; i++)
                {
                    if (CBL.Items[i].Text == Convert.ToString(strDT))
                    {
                        CBL.Items[i].Selected = true;
                    }
                }
            }
            return true;
        }

        public string ClearHTML(string inStr)
        {
            while (inStr.IndexOf("[") > 0)
            {
                inStr = inStr.Substring(0, inStr.IndexOf("[")) + inStr.Substring(inStr.IndexOf("]") + 1);
            }
            return inStr;
        }

        public bool ChkType(HttpPostedFile xfile, string xType)
        {
            //下方應用
            string tmp = xfile.ContentType;
            if (tmp.IndexOf(xType) > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string SaveBmp(HttpPostedFile afile, int boxwidth, int boxheight, string Img)
        {
           
            string MainPath = Server.MapPath("/");
            string FilePath = System.Configuration.ConfigurationSettings.AppSettings.Get("FilePath");



            double scale = 0;
            string filename = "";
            string file2 = "";
            DateTime y = DateTime.Now;
            string tmp = "";
            string result = "";
            if (ChkType(afile, "image") == false)
            {
                return "";
                //return functionReturnValue;
            }
            tmp = (DateTime.Now.Year - 1911) + DateTime.Now.Month + DateTime.Now.Day + y.TimeOfDay.ToString().Replace(":", "").Substring(0, 6);

            if (afile.ContentLength != 0)
            {
                filename = MainPath + FilePath + Img + tmp + Path.GetExtension(afile.FileName);
                file2 = MainPath + FilePath + Img + tmp + "_s" + Path.GetExtension(afile.FileName);
                afile.SaveAs(filename);
                result = FilePath + Img + tmp + "_s" + Path.GetExtension(afile.FileName);
            }

            Bitmap inbmp = new Bitmap(filename);
            if (inbmp.Height < boxheight & inbmp.Width < boxwidth)
            {
                scale = 1;
            }
            else
            {
                if ((inbmp.Width * boxheight / inbmp.Height < boxwidth))
                {
                    scale = (boxheight) / inbmp.Height;
                }
                else
                {
                    scale = (boxwidth) / inbmp.Width;
                }
            }
            int newwidth = Convert.ToInt32(scale * inbmp.Width);
            int newheight = Convert.ToInt32(scale * inbmp.Height);
            Bitmap outbmp = new Bitmap(inbmp, newwidth, newheight);
            outbmp.Save(file2);
            outbmp.Dispose();

            //活動剪影才用
            if (Img == "Photo\\")
            {
                //做大圖
                boxwidth = 600;
                boxheight = 450;
                if (inbmp.Height < boxheight & inbmp.Width < boxwidth)
                {
                    scale = 1;
                }
                else
                {
                    if ((inbmp.Width * boxheight / inbmp.Height < boxwidth))
                    {
                        scale = (boxheight) / inbmp.Height;
                    }
                    else
                    {
                        scale = (boxwidth) / inbmp.Width;
                    }
                }
                newwidth = Convert.ToInt32(scale * inbmp.Width);
                newheight = Convert.ToInt32(scale * inbmp.Height);
                outbmp = new Bitmap(inbmp, newwidth, newheight);
                outbmp.Save(file2.Replace("_s", "_l"));
                outbmp.Dispose();
            }

            inbmp.Dispose();
            //最後再關
            return result;
            //return functionReturnValue;
        }

        public void DrawCenter(ref Graphics G, string inputString, Font FNT, Brush Bru, int leftX, int TopY, int width, int height)
        {
            PointF b1 = this.CenterString(G, inputString, FNT, leftX, TopY, width, height);
            G.DrawString(inputString, FNT, Bru, b1);
        }

        public PointF CenterString(Graphics G, string inputString, Font FNT, int leftX, int TopY, int width, int height)
        {
            PointF aa = new PointF(0, 0);
            SizeF SS = G.MeasureString(inputString, FNT);
            aa.X = leftX + (width / 2) - (SS.Width / 2);
            aa.Y = TopY + (height / 2) - (SS.Height / 2);
            return aa;
        }
          
        public string CNUM(char a)
        {
            string tmp = "";
            switch (Convert.ToString(a))
            {
                case "1":
                    tmp = "壹";
                    break;
                case "2":
                    tmp = "貳";
                    break;
                case "3":
                    tmp = "參";
                    break;
                case "4":
                    tmp = "肆";
                    break;
                case "5":
                    tmp = "伍";
                    break;
                case "6":
                    tmp = "陸";
                    break;
                case "7":
                    tmp = "柒";
                    break;
                case "8":
                    tmp = "捌";
                    break;
                case "9":
                    tmp = "玖";
                    break;
                case "0":
                    tmp = "";

                    break;
            }
            return tmp;
        }

        public bool InItems(string ItemValue, CheckBoxList LIS)
        {
            bool AA = false;
            for (int k = 0; k <= LIS.Items.Count - 1; k++)
            {
                if (ItemValue == LIS.Items[k].Value)
                    AA = true;
            }
            return AA;
        }

        public bool InItems(string ItemValue, DropDownList LIS)
        {
            bool AA = false;
            for (int k = 0; k <= LIS.Items.Count - 1; k++)
            {
                if (ItemValue == LIS.Items[k].Value)
                    AA = true;
            }
            return AA;
        }

        public bool InItems(string ItemValue, RadioButtonList LIS)
        {
            bool AA = false;
            for (int k = 0; k <= LIS.Items.Count - 1; k++)
            {
                if (ItemValue == LIS.Items[k].Value)
                    AA = true;
            }
            return AA;
        }

        public static DataTable GridViewToDataTable(GridView GV)
        {
            DataTable DT = new DataTable();
            if (GV.Rows.Count > 0)
            {
                for (int i = 0; i <= GV.Columns.Count - 1; i++)
                {
                    DT.Columns.Add(GV.Columns[i].HeaderText);
                }

                for (int i = 0; i <= GV.Rows.Count - 1; i++)
                {
                    DataRow rw = DT.NewRow();
                    for (int j = 0; j <= GV.Columns.Count - 1; j++)
                    {
                        rw[j] = GV.Rows[i].Cells[j].Text;
                    }
                    DT.Rows.Add(rw);
                }
            }
            return DT;
        }

        //DataTable 2 Excel CSV
        public static string DataTableToExcelCSV(ref DataTable DataTable, string separaterWord = ",")
        {
            string buffer = "";

            //產出表頭
            foreach (System.Data.DataColumn column in DataTable.Columns)
            {
                //輸出表頭，但是匯出時檢查是否有 separaterWord , 因為 separaterWord 是分隔字元
                buffer += column.ColumnName.Replace(separaterWord, "") + separaterWord;
            }
            //if (!string.IsNullOrEmpty(buffer))
            //{

            //    if (buffer.Substring(buffer.Length)) { }
            //    if (String.Right(buffer, Strings.Len(separaterWord)) == separaterWord)
            //        buffer = buffer.Substring(0, Strings.Len(buffer) - Strings.Len(separaterWord));
            //}
            //buffer += Constants.vbCrLf;
            //依照欄位格式產出表身
            foreach (DataRow row in DataTable.Rows)
            {
                //每個欄位
                for (int i = 0; i <= DataTable.Columns.Count - 1; i++)
                {
                    string dat = "" + row[i];
                    //檢查是否有 separaterWord , 因為 separaterWord 是分隔字元
                    dat = dat.Replace(separaterWord, "");
                    //如果是字串型態，加一個 char(128)避免值全是 數字時，被轉成數字
                    //buffer += "" + dat + (DataTable.Columns[i].DataType.Name == "String" & Information.IsNumeric(dat) ? Strings.Chr(128) : "");
                    //如果不是最後一欄，則加入分隔符號
                    if (i != DataTable.Columns.Count - 1)
                    {
                        buffer += separaterWord;
                    }
                }
                buffer += "\r\n";
            }
            //傳回
            return buffer;
        }

        //自動下載某個檔案 (從文字資料)
        //需設定  <globalization requestEncoding="BIG5" responseEncoding="BIG5" />  
        public static object DownloadFile(System.Web.UI.Page WebForm, string FileNameWhenUserDownload, string FileBody)
        {
            WebForm.Response.ClearHeaders();
            WebForm.Response.Clear();
            WebForm.Response.Expires = 0;
            WebForm.Response.Buffer = true;

            WebForm.Response.AddHeader("Accept-Language", "zh-tw");
            WebForm.Response.AddHeader("content-disposition", "attachment; filename=" + "" + System.Web.HttpUtility.UrlEncode(FileNameWhenUserDownload, System.Text.Encoding.UTF8) + "");
            WebForm.Response.ContentType = "Application/octet-stream";

            //byte [] BB = System.Text.Encoding.GetEncoding("big5").GetBytes(Content);
            byte[] buf = System.Text.Encoding.GetEncoding("big5").GetBytes(FileBody);
            //直接轉big5

            //  WebForm.Response.Write(FileBody)
            WebForm.Response.BinaryWrite(buf);
            WebForm.Response.End();
            return true;
        }
 
        public string SendMailNew(string FromMail, string ToMail, string Subj, string Bodystr)
        {
            string MailSvr = System.Configuration.ConfigurationSettings.AppSettings.Get("MailSvr");
            string MailFromName = System.Configuration.ConfigurationSettings.AppSettings.Get("MailFromName");
            if (string.IsNullOrEmpty(FromMail)) { FromMail = System.Configuration.ConfigurationSettings.AppSettings.Get("MailFrom"); }
            string MailUser = System.Configuration.ConfigurationSettings.AppSettings.Get("MailUser");
            string MailPWD = System.Configuration.ConfigurationSettings.AppSettings.Get("MailPWD");
            string SS = "";
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                msg.From = new System.Net.Mail.MailAddress(FromMail, MailFromName);
                //寄件人
                msg.To.Add(new System.Net.Mail.MailAddress(ToMail));
                //收件人
                msg.Subject = Subj;
                //主旨
                msg.Body = Bodystr;
                //內容
                msg.IsBodyHtml = true;
                //格式

                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(MailSvr);
                if (!string.IsNullOrEmpty(MailUser))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(MailUser, MailPWD);
                }
                smtp.Send(msg);
            }
            catch (Exception ex)
            {
                SS = FromMail + "," + ToMail + "<br>" + MailSvr + "<br>" + ex.Message;
            }
            return SS;
        }

        public string SendMailOld(string FromMail, string ToMail, string Subj, string Bodystr)
        {
            string MailSvr = System.Configuration.ConfigurationSettings.AppSettings.Get("MailSvr");
            if (string.IsNullOrEmpty(FromMail))
                FromMail = System.Configuration.ConfigurationSettings.AppSettings.Get("MailFrom");
            string SS = "";
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage(); 
                mail.To.Add(ToMail);
                mail.From = new System.Net.Mail.MailAddress(FromMail);
                mail.Subject = Subj;
                mail.IsBodyHtml = true;
                //mail.BodyFormat = System.Net.Mail.MailFormat.Html;
                mail.Body = Bodystr;

                System.Net.Mail.SmtpClient sm = new System.Net.Mail.SmtpClient(MailSvr);
                sm.Send(mail);
                //if (!string.IsNullOrEmpty(MailSvr)) {
                //}
                //    //System.Net.Mail.SmtpMail.SmtpServer = MailSvr; 
                //System.Net.Mail.SmtpMail.Send(mail); 
            }
            catch (Exception ex)
            {
                SS = FromMail + "," + ToMail + "<br>" + MailSvr + "<br>" + ex.Message;
            }
            return SS;
        } 

        public string SendMail(string FromMail, string ToMail, string Subj, string Bodystr)
        {
            string MailSvr = System.Configuration.ConfigurationSettings.AppSettings.Get("MailSvr");
            if (!string.IsNullOrEmpty(MailSvr))
            {
                return SendMailNew(FromMail, ToMail, Subj, Bodystr);
            }
            else
            {
                return SendMailOld(FromMail, ToMail, Subj, Bodystr);
            }
        }

        public DataTable TurnDataTable(DataTable DTable)
        {
            DataTable ND = new DataTable();
            for (int i = 0; i <= DTable.Rows.Count - 1; i++)
            {
                ND.Columns.Add();
            }
            for (int i = 0; i <= DTable.Columns.Count - 1; i++)
            {
                DataRow rw = ND.NewRow();
                for (int j = 0; j <= DTable.Rows.Count - 1; j++)
                {
                    rw[j] = DTable.Rows[j][i];
                }
                ND.Rows.Add(rw);
            }
            return ND;
        }

        public string EnCrypTo(string InputString)
        {
            //鎖碼
            string result = "";
            try
            {
                string aa = "0123456789abcdefghijklmnopqrstuvwxyz";
                char[] r1 = aa.ToCharArray();
                string[] r2 = {
			"ygsu",
			"v2zf",
			"cnj9",
			"3hko",
			"80at",
			"lr7e",
			"m4wp",
			"xi15",
			"db6q",
			"f57q",
			"lnbe",
			"vzh0",
			"a4i1",
			"wgm9",
			"8soj",
			"rtcd",
			"u2y3",
			"kp6x",
			"6n1q",
			"vkh0",
			"mydg",
			"sc38",
			"oztj",
			"ifl9",
			"2ab4",
			"p7eu",
			"5rxw",
			"a942",
			"1i0x",
			"octu",
			"f5pe",
			"dkgq",
			"vjlr",
			"bwyz",
			"73nm",
			"f68u"
		};
                char[] istr = InputString.ToLower().ToCharArray();
                for (int i = 0; i <= istr.Length - 1; i++)
                {
                    for (int j = 0; j <= r1.Length - 1; j++)
                    {
                        if (istr[i] == r1[j])
                        {
                            result += r2[j];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = InputString;
            }
            return result;
        }

        public string DeCrypTo(string InputString)
        {
            //解碼
            string result = "";
            try
            {
                string aa = "0123456789abcdefghijklmnopqrstuvwxyz";
                char[] r1 = aa.ToCharArray();
                string[] r2 = {
			"ygsu",
			"v2zf",
			"cnj9",
			"3hko",
			"80at",
			"lr7e",
			"m4wp",
			"xi15",
			"db6q",
			"f57q",
			"lnbe",
			"vzh0",
			"a4i1",
			"wgm9",
			"8soj",
			"rtcd",
			"u2y3",
			"kp6x",
			"6n1q",
			"vkh0",
			"mydg",
			"sc38",
			"oztj",
			"ifl9",
			"2ab4",
			"p7eu",
			"5rxw",
			"a942",
			"1i0x",
			"octu",
			"f5pe",
			"dkgq",
			"vjlr",
			"bwyz",
			"73nm",
			"f68u"
		};
                for (int i = 0; i <= InputString.Length - 1; i += 4)
                {
                    for (int j = 0; j <= r2.Length - 1; j++)
                    {
                        if (InputString.Substring(i, 4) == r2[j])
                        {
                            result += r1[j];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = InputString;
            }
            return result.ToUpper();
        }

        public string IsZero(string s)
        {
            if (s == "") { return "0"; } else { return s; }
        }

        public Bitmap CutImage(Bitmap souceImage, int w, int h)
        {
            //建立新的影像, int w 
            Bitmap cropImage = new Bitmap(w, h);
            //準備繪製新的影像
            Graphics graphics2 = Graphics.FromImage(cropImage);
            //設定裁切範圍
            Rectangle cropRect = new Rectangle(souceImage.Width / 2 - (w / 2), souceImage.Height / 2 - (h / 2), w, h);
            //於座標(0,0)開始繪製裁切影像
            graphics2.DrawImage(souceImage, 0, 0, cropRect, GraphicsUnit.Point);
            graphics2.Dispose();
            //儲存新的影像
            return cropImage;
        }

        public Bitmap zoomImage(Bitmap inbmp, int boxwidth, int boxheight) //圖片縮放
        {
            float scale = 0;
            int newwidth = 0;
            int newheight = 0;
            if (inbmp.Height < boxheight & inbmp.Width < boxwidth)
            {
                scale = 1;
                newwidth = boxwidth;
                newheight = boxheight;

            }
            else
            {
                if ((inbmp.Width * boxheight / inbmp.Height) < boxwidth)
                {

                    newwidth = Convert.ToInt32(inbmp.Width * boxheight / inbmp.Height);
                    newheight = Convert.ToInt32(inbmp.Height * boxheight / inbmp.Height);
                    newwidth = boxwidth;
                    newheight = Convert.ToInt32(inbmp.Height * boxwidth / inbmp.Width);
                    scale = (boxheight / inbmp.Height);
                }
                else
                {
                    newwidth = Convert.ToInt32(inbmp.Width * boxwidth / inbmp.Width);
                    newheight = Convert.ToInt32(inbmp.Height * boxwidth / inbmp.Width);
                    newheight = boxheight;
                    newwidth = Convert.ToInt32(inbmp.Width * boxheight / inbmp.Height);
                    scale = (boxwidth) / inbmp.Width;
                }
            }
            Bitmap outbmp = new Bitmap(inbmp, newwidth, newheight);
            return outbmp;
        }
   
        public Bitmap GrayImage(Bitmap inbmp)
        {
            try
            {
                int Height = inbmp.Height;
                int Width = inbmp.Width;
                Bitmap newBitmap = new Bitmap(Width, Height);
                System.Drawing.Color pixel;
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                    {
                        pixel = inbmp.GetPixel(x, y);
                        int r, g, b, Result = 0;
                        r = pixel.R;
                        g = pixel.G;
                        b = pixel.B;
                        //实例程序以加权平均值法产生黑白图像
                        int iType = 2;
                        switch (iType)
                        {
                            case 0://平均值法
                                Result = ((r + g + b) / 3);
                                break;
                            case 1://最大值法
                                Result = r > g ? r : g;
                                Result = Result > b ? Result : b;
                                break;
                            case 2://加权平均值法
                                Result = ((int)(0.7 * r) + (int)(0.2 * g) + (int)(0.1 * b));
                                break;
                        }
                        int whiteUp = 160;
                        Result = ((255 - whiteUp) * Result / 255) + whiteUp;
                        newBitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(Result, Result, Result));
                    }
                return newBitmap;
            }
            catch (Exception ex)
            {
                WriteLog("GrayImage:" + ex.Message);
                return inbmp;
            }
        }
        
        public int SaveCookie(String CookieName, String CookieValue, int ExpireDays = 365)
        {
            try
            {
                HttpCookie CoKe = new HttpCookie(CookieName);
                CoKe.Value = CookieValue;
                CoKe.Expires = DateTime.Now.AddDays(ExpireDays);
                HttpContext.Current.Response.Cookies.Add(CoKe);
                return 1;
            }
            catch (Exception e)
            {
                WriteLog(e.Message);
                return 0;
            }
        }

        public int DeleCoookie(String CookieName)
        {
            try
            {
                HttpCookie CoKe = new HttpCookie(CookieName);
                CoKe.Value = "";
                CoKe.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(CoKe);
                return 1;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public string strDate(DateTime dd)
        {
            string tmp = Convert.ToString(dd.Year - 1911);
            tmp += GetFullNum(dd.Month, 2);
            tmp += GetFullNum(dd.Day, 2);
            //tmp += GetFullNum(dd.Hour, 2)
            return tmp;
        }

        public void WriteLog(string Mess)
        {
            string MP = System.Configuration.ConfigurationSettings.AppSettings.Get("LOGPath");
            if (string.IsNullOrEmpty(MP))
                MP = Server.MapPath("~/") + "LOGPath\\";

            MP = MP + "\\" + DateTime.Now.Year.ToString() + GetFullNum(DateTime.Now.Month, 2) + "\\";
            if (System.IO.Directory.Exists(MP) == false)
            {
                System.IO.Directory.CreateDirectory(MP);
            }

            StreamWriter SW = new StreamWriter(MP + strDate(DateTime.Today) + ".log", true, System.Text.Encoding.Default);
            string str = "";
            try
            {
                str = Mess;
                SW.WriteLine(str);
                SW.Flush();
                SW.Close();

            }
            catch (Exception ex)
            {
                SW.Flush();
                SW.Close();
            }
            finally
            {
                SW.Close();
            }
        }

        public int PasswordStrength(string PWD)
        {
            int II = 0;
            //(?=.*\d)       因為\d表示[0-9] 
            //(?=.*[a-zA-Z]) 大寫與小寫英文字母> 
            //(?=.*[\W_])    [^A-Za-z0-9_]，\W有排除掉"_"這個符號> 
            Regex regex123 = new Regex("^(?=.*\\d).{8,12}$");
            if (regex123.IsMatch(PWD)) { II += 1; }

            Regex regexabc = new Regex("(?=.*[a-z]).{8,12}$");
            if (regexabc.IsMatch(PWD)) { II += 1; }

            Regex regexDEF = new Regex("(?=.*[A-Z]).{8,12}$");
            if (regexDEF.IsMatch(PWD)) { II += 1; }

            Regex regexW_ = new Regex("(?=.*[\\W_]).{8,12}$");
            if (regexW_.IsMatch(PWD)) { II += 1; }

            return II;
        }

    }
}
