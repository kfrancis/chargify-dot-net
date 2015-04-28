#Region "Author Note"
' Account.aspx/Account.aspx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Imports ChargifyNET
Imports System.Data

Partial Class app_settings_Account
    Inherits ChargifyPage

#Region "Page Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            Dim theUser As MembershipUser = Membership.GetUser()
            If theUser IsNot Nothing Then
                Me.emailLtr.Text = theUser.Email
                Me.lastLoginLtr.Text = theUser.LastLoginDate.ToString()
            End If

            InitData()
            GetBillingInfo()

            If theUser.UserName = "user" Then
                ' This is the demo user, don't let the person on the system cancel the demo user.
                cancelAcctPnl.Visible = False

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

    Private Sub InitData()
        Dim dt As New DataTable()
        dt.Columns.Add("Year", GetType(String))
        For i As Integer = 0 To 10
            Dim year As String = (Date.Today.Year + i).ToString()
            dt.Rows.Add(year)
        Next
        Me.expirationYearDDL.DataSource = dt
        Me.expirationYearDDL.DataBind()

        Dim currentUser As MembershipUser = Membership.GetUser()
        Dim customer As ICustomer = Chargify.LoadCustomer(currentUser.ProviderUserKey.ToString())
        Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
        If subscriptionList.Count > 0 Then
            Dim subscription As ISubscription = subscriptionList.First().Value
            If subscription IsNot Nothing Then
                ' Take your pick here, either "pretty" or not. Since users like to see their name, the demo uses a pretty url.
                'Me.remoteLnk.NavigateUrl = Chargify.GetSubscriptionUpdateURL(subscription.SubscriptionID)
                Me.remoteLnk.NavigateUrl = Chargify.GetPrettySubscriptionUpdateURL(customer.FirstName, customer.LastName, subscription.SubscriptionID)
            End If
        End If
    End Sub

    Private Sub UpdateControls()
        Dim value As String = ccDDL.SelectedValue.ToString()
        Select Case value
            Case "0"
                Me.cardPnl.Visible = False
                Me.ccvPnl.Visible = False
            Case "1"
                Me.cardPnl.Visible = True
                Me.ccvPnl.Visible = True
        End Select
    End Sub

    Private Sub GetBillingInfo()
        Try
            Dim currentUser As MembershipUser = Membership.GetUser()
            Dim customer As ICustomer = Chargify.LoadCustomer(currentUser.ProviderUserKey.ToString())
            Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
            If subscriptionList.Count > 0 Then
                Dim subscription As ISubscription = subscriptionList.First().Value
                Dim ccInfo As ICreditCardView = subscription.CreditCard
                If ccInfo IsNot Nothing Then
                    Dim existingCC As New ListItem(ccInfo.FullNumber, "0")
                    Dim optionCC As New ListItem("Update or Change Card...", "1")
                    Me.ccDDL.Items.Add(existingCC)
                    Me.ccDDL.Items.Add(optionCC)
                    UpdateControls()
                    Me.fNameTb.Text = customer.FirstName
                    Me.lNameTb.Text = customer.LastName
                    Me.expirationMonthDDL.ClearSelection()
                    Me.expirationMonthDDL.SelectedValue = ccInfo.ExpirationMonth.ToString()
                    Me.expirationYearDDL.ClearSelection()
                    Me.expirationYearDDL.SelectedValue = ccInfo.ExpirationYear.ToString()
                Else
                    ' No credit card on file, check to see if they are using the free product or not.
                    If subscription.Product.Handle = "free" Then
                        ' Free product, hide these areas.
                        Me.paymentPanel.Enabled = False
                        paymentMsgLtr.Text = "<div class='note'>No payment information needed for the currently subscribed plan!</div>"
                    Else
                        ' Not a free product, need card on file. Stress this to the user!
                        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "ruhroh", "alert('You need to enter a credit card!');", True)
                        paymentMsgLtr.Text = "<div class='warning'>No payment information has been entered! Enter your information now to prevent issues concerning your account!</div>"
                    End If
                End If
            End If
        Catch ex As Exception
            Page.ClientScript.RegisterStartupScript(GetType(Page), "error", "alert('" & ex.Message & "');", True)
        End Try

    End Sub

    Private Sub DeleteAccount()
        Try
            Dim currentUser As MembershipUser = Membership.GetUser()
            Dim customer As ICustomer = Chargify.LoadCustomer(currentUser.ProviderUserKey.ToString())
            Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
            If subscriptionList.Count > 0 Then
                Dim subscription As ISubscription = subscriptionList.ElementAt(0).Value
                Dim billingCancelled As Boolean = Chargify.DeleteSubscription(subscription.SubscriptionID, "User Cancelled")
                If billingCancelled Then
                    Dim userDeleted As Boolean = Membership.DeleteUser(currentUser.UserName)
                    If userDeleted Then
                        Dim url As String = ResolveUrl("~/AccountEnded.aspx")
                        Response.Redirect(url)
                    End If
                End If
            End If
        Catch ex As Exception
            Page.ClientScript.RegisterStartupScript(GetType(Page), "error", "alert('" & ex.Message & "');", True)
        End Try
    End Sub

