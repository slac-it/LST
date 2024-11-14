<%@ Page Title="" Language="C#" MasterPageFile="~/LST.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" StylesheetTheme="LSTTheme" Inherits="LST.Reports" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <script type="text/javascript" src="Scripts/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script> 
     <script type="text/javascript" src="Scripts/LST.js"></script>
    <script type="text/javascript" src="Scripts/backbutton.js"></script>
   
    <asp:Panel ID="PnlReports" GroupingText="Reports" runat="server">
          <div class="row" style="text-align:center;">
        <asp:Label ID="LblReporttype" runat="server" Text="Choose a report type from the list to view"></asp:Label>
        &nbsp;&nbsp;&nbsp;
        <asp:dropdownlist runat="server" ID="DdlReportType" AutoPostBack="True" OnSelectedIndexChanged="DdlReportType_SelectedIndexChanged">
            <asp:ListItem Text="QLO Summary" Value="0"></asp:ListItem>
            <asp:ListItem Text="LCA Worker Summary" Value="1"></asp:ListItem>
            <asp:ListItem Text="SLSO Summary" Value="2"></asp:ListItem>
            <asp:ListItem Text="Laser Facility Summary" value="3"  Selected="true"></asp:ListItem>
            <asp:ListItem Text="ESH Coordinator Summary" Value="4"></asp:ListItem>
            <asp:ListItem Text="Program Manager Summary" Value="5"></asp:ListItem>
            <asp:ListItem Text="SLSOs with 130 Overdue" Value="6"></asp:ListItem>
            <asp:ListItem Text="Laser Workers with 253 or 219 Overdue" Value="7"></asp:ListItem>
            <asp:ListItem Text="Laser Facility - Approval Requests" Value="8"></asp:ListItem>
            <asp:ListItem Text="Laser Worker - Approval Requests" Value="9"></asp:ListItem>
            <asp:ListItem Text="Admin Supervisor Summary" Value="10"></asp:ListItem>
         </asp:dropdownlist>
 
    </div>
    <p>&nbsp;</p>
    <div class="row">&nbsp;</div>
   
    <div class="row" id="DivQLOSummary" runat="server">
        <asp:Panel ID="PnlQLO" DefaultButton ="CmdSearchQLO" runat="server">
          <div class="row">
             <div class="col-sm-2">  <asp:ImageButton ID="ImgBtnQLO" runat="server"  ImageUrl="~/Images/ExportToExcel.gif" OnClick="ImgBtnQLO_Click" ToolTip="Tip: Please use 'Save and Open' option to view the contents of the file"/></div>
             <div class="col-sm-10">
                 Search QLO: <asp:TextBox id="TxtSearchQLO" runat="server"></asp:TextBox>&nbsp; &nbsp;<asp:Button ID="CmdSearchQLO" Text="Search" runat="server" CssClass="btn btn-primary" OnClick="CmdSearchQLO_Click"   ClientIDMode="Static"/>
                 &nbsp;&nbsp; #QLOs - <%= GetCount(7,0) %>    &nbsp;&nbsp; #active - <%= GetCount(7,5) %>   &nbsp;&nbsp; #inactive - <%= GetCount(7,6) %>*
                 <br />*<i>this number based on meeting requirements that are not lab-specific</i>
             </div>
           </div>
          <div class="row">&nbsp;</div>
          <div class="row">
             <asp:GridView ID="GvQLO" runat="server" AutoGenerateColumns="false" SkinID="gridViewSkin" 
                 Width="100%" AllowPaging="true" AllowSorting="true" PageSize="25"  DataKeyNames="WORKER_ID" DataSourceID="SDSQLOSum"
                  EmptyDataText="No QLOs found">
            <Columns>
                 <asp:HyperLinkField DataNavigateUrlFields="worker_Id"   SortExpression="Worker"
                                              HeaderText="Name" DataTextField="Worker"  DataNavigateUrlFormatString="~/WorkerEntry.aspx?mode=view&id={0}&dd=0"  ItemStyle-Width="20%"/>
                <asp:BoundField HeaderText="131" DataField="C131" SortExpression="C131" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-Width="10%"/>
                <asp:BoundField HeaderText="253" DataField="C253"  SortExpression="C253" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-Width="10%"/>
                <asp:BoundField HeaderText="253PRA" DataField="C253PRA"  SortExpression="C253PRA" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-Width="10%"/>
                <asp:BoundField HeaderText="Active for" DataField="Activefacs" SortExpression="Activefacs" ItemStyle-Width="20%"/>
                <asp:BoundField HeaderText="Inactive for" DataField="Inactivefacs" SortExpression="Inactivefacs" ItemStyle-Width="20%" />
                <asp:BoundField HeaderText="Phonebook Status" DataField="PHONEBOOK_STATUS" SortExpression="PHONEBOOK_STATUS" ItemStyle-Width="10%" />
            </Columns>
             
           </asp:GridView>
              <asp:SqlDataSource id="SDSQLOSum" DataSourceMode="DataSet" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>"
                   SelectCommand="SELECT DISTINCT F.Worker,   F.Worker_Id ,  TO_CHAR(Trainsafe.Get_Last_Session_Date(Slac_Id,'131'),'mm/dd/yyyy') C131,  TO_CHAR(Trainsafe.Get_Last_Session_Date(Slac_Id,'253'),'mm/dd/yyyy') C253,
                            TO_CHAR(Trainsafe.Get_Last_Session_Date(Slac_Id,'253PRA'),'mm/dd/yyyy') C253pra,   (Select Listagg(Facility_Name,',  ') Within Group (Order By Facility_Name) Cnt
                                From Lst_facility f join lst_worker_facility_map m on f.facility_id = m.facility_id
                                Where f.Is_Active = 'Y' and m.is_active= 'Y' and m.worker_id=F.Worker_Id And m.Status_Id=5 and  m.worker_type_id = 7) Activefacs,
                                (Select Listagg(Facility_Name,',  ') Within Group (Order By Facility_Name) Cnt From Lst_facility f
                                join lst_worker_facility_map m on f.facility_id = m.facility_id
                                Where f.Is_Active = 'Y' and m.is_active= 'Y' and m.worker_id=F.Worker_Id And m.Status_Id=6 and  m.worker_type_id = 7)  Inactivefacs,
                                  F.SLAC_EMAIL, F.PHONEBOOK_STATUS,F.PREFERRED_EMAIL                             
                                FROM Vw_Lst_Worker_Facility F WHERE F.WORKER_ID IN   (SELECT WORKER_ID FROM LST_WORKER WHERE STATUS_ID IN (5,6) AND IS_ACTIVE ='Y' )
                                AND F.WORKER_ID IN   (SELECT WORKER_ID  FROM LST_WORKER_FACILITY_MAP  WHERE IS_ACTIVE    ='Y' AND STATUS_ID IN (5,6)  AND worker_type_id = 7) ORDER BY WORKER"
                   FilterExpression= "WORKER like '%{0}%' ">
                    <FilterParameters>
                            <asp:ControlParameter Name="WORKER" ControlID="TxtSearchQLO" PropertyName="Text" ConvertEmptyStringToNull="false" />
                    </FilterParameters>
              </asp:SqlDataSource>
          </div>
            </asp:Panel>
    </div>
        <div class="row" id="DivLCAWSummary" runat="server" visible="false">
            <asp:Panel ID="PnlLCAW" DefaultButton="CmdSearchLCA" runat="server">
          <div class="row">
             <div class="col-sm-2">  <asp:ImageButton ID="ImgBtnLCA" runat="server"  ImageUrl="~/Images/ExportToExcel.gif" OnClick="ImgBtnLCA_Click" ToolTip="Tip: Please use 'Save and Open' option to view the contents of the file"/></div>
             <div class="col-sm-10">
                 Search LCA Worker: <asp:TextBox id="TxtSearchLCA" runat="server"></asp:TextBox>&nbsp; &nbsp;<asp:Button ID="CmdSearchLCA" Text="Search" runat="server" CssClass="btn btn-primary" OnClick="CmdSearchLCA_Click"  ClientIDMode="Static"/>
                  &nbsp;&nbsp; #LCA Workers - <%=GetCount(8,0) %>     &nbsp;&nbsp; # active - <%=GetCount(8,5) %>  &nbsp;&nbsp; #inactive - <%= GetCount(8,6) %>*
                 <br />*<i>this number based on meeting requirements that are not lab-specific</i>
             </div>
           </div>
          <div class="row">&nbsp;</div>
          <div class="row">
             <asp:GridView ID="GvLCAW" runat="server" AutoGenerateColumns="false" SkinID="gridViewSkin" Width="100%" AllowPaging="true" 
                 AllowSorting="true" PageSize="25"  DataKeyNames="WORKER_ID" DataSourceID="SDSLCAW"
                 EmptyDataText="No LCA Workers found">
            <Columns>
                 <asp:HyperLinkField DataNavigateUrlFields="worker_Id"   SortExpression="Worker"
                                              HeaderText="Name" DataTextField="Worker"  DataNavigateUrlFormatString="~/WorkerEntry.aspx?mode=view&id={0}&dd=1" ItemStyle-Width="20%"/>
                <asp:BoundField HeaderText="131" DataField="C131" SortExpression="C131" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-Width="15%"/>
                <asp:BoundField HeaderText="253" DataField="C253"  SortExpression="C253" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" ItemStyle-Width="15%"/>
                <asp:BoundField HeaderText="Active for" DataField="Activefacs" SortExpression="Activefacs" ItemStyle-Width="20%"/>
                <asp:BoundField HeaderText="Inactive for" DataField="Inactivefacs" SortExpression="Inactivefacs"  ItemStyle-Width="20%"/>
                 <asp:BoundField HeaderText="Phonebook Status" DataField="PHONEBOOK_STATUS" SortExpression="PHONEBOOK_STATUS" ItemStyle-Width="10%" />
            </Columns>
           </asp:GridView>
               <asp:SqlDataSource id="SDSLCAW" DataSourceMode="DataSet" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>"
                   SelectCommand="SELECT DISTINCT F.Worker,   F.Worker_Id ,  TO_CHAR(Trainsafe.Get_Last_Session_Date(Slac_Id,'131'),'mm/dd/yyyy') C131,  TO_CHAR(Trainsafe.Get_Last_Session_Date(Slac_Id,'253'),'mm/dd/yyyy') C253,
                            TO_CHAR(Trainsafe.Get_Last_Session_Date(Slac_Id,'253PRA'),'mm/dd/yyyy') C253pra,   (Select Listagg(Facility_Name,',  ') Within Group (Order By Facility_Name) Cnt
                                From Lst_facility f join lst_worker_facility_map m on f.facility_id = m.facility_id
                                Where f.Is_Active = 'Y' and m.is_active= 'Y' and m.worker_id=F.Worker_Id And m.Status_Id=5 and  m.worker_type_id = 8) Activefacs,
                                (Select Listagg(Facility_Name,',  ') Within Group (Order By Facility_Name) Cnt From Lst_facility f
                                join lst_worker_facility_map m on f.facility_id = m.facility_id
                                Where f.Is_Active = 'Y' and m.is_active= 'Y' and m.worker_id=F.Worker_Id And m.Status_Id=6 and  m.worker_type_id = 8)  Inactivefacs,
                                  F.SLAC_EMAIL, F.PHONEBOOK_STATUS,F.PREFERRED_EMAIL                             
                                FROM Vw_Lst_Worker_Facility F WHERE F.WORKER_ID IN   (SELECT WORKER_ID FROM LST_WORKER WHERE STATUS_ID IN (5,6) AND IS_ACTIVE ='Y' )
                                AND F.WORKER_ID IN   (SELECT WORKER_ID  FROM LST_WORKER_FACILITY_MAP  WHERE IS_ACTIVE    ='Y' AND STATUS_ID IN (5,6)  AND worker_type_id = 8) ORDER BY WORKER"
                   FilterExpression= "WORKER like '%{0}%' ">
                    <FilterParameters>
                            <asp:ControlParameter Name="WORKER" ControlID="TxtSearchLCA" PropertyName="Text" ConvertEmptyStringToNull="false" />
                    </FilterParameters>
              </asp:SqlDataSource>
          </div>
                </asp:Panel>
    </div>
    <div class="row" id="DivSLSOSummary" runat="server" visible="false">
        <asp:Panel ID="PnlSLSO" DefaultButton="CmdSearchSLSO" runat="server">
           <div class="row">
               <div class="col-sm-2"><asp:ImageButton ID="ImgBtnSLSO" runat="server" ImageUrl="~/Images/ExportToExcel.gif"  OnClick="ImgBtnSLSO_Click"  ToolTip="Tip: Please use 'Save and Open' option to view the contents of the file" /></div>
               <div class="col-sm-10">
                   Search SLSO: <asp:TextBox ID="TxtSearchSLSO" runat="server"></asp:TextBox> &nbsp;&nbsp;<asp:Button ID="CmdSearchSLSO" Text="Search" runat="server" CssClass="btn btn-primary" OnClick="CmdSearchSLSO_Click" ClientIDMode="Static"  />
                   &nbsp;&nbsp; #SLSO - <%= SLSOCount %>
                </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                 <asp:GridView ID="GVUser" runat="server" SkinID="gridviewSkin" AutoGenerateColumns="false" EmptyDataText ="No User found" PageSize="15" CellPadding="5" CellSpacing="5" Width="100%" 
                        ViewStateMode="Enabled" AllowPaging="true" OnPageIndexChanging="GVUser_PageIndexChanging" AllowSorting="true"  
                         OnRowCreated="GVUser_RowCreated"  OnSorting="GVUser_Sorting">
                        <Columns>
                            <asp:BoundField DataField="USERNAME" HeaderText="Name" ItemStyle-Width="19%"   SortExpression="USERNAME" />
                            <asp:BoundField DataField="USER_ROLE_ID" Visible="false" />
                            <asp:BoundField DataField="C130" HeaderText ="130 Valid thru" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}"  SortExpression="C130" ItemStyle-Width="10%"/>
                            <asp:BoundField DataField="C108" HeaderText ="108 Valid thru" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}"  SortExpression="C108" ItemStyle-Width="10%"/>
                            <asp:BoundField DataField="C108PRA" HeaderText ="108PRA Completed on" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}"  SortExpression="C108PRA" ItemStyle-Width="14%"/>
                            <asp:BoundField DataField="C157" HeaderText ="157 Valid thru" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}"  SortExpression="C157" ItemStyle-Width="11%"/>
                            <asp:TemplateField HeaderText="Facility with corresponding Role" ItemStyle-Width="36%">
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
                            <asp:BoundField DataField="STATUS" HeaderText ="Phonebook Status" SortExpression="STATUS" />                     
                        </Columns>
                        
                    </asp:GridView>

            </div>
      </asp:Panel>             
    </div>
    <div class="row" id="DivFacSummary" runat="server" visible="false">
        <asp:Panel ID="PnlFac" DefaultButton="CmdSearchFac" runat="server">
       <div class="row">
               <div class="col-sm-2"><asp:ImageButton ID="ImgBtnFac" runat="server" ImageUrl="~/Images/ExportToExcel.gif"  OnClick="ImgBtnFac_Click"  ToolTip="Tip: Please use 'Save and Open' option to view the contents of the file"/></div>
               <div class="col-sm-10">
                   Search Facility: <asp:TextBox ID="TxtSearchFac" runat="server"></asp:TextBox> &nbsp;&nbsp;<asp:Button ID="CmdSearchFac" Text="Search" runat="server" CssClass="btn btn-primary" OnClick="CmdSearchFac_Click" ClientIDMode="Static"  />
                   &nbsp;&nbsp; #Facility - <%= FacCount %>
                </div>
            </div>
            <div class="row">&nbsp;</div>
            <div class="row">
                 <asp:GridView ID="GvFacility" runat="server" SkinID="gridviewSkin" AutoGenerateColumns="false" EmptyDataText ="No Facility found" PageSize="15" CellPadding="5" CellSpacing="5" Width="100%" 
                        ViewStateMode="Enabled" AllowPaging="true"  AllowSorting="true"  
                          DataKeyNames="FACILITY_ID" DataSourceID="SDSFacSum" >
                        <Columns>
                                <asp:BoundField DataField="FACILITY_ID" Visible="false" SortExpression="FACILITY_ID" />
                                 <asp:HyperLinkField DataNavigateUrlFields="FACILITY_ID"  DataNavigateUrlFormatString="Facility.aspx?mode=view&id={0}&dd=3" SortExpression="FACILITY_NAME"
                                              HeaderText="Facility" DataTextField="FACILITY_NAME"  ItemStyle-width="17%"/> 
                                <asp:BoundField DataField="BLDG" HeaderText="Bldg"  SortExpression="BLDG" ItemStyle-Width ="7%"/>
                                <asp:BoundField DataField="ROOM" HeaderText="Room" SortExpression="ROOM" ItemStyle-Width ="6%"/>
                                <asp:BoundField DataField="SLSONAME" HeaderText="SLSO" SortExpression="SLSONAME"  ItemStyle-Width ="13%"/>
                                <asp:BoundField DataField="COSLSONAME" HeaderText="Co SLSO" SortExpression="COSLSONAME" ItemStyle-Width ="12%"/>
                                <asp:BoundField DataField="ACTSLSONAME" HeaderText="Acting SLSO" SortExpression="ACTSLSONAME" ItemStyle-Width ="10%" />
                                <asp:BoundField DataField="ActiveQLOs" HeaderText="#Active QLOs" SortExpression="ActiveQLOs" ItemStyle-Width ="10%" />
                                <asp:BoundField DataField="ActiveLCAw" HeaderText="#Active LCA workers" SortExpression="ActiveLCAw" ItemStyle-Width ="10%" />
                                <asp:BoundField DataField="APPROVAL_EXPIRY_DATE" HeaderText="Expiry Date" DataFormatString="{0:d-MMM-yy}" SortExpression="APPROVAL_EXPIRY_DATE" ItemStyle-Width ="10%" />        
                        </Columns>
                        
                    </asp:GridView>
                  <asp:SqlDataSource id="SDSFacSum" DataSourceMode="DataSet" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>"
                   SelectCommand=" SELECT F.FACILITY_ID, F.FACILITY_NAME, F.BLDG, F.ROOM, F.FACILITY_WEBPAGE, F.SOP_REVISED_DATE, F.APPROVAL_EXPIRY_DATE,
                             P1.Employee_Name AS Slsoname, P2.Employee_Name As Actslsoname, P3.Employee_Name As Coslsoname, (Select Count(*) From Lst_Worker_Facility_Map Where Worker_Type_Id = 7 And Status_Id=5 And Is_Active='Y' And Facility_Id = F.Facility_Id) Activeqlos,
                            (Select count(*) from Lst_Worker_Facility_Map where worker_type_id = 8 and status_id=5 and Is_active='Y' and facility_id = F.Facility_Id) ActiveLCAw
                            FROM Lst_Facility F,Lst_User_Roles R,  Lst_User_Roles R1,   Lst_User_Roles R2, Vw_People_Current P1, Vw_People_Current P2, Vw_People_Current P3
                             WHERE F.Slso      = R.User_Role_Id(+)  AND F.Acting_Slso = R1.User_Role_Id (+)  AND F.Co_Slso1    = R2. User_Role_Id(+)  AND R.Slac_Id     = P1.Employee_Id(+)  AND R1.Slac_Id    = P2.Employee_Id (+)
                            And R2.Slac_Id    = P3.Employee_Id (+)   AND F.IS_ACTIVE   ='Y' ORDER BY FACILITY_NAME"
                   FilterExpression= "FACILITY_NAME like '%{0}%' ">
                    <FilterParameters>
                            <asp:ControlParameter Name="FACILITY_NAME" ControlID="TxtSearchFac" PropertyName="Text" ConvertEmptyStringToNull="false" />
                    </FilterParameters>
              </asp:SqlDataSource>

               
            </div>
         </asp:Panel>          
    </div>
    <div class="row" id="DivESHCoSum" runat="server" visible="false">
        <asp:Panel ID="PnlCoord" runat="server" DefaultButton="BtnSearchESHC">
            <div class="row">
                <div class="col-sm-2"><asp:ImageButton ID="ImgBtnESHC" runat="server" ImageUrl="~/Images/ExportToExcel.gif" OnClick="ImgBtnESHC_Click" ToolTip="Tip: Please use 'Save and Open' option to view the contents of the file" /></div>
                 <div class="col-sm-10">
                     Search ESH Coordinator: <asp:TextBox ID="TxtSearchESHC" runat="server" ></asp:TextBox> &nbsp;&nbsp;<asp:Button ID="BtnSearchESHC" Text="Search" runat="server" CssClass="btn btn-primary" OnClick="BtnSearchESHC_Click" ClientIDMode="Static"  />
                   &nbsp;&nbsp; #ESH Co-ordinators - <%= ESHCCount %>
                 </div>
            </div>
             <div class="row">&nbsp;</div>
             <div class="row">
                 <asp:GridView ID="GVESHC" runat="server" SkinID="gridviewSkin" AutoGenerateColumns="false" Width="80%"
                      DataKeyNames="SLAC_ID" DataSourceID="SDSEsh"   EmptyDataText="No ESH Coordinators found" PageSize="15" AllowPaging="true">
                     <Columns>
                         <asp:BoundField DataField="SLAC_ID" Visible="false" />
                         <asp:BoundField DataField="ESH_COORDINATOR" HeaderText ="Esh Co-ordinator" ItemStyle-Width="25%" />
                         <asp:BoundField DataField="Facilities_Associated" HeaderText="Facilities Associated" ItemStyle-Width="50%" />
                         <asp:BoundField DataField="phonebook_status" HeaderText ="Phonebook Status" ItemStyle-Width="25%" />
                     </Columns>
                 </asp:GridView>
                 <asp:SqlDataSource ID="SDSEsh" DataSourceMode="DataSet" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>" SelectCommand="Select * from (select distinct m.esh_coord as slac_id  , pc.name as ESH_COORDINATOR, pc.maildisp as email,
(select Listagg(l.facility_name,',  ') within Group (order by l.facility_name)
from lst_facility l where l.esh_coord = m.esh_coord and l.is_active='Y') Facilities_Associated,
                     initcap(pc.gonet) as phonebook_status
from lst_facility m 
left join persons.person pc on m.esh_coord = pc.key
where m.is_active='Y' and m.esh_coord is not null) order by ESH_COORDINATOR" FilterExpression ="ESH_COORDINATOR LIKE '{0}%'">
                     <FilterParameters>
                            <asp:ControlParameter Name="ESH_COORDINATOR" ControlID="TxtSearchESHC" PropertyName="Text" />
                     </FilterParameters>
                 </asp:SqlDataSource>
               </div>
        </asp:Panel>
    </div>

     <div class="row" id="DivPM" runat="server" visible="false">
        <asp:Panel ID="PnlPM" runat="server" DefaultButton="BtnSearchPM">
            <div class="row">
                <div class="col-sm-2"><asp:ImageButton ID="ImgBtnPM" runat="server" ImageUrl="~/Images/ExportToExcel.gif" OnClick="ImgBtnPM_Click" ToolTip="Tip: Please use 'Save and Open' option to view the contents of the file" /></div>
                 <div class="col-sm-10">
                     Search Program Manager: <asp:TextBox ID="TxtSearchPM" runat="server" ></asp:TextBox> &nbsp;&nbsp;<asp:Button ID="BtnSearchPM" Text="Search" runat="server" CssClass="btn btn-primary" OnClick="BtnSearchPM_Click" ClientIDMode="Static"  />
                   &nbsp;&nbsp; #Program Managers - <%= PMCount %>
                 </div>
            </div>
             <div class="row">&nbsp;</div>
             <div class="row">
                 <asp:GridView ID="GVPM" runat="server" SkinID="gridviewSkin" AutoGenerateColumns="false" Width="80%"
                      DataKeyNames="SLAC_ID" DataSourceID="SDSPM"   EmptyDataText="No Program Managers found" PageSize="15" AllowPaging="true">
                     <Columns>
                         <asp:BoundField DataField="SLAC_ID" Visible="false" />
                         <asp:BoundField DataField="Program_Manager" HeaderText ="Program Manager"  ItemStyle-Width="25%"/>
                         <asp:BoundField DataField="FACILITIES_ASSOCIATED" HeaderText="Facilities Associated" ItemStyle-Width="50%" />
                         <asp:BoundField DataField ="Phonebook_Status" HeaderText="Phonebook Status" />
                     </Columns>
                 </asp:GridView>
                 <asp:SqlDataSource ID="SDSPM" DataSourceMode="DataSet" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>" SelectCommand="Select * from (select distinct m.prgmgr as slac_id, pc.name as Program_Manager,pc.maildisp as Email,
(select Listagg(l.facility_name,',  ') within Group (order by l.facility_name)
from lst_facility l where l.prgmgr = m.prgmgr and l.is_active='Y') Facilities_Associated,
                     initcap(pc.gonet) as phonebook_status
from lst_facility m 
left join persons.person pc on m.prgmgr = pc.key
where m.is_active='Y' and m.prgmgr is not null) order by Program_Manager" FilterExpression ="Program_Manager LIKE '{0}%'">
                     <FilterParameters>
                            <asp:ControlParameter Name="Program_Manager" ControlID="TxtSearchPM" PropertyName="Text" />
                     </FilterParameters>
                 </asp:SqlDataSource>
               </div>
        </asp:Panel>
    </div>

       <div class="row" id="DivSLSOOvd" runat="server" visible="false">
        <asp:Panel ID="PnlSLSOvd" runat="server" DefaultButton="BtnSLSOvd">
            <div class="row">
                <div class="col-sm-2"><asp:ImageButton ID="ImgBtnSLSOvd" runat="server" ImageUrl="~/Images/ExportToExcel.gif" OnClick="ImgBtnSLSOvd_Click" ToolTip="Tip: Please use 'Save and Open' option to view the contents of the file" /></div>
                 <div class="col-sm-10">
                     Search SLSO: <asp:TextBox ID="TxtSLSOvd" runat="server" ></asp:TextBox> &nbsp;&nbsp;<asp:Button ID="BtnSLSOvd" Text="Search" runat="server" CssClass="btn btn-primary" OnClick="BtnSLSOvd_Click" ClientIDMode="Static"  />
                   &nbsp;&nbsp; #SLSOs with 130 overdue - <%= SLSO130ovdCount %>
                 </div>
            </div>
             <div class="row">&nbsp;</div>
             <div class="row">
                 <asp:GridView ID="GVSLSOvd" runat="server" SkinID="gridviewSkin" AutoGenerateColumns="false" Width="80%"
                       DataSourceID="SDSSLSOvd"   EmptyDataText="No SLSO with 130 overdue found" PageSize="15" AllowPaging="true">
                     <Columns>
                         <asp:BoundField DataField="Slac_Id" Visible="false" />
                         <asp:BoundField DataField="SLSO" HeaderText ="SLSO" />
                         <asp:BoundField DataField="C130_LastCompleted" HeaderText="130 Last completed on"  HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" />
                         <asp:BoundField DataField="Phonebook_Status" HeaderText ="Phonebook Status" />
                      </Columns>
                 </asp:GridView>
                 <asp:SqlDataSource ID="SDSSLSOvd" DataSourceMode="DataSet" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" 
                     ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>" SelectCommand="SELECT  SLAC_ID,USERNAME as SLSO,
                     TO_CHAR((Trainsafe.Get_Last_Session_Date(SLAC_ID,'130')),'mm/dd/yyyy') C130_LastCompleted,STATUS as Phonebook_Status,Email  FROM VW_LST_USERS
                        WHERE ROLE_TYPE_ID=15 and Substr(Trainsafe.Get_Non_Sta_Trn_Status(SLAC_ID,'130'),1,3) = 'Ove'  order by SLSO" FilterExpression ="SLSO LIKE '{0}%'">
                     <FilterParameters>
                            <asp:ControlParameter Name="SLSO" ControlID="TxtSLSOvd" PropertyName="Text" />
                     </FilterParameters>
                 </asp:SqlDataSource>
               </div>
        </asp:Panel>
    </div>

     <div class="row" id="DivCourseovd" runat="server" visible="false">
        <asp:Panel ID="PnlCourseovd" runat="server" DefaultButton="BtnCourseovd">
            <div class="row">
                <div class="col-sm-2"><asp:ImageButton ID="ImgBtnCourseovd" runat="server" ImageUrl="~/Images/ExportToExcel.gif" OnClick="ImgBtnCourseovd_Click" ToolTip="Tip: Please use 'Save and Open' option to view the contents of the file" /></div>
                 <div class="col-sm-10">
                     Search Laser Workers: <asp:TextBox ID="TxtCourseovd" runat="server" ></asp:TextBox> &nbsp;&nbsp;<asp:Button ID="BtnCourseovd" Text="Search" runat="server" CssClass="btn btn-primary" OnClick="BtnCourseovd_Click" ClientIDMode="Static"  />
                   &nbsp;&nbsp; #Workers with 253 or 219R overdue - <%= CourseovdCount %>
                 </div>
            </div>
             <div class="row">&nbsp;</div>
             <div class="row">
                 <asp:GridView ID="GVCourseovd" runat="server" SkinID="gridviewSkin" AutoGenerateColumns="false" Width="80%"
                       DataSourceID="SDSCourseovd"   EmptyDataText="No workers with 253,219R overdue found" PageSize="15" AllowPaging="true" OnPageIndexChanging="GVCourseovd_PageIndexChanging">
                     <Columns>
                         <asp:BoundField DataField="SLAC_ID" Visible="false" />
                         <asp:BoundField DataField="WORKER" HeaderText ="Worker" />
                         <asp:BoundField DataField="COURSE" HeaderText="Course" />
                         <asp:BoundField DataField="DATE_LASTCOMPLETED" HeaderText="Course last completed on"  HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}" />
                         <asp:BoundField DataField="PHONEBOOK_STATUS" HeaderText="Phonebook Status" />
                      </Columns>
                 </asp:GridView>
                 <asp:SqlDataSource ID="SDSCourseovd" DataSourceMode="DataSet" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" 
                     ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>"   FilterExpression ="WORKER LIKE '{0}%' ">
                     <FilterParameters>
                            <asp:ControlParameter Name="WORKER" ControlID="TxtCourseovd" PropertyName="Text" />
                     </FilterParameters>
                 </asp:SqlDataSource>
               </div>
        </asp:Panel>
    </div>
    
    <div class="row" id="DivFacReq" runat="server" visible="false">
        <asp:Panel ID="PnlFacReq" runat="server" DefaultButton="BtnSearchFacreq">
            <div class ="row">
                    <div class="col-sm-2"><asp:ImageButton ID="ImgBtnFacreq" runat="server" ImageUrl="~/Images/ExportToExcel.gif" OnClick="ImgBtnFacreq_Click" ToolTip="Tip: Please use 'Save and Open' option to view the contents of the file" /></div>
                    <div class="col-sm-10">
                        Search Facility: <asp:TextBox ID="TxtSearchFacreq" runat="server"></asp:TextBox>&nbsp;
                       Approval Status <asp:DropDownList ID="DdlAppStat" runat="server" AppendDataBoundItems="true" DataSourceID="SDSAppstat" DataTextField="LOOKUP_DESC" DataValueField="LOOKUP_ID">
                            <asp:ListItem Text="All" Value="0" Selected="True"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="BtnSearchFacreq" Text="Search" runat="server" CssClass="btn btn-primary" OnClick="BtnSearchFacreq_Click" ClientIDMode="Static"  />
                        &nbsp; #Facility Operation Approval Requests - <%= FacReqCount %>
                    </div>
            </div>
              <div class="row">&nbsp;</div>
            <div class="row">
                <asp:GridView ID="GVFacReq" runat="server" SkinID="gridviewSkin"  AutoGenerateColumns="false" Width="80%"
                     DataKeyNames="FAC_REQUEST_ID" DataSourceID="SDSFACREQ" EmptyDataText="No Facility Approval requests found" PageSize="15" AllowPaging="true"
                     AllowSorting="true">
                     <Columns>
                          <asp:HyperLinkField DataNavigateUrlFields="FAC_REQUEST_ID"  DataNavigateUrlFormatString="FacilityApprovalRequest.aspx?objid={0}" SortExpression="FAC_REQUEST_ID"
                                              HeaderText="Request Id" DataTextField="FAC_REQUEST_ID"  /> 
                          <asp:HyperLinkField DataNavigateUrlFields="FACILITY_ID"  DataNavigateUrlFormatString="Facility.aspx?mode=view&id={0}&dd=8" SortExpression="FACILITY_NAME"
                                              HeaderText="Facility" DataTextField="FACILITY_NAME" /> 
                         <asp:BoundField DataField="DATE_REQUESTED" HeaderText="Date Requested"  SortExpression="DATE_REQUESTED" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}"/>
                         <asp:BoundField DataField="APPROVAL_STATUS" HeaderText="Approval Status" SortExpression="APPROVAL_STATUS" />
                         <asp:BoundField DataField="APPROVED_DECLINED_DATE" HeaderText="Approved/Declined Date" SortExpression="APPROVED_DECLINED_DATE" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}"/>
                         <asp:BoundField DataField="APPROVAL_EXPIRY_DATE" HeaderText="Approval Expiry Date" SortExpression="APPROVAL_EXPIRY_DATE" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}"/>
                     </Columns>
                </asp:GridView>
                  <asp:SqlDataSource ID="SDSFACREQ" DataSourceMode="DataSet" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>"
                       SelectCommand="SELECT FR.FAC_REQUEST_ID, FR.FACILITY_ID,FR.STATUS_ID, DECODE(FR.STATUS_ID, 3, TO_CHAR(FR.LSO_NEW_FACDATE,'mm/dd/yyyy'),NULL) AS APPROVAL_EXPIRY_DATE,  TO_CHAR(FR.CREATED_ON,'mm/dd/yyyy') AS DATE_REQUESTED,
