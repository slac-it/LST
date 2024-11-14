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
    public partial class FacilityApprovalRequestSummary : System.Web.UI.UserControl
    {
        Data.DML_Util objDml = new Data.DML_Util();
        Business.UserRoles objRoles = new Business.UserRoles();
        Business.Common_Util objCommon = new Business.Common_Util();

        public string ApproverType;
        bool _check;

        public string UserType
        {
            get
            {
                return (ViewState["usertype"] != null) ? ViewState["usertype"].ToString() : "";
            }
            set
            {
                ViewState["usertype"] = value;
            }
        }

        public int UserRoleId
        {
            get;
            set;
        }

        public bool IsDLSOonly
        {
            get;
            set;
        }

        public string SlacId
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                if (UserType == "LSO")
                {
                    if (IsDLSOonly)
                    {
                        SpnDlso.Visible = true;
                        SpnInfo.Visible = false;
                        SpnInfoSLSO.Visible = false;
                    }
                    else
                    {
                        SpnDlso.Visible = false;
                        SpnInfo.Visible = true;
                        SpnInfoSLSO.Visible = false;
                        ApproverType = "LSO";
                        _check = objRoles.CheckifinRole(Convert.ToInt32(objCommon.GetUserId()), Business.UserRoles.UserType.ALTLSO, 0, 0);
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
                else if (UserType.Equals("SLSO"))
                {
                    ApproverType = "SLSO";
                    SpnInfoSLSO.Visible = true;
                    SpnInfo.Visible = false;
                }
                else if (UserType.Equals("PRGMGR"))
                {
                    ApproverType = "Program Manager";
                    SpnInfoSLSO.Visible = false;
                    SpnInfo.Visible = true;
                }
                else if (UserType.Equals("ESHCOORD"))
                {
                    ApproverType = "ESH Co-ordinator";
                    SpnInfoSLSO.Visible = false;
                    SpnInfo.Visible = true;
                }
                BindGrid();
            }
          

        }

        protected void BindGrid()
        {
            using (OracleDataReader _drRequest = objDml.GetfacApprovalRequestDetails(UserType, UserRoleId, SlacId))
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
                    SpnInfoSLSO.Visible = false;
                    PnlFACApprovalReq.Visible = false;
                 }
            }
        }
    }

   
}