//$Header:$
//
//  NameFinderDD.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This page is used to pick a person with more than 1 shows up in a dropdownlist
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using System.Text.RegularExpressions;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace LST
{
    public partial class NameFinderDD : BasePage
    {
        Data.Data_Util objData = new Data.Data_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        Data.DML_Util objDml = new Data.DML_Util();

        protected void Page_Load(object sender, EventArgs e)
        {
            string _dialogName = "";
            string _contrl = "";
            string _contrl2 = "";

            ddlEmplist.Attributes.Add("onkeydown", "return onKeypress1();");
            //TODO: Validate the input to avoid SQL Injection    
            if (!Page.IsPostBack)
            {
                _dialogName = Request.QueryString["dialog"];
                _contrl = Request.QueryString["field"];
                _contrl2 = Request.QueryString["field2"];

                HdnDialog.Value = _dialogName;
                HdnControl.Value = Request.QueryString["field"];
                HdnControl2.Value = Request.QueryString["field2"];
            }
           
           

        }

      

      

        protected void CmdContinue_Click(object sender, EventArgs e)
        {
           
           
            BindDD();
        }

        protected void BindDD()
        {

            string alertmessage = "";
            string strtxtOwner = "";
            //string strSingleQuote = "";
 
           // int intPosition = 0;


            if (string.IsNullOrEmpty(TxtOwner.Text))
            {
                LblMsg2.Text = "Please enter the first few characters to start your search";                
            }
            else
            {
                
                strtxtOwner = TxtOwner.Text.Trim();
                string _name ="";
                string _id="";

                using (OracleDataReader _drEmployee = objCommon.GetMatchingEmployeesDD(strtxtOwner))
                {
                    trMsg2.Visible = true;
                    if (_drEmployee.HasRows)
                    {
                        DataTable _dtPeople = new DataTable();
                        _dtPeople.Load(_drEmployee);
                        
                        if (_dtPeople.Rows.Count > 1)
                        { 
                            trGrid.Visible = true;
                            trButtons.Visible = true;

                            ddlEmplist.DataSource = _dtPeople;
                            ddlEmplist.DataTextField = "EMPNAMEID";
                            ddlEmplist.DataValueField = "EMPLOYEE_ID";
                            ddlEmplist.DataBind();
                            SetFocus(ddlEmplist.ClientID);
                         }
                        else
                        {
                            if (_dtPeople.Rows.Count > 0)
                            {
                                _name = _dtPeople.Rows[0].Field<string>(0);
                                _id = _dtPeople.Rows[0].Field<string>(1);
                            }

                            HdnItemval.Value = _id;
                            ScriptManager.RegisterStartupScript(this, GetType(), "key", "JQueryClose('" + Server.HtmlEncode(_name) + "', '" + Server.HtmlEncode(_id) + "');", true);
                        }
                    }
                    else
                    {
                        LblMsg2.Text = "No such employee found in the directory. Please check the <br> spelling and re-enter.";
                    }

                }
              
            }


        }

        protected void CmdSelect_Click(object sender, EventArgs e)
        {
            string SelectedValue = "";
            string SelectedId = "";
            string selectvalraw = "";
            string[] splittedname ;


            selectvalraw = ddlEmplist.SelectedItem.Text.Replace("'", "\\'");
            splittedname = selectvalraw.Split('-');
            SelectedValue = splittedname[0].Trim();
            SelectedId = ddlEmplist.SelectedValue.ToString();
            if (SelectedValue != "")
            {
                HdnItemval.Value = ddlEmplist.SelectedValue.ToString();
                ScriptManager.RegisterStartupScript(this, GetType(), "key", "JQueryClose('" + Server.HtmlEncode(SelectedValue) + "','" + Server.HtmlEncode(SelectedId) + "');", true);
            }         
        }

    

        protected void CmdBack_Click(object sender, EventArgs e)
        {       
            ScriptManager.RegisterStartupScript(this, GetType(), "key", "JQueryClose('na');", true);

        }

        protected void CmdCancel_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "key", "JQueryClose('na');", true);
        }

       

 
    }
}