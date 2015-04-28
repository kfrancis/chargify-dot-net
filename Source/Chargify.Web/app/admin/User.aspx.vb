#Region "Author Note"
' User.aspx/User.aspx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Imports ChargifyNET

Partial Class app_admin_User
    Inherits ChargifyPage

#Region "Page Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            Load_Usages()
        End If
    End Sub

#End Region

#Region "Data Methods"

    Private Sub Load_Usages()
        Dim customer As ICustomer = GetUserCustomer()
        If customer IsNot Nothing Then
            Dim subscriptionList As Dictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
            If subscriptionList.Count > 0 Then
                Dim subscriptionID As Integer = subscriptionList.Keys(0)
                Dim compnentID As Integer = CInt(ConfigurationManager.AppSettings("BUTTONPUSH_COMPONENT").ToString())
                Me.usageRpt.DataSource = Chargify.GetComponentList(subscriptionID, compnentID).Values
                Me.usageRpt.DataBind()
            End If
        End If
    End Sub

#End Region

#Region "Control Methods"

    Protected Sub submitBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles submitBtn.Click
        Dim customer As ICustomer = GetUserCustomer()
        If customer IsNot Nothing Then
            Dim subscriptionList As Dictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
            If subscriptionList.Count > 0 Then
                Dim subscriptionID As Integer = subscriptionList.Keys(0)
                Dim amount As Decimal = Convert.ToDecimal(Me.amountTb.Text)
                Dim oneTimeCharge As ICharge = Chargify.CreateCharge(subscriptionID, amount, Me.reasonTb.Text)
                If oneTimeCharge IsNot Nothing Then
                    Me.submitBtn.Enabled = False
                    Me.resultLtr.Text = String.Format("You have successfully sent a one-time charge of {0}", amount)
                    Me.resultPnl.CssClass = "note"
                    Me.resultPnl.Visible = True
                Else
                    Me.submitBtn.Enabled = False
                    Me.resultLtr.Text = String.Format("Unfortunately, a one-time charge of {0} could not be charged to the customer.", amount)
                    Me.resultPnl.CssClass = "warning"
                    Me.resultPnl.Visible = True
                    ' Fail
                End If
            End If
        End If
    End Sub

    Protected Sub creditAcctBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles creditAcctBtn.Click
        Dim customer As ICustomer = GetUserCustomer()
        If customer IsNot Nothing Then
            Dim subscriptionList As Dictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
            If subscriptionList.Count > 0 Then
                Dim subscriptionID As Integer = subscriptionList.Keys(0)
                Dim amount As Decimal = Convert.ToDecimal(Me.creditAmountTb.Text)
                Dim credit As ICredit = Chargify.CreateCredit(subscriptionID, amount, Me.creditReasonTb.Text)
                If credit IsNot Nothing Then
                    Me.submitBtn.Enabled = False
                    Me.resultLtr.Text = String.Format("You have successfully credited {0} for ${0}", customer.FullName, amount)
                    Me.resultPnl.CssClass = "note"
                    Me.resultPnl.Visible = True
                Else
                    Me.submitBtn.Enabled = False
                    Me.resultLtr.Text = String.Format("Unfortunately, a one-time credit of ${0} could not be sent to the customer.", amount)
                    Me.resultPnl.CssClass = "warning"
                    Me.resultPnl.Visible = True
                    ' Fail
                End If
            End If
        End If
    End Sub

#End Region

#Region "Utility Methods"

    Private Function GetUserCustomer() As ICustomer
        Dim result As ICustomer = Nothing
        Dim uid As String = Request.QueryString("uid")
        If uid.IsGuid() Then
            Dim userID As Guid = New Guid(uid)
            result = Chargify.LoadCustomer(userID.ToString())
        End If
        Return result
    End Function

#End Region

End Class
