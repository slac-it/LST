<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovalHistory.ascx.cs" Inherits="LST.UserControls.ApprovalHistory" %>

<asp:Panel ID="PnlAppHistory" runat="server" GroupingText="Approval History">
    <asp:GridView ID="GvApprovalHis" runat="server" AutoGenerateColumns="false" SkinID="gridviewSkin"
         EmptyDataText="No approval history available" Width="100%">
        <Columns>
            <asp:BoundField DataField="APPROVAL_ID" HeaderText="Approval Id" />
            <asp:BoundField DataField="APPROVER_TYPE" HeaderText ="Approver Type" />
            <asp:BoundField DataField="APPROVER" HeaderText="Approver" />
            <asp:BoundField DataField="STATUS" HeaderText="Status" />
            <asp:BoundField DataField="APPROVED_ON" HeaderText="Date of Action" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" />
            <asp:BoundField DataField="COMMENTS" HeaderText="Comments" />
        </Columns>

    </asp:GridView>

</asp:Panel>