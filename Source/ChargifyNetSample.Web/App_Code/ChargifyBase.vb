Imports ChargifyNET
Imports ChargifyNET.Configuration

Public Class ChargifyBase
    Public Shared ReadOnly Property Chargify() As ChargifyConnect
        Get
            Dim chargifyConnect As New ChargifyConnect()
            chargifyConnect.apiKey = ConfigurationManager.AppSettings("CHARGIFY_API_KEY")
            chargifyConnect.Password = ConfigurationManager.AppSettings("CHARGIFY_API_PASSWORD")
            chargifyConnect.URL = ConfigurationManager.AppSettings("CHARGIFY_SITE_URL")
            chargifyConnect.SharedKey = ConfigurationManager.AppSettings("CHARGIFY_SHARED_KEY")
            Return chargifyConnect
        End Get
    End Property
End Class
