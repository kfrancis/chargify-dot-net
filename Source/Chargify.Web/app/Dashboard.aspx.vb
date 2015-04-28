#Region "Author Note"
' Dashboard.aspx/Dashboard.aspx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Imports ChargifyNET

Partial Class app_Dashboard
    Inherits ChargifyPage

#Region "Page Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then

            If User.IsInRole("Administrator") Then
                Me.billingPnl.Visible = False
                adminLnkLtr.Text = "<li><a href='admin/Admin.aspx'>Administration</a></li>"
                Me.settingsLtr.Visible = False
                Me.billingResultLtr.Text = "Welcome administrator!"
                Me.billingResultPnl.CssClass = "note"
                Me.billingResultPnl.Visible = True
            Else
                LoadSubscriptionInfo()
            End If

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
        End If
    End Sub

#End Region

#Region "Methods"

    Private Sub LoadSubscriptionInfo()
        Try
            ' Load the customer information for the current user
            Dim customer As ICustomer = Chargify.Find(Of Customer)(Membership.GetUser().ProviderUserKey.ToString())

            ' Alternate syntax
            ' Dim customer As ICustomer = Chargify.LoadCustomer(Membership.GetUser().ProviderUserKey.ToString())

            If customer IsNot Nothing Then
                ' Get the list of subscriptions
                Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
                ' Get the total balance the user owes
                Dim totalBalance As Decimal = 0D
                For Each subscription As ISubscription In subscriptionList.Values
                    totalBalance = totalBalance + subscription.Balance
                    Me.nextPaymentDateLtr.Text = subscription.CurrentPeriodEndsAt.ToShortDateString()
                Next
                ' Display the total balance
                Me.currentBalanceLtr.Text = "$" & totalBalance.ToString("0.00")

                Dim currentSubscription As ISubscription = subscriptionList.Values(0)
                If currentSubscription IsNot Nothing Then
                    Me.planNameLtr.Text = currentSubscription.Product.ProductFamily.Name & " " & currentSubscription.Product.Name
                    Me.pricePerLtr.Text = "$" & currentSubscription.Product.Price & " Every " & currentSubscription.Product.Interval & " " & currentSubscription.Product.IntervalUnit
                End If
            Else
                Me.billingResultLtr.Text = "Could not find the Chargify customer."
                Me.billingResultPnl.CssClass = "warning"
                Me.billingResultPnl.Visible = True
                Me.usageBtn.Enabled = False
            End If
        Catch ex As Exception
            Me.billingResultLtr.Text = "Error loading subscription information."
            Me.billingResultPnl.CssClass = "warning"
            Me.billingResultPnl.Visible = True
            Me.usageBtn.Enabled = False
        End Try
    End Sub

    Private Function GetSubscriptionID() As Integer
        ' Load the customer information for the current user
        Dim customer As ICustomer = Chargify.Find(Of Customer)(Membership.GetUser().ProviderUserKey.ToString())
        ' Alternate syntax
        ' Dim customer As ICustomer = Chargify.LoadCustomer(Membership.GetUser().ProviderUserKey.ToString())        
        If customer IsNot Nothing Then
            ' Get the list of subscriptions
            Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
            Dim currentSubscription As ISubscription = subscriptionList.Values.FirstOrDefault()
            Return currentSubscription.SubscriptionID
        End If
        ' Can't find it
        Return Integer.MinValue
    End Function

#End Region

