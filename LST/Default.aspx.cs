//$Header:$
//
//  Default.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the default page of Laser safety tool. This has the dashboard based on the user's access
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Data.OracleClient;

namespace LST
{
    public partial class _Default : BasePage
    {
        public string UserName;
        Business.Common_Util objCommon = new Business.Common_Util();
        Business.UserRoles objRoles = new Business.UserRoles();
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
                    string _userId = objCommon.GetUserId();
                    if (_userId == "err") { Response.Redirect("Error.aspx"); }
                    Master.FindControl("DivName").Visible = false;
                    if (_userId != "")
                    {
                        UserName = objCommon.GetFullName(objCommon.GetEmpname(_userId));
                    }
                    SetAccessibility();
                    SetPage();
                   
                }
                
 
        }

        private void SetAccessibility()
        {
            objRoles.GetUserRole(objCommon.GetUserId().ToString(), 0);
           
           
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
            string _slacId = objCommon.GetUserId();
            string _userroleid = objDml.GetUserRoleId(_slacId, 15).ToString();

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

            UCPending1.WorkerId = objDml.GetWorkerId(objCommon.GetUserId());
            UCPending1.ShowText = false;
        }

       

    }
}