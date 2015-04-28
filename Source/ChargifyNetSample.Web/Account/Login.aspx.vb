Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports System.Web
Imports System.Web.UI
Imports Microsoft.Owin.Security

Partial Public Class Login
    Inherits Page
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        RegisterHyperLink.NavigateUrl = "Register"
        OpenAuthLogin.ReturnUrl = Request.QueryString("ReturnUrl")
        Dim returnUrl = HttpUtility.UrlEncode(Request.QueryString("ReturnUrl"))
        If Not [String].IsNullOrEmpty(returnUrl) Then
            RegisterHyperLink.NavigateUrl += "?ReturnUrl=" & returnUrl
        End If
    End Sub

    Protected Sub LogIn(sender As Object, e As EventArgs)
        If IsValid Then
            ' Validate the user password
            Dim manager = New UserManager()
            Dim user As ApplicationUser = manager.Find(UserName.Text, Password.Text)
            If user IsNot Nothing Then
                IdentityHelper.SignIn(manager, user, RememberMe.Checked)
                IdentityHelper.RedirectToReturnUrl(Request.QueryString("ReturnUrl"), Response)
            Else
                FailureText.Text = "Invalid username or password."
                ErrorMessage.Visible = True
            End If
        End If
    End Sub
End Class
