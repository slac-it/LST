<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddLink.ascx.cs" Inherits="LST.UserControls.AddLink" %>
<script type="text/javascript">
   
    $(document).keypress(function (e) {
        if (e.which == 13) {
            $("CmdOk").click();
        }
    });
</script>
<div id="divAdd" visible="true" runat="server">
    <asp:Button ID="CmdAddLink" runat="server" Text="Add Link"  CssClass="btn btn-primary nounload"/> &nbsp;&nbsp;&nbsp;
    <a href="" runat="server" id="aLink" target="_blank"></a>&nbsp;&nbsp;
    <asp:ImageButton ID="ImgBtnDelete" runat="server" ToolTip="Delete this Link" ImageUrl="~/Images/deleteicon.gif"  OnClick="ImgBtnDelete_Click" CssClass="nounload" visible="false" OnClientClick="return confirm('Are you sure you want delete this link?');" />
    
</div>
<div id="dialog-link" title="Add link" style="display:none;" >
    <table>
        
        <tr>
            <td colspan="3">
                <asp:ValidationSummary ID="VSErrors" DisplayMode="BulletList"  CssClass="errlabels" EnableClientScript ="true" HeaderText="The following errors occurred:" runat="server" />
            </td>
        </tr>
         <tr><td>&nbsp;</td></tr>
        <tr>
            <td><asp:Label ID="LblText" runat="server" Text="Text to display:" CssClass="control-label" ></asp:Label></td>
            <td>&nbsp;</td>
            <td><asp:TextBox ID="TxtUrltext" runat="server" MaxLength ="50" CssClass="txtWidth"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RfvUrlText" runat="server" SetFocusOnError ="true" ControlToValidate="TxtUrltext" ErrorMessage="Text is required" CssClass ="errlabels" Text="*" ValidationGroup="ok"></asp:RequiredFieldValidator>
            </td>
         </tr>
         <tr><td>&nbsp;</td></tr>
         <tr>
            <td><asp:Label ID="LblUrl" runat="server" Text="Address:" CssClass="control-label"></asp:Label></td>
            <td>&nbsp;</td>
            <td><asp:TextBox ID="TxtUrl" runat="server" MaxLength ="300" CssClass="txtWidth"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RfvURL" runat="server" SetFocusOnError ="true" ControlToValidate="TxtUrl" ErrorMessage ="URL is required" CssClass="errlabels" Text="*" ValidationGroup="ok"></asp:RequiredFieldValidator>
      <asp:RegularExpressionValidator ID="RegExUrl" runat="Server"  ControlToValidate="TxtUrl"  ValidationExpression="(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=-_]*)?" ErrorMessage="Please enter a valid URL"  Display="Dynamic" CssClass="errlabels"  Text="*" ValidationGroup="ok"/>
            </td>
        </tr>
         <tr><td>&nbsp;</td></tr>
         <tr>
            
            <td colspan="3" style="text-align:center;"><asp:Button ID="CmdOk" runat="server" ClientIDMode="Static" Text="Ok" onclick="CmdOk_Click" CssClass="btn btn-primary" ValidationGroup="ok"/>
                    &nbsp;&nbsp;
                    <asp:Button ID="CmdCancel" runat="server" Text ="Cancel" CausesValidation="False" CssClass="btn btn-primary"/></td>
        </tr>
        <tr><td>&nbsp;</td></tr>
        <tr>
            <td colspan="3" style="align-self:center;"">
                 <table style="border:1px solid black;">
                    <tr><td>
                        <div class="errlabels">
                            Important tips for URL:<br />
                            -------------------------------
                            <ul>
                                <li>Allows only http or https </li>
                                <li>Allows characters like - % _ ? = & . </li>
                                <li>Does not allow space infront of http/https </li>
                                <li>Does not allow special characters like [ ] ( ) ~ * '</li>
                            </ul>
                            </div>
                            </td></tr>
                    </table>
            </td>
        </tr>

       
    </table>
     
</div>
