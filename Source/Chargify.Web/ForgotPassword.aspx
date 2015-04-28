<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="ForgotPassword.aspx.vb" Inherits="ForgotPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Forgot Password
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <!-- This menu is static on purpose, since the Subscribe page should only appear when you are on this page. -->
    <div class="hlist">
	    <ul>
		    <li><a href="default.aspx">Home</a></li>
		    <li class="active"><strong>Forgot Password</strong></li>
	    </ul>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <div class="yform columnar">
        <asp:PasswordRecovery ID="PasswordRecovery1" runat="server">
            <UserNameTemplate>
                <fieldset>
                    <legend>Forgot your password?</legend>
                    <p class="note">Please enter your <strong>User Name</strong> to have your password sent to your email.</p>
                    <div id="usernameDiv" class="type-text">
                        <strong id="usernameMsg" class="message" style="display:none; visibility:hidden;">Username is required.</strong>
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Text="User Name:"></asp:Label>
                        <asp:TextBox ID="UserName" runat="server" Width="200px"></asp:TextBox>
                        <asp:CustomValidator ID="PasswordRequired" runat="server" CssClass="validator" ControlToValidate="UserName" ValidationGroup="PasswordRecovery1"
                                ClientValidationFunction="Claims.Add.ValidateUserName" ValidateEmptyText="true" />
                    </div>
                <div class="float_right type-button">
                    <asp:Button ID="SubmitButton" runat="server" CommandName="Submit" Text="Submit" ValidationGroup="PasswordRecovery1" />
                </div>
                <div class="center">
                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                </div>
                </fieldset>
            </UserNameTemplate>
            <QuestionTemplate>
                <fieldset>
                    <legend>Identity Confirmation</legend>
                    <p class="note">Please answer the following question to receive your password.</p>
                    <div class="type-text">
                        <asp:Label ID="UserNameLbl" runat="server" AssociatedControlID="UserName" Text="User Name:"></asp:Label>
                        <asp:Literal ID="UserName" runat="server"></asp:Literal>
                    </div>
                    <div class="type-text">
                        <asp:Label ID="QuestionLbl" runat="server" AssociatedControlID="Question" Text="Question:"></asp:Label>
                        <strong><asp:Literal ID="Question" runat="server"></asp:Literal></strong>
                    </div>
                    <div id="answerDiv" class="type-text">
                        <strong id="answerMsg" class="message" style="display:none; visibility:hidden;">An answer is required.</strong>
                        <asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer" Text="Answer:"></asp:Label>
                        <asp:TextBox ID="Answer" runat="server"></asp:TextBox>
                        <asp:CustomValidator ID="PasswordRequired" runat="server" CssClass="validator" ControlToValidate="Answer" ValidationGroup="PasswordRecovery1"
                                ClientValidationFunction="Claims.Add.ValidateSecretQuestionAnswer" ValidateEmptyText="true" />
                    </div>
                    <div class="center">
                        <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                    </div>
                    <div class="float_right type-button">
                        <asp:Button ID="SubmitButton" runat="server" CommandName="Submit" Text="Submit" ValidationGroup="PasswordRecovery1" />
                    </div>
                </fieldset>
            </QuestionTemplate>
            <SuccessTemplate>
                <p class="note">Your password has been sent to you.</p>
            </SuccessTemplate>
            <MailDefinition From="noreply@clinicalsupportsystems.com" IsBodyHtml="True" 
                Priority="High" Subject="Your new temporary password">
            </MailDefinition>
        </asp:PasswordRecovery>
    </div>
</asp:Content>