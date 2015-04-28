Imports ChargifyNET

Public Class _Default
    Inherits ChargifyPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            ' Not doing much here, since there's already a ASP.NET Webform app .. 
            ' So just get a list of customers and put it on the screen. 
            Dim customerList As List(Of ICustomer) = (Chargify.GetCustomerList().Values).ToList()
            Me.gvCustomers.DataSource = customerList
            Me.gvCustomers.DataBind()
        End If
    End Sub

End Class