F.FACILITY_NAME, L.LOOKUP_DESC AS APPROVAL_STATUS, DECODE(FR.STATUS_ID, 2, NULL,to_Char(FR.MODIFIED_ON,'mm/dd/yyyy')) AS APPROVED_DECLINED_DATE
FROM LST_FACILITY_REQUEST FR
LEFT JOIN LST_FACILITY F ON FR.FACILITY_ID = F.FACILITY_ID
LEFT JOIN LST_LOOKUP L ON FR.STATUS_ID = L.LOOKUP_ID WHERE L.LOOKUP_GROUP = 'Status'
                     AND FR.IS_ACTIVE='Y'
                       order by FR.FAC_REQUEST_ID DESC" 
                      FilterExpression ="FACILITY_NAME LIKE '{0}%'  AND (( '{1}' = 2 and STATUS_ID =  '{1}')
                      or ('{1}'=3 and STATUS_ID='{1}') or ( '{1}'=4 and STATUS_ID= '{1}') or
                      ('{1}'=0 and STATUS_ID IN (2,3,4)))">
                     <FilterParameters>
                            <asp:ControlParameter Name="FACILITY_NAME" ControlID="TxtSearchFacreq" PropertyName="Text"  ConvertEmptyStringToNull="false" />
                            <asp:ControlParameter Name="STATUS_ID" ControlID="DdlAppStat" PropertyName="SelectedValue" DefaultValue="0" /> 
                     </FilterParameters>
        
                 </asp:SqlDataSource>
                 <asp:SqlDataSource ID="SDSAppstat" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>" SelectCommand="select lookup_id, lookup_desc from lst_lookup
