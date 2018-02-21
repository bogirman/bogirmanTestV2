using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    public class JDB
    {
        public string ver = "1.0.170222.1";
        public string ConnStr = System.Configuration.ConfigurationManager.AppSettings.Get("Database");
        public System.Data.SqlClient.SqlConnection CONN = new System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppSettings.Get("Database"));
        public System.Data.SqlClient.SqlCommand CMD = new System.Data.SqlClient.SqlCommand();
        public System.Data.SqlClient.SqlDataAdapter ADP = new System.Data.SqlClient.SqlDataAdapter();
        public int LogStat = 0;
        public string[] DirtyWords = { "Insert", "Delete", "Update", "--", "Create","Drop" };


        public JDB()
        {
            CONN = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings.Get("DataBase"));

            if (IsNumeric(System.Configuration.ConfigurationManager.AppSettings.Get("LogStat")))
            {
                LogStat = Cint2(System.Configuration.ConfigurationManager.AppSettings.Get("LogStat"));
            }
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings.Get("DirtyWords")))
            {
                DirtyWords = System.Configuration.ConfigurationManager.AppSettings.Get("DirtyWords").Split(',');
            }
        }

        public JDB(string ConnectionString)
        {
            if (ConnectionString != "")
            {
                CONN = new SqlConnection(ConnectionString);
                ConnStr = ConnectionString;
            }
            else
            {
                CONN = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings.Get("DataBase"));
            }

            if (IsNumeric(System.Configuration.ConfigurationManager.AppSettings.Get("LogStat")))
            {
                LogStat = Cint2(System.Configuration.ConfigurationManager.AppSettings.Get("LogStat"));
            }
            if (!string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings.Get("DirtyWords")))
            {
                DirtyWords = System.Configuration.ConfigurationManager.AppSettings.Get("DirtyWords").Split(',');
            }
        }

        public void ParaClear()
        {
            CMD = new SqlCommand();
        }

        public void ParaAdd(string ParameterName, object Valu, SqlDbType DaType)
        {
            SqlParameter sp = new SqlParameter(ParameterName, DaType);
            sp.Value = Valu.ToString();
            CMD.Parameters.Add(sp);
        }

        public void ParaAdd(string ParameterName, object Valu, SqlDbType DaType, int DataSize)
        {
            SqlParameter sp = new SqlParameter(ParameterName, DaType, DataSize);
            sp.Value = Valu.ToString();
            CMD.Parameters.Add(sp);
        }

        public int NonQuery(string sqlstr)
        {
            int result = 0;
            string msg = "";
            if (CONN.State != ConnectionState.Open) { CONN.Open(); }
            CMD.Connection = CONN;
            CMD.CommandText = sqlstr;
            CMD.CommandType = CommandType.Text;
            SqlTransaction trans = CONN.BeginTransaction();
            CMD.Transaction = trans;
            try
            {
                result = CMD.ExecuteNonQuery();
                trans.Commit();
                CONN.Close();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                result = 0;
                msg = ex.Message;
            }
            finally
            {
                if (CONN.State == ConnectionState.Open)
                {
                   // trans.Commit();
                    CONN.Close();
                }
                trans.Dispose();
            }

            try  //log用另外的try防止檔案被咬
            {
                if (msg != "")
                {
                    WriteCMD();
                    WriteLog("[" + DateTime.Now.ToLongTimeString() + "][Main.NonQuery.Error]:" + msg + sqlstr);
                }
                else 
                {
                    if (LogStat >= 1) { WriteCMD(); }
                    WriteLog(sqlstr);
                }
                
            }
            catch { }

            return result;
        }

        public int NonQuery(string sqlstr, CommandType CmdType)
        {
            int result = 0;
            string msg = "";
            if (CONN.State != ConnectionState.Open) { CONN.Open(); }
            CMD.Connection = CONN;
            CMD.CommandText = sqlstr;
            CMD.CommandType = CmdType;
            SqlTransaction trans = CONN.BeginTransaction();
            CMD.Transaction = trans;
            try
            {
                result = CMD.ExecuteNonQuery();
                trans.Commit();
                CONN.Close();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                result = 0;
                msg = ex.Message;
            }
            finally
            {
                if (CONN.State == ConnectionState.Open)
                {
                    CONN.Close();
                }
            }

            try  //log用另外的try防止檔案被咬
            {
                if (msg != "")
                {
                    WriteCMD();
                    WriteLog("[" + DateTime.Now.ToLongTimeString() + "][Main.NonQuery.Error]:" + msg + sqlstr);
                }
                else
                {
                    if (LogStat >= 1) { WriteCMD(); }
                    WriteLog(sqlstr);
                }

            }
            catch { }

            return result;
        }

        public int NonQuery(string sqlstr, CommandType CmdType, bool CloseConn)
        {
            int result = 0;
            string msg = "";
            if (CONN.State != ConnectionState.Open) { CONN.Open(); }
            CMD.Connection = CONN;
            CMD.CommandText = sqlstr;
            CMD.CommandType = CmdType;
            SqlTransaction trans = CONN.BeginTransaction();
            CMD.Transaction = trans;
            try
            {
                result = CMD.ExecuteNonQuery();
                trans.Commit();
                if (CloseConn == true) { CONN.Close(); }
            }
            catch (Exception ex)
            {
                trans.Rollback();
                result = 0;
                msg = ex.Message;
            }
            finally
            {
                if (CONN.State == ConnectionState.Open)
                {
                    CONN.Close();
                }
            }

            try  //log用另外的try防止檔案被咬
            {
                if (msg != "")
                {
                    WriteCMD();
                    WriteLog("[" + DateTime.Now.ToLongTimeString() + "][Main.NonQuery.Error]:" + msg + sqlstr);
                }
                else
                {
                    if (LogStat >= 1) { WriteCMD(); }
                    WriteLog(sqlstr);
                }

            }
            catch { }

            return result;
        }

        public int NonQuery(string[] sqlstr, CommandType CmdType)
        {
            int result = 0;
            string msg = "";
            string sqlstrlog = "";
            if (CONN.State != ConnectionState.Open) { CONN.Open(); } 
            SqlTransaction trans = CONN.BeginTransaction();
            try
            {
                SqlCommand[] CMDs = new SqlCommand[sqlstr.Length];
                for (int i = 0; i < sqlstr.Length; i++)
                {
                    CMDs[i].CommandText = sqlstr[i];
                    CMDs[i].Connection = CONN;
                    CMDs[i].Transaction = trans;
                    result += CMDs[i].ExecuteNonQuery();
                    sqlstrlog += sqlstr[i];
                }
                trans.Commit();
                
                CONN.Close();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                result = 0;
                msg = ex.Message;
            }
            finally
            {
                if (CONN.State == ConnectionState.Open)
                {
                    CONN.Close();
                }
            }

            try  //log用另外的try防止檔案被咬
            {
                if (msg != "")
                {
                    WriteCMD();
                    WriteLog("[" + DateTime.Now.ToLongTimeString() + "][Main.NonQuery.Error]:" + msg + sqlstrlog);
                }
                else
                {
                    if (LogStat >= 1) { WriteCMD(); }
                    WriteLog(sqlstrlog);
                }

            }
            catch { }

            return result;
        }

        public DataTable GetDataSet(string sqlstr)
        {
            string msg = "";
            bool chk = false;
            if (DirtyWords.Length > 0)
            {
                foreach (String s in DirtyWords)
                {
                    if (sqlstr.ToUpper().IndexOf(s.ToUpper()) > -1)
                    {
                        chk = true;
                    }
                }
            }
            DataTable DS = new DataTable();
            if (chk)
            {
                WriteLog("[" + DateTime.Now.ToLongTimeString() + "][Main.GetDataSet.Error]:Here is Somthing Wrong!!\n\r" + sqlstr);
                return DS;
            }
            try
            {
                CMD.Connection = CONN;
                CMD.CommandText = sqlstr;
                CMD.CommandType = CommandType.Text;
                ADP = new SqlDataAdapter(CMD);
                ADP.Fill(DS);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            finally
            {
                if (CONN.State == ConnectionState.Open)
                {
                    CONN.Close();
                }
            }


            try  //log用另外的try防止檔案被咬
            {
                if (msg != "")
                {
                    WriteCMD();
                    WriteLog("[" + DateTime.Now.ToLongTimeString() + "][Main.GetDataSet.Error]:" + msg + sqlstr);
                }
                else
                {
                    if (LogStat >= 1) { WriteCMD(); }
                    WriteLogselect(sqlstr);
                }

            }
            catch { }

            return DS;
        }

        public DataTable GetDataSet(string sqlstr, CommandType CmdType)
        {
            string msg = "";
            bool chk = false;
            if (DirtyWords.Length > 0) {
                foreach (String s in DirtyWords) {
                    if (sqlstr.ToUpper().IndexOf(s.ToUpper()) > -1) {
                        chk = true;
                    }
                }
            }
            DataTable DS = new DataTable();
            if (chk)
            {
                WriteLog("[" + DateTime.Now.ToLongTimeString() + "][Main.GetDataSet.Error]:Here is Somthing Wrong!!\n\r" + sqlstr);
                return DS;
            }
            try
            {
                CMD.Connection = CONN;
                CMD.CommandText = sqlstr;
                CMD.CommandType = CmdType;
                ADP = new SqlDataAdapter(CMD);
                ADP.Fill(DS);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            finally
            {
                if (CONN.State == ConnectionState.Open)
                {
                    CONN.Close();
                }
            }


            try  //log用另外的try防止檔案被咬
            {
                if (msg != "")
                {
                    WriteCMD();
                    WriteLog("[" + DateTime.Now.ToLongTimeString() + "][Main.GetDataSet.Error]:" + msg + sqlstr);
                }
                else
                {
                    if (LogStat >= 1) { WriteCMD(); }
                    WriteLogselect(sqlstr);
                }

            }
            catch { }

            return DS;
        }
         
        public DataTable GetDataSetNoNull(string sqlstr)
        {
            DataTable DS = GetDataSet(sqlstr);
            DataTable NewDT = new DataTable();
            if (DS.Rows.Count > 0)
            {
                foreach (DataColumn DC in DS.Columns)
                {
                    NewDT.Columns.Add(DC.ColumnName);
                }
                foreach (DataRow rw in DS.Rows)
                {
                    DataRow Nrw = NewDT.NewRow();
                    for (int i = 0; i <= DS.Columns.Count - 1; i++)
                    {
                        if (rw[i].GetType().ToString() == "System.DBNull")
                        {
                            Nrw[i] = "";
                        }
                        else
                        {
                            Nrw[i] = rw[i];
                        }
                    }
                    NewDT.Rows.Add(Nrw);
                }
            }
            return NewDT;
            //return GetDataSet(sqlstr);
        }

        public DataTable GetDataSetNoNull(string sqlstr, CommandType CmdType)
        {
            DataTable DS = GetDataSet(sqlstr, CmdType);
            DataTable NewDT = new DataTable();
            if (DS.Rows.Count > 0)
            {
                foreach (DataColumn DC in DS.Columns)
                {
                    NewDT.Columns.Add(DC.ColumnName);
                }
                foreach (DataRow rw in DS.Rows)
                {
                    DataRow Nrw = NewDT.NewRow();
                    for (int i = 0; i <= DS.Columns.Count - 1; i++)
                    {
                        if (rw[i].GetType().ToString() == "System.DBNull")
                        {
                            Nrw[i] = "";
                        }
                        else
                        {
                            Nrw[i] = rw[i];
                        }
                    }
                    NewDT.Rows.Add(Nrw);
                }
            }
            return NewDT;
            //return GetDataSet(sqlstr);
        }

        //未完成
        public String JSONData(string sqlstr) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //用DataReader來寫加快速度

            return sb.ToString();
        }

        //未完成
        public String JSONData(string sqlstr, CommandType CmdType)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            return sb.ToString();
        }

        public string Scalar(string sqlstr)
        {
            string msg = "";
            object result = "";
            try
            {
                if (CONN.State != ConnectionState.Open) { CONN.Open(); }
                CMD.Connection = CONN;
                CMD.CommandText = sqlstr;
                CMD.CommandType = CommandType.Text;
                result = CMD.ExecuteScalar();
                
                CONN.Close();
            }
            catch (Exception ex)
            {
                result = "";
                msg = ex.Message;
            }
            finally
            {
                if (CONN.State == ConnectionState.Open)
                {
                    CONN.Close();
                }
            }

            try  //log用另外的try防止檔案被咬
            {
                if (msg != "")
                {
                    WriteCMD();
                    WriteLog("[" + DateTime.Now.ToLongTimeString() + "][Main.Scalar.Error]:" + msg + sqlstr);
                }
                else
                {
                    if (LogStat >= 1) { WriteCMD(); }
                    WriteLogselect(sqlstr);
                }

            }
            catch { }

            if (result == null)
                return "";
            else
                return result.ToString();
        }

        public string Scalar(string sqlstr, CommandType CmdType)
        {
            string msg = "";
            object result = "";
            try
            {
                if (CONN.State != ConnectionState.Open) { CONN.Open(); }
                CMD.Connection = CONN;
                CMD.CommandText = sqlstr;
                CMD.CommandType = CmdType;
                result = CMD.ExecuteScalar();
                CONN.Close();
            }
            catch (Exception ex)
            {
                result = "";
                msg = ex.Message;
            }
            finally
            {
                if (CONN.State == ConnectionState.Open)
                {
                    CONN.Close();
                }
            }

            try  //log用另外的try防止檔案被咬
            {
                if (msg != "")
                {
                    WriteCMD();
                    WriteLog("[" + DateTime.Now.ToLongTimeString() + "][Main.Scalar.Error]:" + msg + sqlstr);
                }
                else
                {
                    if (LogStat >= 1) { WriteCMD(); }
                    WriteLogselect(sqlstr);
                }

            }
            catch { }

            if (result == null)
                return "";
            else
                return result.ToString();
        }

        public string Scalar(string sqlstr, CommandType CmdType, bool CloseConn)
        {
            string msg = "";
            object result = "";
            try
            {
                if (CONN.State != ConnectionState.Open) { CONN.Open(); }
                CMD.Connection = CONN;
                CMD.CommandText = sqlstr;
                CMD.CommandType = CmdType;
                result = CMD.ExecuteScalar();
                if (CloseConn == true) { CONN.Close(); }
            }
            catch (Exception ex)
            {
                result = "";
                msg = ex.Message;
            }
            finally
            {
                if (CONN.State == ConnectionState.Open)
                {
                    CONN.Close();
                }
            }

            try  //log用另外的try防止檔案被咬
            {
                if (msg != "")
                {
                    WriteCMD();
                    WriteLog("[" + DateTime.Now.ToLongTimeString() + "][Main.Scalar.Error]:" + msg + sqlstr);
                }
                else
                {
                    if (LogStat >= 1) { WriteCMD(); }
                    WriteLogselect(sqlstr);
                }

            }
            catch { }

            if (result == null)
                return "";
            else
                return result.ToString();
        }
        

        public int BulkCopy(DataTable insertDT, string TableName, ref bool distinct)
        {
            int result = 0;
            if (insertDT.Rows.Count == 0)
            {
                return result; 
            }

            string NN = "";
            try
            {
                //--------排除重複--------------
                if (distinct)
                {
                    List<string> LList = new List<string>();
                    foreach (DataColumn DC in insertDT.Columns)
                    {
                        LList.Add(DC.ColumnName);
                    }
                    insertDT = insertDT.DefaultView.ToTable(true, LList.ToArray());
                }

                CONN.Open();
                SqlTransaction Trans = CONN.BeginTransaction();
                SqlBulkCopy SqlBC = new SqlBulkCopy(CONN, SqlBulkCopyOptions.KeepIdentity, Trans);

                SqlBC.BatchSize = 1000;
                //設定一個批次量寫入多少筆資料
                SqlBC.BulkCopyTimeout = 180;
                //設定逾時的秒數
                SqlBC.DestinationTableName = "dbo." + TableName;
                //寫入資料表名稱

                //對應資料行名稱(不指定的話欄位類型會判斷錯誤)
                foreach (DataColumn DC in insertDT.Columns)
                {
                    NN += "," + DC.ColumnName;
                    SqlBC.ColumnMappings.Add(DC.ColumnName, DC.ColumnName);
                }

                SqlBC.WriteToServer(insertDT);
                //開始寫入

                Trans.Commit();
                CONN.Close();

            }
            catch (Exception ex)
            {
                result = 0;
                //WriteCMD()
                WriteLog("[" + DateTime.Now.ToString() + "][Main.BulkCopy.Error]:" + ex.Message + ":" + TableName + "(" + NN.Substring(1) + ")");
            }
            finally
            {
                if (CONN.State == ConnectionState.Open)
                {
                    CONN.Close();
                }
            }

            return result;
        }

        public object IsNullInt(string Value)
        {
            if (IsNumeric(Value))
            {
                return Value;
            }
            else
            {
                return DBNull.Value;
            }
        }

        public object IsNullDate(string Value)
        {
            if (IsDate(Value))
            {
                return Value;
            }
            else
            {
                return DBNull.Value;
            }
        }

        public string GetInsertString(string TableName)
        {
            string result = "";
            string str = "Select * from syscolumns where id=(Select id from sysobjects where name=@name ) and name<>'idno'";
            ParaAdd("@name", TableName, SqlDbType.NVarChar);
            DataTable d = GetDataSet(str);
            if (d.Rows.Count > 0)
            {
                string s = "";
                string s2 = "";
                for (int i = 0; i < d.Rows.Count; i++)
                {
                    s += "," + d.Rows[i]["name"];
                    s2 += ",@" + d.Rows[i]["name"];
                }
                result = "Insert into " + TableName + " (" + s.Substring(1) + ") values (" + s2.Substring(1) + ")";

            }
            return result;
        }

        public string GetUpdateString(string TableName, string KeyColName)
        {
            string result = "";
            string str = "Select * from syscolumns where id=(Select id from sysobjects where name=@name ) and name<>'idno'";
            ParaAdd("@name", TableName, SqlDbType.NVarChar);
            DataTable d = GetDataSet(str);
            if (d.Rows.Count > 0)
            {
                string s = "";
                for (int i = 0; i < d.Rows.Count; i++)
                {
                    s += "," + d.Rows[i]["name"] + "=@" + d.Rows[i]["name"];
                }
                result = "Update " + TableName + " Set " + s.Substring(1) + " where " + KeyColName + " = @" + KeyColName;

            }
            return result;
        }

        public string strDate(DateTime dd)
        {
            string tmp = Convert.ToString(dd.Year - 1911);
            tmp += GetFullNum(dd.Month, 2);
            tmp += GetFullNum(dd.Day, 2);
            //tmp += GetFullNum(dd.Hour, 2)
            return tmp;
        }

        public void WriteCMD()
        {
            WriteLog("[" + DateTime.Now.ToLongTimeString() + "]");
            if (CMD.Parameters.Count > 0)
            {
                for (int i = 0; i < CMD.Parameters.Count; i++)
                {
                    WriteLog("Declare " + CMD.Parameters[i].ParameterName.ToString() + "  " + CMD.Parameters[i].SqlDbType.ToString().Replace("NVarChar", "nvarchar(500)").Replace("Int", "int") + " = '" + CMD.Parameters[i].Value.ToString() + "'");
                }
                WriteLog(CMD.CommandText);
            }
        }

        public void WriteCMDselect()
        {
            string msg = "";
            foreach (SqlParameter sp in CMD.Parameters)
            {
                msg += "Declare " + sp.ParameterName + "  " + sp.SqlDbType.ToString() + " = '" + sp.Value + "'\n\r";
            }
            WriteLogselect("[" + DateTime.Now.ToString() + "]\n\r" + msg + "\n\r" + CMD.CommandText);
        }

        public void WriteLogselect(string Mess)
        {
            string MP = System.Configuration.ConfigurationManager.AppSettings.Get("LOGPath");
            if (string.IsNullOrEmpty(MP))
                MP ="LOGPath\\select\\";

            MP = MP + "\\select\\" + DateTime.Now.Year + GetFullNum(DateTime.Now.Month, 2) + "\\";
            if (System.IO.Directory.Exists(MP) == false)
            {
                System.IO.Directory.CreateDirectory(MP);
            }

            StreamWriter SW = new StreamWriter(MP + strDate(DateTime.Now) + ".log", true, System.Text.Encoding.Default);
            string str = "";
            try
            {
                str = "[" + DateTime.Now.ToString() + "-" + System.Web.HttpContext.Current.Request.UserHostAddress + "]\n\r" + Mess;
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

        public void WriteLog(string Mess)
        {
            string MP = System.Configuration.ConfigurationManager.AppSettings.Get("LOGPath");
            if (string.IsNullOrEmpty(MP))
                MP ="LOGPath\\";

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

        private string GetFullNum(int num, int size)
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

        private bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        private bool IsDate(string sdate)
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

        private int Cint2(object s)
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

        private string ClearHTML(string inStr)
        {
            while (inStr.IndexOf("[") > 0)
            {
                inStr = inStr.Substring(0, inStr.IndexOf("[")) + inStr.Substring(inStr.IndexOf("]") + 1);
            }
            return inStr;
        }

    }
}
