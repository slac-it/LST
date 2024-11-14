//$Header:$
//
//  ApprovalHistory.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the usercontrol that has all the approvals pertaining to a worker request.
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
    public partial class FACApprovalHistory : System.Web.UI.UserControl
    {
        Data.DML_Util objDml = new Data.DML_Util();
        protected void Page_Load(object sender, EventArgs e)
        {
            BindApprovalGrid();
        }

        public int RequestId
        {
            get;
            set;
        }

        protected void BindApprovalGrid()
        {
            using (OracleDataReader _drAppHistory = objDml.GetFACApprovalHistory(RequestId))
            {
                 GvFACApprovalHis.DataSource = _drAppHistory;
                 GvFACApprovalHis.DataBind();
            }
               
 
        }

    }
}