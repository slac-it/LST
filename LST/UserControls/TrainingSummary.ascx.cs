//$Header:$
//
//  TrainingSummary.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the usercontrol that has the training summary for each worker
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace LST.UserControls
{
    public partial class TrainingSummary : System.Web.UI.UserControl
    {
        Data.DML_Util objDml = new Data.DML_Util();
        

        #region "Properties"
        public string EmpId
        {
            get;
            set;
        }

        public string EmpType
        {
            get;
            set;
        }

        public string WorkType
        {
            get;
            set;
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (EmpId != "")
            {
                SetVisibility();
                Txt219.Value = objDml.GetTrainingStatus(EmpId, "219").Substring(0, 3).ToLower();
                Txt396.Value = objDml.GetTrainingStatus(EmpId, "396").Substring(0, 3).ToLower();
                Txt219R.Value = objDml.GetTrainingStatus(EmpId, "219R").Substring(0, 3).ToLower();
            }
             
        }

     

        protected string IsInSTA(string courseId)
        {
            string _result = objDml.IsInSTA(EmpId, courseId);
            return _result;
        }

        protected string GetStatus(string courseId)
        {
            string _stat = objDml.GetTrainingStatus(EmpId, courseId);
            string _result = _stat;
            if (((courseId == "219") || (courseId == "396")) && (_result.Substring(0,3).ToLower() == "ove"))
            {
                //return the completed date - wrong completion date??? Need to check
                //Get last session date is not useful anymore and get date from non sta function returns wrong completed date
                _result = objDml.GetCompletionDate(EmpId, courseId);
                
            }
               
            return _result;
        }

      

        protected void SetVisibility()
        {
            //bool Is219 = false;
            //bool Is396 = false;
            bool Is253PRA = false;
            //if (EmpType == "emp")
            //{
            //   trnoemp.Visible = false;
            //}
            //else
            //{
            //    trnoemp.Visible = true;
            //}
            //else if (EmpType == "noemp")
            //{
            //    Is396 = objDml.IsTrainingCurrent(EmpId, "396");
            //    if (Is396)
            //    {
            //        trEmp.Visible = false;
            //        trNoemp.Visible = true;
            //    }
            //    else
            //    {
            //        Is219 = objDml.IsTrainingCurrent(EmpId, "219");
            //        if (Is219)
            //        {
            //            trEmp.Visible = true;
            //            trNoemp.Visible = false;
            //        }
            //        else
            //        {
            //            trEmp.Visible = false;
            //            trNoemp.Visible = true;
            //        }
            //    }
            //}
            //else {
            //    trEmp.Visible = false;
            //    trNoemp.Visible = false;
            //}

            //Show 253PRa for those who completed 253PRA even if they are not QLO's
            if (WorkType == "QLO")
            {
                trQLO.Visible = true;
            }
            else
            { 
                Is253PRA = objDml.IsTrainingCurrent(EmpId, "253PRA");
                if (Is253PRA)
                {
                    trQLO.Visible = true;
                }
                else {trQLO.Visible = false;}
           }
        }
    }
}