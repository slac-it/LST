<%@ Page Title="" Language="C#" MasterPageFile="~/LST.Master" AutoEventWireup="true" CodeBehind="Approval.aspx.cs" StylesheetTheme="LSTTheme" Inherits="LST.Approval1" %>
<%@ Register Src="~/UserControls/ApprovalHistory.ascx" TagName="ApprHis" TagPrefix="UC" %>
<%@ Register Src="~/UserControls/FileAttach.ascx"  TagName="FileAttach" TagPrefix="UC1"%>
<asp:Content ID="HeaderContent1" runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <script type="text/javascript" src="Scripts/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script>    
    <script type="text/javascript" src="Scripts/LST.js"></script>
   
    <script type="text/javascript">
        window.onbeforeunload = function (event) {
            var msgApp = 'Important: You have unsaved data on this page. Please click on \'Approve\' or \'Decline\' button to save your action.';
            var mode = $("#HdnMode").val();

            if (mode == 'approve')
            {
                message = msgApp;
            }
            else
            {
                message = '';
            }
            if (typeof event == 'undefined') {
                event = window.event;
            }
            if (event) {
                if (message != '')
                event.returnValue = message;
            }
            if (message != '')
                return message;
            else null;
        };

        $(function () {
            $(".nounload").click(function () {
                window.onbeforeunload = null;
            });

        });
    </script>

       <div class="row" id="DivBack" runat="server" visible="false" >
          <div class="col-sm-8">&nbsp;</div>
          <div class="col-sm-4">
               <asp:Button ID="BtnBack" runat="server" Text="Back" OnClick="BtnBack_Click" CssClass="btn btn-primary nounload" />
          </div>
                    
       </div>
   <div class="row rightalign" id="Divbuttons" runat="server">
       <div class="col-sm-8 leftalign">
            <asp:Button ID="BtnApprove" runat="server" CssClass="btn btn-primary nounload" Text="Approve" OnClick="BtnApprove_Click"  ValidationGroup="app"/>&nbsp;&nbsp;
            <asp:Button ID="BtnDecline" runat="server" CssClass="btn btn-primary nounload" Text="Decline" ValidationGroup="app" OnClick="BtnDecline_Click" />&nbsp;&nbsp;
            <asp:Button ID="BtnCancel" runat="server" CssClass="btn btn-primary nounload" Text="Cancel" OnClick="BtnCancel_Click" ClientIDMode="Static" OnClientClick="return confirm('Are you certain you want to cancel and go back?');"/>&nbsp;&nbsp;
           <span class="spanrequired">*</span> indicates required field
    </div>
   </div>
    
    <div class="row">
                <div class="col-sm-3">&nbsp;</div>
                <div class="col-sm-5 leftalign">
                     <div style="display:block;"> <asp:ValidationSummary ID="VSErrors" DisplayMode="BulletList" BorderStyle="solid" BorderWidth="1px"  CssClass="errlabels" EnableClientScript ="true" HeaderText="The following errors occurred:" runat="server"  ValidationGroup="app"/></div>
                </div>
            
        </div>
      
    <asp:Panel GroupingText="Approval" runat="server" ID="PnlApproval">
          <div class="row">
                 <div class="col-sm-12 subheading">
                    <asp:Literal ID="LtrlInfo"  runat="server" >
                        
                    </asp:Literal>
                     <span id="SpnSLSO" runat="server" visible="false">SLSOs approving requests must provide OJT completion date 
 information within 30 days; if this is not done, the Worker will become inactive and will be notified.</span>
                 </div>   
          </div>
         <div>&nbsp;</div>
        
         <div class="row">
            <div class="col-sm-12">
                <span id="SpnComments" class="spanrequired" runat="server" visible="false">*</span>
                <asp:Label ID="LblComments" runat="server" Text="Enter comments: " CssClass="labelfieldleft"></asp:Label>
                <span id="spnfrmt" class="formattext">(*required if declining request; max. 480 characters)</span>
                
             </div>
         </div>
        <div class="row">
            <div class="col-sm-12">
                  <asp:Textbox  id="TxtComments" TextMode="MultiLine" runat="server"  CssClass="txtMulti" onkeypress="return textboxMultilineMaxNumber(this,480)"></asp:Textbox>
                  <asp:CustomValidator ID="CVcomments" runat="server" ErrorMessage="Comments required if declined"
                             CssClass="errlabels"  ControlToValidate="TxtComments"  ValidateEmptyText="true" OnServerValidate="CVcomments_ServerValidate" ValidationGroup="app" Text="*" ></asp:CustomValidator> &nbsp&nbsp;
                   <asp:RegularExpressionValidator ID="RegexComments" ControlToValidate="TxtComments" ErrorMessage="Comments exceeded 480 characters" runat="server" ValidationExpression="(?:[\r\n]*.[\r\n]*){0,480}"  CssClass="errlabels" 
                             ValidationGroup="app" Text="*"></asp:RegularExpressionValidator>
                    <asp:RegularExpressionValidator ID="RegexscComments" runat="server" ErrorMessage="< and > is not allowed in Comments."  ValidationGroup="app" Display="Dynamic"  CssClass="errlabels" 
                        ControlToValidate="TxtComments" ValidationExpression="^[^<>]*$"  Text="*"></asp:RegularExpressionValidator>
            </div>
        </div>
      
        <div class="row">&nbsp;</div>
        <div class="row" runat="server" visible="false" id="Div253PRAFile">
             <div class="col-sm-3">
               <span class="spanrequired">*</span> <asp:Label ID="LblFileAttachment" runat="server" CssClass="labelfieldleft">253PRA Attachment:</asp:Label>
             </div>
            <div class="col-sm-8 leftalign">
                  <UC1:FileAttach  ID="UCFile1" runat="server" />
             </div>
        </div>
      
        <div class="row">&nbsp;</div>
        <div class="row" id="DivOJTinst" runat="server" visible="False" >
            <div class="col-sm-12 subheading">
                    If OJT completed, please provide the OJT completion date. 
            </div>
        </div>
        <div class="row" runat="server" visible="false" id="DivOJT">          
            <div class="col-sm-3">
                <asp:Label ID="LblOJTdate" runat="server" CssClass="labelfieldleft" Text="Enter OJT Completion Date:"></asp:Label>              
              
            </div>
            <div class="col-sm-8 leftalign">
                 <asp:TextBox ID="TxtOJTDate" runat="server" ClientIDMode="Static"></asp:TextBox>
                  <span id="spnOJTfrmt" class="formattext">(mm/dd/yyyy)</span>
               
                <asp:CompareValidator ID="CvOJT" runat="server" Type="Date" Operator="DataTypeCheck"
                                                                ControlToValidate="TxtOJTDate" ErrorMessage="Not a valid date!"  Text="*" CssClass="errlabels" ValidationGroup="app"></asp:CompareValidator>
            </div>
        </div>
        <div id="divlineOJT" class="row" runat="server" visible="false">&nbsp;</div>
        <div class="row" runat="server" visible="false" id="DivFiles">
            <div class="col-sm-12">
               <asp:Panel ID="PnlFile" runat="server" Width="100%"  GroupingText="Files Attached">
            <asp:GridView ID="GvFile" runat="server" AutoGenerateColumns="false" onrowcommand="gvFile_RowCommand" Width="80%"  EmptyDataText="No files attached" >
                <Columns>
                    <asp:BoundField DataField="ATTACHMENT_ID"  Visible="false"  HeaderStyle-HorizontalAlign="Left"/>
                    <asp:TemplateField HeaderText="File" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LnkDownload"  CssClass="nounload" runat="server"  CommandArgument='<%# Eval("ATTACHMENT_ID") %>' CommandName="download" Text ='<%# Eval("FILE_NAME") %>'></asp:LinkButton>
                                </ItemTemplate>
                    </asp:TemplateField>                         
                    <asp:BoundField DataField="UPLOADED_BY" HeaderText="Uploaded by"  HeaderStyle-HorizontalAlign="Left"/>
                    <asp:BoundField DataField="UPLOADED_ON" HeaderText="Uploaded On" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" HeaderStyle-HorizontalAlign="Left"/>
                      
                     <asp:BoundField DataField="DOCTYPE" HeaderText="Document Type" />
                </Columns>
                <HeaderStyle BackColor="LightGray" />
            </asp:GridView>
        </asp:Panel>
            </div>
        </div>
       
     </asp:Panel>
     <div class="row">&nbsp;</div> 
     <asp:Panel ID="PnlRequest" runat="server" GroupingText="Request Information">

    
        <div class="row">
            <div class="col-sm-6">
                <asp:Label ID="LblRequestId" Text="Request Id: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblRequestIdVal" runat="server"></asp:Label>
            </div>
             <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
            <div class="col-sm-6">
                <asp:Label ID="LblWorkType" Text="Type of Work: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblWorkTypeVal" runat="server"></asp:Label>
            </div>
        </div>
     <div class="row" id="Divrowline">&nbsp;</div>
        <div class="row" id="DivFac" >
            <div class="col-sm-6">
                <asp:Label ID="LblFacility" Text="Facility: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblFacilityVal"  runat="server"></asp:Label>
            </div>
             <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
            <div class="col-sm-6">
                <asp:Label ID="LblSOP" Text="SOP Review Date: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblSOPVal"  runat="server"></asp:Label>
            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-sm-6">
                <asp:Label ID="LblOperator" Text="Worker: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblOperatorVal" runat="server"></asp:Label>
            </div>
             <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
            <div class="col-sm-6">
                <asp:Label ID="LblBadgeId" Text="Badge Id: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblBadgeIdVal" runat="server"></asp:Label>
            </div>
        </div>
         <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-sm-6">
                <asp:Label ID="LblEmail" Text="Email: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblEmailVal"  runat="server"></asp:Label>
            </div>
             <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
            <div class="col-sm-6">
                <asp:Label ID="LblPrefEmail" Text="Preferred Email: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblPrefEmailVal"  runat="server"></asp:Label>
            </div>
        </div>
        <div class="row">&nbsp;</div>      
        <div class="row" >
             <div class="col-sm-6">
                <asp:Label ID="LblSlacId" Text="Slac Id: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblSlacIdVal" runat="server"></asp:Label>
            </div>
            <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
            <div class="col-sm-6">
                <asp:Label ID="LblAffiliation" Text="SLAC Affiliation: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LbAffiliateVal"  runat="server"></asp:Label>
            </div>
         </div>
         <div class="row">&nbsp;</div>
          <div class="row" >
            
            <div class="col-sm-6">
                <asp:Label ID="LblSvr" Text="Admin Supervisor:" runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblSvrVal" runat="server"></asp:Label>
            </div>
            <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
            <div class="col-sm-6">
                <asp:Label ID="LblAltSvr" Text="Alternate Supervisor: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblAltSvrVal"  runat="server"></asp:Label>
            </div>
         </div>
         <div class="row" id="Divojtjustifyline" runat="server">&nbsp;</div>
           <div class="row">
               <div class="col-sm-6" runat="server" id="DivOJTDate" visible="false">
                <asp:Label ID="LblOJTDateView" Text="OJT Completion Date:" runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblOJTDateval" runat="server"></asp:Label>
                </div>
            
            
            <div class="col-sm-6" id="DivJustify" runat="server" visible="false">
                <asp:Label ID="LblJustify" Text="Justification for Conditional Approval: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblJustifyVal"  runat="server"></asp:Label>
            </div>
        </div>
        <div class="row">&nbsp;</div>
         <div class="row">
              <div class="col-sm-6">
                <asp:Label ID="LblStatus" Text="Status:" runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblStatusVal" runat="server"></asp:Label>
            </div>
             <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
              <div class="col-sm-6" id="DivWrkStat" runat="server" visible="false">
                <asp:Label ID="LblWorkerStat" Text="Worker's current QLO/LCA Worker Status:" runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblWorkerStatVal" runat="server"></asp:Label> <a href="" id="aWrkerlink" Text="; see Worker's Report for information" runat="server"></a>
            </div>
         </div>     
 </asp:Panel>
 <div class="row">&nbsp;</div> 
 <div class="row">
            <div class="col-sm-12">
                <UC:ApprHis ID="UCApphis" runat="server" />
            </div>
 </div>
<div id="dialog-appreqmsg" style="display:none;" class="nounload">
    <p>
        <asp:Label ID="LblMsg" runat="server"></asp:Label>
    </p>
</div>
    <asp:HiddenField ID="HdnMode" runat="server" ClientIDMode="Static" />
</asp:Content>
