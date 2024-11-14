//$Header:$
//
//  Facility.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This page has facility related information with add,edit and view access. Only admin/LSo and slso of that facility can access this page

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Data.OracleClient;
using System.Data;
using Oracle.ManagedDataAccess.Client;


namespace LST
{
    public partial class NewFacility : BasePage
    {
        Data.Data_Util objData = new Data.Data_Util();
        Data.DML_Util objDml = new Data.DML_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        Business.Validation objValid = new Business.Validation();
        Business.UserRoles objRoles = new Business.UserRoles();

        protected void Page_Load(object sender, EventArgs e)
        {
            ImgBtnPM.Attributes.Add("onClick", "OpenDialogForName('dialogowneradmin','TxtPrgMgr','HdnPMId'); return false;");
            ImgBtnCoord.Attributes.Add("onClick", "OpenDialogForName('dialogowneradmin','TxtCoordinator','HdnCoordId'); return false;");
          
            if (!Page.IsPostBack)
            {
                string _mode = (Request.QueryString["mode"] != null) ? Request.QueryString["mode"].ToString() : "";
                HdnMode.Value = _mode;
                int _facid = (Request.QueryString["id"] != null && Request.QueryString["id"] != "") ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                UrlReferrer = (Request.UrlReferrer != null) ? Request.UrlReferrer.ToString() : "";
                string _reportdd = (Request.QueryString["dd"] != null) ? Request.QueryString["dd"].ToString() : null;
                ViewState["dd"] = _reportdd;
                RequestId = (Request.QueryString["objid"] != null) ? Request.QueryString["objid"].ToString() : null;
                IsSLSOAlt = objRoles.CheckIfSLSO(_facid);
                HasAdminAccess = objRoles.CheckAccessibility();

                if ((HasAdminAccess) || ((IsSLSOAlt) && (_mode != "")) ||
                       ((_mode == "view") && ((UrlReferrer.Contains("Reports")) || (UrlReferrer.Contains("Worker"))
                       || (UrlReferrer.Contains("Default")) || (UrlReferrer.Contains("FacilityApprovalRequest")))))
                {
                    //if (objRoles.IsAdmin) { IsAdmin = true; }
                   
                    if ((_mode != "") && (!objDml.CheckIfIdExists(_facid, "fac")))
                    {
                        Response.Redirect("Error.aspx?msg=notexists");
                    }
                    SetPage(_mode, _facid);

                }
                else
                {
                
                    Response.Redirect("Error.aspx?msg=notauthorized");
                }


            }


            UCFile1.ObjId = FacId;
            UCFile1.Mode = Mode;
            //Do not delete this event handler (might be useful in future)
            //page refresh removed due to DEV-7753. File attached will show only after coming back to the page
          UCFile1.AttachButtonClicked += new EventHandler(UCFile1_AttachButtonClicked);
            UCFile1.ObjType = "Facility";
         

        }

       

        #region "Properties"
        public int FacId
        {
            get
            {
                return (ViewState["facid"] != null ? (int)ViewState["facid"] : 0 );
            }
            set
            {
                ViewState["facid"] = value;
            }

        }

        public string Mode
        {
            get
            {
                if (ViewState["mode"] != null)
                {
                    return (string)ViewState["mode"];
                }
                else return "";
            }
            set
            {
                ViewState["mode"] = value;
            }
        }

        public bool IsSLSOAlt
        {
            get
            {
                return (ViewState["slsoalt"] != null ? (bool)ViewState["slsoalt"] : false);
            }
            set
            {
                ViewState["slsoalt"] = value;
            }
        }

        public bool HasAdminAccess
        {
            get
            {
                return (ViewState["admin"] != null ? (bool)ViewState["admin"] : false);
            }
            set
            {
                ViewState["admin"] = value;
            }
        }

        public string UrlReferrer
        {
            get
            {
                return ((ViewState["urlrefer"] != null) ? ViewState["urlrefer"].ToString() : "");
            }
            set
            {
                ViewState["urlrefer"] = value;
            }
        }

