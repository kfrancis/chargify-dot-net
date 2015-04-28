<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="AccountEnded.aspx.vb" Inherits="AccountEnded" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Account Removed
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <!-- This menu is static on purpose, since the Subscribe page should only appear when you are on this page. -->
    <div class="hlist">
	    <ul>
		    <li><a href="default.aspx">Home</a></li>
		    <li class="active"><strong>Account Cancelled</strong></li>
	    </ul>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <h2>Account Cancelled</h2>
    <p>We're sorry to see you go, if you ever change your mind, please don't hesitate to get in touch with us.</p>
</asp:Content>