where is_active='Y' and lookup_group='Status' and lookup_id in (2,3,4) order by LOOKUP_DESC"></asp:SqlDataSource>
            </div>
        </asp:Panel>
    </div>

   <div class="row" id="DivWorkerReq" runat="server" visible="false">
       <asp:Panel ID="PnlWorkerReq" runat="server" DefaultButton="BtnSearchWorkerReq">
           <div class="row">
               <div class="col-sm-2">
                   <asp:ImageButton ID="ImgBtnWorkerReq" runat="server" ImageUrl="~/Images/ExportToExcel.gif" OnClick="ImgBtnWorkerReq_Click" ToolTip="Tip: Please use 'Save and Open' option to view the contents of the file" />
               </div>
                <div class="col-sm-10">
                        Search Worker: <asp:TextBox ID="TxtSearchWorkerReq" runat="server"></asp:TextBox>&nbsp;
                       Approval Status <asp:DropDownList ID="DdlWRAppStat" runat="server" AppendDataBoundItems="true" DataSourceID="SDSWFAppstat" DataTextField="LOOKUP_DESC" DataValueField="LOOKUP_ID">
                            <asp:ListItem Text="All" Value="0" Selected="True"></asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button ID="BtnSearchWorkerReq" Text="Search" runat="server" CssClass="btn btn-primary" OnClick="BtnSearchWorkerReq_Click" ClientIDMode="Static"  />
                        &nbsp; #Worker Approval Requests - <%= WorkerReqCount %>
                </div>
           </div>
           <div class="row">&nbsp;</div>
           <div class="row">
               <asp:GridView ID="GVWorkerReq" runat="server" SkinID="gridviewSkin" AutoGenerateColumns="false" Width="100%"
                    DataKeyNames="REQUEST_ID" DataSourceID="SDSWorkerReq" EmptyDataText="No Worker Approval request found" PageSize="15"
                    AllowPaging="true" AllowSorting="true"  OnRowDataBound="GVWorkerReq_RowDataBound">
                     <Columns>
                          <asp:HyperLinkField DataNavigateUrlFields="REQUEST_ID"  DataNavigateUrlFormatString="Approval.aspx?objid={0}" SortExpression="REQUEST_ID"
                                              HeaderText="Request Id" DataTextField="REQUEST_ID"  ItemStyle-Width="8%" /> 
                         <asp:HyperLinkField DataNavigateUrlFields="WORKER_ID"  DataNavigateUrlFormatString="WorkerEntry.aspx?mode=view&id={0}&&dd=9" SortExpression="WORKER_NAME"
                                              HeaderText="Worker" DataTextField="WORKER_NAME"  ItemStyle-Width="20%"  /> 
                            <asp:TemplateField HeaderText="Facility" SortExpression="FACILITY_NAME"  ItemStyle-Width="25%"  >
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlkFac" runat="server"  NavigateUrl='<%# "Facility.aspx?mode=view&dd=9&id=" + Eval("FACILITY_ID") %>'  Text='<%#Eval("FACILITY_NAME")%>'></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                       
                         <asp:BoundField DataField="WORKER_TYPE" HeaderText="Worker Type"  SortExpression="WORKER_TYPE"  ItemStyle-Width="7%" />
                         <asp:BoundField DataField="DATE_REQUESTED" HeaderText="Date Requested"  SortExpression="DATE_REQUESTED" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}"  ItemStyle-Width="11%" />
                         <asp:BoundField DataField="APPROVAL_STATUS" HeaderText="Approval Status" SortExpression="APPROVAL_STATUS"  ItemStyle-Width="12%" />
                         <asp:BoundField DataField="APPROVED_DECLINED_DATE" HeaderText="Approved/Declined Date" SortExpression="APPROVED_DECLINED_DATE" HtmlEncode="false" DataFormatString="{0:MM/dd/yyyy}"  ItemStyle-Width="17%" />                        
                     </Columns>
               </asp:GridView>
               <asp:SqlDataSource ID="SDSWorkerReq" DataSourceMode="DataSet" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>"
                   SelectCommand="SELECT * FROM (SELECT  WF.MAP_ID AS REQUEST_ID, WF.WORKER_ID, PC.NAME AS WORKER_NAME,WF.FACILITY_ID, 
