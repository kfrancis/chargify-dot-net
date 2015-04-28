#Region "Author Note"
' UsefulExtensions.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Imports Microsoft.VisualBasic

Public Module UsefulExtensions

    <System.Runtime.CompilerServices.Extension()> _
    Public Function IsGuid(ByVal input As String) As Boolean
        Dim isGuidRegex As New Regex("^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$", RegexOptions.Compiled)
        Dim isValid As Boolean = False
        If input IsNot Nothing Then
            If isGuidRegex.IsMatch(input) Then
                isValid = True
            End If
        End If
        Return isValid
    End Function

End Module