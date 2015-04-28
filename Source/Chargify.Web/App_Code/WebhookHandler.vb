Imports Microsoft.VisualBasic
Imports ChargifyNET
Imports ChargifyNET.Configuration

Public Class WebhookHandler
    Inherits ChargifyNET.WebhookHandler

    Public Overrides Sub OnChargifyUpdate(ByVal webhookID As Integer, ByVal signature As String, ByVal data As String)
        ' You'll probably want to do something to an associated database whenever this update gets called.

        ' Just log the updated to a simple text file .. 
        Dim updateLog As New SimpleLog()

        ' Start the log
        updateLog.Write(HttpContext.Current.Server.MapPath("logs/updateLog"), "Started")
        updateLog.Write(HttpContext.Current.Server.MapPath("logs/updateLog"), data)

        Dim config As ChargifyAccountRetrieverSection = TryCast(ConfigurationManager.GetSection("chargify"), ChargifyAccountRetrieverSection)

        Dim result As Boolean = data.IsChargifyWebhookContentValid(signature, config.GetSharedKeyForDefaultOrFirstSite())
        Dim resultStr As String = IIf(result, "VALID", "INVALID")

        updateLog.Write(HttpContext.Current.Server.MapPath("logs/updateLog"), "Data was " & resultStr & " through self-validation")

        ' Stop the log
        updateLog.Write(HttpContext.Current.Server.MapPath("logs/updateLog"), "Finished")

    End Sub
End Class
