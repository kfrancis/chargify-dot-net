#Region "Author Note"
' YamlMenu.ascx/YamlMenu.ascx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Partial Public Class _YamlMenu
    Inherits System.Web.UI.UserControl

    Private Sub rptMenu_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMenu.ItemDataBound
        If Not IsNothing(e.Item.DataItem) Then
            Dim sn As SiteMapNode = CType(e.Item.DataItem, SiteMapNode)
            Dim curNode As SiteMapNode = SiteMap.Providers("DefaultSiteMap").CurrentNode
            Dim hl As HyperLink = CType(e.Item.FindControl("hlNode"), HyperLink)
            Dim li As HtmlGenericControl = CType(e.Item.FindControl("listItem"), HtmlGenericControl)

            If curNode.Equals(sn) Then
                li.Attributes("class") = "active"
                li.Controls.Clear()
                Dim ltr As New LiteralControl()
                ltr.Text = "<strong>" & curNode.Title & "</strong>"
                li.Controls.Add(ltr)
            End If
        End If
    End Sub
End Class