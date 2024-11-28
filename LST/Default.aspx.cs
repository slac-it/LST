//$Header:$
//
//  Default.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the default page of Laser safety tool. This has the dashboard based on the user's access
//

using log4net;
using LST.Business;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;

//using System.Data.OracleClient;

namespace LST
{
    public partial class _Default : BasePage
    {

        protected static readonly ILog Log = LogManager.GetLogger(typeof(_Default));
        public string UserName;
        Business.Common_Util objCommon = new Business.Common_Util();
        Business.UserRoles objRoles = new Business.UserRoles();
        SSO.SSO_Util objSSO = new SSO.SSO_Util();
        Data.DML_Util objDml = new Data.DML_Util();


        public bool IsDLSOOnly
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                // login SSO info
                string _userId = string.Empty;
                objSSO.getSSOSID();
                Log.Info("Login SID: " + HttpContext.Current.Session["LoginSID"].ToString());

                // end login SSO info


                //string _userId = objCommon.GetUserId();
                _userId = (HttpContext.Current.Session["LoginSID"] != null) ? HttpContext.Current.Session["LoginSID"].ToString() : "err";
                //string _userId = (getVar.AttSid != null) ? getVar.AttSid.ToString() : "err";
                if (_userId == "err") { Response.Redirect("Error.aspx"); }

                    Master.FindControl("DivName").Visible = false;
                    if (_userId != "")
                    {
                        //UserName = objCommon.GetFullName(objCommon.GetEmpname(_userId));
                        UserName = HttpContext.Current.Session["LoginName"].ToString();
                }
                    SetAccessibility();
                    SetPage();

            }


        }

        private void SetAccessibility()
        {
            objRoles.GetUserRole(HttpContext.Current.Session["LoginSID"].ToString(), 0);
           
           
           if ((objRoles.IsSLSOGen) || (objRoles.IsAltSLSOGen))
           {
               DivSLSO.Visible = true;
               DivSLSOApp.Visible = true;
           }
           else
           {
               DivSLSO.Visible = false;
               DivSLSOApp.Visible = false;
           }

            if ((objRoles.IsLSOrAlt() || objRoles.IsDLSO))
            {
                DivLSO.Visible = true;
                if ((objRoles.IsDLSO) && (!objRoles.IsLSOrAlt())) IsDLSOOnly = true; else IsDLSOOnly = false;
            }
            else
            {
                DivLSO.Visible = false;
            }

            if ((objRoles.IsAltSVRGen) || (objRoles.IsAdminSvrGen))
            {
                DivAdmSVR.Visible = true;
            }
            else
            {
                DivAdmSVR.Visible = false;
            }

            if (objRoles.IsPrgMgrGen)
            {
                DivPM.Visible = true;
            }
            else
            {
                DivPM.Visible = false;
            }
            if (objRoles.IsEshCoordGen)
            {
                DivCoord.Visible = true;               
            }
            else
            {
                DivCoord.Visible = false;
            }

        }

        private void SetPage()
        {
            string _slacId = HttpContext.Current.Session["LoginSID"].ToString();
            Log.Info("SLAC ID: " + _slacId);
            string _userroleid = objDml.GetUserRoleId(_slacId, 15).ToString();
            Log.Info("User Role ID: " + _userroleid);

            UCSLSOSum.UserRoleId = _userroleid;
            UCSLSOSum.SlacId = _slacId;

            UCUserSum.UserId = _slacId;
            UCUserSum.Mode = "view";
            UCUserSum.ShowText = false;
            if (DivSLSOApp.Visible)
            {
                UCReqSum.UserType = "SLSO";
                UCReqSum.UserRoleId = Convert.ToInt32(_userroleid);
                UCReqSum.SlacId = _slacId;
                UCFacReqSum.UserType = "SLSO";
                UCFacReqSum.UserRoleId = Convert.ToInt32(_userroleid);
            }
            if (DivLSO.Visible)
            {
                UCReqSumLSO.UserType = "LSO";
                UCReqSumLSO.IsDLSOonly = IsDLSOOnly;
                UCFacReqSumLSO.UserType = "LSO";
                UCFacReqSumLSO.IsDLSOonly = IsDLSOOnly;
               
            }
            if (DivAdmSVR.Visible)
            {
                UCReqSumSVR.UserType = "ADMSVR";
                UCReqSumSVR.SlacId = _slacId;
            }

            if (DivPM.Visible)
            {
                UCFacReqSumPM.UserType = "PRGMGR";
                UCFacReqSumPM.SlacId = _slacId;
            }

            if (DivCoord.Visible)
            {
                UCFacReqSumCoord.UserType = "ESHCOORD";
                UCFacReqSumCoord.SlacId = _slacId;
            }

            UCPending1.WorkerId = objDml.GetWorkerId(HttpContext.Current.Session["LoginSID"].ToString());
            UCPending1.ShowText = false;
        }

       

    }
}