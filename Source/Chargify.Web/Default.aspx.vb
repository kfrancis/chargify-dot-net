#Region "Author Note"
' Default.aspx/Default.aspx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Imports ChargifyNET

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim test As String = "update_payment--77--1234"
        Dim token As String = test.GetChargifyHostedToken()
    End Sub
End Class
