//$Header:$
//
// Error.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This page is used for displaying appropriate error messages
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LST
{
    public partial class Error : System.Web.UI.Page
    {
        public string _url;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["urlhost"] != null)
            {
                _url = Session["urlhost"].ToString() + "/LST/";
            }
            if (!Page.IsPostBack)
            {
                string _msg;
                _msg = Request.QueryString["msg"];
                if (_msg == "parserr")
                {
                    LblMSg.Text = "Not a Valid request to the page!";
                }
                else if (_msg == "notauthorized")
                {
                    LblHeader.Text = "Not Authorized!";
                    LblMSg.Text = "You do not have permission to view this page";
                }
                else if (_msg == "notexists")
                {
                    LblHeader.Text = "Does not Exist!";
                    LblMSg.Text = "The data you are trying to retrieve doesn't exist! ";

                }
                else if (_msg == "actiontaken")
                {
                    LblHeader.Text = "Action already Taken";
                    LblMSg.Text = "This page doesn't exist anymore as action has been already taken";
                }
                else if (_msg == "sc")
                {
                    LblHeader.Text = "Some special characters not allowed in fields";
                    LblMSg.Text = @"<html><head><title>HTML Not Allowed</title> </head>
                            <body style='font-family: Arial, Sans-serif;'><h1>Error Page!</h1>
                            <p>Error: for security reason, some characters like <, > are not allowed.</p>
                            <p>Please make sure that your inputs do not contain any angle brackets like &lt; or &gt;.</p>
                            <p><a href='javascript:history.go(-1);'>Go back</a></p></body></html>";
                    DivAddl.Visible = false;
                }
                else
                {
                     LblMSg.Text = " We're sorry. Something unexpected happened!";
                }
               
            }
        }

    }
}