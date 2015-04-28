<%@ Control Language="vb" AutoEventWireup="false" CodeFile="YamlMenu.ascx.vb" Inherits="_YamlMenu" %>

<asp:Repeater runat="server" ID="rptMenu" DataSourceID="SiteMapDataSource1">
    <HeaderTemplate>
        <div class="hlist">
            <ul>
    </HeaderTemplate>
    <ItemTemplate>
                <li ID="listItem" runat="server">
                    <asp:HyperLink ID="hlNode" runat="server" NavigateUrl='<%# Eval("Url") %>'><%# Eval("Title") %></asp:HyperLink>
                </li>
    </ItemTemplate>
    <FooterTemplate>
            </ul>
        </div>
    </FooterTemplate>
</asp:Repeater>
<asp:SiteMapDataSource ID="SiteMapDataSource1" runat="server" ShowStartingNode="false" SiteMapProvider="DefaultSiteMap" />