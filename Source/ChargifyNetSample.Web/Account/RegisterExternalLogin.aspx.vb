Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports System
Imports System.Security.Claims
Imports System.Web
Imports Microsoft.Owin.Security

Partial Public Class RegisterExternalLogin
    Inherits System.Web.UI.Page
    Protected Property ProviderName() As String
        Get
            Return If(DirectCast(ViewState("ProviderName"), String), [String].Empty)
        End Get
        Private Set(value As String)
            ViewState("ProviderName") = value
        End Set
    End Property

    Protected Property ProviderAccountKey() As String
        Get
            Return If(DirectCast(ViewState("ProviderAccountKey"), String), [String].Empty)
        End Get
        Private Set(value As String)
            ViewState("ProviderAccountKey") = value
        End Set
    End Property

    Protected Sub Page_Load() Handles Me.Load
        ' Process the result from an auth provider in the request
        ProviderName = IdentityHelper.GetProviderNameFromRequest(Request)
        If [String].IsNullOrEmpty(ProviderName) Then
            Response.Redirect("~/Account/Login")
        End If

        If Not IsPostBack Then
            Dim manager = New UserManager()
            Dim loginInfo = Context.GetOwinContext().Authentication.GetExternalLoginInfo()
            If loginInfo Is Nothing Then
                Response.Redirect("~/Account/Login")
            End If
            Dim appuser = manager.Find(loginInfo.Login)
            If appuser IsNot Nothing Then
                IdentityHelper.SignIn(manager, appuser, isPersistent:=False)
                IdentityHelper.RedirectToReturnUrl(Request.QueryString("ReturnUrl"), Response)
            ElseIf User.Identity.IsAuthenticated Then
                Dim verifiedloginInfo = Context.GetOwinContext().Authentication.GetExternalLoginInfo(IdentityHelper.XsrfKey, User.Identity.GetUserId())
                If verifiedloginInfo Is Nothing Then
                    Response.Redirect("~/Account/Login")
                End If

                Dim result = manager.AddLogin(User.Identity.GetUserId(), verifiedloginInfo.Login)
                If result.Succeeded Then
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString("ReturnUrl"), Response)
                Else
                    AddErrors(result)
                    Return
                End If
            Else
                userName.Text = loginInfo.DefaultUserName
            End If
        End If
    End Sub

    Protected Sub LogIn_Click(sender As Object, e As EventArgs)
        CreateAndLoginUser()
    End Sub

    Private Sub CreateAndLoginUser()
        If Not IsValid Then
            Return
        End If
        Dim manager = New UserManager()
        Dim user = New ApplicationUser() With {.UserName = userName.Text}
        Dim result = manager.Create(user)
        If Not result.Succeeded Then
            AddErrors(result)
            Return
        End If
        Dim loginInfo = Context.GetOwinContext().Authentication.GetExternalLoginInfo()
        If loginInfo Is Nothing Then
            Response.Redirect("~/Account/Login")
            Return
        End If
        result = manager.AddLogin(user.Id, loginInfo.Login)
        If Not result.Succeeded Then
            AddErrors(result)
            Return
        End If
        IdentityHelper.SignIn(manager, user, False)
        IdentityHelper.RedirectToReturnUrl(Request.QueryString("ReturnUrl"), Response)
        Return
    End Sub

    Private Sub AddErrors(result As IdentityResult)
        For Each [error] As String In result.Errors
            ModelState.AddModelError("", [error])
        Next
    End Sub
End Class