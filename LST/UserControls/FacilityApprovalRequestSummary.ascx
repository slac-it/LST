<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FacilityApprovalRequestSummary.ascx.cs" Inherits="LST.UserControls.FacilityApprovalRequestSummary" %>
<asp:Panel ID="PnlFACApprovalReq" runat="server">
    <br />
   <span id="SpnInfo" runat="Server">As a <%= ApproverType %>, you have following facility requests to approve:</span>
<span id="SpnInfoSLSO" runat="server">Your current Facility Operation Approval requests are:</span>
   <span id="SpnDlso" runat="server" visible="false">These are the following factility requests for LSO to approve:</span>
    <asp:GridView ID="GvApprovalReq" runat="server" AutoGenerateColumns="false" SkinID="gridviewSkin"
        EmptyDataText ="None" Width="100%" >
        <Columns>
            <asp:BoundField DataField="FAC_REQUEST_ID" HeaderText ="Request Id"/>
            <asp:BoundField DataField="FACILITY_NAME"  HeaderText ="Facility"/>
            <asp:BoundField DataField="CURRENTSTATUS" HeaderText="Current Status" />
            <asp:HyperLinkField DataNavigateUrlFields="FAC_REQUEST_ID" DataNavigateUrlFormatString="../FacilityApprovalRequest.aspx?objid={0}"
                 HeaderText ="Action" Text= "View request" />
        </Columns>
    </asp:GridView>

</asp:Panel>