<%@ Page Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMainContent" runat="server">

    <table class="full">
        <thead>
            <tr>
                <th scope="col" colspan="2">Site Options</th>
            </tr>
        </thead>
        <tr>
            <td style="width:48px;">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/images/onebit_39.png" Height="48px" Width="48px" />
            </td>
            <td>
                <h3>Plans and Pricing</h3>
                <p>Great products for your demo needs!</p>
                <%= Html.ActionLink("View our plans ...", "Index", "Plans")%>
            </td>
        </tr>
        <tr>
            <td style="width:48px;">
                <asp:Image ID="Image2" runat="server" ImageUrl="~/Content/images/supportFP.gif" Height="48px" Width="48px" />
            </td>
            <td>
                <h3>Support</h3>
                <p>Something wrong? Get in touch with us ...</p>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="#">Contact Support ...</asp:HyperLink>
            </td>
        </tr>
    </table>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitleContent" runat="server">
    Home
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphMenu" runat="server">
    <div class="hlist">
	    <ul>
		    <li class="active"><strong>Home</strong></li>
		    <li><%= Html.ActionLink("Plans", "Index", "Plans")%></li>
            <li><%= Html.ActionLink("Login", "LogOn", "Account")%></li>
	    </ul>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="cphLeftContent" runat="server">
    <h2>Welcome!</h2>
    <div class="info">
        <p>
            Welcome to the ASP.NET MVC Chargify Demo.
            You can do the following things in this demo:
        </p>
        <ul>
            <li>Login as a sample user</li>
            <li>Create your own subscription</li>
            <li>Change/migrate current subscription</li>
            <li>Change/update credit card information</li>
            <li>View transaction list</li>
        </ul>
        <p>
            To begin this demo, either <%= Html.ActionLink("Login", "LogOn", "Account")%> or start by <%= Html.ActionLink("selecting a plan", "Index", "Plans")%> to subscribe to.
        </p>
        <p>
            - Thanks, Kori<br /><br />
            <span class="dimmed">If you have any issues, feel free to post them on the <a href="http://chargify.codeplex.com/">CodePlex Chargify project</a>.</span>
        </p>
    </div>
</asp:Content>