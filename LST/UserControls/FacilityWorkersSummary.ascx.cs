//$Header:$
//
//  FacilityWorkersSummay.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the usercontrol that lists the workers and status for a particular facility.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;

namespace LST.UserControls
{
    public partial class FacilityWorkersSummary : System.Web.UI.UserControl
    {
        Data.DML_Util objDml = new Data.DML_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        Business.Employee objEmployee = new Business.Employee();

        //Event handler to bubble up the event to parent page
        public delegate void DeleteEventHandler(object sender);
        public event DeleteEventHandler AssociationDeleted;

        private void OnAssociationDeleted()
        {
            if (AssociationDeleted != null)
            {
                AssociationDeleted(this); //bubble up to the parent page
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!Page.IsPostBack)
            {
                FillWorkers();
            }
            //if (IsSLSOAlt)     //|| (IsAdmin))
            //{
            //    spnimg.Visible = true;
            //}
            //else
            //{
            //    spnimg.Visible = false;
            //}
          
        }

      

        #region "Properties"
        public int FacId
        {
            get
            {
                return (ViewState["facid"] != null) ? Convert.ToInt32(ViewState["facid"]) : 0;
            }
            set
            {
                ViewState["facid"] = value;
            }
        }

        public bool IsSLSOAlt
        {
            get
            {
                return (ViewState["slsoalt"] != null) ? (bool)ViewState["slsoalt"] : false;
            }
            set
            {
                ViewState["slsoalt"] = value;
            }
        }

        //public bool IsAdmin
        //{
        //    get
        //    {
        //        return (ViewState["admin"] != null) ? (bool)ViewState["admin"] : false;
        //    }
        //    set
        //    {
        //        ViewState["admin"] = value;
        //    }
        //}
        #endregion

        #region "Gridview Events"
        protected void GvWorkers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GvWorkers.EditIndex = -1;
            LblMsg2.Text = "";
            FillWorkers();
            KeepGridExpanded();
        }

