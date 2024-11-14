//$Header:$
//
//  WorkerPendingRequestSummary.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the usercontrol that has list of pending request for a worker
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;


namespace LST.UserControls
{
    public partial class WorkerPendingRequestSummary : System.Web.UI.UserControl
    {
        Data.DML_Util objDml = new Data.DML_Util();
        Business.Common_Util objCommon = new Business.Common_Util();

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
                 BindPendingGrid();
            if (!ShowText)
            {
                PnlPending.GroupingText = "";
            }
        }

        #region "Properties"
        public int WorkerId
        {
            get
            {
                return ((ViewState["workerid"] != null) ? Convert.ToInt32(ViewState["workerid"]) : 0 );
            }
            set
            {
                ViewState["workerid"] = value;
            }
        }

        public bool ShowText
        {
            get;
            set;
        }
        #endregion

        private void BindPendingGrid()
        {
            using (OracleDataReader _drPending = objDml.GetPendingRequest(WorkerId))
            {
                GvPending.DataSource = _drPending;
                GvPending.DataBind();
            }
        }

     
    }
}