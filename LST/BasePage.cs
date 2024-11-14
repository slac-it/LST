//$Header:$
//
//  Basepage.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the Base page of Laser safety tool.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;


namespace LST
{
    public class BasePage: System.Web.UI.Page
    {
        Data.DML_Util objDml = new Data.DML_Util();
        Business.UserRoles objRoles = new Business.UserRoles();
        Business.Common_Util objCommon = new Business.Common_Util();

     


        protected void FillRoom(string bldgNo)
        {
            using (DataSet _dsRoom =  objDml.GetRoomNos(bldgNo))
            {
                DropDownList DdlRoom = (DropDownList)FindControlRecursive("DdlRoom");

                DdlRoom.Items.Clear();
                DdlRoom.DataValueField = "ROOM_NUMBER";
                DdlRoom.DataTextField = "ROOM_NUMBER";
                DdlRoom.DataSource = _dsRoom.Tables["room"];
                DdlRoom.DataBind(); 
            }
        }

        protected virtual Control FindControlRecursive(string id)
        {
           // return FindControlRecursive(id, this);
            return FindControlRecursive(id, this);
        }

        protected virtual Control FindControlRecursive(string id, Control parent)
        {
            // If parent is the control we're looking for, return it
            if (string.Compare(parent.ID, id, true) == 0)
                return parent;
            // Search through children
            foreach (Control child in parent.Controls)
            {
                Control match = FindControlRecursive(id, child);
                if (match != null)
                    return match;
            }
            // If we reach here then no control with id was found
            return null;
        }

        public void FileData(int id, string objType)
        {
            int sfileid = 0;
            string sfilename = null;
            int sfilesize = 0;
            string scontent = null;
            byte[] sfiledata = null;
           // int _facilityId = 0;
            //string[] scontentsplit = null; 
            using (OracleDataReader drFileinfo = objDml.GetFileInfoById(id, objType))
            {

                while (drFileinfo.Read())
                {
                    sfileid = Convert.ToInt32(drFileinfo["ATTACHMENT_ID"]);
                    sfilename = (string)drFileinfo["FILE_NAME"];
                    sfilesize = Convert.ToInt32(drFileinfo["FILE_SIZE"]);
                    scontent = (string)drFileinfo["FILE_CONTENT_TYPE"];
                    sfiledata = (byte[])drFileinfo["FILE_DATA"];
                    DeliverFile(sfiledata, scontent, sfilesize, sfilename);
                }

            }
        }

        public void DeliverFile(byte[] Data, string Type, int Length, string DownloadFileName)
        {
            Page.Response.ClearHeaders();
            Page.Response.Clear();
            Page.Response.ContentType = Type;
            if (!string.IsNullOrEmpty(DownloadFileName))
            {
                //Add filename to header 

                Page.Response.AddHeader("Connection", "keep-alive");
                Page.Response.AddHeader("Content-Length", Convert.ToString(Length));

                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + DownloadFileName + "\"");

                switch (Type)
                {
                    case "False":
                        Page.Response.ContentType = "application/octet-stream";
                        Page.Response.Charset = "UTF-8";
                        break;
                    default:
                        Page.Response.ContentType = Type;
                        break;
                }
            }
            Page.Response.OutputStream.Write(Data, 0, Length);
            Page.Response.End();
        }
    }
}