        protected void GvWorkers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GvWorkers.EditIndex = e.NewEditIndex;
            LblMsg2.Text = "";
            FillWorkers();
            KeepGridExpanded();
        }

        protected void GvWorkers_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (e.Exception != null)
            {
                e.ExceptionHandled = true;
                e.KeepInEditMode = true;
            }
        }

        protected void GvWorkers_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

            int _mapId;
            TextBox TxtOJT;
            TextBox TxtSOP;

            int _result, _resultSOP;
            bool _error = false;

            if (Page.IsValid)
            {
                string _msgOJT = "";
                string _msgSOP = "";
                _mapId = Convert.ToInt32(GvWorkers.DataKeys[e.RowIndex].Values[0]);
                TxtOJT = (TextBox)(GvWorkers.Rows[e.RowIndex].FindControl("TxtOJT"));
                TxtSOP = (TextBox)(GvWorkers.Rows[e.RowIndex].FindControl("TxtSOP"));

                DateTime _dtSOP = Convert.ToDateTime(TxtSOP.Text);
                DateTime _dtSOPFac = objDml.GetSOPReviewDate(_mapId);
                DateTime _dtOjt = Convert.ToDateTime(TxtOJT.Text);
                

                if (DateTime.Now > _dtOjt.AddYears(1))
                {
                    _msgOJT = "WARNING -- the OJT Completion date you entered is more than 1 year old.  To maintain an active status for this laser facility, OJT must be completed within the past year (+ 30-day grace period). <br>";
                    _error = false;
                }
                if (_dtSOP < _dtSOPFac)
                {

                    _msgSOP = "<br> <font color=red> Error: SOP review date should fall after current SOP approval  date for the facility which is  " + _dtSOPFac.ToShortDateString() + "</font>";
                    _error = true;
                }
                if (_msgOJT != "")
                {
                    LblMsg2.Text = _msgOJT;
                }

                if (_error == false)
                {
                    _result = objDml.UpdateOJT(_mapId, Convert.ToDateTime(TxtOJT.Text), HttpContext.Current.Session["LoginSID"].ToString());
                        if (_result != 0)
                        {
                            LblMsg2.Text = "Error updating OJT Completion Date";
                              }
                        else
                        {
                            _resultSOP = objDml.UpdateSOP(_mapId, _dtSOP, HttpContext.Current.Session["LoginSID"].ToString());
                            if (_resultSOP != 0)
                            {
                                LblMsg2.Text = "Error updating SOP review Date for this facility";
                                                         }
                            else
                            {
                                if (_msgOJT == "") { LblMsg2.Text = ""; }
                            }
                        }
                        GvWorkers.EditIndex = -1;
                        FillWorkers();
                        KeepGridExpanded();
                }
                else
                {
                    LblMsg2.Text += _msgSOP;
                    _msgSOP = "";
                }


                if (LblMsg2.Text != "") { Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-msg2');", true); }

            }
            else
            {
                e.Cancel = true;

            }

        }

        private void KeepGridExpanded()
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "toggleme('DivWorkers','btnShow1','btnHide1');", true);
        }


        protected void GvWorkers_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (!IsSLSOAlt) //&& (!IsAdmin))
            {
               // GvWorkers.Columns[5].Visible = false;
                GvWorkers.Columns[7].Visible = false;
            }
     
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //DataRowView _row = (DataRowView)e.Row.DataItem;
                    //int _statusId;
                    //int _mapId;
                    //bool _isOJTAttached = false;
                   

                    //_statusId = Convert.ToInt32(_row["STATUS_ID"]);
                    //_mapId = Convert.ToInt32(_row["MAP_ID"]);
                   
                    //check if ojt is attached to the facility,worker
                    //if so, hide the lnkattach, show actual file and delete
                    //_isOJTAttached = objDml.CheckifOJTAttached(_mapId);

                    ////UserControls.FileAttach UCFileOJT = (UserControls.FileAttach)e.Row.FindControl("UCFileOJT");
                    ////HtmlGenericControl DivAttach = (HtmlGenericControl)e.Row.FindControl("DivAttach");
                    //HtmlGenericControl DivFile = (HtmlGenericControl)e.Row.FindControl("DivFile");
                    //if (_isOJTAttached)
                    //{
                        
                    ////    DivAttach.Visible = false;
                    //    DivFile.Visible = true;

                    //}
                    //else
                    //{ 
                    ////    UCFileOJT.Visible = true;
                    ////    UCFileOJT.ObjId = _mapId;
                    ////    DivAttach.Visible = true;
                    //    DivFile.Visible = false;
                    //}
                    
            }
        
        }

        //private void UCFileOJT_AttachButtonClicked(object sender, EventArgs e)
        //{
        //    FillWorkers();
        //}

        protected void GvWorkers_RowCommand(object sender, GridViewCommandEventArgs e)
        {       
            //if ((e.CommandName == "download") || (e.CommandName == "delete"))
            //{
            //    int _attachmentId = Convert.ToInt32(e.CommandArgument.ToString());
            //    if (e.CommandName == "download")
            //    {
            //        (this.Page as BasePage).FileData(_attachmentId, "WorkFac");

            //    }

            //    if (e.CommandName == "delete")
            //    {
            //        string _result = "";
            //        _result = objDml.DeleteAttachment(_attachmentId, objCommon.GetUserId(), "WorkFac");
            //        FillWorkers();
            //    }
            //}

            if (e.CommandName == "unassociate")
            {
                int rowIndex = int.Parse(e.CommandArgument.ToString());
                int _mapId = Convert.ToInt32(this.GvWorkers.DataKeys[rowIndex]["MAP_ID"]);
                string _resultD = "";
                _resultD = objDml.DeleteObject(_mapId, HttpContext.Current.Session["LoginSID"].ToString(), "userfac");
                FillWorkers();
                KeepGridExpanded();
                OnAssociationDeleted(); //call the delegate
            }

          
            
        }

        protected void GvWorkers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }
        #endregion

        #region "Other"
        private void FillWorkers()
        {
            DataSet _dsWorkers = new DataSet();

            _dsWorkers = objDml.GetWorkersForFacility(FacId);

            if (_dsWorkers.Tables["workfac"].Rows.Count > 0)
            {
                GvWorkers.DataSource = _dsWorkers.Tables["workfac"];
                GvWorkers.DataBind();
            }
            else
            {
                GvWorkers.DataSource = null;
                GvWorkers.DataBind();
            }
        }

        //protected void ImgRefresh_Click(object sender, ImageClickEventArgs e)
        //{
        //    FillWorkers();
        //}

        public string GetBadgeId(string slacId)
        {
            return objEmployee.GetBadgeId(slacId);
        }
        #endregion

    }
}