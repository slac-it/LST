using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Text;
namespace LST
{
    public partial class OJTFields : System.Web.UI.Page
    {
        public string str;
        Data.DML_Util objDml = new Data.DML_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        
        protected void Page_Load(object sender, EventArgs e)
        {
           
        //Check if SLSO of the facility
          str = "^[^\".]*$";
            
            string _facid =  (Request.QueryString["facid"] != null) ? Request.QueryString["facid"].ToString() : "0";
            int _ifacid;
            if (Int32.TryParse(_facid, out _ifacid))
            {
                FacID = _ifacid;
            }
            else
            {
                Response.Redirect("Error.aspx?msg=parserr");
            }

            BtnClose.Attributes.Add("onClick", "DialogCloseNRefresh('dialogOJTFields');return false;");
            if (!Page.IsPostBack)
            {
                
                BindGrid(_ifacid);
            }
           
        }

        protected int FacID
        {
            get
            {
                return ((Session["FacID"] != null)? Convert.ToInt32(Session["FacID"]) : 0);
            }

            set
            {
                Session["FacID"] = value;
            }
        }

        private void BindGrid(int facid)
        {
            string _filter = " ORDER BY " + SortExpress + " " + SortDirect;

            using (DataSet _ds = objDml.GetOJTFields(facid, _filter))
           {
               if (_ds.Tables["ojtfields"].Rows.Count > 0)
               {
                   GVFields.DataSource = _ds.Tables["ojtfields"];
                   GVFields.DataBind();
               }
               else
               {
                  
                   GVFields.DataSource = GetEmptyDT();
                   GVFields.DataBind();
                   GVFields.Rows[0].Visible = false;
               }
                    
  
           }
        }

        protected DataTable GetEmptyDT()
        {
            DataTable _dtEmpty = new DataTable();
            _dtEmpty.Columns.Add("FIELD_ID", typeof(Int32));
            _dtEmpty.Columns.Add("COLUMN_LABEL", typeof(string));
            
            DataRow _drow = _dtEmpty.NewRow();
            _dtEmpty.Rows.Add(_drow);
          
            return _dtEmpty;
        }
        protected void GVFields_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GVFields.PageIndex = e.NewPageIndex;
            BindGrid(FacID);
        }

        protected void GVFields_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.SortExpress = e.SortExpression;
            this.SortDirect = SortDirect;
            BindGrid(FacID);
        }

        private string SortExpress
        {
            get
            {
                return ((ViewState["se"] != null) ? ViewState["se"].ToString() : "FIELD_ID");
            }
            set
            {
                ViewState["se"] = value;
            }
        }

        private string SortDirect
        {
            get
            {
                return((ViewState["sd"]) != null) ? ViewState["sd"].ToString() : "ASC";
            }
            set
            {
                ViewState["sd"] = value;
            }
        }


        protected void DdlColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow row = (GridViewRow)ddl.Parent.Parent;

            TextBox TxtColumn = (TextBox)row.Cells[1].FindControl("TxtColumn");
            HtmlGenericControl spnFormat = (HtmlGenericControl)row.Cells[1].FindControl("SpnCol");

            if (ddl.SelectedValue == "20000")
            {
                TxtColumn.Visible = true;
                spnFormat.Visible = true;
            }
            else
            {
                TxtColumn.Visible = false;
                spnFormat.Visible = false;
            }
        }

        protected void CvColumn_ServerValidate(object source, ServerValidateEventArgs args)
        {
            TextBox TxtColumn = (TextBox)GVFields.FooterRow.FindControl("TxtColumn");
            if (TxtColumn.Visible)
            {
                if (TxtColumn.Text == "")
                {
                    args.IsValid = false;
                }
                else
                {
                    //Check if column name is already there
                   string _colName = TxtColumn.Text.ToString();
                   _colName = _colName.Replace(" ", string.Empty);
                    bool _ifExists = objDml.CheckifOJTFieldExists(HttpUtility.HtmlEncode(_colName.ToLower()));
                    if (_ifExists)
                    {
                        args.IsValid = false;
                    }
                    else
                    {
                        args.IsValid = true;
                    }
                   
                }
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void GVFields_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            Business.OJTFields objField = new Business.OJTFields();
            if (e.CommandName == "add")
            {
                if (!Page.IsValid)
                {
                    return;
                }
                TextBox TxtColumn = (TextBox)GVFields.FooterRow.FindControl("TxtColumn");
                DropDownList DdlColumn = (DropDownList)GVFields.FooterRow.FindControl("DdlColumn");
               
                string _columnname;
                int _fieldid;
                if ((DdlColumn.SelectedValue == "20000") && (TxtColumn.Visible))
                {
                    _columnname = objCommon.ReplaceSC(TxtColumn.Text.Trim());
                    _fieldid = 0;
                }
                else
                {
                    _columnname = (DdlColumn.SelectedItem.Text.Trim());
                    _fieldid = Convert.ToInt32(DdlColumn.SelectedValue);
                }
                //CreateColumn
                objField.Columnlabel =  _columnname;
                objField.FieldId = _fieldid;
                objField.CreatedBy = objCommon.GetUserId();

                int _result = objDml.CreateOJTFields(objField, FacID);
                if (_result == 0)
                {
                    //refresh the grid
                    BindGrid(FacID);
                }
                else
                {
                    //display msg that it failed
                    LblojtMsg.Text = " Error creating OJT Field ";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-ojtmsg');", true);
                }
            }
            else if (e.CommandName == "del")
            {
                int _fieldId = Convert.ToInt32(e.CommandArgument);
                objField.FieldId = _fieldId;
                objField.CreatedBy = objCommon.GetUserId();

                int _result = objDml.DeleteOJTField(objField, FacID);
                if (_result == 0)
                {
                    BindGrid(FacID);
                }
                else
                {
                    LblojtMsg.Text = "Error deleting the OJT Field ";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-ojtmsg');", true);
                }
            
            }
        }

        



    }
}