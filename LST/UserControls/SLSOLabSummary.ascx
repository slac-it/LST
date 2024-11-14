<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SLSOLabSummary.ascx.cs"  EnableTheming="true"  Inherits="LST.UserControls.SLSOLabSummary" %>

    <asp:GridView ID="GvSLSO" runat="server" AutoGenerateColumns="false" SkinID="gridviewSkin"
         EmptyDataText="None" Width="80%"  ShowHeader="true" OnRowCommand="GvSLSO_RowCommand" OnRowDataBound="GvSLSO_RowDataBound" >
    <Columns>
        <asp:BoundField DataField="FACILITY_ID"  Visible="false" />
        <asp:BoundField dataField="FACILITY_NAME" HeaderText="Facility"  />
        <asp:BoundField DataField="USERTYPE"  HeaderText="SLSO Type" />
        <asp:TemplateField HeaderText="Alternate SLSO">
            <ItemTemplate>
                <asp:Label ID="LblAltSLSO" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "altslso")%>'></asp:Label>
                <asp:LinkButton ID="LnkDesignate" runat="server" Text="Designate an alternate" ></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
         <asp:TemplateField >
            <ItemTemplate>
                <asp:LinkButton ID="LnkView"  Text="View Facility" CommandName="View" commandargument='<%# DataBinder.Eval(Container.DataItem, "FACILITY_ID")%>' runat="Server"/>
            </ItemTemplate>
         </asp:TemplateField> 
        
    </Columns>

    </asp:GridView>
