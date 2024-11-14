<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OJTFields.aspx.cs" Inherits="LST.OJTFields" StylesheetTheme="LSTTheme" %>
 <script type="text/javascript" src="Scripts/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script> 
     <script type="text/javascript" src="Scripts/LST.js"></script>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="~/Content/Site.css" rel="stylesheet" /> 
    <link href="Content/jquery-ui.min.css" rel="stylesheet" type="text/css" />
    <link href="Content/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="Content/LST.css" rel="stylesheet" type="text/css" />
   
</head>
<body>
    <form id="form1" runat="server" style="padding-left:40px;padding-right:40px;padding-top:1px;">
    <div>
         
            <div><asp:ValidationSummary ID="VsSummary" runat="server" ValidationGroup="add" HeaderText="Please correct the following error and then click Add" CssClass ="errlabels" /> </div>
            <hr />
             <asp:Button ID="BtnClose" runat="server" Text="Close this dialog"  /> &nbsp;
             <span style="font-size:small;font-style:italic;font-weight:bold;">Note: When adding an OJT Field, check first for an existing one that meets your requirement; if one doesn't exist, then create a new one.</span>
            <p></p>
            <asp:GridView id="GVFields" runat="server"  AutoGenerateColumns="false"
                 PageSize="15" OnPageIndexChanging="GVFields_PageIndexChanging" AllowPaging="true" AllowSorting="true" CellPadding="5" CellSpacing="5"
                 OnSorting="GVFields_Sorting"  DataKeyNames="FIELD_ID"  ShowFooter="true" SkinID="gridviewSkin"   OnRowCommand="GVFields_RowCommand" GridLines="Both" >
                <Columns>
                     <asp:TemplateField HeaderText="Field Id" SortExpression="FIELD_ID" Visible="false">
                        <ItemTemplate>
                                <asp:Label ID="LblId" runat="server" Text='<%#Bind("FIELD_ID") %>'></asp:Label>
                        </ItemTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="Field Description" SortExpression="Column_label">
                         <ItemTemplate>
                                <asp:Label ID="LblDesc" runat="server" Text='<%#Bind("COLUMN_LABEL") %>'></asp:Label>
                         </ItemTemplate>
                         <FooterTemplate>
                                <asp:DropDownList ID="DdlColumn" AppendDataBoundItems="true" runat="server" Visible="true" DataSourceID="SDSColumnlabels" DataTextField="Column_label" DataValueField="Field_id" OnSelectedIndexChanged="DdlColumn_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                <asp:TextBox ID="TxtColumn" runat="server" Visible="false" MaxLength="25"></asp:TextBox> 
                                <span id="SpnCol" class="formattext" runat="server" visible="false">(max. 25 chars)</span>
                                <asp:CustomValidator ID="CvColumn"  CssClass="errlabels"  ValidateEmptyText="true" OnServerValidate="CvColumn_ServerValidate"  runat="server" ControlToValidate= "TxtColumn" ErrorMessage="Either Description is missing or field with this Description already exists" SetFocusOnError="true" ValidationGroup="add" Text="*"></asp:CustomValidator>
                                <asp:RegularExpressionValidator ID="RegexColumn" ControlToValidate="TxtColumn"
                                     ErrorMessage="Column label exceeded 25 characters" runat="server" ValidationExpression="(?:[\r\n]*.[\r\n]*){0,25}"  CssClass="errlabels" ValidationGroup="add" Text="*"></asp:RegularExpressionValidator>
                                <asp:RequiredFieldValidator ID="RFVColumn" runat="server" ControlToValidate="DdlColumn" InitialValue="-1" Text="*" ValidationGroup="add" ErrorMessage="Choose one of the existing values from the list or create a new value by selecting Create New" CssClass="errlabels" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="RegexChar" runat="server" ControlToValidate="TxtColumn"  ValidationExpression='<%#str%>' ValidationGroup="add"  Text="*" ErrorMessage="Double quotes/ Periods are not allowed" CssClass="errlabels" SetFocusOnError="true"></asp:RegularExpressionValidator>
                                <asp:RegularExpressionValidator ID="RegexscField" runat="server" ErrorMessage="< and > is not allowed in Field Description."  ValidationGroup="add" Display="Dynamic"  CssClass="errlabels" 
                                    ControlToValidate="TxtColumn" ValidationExpression="^[^<>]*$"  Text="*"></asp:RegularExpressionValidator>
                         </FooterTemplate>
                     </asp:TemplateField>
                     <asp:TemplateField HeaderText="Action">
                        <ItemTemplate>
                            <asp:Button ID="BtnDelete" runat="server" CommandName="del" CausesValidation="false" Text="Delete" OnClientClick="return confirm('Are you certain you want to delete this OJT Field for this facility?');" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "FIELD_ID") %>'/>
                        </ItemTemplate>
                        <FooterTemplate>
                            <asp:Button ID="BtnAdd" runat="server" CommandName="add" CausesValidation="true" Text="Add" ValidationGroup="add" />
                        </FooterTemplate>
                     </asp:TemplateField>
               </Columns>     
            </asp:GridView>

            <asp:SqlDataSource ID="SDSColumnlabels" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" 
                            ProviderName="System.Data.OracleClient"  SelectCommand="( ( select -1 as field_id,  '---Choose Existing---' as column_label, 1 as reorder from dual)
                             union (select field_id,column_label, 2 as reorder from lst_ojt_fields where 
                            field_id not in (select field_id from LST_FIELDS_FACILITY_MAP
                            WHERE FACILITY_ID = :FacId and is_active='Y')  and is_active='Y')
                            union
                            (select 20000 as field_id, '---Create New---' as column_label, 3 as reorder from dual)) order by reorder,column_label">
                <SelectParameters>
                    <asp:SessionParameter Name="FacId" SessionField="FacID"                       />
                </SelectParameters>
            </asp:SqlDataSource>
               
    </div>
        <div id="dialog-ojtmsg" style="display:none;" class="nounload">
            <p>
                <asp:Label ID="LblojtMsg" runat="server"></asp:Label>
            </p>
        </div>

       
    </form>
</body>
    
</html>
