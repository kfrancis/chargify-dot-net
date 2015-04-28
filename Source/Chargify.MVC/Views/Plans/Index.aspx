<%@ Page Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMainContent" runat="server">

    <table class="full">
        <thead>
            <tr>
                <th scope="col" colspan="2">Our Plans</th>
            </tr>
        </thead>
        <tr>
            <td style="width: 80px;">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Content/images/Basic.png" />
            </td>
            <td>
                <h3>Basic Plan</h3>
                <p>Here's some info about the basic plan.</p>
                <%= Html.ActionLink("Subscribe", "Subscribe", New With {.id = "basic"})%> | 
                <%= Html.ActionLink("Subscribe (Remote)", "SubscribeRemote", New With {.id = "basic"})%>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Image ID="Image2" runat="server" ImageUrl="~/Content/images/Plus.png" />
            </td>
            <td>
                <h3>Plus Plan</h3>
                <p>Here's some info about the plus plan.</p>
                <%= Html.ActionLink("Subscribe", "Subscribe", New With {.id = "plus"})%> | 
                <%= Html.ActionLink("Subscribe (Remote)", "SubscribeRemote", New With {.id = "plus"})%>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Image ID="Image3" runat="server" ImageUrl="~/Content/images/Premium.png" />
            </td>
            <td>
                <h3>Premium Plan</h3>
                <p>Here's some info about the premium plan.</p>
                <%= Html.ActionLink("Subscribe", "Subscribe", New With {.id = "premium"})%> | 
                <%= Html.ActionLink("Subscribe (Remote)", "SubscribeRemote", New With {.id = "premium"})%>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Image ID="Image4" runat="server" ImageUrl="~/Content/images/Ultimate.png" />
            </td>
            <td>
                <h3>Ultimate Plan</h3>
                <p>Here's some info about the ultimate plan.</p>
                <%= Html.ActionLink("Subscribe", "Subscribe", New With {.id = "ultimate"})%> | 
                <%= Html.ActionLink("Subscribe (Remote)", "SubscribeRemote", New With {.id = "ultimate"})%>
            </td>
        </tr>
    </table>
    <br />
    <p style="position:relative;">
        We accept <asp:Image ID="Image5" runat="server" ImageUrl="~/Content/images/credit_cards.gif" style="position:relative; top:5px;" />, our billing is handled by: 
        <asp:Image ID="Image6" runat="server" ImageUrl="~/Content/images/chargify.gif" style="position:relative; top:4px;" /> and credit card processing by:
        <asp:Image ID="Image7" runat="server" ImageUrl="~/Content/images/beanstream.gif" Height="24px" style="position:relative; top:9px;" />.
    </p>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitleContent" runat="server">
    Plans
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphMenu" runat="server">
    <div class="hlist">
	    <ul>
		    <li><%= Html.ActionLink("Home", "Index", "Home")%></li>
		    <li class="active"><strong>Plans</strong></li>
	    </ul>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="cphLeftContent" runat="server">
    
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