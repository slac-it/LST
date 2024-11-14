//$Header:$
//
// FileSummary.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the usercontrol that lists the file attached to a particular object

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Data.OracleClient;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace LST.UserControls
{
    public partial class FileSummary : System.Web.UI.UserControl
    {
        Data.DML_Util objDml = new Data.DML_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        public event EventHandler DeleteButtonClicked;


        #region "Properties"
        public string ObjType
        {
            get
            {
                return (ViewState["objtype"] != null)? ViewState["objtype"].ToString(): "";
            }
            set
            {
                ViewState["objtype"] = value;
            }
        }

        public int ObjId
        {
            get { return (ViewState["objId"]!= null) ? Convert.ToInt32(ViewState["objId"]):0; }
            set { ViewState["objId"] = value; }
        }

        public bool HideDelete
        {
            get { return (ViewState["hidedel"] != null) ? (bool)(ViewState["hidedel"]) : true; }
            set { ViewState["hidedel"] = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region "Gridview events"

        protected void gvFile_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int _attachmentId = Convert.ToInt32(e.CommandArgument.ToString());
            string _filename = "";
            if (e.CommandName == "download")
            {
               (this.Page as BasePage).FileData(_attachmentId, ObjType);

            }

            if (e.CommandName == "delete")
            {
                string _result = "";
                _result = objDml.DeleteAttachment(_attachmentId, objCommon.GetUserId(), ObjType);
                BindFileGrid();
            }
        }
      
        protected void GvFile_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        public void BindFileGrid()
        {
            DataSet _DsFile = new DataSet();
         
            _DsFile = objDml.GetFileInfo(ObjId, ObjType);

            if (_DsFile.Tables["file"].Rows.Count > 0)
            {
                PnlFile.Visible = true;
                GvFile.DataSource = _DsFile.Tables["file"];
                GvFile.DataBind();
            }
            else
            {
                PnlFile.Visible = false;
                OnDeleteButtonClicked();
            }
 
        }

        private void OnDeleteButtonClicked()
        {
            if (DeleteButtonClicked != null)
            {
                DeleteButtonClicked(this, EventArgs.Empty);
            }
        }

        protected void GvFile_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (HideDelete) { e.Row.Cells[4].Visible = false; }
            else { e.Row.Cells[4].Visible = true; }
        }
        #endregion
    }
}