DECODE(F.FACILITY_NAME,NULL,'None',F.FACILITY_NAME) as FACILITY_NAME, WF.STATUS_ID , L.LOOKUP_DESC AS WORKER_TYPE,TO_CHAR(WF.CREATED_ON,'mm/dd/yyyy') AS DATE_REQUESTED, 
DECODE(L1.LOOKUP_DESC,'Active','Approved','Inactive','Approved',L1.LOOKUP_DESC ) AS APPROVAL_STATUS, 
DECODE(WF.STATUS_ID,2,NULL,(SELECT TO_CHAR(MAX(A.APPROVED_ON),'mm/dd/yyyy') FROM LST_APPROVAL_WORKFLOW A
WHERE A.MAP_ID = WF.MAP_ID)) AS APPROVED_DECLINED_DATE FROM LST_WORKER_FACILITY_MAP WF INNER JOIN LST_APPROVAL_WORKFLOW AW
ON WF.MAP_ID = AW.MAP_ID LEFT JOIN LST_WORKER W ON WF.WORKER_ID = W.WORKER_ID LEFT JOIN PERSONS.PERSON PC ON W.SLAC_ID = PC.KEY
LEFT JOIN LST_FACILITY F ON WF.FACILITY_ID = F.FACILITY_ID LEFT JOIN LST_LOOKUP L ON WF.WORKER_TYPE_ID = L.LOOKUP_ID 
LEFT JOIN LST_LOOKUP L1 ON WF.STATUS_ID = L1.LOOKUP_ID  WHERE L.LOOKUP_GROUP='WorkerType' AND L1.LOOKUP_GROUP = 'Status'  AND WF.IS_aCTIVE='Y' 
UNION SELECT  WF.MAP_ID AS REQUEST_ID, WF.WORKER_ID, PC.NAME AS WORKER_NAME,WF.FACILITY_ID, DECODE( F.FACILITY_NAME,NULL,'None',F.FACILITY_NAME) AS FACILITY_NAME, WF.STATUS_ID , L.LOOKUP_DESC AS WORKER_TYPE,
TO_CHAR(WF.CREATED_ON,'mm/dd/yyyy') AS DATE_REQUESTED, DECODE(L1.LOOKUP_DESC,'Active','Approved','Inactive','Approved',L1.LOOKUP_DESC ) AS APPROVAL_STATUS, 
DECODE(WF.STATUS_ID,2,NULL,(SELECT TO_CHAR(MAX(A.APPROVED_ON),'mm/dd/yyyy') FROM LST_APPROVAL_WORKFLOW A WHERE A.MAP_ID = WF.MAP_ID)) AS APPROVED_DECLINED_DATE
FROM LST_WORKER_FACILITY_MAP WF LEFT JOIN LST_WORKER W ON WF.WORKER_ID = W.WORKER_ID  LEFT JOIN PERSONS.PERSON PC ON W.SLAC_ID = PC.KEY
LEFT JOIN LST_FACILITY F ON WF.FACILITY_ID = F.FACILITY_ID LEFT JOIN LST_LOOKUP L ON WF.WORKER_TYPE_ID = L.LOOKUP_ID 
LEFT JOIN LST_LOOKUP L1 ON WF.STATUS_ID = L1.LOOKUP_ID WHERE L.LOOKUP_GROUP='WorkerType' AND L1.LOOKUP_GROUP = 'Status'  AND WF.IS_aCTIVE='Y' 
AND WF.MAP_ID NOT IN (SELECT MAP_ID FROM LST_APPROVAL_WORKFLOW) AND WF.FACILITY_ID IS NOT NULL) ORDER BY REQUEST_ID DESC"
                    FilterExpression="WORKER_NAME LIKE '{0}%'  AND (( '{1}' = 3 and STATUS_ID IN (5,6))
                      or ('{1}'=2 and STATUS_ID='{1}') or ( '{1}'=4 and STATUS_ID= '{1}') or
                      ('{1}'=0 and STATUS_ID IN (2,4,5,6)))">
                     <FilterParameters>
                            <asp:ControlParameter Name="WORKER_NAME" ControlID="TxtSearchWorkerReq" PropertyName="Text"  ConvertEmptyStringToNull="false" />
                            <asp:ControlParameter Name="STATUS_ID" ControlID="DdlWRAppStat" PropertyName="SelectedValue" DefaultValue="0" /> 
                     </FilterParameters>
                    </asp:SqlDataSource>
               <asp:SqlDataSource ID="SDSWFAppstat" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>" SelectCommand="select lookup_id, lookup_desc from lst_lookup
