using System;
using System.Configuration;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using log4net;



namespace LST.SSO
{
    public class SSO_Util
    {

        private static string _loginSID = string.Empty;
        private static string _loginName = string.Empty;
        private static string _loginEmail = string.Empty;

        public string LoginSID
        {
            get { return _loginSID; }
            set { _loginSID = value; }
        }

        public string LoginName
        {
            get { return _loginName; }
            set { _loginName = value; }
        }

        public string LoginEmail
        {
            get { return _loginEmail; }
            set { _loginEmail = value; }
        }



        protected static readonly ILog Log = LogManager.GetLogger(typeof(SSO_Util));
        string _connStr = ConfigurationManager.ConnectionStrings["TCP_WEB"].ConnectionString;


        public void getSSOSID()
        {
            string _isUAT = System.Web.Configuration.WebConfigurationManager.AppSettings["Prodserver"];
            if (_isUAT == "N")
            {
                if (HttpContext.Current.Request["slacid"] != null && HttpContext.Current.Request["slacid"] != "")
                {
                    string _user = HttpContext.Current.Request["slacid"].ToString();
                    LoginSID = _user;
                    HttpContext.Current.Response.Write(@"User: " + _user + "  | ");
                    login2SID(_user);
                    //getIsOwner(_user);
                }
                else
                {
                    SSOsid();
                }
            }
            else
            {
                SSOsid();
            }

        }

        private void SSOsid()
        {
            var _attSid = HttpContext.Current.Request.ServerVariables["SSO_SID"];
            HttpContext.Current.Response.Write(@"SID: " + _attSid + "  | ");
            Log.Info("Get SID:" + _attSid);
            if (_attSid != null)
            {
                if (_attSid.IndexOf(";") != -1)
                {
                    _attSid = _attSid.Substring(0, _attSid.IndexOf(";"));
                    LoginSID = _attSid;
                    HttpContext.Current.Session["LoginSID"] = _attSid;
                }
                else
                {
                    HttpContext.Current.Session["LoginSID"] = _attSid;
                    LoginSID = _attSid;
                }
            }
            else
            {
                HttpContext.Current.Session["LoginSID"] = "";
                LoginSID = "";
            }

            var loginName = HttpContext.Current.Request.ServerVariables["SSO_FIRSTNAME"];
            HttpContext.Current.Response.Write(@"Login Name: " + loginName + "  | ");
            Log.Info("Login Name:" + loginName);

            if (loginName != null)
            {
                if (loginName.IndexOf(";") != -1)
                {
                    loginName = loginName.Substring(0, loginName.IndexOf(";"));
                    if (loginName.IndexOf("@") != -1)
                    {
                        HttpContext.Current.Session["LoginName"] = loginName.Substring(0, loginName.IndexOf("@"));
                        LoginName = loginName.Substring(0, loginName.IndexOf("@"));
                    }
                    else
                    {
                        HttpContext.Current.Session["LoginName"] = loginName;
                        LoginName = loginName;
                    }
                }
                else
                {
                    if (loginName.IndexOf("@") != -1)
                    {
                        HttpContext.Current.Session["LoginName"] = loginName.Substring(0, loginName.IndexOf("@"));
                        LoginName = loginName.Substring(0, loginName.IndexOf("@"));
                    }
                    else
                    {
                        HttpContext.Current.Session["LoginName"] = loginName;
                        LoginName = loginName;
                    }

                }
            }
            else
            {
                HttpContext.Current.Session["LoginName"] = "None";
            }

            var loginEmail = HttpContext.Current.Request.ServerVariables["SSO_EMAIL"];
            HttpContext.Current.Response.Write(@"Email: " + loginEmail);
            Log.Info("Login Email:" + loginEmail);

            if (loginEmail != null)
            {
                if (loginEmail.IndexOf(";") != -1)
                {
                    loginEmail = loginEmail.Substring(0, loginEmail.IndexOf(";"));
                    HttpContext.Current.Session["LoginEmail"] = loginEmail;
                    LoginEmail = loginEmail;
                }
                else
                {
                    HttpContext.Current.Session["LoginEmail"] = loginEmail;
                    LoginEmail = loginEmail;
                }
            }
            else
            {
                HttpContext.Current.Session["LoginEmail"] = "None";
                LoginEmail = "None";
            }

            //Session["IS_OWNER"] = "1";
            //getIsOwner(_attSid);
        }


        //private void getIsOwner(string oSID)
        //{
        //    //string ownerSID = Session["LoginSID"].ToString();
        //    string ownerSID = oSID;

        //    string _sql = @"select count(*) as isOwner from siims_rir_reviewer where reviewer_sid=" + ownerSID + @" and is_active='Y' and is_owner='Y'";

        //    try
        //    {
        //        OracleConnection con = new OracleConnection();
        //        con.ConnectionString = _connStr;
        //        con.Open();

        //        OracleCommand cmd = con.CreateCommand();
        //        cmd.CommandText = _sql;
        //        OracleDataReader reader = cmd.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            HttpContext.Current.Session["IS_OWNER"] = reader.GetInt32(0).ToString();
        //        }

        //        reader.Close();
        //        con.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error("Is Owner for login=" + HttpContext.Current.Session["LoginName"].ToString(), ex);
        //    }

        //}


        public void login2SID(string loginSID)
        {

            string _sql = @"Select KEY, FNAME, SID_EMAIL from Persons.person Where KEY=lower(:login)";

            try
            {
                OracleConnection con = new OracleConnection();
                con.ConnectionString = _connStr;
                con.Open();

                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = _sql;
                cmd.BindByName = true;
                cmd.Parameters.Add(":login", OracleDbType.Varchar2).Value = loginSID;

                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    HttpContext.Current.Session["LoginSID"] = reader.GetInt32(0).ToString();
                    LoginSID = reader.GetInt32(0).ToString();
                    HttpContext.Current.Session["LoginName"] = reader.GetString(1);
                    LoginName = reader.GetString(1);
                    HttpContext.Current.Response.Write(@"Login Name: " + HttpContext.Current.Session["LoginName"].ToString() + "  | ");
                    HttpContext.Current.Session["LoginEmail"] = reader.GetString(2) == null ? "" : reader.GetString(2);
                    LoginEmail = reader.GetString(2) == null ? "" : reader.GetString(2);
                }

                reader.Close();
                con.Close();
            }
            catch (Exception ex)
            {
                Log.Error("login2SID for login=" + loginSID, ex);
            }
        }



    }
}