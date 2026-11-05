<%@ Page Title="" Language="C#" MasterPageFile="~/main.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="aspnetframework48._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="text-align: center;">
        <br />
        <br />
        Hello world<br />
        <br />
        THIS IS JUST A DEMO<br />
        <br />
        <%=DateTime.UtcNow.ToString("O") %>
    </div>
</asp:Content>
