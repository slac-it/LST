//$Header:$
//
//  Approval.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the Approval page of Laser safety tool.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace LST
{
    public partial class Approval1 : BasePage
    {
        Business.UserRoles objRoles = new Business.UserRoles();
        Business.Common_Util objCommon = new Business.Common_Util();
        Data.DML_Util objDml = new Data.DML_Util();
        Business.Employee objEmployee = new Business.Employee();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                string _requestId = Request.QueryString["objid"];
                int _mapId;
                if (!Int32.TryParse(_requestId, out _mapId))
                {
                    //request id is not number
                    Response.Redirect("Error.aspx?msg=parserr");
                }
                else if (!objDml.CheckIfIdExists(Convert.ToInt32(_requestId), "map"))
                {
                    Response.Redirect("Error.aspx?msg=notexists");
                }
                else
                {
                    UrlReferrer = (Request.UrlReferrer != null) ? Request.UrlReferrer.ToString() : "";
                    CheckAccessibility(_mapId);
                }
               

            }
            SetAllUserControls();
            
        }

        public int WorkerId { 
            get
            {
                return (ViewState["workerid"] != null ? Convert.ToInt32(ViewState["workerid"]) : 0 );
            }
            
            set
            {
                ViewState["workerid"] = value;
            }
        
        }
        public int MapId {
            
            get
            {
                return (ViewState["mapid"] != null ? Convert.ToInt32(ViewState["mapid"]) : 0);
            }
            
            set
            {
                ViewState["mapid"] = value;
            }
        
        }
        public int WorkerSlacId
        {
            get
            {
                return (ViewState["workerslacid"] != null) ? Convert.ToInt32(ViewState["workerslacid"]) : 0;
            }
            set
            {
                ViewState["workerslacid"] = value;
            }
        }
        public int FacilityId { get; set; }

        private void SetAllUserControls()
        {
            UCFile1.ObjId = WorkerId;
            UCFile1.AttachButtonClicked += new EventHandler(UCFile1_AttachButtonClicked);
            UCFile1.ObjType = "Worker";
            UCFile1.DocType = "253PRA";
            //UCFile2.ObjId = MapId;
            //UCFile2.AttachButtonClicked += new EventHandler(UCFile2_AttachButtonClicked);
            //UCFile2.ObjType = "WorkFac";
            //UCFile2.DocType = "OJT";
            UCApphis.MapId = MapId;
        }

        protected void CheckAccessibility(int objId)
        {
                int _facilityId = objDml.GetFacilityId(objId);
                int _workerId = objDml.GetWorkerId(objId);
                int _workerslacId = objDml.GetSlacId(_workerId);

                objRoles.GetUserRole(objCommon.GetUserId().ToString(), _facilityId, _workerslacId);

                //if (objRoles.IsSLSO || objRoles.IsLSOrAlt()  || objRoles.IsAltSLSO || objRoles.IsAdminSvr || objRoles.IsAltSVR || objRoles.IsAdmin
                //    || (_workerslacId.ToString() == objCommon.GetUserId()))
                //{
                    SetPage(objRoles, _facilityId, _workerId, objId, _workerslacId);
               // }
                
               
        }

        private void HideApprovalDiv(Business.UserRoles objRoles, int mapId, int workerSlacId)
        {
            //SLSO approved and the user is slso or altslso and not lso or alt or adminsvr
            //LSO approve and user is lso or altlso and not slso or altslos or admin svr
            //Admin svr approved and the user is adminsvr or altsvr and not lso,altslo, slso or altslso
            // admin or worker and not LSO/Alt LSO

            //Check if the whole request is pending before showing the approval panel (this is for view pages)
            int _statId = objDml.GetStatusId(mapId);
            string _mode = "";

            
            bool _visible = false;

            if (_statId.Equals(2))
            {
                if ((objRoles.IsSLSO || objRoles.IsAltSLSO) && !(objDml.IsApproved("SLSO", mapId)))
                {
                    _visible = true;
                }
                if ((objRoles.IsLSOrAlt()) && !(objDml.IsApproved("LSO", mapId)))
                {
                    _visible = true;
                }
                if ((objRoles.IsAdminSvr || objRoles.IsAltSVR) && !(objDml.IsApproved("ADMSVR", mapId)))
                {
                    _visible = true;

                }

            }

            if (_visible) {
                Divbuttons.Visible = true; PnlApproval.Visible = true;
                DivWrkStat.Visible = true; _mode = "approve";
                if (UrlReferrer.Contains("Reports"))
                {
                    DivBack.Visible = true;
                }
                else
                {
                    DivBack.Visible = false;
                }
               
            }
            else
            {
                _mode = ViewForNonApprovers();
            }
            HdnMode.Value = _mode;
           
        }

        private string ViewForNonApprovers()
        {
            Divbuttons.Visible = false;
            PnlApproval.Visible = false;
            DivWrkStat.Visible = false;
            DivBack.Visible = true;
            return "view";
        }

        protected void SetPage(Business.UserRoles objRoles, int facId,int workerId, int mapId, int workerSlacId)
        {
            //Check if 253 PRA is attached for the worker or not
            HideApprovalDiv(objRoles, mapId, workerSlacId);
            Business.WorkerFacility objWorkFac = new Business.WorkerFacility();
            objWorkFac = objDml.GetWorkerFacilityDetails(Convert.ToInt32(mapId));
             bool _if253PRAAttached = true;
             //bool _ifOJTAttached = objDml.CheckifOJTAttached(mapId);

            if (objWorkFac.WorkType.Equals("QLO"))
            {
                 _if253PRAAttached  = objDml.Checkif253PRAAttached(workerId);
            }
           
            if ((objRoles.IsSLSO) || (objRoles.IsAltSLSO))
            {
                if (!(objDml.IsApproved("SLSO", mapId)))
                {
                    DivOJT.Visible = true;
                    //DivOJTFile.Visible = ! _ifOJTAttached;
                    divlineOJT.Visible = true;
                    DivOJTinst.Visible = true;
                    Div253PRAFile.Visible = !_if253PRAAttached;
                }
                else
                {
                    DivOJT.Visible = false;
                    //DivOJTFile.Visible = false;
                    divlineOJT.Visible = false;
                    DivOJTinst.Visible = false;
                    Div253PRAFile.Visible = false;
                }
                              
                SpnSLSO.Visible = true;
            }
            else
            {
                DivOJT.Visible = false;
                //DivOJTFile.Visible = false;
                divlineOJT.Visible = false;
                DivOJTinst.Visible = false;
                if (objRoles.IsLSOrAlt())
                {
                    if (!(objDml.IsApproved("LSO", mapId)))
                    {
                        Div253PRAFile.Visible = !_if253PRAAttached;
                    }
                    else
                    {
                        Div253PRAFile.Visible = false;
                    }
                    
                }
                else
                {
                    Div253PRAFile.Visible = false;
                }              
            }
            PopulateRequestInfo(objWorkFac);
            if (!Div253PRAFile.Visible)
            {
                DivFiles.Visible = true;
                BindFileGrid(workerId, mapId);
            }
            WorkerId = workerId;
            FacilityId = facId;
            MapId = mapId;
            WorkerSlacId = workerSlacId;
        }

        protected void PopulateRequestInfo(Business.WorkerFacility objWorkFac)
        {
            
            if (objWorkFac.FacilityId != 0)
            {
                //DivFac.Visible = true;
                //Divrowline.Visible = true;
                LblFacilityVal.Text = objWorkFac.FacilityName;
            }
            else
            {
                //DivFac.Visible = false;
                //Divrowline.Visible = false;
                LblFacilityVal.Text = "None selected";
            }
            LblRequestIdVal.Text = objWorkFac.MapId.ToString();
            LblWorkTypeVal.Text = objWorkFac.WorkType;
            //LblFacilityVal.Text = objWorkFac.FacilityName;
            //LblConditionalVal.Text = objWorkFac.ConditionalApproval;
            if (objWorkFac.ConditionalApproval == "Y")
            {
                DivJustify.Visible = true;
               // DivrowJustify.Visible = true;
                LblJustifyVal.Text = objWorkFac.Justification;
                LtrlInfo.Text = "The Worker listed below is requesting approval for the laser facility indicated. All SLAC requirements for this have been met except for ESH131, Laser Lessons Learned class. Conditional approval is requested with the justification given. Note: ESH131 must be completed within 30 days to remain active.";

            }
            else
            {
                DivJustify.Visible = false;
                LtrlInfo.Text = "The Worker listed below is requesting approval for the laser facility indicated. All SLAC requirements for this have been met.";
                        }
           
            LblSOPVal.Text = (objWorkFac.SOPReviewDate.ToShortDateString() != DateTime.MinValue.ToShortDateString()) ? objWorkFac.SOPReviewDate.ToShortDateString() : "-";
            LblStatusVal.Text = objWorkFac.Status;
            if (objWorkFac.StatusId == 4) { Divbuttons.Visible = false; PnlApproval.Visible = false; }
            Business.Worker objWorker = new Business.Worker();
            objWorker = objDml.GetWorkerDetails(objWorkFac.WorkerId, "worker");
            LblOperatorVal.Text = objWorkFac.Worker;
            LblBadgeIdVal.Text = objEmployee.GetBadgeId(objWorker.SlacId.ToString()).ToString() ;
            LblEmailVal.Text  = objWorker.EmailAddr;
            LblPrefEmailVal.Text = objWorker.PreferredEmail;
            LbAffiliateVal.Text = objWorker.Affiliation;
            LblSvrVal.Text = objWorker.Supervisor;
            if ((!string.IsNullOrEmpty(objWorker.AlternateSvr)) && (objWorker.AlternateSvrTo >= DateTime.Today))
            {
                LblAltSvrVal.Text = objCommon.GetEmpname(objWorker.AlternateSvr);
            }
            else LblAltSvrVal.Text = "-";
           
            if (objWorkFac.OJTCompletionDate != DateTime.MinValue)
            {
                DivOJTDate.Visible = true;
               LblOJTDateval.Text = objWorkFac.OJTCompletionDate.ToShortDateString();
                DivOJT.Visible = false;
                DivOJTinst.Visible = false;
                divlineOJT.Visible = false;
               // DivOJTFile.Visible = false;
            }
            else DivOJTDate.Visible = false;
            LblSlacIdVal.Text = objWorker.SlacId.ToString();
            if ((!DivOJTDate.Visible) && (!DivJustify.Visible)) { Divojtjustifyline.Visible = false; }
            string _globalStat = objDml.CheckIfWorkerActiveGlobally(objWorker.SlacId.ToString());
            LblWorkerStatVal.Text = _globalStat;
            if (_globalStat != "None") 
            { aWrkerlink.Visible = true;
                     aWrkerlink.HRef = "~/WorkerEntry.aspx?mode=view&id=" + objWorker.WorkerId;
                     aWrkerlink.InnerText = ";see Worker's Report for information";
                     aWrkerlink.Target = "_blank";
            }
            else
            {
                aWrkerlink.Visible = false;
              
            }
        }

        

        protected void CVcomments_ServerValidate(object source, ServerValidateEventArgs args)
        {
            
        }

        protected void BtnApprove_Click(object sender, EventArgs e)
        {
            bool _isErr = true;
            string _OJTmsg = "";
            //check if 253PRA attached.. for LSO / SLSO
            Page.Validate();
            if (Page.IsValid)
            {
                if (Div253PRAFile.Visible)
                {
                    LblMsg.Text = "Please make sure to attach 253PRA for the Worker before taking action";
                    _isErr = true;
                }
               // else if ((!DivOJTFile.Visible) && (DivOJT.Visible) && (TxtOJTDate.Text == ""))
                //else if ((DivOJT.Visible) && (TxtOJTDate.Text == ""))
                //{
                //    LblMsg.Text = "Please make sure to enter the OJT completion date if OJT is completed";
                //    _isErr = true;
                //}
                //Not needed now
                //else if ((DivOJT.Visible) && (TxtOJTDate.Text != "") && (DivOJTFile.Visible))
                //{
                //    LblMsg.Text = "Please make sure to attach OJT File if you entered OJT Completion date";
                //    _isErr = true;
                //}
                else
                {
                    //if LSO - update the lst approval workflow with status as approved 
                    //check to make sure that others have not approved yet.. if so update the worker facility map with status as active
                    //Even one approver declines, update the status of both lstapproval workflow and workfacilitymap as declined and this request becomes obsolete
                    CheckAccessibility(MapId);
                    _isErr = false;
                    if (objRoles.IsLSOrAlt())
                    {
                       
                       
                        FillNSaveApproval(3, "LSO");
                    }
                    if (objRoles.IsSLSO || objRoles.IsAltSLSO)
                    {
                        
                       // if ((DivOJTFile.Visible) && (TxtOJTDate.Text == ""))
                        if (TxtOJTDate.Text == "")
                        {
                            _OJTmsg = " A 30 day grace period is allowed if OJT not yet completed and OJT Completion date not entered.";
                            _OJTmsg += " After 30 days, if OJT information is still not completed, then QLO/LCA worker status will be changed to inactive";
                            
                        }
                        FillNSaveApproval(3, "SLSO");
                    }
                    if (objRoles.IsAdminSvr || objRoles.IsAltSVR)
                    {           
                      
                        FillNSaveApproval(3, "ADMSVR");
                    }
                   

                }
                if (_isErr)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-appreqmsg');", true);
                }
                else
                {
                    if (_OJTmsg != "") { LblMsg.Text =  LblMsg.Text + _OJTmsg ; }
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialogFac('dialog-appreqmsg','Default.aspx');", true);
                }
               
               
            }
            
        }

        protected void FillNSaveApproval(int statusId, string approverType)
        {
            Business.Approval objApproval = new Business.Approval();

            objApproval.MapId = MapId;
            objApproval.ApproverId = Convert.ToInt32(objCommon.GetUserId());
            objApproval.StatusId = statusId;
            objApproval.Comments = objCommon.ReplaceSC(TxtComments.Text);
            objApproval.ApproverType = approverType;
            DateTime _oJTCompletionDate = (DivOJT.Visible) ? ((TxtOJTDate.Text != "") ? Convert.ToDateTime(TxtOJTDate.Text) : DateTime.MinValue) : DateTime.MinValue;


            int _result = objDml.UpdateApproval(objApproval, _oJTCompletionDate);
            if (_result == 0)
            {
                LblMsg.Text = "Your action submitted successfully! ";
            }
            else
            {
                LblMsg.Text = "Error submitting the action";
            }

        }

        protected void BtnDecline_Click(object sender, EventArgs e)
        {
            //comments are not blank
            if (Page.IsValid)
            {
                if (TxtComments.Text == "")
                {
                    CVcomments.IsValid = false;
                    SpnComments.Visible = true;
                }
                else
                {
                    CheckAccessibility(MapId);

                    if (objRoles.IsLSOrAlt())
                    {
                        FillNSaveApproval(4, "LSO");
                    }
                    if (objRoles.IsSLSO || objRoles.IsAltSLSO)
                    {
                        FillNSaveApproval(4, "SLSO");
                    }
                    if (objRoles.IsAdminSvr || objRoles.IsAltSVR)
                    {
                        FillNSaveApproval(4, "ADMSVR");
                    }
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialogFac('dialog-appreqmsg','Default.aspx');", true);
            }
          
              
            }
           

        }

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void BindFileGrid(int workerId, int mapId)
        {
             DataSet _dsFile = new DataSet();
            
            _dsFile = objDml.GetFileInfo(workerId, mapId);
            if (_dsFile.Tables["file"].Rows.Count > 0)
            {
                DivFiles.Visible = true;
                GvFile.DataSource = _dsFile.Tables["file"];
                GvFile.DataBind();
            }
            else
            {
                GvFile.DataSource = null;
                GvFile.DataBind();
            }
        }

        private void UCFile1_AttachButtonClicked(object sender, EventArgs e)
        {
           Div253PRAFile.Visible = false;
            BindFileGrid(WorkerId, MapId);
        }

        //private void UCFile2_AttachButtonClicked(object sender, EventArgs e)
        //{
        //    //DivOJTFile.Visible = false;
        //    BindFileGrid(WorkerId, MapId);
            
        //}

        protected void gvFile_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int _attachmentId = Convert.ToInt32(e.CommandArgument.ToString());
            GridViewRow gvrow;
               string _docType ="";
            //string _filename = "";
            string _objType = "";

           
            if (e.CommandName == "download")
            {
                gvrow = (GridViewRow)(((LinkButton)e.CommandSource).NamingContainer);
                _docType = ((DataControlFieldCell)gvrow.Cells[4]).Text;
                if (_docType == "253PRA") { _objType = "Worker"; }
                else if (_docType == "OJT") { _objType = "WorkFac"; }
                (this.Page as BasePage).FileData(_attachmentId, _objType);
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
        protected void BtnBack_Click(object sender, EventArgs e)
        {
            if (UrlReferrer.Contains("Reports.aspx"))
            {
                Response.Redirect("Reports.aspx?dd=9");
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }
    }
}