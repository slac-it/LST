using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text.RegularExpressions;

namespace LST.UserControls
{
    public partial class OJTWorkersMatrix : System.Web.UI.UserControl
    {
        Data.DML_Util objDml = new Data.DML_Util();
        Business.OJTFieldMatrix objFM = new Business.OJTFieldMatrix();

        #region "Page related"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SetPage();
            }
           
            aWeb.HRef = FacWebSite;                    
        }

        protected void SetPage()
        {
            LnkOJTFields.Visible = IsSLSOAlt;
            if (LnkOJTFields.Visible)
            {
                string _title = "Editing OJT field for facility " + FacName;
                LnkOJTFields.Attributes.Add("onClick", "OpenDialogCustomTitle('dialogOJTFields', '750','700','OJTFields.aspx?facid= " + FacId + "', '" + _title + "');return false;");            
            }
            //Need to call bindgrid on if not postback. otherwise, the items get created again and again
            BindGrid();
        }

        //These two functions are to make viewstate valid - get error as invalid viewstate
        protected override void LoadViewState(object savedState)
        {

            base.LoadViewState(((Pair)savedState).First);
            BindGrid();
        }

        protected override object SaveViewState()
        {
            return new Pair(base.SaveViewState(), null);
        }
        #endregion


        #region "Properties"
        public int FacId
        {
            get
            {
                return (ViewState["facid"] != null) ? Convert.ToInt32(ViewState["facid"].ToString()) : 0;
            }
            set
            {
                ViewState["facid"] = value;
            }

        }

        public string FacWebSite
        {
            get
            {
                return ((ViewState["website"]) != null) ? Server.UrlDecode(ViewState["website"].ToString()): "" ;
            }
            set
            {
                ViewState["website"] =  Server.UrlEncode(value);
            }
        }

        public string FacName
        {
            get
            {
                return (ViewState["facname"] != null) ? ViewState["facname"].ToString() : "";
            }
            set
            {
                ViewState["facname"] = value;
            }
        }

        public bool IsSLSOAlt
        {
            get
            {
                return (ViewState["slsoalt"] != null) ? (bool)ViewState["slsoalt"] : false;
            }
            set
            {
                ViewState["slsoalt"] = value;
            }
        }



        #endregion


        #region "grid view events"
        public void BindGrid()
        {
            //Edit button should be available only for SLSO and its subcat
            GVOJT.AutoGenerateEditButton = IsSLSOAlt;
           
            DataSet _ds = new DataSet();
            if (FacId > 0)
            {
                _ds = objDml.GetOJT(FacId);
                DataTable _dt = _ds.Tables["ojt"];
                ViewState["dt"] = _dt;
              
                if (_dt.Rows.Count > 0)
                {
                    if (GVOJT.Columns.Count > 0)
                    {
                        GVOJT.Columns.Clear();
                    }
                    for (int i = 0; i < _dt.Columns.Count; i++)
                    {
                        TemplateField tempCol = new TemplateField();
                        tempCol.HeaderTemplate = new Business.DynamicTemplate(ListItemType.Header, _dt.Columns[i].ColumnName);
                        tempCol.ItemTemplate = new Business.DynamicTemplate(ListItemType.Item, _dt.Columns[i].ColumnName);
                        tempCol.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                       
                        if (i == 0)
                        {
                            tempCol.Visible = false;
                        }
                        if (i <= 3) {
                            if (i == 1) { tempCol.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                            tempCol.HeaderStyle.Width = Unit.Pixel(200);                                
                            }
                            else
                            {
                                tempCol.HeaderStyle.Width = Unit.Pixel(100); 
                            }
                           tempCol.ItemStyle.Wrap = false; 
                        }
                         //For first 3 columns and last column, edit is not required
                        if (i > 3) 
                        {
                            tempCol.EditItemTemplate = new Business.DynamicTemplate(ListItemType.EditItem, _dt.Columns[i].ColumnName);
                           tempCol.HeaderStyle.Width = Unit.Pixel(100);
                           tempCol.HeaderStyle.Wrap = true; 
                           
                        }

                        GVOJT.Columns.Add(tempCol);
                    }
                    GVOJT.DataSource = _dt;
                    GVOJT.DataBind();
                }
                else
                {
                    GVOJT.EmptyDataText = (IsSLSOAlt) ? "OJT Matrix not found for this facility.  Please make sure to add OJT Fields to the Facility using the link above " : "OJT Matrix not found for this facility";
                    GVOJT.DataSource = null;
                    GVOJT.DataBind();
                }             
            }
            else
            {
                GVOJT.DataSource = null;
                GVOJT.DataBind();
            }          
        }

        protected void GVOJT_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = "Action";
                e.Row.Cells[0].Width = Unit.Pixel(50);
      

            }
            else
            {
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
            }
           

           
        }

        protected void GVOJT_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GVOJT.EditIndex = -1;
            BindGrid();
            KeepGridExpanded();
          
        }

        protected void GVOJT_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GVOJT.EditIndex = e.NewEditIndex;            
            BindGrid();
            KeepGridExpanded();
        }

        private void KeepGridExpanded()
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "toggleme('divGdOJT','btnShow2','btnHide2');", true);
        }

        protected void GVOJT_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            GridViewRow row = GVOJT.Rows[e.RowIndex];
            DataTable _dt = (DataTable)ViewState["dt"];

            //string mapid =((Label)row.FindControl("lbl" + _dt.Columns[0].ColumnName.Replace(" ",string.Empty))).Text;

            int mapid = Convert.ToInt32(_dt.Rows[e.RowIndex][0]);
            int _statid = objDml.GetStatusId(mapid);
            string _result = objDml.UpdateFieldMatrix(mapid);
            if (_result == "0")
            {
                for (int i = 4; i < _dt.Columns.Count; i++)
                {
                    int field_id = objDml.GetFieldID(_dt.Columns[i].ColumnName);
                    string _newColName = Regex.Replace(_dt.Columns[i].ColumnName, @"[^0-9a-zA-Z]+", string.Empty);
                    string _ddid = "dd" + _newColName + field_id;
                    DropDownList ddlist = (DropDownList)row.FindControl(_ddid);
                    string field_value = ddlist.SelectedValue;
                  
                    Business.OJTFieldMatrix objFM1 = new Business.OJTFieldMatrix();
                    objFM1.WorkerFacMapId = mapid;
                    objFM1.OJTFieldId = field_id;
                    objFM1.FieldValue = field_value;
                    int _matrixId = objDml.InsertFieldMatrix(objFM1);

                }
              
            }
            else
            {
                //give error msg that it cannot be updated
                LblMsgojt.Text = "Db error. Not able to update the OJT Matrix of the worker";
            }

            GVOJT.EditIndex = -1;
            BindGrid();
            KeepGridExpanded();

            if ((_statid == 2) && (_result == "0"))
            {
                LblMsgojt.Text = "Note: this person is still pending approval to be a laser worker for this facility.";
                Parent.Page.MaintainScrollPositionOnPostBack = false;
            }
            else if ((_statid == 6) && (_result == "0"))
            {
                LblMsgojt.Text = "Note: this person is currently inactive for this facility. Click on their name to view their individual report to determine reason for being inactive.";
                Parent.Page.MaintainScrollPositionOnPostBack = false;
            }

            if (LblMsgojt.Text != "") { Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-ojt');", true);
           
            }
         

        }

        #endregion

      

       

    }
}