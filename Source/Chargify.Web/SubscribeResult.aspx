<%@ Page Title="" Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="SubscribeResult.aspx.vb" Inherits="SubscribeResult" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <!-- This menu is static on purpose, since the Subscribe page should only appear when you are on this page. -->
    <div class="hlist">
	    <ul>
		    <li><a href="default.aspx">Home</a></li>
		    <li><a href="Login.aspx">Login</a></li>
		    <li class="active"><strong>Result</strong></li>
	    </ul>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
    <div class="info">
        <p>The user is now both a user of the system, as well as a subscribing customer on the chargify account associated with this site.</p>
        <p>The user can now log into this system and start using it, as well as see additional information about when they might be billed next.</p>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <asp:Panel ID="resultPnl" runat="server" CssClass="note">
        <h3><asp:Literal ID="resultTitleLtr" runat="server"></asp:Literal></h3>
        <p>
            <asp:Literal ID="resultTextLtr" runat="server"></asp:Literal>
        </p>
    </asp:Panel>
</asp:Content>