<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OJTWorkersMatrix.ascx.cs" Inherits="LST.UserControls.OJTWorkersMatrix" %>

 <script type="text/javascript">
 function RefreshGrid() {
     // giving page refresh warning msg... window.location.reload();
     window.location.href = window.location.href;
        }

    </script>


<asp:Panel ID="PnlGV" runat="server" GroupingText="OJT Summary for this Facility's laser workers" >
   <div style="display:inline-block">
       <asp:Label ID="LblInfo" runat="server" style="font-size:medium;font-style:italic" Text="OJT syllabus and refresher OJT syllabus are available on the "></asp:Label>
       <a href="" id="aWeb" runat="server" target="_blank">Facility Web Page.</a>
       &nbsp;&nbsp;&nbsp;<asp:LinkButton ID="LnkOJTFields" runat="server" Text="Click here to Add/Delete OJT Fields"  CssClass="nounload" ></asp:LinkButton>
   </div> 
    <div runat="server" id="divOJT">
        <img id="btnShow2" src="Images/expandbig.gif"   onclick="toggleme('divGdOJT','btnShow2','btnHide2'); return false;" />
        
        <img id="btnHide2" src="Images/collapsebig.gif" style="display:none;"  onclick="toggleme('divGdOJT','btnShow2','btnHide2'); return false;" />    
                <i><strong>Note:</strong> Click on the +/- to expand or collapse the list.</i>
    </div>   
    <div style="width:1150px; overflow:auto; display:none; " id="divGdOJT"  class="divCss">
         <asp:GridView ID="GVOJT" runat="server"   AutoGenerateColumns="false"  OnRowDataBound="GVOJT_RowDataBound" AutoGenerateEditButton="true" 
             OnRowCancelingEdit="GVOJT_RowCancelingEdit" OnRowEditing="GVOJT_RowEditing" OnRowUpdating="GVOJT_RowUpdating"
              EmptyDataText="OJT Matrix not found for this facility. Please make sure to add OJT Fields to the Facility using the link above"
             SkinID="gridviewSkin"  CssClass="nounload" HeaderStyle-CssClass="centerHeaderText" CellPadding="5" CellSpacing="5" Width="100%" >

    </asp:GridView>
        </div>
     <div id="dialog-ojt" class="nounload" style="display:none;">
    
        <asp:Label ID="LblMsgojt" runat="server"></asp:Label>
   
        </div> 
    </asp:Panel>