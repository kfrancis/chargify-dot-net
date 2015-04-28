<%@ Page  Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>
<%@ Register TagPrefix="uc" TagName="mainMenu" Src="~/controls/YamlMenu.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Login
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <uc:mainMenu ID="mainMenu" runat="server" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <div class="yform columnar">
        <asp:Login ID="Login1" runat="server" Width="500px">
            <LayoutTemplate>
                <fieldset>
                    <legend>Login</legend>
                    <p class="info">
                        To login as a customer: <strong>user / password</strong><br />
                        OR <a href="Plans.aspx">signup for a plan</a> and login with your own credentials!<br />
                        <a href="ForgotPassword.aspx">Forgot your password?</a>
                    </p>
                    <div class="type-text">
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:</asp:Label>
                        <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                    </div>
                    <div class="type-text">
                        <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                        <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                    </div>
                    <div class="type-check">
                        <asp:CheckBox ID="RememberMe" runat="server" /> Remember me?
                    </div>
                    <div class="float_right type-button" style="padding-right: 10px;">
                        <asp:Button ID="LoginButton" runat="server" CommandName="Login" Text="Log In" />
                    </div>
                    <div class="center">
                        <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                    </div>
                </fieldset>
            </LayoutTemplate>
        </asp:Login>
        <asp:Panel ID="errorPanel" runat="server" CssClass="warning" Visible="false">
            <asp:Label ID="errorMessageLbl" runat="server" Text="" Font-Italic="true"></asp:Label>
        </asp:Panel>
    </div>
    <script type="text/javascript">
        var myVal = null;
        $(document).ready(function () {
            $myForm = $("#aspnetForm"); // instance of myform for faster dom-select
            myVal = $myForm.validate({
                errorPlacement: function (error, element) {
                    element.parent().addClass("error");
                    error.prependTo(element.parent());
                },
                errorClass: "message",
                errorElement: "strong",
                onkeyup: false, // disable it - make some problems with errorPlacement 
                onclick: false, // disable it - make some problems with errorPlacement
                rules: {
                    '<%= Login1.FindControl("UserName").UniqueID %>': { required: true },
                    '<%= Login1.FindControl("Password").UniqueID %>': { required: true }
                }, messages: {}
            });

            /* Handle error message containers */
            var errorHandlerContainer = function () {
                $(this).valid(); // validate once field before show/hide messages
                var $cont = $(this).parent();
                var haserror = $("strong.message:visible", $cont).size();
                if (haserror < 1) { $cont.removeClass("error"); } else { $cont.addClass("error"); }
            }

            $(":input", $myForm).blur(errorHandlerContainer); // check error fields on blur on all input fields
            $("select", $myForm).change(errorHandlerContainer); // check error fields on change on select/radio fields
        });
    </script>
</asp:Content>