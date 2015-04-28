#Region "Author Note"
' SimpleLog.aspx
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Imports Microsoft.VisualBasic
Imports System.IO

''' <summary>
''' SimpleLog just writes a text log file, and writes the time for every entry.
''' </summary>
Public Class SimpleLog
    Private sLogFormat As String
    Private sErrorTime As String

    Public Sub New()
        sLogFormat = DateTime.Now.ToShortDateString().ToString & " " & DateTime.Now.ToLongTimeString().ToString() & " ==> "
        Dim sYear As String = Date.Now.Year.ToString("0000")
        Dim sMonth As String = Date.Now.Month.ToString("00")
        Dim sDay As String = Date.Now.Day.ToString("00")
        sErrorTime = sYear & sMonth & sDay
    End Sub

    Public Sub Write(ByVal sPathName As String, ByVal sErrorMsg As String)
        Dim sw As New StreamWriter(sPathName & sErrorTime & ".txt", True)
        sw.WriteLine(sLogFormat & sErrorMsg)
        sw.Flush()
        sw.Close()
    End Sub
End Class
