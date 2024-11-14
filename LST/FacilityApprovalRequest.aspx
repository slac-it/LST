
<%@ Page Title="" Language="C#" MasterPageFile="~/LST.Master" AutoEventWireup="true" CodeBehind="FacilityApprovalRequest.aspx.cs" StylesheetTheme="LSTTheme" Inherits="LST.FacilityApprovalRequest" %>
<%@ Register Src="~/UserControls/FACApprovalHistory.ascx" TagName="ApprHis"  TagPrefix="UC"%>


<asp:Content ID="ContentApprovalRequest" ContentPlaceHolderID="MainContent" runat="server"> 
    <script type="text/javascript" src="Scripts/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/LST.js"></script>
    
       <script type="text/javascript">

           $(document).ready(function () {

               window.onbeforeunload = function (event) {
                   var msgsub = 'Important: You might have unsaved data on this page. Please click on \'Submit Request\' button to save your data.';
                   var msgapp = 'Important: You might have unsaved data on this page. Please click on \'Approve\' or \'Decline\' button.';
                   var Mode = $("#HdnMode").val();
                   if (Mode == 'req') { message = msgsub; }
                   else if (Mode == 'approve') { message = msgapp; }
                   else { message = '';}
                   if (typeof event == 'undefined') {
                       event = window.event;
                   }
                   if (event) {
                       if (message != '')
                           event.returnValue = message;
                      
                   }
                   if (message != '')
                       return message;
                   else
                        null;
               };

               $(function () {
                   $(".nounload").click(function () {
                       window.onbeforeunload = null;
                   });


               });

               function askConfirm() {
                   var mode = "req"; //$("#HdnMode").val();
                   alert(mode);
                   if (mode == "") { dispmsg = false; }
                   else { dispmsg = true; }
                   if (dispmsg) {
                       if (mode == "req") {
                           return "Important: You might have unsaved data on this page. Please click on \'Submit Request\' button to save your data.";
                       }
                       else if (mode == "approve") {
                           return "Important: You might have unsaved data on this page. Please click on \'Approve or Decline or Cancel\' button."
                       }

                   }
               }
           });

       </script>

     <div class="row" id="DivBack" runat="server" visible="false" >
          <div class="col-sm-8">&nbsp;</div>
          <div class="col-sm-4">
               <asp:Button ID="BtnBack" runat="server" Text="Back" OnClick="BtnBack_Click" CssClass="btn btn-primary nounload" />
          </div>
                    
       </div>
         <div class="row rightalign" id="Divbuttons" runat="server">
            
                 <div class="col-sm-8 leftalign" id="DivSLSO" runat="server"> 
                     <asp:Button ID="BtnSubmit" runat="server"  CssClass="btn btn-primary nounload" Text="Submit Request"  OnClick="BtnSubmit_Click" ValidationGroup="submit"  ClientIDMode="Static"/>
                      <asp:Button ID="BtnClear" runat="server" CssClass="btn btn-primary nounload" Text="Cancel" OnClick="BtnClear_Click"  CausesValidation="false" ClientIDMode="Static" OnClientClick="return confirm('Are you certain you want to cancel and go back?');"/>
                      <span class="spanrequired">*</span> indicates required field
                 </div>
                 <div class="col-sm-8 leftalign" id="DivApprovers" runat="server" visible="false"> 
                     <asp:Button ID="BtnApprove" runat="server"  CssClass="btn btn-primary nounload" Text="Approve" OnClick="BtnApprove_Click" ValidationGroup="approve"  ClientIDMode="Static"/>
                     <asp:Button ID="BtnDecline" runat="server" CssClass="btn btn-primary nounload" Text="Decline" OnClick="BtnDecline_Click"   ValidationGroup="decline" CausesValidation="true" ClientIDMode="Static"/>
                     <asp:Button ID="BtnCancel" runat="server" CssClass="btn btn-primary nounload" Text="Cancel" OnClick="BtnCancel_Click"  CausesValidation="false" ClientIDMode="Static" OnClientClick="return confirm('Are you certain you want to cancel and go back?');"/>
                      <span class="spanrequired">*</span> indicates required field
                 </div>
                
             
             </div>
     
           <div class="row">
               <div class="col-sm-3">&nbsp;</div>
               <div class="col-sm-5 leftalign">
                   <div style="display:block;">
                       <asp:ValidationSummary ID="VSRequest" runat="server"  BorderStyle="Solid" BorderWidth="1px" DisplayMode="BulletList" ValidationGroup="submit"   CssClass="errlabels" ShowSummary="true"/>
                       <asp:ValidationSummary ID="ValidationSummary1" runat="server"  BorderStyle="Solid" BorderWidth="1px" DisplayMode="BulletList" ValidationGroup="approve"   CssClass="errlabels" ShowSummary="true"/>
                        <asp:ValidationSummary ID="VSDecline" runat="server" BorderStyle="Solid" BorderWidth="1px" displayMode="BulletList" ValidationGroup="decline" CssClass="errlabels" ShowSummary="true" />
                       
                   </div>
               </div>
           </div>
        <asp:Panel id="PnlSLSO" GroupingText="Request Approval" runat="server">
             <div class="row">
                 <div class="col-sm-12 subheading">
                     <asp:Literal ID="LtrlInfo"  runat="server" >
                         To request the next operation approval for your SLAC Laser Facility, you must affirm the required items below have been completed (if needed) and then select "Submit Request". To view the status of your request, please visit the Home page.
                    </asp:Literal>
                 </div>   
            </div>
            <div>&nbsp;</div>
            <div id="divCertification" class="row" runat="server">
                 <div class="col-sm-8 labelfieldleft">
                      <asp:CustomValidator ID="CVCertification" runat="server" ControlToValidate="RblCertification" EnableClientScript="true" CssClass="errlabels" Display="Dynamic" ValidationGroup="submit" SetFocusOnError="true" ErrorMessage="Must complete the annual laser safety certification" ValidateEmptyText="true" OnServerValidate="CVCertification_ServerValidate" Text="*"></asp:CustomValidator>
                      i) Annual laser safety certification tests completed
                </div>
                <div class="col-sm-4 leftalign">
                     <asp:RadioButtonList ID="RblCertification" runat="server" RepeatDirection="Horizontal" CellPadding="2" CellSpacing="5" Visible="true" >
                         <asp:ListItem  Value="0">&nbsp;Yes &nbsp;</asp:ListItem>
                         <asp:ListItem  Value="1" Selected="True">&nbsp; No &nbsp;</asp:ListItem>
                     </asp:RadioButtonList> 
                </div>
             </div>
             <div>&nbsp;</div>
             <div id="divSOPRevision" class="row" runat="server">
                 <div class="col-sm-8 labelfieldleft">
                      <asp:CustomValidator ID="CVRevision" runat="server" ControlToValidate="RblRevision" EnableClientScript="true" CssClass="errlabels" Display="Dynamic" ValidationGroup="submit" SetFocusOnError="true" ErrorMessage="Must complete the SOP revision or affirm a revision is not needed" ValidateEmptyText="true" OnServerValidate="CVRevision_ServerValidate" Text="*"></asp:CustomValidator>
                      ii) SOP revision, if needed, completed
                </div>
                <div class="col-sm-4 leftalign">
                     <asp:RadioButtonList ID="RblRevision" runat="server" RepeatDirection="Horizontal" CellPadding="2" CellSpacing="5" Visible="true" >
                         <asp:ListItem  Value="0">&nbsp;Yes &nbsp;</asp:ListItem>
                         <asp:ListItem  Value="1" Selected="True">&nbsp; No &nbsp;</asp:ListItem>
                     </asp:RadioButtonList> 
                </div>
             </div>
             <div>&nbsp;</div>
             <div class="row" >
                 <div class="col-sm-12">
                      <asp:Label ID="LblSLSOComments" runat="server" Text="Enter comments: " CssClass="labelfieldleft"></asp:Label>
                      <span id="spnslsofrmt" class="formattext">(max. 480 characters)</span>
                 </div>
             </div>
             <div>&nbsp;</div>
            <div class="row" >
                 <div class="col-sm-12">
                      <asp:Textbox  id="TxtSLSOComments" TextMode="MultiLine" runat="server"  CssClass="txtMulti" onkeypress="return textboxMultilineMaxNumber(this,480)"></asp:Textbox>
                      <asp:RegularExpressionValidator ID="RegexSLSOCom" ControlToValidate="TxtSLSOComments" ErrorMessage="Comments exceeded 480 characters" runat="server" ValidationExpression="(?:[\r\n]*.[\r\n]*){0,480}"  CssClass="errlabels" 
                             ValidationGroup="submit" Text="*"></asp:RegularExpressionValidator>
                      <asp:RegularExpressionValidator ID="RegexSCSLSO" runat="server" ErrorMessage="< and > is not allowed in Comments."  ValidationGroup="submit" Display="Dynamic"  CssClass="errlabels" 
                        ControlToValidate="TxtSLSOComments" ValidationExpression="^[^<>]*$"  Text="*"></asp:RegularExpressionValidator>
                 </div>
             </div>
        </asp:Panel>
     
    
        <asp:Panel id="PnlLSO" runat="server"  GroupingText="Approval" Visible="false">
            
              <div class="row">&nbsp;</div>
               <div class="row">
                 <div class="col-sm-8 labelfieldleft">
                         <asp:CustomValidator ID="CVRevApp" runat="server" ControlToValidate="RbRevisionApproved" CssClass="errlabels" Display="Dynamic" ErrorMessage="Must approve SOP revision or affirm a revision is not needed" OnServerValidate="CVRevApp_ServerValidate" Text="*" SetFocusOnError="true" ValidateEmptyText="true" ValidationGroup="approve" ></asp:CustomValidator>                     
                        i) SOP revision, if needed, approved                    
                      </div>
                   <div class="col-sm-4 leftalign">                    
                          <asp:RadioButtonList ID="RbRevisionApproved" runat="server" RepeatDirection="Horizontal" CellPadding="2" CellSpacing="5" visible="true" >
                            <asp:ListItem  Value="0"> &nbsp;Yes &nbsp;</asp:ListItem>
                            <asp:ListItem  Value="1" Selected="True">&nbsp; No &nbsp; </asp:ListItem> 
                        </asp:RadioButtonList>                                            
                   </div>          
              
             </div>
              <div class="row">&nbsp;</div>
              <div class="row">
                  <div class="col-sm-8 labelfieldleft">
                     <asp:CustomValidator ID="CVInspection" runat="server" ControlToValidate="RblInspection" EnableClientScript="true" CssClass="errlabels" Display="Dynamic" ValidationGroup="approve" SetFocusOnError="true" ErrorMessage="Must complete laser lab walkthrough and inspection checklist" ValidateEmptyText="true" OnServerValidate="CVInspection_ServerValidate" Text="*"></asp:CustomValidator>
                    ii) Laser lab walkthrough and inspection checklist completed by LSO
                </div>
                  <div class="col-sm-4 leftalign">
                     <asp:RadioButtonList ID="RblInspection" runat="server" RepeatDirection="Horizontal" CellPadding="2" CellSpacing="5" Visible="true" >
                         <asp:ListItem  Value="0">&nbsp;Yes &nbsp;</asp:ListItem>
                         <asp:ListItem  Value="1" Selected="True">&nbsp; No &nbsp;</asp:ListItem>
                     </asp:RadioButtonList>                     
                     
                 </div>
              </div>        
              <div class="row">&nbsp;</div>
              <div class="row">
                   <div class="col-sm-8 labelfieldleft"> 
                     <asp:CustomValidator ID="CuVNewDate" runat="Server" ControlToValidate="TxtNewExpDate" ErrorMessage ="New Facility Operation Date is required" ValidationGroup="approve"  CssClass="errlabels" Text="*" Display="Dynamic" SetFocusOnError="true" ValidateEmptyText="true" OnServerValidate="CuVNewDate_ServerValidate"></asp:CustomValidator>                   
                     iii) New Laser Facility Operation approval Expiration Date:
                     
                 </div>
                 <div class="col-sm-4 leftalign">
                     <asp:TextBox ID="TxtNewExpDate" runat="server" ClientIDMode="Static"></asp:TextBox>
                     <span id="spnDatefrmt" class="formattext">(mm/dd/yyyy)</span>                  
                      <asp:CompareValidator ID="CvNewdate" runat="server" Type="Date" Operator="DataTypeCheck"
                                                                ControlToValidate="TxtNewExpDate" ErrorMessage="Not a valid date!"  Text="*" CssClass="errlabels" ValidationGroup="approve" Display="Dynamic"></asp:CompareValidator>
                 </div>
              </div>  
             <div class="row">&nbsp;</div>          
         </asp:Panel>

         <asp:Panel ID="PnlApprovers"  Visible="false" runat="server">
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
                             CssClass="errlabels"  ControlToValidate="TxtComments"  ValidateEmptyText="true" OnServerValidate="CVcomments_ServerValidate" ValidationGroup="decline" Text="*" ></asp:CustomValidator> &nbsp&nbsp;
                   <asp:RegularExpressionValidator ID="RegexComments" ControlToValidate="TxtComments" ErrorMessage="Comments exceeded 480 characters" runat="server" ValidationExpression="(?:[\r\n]*.[\r\n]*){0,480}"  CssClass="errlabels" 
                             ValidationGroup="approve" Text="*"></asp:RegularExpressionValidator>
                 <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="TxtComments" ErrorMessage="Comments exceeded 480 characters" runat="server" ValidationExpression="(?:[\r\n]*.[\r\n]*){0,480}"  CssClass="errlabels" 
                             ValidationGroup="decline" Text="*"></asp:RegularExpressionValidator>
                  <asp:RegularExpressionValidator ID="RegexSCComApp" runat="server" ErrorMessage="< and > is not allowed in Comments."  ValidationGroup="approve" Display="Dynamic"  CssClass="errlabels" 
                        ControlToValidate="TxtComments" ValidationExpression="^[^<>]*$"  Text="*"></asp:RegularExpressionValidator>
                 <asp:RegularExpressionValidator ID="RegexSCComDec" runat="server" ErrorMessage="< and > is not allowed in Comments."  ValidationGroup="decline" Display="Dynamic"  CssClass="errlabels" 
                        ControlToValidate="TxtComments" ValidationExpression="^[^<>]*$"  Text="*"></asp:RegularExpressionValidator>
            </div>
        </div>
        <div class="row">&nbsp;</div>   
        </asp:Panel>

         <asp:Panel ID="PnlRequestDet" GroupingText ="Facility Request Information" visible="false" runat="server">
         <!-- Request information and facility details -->
         <div class="row">
             <div class="col-sm-6">
                 <asp:Label ID="LblRequestId" Text="Request Id:" runat="server" CssClass="labelfieldleft"></asp:Label>
                 <asp:Label ID="LblRequestIdVal" runat="server"></asp:Label>
             </div>
             <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
             <div class="col-sm-6">
                 <asp:Label ID="LblStatus" Text="Request Status:" runat="server" CssClass="labelfieldleft"></asp:Label>
                 <asp:Label ID="LblStatusVal" runat="server" BackColor="Yellow"></asp:Label>
             </div>
         </div>
         <div class="row">&nbsp;</div>
         <div class="row">
             <div class="col-sm-6">
                <asp:Label ID="LblFacility" Text="Facility: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblFacilityVal"  runat="server" >   </asp:Label>
                
            </div>
            <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
            <div class="col-sm-6">
                <asp:Label ID="LblExpiryDate" Text="Approval Expiry Date: " runat="server" CssClass="labelfieldleft"></asp:Label>
                <asp:Label ID="LblExpiryDateVal"  runat="server"></asp:Label>
            </div>
         </div>
         <div class="row">&nbsp;</div>
         <div class="row">
             <div class="col-sm-12">
                 <a  id="LnkFac" href="" runat="server">Click here to view </a>
             </div>
         </div>
              <div class="row">&nbsp;</div>
     </asp:Panel>
   
         <asp:Panel ID="PnlApprovalHistory"  Visible="false" runat="server">
           <UC:ApprHis  id="UCApprhis" runat="server"></UC:ApprHis>
    </asp:Panel>

        <div id="dialog-appreqmsg" style="display:none;" class="nounload">
            <p>
                <asp:Label ID="LblFACMsg" runat="server"></asp:Label>
            </p>
        </div>
        <asp:HiddenField ID="HdnMode" runat="server" ClientIDMode="Static" />
    
</asp:Content>
