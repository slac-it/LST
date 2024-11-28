//$Header:$
//
//  LST.Master.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the master page of Laser safety tool.
//

using log4net;
using LST.SSO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LST
{
    public partial class SiteMaster : MasterPage
    {

        public string UserName;
        Business.Common_Util objCommon = new Business.Common_Util();
        Business.UserRoles objRoles = new Business.UserRoles();
        SSO_Util objSSO = new SSO_Util();
        protected static readonly ILog Log = LogManager.GetLogger(typeof(SiteMaster));


        protected void Page_Load(object sender, EventArgs e)
        {
            string _userId = string.Empty;

            if(HttpContext.Current.Session["LoginSID"] != null)
            {
                _userId = HttpContext.Current.Session["LoginSID"].ToString();
            }
            else
            {
                if(objSSO.LoginSID != "")
                {
                    _userId = objSSO.LoginSID;
                }
                else
                {
                    Response.Redirect("Error.aspx");
                }
            }
            Log.Info("Site Master - Login SID: " + _userId);

            //if (_userId == "err") { Response.Redirect("Error.aspx"); }

            if (_userId != "")
            {
                UserName = objCommon.GetFullName(objCommon.GetEmpname(_userId));
                string _isProd = System.Configuration.ConfigurationManager.AppSettings["Prodserver"].ToString();
                if (_isProd == "N")
                {
                    DivTest.Visible = true;
                }

                else
                {
                    DivTest.Visible = false;
                }
            }
            SetAccessibility();

        }

        //SLSO, LSO, DLSO, AltSLSO, Admin have access to all top menu items 
        // Other users like anyone with windows a/c  cannot access admin or reports 
        //Admin supervisor or alternate have access to all except admin
        private void SetAccessibility()
        {
            string loginSID = String.Empty;
            if (HttpContext.Current.Session["LoginSID"] != null)
            {
                loginSID = HttpContext.Current.Session["LoginSID"].ToString();
            }
            else
            {
                loginSID = objSSO.LoginSID;
            }

            Log.Info("Site Master - SetAccessibility - Login SID: " + loginSID);

            objRoles.GetUserRole(loginSID, 0);

            if (( objRoles.IsLSOrAlt() || objRoles.IsAdmin || objRoles.IsDLSO))
            {
                aAdmin.Visible = true;
                aEmail.Visible = true;
            }
            else
            {
                aAdmin.Visible = false;
                if (objRoles.CheckIfSLSO())
                {
                    aEmail.Visible = true;
                }
                else
                {
                    aEmail.Visible = false;
                }
            }
           
               

        }

        protected string MatchesToCurrentLocation(string href)
        {
            if (HttpContext.Current.Request.FilePath.ToLower().Contains("facilityapprovalrequest.aspx"))
            {
                return "";
            }
            else if (HttpContext.Current.Request.FilePath.ToLower().Contains(href.ToLower()))
            {  return "active"; }
            else if (((HttpContext.Current.Request.FilePath.ToLower().Contains("workerentry.aspx")) || (HttpContext.Current.Request.FilePath.ToLower().Contains("facility.aspx")))   && (href=="admin.aspx"))
            {
                return "active";
            }
            
            else
            {  return "";}
        }
    }
}