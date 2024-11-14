<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Designate.aspx.cs" Inherits="LST.Designate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="Content/jquery-ui.min.css" rel="stylesheet" type="text/css" />
     <link href="Content/LST.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
       <script type="text/javascript" src="Scripts/LST.js"></script>
    <script type="text/javascript">

        function ResetTextbox() {
            return true;
        }

        function DropdownValidateFacility(source, arguments) {
            var ctrl = document.forms[0].elements["DdlFacility"].value;
            if (ctrl == "-1") {
                arguments.IsValid = false;
                return false;
            }
            else { return true; }
        }
        /*adding function(duplicate) here due to chrome */
        function OnsuccessRefreshParent(divid, divid2, mode, type) {

            $("#" + divid).dialog({
                modal: true,
                buttons: {
                    Ok: function () {

                        $(this).dialog("close");
                        window.parent.$('#' + divid2).dialog('close');
                                window.parent.RefreshGrid(mode, type);
   

                    }
                }
            });
        }


   

      
    </script>
  </head>
<body>
    <form id="form1" runat="server" class="form-horizontal" style="padding:10px;">
            <div class="form-group">
                <div class="col-xs-4 rightalign">
                    <span class="spanrequired">*</span>indicates required field
                </div>
                <div class="col-xs-8 rightalign">
                    <asp:Button CssClass="btn btn-primary" Text="Designate" runat="server" id="BtnDesignate" ValidationGroup="add" OnClick="BtnDesignate_Click"/>&nbsp;
                    <asp:Button Id="BtnCancel" CssClass="btn btn-primary" Text="Cancel" runat="server" CausesValidation="false"/>
                </div>
            </div>
            <div class="form-group">
                <div class="col-xs-3">&nbsp;</div>
                <div class="col-xs-5" style="text-align:left;">
                    <div style="display:block;"><asp:ValidationSummary ID="VSDesignate" runat="server"  BorderStyle="Solid" BorderWidth="1px" DisplayMode="BulletList" ValidationGroup="add"   CssClass="errlabels" ShowSummary="true"/></div>
                </div>
            </div>  
            <div class="form-group">
                 <div class="col-xs-1">&nbsp;</div>
                <div class="col-xs-10">
                     <asp:Literal ID="LtrlInfo" runat="server" >
                    <i>Note: The alternate delegates will be active only till the To date. After that they will become automatically inactive.
                        <b>Please use the binocular icon to find the correct name with Id.</b>
                    </i>
                 </asp:Literal>
                </div>  
                    <div class="col-xs-1">&nbsp;</div>         
            </div>
   
          <div class="form-group">
             <asp:Label ID="LblAlternate" runat="server" CssClass="col-xs-4 control-label rightalign labelfieldleft"><span  class="spanrequired" runat="server">*</span>Alternate Name:</asp:Label>
              <div class="col-xs-8 leftalign">
                  <asp:TextBox ID="TxtAlternate" runat="server" ClientIDMode="Static" MaxLength="30" CssClass="txtnamewidth" AutoPostBack="true"></asp:TextBox>
                        &nbsp;<asp:ImageButton ID="ImgBtAlt" runat="server" ImageUrl="~/Images/find.gif" />
                      &nbsp;<asp:Label ID="Lblformat" runat="server" CssClass="formattext" Text="(Lastname, firstname)"></asp:Label>
                       <asp:RequiredFieldValidator ID="RFVAlt" ErrorMessage="Alternate Name is required"  ControlToValidate="TxtAlternate" CssClass="errlabels"  ValidationGroup="add" runat="server" SetFocusOnError="true" Display="Dynamic" Text="*"></asp:RequiredFieldValidator>
             &nbsp;<asp:CustomValidator ID="CvAlt" ErrorMessage="Not a valid Alternate name / format"  ControlToValidate="TxtAlternate" CssClass="errlabels" ValidationGroup="add" runat="server" SetFocusOnError="true" OnServerValidate="CvAlt_ServerValidate" Display="Dynamic" Text="*"></asp:CustomValidator>
              </div>
            
        </div>
     
        <div></div>     
        <div class="form-group" runat="server" id="divLab" visible="false"> 
            <asp:Label ID="LblFacility" runat="server" CssClass="col-xs-4 control-label rightalign labelfieldleft"><span  class="spanrequired" runat="server">*</span>Facility Name:</asp:Label>
              <div class="col-xs-8 leftalign">
                    <asp:DropDownList ID="DdlFacility" runat="server" OnDataBound="DdlFacility_DataBound" >
                    </asp:DropDownList>
                     <asp:CustomValidator ID="CvFacility" runat="server" EnableClientScript="true" Display="Dynamic" ClientValidationFunction="DropdownValidateFacility" ControlToValidate="DdlFacility"  ErrorMessage="Facility Name is required" SetFocusOnError="true" CssClass="errlabels"  ValidationGroup="add"  Text="*" ></asp:CustomValidator>
              </div>
        </div>
        <div></div>
      
        <div class="form-group">
            <asp:Label ID="LblFromDate" runat="Server" CssClass="col-xs-4 control-label rightalign labelfieldleft"><span  class="spanrequired" runat="server">*</span>Date From:</asp:Label>
            <div class="col-xs-8 leftalign">
                <asp:TextBox ID="TxtFromDate" runat="server"></asp:TextBox>
                 <span id="spnFromfrmt" class="formattext">(mm/dd/yyyy)</span>
                <asp:RequiredFieldValidator ID="RvFromdate" runat="server" ControlToValidate="TxtFromDate"  ErrorMessage="From Date is required" ValidationGroup="add" CssClass="errlabels" Text ="*"></asp:RequiredFieldValidator>
                 <asp:CompareValidator ID="CvFrom" runat="server" Type="Date" Operator="DataTypeCheck"
                                                                ControlToValidate="TxtFromDate" ErrorMessage="Date From not a valid date!"  Text="*" CssClass="errlabels" ValidationGroup="add"></asp:CompareValidator>
            </div>
        </div>
         <div></div>
        <div class="form-group">
            <asp:Label ID="LblToDate" runat="Server" CssClass="col-xs-4 control-label rightalign labelfieldleft"><span  class="spanrequired" runat="server">*</span>Date To:</asp:Label>
            <div class="col-xs-8 leftalign">
                <asp:TextBox ID="TxtToDate" runat="server"></asp:TextBox>
                <span id="spnTofrmt" class="formattext">(mm/dd/yyyy)</span>
                  <asp:RequiredFieldValidator ID="RvTodate" runat="server" ControlToValidate="TxtToDate"  ErrorMessage="To Date is required" ValidationGroup="add" CssClass="errlabels" Text ="*"></asp:RequiredFieldValidator>
                 <asp:CompareValidator ID="CvTo" runat="server" Type="Date" Operator="DataTypeCheck"
                                                                ControlToValidate="TxtToDate" ErrorMessage="Date To not a valid date!"  Text="*" CssClass="errlabels" ValidationGroup="add"></asp:CompareValidator>
                <asp:CustomValidator ID="CustValDate" runat="server"  OnServerValidate="CustValDate_ServerValidate"  ControlToValidate="TxtToDate" SetFocusOnError="true" ValidationGroup="add" Text="*"  CssClass="errlabels"></asp:CustomValidator>
            </div>        
        </div>
       
          <div id="dialog-msg"  style="display:none;" class="result-dialog">
            <asp:Label ID="LblAltMsg" runat="server"></asp:Label>
        </div>  
         <div id="dialogowneradmin" style="display:none;" >             
                <iframe id="modaldialogowneradmin" width="100%" height="100%">
                </iframe>
          </div>                
                         
     <asp:SqlDataSource ID="SDSFacility" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>" SelectCommand="SELECT FACILITY_ID, FACILITY_NAME FROM LST_FACILITY WHERE IS_ACTIVE='Y' ORDER BY FACILITY_NAME ASC"></asp:SqlDataSource>                     
      <asp:HiddenField ID="HdnType" runat="server" />      
       <asp:HiddenField ID="HdnAltId" runat="server" ClientIDMode="Static" />
    </form>
    
          </body>
</html>
