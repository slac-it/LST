//$Header:$
//
//  AddLink.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is user control for adding links on any page.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LST.UserControls
{
    public partial class AddLink : System.Web.UI.UserControl
    {
        public string UrlText
        {
            get
            {
                return (ViewState["urltext"] != null) ? ViewState["urltext"].ToString() : "";
            }
            set { ViewState["urltext"] = value; }
        }

        public string Url
        {
            get
            {
                return (ViewState["url"] != null) ?  ViewState["url"].ToString() : "";
            }
            set { ViewState["url"] = value; }

        }
  
        protected void Page_Load(object sender, EventArgs e)
        {
            CmdCancel.Attributes.Add("onClick", "DialogClose('dialog-link');return false;");
            CmdAddLink.Attributes.Add("onClick", "OpenDialogForAdding('dialog-link');return false;");
            if (!Page.IsPostBack)
            {
                DisplayLink();
               
            }
        }

        protected void CmdOk_Click(object sender, EventArgs e)
        {
                UrlText = TxtUrltext.Text;
                Url = TxtUrl.Text;
                DisplayLink();
        }

        protected void DisplayLink()
        {
            if ((UrlText != "") && (Url != ""))
            {
                aLink.HRef = Url;
                aLink.InnerText = UrlText;
                CmdAddLink.Text = "Change this link";
                ImgBtnDelete.Visible = true;
            }
            else
            {
                ClearLink();
            }
        }

        private void ClearLink()
        {
            Url = "";
            UrlText = "";
            aLink.InnerText = "";
            aLink.HRef = "";
            CmdAddLink.Text = "Add Link";
            ImgBtnDelete.Visible = false;
        }

        protected void ImgBtnDelete_Click(object sender, ImageClickEventArgs e)
        {
            ClearLink();
            CmdAddLink.Focus();
        }

 
    }
}