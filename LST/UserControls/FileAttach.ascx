<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileAttach.ascx.cs" Inherits="LST.UserControls.FileAttach" %>


    <div id="DivFile">
        <table>
           
            <tr>
                <td><asp:FileUpload ID="FUDocument" runat="server" /></td>
                <td>&nbsp;</td>
                <td> <asp:button ID="BtnAttach" runat="server" text="Attach" ClientIDMode="Static"  CssClass="btn btn-primary nounload" OnClick="BtnAttach_Click" CausesValidation="false"/></td>
            </tr>
            
        </table>
             
         
    </div>

    <div id="dialog-msg" style="display:none;">
        <asp:Label ID="LblMsg" runat="server" ></asp:Label>
      
    </div>
