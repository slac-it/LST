//$Header:$
//
//  UserRoles.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the utility class that has different user roles and their access
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
//using System.Data.OracleClient;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using log4net;
using LST.SSO;

namespace LST.Business
{
    public class UserRoles
    {
        Data.Data_Util objData = new Data.Data_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        Data.DML_Util objDml = new Data.DML_Util();
        SSO_Util objSSO = new SSO_Util();
        protected static readonly ILog Log = LogManager.GetLogger(typeof(UserRoles));


        public enum UserType
        {
            LSO,
            DLSO,
            SLSO,
            ActingSLSO,
            COSLSO,
            ADMIN,
            ADMINSVR,
            ALTSVR,
            ALTSLSO,
            ALTLSO,
            PRGMGR,
            ESHCOORD
        }

   

        public bool IsLSO  { get; set; }
        public bool IsDLSO { get; set; }
        public bool IsSLSO { get; set; }
        public bool IsActSLSO { get; set; }
        public bool IsCoSLSO { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAdminSvr { get; set; }
        public bool IsAltSVR { get; set; }
        public bool IsAltSLSO { get; set; }
        public bool IsSLSOGen { get; set; } //can be SLSO, co-SLSO, acting with no facility associated
        public bool IsAltSVRGen { get; set; }
        public bool IsAltSLSOGen { get; set; }
        public bool IsSLSOnofac { get; set; } //SLSO of any facility not with any particular one
        public bool IsAltLSO { get; set; }
        public bool IsAdminSvrGen { get; set; }
        public bool IsProgramMgr { get; set; }   //One program manger per facility
        public bool IsESHCoordinator { get; set; } //One ESH coordinator per facility
        public bool IsPrgMgrGen { get; set; }
        public bool IsEshCoordGen { get; set; }


       
        public void GetUserRole(string _userId, int _facilityId, int _workerslacid = 0)
        {
            Log.Info("GetUserRole: " + _userId + " Facility: " + _facilityId);

            string _strUser = @"Select L.Lookup_Desc As Roletype, Ur.User_Role_Id From Lst_User_Roles Ur Left Join Lst_Lookup L On L.Lookup_Id = Ur.Role_Type_Id
                                            WHERE ur.IS_ACTIVE= 'Y' AND ur.SLAC_ID = :UserId";
            IsLSO = IsDLSO = IsSLSO = IsActSLSO = IsCoSLSO = IsAdmin = IsAdminSvr = IsAltSVR = IsAltSLSO = IsSLSOGen = IsAltSVRGen = IsAltSLSOGen = IsSLSOnofac = IsAltLSO = IsProgramMgr = IsESHCoordinator = false;
            using (OracleCommand _cmdUser = new OracleCommand())
            {
                _cmdUser.CommandType = CommandType.Text;
                _cmdUser.Parameters.Add("UserId", _userId);
                _cmdUser.CommandText = _strUser;

                using (DataSet _dsUser = objData.ReturnDataset(_strUser,"roles",_cmdUser))
                {
                    DataTable _dtRoles = _dsUser.Tables["roles"];
                    for(int i=0;i<_dtRoles.Rows.Count;i++)
                    {
                        string _type;
                        int _roleId;
                        _type = _dtRoles.Rows[i][0].ToString();
                        _roleId = Convert.ToInt32(_dtRoles.Rows[i][1]);
                        if (_type.Equals(UserType.LSO.ToString()))
                        {
                            IsLSO = true;
                        }
                        if (_type.Equals(UserType.DLSO.ToString()))
                        {
                            IsDLSO = true;
                        }
                        if (_type.Equals(UserType.ADMIN.ToString()))
                        {
                            IsAdmin = true;
                        }
                        if (_type.Equals(UserType.ALTLSO.ToString()))
                        {
                            IsAltLSO = CheckifinRole(Convert.ToInt32(_userId), UserType.ALTLSO,0,0);
                        }
                        if (_type.Equals(UserType.SLSO.ToString()))
                        {
                            IsSLSOGen = true;
                            if ((IsSLSOGen) && (_facilityId == 0))
                            {
                                IsSLSOnofac = CheckifinRole(_roleId, 0, UserType.SLSO);
                            }
                            // else IsSLSOnofac = false;
                            if ((IsSLSOGen) && (_facilityId != 0))
                            {
                                IsSLSO = CheckifinRole(_roleId, _facilityId, UserType.SLSO);
                            }
                            //else IsSLSO = false;
                            if ((IsSLSOGen) && (_facilityId != 0))
                            {
                                IsActSLSO = CheckifinRole(_roleId, _facilityId, UserType.ActingSLSO);
                            }
                            //else IsActSLSO = false;
                            if ((IsSLSOGen) && (_facilityId != 0))
                            {
                                IsCoSLSO = CheckifinRole(_roleId, _facilityId, UserType.COSLSO);
                            }
                            //else IsCoSLSO = false;
                      
                        }
                                            
                    }
                }
              
            }
            IsAltSLSOGen = CheckifinRole(Convert.ToInt32(_userId), UserType.ALTSLSO, 0, 0);
            IsAltSVRGen = CheckifinRole(Convert.ToInt32(_userId), UserType.ALTSVR, 0, 0);
            IsPrgMgrGen = CheckifinRole(Convert.ToInt32(_userId), UserType.PRGMGR, 0, 0);
            IsEshCoordGen = CheckifinRole(Convert.ToInt32(_userId), UserType.ESHCOORD, 0, 0);

            if (_facilityId != 0)
            {
                IsAltSLSO = CheckifinRole(Convert.ToInt32(_userId), UserType.ALTSLSO, _facilityId, 0);
                IsProgramMgr = CheckifinRole(Convert.ToInt32(_userId), UserType.PRGMGR, _facilityId, 0);
                IsESHCoordinator = CheckifinRole(Convert.ToInt32(_userId), UserType.ESHCOORD,_facilityId,0 );
            }
            IsAdminSvrGen = CheckifinRole(Convert.ToInt32(_userId), 0);
            if (_workerslacid != 0)
            {
                IsAdminSvr = CheckifinRole(Convert.ToInt32(_userId), _workerslacid);
                IsAltSVR = CheckifinRole(Convert.ToInt32(_userId), UserType.ALTSVR, 0, _workerslacid);
            }
        }

        //Check if the Logged In user is SLSO, Acting SLSO, CO-SLSO for the Facility that is requested to work on by LCA Worker/QLO
        public Boolean CheckifinRole(int roleId, int facilityId, UserType userType)
        {
            StringBuilder _sbSql = new StringBuilder();
            int _count = 0;
            if (facilityId != 0)
            {
                _sbSql.Append("SELECT COUNT(*) FROM LST_FACILITY WHERE FACILITY_ID = :FacilityId AND IS_ACTIVE='Y'");
            }
            else { _sbSql.Append("SELECT COUNT(*) FROM LST_FACILITY WHERE IS_ACTIVE='Y'"); }

            if (userType.Equals(UserType.SLSO))
            { _sbSql.Append(" AND SLSO = :roleId"); }
            else if (userType.Equals(UserType.ActingSLSO))
            { _sbSql.Append(" AND ACTING_SLSO = :roleId"); }
            else if (userType.Equals(UserType.COSLSO))
            {
                _sbSql.Append(" AND (CO_SLSO1 = :roleId)");
            }
            

            using (OracleCommand _cmdUserrole = new OracleCommand())
            {
                if (facilityId != 0) { _cmdUserrole.Parameters.Add(":FacilityId", OracleDbType.Int32).Value = facilityId; }
                _cmdUserrole.Parameters.Add(":roleId", OracleDbType.Int32).Value = roleId;
                using (OracleDataReader _drRole = objData.GetReader(_sbSql.ToString(), _cmdUserrole))
                {
                    while (_drRole.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drRole[0]));
                    }
                }
            }
            return (_count > 0) ? true : false;
        }

      
        //Check if the Loggedin User is the Supervisor for the QLO / LCA Worker
        public Boolean CheckifinRole(int userId, int workerSlacId)
        {
            string _sqlSVR;
            int _count = 0;
          

            using (OracleCommand _cmdSvr = new OracleCommand())
            {
                if (workerSlacId != 0)
                {
                    _sqlSVR = "SELECT COUNT(*) FROM VW_PEOPLE_CURRENT WHERE SUPERVISOR_ID = :UserId AND EMPLOYEE_ID = :WorkerSlacId ";
                    _cmdSvr.Parameters.Add(":WorkerSlacId", OracleDbType.Int32).Value = workerSlacId;
                }
                else
                {
                    _sqlSVR = "SELECT COUNT(*) FROM VW_PEOPLE_CURRENT WHERE SUPERVISOR_ID = :UserId ";
                }
           
                _cmdSvr.Parameters.Add(":UserId", OracleDbType.Int32).Value = userId;
                
                using (OracleDataReader _drSvr = objData.GetReader(_sqlSVR, _cmdSvr))
                {
                    while (_drSvr.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drSvr[0]));
                    }
                }
            }
            return (_count > 0) ? true : false;
        }

        //Check if the Loggedin User is the Alternate for Supervisor or SLSO based on userid
        //Also check if the logged in user is a program manager or eshcoordinator for a facility
        public Boolean CheckifinRole(int slacId, UserType userType, int facilityId, int workerSlacId)
        {
            string _sqlAlt = "";
            int _count = 0;
            
            using (OracleCommand _cmdAlt = new OracleCommand())
            {
                if (userType.Equals(UserType.ALTSLSO))
                {
                    if (facilityId != 0)
                    {
                        _sqlAlt = "SELECT COUNT(*) FROM LST_FACILITY WHERE ALTERNATE_SLSO = :SlacId and FACILITY_ID = :FacilityId and trunc(ALTSLSO_TO) >= TRUNC(SYSDATE) AND IS_ACTIVE='Y'";
                        _cmdAlt.Parameters.Add(":FacilityId", OracleDbType.Int32).Value = facilityId;
                    }
                    else
                    {
                        _sqlAlt = "SELECT COUNT(*) FROM LST_FACILITY WHERE ALTERNATE_SLSO = :SlacId  and trunc(ALTSLSO_TO) >= TRUNC(SYSDATE) AND IS_ACTIVE='Y'";
                    }
                   
                }
                else if (userType.Equals(UserType.ALTSVR))
                {
                    if (workerSlacId != 0)
                    {
                        _sqlAlt = "SELECT COUNT(*) FROM LST_WORKER WHERE ALTERNATE_SVR = :SlacId AND SLAC_ID = :WorkerslacId and trunc(ALTSVR_TO) >= TRUNC(SYSDATE) AND IS_ACTIVE='Y'";
                        _cmdAlt.Parameters.Add(":WorkerSlacId", OracleDbType.Int32).Value = workerSlacId;
                    }
                    else
                    {
                        _sqlAlt = "SELECT COUNT(*) FROM LST_WORKER WHERE ALTERNATE_SVR = :SlacId and trunc(ALTSVR_TO) >= TRUNC(SYSDATE) AND IS_ACTIVE='Y'";
                    }
                }
                else if (userType.Equals(UserType.ALTLSO))
                {
                    _sqlAlt = "SELECT COUNT(*) FROM LST_USER_ROLES WHERE SLAC_ID = :SlacId and trunc(ALTLSO_TO) >= TRUNC(SYSDATE) AND IS_ACTIVE='Y' AND ROLE_TYPE_ID=20";
                }
                else if (userType.Equals(UserType.PRGMGR))
                {
                    if (facilityId != 0)
                    {
                        _sqlAlt = "SELECT COUNT(*) FROM LST_FACILITY WHERE PRGMGR = :SlacId and FACILITY_ID = :FacilityId and IS_ACTIVE='Y'";
                        _cmdAlt.Parameters.Add(":FacilityId", OracleDbType.Int32).Value = facilityId;
                    }
                    else
                    {
                        _sqlAlt = "SELECT COUNT(*) FROM LST_FACILITY WHERE PRGMGR = :SlacId  and IS_ACTIVE='Y'";
                    }
                }
                else if (userType.Equals(UserType.ESHCOORD))
                {
                    if (facilityId != 0)
                    {
                        _sqlAlt = "SELECT COUNT(*) FROM LST_FACILITY WHERE ESH_COORD = :SlacId and FACILITY_ID = :FacilityId and IS_ACTIVE='Y'";
                        _cmdAlt.Parameters.Add(":FacilityId", OracleDbType.Int32).Value = facilityId;
                    }
                    else
                    {
                        _sqlAlt = "SELECT COUNT(*) FROM LST_FACILITY WHERE ESH_COORD = :SlacId  and IS_ACTIVE='Y'";
                    }
                }
                _cmdAlt.Parameters.Add(":SlacId", OracleDbType.Int32).Value = slacId;
                    
                using (OracleDataReader _drAlt = objData.GetReader(_sqlAlt, _cmdAlt))
                {
                    if (_drAlt.HasRows)
                    {
                        while (_drAlt.Read())
                        {
                            _count = Convert.ToInt32(objCommon.FixDBNull(_drAlt[0]));
                        }
                    }
                    else _count = 0;
                   
                }
            }
            return (_count > 0) ? true : false;
            
        }

    
        public bool CheckAccessibility()
        {
            if (HttpContext.Current.Session["LoginSID"] != null)
            {
                GetUserRole(HttpContext.Current.Session["LoginSID"].ToString(), 0);
            }
            else
            {
                GetUserRole(objSSO.LoginSID, 0);
            }


            if ((IsLSOrAlt()) || (IsDLSO)  || (IsAdmin))
            { return true; }
            else
            { return false; }
        }

        public bool CheckAccessibility(int facId)
        {
            if (HttpContext.Current.Session["LoginSID"] != null)
            {
                GetUserRole(HttpContext.Current.Session["LoginSID"].ToString(), facId);
            }
            else
            {
                GetUserRole(objSSO.LoginSID, facId);
            }


            if ((IsLSOrAlt()) || (IsDLSO) || (IsAdmin) || (IsActSLSO) || (IsAltSLSO) || (IsSLSO) || (IsCoSLSO) || (IsProgramMgr) || (IsESHCoordinator))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        

        public bool CheckIfSLSO(int facId)
        {
            StringBuilder _sbSql = new StringBuilder();
            int _count = 0;
            string loginSID = String.Empty;
            var sID = HttpContext.Current.Session["LoginSID"];
            if (sID == null)
            {
                loginSID = objSSO.LoginSID;
            }
            else
            {
                loginSID = sID.ToString();
            }


            int _userRoleId = objDml.GetUserRoleId(loginSID, objDml.GetRoleId("SLSO"));
            _sbSql.Append("SELECT COUNT(*) FROM LST_FACILITY WHERE FACILITY_ID = :FacilityId AND IS_ACTIVE='Y' ");
            _sbSql.Append(" AND ((SLSO = :RoleId) OR (ACTING_SLSO = :RoleId) OR (CO_SLSO1 = :RoleId) OR (ALTERNATE_SLSO= :SlacId AND TRUNC(ALTSLSO_TO) >= TRUNC(SYSDATE)))");
      
            using (OracleCommand _cmdUserrole = new OracleCommand())
            {
                _cmdUserrole.Parameters.Add(":FacilityId", OracleDbType.Int32).Value = facId;
                _cmdUserrole.Parameters.Add(":RoleId", OracleDbType.Int32).Value = _userRoleId;
                _cmdUserrole.Parameters.Add(":SlacId", OracleDbType.Varchar2).Value = loginSID;
                using (OracleDataReader _drRole = objData.GetReader(_sbSql.ToString(), _cmdUserrole))
                {
                    while (_drRole.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drRole[0]));
                    }
                }
            }
            return (_count > 0) ? true : false;
        }

        public bool IsLSOrAlt()
        {

            if ((IsLSO) || (IsAltLSO))
                return true;
            else return false;
        }

        //No particular facility.. can be for any facility
        public bool CheckIfSLSO()
        {
            StringBuilder _sbSql = new StringBuilder();
            int _count = 0;

            string loginSID = String.Empty;
            var sID = HttpContext.Current.Session["LoginSID"];
            if (sID == null)
            {
                loginSID = objSSO.LoginSID;
            }
            else
            {
                loginSID = sID.ToString();
            }



            int _userRoleId = objDml.GetUserRoleId(loginSID, objDml.GetRoleId("SLSO"));
            _sbSql.Append("SELECT COUNT(*) FROM LST_FACILITY WHERE  IS_ACTIVE='Y' ");
            _sbSql.Append(" AND ((SLSO = :RoleId) OR (ACTING_SLSO = :RoleId) OR (CO_SLSO1 = :RoleId) OR (ALTERNATE_SLSO= :SlacId AND TRUNC(ALTSLSO_TO) >= TRUNC(SYSDATE)))");

            using (OracleCommand _cmdUserrole = new OracleCommand())
            {
                _cmdUserrole.Parameters.Add(":RoleId", OracleDbType.Int32).Value = _userRoleId;
                _cmdUserrole.Parameters.Add(":SlacId", OracleDbType.Varchar2).Value = loginSID;
                using (OracleDataReader _drRole = objData.GetReader(_sbSql.ToString(), _cmdUserrole))
                {
                    while (_drRole.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drRole[0]));
                    }
                }
            }
            return (_count > 0) ? true : false;
        }

    
    }
}