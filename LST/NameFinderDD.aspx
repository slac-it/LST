﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NameFinderDD.aspx.cs" Inherits="LST.NameFinderDD" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Find Employee</title>
   <link href="Content/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="Content/jquery-ui.min.css" rel="stylesheet" type="text/css" />
    <link href="Content/LST.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/LST.js"></script>
    
    <script type="text/javascript" language="javascript">

        
            $("#TxtOwner").keypress(function (event) {
                if (event.keyCode == 13) {
                    $("#cmdContinue").click();
                }
            });

           
     
           
            function onKeypress1() {
                if ((event.which && event.which == 13) || (event.keyCode && event.keyCode == 13)) {
                    var btn = document.getElementById('CmdSelect');
                    if (btn != null) { //If we find the button click it
                        btn.click();
                        event.keyCode = 0
                    }


                    return false;
                }
                else
                    return true;
            }


        
       

        function JQueryClose(selectedvalue, selectedid) {
            var dialog = $('#HdnDialog').val();
            var control = $('#HdnControl').val();
            var control2 = $('#HdnControl2').val();

            if (selectedvalue == ' ') {
                window.parent.$('#' + control).val(' ');
            }
            else if (selectedvalue == 'na') {

            }
            else {
                selectedvalue = htmlDecode(selectedvalue);
                selectedid = htmlDecode(selectedid);
                alert(control2);
                window.parent.$('#' + control).val(selectedvalue);
                window.parent.$('#' + control2).val(selectedid);
                window.parent.$('#' + control2).html(selectedid);

            }
            
            window.parent.$("#" + dialog).dialog('close');
            if ((selectedvalue != 'na') && (control2 != 'undefined'))
            { parent.ResetTextbox(); }
              
           return false;
        }

        function htmlDecode(value) {
            return $('<div/>').html(value).text();
        }




     </script>
</head>
<body style="background-color:White;">
    <form id="form1" runat="server" defaultbutton="CmdContinue">
    <div>
        <asp:Panel ID="PnlName"  runat="server" >
            <table id="GridTable" cellpadding="5" cellspacing="5">
                <tr>
                    <td>&nbsp;<asp:Label ID="LblMsg" runat="server" Text="Please enter the first few characters of the employee last name. If you don't know the employee last name, you may enter the  employee id."></asp:Label></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                        <asp:TextBox ID="TxtOwner" runat="server" ClientIDMode="Static"></asp:TextBox>
                        <asp:Button ID="CmdContinue" runat="server"  Text="Continue"  ClientIDMode="Static"
                            onclick="CmdContinue_Click" CssClass="nounload"/>
                        <asp:Button ID="CmdCancel" runat="server" Text="Cancel" 
                            onclick="CmdCancel_Click" CssClass="nounload" />                    
                    </td>
                </tr>
                  <tr>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td> &nbsp;<asp:Label ID="LblError" runat="server" Visible="false"></asp:Label></td>
                </tr>
                <tr id="trMsg2" runat="server" visible="false">
                    <td>&nbsp;<asp:Label ID="LblMsg2" runat="server" Text="Select a Name from the list below:"></asp:Label></td>
                </tr>
              
                <tr id="trGrid" runat="server" visible="false">
                    <td>&nbsp;
                             <asp:dropdownlist id="ddlEmplist" runat="server"  ></asp:dropdownlist>
                    </td>
                </tr>
                <tr>
                <td>&nbsp;</td>
            </tr>
            <tr id="trButtons"  Visible="false" runat="server">
                <td>&nbsp; <asp:Button ID="CmdSelect" runat="server" Text="Ok" 
                        onclick="CmdSelect_Click"  CssClass="nounload" />
                    <asp:Button ID="CmdBack" runat="server" Text="Cancel & Exit" 
                        onclick="CmdBack_Click" CssClass="nounload" />
                </td>
            </tr>
            </table>
        <asp:HiddenField ID="HdnDialog" runat="server" />
        <asp:HiddenField ID="HdnControl" runat="server" />
        <asp:HiddenField ID="HdnItemval" runat="server" />
         <asp:HiddenField ID="HdnControl2" runat="server" />
        </asp:Panel>
    
    </div>
    </form>
</body>
</html>