#Region "Control Methods"

    Protected Sub offBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles offBtn.Click
        Dim subscriptionID As Integer = GetSubscriptionID()
        Dim componentID As Integer = CInt(ConfigurationManager.AppSettings("SUPPORT_COMPONENT").ToString())
        Dim result As IComponentAttributes = Chargify.SetComponent(subscriptionID, componentID, False)
        If result IsNot Nothing Then
            Me.onOffResultLtr.Text = "This on/off component has now been set 'OFF'"
        Else
            Me.onOffResultLtr.Text = "There was an issue setting this component to 'OFF'"
        End If
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "highlight", "Highlight('onOffResult');", True)
    End Sub

    Protected Sub onBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles onBtn.Click
        Dim subscriptionID As Integer = GetSubscriptionID()
        Dim componentID As Integer = CInt(ConfigurationManager.AppSettings("SUPPORT_COMPONENT").ToString())
        Dim result As IComponentAttributes = Chargify.SetComponent(subscriptionID, componentID, True)
        If result IsNot Nothing Then
            Me.onOffResultLtr.Text = "This on/off component has now been set 'ON'"
        Else
            Me.onOffResultLtr.Text = "There was an issue setting this component to 'ON'"
        End If
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "highlight", "Highlight('onOffResult');", True)
    End Sub

    ' Just a test of how to update the component allocation
    'Protected Sub componentInfoBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles componentInfoBtn.Click

    '    ' Load the customer information for the current user
    '    Dim customer As ICustomer = Chargify.Find(Of Customer)(Membership.GetUser().ProviderUserKey.ToString())
    '    ' Alternate syntax
    '    ' Dim customer As ICustomer = Chargify.LoadCustomer(Membership.GetUser().ProviderUserKey.ToString())        
    '    If customer IsNot Nothing Then
    '        ' Get the list of subscriptions
    '        Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
    '        Dim currentSubscription As ISubscription = subscriptionList.Values.FirstOrDefault()
    '        If currentSubscription IsNot Nothing Then
    '            Dim componentID As Integer = CInt(ConfigurationManager.AppSettings("BUTTONPUSH_COMPONENT").ToString())

    '            ' Update the amount allocated
    '            Dim info As IComponentAttributes = Chargify.UpdateComponentAllocationForSubscription(currentSubscription.SubscriptionID, componentID, 5)
    '            ' Get the amount allocated
    '            Dim newInfo As IComponentAttributes = Chargify.GetComponentInfoForSubscription(currentSubscription.SubscriptionID, componentID)

    '        End If
    '    End If

    'End Sub

    Protected Sub usageBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles usageBtn.Click
        Try
            ' Load the customer information for the current user
            Dim customer As ICustomer = Chargify.Find(Of Customer)(Membership.GetUser().ProviderUserKey.ToString())
            ' Alternate syntax
            ' Dim customer As ICustomer = Chargify.LoadCustomer(Membership.GetUser().ProviderUserKey.ToString())

            If customer IsNot Nothing Then
                ' Get the list of subscriptions
                Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
                Dim currentSubscription As ISubscription = subscriptionList.Values(0)
                If currentSubscription IsNot Nothing Then

                    If currentSubscription.Product.Handle = "free" Then
                        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "ruhroh", "alert('You cannot use this feature until you upgrade to a paying fake subscription!');", True)
                        Exit Sub
                    End If

                    Dim componentID As Integer = CInt(ConfigurationManager.AppSettings("BUTTONPUSH_COMPONENT").ToString())
                    Dim result As IUsage = Chargify.AddUsage(currentSubscription.SubscriptionID, componentID, 1, "Pushed a button")
                    If result IsNot Nothing Then
                        ' Charged OK
                        Me.usageResultLtr.Text = String.Format("You were charged $0.10 for that button press ({0}).", DateTime.Now.ToString())
                        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "highlight", "HighlightResult();", True)
                    Else
                        ' Not OK
                        Me.usageResultLtr.Text = String.Format("Hmm, usage error. ({0})", DateTime.Now.ToString())
                        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "highlight", "HighlightResult();", True)
                    End If
                End If
            End If
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error", "alert('" & ex.Message & "');", True)
        End Try
    End Sub

#End Region

#Region "Navigation Methods"

    Protected Sub logoutBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logoutBtn.Click
        Dim url As String = ResolveUrl("~/Logout.aspx")
        Response.Redirect(url)
    End Sub

#End Region

End Class
