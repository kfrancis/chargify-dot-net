<%@ Page Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage(Of Chargify.MVC.LogOnModel)" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMainContent" runat="server">

    <% Using Html.BeginForm() %>
        <%=Html.ValidationSummary(True, "Login was unsuccessful. Please correct the errors and try again.")%>
        <div class="yform columnar">
            <fieldset style="width: 500px;">
                <legend>Login</legend>
                <p class="info">
                    To login as a customer: <strong>user / password</strong><br />
                    OR <%= Html.ActionLink("signup for a plan", "Index", "Plans")%> and login with your own credentials!
                </p>
                <div class="type-text">
                    <%= Html.LabelFor(Function(m) m.UserName) %>
                    <%= Html.TextBoxFor(Function(m) m.UserName) %>
                </div>
                <div class="type-text">
                    <%= Html.LabelFor(Function(m) m.Password) %>
                    <%= Html.PasswordFor(Function(m) m.Password) %>
                </div>
                <div class="type-check">
                    <%= Html.CheckBoxFor(Function(m) m.RememberMe) %>
                    <%= Html.LabelFor(Function(m) m.RememberMe) %>
                </div>
                <div class="type-button float_right">
                    <input type="submit" value="Log On" />
                </div>
            </fieldset>
        </div>
    <% End Using %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitleContent" runat="server">
    Login
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphMenu" runat="server">
    <div class="hlist">
	    <ul>
		    <li><%= Html.ActionLink("Home", "Index", "Home")%></li>
		    <li class="active"><strong>Login</strong></li>
	    </ul>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="cphLeftContent" runat="server">
</asp:Content>