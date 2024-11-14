//$Header:$
//
//  DML_Util.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the utility function that does DML / CRUD operations against the database
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
//using System.Data.OracleClient;
using System.Text;
using Oracle.ManagedDataAccess.Client;

namespace LST.Data
{
    public class DML_Util
    {
        Data.Data_Util objData = new Data.Data_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        DataSet _dsList = new DataSet();



        public DataSet GetRoomNos(string bldgNo)
        {
            string _sqlBldg = "";
            DataSet _dsBldg = new DataSet();

            using (OracleCommand _cmdBldg = new OracleCommand())
            {
                _sqlBldg = "SELECT ROOM_NUMBER FROM ROOMS WHERE BUILDING_NUMBER = :Bldg ORDER BY ROOM_NUMBER";
                try
                {
                    _cmdBldg.Parameters.Add(":Bldg",OracleDbType.Varchar2).Value = bldgNo;
                    _dsBldg = objData.ReturnDataset(_sqlBldg, "room", _cmdBldg);
                }
                // catch (Exception ex)
                //log
                finally
                {
                    _dsBldg.Dispose();

                }
                return _dsBldg;
            }
        }

        public bool CheckIfIdExists(int objId, string objType)
        {
            string _sqlCheck = "";
            int _count = 0;

            if (objType.Equals("fac"))
            {
                _sqlCheck = "SELECT COUNT(*) FROM LST_FACILITY WHERE FACILITY_ID = :ObjId AND IS_ACTIVE='Y'";
            }
            else if (objType.Equals("worker"))
            {
                _sqlCheck = "SELECT COUNT(*) FROM LST_WORKER WHERE WORKER_ID = :ObjId AND IS_ACTIVE='Y'";
            }
            else if (objType.Equals("map"))
            {
                _sqlCheck = "SELECT COUNT(*) FROM LST_WORKER_FACILITY_MAP WHERE MAP_ID = :ObjId AND IS_ACTIVE='Y'";
            }
            using (OracleCommand _cmdCheck = new OracleCommand())
            {
                _cmdCheck.Parameters.Add(":ObjId",  OracleDbType.Int32).Value = objId;
                using (OracleDataReader _drObj = objData.GetReader(_sqlCheck, _cmdCheck))
                {
                    while (_drObj.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drObj[0]));
                    }
                    if (_count > 0) return true;
                    else return false;
                }
            }
        }

        public bool CheckIfitHasChild(int objId, string objType)
        {
            string _sqlCheck = "";
            int _count = 0;


            using (OracleCommand _cmdCheck = new OracleCommand())
            {
                if (objType.Equals("fac"))
                {
                    _sqlCheck = "SELECT COUNT(*) FROM LST_WORKER_FACILITY_MAP WHERE FACILITY_ID = :ObjId AND IS_ACTIVE='Y' AND STATUS_ID <> 4";
                }
                else if (objType.Equals("worker"))
                {
                    _sqlCheck = "SELECT COUNT(*) FROM VW_PEOPLE_CURRENT PC INNER JOIN LST_WORKER W ON W.SLAC_ID = PC.EMPLOYEE_ID AND W.WORKER_ID = :ObjId";
                }
                else if (objType.Equals("slso"))
                {
                    int roleId = GetUserRoleId(objId.ToString(), 15);
                    _sqlCheck = @"SELECT COUNT(*) FROM LST_FACILITY WHERE (SLSO = :RoleId OR CO_SLSO1 = :RoleId OR  ACTING_SLSO = :RoleId OR 
                                     (ALTERNATE_SLSO = :objId and TRUNC(ALTSLSO_TO) >= TRUNC(SYSDATE))) and IS_ACTIVE='Y'";
                    _cmdCheck.Parameters.Add(":RoleId", OracleDbType.Int32).Value =  roleId;
                }
                _cmdCheck.Parameters.Add(":ObjId", OracleDbType.Int32).Value = objId;
                using (OracleDataReader _drObj = objData.GetReader(_sqlCheck, _cmdCheck))
                {
                    while (_drObj.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drObj[0]));
                    }
                    if (_count > 0) return true;
                    else return false;
                }
            }
        }

        public string GetName(int objId, string objType)
        {
            string _sqlName;
            string _name = "";
            if (objType == "fac")
            {
                _sqlName = "SELECT FACILITY_NAME FROM LST_FACILITY WHERE FACILITY_ID = :ObjId";
            }
            else if (objType == "worker")
            {
                _sqlName = "SELECT PC.EMPLOYEE_NAME FROM LST_WORKER W INNER JOIN VW_PEOPLE PC ON W.SLAC_ID = PC.EMPLOYEE_ID AND W.WORKER_ID = :ObjId";
            }
            else
            {
                _sqlName = "SELECT PC.EMPLOYEE_NAME FROM LST_USER_ROLES U INNER JOIN VW_PEOPLE PC ON U.SLAC_ID = PC.EMPLOYEE_ID AND U.ROLE_TYPE_ID = 15 AND U.SLAC_ID= :ObjId AND U.IS_ACTIVE='Y'";
            }
            using (OracleCommand _cmdName = new OracleCommand())
            {
                _cmdName.Parameters.Add(":ObjId", OracleDbType.Int32).Value = objId;
                using (OracleDataReader _drName = objData.GetReader(_sqlName, _cmdName))
                {

                    if (_drName.HasRows)
                    {
                        while (_drName.Read())
                        {
                            _name = objCommon.FixDBNull(_drName[0]);
 
                        }
                    }
                    return _name;
                }
            }

        }

        # region "Files"
        public string InsertFileData(int objId, string fileName, int fileSize, string contentType, Byte[] fileData, string uploadedBy, string objType, string docType)
        {
            string _attachmentId = "";
            string _sqlInsert = "";

            if (objType.Equals("Facility"))
            {
                _sqlInsert = "Insert into LST_FACILITY_ATTACHMENT(FACILITY_ID,FILE_NAME,FILE_SIZE,FILE_CONTENT_TYPE,FILE_DATA,UPLOADED_BY,UPLOADED_ON,IS_ACTIVE) VALUES" +
                  "(:id,:Filename,:Filesize,:Contenttype,:Filedata,:Uploadedby,:Uploadedon,:Active) Returning ATTACHMENT_ID INTO :Fileid";
            }
            else if (objType.Equals("Worker"))
            {
                _sqlInsert = "Insert into LST_WORKER_ATTACHMENT(WORKER_ID,FILE_NAME,FILE_SIZE,FILE_CONTENT_TYPE,FILE_DATA,UPLOADED_BY,UPLOADED_ON,IS_ACTIVE, DOCTYPE) VALUES" +
                 "(:id,:Filename,:Filesize,:Contenttype,:Filedata,:Uploadedby,:Uploadedon,:Active,:DocType) Returning ATTACHMENT_ID INTO :Fileid";
            }
            else if (objType.Equals("WorkFac"))
            {
                _sqlInsert = "Insert into LST_WORKERFACILITY_ATTACHMENT(MAP_ID,FILE_NAME,FILE_SIZE,FILE_CONTENT_TYPE,FILE_DATA,UPLOADED_BY,UPLOADED_ON,IS_ACTIVE, DOCTYPE) VALUES" +
                 "(:id,:Filename,:Filesize,:Contenttype,:Filedata,:Uploadedby,:Uploadedon,:Active,:DocType) Returning ATTACHMENT_ID INTO :Fileid";
            }
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand(_sqlInsert, _ocon))
                {
                    _ocmd.CommandType = CommandType.Text;

                    OracleParameter prmrid = new OracleParameter();
                    prmrid.ParameterName = ":id";
                    prmrid.OracleDbType = OracleDbType.Varchar2;
                    if (objId == -1)
                    {
                        prmrid.Value = System.DBNull.Value;

                    }
                    else
                    {
                        prmrid.Value = objId;
                    }



                    OracleParameter prmfilename = new OracleParameter();

                    prmfilename.ParameterName = ":Filename";
                    prmfilename.OracleDbType = OracleDbType.Varchar2;
                    prmfilename.Value = fileName;


                    OracleParameter prmfilesize = new OracleParameter();

                    prmfilesize.ParameterName = ":Filesize";
                    prmfilesize.OracleDbType =  OracleDbType.Varchar2;
                    prmfilesize.Value = fileSize;


                    OracleParameter prmcontent = new OracleParameter();
                    prmcontent.ParameterName = ":Contenttype";
                    prmcontent.OracleDbType = OracleDbType.Varchar2;
                    prmcontent.Value = contentType;


                    OracleParameter prmfiledata = new OracleParameter();

                    prmfiledata.ParameterName = ":Filedata";
                    prmfiledata.OracleDbType = OracleDbType.Blob;
                    prmfiledata.Value = fileData;

                    OracleParameter prmuploadby = new OracleParameter();
                    prmuploadby.ParameterName = ":Uploadedby";
                    prmuploadby.OracleDbType= OracleDbType.Varchar2;
                    prmuploadby.Value = uploadedBy;

                    OracleParameter prmuploadon = new OracleParameter();
                    prmuploadon.ParameterName = ":Uploadedon";
                    prmuploadon.OracleDbType = OracleDbType.Date;
                    prmuploadon.Value = DateTime.Now;

                    OracleParameter prmactive = new OracleParameter();
                    prmactive.ParameterName = ":active";
                    prmactive.OracleDbType = OracleDbType.Char;
                    prmactive.Value = 'Y';

                    OracleParameter prmdocType = new OracleParameter();
                    if (objType.Equals("Worker") || objType.Equals("WorkFac"))
                    {

                        prmdocType.ParameterName = ":DocType";
                        prmdocType.OracleDbType = OracleDbType.Varchar2;
                        prmdocType.Value = docType;
                    }

                    OracleParameter prmfileid = new OracleParameter();
                    prmfileid.ParameterName = ":Fileid";
                    prmfileid.OracleDbType = OracleDbType.Int32;
                    prmfileid.Direction = ParameterDirection.Output;


                    _ocmd.Parameters.Add(prmrid);
                    _ocmd.Parameters.Add(prmfilename);
                    _ocmd.Parameters.Add(prmfilesize);
                    _ocmd.Parameters.Add(prmcontent);
                    _ocmd.Parameters.Add(prmfiledata);
                    _ocmd.Parameters.Add(prmuploadby);
                    _ocmd.Parameters.Add(prmuploadon);
                    _ocmd.Parameters.Add(prmactive);
                    if (objType.Equals("Worker") || objType.Equals("WorkFac"))
                    {
                        _ocmd.Parameters.Add(prmdocType);
                    }
                    _ocmd.Parameters.Add(prmfileid);
                    _ocon.Open();


                    try
                    {
                        _ocmd.ExecuteNonQuery();
                        _attachmentId = _ocmd.Parameters[":Fileid"].Value.ToString();

                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        _attachmentId = "-1";
                    }

                    return _attachmentId;

                }


            }


        }

        public DataSet GetFileInfo(int objId, string objType)
        {
            DataSet _dsFile = new DataSet();
            string _sqlFile = "";


            using (OracleCommand _cmdFile = new OracleCommand())
            {
                if (objType.Equals("Facility"))
                {

                    _sqlFile = "SELECT ATTACHMENT_ID, FACILITY_ID, FILE_NAME,FILE_DATA,UPLOADED_BY, UPLOADED_ON FROM LST_FACILITY_ATTACHMENT WHERE IS_ACTIVE='Y'";
                    if (objId == -1)
                    {
                        _sqlFile += " AND FACILITY_ID IS NULL";
                    }
                    else
                    {
                        _sqlFile += " AND FACILITY_ID = :ObjId";
                        _cmdFile.Parameters.Add(":ObjId", objId);
                    }
                }
                else if (objType.Equals("Worker"))
                {
                    _sqlFile = "SELECT ATTACHMENT_ID, WORKER_ID, FILE_NAME,FILE_DATA,UPLOADED_BY, UPLOADED_ON FROM LST_WORKER_ATTACHMENT WHERE IS_ACTIVE='Y'";
                    if (objId == -1)
                    {
                        _sqlFile += " AND WORKER_ID IS NULL";
                    }
                    else
                    {
                        _sqlFile += " AND WORKER_ID = :ObjId";
                        _cmdFile.Parameters.Add(":ObjId", objId);
                    }
                }

                _dsFile = objData.ReturnDataset(_sqlFile, "file", _cmdFile);
                return _dsFile;
            }
        }

        public string DeleteAttachment(int attachmentId, string changedBy, string objType)
        {
            string _errorCode;
            using (OracleConnection _conAttach = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _cmdAttach = new OracleCommand())
                {
                    try
                    {

                        _cmdAttach.Connection = _conAttach;
                        _conAttach.Open();
                        if (objType.Equals("Facility"))
                        {
                            _cmdAttach.CommandText = "LST_FILEUPLOAD_PKG.Proc_Del_Facility_Attachment";
                        }
                        else if (objType.Equals("Worker"))
                        {
                            _cmdAttach.CommandText = "LST_FILEUPLOAD_PKG.Proc_Del_Worker_Attachment";
                        }
                        else if (objType.Equals("WorkFac"))
                        {
                            _cmdAttach.CommandText = "LST_FILEUPLOAD_PKG.Proc_Del_WorkerFac_Attachment";
                        }
                        _cmdAttach.CommandType = CommandType.StoredProcedure;

                        _cmdAttach.Parameters.Add("PI_ATTACHMENT_ID",  OracleDbType.Int32).Value = attachmentId;
                        _cmdAttach.Parameters["PI_ATTACHMENT_ID"].Direction = ParameterDirection.Input;

                        _cmdAttach.Parameters.Add("PI_CHANGED_BY", OracleDbType.Varchar2).Value = changedBy;
                        _cmdAttach.Parameters["PI_CHANGED_BY"].Direction = ParameterDirection.Input;

                        _cmdAttach.Parameters.Add("po_RETURN_CODE", OracleDbType.Int32);
                        _cmdAttach.Parameters["po_RETURN_CODE"].Direction = ParameterDirection.Output;

                        _cmdAttach.ExecuteNonQuery();



                        if (_cmdAttach.Parameters["po_RETURN_CODE"].Value.ToString() != "0")
                        {
                            _errorCode = _cmdAttach.Parameters["po_RETURN_CODE"].Value.ToString();
                        }
                        else
                        {
                            _errorCode = "0";
                        }

                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                        _errorCode = "-1";
                    }
                    return _errorCode;
                }

            }

        }

        public OracleDataReader GetFileInfoById(int attachmentId, string objType)
        {
            string _sqlFileInfo = "";
            using (OracleCommand _cmdattach = new OracleCommand())
            {
                if (objType == "Facility")
                {
                    _sqlFileInfo = "SELECT * FROM LST_FACILITY_ATTACHMENT WHERE ATTACHMENT_ID = :AttachmentId";
                }
                else if (objType == "Worker")
                {
                    _sqlFileInfo = "SELECT * FROM LST_WORKER_ATTACHMENT WHERE ATTACHMENT_ID = :AttachmentId";
                }
                else
                {
                    _sqlFileInfo = "SELECT * FROM LST_WORKERFACILITY_ATTACHMENT WHERE ATTACHMENT_ID = :AttachmentId";
                }
                _cmdattach.Parameters.Add(":AttachmentId", OracleDbType.Int32).Value = attachmentId;
                OracleDataReader _drFileInfo = objData.GetReader(_sqlFileInfo, _cmdattach);
                return _drFileInfo;
            }

        }

        public bool Checkif253PRAAttached(int workerId)
        {
            int _count = 0;
            string _sqlCount = "SELECT COUNT(*) FROM LST_WORKER_ATTACHMENT WHERE WORKER_ID = :WorkerId and DOCTYPE='253PRA' AND IS_ACTIVE='Y'";

            using (OracleCommand _cmdCount = new OracleCommand())
            {
                _cmdCount.Parameters.Add(":WorkerId", OracleDbType.Int32).Value = workerId;

                using (OracleDataReader _dr253pra = objData.GetReader(_sqlCount, _cmdCount))
                {
                    while (_dr253pra.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_dr253pra[0]));
                    }
                }
                return (_count > 0) ? true : false;
            }
        }

        public bool CheckifOJTAttached(int mapID)
        {
            int _count = 0;
            string _sqlCount = "SELECT COUNT(*) FROM LST_WORKERFACILITY_ATTACHMENT WHERE MAP_ID = :MapId and DOCTYPE='OJT' AND IS_ACTIVE='Y'";

            using (OracleCommand _cmdCount = new OracleCommand())
            {
                _cmdCount.Parameters.Add(":MapId", OracleDbType.Int32).Value = mapID;

                using (OracleDataReader _drOJT = objData.GetReader(_sqlCount, _cmdCount))
                {
                    while (_drOJT.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drOJT[0]));
                    }
                }
                return (_count > 0) ? true : false;
            }
        }

        public DataSet GetFileInfo(int workerId, int mapId)
        {
            DataSet _dsFile = new DataSet();
            string _sqlFile = "";


            using (OracleCommand _cmdFile = new OracleCommand())
            {

                _sqlFile = @"Select Attachment_Id,  File_Name, Uploaded_By, Uploaded_On,Doctype From Lst_Worker_Attachment Where Is_Active='Y'
                                AND DOCTYPE='253PRA' AND WORKER_ID= :WorkerId Union Select Attachment_Id,  File_Name,  Uploaded_By, Uploaded_On, 
                                Doctype From Lst_Workerfacility_Attachment WHERE IS_ACTIVE='Y' AND DOCTYPE='OJT' AND MAP_ID=:MapId";
                try
                {
                    _cmdFile.Parameters.Add(":WorkerId", OracleDbType.Int32).Value = workerId;
                    _cmdFile.Parameters.Add(":MapId", OracleDbType.Int32).Value = mapId;

                    _dsFile = objData.ReturnDataset(_sqlFile, "file", _cmdFile);
                }
                catch (Exception e)
                {
                    Log.Error(" Getting the file information for worker facility threw the following exception " + e);
                    return null;
                }
                return _dsFile;
            }
        }

      
        #endregion

        # region "Facility"

        public OracleCommand AddParametersFacility(Business.Facility objFac, OracleCommand _ocmd)
        {
            _ocmd.Parameters.Add("Pi_Facility_Name", OracleDbType.Varchar2).Value = objFac.FacilityName;
            _ocmd.Parameters["Pi_Facility_Name"].Direction = ParameterDirection.Input;

            if (objFac.Bldg == "") { _ocmd.Parameters.Add("Pi_Bldg",  OracleDbType.Varchar2).Value = System.DBNull.Value; }
            else { _ocmd.Parameters.Add("Pi_Bldg", OracleDbType.Varchar2).Value = objFac.Bldg; }
            _ocmd.Parameters["Pi_Bldg"].Direction = ParameterDirection.Input;

            if (objFac.Room == "") { _ocmd.Parameters.Add("Pi_Room",OracleDbType.Varchar2).Value = System.DBNull.Value; }
            else { _ocmd.Parameters.Add("Pi_Room", OracleDbType.Varchar2).Value = objFac.Room; }
            _ocmd.Parameters["Pi_Room"].Direction = ParameterDirection.Input;

            _ocmd.Parameters.Add("Pi_Location", OracleDbType.Varchar2).Value = objFac.OtherLocation;
            _ocmd.Parameters["Pi_Location"].Direction = ParameterDirection.Input;

            if (objFac.SLSO == 0) { _ocmd.Parameters.Add("Pi_Slso", OracleDbType.Int32).Value = System.DBNull.Value; }
            else { _ocmd.Parameters.Add("Pi_Slso", OracleDbType.Int32).Value = objFac.SLSO; }
            _ocmd.Parameters["Pi_Slso"].Direction = ParameterDirection.Input;

            if (objFac.ActSLSO == 0) { _ocmd.Parameters.Add("Pi_Acting_Slso", OracleDbType.Int32).Value = System.DBNull.Value; }
            else { _ocmd.Parameters.Add("Pi_Acting_Slso", OracleDbType.Int32).Value = objFac.ActSLSO; }
            _ocmd.Parameters["Pi_Acting_Slso"].Direction = ParameterDirection.Input;

            if (objFac.CoSLSO1 == 0) { _ocmd.Parameters.Add("Pi_Co_Slso1", OracleDbType.Int32).Value = System.DBNull.Value; }
            else { _ocmd.Parameters.Add("Pi_Co_Slso1", OracleDbType.Int32).Value = objFac.CoSLSO1; }
            _ocmd.Parameters["Pi_Co_Slso1"].Direction = ParameterDirection.Input;
            //_ocmd.Parameters.Add("Pi_Co_Slso2", OracleType.Number).Value = coSlso2;
            //_ocmd.Parameters["Pi_Co_Slso2"].Direction = ParameterDirection.Input;

            //_ocmd.Parameters.Add("Pi_Altslso", OracleType.Number).Value = altSlso;
            //_ocmd.Parameters["Pi_Altslso"].Direction = ParameterDirection.Input;

            //_ocmd.Parameters.Add("Pi_Altslso_From", OracleType.DateTime).Value = altSlsoFrom;
            //_ocmd.Parameters["Pi_Altslso_From"].Direction = ParameterDirection.Input;

            //_ocmd.Parameters.Add("Pi_Altslso_To", OracleType.DateTime).Value = altSlsoTo;
            //_ocmd.Parameters["Pi_Altslso_To"].Direction = ParameterDirection.Input;
            if (objFac.PMId == 0) { _ocmd.Parameters.Add("Pi_PM",  OracleDbType.Int32).Value = System.DBNull.Value; }
            else { _ocmd.Parameters.Add("Pi_PM",  OracleDbType.Int32).Value = objFac.PMId; }
            _ocmd.Parameters["Pi_PM"].Direction = ParameterDirection.Input;

            if (objFac.CoordId== 0) { _ocmd.Parameters.Add("Pi_Coord",  OracleDbType.Int32).Value = System.DBNull.Value; }
            else { _ocmd.Parameters.Add("Pi_Coord",  OracleDbType.Int32).Value = objFac.CoordId; }
            _ocmd.Parameters["Pi_Coord"].Direction = ParameterDirection.Input;


            _ocmd.Parameters.Add("Pi_Fac_Webpage", OracleDbType.Varchar2).Value = objFac.FacWebpage;
            _ocmd.Parameters["Pi_Fac_Webpage"].Direction = ParameterDirection.Input;

            if (objFac.SopRevisedDate.Equals(DateTime.MinValue)) { _ocmd.Parameters.Add("Pi_Soprevised_Date", OracleDbType.Date).Value = System.DBNull.Value; }
            else { _ocmd.Parameters.Add("Pi_Soprevised_Date", OracleDbType.Date).Value = objFac.SopRevisedDate; }
            _ocmd.Parameters["Pi_Soprevised_Date"].Direction = ParameterDirection.Input;

            if (objFac.ApprovalExpDate.Equals(DateTime.MinValue)) { _ocmd.Parameters.Add("Pi_Apprval_Exp_Date", OracleDbType.Date).Value = System.DBNull.Value; }
            else { _ocmd.Parameters.Add("Pi_Apprval_Exp_Date",  OracleDbType.Date).Value = objFac.ApprovalExpDate; }
            _ocmd.Parameters["Pi_Apprval_Exp_Date"].Direction = ParameterDirection.Input;

            _ocmd.Parameters.Add("Pi_Link_Text", OracleDbType.Varchar2).Value = objFac.LinkText;
            _ocmd.Parameters["Pi_Link_Text"].Direction = ParameterDirection.Input;

            _ocmd.Parameters.Add("Pi_Link_Url", OracleDbType.Varchar2).Value = objFac.LinkUrl;
            _ocmd.Parameters["Pi_Link_Url"].Direction = ParameterDirection.Input;

            return _ocmd;

        }

        public int CreateFacility(Business.Facility objFac)
        {
            int _facilityId;

            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.Proc_Ins_Facility";
                        _ocmd.CommandType = CommandType.StoredProcedure;


                        AddParametersFacility(objFac, _ocmd);

                        _ocmd.Parameters.Add("Pi_Created_By", OracleDbType.Varchar2).Value = objFac.CreatedBy;
                        _ocmd.Parameters["Pi_Created_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PO_FACILITY_ID",  OracleDbType.Int32).Direction = ParameterDirection.Output;

                        _ocmd.ExecuteNonQuery();



                        if (_ocmd.Parameters["PO_FACILITY_ID"].Value != null)
                        {
                            _facilityId = Convert.ToInt32(_ocmd.Parameters["PO_FACILITY_ID"].Value.ToString());
                            if (_facilityId <= 0)
                            {
                                Log.Error("Insert Facility failed with the following SQL Error code: " + _facilityId);
                                _facilityId = 0;
                            }
                        }
                        else
                        {
                            Log.Error("Insert Facility failed with no error code");
                            _facilityId = 0;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Insert Facility Failed with exception: " + e);
                        _facilityId = 0;
                    }
                }
            }
            return _facilityId;
        }

        public int UpdateFacility(Business.Facility objFac)
        {
            int _returncode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.Proc_Upd_Facility";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("Pi_Facility_Id", OracleDbType.Int32).Value = objFac.FacilityId;
                        _ocmd.Parameters["Pi_Facility_Id"].Direction = ParameterDirection.Input;

                        AddParametersFacility(objFac, _ocmd);

                        _ocmd.Parameters.Add("Pi_Modified_By", OracleDbType.Int32).Value = objFac.ModifiedBy;
                        _ocmd.Parameters["Pi_Modified_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PO_RETURN_CODE",  OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        //return code 0 is considered successful. If other than 0, it is a failure.
                        if (_ocmd.Parameters["PO_RETURN_CODE"].Value != null)
                        {
                            _returncode = Convert.ToInt32(_ocmd.Parameters["PO_RETURN_CODE"].Value.ToString());
                        }
                        else { _returncode = -1; }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Update Facility failed with the following exception: " + e);
                        _returncode = -1;
                    }
                }

            }
            return _returncode;
        }

        public DataSet GetFacilityInfo(OracleCommand cmdList, string filter, string searchText)
        {
             StringBuilder _sbFac = new StringBuilder();
            _sbFac.Append("SELECT FACILITY_ID, FACILITY_NAME, BLDG, ROOM,SLSONAME,ACTSLSONAME,APPROVAL_EXPIRY_DATE,COSLSONAME,FACILITY_WEBPAGE FROM VW_LST_FACILITY ");
            if (searchText != "")
            {

                _sbFac.Append(" WHERE LOWER(FACILITY_NAME) like :FacName" );
                cmdList.Parameters.Add(":FacName", OracleDbType.Varchar2).Value = "%" + searchText.ToLower() + "%";
               
            }
            _sbFac.Append(filter);
            _dsList = objData.ReturnDataset(_sbFac.ToString(), "fac", cmdList);
            return _dsList;
        }

        public Business.Facility GetFacilityDetails(int facilityId)
        {
            string _sqlFac = "";
            Business.Facility objFacility = new Business.Facility();

            _sqlFac = "SELECT * FROM VW_LST_FACILITY WHERE FACILITY_ID = :FacilityId";
            using (OracleCommand _cmdFac = new OracleCommand())
            {
                _cmdFac.Parameters.Add(":FacilityId", OracleDbType.Int32).Value = facilityId;
                using (OracleDataReader _drFac = objData.GetReader(_sqlFac, _cmdFac))
                {
                    while (_drFac.Read())
                    {
                        objFacility.FacilityId = Convert.ToInt32(_drFac["FACILITY_ID"]);
                        objFacility.FacilityName = objCommon.FixDBNull(_drFac["FACILITY_NAME"]);
                        objFacility.Bldg = objCommon.FixDBNull(_drFac["BLDG"]);
                        objFacility.Room = objCommon.FixDBNull(_drFac["ROOM"]);
                        objFacility.OtherLocation = objCommon.FixDBNull(_drFac["OTHER_LOCATION"]);
                        objFacility.SLSOName = objCommon.FixDBNull(_drFac["SLSONAME"]);
                        objFacility.ActSLSOName = objCommon.FixDBNull(_drFac["ACTSLSONAME"]);
                        objFacility.CoSLSOName = objCommon.FixDBNull(_drFac["COSLSONAME"]);
                        objFacility.AltSLSO = (_drFac["ALTERNATE_SLSO"] != System.DBNull.Value) ? Convert.ToInt32(_drFac["ALTERNATE_SLSO"]) : 0;
                        objFacility.AltSLSOFrom = (_drFac["ALTSLSO_FROM"] != System.DBNull.Value) ? Convert.ToDateTime(_drFac["ALTSLSO_FROM"]) : DateTime.MinValue;
                        objFacility.AltSLSOTo = (_drFac["ALTSLSO_TO"] != System.DBNull.Value) ? Convert.ToDateTime(_drFac["ALTSLSO_TO"]) : DateTime.MinValue;
                        objFacility.SopRevisedDate = (_drFac["SOP_REVISED_DATE"] != System.DBNull.Value) ? Convert.ToDateTime(_drFac["SOP_REVISED_DATE"]) : DateTime.MinValue;
                        objFacility.ApprovalExpDate = (_drFac["APPROVAL_EXPIRY_DATE"] != System.DBNull.Value) ? Convert.ToDateTime(_drFac["APPROVAL_EXPIRY_DATE"]) : DateTime.MinValue;
                        objFacility.FacWebpage = objCommon.FixDBNull(_drFac["FACILITY_WEBPAGE"]);
                        objFacility.LinkUrl = objCommon.FixDBNull(_drFac["LINK_URL"]);
                        objFacility.LinkText = objCommon.FixDBNull(_drFac["LINK_TEXT"]);
                        objFacility.SLSO = (_drFac["SLSO"] != System.DBNull.Value) ? Convert.ToInt32(_drFac["SLSO"]) : 0;
                        objFacility.ActSLSO = (_drFac["ACTING_SLSO"] != System.DBNull.Value) ? Convert.ToInt32(_drFac["ACTING_SLSO"]) : 0;
                        objFacility.CoSLSO1 = (_drFac["CO_SLSO1"] != System.DBNull.Value) ? Convert.ToInt32(_drFac["CO_SLSO1"]) : 0;
                        objFacility.PMName = (_drFac["PRGMGRNAME"] != System.DBNull.Value) ? _drFac["PRGMGRNAME"].ToString() : "";
                        objFacility.CoordName = (_drFac["COORDNAME"] != System.DBNull.Value) ? _drFac["COORDNAME"].ToString() : "";
                        objFacility.PMId = (_drFac["PRGMGR"] != System.DBNull.Value) ? Convert.ToInt32(_drFac["PRGMGR"]) : 0;
                        objFacility.CoordId = (_drFac["ESH_COORD"] != System.DBNull.Value) ? Convert.ToInt32(_drFac["ESH_COORD"]) : 0;
                        objFacility.SLSOSlacId = (_drFac["SLSOID"] != System.DBNull.Value) ? _drFac["SLSOID"].ToString() : "";
 
                    }
                    return objFacility;
                }
            }
        }

        public DataSet GetFacilityForSLSO(bool slsoonly)
        {

            using (OracleCommand _cmdfac = new OracleCommand())
            {
                int SlacId = Convert.ToInt32(objCommon.GetUserId());
                String _sqlFac = "";
                try
                {
                    if (slsoonly)
                    {
                        _sqlFac = @"Select Facility_Id, Facility_Name From Lst_Facility Where (Slso = (Select User_Role_Id from Lst_User_Roles Where Slac_Id =:slacId And Is_Active='Y' And Role_Type_Id =15))
                            Or (Alternate_Slso = :slacId And Trunc(Altslso_To) >= Trunc(Sysdate) And Is_Active='Y') order by facility_Name";
                        _cmdfac.Parameters.Add(":slacId",  OracleDbType.Int32).Value = SlacId;
                    }
                    else
                    {
                         _sqlFac = @"SELECT FACILITY_ID, FACILITY_NAME FROM LST_FACILITY WHERE IS_ACTIVE='Y' ORDER BY FACILITY_NAME";
                    }
                     
                   
                    _dsList = objData.ReturnDataset(_sqlFac, "facslso", _cmdfac);
                    return _dsList;
                }
                catch (Exception e)
                {
                    Log.Error("Facility list for SLSO failed with the following exception " + e);
                    return null;
                }

            }


        }

        public DateTime GetSOPReviewDate(int mapID)
        {
            DateTime _SOPRevised = DateTime.MinValue;
            string _sqlSOP = "select sop_revised_Date from Lst_facility where facility_Id = (select facility_id from lst_worker_facility_map where map_id = :MapId)";

            using (OracleCommand _cmdSOP = new OracleCommand())
            {
                _cmdSOP.Parameters.Add(":MapId",  OracleDbType.Int32).Value = mapID;

                using (OracleDataReader _drOJT = objData.GetReader(_sqlSOP, _cmdSOP))
                {
                    while (_drOJT.Read())
                    {
                        _SOPRevised = Convert.ToDateTime(objCommon.FixDBNull(_drOJT[0]));
                    }
                }
                return _SOPRevised;
            }
        }

        public string DeleteObject(int objId, string modifiedBy, string objType)
        {
            string _errorCode;
            using (OracleConnection _conDel= new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _cmdDel = new OracleCommand())
                {
                    try
                    {

                        _cmdDel.Connection = _conDel;
                        _conDel.Open();
                        if (objType.Equals("fac"))
                        {
                            _cmdDel.CommandText = "LST_ADMINISTRATION_PKG.PROC_DEL_FACILITY";
                        }
                        else if (objType.Equals("worker"))
                        {
                            _cmdDel.CommandText = "LST_ADMINISTRATION_PKG.PROC_DEL_WORKER";
                        }
                        else if (objType.Equals("slso"))
                        {
                            _cmdDel.CommandText = "LST_ADMINISTRATION_PKG.PROC_DEL_USER";

                        }
                        else if (objType.Equals("userfac"))
                        {
                            _cmdDel.CommandText = "LST_ADMINISTRATION_PKG.PROC_DEL_USER_FACILITY";
                        }
                        _cmdDel.CommandType = CommandType.StoredProcedure;

                        if (objType.Equals("slso"))
                        {
                            _cmdDel.Parameters.Add("PI_ROLE_TYPE_ID", OracleDbType.Int32).Value = 15;
                            _cmdDel.Parameters["PI_ROLE_TYPE_ID"].Direction = ParameterDirection.Input;
                        }                   

                        _cmdDel.Parameters.Add("PI_OBJECT_ID",  OracleDbType.Int32).Value = objId;
                        _cmdDel.Parameters["PI_OBJECT_ID"].Direction = ParameterDirection.Input;

                        _cmdDel.Parameters.Add("Pi_Modified_By", OracleDbType.Varchar2).Value = modifiedBy;
                        _cmdDel.Parameters["Pi_Modified_By"].Direction = ParameterDirection.Input;

                        _cmdDel.Parameters.Add("po_RETURN_CODE", OracleDbType.Int32);
                        _cmdDel.Parameters["po_RETURN_CODE"].Direction = ParameterDirection.Output;

                        _cmdDel.ExecuteNonQuery();



                        if (_cmdDel.Parameters["po_RETURN_CODE"].Value.ToString() != "0")
                        {
                            _errorCode = _cmdDel.Parameters["po_RETURN_CODE"].Value.ToString();
                        }
                        else
                        {
                            _errorCode = "0";
                        }

                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                        _errorCode = "-1";
                    }
                    return _errorCode;
                }

            }

        }

        public int GetFacilityIdAR(int requestId)
        {
            string _sqlFacility = "SELECT FACILITY_ID FROM LST_FACILITY_REQUEST WHERE FAC_REQUEST_ID = :FacilityId ";
            int _facilityId = 0;

            using (OracleCommand _cmdFacility = new OracleCommand())
            {
                _cmdFacility.Parameters.Add(":FacilityId",  OracleDbType.Int32).Value = requestId;

                using (OracleDataReader _drFacility = objData.GetReader(_sqlFacility, _cmdFacility))
                {
                    while (_drFacility.Read())
                    {
                        _facilityId = Convert.ToInt32(objCommon.FixDBNull(_drFacility[0]));
                    }
                }
                return _facilityId;
            }

        }

        public int CreateFacilityRequest(Business.FacilityRequest objFacReq, string comments)
        {
            int _facRequestId;

            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_APPROVAL_PKG.Proc_Ins_FacApprovalrequest";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("Pi_Facility_Id",  OracleDbType.Int32).Value = objFacReq.FacilityId;
                        _ocmd.Parameters["Pi_Facility_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Sop_Completed", OracleDbType.Varchar2).Value = objFacReq.IsSOPCompleted;
                        _ocmd.Parameters["Pi_Sop_Completed"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Annual_Cert",  OracleDbType.Varchar2).Value = objFacReq.IsAnnualCertCompleted;
                        _ocmd.Parameters["Pi_Annual_Cert"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Comments", OracleDbType.Varchar2).Value = comments;
                        _ocmd.Parameters["Pi_Comments"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Created_By", OracleDbType.Varchar2).Value = objFacReq.CreatedBy;
                        _ocmd.Parameters["Pi_Created_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Po_Request_Id", OracleDbType.Int32).Direction = ParameterDirection.Output;

                        _ocmd.ExecuteNonQuery();

                        if (_ocmd.Parameters["Po_Request_Id"].Value != null)
                        {
                            _facRequestId = Convert.ToInt32(_ocmd.Parameters["Po_Request_Id"].Value.ToString());
                            if (_facRequestId <= 0)
                            {
                                Log.Error("Creating Approval Request for Facility failed with the following sql error: " +  _facRequestId);
                                _facRequestId = 0;
                            }
                        }
                        else
                        {
                            Log.Error("Creating Approval Request failed with no error code");
                            _facRequestId = 0;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Creating Approval request Failed with exception: " + e);
                        _facRequestId = 0;
                    }
                }
            }
            return _facRequestId;

        }

        public int UpdateFacApproval(Business.FacilityApproval objFacApproval, string sopApproved, string inspCompleted, DateTime newFacDate)
        {
            int _returnCode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_APPROVAL_PKG.Proc_Upd_FacApproval";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("Pi_Request_Id",  OracleDbType.Int32).Value = objFacApproval.RequestId;
                        _ocmd.Parameters["Pi_Request_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Approver_Type", OracleDbType.Varchar2).Value = objFacApproval.ApproverType;
                        _ocmd.Parameters["Pi_Approver_Type"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Approver_Id",  OracleDbType.Int32).Value = objFacApproval.ApproverId;
                        _ocmd.Parameters["Pi_Approver_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Status_Id",  OracleDbType.Int32).Value = objFacApproval.StatusId;
                        _ocmd.Parameters["Pi_Status_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Comments", OracleDbType.Varchar2).Value = objFacApproval.Comments;
                        _ocmd.Parameters["Pi_Comments"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_SOPRev_Approved", OracleDbType.Varchar2).Value = sopApproved;
                        _ocmd.Parameters["Pi_SOPRev_Approved"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Insp_Completed", OracleDbType.Varchar2).Value = inspCompleted;
                        _ocmd.Parameters["Pi_Insp_Completed"].Direction = ParameterDirection.Input;    

                        if (newFacDate != DateTime.MinValue)
                        {
                            _ocmd.Parameters.Add("Pi_New_Date", OracleDbType.Date).Value = newFacDate;
                        }
                        else
                        {
                            _ocmd.Parameters.Add("Pi_New_Date",  OracleDbType.Date).Value = System.DBNull.Value;
                        }
                        _ocmd.Parameters["Pi_New_Date"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Modified_by", OracleDbType.Varchar2).Value = objCommon.GetUserId();
                        _ocmd.Parameters["Pi_Modified_by"].Direction = ParameterDirection.Input;
                        
                        _ocmd.Parameters.Add("PO_RETURN_CODE",  OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        //return code 0 is considered successful. If other than 0, it is a failure.
                        if (_ocmd.Parameters["PO_RETURN_CODE"].Value != null)
                        {
                            _returnCode = Convert.ToInt32(_ocmd.Parameters["PO_RETURN_CODE"].Value.ToString());
                        }
                        else { _returnCode = -1; }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Update Facility Approval failed with the following exception: " + e);
                        _returnCode = -1;
                    }
                }
            }
            return _returnCode;
        }

        public bool CheckIfFacApprovalInProg(int facId)
        {
            string _sqlCheck = "";
            int _count = 0;


            using (OracleCommand _cmdCheck = new OracleCommand())
            {
              
                 _sqlCheck = "SELECT COUNT(*) FROM LST_FACILITY_REQUEST WHERE FACILITY_ID = :FacId AND IS_ACTIVE='Y' AND STATUS_ID = 2";

                 _cmdCheck.Parameters.Add(":FacId", facId);
                using (OracleDataReader _drObj = objData.GetReader(_sqlCheck, _cmdCheck))
                {
                    while (_drObj.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drObj[0]));
                    }
                    if (_count > 0) return true;
                    else return false;
                }
            }
        }

        public bool CheckIfFacApprovalReqInProg(int reqId)
        {
            string _sqlCheck = "";
            int _count = 0;


            using (OracleCommand _cmdCheck = new OracleCommand())
            {

                _sqlCheck = "SELECT COUNT(*) FROM LST_FACILITY_REQUEST WHERE FAC_REQUEST_ID = :ReqId AND IS_ACTIVE='Y' AND STATUS_ID = 2";

                _cmdCheck.Parameters.Add(":ReqId", reqId);
                using (OracleDataReader _drObj = objData.GetReader(_sqlCheck, _cmdCheck))
                {
                    while (_drObj.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drObj[0]));
                    }
                    if (_count > 0) return true;
                    else return false;
                }
            }
        }

        public OracleDataReader GetfacApprovalRequestDetails(string userType, int userRoleId, string slacId)
        {
            using (OracleCommand _cmdRequest = new OracleCommand())
            {
                StringBuilder _sbRequest = new StringBuilder();
                _sbRequest.Append(@"select R.FAC_REQUEST_ID, F.FACILITY_NAME, L.LOOKUP_DESC AS CURRENTSTATUS FROM LST_FACILITY_REQUEST R LEFT JOIN  LST_FACILITY F ON R.FACILITY_ID = F.FACILITY_ID
                                    LEFT JOIN LST_LOOKUP L ON L.LOOKUP_ID = R.STATUS_ID
                                    WHERE R.STATUS_ID =2 AND  L.LOOKUP_GROUP = 'Status' and R.IS_ACTIVE ='Y' ");
                if (userType == "SLSO")
                {
                    _sbRequest.Append(@" AND F.FACILITY_ID IN (SELECT FACILITY_ID FROM LST_FACILITY WHERE IS_ACTIVE ='Y' AND SLSO = :UserRoleId)
                                         AND R.FAC_REQUEST_ID IN (SELECT REQUEST_ID FROM LST_FAC_APPROVAL_WF WHERE APPROVER_TYPE='SLSO' AND STATUS_ID IN (2,3))");
                    _cmdRequest.Parameters.Add(":UserRoleId",  OracleDbType.Int32).Value = userRoleId;
                }
                else if (userType == "LSO")
                {
                    _sbRequest.Append(@" AND R.FAC_REQUEST_ID IN (SELECT REQUEST_ID FROM LST_FAC_APPROVAL_WF WHERE APPROVER_TYPE ='LSO' AND STATUS_ID = 2) ");
                }
                else if (userType == "PRGMGR")
                {
                    _sbRequest.Append(@"AND F.FACILITY_ID IN (SELECT FACILITY_ID FROM LST_FACILITY WHERE IS_ACTIVE ='Y' AND PRGMGR = :SlacId)
                                    AND R.FAC_REQUEST_ID IN (SELECT REQUEST_ID FROM LST_FAC_APPROVAL_WF WHERE APPROVER_TYPE='PRGMGR' AND STATUS_ID = 2) ");
                    _cmdRequest.Parameters.Add(":SlacId",  OracleDbType.Int32).Value = slacId;
                }
                else if (userType == "ESHCOORD")
                {
                    _sbRequest.Append(@"AND F.FACILITY_ID IN (SELECT FACILITY_ID FROM LST_FACILITY WHERE IS_ACTIVE ='Y' AND ESH_COORD = :SlacId)
                                    AND R.FAC_REQUEST_ID IN (SELECT REQUEST_ID FROM LST_FAC_APPROVAL_WF WHERE APPROVER_TYPE='ESHCOORD' AND STATUS_ID = 2) ");
                    _cmdRequest.Parameters.Add(":SlacId", OracleDbType.Int32).Value = slacId;
                }
                _sbRequest.Append(" order by R.FAC_REQUEST_ID ");

                try
                {
                    OracleDataReader _drRequest = objData.GetReader(_sbRequest.ToString(), _cmdRequest);
                    if (_drRequest.HasRows)
                    {
                        return _drRequest;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Fac approval request details for " + userType + " failed with the following exception " + e);
                    return null;
                }


            }
        }
        public bool IsFACApproved(string apprType, int reqId)
        {
            string _sqlAppwf;
            _sqlAppwf = "SELECT COUNT(*) FROM LST_FAC_APPROVAL_WF WHERE REQUEST_ID=:RequestId and APPROVER_TYPE = :ApproverType and STATUS_ID=3";
            int _count = 0;

            using (OracleCommand _cmdAppwf = new OracleCommand())
            {
                _cmdAppwf.Parameters.Add(":ApproverType", OracleDbType.Varchar2).Value = apprType;
                _cmdAppwf.Parameters.Add(":RequestId", OracleDbType.Int32).Value = reqId;
                using (OracleDataReader _drAppwf = objData.GetReader(_sqlAppwf, _cmdAppwf))
                {
                    if (_drAppwf.HasRows)
                    {
                        while (_drAppwf.Read())
                        {
                            _count = Convert.ToInt32(_drAppwf[0]);
                        }
                    }

                }
                return (_count > 0) ? true : false;

            }
        }

        public OracleDataReader GetFACApprovalHistory(int requestId)
        {
            using (OracleCommand _cmdAppHis = new OracleCommand())
            {
                string _sqlAppHis = @"Select Wf.FAC_Approval_Id, wf.APPROVER_TYPE, Wf.Approver_Id, Wf.Action_date, Wf.Status_Id, Wf.Comments, 
                                    pc.employee_name as Approver, lp.lookup_desc as status
                                    From Lst_FAC_Approval_WF  Wf
                                    Left Join Vw_People Pc On Wf.Approver_Id= Pc.employee_Id
                                    left join lst_lookup lp on lp.lookup_id = wf.status_id
                                    where wf.request_id=:RequestId";
                _cmdAppHis.Parameters.Add(":RequestId",  OracleDbType.Int32).Value = requestId;
                try
                {
                    OracleDataReader _drApphis = objData.GetReader(_sqlAppHis, _cmdAppHis);
                    if (_drApphis.HasRows)
                    {
                        return _drApphis;
                    }
                    else
                    {
                        return null;
                    }

                }
                catch (Exception e)
                {
                    Log.Error("FAC Approval History details failed with the following exception " + e);
                    return null;
                }

            }
        }

        public string GetRequestStatus(int requestId)
        {
            string _sqlStat = @"SELECT LOOKUP_dESC  FROM LST_FACILITY_REQUEST R
                                LEFT JOIN LST_LOOKUP L ON L.LOOKUP_ID = R.STATUS_ID
                                WHERE R.FAC_REQUEST_ID = :RequestId";
            string _status = "";
           using (OracleCommand _cmdStatus = new OracleCommand())
           {
               _cmdStatus.Parameters.Add(":RequestId", OracleDbType.Int32).Value = requestId;
               using (OracleDataReader _drRequest = objData.GetReader(_sqlStat, _cmdStatus))
               {
                   while (_drRequest.Read())
                   {
                       _status = objCommon.FixDBNull(_drRequest[0]);
                   }
                 
               }
               return _status;
           }

        }

        public string GetPivotArray(int facId)
        {
            string _pivotArray = "";
            string _fieldid = "";
            string _fieldname = "";
            StringBuilder _strhelp = new StringBuilder();
            using (OracleCommand _cmd = new OracleCommand() )
            {
                string _sqlpivot = @"select field_id,column_label from lst_ojt_fields where 
                                        field_id in (select field_id from LST_FIELDS_FACILITY_MAP
                                        WHERE FACILITY_ID = :FacilityId and is_active='Y')";
                _cmd.Parameters.Add(":FacilityId",  OracleDbType.Int32).Value = facId;
                using (OracleDataReader _dr = objData.GetReader(_sqlpivot, _cmd))
                {
                    if (_dr.HasRows)
                    {
                        while (_dr.Read())
                        {
                            _fieldid = objCommon.FixDBNull(_dr[0]);
                            _fieldname = objCommon.FixDBNull(_dr[1]);
                            _strhelp.Append("'");
                            _strhelp.Append(_fieldid);
                            _strhelp.Append("'");
                            _strhelp.Append(" as ");
                            _strhelp.Append("\"");
                            _strhelp.Append(_fieldname);
                            _strhelp.Append("\"");
                            _strhelp.Append(",");
                            _pivotArray = _strhelp.ToString();
                        }
                        string _pivotArrayfin = _pivotArray.Remove(_pivotArray.Length - 1, 1);
                        return _pivotArrayfin;
                    }
                    else return "";
                }
             
            }
        }

        public DataSet GetOJT(int _facId)
        {
            string _pivotarray = GetPivotArray(_facId);
            DataSet _ds = new DataSet();

            using (OracleCommand _cmdOJT = new OracleCommand())
            {

                string _sqlOJT ;
                //= @"select * from 
//                                    (
//                                      select fm.workerfac_mapid, p.name as Worker, 
//                                            decode(worker_type_id, 7, 'QLO', 8, 'LCA Worker') as WorkType,
//                                            to_char(fm.created_on,'DD/MM/YYYY') as DateUpdated,
//                                            fm.ojt_field_id, fm.fm_String_value
//                                            from lst_ojt_fieldmatrix fm left join lst_worker_facility_map m 
//                                            on fm.workerfac_mapid = m.map_id
//                                            left join lst_worker w on w.worker_id = m.worker_id
//                                            left join persons.person p on p.key = w.slac_id where fm.is_Active ='Y'
//                                            and m.facility_id = :FacId
//                                            )
// 
//                                      pIVOT (
//                                      max(FM_STRING_VALUE)
//                                      FOR OJT_FIELD_ID IN ( " + _pivotarray + "))  ";
                StringBuilder _sbQuery = new StringBuilder();
                _sbQuery.Append("select * from (select fm.workerfac_mapid, p.name as ");
                _sbQuery.Append("\"");
                _sbQuery.Append("Worker");
                _sbQuery.Append("\"");
                _sbQuery.Append(", decode(worker_type_id, 7, 'QLO', 8, 'LCA Worker') as ");
                _sbQuery.Append("\"");
                _sbQuery.Append("Work Type");
                _sbQuery.Append("\"");
                _sbQuery.Append(", to_char(m.latest_ojtupd_date,'MM/DD/YY') as ");
                _sbQuery.Append("\"");
                _sbQuery.Append("Date updated");
                _sbQuery.Append("\"");
                _sbQuery.Append(", fm.ojt_field_id, fm.fm_String_value  from lst_ojt_fieldmatrix fm left join lst_worker_facility_map m  on fm.workerfac_mapid = m.map_id");
                _sbQuery.Append("   left join lst_worker w on w.worker_id = m.worker_id left join persons.person p on p.key = w.slac_id where fm.is_Active ='Y' ");
                _sbQuery.Append(" and m.facility_id = :FacId and m.status_id <> 4 and m.is_active='Y' order by p.name )    pIVOT (max(FM_STRING_VALUE) FOR OJT_FIELD_ID IN (");
                _sbQuery.Append(_pivotarray);      
                _sbQuery.Append("))");
                _sbQuery.Append(" ORDER BY \"Worker\"");
                _sqlOJT = _sbQuery.ToString();
                _cmdOJT.Parameters.Add(":FacId", OracleDbType.Int32).Value = _facId;
                _ds = objData.ReturnDataset(_sqlOJT, "ojt", _cmdOJT);
                return _ds;
                //if (_drOJT.HasRows)
                //{
                //    return _drOJT;
                //}
                //else
                //{
                //    return null;
                //}
            }
        }

        public int GetFieldID(string colName)
        {
            int _fieldid = 0;
            using (OracleCommand _cmdfield = new OracleCommand())
            {
                string _sqlField ="SELECT FIELD_ID FROM LST_OJT_FIELDS WHERE LOWER(COLUMN_LABEL) = :ColName AND IS_ACTIVE='Y'";
                _cmdfield.Parameters.Add(":ColName",  OracleDbType.Varchar2).Value = colName.ToLower();
                using (OracleDataReader _dr = objData.GetReader(_sqlField, _cmdfield))
                {
                    if (_dr.HasRows)
                    {
                        while (_dr.Read())
                        {
                            _fieldid = Convert.ToInt32(objCommon.FixDBNull(_dr[0]));
                        }
                    }
                }
                return _fieldid;
            }

        }

        #endregion

        #region"Worker"
        public OracleDataReader GetWorkerLabInfo(string slacId)
        {

            OracleDataReader _drWorkerLab;
            string _sqlWorkerLab = "";
            using (OracleCommand _cmdWorklab = new OracleCommand())
            {
                _sqlWorkerLab = "SELECT * FROM VW_LST_WORKER_FACILITY WHERE SLAC_ID = :SlacId and status_id in (5,6)  AND IS_ACTIVE='Y'";
                _cmdWorklab.Parameters.Add(":SlacId", slacId);
                _drWorkerLab = objData.GetReader(_sqlWorkerLab, _cmdWorklab);

                return _drWorkerLab;
            }
        }

        public int UpdateOJT(int mapId, DateTime OJTCompletionDate, string modifiedBy)
        {
            int _returncode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.Proc_Upd_UserFacility_OJT";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("Pi_Map_Id",  OracleDbType.Int32).Value = mapId;
                        _ocmd.Parameters["Pi_Map_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Ojt_Date", OracleDbType.Date  ).Value = OJTCompletionDate;
                        _ocmd.Parameters["Pi_Ojt_Date"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Modified_By", OracleDbType.Varchar2).Value = modifiedBy;
                        _ocmd.Parameters["Pi_Modified_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PO_RETURN_CODE",  OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        //return code 0 is considered successful. If other than 0, it is a failure.
                        if (_ocmd.Parameters["PO_RETURN_CODE"].Value != null)
                        {
                            _returncode = Convert.ToInt32(_ocmd.Parameters["PO_RETURN_CODE"].Value.ToString());
                        }
                        else { _returncode = -1; }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Update OJT for User's Lab failed with the following exception: " + e);
                        _returncode = -1;
                    }
                }

            }
            return _returncode;
        }

        public int UpdateSOP(int mapId, DateTime SOPReviewDate, string modifiedBy)
        {
            int _returncode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.Proc_Upd_UserFacility_SOP";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("Pi_Map_Id",  OracleDbType.Int32).Value = mapId;
                        _ocmd.Parameters["Pi_Map_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_SOP_Date",  OracleDbType.Date).Value = SOPReviewDate;
                        _ocmd.Parameters["Pi_SOP_Date"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Modified_By", OracleDbType.Varchar2).Value = modifiedBy;
                        _ocmd.Parameters["Pi_Modified_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PO_RETURN_CODE", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        //return code 0 is considered successful. If other than 0, it is a failure.
                        if (_ocmd.Parameters["PO_RETURN_CODE"].Value != null)
                        {
                            _returncode = Convert.ToInt32(_ocmd.Parameters["PO_RETURN_CODE"].Value.ToString());
                        }
                        else { _returncode = -1; }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Update SOP for User's Lab failed with the following exception: " + e);
                        _returncode = -1;
                    }
                }

            }
            return _returncode;
        }

        public int CreateWorker(Business.Worker objWorker)
        {
            int _workerId;

            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.Proc_Ins_Worker";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("Pi_Slac_Id",  OracleDbType.Int32).Value = objWorker.SlacId;
                        _ocmd.Parameters["Pi_Slac_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Affiliation_Id",  OracleDbType.Int32).Value = objWorker.AffiliationId;
                        _ocmd.Parameters["Pi_Affiliation_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Preferred_Email", OracleDbType.Varchar2).Value = objWorker.PreferredEmail;
                        _ocmd.Parameters["Pi_Preferred_Email"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Eshmanual_Reviewed", OracleDbType.Varchar2).Value = objWorker.IsESHManualReviewed;
                        _ocmd.Parameters["Pi_Eshmanual_Reviewed"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Studreq_Reviewed", OracleDbType.Varchar2).Value = objWorker.IsStudReqReviewed;
                        _ocmd.Parameters["Pi_Studreq_Reviewed"].Direction = ParameterDirection.Input;

                        if (objWorker.AlternateSvr != "")
                        {
                            _ocmd.Parameters.Add("Pi_Alternate",  OracleDbType.Int32).Value = Convert.ToInt32(objWorker.AlternateSvr);
                        }
                        else
                        {
                            _ocmd.Parameters.Add("Pi_Alternate",  OracleDbType.Int32).Value = System.DBNull.Value;
                        }
                        _ocmd.Parameters["Pi_Alternate"].Direction = ParameterDirection.Input;

                        if (objWorker.AlternateSvrFrom != DateTime.MinValue)
                        {
                            _ocmd.Parameters.Add("Pi_Alt_From",  OracleDbType.Date).Value = objWorker.AlternateSvrFrom;
                        }
                        else
                        {
                            _ocmd.Parameters.Add("Pi_Alt_From",  OracleDbType.Date).Value = System.DBNull.Value;
                        }
                         _ocmd.Parameters["Pi_Alt_From"].Direction = ParameterDirection.Input;

                        if (objWorker.AlternateSvrTo != DateTime.MinValue)
                        {
                              _ocmd.Parameters.Add("Pi_Alt_To",  OracleDbType.Date).Value = objWorker.AlternateSvrTo;
                        }
                        else
                        {
                             _ocmd.Parameters.Add("Pi_Alt_To",  OracleDbType.Date).Value = System.DBNull.Value;
                        }
                      
                        _ocmd.Parameters["Pi_Alt_To"].Direction = ParameterDirection.Input;


                        _ocmd.Parameters.Add("Pi_Created_By",  OracleDbType.Varchar2).Value = objWorker.CreatedById;
                        _ocmd.Parameters["Pi_Created_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PO_WORKER_ID",  OracleDbType.Int32).Direction = ParameterDirection.Output;

                        _ocmd.ExecuteNonQuery();

                        if (_ocmd.Parameters["PO_WORKER_ID"].Value != null)
                        {
                            _workerId = Convert.ToInt32(_ocmd.Parameters["PO_WORKER_ID"].Value.ToString());
                            if (_workerId <= 0)
                            {
                                Log.Error("Insert Worker failed with the following sql error: " + _workerId);
                                _workerId = 0;
                            }
                        }
                        else
                        {
                            Log.Error("Insert Worker failed with no error code");
                            _workerId = 0;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Insert Worker Failed with exception: " + e);
                        _workerId = 0;
                    }
                }
            }
            return _workerId;
        }

        public bool CheckIfWorkerExists(string slacId, string isActive="Y")
        {
            bool _exists = false;
            int _count = 0;
            string _sqlWorker = "SELECT COUNT(*) FROM LST_WORKER WHERE SLAC_ID =:SlacId AND IS_ACTIVE=:IsActive";
            using (OracleCommand _cmdWorker = new OracleCommand())
            {
                _cmdWorker.Parameters.Add(":SlacId", OracleDbType.Varchar2).Value = slacId;
                _cmdWorker.Parameters.Add(":IsActive", OracleDbType.Varchar2).Value = isActive;

                using (OracleDataReader _drWorker = objData.GetReader(_sqlWorker, _cmdWorker))
                {
                    while (_drWorker.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drWorker[0]));
                    }
                    if (_count > 0)
                    {
                        _exists = true;
                    }
                    else _exists = false;
                    return _exists;
                }

            }

        }

        public bool CheckIfActive(string slacId)
        {
            bool _check = false;
            int _statusid = 0;
            string _sqlWorker = "SELECT STATUS_ID FROM LST_WORKER WHERE SLAC_ID =:SlacId AND IS_ACTIVE='Y'";
            using (OracleCommand _cmdWorker = new OracleCommand())
            {
                _cmdWorker.Parameters.Add(":SlacId", OracleDbType.Varchar2).Value = slacId;

                using (OracleDataReader _drWorker = objData.GetReader(_sqlWorker, _cmdWorker))
                {
                    while (_drWorker.Read())
                    {
                        _statusid = Convert.ToInt32(objCommon.FixDBNull(_drWorker[0]));
                    }
                    if (_statusid == 5)
                    {
                        _check = true;
                    }
                    else _check = false;
                    return _check;
                }

            }

        }

        public string CheckIfWorkerActiveGlobally(string slacId)
        {
          
            string _status = "";
            string _sqlWorker = @"SELECT  status From Vw_Lst_Worker_Details Where is_active='Y' and slac_id = :SlacId 
                                and Worker_Id In (select worker_id from Lst_worker_facility_Map where status_id in (5,6) and is_active = 'Y') ";
            using (OracleCommand _cmdWorker = new OracleCommand())
            {
                _cmdWorker.Parameters.Add(":SlacId", OracleDbType.Varchar2).Value = slacId;

                using (OracleDataReader _drWorker = objData.GetReader(_sqlWorker, _cmdWorker))
                {
                    while (_drWorker.Read())
                    {
                        _status = objCommon.FixDBNull(_drWorker[0]);
                    }
                    if (_status == "")
                    {
                        _status = "None";
                    }

                    return _status;
                }

            }

        }

        public DataSet GetWorkerInfo(OracleCommand cmdWorker, string filter, string searchText)
        {
            StringBuilder _sbWorker = new StringBuilder();

            _sbWorker.Append("Select Worker_Id,Slac_Id,Worker_Name,Email,Affiliation,Status,reason_inactive,Initcap(gonet) as gonet From Vw_Lst_Worker_Details Where is_active='Y' and Worker_Id In (select worker_id from Lst_worker_facility_Map where status_id in (5,6) and is_active = 'Y') ");
            if (searchText != "")
            {
                _sbWorker.Append(" AND LOWER(Worker_Name) like :Name");
                cmdWorker.Parameters.Add(":Name", OracleDbType.Varchar2).Value = "%" + searchText.ToLower() + "%";
            }
            _sbWorker.Append(filter);
            _dsList = objData.ReturnDataset(_sbWorker.ToString(), "worker", cmdWorker);
            return _dsList;
        }

        public Business.Worker GetWorkerDetails(int Id, string idType)
        {
            string _sqlWorker = "";
            Business.Worker objWorker = new Business.Worker();

            _sqlWorker = "SELECT * FROM VW_LST_WORKER_DETAILS WHERE ";
            using (OracleCommand _cmdworker = new OracleCommand())
            {
                if (idType == "worker")
                {
                    _sqlWorker += "WORKER_ID = :WorkerId";
                    _cmdworker.Parameters.Add(":WorkerId", OracleDbType.Int32).Value = Id;
                }
                else
                {
                    _sqlWorker += "SLAC_ID = :SlacId";
                    _cmdworker.Parameters.Add(":SlacId",  OracleDbType.Int32).Value = Id;
                }

                using (OracleDataReader _drWorker = objData.GetReader(_sqlWorker, _cmdworker))
                {
                    while (_drWorker.Read())
                    {
                        objWorker.WorkerId = Convert.ToInt32(_drWorker["WORKER_ID"]);
                        objWorker.WorkerName = objCommon.FixDBNull(_drWorker["WORKER_NAME"]);
                        objWorker.Affiliation = objCommon.FixDBNull(_drWorker["AFFILIATION"]);
                        objWorker.AffiliationId = Convert.ToInt32(_drWorker["AFFILIATION_ID"]);
                        objWorker.PreferredEmail = objCommon.FixDBNull(_drWorker["PREFERRED_EMAIL"]);
                        objWorker.CreatedById = objCommon.FixDBNull(_drWorker["CREATED_BY"]);
                        objWorker.EmailAddr = objCommon.FixDBNull(_drWorker["EMAIL"]);
                        objWorker.ManualReviewDate = (_drWorker["ESHMANUAL_REVIEW_DATE"] != System.DBNull.Value) ? Convert.ToDateTime(_drWorker["ESHMANUAL_REVIEW_DATE"]) : DateTime.MinValue;
                        objWorker.IsESHManualReviewed = (_drWorker["ESHMANUAL_REVIEW_DATE"] != System.DBNull.Value) ? "Y" : "N";
                        objWorker.StudentReqReviewDate = (_drWorker["STUDREQ_REVIEW_DATE"] != System.DBNull.Value) ? Convert.ToDateTime(_drWorker["STUDREQ_REVIEW_DATE"]) : DateTime.MinValue;
                        objWorker.IsStudReqReviewed = (_drWorker["STUDREQ_REVIEW_DATE"] != System.DBNull.Value) ? "Y" : "N";
                        objWorker.SlacId = Convert.ToInt32(objCommon.FixDBNull(_drWorker["SLAC_ID"]));
                        objWorker.StatusId = Convert.ToInt32(objCommon.FixDBNull(_drWorker["STATUS_ID"]));
                        objWorker.Status = objCommon.FixDBNull(_drWorker["STATUS"]);
                        objWorker.AlternateSvr = objCommon.FixDBNull(_drWorker["ALTERNATE_SVR"]);
                        objWorker.AlternateSvrFrom = (_drWorker["ALTSVR_FROM"] != System.DBNull.Value) ? Convert.ToDateTime(_drWorker["ALTSVR_FROM"]) : DateTime.MinValue;
                        objWorker.AlternateSvrTo = (_drWorker["ALTSVR_TO"] != System.DBNull.Value) ? Convert.ToDateTime(_drWorker["ALTSVR_TO"]) : DateTime.MinValue;
                    }
                    objWorker.Supervisor = GetSupervisorName(objWorker.SlacId.ToString());

                    return objWorker;
                }
            }
        }

        public int UpdateWorker(Business.Worker objWorker)
        {
            int _returncode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.Proc_Upd_Worker";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("Pi_Worker_Id",  OracleDbType.Int32).Value = objWorker.WorkerId;
                        _ocmd.Parameters["Pi_Worker_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Affiliation_Id",  OracleDbType.Int32).Value = objWorker.AffiliationId;
                        _ocmd.Parameters["Pi_Affiliation_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Preferred_Email", OracleDbType.Varchar2).Value = objWorker.PreferredEmail;
                        _ocmd.Parameters["Pi_Preferred_Email"].Direction = ParameterDirection.Input;

                        if (objWorker.AlternateSvr != "")
                        {
                            _ocmd.Parameters.Add("Pi_Alternate", OracleDbType.Int32).Value = Convert.ToInt32(objWorker.AlternateSvr);
                        }
                        else
                        {
                            _ocmd.Parameters.Add("Pi_Alternate", OracleDbType.Int32).Value = System.DBNull.Value;
                        }
                        _ocmd.Parameters["Pi_Alternate"].Direction = ParameterDirection.Input;

                        if (objWorker.AlternateSvrFrom != DateTime.MinValue)
                        {
                            _ocmd.Parameters.Add("Pi_Alt_From", OracleDbType.Date).Value = objWorker.AlternateSvrFrom;
                        }
                        else
                        {
                            _ocmd.Parameters.Add("Pi_Alt_From", OracleDbType.Date).Value = System.DBNull.Value;
                        }
                        _ocmd.Parameters["Pi_Alt_From"].Direction = ParameterDirection.Input;

                        if (objWorker.AlternateSvrTo != DateTime.MinValue)
                        {
                            _ocmd.Parameters.Add("Pi_Alt_To", OracleDbType.Date).Value = objWorker.AlternateSvrTo;
                        }
                        else
                        {
                            _ocmd.Parameters.Add("Pi_Alt_To", OracleDbType.Date).Value = System.DBNull.Value;
                        }
                        _ocmd.Parameters["Pi_Alt_To"].Direction = ParameterDirection.Input;


                        _ocmd.Parameters.Add("Pi_Modified_By",  OracleDbType.Varchar2).Value = objWorker.ModifiedBy;
                        _ocmd.Parameters["Pi_Modified_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PO_RETURN_CODE",  OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        //return code 0 is considered successful. If other than 0, it is a failure.
                        if (_ocmd.Parameters["PO_RETURN_CODE"].Value != null)
                        {
                            _returncode = Convert.ToInt32(_ocmd.Parameters["PO_RETURN_CODE"].Value.ToString());
                        }
                        else { _returncode = -1; }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Update Worker failed with the following exception: " + e);
                        _returncode = -1;
                    }
                }

            }
            return _returncode;
        }

        public int ReinstateWorker(Business.Worker objWorker)
        {
            int _returncode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.PROC_Upd_Deleted_Worker";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("Pi_Worker_Id",  OracleDbType.Int32).Value = objWorker.WorkerId;
                        _ocmd.Parameters["Pi_Worker_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Affiliation_Id",  OracleDbType.Int32).Value = objWorker.AffiliationId;
                        _ocmd.Parameters["Pi_Affiliation_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Preferred_Email", OracleDbType.Varchar2).Value = objWorker.PreferredEmail;
                        _ocmd.Parameters["Pi_Preferred_Email"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Eshmanual_Reviewed",  OracleDbType.Varchar2).Value = objWorker.IsESHManualReviewed;
                        _ocmd.Parameters["Pi_Eshmanual_Reviewed"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Studreq_Reviewed", OracleDbType.Varchar2).Value = objWorker.IsStudReqReviewed;
                        _ocmd.Parameters["Pi_Studreq_Reviewed"].Direction = ParameterDirection.Input;

                        if (objWorker.AlternateSvr != "")
                        {
                            _ocmd.Parameters.Add("Pi_Alternate",  OracleDbType.Int32).Value = Convert.ToInt32(objWorker.AlternateSvr);
                        }
                        else
                        {
                            _ocmd.Parameters.Add("Pi_Alternate",  OracleDbType.Int32).Value = System.DBNull.Value;
                        }
                        _ocmd.Parameters["Pi_Alternate"].Direction = ParameterDirection.Input;

                        if (objWorker.AlternateSvrFrom != DateTime.MinValue)
                        {
                            _ocmd.Parameters.Add("Pi_Alt_From",  OracleDbType.Date).Value = objWorker.AlternateSvrFrom;
                        }
                        else
                        {
                            _ocmd.Parameters.Add("Pi_Alt_From",  OracleDbType.Date).Value = System.DBNull.Value;
                        }
                        _ocmd.Parameters["Pi_Alt_From"].Direction = ParameterDirection.Input;

                        if (objWorker.AlternateSvrTo != DateTime.MinValue)
                        {
                            _ocmd.Parameters.Add("Pi_Alt_To",  OracleDbType.Date).Value = objWorker.AlternateSvrTo;
                        }
                        else
                        {
                            _ocmd.Parameters.Add("Pi_Alt_To",  OracleDbType.Date).Value = System.DBNull.Value;
                        }
                        _ocmd.Parameters["Pi_Alt_To"].Direction = ParameterDirection.Input;


                        _ocmd.Parameters.Add("Pi_Modified_By", OracleDbType.Varchar2).Value = objWorker.ModifiedBy;
                        _ocmd.Parameters["Pi_Modified_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PO_RETURN_CODE",  OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        //return code 0 is considered successful. If other than 0, it is a failure.
                        if (_ocmd.Parameters["PO_RETURN_CODE"].Value != null)
                        {
                            _returncode = Convert.ToInt32(_ocmd.Parameters["PO_RETURN_CODE"].Value.ToString());
                        }
                        else { _returncode = -1; }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Bringing back the deleted Worker failed with the following exception: " + e);
                        _returncode = -1;
                    }
                }

            }
            return _returncode;
        }

        public int UpdateWorkerStatus(int slacId, int statusId)
        {
            //TODO: Need to check all the lab related status before turning the worker to inactive
            int _returncode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.Proc_ChgStatus_Worker";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("Pi_Slac_Id",  OracleDbType.Int32).Value = slacId;
                        _ocmd.Parameters["Pi_Slac_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Stat",  OracleDbType.Int32).Value = statusId;
                        _ocmd.Parameters["Pi_Stat"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PO_RETURN_CODE",  OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        //return code 0 is considered successful. If other than 0, it is a failure.
                        if (_ocmd.Parameters["PO_RETURN_CODE"].Value != null)
                        {
                            _returncode = Convert.ToInt32(_ocmd.Parameters["PO_RETURN_CODE"].Value.ToString());
                        }
                        else { _returncode = -1; }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Update Worker Status failed with the following exception: " + e);
                        _returncode = -1;
                    }
                }

            }
            return _returncode;
        }

        public int GetWorkerId(string slacId, string isActive="Y")
        {
            string _sqlWorker = "SELECT WORKER_ID FROM LST_WORKER WHERE SLAC_ID = :SlacId AND IS_ACTIVE=:IsActive";
            int _workerId = 0;

            using (OracleCommand _cmdWorker = new OracleCommand())
            {
                _cmdWorker.Parameters.Add(":SlacId", OracleDbType.Varchar2).Value = slacId;
                _cmdWorker.Parameters.Add(":IsActive", OracleDbType.Varchar2).Value = isActive;

                using (OracleDataReader _drWorker = objData.GetReader(_sqlWorker, _cmdWorker))
                {
                    while (_drWorker.Read())
                    {
                        _workerId = Convert.ToInt32(objCommon.FixDBNull(_drWorker[0]));
                    }
                }
                return _workerId;
            }

        }

        public int GetSlacId(int workerId)
        {
            string _sqlWorker = "SELECT SLAC_ID FROM LST_WORKER WHERE WORKER_ID = :WorkerId AND IS_ACTIVE='Y'";
            int _workerSlacId = 0;

            using (OracleCommand _cmdWorker = new OracleCommand())
            {
                _cmdWorker.Parameters.Add(":WorkerId", OracleDbType.Varchar2).Value = workerId;

                using (OracleDataReader _drWorker = objData.GetReader(_sqlWorker, _cmdWorker))
                {
                    while (_drWorker.Read())
                    {
                        _workerSlacId = Convert.ToInt32(objCommon.FixDBNull(_drWorker[0]));
                    }
                }
                return _workerSlacId;
            }
        }

        public int GetRoleId(string roletype)
        {
            string _sqlRole = "SELECT LOOKUP_ID FROM LST_LOOKUP WHERE LOOKUP_DESC = :RoleType AND IS_ACTIVE='Y' AND LOOKUP_GROUP='Roles' AND PARENT_ID IS NULL";
            int _roleId = 0;

            using (OracleCommand _cmdRole = new OracleCommand())
            {
                _cmdRole.Parameters.Add(":RoleType", OracleDbType.Varchar2).Value = roletype;

                using (OracleDataReader _drRoles = objData.GetReader(_sqlRole, _cmdRole))
                {
                    while (_drRoles.Read())
                    {
                        _roleId = Convert.ToInt32(_drRoles[0]);
                    }
                }
                return _roleId;
            }
        }

        #endregion

        #region "Training"

        public string GetTrainingStatus(string personId, string courseId)
        {
            string _sqlStatus = "";
            string _status = "";

            _sqlStatus = "SELECT TRAINSAFE.GET_NON_STA_TRN_STATUS(:PersonId, :CourseId) FROM DUAL";
            using (OracleCommand _cmdStat = new OracleCommand())
            {
                _cmdStat.Parameters.Add(":PersonId", OracleDbType.Varchar2).Value = personId;
                _cmdStat.Parameters.Add(":CourseId", OracleDbType.Varchar2).Value = courseId;

                using (OracleDataReader _drStat = objData.GetReader(_sqlStatus, _cmdStat))
                {
                    while (_drStat.Read())
                    {
                        _status = objCommon.FixDBNull(_drStat[0]);
                    }
                    return _status;
                }
            }
        }

        public string GetCompletionDate(string personId, string courseId)
        {
            string _sqlCompleted = "";
            string _completedOn = "";

            //This should not be the case for 219 , 219 R since it is combining the 2 to get the latest date
            if ((courseId != "219") && (courseId != "219R"))
            {
                _sqlCompleted = "SELECT Trainsafe.Get_Last_Session_Date(:PersonId,:CourseId) FROM DUAL";
            }
            else
            {
                _sqlCompleted = @"SELECT MAX(session_date) FROM   trainsafe.sessions s, trainsafe.roster r
                     WHERE  s.session_num = r.session_num   and s.session_status ='C'   and r.roster_status = 'P'
                         and s.course_num = :CourseId  and r.student_num = :PersonId";
            }
           
            using (OracleCommand _cmdStat = new OracleCommand())
            {
                _cmdStat.Parameters.Add(":PersonId", OracleDbType.Varchar2).Value = personId;
                _cmdStat.Parameters.Add(":CourseId", OracleDbType.Varchar2).Value = courseId;

                using (OracleDataReader _drStat = objData.GetReader(_sqlCompleted, _cmdStat))
                {
                    while (_drStat.Read())
                    {
                        string _completed = objCommon.FixDBNull(_drStat[0]);
                        if (_completed != "")
                        {
                            DateTime _dtCompleted = Convert.ToDateTime(_completed);
                            _completedOn = "Completed on " + _dtCompleted.ToShortDateString();
                        }
                        
                    }
                    return _completedOn;
                }
            }
        }

       

        public string IsInSTA(string personId, string courseId)
        {
            string _sqlSTA = "";
            int _InSTA = 0;

            _sqlSTA = "SELECT COUNT(*) FROM TRAINSAFE.ETA_COURSE_LINK_PER WHERE COURSE_NUM = :CourseId AND PERSON_ID = :PersonId";
            using (OracleCommand _cmdSTA = new OracleCommand())
            {
                _cmdSTA.Parameters.Add(":PersonId", OracleDbType.Varchar2).Value = personId;
                _cmdSTA.Parameters.Add(":CourseId", OracleDbType.Varchar2).Value = courseId;

                using (OracleDataReader _drSTA = objData.GetReader(_sqlSTA, _cmdSTA))
                {
                    while (_drSTA.Read())
                    {
                        _InSTA = Convert.ToInt32(objCommon.FixDBNull(_drSTA[0]));
                    }
                    if (_InSTA == 0)
                    {
                        return "N";
                    }
                    else return "Y";
                }
            }
        }

        public bool IsTrainingCurrent(string personId, string courseId)
        {
            string _sqlTraining = "";
            string _result = "";

            _sqlTraining = @"Select  Decode(Substr(Trainsafe.Get_non_sta_trn_status(:1,:2),1,3),'Nev','N','Ove','N','Val','Y','Com','Y','N') Training
                            From  trainsafe.Courses C, (select course_num, count(*) from trainsafe.eta_course_link_per where course_num = :2 and person_id = :1 group by course_num) e
                            where  c.course_num = e.course_num(+) And C.Course_Num = :2 group by c.course_num";
            using (OracleCommand _cmdTraining = new OracleCommand())
            {
                _cmdTraining.Parameters.Add(":1", OracleDbType.Varchar2).Value = personId;
                _cmdTraining.Parameters.Add(":2", OracleDbType.Varchar2).Value = courseId;

                using (OracleDataReader _drTraining = objData.GetReader(_sqlTraining, _cmdTraining))
                {
                    while (_drTraining.Read())
                    {
                        _result = objCommon.FixDBNull(_drTraining["Training"]);
                    }
                    if (_result == "Y") return true;
                    else return false;
                }
            }

        }

        #endregion

        #region "User"
        public int GetUserRoleId(string userId, int roleTypeId)
        {
            //Only get role id for other SLSO as the same user can be a LSO or Deputy LSO - can they be? but just in case
            string _sqlUserRole = "SELECT USER_ROLE_ID FROM LST_USER_ROLES WHERE SLAC_ID= :UserId and ROLE_TYPE_ID = :RoleTypeId AND IS_ACTIVE='Y'";
            int _userroleid = 0;

            using (OracleCommand _cmdRole = new OracleCommand())
            {
                _cmdRole.Parameters.Add(":UserId", OracleDbType.Varchar2).Value = userId;
                _cmdRole.Parameters.Add(":RoleTypeId", OracleDbType.Int32).Value = roleTypeId;

                using (OracleDataReader _drRole = objData.GetReader(_sqlUserRole, _cmdRole))
                {
                    while (_drRole.Read())
                    {
                        _userroleid = Convert.ToInt32(objCommon.FixDBNull(_drRole[0]));
                    }
                    return _userroleid;
                }
            }
        }

        public DataSet GetSLSOLabInfo(int userRoleId, bool isperUser = false, string slacId = "", string searchText = "")
        {
            DataSet _dsSLSOLab = new DataSet();
            StringBuilder _sbSLSO = new StringBuilder();

            _sbSLSO.Append("Select fac.Facility_Id, fac.Facility_Name, Lst_Get_Usertype(:UserRoleId, fac.Facility_Id) As Usertype, (SELECT FAC1.ALTERNATE_SLSO FROM LST_FACILITY FAC1 WHERE FAC1.FACILITY_ID = FAC.FACILITY_ID AND TRUNC(FAC1.ALTSLSO_TO) >= TRUNC(SYSDATE) ) AS Alternate_SLSO,  (SELECT PC.EMPLOYEE_NAME FROM VW_PEOPLE PC WHERE FAC.ALTERNATE_SLSO = PC.EMPLOYEE_ID(+) AND TRUNC(FAC.ALTSLSO_TO) >= TRUNC(SYSDATE) ) as altslso from lst_facility fac where Lst_Get_Usertype(:UserRoleId, fac.Facility_Id) <> 'NONE' AND FAC.IS_ACTIVE='Y' ");


            using (OracleCommand _cmdSLSOLab = new OracleCommand())
            {
                if (isperUser)
                {
                    _sbSLSO.Append(" AND (fac.SLSO = :UserRoleId OR fac.CO_SLSO1 = :UserRoleId OR fac.ACTING_SLSO = :UserRoleId)  ");                 
                }
                //Will append the facilities that the user is active alternate slso
                    _sbSLSO.Append(" UNION SELECT fac.FACILITY_ID,fac.FACILITY_NAME, 'Alternate SLSO' as Usertype, fac.alternate_slso, pc.employee_name as altslso From Lst_Facility fac, vw_people pc Where fac.Alternate_Slso = :SlacId And Trunc(fac.Altslso_To) >= TRUNC(SYSDATE) and fac.alternate_slso = pc.employee_id(+) AND FAC.IS_ACTIVE='Y' ");
                    _cmdSLSOLab.Parameters.Add(":SlacId", OracleDbType.Varchar2).Value = slacId;
                _cmdSLSOLab.Parameters.Add(":UserRoleId", OracleDbType.Int32).Value = userRoleId;
                _dsSLSOLab = objData.ReturnDataset(_sbSLSO.ToString(), "slsolabs", _cmdSLSOLab);
                return _dsSLSOLab;
            }
        }

        public int CreateUser(Business.User objUser)
        {
            int _userRoleId;

            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.Proc_Ins_User";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("Pi_Role_Type_ID", OracleDbType.Int32).Value = objUser.RoleTypeId;
                        _ocmd.Parameters["Pi_Role_Type_ID"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Slac_Id", OracleDbType.Int32).Value = objUser.SlacId;
                        _ocmd.Parameters["Pi_Slac_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Created_By", OracleDbType.Varchar2).Value = objUser.CreatedBy;
                        _ocmd.Parameters["Pi_Created_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Po_User_Role_Id", OracleDbType.Int32).Direction = ParameterDirection.Output;

                        _ocmd.ExecuteNonQuery();
                        if (_ocmd.Parameters["Po_User_Role_Id"].Value != null)
                        {
                            _userRoleId = Convert.ToInt32(_ocmd.Parameters["Po_User_Role_Id"].Value.ToString());
                            if (_userRoleId <= 0)
                            {
                                Log.Error("Insert User failed with the following sql error: " + _userRoleId);
                                _userRoleId = 0;
                            }
                        }
                        else
                        {
                            Log.Error("Insert User failed with no error code");
                            _userRoleId = 0;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Insert User failed with exception: " + e);
                        _userRoleId = 0;
                    }
                }
            }
            return _userRoleId;
        }

        public bool CheckIfUserExists(string slacId, int roletype)
        {
            bool _exists = false;
            int _count = 0;
            string _sqlUser = "SELECT COUNT(*) FROM LST_USER_ROLES WHERE SLAC_ID =:SlacId AND IS_ACTIVE='Y' AND ROLE_TYPE_ID = :RoleTypeId";
            using (OracleCommand _cmdUser = new OracleCommand())
            {
                _cmdUser.Parameters.Add(":SlacId", OracleDbType.Varchar2).Value = slacId;
                _cmdUser.Parameters.Add(":RoleTypeId", OracleDbType.Int32).Value = roletype;

                using (OracleDataReader _drUser = objData.GetReader(_sqlUser, _cmdUser))
                {
                    while (_drUser.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drUser[0]));
                    }
                    if (_count > 0)
                    {
                        _exists = true;
                    }
                    else _exists = false;
                    return _exists;
                }

            }

        }

        public bool CheckIfSLSO(string slacId, int facId)
        {
            bool _exists = false;
            int _count = 0;
            string _sqlUser =@" select count(*) from lst_Facility where facility_id =:FacId
 and slso in (select user_role_id from lst_user_roles
 where role_type_id = 15 and is_active = 'Y' and slac_id = :SlacId) ";
            using (OracleCommand _cmdUser = new OracleCommand())
            {
                _cmdUser.Parameters.Add(":SlacId", OracleDbType.Varchar2).Value = slacId;
                _cmdUser.Parameters.Add(":FacId", OracleDbType.Int32).Value = facId;

                using (OracleDataReader _drUser = objData.GetReader(_sqlUser, _cmdUser))
                {
                    while (_drUser.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drUser[0]));
                    }
                    if (_count > 0)
                    {
                        _exists = true;
                    }
                    else _exists = false;
                    return _exists;
                }

            }

        }


        public DataSet GetUserInfo(OracleCommand cmdUser, string filter, string searchKey = "")
        {
            StringBuilder _sbUser = new StringBuilder();
            _sbUser.Append("SELECT USERNAME,LOOKUP_DESC,USER_ROLE_ID,To_date(Trainsafe.Get_Non_Sta_Trn_Status(SLAC_ID,'130','DATE'),'MM/dd/yyyy') C130,To_date(Trainsafe.Get_Non_Sta_Trn_Status(SLAC_ID,'108','DATE'),'MM/dd/yyyy') C108,");
            _sbUser.Append("To_date(Trainsafe.Get_Non_Sta_Trn_Status(SLAC_ID,'108PRA','DATE'),'MM/dd/yyyy') C108PRA,To_date(Trainsafe.Get_Non_Sta_Trn_Status(SLAC_ID,'157','DATE'),'MM/dd/yyyy') C157,SLAC_ID,STATUS,EMAIL FROM VW_LST_USERS WHERE ROLE_TYPE_ID=15 ");

            if (searchKey != "")
            {
                _sbUser.Append(" AND LOWER(USERNAME) LIKE :Name");
                cmdUser.Parameters.Add(":Name", OracleDbType.Varchar2).Value = "%" + searchKey.ToLower() + "%";
            }

            if (filter != "") { _sbUser.Append(filter); }

            _dsList = objData.ReturnDataset(_sbUser.ToString(), "user", cmdUser);
            return _dsList;
        }

        public int CreateAlternate(int objid, string altType, int altId, DateTime altFrom, DateTime altTo, string modifiedBy)
        {
            int _returncode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.Proc_Designate_Alternate";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("Pi_Objid", OracleDbType.Int32).Value = objid;
                        _ocmd.Parameters["Pi_Objid"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Alternate_Type", OracleDbType.Varchar2).Value = altType;
                        _ocmd.Parameters["Pi_Alternate_Type"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Alternate", OracleDbType.Int32).Value = altId;
                        _ocmd.Parameters["Pi_Alternate"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Alt_From", OracleDbType.Date).Value = altFrom;
                        _ocmd.Parameters["Pi_Alt_From"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Alt_To", OracleDbType.Date).Value = altTo;
                        _ocmd.Parameters["Pi_Alt_To"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Modified_By", OracleDbType.Varchar2).Value = modifiedBy;
                        _ocmd.Parameters["Pi_Modified_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Po_Return_Code", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        //return code 0 is considered successful. If other than 0, it is a failure.
                        if (_ocmd.Parameters["Po_Return_Code"].Value != null)
                        {
                            _returncode = Convert.ToInt32(_ocmd.Parameters["Po_Return_Code"].Value.ToString());
                        }
                        else { _returncode = -2; }
                    }
                    catch (Exception e)
                    {
                        Log.Error(" failed with the following exception: " + e);
                        _returncode = -2;
                    }
                }

            }
            return _returncode;
        }

        public int UpdateAltLSO(int userRoleId, DateTime altFrom, DateTime altTo)
        {
            int _returncode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.PROC_UPD_ALTERNATELSO";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("PI_USER_ROLE_ID", OracleDbType.Int32).Value = userRoleId;
                        _ocmd.Parameters["PI_USER_ROLE_ID"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Alt_From", OracleDbType.Date).Value = altFrom;
                        _ocmd.Parameters["Pi_Alt_From"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Alt_To", OracleDbType.Date).Value = altTo;
                        _ocmd.Parameters["Pi_Alt_To"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Modified_By", OracleDbType.Varchar2).Value = objCommon.GetUserId();
                        _ocmd.Parameters["Pi_Modified_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Po_Return_Code", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        //return code 0 means no rows updated.
                        if (_ocmd.Parameters["Po_Return_Code"].Value != null)
                        {
                            _returncode = Convert.ToInt32(_ocmd.Parameters["Po_Return_Code"].Value.ToString());
                        }
                        else { _returncode = 0; }
                    }
                    catch (Exception e)
                    {
                        Log.Error(" failed with the following exception: " + e);
                        _returncode = 0;
                    }
                }

            }
            return _returncode;


        }

        public int DeleteAltLSO(int userRoleId, string _modifiedBy)
        {
            int _returncode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.PROC_DEL_ALTERNATELSO";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("PI_USER_ROLE_ID", OracleDbType.Int32).Value = userRoleId;
                        _ocmd.Parameters["PI_USER_ROLE_ID"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Modified_By", OracleDbType.Varchar2).Value = _modifiedBy;
                        _ocmd.Parameters["Pi_Modified_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Po_Return_Code", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        //return code 0 means no rows updated.
                        if (_ocmd.Parameters["Po_Return_Code"].Value != null)
                        {
                            _returncode = Convert.ToInt32(_ocmd.Parameters["Po_Return_Code"].Value.ToString());
                        }
                        else { _returncode = 0; }
                    }
                    catch (Exception e)
                    {
                        Log.Error(" failed with the following exception: " + e);
                        _returncode = 0;
                    }
                }

            }
            return _returncode;

        }

        public string GetSupervisorName(string slacId, string svrType = "")
        {
            string _sqlSvr;
            string _svrName = "";

            _sqlSvr = "select P2.Employee_name from VW_people_current P1, Vw_people_Current P2 where P1.employee_id=:EmpID and P1.Supervisor_Id = P2.employee_id";

            using (OracleCommand _cmdSvr = new OracleCommand())
            {
                _cmdSvr.Parameters.Add(":EmpID", OracleDbType.Varchar2).Value = slacId;

                using (OracleDataReader _drSvr = objData.GetReader(_sqlSvr, _cmdSvr))
                {
                    while (_drSvr.Read())
                    {
                        _svrName = objCommon.FixDBNull(_drSvr[0]);
                    }
                    return _svrName;
                }
            }
        }

        public bool IsApproved(string apprType, int mapId)
        {
            string _sqlAppwf;
            _sqlAppwf = "SELECT COUNT(*) FROM LST_APPROVAL_WORKFLOW WHERE MAP_ID=:MapId and APPROVER_TYPE = :ApproverType and STATUS_ID=3";
            int _count = 0;

            using (OracleCommand _cmdAppwf = new OracleCommand())
            {
                _cmdAppwf.Parameters.Add(":ApproverType", OracleDbType.Varchar2).Value = apprType;
                _cmdAppwf.Parameters.Add(":MapId", OracleDbType.Int32).Value = mapId;
                using (OracleDataReader _drAppwf = objData.GetReader(_sqlAppwf, _cmdAppwf))
                {
                    if (_drAppwf.HasRows)
                    {
                        while (_drAppwf.Read())
                        {
                            _count = Convert.ToInt32(_drAppwf[0]);
                        }
                    }

                }
                return (_count > 0) ? true : false;

            }
        }
        public string GetActivePeriodLSO()
        {
            string _sqlFromTo = "";
            string _dtFrom;
            string _dtTo;
            string _fromTo = "";
            _sqlFromTo = @"Select ALTLSO_FROM , ALTLSO_TO FROM LST_USER_ROLES WHERE  TRUNC(ALTLSO_TO) >= TRUNC(SYSDATE) AND IS_ACTIVE='Y' AND TRUNC(ALTLSO_FROM) <= TRUNC(SYSDATE)";
            using (OracleCommand _cmdFromto = new OracleCommand())
            {
                using (OracleDataReader _drFromTo = objData.GetReader(_sqlFromTo,_cmdFromto))
                {
                    if (_drFromTo.HasRows)
                    {
                        while (_drFromTo.Read())
                        {
                            _dtFrom = Convert.ToDateTime(_drFromTo[0]).ToShortDateString();
                            _dtTo = Convert.ToDateTime(_drFromTo[1]).ToShortDateString();

                            _fromTo = " from " + _dtFrom + " to " + _dtTo;
                        }
                    }
                    else _fromTo =  " ";
                }
            }
            return _fromTo;

        }

        public bool CheckifDatesROutsideCur(DateTime altFrom, DateTime altTo, int userRoleId)
        {
            bool _isValid = false;
            int _count = 0;
            string _sqlValid;
            
            //if input from is greater than alt_to --count > 0 , return valid=true else false
            //if input from is less than any of the alt_to, check if altTo is not between
            // altlso_from and altlsoto, or altfrom is not between altlso from to
            //altFrom < ALTLSO_TO  
           // _sqlValid = @"SELECT COUNT(*) FROM LST_USER_ROLES WHERE TRUNC(ALTLSO_TO) >= :FromNewDate AND ROLE_TYPE_ID=20";
            _sqlValid = @"SELECT COUNT(*) FROM lst_user_roles
WHERE  ((trunc(ALTLSO_FROM) <= trunc(TO_DATE(:FromNewDate,'mm/dd/yyyy'))
AND  trunc(ALTLSO_TO) >= trunc(TO_DATE(:FromNewDate,'mm/dd/yyyy')))
OR (trunc(ALTLSO_FROM) <= trunc(TO_DATE(:ToNewDate,'mm/dd/yyyy'))
AND  trunc(ALTLSO_TO) >= trunc(TO_DATE(:ToNewDate,'mm/dd/yyyy')))
OR (TRUNC(ALTLSO_FROM) >=  trunc(TO_DATE(:FromNewDate,'mm/dd/yyyy'))
and trunc(ALTLSO_TO) <= trunc(TO_DATE(:ToNewDate,'mm/dd/yyyy')))) ";

            using (OracleCommand _cmdVal = new OracleCommand())
            {
                _cmdVal.Parameters.Add(":FromNewDate", OracleDbType.Varchar2).Value = altFrom.ToShortDateString();
                _cmdVal.Parameters.Add(":ToNewDate", OracleDbType.Varchar2).Value =  altTo.ToShortDateString();
                if (userRoleId != 0)
                {
                    _sqlValid += " AND USER_ROLE_ID <> :UserRoleId";
                    _cmdVal.Parameters.Add(":UserRoleId", OracleDbType.Int32).Value = userRoleId;
                }

                using (OracleDataReader _drVal = objData.GetReader(_sqlValid, _cmdVal))
                {
                    if (_drVal.HasRows)
                    {
                        while (_drVal.Read())
                        {
                            _count = Convert.ToInt32(_drVal[0]);
                        }
                    }
                    _isValid = (_count == 0) ? true : false;
                }
            }
            return _isValid;

        }

        public string GetLSOName(string lsoType="")
        {
            string _sqlLSOName = "";
            string _ListName = "";
            int _roletypeId = 0;
            if (lsoType.Equals("lso")) 
                _roletypeId = 13;
            else if (lsoType.Equals("dlso"))
                _roletypeId = 14;
            else if (lsoType.Equals("altlso"))
                _roletypeId = 20;
            

            _sqlLSOName = @"Select Pc.Employee_Name From Lst_User_Roles R, Vw_People Pc Where R.Slac_Id = Pc.Employee_Id And R.Role_Type_Id = :RoleTypeId And
                            R.Is_Active ='Y'";
            if (lsoType.Equals("altlso"))
            {
                _sqlLSOName += " and trunc(R.Altlso_To) >= trunc(sysdate) AND TRUNC(R.ALTLSO_FROM) <= TRUNC(SYSDATE)";
            }
            using (OracleCommand _cmdName = new OracleCommand())
            {
                _cmdName.Parameters.Add(":RoleTypeId", OracleDbType.Int32).Value = _roletypeId;
                using (OracleDataReader _drName = objData.GetReader(_sqlLSOName, _cmdName))
                {

                    if (_drName.HasRows)
                    {
                         while (_drName.Read())
                         {
                             if (_ListName != "")
                             {
                                 _ListName += " ; " + objCommon.FixDBNull(_drName[0]);
                             }
                             else
                             {
                                 _ListName = objCommon.FixDBNull(_drName[0]);
                             }
                            

                         }
                    }
                    return _ListName;
                }
            }

        }

        public int UpdateAltSLSO(int facId, DateTime altFrom, DateTime altTo)
        {
            int _returncode;

            using (OracleConnection _ocon = new  OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.PROC_UPD_ALTERNATESLSO";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("PI_FACILITY_ID", OracleDbType.Int32).Value = facId;
                        _ocmd.Parameters["PI_FACILITY_ID"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Alt_From", OracleDbType.Date).Value = altFrom;
                        _ocmd.Parameters["Pi_Alt_From"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Alt_To", OracleDbType.Date).Value = altTo;
                        _ocmd.Parameters["Pi_Alt_To"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Modified_By", OracleDbType.Varchar2).Value = objCommon.GetUserId();
                        _ocmd.Parameters["Pi_Modified_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Po_Return_Code", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        //return code 0 means no rows updated.
                        if (_ocmd.Parameters["Po_Return_Code"].Value != null)
                        {
                            _returncode = Convert.ToInt32(_ocmd.Parameters["Po_Return_Code"].Value.ToString());
                        }
                        else { _returncode = 0; }
                    }
                    catch (Exception e)
                    {
                        Log.Error(" failed with the following exception: " + e);
                        _returncode = 0;
                    }
                }
              }
    
        return _returncode;
        }

        public int DeleteAltSLSO(int facId, string _modifiedBy)
        {
            int _returncode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.PROC_DEL_ALTERNATESLSO";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("PI_FACILITY_ID", OracleDbType.Int32).Value = facId;
                        _ocmd.Parameters["PI_FACILITY_ID"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Modified_By", OracleDbType.Varchar2).Value = _modifiedBy;
                        _ocmd.Parameters["Pi_Modified_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Po_Return_Code", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        //return code 0 means no rows updated.
                        if (_ocmd.Parameters["Po_Return_Code"].Value != null)
                        {
                            _returncode = Convert.ToInt32(_ocmd.Parameters["Po_Return_Code"].Value.ToString());
                        }
                        else { _returncode = 0; }
                    }
                    catch (Exception e)
                    {
                        Log.Error(" failed with the following exception: " + e);
                        _returncode = 0;
                    }
                }

            }
            return _returncode;

        }

        public int GetAdminSVRCount()
        {
            int _svrCount = 0;
            string _sqlSVR = @"SELECT COUNT(DISTINCT AW.APPROVER_ID) FROM LST_APPROVAL_WORKFLOW AW    LEFT JOIN LST_WORKER_FACILITY_MAP WFM ON AW.MAP_ID = WFM.MAP_ID 
                                LEFT JOIN PERSONS.PERSON PC ON AW.APPROVER_ID = PC.KEY  LEFT JOIN LST_WORKER W ON WFM.WORKER_ID= W.WORKER_ID 
                                LEFT JOIN PERSONS.PERSON PC1 ON W.SLAC_ID = PC1.KEY  WHERE  PC1.SUPERVISOR_ID = AW.APPROVER_ID AND  AW.STATUS_ID=3 AND
                                AW.APPROVER_TYPE='ADMSVR'  AND  WFM.STATUS_ID IN (5,6) AND  WFM.IS_ACTIVE='Y'";
            using (OracleDataReader _drSvr = objData.GetReader(_sqlSVR))
            {
                try
                {
                    if (_drSvr.HasRows)
                    {
                        while (_drSvr.Read())
                        {
                            _svrCount = Convert.ToInt32(objCommon.FixDBNull(_drSvr[0]));
                        }
                    }
                    return _svrCount;
                }
                catch (Exception e)
                {
                    Log.Error(" Error getting Admin Supervisor count in GetAdminSVRCount() ", e);
                    return 0;
                }
               
            }

        }

        #endregion

        #region "UserLabInfo"
        public bool IsQLO(int mapId)
        {
            int _count = 0;
            string _sqlQLO = "SELECT COUNT(*) FROM LST_WORKER_FACILITY_MAP WHERE WORKER_TYPE_ID = 7 AND IS_ACTIVE='Y' AND MAP_ID=:MapId";

            using (OracleCommand _cmdQLO = new OracleCommand())
            {
                _cmdQLO.Parameters.Add(":MapId",  OracleDbType.Int32).Value = mapId;

                using (OracleDataReader _drQLO = objData.GetReader(_sqlQLO, _cmdQLO))
                {
                    while (_drQLO.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drQLO[0]));
                    }
                }
                return (_count > 0) ? true : false;
            }
        }

        public bool CheckIfWorkerForLab(int workerId, int LabId , int workTypeId)
        {
            bool _exists = false;
            int _count = 0;
            string _sqlWorker;


            using (OracleCommand _cmdWorker = new OracleCommand())
            {
                if (LabId == 0)
                {
                    _sqlWorker = "SELECT COUNT(*) FROM LST_WORKER_FACILITY_MAP WHERE  WORKER_ID =:WorkerId AND IS_ACTIVE='Y' AND FACILITY_ID IS NULL AND WORKER_TYPE_ID = :WorktypeId";
                }
                else
                {
                    //it should allow to create a new request for that facility if the previous request has been declined.
                    _sqlWorker = "SELECT COUNT(*) FROM LST_WORKER_FACILITY_MAP WHERE FACILITY_ID=:FacilityId AND WORKER_ID =:WorkerId AND IS_ACTIVE='Y' AND STATUS_ID NOT IN (4) AND WORKER_TYPE_ID = :WorktypeId";
                    _cmdWorker.Parameters.Add(":FacilityId", OracleDbType.Int32).Value = LabId;
                }

                _cmdWorker.Parameters.Add(":WorkerId", OracleDbType.Int32).Value = workerId;
                _cmdWorker.Parameters.Add(":WorkTypeId", OracleDbType.Int32).Value = workTypeId;

                using (OracleDataReader _drWorker = objData.GetReader(_sqlWorker, _cmdWorker))
                {
                    while (_drWorker.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drWorker[0]));
                    }
                    if (_count > 0)
                    {
                        _exists = true;
                    }
                    else _exists = false;
                    return _exists;
                }

            }

        }

        public int CreateWorkerForLab(Business.WorkerFacility objWorkerFacility, string eshReviewed, string studReviewed)
        {
            int _workerFacId;

            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_APPROVAL_PKG.Proc_Ins_Approvalrequest";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("Pi_Worker_Id", OracleDbType.Int32).Value = objWorkerFacility.WorkerId;
                        _ocmd.Parameters["Pi_Worker_Id"].Direction = ParameterDirection.Input;

                        if (objWorkerFacility.FacilityId > 0)
                        {
                            _ocmd.Parameters.Add("Pi_Facility_Id", OracleDbType.Int32).Value = objWorkerFacility.FacilityId;
                        }
                        else { _ocmd.Parameters.Add("Pi_Facility_Id", OracleDbType.Int32).Value = System.DBNull.Value; }

                        _ocmd.Parameters["Pi_Facility_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Worker_Type_Id", OracleDbType.Int32).Value = objWorkerFacility.WorkTypeId;
                        _ocmd.Parameters["Pi_Worker_Type_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Conditional_Approval", OracleDbType.Varchar2).Value = objWorkerFacility.ConditionalApproval;
                        _ocmd.Parameters["Pi_Conditional_Approval"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Justification", OracleDbType.Varchar2).Value = objWorkerFacility.Justification;
                        _ocmd.Parameters["Pi_Justification"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Sop_Reviewed", OracleDbType.Varchar2).Value = objWorkerFacility.SOPReviewed;
                        _ocmd.Parameters["Pi_Sop_Reviewed"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Eshmanual_Reviewed", OracleDbType.Varchar2).Value = eshReviewed;
                        _ocmd.Parameters["Pi_Eshmanual_Reviewed"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Studreq_Reviewed", OracleDbType.Varchar2).Value = studReviewed;
                        _ocmd.Parameters["Pi_Studreq_Reviewed"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Created_By", OracleDbType.Varchar2).Value = objWorkerFacility.CreatedBy;
                        _ocmd.Parameters["Pi_Created_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PO_MAP_ID", OracleDbType.Int32).Direction = ParameterDirection.Output;

                        _ocmd.ExecuteNonQuery();

                        if (_ocmd.Parameters["PO_MAP_ID"].Value != null)
                        {
                            _workerFacId = Convert.ToInt32(_ocmd.Parameters["PO_MAP_ID"].Value.ToString());
                            if (_workerFacId <= 0)
                            {
                                Log.Error("Insert Approval Request failed with the following sql error: " + _workerFacId);
                                _workerFacId = 0;
                            }
                        }
                        else
                        {
                            Log.Error("Insert Worker failed with no error code");
                            _workerFacId = 0;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Insert Worker Failed with exception: " + e);
                        _workerFacId = 0;
                    }
                }
            }
            return _workerFacId;

        }

        public int GetWorkerId(int mapId)
        {
            string _sqlWorker = "SELECT WORKER_ID FROM LST_WORKER_FACILITY_MAP WHERE MAP_ID = :MapId AND IS_ACTIVE='Y'";
            int _workerId = 0;

            using (OracleCommand _cmdWorker = new OracleCommand())
            {
                _cmdWorker.Parameters.Add(":MapId", OracleDbType.Int32).Value = mapId;

                using (OracleDataReader _drWorker = objData.GetReader(_sqlWorker, _cmdWorker))
                {
                    while (_drWorker.Read())
                    {
                        _workerId = Convert.ToInt32(objCommon.FixDBNull(_drWorker[0]));
                    }
                }
                return _workerId;
            }
        }

        public int GetFacilityId(int mapId)
        {
            string _sqlFacility = "SELECT FACILITY_ID FROM LST_WORKER_FACILITY_MAP WHERE MAP_ID = :MapId";
            int _facilityId = 0;
          
            using (OracleCommand _cmdFac = new OracleCommand())
            {
                _cmdFac.Parameters.Add(":MapId", OracleDbType.Int32).Value = mapId;

                using (OracleDataReader _drFac = objData.GetReader(_sqlFacility, _cmdFac))
                {
                    while (_drFac.Read())
                    {
                        if (_drFac[0] != System.DBNull.Value)
                        {
                            _facilityId = Convert.ToInt32(_drFac[0]);
                        }


                    }
                }
                return _facilityId;
            }
        }

        public DataSet GetWorkersForFacility(int facId)
        {
            DataSet _dsWorkerFac = new DataSet();
            string _sqlWorkerFac = "";

            using (OracleCommand _cmdWorkerFac = new OracleCommand())
            {
                _sqlWorkerFac = @"select * from (Select Wf.Map_Id, Wf.Worker,Wf.Worktype, Wf.Status,Wf.Ojt_Completion_Date,Wf.Status_Id,
                                    Wfa.Attachment_Id, Wfa.File_Name, Wfa.File_Data, Wfa.Doctype,wf.worker_id, wf.sop_review_date,wf.slac_id
                                    From Vw_Lst_Worker_Facility  Wf 
                                    Left outer Join Lst_Workerfacility_Attachment Wfa On Wf.Map_Id = Wfa.Map_Id 
                                    Where Wf.Facility_Id = :FacilityId And Wf.Is_Active='Y' And Wf.Status_Id <> 4
                                    and wfa.is_active='Y' and wfa.Doctype ='OJT'
                                    Union all
                                    Select Wf.Map_Id, Wf.Worker,Wf.Worktype, Wf.Status,Wf.Ojt_Completion_Date,Wf.Status_Id, 0 As Attachment_Id, '' As File_Name,
                                    null as file_data, '' as doctype,wf.worker_id, wf.sop_review_date,wf.slac_id
                                    From Vw_Lst_Worker_Facility  Wf Where Wf.Facility_Id = :FacilityId And Wf.Is_Active='Y' And Wf.Status_Id <> 4
                                    And Wf.Map_Id Not In ( Select Distinct Map_Id From Lst_Workerfacility_Attachment Where Is_Active ='Y' 
                                    and doctype='OJT')) order by Worker";
                //_sqlWorkerFac = "SELECT MAP_ID, WORKER, WORKTYPE, STATUS,OJT_COMPLETION_DATE,STATUS_ID FROM VW_LST_WORKER_FACILITY WHERE FACILITY_ID = :FacilityId and IS_ACTIVE='Y' AND STATUS_ID <> 4";
                _cmdWorkerFac.Parameters.Add(":FacilityId", OracleDbType.Int32).Value = facId;
                _dsWorkerFac = objData.ReturnDataset(_sqlWorkerFac, "workfac", _cmdWorkerFac);
                return _dsWorkerFac;
            }
        }

        public OracleDataReader GetApprovalRequestDetails(string userType, int userRoleId, string slacId)
        {
            using (OracleCommand _cmdRequest = new OracleCommand())
            {
                StringBuilder _sbRequest = new StringBuilder();
                _sbRequest.Append(@"Select Wfm.Map_Id, Wfm.Worker_Id, Wfm.Facility_Id, Wfm.Worker_Type_Id, Wfm.Conditional_Approval, Lu.Lookup_Desc As Workertype, 
                                        FAC.FACILITY_NAME, WKR.SLAC_ID, P.EMPLOYEE_NAME AS WORKERNAME, LU1.LOOKUP_DESC AS CURRENTSTATUS 
                                        From Lst_Worker_Facility_Map Wfm 
                                        Left Join Lst_Lookup Lu On Lu.Lookup_Id = Wfm.Worker_Type_Id 
                                        Left Join Lst_Facility Fac On Fac.Facility_Id = Wfm.Facility_Id
                                        Left Join Lst_Worker Wkr On Wkr.Worker_Id = Wfm.Worker_Id
                                        LEFT JOIN VW_PEOPLE P ON WKR.SLAC_ID = P.EMPLOYEE_ID
                                         LEFT JOIN LST_LOOKUP LU1 ON LU1.LOOKUP_ID = WFM.STATUS_ID Where WFM.Status_Id = 2 AND Lu.Lookup_Group='WorkerType' AND LU1.LOOKUP_GROUP='Status' and wfm.is_active='Y' ");
                if (userType == "SLSO")
                {
                    _sbRequest.Append(@" AND Wfm.Facility_Id In (Select Facility_Id From Lst_Facility Where Is_Active = 'Y' And (Slso =:UserRoleId  or (Alternate_slso = :SlacId and Trunc(altslso_to) >= Trunc(sysdate)))) 
                                        and WFM.map_id in (select map_id from lst_approval_workflow where approver_type ='SLSO' and status_id =2)  ");
                    _cmdRequest.Parameters.Add(":UserRoleId", OracleDbType.Int32).Value = userRoleId;
                    _cmdRequest.Parameters.Add(":SlacId", OracleDbType.Varchar2).Value = slacId;
                }
                else if (userType == "LSO")
                {
                    _sbRequest.Append(@" and WFM.map_id in (select map_id from lst_approval_workflow where approver_type ='LSO' and status_id =2) ");
                }
                else if (userType == "ADMSVR")
                {
                    _sbRequest.Append(@"And Wfm.Worker_Id In (Select W.Worker_Id From Lst_Worker W Left Join Vw_People_Current Pc 
                                        On W.Slac_Id = Pc.Employee_Id Where Pc.Supervisor_Id=:SlacId  Union
                                        SELECT WORKER_ID FROM LST_WORKER WHERE ALTERNATE_SVR = :SlacId AND TRUNC(ALTSVR_TO) >= TRUNC(SYSDATE) )
                                        And Wfm.Map_Id In (Select Map_Id From Lst_Approval_Workflow where approver_type='ADMSVR' AND STATUS_ID=2) ");
                    _cmdRequest.Parameters.Add(":SlacId", OracleDbType.Varchar2).Value = slacId;
                }
                _sbRequest.Append(" order by wfm.map_id");

                try
                {
                    OracleDataReader _drRequest = objData.GetReader(_sbRequest.ToString(), _cmdRequest);
                    if (_drRequest.HasRows)
                    {
                        return _drRequest;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Approval request details for " + userType + " failed with the following exception " + e);
                    return null;
                }


            }
        }

        public OracleDataReader GetPendingRequest(int workerId)
        {
            using (OracleCommand _cmdPending = new OracleCommand())
            {
                StringBuilder _sbPending = new StringBuilder();
                _sbPending.Append(@"Select Wfm.Map_Id, Wfm.Worker_Id, Wfm.Facility_Id, Wfm.Worker_Type_Id, Wfm.Sop_Review_Date, Wfm.Conditional_Approval, Lu.Lookup_Desc As Workertype, 
                                        FAC.FACILITY_NAME, WKR.SLAC_ID, P.EMPLOYEE_NAME AS WORKERNAME, LU1.LOOKUP_DESC AS CURRENTSTATUS 
                                        From Lst_Worker_Facility_Map Wfm 
                                        Left Join Lst_Lookup Lu On Lu.Lookup_Id = Wfm.Worker_Type_Id 
                                        Left Join Lst_Facility Fac On Fac.Facility_Id = Wfm.Facility_Id
                                        Left Join Lst_Worker Wkr On Wkr.Worker_Id = Wfm.Worker_Id
                                        Left Join Vw_People P On Wkr.Slac_Id = P.Employee_Id
                                         Left Join Lst_Lookup Lu1 On Lu1.Lookup_Id = Wfm.Status_Id 
                                         Where Wfm.Status_Id In (1, 2) And Wfm.Worker_Id = :WorkerId And
                                         Lu.Lookup_Group='WorkerType' AND LU1.LOOKUP_GROUP='Status' and wfm.is_active='Y' order by wfm.map_id");
                _cmdPending.Parameters.Add(":WorkerId", OracleDbType.Int32).Value = workerId;
                try
                {
                    OracleDataReader _drPending = objData.GetReader(_sbPending.ToString(), _cmdPending);
                    if (_drPending.HasRows)
                    {
                        return _drPending;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Pending request details failed with the following exception " + e);
                    return null;
                }

            }
        }

        public OracleDataReader GetApprovalHistory(int mapId)
        {
            using (OracleCommand _cmdAppHis = new OracleCommand())
            {
                string _sqlAppHis = @"Select Wf.Approval_Id,DeCode(Wf.Approver_Type,'SLSO','SLSO','LSO','LSO','ADMSVR','ADMIN SPVR') APPROVER_TYPE, Wf.Approver_Id, Wf.Approved_On, Wf.Status_Id, Wf.Comments, 
                                    pc.employee_name as Approver, lp.lookup_desc as status
                                    From Lst_Approval_Workflow  Wf
                                    Left Join Vw_People Pc On Wf.Approver_Id= Pc.employee_Id
                                    left join lst_lookup lp on lp.lookup_id = wf.status_id
                                    where wf.map_id=:MapId";
                _cmdAppHis.Parameters.Add(":MapId", OracleDbType.Int32).Value = mapId;
                try
                {
                    OracleDataReader _drApphis = objData.GetReader(_sqlAppHis, _cmdAppHis);
                    if (_drApphis.HasRows)
                    {
                        return _drApphis;
                    }
                    else
                    {
                        return null;
                    }

                }
                catch (Exception e)
                {
                    Log.Error("Approval History details failed with the following exception " + e);
                    return null;
                }

            }
        }

        public Business.WorkerFacility GetWorkerFacilityDetails(int mapId)
        {
            string _sqlWkrFac = "";
            Business.WorkerFacility objWorkerFac = new Business.WorkerFacility();

            _sqlWkrFac = "SELECT * FROM VW_LST_WORKER_FACILITY WHERE MAP_ID = :MapId";

            using (OracleCommand _cmdWorkerFac = new OracleCommand())
            {
                _cmdWorkerFac.Parameters.Add(":MapId", OracleDbType.Int32).Value = mapId;
                using (OracleDataReader _drWorkerFac = objData.GetReader(_sqlWkrFac, _cmdWorkerFac))
                {
                    while (_drWorkerFac.Read())
                    {
                        objWorkerFac.MapId = Convert.ToInt32(_drWorkerFac["MAP_ID"]);
                        objWorkerFac.WorkerId = Convert.ToInt32(_drWorkerFac["WORKER_ID"]);
                        objWorkerFac.FacilityId = (_drWorkerFac["FACILITY_ID"] != System.DBNull.Value) ? Convert.ToInt32(_drWorkerFac["FACILITY_ID"]) : 0;
                        objWorkerFac.WorkTypeId = Convert.ToInt32(_drWorkerFac["WORKER_TYPE_ID"]);
                        objWorkerFac.ConditionalApproval = objCommon.FixDBNull(_drWorkerFac["CONDITIONAL_APPROVAL"]);
                        objWorkerFac.Justification = objCommon.FixDBNull(_drWorkerFac["JUSTIFICATION"]);
                        objWorkerFac.SOPReviewed = objCommon.FixDBNull(_drWorkerFac["SOP_REVIEWED"]);
                        objWorkerFac.SOPReviewDate = (_drWorkerFac["SOP_REVIEW_DATE"] != System.DBNull.Value) ? Convert.ToDateTime(_drWorkerFac["SOP_REVIEW_DATE"]) : DateTime.MinValue;
                        objWorkerFac.OJTCompletionDate = (_drWorkerFac["OJT_COMPLETION_DATE"] != System.DBNull.Value) ? Convert.ToDateTime(_drWorkerFac["OJT_COMPLETION_DATE"]) : DateTime.MinValue;
                        objWorkerFac.FacilityName = objCommon.FixDBNull(_drWorkerFac["FACILITY_NAME"]);
                        objWorkerFac.Worker = objCommon.FixDBNull(_drWorkerFac["WORKER"]);
                        objWorkerFac.WorkType = objCommon.FixDBNull(_drWorkerFac["WORKTYPE"]);
                        objWorkerFac.Status = objCommon.FixDBNull(_drWorkerFac["STATUS"]);
                        objWorkerFac.StatusId = Convert.ToInt32(_drWorkerFac["STATUS_ID"]);
                       }
                    return objWorkerFac;
                }
            }
        }

        public int UpdateApproval(Business.Approval objApproval, DateTime OJTCompletiondate)
        {
            int _returnCode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_APPROVAL_PKG.Proc_Upd_Approval";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("Pi_Map_Id", OracleDbType.Int32).Value = objApproval.MapId;
                        _ocmd.Parameters["Pi_Map_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Comments", OracleDbType.Varchar2).Value = objApproval.Comments;
                        _ocmd.Parameters["Pi_Comments"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Status_Id", OracleDbType.Int32).Value = objApproval.StatusId;
                        _ocmd.Parameters["Pi_Status_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Approver_Id", OracleDbType.Int32).Value = objApproval.ApproverId;
                        _ocmd.Parameters["Pi_Approver_Id"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Approver_Type", OracleDbType.Varchar2).Value = objApproval.ApproverType;
                        _ocmd.Parameters["Pi_Approver_Type"].Direction = ParameterDirection.Input;

                        if (OJTCompletiondate != DateTime.MinValue)
                        {
                            _ocmd.Parameters.Add("Pi_OJT_Completion_Date", OracleDbType.Date).Value = OJTCompletiondate;
                        }
                        else
                        {
                            _ocmd.Parameters.Add("Pi_OJT_Completion_Date", OracleDbType.Date).Value = System.DBNull.Value;
                        }
                        _ocmd.Parameters["Pi_OJT_Completion_Date"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PO_RETURN_CODE", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        //return code 0 is considered successful. If other than 0, it is a failure.
                        if (_ocmd.Parameters["PO_RETURN_CODE"].Value != null)
                        {
                            _returnCode = Convert.ToInt32(_ocmd.Parameters["PO_RETURN_CODE"].Value.ToString());
                        }
                        else { _returnCode = -1; }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Update Approval failed with the following exception: " + e);
                        _returnCode = -1;
                    }
                }
            }
            return _returnCode;
        }

        public int GetStatusId(int mapId)
        {
            string _sqlStat = "SELECT STATUS_ID FROM LST_WORKER_FACILITY_MAP WHERE MAP_ID = :MapId and IS_ACTIVE='Y'";
            int _statusId = 0;

            using (OracleCommand _cmdStat = new OracleCommand())
            {
                _cmdStat.Parameters.Add(":MapId", OracleDbType.Varchar2).Value = mapId;

                using (OracleDataReader _drWorker = objData.GetReader(_sqlStat, _cmdStat))
                {
                    while (_drWorker.Read())
                    {
                        _statusId = Convert.ToInt32(objCommon.FixDBNull(_drWorker[0]));
                    }
                }
                return _statusId;
            }

        }
        #endregion

        #region "Reports"
      
        public int GetCountforReport(int worktypeId, int statusId)
        {
            int _count = 0;
            string _sqlCount = "SELECT COUNT(distinct worker_id) FROM LST_WORKER_FACILITY_MAP WHERE IS_ACTIVE='Y' AND WORKER_TYPE_ID=:WorkTypeId ";

            using (OracleCommand _cmdCount = new OracleCommand())
            {
                _cmdCount.Parameters.Add(":WorkTypeId", OracleDbType.Int32).Value = worktypeId;
                if (statusId == 5)
                {
                    _sqlCount += " AND STATUS_ID IN (5,6) AND WORKER_ID IN (SELECT WORKER_ID FROM LST_WORKER WHERE STATUS_ID=5 AND IS_ACTIVE ='Y') ";
                   // _cmdCount.Parameters.Add(":StatusId", OracleType.Number).Value = statusId;
                }
                else if (statusId == 6)
                {
                    _sqlCount += " AND STATUS_ID IN (5,6) AND WORKER_ID IN (SELECT WORKER_ID FROM LST_WORKER WHERE STATUS_ID = 6 AND IS_ACTIVE='Y')";
                }
                else { _sqlCount += " AND STATUS_ID IN (5,6) AND WORKER_ID IN (SELECT WORKER_ID FROM LST_WORKER WHERE STATUS_ID IN (5,6) AND IS_ACTIVE='Y')"; }

                using (OracleDataReader _drCount = objData.GetReader(_sqlCount, _cmdCount))
                {
                    if (_drCount.HasRows)
                    {
                        while (_drCount.Read())
                        {
                            _count = Convert.ToInt32(_drCount[0]);
                        }
                    }
                }
                return _count;
            }
        }

     
        public int GetCountWorker253219()
        {
            int _count = 0;
            string _sqlcount = @"SELECT count(*) FROM
                (select PC.NAME as Worker
                from lst_worker W LEFT JOIN PERSONS.PERSON PC ON PC.KEY = W.SLAC_ID
                where ((trunc(To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'253','DATE'),'MM/dd/yyyy')) - trunc(sysdate) <= -1)
                or ((To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'219','DATE'),'MM/dd/yyyy') IS  NULL AND
                To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'219R','DATE'),'MM/dd/yyyy') IS NULL AND
                To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'396','DATE'),'MM/dd/yyyy') IS NULL)
                OR
                (
                ((trunc(To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'219','DATE'),'MM/dd/yyyy')) - trunc(sysdate) <= -1) OR 
                (To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'219','DATE'),'MM/dd/yyyy') IS NULL)) and 
                ((trunc(To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'219R','DATE'),'MM/dd/yyyy')) - trunc(sysdate) <= -1) OR 
                (To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'219R','DATE'),'MM/dd/yyyy') IS NULL)) and  
                ((trunc(To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'396','DATE'),'MM/dd/yyyy')) - trunc(sysdate) <= -1) OR 
                (To_date(Trainsafe.Get_Non_Sta_Trn_Status(slac_id,'396','DATE'),'MM/dd/yyyy') IS NULL)))))
                and status_id in (5,6) and is_active='Y') WHERE WORKER IS NOT NULL";
            using (OracleCommand _cmdCount = new OracleCommand())
            { 
                using (OracleDataReader _drCount = objData.GetReader(_sqlcount))
                {
                    if (_drCount.HasRows)
                    {
                        while (_drCount.Read())
                        {
                            _count = Convert.ToInt32(_drCount[0]);
                        }
                        
                    }
                }
            }
            return _count;
        }
        #endregion

        #region "Email"
       
        public int CustomEmail(Business.CustomEmail objEmail)
        {    
               int _errCode;
            using (OracleConnection _oconEmail = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmdEmail = new OracleCommand())
                {
                        try
                        {
                                _ocmdEmail.Connection = _oconEmail;
                                _oconEmail.Open();
                                _ocmdEmail.CommandText = "LST_EMAIL_PKG.LST_CUSTOM_EMAIL";
                                _ocmdEmail.CommandType = CommandType.StoredProcedure;

                                if (objEmail.FacId == 0)
                                {
                                    _ocmdEmail.Parameters.Add("PI_TO_FACID", OracleDbType.Int32).Value = System.DBNull.Value;
                                }
                                else
                                {
                                    _ocmdEmail.Parameters.Add("PI_TO_FACID", OracleDbType.Int32).Value = objEmail.FacId;
                                }
                               
                                _ocmdEmail.Parameters["PI_TO_FACID"].Direction = ParameterDirection.Input;

                                _ocmdEmail.Parameters.Add("PI_TO_LISTID", OracleDbType.Int32).Value = objEmail.ListId;
                                _ocmdEmail.Parameters["PI_TO_LISTID"].Direction = ParameterDirection.Input;

                                _ocmdEmail.Parameters.Add("PI_SUBJECT", OracleDbType.Varchar2).Value = objEmail.Subject;
                                _ocmdEmail.Parameters["PI_SUBJECT"].Direction = ParameterDirection.Input;

                                _ocmdEmail.Parameters.Add("PI_BODY", OracleDbType.Varchar2).Value = objEmail.BodyMsg;
                                _ocmdEmail.Parameters["PI_BODY"].Direction = ParameterDirection.Input;

                                _ocmdEmail.Parameters.Add("PI_CREATED_BY", OracleDbType.Varchar2).Value = objEmail.CreatedBy;
                                _ocmdEmail.Parameters["PI_CREATED_BY"].Direction = ParameterDirection.Input;

                                _ocmdEmail.Parameters.Add("PO_RETURN_CODE", OracleDbType.Int32);
                                _ocmdEmail.Parameters["PO_RETURN_CODE"].Direction = ParameterDirection.Output;

                                _ocmdEmail.ExecuteNonQuery();

                             

                                if ((!string.IsNullOrEmpty(_ocmdEmail.Parameters["Po_Return_Code"].Value.ToString())) && (_ocmdEmail.Parameters["Po_Return_Code"].Value.ToString() != ""))
                                {
                                    _errCode = Convert.ToInt32(_ocmdEmail.Parameters["Po_Return_Code"].Value.ToString());
                                }
                                else
                                {
                                    _errCode = -1;
                                }

                                

                            }
                            catch (OracleException ex)
                            {
                                Log.Error("Error sending Email - CustomEmail " + ex );
                                _errCode = -1;
                            }
                            return _errCode;
                }
            }
        }

        #endregion

        #region "OJT"
        public string UpdateFieldMatrix(int mapId)
        {
            string _sqlUpdate = "";
           
            string _result = "";
            int _result2 = 0;
            _sqlUpdate = " UPDATE LST_OJT_FIELDMATRIX SET IS_ACTIVE='N' WHERE WORKERFAC_MAPID = :MapId ";
            
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand(_sqlUpdate, _ocon))
                {
                    _ocmd.CommandType = CommandType.Text;

                    OracleParameter prmMapid = new OracleParameter();
                    prmMapid.ParameterName = ":MapId";
                    prmMapid.OracleDbType =  OracleDbType.Int32;
                    prmMapid.Value = mapId;

                    _ocmd.Parameters.Add(prmMapid);
                    _ocon.Open();

                    try
                    {
                        _ocmd.ExecuteNonQuery();
                        _result2 = UpdateLatestDate(mapId);
                        _result = "0";
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        _result = "-1";
                    }
                    return _result;
                }
            }
        }

        public int UpdateLatestDate(int mapId)
        {
            int _returncode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.PROC_UPD_OJTLATEST_DATE";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("PI_MAP_ID", OracleDbType.Int32).Value = mapId;
                        _ocmd.Parameters["PI_MAP_ID"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Modified_By", OracleDbType.Varchar2).Value = objCommon.GetUserId();
                        _ocmd.Parameters["Pi_Modified_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PO_RETURN_CODE",  OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        if (_ocmd.Parameters["PO_RETURN_CODE"].Value != null)
                        {
                            _returncode = Convert.ToInt32(_ocmd.Parameters["PO_RETURN_CODE"].Value.ToString());
                        }
                        else { _returncode = -1; }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Update Facility failed with the following exception: " + e);
                        _returncode = -1;
                    }
                }

            }
            return _returncode;
        }

       

        public int InsertFieldMatrix(Business.OJTFieldMatrix objFM)
        {
            string _sqlInsert = "";
            int _matrixId = 0;

            _sqlInsert = @"INSERT INTO LST_OJT_FIELDMATRIX (WORKERFAC_MAPID, OJT_FIELD_ID, FM_STRING_VALUE, IS_ACTIVE)
                            values(:MapId, :FieldId, :FieldValue,'Y') returning FIELD_MATRIX_ID INTO :FieldMatrixId";
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand(_sqlInsert, _ocon))
                {
                    _ocmd.CommandType = CommandType.Text;

                    OracleParameter prmMapId = new OracleParameter();
                    prmMapId.ParameterName = ":MapId";
                    prmMapId.OracleDbType = OracleDbType.Int32;
                    prmMapId.Value = objFM.WorkerFacMapId;

                    OracleParameter prmFieldId = new OracleParameter();
                    prmFieldId.ParameterName = ":FieldId";
                    prmFieldId.OracleDbType = OracleDbType.Int32;
                    prmFieldId.Value = objFM.OJTFieldId;

                    OracleParameter prmFieldValue = new OracleParameter();
                    prmFieldValue.ParameterName = ":FieldValue";
                    prmFieldValue.OracleDbType = OracleDbType.Varchar2;
                    prmFieldValue.Value = objFM.FieldValue;

                    OracleParameter prmMatrixId = new OracleParameter();
                    prmMatrixId.ParameterName = ":FieldMatrixId";
                    prmMatrixId.OracleDbType = OracleDbType.Int32;
                    prmMatrixId.Direction = ParameterDirection.Output;
                    prmMatrixId.Value = objFM.FieldMatrixId;

                    _ocmd.Parameters.Add(prmMapId);
                    _ocmd.Parameters.Add(prmFieldId);
                    _ocmd.Parameters.Add(prmFieldValue);
                    _ocmd.Parameters.Add(prmMatrixId);
                    _ocon.Open();

                    try
                    {
                        _ocmd.ExecuteNonQuery();
                        _matrixId = Convert.ToInt32(_ocmd.Parameters[":FieldMatrixId"].Value.ToString());

                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        _matrixId = -1;
                    }
                }
                return _matrixId;
            }

        }

        public  DataSet GetOJTFields(int facId, string filter)
        {
            using (OracleCommand _cmdOJT = new OracleCommand())
            {
                string _sqlOJT = @"select field_id,column_label from lst_ojt_fields where 
                                        field_id in (select field_id from LST_FIELDS_FACILITY_MAP
                                        WHERE FACILITY_ID = :FacId and is_active='Y')" + filter;
                _cmdOJT.Parameters.Add(":FacId", OracleDbType.Int32).Value = facId;
                try
                {
                    _dsList = objData.ReturnDataset(_sqlOJT, "ojtfields", _cmdOJT);
                    return _dsList;
                }
                catch (Exception e)
                {
                    Log.Error("Getting OJT Fields with the following exception " + e);
                    return null;
                }

            }
        }

        public int CreateOJTFields(Business.OJTFields objOJTField, int facid)
        {
            int _result = 0;

            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try{
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.PROC_INS_OJTFIELD";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("PI_FACILITY_ID", OracleDbType.Int32).Value = facid;
                        _ocmd.Parameters["PI_FACILITY_ID"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PI_COLUMN_LABEL", OracleDbType.Varchar2).Value = objOJTField.Columnlabel;
                        _ocmd.Parameters["PI_COLUMN_LABEL"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PI_FIELD_ID_EXST", OracleDbType.Int32).Value = objOJTField.FieldId;
                        _ocmd.Parameters["PI_FIELD_ID_EXST"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PI_CREATED_BY",  OracleDbType.Varchar2).Value = objOJTField.CreatedBy;
                        _ocmd.Parameters["PI_CREATED_BY"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PO_RETURN_CODE", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        if (_ocmd.Parameters["PO_RETURN_CODE"].Value != null)
                        {
                            _result = Convert.ToInt32(_ocmd.Parameters["PO_RETURN_CODE"].Value.ToString());
                            if (_result != 0)
                            {
                                Log.Error("Creating OJT fields for facility " + facid + " failed with the following sql error: " + _result);
                            }
                        }
                        else
                        {
                            Log.Error("Creating OJT fields for facility " + facid + " failed with no error code");
                            _result = -1;
                        }
                        
                    }
                    catch(Exception e)
                    {
                        Log.Error(" Creating OJT fields for facility " + facid + " failed with exception: " + e);
                        _result = -1;
                    }
                }
            }
            return _result;
        }

        public int DeleteOJTField(Business.OJTFields objOJTField, int facid)
        {
            int _result = 0;

            using (OracleConnection _oCon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _oCon;
                        _oCon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.PROC_DEL_OJTFIELD";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("PI_FACILITY_ID", OracleDbType.Int32).Value = facid;
                        _ocmd.Parameters["PI_FACILITY_ID"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PI_FIELD_ID", OracleDbType.Int32).Value = objOJTField.FieldId;
                        _ocmd.Parameters["PI_FIELD_ID"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PI_MODIFIED_BY", OracleDbType.Varchar2).Value = objOJTField.CreatedBy;
                        _ocmd.Parameters["PI_MODIFIED_BY"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("po_RETURN_CODE", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        if (_ocmd.Parameters["PO_RETURN_CODE"].Value != null)
                        {
                           
                            _result = Convert.ToInt32(_ocmd.Parameters["PO_RETURN_CODE"].Value.ToString());
                            if (_result != 0)
                            {
                                Log.Error(" Deleting OJt Field failed with following sql error: " + _result);
                            }
                            
                        }
                        else
                        {
                            Log.Error(" Deleting OJt Field failed with no error code: " );
                            _result = -1;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(" Deleting OJt Field failed with following exception: "  + e);
                        _result = -1;
                    }
                    
                }
            }
            return _result;
        }

        

        public bool CheckifOJTFieldExists(string colName, int fieldId = 0)
        {
            string _sqlCheck = "";
            int _count = 0;

            _sqlCheck = "SELECT COUNT(*) FROM LST_OJT_FIELDS WHERE (REPLACE(LOWER(COLUMN_LABEL), ' ' ,'')) =:ColName and IS_ACTIVE='Y'";

           
            using(OracleCommand _cmdCheck = new OracleCommand())
            {
                _cmdCheck.Parameters.Add(":ColName", colName);
                if (fieldId > 0)
                {
                    _sqlCheck += " AND FIELD_ID <> :FieldId";
                    _cmdCheck.Parameters.Add(":FieldId", fieldId);
                }
                using(OracleDataReader _drCheck = objData.GetReader(_sqlCheck, _cmdCheck))
                {
                    while (_drCheck.Read())
                    {
                        _count = Convert.ToInt32(objCommon.FixDBNull(_drCheck[0]));
                    }
                    if (_count > 0) return true;
                    else return false;
                }
            }
        }

        public DataSet GetOJTFieldsSummary(string filter)
        {
            string _sqlSum = "";
            DataSet _dsOJTFieldSum = new DataSet();

            using (OracleCommand _cmdSum = new OracleCommand())
            {

                _sqlSum = @" select fie.field_id, Fie.column_label,
                        (select ListAgg(facility_name, ',  ') within Group (order by facility_name) Cnt
                        from lst_facility fac join lst_fields_facility_map ff on fac.facility_id = ff.facility_id where ff.field_id = fie.field_id
                        and ff.is_active='Y') facilities
                        from lst_ojt_fields fie where fie.is_active='Y' " + filter;


                _dsOJTFieldSum = objData.ReturnDataset(_sqlSum, "ojtsum", _cmdSum);
            }
                return _dsOJTFieldSum;
        }

        public int UpdateOJTField(int fieldId, string columnLabel)
        {
            int _returncode;
            using (OracleConnection _ocon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _ocon;
                        _ocon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.PROC_UPD_OJTFIELD";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("PI_FIELD_ID", OracleDbType.Int32).Value = fieldId;
                        _ocmd.Parameters["PI_FIELD_ID"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PI_COLUMN_LABEL",  OracleDbType.Varchar2).Value = columnLabel;
                        _ocmd.Parameters["PI_COLUMN_LABEL"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("Pi_Modified_By", OracleDbType.Varchar2).Value = objCommon.GetUserId();
                        _ocmd.Parameters["Pi_Modified_By"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PO_RETURN_CODE", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        if (_ocmd.Parameters["PO_RETURN_CODE"].Value != null)
                        {
                            _returncode = Convert.ToInt32(_ocmd.Parameters["PO_RETURN_CODE"].Value.ToString());
                        }
                        else { _returncode = -1; }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Update OJT Field failed with the following exception: " + e);
                        _returncode = -1;
                    }
                }

            }
            return _returncode;
        }

        public int DeleteMasterOJTField(int fieldId)
        {
            int _result = 0;

            using (OracleConnection _oCon = new OracleConnection(objData.GetConnectionString()))
            {
                using (OracleCommand _ocmd = new OracleCommand())
                {
                    try
                    {
                        _ocmd.Connection = _oCon;
                        _oCon.Open();
                        _ocmd.CommandText = "LST_ADMINISTRATION_PKG.PROC_DEL_MAIN_OJTFIELD";
                        _ocmd.CommandType = CommandType.StoredProcedure;

                        _ocmd.Parameters.Add("PI_FIELD_ID", OracleDbType.Int32).Value = fieldId;
                        _ocmd.Parameters["PI_FIELD_ID"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("PI_MODIFIED_BY",  OracleDbType.Varchar2).Value = objCommon.GetUserId();
                        _ocmd.Parameters["PI_MODIFIED_BY"].Direction = ParameterDirection.Input;

                        _ocmd.Parameters.Add("po_RETURN_CODE", OracleDbType.Int32).Direction = ParameterDirection.Output;
                        _ocmd.ExecuteNonQuery();

                        if (_ocmd.Parameters["PO_RETURN_CODE"].Value != null)
                        {
                            _result = Convert.ToInt32(_ocmd.Parameters["PO_RETURN_CODE"].Value.ToString());
                        }
                        else
                        {
                            Log.Error(" Deleting Master OJt Field failed with no error code: ");
                            _result = -1;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(" Deleting Master OJt Field failed with following exception: " + e);
                        _result = -1;
                    }

                }
            }
            return _result;
        }

        //public bool CheckIfFieldbyFACExists(int fieldId)
        //{
        //    string _sqlCheck = "";
        //    int _count = 0;

        //    _sqlCheck = "SELECT COUNT(*) FROM LST_FIELDS_FACILITY_MAP WHERE FIELD_ID=:FieldId  and IS_ACTIVE='Y'";

        //    using (OracleCommand _cmdCheck = new OracleCommand())
        //    {
        //        _cmdCheck.Parameters.AddWithValue(":FieldId", fieldId);
        //        using (OracleDataReader _drCheck = objData.GetReader(_sqlCheck, _cmdCheck))
        //        {
        //            while (_drCheck.Read())
        //            {
        //                _count = Convert.ToInt32(objCommon.FixDBNull(_drCheck[0]));
        //            }
        //            if (_count > 0) return true;
        //            else return false;
        //        }
        //    }
        //}

        #endregion

    }
}
