//$Header:$
//
//  Common_Util.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the utility class that has common functions
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Data.OracleClient;
using log4net;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;


namespace LST.Business
{
    public class Common_Util
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(Common_Util));
        Data.Data_Util objData = new Data.Data_Util();

        public string FixDBNull(object rdrObj)
        {
            return (rdrObj.Equals(DBNull.Value))? "" : rdrObj.ToString();
        }

       //public string GetUserId()
       // {
       //     string _userId = null;
       //     string _currentUser = null;
       //     string _serVar = null;
       //     string _userName = null;
       //     int _pos = 0;
       //     string _sqlSelect = null;

       //     _userId = Convert.ToString(HttpContext.Current.Session["EmpID"]);

       //     if (!string.IsNullOrEmpty(_userId))
       //     {
       //         HttpContext.Current.Session["EmpID"] = _userId;
       //         _currentUser = _userId;
       //     }
       //     else
       //     {
       //         _serVar =  HttpContext.Current.Request.ServerVariables["LOGON_USER"];
       //         _pos = _serVar.LastIndexOf("\\");
       //         _userName = _serVar.Substring(_pos + 1);

       //         _sqlSelect = "select max(but_sid) from but where but_kid = upper(:UserName)";

       //         using (OracleCommand _cmdUser = new OracleCommand())
       //         {
       //             _cmdUser.Parameters.Add(":UserName", OracleDbType.Varchar2).Value = _userName;
       //             using (OracleDataReader _drUser = objData.GetReader(_sqlSelect, _cmdUser))
       //             {
       //                 try
       //                 {
       //                     if (_drUser.HasRows)
       //                     {
       //                         while (_drUser.Read())
       //                         {
       //                             _userId = Convert.ToString(_drUser[0]);
       //                         }
       //                     }
       //                     if (_userId == "")
       //                     {
       //                         Log.Error("User ID is empty");
       //                         _userId = "err";
       //                     }
 
       //                 }
       //                 catch (NullReferenceException exNull)
       //                 {
       //                     Log.Error(exNull);
       //                     _userId = "err";
       //                 }
       //                 catch (Exception ex)
       //                 {
       //                     Log.Error(ex);
       //                     _userId = "err";
       //                 }

       //             }
                     
                       
       //                 HttpContext.Current.Session["EmpID"] = _userId;
       //                 _currentUser = _userId;
       //         }
                 

               
       //     }
       //     return _currentUser;
               
       // }

       public OracleDataReader GetMatchingEmployees(string Owner)
       {

           string _sqlEmployee;

           Regex reCheck = new Regex("^[0-9]*$");

           using (OracleCommand _cmdEmp = new OracleCommand())
           {
               if (reCheck.IsMatch(Owner))
               {
                   //Employee id query
                   _sqlEmployee = @"SELECT PC.EMPLOYEE_NAME, PC.EMPLOYEE_ID ,PC.EPO,PC.ORG_LEVEL_1_DESC,ORG.DESCRIPTION FROM VW_PEOPLE_CURRENT PC
                                        LEFT JOIN SID.ORGANIZATIONS ORG  ON ORG.ORG_ID = PC.ORG_LEVEL_1_CODE WHERE EMPLOYEE_ID=:Owner";
                   _cmdEmp.Parameters.Add(":Owner", OracleDbType.Varchar2).Value = Owner;
               }
               else
               {
                   //Employee name query
                   Owner = Owner.ToLower();
                   _sqlEmployee = @"SELECT PC.EMPLOYEE_NAME, PC.EMPLOYEE_ID ,PC.EPO,PC.ORG_LEVEL_1_DESC,ORG.DESCRIPTION FROM VW_PEOPLE_CURRENT PC
                                        LEFT JOIN SID.ORGANIZATIONS ORG  ON ORG.ORG_ID = PC.ORG_LEVEL_1_CODE WHERE LOWER(EMPLOYEE_NAME) LIKE :Owner ORDER BY EMPLOYEE_NAME";
                   _cmdEmp.Parameters.Add(":Owner", OracleDbType.Varchar2).Value = Owner + "%";
               }
               OracleDataReader _drEmployees = objData.GetReader(_sqlEmployee, _cmdEmp);

               return _drEmployees;

           }
       }

       public OracleDataReader GetMatchingEmployeesDD(string Owner)
       {

           string _sqlEmployee;

           Regex reCheck = new Regex("^[0-9]*$");

           using (OracleCommand _cmdEmp = new OracleCommand())
           {
               if (reCheck.IsMatch(Owner))
               {
                   //Employee id query
                   _sqlEmployee = @"SELECT EMPLOYEE_NAME, EMPLOYEE_ID, EMPLOYEE_NAME || ' - ' || EMPLOYEE_ID AS EMPNAMEID FROM VW_PEOPLE_CURRENT WHERE EMPLOYEE_ID=:Owner";
                   _cmdEmp.Parameters.Add(":Owner", OracleDbType.Varchar2).Value = Owner;
               }
               else
               {
                   //Employee name query
                   Owner = Owner.ToLower();
                   _sqlEmployee = @"SELECT EMPLOYEE_NAME, EMPLOYEE_ID,EMPLOYEE_NAME || ' - ' || EMPLOYEE_ID AS EMPNAMEID FROM VW_PEOPLE_CURRENT WHERE LOWER(EMPLOYEE_NAME) LIKE :Owner ORDER BY EMPLOYEE_NAME";
                   _cmdEmp.Parameters.Add(":Owner", OracleDbType.Varchar2).Value = Owner + "%";
               }
               OracleDataReader _drEmployees = objData.GetReader(_sqlEmployee, _cmdEmp);

               return _drEmployees;

           }
       }

       public int GetEmpid(string empname)
       {
           int _empId = 0;
           OracleDataReader _empIddr = null;
           using (OracleCommand _cmdEmpid = new OracleCommand())
           {
                try
                {
                    string _sqlEmpid = "SELECT EMPLOYEE_ID FROM VW_PEOPLE_CURRENT WHERE UPPER(EMPLOYEE_NAME) LIKE UPPER(:Empname)";
                    _cmdEmpid.Parameters.Add(":Empname", OracleDbType.Varchar2).Value = empname.Trim();
                    using (_empIddr = objData.GetReader(_sqlEmpid, _cmdEmpid))
                    {
                        while (_empIddr.Read())
                        {
                            _empId = Convert.ToInt32(FixDBNull(_empIddr[0]));

                        }
                    }
                }
       
                catch { _empId = 0; }
                finally
                {
                    _empIddr.Close();
                    _cmdEmpid.Dispose();
                }
            }
           
           return _empId;
       }

       public bool IsEmpIDValid(string empId)
       {
           bool _isValid = false;
           int _count =0 ;
            using (OracleCommand _cmdEmpid = new OracleCommand())
            {
                string _sqlEmp = "SELECT COUNT(*) FROM VW_PEOPLE_CURRENT WHERE EMPLOYEE_ID = :EmpID";
                _cmdEmpid.Parameters.Add(":EmpID", OracleDbType.Varchar2).Value = empId;
                using (OracleDataReader _drEmp = objData.GetReader(_sqlEmp, _cmdEmpid))
                {
                    if (_drEmp.HasRows)
                    {
                        while (_drEmp.Read())
                        {
                            _count = Convert.ToInt32(_drEmp[0]);
                        }
                    
                    }
                    else
                    {
                        _count = 0;
                    }
                }
            }
            _isValid = (_count > 0) ? true : false;
            return _isValid;
       }

       public string GetEmpname(string empId)
        {
           string _empName = "";
            string _sqlName = "SELECT EMPLOYEE_NAME FROM VW_PEOPLE_CURRENT WHERE EMPLOYEE_ID = :EmpId";
           using (OracleCommand  _cmd = new OracleCommand())
           {
               _cmd.Parameters.Add(":EmpId", OracleDbType.Varchar2).Value = empId;
               try
               {
                   using (OracleDataReader _drempname = objData.GetReader(_sqlName, _cmd))
                   {
                       while (_drempname.Read())
                       {
                           _empName = FixDBNull(_drempname[0]).ToString();
                       }
                   }
               }
               catch (Exception e)
               {
                   Log.Error("Getting Employee name failed with the following exception " + e);
                   _empName = "";
               }
               
               return _empName;
           }
       }

       public string GetFullName(string name)
       {
           string[] mAr = null;
           string temp = null;

           name = name.Replace(",", "");

           mAr = name.Split(' ');
           temp = (mAr[1]) + " " + (mAr[0]);
           return temp;

       }

       public bool tryParse(string input)
       {

           int myIntNumber;




           if (int.TryParse(input, out myIntNumber) == true)
           {

               return true;

           }
           else { return false; }
       }

       public string  ReplaceWordChars(string text)
        {
            var s = text;
            // smart single quotes and apostrophe
            s = Regex.Replace(s, "[\u2018\u2019\u201A]", "'");
            // smart double quotes
            s = Regex.Replace(s, "[\u201C\u201D\u201E]", "\"");
            // ellipsis
            s = Regex.Replace(s, "\u2026", "...");
            // dashes
            s = Regex.Replace(s, "[\u2013\u2014]", "-");
            // circumflex
            s = Regex.Replace(s, "\u02C6", "^");
            // open angle bracket
            s = Regex.Replace(s, "\u2039", "<");
            // close angle bracket
            s = Regex.Replace(s, "\u203A", ">");
            // spaces
            s = Regex.Replace(s, "[\u02DC\u00A0]", "&nbsp;");
            //bullets
            s = Regex.Replace(s, "\u2022", "&bull;");
            //return carriage return with line
            s = Regex.Replace(s, "\r", "<br />");
            //replace tab with nbsp;
            s = Regex.Replace(s, "\t", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
            return s;
        }

       public string ReplaceSC(string text)
       {
            text = text.Replace("<", "");
            text = text.Replace(">", "");

            return text;
       }


    }

    public class Employee
    {
        public string EmpName { get; set; }
        public string Email { get; set; }
        public string Supervisor { get; set; }
        public string BadgeId { get; set; }

        Data.Data_Util objData = new Data.Data_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        protected static readonly ILog Log = LogManager.GetLogger(typeof(Employee));

        public bool GetEmpDetails(string empid)
        {
            bool _result = GetEmpInfo(empid);
             BadgeId =  GetBadgeId(empid);
             return _result;
        }

        public bool GetEmpInfo(string empid)
        {               
            using (OracleCommand _cmd = new OracleCommand())
            {
                string _sqlSelect = "SELECT EMPLOYEE_NAME, EPO, Supervisor_ID FROM VW_PEOPLE_CURRENT WHERE EMPLOYEE_ID = :Empid";
                _cmd.Parameters.Add(":Empid", OracleDbType.Varchar2).Value = empid;
                
                using (OracleDataReader _drEmp =  objData.GetReader(_sqlSelect, _cmd))
                {
                     try
                     {
                         while (_drEmp.Read())
                         {
                             EmpName = (string)(objCommon.FixDBNull(_drEmp["EMPLOYEE_NAME"]));
                             Email = (string)(objCommon.FixDBNull(_drEmp["EPO"]));
                             Supervisor = (string)(objCommon.FixDBNull(_drEmp["Supervisor_ID"]));                             
                         }
                            return true;
                     }
                     catch (Exception e)
                     {
                        Log.Error("Employee Info failed with: " + e);
                        return false;
                    }
                }

            }           
        }

        public string GetBadgeId(string empid)
        {
            using (OracleCommand _cmd = new OracleCommand())
            {
                //This query will handle all the badges with atleast one return date being null - latest badge which is not returned.
                //if there is no rows with return date null basically means the employee is gone, need to check with mike if needed to display based on this issue
                string _sqlSelect = "select Badge_number from site_security.badges where badge_id in (SELECT badge_id FROM (SELECT date_issued, badge_id, person_id  FROM site_security.badges WHERE badge_number IS NOT NULL AND return_date IS NULL ORDER BY date_issued DESC, badge_id DESC) where rownum=1 and person_id=:Empid)";
                _cmd.Parameters.Add(":Empid", OracleDbType.Varchar2).Value = empid;

                using (OracleDataReader _drBadge = objData.GetReader(_sqlSelect, _cmd))
                {
                    try
                    {
                        if (_drBadge.HasRows)
                        {
                            while (_drBadge.Read())
                            {
                                BadgeId = (string)(objCommon.FixDBNull(_drBadge["Badge_number"]));

                            }
                        }
                        else { BadgeId = ""; }
                        
                        return BadgeId;
                    }
                    catch (Exception e)
                    {
                        Log.Error("Badge Info failed with: " + e);
                        return "";
                    }
                }

            }           

        }
    }


}