#End Region

#Region "Control Methods"

    Protected Sub cancelBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cancelBtn.Click
        DeleteAccount()
    End Sub

    Protected Sub updateCCBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles updateCCBtn.Click
        Try
            Dim currentUser As MembershipUser = Membership.GetUser()
            Dim customer As ICustomer = Chargify.LoadCustomer(currentUser.ProviderUserKey.ToString())
            Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
            If subscriptionList.Count > 0 Then
                Dim subscription As ISubscription = subscriptionList.First().Value

                Dim ccChanged As New CreditCardAttributes()
                Dim updatedCC As Boolean = False
                Dim updateCustomer As Boolean = False
                Dim ccInfo As ICreditCardView = subscription.CreditCard
                If ccInfo IsNot Nothing Then
                    If customer.FirstName <> Me.fNameTb.Text Then
                        customer.FirstName = Me.fNameTb.Text
                        ccChanged.FirstName = Me.fNameTb.Text
                        updateCustomer = True
                        updatedCC = True
                    End If

                    If customer.LastName <> Me.lNameTb.Text Then
                        customer.LastName = Me.lNameTb.Text
                        ccChanged.LastName = Me.lNameTb.Text
                        updateCustomer = True
                        updatedCC = True
                    End If

                    If ccInfo.ExpirationMonth <> CInt(Me.expirationMonthDDL.SelectedValue) Then
                        ccChanged.ExpirationMonth = CInt(Me.expirationMonthDDL.SelectedValue)
                        updatedCC = True
                    End If

                    If ccInfo.ExpirationYear <> CInt(Me.expirationYearDDL.SelectedValue) Then
                        ccChanged.ExpirationYear = CInt(Me.expirationYearDDL.SelectedValue)
                        updatedCC = True
                    End If
                End If

                Dim updateSuccessful As Boolean = True

                If updateCustomer Then
                    Dim customerUpdated As ICustomer = Chargify.Save(Of Customer)(customer)
                    If customerUpdated Is Nothing Then
                        ' failure
                        updateSuccessful = False
                    End If
                End If

                If updatedCC Then
                    Dim updatedSubscription As ISubscription = Chargify.UpdateSubscriptionCreditCard(subscription, ccChanged)
                    If updatedSubscription Is Nothing Then
                        ' failure
                        updateSuccessful = False
                    End If
                End If

                If updateSuccessful Then
                    ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "success", "alert('Successfully Updated Account.');", True)
                Else
                    ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error", "alert('Subscription information could not be updated.');", True)
                End If

            End If
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error", "alert('" & ex.Message & "');", True)
        End Try
    End Sub

    Protected Sub ccDDL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ccDDL.SelectedIndexChanged
        UpdateControls()
    End Sub

#End Region

#Region "Navigation Methods"

    Protected Sub logoutBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logoutBtn.Click
        Dim url As String = ResolveUrl("~/Logout.aspx")
        Response.Redirect(url)
    End Sub

#End Region

End Class
