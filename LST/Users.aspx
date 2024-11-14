<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="LST.NewSLSO" %>

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
            $('#TxtUser').trigger('change');
        }

       
        

       /*function success() {
           $("#dialog-msg").dialog({
                modal: true,
                buttons: {
                    Ok: function () {
                       $(this).dialog("close");
                       window.parent.$('#dialogUser').dialog('close');
                    }
                }
            });
       }*/

       function DropdownValidation(source, arguments) {

           var ctrl = document.forms[0].elements["DdlRole"].value;

           if (ctrl == "-1") {
               arguments.IsValid = false;
               return false;
           }
           else {
               return true;
           }
       }

   
  
    </script>
    
</head>
<body>
    <form  class="form-horizontal" runat="server" style="padding:10px;">
        <div class="form-group">
            <div class="col-xs-5">
                <span class="spanrequired">*</span> indicates required field
            </div>
            <div class="col-xs-7 rightalign">
                 <asp:Button cssClass="btn btn-primary" Text="Submit" runat="server" id="BtnSubmit" OnClick="BtnSubmit_Click"  ValidationGroup="add"/>&nbsp;
                  <asp:Button CssClass="btn btn-primary" Text="Cancel" runat="server" ID="BtnCancel" />
            </div>
         
        </div>
        <div class="form-group">
            <div class="col-xs-3">&nbsp;</div>
            <div class="col-xs-5" style="text-align:left;">
                <div style="display:block;"><asp:ValidationSummary ID="VSUser" runat="server"  BorderStyle="Solid" BorderWidth="1px" DisplayMode="BulletList" ValidationGroup="add"   CssClass="errlabels" ShowSummary="true"/></div>
            </div>
            
        </div>

        <div class="form-group">
            <asp:label id="LblName" runat="server" CssClass="col-xs-3 control-label rightalign"> <span  class="spanrequired" runat="server">*</span> Name: </asp:label>
              <div class="col-xs-9 leftalign">
                   <asp:TextBox ID="TxtUser" runat="server" ClientIDMode="Static" MaxLength="30" CssClass="txtnamewidth" AutoPostBack="true" OnTextChanged="TxtWorker_TextChanged"></asp:TextBox>
                        &nbsp;<asp:ImageButton ID="ImgBtnUser" runat="server" ImageUrl="~/Images/find.gif" />
                      &nbsp;<asp:Label ID="Lblformat" runat="server" CssClass="formattext" Text="(Lastname, firstname)"></asp:Label>
                       <asp:RequiredFieldValidator ID="RFVUser" ErrorMessage="Name is required"  ControlToValidate="TxtUser" CssClass="errlabels"  ValidationGroup="add" runat="server" SetFocusOnError="true" Display="Dynamic" Text="*"></asp:RequiredFieldValidator>
             &nbsp;<asp:CustomValidator ID="CvUser" ErrorMessage="Not a valid name / format"  ControlToValidate="TxtUser" CssClass="errlabels" ValidationGroup="add" runat="server" SetFocusOnError="true" OnServerValidate="CvUser_ServerValidate" Display="Dynamic" Text="*"></asp:CustomValidator>
              </div>           
        </div>
       
        
        
        <div>&nbsp;</div>
        
          <div>
         <asp:Panel ID="PnlTraining" runat="server" GroupingText="Training Summary" Visible="False" >
            <table class="table table-bordered table-condensed">
                <tr class="trhead">
                     <th>Course Name</th>
                     <th>Assigned in STA</th>
                     <th>Status</th>
                </tr>
                <tr>
                    <td>130</td>
                    <td><%=IsInSTA("130") %></td>
                    <td><%=GetStatus("130") %></td>
                </tr>
               
                <tr>
                     <td>157</td>
                     <td><%=IsInSTA("157") %></td>
                     <td><%=GetStatus("157") %></td>
                </tr>
                 <tr>
                     <td>157R</td>
                     <td><%=IsInSTA("157R") %></td>
                     <td><%=GetStatus("157R") %></td>
                </tr>
                <tr class="tralternate">
                      <td>108</td>
                      <td><%=IsInSTA("108") %></td>
                      <td><%=GetStatus("108") %></td>
                </tr>
                <tr>
                    <td>108PRA</td>
                    <td><%=IsInSTA("108PRA") %></td>
                    <td><%=GetStatus("108PRA") %></td>
                </tr>
                        
            </table>
    </asp:Panel>
           </div>
        <div></div>
    
        

        <div id="dialog-msg"  style="display:none;" class="result-dialog">
           <asp:Label ID="LblUserMsg" runat="server"></asp:Label>
        </div>
         <div id="dialogowneradmin" style="display:none;" >             
                <iframe id="modaldialogowneradmin" width="100%" height="100%">
                </iframe>
          </div>
        <asp:HiddenField id="HdnUserId" runat="server" ClientIDMode="Static" />
        <asp:SqlDataSource ID="SDSFacilities" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>" SelectCommand="SELECT FACILITY_ID, FACILITY_NAME FROM LST_FACILITY WHERE IS_ACTIVE='Y' ORDER BY FACILITY_NAME ASC"></asp:SqlDataSource>
</form>
    
</body>
</html>
