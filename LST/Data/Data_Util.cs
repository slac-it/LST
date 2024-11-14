//$Header:$
//
//  Data_Util.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the utility class that has all the functions to connect to the database
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Data.OracleClient;
using System.Configuration;
using System.Data;
using log4net;
using Oracle.ManagedDataAccess.Client;


namespace LST.Data
{
    public class Data_Util
    {
        private string _con;
        private string _errMsg;
        private string _errCode;
        //private OracleConnection _ocon;
        //private OracleCommand _ocmd;
        //private OracleTransaction _otran;
        private string _spName;

       private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string SPName
        {
            get { return _spName; }
            set { _spName = value; }
        }

        //public OracleCommand CmdName
        //{
        //    get { return _ocmd; }
        //    set { _ocmd = value; }
        //}

        //public OracleConnection ConName
        //{
        //    get { return _ocon; }
        //    set { _ocon = value; }
        //}

        public string GetConnectionString()
        {

            _con = ConfigurationManager.ConnectionStrings["TCP_WEB"].ConnectionString;
            return _con;
        }



        //public void ConnectToDB(bool tflag)
        //{

        //    try
        //    {
        //        _ocon = new OracleConnection();
        //        // ConName.ConnectionString = GetConnectionString(cname);
        //        _ocon.ConnectionString = GetConnectionString();
        //        _ocon.Open();

        //        if (tflag)
        //        {
        //            _otran = _ocon.BeginTransaction();
        //            _ocmd = new OracleCommand("", _ocon);
        //            _ocmd.Transaction = _otran;
        //        }
        //        else
        //        {
        //            _ocmd = new OracleCommand("", _ocon);
        //        }

        //    }
        //    catch (OracleException oraex)
        //    {
        //        Log.Error(oraex);
        //        _errMsg = oraex.Message;
        //        //_errCode = oraex.Code.ToString();
        //        _errCode = oraex.ErrorCode.ToString();

        //    }
        //}

        //public void DisconnectDB()
        //{
        //    if (_ocon.State == ConnectionState.Open)
        //    {
        //        _ocon.Close();
        //        _ocmd.Dispose();
        //    }
        //}

        public OracleDataReader GetReader(string sqlText)
        {
            OracleDataReader _odr;
            OracleConnection _ocon = new OracleConnection();
            OracleCommand _ocmd = _ocon.CreateCommand();
 
                        try
                        {
                            _ocon.ConnectionString = GetConnectionString();
                            _ocon.Open();
                            _ocmd.Connection = _ocon;
                                _ocmd.CommandText = sqlText;
                                _ocmd.BindByName = true;
                                _odr = _ocmd.ExecuteReader(CommandBehavior.CloseConnection);
                                return _odr;
                          }
                            catch (OracleException ex)
                            {
                                Log.Error(ex);
                                _errMsg = ex.Message.ToString();
                                // _errCode = ex.Code.ToString();
                                _errCode = ex.ErrorCode.ToString();
                                return null;
                            }
 
        }

        public OracleDataReader GetReader(string sqlText, OracleCommand cmdPm)
        {
            OracleDataReader _odr;
            OracleConnection _ocon = new OracleConnection();

                try
                {
                    _ocon.ConnectionString = GetConnectionString();
                    _ocon.Open();
                    cmdPm.Connection = _ocon;
                    cmdPm.CommandText = sqlText;
                    cmdPm.BindByName = true;
                    _odr = cmdPm.ExecuteReader(CommandBehavior.CloseConnection);
                    return _odr;
                }
                catch (OracleException ex)
                {
                    Log.Error(ex);
                    _errMsg = ex.Message.ToString();
                    //  _errCode = ex.Code.ToString();
                    _errCode = ex.ErrorCode.ToString();
                    return null;
                }
        }


        //public DataSet ReturnDataset(string sqlText, string tableName)
        //{
        //    DataSet _ods = new DataSet();
        //    OracleDataAdapter _oda;

