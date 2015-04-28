Imports System.Text
Imports ChargifyNET

<TestClass()>
Public Class ProductTests
    Inherits ChargifyTestClass

#Region "Additional Test Attributes"
    <ClassInitialize()> _
    Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
        If MyChargify Is Nothing Then
            MyChargify = New ChargifyConnect()
            MyChargify.apiKey = "AbCdEfGhIjKlMnOpQrSt"
            MyChargify.Password = "P"
            MyChargify.URL = "https://subdomain.chargify.com/"
        End If
    End Sub
#End Region

    <TestMethod()>
    Public Sub GetProductListTest()
        Dim productList As IDictionary(Of Integer, IProduct) = Nothing
        productList = ProductTests.MyChargify.GetProductList()

        Assert.IsNotNull(productList)
        Assert.AreNotEqual(0, productList.Count)
    End Sub

End Class
