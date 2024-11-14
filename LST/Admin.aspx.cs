//$Header:$
//
//  Admin.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the Admin Page of LST.
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


namespace LST
{
    public partial class Admin : System.Web.UI.Page
    {
        Business.Common_Util objcommon = new Business.Common_Util();
        Data.DML_Util objDml = new Data.DML_Util();
        Business.Employee objEmp = new Business.Employee();
        Business.UserRoles objRoles = new Business.UserRoles();
        public string str;

        protected void Page_Load(object sender, EventArgs e)
        {
            str = "^[^\".]*$";
            BtnAddSLSO.Attributes.Add("onClick", "OpenJQueryDialog('dialogUser','700','700','Users.aspx');return false;");
            BtnDesignate.Attributes.Add("onClick", "OpenJQueryDialog('dialogDesignate','700','700','Designate.aspx?type=lso');return false;");
            BtnDesignateSLSO.Attributes.Add("onClick", "OpenJQueryDialog('dialogDesignate','700','700','Designate.aspx?type=slso');return false;");
            GVUser.RowCreated +=new GridViewRowEventHandler(GVUser_RowCreated);
            if (!Page.IsPostBack)
            {
               if  (objRoles.CheckAccessibility())
               {
                   BindLSODetails();
                   BindFacilityGrid();
                   BindWorkerGrid();
                   BindUserGrid();

                    FillOJTFields();
                   FillAltLSO();
                
                   string _mode = Request.QueryString["mode"];
                   string _type = Request.QueryString["type"];                     
                   SetPanel(_mode);
                    if ((_mode == "5") && (_type == "slso"))
                    {
                        if (_type == "slso")
                        {
                            DdlAlternates.SelectedValue = "2";
                            DdlAlternates_SelectedIndexChanged(null, null);
                        }
                    }
               }
               else
               {
                    Response.Redirect("Error.aspx?msg=notauthorized");
               }

              
            }
          //Bind user grid moved into not page postback due to error in deleting. not sure if this will cause any other issue with any SLSO tab

        }

        private void BindLSODetails()
        {
           LblLso.Text = objDml.GetLSOName("lso");
           LblDLSO.Text = objDml.GetLSOName("dlso");
            BindAltLSODetails();
        }

        private void BindAltLSODetails()
        {
            string _altlso = "";
            _altlso = objDml.GetLSOName("altlso");
            DivAlternate.Visible = true;
            if (_altlso == "")
            {
                LblAlternate.Text = "";
                LblFromTo.Text = "";
            }
            else
            {
                LblFromTo.Text = objDml.GetActivePeriodLSO();
                LblAlternate.Text = _altlso;
            }

        }

