<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileSummary.ascx.cs" Inherits="LST.UserControls.FileSummary" %>
    <asp:Panel ID="PnlFile" runat="server" Width="100%"  GroupingText="Files Attached">
            <asp:GridView ID="GvFile" runat="server" AutoGenerateColumns="false" onrowcommand="gvFile_RowCommand" Width="80%" OnRowDeleting="GvFile_RowDeleting"  OnRowDataBound="GvFile_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="ATTACHMENT_ID"  Visible="false"  HeaderStyle-HorizontalAlign="Left"/>
                    <asp:TemplateField HeaderText="File" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:LinkButton ID="LnkDownload"  CssClass="nounload" runat="server"  CommandArgument='<%# Eval("ATTACHMENT_ID") %>' CommandName="download" Text ='<%# Eval("FILE_NAME") %>'></asp:LinkButton>
                                </ItemTemplate>
                    </asp:TemplateField>                         
                    <asp:BoundField DataField="UPLOADED_BY" HeaderText="Uploaded by"  HeaderStyle-HorizontalAlign="Left"/>
                    <asp:BoundField DataField="UPLOADED_ON" HeaderText="Uploaded On" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" HeaderStyle-HorizontalAlign="Left"/>
                    <asp:TemplateField HeaderText="Delete" HeaderStyle-HorizontalAlign="Left">
                                <ItemTemplate>
                                    <asp:Label ID="Lbldelinfo" runat="server" Text="Not allowed"  Visible="false"></asp:Label>
                                    <asp:ImageButton ID="ImgBtnDelete" runat="server" ImageUrl="~/Images/deleteicon.gif"  CssClass="nounload" CommandArgument='<%# Eval("ATTACHMENT_ID") %>' CommandName="delete" OnClientClick = "return confirm('Warning! Are you sure you want to delete this attachment?');"/>
                                </ItemTemplate>
                    </asp:TemplateField>   
                              
                </Columns>
                <HeaderStyle BackColor="LightGray" />
            </asp:GridView>
        </asp:Panel>