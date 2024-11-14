//$Header:$
//
//  Reports.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the report page of Laser safety tool.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Data.OracleClient;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace LST
{
    public partial class Reports : System.Web.UI.Page
    {
        Data.DML_Util objDml = new Data.DML_Util();
        protected void Page_Load(object sender, EventArgs e)
        {
         
             TxtSearchFac.Attributes.Add("onkeydown", "return onKeypress('CmdSearchFac');");
             TxtSearchLCA.Attributes.Add("onkeydown", "return onKeypress('CmdSearchLCA');");
             TxtSearchQLO.Attributes.Add("onkeydown", "return onKeypress('CmdSearchQLO');");
             TxtSearchSLSO.Attributes.Add("onkeydown", "return onKeypress('CmdSearchSLSO');");
            if (!Page.IsPostBack)
            {
                string _dd = (Request.QueryString["dd"] != null) ? Request.QueryString["dd"].ToString() : "";
                if ((_dd == "3") || (_dd ==""))
                {
                    DdlReportType.SelectedValue = "3";
                  
                }
                else
                {
                    DdlReportType.SelectedValue = _dd;
                }
               
                DdlReportType_SelectedIndexChanged(this, EventArgs.Empty);
              
            }
        }

        protected void DdlReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DdlReportType.SelectedValue == "0")
            {
                DivQLOSummary.Visible = true;
                BindQLOGrid();
                DivSLSOSummary.Visible = false;
                DivFacSummary.Visible = false;
                DivLCAWSummary.Visible = false;
                DivESHCoSum.Visible = false;
                DivSLSOOvd.Visible = false;
                DivPM.Visible = false;
                DivCourseovd.Visible = false;
                DivFacReq.Visible = false;
                DivWorkerReq.Visible = false;
                DivAdminSvr.Visible = false;
            }
            else if (DdlReportType.SelectedValue == "1")
            {
                DivQLOSummary.Visible = false;
                DivFacSummary.Visible = false;
                DivLCAWSummary.Visible = true;
                DivSLSOSummary.Visible = false;
                DivESHCoSum.Visible = false;
                DivPM.Visible = false;
                DivSLSOOvd.Visible = false;
                DivCourseovd.Visible = false;
                DivFacReq.Visible = false;
                DivWorkerReq.Visible = false;
                DivAdminSvr.Visible = false;
                BindLCAGrid();

            }
            else if (DdlReportType.SelectedValue == "2")
            {
                DivQLOSummary.Visible = false;
                DivFacSummary.Visible = false;
                DivLCAWSummary.Visible = false;
                DivSLSOSummary.Visible = true;
                DivESHCoSum.Visible = false;
                DivPM.Visible = false;
                DivSLSOOvd.Visible = false;
                DivCourseovd.Visible = false;
                DivFacReq.Visible = false;
                DivWorkerReq.Visible = false;
                DivAdminSvr.Visible = false;
                BindUserGrid("y");
            }
            else if (DdlReportType.SelectedValue == "3")
            {
                DivQLOSummary.Visible = false;
                DivFacSummary.Visible = true;
                DivLCAWSummary.Visible = false;
                DivSLSOSummary.Visible = false;
                DivESHCoSum.Visible = false;
                DivPM.Visible = false;
                DivSLSOOvd.Visible = false;
                DivCourseovd.Visible = false;
                DivFacReq.Visible = false;
                DivWorkerReq.Visible = false;
                DivAdminSvr.Visible = false;
                BindFacGrid("y");
            }
            else if (DdlReportType.SelectedValue == "4")
            {
                DivQLOSummary.Visible = false;
                DivFacSummary.Visible = false;
                DivLCAWSummary.Visible = false;
                DivSLSOSummary.Visible = false;
                DivESHCoSum.Visible = true;
                DivPM.Visible = false;
                DivSLSOOvd.Visible = false;
                DivCourseovd.Visible = false;
                DivFacReq.Visible = false;
                DivWorkerReq.Visible = false;
                DivAdminSvr.Visible = false;
                BindESHCGrid();
            }
            else if (DdlReportType.SelectedValue == "5")
            {
                DivQLOSummary.Visible = false;
                DivFacSummary.Visible = false;
                DivLCAWSummary.Visible = false;
                DivSLSOSummary.Visible = false;
                DivESHCoSum.Visible = false;
                DivPM.Visible = true;
                DivSLSOOvd.Visible = false;
                DivCourseovd.Visible = false;
                DivFacReq.Visible = false;
                DivWorkerReq.Visible = false;
                DivAdminSvr.Visible = false;
                BindPMGrid();
            }
            else if (DdlReportType.SelectedValue == "6")
            {
                DivQLOSummary.Visible = false;
                DivFacSummary.Visible = false;
                DivLCAWSummary.Visible = false;
                DivSLSOSummary.Visible = false;
                DivESHCoSum.Visible = false;
                DivPM.Visible = false;
                DivSLSOOvd.Visible = true;
                DivCourseovd.Visible = false;
                DivFacReq.Visible = false;
                DivWorkerReq.Visible = false;
                DivAdminSvr.Visible = false;
                BindSLSO130ovd();
            }
            else if (DdlReportType.SelectedValue =="7")
            {
                DivQLOSummary.Visible = false;
                DivFacSummary.Visible = false;
                DivLCAWSummary.Visible = false;
                DivSLSOSummary.Visible = false;
                DivESHCoSum.Visible = false;
                DivPM.Visible = false;
                DivSLSOOvd.Visible = false;
                DivCourseovd.Visible = true;
                DivFacReq.Visible = false;
                DivWorkerReq.Visible = false;
                DivAdminSvr.Visible = false;
                BindCourseovdGrid();
            }
            else if (DdlReportType.SelectedValue == "8")
            {
                DivQLOSummary.Visible = false;
                DivFacSummary.Visible = false;
                DivLCAWSummary.Visible = false;
                DivSLSOSummary.Visible = false;
                DivESHCoSum.Visible = false;
                DivPM.Visible = false;
                DivSLSOOvd.Visible = false;
                DivCourseovd.Visible = false;
                DivFacReq.Visible = true;
                DivWorkerReq.Visible = false;
                DivAdminSvr.Visible = false;
                BindFACReqGrid();
            }
            else if (DdlReportType.SelectedValue == "9")
            {
                DivQLOSummary.Visible = false;
                DivFacSummary.Visible = false;
                DivLCAWSummary.Visible = false;
                DivSLSOSummary.Visible = false;
                DivESHCoSum.Visible = false;
                DivPM.Visible = false;
                DivSLSOOvd.Visible = false;
                DivCourseovd.Visible = false;
                DivFacReq.Visible = false;
                DivWorkerReq.Visible = true;
                DivAdminSvr.Visible = false;
                BindWorkerReqGrid();
            }
            else if (DdlReportType.SelectedValue == "10")
            {
                DivQLOSummary.Visible = false;
                DivFacSummary.Visible = false;
                DivLCAWSummary.Visible = false;
                DivSLSOSummary.Visible = false;
                DivESHCoSum.Visible = false;
                DivPM.Visible = false;
                DivSLSOOvd.Visible = false;
                DivCourseovd.Visible = false;
                DivFacReq.Visible = false;
                DivWorkerReq.Visible = false;
                DivAdminSvr.Visible = true;
                BindAdminSvrGrid();
            }
        }




        #region"QLO Summary"
       
        private void BindQLOGrid()
        {
            GvQLO.DataBind();
         }

        protected void CmdSearchQLO_Click(object sender, EventArgs e)
        {
            BindQLOGrid();
        }

       
        protected void ImgBtnQLO_Click(object sender, ImageClickEventArgs e)
        {
            DataView dv = (DataView)SDSQLOSum.Select(DataSourceSelectArguments.Empty);
            
            DataGrid dg = new DataGrid();
            dg.DataSource = dv; 
            dg.DataBind();
            ExportToExcel("QLO-Summary.xls", dg);
            dg = null;
            dg.Dispose();
        }
        #endregion

        #region "Facility Summary"

        protected int FacCount
        {
            get
            {
                return (ViewState["faccount"] != null ? Convert.ToInt32(ViewState["faccount"]) : 0);
            }
            set
            {
                ViewState["faccount"] = value;
            }
        }

        private void BindFacGrid(string initial)
        {
            GVFacReq.DataBind();
            DataView dv = (DataView)SDSFacSum.Select(DataSourceSelectArguments.Empty);

            int _count = dv.Count; 
            if (initial.Equals("y"))
            {
                FacCount = _count;
            }
        }

        protected void ImgBtnFac_Click(object sender, ImageClickEventArgs e)
        {
            DataView dv = (DataView)SDSFacSum.Select(DataSourceSelectArguments.Empty);

            DataGrid dg = new DataGrid();
            dg.DataSource = dv; 
            dg.DataBind();
            ExportToExcel("Facility-summary.xls", dg);
            dg = null;
            dg.Dispose();
        }

        protected void CmdSearchFac_Click(object sender, EventArgs e)
        {
            BindFacGrid("n");
        }

       
        #endregion

        #region "LCA Worker Summary"
        
        private void BindLCAGrid()
        {
            GvLCAW.DataBind();
        }

        protected void ImgBtnLCA_Click(object sender, ImageClickEventArgs e)
        {
            DataView dv = (DataView)SDSLCAW.Select(DataSourceSelectArguments.Empty);

            DataGrid dg = new DataGrid();
            dg.DataSource = dv; 
            dg.DataBind();
            ExportToExcel("LCA-Summary.xls", dg);
            dg = null;
            dg.Dispose();
        }

        protected void CmdSearchLCA_Click(object sender, EventArgs e)
        {
            BindLCAGrid();
        }

        #endregion

        private void ExportToExcel(string fileName, DataGrid dg)
        {
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
            Response.ContentType = "application/vnd.ms-excel";
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            dg.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }
    
        protected int GetCount(int worktypeId, int statId)
        {
            int _count;
            _count = objDml.GetCountforReport(worktypeId, statId);
            return _count;
        }

        #region "User"

        public string SortExpressionUsr
        {
            get
            {
                if (null == ViewState["sortusr"])
                {
                    ViewState["sortusr"] = "USERNAME";
                }
                return ViewState["sortusr"].ToString();
            }
            set { ViewState["sortusr"] = value; }
        }

        public string SortDirectUsr
        {
            get
            {
                if (null == ViewState["sortdirusr"])
                {
                    ViewState["sortdirusr"] = "ASC";
                }
                return ViewState["sortdirusr"].ToString();
            }
            set
            {
                ViewState["sortdirusr"] = value;
            }

        }

        private int _userRoleID;

        public int UserRoleID
        {
            get { return _userRoleID; }
            set
            {
                _userRoleID = value;
                this.DataBind();
            }
        }

        public int SLSOCount
        {
            get
            {
                return (ViewState["slsocount"] != null ? Convert.ToInt32(ViewState["slsocount"]) : 0);
            }
            set
            {
                ViewState["slsocount"] = value;
            }
        }

        protected void FillUserDetails(OracleCommand cmdUser, string initial)
        {
            DataSet _dsUser = new DataSet();
            string _filter = " ORDER BY " + SortExpressionUsr + " " + SortDirectUsr;
            _dsUser = objDml.GetUserInfo(cmdUser, _filter, TxtSearchSLSO.Text);
            ViewState["user"] = _dsUser.Tables["user"];

            DataView _dvUser = new DataView(_dsUser.Tables["user"]);
            int _count = 0 ;
            _count = _dvUser.Count;
            if (initial == "y")
            {
               
                SLSOCount = _count;
            }
            

            if (_count > 0)
            {
                GVUser.DataSource = _dvUser;
                GVUser.DataBind();
            }
            else
            {
                GVUser.DataSource = null;
                GVUser.DataBind();
            }
        }

        private void BindUserGrid(string initial)
        {
            using (OracleCommand _cmdUser = new OracleCommand())
            {
                FillUserDetails(_cmdUser, initial);
            }
        }

        protected void GVUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GVUser.PageIndex = e.NewPageIndex;
            BindUserGrid("n");
         }

        protected void GVUser_Sorting(object sender, GridViewSortEventArgs e)
        {
            SortExpressionUsr = e.SortExpression;
            SortDirectUsr = (ViewState["sortdirusr"].ToString() == "ASC") ? "DESC" : "ASC";
            BindUserGrid("n");
            
        }


        protected DataTable BindGridSLSO(int userRoleId)
        {
            //int _userroleid = objDml.GetUserRoleId(UserId);
            DataSet _dsSLSOLab = new DataSet();

            _dsSLSOLab = objDml.GetSLSOLabInfo(userRoleId);

            if (_dsSLSOLab != null)
            {
                if (_dsSLSOLab.Tables["slsolabs"].Rows.Count > 0)
                    return _dsSLSOLab.Tables["slsolabs"];
                else
                    return null;
            }
            else return null;

        }

        protected void GVUser_RowCreated(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int _userRoleid = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "USER_ROLE_ID"));
                DataView _dv = new DataView(BindGridSLSO(_userRoleid));

                GridView gvSlso = e.Row.FindControl("GvSLSO") as GridView;
                gvSlso.DataSource = _dv;
                gvSlso.DataBind();

            }
        }

        protected void CmdSearchSLSO_Click(object sender, EventArgs e)
        {
            BindUserGrid("n");
        }

        protected void ImgBtnSLSO_Click(object sender, ImageClickEventArgs e)
        {
            DataTable _dtTemp = new DataTable();
            _dtTemp = AppendToTable(_dtTemp);
            DataGrid dg = new DataGrid();
            dg.DataSource = _dtTemp;
            dg.DataBind();
            ExportToExcel("SLSO-summary.xls", dg);
            dg = null;
            dg.Dispose();
        }

        private DateTime ReturnDateForm(object objOriginal)
        {
            DateTime _dtDate = new DateTime();
            if ((objOriginal != null) && (objOriginal.ToString() != ""))
            {
                _dtDate = (DateTime)objOriginal;
            }
            else
            {
                _dtDate = DateTime.MinValue;
            }
            return _dtDate;
        }

        private DataTable AppendToTable(DataTable dtTemp)
        {
            DataTable _dtOriginal = new DataTable();
            _dtOriginal = ViewState["user"] as DataTable;

          
            DataTable _dtslso = new DataTable();
            
            dtTemp.Columns.Add("<b>Name</b>");
            dtTemp.Columns.Add("<b>130 Valid thru</b>");
            dtTemp.Columns.Add("<b>108 Valid thru</b>");
            dtTemp.Columns.Add("<b>108PRA Completed on</b>");
            dtTemp.Columns.Add("<b>136 Completed on</b>");
            dtTemp.Columns.Add("<b>Phonebook Status</b>");
            dtTemp.Columns.Add("<b>Email</b>");
          
            DataRow _drowAddItem;
            for (int i = 0; i < _dtOriginal.Rows.Count; i++)
            {
                _drowAddItem = dtTemp.NewRow();

                _drowAddItem[0] = _dtOriginal.Rows[i]["USERNAME"].ToString();
                _drowAddItem[1] = (ReturnDateForm(_dtOriginal.Rows[i]["C130"]) != DateTime.MinValue)? ReturnDateForm(_dtOriginal.Rows[i]["C130"]).ToShortDateString() : "";
                _drowAddItem[2] = (ReturnDateForm(_dtOriginal.Rows[i]["C108"]) != DateTime.MinValue)? ReturnDateForm(_dtOriginal.Rows[i]["C108"]).ToShortDateString():"";
                _drowAddItem[3] = (ReturnDateForm(_dtOriginal.Rows[i]["C108PRA"]) != DateTime.MinValue) ? ReturnDateForm(_dtOriginal.Rows[i]["C108PRA"]).ToShortDateString():"";
                _drowAddItem[4] = (ReturnDateForm(_dtOriginal.Rows[i]["C136"]) != DateTime.MinValue) ? ReturnDateForm(_dtOriginal.Rows[i]["C136"]).ToShortDateString() : "";
                _drowAddItem[5] = _dtOriginal.Rows[i]["STATUS"].ToString();
                _drowAddItem[6] = _dtOriginal.Rows[i]["EMAIL"].ToString();


                //if (_dtOriginal.Rows[i]["USER_ROLE_ID"] != null)
                //{

                //        _dtslso = objDml.GetSLSOLabInfo(Convert.ToInt32(_dtOriginal.Rows[i]["USER_ROLE_ID"])).Tables["slso"];
                //        if (_dtslso != null)
                //        {
                //            DataRow _drowFac = null;
                //            for (int j = 0; j < _dtslso.Rows.Count;j++ )
                //            {
                //                _drowFac = dtTemp.NewRow();
                //                _drowFac[0] = _dtslso.Rows[j]["FACILITY_NAME"].ToString();
                //                _drowFac[1] = _dtslso.Rows[j]["USERTYPE"].ToString();
                //            }
                //            dtTemp.Rows.Add(_drowFac);

                //        }

                //   }
                //else
                //{
                //    _drowAddItem[0] = "";
                //    _drowAddItem[1] = "";


                // }

                dtTemp.Rows.Add(_drowAddItem);
            }
            return dtTemp;


        }

        #endregion

        #region "ESH coord"

        protected void ImgBtnESHC_Click(object sender, ImageClickEventArgs e)
        {
            DataView dv = (DataView)SDSEsh.Select(DataSourceSelectArguments.Empty);
            var dt = dv.ToTable();
          
            DataGrid dg = new DataGrid();
            dg.DataSource = dt;
            dg.DataBind();
            ExportToExcel("ESHCo-ord-Summary.xls", dg);
            dg = null;
            dg.Dispose();
        }

        public string ESHCCount
        {
            //get
            //{
            //    return (ViewState["coordcount"] != null ? ViewState["coordcount"].ToString() : "");
            //}
            //set
            //{
            //    ViewState["coordcount"] = value;
            //}
            get;
            set;
        }

       protected void BindESHCGrid()
       {
           GVESHC.DataBind();
           DataView dv = (DataView)SDSEsh.Select(DataSourceSelectArguments.Empty);
           string _count = dv.Table.Rows.Count.ToString();
           ESHCCount = _count;
       }


       protected void BtnSearchESHC_Click(object sender, EventArgs e)
       {
           BindESHCGrid();
       }

        #endregion

        #region "Program Mgr"

       protected void ImgBtnPM_Click(object sender, ImageClickEventArgs e)
       {
           DataView dv = (DataView)SDSPM.Select(DataSourceSelectArguments.Empty);
           var dt = dv.ToTable();

           DataGrid dg = new DataGrid();
           dg.DataSource = dt;
           dg.DataBind();
           ExportToExcel("Prgmgr-Summary.xls", dg);
           dg = null;
           dg.Dispose();
       }

       protected void BtnSearchPM_Click(object sender, EventArgs e)
       {
           BindPMGrid();
       }

        protected void BindPMGrid()
       {
           GVPM.DataBind();
           DataView dv = (DataView)SDSPM.Select(DataSourceSelectArguments.Empty);
           string _count = dv.Table.Rows.Count.ToString();
           PMCount = _count;
       }

        protected string PMCount
        {
            get;
            set;

        }

       #endregion
 
        #region "SLSO 130 overdue"
        protected void ImgBtnSLSOvd_Click(object sender, ImageClickEventArgs e)
        {
            DataView dv = (DataView)SDSSLSOvd.Select(DataSourceSelectArguments.Empty);
            var dt = dv.ToTable();

            DataGrid dg = new DataGrid();
            dg.DataSource = dt;
            dg.DataBind();
            ExportToExcel("SLSO-130overdue.xls", dg);
            dg = null;
            dg.Dispose();
        }

        protected void BtnSLSOvd_Click(object sender, EventArgs e)
        {
            BindSLSO130ovd();
        }

        protected void BindSLSO130ovd()
        {
            GVSLSOvd.DataBind();
            DataView dv = (DataView)SDSSLSOvd.Select(DataSourceSelectArguments.Empty);
            string _count = dv.Table.Rows.Count.ToString();
            SLSO130ovdCount = _count;
        }

        protected string SLSO130ovdCount
        {
            get
            {
                return (ViewState["slso130"] != null ? ViewState["slso130"].ToString() : "");
            }
            set
            {
                ViewState["slso130"] = value;
            }

        }

        #endregion 

        #region "Course 253,219R Overdue"
        protected void ImgBtnCourseovd_Click(object sender, ImageClickEventArgs e)
        {
            SetSDSCourseOD();
            DataView dv = (DataView)SDSCourseovd.Select(DataSourceSelectArguments.Empty);
            var dt = dv.ToTable();

            DataGrid dg = new DataGrid();
            dg.DataSource = dt;
            dg.DataBind();
            ExportToExcel("Course253219overdue.xls", dg);
            dg = null;
            dg.Dispose();
        }

        protected void BtnCourseovd_Click(object sender, EventArgs e)
        {
            
            BindCourseovdGrid();
        }

        protected void SetSDSCourseOD()
        {
            SDSCourseovd.SelectCommand = @"SELECT * FROM
                (select PC.NAME as Worker,w.slac_id ,'253' AS COURSE,Trainsafe.Get_Last_Session_Date(w.SLAC_ID,'253') AS DATE_LASTCOMPLETED ,
                pc.MAILDISP AS SLAC_EMAIL, W.PREFERRED_EMAIL,initcap(PC.GONET) AS PHONEBOOK_STATUS
                from lst_worker W LEFT JOIN PERSONS.PERSON PC ON PC.KEY = W.SLAC_ID
                where trunc(To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'253','DATE'),'MM/dd/yyyy')) - trunc(sysdate) <= -1
                and status_id in (5,6) and is_active='Y'
                UNION
                Select PC1.NAME as Worker, W1.SLAC_ID,'219R' AS COURSE,Trainsafe.Get_Last_Session_Date(W1.SLAC_ID,'219R') AS DATE_LASTCOMPLETED,
                PC1.MAILDISP AS SLAC_EMAIL, W1.PREFERRED_EMAIL,initcap(PC1.GONET) AS PHONEBOOK_STATUS 
                from lst_worker W1 LEFT JOIN PERSONS.PERSON PC1 ON PC1.KEY = W1.SLAC_ID
                where ((To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'219','DATE'),'MM/dd/yyyy') IS  NULL AND
                To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'219R','DATE'),'MM/dd/yyyy') IS NULL AND
                To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'396','DATE'),'MM/dd/yyyy') IS NULL)
                OR
                (
                ((trunc(To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'219','DATE'),'MM/dd/yyyy')) - trunc(sysdate) <= -1) OR 
                (To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'219','DATE'),'MM/dd/yyyy') IS NULL)) and 
                ((trunc(To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'219R','DATE'),'MM/dd/yyyy')) - trunc(sysdate) <= -1) OR 
                (To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'219R','DATE'),'MM/dd/yyyy') IS NULL)) and  
                ((trunc(To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'396','DATE'),'MM/dd/yyyy')) - trunc(sysdate) <= -1) OR 
                (To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'396','DATE'),'MM/dd/yyyy') IS NULL))))
                and status_id in (5,6) and is_active='Y') WHERE WORKER IS NOT NULL ORDER BY WORKER";
            SDSCourseovd.DataBind();
        }

        protected void BindCourseovdGrid()
        {
            SetSDSCourseOD();
            //DataView dv = (DataView)SDSCourseovd.Select(DataSourceSelectArguments.Empty);
            CourseovdCount = objDml.GetCountWorker253219().ToString();
            GVCourseovd.DataBind();          
        }

        protected string CourseovdCount
        {
            get;
            set;

        }

        protected void GVCourseovd_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GVCourseovd.PageIndex = e.NewPageIndex;
            BindCourseovdGrid();
        }



        #endregion

        #region "Facility Approval Request"
        protected void BindFACReqGrid()
        {
            GVFacReq.DataBind();
            DataView dv = (DataView)SDSFACREQ.Select(DataSourceSelectArguments.Empty);
            string _count = dv.Table.Rows.Count.ToString();
            FacReqCount = _count;
        }

        protected string FacReqCount
        {
            get
            {
                return (ViewState["facreqc"] != null) ? ViewState["facreqc"].ToString() : "0";
            }
            set
            {
                ViewState["facreqc"] = value;
            }
        }

        protected void ImgBtnFacreq_Click(object sender, ImageClickEventArgs e)
        {
            DataView dv = (DataView)SDSFACREQ.Select(DataSourceSelectArguments.Empty);
            var dt = dv.ToTable();
            dt.Columns.Remove("FACILITY_ID");
            dt.Columns.Remove("STATUS_ID");
            DataGrid dg = new DataGrid();
            dg.DataSource = dt;
            dg.DataBind();
            ExportToExcel("Facility Approval Requests.xls", dg);
            dg = null;
            dg.Dispose();
        }

        protected void BtnSearchFacreq_Click(object sender, EventArgs e)
        {
            BindFACReqGrid();
        }
        #endregion

        #region "Worker Approval Requests"
        protected void BindWorkerReqGrid()
        {
            GVWorkerReq.DataBind();
            DataView dv = (DataView)SDSWorkerReq.Select(DataSourceSelectArguments.Empty);
            string _count = dv.Table.Rows.Count.ToString();
            WorkerReqCount = _count;
        }

        protected string WorkerReqCount
        {
            get
            {
                return (ViewState["workreqc"] != null) ? ViewState["workreqc"].ToString() : "0";
            }
            set
            {
                ViewState["workreqc"] = value;
            }
        }
        protected void BtnSearchWorkerReq_Click(object sender, EventArgs e)
        {
            BindWorkerReqGrid();
        }

        protected void ImgBtnWorkerReq_Click(object sender, ImageClickEventArgs e)
        {
            DataView dv = (DataView)SDSWorkerReq.Select(DataSourceSelectArguments.Empty);
            var dt = dv.ToTable();
            dt.Columns.Remove("STATUS_ID");
            dt.Columns.Remove("FACILITY_ID");
            dt.Columns.Remove("WORKER_ID");
            DataGrid dg = new DataGrid();
            dg.DataSource = dt;
            dg.DataBind();
            ExportToExcel("Laser Worker Approval Requests.xls", dg);
            dg = null;
            dg.Dispose();
        }
        #endregion


        #region "Admin Supervisor Report"
        protected void BindAdminSvrGrid()
        {
            GVAdminSvr.DataBind();
            DataView dv = (DataView)SDSAdminSvr.Select(DataSourceSelectArguments.Empty);
            int _count = objDml.GetAdminSVRCount();
            AdminSvrCount = _count.ToString();
        }

        protected string AdminSvrCount
        {
            get
            {
                return (ViewState["adminsvrc"] != null) ? ViewState["adminsvrc"].ToString() : "0";
            }
            set
            {
                ViewState["adminsvrc"] = value;
            }
        }
        protected void ImgBtnAdminsvr_Click(object sender, ImageClickEventArgs e)
        {
            DataView dv = (DataView)SDSAdminSvr.Select(DataSourceSelectArguments.Empty);
            var dt = dv.ToTable();
            dt.Columns.Remove("APPROVER_ID");
            dt.Columns.Remove("WORKER_ID");
            DataGrid dg = new DataGrid();
            dg.DataSource = dt;
            dg.DataBind();
            ExportToExcel("Admin Supervisor Approved Requests.xls", dg);
            dg = null;
            dg.Dispose();
        }

        protected void BtnSearchSvr_Click(object sender, EventArgs e)
        {
            BindAdminSvrGrid();
        }

        protected void GVWorkerReq_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HyperLink hlkfac = (HyperLink)e.Row.FindControl("hlkFac");
                
               if  (hlkfac.Text == "None")
                {
                    hlkfac.NavigateUrl = "";
                    hlkfac.Enabled = false;
                }

            }
        }

        #endregion
    }
}