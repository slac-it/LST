//$Header:$
//
//  SLSOLabSummary.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the usercontrol that has slso roles and their corresponding facilities
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


namespace LST.UserControls
{
    public partial class SLSOLabSummary : System.Web.UI.UserControl
    {
        Data.DML_Util objDml = new Data.DML_Util();
        Business.UserRoles objRoles = new Business.UserRoles();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                BindGrid();
            }
           
        }

        #region "Properties"
        public string UserRoleId
        { 
            get
            {
                return (ViewState["userroleid"] != null) ? ViewState["userroleid"].ToString() : "";
            }
            
            set
            {
                ViewState["userroleid"] = value;
            }
            
         }

        public string SlacId
        {
            get;
            set;
        }

        #endregion

        #region "Gridview Events"
        protected void BindGrid()
        {
             DataSet _dsSLSOLab = new DataSet();

            _dsSLSOLab = objDml.GetSLSOLabInfo(Convert.ToInt32(UserRoleId), true, SlacId );

            if (_dsSLSOLab.Tables["slsolabs"].Rows.Count > 0)
            {
                GvSLSO.DataSource = _dsSLSOLab.Tables["slsolabs"];
                GvSLSO.DataBind();
            }
            else
            {
                GvSLSO.DataSource = null;
                GvSLSO.DataBind();
            }
        }

        protected void GvSLSO_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "View")
            {
                int _facilityId = Convert.ToInt32(e.CommandArgument.ToString());
                Response.Redirect("Facility.aspx?mode=view&id=" + _facilityId);
            }
         }

        protected void GvSLSO_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView _row = (DataRowView)e.Row.DataItem;
                string _altslso;
                int _facId;

                _altslso = _row["ALTERNATE_SLSO"].ToString();
                _facId = Convert.ToInt32(_row["FACILITY_ID"]);
                Label LblSLSO = (Label)e.Row.FindControl("LblAltSLSO");
                LinkButton LnkDesignate = (LinkButton)e.Row.FindControl("LnkDesignate");

                if (_altslso != "")
                {
                    LblSLSO.Visible = true;
                    LnkDesignate.Visible = false;
                }
                else
                {
                    LblSLSO.Visible = false;
                    LnkDesignate.Visible = true;
                    LnkDesignate.Attributes.Add("onClick", "OpenJQueryDialog('dialogDesignate','700','600','Designate.aspx?type=slso&id= " + _facId + "');return false;");
                }
            }
        }
        #endregion

    }
}