//$Header:$
//
//  WorkerEntry.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the page where worker details can be edited or viewed.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using System.Drawing;

namespace LST
{
    public partial class WorkerEntry : BasePage
    {
        public string _mode;
        Data.DML_Util objDml = new Data.DML_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        Business.Employee objEmp = new Business.Employee();
        Business.Rules objRule = new Business.Rules();
        Business.Validation objValid = new Business.Validation();
        Business.UserRoles objRoles = new Business.UserRoles();

        protected void Page_Load(object sender, EventArgs e)
        {
            ImgBtnWorker.Attributes.Add("onClick", "OpenDialogForName('dialogowneradmin','TxtWorker'); return false;");
           
           if (!Page.IsPostBack)
           {
               string _mode = (Request.QueryString["mode"] != null) ? Request.QueryString["mode"].ToString() : "";
               HdnMode.Value = _mode;
               int _workerid = ((Request.QueryString["id"] != null) && (Request.QueryString["id"] != "")) ? Convert.ToInt32(Request.QueryString["id"]) : 0;
               string _reportdd = (Request.QueryString["dd"] != null) ? Request.QueryString["dd"].ToString() : "";
               ViewState["dd"] = _reportdd;
               UrlReferrer = (Request.UrlReferrer != null) ? Request.UrlReferrer.ToString() : null; 

               if (((objRoles.CheckAccessibility()) && (_mode != "")) || (UrlReferrer.Contains("Reports") && (_mode == "view")) || (UrlReferrer.Contains("Facility") && (_mode == "view"))
                   || (UrlReferrer.Contains("Approval") && (_mode == "view")))
               {
                   if ((_mode != "") && (!objDml.CheckIfIdExists(_workerid, "worker")))
                   {
                       Response.Redirect("Error.aspx?msg=notexists");
                   }

                   if (UrlReferrer.Contains("Approval") && (_mode == "view"))
                   {
                       BtnBack.Visible = false;
                   }

                   SetPage(_mode, _workerid);
               }
               else
               {
                   Response.Redirect("Error.aspx?msg=notauthorized");
               }
 
           }
           SetAllUserControls();
        }

      
        private void SetAllUserControls()
        {
            UCFile1.ObjId = WorkerId;
            UCFile1.Mode = Mode;
            UCFile1.AttachButtonClicked += new EventHandler(UCFile1_AttachButtonClicked);
            UCFile1.ObjType = "Worker";
            UCFile1.DocType = "253PRA";
            UCUserLab.Mode = Mode;
            UCUserLab.ShowText = true;
            UCTraining1.EmpType = EmpType;
            string _slacId;
            _slacId = objDml.GetSlacId(WorkerId).ToString();
            SlacId = _slacId;
            //if (Mode.Equals("edit") || Mode.Equals(""))
            //{
            //    UCTraining1.EmpId = _slacId;
            //    UCUserLab.UserId = _slacId;
             
            //}
            //else if (Mode.Equals("view"))
            //{
                UCTraining1.EmpId = _slacId;
                UCUserLab.UserId = _slacId;
           // }
            UCFileList1.DeleteButtonClicked += new EventHandler(UCFileList1_DeleteButtonClicked);
            UCPending1.WorkerId = WorkerId;
            UCPending1.ShowText = true;
           if (WorkerId == objDml.GetWorkerId(objCommon.GetUserId()))
           {
               //Check if user is admin or SLSO for the facility
               UCUserLab.HideEdit = false;
           }
           else { UCUserLab.HideEdit = true; }
            
           
          
        }

        #region "Properties"
        public string EmpType
        {
            get
            {
                if (PnlAdd.Visible)
                {
                    return (DdlSlacAffiliation.SelectedValue == "9" ? "emp" : "noemp");
                }
                else
                {
                    if (ViewState["affid"] != null)
                    {
                        return (Convert.ToInt32(ViewState["affid"]) == 9 ? "emp" : "noemp");
                    }
                    else { return "noemp"; }
                }
            }
        }

