<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FACApprovalHistory.ascx.cs" Inherits="LST.UserControls.FACApprovalHistory" %>

<asp:Panel ID="PnlFACAppHistory" runat="server" GroupingText="Facility Approval History">
    <asp:GridView ID="GvFACApprovalHis" runat="server" AutoGenerateColumns="false" SkinID="gridviewSkin"
         EmptyDataText="No approval history available" Width="100%">
        <Columns>
            <asp:BoundField DataField="FAC_Approval_Id" HeaderText="Approval Id" />
            <asp:BoundField DataField="APPROVER_TYPE" HeaderText ="Approver Type" />
            <asp:BoundField DataField="APPROVER" HeaderText="Approver" />
            <asp:BoundField DataField="STATUS" HeaderText="Status" />
            <asp:BoundField DataField="ACTION_DATE" HeaderText="Date of Action" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" />
            <asp:BoundField DataField="COMMENTS" HeaderText="Comments" />
        </Columns>

    </asp:GridView>

</asp:Panel>