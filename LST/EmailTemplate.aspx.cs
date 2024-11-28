using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LST
{
    public partial class EmailTemplate : System.Web.UI.Page
    {
        Business.Common_Util objCommon = new Business.Common_Util();
        Data.DML_Util objDml = new Data.DML_Util();
            
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void DdlFacility_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DdlFacility.SelectedValue != "-1")
            {
                DdlEmailList.Visible = true;
                if (DdlFacility.SelectedValue == "0")
                {
                    SDSList.SelectCommand = "SELECT LOOKUP_ID , LOOKUP_DESC  FROM LST_LOOKUP WHERE IS_ACTIVE='Y' AND LOOKUP_GROUP='ToEmailList' and PARENT_ID IS not NULL AND Parent_ID = 21 ORDER BY LOOKUP_ID";
                }
                else
                {
                    SDSList.SelectCommand = "SELECT LOOKUP_ID , LOOKUP_DESC  FROM LST_LOOKUP WHERE IS_ACTIVE='Y' AND LOOKUP_GROUP='ToEmailList' and PARENT_ID IS not NULL AND Parent_ID = 22 ORDER BY LOOKUP_ID";
                }
                SDSList.DataBind();
            }
            else
                DdlEmailList.Visible = false;
            
        }

        protected void BtnCancel_Click(object sender, EventArgs e)
        {
            ClearFields();       
        }

        private void ClearFields()
        {
            DdlFacility.SelectedIndex = 0;
            DdlFacility_SelectedIndexChanged(null, null);
            TxtSubject.Text = "";
            TxtBody.Text = "";
        }

        protected void BtnSendEmail_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //fill object and call package to send email
                FillEmailObjNSend();
                ClearFields();
            }
        }

        private void FillEmailObjNSend()
        {
            Business.CustomEmail objEmail = new Business.CustomEmail();
            objEmail.FacId = Convert.ToInt32(DdlFacility.SelectedValue);
            objEmail.ListId = Convert.ToInt32(DdlEmailList.SelectedValue);
            objEmail.Subject = objCommon.ReplaceSC(TxtSubject.Text);
            objEmail.BodyMsg =  objCommon.ReplaceWordChars(objCommon.ReplaceSC(TxtBody.Text));
            objEmail.CreatedBy = HttpContext.Current.Session["LoginSID"].ToString();

            int _result = objDml.CustomEmail(objEmail);
            if (_result == 0)
            {
                LblMsg.Text = "Email sent successfully";

            }
            else if (_result == -2)
            {
                LblMsg.Text = "There are no records for the selected list. So email could not be sent.";
            }
            else
            {
                LblMsg.Text = "Error! Email could not be sent";
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-msg');", true);
        }

     
    }
}