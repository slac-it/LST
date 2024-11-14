<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile ="~/LST.Master" CodeBehind="Facility.aspx.cs"  StylesheetTheme="LSTTheme" Inherits="LST.NewFacility" MaintainScrollPositionOnPostback="true" %>
<%@ Register Src="~/UserControls/AddLink.ascx" TagName="UCLink" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/FileAttach.ascx" TagName ="UCFile" TagPrefix="uc2" %>
<%@ Register Src="~/UserControls/FileSummary.ascx" TagName="UCFileList" TagPrefix="uc3" %>
<%@ Register Src="~/UserControls/FacilityWorkersSummary.ascx" TagName="UCWorkFac" TagPrefix="uc4" %>
<%@ Register Src="~/UserControls/OJTWorkersMatrix.ascx" TagName="UCOJT" TagPrefix="uc5" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     
    <script type="text/javascript" src="Scripts/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script>    
    <script type="text/javascript" src="Scripts/LST.js"></script>
     
    <script type="text/javascript">

  
        function ResetTextbox()
        {
            //just a workaround 
            return true;
        }

       


        function DdValidateSLSO(source, arguments) {

            var ctrl = document.forms[0].elements["DdlSLSO"].value;

            if (ctrl == "-1") {
                arguments.IsValid = false;
                return false;
            }
            else {
                return true;
            }
        }

        $(document).ready(function () {
            needToConfirm = false;
            window.onbeforeunload = askConfirm;
        });

        function askConfirm() {
            var mode = $("#HdnMode").val();
            if ((mode == "edit") || (mode == "")) { needToConfirm = true;}
            if (needToConfirm) {
                // Put your custom message here 
                return "Important: You have unsaved data on this page. Please click on \'Submit\' or \'Update\' buttons to save your data";
            }
        }


       

        $(function () {
            $(".nounload").click(function () {
                window.onbeforeunload = null;
            });

        });
      
       

    </script>
    
   
       <asp:Panel ID="PnlAdd" runat="server" Visible="true" GroupingText="Facility">
       
        
            <div class="row">
                
             <div class="col-xs-6 rightalign">
                     <span  class="spanrequired">* </span> indicates required field
              </div>
                 <div class="col-xs-6 leftalign">
                <div id="divAdd" runat="server" visible="true">
                    <asp:Button cssClass="btn btn-primary nounload" Text="Submit" runat="server" id="BtnSubmit" OnClick="BtnSubmit_Click" ValidationGroup="add" /> &nbsp;
                    <asp:Button CssClass="btn btn-primary" Text="Cancel" runat="server" ID="BtnCancel" OnClick="BtnCancel_Click" /> &nbsp;
                </div>
                 <div id="divEdit" runat="server" visible="false">
                    <asp:Button cssClass="btn btn-primary nounload" Text="Update" runat="server" id="BtnUpdate"  ValidationGroup="add" OnClick="BtnUpdate_Click" /> &nbsp;
                    <asp:Button CssClass="btn btn-primary" Text="Cancel" runat="server" ID="BtnCancelupd" OnClick="BtnCancelupd_Click" /> &nbsp;
                </div>
                 
            </div>     
        </div>
        <div>&nbsp;</div>
        <div class="row">
                <div class="col-xs-3">&nbsp;</div>
                <div class="col-xs-5 leftalign">
                    <asp:ValidationSummary ID="VSErrors" DisplayMode="BulletList" BorderStyle="solid" BorderWidth="1px"  CssClass="errlabels" EnableClientScript ="true" HeaderText="The following errors occurred:" runat="server"  ValidationGroup="add"/>
                </div>
            
        </div>
           <div>&nbsp;</div>
        <div class="row">
        
                 <div class="col-xs-4 rightalign" ><span id="spnFac" class="spanrequired">*</span><asp:label id="LblFacility" runat="server" CssClass="control-label  labelfieldleft">Facility Name: </asp:label></div>               
                 <div class="col-xs-8 leftalign" ><asp:Textbox id="TxtFacility"  runat="server" MaxLength="100" CssClass="txtWidth"></asp:Textbox>
                 <asp:RequiredFieldValidator  ID="RfvFacility" runat="server" CssClass="errlabels" ControlToValidate="TxtFacility" ValidationGroup="add" ErrorMessage="Facility Name required" Text="*"></asp:RequiredFieldValidator>
                     <asp:RegularExpressionValidator ID="RegexscFacility" runat="server" ErrorMessage="< and > is not allowed in Facility Name."  ValidationGroup="add" Display="Dynamic"  CssClass="errlabels" 
                        ControlToValidate="TxtFacility" ValidationExpression="^[^<>]*$"  Text="*"></asp:RegularExpressionValidator>
                </div>
        </div>
        <div>&nbsp;</div>
        <div class="row">       
                <div class="col-xs-4 rightalign"><asp:label id="LblBldg" runat="server" CssClass="control-label labelfieldleft">Building: </asp:label></div> 
                <div class="col-xs-8 leftalign"> <asp:Dropdownlist ID="DdlBldg" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlBldg_SelectedIndexChanged" ClientIDMode="Static" CssClass="nounload">
              </asp:Dropdownlist><span id="spnbldg" runat="server" visible="false"></span>
               &nbsp;&nbsp;
              <asp:Label ID="LblRoom" runat="server" CssClass="labelfieldleft">Room:</asp:Label>
              <asp:Dropdownlist ID="DdlRoom" runat="server">
                             </asp:Dropdownlist>
            
            </div>
       </div>
  
       <div>&nbsp;</div>
        <div class="row">
                <div class="col-xs-4 rightalign"><asp:Label ID="LblLocate" runat="server" CssClass="control-label labelfieldleft">Other Location:</asp:Label></div>
                <div class="col-xs-8 leftalign"><asp:TextBox ID="TxtLocate" runat="server" CssClass="txtWidth" MaxLength="100" ClientIDMode="Static"></asp:TextBox>
                    <span class="formattext">(max. 100 characters)</span>
                    <asp:CustomValidator ID="CVLocation" runat="server" EnableClientScript="true" Display="Dynamic" ControlToValidate="TxtLocate"  ErrorMessage="Location or bldg is required. Please enter one" SetFocusOnError="true" CssClass="errlabels"  ValidationGroup="add"  Text="*"  ></asp:CustomValidator>
                     <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="< and > is not allowed in Other Location."  ValidationGroup="add" Display="Dynamic"  CssClass="errlabels" 
                        ControlToValidate="TxtLocate" ValidationExpression="^[^<>]*$"  Text="*"></asp:RegularExpressionValidator>
                </div>
        </div>
        <div>&nbsp;</div>
        <div  class="row">
                <div class="col-xs-4 rightalign"><asp:Label ID="LblWeb" runat="server" CssClass="labelfieldleft"><span id="spnwebpage" class="spanrequired">*</span>Facility Web Page:</asp:Label></div>
                <div class="col-xs-8 leftalign"><asp:TextBox ID="TxtWeb" runat="server"  CssClass="txtWidth" MaxLength="100"></asp:TextBox> <span class="formattext">(max. 100 characters)</span>
                    <asp:RequiredFieldValidator ID="RFVUrl" runat="server" ControlToValidate="TxtWeb" ErrorMessage="Facility Web Page is required" Text="*" SetFocusOnError="true" Display="Dynamic" ValidationGroup="add" CssClass="errlabels"></asp:RequiredFieldValidator>
                     <asp:RegularExpressionValidator ID="RegExUrl" runat="Server"  ControlToValidate="TxtWeb"  ValidationExpression="(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=-_]*)?" ErrorMessage="Facility Webpage doesn't have a valid URL"  Display="Dynamic" CssClass="errlabels"  Text="*"  ValidationGroup="add" />
                </div>
        </div>
        <div>&nbsp;</div>
        <div class="row">
                 <div class="col-xs-4 rightalign">
                     
                     <asp:Label ID="LblSLSO" runat="server" CssClass="labelfieldleft"><span id="spnslso" class="spanrequired">*</span>SLSO:</asp:Label></div>
                <div class="col-xs-8 leftalign">

                <asp:Dropdownlist ID="DdlSLSO" runat="server"   OnDataBound="DdlSLSO_DataBound" ClientIDMode="Static" CssClass="nounload">                    
              </asp:Dropdownlist>
               <asp:CustomValidator ID="CvSLSO" runat="server" EnableClientScript="true" Display="Dynamic" ClientValidationFunction="DdValidateSLSO" ControlToValidate="DdlSLSO"  ErrorMessage="SLSO is required. Please select one" SetFocusOnError="true" CssClass="errlabels"  ValidationGroup="add"  Text="*" OnServerValidate="CvSLSO_ServerValidate" ></asp:CustomValidator>
            </div>
        </div>
        <div>&nbsp;</div>
          <div class="row">
                 <div class="col-xs-4 rightalign"><asp:Label ID="LblcoSLSO" runat="server" CssClass="labelfieldleft">Co-SLSO:</asp:Label></div>
                 <div class="col-xs-8 leftalign">
                 <asp:Dropdownlist ID="DdlCoSLSO" runat="server"  OnDataBound="DdlCoSLSO_DataBound" ClientIDMode="Static" CssClass="nounload">
                 </asp:Dropdownlist></div>     
           </div>
        <div>&nbsp;</div>
         <div class="row">
                 <div class="col-xs-4 rightalign"><asp:Label ID="LblActing" runat="server" CssClass="labelfieldleft">Acting SLSO:</asp:Label></div>
                 <div class="col-xs-8 leftalign">
                     <asp:Dropdownlist ID="DdlActSLSO" runat="server"  OnDataBound="DdlActSLSO_DataBound" CssClass="nounload">
                 
              </asp:Dropdownlist></div>          
           </div>
           <div>&nbsp;</div>
           <div class="row">
                <div class="col-xs-4 rightalign"><asp:Label ID="LblPgMgr" runat="server" CssClass="labelfieldleft">Program Manager:</asp:Label></div>
                 <div class="col-xs-8 leftalign">
                      <asp:TextBox ID="TxtPrgMgr"  runat="server" ClientIDMode="Static" MaxLength="30" CssClass="txtnamewidth nounload" OnTextChanged="TxtPrgMgr_TextChanged" AutoPostBack="true" ></asp:TextBox>
                   
                        &nbsp;<asp:ImageButton ID="ImgBtnPM" runat="server" ImageUrl="~/Images/find.gif"  CssClass="nounload"/>
                      &nbsp;<asp:Label ID="LblformatPM" runat="server" CssClass="formattext" Text="(Lastname, firstname)"></asp:Label>                     
                       <asp:CustomValidator ID="CvPM" ErrorMessage="Not a valid name / format" Text="*" ControlToValidate="TxtPrgMgr" CssClass="errlabels" ValidationGroup="add" runat="server" SetFocusOnError="true" OnServerValidate="CvPM_ServerValidate" Display="Dynamic"></asp:CustomValidator>
                       
                  </div>  
           </div>
           <div>&nbsp;</div>
           <div class="row">
                <div class="col-xs-4 rightalign"><asp:Label ID="LblCoord" runat="server" CssClass="labelfieldleft">ESH Co-ordinator:</asp:Label></div>
                 <div class="col-xs-8 leftalign">
                      <asp:TextBox ID="TxtCoordinator"  runat="server" ClientIDMode="Static" MaxLength="30" CssClass="txtnamewidth nounload" OnTextChanged="TxtCoordinator_TextChanged" AutoPostBack="true"  ></asp:TextBox>
                       
                        &nbsp;<asp:ImageButton ID="ImgBtnCoord" runat="server" ImageUrl="~/Images/find.gif"  CssClass="nounload"/>
                      &nbsp;<asp:Label ID="LblFormatCoord" runat="server" CssClass="formattext" Text="(Lastname, firstname)"></asp:Label>
                      <asp:CustomValidator ID="CvCoord" ErrorMessage="Not a valid name / format" Text="*" ControlToValidate="TxtCoordinator" CssClass="errlabels" ValidationGroup="add" runat="server" SetFocusOnError="true" OnServerValidate="CvCoord_ServerValidate" Display="Dynamic"></asp:CustomValidator>
                     
                  </div>  
           </div>
        <div>&nbsp;</div>
        <div class="row">
                    <div class="col-xs-4 rightalign"><span id="SpnSOP" class="spanrequired">*</span><asp:Label ID="LblSOP" runat="server" CssClass="labelfieldleft">SOP Approval Date:</asp:Label></div> 
                    <div class="col-xs-8 leftalign"><asp:TextBox ID="TxtSOP" runat="server" ClientIDMode="Static"></asp:TextBox>
                        <span id="spnSOPfrmt" class="formattext">(mm/dd/yyyy)</span>
                        <asp:RequiredFieldValidator ID="RfvSOP" runat="Server" ControlToValidate="TxtSOP" ErrorMessage ="SOP Approval Date is required" ValidationGroup="add" Text="*" CssClass="errlabels"></asp:RequiredFieldValidator>
                         <asp:CompareValidator ID="CvSOP" runat="server" Type="Date" Operator="DataTypeCheck"
                                                                ControlToValidate="TxtSOP" ErrorMessage="Not a valid date!"  Text="*" CssClass="errlabels" ValidationGroup="add"></asp:CompareValidator>
                    </div>
        </div>
        <div>&nbsp;</div>
        <div class="row">
                 <div class="col-xs-4 rightalign"><asp:Label ID="LblExpirydate" runat="server" CssClass="labelfieldleft">Approval Expiry Date:</asp:Label></div>
                 <div class="col-xs-8 leftalign"><asp:TextBox ID="TxtExpirydate" runat="server" ClientIDMode="Static"></asp:TextBox>
                      <span id="spnExpformat" class="formattext">(mm/dd/yyyy)</span>
                     <asp:CompareValidator ID="CvExpiryDate" runat="server" Type="Date" Operator="DataTypeCheck"
                                                                ControlToValidate="TxtExpirydate" ErrorMessage="Not a valid date!"  Text="*" CssClass="errlabels" ValidationGroup="add"></asp:CompareValidator>
                 </div>
        </div>
        <div>&nbsp;</div>
        
           <div class="row">
               <div class="col-xs-12" style="text-align:center;"> <span style="font-size:x-small;">**Note :- File Upload size limited to 10MB (doc, docx, jpg, bmp, pdf, xls, xlsx, png, txt, gif)</span></div>
           </div>
        <div class="row">
            <div class="col-xs-4 rightalign">
                <asp:Label ID="LblFileAttachment" runat="server" CssClass="labelfieldleft">Attachments:</asp:Label>
             </div>
            <div class="col-xs-8 leftalign">
               <uc2:UCFile ID="UCFile1" runat="server" />
            </div>
            
        </div>
        <div>&nbsp;</div>
       

        </asp:Panel>
       <asp:Panel ID="PnlView" runat="server" Visible="false" GroupingText="Facility Information">
           <div class="row">
               <div class="col-xs-8">&nbsp;</div>
               <div class="col-xs-4"><asp:Button ID="BtnBack" runat="server" CausesValidation="false" Text="Back" CssClass="btn btn-primary nounload" OnClick="BtnBack_Click" /></div>
           </div>
           <div class="row">
               <div class="col-xs-4 rightalign"><asp:Label ID="LblFacilityView" runat="server" CssClass="control-label labelfieldleft" Text="Facility Name:"></asp:Label></div>
               <div class="col-xs-8 leftalign"><asp:Label ID="LblFacilityVal" runat="server"></asp:Label> </div>
           </div>
               <div>&nbsp;</div>
           <div class="row">
               <div class="col-xs-4 rightalign"><asp:Label ID="LblBldgView" runat="server" CssClass="control-label labelfieldleft" Text="Building:"></asp:Label></div>
               <div class="col-xs-8 leftalign"><asp:Label ID="LblBldgVal" runat="server"></asp:Label>
                &nbsp;&nbsp; <asp:Label ID="LblRoomView" runat="server" CssClass="control-label labelfieldleft" Text="Room:"></asp:Label>
                   <asp:Label ID="LblRoomVal" runat="server"></asp:Label>
               </div>
           </div>
           <div>&nbsp;</div>
           <div class="row">
               <div class="col-xs-4 rightalign"><asp:Label ID="LblLocateView" runat="server" CssClass="control-label labelfieldleft" Text="Other Location:"></asp:Label></div>
               <div class="col-xs-8 leftalign"><asp:Label ID="LblLocateVal" runat="server" ></asp:Label></div>
           </div>
           <div>&nbsp;</div>
           <div class="row">
               <div class="col-xs-4 rightalign"><asp:Label ID="LblWebView" runat="server" CssClass="control-label labelfieldleft" Text="Facility Web Page:"></asp:Label></div>
               <div class="col-xs-8 leftalign"><a href="" id="aWebPage" runat="server" target="_blank"></a>
                   <asp:Label ID="LblWebVal" runat="server" Visible="false"></asp:Label>
               </div>
           </div>
           <div>&nbsp;</div>
           <div class="row">
                <div class="col-xs-4 rightalign"><asp:Label ID="LblSLSOView" runat="server" CssClass="control-label labelfieldleft" text="SLSO:"></asp:Label></div>
                <div class="col-xs-8 leftalign"> <asp:Label ID="LblSLSOVal" runat="server"></asp:Label></div>
           </div>
           <div>&nbsp;</div>
           <div class="row">
                <div class="col-xs-4 rightalign"><asp:Label ID="LblcoSLSOView" runat="server" CssClass="control-label labelfieldleft" text="Co-SLSO:"></asp:Label></div>
                <div class="col-xs-8 leftalign"> <asp:Label ID="LblcoSLSOVal" runat="server"></asp:Label></div>
           </div>
           <div>&nbsp;</div>
           <div class="row">
                <div class="col-xs-4 rightalign"><asp:Label ID="LblActingView" runat="server" CssClass="control-label labelfieldleft" text="Acting SLSO:"></asp:Label></div>
                <div class="col-xs-8 leftalign"> <asp:Label ID="LblActingVal" runat="server"></asp:Label></div>
           </div>
           <div>&nbsp;</div>
           <div class="row" id="DivAltSlso" runat="server" visible="false">
                <div class="col-xs-4 rightalign"><asp:Label ID="LblAltSlsoView" runat="server" CssClass="labelfieldleft">Alternate SLSO:</asp:Label> </div>
                <div class="col-xs-8 leftalign"><asp:Label ID="LblAltSlsoVal" runat="server"></asp:Label>
                  &nbsp;&nbsp;<b>From:</b>&nbsp;&nbsp;  <asp:Label ID="LblFrom" runat="server"></asp:Label>&nbsp;&nbsp;<b>To:&nbsp;&nbsp;</b> <asp:Label ID="LblTo" runat="server"></asp:Label>
                </div>
            </div>

           <div id="divaltslsoline" runat="server" visible="false">&nbsp;</div>
           <div class="row">
               <div class="col-xs-4 rightalign"><asp:Label ID="LblPMView" runat="server" CssClass="control-label labelfieldleft" Text="Program Manager:"></asp:Label></div>
               <div class="col-xs-8 leftalign"><asp:Label ID="LblPMVal" runat="server"></asp:Label></div>
           </div>
            <div>&nbsp;</div>
            <div class="row">
               <div class="col-xs-4 rightalign"><asp:Label ID="LblCoordView" runat="server" CssClass="control-label labelfieldleft" Text="ESH Co-ordinator:"></asp:Label></div>
               <div class="col-xs-8 leftalign"><asp:Label ID="LblCoordVal" runat="server"></asp:Label></div>
           </div>
            <div>&nbsp;</div>
           <div class="row">
                <div class="col-xs-4 rightalign"><asp:Label ID="LblSOPView" runat="server" CssClass="control-label labelfieldleft" text="SOP Approval Date:"></asp:Label></div>
                <div class="col-xs-8 leftalign"> <asp:Label ID="LblSOPVal" runat="server"></asp:Label>
                </div>

           </div>
            <div>&nbsp;</div>
           <div class="row">
                <div class="col-xs-4 rightalign"><asp:Label ID="LblExpdateView" runat="server" CssClass="control-label labelfieldleft" text="Approval Expiry Date:"></asp:Label></div>
                <div class="col-xs-8 leftalign"> <asp:Label ID="LblExpdateVal" runat="server"></asp:Label>
                  &nbsp;&nbsp;&nbsp;&nbsp; 
                    <asp:LinkButton ID="LnkReqFacApp" runat="server" CssClass="nounload" OnClick ="LnkReqFacApp_Click" Text="Request Facility Approval"></asp:LinkButton>
                </div>
                
           </div>
            <div>&nbsp;</div>
  
       </asp:Panel>
    
      <p></p>
      
 
     <uc3:UCFileList ID="UCFileList1" runat="server" ClientIDMode="Static"  />
     
    <br /> 
     <uc4:UCWorkFac ID="UCWorkfac1" runat="server"  Visible="false" />
     <br /> 
       
     <uc5:UCOJT ID="UCOJTMatrix" runat="server" Visible="false" ClientIDMode="Static" />
     
         
        <div id="dialog-facmsg" title="Facility" style="display:none;">
            <p>
                <asp:Label ID="LblFacMsg" runat="server"></asp:Label>
            </p>
        </div>

         <div id="dialogowneradmin" style="display:none;"  >             
                <iframe id="modaldialogowneradmin"  frameborder="1" width="100%" height="100%" >
                </iframe>
          </div>

        <div id="dialogOJTFields" style="display:none;">
             <iframe id="modaldialogOJTFields"  frameborder="1" width="100%" height="100%" >
                </iframe>   
        </div>

        <asp:SqlDataSource ID="SDSRoles" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>" SelectCommand="SELECT UR.USER_ROLE_ID,  PC.EMPLOYEE_NAME FROM LST_USER_ROLES UR LEFT JOIN VW_PEOPLE_CURRENT PC ON PC.EMPLOYEE_ID = UR.SLAC_ID WHERE UR.IS_ACTIVE ='Y' AND UR.ROLE_TYPE_ID = 15 ORDER BY PC.EMPLOYEE_NAME "></asp:SqlDataSource>
       <asp:HiddenField ID="HdnPMId" runat="server" ClientIDMode="Static" />
       <asp:HiddenField ID="HdnCoordId" runat="server" ClientIDMode ="Static" />
            <asp:HiddenField ID="HdnMode" runat="server" ClientIDMode="Static" />
</asp:Content>
   