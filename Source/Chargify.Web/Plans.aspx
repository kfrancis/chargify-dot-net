<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="Plans.aspx.vb" Inherits="Plans" %>

<%@ Register TagPrefix="uc" TagName="mainMenu" Src="~/controls/YamlMenu.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Plans
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
    <link href="css/screen/yaml.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <uc:mainMenu ID="mainMenu" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
    <div class="info">
        <p>
            You can subscribe to any one of these plans in two different ways.
            You can subscribe:
        </p>
        <ul>
            <li><strong>Local</strong> - use API from this site</li>
            <li><strong>Remote</strong> - use the hosted page</li>
        </ul>
        <p>
            If you choose to signup <strong>locally</strong>, then you will be directed to a page 
            that will ask for both account and billing information in a single form. After completing, 
            you will be redirected to a 'result' page that will either be successful or not successful.
        </p>
        <p>
            If you choose to signup <strong>remotely</strong>, via the Chargify hosted sign up page - you
            will be asked for some information about your new account (such as login information) before
            being redirected to the hosted signup page. Once you complete the form on the hosted signup page,
            you will be redirected back to this site to the 'result' page that will either show you that the
            subscription was successful or not successful.            
        </p>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <table class="full">
        <thead>
            <tr>
                <th scope="col" colspan="2">Our Plans</th>
            </tr>
        </thead>
        <tr>
            <td style="width: 80px;">
                <asp:Image ID="Image8" runat="server" ImageUrl="~/images/Free.png" />
            </td>
            <td>
                <h3>Free Plan</h3>
                <p>Here's some info about our skimpy free plan.</p>
                <%-- Just remember, that at this point - you need to hard code the product handles in, since we're not dyanamically
                generating this table from what's in Chargify. --%>
                <asp:HyperLink ID="freeLnk" runat="server" NavigateUrl="~/Subscribe.aspx?plan=free">Subscribe (Local)</asp:HyperLink> | 
                <asp:HyperLink ID="freeRemoteLnk" runat="server" NavigateUrl="~/Subscribe_Step1.aspx?plan=free">Subscribe (Remote)</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td style="width: 80px;">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/Basic.png" />
            </td>
            <td>
                <h3>Basic Plan</h3>
                <p>Here's some info about the basic plan.</p>
                <%-- Just remember, that at this point - you need to hard code the product handles in, since we're not dyanamically
                generating this table from what's in Chargify. --%>
                <asp:HyperLink ID="basicLnk" runat="server" NavigateUrl="~/Subscribe.aspx?plan=basic">Subscribe (Local)</asp:HyperLink> | 
                <asp:HyperLink ID="basicRemoteLnk" runat="server" NavigateUrl="~/Subscribe_Step1.aspx?plan=basic">Subscribe (Remote)</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Image ID="Image2" runat="server" ImageUrl="~/images/Plus.png" />
            </td>
            <td>
                <h3>Plus Plan</h3>
                <p>Here's some info about the plus plan.</p>
                <asp:HyperLink ID="plusLnk" runat="server" NavigateUrl="~/Subscribe.aspx?plan=plus">Subscribe (Local)</asp:HyperLink> | 
                <asp:HyperLink ID="plusRemoteLnk" runat="server" NavigateUrl="~/Subscribe_Step1.aspx?plan=plus">Subscribe (Remote)</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Image ID="Image3" runat="server" ImageUrl="~/images/Premium.png" />
            </td>
            <td>
                <h3>Premium Plan</h3>
                <p>Here's some info about the premium plan.</p>
                <asp:HyperLink ID="premiumLnk" runat="server" NavigateUrl="~/Subscribe.aspx?plan=premium">Subscribe (Local)</asp:HyperLink> | 
                <asp:HyperLink ID="premiumRemoteLnk" runat="server" NavigateUrl="~/Subscribe_Step1.aspx?plan=premium">Subscribe (Remote)</asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Image ID="Image4" runat="server" ImageUrl="~/images/Ultimate.png" />
            </td>
            <td>
                <h3>Ultimate Plan</h3>
                <p>Here's some info about the ultimate plan.</p>
                <asp:HyperLink ID="ultimateLnk" runat="server" NavigateUrl="~/Subscribe.aspx?plan=ultimate">Subscribe (Local)</asp:HyperLink> | 
                <asp:HyperLink ID="ultimateRemoteLnk" runat="server" NavigateUrl="~/Subscribe_Step1.aspx?plan=ultimate">Subscribe (Remote)</asp:HyperLink>
            </td>
        </tr>
    </table>
    <br />
    <p style="position:relative;">
        We accept <asp:Image ID="Image5" runat="server" ImageUrl="~/images/credit_cards.gif" style="position:relative; top:5px;" />, our billing is handled by: 
        <asp:Image ID="Image6" runat="server" ImageUrl="~/images/chargify.gif" style="position:relative; top:4px;" /> and credit card processing by:
        <asp:Image ID="Image7" runat="server" ImageUrl="~/images/beanstream.gif" Height="24px" style="position:relative; top:9px;" />.
    </p>
</asp:Content>