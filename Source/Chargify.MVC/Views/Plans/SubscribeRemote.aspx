<%@ Page Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage(Of Chargify.MVC.SubscribeRemoteModel)" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMainContent" runat="server">

    <h2>Subscribe Remote</h2>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitleContent" runat="server">
    Subscribe - Step 1
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphMenu" runat="server">
    <div class="hlist">
	    <ul>
		    <li><%= Html.ActionLink("Home", "Index", "Home")%></li>
		    <li><%= Html.ActionLink("Plans", "Index")%></li>
		    <li class="active"><strong>Subscribe</strong></li>
	    </ul>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="cphLeftContent" runat="server">
    <h2><asp:Literal ID="planNameLtr" runat="server"></asp:Literal> Plan</h2>
    <div class="info">
        <p>Please fill in the following information.</p>
        <p>When finished, please click '<strong>Continue ...</strong>' to be taken to the Chargify product signup page.</p>
    </div>
</asp:Content>