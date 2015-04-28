<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="FileNotFound.aspx.vb" Inherits="FileNotFound" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    404 - File Not Found
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <div class="hlist">
	    <ul>
		    <li><a href="Default.aspx">Home</a></li>
		    <li><a href="Plans.aspx">Plans</a></li>
		    <li class="active"><strong>File Not Found</strong></li>
	    </ul>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <asp:Panel ID="Panel1" runat="server" CssClass="warning">
        <h1 class="title">We couldn't find what you were looking for ..</h1>
        <p>Looks like the page that you requested couldn't befound, but don't worry, we're on it!</p>
        <p>Please try again, if it still doesn't work - then email <a href="mailto:&#115;&#117;&#112;&#112;&#111;&#114;&#116;&#64;&#99;&#97;&#98;&#46;&#109;&#100;">me</a>.</p>
    </asp:Panel>
</asp:Content>