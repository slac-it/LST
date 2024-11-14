<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/LST.Master" StylesheetTheme="LSTTheme" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LST._Default" %>
<%@ Register Src="~/UserControls/SLSOLabSummary.ascx" TagName="SLSOSum" TagPrefix="UC" %>
<%@ Register Src="~/UserControls/UserLabSummary.ascx" TagName="UserSum" TagPrefix="UC1" %>
<%@ Register Src="~/UserControls/ApprovalRequestSummary.ascx" TagName="ReqSum" TagPrefix="UC2" %>
<%@ Register Src="~/UserControls/WorkerPendingRequestSummary.ascx" TagName="PendingSum" TagPrefix="UC3" %>
<%@ Register Src="~/UserControls/FacilityApprovalRequestSummary.ascx" TagName="FacreqSum" TagPrefix="UC4" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
     <script type="text/javascript" src="Scripts/jquery-1.10.2.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/LST.js"></script>
    <script type="text/javascript" src="Scripts/backbutton.js"></script>
    <script type="text/javascript">
        function RefreshGrid() {
            window.location.reload();
        }

    </script>
      <div class="jumbotron" style="text-align:center;"> 
          <p class="lead" >
              <b>Laser Safety: Database and Electronic Approvals</b> 
            <br /><br />
 Welcome to the web interface for accessing database information for laser personnel and laser facilities, and for electronic approvals for QLOs and LCA Workers.
   
          </p>     
       
    </div>
    <div class="row" style="text-align:center;" >
           <h4>You are currently logged in as: <b><%= UserName %></b></h4>
      </div>

    <div class="row">
        <br />
             To request QLO or LCA Worker approval for a SLAC laser facility, select the “Request Approval” webpage.
        <br />
        To access database information on SLAC’s laser personnel and laser facilities, select the “Reports” webpage

        </div>
    <div>&nbsp;</div>  
    <div class="row">
        Your current requests for QLO/LCA Worker Approval are:
        <UC3:PendingSum ID="UCPending1" runat="server" />
    </div>
    <div>&nbsp;</div>
    <div class="row" id="DivWorker" runat="server">
        Your current QLO/LCA Worker Status is:
        <UC1:UserSum ID="UCUserSum" runat="server" />
    </div>
     <div>&nbsp;</div>
    <div  class="row" id="DivSLSO" runat="server" visible="false">
        Your current SLSO responsibilities are:
        <UC:SLSOSum id="UCSLSOSum" runat="Server"></UC:SLSOSum>
    </div>
   
    <div class="row" id="DivSLSOApp" runat="server">    
         <UC4:FacReqSum ID="UCFacReqSum" runat="Server"></UC4:FacReqSum> 
        
          <UC2:ReqSum ID="UCReqSum" runat="server" />
   </div>
    
    <div class="row" id="DivLSO" runat="server">   
         <UC4:FacReqSum ID="UCFacReqSumLSO" runat="Server"></UC4:FacReqSum> 
         
         <UC2:ReqSum ID="UCReqSumLSO" runat="server" />
         
          
    </div>
   
    <div  class="row" id="DivAdmSVR" runat="server">
         <UC2:ReqSum ID="UCReqSumSVR" runat="server" />
    </div>
    
     <div class="row" id="DivPM" runat="server">
         <UC4:FacReqSum ID="UCFacReqSumPM" runat="Server"></UC4:FacReqSum>
     </div>
    
     <div class="row" id="DivCoord" runat="server">
         <UC4:FacReqSum ID="UCFacReqSumCoord" runat="Server"></UC4:FacReqSum>
     </div>
       
   <div id="dialogDesignate" style="display:none;">
                        <iframe id="modaldialogDesignate" width="100%" height="100%">
                           
                        </iframe>
       </div>
      
</asp:Content>
