Imports Microsoft.VisualBasic
Imports ChargifyNET

#Region "Author Note"
' PostbackHandler.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

''' <summary>
''' This is a postback handler which should be recieving the subscription updates from Chargify.com if you have it setup correctly.
''' To install this, you must have a class which inherits from ChargifyNET.SubscriptionPostbackHandler as well as the following entry in IIS:
''' For IIS7 - <add name="PostbackHandler" path="Update.axd" verb="POST" type="PostbackHandler" resourceType="Unspecified" preCondition="integratedMode" />
''' For IIS6 - <add verb="POST" path="Update.axd" type="PostbackHandler" />
''' Check the web.config for this app to see those entries and where they are placed.
''' </summary>
''' <remarks></remarks>
Public Class PostbackHandler
    Inherits SubscriptionPostbackHandler

    Public Overrides Sub OnChargifyUpdate(ByVal ids() As String)

        ' You'll probably want to do something to an associated database whenever this update gets called.

        ' Just log the updated to a simple text file .. 
        Dim updateLog As New SimpleLog()

        ' Start the log
        updateLog.Write(HttpContext.Current.Server.MapPath("logs/updateLog"), "Started")

        For Each id As String In ids
            updateLog.Write(HttpContext.Current.Server.MapPath("logs/updateLog"), id)
        Next

        ' Stop the log
        updateLog.Write(HttpContext.Current.Server.MapPath("logs/updateLog"), "Finished")

    End Sub
End Class