        public int WorkerId
        {
            get
            {
                return (int)ViewState["workerid"];
            }
            set
            {
                ViewState["workerid"] = value;
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

        private string UrlReferrer
        {
            get { return (ViewState["urlref"] != null ? ViewState["urlref"].ToString() : ""); }
            set { ViewState["urlref"] = value; }
        }

        private string SlacId
        {
            get;
            set;
        }

        #endregion

        private void BindGridFile()
        {
            UCFileList1.ObjId = WorkerId;
            UCFileList1.ObjType = "Worker";
            // only LSO or alternate can delete 253PRA
            if (objRoles.CheckAccessibility())
            {
                UCFileList1.HideDelete = false;
            }
            else { UCFileList1.HideDelete = true; }
            UCFileList1.BindFileGrid();
            if (((Panel)UCFileList1.FindControl("PnlFile")).Visible)
            {
                ToggleAttachment(false);

            }
            else
            {
                ToggleAttachment(true);
            }
        }

        private void UCFileList1_DeleteButtonClicked(object sender, EventArgs e)
        {
            ToggleAttachment(true);
        }

        private void UCFile1_AttachButtonClicked(object sender, EventArgs e)
        {
            ToggleAttachment(false);
            BindGridFile();
        }

        private void ToggleAttachment(bool boolval)
        {
            Div253File.Visible = boolval;
            DivFileInst.Visible = boolval;
        }
      
        protected void SetPage(string pgMode , int workerId)
      {
          if ((pgMode == "") || (pgMode.Equals("edit")))
          {
              PnlAdd.Visible = true;
              PnlView.Visible = false;
              //BtnActivate.Visible = false;
              //BtnDeactivate.Visible = false;
              if ((pgMode != "") && (pgMode.Equals("edit")))
              {
                  WorkerId = workerId; divAdd.Visible = false; divEdit.Visible = true;
                  UCTraining1.Visible = true;
                  UCUserLab.Visible = true;
                  divWrkerDetails.Visible = true;
                  spnName.Visible = false;
                  Lblformat.Visible = false;
                  ImgBtnWorker.Visible = false;
                  RFVWorker.Visible = false;
                  CvWorker.Visible = false;
                  TxtWorker.ReadOnly = true;
                  TxtWorker.BackColor = Color.Transparent;
                  TxtWorker.BorderStyle = BorderStyle.None;
                  DivStatus.Visible = true;
                  BtnDesignate.Attributes.Add("onClick", "OpenJQueryDialog('dialogDesignate','700','500','Designate.aspx?type=svr&id= " + WorkerId + "');return false;");
                 }
              else
              {
                  WorkerId = -1; divAdd.Visible = true; divEdit.Visible = false;
                  UCTraining1.Visible = false;
                  UCUserLab.Visible = false;
                  divWrkerDetails.Visible = false;
                  DivStatus.Visible = false;
              }
              FillDropdownlist();
          }
          else if (pgMode.Equals("view"))
          {
              PnlAdd.Visible = false;
              PnlView.Visible = true;
              WorkerId = workerId;
              UCTraining1.Visible = true;
              UCUserLab.Visible = true;
          }

          if (WorkerId > 0)
          {
              Business.Worker objWorker = new Business.Worker();
              objWorker = objDml.GetWorkerDetails(WorkerId,"worker");
              if (pgMode.Equals("view")) { FillFormView(objWorker); }
              else { FillFormEdit(objWorker); }
          }
          Mode = pgMode;
          BindGridFile();
      }

        private void FillFormView(Business.Worker objWorker)
        {
            LblWorkerVal.Text = objWorker.WorkerName;
            LblBadgeVal.Text = objEmp.GetBadgeId(objWorker.SlacId.ToString());
            LblAffiliationVal.Text = (objWorker.Affiliation != "")? objWorker.Affiliation:"-";
            ViewState["affid"] = objWorker.AffiliationId;
            LblEmailVal1.Text = objWorker.EmailAddr;
            LblPreferredVal.Text = (objWorker.PreferredEmail != "") ? objWorker.PreferredEmail:"-";
            LblStatusVal.Text = objWorker.Status;
            LblSlacIdVal.Text = objWorker.SlacId.ToString();
            LblSvrViewVal.Text = objWorker.Supervisor;
            LblESHVal.Text = (objWorker.ManualReviewDate != DateTime.MinValue) ? objWorker.ManualReviewDate.ToShortDateString() : "-";
           if ((!string.IsNullOrEmpty(objWorker.AlternateSvr)) && (objWorker.AlternateSvrTo >= DateTime.Today))
           {
               divrowAltsvr.Visible = true;
               DivAltSvr.Visible = true;
               LblAltSvrVal.Text = objCommon.GetEmpname(objWorker.AlternateSvr);
               LblFrom.Text = objWorker.AlternateSvrFrom.ToShortDateString();
               LblTo.Text = objWorker.AlternateSvrTo.ToShortDateString();

           }
           if ((objWorker.AffiliationId == 10) || (objWorker.AffiliationId == 11))
           {
               DivStudent.Visible = true;
               LblStudVal.Text = (objWorker.StudentReqReviewDate != DateTime.MinValue) ? objWorker.StudentReqReviewDate.ToShortDateString() : "-";
           }
           else DivStudent.Visible = false;
           
        }

        private void FillFormEdit(Business.Worker objWorker)
        {
            TxtWorker.Text = objWorker.WorkerName;
            LblBadgeIdVal.Text = objEmp.GetBadgeId(objWorker.SlacId.ToString());
            if (DdlSlacAffiliation.Items.FindByValue(objWorker.AffiliationId.ToString()) != null)
            {
                DdlSlacAffiliation.ClearSelection();
                DdlSlacAffiliation.Items.FindByValue(objWorker.AffiliationId.ToString()).Selected = true;
            }
            else { DdlSlacAffiliation.SelectedIndex = 0; }
            LblEmailVal.Text = objWorker.EmailAddr;
            TxtPreferred.Text = objWorker.PreferredEmail;
           
             //if (DdlStatus.Items.FindByValue(objWorker.StatusId.ToString()) != null)
             //{
             //    ViewState["statid"] = objWorker.StatusId;
             //    DdlStatus.ClearSelection();
             //    DdlStatus.Items.FindByValue(objWorker.StatusId.ToString()).Selected = true;
             //}
             //else { DdlStatus.SelectedIndex = 0; }
            LblStatusEditVal.Text = objWorker.Status;
            LblSupervisorVal.Text = objWorker.Supervisor;
            if ((!string.IsNullOrEmpty(objWorker.AlternateSvr)) && (objWorker.AlternateSvrTo >= DateTime.Today))
            {
                divAltsvrEditrow.Visible = true;
                DivAltSvrEdit.Visible = true;
                lblAltsvreditval.Text = objCommon.GetEmpname(objWorker.AlternateSvr);
                LblFromedit.Text = objWorker.AlternateSvrFrom.ToShortDateString();
                LblToedit.Text = objWorker.AlternateSvrTo.ToShortDateString();
                BtnDesignate.Visible = false;
            }
            else { BtnDesignate.Visible = true; }
        }

        private void FillDropdownlist()
      {
          DdlSlacAffiliation.DataSourceID = "SDSAffiliation";
          DdlSlacAffiliation.DataTextField = "LOOKUP_DESC";
          DdlSlacAffiliation.DataValueField = "LOOKUP_ID";
          DdlSlacAffiliation.DataBind();
      }
      
        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            
           if (Page.IsValid)
           {
               if(ValidateOnServer())
               { 
                      // string _userId = objCommon.GetEmpid(TxtWorker.Text).ToString();
                       bool _exists = objDml.CheckIfWorkerExists(SlacId);
                       if (_exists)
                       {
                           LblWorkerMsg.Text = "The Worker already exists in the database. Can't add again!";
                       }
                       else
                       {
                           //Incase we want to override the check for trainings 
                           bool _override = objRule.OverrideTraining();
                           if (!_override)
                           {
                               bool _check = objRule.AreTrainingReqsMetForWrkr(SlacId, Convert.ToInt32(DdlSlacAffiliation.SelectedValue));
                               if (_check)
                               {
                                   FillNSaveWorkerObject("add");
                               }
                               else
                               {
                                   LblWorkerMsg.Text = "Request Denied - This person does not meet the Worker training requirements.  They  must complete or renew courses as shown in the Training Summary";
                               }
                           }
                           else { FillNSaveWorkerObject("add"); }
                          
                        }
                       if (LblWorkerMsg.Text != "")
                       { Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialogFac('dialog-updmsg','Admin.aspx?mode=2');", true); }
                }  // End of ValidateOnServer              
            } // End of Page.isvalid
        }

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Admin.aspx?mode=2");
        }

        protected void DdlSlacAffiliation_DataBound(object sender, EventArgs e)
        {
            DdlSlacAffiliation.Items.Insert(0, new ListItem("--Choose One--", "-1"));
        }

        protected void FillNSaveWorkerObject(string mode)
        {
            Business.Worker objLSTWorker = new Business.Worker();
           
            objLSTWorker.WorkerName = TxtWorker.Text.Trim();
            objLSTWorker.SlacId = objDml.GetSlacId(WorkerId);
            objLSTWorker.PreferredEmail = TxtPreferred.Text.Trim();
            objLSTWorker.AffiliationId = (DdlSlacAffiliation.SelectedIndex != 0) ? Convert.ToInt32(DdlSlacAffiliation.SelectedValue) : 0;
            objLSTWorker.IsESHManualReviewed = "N";
            objLSTWorker.IsStudReqReviewed = "N";
           
           
            if (mode =="edit")
            {
                objLSTWorker.WorkerId = WorkerId;
                objLSTWorker.ModifiedBy = objCommon.GetUserId();
                objLSTWorker.AlternateSvr = "";
               // objLSTWorker.StatusId = Convert.ToInt32(DdlStatus.SelectedValue);
                int _result = objDml.UpdateWorker(objLSTWorker);
                string _msgStat = "";
                //TODO - if status is changed, need to get the reason from the person who changed it
                //if (IsStatusChanged())
                //{
                //    int _resultStat = objDml.UpdateWorkerStatus(objLSTWorker.SlacId, objLSTWorker.StatusId);
                //    if (_resultStat == 0) { _msgStat = "Status updated to " + DdlStatus.SelectedItem.Text; }
                //    else { _msgStat = "Status could not be updated to" + DdlStatus.SelectedItem.Text;  }
                //}
                if (_result == 0) { LblWorkerMsg.Text = "Worker - " + objLSTWorker.WorkerName + " successfully updated. " + _msgStat; }
                else { LblWorkerMsg.Text = "Error! " + objLSTWorker.WorkerName + " could not be updated. " + _msgStat; }
            }
            else
            {
                objLSTWorker.CreatedById = objCommon.GetUserId();
                int _workerId = objDml.CreateWorker(objLSTWorker);
                if (_workerId != 0) { LblWorkerMsg.Text = "Worker - " + objLSTWorker.WorkerName + " successfully added"; }
                else { LblWorkerMsg.Text = "Error! " + objLSTWorker.WorkerName + " could not be added"; }
            }
           
            Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialogFac('dialog-updmsg','Admin.aspx?mode=2');", true);
        }

        protected void TxtWorker_TextChanged(object sender, EventArgs e)
        {
            if (TxtWorker.Text != "")
            {
                if (objValid.CheckUserValidity(TxtWorker.Text))
                {
                    if (Mode.Equals(""))
                    {
                        divWrkerDetails.Visible = true;
                        UCTraining1.Visible = true;
                    }                  
                    objEmp.GetEmpDetails(SlacId);
                    LblEmailVal.Text = objEmp.Email;
                    LblBadgeIdVal.Text = objEmp.BadgeId;
                    LblSupervisorVal.Text = objCommon.GetEmpname(objEmp.Supervisor);
                }
                else CvWorker.IsValid = false;
            }
            else
            {
                RFVWorker.IsValid = false;
                divWrkerDetails.Visible = false;
                UCTraining1.Visible = false;
            }
                
           
           
        }

        protected void CvWorker_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = objValid.CheckUserValidity(TxtWorker.Text);

        }

