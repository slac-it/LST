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
    public partial class FacilityApprovalRequest : System.Web.UI.Page
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

        public int FacilityId { 
            get
            {
                return (ViewState["facid"] != null) ? Convert.ToInt32(ViewState["facid"]) : 0;
            }
            set
            {
                ViewState["facid"] = value;
            }
        }

        public int RequestId
        {
            get
            {
                return (ViewState["requestid"] != null) ? Convert.ToInt32(ViewState["requestid"]) : 0;
            }
            set
            {
                ViewState["requestid"] = value;
            }
        }
      
        public string Mode
         {
             get; set;         
         }

     

        #endregion 

        #region "Page / General"

        protected void Page_Load(object sender, EventArgs e)
        {
           
            if (!Page.IsPostBack)
            {
                int _requestid;
                string _srequestid = Request.QueryString["objid"];
                string _mode = Request.QueryString["mode"];
                UrlReferrer = (Request.UrlReferrer != null) ? Request.UrlReferrer.ToString() : "";
                if (!Int32.TryParse(_srequestid, out _requestid))
                {
                    Response.Redirect("Error.aspx?msg=parserr");
                }
                else
                {
                    Mode = (_mode != null) ? _mode : "";
                     CheckAccessibility(_requestid);
                }              
                
            }
            else
            {
                SetAllUserControls();
            }
         
        }

          
        private void CheckAccessibility(int reqId)
        {
         
                int _facId;
                if ((Mode != null) && (Mode.Equals("req")))
                {
                    _facId = reqId;
                    RequestId = 0;
                }
                else
                {
                    _facId = objDml.GetFacilityIdAR(reqId);
                    RequestId = reqId;
                }

                if (_facId <= 0) { Response.Redirect("Error.aspx?msg=notexists"); }

             
               
            
                FacilityId = _facId;



                objRoles.GetUserRole(objCommon.GetUserId().ToString(), FacilityId, 0);


                //if (objRoles.IsSLSO || objRoles.IsLSOrAlt() || objRoles.IsProgramMgr || objRoles.IsESHCoordinator || objRoles.IsAdmin)
                //{
                    SetPage(objRoles, FacilityId, reqId);
               // }
            //    else
            //    {
            //       //If comes from report page, everyone should be able to view
            //        Mode = "";
            //        PnlRequestDet.Visible = true;
            //        PnlApprovalHistory.Visible = true;
            //    //for others
            //    //Response.Redirect("Error.aspx?msg=notauthorized");
            //}
          
           

               
           
           
           
        }

        protected void SetPage(Business.UserRoles objRoles, int facId, int reqId )
        {
            //for admin and or for other roles, if mode is nothing, it should be view
            PnlSLSO.Visible = false;
            PnlLSO.Visible = false;
            PnlApprovers.Visible = false;
            DivSLSO.Visible = false;
            DivApprovers.Visible = false;
            Divbuttons.Visible = false;

            bool _isInProg = objDml.CheckIfFacApprovalReqInProg(reqId);

            if ((objRoles.IsSLSO) && (!objDml.IsFACApproved("SLSO", RequestId)) )
            {
                PnlSLSO.Visible = true;
                PnlLSO.Visible = false;
                PnlApprovers.Visible = false;
                Divbuttons.Visible = true;
                DivSLSO.Visible = true;
                DivApprovers.Visible = false;
                PnlRequestDet.Visible = false;
                PnlApprovalHistory.Visible = false;
               
                Mode = "req";
                
            }
            else if ((objRoles.IsLSOrAlt()) && (!objDml.IsFACApproved("LSO", RequestId)) && (_isInProg) )
            {
                PnlSLSO.Visible = false;
                PnlLSO.Visible = true;
                PnlApprovers.Visible = true;
                DivSLSO.Visible = false;
                Divbuttons.Visible = true;
                DivApprovers.Visible = true;
                PnlRequestDet.Visible = true;
                PnlApprovalHistory.Visible = true;
               
                Mode = "approve";
            }
            else if (((objRoles.IsProgramMgr) && (!objDml.IsFACApproved("PRGMGR", RequestId)) && (_isInProg)) ||
                       ((objRoles.IsESHCoordinator) && (!objDml.IsFACApproved("ESHCOORD", RequestId)) && (_isInProg)))
            {
                PnlSLSO.Visible = false;
                PnlLSO.Visible = false;
                PnlApprovers.Visible = true;
                DivSLSO.Visible = false;
                Divbuttons.Visible = true;
                DivApprovers.Visible = true;
                PnlRequestDet.Visible = true;
                PnlApprovalHistory.Visible = true;
                           
                Mode = "approve";
            }
  
            else
            {
                Mode = "";
                PnlRequestDet.Visible = true;
                PnlApprovalHistory.Visible = true;
                PnlSLSO.Visible = false;
                PnlLSO.Visible = false;
                PnlApprovers.Visible = false;
                DivSLSO.Visible = false;
                Divbuttons.Visible = false;
                DivApprovers.Visible = false;
                //LnkFac.Attributes.Add("class", "nounload");

            }
            HdnMode.Value = Mode;

            if (Mode == "req")
            {
                DivBack.Visible = false;
            }
            else if ((Mode == "") || ((Mode == "approve") && (UrlReferrer.Contains("Reports.aspx"))))
            {
                DivBack.Visible = true;
            }
            else
            {
                DivBack.Visible = false;
            }

            if (PnlRequestDet.Visible)
            {
                PopulateFacilityInfo();
            }
           
                SetAllUserControls();
           
           
        } 
       
        protected void PopulateFacilityInfo()
        {
            Business.Facility objFacility = new Business.Facility();

            if (FacilityId != -1)
            {
                objFacility = objDml.GetFacilityDetails(FacilityId);
                //LblBldgVal.Text = (objFacility.Bldg != "") ? objFacility.Bldg : " - ";
                //LblRoomVal.Text = (objFacility.Room != "") ? objFacility.Room.ToString() : " - ";
                //LblLocationVal.Text = (objFacility.OtherLocation != "") ? objFacility.OtherLocation.ToString() : " - ";
                //LblSLSOVal.Text = (objFacility.SLSOName != "") ? objFacility.SLSOName : " - ";
                //aFacSite.HRef = objFacility.FacWebpage;
                LblFacilityVal.Text = objFacility.FacilityName;
                LblExpiryDateVal.Text = (objFacility.ApprovalExpDate == DateTime.MinValue)? "" : objFacility.ApprovalExpDate.ToShortDateString();
                LblRequestIdVal.Text = RequestId.ToString();
                LblStatusVal.Text = objDml.GetRequestStatus(RequestId);
                if ((PnlLSO.Visible) && (Mode.Equals("approve")))
                {
                    TxtNewExpDate.Text = DateTime.Today.AddYears(1).ToShortDateString();
                   
                }
                LnkFac.HRef = "~/Facility.aspx?mode=view&id=" + FacilityId + "&objid=" + RequestId;
                LnkFac.InnerText = "Click here to view " + objFacility.FacilityName + " in detail";
            }
          
            
        }

        private void SetAllUserControls()
        {
            if (PnlApprovalHistory.Visible)
            {
                    UCApprhis.RequestId = RequestId;
            }
        }


        #endregion

   

        #region "Server validate"

        protected void CVRevApp_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (PnlLSO.Visible) ? ((RbRevisionApproved.SelectedValue == "1") ? false : true) : true;
        }

        protected void CVInspection_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (PnlLSO.Visible) ? ((RblInspection.SelectedValue == "1") ? false : true) : true;
        }

        protected void CVCertification_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (PnlSLSO.Visible) ? ((RblCertification.SelectedValue == "1") ? false : true) : true;
        }

        protected void CVRevision_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (PnlSLSO.Visible) ? ((RblRevision.SelectedValue == "1") ? false : true) : true;
        }

        protected void CuVNewDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = (PnlLSO.Visible) ? ((TxtNewExpDate.Text == "") ? false : true) : true;

        }

        #endregion
     
        #region "Button/TB Events"





        protected void BtnDecline_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (TxtComments.Text.Trim() == "")
                {
                    CVcomments.IsValid = false;
                    SpnComments.Visible = true;
                }

                else
                {
                    CheckAccessibility(RequestId);
                    if (objRoles.IsLSOrAlt())
                    {
                        FillNSaveApproval(4, "LSO");
                    }
                    if (objRoles.IsProgramMgr)
                    {
                        FillNSaveApproval(4, "PRGMGR");
                    }
                    if (objRoles.IsESHCoordinator)
                    {
                        FillNSaveApproval(4, "ESHCOORD");
                    }


                }
            }
            else
            {
                return;
            }
        }

        protected void BtnApprove_Click(object sender, EventArgs e)
        {
           
            Page.Validate();
            if (Page.IsValid)
            {
                objRoles.GetUserRole(objCommon.GetUserId().ToString(), FacilityId, 0); ;

                if (objRoles.IsLSOrAlt())
                {
                    if (ValidateDate())
                    {
                        FillNSaveApproval(3, "LSO");
                    }
                    else
                    {
                        return;
                    }
                }
                if (objRoles.IsProgramMgr)
                {
                    FillNSaveApproval(3, "PRGMGR");
                }
                if (objRoles.IsESHCoordinator)
                {
                    FillNSaveApproval(3, "ESHCOORD");
                }
               
            }
            else
            {
                return;
            } 
        }

        protected bool ValidateDate()
        {
             string _msgerr = "";
              if ((TxtNewExpDate.Text != "") && (LblExpiryDateVal.Text !=""))
                    {
                        DateTime _dtNewDate = Convert.ToDateTime(TxtNewExpDate.Text);
                        DateTime _dtoldDate = Convert.ToDateTime(LblExpiryDateVal.Text);
                        DateTime _dtToday = DateTime.Today;

                        if ((_dtNewDate < _dtToday.AddDays(1)) || (_dtNewDate > _dtToday.AddMonths(18)))
                        {
                            _msgerr = "New Facility Operation date should be greater than today's date and less than 18 months from today";
                        }
                }
                 if (_msgerr != "")
                    {
                        LblFACMsg.Text = _msgerr;  
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-appreqmsg');", true); 
                        return false;
                    }
                    else return true;
        }

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

      

        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            
          //  Page.Validate(); //custom validator - server validate not firing
           
            if (Page.IsValid )
            {
                int _requestId = 0;
               _requestId = FillNSaveFACApprovalRequest();            
            }
            else
            {
                return;
            }

        }

        protected void BtnClear_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

       protected int FillNSaveFACApprovalRequest()
        {
            Business.FacilityRequest objFacRequest = new Business.FacilityRequest();

            objFacRequest.FacilityId = Convert.ToInt32(FacilityId);
            objFacRequest.IsSOPCompleted = (PnlSLSO.Visible) ? ((RblRevision.SelectedValue == "0") ? "Y" : "N") : "N";
            objFacRequest.IsAnnualCertCompleted = (PnlSLSO.Visible) ? ((RblCertification.SelectedValue == "0") ? "Y" : "N") : "N";
            objFacRequest.CreatedBy = objCommon.GetUserId();
            string _comments = objCommon.ReplaceSC(TxtSLSOComments.Text.Trim());
         

            int _requestId = objDml.CreateFacilityRequest(objFacRequest,_comments);

            if (_requestId != 0)
            {
                LblFACMsg.Text = "Request for Facility Approval successfully submitted";
            }

            else
            {
                LblFACMsg.Text = "Error! Request for Facility Approval could not be submitted";
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialogFac('dialog-appreqmsg','Default.aspx');", true);
            return _requestId;
        }

        protected void FillNSaveApproval(int statusId, string approverType)
       {
           Business.FacilityApproval objFacApproval = new Business.FacilityApproval();

           objFacApproval.RequestId = RequestId;
           objFacApproval.ApproverId = Convert.ToInt32(objCommon.GetUserId());
           objFacApproval.StatusId = statusId;
           objFacApproval.Comments = objCommon.ReplaceSC(TxtComments.Text);
           objFacApproval.ApproverType = approverType;
           
           string _sopApproved = (PnlLSO.Visible) ? ((RbRevisionApproved.SelectedValue == "0") ? "Y" : "N") : "N";
           string _inspCompleted = (PnlLSO.Visible) ? ((RblInspection.SelectedValue == "0") ? "Y" : "N") : "N";
           DateTime _newFacDate = (PnlLSO.Visible) ? ((TxtNewExpDate.Text != "") ? Convert.ToDateTime(TxtNewExpDate.Text) : DateTime.MinValue) : DateTime.MinValue;

           int _result = objDml.UpdateFacApproval(objFacApproval, _sopApproved, _inspCompleted, _newFacDate);
            if (_result == 0)
            {
                LblFACMsg.Text = "Your action submitted successfully";
            }
            else
            {
                LblFACMsg.Text = "Error submitting your action";
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialogFac('dialog-appreqmsg','Default.aspx');", true);

       }



        #endregion

        protected void CVcomments_ServerValidate(object source, ServerValidateEventArgs args)
        {

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
                Response.Redirect("Reports.aspx?dd=8");
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }
    }
}