where is_active='Y' and lookup_group='Status' and lookup_id in (2,3,4) order by LOOKUP_DESC"></asp:SqlDataSource>
           </div>

       </asp:Panel>
   </div>

    <div class="row" id="DivAdminSvr" runat="server" visible="false">
        <asp:Panel ID="PnlAdminSvr" runat="server" DefaultButton="BtnSearchSvr">
            <div class="row">
                 <div class="col-sm-2">
                   <asp:ImageButton ID="ImgBtnAdminsvr" runat="server" ImageUrl="~/Images/ExportToExcel.gif" OnClick="ImgBtnAdminsvr_Click" ToolTip="Tip: Please use 'Save and Open' option to view the contents of the file" />
               </div>
                <div class="col-sm-10">
                        Search Admin Supervisor: <asp:TextBox ID="TxtSearchSvr" runat="server"></asp:TextBox>&nbsp;
                        <asp:Button ID="BtnSearchSvr" Text="Search" runat="server" CssClass="btn btn-primary" OnClick="BtnSearchSvr_Click" ClientIDMode="Static"  />
                        &nbsp; #Admin Supervisors - <%= AdminSvrCount %>
                </div>
            </div>
             <div class="row">&nbsp;</div>
            <div class="row">
                 <asp:GridView ID="GVAdminSvr" runat="server" SkinID="gridviewSkin" AutoGenerateColumns="false" Width="80%"
                    DataKeyNames="APPROVER_ID" DataSourceID="SDSAdminSvr" EmptyDataText="No Admin Supervisor approved requests found" 
                     PageSize="15"
                    AllowPaging="true" AllowSorting="true">
                     <Columns>                         
                         <asp:BoundField DataField="ADMIN_SUPERVISOR" HeaderText="Admin Supervisor"  SortExpression="ADMIN_SUPERVISOR" ItemStyle-Width="15%" />
                         <asp:BoundField DataField="WORKER" HeaderText="Worker"  SortExpression="WORKER" ItemStyle-Width="15%" />
                         <asp:BoundField DataField="Facilities_QLO" HeaderText="Facilities with QLO Affiliation"  SortExpression="Facilities_QLO"  ItemStyle-Width="35%" />
                         <asp:BoundField DataField="Facilities_LCA" HeaderText="Facilities with LCA Worker Affiliation"  SortExpression="Facilities_LCA"  ItemStyle-Width="35%" />
                     </Columns>
               </asp:GridView>
                 <asp:SqlDataSource ID="SDSAdminSvr" DataSourceMode="DataSet" runat="server" ConnectionString="<%$ ConnectionStrings:TCP_WEB %>" ProviderName="<%$ ConnectionStrings:TCP_WEB.ProviderName %>"
                           SelectCommand="SELECT DISTINCT AW.APPROVER_ID, PC.NAME AS ADMIN_SUPERVISOR, WFM.WORKER_ID, PC1.NAME AS WORKER,DECODE(PC.MAILDISP,NULL, PC.SID_EMAIL,PC.MAILDISP) AS SUPERVISOR_EMAIL,
        (select Listagg(decode(l.facility_name, null, 'Global QLO',l.facility_name),',  ') within Group (order by l.facility_name) from LST_APPROVAL_WORKFLOW FACW 
        LEFT JOIN LST_WORKER_FACILITY_MAP FACM ON FACW.MAP_ID = FACM.MAP_ID LEFT JOIN lst_facility l  ON FACM.FACILITY_ID = L.FACILITY_ID
        where FACM.WORKER_TYPE_ID=7 AND FACM.IS_ACTIVE='Y' AND FACM.STATUS_ID IN (5,6) AND  FACM.WORKER_ID = WFM.WORKER_ID  AND FACW.APPROVER_ID=AW.APPROVER_ID AND FACW.APPROVER_TYPE='ADMSVR') 
        Facilities_QLO, (select Listagg(decode(l.facility_name, null, 'Global LCA Worker', l.facility_name),',  ') within Group (order by l.facility_name) from  LST_APPROVAL_WORKFLOW FACW 
        LEFT JOIN LST_WORKER_FACILITY_MAP FACM ON FACW.MAP_ID = FACM.MAP_ID LEFT JOIN lst_facility l  ON FACM.FACILITY_ID = L.FACILITY_ID
        where FACM.WORKER_TYPE_ID=8 AND FACM.IS_ACTIVE='Y' AND FACM.STATUS_ID IN (5,6) AND FACM.WORKER_ID = WFM.WORKER_ID  AND FACW.APPROVER_ID=AW.APPROVER_ID AND FACW.APPROVER_TYPE='ADMSVR') 
        Facilities_LCA FROM LST_APPROVAL_WORKFLOW AW LEFT JOIN LST_WORKER_FACILITY_MAP WFM ON AW.MAP_ID = WFM.MAP_ID LEFT JOIN PERSONS.PERSON PC ON AW.APPROVER_ID = PC.KEY
        LEFT JOIN LST_WORKER W ON WFM.WORKER_ID= W.WORKER_ID LEFT JOIN PERSONS.PERSON PC1 ON W.SLAC_ID = PC1.KEY  WHERE
                    PC1.SUPERVISOR_ID = AW.APPROVER_ID AND AW.STATUS_ID=3 AND AW.APPROVER_TYPE='ADMSVR'
         AND WFM.STATUS_ID IN (5,6) AND WFM.IS_ACTIVE='Y' ORDER BY ADMIN_SUPERVISOR"
                    FilterExpression="ADMIN_SUPERVISOR LIKE '{0}%' ">
                     <FilterParameters>
                            <asp:ControlParameter Name="ADMIN_SUPERVISOR" ControlID="TxtSearchSvr" PropertyName="Text"  ConvertEmptyStringToNull="false" />                           
                     </FilterParameters>
                    </asp:SqlDataSource>
            </div>
        </asp:Panel>

    </div>

   </asp:Panel>
 

</asp:Content>
