#Region "Author Note"
' Subscribe_Step2.aspx/Subscribe_Step2.aspx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Partial Class Subscribe_Step2
    Inherits System.Web.UI.Page

#Region "Page Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            Dim subscriptionID As String = Request.QueryString("subscription_id")
            Dim productHandle As String = Request.QueryString("product")
            Dim result As String = "true"
            If String.IsNullOrEmpty(subscriptionID) Then
                ' if (for some reason) we don't get passed back the subscription id, then pass the user on and show them an error
                result = "false"
            End If
            Dim url As String = ResolveUrl("~/SubscribeResult.aspx?r=" & result)
            If Not String.IsNullOrEmpty(productHandle) Then
                url = url & "&p=" & productHandle
            End If
            Response.Redirect(url)
        End If
    End Sub

#End Region
    
End Class