        private bool ValidateOnServer()
        {
            bool _result = true;
            if (objValid.CheckIfSelected(DdlSlacAffiliation.SelectedIndex.ToString()) && objValid.CheckIfSelected(DdlSlacAffiliation.SelectedValue.ToString()))
            {
                CvAffiliate.IsValid = true;
                _result = true;
            }

            else
            {
                CvAffiliate.IsValid = false;
                _result = false;
            }
            return _result;
        }

        protected void BtnBack_Click(object sender, EventArgs e)
        {
            if (ViewState["urlref"]!=null)
            {
                if (ViewState["urlref"].ToString().Contains("Reports"))
                {
                    Response.Redirect("Reports.aspx?dd=" + ViewState["dd"].ToString());
                }
                else if (ViewState["urlref"].ToString().Contains("Facility"))
                {
                    Response.Redirect(ViewState["urlref"].ToString());
                }
                else { Response.Redirect("Admin.aspx?mode=2"); }
            }
            else
            {
                Response.Redirect("Admin.aspx?mode=2");
            }
            
        }

        protected void BtnCancelEdit_Click(object sender, EventArgs e)
        {
            Response.Redirect("Admin.aspx?mode=2");
        }

        protected void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                FillNSaveWorkerObject("edit");
            }
        }

        protected void DdlSlacAffiliation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DdlSlacAffiliation.SelectedValue == "9")
            {
                UCTraining1.EmpType = "emp";
            }
            else
            {
                UCTraining1.EmpType = "noemp";
            }
        }

        //private bool IsStatusChanged()
        //{
        //     string _initialStat = (ViewState["statid"] != null) ? ViewState["statid"].ToString() : "";
        //     return (_initialStat != DdlStatus.SelectedValue) ? true : false;
        //  }

        //protected void DdlStatus_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (IsStatusChanged())
        //    {
        //        //give warning that user has changed the status.
        //        LblWorkerMsg.Text = "Warning! You have changed the status of the worker to " + DdlStatus.SelectedItem.Text + ". Please revert your selection if it is not intended ";
        //        Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-updmsg');", true);          
        //    }              
        //}
       
    }
}