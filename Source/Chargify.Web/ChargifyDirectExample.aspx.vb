Imports ChargifyNET
Imports System.IO

Partial Class ChargifyDirectExample
    Inherits System.Web.UI.Page

#Region "Page Events"
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            Dim origin As New DateTime(1970, 1, 1, 0, 0, 0, 0)
            Dim diff As TimeSpan = DateTime.Now - origin

            Me.timestampTb.Text = Math.Floor(diff.TotalSeconds)
        End If
    End Sub
#End Region

#Region "Control Events"
    Protected Sub computeSignatureBtn_Click(sender As Object, e As System.EventArgs) Handles computeSignatureBtn.Click
        Dim origin As New DateTime(1970, 1, 1, 0, 0, 0, 0)
        Dim diff As TimeSpan = DateTime.Now - origin
        Dim timestamp As String = Math.Floor(diff.TotalSeconds).ToString()
        Dim sigMessage As String = apiIDTb.Text & timestamp & nonceTb.Text & dataTb.Text
        Dim signature As String = "" 'sigMessage.GetChargifyDirectSignature(secretTb.Text)
        If Not String.IsNullOrEmpty(signature) Then
            Me.apiIDLtr.Text = apiIDTb.Text
            Me.timestampLtr.Text = timestampTb.Text
            Me.nonceLtr.Text = Me.nonceTb.Text
            Me.dataLtr.Text = Me.dataTb.Text
            Me.apiSecretLtr.Text = Me.secretTb.Text
            Me.signatureLtr.Text = signature
            Me.sigResultPnl.Visible = True
        End If

        Me.formLtr.Text = "<code>" & CreateExampleForm(Me.apiIDTb.Text, Me.timestampTb.Text, Me.nonceTb.Text, Me.dataTb.Text, signature) & "</code>"
    End Sub

    Protected Sub resetLnkBtn_Click(sender As Object, e As System.EventArgs) Handles resetLnkBtn.Click
        Me.apiIDTb.Text = ""
        Me.timestampTb.Text = ""
        Me.nonceTb.Text = ""
        Me.dataTb.Text = ""
        Me.secretTb.Text = ""

        Me.apiIDLtr.Text = ""
        Me.timestampLtr.Text = ""
        Me.nonceLtr.Text = ""
        Me.dataLtr.Text = ""
        Me.apiSecretLtr.Text = ""
        Me.signatureLtr.Text = ""
        Me.formLtr.Text = ""
        Me.sigResultPnl.Visible = False
    End Sub
#End Region

#Region "Utility Methods"
    Private Function CreateExampleForm(ByVal apiID As String, ByVal timestamp As String, ByVal nonce As String, ByVal data As String, ByVal signature As String) As String
        Dim form As New HtmlGenericControl("form")
        form.Attributes("method") = "post"
        form.Attributes("action") = "https://api.chargify.com/api/v2/signups"

        Dim apiHdn As New HtmlGenericControl("input")
        apiHdn.Attributes("type") = "hidden"
        apiHdn.Attributes("name") = "secure[api_id]"
        apiHdn.Attributes("value") = apiID
        form.Controls.Add(apiHdn)

        Dim signatureHdn As New HtmlGenericControl("input")
        signatureHdn.Attributes("type") = "hidden"
        signatureHdn.Attributes("name") = "secure[signature]"
        signatureHdn.Attributes("value") = signature
        form.Controls.Add(signatureHdn)

        If Not String.IsNullOrEmpty(timestamp) Then
            Dim timestampHdn As New HtmlGenericControl("input")
            timestampHdn.Attributes("type") = "hidden"
            timestampHdn.Attributes("name") = "secure[timestamp]"
            timestampHdn.Attributes("value") = timestamp
            form.Controls.Add(timestampHdn)
        End If

        If Not String.IsNullOrEmpty(nonce) Then
            Dim nonceHdn As New HtmlGenericControl("input")
            nonceHdn.Attributes("type") = "hidden"
            nonceHdn.Attributes("name") = "secure[nonce]"
            nonceHdn.Attributes("value") = nonce
            form.Controls.Add(nonceHdn)
        End If

        If Not String.IsNullOrEmpty(data) Then
            Dim dataHdn As New HtmlGenericControl("input")
            dataHdn.Attributes("type") = "hidden"
            dataHdn.Attributes("name") = "secure[nonce]"
            dataHdn.Attributes("value") = HttpUtility.HtmlEncode(data)
            form.Controls.Add(dataHdn)
        End If

        Dim exampleForm As String = RenderFormControl(form)

        Return HttpUtility.HtmlEncode(exampleForm)
    End Function

    Private Function RenderFormControl(ByVal ctrl As Control) As String
        Dim sb As New StringBuilder()
        Dim tw As New StringWriter(sb)
        Dim hw As New HtmlTextWriter(tw)
        ctrl.RenderControl(hw)
        Return sb.ToString()
    End Function
#End Region
    
End Class
