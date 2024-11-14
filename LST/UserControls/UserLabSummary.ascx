<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserLabSummary.ascx.cs" Inherits="LST.UserControls.UserLabSummary" %>
<script type="text/javascript">
    $(document).ready(function () {
      // Sys.WebForms.PageRequestManager.getInstance().add_endRequest(bindPicker);
        bindPicker();
    });
    function bindPicker() {
        $("input[type=text][id*=TxtSOP]").datepicker({ showOn: "button", buttonImage: "Images/cal.gif", buttonImageOnly: true, buttonText: "Select date", changeMonth: true, changeYear: true });
    }
    </script>
<asp:Panel ID="PnlUserLab" runat="server" GroupingText="Facilities Associated">
    <asp:GridView ID="GvUserLabs" runat="server" AutoGenerateColumns="false"  
        EmptyDataText="None"  SkinID="gridviewSkin"  Width="100%" 
         DataKeyNames="MAP_ID" OnRowCancelingEdit="GvUserLabs_RowCancelingEdit" OnRowEditing="GvUserLabs_RowEditing"  OnRowUpdated="GvUserLabs_RowUpdated" OnRowUpdating="GvUserLabs_RowUpdating" OnRowDataBound ="GvUserLabs_RowDataBound" >
        <columns>
            <asp:BoundField DataField="MAP_ID" Visible="false" />
            <asp:BoundField DataField="WORKTYPE" ReadOnly="true" HeaderText="Worker Type" />
             <asp:HyperLinkField DataNavigateUrlFields="FACILITY_ID" DataNavigateUrlFormatString="../Facility.aspx?id={0}&mode=view"
                  DataTextField ="FACILITY_NAME"  HeaderText="Facility" />          
            <asp:BoundField DataField ="STATUS" ReadOnly="true" HeaderText="Status" />
            <asp:BoundField DataField="OJT_COMPLETION_DATE" HeaderText="OJT Completion Date" HtmlEncode="false" DataFormatString="{0:MM/dd/yy}"  ReadOnly="true"/>
              <asp:TemplateField HeaderText="SOP Review Date">
                <EditItemTemplate>
                      <asp:TextBox ID="TxtSOP"   runat="server" Text='<%#Eval("SOP_REVIEW_DATE", "{0:MM/dd/yy}") %>' ClientIDMode="Static"></asp:TextBox>                    
                       <span id="spnSOPfrmt" class="formattext">(mm/dd/yyyy)</span>
                        <asp:RequiredFieldValidator ID="RfvSOP" runat="Server" ControlToValidate="TxtSOP" ErrorMessage ="SOP review Date is required" ValidationGroup="edit"  CssClass="errlabels"></asp:RequiredFieldValidator>
                         <asp:CompareValidator ID="CvSOP" runat="server" Type="Date" Operator="DataTypeCheck"
                                                                ControlToValidate="TxtSOP" ErrorMessage="Not a valid date!"  CssClass="errlabels" ValidationGroup="edit"></asp:CompareValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="LblSOP" runat="server" Text='<%#Eval("SOP_REVIEW_DATE", "{0:MM/dd/yy}") %>' ></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
                <asp:TemplateField>
                <EditItemTemplate>
                    <asp:LinkButton ID="cmdEditupdate" runat="server" CausesValidation="True" CommandName="Update"   Text="Update" ValidationGroup="edit" CssClass="nounload" />&nbsp;
                    <asp:LinkButton ID="Button2" runat="server" CausesValidation="False"
                            CommandName="Cancel" Text="Cancel" CssClass="nounload" />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="cmdEdit" runat="server" CausesValidation="False" CommandName="Edit" CssClass="nounload"
                        Text="Edit SOP Review Date"  />
                </ItemTemplate>
            </asp:TemplateField>
           <asp:BoundField DataField="FACILITY_ID" Visible="false" />
        </columns>
    </asp:GridView>
    <div id="dialog-labmsg" style="display:none;" class="nounload">
    <p>
        <asp:Label ID="LblMsg" runat="server"></asp:Label>
    </p>
</div> 
</asp:Panel>