//$Header:$
//
//  Designate.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This page is used for designating alternates - SLSO/SVR/LSO
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LST
{
    public partial class Designate : System.Web.UI.Page
    {
        Business.Validation objValid = new Business.Validation();
        Business.Common_Util objCommon = new Business.Common_Util();
        Data.DML_Util objDml = new Data.DML_Util();
        Business.Rules objRule = new Business.Rules();
        Business.UserRoles objRoles = new Business.UserRoles();

        protected void Page_Load(object sender, EventArgs e)
        {
            BtnCancel.Attributes.Add("onClick", "JQueryParentClose('dialogDesignate');return false;");
            ImgBtAlt.Attributes.Add("onClick", "OpenDialogForName('dialogowneradmin','TxtAlternate','HdnAltId'); return false;");
           

            if (!Page.IsPostBack)
            {
                string _type = Request.QueryString["type"];
                HdnType.Value = _type;
                int _id = Request.QueryString["id"] != null ? Convert.ToInt32(Request.QueryString["id"]):0;
                ViewState["id"] = _id;
                bool _allowedaccess = false;

                if (_id != 0)
                {
                    _allowedaccess = objRoles.CheckAccessibility(_id);
                }
                else
                {
                    _allowedaccess = objRoles.CheckAccessibility();
                }


                if (!_allowedaccess)
                {
                     Response.Redirect("Error.aspx?msg=notauthorized");
                   
                }
                else
                {
                    if ((_id == 0) && (_type =="slso"))
                    {
                        divLab.Visible = true;
                        FillDropdownLists();
                     }
                    else
                    {
                        divLab.Visible = false;
                    }
                }

               
            }
        }


        protected void FillDropdownLists()
        {
            DdlFacility.DataSourceID = "SDSFacility";
            DdlFacility.DataTextField = "FACILITY_NAME";
            DdlFacility.DataValueField = "FACILITY_ID";
        }


          
        protected void CvAlt_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if ((HdnAltId.Value == "") && (TxtAlternate.Text != ""))
            {
                CvAlt.ErrorMessage = "Please use the binocular icon to select the correct name with Id";
                args.IsValid = false;
            }
            else
            {
                args.IsValid = objCommon.IsEmpIDValid(HdnAltId.Value);
            }

        }

        protected void DdlFacility_DataBound(object sender, EventArgs e)
        {
            DdlFacility.Items.Insert(0, new ListItem("--Choose One--", "-1"));
        }

   
        protected void BtnDesignate_Click(object sender, EventArgs e)
        {
         
            if (Page.IsValid)
            {
                FillNSaveAlternateInfo();
            }
        }

    

        protected void FillNSaveAlternateInfo()
        {
            int _result = -2;
            string _type = "";
            _type = HdnType.Value.ToUpper();
            LblAltMsg.Text = "";
            if (HdnType.Value == "slso")
            {
                //fill objFacility
                Business.Facility objFacility = new Business.Facility();
                objFacility.AltSLSO = (HdnAltId.Value != "") ? Convert.ToInt32(HdnAltId.Value.Trim()) : 0;//objCommon.GetEmpid(TxtAlternate.Text);
                objFacility.AltSLSOFrom =(TxtFromDate.Text != "")? Convert.ToDateTime(TxtFromDate.Text):DateTime.MinValue;
                objFacility.AltSLSOTo = (TxtToDate.Text != "") ? Convert.ToDateTime(TxtToDate.Text):DateTime.MinValue;
                if (divLab.Visible) { objFacility.FacilityId = (DdlFacility.SelectedIndex != 0) ? Convert.ToInt32(DdlFacility.SelectedValue) : 0; }
                else { objFacility.FacilityId = (ViewState["id"] != null) ? Convert.ToInt32(ViewState["id"]) : 0; }
                objFacility.ModifiedBy = HttpContext.Current.Session["LoginSID"].ToString();
                if (objFacility.FacilityId != 0 && objFacility.AltSLSO != 0)
                {
                    // Check if the Facility has a valid alternate SLSo (Date is valid)
                    bool _check = objRule.CheckIfActiveAlternate(objFacility.FacilityId,objFacility.AltSLSO);
                    if (!_check)
                    {
                        //Check if the alternate is already an SLSO for that facility
                        bool _isSLSO = objDml.CheckIfSLSO(objFacility.AltSLSO.ToString(), objFacility.FacilityId);
                        if (_isSLSO)
                        {
                            _result = -2;
                            LblAltMsg.Text = " Person to be designated as Alternate SLSO is already SLSO for this facility. Please choose another person.";
                        }
                         else
                        {
                            //Check if alternate SLSO has current ESH130
                            bool _checkTraining = objRule.AreTrainingReqsMetForAltSLSO(objFacility.AltSLSO.ToString());
                            //Check if alternate SLSO is also active QLO for that facility
                            bool _checkQLO = objRule.CheckIfActiveQLO(objFacility.FacilityId, objFacility.AltSLSO);

                            if ((_checkQLO) && (_checkTraining))
                            {
                                _result = objDml.CreateAlternate(objFacility.FacilityId, _type, objFacility.AltSLSO, objFacility.AltSLSOFrom, objFacility.AltSLSOTo, objFacility.ModifiedBy);
                            }
                            else if ((_checkTraining) && (!_checkQLO))
                            {
                                _result = -2;
                                LblAltMsg.Text += " Person to be designated as alternate SLSO should be an active QLO for that facility";
                            }
                            else if ((!_checkTraining) && (_checkQLO))
                            {
                                _result = -2;
                                LblAltMsg.Text += " Person to be designated as alternate SLSO should have current ESH130 training";
                            }
                            else
                            {
                                _result = -2;
                                LblAltMsg.Text += " Person to be designated as alternate SLSO should be an active QLO for that facility and have current ESH130 training";
                            }
                        }

                    }
                    else
                    {
                        _result = -1;
                        LblAltMsg.Text = "Alternate SLSO already exists and active for this facility. Can't add!";
                    }
                   
                }

            }
            else if (HdnType.Value == "svr")
            {
                //fill objWorker
                Business.Worker objWorker = new Business.Worker();
                objWorker.AlternateSvr = (HdnAltId.Value != "") ? HdnAltId.Value.Trim().ToString() : "";//objCommon.GetEmpid(TxtAlternate.Text).ToString();
                objWorker.AlternateSvrFrom = (TxtFromDate.Text != "") ? Convert.ToDateTime(TxtFromDate.Text) : DateTime.MinValue;
                objWorker.AlternateSvrTo = (TxtToDate.Text != "") ? Convert.ToDateTime(TxtToDate.Text) : DateTime.MinValue;
                objWorker.WorkerId = (ViewState["id"] != null)? Convert.ToInt32(ViewState["id"]) : 0;
                objWorker.ModifiedBy = HttpContext.Current.Session["LoginSID"].ToString();
               
                if (objWorker.WorkerId != 0 && objWorker.AlternateSvr != "0")
                {
                    _result = objDml.CreateAlternate(objWorker.WorkerId, _type, Convert.ToInt32(objWorker.AlternateSvr), objWorker.AlternateSvrFrom, objWorker.AlternateSvrTo, objWorker.ModifiedBy);
                }

            }
            else if (HdnType.Value == "lso")
            {
                Business.User objUser = new Business.User();
                objUser.SlacId = (HdnAltId.Value != "") ? Convert.ToInt32(HdnAltId.Value.Trim()) : 0;//objCommon.GetEmpid(TxtAlternate.Text);
                objUser.AlternateLSOFrom = (TxtFromDate.Text != "") ? Convert.ToDateTime(TxtFromDate.Text) : DateTime.MinValue;
                objUser.AlternateLSOTo = (TxtToDate.Text != "") ? Convert.ToDateTime(TxtToDate.Text) : DateTime.MinValue;
                objUser.CreatedBy = HttpContext.Current.Session["LoginSID"].ToString();
                //----Check if Active QLO or Deputy LSO
                //----Check if training - esh130
                //----check if the person added as alternate already exists            
                //----Check if the from date doesn't fall within another alternates to and from - only one can be active at a time


                if ((objUser.SlacId != 0) && (objUser.AlternateLSOFrom != DateTime.MinValue) && (objUser.AlternateLSOTo != DateTime.MinValue))
                {
                    bool _check = objRule.CheckIfActiveAlternate(0, objUser.SlacId); //can be inactive too as dates can be edited now
                    if (!_check)
                    {
                        bool _checkTraining = objRule.AreTrainingReqsMetForAltSLSO(objUser.SlacId.ToString());

                        bool _checkQLO = objRule.CheckIfActiveQLO(0, objUser.SlacId);

                        objRoles.GetUserRole(objUser.SlacId.ToString(),0,0);
                        bool _checkDLSO = objRoles.IsDLSO;

                        bool _checkDatesOutside = objDml.CheckifDatesROutsideCur(objUser.AlternateLSOFrom, objUser.AlternateLSOTo, 0);

                        if (_checkDatesOutside)
                        {
                            if (objRoles.IsLSO)
                            {
                                _result = -2;
                                LblAltMsg.Text += " Person to be designated is already LSO and cannot be added as Alternate ";
                            }
                            else if ((_checkTraining) && ((_checkQLO) || (_checkDLSO)))
                            {
                                _result = objDml.CreateAlternate(0, _type, objUser.SlacId, objUser.AlternateLSOFrom, objUser.AlternateLSOTo, objUser.CreatedBy);
                            }
                            else if ((_checkTraining) && (!((_checkQLO) || (_checkDLSO))))
                            {
                                _result = -2;
                                LblAltMsg.Text += " Person to be designated as alternate LSO should either be an Active QLO or DLSO";
                            }
                            else if ((!_checkTraining) && ((_checkQLO) || (_checkDLSO)))
                            {
                                _result = -2;
                                LblAltMsg.Text += "Person to be designated as Alternate LSO should have current ESH 130 training";
                            }
                            else
                            {
                                _result = -2;
                                LblAltMsg.Text += " Person to be designated as alternate LSO should be either an active QLO or DLSO and have current ESH130 training";
                            }
                        }
                        else
                        {
                            _result = -2;
                            LblAltMsg.Text += " There is already an Alternate LSO assigned on those dates. Please choose a different From date as only one alternate can be active at a time";
                        }
                       
                    }
                    else
                    {
                        _result = -1;
                        LblAltMsg.Text += " This person is already designated as Alternate LSO. Please choose Edit to change the From and To dates of this person in the Manage Alternate LSO list";
                    }

                   
                }
            }
           
            if (_result == 0) { LblAltMsg.Text = "Alternate " + _type + " successfully added";  }
            else { if ((_result != -1) && (_result != -2)) { LblAltMsg.Text = "Error! Alternate" + _type + " could not be added"; } }

        
            if (_result == -2)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-msg');", true);
               
            }
            else if (_result == 0)
            {
                if ((LblAltMsg.Text != "") && ((HdnType.Value == "lso") || (HdnType.Value == "slso")))
                {
                    if (HdnType.Value  == "slso")
                    {
                       Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OnsuccessRefreshParent('dialog-msg','dialogDesignate',5,'slso');", true);
                      //  Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "Test();", true);

                    }
                    else if (HdnType.Value == "lso")
                    {
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OnsuccessRefreshParent('dialog-msg','dialogDesignate',5,'lso');", true);
                    }
                   
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OnsuccessRefreshParent('dialog-msg','dialogDesignate','','');", true);
                }
            }
            else
            {
                //just close and do not refresh
                Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "Onsuccess('dialog-msg','dialogDesignate');", true);
            }
          
            
        }

        protected void CustValDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = CheckDate();
        }

        private bool CheckDate()
        {
            DateTime _fromdt;
            DateTime _todt;
            DateTime _dtNow;
            string fromval;
            string toval;

            if ((TxtFromDate.Text != "") && (TxtToDate.Text != ""))
            {
                fromval = DateTime.Parse(TxtFromDate.Text).ToShortDateString();
                toval = DateTime.Parse(TxtToDate.Text).ToShortDateString();

                _fromdt = DateTime.Parse(fromval);
                _todt = DateTime.Parse(toval);
                string _nowVal = DateTime.Now.ToShortDateString();
                _dtNow = DateTime.Parse(_nowVal);

                if (_todt < _fromdt)
                {
                    CustValDate.ErrorMessage = "From Date cannot be greater than To Date";
                    return false;
                }
                else if (_todt < _dtNow)
                {
                    CustValDate.ErrorMessage = " To Date should be greater than or equal to Today's date";
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
    }
}