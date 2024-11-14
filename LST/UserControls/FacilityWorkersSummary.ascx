<%@ Control Language="C#" AutoEventWireup="true"   CodeBehind="FacilityWorkersSummary.ascx.cs" Inherits="LST.UserControls.FacilityWorkersSummary" %>
<%@ Register Src="~/UserControls/FileAttach.ascx" TagName ="UCFile" TagPrefix="uc2"  %>
<script type="text/javascript">
    $(document).ready(function () {
      // Sys.WebForms.PageRequestManager.getInstance().add_endRequest(bindPicker);
        bindPicker();
    });
    function bindPicker() {
        $("input[type=text][id*=TxtOJT]").datepicker({ showOn: "button", buttonImage: "Images/cal.gif", buttonImageOnly: true, buttonText: "Select date", changeMonth: true, changeYear: true });
    }

   /* function RefreshGrid() {
        alert("true");
        window.location.reload();
    }*/

    function Refresh()
    {
        window.location.href = window.location.href;
    }
    </script>


        <asp:Panel ID="PnlWorkers" runat="server" GroupingText="Workers Associated with this Facility - Summary Info">
             <div  runat="server" id="divWorkfac">
        <asp:ImageButton ID="btnShow1" ImageUrl="~/Images/expandbig.gif" runat="server" ClientIDMode="Static"   OnClientClick="toggleme('DivWorkers','btnShow1','btnHide1'); return false;" />
        <asp:ImageButton ID="btnHide1" ImageUrl="~/Images/collapsebig.gif" runat="server"  ClientIDMode="Static"  style="display: none" OnClientClick="toggleme('DivWorkers','btnShow1','btnHide1'); return false;" />    
        <i><strong>Note:</strong> Click on the +/- to expand or collapse the list.</i>  
    </div>   

           
            <div id="DivWorkers" style="display:none;">
               
        <asp:GridView ID="GvWorkers" runat="server"  AutoGenerateColumns="false" 
        Width="100%" SkinID="gridviewSkin" DataKeyNames="MAP_ID" EmptyDataText="None" OnRowCancelingEdit="GvWorkers_RowCancelingEdit" 
            OnRowEditing="GvWorkers_RowEditing" OnRowUpdated="GvWorkers_RowUpdated" OnRowUpdating="GvWorkers_RowUpdating" OnRowDataBound="GvWorkers_RowDataBound" OnRowCommand="GvWorkers_RowCommand" OnRowDeleting="GvWorkers_RowDeleting" >
        <Columns>
            <asp:BoundField DataField="MAP_ID" Visible="false" ReadOnly="true" />
             <asp:HyperLinkField DataNavigateUrlFields="WORKER_ID" DataNavigateUrlFormatString="../WorkerEntry.aspx?id={0}&mode=view"
                  DataTextField ="Worker"  HeaderText="Worker"  ItemStyle-Width="14%" ItemStyle-VerticalAlign="Middle" />
           
            <asp:BoundField DataField= "WORKTYPE" HeaderText="Work Type"  ReadOnly="true" ItemStyle-Width="10%" ItemStyle-VerticalAlign="Middle"  HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
             <asp:TemplateField HeaderText ="Badge Id" ItemStyle-Width="5%"  HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label ID="LblBadge" runat="server" Text='<%#GetBadgeId(Eval("SLAC_ID").ToString()) %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="STATUS"  HeaderText="Status" ReadOnly="true" ItemStyle-Width="10%" ItemStyle-VerticalAlign="Middle" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"/>
            
            <asp:TemplateField HeaderText="OJT Completion Date" ItemStyle-Width="16%" ItemStyle-VerticalAlign="Middle" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <EditItemTemplate>
                      <asp:TextBox ID="TxtOJT"   runat="server" Text='<%#Eval("OJT_COMPLETION_DATE", "{0:MM/dd/yy}") %>' ClientIDMode="Static" Width="50%"></asp:TextBox>                    
                     <span id="spnOJTfrmt" class="formattext">(mm/dd/yyyy)</span> <br />
                       <asp:RequiredFieldValidator ID="RfvOJT" runat="Server" ControlToValidate="TxtOJT" ErrorMessage ="OJT Date is required" ValidationGroup="edit"  CssClass="errlabels" Display="Dynamic"></asp:RequiredFieldValidator>
                         <asp:CompareValidator ID="CvOJT" runat="server" Type="Date" Operator="DataTypeCheck"
                                                                ControlToValidate="TxtOJT" ErrorMessage="Not a valid date!"  CssClass="errlabels" ValidationGroup="edit" Display="Dynamic"></asp:CompareValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="LblOJT" runat="server" Text='<%#Eval("OJT_COMPLETION_DATE", "{0:MM/dd/yy}") %>' ></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
             <asp:TemplateField HeaderText="SOP Review Date" ItemStyle-Width="16%" ItemStyle-VerticalAlign="Middle" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <EditItemTemplate>
                      <asp:TextBox ID="TxtSOP"   runat="server" Text='<%#Eval("SOP_REVIEW_DATE", "{0:MM/dd/yy}") %>' ClientIDMode="Static" Width="50%"></asp:TextBox>                    
                    <span id="spnsopfrmt" class="formattext">(mm/dd/yyyy)</span><br />
                        <asp:RequiredFieldValidator ID="RfvSOP" runat="Server" ControlToValidate="TxtSOP" ErrorMessage ="SOP Review Date is required" ValidationGroup="edit"  CssClass="errlabels" Display="Dynamic"></asp:RequiredFieldValidator>
                         <asp:CompareValidator ID="CvSOP" runat="server" Type="Date" Operator="DataTypeCheck"
                                                                ControlToValidate="TxtSOP" ErrorMessage="Not a valid date!"  CssClass="errlabels" ValidationGroup="edit" Display="Dynamic"></asp:CompareValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="LblSOP" runat="server" Text='<%#Eval("SOP_REVIEW_DATE", "{0:MM/dd/yy}") %>' ></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText ="Action" ItemStyle-Width="12%" ItemStyle-VerticalAlign="Middle" ItemStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <EditItemTemplate>
                    <asp:LinkButton ID="cmdEditupdate" runat="server" CausesValidation="True" CommandName="Update"   Text="Update" ValidationGroup="edit" CssClass="nounload" />&nbsp;
                    <asp:LinkButton ID="Button2" runat="server" CausesValidation="False"
                            CommandName="Cancel" Text="Cancel" CssClass="nounload" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="cmdEdit" runat="server" CausesValidation="False" CommandName="Edit" CssClass="nounload"
                        Text="Edit Date" /> &nbsp; | &nbsp;
                    <asp:LinkButton ID="cmdDelete" runat="server" CausesValidation="false" CommandName="unassociate" CssClass="nounload" 
                        text ="Delete the association" CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' OnClientClick = "return confirm('Warning! Are you sure you want to delete the user from this facility?');"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
           
            <asp:BoundField DataField="STATUS_ID" Visible="false" />
        </Columns>
           <HeaderStyle CssClass="centerHeaderText" />
    </asp:GridView>
                </div>
   <div id="dialog-msg2" style="display:none;" class="nounload">
    <p>
        <asp:Label ID="LblMsg2" runat="server"></asp:Label>
    </p>
</div> 
  
</asp:Panel>
  
<asp:ObjectDataSource ID="OdsWorkFac" runat="server" SelectMethod="GetWorkersForFacility" TypeName="LST.Data.DML_Util" UpdateMethod="UpdateOJT">
    <SelectParameters>
        <asp:Parameter Name="facId" Type="Int32" />
    </SelectParameters>
    <UpdateParameters>
        <asp:Parameter Name="MapId" Type="Int32" />
        <asp:Parameter Name="OJTCompletionDate" Type="DateTime" />
    </UpdateParameters>
</asp:ObjectDataSource>
