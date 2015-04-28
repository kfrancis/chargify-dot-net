<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<%@ Register TagPrefix="uc" TagName="mainMenu" Src="~/controls/YamlMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Home
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
    <link href="css/screen/yaml.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <uc:mainMenu ID="mainMenu" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
    <h2>Welcome!</h2>
    <div class="info">
        <p>
            Welcome to the ASP.NET Chargify Demo.
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
            To begin this demo, either <a href="Login.aspx">Login</a> or start by <a href="Plans.aspx">selecting a plan</a> to subscribe to.
        </p>
        <p>
            - Thanks, Kori<br /><br />
            <span class="dimmed">If you have any issues, feel free to post them on the <a href="http://chargify.codeplex.com/">CodePlex Chargify project</a>.</span>
        </p>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <table class="full">
        <thead>
            <tr>
                <th scope="col" colspan="2">Site Options</th>
            </tr>
        </thead>
        <tr>
            <td style="width:48px;">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/onebit_39.png" Height="48px" Width="48px" />
            </td>
            <td>
                <h3>Plans and Pricing</h3>
                <p>Great products for your demo needs!</p>
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Plans.aspx">View our plans ...</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td style="width:48px;">
                <asp:Image ID="Image2" runat="server" ImageUrl="~/images/supportFP.gif" Height="48px" Width="48px" />
            </td>
            <td>
                <h3>Support</h3>
                <p>Something wrong? Get in touch with us ...</p>
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="#" onclick="alert('Just kidding!'); return false;">Contact Support ...</asp:HyperLink>
            </td>
        </tr>
    </table>
</asp:Content>