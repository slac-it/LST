<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/LST.Master" CodeBehind="WorkerEntry.aspx.cs" Inherits="LST.WorkerEntry"  StylesheetTheme="LSTTheme"%>
<%@ Register Src="~/UserControls/FileAttach.ascx" TagName="UCFileWrkr" TagPrefix="UC1" %>
<%@ Register Src="~/UserControls/FileSummary.ascx" TagName="UCFileList" TagPrefix="UC2" %>
<%@ Register Src="~/UserControls/TrainingSummary.ascx" TagName="UCTraining" TagPrefix="UC3" %>
<%@ Register Src="~/UserControls/UserLabSummary.ascx" TagName="UCUserLab" TagPrefix="UC4" %>
<%@ Register  Src="~/UserControls/WorkerPendingRequestSummary.ascx" TagName="UCPending" TagPrefix="UC5" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" src="Scripts/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/LST.js"></script>
   
    <script type="text/javascript">
        function ResetTextbox() {
            $('#TxtWorker').trigger('change');
        }

        function DropdownValidation(source, arguments) {
           
            var ctrl = document.forms[0].elements["DdlSlacAffiliation"].value;

            if (ctrl == "-1")
            {
                arguments.IsValid = false;
                return false;
            }
            else
            {
                return true;
            }
        }

        //window.onbeforeunload = function (event) {
        //    var message = 'Important: You have unsaved data on this page. Please click on \'Submit\' or \'Update\' button to save your data.';
        //    if (typeof event == 'undefined') {
        //        event = window.event;
               
        //    }
        //    if (event) {
        //        event.returnValue = message;
        //    }
        //    return message;
        //};

        $(document).ready(function () {
            needToConfirm = false;
            window.onbeforeunload = askConfirm;
        });

        function askConfirm() {
            var mode = $("#HdnMode").val();
            if (mode == "edit") { needToConfirm = true; }
            if (needToConfirm) {
                // Put your custom message here 
                return "Important: You have unsaved data on this page. Please click on \'Update\' button to save your data";
            }
        }

        $(function () {
            $(".nounload").click(function () {
                window.onbeforeunload = null;
            });

        });

        function RefreshGrid()
        {
            window.location.reload();
        }

        

    </script>
  
     <asp:Panel ID="PnlAdd" runat="server" Visible="true" GroupingText="Worker Information">
          <div class="row">
              <div class="col-xs-6 rightalign">
                  <span class="spanrequired">*</span> indicates required field
              </div>
              <div class="col-xs-6 leftalign">
                    <div id="divAdd" runat="server" visible="true">
                        <asp:Button cssClass="btn btn-primary nounload" Text="Submit" runat="server" id="BtnSubmit" OnClick="BtnSubmit_Click" ValidationGroup="add" /> &nbsp;
                        <asp:Button CssClass="btn btn-primary" Text="Cancel" runat="server" ID="BtnCancel" OnClick="BtnCancel_Click" /> &nbsp;
                    </div>
                    <div id="divEdit" runat="server" visible="false">
                        <asp:Button cssClass="btn btn-primary nounload" Text="Update" runat="server" id="BtnUpdate"  ValidationGroup="add" OnClick="BtnUpdate_Click" /> &nbsp;
                        <asp:Button CssClass="btn btn-primary" Text="Cancel" runat="server" ID="BtnCancelEdit" OnClick="BtnCancelEdit_Click" /> &nbsp;
                    </div>
                </div>
          </div>
         <div>&nbsp;</div>
          <div class="row">
            <div class="col-xs-3">&nbsp;</div>
            <div class="col-xs-4" style="text-align:left;" >
                <div style="display:block;"><asp:ValidationSummary ID="VSErrors" DisplayMode="BulletList" BorderStyle="Solid" BorderWidth="1px" CssClass="errlabels" EnableClientScript ="true" HeaderText="The following errors occurred:" runat="server"  ValidationGroup="add"/></div>
            </div>
            
           </div>
          <div>&nbsp;</div>
          <div class="row">
                  <div class="col-xs-4 rightalign" >
                    <span  class="spanrequired" id="spnName" runat="server">*</span>   <asp:label id="LblWorker" runat="server" CssClass="labelfieldleft">Worker Name: </asp:label>
                  </div>
                   <div class="col-xs-8 leftalign" style="display:inline-block;">
                       <asp:TextBox ID="TxtWorker"  runat="server" ClientIDMode="Static" MaxLength="30" CssClass="txtnamewidth" AutoPostBack="true" OnTextChanged="TxtWorker_TextChanged" ></asp:TextBox>
                        &nbsp;<asp:ImageButton ID="ImgBtnWorker" runat="server" ImageUrl="~/Images/find.gif"  CssClass="nounload"/>
                      &nbsp;<asp:Label ID="Lblformat" runat="server" CssClass="formattext" Text="(Lastname, firstname)"></asp:Label>
                      <asp:RequiredFieldValidator ID="RFVWorker" ErrorMessage="Worker Name is required" Text="*" ControlToValidate="TxtWorker" CssClass="errlabels"  ValidationGroup="add" runat="server" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                       <asp:CustomValidator ID="CvWorker" ErrorMessage="Not a valid name / format" Text="*" ControlToValidate="TxtWorker" CssClass="errlabels" ValidationGroup="add" runat="server" SetFocusOnError="true" OnServerValidate="CvWorker_ServerValidate" Display="Dynamic"></asp:CustomValidator>
                   </div>
          </div>
          <div>&nbsp;</div>
          <div id="divWrkerDetails" runat="server" visible="false">
              <div class="row">
                 <div class="col-xs-4 rightalign">
                       <asp:Label ID="LblBadgeID" runat="server" CssClass="labelfieldleft">Badge ID:</asp:Label>
                  </div>
                 <div class="col-xs-8 leftalign">
                        <asp:Label ID="LblBadgeIdVal" runat="server"></asp:Label>
                  </div>                                  
               </div>  
              <div>&nbsp;</div>     
              <div class="row">
                  <div class="col-xs-4 rightalign">
                        <span  class="spanrequired">*</span> <asp:Label ID="LblSlacaff" runat="server" CssClass="labelfieldleft">SLAC Affiliation:</asp:Label>
                   </div>
                   <div class="col-xs-8 leftalign">              
                        <asp:DropDownList ID="DdlSlacAffiliation" runat="server" DataSourceID="SDSAffiliation" DataTextField="LOOKUP_ID" DataValueField="LOOKUP_DESC" OnDataBound="DdlSlacAffiliation_DataBound" ClientIDMode="Static" AutoPostBack="True" OnSelectedIndexChanged="DdlSlacAffiliation_SelectedIndexChanged" CssClass="nounload">
                        </asp:DropDownList>
                       <asp:CustomValidator ID="CvAffiliate" runat="server" EnableClientScript="true" Display="Dynamic" ClientValidationFunction="DropdownValidation" ControlToValidate="DdlSlacAffiliation"  ErrorMessage="Slac Affiliation is required" SetFocusOnError="true" CssClass="errlabels"  ValidationGroup="add" Text="*" ></asp:CustomValidator>
                   </div>          
               </div>          
              <div>&nbsp;</div>
              <div class="row">
                 <div class="col-xs-4 rightalign">
                    <asp:Label ID="LblEmail" runat="server" CssClass="labelfieldleft">Email:</asp:Label>
                 </div>
                 <div class="col-xs-8 leftalign">
                     <asp:Label id="LblEmailVal"  runat="server"></asp:Label>
                 </div>                  
               </div>
              <div>&nbsp;</div>
              <div class="row">
                  <div class="col-xs-4 rightalign">
                       <asp:Label ID="LblPreferred" runat="server" CssClass="labelfieldleft">Preferred Email: <br />  <label  for="lbltxtemail"  class="formattext">(if different from SLAC Email)</label></asp:Label>
                  </div>
                  <div class="col-xs-8 leftalign">
                       <asp:TextBox ID="TxtPreferred" runat="server" CssClass="txtemailwidth"></asp:TextBox> 
                      <asp:RegularExpressionValidator id="REVPreferred" runat="Server" ControlToValidate="TxtPreferred"  ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$" ErrorMessage="Not a Valid Email"  CssClass="errlabels" SetFocusOnError="true" ValidationGroup="add" Display="Dynamic" Text="*"></asp:RegularExpressionValidator>
                  </div>
              </div>
              <div class="row">
                  <div class="col-xs-4 rightalign">
                      <asp:Label ID="LblSupervisor" runat="server" CssClass="labelfieldleft">Admin Supervisor:</asp:Label>
                  </div>
                  <div class="col-xs-8 leftalign">
                      <asp:Label ID="LblSupervisorVal" runat="server"></asp:Label>
                      &nbsp;&nbsp; <asp:Button ID="BtnDesignate" CssClass= "btn btn-primary nounload" runat ="server" Text="Designate Alternate" Font-Bold="true"   />
                  </div>
              </div>
              <div id="divAltsvrEditrow" runat="server" visible="false">&nbsp;</div>
        <div class="row" id="DivAltSvrEdit" runat="server" visible="false">
            <div class="col-xs-4 rightalign"><asp:Label ID="LblAltsvredit" runat="server" CssClass="labelfieldleft">Alternate Admin Supervisor:</asp:Label> </div>
            <div class="col-xs-8 leftalign"><asp:Label ID="lblAltsvreditval" runat="server"></asp:Label>
              &nbsp;&nbsp;<b>From:</b>&nbsp;&nbsp;  <asp:Label ID="LblFromedit" runat="server"></asp:Label>&nbsp;&nbsp;<b>To:&nbsp;&nbsp;</b> <asp:Label ID="LblToedit" runat="server"></asp:Label>
            </div>
        </div>
              <div>&nbsp;</div>
              <div class="row" id="DivStatus" visible="false" runat="server">
                  <div class="col-xs-4 rightalign">
                       <asp:Label ID="LblStatus" runat="server" CssClass="labelfieldleft">Status:</asp:Label>
                  </div>
                  <div class="col-xs-8 leftalign">
                       
                       <asp:Label ID="LblStatusEditVal" runat="server" CssClass="labelfieldleft"></asp:Label>
                  </div>
              </div>
                <div>&nbsp;</div>
              <div class="row" id="DivFileInst" runat="server">
                <div class="col-xs-12" style="text-align:center;"> <span style="font-size:x-small;">**Note :- File Upload size limited to 10MB (doc, docx, jpg, bmp, pdf, xls, xlsx, png, txt, gif)</span></div>
               </div>
              <div class="row" id="Div253File" runat="server">
                    <div class="col-xs-4 rightalign">
                        <asp:Label ID="LblFileAttachment" runat="server" CssClass="labelfieldleft">253PRA Attachment:</asp:Label>
                     </div>
                    <div class="col-xs-8 leftalign">
                       <UC1:UCFileWrkr ID="UCFile1" runat="server" />
                    </div>           
                </div>
          </div>
    </asp:Panel>
    <br />
    <asp:Panel ID="PnlView" runat="server" Visible="false" GroupingText="Worker Information">
        <div class="row">
            <div class="col-xs-6">&nbsp;</div>
            <div class="col-xs-6"><asp:Button ID="BtnBack" runat="Server" CausesValidation="false" Text="Back" CssClass="btn btn-primary nounload" OnClick="BtnBack_Click" />
              
            </div>           
        </div>
        <div class="row">
            <div class="col-xs-4 rightalign"><asp:Label ID="LblWorkerView" runat="Server" CssClass="control-label labelfieldleft" Text="Worker Name:"></asp:Label></div>
            <div class="col-xs-8 leftalign"><asp:Label ID="LblWorkerVal" runat="server"></asp:Label></div>
        </div>
         <div>&nbsp;</div>
        <div class="row">
            <div class="col-xs-4 rightalign"><asp:Label ID="LblBadgeView" runat="Server" CssClass="control-label labelfieldleft" Text="Badge Id:"></asp:Label></div>
            <div class="col-xs-8 leftalign"><asp:Label ID="LblBadgeVal" runat="server"></asp:Label></div>
        </div>
         <div>&nbsp;</div>
        <div class="row">
            <div class="col-xs-4 rightalign"><asp:Label ID="LblAffiliationView" runat="Server" CssClass="control-label labelfieldleft" Text="Slac Affiliation:"></asp:Label></div>
            <div class="col-xs-8 leftalign"><asp:Label ID="LblAffiliationVal" runat="server"></asp:Label></div>
        </div>
         <div>&nbsp;</div>
        <div class="row">
            <div class="col-xs-4 rightalign"><asp:Label ID="LblSlacIdView" runat="Server" CssClass="control-label labelfieldleft" Text="Slac Id:"></asp:Label></div>
            <div class="col-xs-8 leftalign"><asp:Label ID="LblSlacIdVal" runat="server"></asp:Label></div>
        </div>
          <div>&nbsp;</div>
        <div class="row">
            <div class="col-xs-4 rightalign"><asp:Label ID="LblEmailView" runat="Server" CssClass="control-label labelfieldleft" Text="Email:"></asp:Label></div>
            <div class="col-xs-8 leftalign"><asp:Label ID="LblEmailVal1" runat="server"></asp:Label></div>
        </div>
          <div>&nbsp;</div>
        <div class="row">
            <div class="col-xs-4 rightalign"><asp:Label ID="LblPreferredView" runat="Server" CssClass="control-label labelfieldleft" Text="Preferred Email:"></asp:Label></div>
            <div class="col-xs-8 leftalign"><asp:Label ID="LblPreferredVal" runat="server"></asp:Label></div>
        </div>
         <div>&nbsp;</div>
        <div class="row">
            <div class="col-xs-4 rightalign"><asp:Label ID="LblSvrView" runat="server" CssClass="labelfieldleft">Admin Supervisor:</asp:Label> </div>
            <div class="col-xs-8 leftalign"><asp:Label ID="LblSvrViewVal" runat="server"></asp:Label></div>
        </div>
        <div id="divrowAltsvr" runat="server" visible="false">&nbsp;</div>
        <div class="row" id="DivAltSvr" runat="server" visible="false">
            <div class="col-xs-4 rightalign"><asp:Label ID="LblAltSvrView" runat="server" CssClass="labelfieldleft">Alternate Admin Supervisor:</asp:Label> </div>
            <div class="col-xs-8 leftalign"><asp:Label ID="LblAltSvrVal" runat="server"></asp:Label>
              &nbsp;&nbsp;<b>From:</b>&nbsp;&nbsp;  <asp:Label ID="LblFrom" runat="server"></asp:Label>&nbsp;&nbsp;<b>To:&nbsp;&nbsp;</b> <asp:Label ID="LblTo" runat="server"></asp:Label>
            </div>
        </div>
        <div>&nbsp;</div>
        <div class="row">
            <div class="col-xs-4 rightalign"><asp:Label ID="LblStatusView" runat="Server" CssClass="control-label labelfieldleft" Text="Status:"></asp:Label></div>
            <div class="col-xs-8 leftalign"><asp:Label ID="LblStatusVal" runat="server"></asp:Label></div>
        </div>
         <div>&nbsp;</div>
        <div class="row">
            <div class="col-xs-4 rightalign"><asp:Label ID="LblESH" runat="server" CssClass="control-label labelfieldleft" text="ESH Manual Review date:"></asp:Label></div>
             <div class="col-xs-8 leftalign"><asp:Label ID="LblESHVal" runat="server"></asp:Label></div>
        </div>
         <div>&nbsp;</div>
        <div class="row" id="DivStudent" runat="server" visible="false">
            <div class="col-xs-4 rightalign"><asp:Label ID="LblStudent" runat="server" CssClass="control-label labelfieldleft" text="Student Agreement date:"></asp:Label></div>
             <div class="col-xs-8 leftalign"><asp:Label ID="LblStudVal" runat="server"></asp:Label></div>
        </div>
    </asp:Panel>
    <br />
    <UC2:UCFileList ID="UCFileList1" runat="server" />
    <br />
    <UC4:UCUserLab ID="UCUserLab" runat="server" />
     <br />
    <UC5:UCPending ID="UCPending1" runat="server" />
    <br />
    <UC3:UCTraining ID="UCTraining1" runat="server" />

     <div id="dialog-updmsg" title="" style="display:none;" >
            <p>
                <asp:Label ID="LblWorkerMsg" runat="server"></asp:Label>
            </p>
        </div>

      <div id="dialogowneradmin" style="display:none;"  >             
                <iframe id="modaldialogowneradmin"  frameborder="1" width="100%" height="100%" >
                </iframe>
          </div>

      <div id="dialogDesignate" style="display:none;">
                        <iframe id="modaldialogDesignate" width="100%" height="100%">
                           
                        </iframe>
       </div>
     <asp:SqlDataSource ID="SDSAffiliation" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>" SelectCommand="SELECT LOOKUP_ID, LOOKUP_DESC FROM LST_LOOKUP WHERE IS_ACTIVE='Y' AND LOOKUP_GROUP='Affiliation'"></asp:SqlDataSource>
      <asp:HiddenField ID="HdnMode" runat="server" ClientIDMode="Static" />
</asp:Content>