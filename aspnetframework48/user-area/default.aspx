<%@ Page Title="" Language="C#" MasterPageFile="~/main.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="aspnetframework48.user_area._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>User area</h2>
    <br />
    CLAIMS:<br />
    <br />
    <asp:DataGrid ID="DataGrid1" runat="server" CellPadding="4" Font-Size="Small"></asp:DataGrid>
    <br />
    <br />
</asp:Content>
