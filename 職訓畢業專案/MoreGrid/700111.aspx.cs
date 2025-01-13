//---------------------------------------------------------------------------- 
//程式功能	線上客服-客戶使用端 > 對話視窗 > 發送訊息
//---------------------------------------------------------------------------- 
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Text;

public partial class _7001_700111 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        string mErr = "";
        int cu_sid = -1, mg_sid = -1;

        if (!IsPostBack)
        {
         
            mErr = "參數傳送錯誤!返回主頁面\\n";

            if (Request["sid"] != null && Request["mg_sid"] != null)
            {
                if (int.TryParse(Request["sid"], out cu_sid) && int.TryParse(Request["mg_sid"], out mg_sid))
                {
                    lb_cu_sid.Text = cu_sid.ToString();
                    lb_mg_sid.Text = mg_sid.ToString();

                    if (Chk_Cs_User(cu_sid, mg_sid))
                        mErr = "";
                    else
                        mErr = "找不到客服要求紀錄!\\n";
                }
            }

            if (mErr != "")
                ClientScript.RegisterStartupScript(this.GetType(), "ClientScript", "alert(\"" + mErr + "\");parent.location.replace(\"7001.aspx\");", true);
        }
    }

 

    // 傳送訊息 (存入客服交談紀錄)
    protected void bn_smsg_Click(object sender, EventArgs e)
    {
        String_Func sfc = new String_Func();
        string SqlString = "", cu_rtn = "0";

        if (tb_cm_desc.Text.Trim() != "")
        {
            cu_rtn = Chk_Talk();

            if (cu_rtn == "2")
            {
                // 已結束就不可上傳
                bn_smsg.Enabled = false;
         

                ClientScript.RegisterStartupScript(this.GetType(), "ClientScript", "alert(\"交談已結束，請重新提出客服要求！\");parent.location.replace(\"7001.aspx\");", true);
            }
            else
            {
                // 處理換行字元，並取得左方1000個字，以附超過資料庫限制
                tb_cm_desc.Text = sfc.Left(tb_cm_desc.Text.Replace("\n", "<br>").Trim(), 1000);

                using (SqlConnection Sql_Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString))
                {
                    using (SqlCommand Sql_Command = new SqlCommand())
                    {
                        Sql_Conn.Open();

                        SqlString = "Insert Into Cs_Message (cu_sid, cm_time, cm_object, cm_desc) Values ";
                        SqlString += "(@cu_sid, getdate(), 1, @cm_desc)";

                        Sql_Command.Connection = Sql_Conn;
                        Sql_Command.CommandText = SqlString;
                        Sql_Command.Parameters.AddWithValue("cu_sid", lb_cu_sid.Text);
                        Sql_Command.Parameters.AddWithValue("cm_desc", tb_cm_desc.Text);

                        Sql_Command.ExecuteNonQuery();

                        Sql_Command.Dispose();

                        Sql_Conn.Close();
                    }
                }
            }
        }

        tb_cm_desc.Text = "";
        tb_cm_desc.Focus();
    }


    private bool Chk_Cs_User(int cu_sid, int mg_sid)
    {
        string SqlString = "";
        bool cktf = false;

        using (SqlConnection Sql_Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString))
        {
            using (SqlCommand Sql_Command = new SqlCommand())
            {
                Sql_Conn.Open();
                SqlString = "Select Top 1 cu_sid From Cs_User u Where u.cu_sid = @cu_sid And u.mg_sid = @mg_sid";

                Sql_Command.Connection = Sql_Conn;
                Sql_Command.CommandText = SqlString;
                Sql_Command.Parameters.AddWithValue("cu_sid", cu_sid);
                Sql_Command.Parameters.AddWithValue("mg_sid", mg_sid);

                using (SqlDataReader Sql_Reader = Sql_Command.ExecuteReader())
                {
                    if (Sql_Reader.Read())
                        cktf = true;
                    else
                        cktf = false;

                    Sql_Reader.Close();
                }
            }
        }

        return cktf;
    }

    // 檢查交談是否已結束
    private string Chk_Talk()
    {
        string SqlString = "", cu_rtn = "0";

        using (SqlConnection Sql_Conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["C501_04ConnectionString"].ConnectionString))
        {
            using (SqlCommand Sql_Command = new SqlCommand())
            {
                Sql_Conn.Open();
                Sql_Command.Connection = Sql_Conn;

                SqlString = "Select Top 1 cu_rtn From Cs_User Where cu_sid = @cu_sid";
                Sql_Command.CommandText = SqlString;
                Sql_Command.Parameters.AddWithValue("cu_sid", lb_cu_sid.Text);

                using (SqlDataReader Sql_Reader = Sql_Command.ExecuteReader())
                {
                    if (Sql_Reader.Read())
                    {
                        cu_rtn = Sql_Reader["cu_rtn"].ToString();
                    }
                    Sql_Reader.Close();
                    Sql_Reader.Dispose();
                }

                Sql_Conn.Close();
            }
        }

        return cu_rtn;
    }
    public class String_Func
    {

        #region IsInteger() 檢查是否為整數值
        public bool IsInteger(string mdata)
        {
            bool mfg = true;

            for (int iCnt = 0; iCnt < mdata.Length; iCnt++)
            {
                if (!char.IsDigit(mdata[iCnt]))
                {
                    mfg = false;
                    iCnt = mdata.Length;
                }
            }

            return mfg;
        }
        #endregion

        #region Left() 擷取左側字元
        public string Left(string mdata)
        {
            return Left(mdata, 1);
        }
        public string Left(string mdata, int leng)
        {
            mdata = mdata.Trim();
            if (mdata.Length < leng)
                leng = mdata.Length;

            return mdata.Trim().Substring(0, leng);
        }
        #endregion

        #region Right() 擷取右側字元
        public string Right(string mdata)
        {
            return Right(mdata, 1);
        }

        public string Right(string mdata, int leng)
        {
            mdata = mdata.Trim();

            if (mdata.Length < leng)
                leng = mdata.Length;

            return mdata.Substring(mdata.Length - leng, leng);
        }
        #endregion

        #region FillRight() 以指定字元填滿右方不足長度的位置
        public string FillRight(string mstr, int ilen)
        {
            return FillRight(mstr, ilen, "0");
        }

        public string FillRight(string mstr, int ilen, string fstr)
        {
            string nstr = "";

            // 只取第一個字元作為填滿字元
            if (fstr.Length > 1)
                fstr = fstr.Substring(0, 1);

            if (mstr.Length >= ilen)
            {
                nstr = mstr;
            }
            else
            {
                nstr = mstr + Duplicate(fstr, ilen - mstr.Length);
            }

            return nstr;
        }
        #endregion

        #region FillLeft() 以指定字元填滿左方不足長度的位置
        public string FillLeft(string mstr, int ilen)
        {
            return FillLeft(mstr, ilen, "0");
        }

        public string FillLeft(string mstr, int ilen, string fstr)
        {
            string nstr = "";

            // 只取第一個字元作為填滿字元
            if (fstr.Length > 1)
                fstr = fstr.Substring(0, 1);

            if (mstr.Length >= ilen)
            {
                nstr = mstr;
            }
            else
            {
                nstr = Duplicate(fstr, ilen - mstr.Length) + mstr;
            }

            return nstr;
        }
        #endregion

        #region Dupliacte() 產生重覆字串
        public string Duplicate(string mstr, int ncnt)
        {
            // 使用 StringBuilder 加速字串重覆產生的速度
            StringBuilder dstr = new StringBuilder();
            int icnt = 0;

            for (icnt = 0; icnt < ncnt; icnt++)
            {
                dstr.Append(mstr);
            }

            return dstr.ToString();
        }
        #endregion

        #region ChNumber() 個位數字轉中文數字 (零、一、二....)
        public string ChNumber(int NInt)
        {
            return ChNumber(NInt.ToString());
        }

        public string ChNumber(string NStr)
        {
            // 只取右方一個字元
            NStr = Left(NStr, 1);

            switch (NStr)
            {
                case "0":
                    NStr = "零";
                    break;

                case "1":
                    NStr = "一";
                    break;

                case "2":
                    NStr = "二";
                    break;

                case "3":
                    NStr = "三";
                    break;

                case "4":
                    NStr = "四";
                    break;

                case "5":
                    NStr = "五";
                    break;

                case "6":
                    NStr = "六";
                    break;

                case "7":
                    NStr = "七";
                    break;

                case "8":
                    NStr = "八";
                    break;

                case "9":
                    NStr = "九";
                    break;

                default:
                    NStr = "？";
                    break;
            }
            return NStr;
        }
        #endregion

        #region ChCapitalNumber() 個位數字轉中文大寫數字 (零、壹、貳...)
        public string ChCapitalNumber(int NInt)
        {
            return ChCapitalNumber(NInt.ToString());
        }

        public string ChCapitalNumber(string NStr)
        {
            NStr = Left(NStr, 1);

            switch (NStr)
            {
                case "0":
                    NStr = "零";
                    break;

                case "1":
                    NStr = "壹";
                    break;

                case "2":
                    NStr = "貳";
                    break;

                case "3":
                    NStr = "參";
                    break;

                case "4":
                    NStr = "肆";
                    break;

                case "5":
                    NStr = "伍";
                    break;

                case "6":
                    NStr = "陸";
                    break;

                case "7":
                    NStr = "柒";
                    break;

                case "8":
                    NStr = "捌";
                    break;

                case "9":
                    NStr = "玖";
                    break;

                default:
                    NStr = "？";
                    break;
            }
            return NStr;
        }
        #endregion

        #region GetFourChNumber() 取得每四位數的中文位數字 (萬、億、兆...)
        public string GetFourChNumber(int iDigit)
        {
            string sDigit = "";

            // 限於整數態的上限，「京」之後的中文數字是用不到
            switch (iDigit)
            {
                case 0:
                    sDigit = "";
                    break;
                case 1:
                    sDigit = "萬";
                    break;
                case 2:
                    sDigit = "億";
                    break;
                case 3:
                    sDigit = "兆";
                    break;
                case 4:
                    sDigit = "京";
                    break;
                case 5:
                    sDigit = "垓";
                    break;
                case 6:
                    sDigit = "秭";
                    break;
                case 7:
                    sDigit = "穰";
                    break;
                case 8:
                    sDigit = "溝";
                    break;
                case 9:
                    sDigit = "澗";
                    break;
                case 10:
                    sDigit = "正";
                    break;
                case 11:
                    sDigit = "載";
                    break;
                case 12:
                    sDigit = "極";
                    break;
                default:
                    sDigit = "∞";
                    break;
            }

            return sDigit;
        }
        #endregion

        #region GetChNumber() 整數轉中文數字 （10050 => 一萬零五十）
        public string GetChNumber(int mInt)
        {
            return GetChNumber(mInt.ToString());
        }

        public string GetChNumber(Int64 mInt)
        {
            return GetChNumber(mInt.ToString());
        }

        public string GetChNumber(UInt64 mInt)
        {
            return GetChNumber(mInt.ToString());
        }

        public string GetChNumber(String NumStr)
        {
            string ChStr = "", ChSubStr = "", tmpStr = "";
            int iCnt = 0, jCnt = 0, kCnt = 0, lCnt = -1, nCnt = 0;

            if (!IsInteger(NumStr))
                NumStr = "0";

            nCnt = NumStr.Length;

            // 中文數字以四位數為一個處理單位(萬、億、兆、京....)
            iCnt = nCnt % 4;
            NumStr = Duplicate("0", 4 - iCnt) + NumStr;
            nCnt = NumStr.Length;

            for (iCnt = 0; iCnt < nCnt; iCnt += 4)
            {
                lCnt++;
                ChSubStr = "";

                for (jCnt = 0; jCnt < 4; jCnt++)
                {
                    kCnt = nCnt - iCnt - jCnt - 1;
                    tmpStr = ChNumber(NumStr.Substring(kCnt, 1));

                    if (tmpStr == "零")
                    {
                        if (Left(ChSubStr, 1) != "零" && Left(ChSubStr, 1) != "")
                            ChSubStr = tmpStr + ChSubStr;
                    }
                    else
                    {
                        switch (jCnt)
                        {
                            case 0:
                                ChSubStr = tmpStr;
                                break;
                            case 1:
                                ChSubStr = tmpStr + "十" + ChSubStr;
                                break;
                            case 2:
                                ChSubStr = tmpStr + "百" + ChSubStr;
                                break;
                            case 3:
                                ChSubStr = tmpStr + "千" + ChSubStr;
                                break;
                            default:
                                ChSubStr = tmpStr + "∞" + ChSubStr;
                                break;
                        }
                    }
                }

                if (ChSubStr == "零" && Left(ChStr, 1) != "" && Left(ChStr, 1) != "零")
                    ChStr = "零" + ChStr;
                else
                {
                    if (ChSubStr != "")
                    {
                        // 取得10000的次方中文數字
                        // 限於整數態的上限，「京」之後的中文數字是用不到
                        ChStr = ChSubStr + GetFourChNumber(lCnt) + ChStr;
                    }
                }
            }

            if (ChStr == "")
                ChStr = "零";
            else if (Left(ChStr, 1) == "零")
                ChStr = ChStr.Substring(1, ChStr.Length - 1);

            return ChStr;
        }
        #endregion

        #region GetChNumberFill() 整數轉中文數字 （10050 => 一萬零千零百五十零）
        public string GetChNumberFill(int mInt)
        {
            return GetChNumberFill(mInt.ToString());
        }

        public string GetChNumberFill(Int64 mInt)
        {
            return GetChNumberFill(mInt.ToString());
        }

        public string GetChNumberFill(UInt64 mInt)
        {
            return GetChNumberFill(mInt.ToString());
        }

        public string GetChNumberFill(string NumStr)
        {
            string ChStr = "", ChSubStr = "", tmpStr = "";
            int iCnt = 0, jCnt = 0, kCnt = 0, lCnt = -1, nCnt = 0;

            if (!IsInteger(NumStr))
                NumStr = "0";
            nCnt = NumStr.Length;

            // 中文數字以四位數為一個處理單位(萬、億、兆、京....)
            iCnt = nCnt % 4;
            NumStr = Duplicate("0", 4 - iCnt) + NumStr;
            nCnt = NumStr.Length;

            for (iCnt = 0; iCnt < nCnt; iCnt += 4)
            {
                lCnt++;
                ChSubStr = "";

                for (jCnt = 0; jCnt < 4; jCnt++)
                {
                    kCnt = nCnt - iCnt - jCnt - 1;
                    tmpStr = ChNumber(NumStr.Substring(kCnt, 1));

                    switch (jCnt)
                    {
                        case 0:
                            ChSubStr = tmpStr;
                            break;
                        case 1:
                            ChSubStr = tmpStr + "十" + ChSubStr;
                            break;
                        case 2:
                            ChSubStr = tmpStr + "百" + ChSubStr;
                            break;
                        case 3:
                            ChSubStr = tmpStr + "千" + ChSubStr;
                            break;
                        default:
                            ChSubStr = tmpStr + ChSubStr;
                            break;
                    }
                }

                if (ChSubStr != "")
                {
                    // 取得10000的次方中文數字
                    // 限於整數態的上限，「京」之後的中文數字是用不到
                    ChStr = ChSubStr + GetFourChNumber(lCnt) + ChStr;
                }
            }

            while (Left(ChStr, 1) == "零" && ChStr.Length > 1)
            {
                if (Left(ChStr, 1) == "零")
                    ChStr = ChStr.Substring(2, ChStr.Length - 2);
            }

            return ChStr;
        }
        #endregion

        #region GetChNumberShort() 整數轉簡略中文數字 (10050 => 一零零五零)
        public string GetChNumberShort(int mInt)
        {
            return GetChNumberShort(mInt.ToString());
        }

        public string GetChNumberShort(Int64 mInt)
        {
            return GetChNumberShort(mInt.ToString());
        }

        public string GetChNumberShort(UInt64 mInt)
        {
            return GetChNumberShort(mInt.ToString());
        }

        public string GetChNumberShort(string NumStr)
        {
            string ChStr = "";
            int iCnt = 0, nCnt = 0;

            if (!IsInteger(NumStr))
                NumStr = "0";
            nCnt = NumStr.Length;

            for (iCnt = 0; iCnt < nCnt; iCnt++)
            {
                ChStr += ChNumber(NumStr[iCnt].ToString());
            }

            return ChStr;
        }
        #endregion

        #region GetChCapitalNumber() 整數轉中文大寫數字（10050 => 壹萬零伍拾）
        public string GetChCapitalNumber(int mInt)
        {
            return GetChCapitalNumber(mInt.ToString());
        }

        public string GetChCapitalNumber(Int64 mInt)
        {
            return GetChCapitalNumber(mInt.ToString());
        }

        public string GetChCapitalNumber(UInt64 mInt)
        {
            return GetChCapitalNumber(mInt.ToString());
        }

        public string GetChCapitalNumber(string NumStr)
        {
            string ChStr = "", ChSubStr = "", tmpStr = "";
            int iCnt = 0, jCnt = 0, kCnt = 0, lCnt = -1, nCnt = 0;

            if (!IsInteger(NumStr))
                NumStr = "0";
            nCnt = NumStr.Length;

            // 中文數字以四位數為一個處理單位(萬、億、兆、京....)
            iCnt = nCnt % 4;
            NumStr = Duplicate("0", 4 - iCnt) + NumStr;
            nCnt = NumStr.Length;

            for (iCnt = 0; iCnt < nCnt; iCnt += 4)
            {
                lCnt++;
                ChSubStr = "";

                for (jCnt = 0; jCnt < 4; jCnt++)
                {
                    kCnt = nCnt - iCnt - jCnt - 1;
                    tmpStr = ChCapitalNumber(NumStr.Substring(kCnt, 1));

                    if (tmpStr == "零")
                    {
                        if (Left(ChSubStr, 1) != "零" && Left(ChSubStr, 1) != "")
                            ChSubStr = tmpStr + ChSubStr;
                    }
                    else
                    {
                        switch (jCnt)
                        {
                            case 0:
                                ChSubStr = tmpStr;
                                break;
                            case 1:
                                ChSubStr = tmpStr + "拾" + ChSubStr;
                                break;
                            case 2:
                                ChSubStr = tmpStr + "佰" + ChSubStr;
                                break;
                            case 3:
                                ChSubStr = tmpStr + "仟" + ChSubStr;
                                break;
                            default:
                                ChSubStr = tmpStr + ChSubStr;
                                break;
                        }
                    }
                }

                if (ChSubStr == "零" && Left(ChStr, 1) != "" && Left(ChStr, 1) != "零")
                    ChStr = "零" + ChStr;
                else
                {
                    if (ChSubStr != "")
                    {
                        // 取得10000的次方中文數字
                        // 限於整數態的上限，「京」之後的中文數字是用不到
                        ChStr = ChSubStr + GetFourChNumber(lCnt) + ChStr;
                    }
                }
            }

            if (ChStr == "")
                ChStr = "零";
            else if (Left(ChStr, 1) == "零")
                ChStr = ChStr.Substring(1, ChStr.Length - 1);

            return ChStr;
        }
        #endregion

        #region GetChCapitalNumberFill() 整數轉中文大寫數字 （10050 => 壹萬零仟零佰伍拾零）
        public string GetChCapitalNumberFill(int mInt)
        {
            return GetChCapitalNumberFill(mInt.ToString());
        }

        public string GetChCapitalNumberFill(Int64 mInt)
        {
            return GetChCapitalNumberFill(mInt.ToString());
        }

        public string GetChCapitalNumberFill(UInt64 mInt)
        {
            return GetChCapitalNumberFill(mInt.ToString());
        }

        public string GetChCapitalNumberFill(string NumStr)
        {
            string ChStr = "", ChSubStr = "", tmpStr = "";
            int iCnt = 0, jCnt = 0, kCnt = 0, lCnt = -1, nCnt = 0;

            if (!IsInteger(NumStr))
                NumStr = "0";
            nCnt = NumStr.Length;

            // 中文數字以四位數為一個處理單位(萬、億、兆、京....)
            iCnt = nCnt % 4;
            NumStr = Duplicate("0", 4 - iCnt) + NumStr;
            nCnt = NumStr.Length;

            for (iCnt = 0; iCnt < nCnt; iCnt += 4)
            {
                lCnt++;
                ChSubStr = "";

                for (jCnt = 0; jCnt < 4; jCnt++)
                {
                    kCnt = nCnt - iCnt - jCnt - 1;
                    tmpStr = ChCapitalNumber(NumStr.Substring(kCnt, 1));

                    switch (jCnt)
                    {
                        case 0:
                            ChSubStr = tmpStr;
                            break;
                        case 1:
                            ChSubStr = tmpStr + "拾" + ChSubStr;
                            break;
                        case 2:
                            ChSubStr = tmpStr + "佰" + ChSubStr;
                            break;
                        case 3:
                            ChSubStr = tmpStr + "仟" + ChSubStr;
                            break;
                        default:
                            ChSubStr = tmpStr + ChSubStr;
                            break;
                    }
                }

                if (ChSubStr != "")
                {
                    // 取得10000的次方中文數字
                    // 限於整數態的上限，「京」之後的中文數字是用不到
                    ChStr = ChSubStr + GetFourChNumber(lCnt) + ChStr;
                }
            }

            while (Left(ChStr, 1) == "零" && ChStr.Length > 1)
            {
                if (Left(ChStr, 1) == "零")
                    ChStr = ChStr.Substring(2, ChStr.Length - 2);
            }

            return ChStr;
        }
        #endregion

        #region GetChCapitalNumberShort() 整數轉簡略中文大寫數字 (10050 => 壹零零伍零)
        public string GetChCapitalNumberShort(int mInt)
        {
            return GetChCapitalNumberShort(mInt.ToString());
        }

        public string GetChCapitalNumberShort(Int64 mInt)
        {
            return GetChCapitalNumberShort(mInt.ToString());
        }

        public string GetChCapitalNumberShort(UInt64 mInt)
        {
            return GetChCapitalNumberShort(mInt.ToString());
        }

        public string GetChCapitalNumberShort(string NumStr)
        {
            string ChStr = "";
            int iCnt = 0, nCnt = 0;

            if (!IsInteger(NumStr))
                NumStr = "0";
            nCnt = NumStr.Length;

            for (iCnt = 0; iCnt < nCnt; iCnt++)
            {
                ChStr += ChCapitalNumber(NumStr[iCnt].ToString());
            }

            return ChStr;
        }
        #endregion
    }


}