        public string RequestId
        {
            get
            {
                return ((ViewState["reqid"] != null) ? ViewState["reqid"].ToString() : "");
            }
            set
            {
                ViewState["reqid"] = value;
            }
        }

        #endregion

        #region "Dropdown Events"
        protected void DdlSLSO_DataBound(object sender, EventArgs e)
        {
            DdlSLSO.Items.Insert(0, new ListItem("--Choose One--", "-1"));
        }

        protected void DdlCoSLSO_DataBound(object sender, EventArgs e)
        {
            DdlCoSLSO.Items.Insert(0, new ListItem("--Choose One--", "-1"));
        }

        protected void DdlActSLSO_DataBound(object sender, EventArgs e)
        {
            DdlActSLSO.Items.Insert(0, new ListItem("--Choose One--", "-1"));
        }

        private void FillDropdownList()
        {

            OracleConnection _ocon = new OracleConnection();
            OracleCommand _ocmd = new OracleCommand();

            try
            {
                _ocon.ConnectionString = objData.GetConnectionString();
                _ocon.Open();
                _ocmd.Connection = _ocon;
                _ocmd.CommandText = "LST_LOOKUP_PKG.Getlookupvalues";
                _ocmd.CommandType = CommandType.StoredProcedure;
                _ocmd.BindByName = true;
                _ocmd.Parameters.Add(new OracleParameter("LabCur", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
                _ocmd.Parameters.Add(new OracleParameter("BldgCur", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;

                OracleDataReader Rdr = _ocmd.ExecuteReader();
                while (Rdr.Read())
                {
                    Rdr.NextResult();
                    DdlBldg.DataSource = Rdr;
                    DdlBldg.DataValueField = "BUILDING_NUMBER";
                    DdlBldg.DataTextField = "BUILDING_NUMBER";
                    DdlBldg.DataBind();
                }
            }
            finally
            {
                if (_ocon.State == ConnectionState.Open)
                {
                    _ocon.Close();
                }
            }

            //objData.SPName = "LST_LOOKUP_PKG.Getlookupvalues";

            //using (DataSet _dsMulti = new DataSet())
            //{
            //    using (OracleDataReader _drMulti = objData.GetMultiresult())
            //    {
            //        while (_drMulti.Read())
            //        {
            //            _drMulti.NextResult();
            //            DdlBldg.DataSource = _drMulti;
            //            DdlBldg.DataValueField = "BUILDING_NUMBER";
            //            DdlBldg.DataTextField = "BUILDING_NUMBER";
            //            DdlBldg.DataBind();
            //        }
            //    }
    
            //}


            DdlBldg.Items.Insert(0, new ListItem("N/A", "0"));
            DdlBldg.Items.Insert(0, new ListItem("--Choose One--", "-1"));


            FillSLSO();
            FillActSLSO();
            FillCoSLSO();

        }

        private void FillSLSO()
        {
            DdlSLSO.Items.Clear();
            DdlSLSO.DataSourceID = "SDSRoles";
            DdlSLSO.DataTextField = "EMPLOYEE_NAME";
            DdlSLSO.DataValueField = "USER_ROLE_ID";
            DdlSLSO.DataBind();
        }

        private void FillActSLSO()
        {
            DdlActSLSO.Items.Clear();
            DdlActSLSO.DataSourceID = "SDSRoles";
            DdlActSLSO.DataTextField = "EMPLOYEE_NAME";
            DdlActSLSO.DataValueField = "USER_ROLE_ID";
            DdlActSLSO.DataBind();
        }

        private void FillCoSLSO()
        {
            DdlCoSLSO.Items.Clear();
            DdlCoSLSO.DataSourceID = "SDSRoles";
            DdlCoSLSO.DataTextField = "EMPLOYEE_NAME";
            DdlCoSLSO.DataValueField = "USER_ROLE_ID";
            DdlCoSLSO.DataBind();
        }

        protected void DdlBldg_SelectedIndexChanged(object sender, EventArgs e)
        {
            string _bldg;
            _bldg = DdlBldg.SelectedValue;
            if ((_bldg != "-1") || (_bldg != "0"))
            {
                FillRoom(_bldg);
                DdlRoom.Items.Insert(0, new ListItem("N/A", "0"));
            }
            else
            {
                DdlRoom.Items.Clear();

            }
        }

        #endregion

        #region "Button Events"
        protected void BtnCancel_Click(object sender, EventArgs e)
        {

            Response.Redirect("Admin.aspx?mode=1");


        }

        protected void BtnSubmit_Click(object sender, EventArgs e)
        {

            if (Page.IsValid)
            {
                if (ValidLocation())
                {
                    FillNSaveFacilityObject("add");
                }


            }
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (ValidLocation()) //Need to add this to server validate function
                {

                    FillNSaveFacilityObject("edit");
                }

            }
        }

        protected void BtnCancelupd_Click(object sender, EventArgs e)
        {
            if (UrlReferrer.Contains("Admin.aspx"))
            {
                Response.Redirect("Admin.aspx?mode=1");
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }


        protected void BtnBack_Click(object sender, EventArgs e)
        {

            if (UrlReferrer.Contains("Admin.aspx"))
            {
                Response.Redirect("Admin.aspx?mode=1");
            }
            else if (UrlReferrer.Contains("Reports") || (UrlReferrer.Contains("Worker")) && (ViewState["dd"] != null))
            {
                Response.Redirect("Reports.aspx?dd=" + ViewState["dd"].ToString());
            }
            else if ((UrlReferrer.Contains("FacilityApprovalRequest")) && RequestId != "")
            {
                Response.Redirect("FacilityApprovalRequest.aspx?objid=" + RequestId);
            }
            else
            {
                if (HasAdminAccess)
                {
                    Response.Redirect("Admin.aspx?mode=1");
                }
                else
                {
                    Response.Redirect("Default.aspx");
                }

            }
        }


        protected void CvSLSO_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if ((objValid.CheckIfSelected(DdlSLSO.SelectedIndex.ToString())) && (objValid.CheckIfSelected(DdlSLSO.SelectedValue.ToString())))
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }

        }

        //Do not delete this event handler (might be useful in future)
        private void UCFile1_AttachButtonClicked(object sender, EventArgs e)
        {
            BindGridFile();
        }


        #endregion

        #region "Other"

        protected void FillNSaveFacilityObject(string mode)
        {
            Business.Facility objLSTFac = new Business.Facility();

            objLSTFac.FacilityName = objCommon.ReplaceSC(TxtFacility.Text.Trim());
            objLSTFac.Bldg = (DdlBldg.SelectedIndex != 0) ? DdlBldg.SelectedItem.Text : "";
            objLSTFac.Room = ((DdlRoom.SelectedIndex != 0) && (DdlRoom.SelectedIndex != -1)) ? DdlRoom.SelectedItem.Value : "";
            objLSTFac.OtherLocation = objCommon.ReplaceSC(TxtLocate.Text.Trim());
            objLSTFac.SLSO = (DdlSLSO.SelectedIndex != 0) ? Convert.ToInt32(DdlSLSO.SelectedItem.Value) : 0;
            objLSTFac.ActSLSO = (DdlActSLSO.SelectedIndex != 0) ? Convert.ToInt32(DdlActSLSO.SelectedItem.Value) : 0;
            objLSTFac.CoSLSO1 = (DdlCoSLSO.SelectedIndex != 0) ? Convert.ToInt32(DdlCoSLSO.SelectedItem.Value) : 0;
            objLSTFac.FacWebpage = TxtWeb.Text.Trim();
            objLSTFac.SopRevisedDate = (TxtSOP.Text != "") ? Convert.ToDateTime(TxtSOP.Text) : DateTime.MinValue;
            objLSTFac.ApprovalExpDate = (TxtExpirydate.Text != "") ? Convert.ToDateTime(TxtExpirydate.Text) : DateTime.MinValue;
            objLSTFac.LinkText = "";
            objLSTFac.LinkUrl = "";
            objLSTFac.PMId = (HdnPMId.Value != "") ? Convert.ToInt32(HdnPMId.Value) : 0;
            objLSTFac.CoordId = (HdnCoordId.Value != "") ? Convert.ToInt32(HdnCoordId.Value) : 0;

            if (mode == "edit")
            {
                objLSTFac.FacilityId = FacId;
                objLSTFac.ModifiedBy = objCommon.GetUserId();
                int _result = objDml.UpdateFacility(objLSTFac);
                if (_result == 0) { LblFacMsg.Text = "Facility - " + objLSTFac.FacilityName + " successfully updated"; }
                else { LblFacMsg.Text = "Error! " + objLSTFac.FacilityName + " could not be updated"; }
            }
            else
            {
                objLSTFac.CreatedBy = objCommon.GetUserId();
                int _facilityId = objDml.CreateFacility(objLSTFac);
                if (_facilityId != 0) { LblFacMsg.Text = "Facility - " + objLSTFac.FacilityName + " successfully added"; }
                else { LblFacMsg.Text = "Error! " + objLSTFac.FacilityName + " could not be added"; }
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialogFac('dialog-facmsg','Admin.aspx?mode=1');", true);

        }


        private void FillFormView(Business.Facility objFac)
        {

            LblFacilityVal.Text = objFac.FacilityName;
            LblBldgVal.Text = (objFac.Bldg != "") ? objFac.Bldg : "-";
            LblRoomVal.Text = (objFac.Room != "") ? objFac.Room.ToString() : "-";
            LblLocateVal.Text = (objFac.OtherLocation != "") ? objFac.OtherLocation.ToString() : "NA";
            if (objFac.FacWebpage != "")
            {
                aWebPage.Visible = true;
                LblWebVal.Visible = false;
                aWebPage.HRef = objFac.FacWebpage;
                aWebPage.InnerText = objFac.FacWebpage;
            }
            else
            {
                aWebPage.Visible = false;
                LblWebVal.Visible = true;
                LblWebVal.Text = "-";
            }
            //if (objFac.LinkUrl != "") 
            //{ 
            //    aLink.Visible = true; 
            //    aLink.HRef = objFac.LinkUrl; 
            //    aLink.InnerText = (objFac.LinkText != "")? objFac.LinkText: objFac.LinkUrl; 
            //}
            //else
            //{
            //    aLink.Visible = false; 
            //    LblLinkVal.Visible = true;
            //    LblLinkVal.Text = "-";
            //}
            LblSLSOVal.Text = (objFac.SLSOName != "") ? objFac.SLSOName : "-";
            LblActingVal.Text = (objFac.ActSLSOName != "") ? objFac.ActSLSOName : "-";
            LblcoSLSOVal.Text = (objFac.CoSLSOName != "") ? objFac.CoSLSOName : "-";
            LblSOPVal.Text = (objFac.SopRevisedDate == DateTime.MinValue) ? "-" : objFac.SopRevisedDate.ToShortDateString();
            LblExpdateVal.Text = (objFac.ApprovalExpDate == DateTime.MinValue) ? "-" : objFac.ApprovalExpDate.ToShortDateString();
            // UCLink1.Visible = false;
            SetSLSOforUC();
            UCWorkfac1.FacId = objFac.FacilityId;
            if ((objFac.AltSLSO != 0) && (objFac.AltSLSOTo >= DateTime.Today))
            {
                divaltslsoline.Visible = true;
                DivAltSlso.Visible = true;
                LblAltSlsoVal.Text = objCommon.GetEmpname(objFac.AltSLSO.ToString());
                LblFrom.Text = objFac.AltSLSOFrom.ToShortDateString();
                LblTo.Text = objFac.AltSLSOTo.ToShortDateString();

            }
            else { DivAltSlso.Visible = false; divaltslsoline.Visible = false; }
            LblPMVal.Text = (objFac.PMName != "") ? objFac.PMName : "-";
            LblCoordVal.Text = (objFac.CoordName != "") ? objFac.CoordName : "-";
            HdnCoordId.Value = Server.HtmlEncode(objFac.CoordId.ToString());
            HdnPMId.Value = Server.HtmlEncode(objFac.PMId.ToString());
            //Todo: Hide this link if already SLSO requested and it is in pending approval status
            if (objFac.SLSOSlacId.Equals(objCommon.GetUserId()))  //if the user is SLSO
            {
                //Check if there is a request for approval pending
                bool _isApprInProg = objDml.CheckIfFacApprovalInProg(objFac.FacilityId);
                if (!_isApprInProg)
                {
                    LnkReqFacApp.Visible = true;

                }
                else
                {
                    LnkReqFacApp.Visible = false;
                }
             }
            else
            {
                LnkReqFacApp.Visible = false;
            }
            UCOJTMatrix.Visible = true;
           
            SetUCforOJT(objFac);
          

        }


      protected override void OnInit(EventArgs e)
      {
            InitializeComponent();
 	         base.OnInit(e);
      }
      
        private void InitializeComponent()
      {
            UCWorkfac1.AssociationDeleted += UCWorkfac1_AssociationDeleted;
      }

        protected void UCWorkfac1_AssociationDeleted(object sender)
            
        {
            UCOJTMatrix.BindGrid();
        }

        private void SetUCforOJT(Business.Facility objFac)
        {
            UCOJTMatrix.FacId = objFac.FacilityId;
            UCOJTMatrix.FacWebSite = objFac.FacWebpage;
            UCOJTMatrix.FacName = objFac.FacilityName;
            UCOJTMatrix.IsSLSOAlt = IsSLSOAlt;
        }

        private void SetSLSOforUC()
        {
            if (IsSLSOAlt) 
            { UCWorkfac1.IsSLSOAlt = true; }
            else
            {
                UCWorkfac1.IsSLSOAlt = false;
            }
          
        }

        private void FillFormEdit(Business.Facility objFac)
        {
            TxtFacility.Text = objFac.FacilityName;
            if (DdlBldg.Items.FindByValue(objFac.Bldg) != null)
            {
                DdlBldg.ClearSelection();
                DdlBldg.Items.FindByValue(objFac.Bldg.ToString()).Selected = true;
            }
            else DdlBldg.SelectedIndex = 0;
            DdlBldg_SelectedIndexChanged(null, null);
            if (DdlRoom.Items.FindByValue(objFac.Room.ToString()) != null)
            {
                DdlRoom.ClearSelection();
                DdlRoom.Items.FindByValue(objFac.Room.ToString()).Selected = true;
            }
            TxtLocate.Text = objFac.OtherLocation.ToString();
            TxtWeb.Text = objFac.FacWebpage.ToString();
            if (DdlSLSO.Items.FindByValue(objFac.SLSO.ToString()) != null)
            {
                DdlSLSO.ClearSelection();
                DdlSLSO.Items.FindByValue(objFac.SLSO.ToString()).Selected = true;
            }
            else DdlSLSO.SelectedIndex = 0;
            if (DdlActSLSO.Items.FindByValue(objFac.ActSLSO.ToString()) != null)
            {
                DdlActSLSO.ClearSelection();
                DdlActSLSO.Items.FindByValue(objFac.ActSLSO.ToString()).Selected = true;
            }
            else DdlActSLSO.SelectedIndex = 0;
            if (DdlCoSLSO.Items.FindByValue(objFac.CoSLSO1.ToString()) != null)
            {
                DdlCoSLSO.ClearSelection();
                DdlCoSLSO.Items.FindByValue(objFac.CoSLSO1.ToString()).Selected = true;
            }
            else DdlCoSLSO.SelectedIndex = 0;
            TxtSOP.Text = (objFac.SopRevisedDate == DateTime.MinValue) ? "" : objFac.SopRevisedDate.ToShortDateString();
            TxtExpirydate.Text = (objFac.ApprovalExpDate == DateTime.MinValue) ? "" : objFac.ApprovalExpDate.ToShortDateString();
            //UCLink1.Visible = true;
            //UCLink1.Url = objFac.LinkUrl;
            //UCLink1.UrlText = objFac.LinkText;
            if (UCWorkfac1.Visible)
            {
                SetSLSOforUC();
                UCWorkfac1.FacId = objFac.FacilityId;
            }
            TxtPrgMgr.Text = objFac.PMName.ToString();
            TxtCoordinator.Text = objFac.CoordName.ToString();
            HdnPMId.Value = objFac.PMId.ToString();
            HdnCoordId.Value = objFac.CoordId.ToString();


        }

        //TODO: Need to fix this
        protected bool ValidLocation()
        {
            if ((DdlBldg.SelectedValue == "0" || DdlBldg.SelectedValue == "-1") && (TxtLocate.Text == ""))
            {
                CVLocation.IsValid = false;
                spnbldg.Visible = true;
                return false;
            }
            else
            {
                CVLocation.IsValid = true;
                spnbldg.Visible = false;
                return true;
            }

        }

        protected void SetPage(string mode, int facid)
        {
            if ((mode == "") || (mode.Equals("edit")))
            {
                PnlAdd.Visible = true;
                PnlView.Visible = false;
                FillDropdownList();
                if (mode != "") { FacId = facid; divAdd.Visible = false; divEdit.Visible = true; UCWorkfac1.Visible = true; }
                else { FacId = -1; divAdd.Visible = true; divEdit.Visible = false; UCWorkfac1.Visible = false; }

            }
            else if (mode.Equals("view"))
            {
                PnlAdd.Visible = false;
                PnlView.Visible = true;
                FacId = facid;
                UCWorkfac1.Visible = true;
            }
            if (FacId > 0)
            {
                Business.Facility objFac = new Business.Facility();
                objFac = objDml.GetFacilityDetails(FacId);
                if (mode.Equals("view")) { FillFormView(objFac); }
                else { FillFormEdit(objFac); }
            }
            Mode = mode;
            BindGridFile();
        }

        private void BindGridFile()
        {
            UCFileList1.ObjId = FacId;
            UCFileList1.ObjType = "Facility";
            if (UrlReferrer.Contains("Report") || (UrlReferrer.Contains("Worker")))
            {
                UCFileList1.HideDelete = true;
            }
            else UCFileList1.HideDelete = false;
            UCFileList1.BindFileGrid();
        }

        #endregion

        protected void CvPM_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = objValid.CheckUserValidity(TxtPrgMgr.Text);
        }

        protected void CvCoord_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = objValid.CheckUserValidity(TxtCoordinator.Text);
        }

        protected void LnkReqFacApp_Click(object sender, EventArgs e)
        {
            //Check if prgmgr and eshcorod is present
            bool _bCheckIfExists = false;
            _bCheckIfExists = CheckifPMCoord();


            if (!_bCheckIfExists)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-facmsg');", true);
            }
            else
            {

                Response.Redirect("~/FacilityApprovalRequest.aspx?objid=" + FacId + "&mode=req");
            }

        }

