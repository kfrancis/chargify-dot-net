<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="ChangeSubscription.aspx.vb" Inherits="app_settings_ChangeSubscription" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Change Subscription
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
    <link href="../../css/screen/yaml.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <div class="hlist">
	    <ul>
		    <li><asp:LinkButton ID="logoutBtn" runat="server">Logout</asp:LinkButton></li>
		    <li><a href="../Dashboard.aspx">Dashboard</a></li>
		    <li class="active"><strong>Subscriptions</strong></li>
	    </ul>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
    <ul> 
        <li>
            <div class="floatbox">
                <asp:Image ID="Image3" runat="server" CssClass="float_left" ImageUrl="~/images/key.png" />
                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/app/settings/Account.aspx">Account Information</asp:HyperLink>
                <p>Summary, change password, edit billing</p>
            </div>
        </li>
        <li>
            <div class="floatbox">
                <asp:Image ID="Image1" runat="server" CssClass="float_left" ImageUrl="~/images/walletBig.gif" />
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/app/settings/BillingHistory.aspx">Billing History</asp:HyperLink>
                <p>Billing history, transactions</p>
            </div>
        </li>
        <li>
            <div class="floatbox">
                <asp:Image ID="Image2" runat="server" CssClass="float_left" ImageUrl="~/images/buy.png" />
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/app/settings/ChangeSubscription.aspx">Upgrade & Extend</asp:HyperLink>
                <p>More power, more features on demand</p>
            </div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <h2>Settings > Upgrade & Extend</h2>
    <asp:Panel ID="currentPlanPnl" runat="server" CssClass="important">
        You are using the <strong><asp:Literal ID="currentPlanLtr" runat="server"></asp:Literal></strong> plan.
    </asp:Panel>
    <asp:Repeater ID="addOnRpt" runat="server">
        <HeaderTemplate>
            <table class="full">
                <tr>
                    <th scope="col">Available Upgrades</th>
                    <th scope="col">Price</th>
                    <th scope="col">Status</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class='<%# GetSubscriptionStyle(IsSubscribed(Eval("Handle"))) %>'>
                <td>
                    <strong><%#Eval("Name")%></strong><br />
                    <p class="dimmed"><%#Eval("Description")%></p>
                </td>
                <td valign="top" style="vertical-align: top; width: 175px;">
                    <strong>$<%#Eval("Price")%> per <%# Eval("Interval") %> <%#Eval("IntervalUnit")%></strong>
                </td>
                <td valign="top" style="vertical-align: top; width: 75px;">
                    <asp:Button ID="enableBtn_Edit" runat="server" Text="Change" Width="75px" CommandName="ChangePlan" CssClass="button green small" CommandArgument='<%# Eval("Handle") %>' Visible='<%# Not IsSubscribed(Eval("Handle")) %>' OnClientClick="if(confirm('Are you sure you want to change plans? (Not prorated)')==false){return false;}" />
                    <asp:Button ID="enableBtn_Migrate" runat="server" Text="Migrate" Width="75px" CommandName="MigratePlan" CssClass="button green small" CommandArgument='<%# Eval("Handle") %>' Visible='<%# Not IsSubscribed(Eval("Handle")) %>' OnClientClick="if(confirm('Are you sure you want to migrate plans? (With proration)')==false){return false;}" style="margin-top: 2px;" />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
    <p class="dimmed">
        Prorating is a feature in Chargify, so you (the user) will be charged the difference based on time.
    </p>
    <asp:Panel ID="resultPnl" runat="server" CssClass="note" Visible="false">
        <asp:Literal ID="resultLtr" runat="server"></asp:Literal>
    </asp:Panel>
</asp:Content>