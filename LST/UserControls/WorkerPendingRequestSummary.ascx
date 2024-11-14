<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkerPendingRequestSummary.ascx.cs" Inherits="LST.UserControls.WorkerPendingRequestSummary" %>

<asp:Panel ID="PnlPending" runat="server" GroupingText="Requests for QLO/LCA Worker Approval">
     <asp:GridView ID="GvPending" runat="server" EmptyDataText="No Pending request" AutoGenerateColumns="false" SkinID="gridviewSkin" Width="100%" DataKeyNames="MAP_ID"
          >
           <Columns>
                <asp:HyperLinkField DataNavigateUrlFields="MAP_ID" DataNavigateUrlFormatString="../Approval.aspx?objid={0}"
                 HeaderText ="Action" Text= "View request"  />
               <asp:BoundField DataField="Map_Id" HeaderText="Request Id" visible="false" />
               <asp:BoundField DataField="WorkerType" HeaderText ="Worker Type" ReadOnly="true" />
               <asp:BoundField DataField="Facility_Name" HeaderText="Facility" ReadOnly="true"/>
               <asp:BoundField DataField="CURRENTSTATUS" HeaderText="Status" ReadOnly="true"/>
             
              
              
           </Columns>
       </asp:GridView>
    <div id="dialog-pendmsg" style="display:none;" class="nounload">
    <p>
        <asp:Label ID="LblMsg" runat="server"></asp:Label>
    </p>
</div> 
</asp:Panel>
