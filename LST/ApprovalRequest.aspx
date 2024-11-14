<%@ Page Title="" Language="C#" MasterPageFile="~/LST.Master" AutoEventWireup="true" CodeBehind="ApprovalRequest.aspx.cs" StylesheetTheme="LSTTheme" Inherits="LST.ApprovalRequest" %>
<%@ Register Src="~/UserControls/TrainingSummary.ascx" TagName="UCTraining"  TagPrefix="UC1"%>
<%@ Register Src="~/UserControls/UserLabSummary.ascx" TagName="UCUserLab" TagPrefix="UC2" %>

<asp:Content ID="ContentApprovalRequest" ContentPlaceHolderID="MainContent" runat="server"> 
    <script type="text/javascript" src="Scripts/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/LST.js"></script>
     <script type="text/javascript" src="Scripts/backbutton.js"></script>


       <script type="text/javascript">

           function ResetTextbox() {
               var empid = $('#LblEmpId').val();
               $('#HdnEmpid').val(empid);
               $("#TxtWorker").val("");               
               $('#TxtWorker').trigger('change');
             
           }

           function DropdownValidationWtype(source, arguments) {
               var ctrl = document.forms[0].elements["DdlWorkType"].value;
               if (ctrl == "-1") {
                   arguments.IsValid = false;
                   return false;
               }
               else {
                   return true;
               }
           }


           function DropdownValidationLab(source, arguments) {
               var ctrl = document.forms[0].elements["DdlLab"].value;
               if (ctrl == "-1") {
                   arguments.IsValid = false;
                  
               }
               else {
                   arguments.IsValid = true;
               }
           }

           function DdValidateAffiliation(source, arguments) {

               var ctrl = document.forms[0].elements["DdlSlacAffiliation"].value;

               if (ctrl == "-1") {
                   arguments.IsValid = false;
                   return false;
               }
               else {
                   return true;
               }
           }

           //function CheckAltSvrFromTo(source, arguments) {
           //    var isVisible = $('#DivAltsvrAdd').is(':visible');
           //    var TxtAlt = $('#TxtAlternate').val();
           //    if (isVisible && TxtAlt == 0) {
           //        arguments.IsValid = false;
           //    } else arguments.IsValid = true;

           //}

           window.onbeforeunload = function (event) {
               var message = 'Important: You have unsaved data on this page. Please click on \'Submit Request\' button to save your data.';
               if (typeof event == 'undefined') {
                   event = window.event;
               }
               if (event) {
                   event.returnValue = message;
               }
               return message;
           };

           $(function () {
               $(".nounload").click(function () {
                   window.onbeforeunload = null;
               });

       
           });

       </script>
         <div class="row rightalign" id="Divbuttons" runat="server">
            
                 <div class="col-sm-8 leftalign">
                     <asp:Button ID="BtnSubmit" runat="server"  CssClass="btn btn-primary nounload" Text="Submit Request" OnClick="BtnSubmit_Click" ValidationGroup="add"  ClientIDMode="Static"/>
                      <asp:Button ID="BtnClear" runat="server" CssClass="btn btn-primary nounload" Text="Start Over" OnClick="BtnClear_Click"  CausesValidation="false" ClientIDMode="Static"/>
                      <span class="spanrequired">*</span> indicates required field
                 </div>
             
             </div>
           <div class="row">
               <div class="col-sm-3">&nbsp;</div>
               <div class="col-sm-5 leftalign">
                   <div style="display:block;"><asp:ValidationSummary ID="VSRequest" runat="server"  BorderStyle="Solid" BorderWidth="1px" DisplayMode="BulletList" ValidationGroup="add"   CssClass="errlabels" ShowSummary="true"/></div>
               </div>
           </div>
        <asp:Panel GroupingText="Request Approval" runat="server">
             <div class="row">
                 <div class="col-sm-12 subheading">
                     <asp:Literal ID="LtrlInfo"  runat="server" >
                         To request approval as a QLO or LCA Worker for SLAC Laser Facility, provide your Laser Worker information below and then select "Submit Request". To review your current
                        QLO/LCA Worker status, go to the Home page.
                    </asp:Literal>
                 </div>   
            </div>
            <div>&nbsp;</div>
             <div class="row">
                <div class="col-sm-6">
                     <span class="spanrequired">*</span><asp:Label ID="LblWorkType" runat="server" Text="Select Type of Work: " CssClass="labelfieldleft"></asp:Label>
                    <asp:DropDownList ID="DdlWorkType" runat="server" OnDataBound="DdlWorkType_DataBound"  CssClass="txtWidth nounload" ClientIDMode="Static" AutoPostBack="True" OnSelectedIndexChanged="DdlWorkType_SelectedIndexChanged">
                    </asp:DropDownList>
                     <asp:CustomValidator ID="CvWorkType" runat="server" EnableClientScript="true" Display="Dynamic" ClientValidationFunction="DropdownValidationWtype" ControlToValidate="DdlWorkType"  ErrorMessage="WorkType is required. Please select one" SetFocusOnError="true" CssClass="errlabels"  ValidationGroup="add"  Text="*" OnServerValidate="CvWorkType_ServerValidate" ></asp:CustomValidator>
                 </div>
                 <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
                 <div class="col-sm-6">
                     <span class="spanrequired">*</span>
                    <asp:Label ID="LblLabName" runat="server" Text="Select Facility Name: " CssClass="labelfieldleft"></asp:Label>
                     <asp:DropDownList ID="DdlLab" runat="server"  AutoPostBack="True" OnSelectedIndexChanged="DdlLab_SelectedIndexChanged" OnDataBound="DdlLab_DataBound"  ClientIDMode="Static" CssClass="nounload"  >
                     </asp:DropDownList>
                      <asp:CustomValidator ID="CvLab" runat="server" EnableClientScript="true" Display="Static" ClientValidationFunction="DropdownValidationLab" ControlToValidate="DdlLab"  ErrorMessage="Facility is required. Please select one" SetFocusOnError="true" CssClass="errlabels"  ValidationGroup="add"  Text="*" OnServerValidate="CvLab_ServerValidate" ></asp:CustomValidator>
                  </div>
             </div>
             <div class="row">&nbsp;</div>
             <div id="divlocation" class="row" runat="server" visible="false">
                <div class="col-sm-6">                 
                    <asp:Label ID="LblBldg" Text="Bldg: " runat="Server" CssClass="labelfieldleft"></asp:Label>
                    <asp:Label ID="LblBldgVal"  runat="server"></asp:Label> &nbsp;&nbsp; &nbsp;&nbsp;
                    <asp:Label ID="LblRoom" Text="Room: " runat="server" CssClass="labelfieldleft"></asp:Label>
                    <asp:Label ID="LblRoomVal"  runat="server"></asp:Label>                   
                </div>
             <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
             <div class="col-sm-6 leftalign">
                    <asp:Label ID="LblLocation" Text="Other Location: " runat="server" CssClass="labelfieldleft"></asp:Label>
                    <asp:Label ID="LblLocationVal" runat="server"></asp:Label> 
              </div>
            </div>
            <div class="row">&nbsp;</div>
            <div id="DivSLSO" class="row" runat="server" visible="false" >
                <div class="col-sm-6">
                    <asp:Label ID="LblSLSO" Text="SLSO Name: " runat="server" CssClass="labelfieldleft"></asp:Label>
                    <asp:Label ID="LblSLSOVal"  runat="server"></asp:Label>
                </div>
                <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
                <div class="col-sm-6 leftalign">
                    <asp:Label ID="LblAltSLSO" Text="Alternate SLSO: " runat="server" CssClass="labelfieldleft"></asp:Label>
                    <asp:Label ID="LblAltSLSOVal" runat="server"></asp:Label> 
              </div>
            </div>
             <div class="row" id="divline" visible="false" runat="server">&nbsp;</div>
            <div id="DivCondition" runat="server" visible="false">
                <div class="row">
                    <div class="col-sm-12 subheading">
                        <span class="spanrequired">131 not completed.</span> &nbsp; Please complete prior to submitting request. Request for conditional approval for 30 days is allowed,
                        if provide justification. If approved, must complete 131 within 30 days.
                    </div>
                </div>
                <div class="row">
                     <div class="col-sm-12">
                         <asp:Label ID="LblJustification" Text="Enter Justification for Conditional Approval" runat="server" CssClass="labelfieldleft">    </asp:Label> 
                         <span id="spnfrmt" class="formattext">(required; max. 480 characters)</span>
                     </div>
                </div>
                <div class="row">
                    <div class="col-sm-12">
                        <asp:TextBox ID="TxtJustification" TextMode="MultiLine" runat="server" CssClass="txtMulti" onkeypress="return textboxMultilineMaxNumber(this,480)" ></asp:TextBox>
                        <asp:CustomValidator ID="CVJustification" runat="server" ErrorMessage="Justification needed if a conditional approval is requested"
                             CssClass="errlabels"  ControlToValidate="TxtJustification"  ValidateEmptyText="true" OnServerValidate="CVJustification_ServerValidate" ValidationGroup="add" Text="*"></asp:CustomValidator>
                        <asp:RegularExpressionValidator ID="RegexJustification" ControlToValidate="TxtJustification"
                            ErrorMessage="Justification exceeded 480 characters" runat="server" ValidationExpression="(?:[\r\n]*.[\r\n]*){0,480}"  CssClass="errlabels" 
                             ValidationGroup="add" Text="*"></asp:RegularExpressionValidator>
                         <asp:RegularExpressionValidator ID="RegexscJustify" runat="server" ErrorMessage="< and > is not allowed in Justification."  ValidationGroup="add" Display="Dynamic"  CssClass="errlabels" 
                        ControlToValidate="TxtJustification" ValidationExpression="^[^<>]*$"  Text="*"></asp:RegularExpressionValidator>
                    </div>
                </div>
                  <div class="row">&nbsp;</div>
            </div>
           
        </asp:Panel>
       
    
         <asp:Panel GroupingText ="Laser Worker Information" runat="server" >
              <div class="row">
                <div class="col-sm-6">
                     <asp:Label ID="LblOperator" runat="server" Text="Laser Worker: " CssClass="labelfieldleft"></asp:Label>
                    <div id="divOp" runat="server" style="display:inline-block;" visible="false">
                        <asp:TextBox ID="TxtWorker"  runat="server" ClientIDMode="Static" MaxLength="30" CssClass="txtnamewidth nounload" AutoPostBack="true" OnTextChanged="TxtWorker_TextChanged" ></asp:TextBox>
                        &nbsp;<asp:ImageButton ID="ImgBtnWorker" runat="server" ImageUrl="~/Images/find.gif"  CssClass="nounload"/>
                      &nbsp;<asp:Label ID="Lblformat" runat="server" CssClass="formattext" Text="(Lastname, firstname)"></asp:Label>
                      <asp:RequiredFieldValidator ID="RFVWorker" ErrorMessage="Worker Name is required" Text="*" ControlToValidate="TxtWorker" CssClass="errlabels"  ValidationGroup="add" runat="server" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                       <asp:CustomValidator ID="CvWorker" ErrorMessage="Not a valid name / format" Text="*" ControlToValidate="TxtWorker" CssClass="errlabels" ValidationGroup="add" runat="server" SetFocusOnError="true" OnServerValidate="CvWorker_ServerValidate" Display="Dynamic"></asp:CustomValidator>
                        <asp:Label ID="LblEmpId" runat="server" ClientIDMode="Static"  Width="36px" ForeColor="White"></asp:Label>
                    </div>
                    <div id="divOpview" runat="server" visible="true" style="display:inline-block;">
                         <asp:Label ID="LblOperatorVal" runat="server"></asp:Label>
                    </div>
                     
                </div>
                 <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
                <div class="col-sm-6">
                    <asp:Label ID="LblBadge" runat="server" Text="SLAC Badge Id: " CssClass="labelfieldleft"></asp:Label>
                    <asp:Label ID="LblBadgeVal" runat="server" ></asp:Label>
                </div>
                  </div>
             <div class="row">&nbsp;</div> 
             <div class="row">
                 <div class="col-sm-6">
                    <asp:Label ID="LblEmail" runat="server"  Text="SLAC Email: " CssClass="labelfieldleft"></asp:Label>
                    <asp:Label ID="LblEmailVal" runat="server"></asp:Label>
                </div>
                 <div class="visible-xs hidden-md hidden-sm hidden-lg" style="vertical-align:top;">&nbsp;</div>
                 <div class="col-sm-6">
                     <asp:Label ID="LblPreferredEmail" runat="server" Text="Enter Preferred Email: " CssClass="labelfieldleft"></asp:Label>
                     <asp:Textbox ID="TxtPreferredEmail" runat="server" CssClass="txtWidth"></asp:Textbox>
                      <asp:RegularExpressionValidator id="REVPreferred" runat="Server" ControlToValidate="TxtPreferredEmail"  ValidationExpression="^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$" ErrorMessage="Not a valid Preferred Email"  CssClass="errlabels" SetFocusOnError="true" ValidationGroup="add" Display="Dynamic" Text="*"></asp:RegularExpressionValidator>
                     <asp:RequiredFieldValidator ID="RFVPreferred" runat="server" ControlToValidate="TxtPreferredEmail" ErrorMessage="Preferred email required if no valid slac email" CssClass="errlabels" SetFocusOnError="true" Display="Dynamic" Text="*" ValidationGroup="add"></asp:RequiredFieldValidator>
                 </div>
                 
             </div>
              <div class="row"><div class="col-sm-6 hidden-xs">&nbsp;</div> <div class="col-sm-6">
                    <label  for="lbltxtemail"  class="formattext">(if different from SLAC Email)</label>

                 </div></div>
             <div class="visible-xs hidden-md hidden-sm hidden-lg" style="vertical-align:top;">&nbsp;</div>
             <div class="row">
                    <div class="col-sm-6">
                            <span  class="spanrequired" id="SpnAffiliate" runat="server">*</span> <asp:Label ID="LblSlacAffiliate" runat="server" Text="Select SLAC Affiliation: " CssClass="labelfieldleft"></asp:Label>
                             <asp:DropDownList ID="DdlSlacAffiliation" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlSlacAffiliation_SelectedIndexChanged" OnDataBound="DdlSlacAffiliation_DataBound" ClientIDMode="Static" CssClass="nounload" >
                             </asp:DropDownList>
                              <asp:CustomValidator ID="CvAffiliate" runat="server" EnableClientScript="true" Display="Dynamic" ClientValidationFunction="DdValidateAffiliation" ControlToValidate="DdlSlacAffiliation"  ErrorMessage="Slac Affiliation is required" SetFocusOnError="true" CssClass="errlabels"  ValidationGroup="add" Text="*" OnServerValidate="CvAffiliate_ServerValidate" ></asp:CustomValidator>
                      </div>
                      <div class="visible-xs hidden-md hidden-sm hidden-lg" style="vertical-align:top;">&nbsp;</div>
                      <div class="col-sm-6">
                            <asp:Label ID="LblSupervisor" runat="server"  Text="Admin Supervisor: " CssClass="labelfieldleft"></asp:Label>
                            <asp:Label ID="LblSupervisorVal" runat="server"></asp:Label>
                      </div>            
             </div>
             <div class="row">&nbsp;</div> 
             <div class="row">
                  <div class="col-sm-6">
                     <asp:Label ID="LblSlacId" runat="server" Text="Slac Id: " CssClass="labelfieldleft"></asp:Label>
                     <asp:Label ID="LblSlacIdVal" runat="server"></asp:Label>
                 </div>
             </div>
             <div class="row">&nbsp;</div> 
             <div class="row" id="DivAltsvrAdd" runat="server">
                 <div class="col-sm-6" >
                      <asp:Label ID="LblAlternate" runat="server"  CssClass="labelfieldleft">Alternate Supervisor Name:</asp:Label>
                      <asp:TextBox ID="TxtAlternate" runat="server" ClientIDMode="Static" MaxLength="30" CssClass="txtnamewidth" AutoPostBack="false" ></asp:TextBox>
                      &nbsp;<asp:ImageButton ID="ImgBtAlt" runat="server" ImageUrl="~/Images/find.gif" CssClass="nounload" />
                      &nbsp;<asp:Label ID="Label1" runat="server" CssClass="formattext" Text="(Lastname, firstname)"></asp:Label>                   
                     <asp:CustomValidator ID="CvAlt" ErrorMessage="Not a valid Alternate name / format"  ControlToValidate="TxtAlternate" CssClass="errlabels" ValidationGroup="add" runat="server" SetFocusOnError="true" OnServerValidate="CvAlt_ServerValidate" Display="Dynamic" Text="*"></asp:CustomValidator>
                    
                  </div>
                 <div class="visible-xs hidden-md hidden-sm hidden-lg" style="vertical-align:top;">&nbsp;</div>
                 <div class="col-sm-3">
                 
                      <asp:Label ID="LblFromDate" runat="Server" CssClass="labelfieldleft" >Date From:</asp:Label>
                      <asp:TextBox ID="TxtFromDate" runat="server" Width="110px" ClientIDMode="Static"></asp:TextBox>
                      
                      <asp:CustomValidator ID="CustomFrom" runat="server" ErrorMessage="Alt Svr From date required" ControlToValidate="TxtFromDate" CssClass="errlabels" ValidationGroup="add"   OnServerValidate="CustomFrom_ServerValidate" Display="Dynamic" Text="*" ValidateEmptyText="true" ></asp:CustomValidator>
                      <asp:CompareValidator ID="CvFrom" runat="server" Type="Date" Operator="DataTypeCheck"
                                                                ControlToValidate="TxtFromDate" ErrorMessage="Date From not a valid date!"  Text="*" CssClass="errlabels" ValidationGroup="add"></asp:CompareValidator>
                     <span id="spnFromfrmt" class="formattext">(mm/dd/yyyy)</span>
                 </div>
                 <div class="visible-xs hidden-md hidden-sm hidden-lg" style="vertical-align:top;">&nbsp;</div>
                 <div class="col-sm-3">
                     <asp:Label ID="LblToDate" runat="Server" CssClass="labelfieldleft" >Date To:</asp:Label>
                      <asp:TextBox ID="TxtToDate" runat="server" Width="110px" ClientIDMode="Static"></asp:TextBox>
                    
                      <asp:CustomValidator ID="CustomTo" runat="server" ErrorMessage="Alt Svr To date required"  ControlToValidate="TxtToDate" CssClass="errlabels" ValidationGroup="add"  OnServerValidate="CustomTo_ServerValidate" Display="Dynamic" Text="*"  ValidateEmptyText="true" ></asp:CustomValidator>
                        <asp:CompareValidator ID="CvTo" runat="server" Type="Date" Operator="DataTypeCheck"
                                                                ControlToValidate="TxtToDate" ErrorMessage="Date To not a valid date!"  Text="*" CssClass="errlabels" ValidationGroup="add"></asp:CompareValidator>
                      <span id="spnTofrmt" class="formattext">(mm/dd/yyyy)</span>
                 </div>
             </div>
             <div class="row" id ="DivAltsvr" runat="server" visible="false"> 
                 
                 <div class="col-sm-6" >
                     <asp:Label ID="LblAltSvr" runat="server" Text="Alternate Supervisor: " CssClass="labelfieldleft"></asp:Label>
                      <asp:Label ID="LblAltSvrVal" runat="server"></asp:Label>&nbsp;&nbsp;<b>From:</b>&nbsp;&nbsp;
                      <asp:Label ID="LblFrom" runat="server"></asp:Label>&nbsp;&nbsp;<b>To:</b>&nbsp;&nbsp;
                      <asp:Label ID="LblTo" runat="server"></asp:Label>
                 </div>
                 
             </div>
              <div class="row">&nbsp;</div>
               <div class="row">
                 <div class="col-sm-8 labelfieldleft">
                     
                     <span  class="spanrequired" id="spnESHManual" runat="server">*</span>
                     I have read ESH Manual <a href="http://www-group.slac.stanford.edu/esh/hazardous_activities/laser/" target="_blank">Chapter 10, "Laser Safety",</a> and accepts roles and responsibilities described therein.
                      </div>
                   <div class="col-sm-4 labelfieldleft">
                      <div style="float:left;">
                          <asp:RadioButtonList ID="RbEshManual" runat="server" RepeatDirection="Horizontal" CellPadding="2" CellSpacing="5" visible="true" >
                            <asp:ListItem  Value="0"> &nbsp;Yes &nbsp;</asp:ListItem>
                            <asp:ListItem  Value="1" Selected="True">&nbsp; No &nbsp; </asp:ListItem> 
                        </asp:RadioButtonList>
                       </div>
                        <asp:CustomValidator ID="CVESH" runat="server" ControlToValidate="RbEshManual" CssClass="errlabels" Display="Dynamic" ErrorMessage="Must read ESH Manual Chapter 10 and accept roles and responsibilities described therein" OnServerValidate="CVESH_ServerValidate" Text="*" SetFocusOnError="true" ValidateEmptyText="true" ValidationGroup="add" ></asp:CustomValidator> 
                     
                         <div>
                             <asp:Label ID="LblEshManualVal" runat="server" Visible="false"></asp:Label>
                         </div>  
                        
                      
                   </div> 
                 

                
             </div>
              <div class="row">&nbsp;</div>           
             
             <div  id="DivSOPterms" class="row" runat="server" visible ="false">
                 <div class="col-sm-8 labelfieldleft">
                      <asp:CustomValidator ID="CVSOP" runat="server" ControlToValidate="RblSOP" EnableClientScript="true" CssClass="errlabels" Display="Dynamic" ValidationGroup="add" SetFocusOnError="true" ErrorMessage="Must read the applicable standard operating procedure for the facility selected" ValidateEmptyText="true" OnServerValidate="CVSOP_ServerValidate" Text="*"></asp:CustomValidator>
                    I have read and understood the applicable standard operating procedure (SOP) document(s) for the <asp:Label ID="Lblfac" runat="server"></asp:Label>
                    laser facility,  linked from its  <a href="" id="aFacSite" runat="server" target="_blank">SharePoint webpage</a> 
                </div>
                  <div class="col-sm-4 leftalign">
                     <asp:RadioButtonList ID="RblSOP" runat="server" RepeatDirection="Horizontal" CellPadding="2" CellSpacing="5" Visible="true" >
                         <asp:ListItem  Value="0">&nbsp;Yes &nbsp;</asp:ListItem>
                         <asp:ListItem  Value="1" Selected="True">&nbsp; No &nbsp;</asp:ListItem>
                     </asp:RadioButtonList> 
                      <asp:Label ID="LblSOPVal" runat="server" Visible="false"></asp:Label>
                     
                 </div>
             </div>
             <div id="divrowstud" class="row" runat="server" visible="false">&nbsp;</div>
             <div id="divStudent" class="row" runat="server" visible ="false">
                 <div class="col-sm-8 labelfieldleft">
                     <asp:CustomValidator ID="CVStudent" runat="server" ControlToValidate="RblStudreq" CssClass="errlabels" Display="Dynamic" ErrorMessage="Students must read and agree to comply with Student Requirements" ValidateEmptyText=" true" SetFocusOnError="true" ValidationGroup="add" OnServerValidate="CVStudent_ServerValidate" Text="*"></asp:CustomValidator>
                     I have read and agrees to comply with 
                     <a href="http://www-group.slac.stanford.edu/esh/eshmanual/references/laserReqStudent.pdf" target="_blank">Laser Safety: Student Requirements</a>
                 </div>
                 <div class="col-sm-4 leftalign">
                     <asp:RadioButtonList ID="RblStudreq" runat="server" RepeatDirection="Horizontal" CellPadding="2" CellSpacing="5" >
                         <asp:ListItem  Value="0">&nbsp;Yes &nbsp;</asp:ListItem>
                         <asp:ListItem  Value="1" Selected="true">&nbsp; No &nbsp;</asp:ListItem>
                     </asp:RadioButtonList>
                     <asp:Label ID="LblStudVal" runat="server" ></asp:Label>
                     
                 </div>
             </div>
         </asp:Panel>
        <br />
    
     <UC1:UCTraining ID="UCTraining1" runat="server" Visible="false" />
      
       <UC2:UCUserLab ID="UCUserLab" runat="server" Visible="false"/>

        <asp:SqlDataSource ID="SDSWorkType" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>" SelectCommand="SELECT LOOKUP_ID, LOOKUP_DESC FROM LST_LOOKUP WHERE LOOKUP_GROUP = 'WorkerType' and IS_ACTIVE='Y' ORDER BY LOOKUP_DESC"></asp:SqlDataSource>
    
        <asp:SqlDataSource ID="SDSAffiliation" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>" SelectCommand="SELECT LOOKUP_ID, LOOKUP_DESC FROM LST_LOOKUP WHERE LOOKUP_GROUP='Affiliation' AND IS_ACTIVE='Y' ORDER BY LOOKUP_DESC"></asp:SqlDataSource>
      <div id="dialog-appreqmsg" style="display:none;" class="nounload">
            <p>
                <asp:Label ID="LblWorkerMsg" runat="server"></asp:Label>
            </p>
        </div>
     <div id="dialogowneradmin" style="display:none;"  >             
                <iframe id="modaldialogowneradmin"  frameborder="1" width="100%" height="100%" >
                </iframe>
          </div>
    <asp:HiddenField ID="HdnEmpid" runat="server" ClientIDMode="Static"/>
</asp:Content>
