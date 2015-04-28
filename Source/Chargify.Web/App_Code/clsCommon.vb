Imports Microsoft.VisualBasic
Imports ChargifyNET
Imports ChargifyNET.Configuration

Public Class clsCommon

    Public Shared ReadOnly Property Chargify() As ChargifyConnect
        Get
            Dim config As ChargifyAccountRetrieverSection = CType(ConfigurationManager.GetSection("chargify"), ChargifyAccountRetrieverSection)
            Dim accountInfo As ChargifyAccountElement = config.GetDefaultOrFirst()
            Dim chargifyConnect As New ChargifyConnect()
            chargifyConnect.apiKey = accountInfo.ApiKey
            chargifyConnect.Password = accountInfo.ApiPassword
            chargifyConnect.URL = accountInfo.Site
            chargifyConnect.SharedKey = accountInfo.SharedKey
            Return chargifyConnect
        End Get
    End Property


End Class
