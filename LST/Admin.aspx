<%@ Page Title="" Language="C#" MasterPageFile="~/LST.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="LST.Admin" StylesheetTheme="LSTTheme" MaintainScrollPositionOnPostback="true" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
   <link rel="stylesheet" href="Content/jquery-ui.css" />
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
   <script src="Scripts/jquery-1.10.2.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui.js" type="text/javascript"></script>
      <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
       <script type="text/javascript" src="Scripts/LST.js"></script> 
     <link href="Content/LST.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="Scripts/backbutton.js"></script>
     
    <script type="text/javascript">
        $(document).ready(function () {
            BindPicker();
        });

        function BindPicker() {
            $("input[type=text][id*=TxtEditAltFromDate]").datepicker({ showOn: "button", buttonImage: "Images/cal.gif", buttonImageOnly: true, buttonText: "Select date", changeMonth: true, changeYear: true });
            $("input[type=text][id*=TxtEditAltToDate]").datepicker({ showOn: "button", buttonImage: "Images/cal.gif", buttonImageOnly: true, buttonText: "Select date", changeMonth: true, changeYear: true });
        }

        function RefreshGrid(mode,type)
        {
           
           window.location.href = "Admin.aspx?mode=" + mode + "&type=" + type;
        }

    

        function SetPanel(panelid)
        {
            if (panelid == 'collapseOne')
            {
                $('#collapseOne').removeClass('collapse').addClass('in');
                $('#collapseTwo').removeClass('in').addClass('collapse');
                $('#collapseThree').removeClass('in').addClass('collapse');
                $('#collapseFour').removeClass('in').addClass('collapse');
                $('#collapseFive').removeClass('in').addClass('collapse');
                
            }
            else if (panelid == 'collapseTwo') 
            {
                $('#collapseTwo').removeClass('collapse').addClass('in');
                $('#collapseOne').removeClass('in').addClass('collapse');               
                $('#collapseThree').removeClass('in').addClass('collapse');
                $('#collapseFour').removeClass('in').addClass('collapse');
                $('#collapseFive').removeClass('in').addClass('collapse');
            }
            else if (panelid == 'collapseThree')
            {
                $('#collapseThree').removeClass('collapse').addClass('in');
                $('#collapseOne').removeClass('in').addClass('collapse');
                $('#collapseTwo').removeClass('in').addClass('collapse');
                $('#collapseFour').removeClass('in').addClass('collapse');
                $('#collapseFive').removeClass('in').addClass('collapse');
               
            }
            else if (panelid == 'collapseFour') {
                $('#collapseFour').removeClass('collapse').addClass('in');
                $('#collapseThree').removeClass('in').addClass('collapse');
                $('#collapseOne').removeClass('in').addClass('collapse');
                $('#collapseTwo').removeClass('in').addClass('collapse');
                $('#collapseFive').removeClass('in').addClass('collapse');

            }
            else if (panelid == 'collapseFive')
            {
                $('#collapseFive').removeClass('collapse').addClass('in');
                $('#collapseThree').removeClass('in').addClass('collapse');
                $('#collapseOne').removeClass('in').addClass('collapse');
                $('#collapseTwo').removeClass('in').addClass('collapse');
                $('#collapseFour').removeClass('in').addClass('collapse');
            }
            else
            {
                $('#collapseOne').removeClass('in').addClass('collapse');
                $('#collapseTwo').removeClass('in').addClass('collapse');
                $('#collapseThree').removeClass('in').addClass('collapse');
                $('#collapseFour').removeClass('in').addClass('collapse');
                $('#collapseFive').removeClass('in').addClass('collapse');
            }

        }

       /* jQuery('.panel-heading a').click(function () {
            $('.panel-heading').removeClass('actives');
            $(this).parents('.panel-heading').addClass('actives');
        });*/
      
    </script>
    <div style="align-content:center;">
       <b> LSO Details</b>
        <table class="table table-bordered"  style="width:70%; ">
            <tr>
                <td> LSO </td>
                <td><asp:Label ID="LblLso" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td>Deputy LSO </td>
                <td><asp:Label ID="LblDLSO" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td>Alternate LSO (Currently Active)</td>
                <td><div id="DivAlternate" runat="server">
                            <asp:Label ID="LblAlternate" runat="server"></asp:Label>
                            <asp:Label ID="LblFromTo" runat="server"></asp:Label>
                           
                            
                    </div>
                    
                </td>     
            </tr>
        </table>
    </div>
    <div class="bs-example">
      <p><i><strong>Note:</strong> Click on the linked heading text to expand or collapse  panels.</i></p>
    <div class="panel-group" id="accordion">
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title" style="font-weight:bold;">
                    <a data-toggle="collapse" data-parent="#accordion" href="#collapseOne">Facilities</a>
                </h4>
            </div>
            <div id="collapseOne" class="panel-collapse collapse">
                <asp:Panel ID="PnlFac" runat="server" DefaultButton="CmdSearchFac">
                     
                      
                      <div class="panel-body">
                         <div id="dialogfacility" style="display:none;">             
                            <iframe id="modaldialogfacility" width="100%" height="100%" ></iframe>
                       </div>
                        <p> <asp:TextBox ID="TxtSearchFac" runat="server"></asp:TextBox>&nbsp;&nbsp;
                          <asp:Button ID="CmdSearchFac" Text="Search" runat="server" CssClass ="btn btn-primary" Font-Bold="true" OnClick="CmdSearchFac_Click"/>&nbsp;&nbsp;<asp:Button ID="BtnAddFacility" runat="server"  ClientIDMode="Static" CssClass ="btn btn-primary"  Text="Add a Facility" OnClick="BtnAddFacility_Click" Font-Bold="true"/></p>
                        <asp:GridView id="GvFacility"  SkinID="gridviewSkin" runat="server" EmptyDataText="No Facilities found" AutoGenerateColumns="false" PageSize="20" Width="100%" CellPadding="5" CellSpacing="5" OnRowCommand="GvFacility_RowCommand"
                             OnRowEditing="GvFacility_RowEditing" ViewStateMode="Enabled" AllowPaging="true" OnPageIndexChanging="GvFacility_PageIndexChanging" AllowSorting="true"  OnSorting="GvFacility_Sorting"  OnRowDeleting="GvFacility_RowDeleting">
                            <Columns>
                                 <asp:TemplateField HeaderText="View">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgbtnView" ImageUrl ="~/Images/view.gif" CommandName="View" commandargument='<%# DataBinder.Eval(Container.DataItem, "FACILITY_ID")%>' runat="Server"/>
                                    </ItemTemplate>
                                 </asp:TemplateField> 
                                 <asp:TemplateField HeaderText="Edit">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="imgbtnEdit" ImageUrl ="~/Images/Edit1616.png" CommandName="Edit" commandargument='<%# DataBinder.Eval(Container.DataItem, "FACILITY_ID")%>' runat="Server"/>
                                    </ItemTemplate>                                                 
                                 </asp:TemplateField> 
                                <asp:TemplateField HeaderText ="Delete">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImgbtnDelete" ImageUrl="~/Images/deleteicon.gif" CommandName="Delete"  commandargument='<%# DataBinder.Eval(Container.DataItem, "FACILITY_ID")%>' runat="Server" OnClientClick="return confirm('Are you certain you want to delete this Facility?');"/>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="FACILITY_ID" Visible="false" SortExpression="FACILITY_ID" />
                                <asp:HyperLinkField DataNavigateUrlFields="FACILITY_WEBPAGE"   SortExpression="FACILITY_NAME"
                                              HeaderText="Facility Name" DataTextField="FACILITY_NAME"  Target="_blank"/>
                                <asp:BoundField DataField="BLDG" HeaderText="Bldg"  SortExpression="BLDG"/>
                                <asp:BoundField DataField="ROOM" HeaderText="Room" SortExpression="ROOM" />
                                <asp:BoundField DataField="SLSONAME" HeaderText="SLSO" SortExpression="SLSONAME" />
                                <asp:BoundField DataField="COSLSONAME" HeaderText="Co SLSO" SortExpression="COSLSONAME" />
                                <asp:BoundField DataField="ACTSLSONAME" HeaderText="Acting SLSO" SortExpression="ACTSLSONAME" />
                                <asp:BoundField DataField="APPROVAL_EXPIRY_DATE" HeaderText="Expiry Date" DataFormatString="{0:d-MMM-yy}" SortExpression="APPROVAL_EXPIRY_DATE" />
                            </Columns>
                           </asp:GridView>
            
                    
                </div>
                </asp:Panel>
              
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title" style="font-weight:bold;">
                    <a data-toggle="collapse" data-parent="#accordion" href="#collapseTwo">QLOs / LCA Workers</a>
                </h4>
            </div>
            <div id="collapseTwo" class="panel-collapse collapse">
                 <asp:Panel ID="PnlWorker" runat="server" DefaultButton="CmdSearchWorker">
                <div class="panel-body">
                    <p><asp:TextBox ID="TxtSearchWorker" runat="server"></asp:TextBox>&nbsp;&nbsp;
                          <asp:Button ID="CmdSearchWorker" Text="Search" runat="server" CssClass ="btn btn-primary" Font-Bold="true" OnClick="CmdSearchWorker_Click"/>&nbsp;&nbsp;<asp:Button ID="BtnAddWorker" runat="server" ClientIDMode="Static" CssClass="btn btn-primary" Text="Add a Worker" OnClick="BtnAddWorker_Click"  Font-Bold="true" Visible="false"/></p>
                    <asp:GridView ID="GvWorker" SkinID="gridviewSkin" runat="server" EmptyDataText="No Worker found" AutoGenerateColumns="false" PageSize="20" Width="100%" CellPadding="5" CellSpacing="5" OnRowCommand="GvWorker_RowCommand" OnRowEditing="GvWorker_RowEditing" 
                        OnRowDataBound="GvWorker_RowDataBound" ViewStateMode="Enabled" AllowPaging="true" OnPageIndexChanging="GvWorker_PageIndexChanging" AllowSorting="true" OnSorting="GvWorker_Sorting"  OnRowDeleting="GvWorker_RowDeleting">
                        <Columns>
                            <asp:TemplateField HeaderText="View">
                                <ItemTemplate>
                                        <asp:ImageButton ID="ImgbtnView" ImageUrl="~/Images/view.gif" CommandName="View" commandargument='<%# DataBinder.Eval(Container.DataItem, "WORKER_ID")%>' runat="Server"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Edit">
                                <ItemTemplate>
                                      <asp:ImageButton ID="imgbtnEdit" ImageUrl ="~/Images/Edit1616.png" CommandName="Edit" commandargument='<%# DataBinder.Eval(Container.DataItem, "WORKER_ID")%>' runat="Server"/>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText ="Delete">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImgbtnDelete" ImageUrl="~/Images/deleteicon.gif" CommandName="Delete"  commandargument='<%# DataBinder.Eval(Container.DataItem, "WORKER_ID")%>' runat="Server" OnClientClick="return confirm('Are you certain you want to delete this person?');"/>
                                    </ItemTemplate>
                                </asp:TemplateField> 
                                <asp:BoundField DataField="WORKER_ID" Visible="false" />
                                <asp:BoundField DataField="SLAC_ID"  HeaderText="Slac Id"  SortExpression="SLAC_ID"/>
                                <asp:BoundField DataField="WORKER_NAME" HeaderText="Name"  SortExpression="WORKER_NAME"/>
                               <asp:TemplateField HeaderText="Badge Id">
                                   <ItemTemplate>
                                       <asp:Label ID="LblBadge" runat="server"></asp:Label>
                                   </ItemTemplate>
                               </asp:TemplateField>
                                <asp:BoundField DataField="EMAIL" HeaderText="Email"  SortExpression="EMAIL"/>
                                <asp:BoundField DataField="AFFILIATION" HeaderText="Affiliation"  SortExpression="AFFILIATION"/>                        
                                <asp:BoundField DataField="STATUS" HeaderText="Status" SortExpression="STATUS" />
                                <asp:BoundField DataField="REASON_INACTIVE" HeaderText="Reason, if not active" SortExpression="REASON_INACTIVE" />
                                <asp:BoundField DataField="GONET" HeaderText="Phonebook Status" SortExpression="GONET" />
                            </Columns>
                         
                         </asp:GridView>            
                </div>
                     </asp:Panel>
            </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title" style="font-weight:bold;">
                    <a data-toggle="collapse" data-parent="#accordion" href="#collapseThree">System Laser Safety Officers</a>
                </h4>
            </div>
            <div id="collapseThree" class="panel-collapse collapse">
                 <asp:Panel ID="PnlSLSO" runat="server" DefaultButton="CmdSearchSLSO">
                <div class="panel-body">
                     <div id="dialogUser" style="display:none;" >             
                            <iframe id="modaldialogUser" width="100%" height="100%" ></iframe>
                       </div>
                    <div id="dialogDesignate" style="display:none;">
                        <iframe id="modaldialogDesignate" width="100%" height="100%">
                           
                        </iframe>
                    </div><asp:TextBox ID="TxtSearchSLSO" runat="server"></asp:TextBox>&nbsp;&nbsp;
                          <asp:Button ID="CmdSearchSLSO" Text="Search" runat="server" CssClass ="btn btn-primary" Font-Bold="true" OnClick="CmdSearchSLSO_Click"/>&nbsp;&nbsp;
                   
                    <asp:Button ID="BtnAddSLSO" CssClass ="btn btn-primary" runat="server" Text="Add a SLSO" Font-Bold="true" />
                    <hr />
                    <asp:GridView ID="GVUser" runat="server" SkinID="gridviewSkin" AutoGenerateColumns="false" EmptyDataText ="No User found" PageSize="15" CellPadding="5" CellSpacing="5" Width="80%" 
                        ViewStateMode="Enabled" AllowPaging="true" OnPageIndexChanging="GVUser_PageIndexChanging" AllowSorting="true"  
                         OnRowCreated="GVUser_RowCreated"  OnSorting="GVUser_Sorting" OnRowDeleting="GVUser_RowDeleting" OnRowCommand="GVUser_RowCommand" OnRowDataBound="GVUser_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="USERNAME" HeaderText="Name" ItemStyle-Width="35%"   SortExpression="USERNAME" />
                            <asp:BoundField DataField="USER_ROLE_ID" Visible="false" />
                            <asp:BoundField DataField="SLAC_ID" Visible="false" />
                            <asp:TemplateField HeaderText="Facility with corresponding Role" ItemStyle-Width="50%">
                                <ItemTemplate>
                                        <asp:GridView ID="GvSLSO" runat="server" AutoGenerateColumns="false"
                                                 EmptyDataText="None" Width="100%" GridLines="None" ShowHeader="false">
                                            <Columns>
                                                <asp:BoundField DataField="FACILITY_ID" Visible="false" />
                                                <asp:BoundField dataField="FACILITY_NAME"  ItemStyle-Width="50%"/>
                                                <asp:BoundField DataField="USERTYPE"  ItemStyle-Width="50%"/>
                                               
                                            </Columns>
                                            </asp:GridView>
                                </ItemTemplate>

                            </asp:TemplateField>  
                             <asp:BoundField DataField="STATUS"  HeaderText="Phonebook Status" /> 
                             <asp:TemplateField HeaderText ="Delete">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImgbtnDelete" ImageUrl="~/Images/deleteicon.gif" CommandName="Delete"  commandargument='<%# DataBinder.Eval(Container.DataItem, "SLAC_ID")%>' runat="Server" OnClientClick="return confirm('Are you certain you want to delete this SLSO?');"/>
                                    </ItemTemplate>
                                </asp:TemplateField>                     
                        </Columns>
                        
                    </asp:GridView>
                </div>
                     </asp:Panel>
            </div>

        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title" style="font-weight:bold">
                    <a data-toggle="collapse" data-parent="#accordion" href="#collapseFour">OJT Fields Master</a>
                </h4>
            </div>
            <div id="collapseFour" class="panel-collapse collapse">
                <asp:Panel ID="PnlOJT" runat="server">
                      <div class="panel-body">
                    
                <asp:GridView ID="GVOJTField" runat="server" SkinID="gridviewSkin" AutoGenerateColumns="false" EmptyDataText="No OJT Fields found" PageSize="15"  DataKeyNames="FIELD_ID"
                     CellPadding="5" CellSpacing="5" width="90%"  ViewStateMode="Enabled" AllowPaging="true" OnPageIndexChanging="GVOJTField_PageIndexChanging" AllowSorting="true"
                      OnSorting ="GVOJTField_Sorting" OnRowDeleting="GVOJTField_RowDeleting" OnRowCommand="GVOJTField_RowCommand" OnRowUpdated="GVOJTField_RowUpdated" OnRowUpdating="GVOJTField_RowUpdating" OnRowCancelingEdit="GVOJTField_RowCancelingEdit" OnRowEditing="GVOJTField_RowEditing">
                    <Columns>
                         <asp:TemplateField HeaderText="Edit  Delete" HeaderStyle-Width="11%" ItemStyle-Width="11%" HeaderStyle-Wrap="false" >
                                <EditItemTemplate>
                                    <asp:LinkButton ID="cmdEditupdate" runat="server" CausesValidation="True" CommandName="Update"   Text="Update" ValidationGroup="edit" />
                                    <asp:LinkButton ID="cmdCancel" runat="server" CausesValidation="False"
                                            CommandName="Cancel" Text="Cancel" CssClass="nounload" />
                                </EditItemTemplate>
                                <ItemTemplate>
                                    &nbsp;
                                    <asp:ImageButton ID="cmdEdit" runat="server" CausesValidation="False" ImageUrl="~/Images/Edit1616.png" CommandName="Edit" CssClass="nounload"
                                        Text="Edit" /> &nbsp;&nbsp;
                                    <asp:ImageButton ID="cmdDelete" CausesValidation="false" ImageUrl="~/Images/deleteicon.gif" CommandName="delete"  CommandArgument='<%# DataBinder.Eval(Container, "RowIndex") %>' runat="Server" OnClientClick = "return confirm('Warning! Are you sure you want to delete this OJT Field?');"/>
                                    &nbsp;
                                </ItemTemplate>
                       </asp:TemplateField> 
                      
                        <asp:BoundField DataField ="FIELD_ID"  HeaderText ="Field Id" SortExpression="FIELD_ID" ReadOnly="true"  HeaderStyle-Width="7%" HeaderStyle-Wrap="false" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"  ItemStyle-VerticalAlign="Middle"/>
                        <asp:TemplateField HeaderText="Description" SortExpression="COLUMN_LABEL" HeaderStyle-Wrap="false" HeaderStyle-Width="35%" >
                            <ItemStyle VerticalAlign="Middle" />
                            <EditItemTemplate >
                                    
                                    <asp:TextBox id="TxtDesc" runat="server" Text='<%#Eval("COLUMN_LABEL") %>' MaxLength="25"></asp:TextBox>
                                    <span id="SpnCol" class="formattext" runat="server">(max. 25 chars)</span> 
                                 <br />
                                    <asp:RequiredFieldValidator ID="RFVDesc" runat="server" ControlToValidate="TxtDesc" ErrorMessage="Field Description required" CssClass="errlabels" ValidationGroup="edit" Display="Dynamic" ></asp:RequiredFieldValidator>                                   
                                    <asp:CustomValidator ID="CvColumn"  CssClass="errlabels"  ValidateEmptyText="true"   runat="server" ControlToValidate= "TxtDesc" ErrorMessage="Field with this Description already exists" SetFocusOnError="true" ValidationGroup="edit" Display="Dynamic"></asp:CustomValidator>
                                    <asp:RegularExpressionValidator ID="RegexColumn" ControlToValidate="TxtDesc" CssClass="errlabels"
                                     ErrorMessage="Exceeded 25 chars" runat="server" ValidationExpression="(?:[\r\n]*.[\r\n]*){0,25}"  ValidationGroup="edit" Display="Dynamic"></asp:RegularExpressionValidator>
                                     <asp:RegularExpressionValidator ID="RegexChar" runat="server" ControlToValidate="TxtDesc" CssClass="errlabels" ValidationExpression='<%#str%>' ValidationGroup="edit"  Display="Dynamic" ErrorMessage="Double quotes/Periods are not allowed" SetFocusOnError="true"></asp:RegularExpressionValidator>
                                     <asp:RegularExpressionValidator ID="RegexscDesc" runat="server" ErrorMessage="< and > is not allowed in Description."  ValidationGroup="edit" Display="Dynamic" CssClass="errlabels"
                                     ControlToValidate="TxtDesc" ValidationExpression="^[^<>]*$" ></asp:RegularExpressionValidator>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="LblDesc" runat="Server" Text='<%#Eval("COLUMN_LABEL") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Facilities using this Field" HeaderStyle-Wrap="false">
                            <ItemTemplate>
                                <asp:Label ID="LblFacs" runat="server" Text='<%#Eval("FACILITIES") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                       
                    </Columns>
                </asp:GridView>
            </div>
                    </asp:Panel>
             </div>
        </div>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h4 class="panel-title" style="font-weight:bold;">
                    <a data-toggle="collapse" data-parent="#accordion" href="#collapseFive">Manage Alternate LSO(s)/SLSO(s)</a>
                </h4>
            </div>
            <div id="collapseFive" class="panel-collapse collapse">
                <asp:Panel ID="PnlUsers" runat="server">
                    <div class="panel-body">
                        <asp:DropDownList ID="DdlAlternates" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlAlternates_SelectedIndexChanged">
                            <asp:ListItem Text="Alternate LSO" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Alternate SLSO" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                         
                        <div id="DivAltLSO" runat="server" visible="true">
       
                            <br />
                            <asp:Button ID="BtnDesignate" CssClass= "btn btn-primary nounload" runat ="server" Text="Designate Alternate LSO" Font-Bold="true" />
     
                            <br />
                              <asp:Label ID="LblMsgAL" runat="server" CssClass="errlabels"></asp:Label>
                             <br />
                             <asp:GridView ID="GvAltLSO" runat="server" SkinID="gridviewSkin" CellSpacing="5"
                                 pagesize="15"  AutoGenerateColumns="false"  OnRowCommand="GvAltLSO_RowCommand" AllowPaging="true" AllowSorting="true" 
                                  OnRowEditing="GvAltLSO_RowEditing" OnRowUpdating="GvAltLSO_RowUpdating" ViewStateMode="Enabled"
                                  OnRowCancelingEdit="GvAltLSO_RowCancelingEdit" OnSorting="GvAltLSO_Sorting" DataKeyNames="USER_ROLE_ID" 
                                  OnRowUpdated="GvAltLSO_RowUpdated" OnPageIndexChanging="GvAltLSO_PageIndexChanging" OnRowDeleting="GvAltLSO_RowDeleting">
                                 <Columns>
                                      <asp:TemplateField HeaderText="Edit  Delete" HeaderStyle-Width="15%" ItemStyle-Width="15%"  HeaderStyle-Wrap="false" >
                                        <EditItemTemplate>
                                            <asp:LinkButton ID="cmdEditupdate" runat="server" CausesValidation="True" CommandName="Update"   Text="Update" ValidationGroup="EditAL" />
                                            <asp:LinkButton ID="cmdCancel" runat="server" CausesValidation="False"
                                                    CommandName="Cancel" Text="Cancel" CssClass="nounload" />
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            &nbsp;
                                            <asp:ImageButton ID="cmdEdit" runat="server" CausesValidation="False" ImageUrl="~/Images/Edit1616.png" CommandName="Edit" CssClass="nounload"
                                                Text="Edit" CommandArgument='<%#Eval("USER_ROLE_ID") %>' /> &nbsp;&nbsp;
                                            <asp:ImageButton ID="cmdDelete" CausesValidation="false" ImageUrl="~/Images/deleteicon.gif" CommandName="delete"  CommandArgument='<%#Eval("USER_ROLE_ID") %>' runat="Server" OnClientClick = "return confirm('Warning! Are you sure you want to delete this Alternate LSO?');"/>
                                            &nbsp;
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                                     <asp:BoundField  DataField="USER_ROLE_ID" Visible="false"/>
                                     <asp:TemplateField HeaderText="Alternate LSO Name" SortExpression="MGRNAME" HeaderStyle-Width="20%" ItemStyle-Width="20%" >
                                            <ItemTemplate>
                                                <asp:Label ID="LblName" runat="server" Text='<%# Eval("MGRNAME")%>'></asp:Label>
                                            </ItemTemplate>
                                  
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date From" SortExpression="ALT_FROM" HeaderStyle-Width="30%" ItemStyle-Width="30%">
                                        <ItemTemplate>
                                            <asp:Label ID="LblAltFrom" runat="server" Text='<%#Eval("ALT_FROM","{0:MM/dd/yyyy}") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                             <asp:Label id="LblEditAltFrom" runat="server" Text=""></asp:Label>
                                             <asp:TextBox ID="TxtEditAltFromDate" runat="server" Width="50%"  Text='<%#Eval("ALT_FROM","{0:MM/dd/yyyy}") %>' ></asp:TextBox>
                                              <span class="formattext" id="spnEditFrom" runat="server" >mm/dd/yyyy</span> 
                                            <br />
                                            <asp:RequiredFieldValidator ID="RFVEditAltFromDate" runat="server" ControlToValidate="TxtEditAltFromDate" ValidationGroup="EditAL" ErrorMessage="From Date is required for Alternates" Display="Dynamic"     SetFocusOnError="true" CssClass="errlabels"></asp:RequiredFieldValidator>
                                             <asp:CompareValidator ID="CvEditAltFromDate" runat="server"  Type="Date" Operator="DataTypeCheck"
                                                                                    ControlToValidate="TxtEditAltFromDate" ErrorMessage="From date is not valid. Format should be MM/dd/yyyy"  CssClass="errlabels" ValidationGroup="EditAL" Display="Dynamic"    SetFocusOnError="true"></asp:CompareValidator>

                                        </EditItemTemplate>
                                       
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date To" SortExpression="ALT_TO" HeaderStyle-Width="30%" ItemStyle-Width="30%" >
                                            <ItemTemplate>
                                                <asp:Label ID="LblAltTo" runat="server" Text='<%#Eval("ALT_TO","{0:MM/dd/yyyy}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="LblEditAltTo" runat="server" Text=""></asp:Label>
                                                <asp:TextBox ID="TxtEditAltToDate" runat="server" Width="50%"   Text='<%#Eval("ALT_TO","{0:MM/dd/yyyy}") %>'></asp:TextBox>
                                                <span class ="formattext" id="spnEditTo" runat="server">mm/dd/yyyy</span> 
                                                <br />
                                                <asp:RequiredFieldValidator ID="RFVEditAltToDate" runat="server" ControlToValidate="TxtEditAltToDate" ValidationGroup="EditAL" ErrorMessage="To Date is required for Alternates" Display="Dynamic"     SetFocusOnError="true" CssClass="errlabels"></asp:RequiredFieldValidator>
                                                 <asp:CompareValidator ID="CvEditAltToDate" runat="server"  Type="Date" Operator="DataTypeCheck"
                                                                                        ControlToValidate="TxtEditAltToDate" ErrorMessage="To date is not valid. Format should be MM/dd/yyyy"  CssClass="errlabels" Display="Dynamic" ValidationGroup="EditAL"     SetFocusOnError="true"></asp:CompareValidator>
                                                <asp:CustomValidator ID="CustEditValDate" runat="server"  OnServerValidate="CustEditValDate_ServerValidate"  ControlToValidate="TxtEditAltToDate" SetFocusOnError="true" Display="Dynamic" ValidationGroup="EditAL"  CssClass="errlabels"></asp:CustomValidator>
                                              
                                            </EditItemTemplate>
                                           
                                       </asp:TemplateField>
                                    
                                 </Columns>
                                   
                                  
              
                
                
               
                
                             </asp:GridView> 
                        </div>

                        <div id="DivAltSLSO" runat="server" visible="false">
                             <br />
                             <asp:Button ID="BtnDesignateSLSO" CssClass="btn btn-primary" runat="server" Text="Designate an Alternate SLSO" Font-Bold="true" />
                            <br />
                             <br />
                             <asp:GridView ID="GvAltSLSO" runat="server" SkinID="gridviewSkin" CellSpacing="5"
                                 pagesize="15" AutoGenerateColumns="false" OnRowCommand="GvAltSLSO_RowCommand"
                                 AllowPaging="true" AllowSorting="true" OnRowEditing="GvAltSLSO_RowEditing"
                                 OnRowUpdating="GvAltSLSO_RowUpdating" ViewStateMode="Enabled"
                                  OnRowCancelingEdit="GvAltSLSO_RowCancelingEdit" OnSorting="GvAltSLSO_Sorting"
                                  DataKeyNames="FACILITY_ID"  OnRowUpdated="GvAltSLSO_RowUpdated" OnPageIndexChanging="GvAltSLSO_PageIndexChanging" OnRowDeleting="GvAltSLSO_RowDeleting">
                                   <Columns>
                                      <asp:TemplateField HeaderText="Edit  Delete" HeaderStyle-Width="10%" ItemStyle-Width="10%"  HeaderStyle-Wrap="false" >
                                        <EditItemTemplate>
                                            <asp:LinkButton ID="cmdEditupdate" runat="server" CausesValidation="True" CommandName="Update"   Text="Update" ValidationGroup="EditASL" />
                                            <asp:LinkButton ID="cmdCancel" runat="server" CausesValidation="False"
                                                    CommandName="Cancel" Text="Cancel" CssClass="nounload" />
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            &nbsp;
                                            <asp:ImageButton ID="cmdEdit" runat="server" CausesValidation="False" ImageUrl="~/Images/Edit1616.png" CommandName="Edit" CssClass="nounload"
                                                Text="Edit" CommandArgument='<%#Eval("FACILITY_ID") %>' /> &nbsp;&nbsp;
                                            <asp:ImageButton ID="cmdDelete" CausesValidation="false" ImageUrl="~/Images/deleteicon.gif" CommandName="delete"  CommandArgument='<%#Eval("FACILITY_ID") %>' runat="Server" OnClientClick = "return confirm('Warning! Are you sure you want to delete this Alternate SLSO?');"/>
                                            &nbsp;
                                        </ItemTemplate>
                                    </asp:TemplateField> 
                                     <asp:BoundField  DataField="FACILITY_ID" Visible="false"/>
                                     <asp:BoundField DataField="FACILITY_NAME" SortExpression="FACILITY_NAME" HeaderText="Facility"  ReadOnly="true" ItemStyle-Width="22%"/>
                                     <asp:TemplateField HeaderText="Alternate SLSO Name" SortExpression="ALTSLSO" HeaderStyle-Width="22%" ItemStyle-Width="22%" >
                                            <ItemTemplate>
                                                <asp:Label ID="LblName" runat="server" Text='<%# Eval("ALTSLSO")%>'></asp:Label>
                                            </ItemTemplate>
                                  
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date From" SortExpression="ALTSLSO_FROM" HeaderStyle-Width="23%" ItemStyle-Width="23%">
                                        <ItemTemplate>
                                            <asp:Label ID="LblAltFrom" runat="server" Text='<%#Eval("ALTSLSO_FROM","{0:MM/dd/yyyy}") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                             <asp:Label id="LblEditAltFrom" runat="server" Text=""></asp:Label>
                                             <asp:TextBox ID="TxtEditAltFromDate" runat="server" Width="50%"  Text='<%#Eval("ALTSLSO_FROM","{0:MM/dd/yyyy}") %>' ></asp:TextBox>
                                              <span class="formattext" id="spnEditFrom" runat="server" >mm/dd/yyyy</span> 
                                            <br />
                                            <asp:RequiredFieldValidator ID="RFVEditAltFromDate" runat="server" ControlToValidate="TxtEditAltFromDate" ValidationGroup="EditASL" ErrorMessage="From Date is required for Alternates" Display="Dynamic"     SetFocusOnError="true" CssClass="errlabels"></asp:RequiredFieldValidator>
                                             <asp:CompareValidator ID="CvEditAltFromDate" runat="server"  Type="Date" Operator="DataTypeCheck"
                                                                                    ControlToValidate="TxtEditAltFromDate" ErrorMessage="From date is not valid. Format should be MM/dd/yyyy"  CssClass="errlabels" ValidationGroup="EditASL" Display="Dynamic"    SetFocusOnError="true"></asp:CompareValidator>

                                        </EditItemTemplate>
                                       
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date To" SortExpression="ALTSLSO_TO" HeaderStyle-Width="23%" ItemStyle-Width="23%" >
                                            <ItemTemplate>
                                                <asp:Label ID="LblAltTo" runat="server" Text='<%#Eval("ALTSLSO_TO","{0:MM/dd/yyyy}") %>'></asp:Label>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:Label ID="LblEditAltTo" runat="server" Text=""></asp:Label>
                                                <asp:TextBox ID="TxtEditAltToDate" runat="server" Width="50%"   Text='<%#Eval("ALTSLSO_TO","{0:MM/dd/yyyy}") %>'></asp:TextBox>
                                                <span class ="formattext" id="spnEditTo" runat="server">mm/dd/yyyy</span> 
                                                <br />
                                                <asp:RequiredFieldValidator ID="RFVEditAltToDate" runat="server" ControlToValidate="TxtEditAltToDate" ValidationGroup="EditASL" ErrorMessage="To Date is required for Alternates" Display="Dynamic"     SetFocusOnError="true" CssClass="errlabels"></asp:RequiredFieldValidator>
                                                 <asp:CompareValidator ID="CvEditAltToDate" runat="server"  Type="Date" Operator="DataTypeCheck"
                                                                                        ControlToValidate="TxtEditAltToDate" ErrorMessage="To date is not valid. Format should be MM/dd/yyyy"  CssClass="errlabels" Display="Dynamic" ValidationGroup="EditASL"     SetFocusOnError="true"></asp:CompareValidator>
                                                <asp:CustomValidator ID="CustEditValDateASL" runat="server"  OnServerValidate="CustEditValDateASL_ServerValidate"  ControlToValidate="TxtEditAltToDate" SetFocusOnError="true" Display="Dynamic" ValidationGroup="EditASL"  CssClass="errlabels"></asp:CustomValidator>
                                              
                                            </EditItemTemplate>
                                           
                                       </asp:TemplateField>
                                    
                                 </Columns>
                                   
                                  
                             </asp:GridView>
                        </div>
            

                        <asp:SqlDataSource ID="dsUsers" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>"
                            SelectCommand="SELECT M.USER_ROLE_ID,M.SLAC_ID, P.NAME AS MGRNAME, M.ALTLSO_FROM AS ALT_FROM, M.ALTLSO_TO AS ALT_TO FROM LST_USER_ROLES M LEFT JOIN PERSONS.PERSON P ON M.SLAC_ID = P.KEY 
                                WHERE  M.ROLE_TYPE_ID = 20 ">   
                        </asp:SqlDataSource>
                        
                        <asp:SqlDataSource ID="SDSAltSlso" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" DataSourceMode="DataSet"  ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>"
                            SelectCommand="SELECT F.FACILITY_ID, F.FACILITY_NAME, F.ALTERNATE_SLSO, PC.NAME AS ALTSLSO, F.ALTSLSO_FROM,
                            F.ALTSLSO_TO FROM LST_FACILITY F LEFT JOIN PERSONS.PERSON PC ON F.ALTERNATE_SLSO = PC.KEY WHERE F.ALTERNATE_SLSO IS NOT NULL
                            AND F.IS_ACTIVE='Y' ">   
                        </asp:SqlDataSource>      
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

         <div id="dialog-msg"  style="display:none;" class="result-dialog">
           <asp:Label ID="LblMsg" runat="server"></asp:Label>

        </div>
	
</div>








</asp:Content>
