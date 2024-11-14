<%@ Page Title="" Language="C#" MasterPageFile="~/LST.Master" AutoEventWireup="true" CodeBehind="EmailTemplate.aspx.cs" Inherits="LST.EmailTemplate" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <script type="text/javascript" src="Scripts/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script>    
    <script type="text/javascript" src="Scripts/LST.js"></script>
     <script type="text/javascript" src="Scripts/backbutton.js"></script>

      <script type="text/javascript">
        window.onbeforeunload = function (event) {
            var message = 'Important: You have unsaved data on this page. Please click \'Send Email \' to send or \'Cancel\' button to clear the entries';
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
    <!-- Todo add unload msg and add validation required and check max length -->
     <div class="row">
                <div class="col-sm-3">&nbsp;</div>
                <div class="col-sm-5 leftalign">
                     <div style="display:block;"> <asp:ValidationSummary ID="VSErrors" DisplayMode="BulletList" BorderStyle="solid" BorderWidth="1px"  CssClass="errlabels" EnableClientScript ="true" HeaderText="The following errors occurred:" runat="server"  ValidationGroup="send"/></div>
                </div>
            
        </div>
    <asp:Panel ID="PnlEmail" runat="server" Visible="true" GroupingText="Email" Width="100%" >
        <div class="row">
            <div class="col-sm-4">
                <asp:Label ID="LblTo" runat="server" Text="To" CssClass="labelfieldleft"></asp:Label>
            </div>
            <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
            <div class="col-sm-8">
                <asp:DropDownList ID="DdlFacility" cssClass="form-control input-md ddinline nounload"  Width="45%"  runat="server" DataSourceID="SDSFac" DataValueField="FACILITY_ID" DataTextField="FACILITY_NAME" AutoPostBack="True" OnSelectedIndexChanged="DdlFacility_SelectedIndexChanged" AppendDataBoundItems="true" >
                    <asp:ListItem Value="-1" Text="--Choose One--" Selected="True"></asp:ListItem>
                    <asp:ListItem Value="0" Text="All Facilities" ></asp:ListItem>
                </asp:DropDownList>
                &nbsp; <asp:DropDownList ID="DdlEmailList" runat="server" Width="45%"  cssClass="form-control input-md ddinline" DataSourceID="SDSList" DataValueField="LOOKUP_ID" DataTextField="LOOKUP_DESC" Visible="false"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="RVFac" runat="server" ControlToValidate="DdlFacility" ValidationGroup="send" Text="*" InitialValue="-1" ErrorMessage="All Facilities or one of the facilities must be selected" SetFocusOnError="true" CssClass="errlabels"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row" >
            <div class="col-sm-4">
                <asp:Label ID="LblSubject" runat="server" Text="Subject" CssClass="labelfieldleft"></asp:Label>
            </div>
            <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
            <div class="col-sm-8">
               
                <asp:TextBox ID="TxtSubject"  cssClass="form-control input-sm"  runat="server" Width="600px" MaxLength="480"  TextMode="SingleLine" style="display: inline"></asp:TextBox>
                 <span id="spnfrmt" class="formattext"> (max. 480 chars) </span>
                 <asp:RequiredFieldValidator ID="RVSubject" runat="server" ErrorMessage="Subject cannot be empty"
                             CssClass="errlabels"  ControlToValidate="TxtSubject" SetFocusOnError="true"     ValidationGroup="send" Text="*" Display="Dynamic" ></asp:RequiredFieldValidator> &nbsp&nbsp;
                   <asp:RegularExpressionValidator ID="RegexSubject"  ControlToValidate="TxtSubject" ErrorMessage="Subject exceeded 480 characters" runat="server" ValidationExpression="(?:[\r\n]*.[\r\n]*){0,480}"  CssClass="errlabels" 
                             ValidationGroup="send" Text="*"></asp:RegularExpressionValidator>
                 <asp:RegularExpressionValidator ID="RegexscSubject" runat="server" ErrorMessage="< and > is not allowed in Subject."  ValidationGroup="send" Display="Dynamic"  CssClass="errlabels" 
                        ControlToValidate="TxtSubject" ValidationExpression="^[^<>]*$" Text="*"></asp:RegularExpressionValidator>
            </div>
        </div>
        <div class="row">&nbsp;</div>
        <div class="row">
            <div class="col-sm-4">
                <asp:Label ID="LblBody" runat="server" Text="Message" CssClass="labelfieldleft"></asp:Label>
            </div>
            <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
            <div class="col-sm-8">
                <asp:TextBox ID="TxtBody" runat="server" cssClass="form-control input-sm" TextMode="MultiLine" Width="600px" Height="500px" style="display: inline"></asp:TextBox> <span id="spnfrmtbdy" class="formattext"> (max. 3800 chars) </span>
                 <asp:RequiredFieldValidator ID="RVBody" runat="server" SetFocusOnError="true" ErrorMessage="Message body cannot be empty"
                             CssClass="errlabels"  ControlToValidate="TxtBody"  ValidationGroup="send" Text="*" Display="Dynamic" ></asp:RequiredFieldValidator> &nbsp&nbsp;
                   <asp:RegularExpressionValidator ID="RegexBody" ControlToValidate="TxtBody" ErrorMessage="Message body exceeded 3800 characters" runat="server" ValidationExpression="(?:[\r\n]*.[\r\n]*){0,3800}"  CssClass="errlabels" 
                             ValidationGroup="send" Text="*"></asp:RegularExpressionValidator>
                 <asp:RegularExpressionValidator ID="RegexscBody" runat="server" ErrorMessage="< and > is not allowed in Message."  ValidationGroup="send" Display="Dynamic"  CssClass="errlabels" 
                        ControlToValidate="TxtBody" ValidationExpression="^[^<>]*$"  Text="*"></asp:RegularExpressionValidator>
            </div>
        </div>
        <asp:SqlDataSource ID="SDSList" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>" SelectCommand="SELECT LOOKUP_ID , LOOKUP_DESC  FROM LST_LOOKUP WHERE IS_ACTIVE='Y'
AND LOOKUP_GROUP='ToEmailList' and PARENT_ID IS not NULL AND Parent_ID = 21 ORDER BY LOOKUP_ID"></asp:SqlDataSource>
        <div class="row">&nbsp;</div>
        <div class="row">
             <div class="col-sm-4"></div>
             <div class="visible-xs hidden-md hidden-sm hidden-lg">&nbsp;</div>
            <div class="col-sm-8"  style="text-align:match-parent;" >
                <asp:Button ID="BtnSendEmail" runat="server" Text="Send Email"  CssClass="btn btn-primary nounload" OnClick="BtnSendEmail_Click" CausesValidation="true" ValidationGroup="send"/>
                &nbsp; <asp:Button ID="BtnCancel" runat="server" Text="Cancel" CssClass="btn btn-primary nounload" OnClick="BtnCancel_Click" CausesValidation="false" OnClientClick="return confirm('Warning! Data will be lost. Are you sure you want to cancel?');"/>
            </div>
        </div>
        
    <asp:SqlDataSource ID="SDSFac" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>" SelectCommand="SELECT FACILITY_ID, FACILITY_NAME FROM LST_FACILITY WHERE IS_ACTIVE='Y'
order by facility_name"></asp:SqlDataSource>
    </asp:Panel>
    <div id="dialog-msg" style="display:none;" class="nounload">
    <p>
        <asp:Label ID="LblMsg" runat="server"></asp:Label>
    </p>
</div>
</asp:Content>