        protected bool CheckifPMCoord()
        {
            //TODO: Risk - Should i check if LSO is in the system as it can do partial insert if LSO is not there
            int _pmId = Convert.ToInt32(Server.HtmlDecode(HdnPMId.Value));
            int _coordId = Convert.ToInt32(Server.HtmlDecode(HdnCoordId.Value));
            bool _bresult;
            LblFacMsg.Text = "Request Facility Approval cannot be completed this time. ";
            if ((_pmId == 0) && (_coordId ==0))
            {
                LblFacMsg.Text += " Contact LSO to assign Program Mgr and ESH coordinator for this facility, as needed";
                _bresult = false;
            }
            else if (_pmId == 0)
            {
                LblFacMsg.Text += " Contact LSO to assign Program Mgr for this facility";
                _bresult = false;
            }
            else if (_coordId == 0)
            {
                LblFacMsg.Text += " Contact LSO to assign ESH Co-ordinator for this facility";
                _bresult = false;
            }

            else
            {
                LblFacMsg.Text = "";
                _bresult = true;
            }
            return _bresult;
        }

        protected void TxtPrgMgr_TextChanged(object sender, EventArgs e)
        {
            HdnPMId.Value = (TxtPrgMgr.Text != "") ? objCommon.GetEmpid(TxtPrgMgr.Text).ToString() : "0";
        }

        protected void TxtCoordinator_TextChanged(object sender, EventArgs e)
        {
            HdnCoordId.Value = (TxtCoordinator.Text != "") ? objCommon.GetEmpid(TxtCoordinator.Text).ToString() : "0";
        }
    }     
}