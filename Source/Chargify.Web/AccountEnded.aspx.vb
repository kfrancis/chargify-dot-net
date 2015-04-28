#Region "Author Note"
' AccountEnded.aspx/AccountEnded.aspx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Partial Class AccountEnded
    Inherits System.Web.UI.Page

#Region "Page Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            FormsAuthentication.SignOut()
        End If
    End Sub

#End Region
    
End Class
