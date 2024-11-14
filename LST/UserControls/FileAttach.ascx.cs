//$Header:$
//
//  FileAttach.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the usercontrol that file upload control to attach files.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace LST.UserControls
{
    
    public partial class FileAttach : System.Web.UI.UserControl
    {
        Data.File_Util objFile = new Data.File_Util();
        Business.Common_Util objCommon = new Business.Common_Util();
        Data.DML_Util objDml = new Data.DML_Util();
        public event EventHandler AttachButtonClicked;

        #region "Properties"
        public int ObjId
        {
            get
            {
                return(ViewState["objid"] != null) ? Convert.ToInt32(ViewState["objid"]) : 0;
            }
            set
            {
                ViewState["objid"] = value;
            }
        }

        public string Mode { get; set; }

        public string ObjType { get; set; }

        public string DocType { get; set; }
        #endregion

     
        protected void Page_Load(object sender, EventArgs e)
        {

           
        }

        private void OnAttachButtonClicked()
        {
            if (AttachButtonClicked != null)
            {
                AttachButtonClicked(this, EventArgs.Empty);
            }
           }

        protected void BtnAttach_Click(object sender, EventArgs e)
        {
            string _result="";

            if ((DocType == "OJT") && (objDml.CheckifOJTAttached(ObjId)))
            {
                LblMsg.Text = "Error! OJT already attached to this worker facility. Please refresh the grid to see the changes";
            }
            else
            {
                if (FileValidation(FUDocument.PostedFile))
                {
                    _result = GetFileData(FUDocument.PostedFile, ObjId, DocType);
                    if (_result == "-1")
                    {
                        //dialog open 
                        LblMsg.Text = "Error! File could not be attached. Please try later";
                    }
                    else
                    {
                        //Refresh parent grid to show the files
                        LblMsg.Text = "File successfully attached";
                    }
                }
           
            }
           
             
                if ((_result != "-1") && (_result != ""))
                {
                    OnAttachButtonClicked();
                }
                if (LblMsg.Text != "") { Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-msg');", true); }
                //else
                //{
                //    LblMsg.Text = "Hello! what just happened?";
                //    Page.ClientScript.RegisterStartupScript(this.GetType(), "key", "OpenDialog('dialog-msg');", true);
                //}
           
        }

        protected string GetFileData(HttpPostedFile m_objFile, int objId, string DocType)
        {
            string _extension;
            string _filename;
            string _content;
            string _fileid;
            Byte[] _filedata;
            int _filesize;
          
            _filename = objFile.FileFieldFilename(m_objFile);
            _extension = _filename.Substring(_filename.IndexOf("."));
            _content = objFile.FileFieldType(m_objFile);
            _filedata = objFile.GetByteArrayFromFileField(m_objFile);
            _filesize = objFile.FileFieldLength(m_objFile);

            try
            {
                _fileid = objDml.InsertFileData(objId, _filename, _filesize, _content, _filedata, objCommon.GetUserId(), ObjType,DocType);
                return _fileid.ToString();
            }
            catch
            {
                return "-1";
            }
        }

        protected bool FileValidation(HttpPostedFile postedFile)
        {
            string _fileName;
            string _fileExtn;

            StringBuilder _sbText = new StringBuilder();
            string[] _allowedextn = { ".doc", ".docx", ".jpg", ".bmp", ".pdf", ".xls", ".xlsx", ".png", ".txt", ".gif" };


            _fileName = Server.HtmlEncode(objFile.FileFieldFilename(postedFile));
            _sbText.Append("Error attaching the file ");


            if (!string.IsNullOrEmpty(_fileName))
            {
                _sbText.Append("~");
                _sbText.Append(_fileName);
                _sbText.Append("~ <br />");
                if (_fileName.Length > 90)
                {
                    _sbText.Append("Reason: File Name is too long. <br />");
                    _sbText.Append("Action: Pick a shorter name and try attaching the file/s again");
                    LblMsg.Text = _sbText.ToString();
                    return false;
                }
                if (_fileName.LastIndexOf(".") > -1)
                {
                    _fileExtn = _fileName.Substring(_fileName.LastIndexOf(".")).ToLower();
                }
                else { _fileExtn = ""; }

                if (_fileExtn == ".exe")
                {
                    _sbText.Append("Reason: Executable files cannot be uploaded.<br />");
                    _sbText.Append("Action: Pick a different file and try attaching it again");
                    LblMsg.Text = _sbText.ToString();
                    return false;
                }

                if (_fileExtn == "")
                {
                    _sbText.Append("Reason: Files with no extension cannot be uploaded.<br />");
                    _sbText.Append("Action: Pick a different file and try attaching it again");
                    LblMsg.Text = _sbText.ToString();
                    return false;

                }
                if (!(_allowedextn.Contains(_fileExtn)))
                {
                    _sbText.Append("Reason: File Type not allowed.<br />");
                    _sbText.Append("Action: Pick a file with allowed Type and try attaching it again");
                    LblMsg.Text = _sbText.ToString();
                    return false;
                }


                if (!(postedFile.FileName == "" || postedFile.ContentLength < 1))
                {
                    if ((postedFile.ContentLength / 1024) > 10240)
                    {
                        _sbText.Append("Reason: File size exceeded the allowed limit. <br />");
                        _sbText.Append("Action: Pick a file with allowed size and try attaching the file again");                
                        LblMsg.Text = _sbText.ToString();
                        return false;
                    }
                }
                return true;
            }
            else
            {
                _sbText.Append("<br />");
                _sbText.Append("Reason: There is no file to attach. <br />");
                _sbText.Append("Action: Pick a file with allowed size and try attaching the file");
                LblMsg.Text = _sbText.ToString();
               return false;

            }
           
          
        }

      
    }
}