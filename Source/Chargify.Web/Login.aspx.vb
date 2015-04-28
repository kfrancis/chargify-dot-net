#Region "Author Note"
' Login.aspx/Login.aspx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Partial Class Login
    Inherits System.Web.UI.Page

#Region "Control Methods"

    Protected Sub Login1_LoggedIn(ByVal sender As Object, ByVal e As System.EventArgs) Handles Login1.LoggedIn
        Dim appDirectory As String = ConfigurationManager.AppSettings("SECURE_APPLICATION_DIRECTORY")
        Dim url As String = ResolveUrl("~/" & appDirectory & "/Dashboard.aspx")
        Response.Redirect(url)
    End Sub

    Protected Sub Login1_LoginError(ByVal sender As Object, ByVal e As System.EventArgs) Handles Login1.LoginError
        Dim showPanel As Boolean = True

        ' See if the user exists in the database
        Dim userInfo As MembershipUser = Membership.GetUser(Login1.UserName)

        If userInfo Is Nothing Then
            ' The user entered an invalid username
            errorMessageLbl.Text = "There is no user in the database with the username '" & Login1.UserName & "'."
        Else
            ' See if the user is locked out or not approved
            If Not userInfo.IsApproved Then
                errorMessageLbl.Text = "Your account has not yet been approved by the site's administrators. Please try again later ..."
            ElseIf userInfo.IsLockedOut Then
                errorMessageLbl.Text = "Your account has been locked out because of a maximum number of incorrect login attempts. You will NOT be able to login until you contact a site administrator and have your account unlocked."
            Else
                ' The password was incorrect (don't show anything, the Login control already describes the problem)
                errorMessageLbl.Text = ""
                showPanel = False
            End If
        End If

        Me.errorPanel.Visible = showPanel
    End Sub

#End Region

End Class
