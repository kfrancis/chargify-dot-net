#Region "Author Note"
' SubscribeResult.aspx/SubscribeResult.aspx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Imports ChargifyNET

Partial Class SubscribeResult
    Inherits ChargifyPage

#Region "Page Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            Dim resultStr As String = Request.QueryString("r")
            Dim productHandle As String = Request.QueryString("p")
            Dim result As Boolean = True
            If Not Boolean.TryParse(resultStr, result) Then
                Dim url As String = ResolveUrl("~/Default.aspx")
                Response.Redirect(url)
            End If

            If result Then

                Dim subscribedProduct As IProduct = Chargify.LoadProduct(productHandle, True)

                Dim thanksForChoosing As String = "us"
                If subscribedProduct IsNot Nothing Then
                    Dim productString As String = subscribedProduct.ProductFamily.Name & " " & subscribedProduct.Name
                    thanksForChoosing = productString
                End If

                Me.resultTitleLtr.Text = "Subscription Success!"
                Me.resultTextLtr.Text = "Thanks for choosing " & thanksForChoosing & "! To start using this application, <a href='" & ResolveUrl(FormsAuthentication.LoginUrl) & "'><strong>login</strong></a>!"
            Else
                Me.resultTitleLtr.Text = "Subscription Failure"
                Me.resultTextLtr.Text = "Something went wrong during the process of adding your subscription, please check your email."
                Me.resultPnl.CssClass = "warning"
            End If
        End If
    End Sub

#End Region

End Class
