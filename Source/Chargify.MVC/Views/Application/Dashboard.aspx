<%@ Page Title="" Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMainContent" runat="server">

    <h2>Dashboard</h2>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitleContent" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphMenu" runat="server">
    <div class="hlist">
	    <ul>
		    <li><%= Html.ActionLink("Logout", "LogOff", "Account")%></li>
		    <li class="active"><strong>Dashboard</strong></li>
		    <li><%= Html.ActionLink("Settings", "Account", "Settings")%></li>
	    </ul>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="cphLeftContent" runat="server">
</asp:Content>