        //    try
        //    {
        //        ConnectToDB(false);
        //        _ocmd.CommandText = sqlText;
        //        _ocmd.BindByName = true;
        //        _oda = new OracleDataAdapter(_ocmd);
        //        _oda.Fill(_ods, tableName);
        //        return _ods;
        //    }
        //    catch (OracleException oraEx)
        //    {
        //        Log.Error(oraEx);
        //        _errMsg = oraEx.Message.ToString();
        //        //_errCode = oraEx.Code.ToString();
        //        _errCode = oraEx.ErrorCode.ToString();
        //        return null;
        //    }
        //    finally
        //    {
        //        DisconnectDB();
        //    }
        //}

        public DataSet ReturnDataset(string sqlText, string tableName, OracleCommand cmdPm)
        {
            DataSet _ods = new DataSet();
            //OracleDataAdapter _oda;

            using (OracleConnection _ocon = new OracleConnection(GetConnectionString()))
            {
                try
                {
                    _ocon.Open();
                    cmdPm.Connection = _ocon;                   
                    cmdPm.CommandText = sqlText;
                    cmdPm.BindByName = true;
                    using (OracleDataAdapter _oda = new OracleDataAdapter(cmdPm))
                    {
                        _oda.Fill(_ods, tableName);
                        return _ods;
                    }
                }
                catch (OracleException oraEx)
                {
                    Log.Error(oraEx);
                    _errMsg = oraEx.Message.ToString();
                    // _errCode = oraEx.Code.ToString();
                    _errCode = oraEx.ErrorCode.ToString();
                    return null;
                }
               
            }
                //try
                //{
                //    _ocon = new OracleConnection();

                //    _ocon.ConnectionString = GetConnectionString();
                //    _ocon.Open();
                //    cmdPm.Connection = _ocon;
                //    cmdPm.CommandText = sqlText;
                //    cmdPm.BindByName = true;

                //    _oda = new OracleDataAdapter(cmdPm);
                //    _oda.Fill(_ods, tableName);
                //    return _ods;
                //}
                //catch (OracleException oraEx)
                //{
                //    Log.Error(oraEx);
                //    _errMsg = oraEx.Message.ToString();
                //    // _errCode = oraEx.Code.ToString();
                //    _errCode = oraEx.ErrorCode.ToString();
                //    return null;
                //}
                //finally
                //{
                //    if (_ocon.State == ConnectionState.Open)
                //    {
                //        _ocon.Close();

                //    }

                //}
        }

        //public DataTable ReturnDataTable(string sqlText, OracleCommand cmdPm)
        //{
        //    DataTable _dt = new DataTable();
        //    OracleDataAdapter _oda;

        //    try
        //    {
        //        _ocon = new OracleConnection();

        //        _ocon.ConnectionString = GetConnectionString();
        //        _ocon.Open();
        //        cmdPm.Connection = _ocon;
        //        cmdPm.CommandText = sqlText;
        //        cmdPm.BindByName = true;
        //        _oda = new OracleDataAdapter(cmdPm);
        //        _oda.Fill(_dt);
        //        return _dt;
        //    }
        //    catch (OracleException oraEx)
        //    {
        //         Log.Error(oraEx);
        //        _errMsg = oraEx.Message.ToString();
        //        // _errCode = oraEx.Code.ToString();
        //        _errCode = oraEx.ErrorCode.ToString();
        //        return null;
        //    }
        //    finally
        //    {
        //        if (_ocon.State == ConnectionState.Open)
        //        {
        //            _ocon.Close();

        //        }

        //    }
        //}
        //public OracleDataReader GetMultiresult()
        //{
        //    OracleConnection _ocon = new OracleConnection();
        //    OracleCommand _ocmd = new OracleCommand();
  
        //            try
        //            {
        //                _ocon.ConnectionString = GetConnectionString();
        //                _ocon.Open();
        //                 _ocmd.Connection = _ocon;                      
        //                _ocmd.CommandText = SPName;
        //                _ocmd.CommandType = CommandType.StoredProcedure;
        //                 _ocmd.BindByName = true;
        //                _ocmd.Parameters.Add(new OracleParameter("LabCur", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
        //                _ocmd.Parameters.Add(new OracleParameter("BldgCur", OracleDbType.RefCursor)).Direction = ParameterDirection.Output;
        //                OracleDataReader Rdr = _ocmd.ExecuteReader();
        //                return Rdr;

        //            }
        //            catch (OracleException e)
        //            {
        //                Log.Error(e);
        //                return null;
        //            }
 
               
        //}

       
      

    }
}