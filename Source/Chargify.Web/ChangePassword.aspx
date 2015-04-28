<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="ChangePassword.aspx.vb" Inherits="ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Change Password
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <!-- This menu is static on purpose, since the Subscribe page should only appear when you are on this page. -->
    <div class="hlist">
	    <ul>
		    <li><a href="default.aspx">Home</a></li>
		    <li class="active"><strong>Change Password</strong></li>
	    </ul>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <div class="yform columnar">
        <asp:ChangePassword ID="ChangePassword1" runat="server" Width="390px" ContinueDestinationPageUrl="~/Default.aspx" style="margin: 10px 20px 20px 0px;" CancelDestinationPageUrl="~/Default.aspx">  
            <ChangePasswordTemplate>
                <fieldset>
                    <legend>Change Your Password</legend>
                    <div class="type-text">
                        <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword">Password:</asp:Label>
                        <asp:TextBox ID="CurrentPassword" runat="server" TextMode="Password" CssClass="required" />
                    </div>
                    <div class="type-text">
                        <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">New Password:</asp:Label>
                        <asp:TextBox ID="NewPassword" runat="server" TextMode="Password" CssClass="required" />
                    </div>
                    <div id="confirmPasswordDiv" class="type-text">
                        <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword">Confirm New Password:</asp:Label>
                        <asp:TextBox ID="ConfirmNewPassword" runat="server" TextMode="Password" CssClass="required" />
                    </div>
                    <div class="float_right type-button">
                        <asp:Button ID="ChangePasswordPushButton" runat="server" CommandName="ChangePassword" Text="Change Password" ValidationGroup="ChangePass" />
                        <asp:Button ID="CancelPushButton" runat="server" CommandName="Cancel" Text="Cancel" CausesValidation="false" />
                    </div>
                    <div class="center">
                        <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                    </div>
                </fieldset>
            </ChangePasswordTemplate>
            <SuccessTemplate>
                <div>
                    <div class="note">
                        <h3 class="title">Your pasword has been changed.</h3>
                    </div>
                    <div class="type-button">
                        <asp:Button ID="ContinueButton" runat="server" CommandName="Continue" Text="Continue" CausesValidation="false" ValidationGroup="ChangePass" />
                    </div>
                </div>
            </SuccessTemplate>
        </asp:ChangePassword>
    </div>
</asp:Content>