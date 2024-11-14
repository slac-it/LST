//$Header:$
//
//  ApprovalRequest.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the approval request page of Laser safety tool.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace LST
{
    public partial class ApprovalRequest : System.Web.UI.Page
    {
        Business.Common_Util objCommon = new Business.Common_Util();
        Data.DML_Util objDml = new Data.DML_Util();
        Business.Employee objEmp = new Business.Employee();
        Business.Validation objValid = new Business.Validation();
        Business.Rules objRules = new Business.Rules();
        Business.UserRoles objRoles = new Business.UserRoles();
        protected enum WorkerStat
        {
            Active, //exists and active
            Inactive, //exists and inactive
            New, //new worker
            Deleted //soft deleted from system before..slacid exists but their IS_ACTIVE is N
        }

        #region "Properties"
        public string EmpType
        {
            get
            {
                return (DdlSlacAffiliation.SelectedValue == "9" ? "emp" : "noemp");
             }
        }

        public string WorkType
        {
            get
            {
                return (DdlWorkType.SelectedValue == "7" ? "QLO" : "LCA");
            }
        }
   
      

        protected string SlacId
        {
            get
            {
              // return (divOp.Visible? objCommon.GetEmpid(TxtWorker.Text).ToString() : objCommon.GetEmpid(LblOperatorVal.Text).ToString() );
              // return (divOp.Visible ? LblEmpId.Text.ToString() : objCommon.GetUserId().ToString());
              //  return (divOp.Visible ? objCommon.GetEmpid(TxtWorker.Text).ToString() : objCommon.GetEmpid(LblOperatorVal.Text).ToString());
                return (HdnEmpid.Value != "") ? HdnEmpid.Value.ToString() : "";
            }
            set
            {
                HdnEmpid.Value = value;
            }
        }

        protected WorkerStat WorkerStatus
        { //New, Active, Inactive
            get
            {
                return (WorkerStat)ViewState["workerstatus"];
            }
            set
            {
                ViewState["workerstatus"] = value;
            }
        }

        protected string ValidateMsg
        {
            get;
            set;
        }

        protected bool IsSLSOAlt
        {
            get
            {
                return (ViewState["slso"] != null) ? (bool)ViewState["slso"] : false;
            }
            set
            {
                ViewState["slso"] = value;
            }
        }

        private string SelFac
        {
            get
            {
                return (ViewState["selfac"] != null) ? ViewState["selfac"].ToString() : "";
            }
            set
            {
                ViewState["selfac"] = value;
            }
        }

        public string WorkerId
        {
            get;
            set;
        }

        #endregion 

        #region "Page / General"

        protected void Page_Load(object sender, EventArgs e)
        {
            ImgBtAlt.Attributes.Add("onClick", "OpenDialogForName('dialogowneradmin','TxtAlternate'); return false;");
            if (!Page.IsPostBack)
            {
                CheckAccessibility();
                
            }
            SetAllUserControls();
          
           
        }

        protected void SetPage(string workerslacId)
        {
            
            objEmp.GetEmpDetails(workerslacId);
            if (divOpview.Visible)
            { LblOperatorVal.Text = objEmp.EmpName; }
            else if (divOp.Visible)
            { TxtWorker.Text = objEmp.EmpName;
            LblEmpId.Text = workerslacId;
            }
            LblBadgeVal.Text = objEmp.BadgeId;
            LblEmailVal.Text =  objEmp.Email;
            LblSupervisorVal.Text = objCommon.GetEmpname(objEmp.Supervisor);
            UCTraining1.Visible = true;
            LblSlacIdVal.Text = workerslacId;
            PopulateWorkerInfo();              
          
            if (WorkerStatus.Equals(WorkerStat.New) || WorkerStatus.Equals(WorkerStat.Deleted))
            {
                ResetAllFields();
                RbEshManual.Visible = true;
                LblEshManualVal.Visible = false;
            }
            if (LblEmailVal.Text != "") { RFVPreferred.Enabled = false; }
            else { RFVPreferred.Enabled = true; }
            Check131Status();
        }

        private void CheckWorkerStatus(string workerslacId)
        {
            // Possibility 1 : If SLSO,LSO,Admin - filling on behalf of someone
            // Possibility 2: If new user, do not have a worker portfolio
            // Possibility 3: If not a new user, already existing but active
            // Possibility 4: If not a new user, already existing but inactive


            if (!string.IsNullOrEmpty(workerslacId))
            {
                bool _IsExists = objDml.CheckIfWorkerExists(workerslacId);
                if (_IsExists)
                {
                    bool _check = objDml.CheckIfActive(workerslacId);
                    if (_check)
                    {
                        WorkerStatus = WorkerStat.Active;
                    }
                    else
                    {
                        WorkerStatus = WorkerStat.Inactive;
                       // LblWorkerMsg.Text = "Error! Worker is not active. Request for approval can be created only for active workers";
                    }
                }
                else
                {
                    bool _didExisted = objDml.CheckIfWorkerExists(workerslacId, "N");
                    if (!_didExisted)
                    {
                        WorkerStatus = WorkerStat.New;
                    }
                    else
                    {
                        WorkerStatus = WorkerStat.Deleted;
                    }
                    
                }

            }
            //if deleted, request approval should work the same way as that of a new worker 
          //  if (WorkerStatus.Equals(WorkerStat.Active) || WorkerStatus.Equals(WorkerStat.New) || WorkerStatus.Equals(WorkerStat.Deleted))
           // {
                SetPage(workerslacId);
            //}
           // else { Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialogFac('dialog-appreqmsg','Default.aspx');", true); }


        }

        private void SetPageForLSO()
        {
            divOp.Visible = true;
            divOpview.Visible = false;
            ImgBtnWorker.Attributes.Add("onClick", "OpenDialogForName('dialogowneradmin','TxtWorker','LblEmpId'); return false;");
        }

        private void ResetAllFields()
        {
            TxtPreferredEmail.Text = "";
            DdlSlacAffiliation.SelectedIndex = 0;
            DivAltsvr.Visible = false;
            UCUserLab.Visible = false;
          
        }

        private void SetAllUserControls()
        {
           
            UCTraining1.EmpId = SlacId;
            UCTraining1.EmpType = EmpType;
            UCUserLab.UserId = SlacId;
            UCUserLab.ShowText = true;
            UCTraining1.WorkType = WorkType;
        }


       
        private void CheckAccessibility()
        {

            bool _isSLSOAlt = false;
            objRoles.GetUserRole(objCommon.GetUserId().ToString(), 0, 0);

            if ((objRoles.IsSLSOGen) || (objRoles.IsAltSLSOGen) || (objRoles.IsLSOrAlt()))
            {
                SetPageForLSO();
                if (((objRoles.IsSLSOGen) || (objRoles.IsAltSLSOGen)) && !(objRoles.IsLSOrAlt()))
                {
                    _isSLSOAlt = true;
                    
                }
            }
            IsSLSOAlt = _isSLSOAlt;
            HdnEmpid.Value = objCommon.GetUserId();
           CheckWorkerStatus(objCommon.GetUserId());
            FillDropdownlists(); //Initially fill all the facility for slso
        }

        protected void Check131Status()
        {
            string _status131 = objDml.GetTrainingStatus(SlacId, "131").Substring(0, 3).ToUpper();
            if (_status131.Equals("NEV"))
            {
                DivCondition.Visible = true;
            }
            else
            {
                DivCondition.Visible = false;
            }
        }

        protected void PopulateWorkerInfo()
        {
            Business.Worker objWorker = new Business.Worker();
            objWorker = objDml.GetWorkerDetails(Convert.ToInt32(SlacId), "slac");
            TxtPreferredEmail.Text = objWorker.PreferredEmail;
            if (DdlSlacAffiliation.Items.FindByValue(objWorker.AffiliationId.ToString()) != null)
            {
                DdlSlacAffiliation.SelectedValue = objWorker.AffiliationId.ToString();
                DdlSlacAffiliation_SelectedIndexChanged(null, null);
            }
            else { DdlSlacAffiliation.SelectedIndex = 0; }
            if ((objWorker.IsESHManualReviewed == "Y") && (objWorker.ManualReviewDate != DateTime.MinValue))
            {
                RbEshManual.Visible = false;
                LblEshManualVal.Visible = true;
                LblEshManualVal.Text = objWorker.ManualReviewDate.ToShortDateString();
                spnESHManual.Visible = false;
            }
            else
            {
                RbEshManual.Visible = true;
                LblEshManualVal.Visible = false;
                spnESHManual.Visible = true ;
            }
            if ((objWorker.IsStudReqReviewed == "Y") && (objWorker.StudentReqReviewDate != DateTime.MinValue))
            {
                RblStudreq.Visible = false;
                LblStudVal.Visible = true;
                LblStudVal.Text = objWorker.StudentReqReviewDate.ToShortDateString();
            }
            else
            {
                RblStudreq.Visible = true;
                LblStudVal.Visible = false;
            }
            if ((objWorker.AlternateSvr != "") && (objWorker.AlternateSvrTo >= DateTime.Today))
            {
                DivAltsvr.Visible = true;
                DivAltsvrAdd.Visible = false;
                LblAltSvrVal.Text = objCommon.GetEmpname(objWorker.AlternateSvr);
                LblFrom.Text = objWorker.AlternateSvrFrom.ToShortDateString();
                LblTo.Text = objWorker.AlternateSvrTo.ToShortDateString();
            }
            else
            {
                DivAltsvr.Visible = false;
                objRoles.GetUserRole(objCommon.GetUserId().ToString(), 0, 0);
                if ((objRoles.IsSLSOGen) || (objRoles.IsAltSLSOGen) || (objRoles.IsLSOrAlt()))
                {
                    DivAltsvrAdd.Visible = true;
                }
                else { DivAltsvrAdd.Visible = false;  }
                LblAltSvrVal.Text = "";
                LblFrom.Text = "";
                LblTo.Text = "";
            }
           
            UCUserLab.Visible = true;
            
        }

        protected void PopulateFacilityInfo()
        {
            Business.Facility objFacility = new Business.Facility();
            int _facilityId = Convert.ToInt32(DdlLab.SelectedValue);

            if (_facilityId != -1)
            {
                objFacility = objDml.GetFacilityDetails(_facilityId);
                LblBldgVal.Text = (objFacility.Bldg != "") ? objFacility.Bldg : " - ";
                LblRoomVal.Text = (objFacility.Room != "") ? objFacility.Room.ToString() : " - ";
                LblLocationVal.Text = (objFacility.OtherLocation != "") ? objFacility.OtherLocation.ToString() : " - ";
                LblSLSOVal.Text = (objFacility.SLSOName != "") ? objFacility.SLSOName : " - ";
                aFacSite.HRef = objFacility.FacWebpage;
                Lblfac.Text = objFacility.FacilityName;
                RblSOP.SelectedValue = "1";
                if ((objFacility.AltSLSO != 0) && (objFacility.AltSLSOTo > DateTime.Now))
                {
                    LblAltSLSOVal.Text = objCommon.GetEmpname(objFacility.AltSLSO.ToString());
                }
                else
                {
                    LblAltSLSOVal.Text = "-";
                }
                //objRoles.GetUserRole(objCommon.GetUserId().ToString(), _facilityId, 0);
                //if (( (objRoles.IsSLSO || objRoles.IsAltSLSO) && (!objRoles.IsLSOrAlt())) || (objRoles.IsLSOrAlt()))
                //{
                //    divOp.Visible = true;
                //    divOpview.Visible = false;
                //    TxtWorker.Text = objCommon.GetEmpname(objCommon.GetUserId());
                //}
                //else
                //{                
                //    divOp.Visible = false;
                //    divOpview.Visible = true; ;
                //    LblOperatorVal.Text = objCommon.GetEmpname(objCommon.GetUserId());
                //}
            }
            else { ToggleFacInfo(false); }
            
        }

        protected void Togglerow()
        {
            if ((DivSOPterms.Visible) && (divStudent.Visible)) { divrowstud.Visible = true; }
            else { divrowstud.Visible = false; }
        }

        private void ToggleFacInfo(bool boolval)
        {
            DivSLSO.Visible = boolval;
            divlocation.Visible = boolval;
            DivSOPterms.Visible = boolval;
        }

        protected int FillNSaveWorkerObject(WorkerStat workerstat)
        {
            int _result = 0;
            Business.Worker objWorker = new Business.Worker();

            objWorker.PreferredEmail = TxtPreferredEmail.Text.Trim();
            objWorker.AffiliationId = (DdlSlacAffiliation.SelectedIndex != 0) ? Convert.ToInt32(DdlSlacAffiliation.SelectedValue) : 0;
            objWorker.SlacId = Convert.ToInt32(SlacId); //(divOpview.Visible) ? objCommon.GetEmpid(LblOperatorVal.Text.Trim()) : objCommon.GetEmpid(TxtWorker.Text.Trim());
          
            objWorker.AlternateSvr = ((DivAltsvrAdd.Visible) && (TxtAlternate.Text != "")) ? objCommon.GetEmpid(TxtAlternate.Text).ToString() : "";

            if (objWorker.AlternateSvr != "")
            {
                objWorker.AlternateSvrFrom = (TxtFromDate.Text != "") ? Convert.ToDateTime(TxtFromDate.Text) : DateTime.MinValue;
                objWorker.AlternateSvrTo = (TxtToDate.Text != "") ? Convert.ToDateTime(TxtToDate.Text) : DateTime.MinValue;
            }
            int _workType = (DdlWorkType.SelectedIndex != 0) ? Convert.ToInt32(DdlWorkType.SelectedValue) : 0;
            bool _override = objRules.OverrideTraining();
            if (!_override)
            {
                bool _check = objRules.AreTrainingReqsMetForWrkr(objWorker.SlacId.ToString(), objWorker.AffiliationId, _workType);
                if (!_check)
                {
                    //Removing this as part of phase 2 requirement not needed to send email
                    //if (divOp.Visible)
                    //{
                    //    //Only if entered by slso or lso, send the email to authenticated user
                    //    //if the slso or lso is cnot the worker.. check
                    //    if (objWorker.SlacId != Convert.ToInt32(objCommon.GetUserId()))
                    //    {
                    //         int facId = (Convert.ToInt32(DdlLab.SelectedValue) > 0) ? Convert.ToInt32(DdlLab.SelectedValue) : 0;
                    //         objDml.SendEmailAuthenticatedUser(objWorker.SlacId, facId, objWorker.PreferredEmail);
                    //    }
                       

                    //}
                    return -1;
                }
            }
         

            if (workerstat == WorkerStat.New)
            {

                //_result is the worker id if successful insert
                objWorker.WorkerName = (divOpview.Visible) ? LblOperatorVal.Text.Trim() : TxtWorker.Text.Trim();
                objWorker.IsESHManualReviewed = "N";
                objWorker.IsStudReqReviewed = "N";
                objWorker.CreatedById = objCommon.GetUserId();
               
          
                _result = objDml.CreateWorker(objWorker);
                if (_result > 0) {
                     objWorker.WorkerId = _result;
                    
                }
               
            }
            else if (workerstat == WorkerStat.Deleted)
            {
                objWorker.IsESHManualReviewed = "N";
                objWorker.IsStudReqReviewed = "N";
                objWorker.ModifiedBy = objCommon.GetUserId();
                objWorker.WorkerId = objDml.GetWorkerId(objWorker.SlacId.ToString(), "N");
                
                _result = objDml.ReinstateWorker(objWorker);
            }
            else
            {
                //_result is 0 if updated successfully
                objWorker.ModifiedBy = objCommon.GetUserId();
                objWorker.WorkerId = objDml.GetWorkerId(objWorker.SlacId.ToString());
                _result = objDml.UpdateWorker(objWorker);
            }
            WorkerId = objWorker.WorkerId.ToString();


            return _result;
        }

        protected void FillNSaveWorkerLabRequest()
        {
            Business.WorkerFacility objWorkerFacility = new Business.WorkerFacility();
            //string _slacId = (divOp.Visible) ? objCommon.GetEmpid(TxtWorker.Text).ToString() : objCommon.GetEmpid(LblOperatorVal.Text).ToString();
            objWorkerFacility.WorkerId =Convert.ToInt32(WorkerId);//objDml.GetWorkerId(_slacId);
            objWorkerFacility.FacilityId = (DdlLab.SelectedIndex != 0 || DdlLab.SelectedIndex != 1) ? Convert.ToInt32(DdlLab.SelectedValue) : 0;
            objWorkerFacility.WorkTypeId = (DdlWorkType.SelectedIndex != 0) ? Convert.ToInt32(DdlWorkType.SelectedValue) : 0;
            objWorkerFacility.Justification = Server.HtmlEncode(objCommon.ReplaceSC(TxtJustification.Text));
            objWorkerFacility.ConditionalApproval = (DivCondition.Visible) ? ((TxtJustification.Text != "") ? "Y" : "N") : "N";
            objWorkerFacility.SOPReviewed = (DivSOPterms.Visible) ? ((RblSOP.SelectedValue == "0") ? "Y" : "N") : "N";
            objWorkerFacility.CreatedBy = objCommon.GetUserId();
            string _eshManualReviewed = (RbEshManual.Visible) ? ((RbEshManual.SelectedValue == "0") ? "Y" : "N") : "N";
            string _studReqreviewed = (divStudent.Visible) ? ((RblStudreq.Visible) ? ((RblStudreq.SelectedValue == "0") ? "Y" : "N") : "N") : "N";
            bool _condApp = (objWorkerFacility.ConditionalApproval == "Y") ? true : false;
            bool _CheckifReqsMet = false;

            _CheckifReqsMet = objRules.AllRulesMetForRequestApproval(objWorkerFacility.WorkerId, SlacId, objWorkerFacility.FacilityId, objWorkerFacility.WorkTypeId, _condApp, Convert.ToInt32(DdlSlacAffiliation.SelectedValue));

            if (_CheckifReqsMet)
            {
                int _mapId = objDml.CreateWorkerForLab(objWorkerFacility, _eshManualReviewed, _studReqreviewed);
                if (_mapId != 0) { LblWorkerMsg.Text = "Approval request successfully submitted"; }
                else { LblWorkerMsg.Text = "Error! Request for approval could not submitted."; }
            }
            else
            {
                LblWorkerMsg.Text = objRules.ApprReqMsg;
            }
            if (LblWorkerMsg.Text.Contains("Worker needs 131 Training"))
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-appreqmsg');", true);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialogFac('dialog-appreqmsg','Default.aspx');", true);
            }

        }

        #endregion

        #region "Dropdown Events"
        protected void FillDropdownlists()
        {
           
            DdlWorkType.DataSourceID = "SDSWorkType";
            DdlWorkType.DataTextField = "LOOKUP_DESC";
            DdlWorkType.DataValueField = "LOOKUP_ID";
            DdlWorkType.DataBind();

            DdlLab.DataSource = objDml.GetFacilityForSLSO(false).Tables["facslso"];
            DdlLab.DataTextField = "FACILITY_NAME";
            DdlLab.DataValueField = "FACILITY_ID";
            DdlLab.DataBind();

            DdlSlacAffiliation.DataSourceID = "SDSAffiliation";
            DdlSlacAffiliation.DataTextField = "LOOKUP_DESC";
            DdlSlacAffiliation.DataValueField = "LOOKUP_ID";
            DdlSlacAffiliation.DataBind();

            FillDropdownLab(false);

        }

        protected void FillDropdownLab(bool slsoonly)
        {
            DdlLab.Items.Clear();
            DdlLab.DataSource = objDml.GetFacilityForSLSO(slsoonly).Tables["facslso"];
            DdlLab.DataTextField = "FACILITY_NAME";
            DdlLab.DataValueField = "FACILITY_ID";
            DdlLab.DataBind();
        }

        protected void DdlSlacAffiliation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((DdlSlacAffiliation.SelectedValue == "10") || (DdlSlacAffiliation.SelectedValue == "11"))
            {                
                  divStudent.Visible = true;
                  UCTraining1.EmpType = "noemp";
            }
            else
            {
                 divStudent.Visible = false;
                if (DdlSlacAffiliation.SelectedValue == "9")
                {
                    UCTraining1.EmpType = "emp";
                }
                else { UCTraining1.EmpType = "noemp";  }
            }
            Togglerow();
               
        }
 
        protected void DdlLab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DdlLab.SelectedIndex != -1)
            {
                if (DdlLab.SelectedValue == "0")
                {
                    ToggleFacInfo(false);
                }
                else
                {
                    ToggleFacInfo(true);
                    divline.Visible = true;
                    PopulateFacilityInfo();
                }
            }
            Togglerow();
            SelFac = DdlLab.SelectedValue.ToString();

        }

        protected void DdlWorkType_DataBound(object sender, EventArgs e)
        {
            DdlWorkType.Items.Insert(0, new ListItem("--Choose One", "-1"));
        }

        protected void DdlLab_DataBound(object sender, EventArgs e)
        {
            DdlLab.Items.Insert(0, new ListItem("None", "0"));
            DdlLab.Items.Insert(0, new ListItem("--Choose One--", "-1"));
           
        }

        protected void DdlSlacAffiliation_DataBound(object sender, EventArgs e)
        {
            DdlSlacAffiliation.Items.Insert(0, new ListItem("--Choose One--", "-1"));
        }

        protected void DdlWorkType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DdlWorkType.SelectedValue == "7")
            {
                UCTraining1.WorkType = "QLO";
            }
            else
            {
                UCTraining1.WorkType = "LCA";
            }
        }
        #endregion

        #region "Server validate"

        protected void CvLab_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if ((objValid.CheckIfSelected(DdlLab.SelectedIndex.ToString())) && (objValid.CheckIfSelected(DdlLab.SelectedValue.ToString())))
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }

        protected void CvWorkType_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if ((objValid.CheckIfSelected(DdlWorkType.SelectedIndex.ToString())) && (objValid.CheckIfSelected(DdlWorkType.SelectedValue.ToString())))
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }

        protected void CVJustification_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (DivCondition.Visible)
            {
                if (TxtJustification.Text == "")
                {
                    args.IsValid = false;
                }
                else args.IsValid = true;
            }
            else args.IsValid = true;
        }

        protected void CvAffiliate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (DdlSlacAffiliation.Enabled)
            {
                if ((objValid.CheckIfSelected(DdlSlacAffiliation.SelectedIndex.ToString())) && (objValid.CheckIfSelected(DdlSlacAffiliation.SelectedValue.ToString())))
                {
                    args.IsValid = true;
                }
                else
                {
                    args.IsValid = false;
                }
            }

        }

        protected void CVSOP_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ((DivSOPterms.Visible) && (RblSOP.Visible)) ? ((RblSOP.SelectedValue == "1") ? false : true) : true;
        }

        protected void CVStudent_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ((divStudent.Visible) && (RblStudreq.Visible)) ? ((RblStudreq.SelectedValue == "1") ? false : true) : true;
        }

        protected void CVESH_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (RbEshManual.Visible) ? ((RbEshManual.SelectedValue == "1") ? false : true) : true;
        }

        protected void CvWorker_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = objValid.CheckUserValidity(TxtWorker.Text);

        }
        protected void CvAlt_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if ((DivAltsvrAdd.Visible) && (TxtAlternate.Text != ""))
            {
                args.IsValid = objValid.CheckUserValidity(TxtAlternate.Text);
            }
            else args.IsValid = true;          
        }

        #endregion
     
        #region "Button/TB Events"

        

        protected void TxtWorker_TextChanged(object sender, EventArgs e)
        {
            LblEmpId.Text = HdnEmpid.Value;
            SlacId = HdnEmpid.Value;
            if (TxtWorker.Text == "")
            {
                TxtWorker.Text = objCommon.GetEmpname(LblEmpId.Text);
            }
            if (TxtWorker.Text != "")
            {
               
                if (objCommon.IsEmpIDValid(LblEmpId.Text))               
                {
                    CheckWorkerStatus(SlacId);
                    SetAllUserControls();
                    
                }

            }
            else
            {
                UCTraining1.Visible = false;
                UCUserLab.Visible = false;
            }
            //if slso or altslso , list only the facilities they belong to
            if ((IsSLSOAlt) && (SlacId != objCommon.GetUserId()))
                FillDropdownLab(true);
            else FillDropdownLab(false);

            if (DdlLab.Items.FindByValue(SelFac) != null)
            {
                DdlLab.Items.FindByValue(SelFac).Selected = true;
            }
            else { DdlLab.SelectedValue = "-1"; }
            DdlLab_SelectedIndexChanged(null, null);
            UCUserLab.BindOJTGrid();
        }

       

        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            //Check if already a worker for this Lab 
            //QLO has to have 238 PRA training
            //131 requirement not fullfilled, then conditional request with justification
            //Read all requirements ..if no radiobutton, check if valid date is present
          
            Page.Validate(); //custom validator - server validate not firing
           
            if (Page.IsValid )
            {
                //check if the eshManual reviewed is yes and sop if lab selsected is yes and student req is yes for students

                int _workerId = 0;
                int _updateRes = 0;
                if (WorkerStatus == WorkerStat.New)
                {
                    _workerId = FillNSaveWorkerObject(WorkerStat.New);

                }
                else
                {
                    //string _slacId = (divOpview.Visible) ? objCommon.GetEmpid(LblOperatorVal.Text.Trim()).ToString() : objCommon.GetEmpid(TxtWorker.Text.Trim()).ToString();
                    if (WorkerStatus == WorkerStat.Deleted)
                    {
                        _workerId = objDml.GetWorkerId(SlacId, "N");      
                    }
                    else
                    {
                        _workerId = objDml.GetWorkerId(SlacId);   
                    }
                                    
                    _updateRes = FillNSaveWorkerObject(WorkerStatus);
                }

                if ((_workerId > 0) && (_updateRes == 0))
                    FillNSaveWorkerLabRequest();
                else
                {
                    LblWorkerMsg.Text = "Request Denied - This person does not meet the " + DdlWorkType.SelectedItem.Text + " training requirements.  They  must complete or renew courses as shown in the Training Summary";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-appreqmsg');", true);
                }



            }
            else
            {

                return;
            }

        }

        protected void BtnClear_Click(object sender, EventArgs e)
        {
            TxtPreferredEmail.Text = "";
            DdlWorkType.SelectedIndex = 0;
            DdlLab.SelectedIndex = 0;
            RbEshManual.SelectedValue = "1";
            RblStudreq.SelectedValue = "1";

            DdlSlacAffiliation.SelectedIndex = 0;
            ToggleFacInfo(false);
            divStudent.Visible = false;
            TxtJustification.Text = "";

        }

      

        #endregion

        protected void CustomFrom_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if ((DivAltsvrAdd.Visible) && (TxtAlternate.Text != "") && (TxtFromDate.Text == ""))
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void CustomTo_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if ((DivAltsvrAdd.Visible) && (TxtAlternate.Text != "") && (TxtToDate.Text == ""))
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

    }
}