        private void SetPanel(string mode)
        {
            if (mode == "2")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "SetPanel('collapseTwo')", true);
            }
            else if (mode == "3")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "SetPanel('collapseThree')", true);
            }
            else if (mode == "1")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "SetPanel('collapseOne')", true);
            }
            else if (mode == "4")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "SetPanel('collapseFour')", true);
            }
            else if (mode == "5")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "SetPanel('collapseFive')", true);
               
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "SetPanel('')", true);
            }
        }

        private void DeleteObjects(int objId, string objType, string objName)
        {
            bool _hasChild = objDml.CheckIfitHasChild(objId, objType);
            string _object = "";
            if (objType == "fac") { _object = "Facility"; }
            else { _object = "slso"; }
            if (!_hasChild)
            {
                //Delete facility
                string _result = objDml.DeleteObject(objId, objcommon.GetUserId().ToString(), objType);

               
                if (_result == "0")
                {
                    LblMsg.Text = ((_object == "slso") ? "SLSO" : _object) + " " + objName + " deleted successfully!";
                }
                else
                {
                    LblMsg.Text = ((_object == "slso") ? "SLSO" : _object) + " " + objName + " could not be deleted at this time. Please try later!";
                }

            }
            else
            {
                //msg that the facility cannot be deleted as it has child records
                if (_object == "slso")
                {
                    LblMsg.Text = objName + " cannot be deleted from SLSO role as he has one/more facilities for which he currently is SLSO/Acting/Co-SLSO/Alternate. ";
                }
                else
                {
                    LblMsg.Text = _object + " " + objName + " could not be deleted as this facility has workers associated with it";
                }
                
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-msg','Default.aspx');", true);
           
        }

        private void DeleteGonePeople(int workerId, string workerName)
        {
            bool _isActive = objDml.CheckIfitHasChild(workerId, "worker");
            if (!_isActive)
            {
                string _result = objDml.DeleteObject(workerId, objcommon.GetUserId().ToString(), "worker");
                if (_result == "0")
                {
                    LblMsg.Text = "Worker " + workerName + " deleted and the worker's association with the facilities has been removed";
                }
                else
                {
                    LblMsg.Text ="Worker " + workerName + " could not be deleted at this time. Please try later!";
                }
            }
            else
            {
                LblMsg.Text = workerName + " cannot be removed. Only people who are no longer at SLAC can be removed";
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-msg','Default.aspx');", true);
        }

        #region "Facility"

        public string SortExpressionFac
        {
            get
            {
                if (null == ViewState["sortfac"])
                {
                    ViewState["sortfac"] = "FACILITY_NAME";
                }
                return ViewState["sortfac"].ToString();
            }
            set { ViewState["sortfac"] = value; }
        }

        public string SortDirectFac
        {
            get
            {
                if (null == ViewState["sortdirfac"])
                {
                    ViewState["sortdirfac"] = "ASC";
                }
                return ViewState["sortdirfac"].ToString();
            }
            set
            {
                ViewState["sortdirfac"] = value;
            }

        }

        protected void FillFacilityDetails(OracleCommand cmdList)
        {
            DataSet _dsFac = new DataSet();
            string _filter = " ORDER BY " + SortExpressionFac + " " + SortDirectFac;
            _dsFac = objDml.GetFacilityInfo(cmdList, _filter, TxtSearchFac.Text);

            DataView _dvFac = new DataView(_dsFac.Tables["fac"]);
            int _count = 0;

            _count = _dvFac.Count;
            if (_count > 0) { 
            GvFacility.DataSource = _dvFac;
            GvFacility.DataBind();
            }
            else
            {
                GvFacility.DataSource = null;
                GvFacility.DataBind();
            }
        }

        private void BindFacilityGrid()
        {
           using (OracleCommand cmdList = new OracleCommand())
           {
               FillFacilityDetails(cmdList);
           }
        }

        protected void BtnAddFacility_Click(object sender, EventArgs e)
        {
           Response.Redirect("Facility.aspx");
        }
        
        protected void GvFacility_RowCommand(object sender, GridViewCommandEventArgs e)
        {
             
             if (e.CommandName == "View")
             {
                 int _facilityId = Convert.ToInt32(e.CommandArgument.ToString());
                 Response.Redirect("Facility.aspx?mode=view&id=" + _facilityId);
             }
             else if (e.CommandName == "Edit")
             {
                 int _facilityId = Convert.ToInt32(e.CommandArgument.ToString());
                 Response.Redirect("Facility.aspx?mode=edit&id=" + _facilityId);
             }
             else if (e.CommandName == "Delete")
             {
                 int _facilityId = Convert.ToInt32(e.CommandArgument.ToString());
                 string _facname = objDml.GetName(_facilityId, "fac");
                 DeleteObjects(_facilityId, "fac", _facname);
                 BindFacilityGrid();
               }
        }

        protected void GvFacility_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }

        protected void GvFacility_Sorting(object sender,  GridViewSortEventArgs e)
        {
            this.SortExpressionFac = e.SortExpression;
            SortDirectFac = (ViewState["sortdirfac"].ToString() == "ASC") ? "DESC" : "ASC";
            BindFacilityGrid();
            SetPanel("1");
        }

        protected void GvFacility_PageIndexChanging(object sender, GridViewPageEventArgs e)
          {
              GvFacility.PageIndex = e.NewPageIndex;
              BindFacilityGrid();
              SetPanel("1");
          }

        protected void GvFacility_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void CmdSearchFac_Click(object sender, EventArgs e)
          {
              BindFacilityGrid();
              SetPanel("1");
          }
        #endregion

        
        #region "Worker"

          public string SortExpressionWrker
          {
              get
              {
                  if (null == ViewState["sortwrkr"])
                  {
                      ViewState["sortwrkr"] = "WORKER_NAME";
                  }
                  return ViewState["sortwrkr"].ToString();
              }
              set { ViewState["sortwrkr"] = value; }
          }

          public string SortDirectWrker
          {
              get
              {
                  if (null == ViewState["sortdirwrkr"])
                  {
                      ViewState["sortdirwrkr"] = "ASC";
                  }
                  return ViewState["sortdirwrkr"].ToString();
              }
              set
              {
                  ViewState["sortdirwrkr"] = value;
              }

          }

        protected void FillWorkerDetails(OracleCommand cmdWorker)
        {
            DataSet _dsWorker = new DataSet();

            string _filter = " ORDER BY " + SortExpressionWrker + " " + SortDirectWrker;

            _dsWorker = objDml.GetWorkerInfo(cmdWorker, _filter, TxtSearchWorker.Text);

            DataView _dvWorker = new DataView(_dsWorker.Tables["worker"]);
            int _count = 0;
            _count = _dvWorker.Count;
            if (_count > 0)
            {
                GvWorker.DataSource = _dvWorker;
                GvWorker.DataBind();
            }
            else
            {
                GvWorker.DataSource = null;
                GvWorker.DataBind();
            }
                
        }

        private void BindWorkerGrid()
        {
            using (OracleCommand _cmdWorker = new OracleCommand())
            {
                FillWorkerDetails(_cmdWorker);
            }
        }

        protected void GvWorker_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }

        protected void GvWorker_RowCommand(object sender, GridViewCommandEventArgs e)
        {
           
            if (e.CommandName == "View")
            {
                int _workerId = Convert.ToInt32(e.CommandArgument.ToString());
                Response.Redirect("WorkerEntry.aspx?mode=view&id=" + _workerId);
            }
            else if (e.CommandName == "Edit")
            {
                int _workerId = Convert.ToInt32(e.CommandArgument.ToString());
                Response.Redirect("WorkerEntry.aspx?mode=edit&id=" + _workerId);
            }
            else if (e.CommandName == "Delete")
            {
                int _workerId = Convert.ToInt32(e.CommandArgument.ToString());
                string _workerName = objDml.GetName(_workerId, "worker");
                DeleteGonePeople(_workerId,  _workerName);
                BindWorkerGrid();
            }
        }

        protected void BtnAddWorker_Click(object sender, EventArgs e)
        {
            Response.Redirect("WorkerEntry.aspx");
        }
   
        protected void GvWorker_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView _row = (DataRowView)e.Row.DataItem;
                string _slacId;
             
                _slacId = _row["SLAC_ID"].ToString();
                objEmp.GetEmpDetails(_slacId);

                Label LblBadge = new Label();
                LblBadge = (Label)e.Row.FindControl("LblBadge");
                LblBadge.Text = objEmp.BadgeId;

                string _reason = _row["REASON_INACTIVE"].ToString();
                if (_reason != "")
                {
                    e.Row.Cells[9].Text = _reason + " overdue";
                }

                string _gonet = _row["GONET"].ToString();
                if (_gonet.ToLower() != "active")
                {
                    e.Row.BackColor = System.Drawing.Color.OrangeRed;
                }
            }
        }

        protected void GvWorker_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GvWorker.PageIndex = e.NewPageIndex;
            BindWorkerGrid();
            SetPanel("2");
        }

         protected void GvWorker_Sorting(object sender, GridViewSortEventArgs e)
        {
            SortExpressionWrker = e.SortExpression;
            SortDirectWrker = (ViewState["sortdirwrkr"].ToString() == "ASC") ? "DESC" : "ASC";
            BindWorkerGrid();
            SetPanel("2");
        }

         protected void GvWorker_RowDeleting(object sender, GridViewDeleteEventArgs e)
         {

         }

         protected void CmdSearchWorker_Click(object sender, EventArgs e)
         {
             BindWorkerGrid();
             SetPanel("2");
         }

        #endregion


        #region "User"

         public string SortExpressionUsr
         {
             get
             {
                 if (null == ViewState["sortusr"])
                 {
                     ViewState["sortusr"] = "USERNAME";
                 }
                 return ViewState["sortusr"].ToString();
             }
             set { ViewState["sortusr"] = value; }
         }

         public string SortDirectUsr
         {
             get
             {
                 if (null == ViewState["sortdirusr"])
                 {
                     ViewState["sortdirusr"] = "ASC";
                 }
                 return ViewState["sortdirusr"].ToString();
             }
             set
             {
                 ViewState["sortdirusr"] = value;
             }

         }

         private int _userRoleID;

         public int UserRoleID
         {
             get { return _userRoleID; }
             set
             {
                 _userRoleID = value;
                 this.DataBind();
             }
         }

        protected void FillUserDetails(OracleCommand cmdUser)
        {
            DataSet _dsUser = new DataSet();
            string _filter = " ORDER BY " + SortExpressionUsr + " " + SortDirectUsr;
            _dsUser = objDml.GetUserInfo(cmdUser, _filter,TxtSearchSLSO.Text);

            DataView _dvUser = new DataView(_dsUser.Tables["user"]);
            int _count = 0;
            _count = _dvUser.Count;

            if (_count > 0)
            {
                GVUser.DataSource = _dvUser;
                GVUser.DataBind();
            }
            else
            {
                GVUser.DataSource = null;
                GVUser.DataBind();
            }
        }

        private void  BindUserGrid()
        {
            using (OracleCommand _cmdUser = new OracleCommand())
            {
                FillUserDetails(_cmdUser);
            }
        }

        protected void GVUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GVUser.PageIndex = e.NewPageIndex;
            BindUserGrid();
            SetPanel("3");
        }

        protected void GVUser_Sorting(object sender, GridViewSortEventArgs e)
        {
            SortExpressionUsr = e.SortExpression;
            SortDirectUsr = (ViewState["sortdirusr"].ToString() == "ASC") ? "DESC" : "ASC";
            BindUserGrid();
            SetPanel("3");
        }

        protected DataTable BindGridSLSO(int userRoleId, int slacId)
        {
            //Need to add Alternate SLSO to the userroles table for it to appear in this list (if the user is only alternate slso and not any other role)
              DataSet _dsSLSOLab = new DataSet();

            _dsSLSOLab = objDml.GetSLSOLabInfo(userRoleId, false, slacId.ToString());

            if (_dsSLSOLab != null)
            {
                if (_dsSLSOLab.Tables["slsolabs"].Rows.Count > 0)
                    return _dsSLSOLab.Tables["slsolabs"];
                else
                    return null;
            }
            else return null;

        }

        protected void GVUser_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                int _slacId = Convert.ToInt32(e.CommandArgument.ToString());
                string _slsoName = objDml.GetName(_slacId, "slso");
                DeleteObjects(_slacId, "slso", _slsoName);
                BindUserGrid();
            }
        }

        protected void GVUser_RowCreated(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int _userRoleid = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "USER_ROLE_ID"));
                int _slacid = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "SLAC_ID"));
                DataView _dv = new DataView(BindGridSLSO(_userRoleid, _slacid));

                GridView gvSlso = e.Row.FindControl("GvSLSO") as GridView;
                gvSlso.DataSource = _dv;
                gvSlso.DataBind();

            }
        }


        protected void CmdSearchSLSO_Click(object sender, EventArgs e)
        {
            BindUserGrid();
            SetPanel("3");
        }

        protected void GVUser_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void GVUser_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView _row = (DataRowView)e.Row.DataItem;
                string _pbstatus = _row["STATUS"].ToString();
                if (_pbstatus.ToLower() != "active")
                {
                    e.Row.BackColor = System.Drawing.Color.OrangeRed;
                }
            }
        }
        #endregion


        #region "OJT Fields"

        public string SortExpressionOJT
        {
            get
            {
                if (null == ViewState["sortojt"])
                {
                    ViewState["sortojt"] = "FIELD_ID";
                }
                return ViewState["sortojt"].ToString();
            }
            set
            {
                ViewState["sortojt"] = value;
            }
        }

        public string SortDirectOJT
        {
            get
            {
                if (null == ViewState["sortdirojt"])
                {
                    ViewState["sortdirojt"] = "ASC";
                }
                return ViewState["sortdirojt"].ToString();
            }
            set
            {
                ViewState["sortdirojt"] = value;
            }
        }

        protected void FillOJTFields()
        {
            DataSet _dsOJT = new DataSet();
            string _filter = " ORDER BY " + SortExpressionOJT + " " + SortDirectOJT;
            _dsOJT = objDml.GetOJTFieldsSummary(_filter);

            DataView _dvOJT = new DataView(_dsOJT.Tables["ojtsum"]);
            int _count = 0;

            _count = _dvOJT.Count;
            if (_count > 0)
            {
                GVOJTField.DataSource = _dvOJT;
                GVOJTField.DataBind();
            }
            else
            {
                GVOJTField.DataSource = null;
                GVOJTField.DataBind();
            }
           
        }

        protected void GVOJTField_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GVOJTField.PageIndex = e.NewPageIndex;
            BindOJTGrid();
        }

        protected void GVOJTField_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.SortExpressionOJT = e.SortExpression;
            this.SortDirectOJT = ViewState["sortdirojt"] != null ? (ViewState["sortdirojt"].ToString() == "ASC" ? "DESC" : "ASC") : "ASC";
            BindOJTGrid();

        }

        protected void GVOJTField_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void GVOJTField_RowCommand(object sender, GridViewCommandEventArgs e)
        {            
            if (e.CommandName == "delete")
            {
                int _rowIndex = Convert.ToInt32(e.CommandArgument.ToString());
                int _fieldId = Convert.ToInt32(this.GVOJTField.DataKeys[_rowIndex]["FIELD_ID"]);
               // int _fieldId = Convert.ToInt32(e.CommandArgument.ToString());
                //Check if the field is associated with any facility and if not, delete \
                //otherwise give msg

                string _facs = ((Label)GVOJTField.Rows[_rowIndex].FindControl("LblFacs")).Text;
                //bool CheckIfExists = objDml.CheckIfFieldbyFACExists(_fieldId);
                //if (! CheckIfExists)
                //{
                if (_facs == "")
                {
                    int _resultId = objDml.DeleteMasterOJTField(_fieldId);
                    if (_resultId == 1)
                    {
                       BindOJTGrid();
                       LblMsg.Text = "";
                    }
                    else
                    {
                        LblMsg.Text = "Error deleting the OJT Field";
                    }
                }
                else
                {
                    LblMsg.Text = "OJT Field cannot be deleted as it is associated with one or more facilities";
                }
               
                if (LblMsg.Text != "") {
                   
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-msg');", true);
                    
                }
                
               
            }
        }

        protected void GVOJTField_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                e.KeepInEditMode = true;
            }
        }

        protected void GVOJTField_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int _fieldId;
            TextBox TxtDesc;
            CustomValidator _cvCheck;

            int _result;
            if (Page.IsValid)
            {
                _fieldId = Convert.ToInt32(GVOJTField.DataKeys[e.RowIndex].Values[0]);
                TxtDesc = (TextBox)(GVOJTField.Rows[e.RowIndex].FindControl("TxtDesc"));
                _cvCheck = (CustomValidator)(GVOJTField.Rows[e.RowIndex].FindControl("CvColumn"));
                string _collabel = (TxtDesc.Text != "") ? TxtDesc.Text.ToString().Trim() : "";
                if (_collabel != "")
                {
                    _collabel = _collabel.Replace(" ", string.Empty);
                    bool _ifexists = objDml.CheckifOJTFieldExists(_collabel.ToLower(), _fieldId);
                    if (_ifexists)
                    {
                        _cvCheck.IsValid = false;
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "SetPanel('collapseFour')", true);
                    }
                    else
                    {
                        _result = objDml.UpdateOJTField(_fieldId, objcommon.ReplaceSC(TxtDesc.Text.Trim()));
                        if (_result != 1)
                        {
                            LblMsg.Text = "Error updating the OJT Field Descriptor";
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-msg');", true);
                        }
                        else
                        {
                            GVOJTField.EditIndex = -1;
                            BindOJTGrid();
                        }
                    }

                }

            }
        }

        protected void GVOJTField_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GVOJTField.EditIndex = -1;
            BindOJTGrid();

        }

        protected void GVOJTField_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GVOJTField.EditIndex = e.NewEditIndex;
            BindOJTGrid();

        }

        protected void BindOJTGrid()
        {
            FillOJTFields();
            SetPanel("4");
        }




        #endregion


        #region "Alternate LSO"
        private void FillAltLSO()
        {
            dsUsers.SelectCommand += " ORDER BY " + SortExpressionAL + " " + SortDirectAL;
            GvAltLSO.DataSourceID = null;
            GvAltLSO.DataSource = dsUsers;
            GvAltLSO.DataBind();
        }

        private void BindAltLSO()
        {
            FillAltLSO();
            SetPanel("5");
        }

        protected string SortExpressionAL
        {
            get
            {
                if (null == ViewState["sort"])
                {
                    ViewState["sort"] = "MGRNAME";
                }
                return ViewState["sort"].ToString();
            }

            set { ViewState["sort"] = value; }
        }

        protected string SortDirectAL
        {
            get
            {
                if (null == ViewState["sortdirection"])
                {
                    ViewState["sortdirection"] = "ASC";
                }
                return ViewState["sortdirection"].ToString();
            }
            set
            {
                ViewState["sortdirection"] = value;
            }
        }
        protected void GvAltLSO_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                int _mgrId = Convert.ToInt32(e.CommandArgument);
                int _result = objDml.DeleteAltLSO(_mgrId, objcommon.GetUserId().ToString());
                if (_result == 1)
                {
                    BindAltLSO();
                    BindAltLSODetails();
                    LblMsg.Text = "";
                }
                else
                {
                    LblMsg.Text = "Error deleting the Alternate LSO";

                }
                if (LblMsg.Text != "")
                {

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-msg');", true);

                }
            }
        }

        protected void GvAltLSO_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GvAltLSO.EditIndex = e.NewEditIndex;
            BindAltLSO();
        }

        protected void GvAltLSO_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (Page.IsValid)
            {
                LblMsgAL.Visible = false;
                LblMsgAL.Text = "";
                int _mgrId = Convert.ToInt32(GvAltLSO.DataKeys[e.RowIndex].Values[0]);
                DateTime _altFrom;
                DateTime _altTo;
                TextBox TxtAltFrom = (TextBox)GvAltLSO.Rows[e.RowIndex].FindControl("TxtEditAltFromDate");
                TextBox TxtAltTo = (TextBox)GvAltLSO.Rows[e.RowIndex].FindControl("TxtEditAltToDate");
                _altFrom = Convert.ToDateTime(TxtAltFrom.Text);
                _altTo = Convert.ToDateTime(TxtAltTo.Text);

                bool _checkDatesOutside = objDml.CheckifDatesROutsideCur(_altFrom, _altTo, _mgrId);

                if (_checkDatesOutside)
                {
                  
                    int _result = objDml.UpdateAltLSO(_mgrId, _altFrom, _altTo);
                    if (_result != 1)
                    {
                        LblMsg.Text = "Error updating From and To dates of Alternate LSO";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-msg');", true);

                    }
                    else
                    {
                        GvAltLSO.EditIndex = -1;
                        BindAltLSO();
                        BindAltLSODetails();
                    }
                }
                else
                {
                    LblMsgAL.Visible = true;
                    LblMsgAL.Text = "There is already an Alternate LSO assigned on those dates. Please choose a different date as only one alternate can be active at a time";      
                  
                }
            }
        }


        protected void GvAltLSO_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GvAltLSO.EditIndex = -1;
            LblMsgAL.Visible = false;
            LblMsgAL.Text = "";
            BindAltLSO();
        }

        protected void CustEditValDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator custval = new CustomValidator();
            custval = (CustomValidator)source;
            GridViewRow gvr = (GridViewRow)custval.NamingContainer;
            TextBox TxtAltFrom = (TextBox)gvr.FindControl("TxtEditAltFromDate");
            TextBox TxtAltTo = (TextBox)gvr.FindControl("TxtEditAltToDate");
            CustomValidator CvDate = (CustomValidator)gvr.FindControl("CustEditValDate");

            bool _isValid = CheckToDate(TxtAltFrom.Text, TxtAltTo.Text, CvDate);
            args.IsValid = _isValid;
            LblMsgAL.Visible = false;
            LblMsgAL.Text = "";
            SetPanel("5");
        }

        private bool CheckToDate(string fromVal, string toVal, CustomValidator CvDate)
        {
            DateTime _dtFrom;
            DateTime _dtTo;
            DateTime _dtNow;

            if ((fromVal != "") && (toVal != ""))
            {
                
                toVal = DateTime.Parse(toVal).ToShortDateString();
                string _nowVal = DateTime.Now.ToShortDateString();
                fromVal = DateTime.Parse(fromVal).ToShortDateString();
                _dtFrom = DateTime.Parse(fromVal);
                _dtTo = DateTime.Parse(toVal);
                _dtNow = DateTime.Parse(_nowVal);

                if (_dtTo < _dtFrom)
                {
                    //return error msg as From date is greater than To date
                    CvDate.ErrorMessage = "Alt From Date cannot be greater than Alt To Date";
                    return false;
                }
                //else if (_dtFrom < _dtNow)
                //{
                //    CvDate.ErrorMessage = "Alt From Date should be greater than or equal to today's date";
                //    return false;
                //}
                else if (_dtTo < _dtNow)
                {
                    //return error msg as To date should be greater than the current date
                    CvDate.ErrorMessage = "Alt To Date should be greater than or equal to today's date";
                    return false;
                }               
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        protected void GvAltLSO_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                e.KeepInEditMode = true;
            }
        }

        protected void GvAltLSO_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GvAltLSO.PageIndex = e.NewPageIndex;
            BindAltLSO();
        }

        protected void GvAltLSO_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void DdlAlternates_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (DdlAlternates.SelectedValue.Equals("1"))
            {
                DivAltLSO.Visible = true;
                DivAltSLSO.Visible = false;
                FillAltLSO();
            }
            else
            {
                DivAltLSO.Visible = false;
                DivAltSLSO.Visible = true;
                FillAltSLSO();
            }
            
            SetPanel("5");
            
        }

        protected void GvAltLSO_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.SortExpressionAL = e.SortExpression;
            this.SortDirectAL = ViewState["sortdirection"] != null ? (ViewState["sortdirection"].ToString() == "ASC" ? "DESC" : "ASC") : "ASC";
            BindAltLSO();
        }
        #endregion


        #region "Alternate SLSO"
        private void FillAltSLSO()
        {
            SDSAltSlso.SelectCommand += " ORDER BY " + SortExpressionASL + " " + SortDirectASL;
            GvAltSLSO.DataSourceID = null;
            GvAltSLSO.DataSource = SDSAltSlso;
            GvAltSLSO.DataBind();
        }

        private void BindAltSLSO()
        {
            FillAltSLSO();
            SetPanel("5");
           
        }

        protected string SortExpressionASL
        {
            get
            {
                if (null == ViewState["sortASL"])
                {
                    ViewState["sortASL"] = "ALTSLSO";
                }
                return ViewState["sortASL"].ToString();
            }

            set { ViewState["sortASL"] = value; }
        }

        protected string SortDirectASL
        {
            get
            {
                if (null == ViewState["sortdirectionASL"])
                {
                    ViewState["sortdirectionASL"] = "ASC";
                }
                return ViewState["sortdirectionASL"].ToString();
            }
            set
            {
                ViewState["sortdirectionASL"] = value;
            }
        }
        protected void GvAltSLSO_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                int _facId = Convert.ToInt32(e.CommandArgument);
                int _result = objDml.DeleteAltSLSO(_facId, objcommon.GetUserId().ToString());
                if (_result == 1)
                {
                    BindAltSLSO();
                    LblMsg.Text = "";
                }
                else
                {
                    LblMsg.Text = "Error deleting the Alternate SLSO";
                }

                if (LblMsg.Text != "")
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-msg');", true);
                }
            }
        }

        protected void GvAltSLSO_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GvAltSLSO.EditIndex = e.NewEditIndex;
            BindAltSLSO();
        }

        protected void GvAltSLSO_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (Page.IsValid)
            {
              
                int _facId = Convert.ToInt32(GvAltSLSO.DataKeys[e.RowIndex].Values[0]);
                DateTime _altFrom;
                DateTime _altTo;
                TextBox TxtAltFrom = (TextBox)GvAltSLSO.Rows[e.RowIndex].FindControl("TxtEditAltFromDate");
                TextBox TxtAltTo = (TextBox)GvAltSLSO.Rows[e.RowIndex].FindControl("TxtEditAltToDate");
                _altFrom = Convert.ToDateTime(TxtAltFrom.Text);
                _altTo = Convert.ToDateTime(TxtAltTo.Text);

                int _result = objDml.UpdateAltSLSO(_facId, _altFrom, _altTo);
                if (_result != 1)
                {
                    LblMsg.Text = "Error updating From and To dates of Alternate SLSO";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-msg');", true);
                }
                else
                {
                    GvAltSLSO.EditIndex = -1;
                    BindAltSLSO();
                }

            }
        }

        protected void GvAltSLSO_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GvAltSLSO.EditIndex = -1;
            BindAltSLSO();
        }

        protected void GvAltSLSO_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.SortExpressionASL= e.SortExpression;
            this.SortDirectASL = ViewState["sortdirectionASL"] != null ? (ViewState["sortdirectionASL"].ToString() == "ASC" ? "DESC" : "ASC") : "ASC";
            BindAltSLSO();
        }

        protected void GvAltSLSO_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                e.KeepInEditMode = true;
            }
        }

        protected void GvAltSLSO_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GvAltSLSO.PageIndex = e.NewPageIndex;
            BindAltSLSO();
        }

        protected void GvAltSLSO_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void CustEditValDateASL_ServerValidate(object source, ServerValidateEventArgs args)
        {
            CustomValidator custval = new CustomValidator();
            custval = (CustomValidator)source;
            GridViewRow gvr = (GridViewRow)custval.NamingContainer;
            TextBox TxtAltFrom = (TextBox)gvr.FindControl("TxtEditAltFromDate");
            TextBox TxtAltTo = (TextBox)gvr.FindControl("TxtEditAltToDate");
            CustomValidator CvDate = (CustomValidator)gvr.FindControl("CustEditValDateASL");

            bool _isValid = CheckToDate(TxtAltFrom.Text, TxtAltTo.Text, CvDate);
            args.IsValid = _isValid;
            SetPanel("5");
        }

        #endregion
    }
}