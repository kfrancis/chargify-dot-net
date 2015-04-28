<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="Dashboard.aspx.vb" Inherits="app_Dashboard" %>
<%@ MasterType VirtualPath="~/site.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Dashboard
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
    <link href="../css/screen/yaml.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">    
    <div class="hlist">
	    <ul>
		    <li><asp:LinkButton ID="logoutBtn" runat="server">Logout</asp:LinkButton></li>
		    <li class="active"><strong>Dashboard</strong></li>
		    <asp:Literal ID="settingsLtr" runat="server"><li><a href="settings/Account.aspx">Settings</a></li></asp:Literal>
            <asp:Literal ID="adminLnkLtr" runat="server"></asp:Literal>
	    </ul>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
    <asp:Panel ID="billingPnl" runat="server">
        <table class="full">
            <thead> 
                <tr><th scope="col" colspan="2">Account Details</th></tr> 
            </thead>
            <tr>
                <td align="right">Plan:</td>
                <td align="left"><asp:Literal ID="planNameLtr" runat="server"></asp:Literal></td>
            </tr> 
            <tr>
                <td align="right">Price:</td>
                <td align="left"><asp:Literal ID="pricePerLtr" runat="server"></asp:Literal></td>
            </tr> 
            <tr>
                <td align="right">Current Balance:</td>
                <td align="left"><asp:Literal ID="currentBalanceLtr" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <td align="right">Next Payment Date:</td>
                <td align="left"><asp:Literal ID="nextPaymentDateLtr" runat="server"></asp:Literal></td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="billingResultPnl" runat="server">
        <asp:Literal ID="billingResultLtr" runat="server" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <h2>Application</h2>
    <p>Here's the nifty application that you get to access now.</p>
    <hr />
    <div class="yform columnar">
        <fieldset>
            <legend>Usage Demo</legend>
            <p id="result" style="display: none;"><asp:Literal ID="usageResultLtr" runat="server"></asp:Literal></p>
            <asp:LinkButton ID="usageBtn" runat="server" CssClass="button green" Text="Click Me" /><br />&nbsp;
            <p class="dimmed">Clicking this button will add a single "Button Push" usage to your account.</p>
        </fieldset>
        <%--<fieldset>
            <legend>Component Demo</legend>
            <p id="P1" style="display: none;"><asp:Literal ID="Literal1" runat="server"></asp:Literal></p>
            <asp:LinkButton ID="componentInfoBtn" runat="server" CssClass="button green" Text="Click Me" /><br />&nbsp;
            <p class="dimmed">Blah blah</p>
        </fieldset>--%>
        <fieldset>
            <legend>On/Off Demo</legend>
            <p id="onOffResult" style="display: none;"><asp:Literal ID="onOffResultLtr" runat="server"></asp:Literal></p>
            <asp:LinkButton ID="onBtn" runat="server" CssClass="button green" Text="Turn Support ON" />
            <asp:LinkButton ID="offBtn" runat="server" CssClass="button green" Text="Turn Support OFF" /><br />&nbsp;
        </fieldset>
    </div>
    <script type="text/javascript">
        // Do a highlight effect to make the user aware they have changed something.
        function HighlightResult() {
            $("#result").show();
            $("#result").effect("highlight", {}, 3000);
        }

        function Highlight(id) {
            $("#" + id).show();
            $("#" + id).effect("highlight", {}, 3000);
        }
    </script>
</asp:Content>