<%@ Page Title="Confirmation" Language="C#" MasterPageFile="~/LST.Master" AutoEventWireup="true" CodeBehind="ConfirmRequest.aspx.cs" Inherits="LST.ConfirmRequest" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %></h2>

    <div>
        <asp:PlaceHolder runat="server" ID="successPanel" ViewStateMode="Disabled" Visible="true">
            <p>
               <asp:Label ID="LblMsg" text="Your request has been submitted successfully. Email has been sent to respective approvers and you will be notified after the final approval." runat="server"></asp:Label>
            </p>
        </asp:PlaceHolder>
      
    </div>
</asp:Content>
