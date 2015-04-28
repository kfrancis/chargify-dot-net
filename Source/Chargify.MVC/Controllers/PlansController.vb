Namespace Chargify.MVC
    Public Class PlansController
        Inherits System.Web.Mvc.Controller

        ''' <summary>
        ''' Plans.aspx
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Index() As ActionResult
            Return View()
        End Function

        ''' <summary>
        ''' Subscribe.aspx
        ''' </summary>
        ''' <param name="id">The product handle being subscribed to</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function Subscribe(ByVal id As String) As ActionResult

            Dim provinces As New List(Of SelectListItem)()

            provinces.Add(New SelectListItem() With {.Text = "Alberta", .Value = "AB"})
            provinces.Add(New SelectListItem() With {.Text = "British Columbia", .Value = "BC"})
            provinces.Add(New SelectListItem() With {.Text = "Manitoba", .Value = "MB"})
            provinces.Add(New SelectListItem() With {.Text = "Newfoundland and Labrador", .Value = "NL"})
            provinces.Add(New SelectListItem() With {.Text = "New Brunswick", .Value = "NB"})
            provinces.Add(New SelectListItem() With {.Text = "Northwest Territories", .Value = "NT"})
            provinces.Add(New SelectListItem() With {.Text = "Nova Scotia", .Value = "NS"})
            provinces.Add(New SelectListItem() With {.Text = "Ontario", .Value = "ON"})
            provinces.Add(New SelectListItem() With {.Text = "Prince Edward Island", .Value = "PE"})
            provinces.Add(New SelectListItem() With {.Text = "Saskatchewan", .Value = "SK"})
            provinces.Add(New SelectListItem() With {.Text = "Nunavut", .Value = "NU"})
            provinces.Add(New SelectListItem() With {.Text = "Yukon", .Value = "YT"})

            ViewData("Provinces") = New SelectList(provinces, "Value", "Text")

            Return View()
        End Function

        <HttpPost()> _
        Public Function Subscribe(ByVal model As SubscribeLocalModel) As ActionResult
            Return View()
        End Function

        ''' <summary>
        ''' SubscribeRemote.aspx
        ''' </summary>
        ''' <param name="id">The product handle being subscribed to</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function SubscribeRemote(ByVal id As String) As ActionResult
            Return View()
        End Function

        <HttpPost()> _
        Public Function SubscribeRemote(ByVal model As SubscribeRemoteModel) As ActionResult
            Return View()
        End Function

    End Class
End Namespace