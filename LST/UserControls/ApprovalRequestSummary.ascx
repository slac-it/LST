<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovalRequestSummary.ascx.cs" Inherits="LST.UserControls.ApprovalRequestSummary" %>
<asp:Panel ID="PnlApprovalReq" runat="server">
    <br />
   <span id="SpnInfo" runat="Server">As a <%= ApproverType %>, you have following worker requests to approve:</span>
   <span id="SpnDlso" runat="server" visible="false">These are the following worker requests for LSO to approve:</span>
    <asp:GridView ID="GvApprovalReq" runat="server" AutoGenerateColumns="false" SkinID="gridviewSkin"
        EmptyDataText ="None" Width="100%" >
        <Columns>
            <asp:BoundField DataField="MAP_ID" HeaderText ="Request Id"/>
            <asp:BoundField DataField="WORKERNAME"  HeaderText ="Worker Name"/>
            <asp:BoundField DataField="WORKERTYPE"  HeaderText ="Worker Type"/>
            <asp:BoundField DataField="FACILITY_NAME"  HeaderText ="Facility"/>
            <asp:BoundField DataField="CURRENTSTATUS" HeaderText="Current Status" />
            <asp:HyperLinkField DataNavigateUrlFields="MAP_ID" DataNavigateUrlFormatString="../Approval.aspx?objid={0}"
                 HeaderText ="Action" Text= "View request" />
        </Columns>
    </asp:GridView>

</asp:Panel>