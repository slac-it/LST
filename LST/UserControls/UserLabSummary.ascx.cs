//$Header:$
//
//  USerLabSummary.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the usercontrol that has list of facilities for a user
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;

namespace LST.UserControls
{
    public partial class UserLabSummary : System.Web.UI.UserControl
    {
        Data.DML_Util objDml = new Data.DML_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        Business.UserRoles objRoles = new Business.UserRoles();

        #region "Properties"
        public string UserId {
            get
            {
                if (ViewState["userid"] != null)
                {
                    return (string)ViewState["userid"];
                }
                else return "";
            }
            set
            {
                ViewState["userid"] = value;
            }
        }

        public string Mode
        {
            get;
            set;
        }

        public bool ShowText
        {
            get;
            set;

        }
        public bool HideEdit
        {
            get;
            set;
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindOJTGrid();
            }
           
            if (ShowText)
            {
                PnlUserLab.GroupingText = "Facilities Associated";
            }
            else { PnlUserLab.GroupingText = ""; }
          
        }

        #region "Gridview Events"

        public void BindOJTGrid()
        {
            using (OracleDataReader _drWorkerFac =  objDml.GetWorkerLabInfo(UserId))
            {
                if (_drWorkerFac.HasRows)
                {
                    GvUserLabs.DataSource = _drWorkerFac;
                   
                }
                else
                {
                    GvUserLabs.DataSource = null;
                }
                GvUserLabs.DataBind();
              
            }            
        }

        protected void GvUserLabs_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GvUserLabs.EditIndex = -1;
            BindOJTGrid();
        }

        protected void GvUserLabs_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GvUserLabs.EditIndex = e.NewEditIndex;
            BindOJTGrid();
        }

        protected void GvUserLabs_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                e.KeepInEditMode = true;
            }
        }

        protected void GvUserLabs_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int _mapId;
            TextBox TxtSOP;

            int _result;
          
            if (Page.IsValid)
            {
                GridViewRow row = GvUserLabs.Rows[e.RowIndex];
                TxtSOP = (TextBox)row.FindControl("TxtSOP");

                _mapId = Convert.ToInt32(GvUserLabs.DataKeys[e.RowIndex].Values[0]);
             
                //check if date reviewd is later than the sop date for the facility, if not give warning
                DateTime _dtSOP = Convert.ToDateTime(TxtSOP.Text);
                DateTime _dtSOPFac = objDml.GetSOPReviewDate(_mapId);

                if (_dtSOP < _dtSOPFac)
                {
                    LblMsg.Text = "Error: SOP review date should fall after current SOP approval  date for the facility which is  " + _dtSOPFac.ToShortDateString();
                }
                else
                {
                    LblMsg.Text = "";
                    _result = objDml.UpdateSOP(_mapId, _dtSOP, HttpContext.Current.Session["LoginSID"].ToString());
                    if (_result != 0)
                    {
                        LblMsg.Text = "Error updating SOP review Date for this facility";
                    }
                }
                
               if (LblMsg.Text != "")
                   Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-labmsg');", true);
               
                GvUserLabs.EditIndex = -1;
               // LblMsg.Text = "";
                BindOJTGrid();

            }
            else
            {
                e.Cancel = true;
            }
        }

        #endregion

        protected void GvUserLabs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string _facname = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "FACILITY_NAME"));
              
                LinkButton LnkEdit = (LinkButton)e.Row.FindControl("cmdEdit");
                if (LnkEdit != null)
                {
                    if ((_facname != "") && (!HideEdit))
                    {
                        LnkEdit.Visible = true;
                    }
                    else
                    {
                        LnkEdit.Visible = false;
                    }
                }
               
            }
        }

    
    }
}