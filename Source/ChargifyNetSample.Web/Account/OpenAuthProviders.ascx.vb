Imports Microsoft.Owin.Security
Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports System.Web
Imports Microsoft.AspNet.Identity

Partial Public Class OpenAuthProviders
    Inherits System.Web.UI.UserControl
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then
            Dim provider = Request.Form("provider")
            If provider Is Nothing Then
                Return
            End If
            ' Request a redirect to the external login provider
            Dim redirectUrl As String = ResolveUrl([String].Format(CultureInfo.InvariantCulture, "~/Account/RegisterExternalLogin?{0}={1}&returnUrl={2}", IdentityHelper.ProviderNameKey, provider, ReturnUrl))
            Dim properties As AuthenticationProperties = New AuthenticationProperties() With {.RedirectUri = redirectUrl}
            'Add xsrf verification when linking accounts
            If (Context.User.Identity.IsAuthenticated) Then
                properties.Dictionary.Item(IdentityHelper.XsrfKey) = Context.User.Identity.GetUserId()
            End If
            Context.GetOwinContext().Authentication.Challenge(properties, provider)
            Response.StatusCode = 401
            Response.[End]()
        End If
    End Sub

    Public Property ReturnUrl() As String
        Get
            Return m_ReturnUrl
        End Get
        Set(value As String)
            m_ReturnUrl = value
        End Set
    End Property
    Private m_ReturnUrl As String

    Public Function GetProviderNames() As IEnumerable(Of String)
        Return Context.GetOwinContext().Authentication.GetExternalAuthenticationTypes().[Select](Function(t) t.AuthenticationType)
    End Function
End Class
