<%@ Page Title="" Language="C#" MasterPageFile="~/LST.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="LST.Error" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <asp:Label ID="LblHeader" runat="server" Text="Error!" Font-Size="Large"></asp:Label>
        <br /><br />
    </div>
    <div>
        <asp:Label ID="LblMSg" runat="server" ></asp:Label>
    </div>
    <div id="DivAddl" runat="server">
          <p>
                <br /><br />
                Please continue by either clicking the browser back button or "Home" menu on the top navigation bar or use the following link to navigate to the Home page: 
                <a href="Default.aspx"><%=_url %></a>


                <br /><br />
                If application is not accessible, please 
                 <a href="https://slacspace.slac.stanford.edu/Operations/SCCS/AppDev/request">Contact AppDev team</a> for help.  Thank you!
                </p>
    </div>
</asp:Content>
