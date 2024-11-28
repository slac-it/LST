//$Header:$
//
//  ApprovalRequestSummary.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the usercontrol that has the list of requests pending for an approver to approve
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;


namespace LST.UserControls
{
    public partial class ApprovalRequestSummary : System.Web.UI.UserControl
    {
        Data.DML_Util objDml = new Data.DML_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        Business.UserRoles objRoles = new Business.UserRoles();

        public string ApproverType;
        bool _check;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindGrid();

                if (UserType == "SLSO")
                {
                    _check = objRoles.CheckifinRole(Convert.ToInt32(HttpContext.Current.Session["LoginSID"]), Business.UserRoles.UserType.ALTSLSO, 0, 0);
                    if (_check)
                    {
                        ApproverType = " SLSO (or as a designated alternate SLSO) ";
                    }
                    else
                    {
                        ApproverType = " SLSO  ";
                    }

                }
                else if (UserType == "LSO")
                {
                    if (IsDLSOonly)
                    {
                        SpnDlso.Visible = true;
                        SpnInfo.Visible = false;
                    }
                    else
                    {
                        SpnDlso.Visible = false;
                        SpnInfo.Visible = true;
                        ApproverType = " LSO ";
                        _check = objRoles.CheckifinRole(Convert.ToInt32(HttpContext.Current.Session["LoginSID"]), Business.UserRoles.UserType.ALTLSO, 0, 0);
                        if (_check)
                        {
                            ApproverType = " LSO (or as a designated alternate LSO) ";

                        }
                        else
                        {
                            ApproverType = " LSO ";
                        }
                    }

                }
                else if (UserType == "ADMSVR")
                {
                    _check = objRoles.CheckifinRole(Convert.ToInt32(HttpContext.Current.Session["LoginSID"]), Business.UserRoles.UserType.ALTSVR, 0, 0);
                    if (_check)
                    {
                        ApproverType = "Admin Supervisor (or as a designated alternate Supervisor) ";
                    }
                    else
                    {
                        ApproverType = "Admin Supervisor ";
                    }
                }
            }
           
        }

        #region "Properties"
        public int UserRoleId
        {
            get;
            set;
        }

        public string UserType
        {
            get;
            set;
        }
        public string SlacId
        {
            get;
            set;
        }

        public bool IsDLSOonly
        {
            get;
            set;
        }
        #endregion

        protected void BindGrid()
        {
            using (OracleDataReader _drRequest = objDml.GetApprovalRequestDetails(UserType, UserRoleId, SlacId))
            {
                if (_drRequest != null)
                {
                    GvApprovalReq.DataSource = _drRequest;
                    GvApprovalReq.DataBind();
                }
                else
                {
                    GvApprovalReq.Visible = false;
                    SpnInfo.Visible = false;
                    PnlApprovalReq.Visible = false;
                }
            }        
        }

     
    }

 
}