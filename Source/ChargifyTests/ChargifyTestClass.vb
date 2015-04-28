Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports ChargifyNET

<CLSCompliant(True)> _
Public Class ChargifyTestClass

    Private textContextInstance As TestContext
    Public Property TextContext() As TestContext
        Get
            Return textContextInstance
        End Get
        Set(ByVal value As TestContext)
            textContextInstance = value
        End Set
    End Property

    Protected Shared MyChargify As ChargifyConnect

End Class