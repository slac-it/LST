//$Header:$
//
//  USers.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the  page for SLSO - adding a new
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LST
{
    public partial class NewSLSO : System.Web.UI.Page
    {
        Data.DML_Util objDml = new Data.DML_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        Business.Validation objValid = new Business.Validation();
        Business.Rules objRule = new Business.Rules();
        Business.UserRoles objRoles = new Business.UserRoles();

        protected void Page_Load(object sender, EventArgs e)
        {
            BtnCancel.Attributes.Add("onClick", "JQueryParentClose('dialogUser');return false;");
         
            ImgBtnUser.Attributes.Add("onClick", "OpenDialogForName('dialogowneradmin','TxtUser','HdnUserId'); return false;");
            if (!Page.IsPostBack)
            {
                if (!objRoles.CheckAccessibility())
                {
                    Response.Redirect("Error.aspx?msg=notauthorized");
                }
             }
        }

        public string EmpId
        {
            get
            {
                return (ViewState["empid"] != null) ? ViewState["empid"].ToString():"";
            }

            set
            {
                ViewState["empid"] = value;
            }
        }

        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
           if (Page.IsValid)
            {
  
                     bool _exists = objDml.CheckIfUserExists(EmpId, 15);
                       if (_exists)
                       {
                           LblUserMsg.Text = "SLSO already exists! Can't add again!";
                       }
                       else
                       {
                                 bool _override = objRule.OverrideTraining();
                                 if (!_override)
                                {
                                        bool _check = objRule.AreTrainingReqsMetForUser(EmpId);
                                        if (_check)
                                        {
                                            FillNSaveUserObject();
                                        }
                                        else
                                        {
                                            LblUserMsg.Text = "Request Denied -This person doesn't meet the training requirements for an SLSO";
                                        } //End of Check
                                 }
                                 else
                                 {
                                    FillNSaveUserObject();
                                 } //End of override
                         
                       } //End of user exists                      
                 
                   if (LblUserMsg.Text != "")
                   { Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OnsuccessRefreshParent('dialog-msg','dialogUser',3);", true); }
               } //End of Page valid
        }



        protected string IsInSTA(string courseId)
        {
            string _result = objDml.IsInSTA(EmpId, courseId);
            return _result;
        }

        protected string GetStatus(string courseId)
        {
            string _result = objDml.GetTrainingStatus(EmpId, courseId);
            return _result;
        }

        protected void TxtWorker_TextChanged(object sender, EventArgs e)
        {
            if (HdnUserId.Value != "")
            {
                EmpId = HdnUserId.Value;//objCommon.GetEmpid(TxtUser.Text).ToString();

                if (objCommon.IsEmpIDValid(EmpId))
                {
                  PnlTraining.Visible = true;
                }
                else CvUser.IsValid = false;
            }
            else
            {
                RFVUser.IsValid = false;
                PnlTraining.Visible = false;
            }              
           
        }

        protected void FillNSaveUserObject()
        {
            Business.User objUser = new Business.User();

            objUser.RoleTypeId = 15;
            objUser.UserName = TxtUser.Text.Trim();
            objUser.SlacId = (HdnUserId.Value != "")?Convert.ToInt32(HdnUserId.Value) :0;//objCommon.GetEmpid(objUser.UserName);
            objUser.CreatedBy = objCommon.GetUserId();
            int _userRoleId = objDml.CreateUser(objUser);
            if (_userRoleId != 0)
            {
                LblUserMsg.Text = "SLSO - " + objUser.UserName + " successfully added";
            }
            else
            {
                LblUserMsg.Text = "Error! " + objUser.UserName + " could not be added";
            }
           
        }

       

        protected void CvUser_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = objCommon.IsEmpIDValid(EmpId);
        }

    
     
    }
}