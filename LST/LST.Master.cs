//$Header:$
//
//  LST.Master.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the master page of Laser safety tool.
//

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

        protected void Page_Load(object sender, EventArgs e)
        {
            string _userId;

            _userId = objCommon.GetUserId();

            if (_userId == "err") { Response.Redirect("Error.aspx"); }

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
            objRoles.GetUserRole(objCommon.GetUserId().ToString(), 0);

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