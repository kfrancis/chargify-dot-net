<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="BillingHistory.aspx.vb" Inherits="app_settings_BillingHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Billing History
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
    <link href="../../css/screen/yaml.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <div class="hlist">
	    <ul>
		    <li><asp:LinkButton ID="logoutBtn" runat="server">Logout</asp:LinkButton></li>
		    <li><a href="../Dashboard.aspx">Dashboard</a></li>
		    <li class="active"><strong>Billing History</strong></li>
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
    <h2>Settings > Billing History</h2>
    
    <asp:DropDownList ID="transactionTypeDDL" runat="server" AppendDataBoundItems="true" AutoPostBack="true">
        <asp:ListItem Text="All" Value="-1" />
    </asp:DropDownList>

    <asp:Repeater ID="transactionRpt" runat="server">
        <HeaderTemplate>
            <table class="full">
                <tr>
                    <th scope="col">Date</th>
                    <th scope="col">Product</th>
                    <th scope="col">Type</th>
                    <th scope="col">Memo</th>
                    <th scope="col">Amount</th>
                    <th scope="col">Balance</th>
                    <th scope="col">Status</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("CreatedAt")%></td>
                <td><%# GetProductName(Eval("ProductID"))%></td>
                <td><%# Eval("Type")%></td>
                <td><%# Eval("Memo")%></td>
                <td>$<%# Eval("Amount")%></td>
                <td>$<%# Eval("EndingBalance")%></td>
                <td><%# IsSuccessful(Eval("Success"))%></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</asp:Content>