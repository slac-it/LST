//$Header:$
//
//  Rules.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the utility class that has all the rules defined for the LST 
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
//using System.Data.OracleClient;
using System.Configuration;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace LST.Business
{
    
    public class TrainingRecord
    {
         private string _courseId;
        private string _completed;
        private string _inSTA;
       

        public string CourseID
        {
            get { return _courseId; }
            set { _courseId = value; }
        }

        public string Completed
        {
            get { return _completed; }
            set { _completed = value; }
        }

        public string InSTA
        {
            get { return _inSTA;}
            set {_inSTA = value;}
        }

  
        public TrainingRecord(string courseID, string completed, string inSTA)
        {
            _courseId = courseID;
            _completed = completed;
            _inSTA = inSTA;
  
        }

       
       
    }

    public class Rules
    {
        //Check if the worker has all the required training.. 131 if not taken can be allowed temporarily
        Data.Data_Util objData = new Data.Data_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        Data.DML_Util objDml = new Data.DML_Util();

        public bool Is253 { get; set; }
        public bool Is253ME { get; set; }
        public bool Is131 { get; set; }
        public bool Is120 { get; set; }
        public bool Is219 { get; set; }
        public bool Is396 { get; set; }  
        public bool Is253PRA { get; set; }
        public bool Is130 { get; set; }
        public bool Is136 { get; set; }
        public bool Is157 { get; set; }
        public bool Is108 { get; set; }
        public bool Is108PRA { get; set; }
        public bool Is219R { get; set; }
        public bool Is219Current { get; set; }
        public bool Is157R { get; set; }

        public enum UserType
        {
            Worker,
            User,
            AltSLSO
        }
        protected void GetTrainingStatus(string _userId, UserType usertype)
        {
            string _sqlTrain = "";
            string  _completedcrs = "";

            if (_userId != "")
            {
                List<TrainingRecord> trainrecs = new List<TrainingRecord>();
                if (usertype.Equals(UserType.Worker))
                {
                    trainrecs.Add(new TrainingRecord("253", "", ""));
                    trainrecs.Add(new TrainingRecord("253ME", "", ""));
                    trainrecs.Add(new TrainingRecord("131", "", ""));
                    trainrecs.Add(new TrainingRecord("120", "", ""));
                    trainrecs.Add(new TrainingRecord("219", "", ""));
                    trainrecs.Add(new TrainingRecord("219R", "", ""));
                    trainrecs.Add(new TrainingRecord("396", "", ""));
                    trainrecs.Add(new TrainingRecord("253PRA", "", ""));
                }
                else if (usertype.Equals(UserType.User))
                {
                    trainrecs.Add(new TrainingRecord("130", "", ""));
                    trainrecs.Add(new TrainingRecord("136", "", ""));
                    trainrecs.Add(new TrainingRecord("157", "", ""));
                    trainrecs.Add(new TrainingRecord("157R", "", ""));
                    trainrecs.Add(new TrainingRecord("108", "", ""));
                    trainrecs.Add(new TrainingRecord("108PRA", "", ""));
                }
                else if (usertype.Equals(UserType.AltSLSO))
                {
                    trainrecs.Add(new TrainingRecord("130", "", ""));
                }
               
                _sqlTrain = @"Select C.Course_Num, Decode(Count(E.Course_Num),0,'N','Y') Sta, 
                            Decode(Substr(Trainsafe.Get_non_sta_trn_status(:1,:2),1,3),'Nev','N','Ove','N','Val','Y','Com','Y','N') Training
                            From  trainsafe.Courses C, 
                            (select course_num, count(*) from trainsafe.eta_course_link_per where course_num = :2 and person_id = :1  AND course_optionality IN ('REQUIRED','RECOMMENDED') group by course_num) e
                            where  c.course_num = e.course_num(+) And C.Course_Num = :2 group by c.course_num";
                using (OracleCommand _cmdTrainrec = new OracleCommand())
                {
                    _cmdTrainrec.Parameters.Add(":1", OracleDbType.Varchar2);
                    _cmdTrainrec.Parameters.Add(":2", OracleDbType.Varchar2);
                    _cmdTrainrec.Parameters[0].Value = _userId;

                    
                        foreach (TrainingRecord rec in trainrecs)
                        {
                            _cmdTrainrec.Parameters[1].Value = rec.CourseID;
                            using (OracleDataReader _drTrainrec = objData.GetReader(_sqlTrain, _cmdTrainrec))
                            {
                                while (_drTrainrec.Read())
                                {
                                    _completedcrs = objCommon.FixDBNull(_drTrainrec["Training"]);
                                    SetCourseProperty(rec.CourseID, _completedcrs);
                                }
                            }
                        }                                           
                }
            }
        
        }

        protected void  SetCourseProperty(string courseId, string completeStat)
        {
            bool _compStat;
            if (completeStat == "Y") { _compStat = true;}
            else {_compStat = false;}
            switch (courseId)
            {
                    // For Worker
                case "253":
                    Is253 = _compStat;
                    break;
                case "253ME":
                    Is253ME = _compStat;
                    break;
                case "131":
                    Is131 = _compStat;
                    break;
                case "120":
                    Is120 = _compStat;
                    break;
                case "219":
                    Is219 = _compStat;
                    break;
                case "219R":
                    Is219R = _compStat;
                    break;
                case "396":
                    Is396 = _compStat;
                    break;
                case "253PRA":
                    Is253PRA =_compStat;
                    break;
                    // For users like SLSO
                case "130":
                    Is130 = _compStat;
                    break;
                case "136":
                    Is136 = _compStat;
                    break;
                case "157":
                    Is157 = _compStat;
                    break;

                case "108":
                    Is108 = _compStat;
                    break;
                case "108PRA":
                    Is108PRA = _compStat;
                    break;
                case "157R":
                    Is157R = _compStat;
                    break;


            }
                
        }

        public  bool HasGeneralTraining()
        {
            if ((Is253) && (Is253ME) && (Is120))
                return true;
            else
                return false;
        }

        public bool CheckIfCurrent(string _userId, string _courseId)
        {
            //string _sqlCheck = "select (To_Date(sysdate) - Trainsafe.Get_Last_Session_Date(:UserId,:CourseId)) from dual";
            string _sqlCheck = "select (trunc(TO_DATE(TRAINSAFE.GET_NON_STA_TRN_STATUS(:UserId,:CourseId,'DATE'),'MM/dd/yyyy')) - trunc(sysdate)) from dual";
            bool _current = false;
            int _days = 0;
            using  (OracleCommand _cmdCheck = new OracleCommand())
            {
                _cmdCheck.Parameters.Add(":UserId", OracleDbType.Varchar2).Value = _userId;
                _cmdCheck.Parameters.Add(":CourseId", OracleDbType.Varchar2).Value = _courseId;

                using (OracleDataReader _drCheck = objData.GetReader(_sqlCheck,_cmdCheck))
                {
                    while (_drCheck.Read())
                    {
                        if (_drCheck[0] != null)
                        {
                            _days = Convert.ToInt32(_drCheck[0]);
                        }
                    }
                    if (_days <= -1) _current = false;   // if within 24 months - their proposed due date minus system date
                    else _current = true;
                }
                return _current;
            }
        }

        public bool HasEmployeeTraining(int affiliationId, string _userId)
        {
            if (Is219)
            {
                //Check if 219 is current based on last completed date. It should be within the last 24 months
                Is219Current = CheckIfCurrent(_userId, "219");
            }

            //for employees, check 219 and 219R and for others, check 219,219R and 396
            //if (affiliationId == 9)
            //{
            //    if ((Is219Current) || (Is219R)) return true;
            //    else return false;
            //}
            //else
            //{
                if ((Is219Current) || (Is219R) || (Is396)) return true;
                else return false;
            //}

        }

        public bool HasQLOTraining(int workTypeId)
        {
             //For QLO, 253PRA is needed and not LCA Worker
            if (workTypeId == 7)
            {
                if (Is253PRA) return true;
                else return false;
            }
            else return true;
        }

        public bool AreTrainingReqsMetForWrkr(string _userId, int _affiliation,int _workType=0)
        {

            GetTrainingStatus(_userId, UserType.Worker);

            if (HasGeneralTraining() && HasEmployeeTraining(_affiliation, _userId) && HasQLOTraining(_workType))
            {
                return true;
            }
            else return false;    
        }

        public bool AreTrainingReqsMetForAltSLSO(string _userId)
        {
            GetTrainingStatus(_userId, UserType.AltSLSO);
            if (Is130)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

       

        public bool OverrideTraining()
        {
            string _override = ConfigurationManager.AppSettings["override"];
            if (_override == "Y")
                return true;
            else return false;
        }

       


        public bool AreTrainingReqsMetForUser(string _userId)
        {
            //Temporarily SLSO can't be checked for 108 and 108pra as they r not ready. SLSO override has to be turned off for checking these two courses also
            string _overrideslso = ConfigurationManager.AppSettings["slsoveride"];
            //bool Is157Completed = false;
            //bool Is157RCompleted = false;
            //string _157 = objDml.GetTrainingStatus(_userId, "157").Substring(0, 3).ToLower();
            //string _157R = objDml.GetTrainingStatus(_userId, "157R").Substring(0, 3).ToLower();
            //Is157Completed = ( _157 == "nev") ? false : true;
            //Is157RCompleted = (_157R == "nev") ? false : true;
            GetTrainingStatus(_userId, UserType.User);
            if (_overrideslso == "Y")
            {
               // if ((Is130) && ((Is136) || (Is157Completed) || (Is157RCompleted))) 
               if (Is130)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
               // if ((Is130) && ((Is136) || (Is157Completed) || (Is157RCompleted)) && (Is108) && (Is108PRA))
               if (Is130 && (Is108) && (Is108PRA))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
          
            
        }

        //Check if Alternate SLSO already active for that facility before adding a new one.
        public bool CheckIfActiveAlternate(int facilityId, int slacID)
        {
            int _count = 0;
            bool _result = false;
            string _sqlAlt = "";

            using (OracleCommand _cmdAlt = new OracleCommand())
            {
                if (facilityId == 0)
                {
                    //Need not check if alternate is not active becos the dates can be edited and make sure there is only once the alternate lso are added
                    _sqlAlt = "SELECT COUNT(*) FROM LST_USER_ROLES WHERE ROLE_TYPE_ID=20 AND SLAC_ID= :SlacId";
                    _cmdAlt.Parameters.Add(":SlacId", OracleDbType.Int32).Value = slacID;
                    //AND trunc(ALTLSO_TO) >= TRUNC(SYSDATE)";
                }
                else
                {
                    _sqlAlt = "SELECT COUNT(*) FROM LST_FACILITY WHERE FACILITY_ID = :FacilityId AND  Alternate_Slso Is Not Null And Trunc(Altslso_To) >= TRUNC(SYSDATE)";
                    _cmdAlt.Parameters.Add(":FacilityId", OracleDbType.Int32).Value = facilityId;
                    _cmdAlt.Parameters.Add(":SlacId", OracleDbType.Int32).Value = slacID;
                }
                

                using (OracleDataReader _drAlt = objData.GetReader(_sqlAlt,_cmdAlt))
                {
                    while (_drAlt.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drAlt[0]));
                    }
                    if (_count > 0)
                    {
                        _result = true;
                    }
                    else _result = false;
                    
                }
            }
            return _result;
        }

        //Check if person to be designated as alternate slso is an active QLO for that facility
        //Only then he/she can be added as Alternate SLSO
        public bool CheckIfActiveQLO(int facilityId, int alternateId)
        {
            int _count = 0;
            bool _result = false;

            string _SqlQLO= "";
            int _workerId = 0;

            _workerId = objDml.GetWorkerId(alternateId.ToString());

            _SqlQLO = "select count(*) from LST_WORKER_FACILITY_MAP WHERE  WORKER_TYPE_ID = 7 AND STATUS_ID= 5  AND WORKER_ID = :workerId ";
            using (OracleCommand _cmdQLO = new OracleCommand())
            {
                if (facilityId != 0)
                {
                    _SqlQLO += " AND FACILITY_ID = :FacilityId ";
                    _cmdQLO.Parameters.Add(":FacilityId", OracleDbType.Int32).Value = facilityId;
                }
                _cmdQLO.Parameters.Add(":WorkerId", OracleDbType.Int32).Value = _workerId;
                      
                using (OracleDataReader _drQLO = objData.GetReader(_SqlQLO,_cmdQLO))
                {
                    while(_drQLO.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drQLO[0]));
                    }
                    if (_count > 0)
                    {
                        _result = true;
                    }
                    else _result = false;

                }
            }
            return _result;
        }

        public string ApprReqMsg
        {
            get;
            set;
        }

        //Approval Request Rules
        //For an existing Worker
        //-------Check if already a worker for this Lab (workerid, facilityid)
        //-------Check for training 253 PRA for QLO (workertype)
        //-------Check if worker is active (workerid)
        //-------check if 131 is not completed and conditional approval not checked
        public bool AllRulesMetForRequestApproval(int workerId, string  slacId, int facilityId, int workerType, bool condApp, int affiliation)
        {
            bool _isAlreadyWorkerForLab,   _isJustification131;
            StringBuilder _sbMsg = new StringBuilder();

            _isAlreadyWorkerForLab = objDml.CheckIfWorkerForLab(workerId, facilityId, workerType);
            if (_isAlreadyWorkerForLab) {
                _sbMsg.Append("* Worker already requested approval or  associated with this facility  ");                           
            }
            //_isActiveWorker = objDml.CheckIfActive(slacId);
            //if (!_isActiveWorker) { _sbMsg.AppendLine("<br />* Worker is not active"); }

            //Check if 131 is current, if not, check if justification is entered
            GetTrainingStatus(slacId, UserType.Worker);
            if (!Is131)
            {
                _isJustification131 = (condApp) ? true : false;
                if (!_isJustification131) { _sbMsg.AppendLine("<br/>* Worker needs 131 Training. Otherwise choose conditional approval"); }
            } 
            else _isJustification131 = true;
            


            ApprReqMsg = _sbMsg.ToString();
            if (!_isAlreadyWorkerForLab  && _isJustification131)
                return true;
            else
                return false;
           
            
        }



    }
}