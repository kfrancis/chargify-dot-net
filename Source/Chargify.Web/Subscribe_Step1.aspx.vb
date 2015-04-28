#Region "Author Note"
' Subscribe_Step1.aspx/Subscribe_Step1.aspx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Imports ChargifyNET

Partial Class Subscribe_Step1
    Inherits ChargifyPage

#Region "Page Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            Dim plan As String = Request.QueryString("plan")

            ' Only allow the user to stay on this page IF the Product handle is correct
            If Not String.IsNullOrEmpty(plan) Then
                If IsChargifyProduct(plan) Then
                    Dim planName As String = UCase(Left(plan, 1)) & LCase(Mid(plan, 2))
                    Me.planNameLtr.Text = planName
                Else
                    Response.Redirect(ResolveUrl("~/Plans.aspx"))
                End If
            Else
                Response.Redirect(ResolveUrl("~/Plans.aspx"))
            End If
        End If
    End Sub

#End Region

#Region "Control Methods"

    Protected Sub submitBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles submitBtn.Click
        Try
            ' Try and create the system user
            Dim userStatus As MembershipCreateStatus
            Dim user As MembershipUser = Membership.CreateUser(Me.usernameTb.Text, Me.passwordTb.Text, Me.emailTb.Text, Me.secretQuestionTb.Text, Me.secretAnswerTb.Text, True, userStatus)
            If userStatus <> MembershipCreateStatus.Success Then
                Throw New Exception(userStatus.ToString())
            Else
                Roles.AddUserToRole(user.UserName, "User")
            End If

            If user.ProviderUserKey = Guid.Empty Then
                Throw New Exception("UserID Empty!")
            End If

            ' Get the productID from Chargify, since we know the corresponding product handle.
            Dim productHandle As String = Request.QueryString("plan")
            Dim productID As String = GetProductIDFromHandle(productHandle).ToString()

            ' Create the url for the subscription, passing any existing information we know to this point.
            Dim hostedPageUrl As String = Chargify.URL
            hostedPageUrl = hostedPageUrl & "h/" & productID & "/subscriptions/new?reference=" & user.ProviderUserKey.ToString() & "&email=" & Me.emailTb.Text
            Response.Redirect(hostedPageUrl, False)
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error", "alert('" & ex.Message & "');", True)
        End Try
    End Sub

#End Region

#Region "Utility Methods"

    Private Function IsChargifyProduct(ByVal productHandle As String) As Boolean
        Dim Products As IDictionary(Of Integer, IProduct) = Chargify.GetProductList()

        Dim result = From a In Products.Values _
                     Where a.Handle = productHandle _
                     Select a

        If result IsNot Nothing Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function GetProductIDFromHandle(ByVal handle As String) As Integer
        Dim productID As Integer = 0

        Dim product As IProduct = Chargify.LoadProduct(handle)
        If product IsNot Nothing Then
            productID = product.ID
        End If

        Return productID
    End Function


#End Region

End Class
