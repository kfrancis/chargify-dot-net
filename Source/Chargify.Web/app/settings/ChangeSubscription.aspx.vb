
#Region "Author Note"
' ChangeSubscription.aspx/ChangeSubscription.aspx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Imports ChargifyNET

Partial Class app_settings_ChangeSubscription
    Inherits ChargifyPage

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            If Membership.GetUser().UserName = "user" Then
                ' This is the demo user, don't let the person on the system change the password for the demo user.
                Dim loginView As LoginView = CType(Page.Master.FindControl("LoginView1"), LoginView)
                If loginView IsNot Nothing Then
                    Dim changePnl As Panel = CType(loginView.FindControl("changePnl"), Panel)
                    If changePnl IsNot Nothing Then
                        changePnl.Visible = False
                    End If
                End If
            End If

            Load_SubscriptionInfo()
            Load_Grid()
        End If
    End Sub

#End Region

#Region "Data Loading Methods"

    Private Sub Load_Grid()
        Dim Products As IDictionary(Of Integer, IProduct) = Chargify.GetProductList()

        Dim MainProducts As List(Of IProduct) = New List(Of IProduct)()
        For Each Product As IProduct In Products.Values
            MainProducts.Add(Product)
        Next

        Dim AddOns As List(Of IProduct) = New List(Of IProduct)()
        For Each Product As IProduct In Products.Values
            If Product.ProductFamily.Name.Contains("Add") Then
                AddOns.Add(Product)
            End If
        Next

        addOnRpt.DataSource = MainProducts
        addOnRpt.DataBind()
    End Sub

#End Region

#Region "Methods"

    Private Sub Load_SubscriptionInfo()
        Try
            Dim customer As ICustomer = Chargify.LoadCustomer(Membership.GetUser().ProviderUserKey.ToString())
            Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
            If subscriptionList.Count > 0 Then
                Dim subscription As ISubscription = subscriptionList.ElementAt(0).Value
                Dim planName As String = subscription.Product.ProductFamily.Name & " " & subscription.Product.Name
                Me.currentPlanLtr.Text = planName
            End If
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error", "alert('" & ex.Message & "');", True)
        End Try
    End Sub

    Public Function IsSubscribed(ByRef productHandle As String) As Boolean
        Dim result As Boolean = False
        Dim customer As ICustomer = Chargify.LoadCustomer(Membership.GetUser().ProviderUserKey.ToString())
        Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
        If subscriptionList.Count > 0 Then
            Dim subscription = subscriptionList.ElementAt(0).Value
            If subscription.Product.Handle = productHandle Then
                result = True
            End If
        End If
        Return result
    End Function

    Public Function GetSubscriptionStyle(ByRef isSubscribed As Boolean) As String
        If isSubscribed Then
            Return "selected"
        Else
            Return ""
        End If
    End Function

    Private Sub UpdateMessage(ByVal message As String, ByVal isSuccessful As Boolean)
        If isSuccessful Then
            Me.resultPnl.CssClass = "note"
        Else
            Me.resultPnl.CssClass = "warning"
        End If
        Me.resultLtr.Text = message
        Me.resultPnl.Visible = True
    End Sub

#End Region

#Region "Control Methods"

    Protected Sub addOnRpt_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles addOnRpt.ItemCommand
        If e.CommandName = "ChangePlan" Then
            Dim currentUser As MembershipUser = Membership.GetUser()
            Dim customer As ICustomer = Chargify.LoadCustomer(currentUser.ProviderUserKey.ToString())
            Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
            If subscriptionList.Count > 0 Then
                Dim existingSubscription As ISubscription = subscriptionList.ElementAt(0).Value
                Dim updatedSubscription As ISubscription = Chargify.EditSubscriptionProduct(existingSubscription, e.CommandArgument.ToString())
                If updatedSubscription IsNot Nothing Then
                    UpdateMessage("Subscription successfully changed.", True)
                Else
                    UpdateMessage("Update not successful.", False)
                End If

                ' Update this info regardless
                Load_SubscriptionInfo()
                Load_Grid()
            End If
        ElseIf e.CommandName = "MigratePlan" Then
            Dim currentUser As MembershipUser = Membership.GetUser()
            Dim customer As ICustomer = Chargify.LoadCustomer(currentUser.ProviderUserKey.ToString())
            Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
            If subscriptionList.Count > 0 Then
                Dim existingSubscription As ISubscription = subscriptionList.ElementAt(0).Value
                Dim updatedSubscription As ISubscription = Chargify.MigrateSubscriptionProduct(existingSubscription, e.CommandArgument.ToString(), False, False)
                If updatedSubscription IsNot Nothing Then
                    UpdateMessage("Subscription successfully migrated.", True)
                Else
                    UpdateMessage("Migration was not successful.", False)
                End If

                ' Update this info regardless
                Load_SubscriptionInfo()
                Load_Grid()
            End If
        End If
    End Sub

#End Region

#Region "Navigation Methods"

    Protected Sub logoutBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logoutBtn.Click
        Dim url As String = ResolveUrl("~/Logout.aspx")
        Response.Redirect(url)
    End Sub

#End Region

End Class
