Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports System
Imports System.Linq
Imports System.Web
Imports System.Web.UI

Partial Public Class Register
    Inherits Page
    Protected Sub CreateUser_Click(sender As Object, e As EventArgs)
        Dim userName As String = UserNameCtrl.Text
        Dim manager = New UserManager()
        Dim user = New ApplicationUser() With {.UserName = userName}
        Dim result = manager.Create(user, Password.Text)
        If result.Succeeded Then
            IdentityHelper.SignIn(manager, user, isPersistent:=False)
            IdentityHelper.RedirectToReturnUrl(Request.QueryString("ReturnUrl"), Response)
        Else
            ErrorMessage.Text = result.Errors.FirstOrDefault()
        End If
    End Sub
End Class

