
#Region "Author Note"
' BillingHistory.aspx/BillingHistory.aspx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Imports ChargifyNET

Partial Class app_settings_BillingHistory
    Inherits ChargifyPage

    Private products As IDictionary(Of Integer, IProduct) = Nothing

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            Load_Data()
            Load_Products()
            Load_Grid()
        End If
    End Sub

#End Region

#Region "Data Methods"

    Private Sub Load_Data()
        transactionTypeDDL.DataSource = [Enum].GetNames(GetType(ChargifyNET.TransactionType))
        transactionTypeDDL.DataBind()
    End Sub

    Private Sub Load_Products()

        ' Keep a local copy of this for getting the name for use in the grid
        Me.products = Chargify.GetProductList()

    End Sub

    Private Sub Load_Grid()
        Try
            Dim customer As ICustomer = Chargify.LoadCustomer(Membership.GetUser().ProviderUserKey.ToString())
            Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
            If subscriptionList.Count > 0 Then
                Dim subscription As ISubscription = subscriptionList.ElementAt(0).Value
                Dim results As IDictionary(Of Integer, ITransaction) = Chargify.GetTransactionsForSubscription(subscription.SubscriptionID)
                transactionRpt.DataSource = results.Values
                transactionRpt.DataBind()
            End If
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error", "alert('" & ex.Message & "');", True)
        End Try
    End Sub

#End Region

#Region "Control Events"

    Protected Sub transactionTypeDDL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles transactionTypeDDL.SelectedIndexChanged
        Try
            Dim customer As ICustomer = Chargify.LoadCustomer(Membership.GetUser().ProviderUserKey.ToString())
            Dim subscriptionList As IDictionary(Of Integer, ISubscription) = Chargify.GetSubscriptionListForCustomer(customer.ChargifyID)
            If subscriptionList.Count > 0 Then
                Dim subscription As ISubscription = subscriptionList.ElementAt(0).Value
                Dim kind As TransactionType = CType([Enum].Parse(GetType(TransactionType), transactionTypeDDL.SelectedValue, True), TransactionType)
                Dim list As New List(Of TransactionType)()
                list.Add(kind)
                Dim results As IDictionary(Of Integer, ITransaction) = Chargify.GetTransactionsForSubscription(subscription.SubscriptionID, list)
                transactionRpt.DataSource = results.Values
                transactionRpt.DataBind()
            End If
        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error", "alert('" & ex.Message & "');", True)
        End Try
    End Sub

#End Region

#Region "Utility Methods"

    Public Function IsSuccessful(ByVal success As Boolean) As String
        If success Then
            Return "Successful"
        Else
            Return ""
        End If
    End Function

    Public Function GetProductName(ByVal productID As Integer) As String

        If Me.products.ContainsKey(productID) Then
            Dim product As IProduct = Me.products(productID)
            Return product.Name
        End If

        Return "Unknown"
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
