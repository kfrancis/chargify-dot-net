Imports System
Imports System.Data.Entity
Imports System.Data.Entity.Migrations
Imports System.Linq
Imports ChargifyNET
Imports ChargifyNET.Configuration
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework

Namespace Migrations

    Friend NotInheritable Class Configuration 
        Inherits DbMigrationsConfiguration(Of ApplicationDbContext)

        Public Sub New()
            AutomaticMigrationsEnabled = False
        End Sub

        Protected Overrides Sub Seed(context As ApplicationDbContext)
            '  This method will be called after migrating to the latest version.
            AddUsersAndRoles()

            '  You can use the DbSet(Of T).AddOrUpdate() helper extension method 
            '  to avoid creating duplicate seed data. E.g.
            '
            '    context.People.AddOrUpdate(
            '       Function(c) c.FullName,
            '       New Customer() With {.FullName = "Andrew Peters"},
            '       New Customer() With {.FullName = "Brice Lambson"},
            '       New Customer() With {.FullName = "Rowan Miller"})
        End Sub

        ''' <summary>
        ''' Method for automatically creating the base users with the proper roles, and a sample subscription for the user
        ''' </summary>
        ''' <returns>The result of the operation</returns>
        Private Function AddUsersAndRoles() As Boolean
            Dim ir As IdentityResult
            Dim rm As New RoleManager(Of IdentityRole)(New RoleStore(Of IdentityRole)(New ApplicationDbContext()))
            ir = rm.Create(New IdentityRole("Administrator"))
            ir = rm.Create(New IdentityRole("User"))

            Dim um As New UserManager(Of ApplicationUser)(New UserStore(Of ApplicationUser)(New ApplicationDbContext()))

            Dim adminUser As New ApplicationUser() With {.UserName = "admin"}
            If um.FindByName(adminUser.UserName) Is Nothing Then
                ir = um.Create(adminUser, "admin123")
                If Not ir.Succeeded Then
                    Return ir.Succeeded
                End If
            Else
                adminUser = um.FindByName("admin")
            End If

            If adminUser IsNot Nothing And um.IsInRole(adminUser.Id, "Administrator") = False Then
                ir = um.AddToRole(adminUser.Id, "Administrator")
                If Not ir.Succeeded Then
                    Return ir.Succeeded
                End If
            End If

            Dim regularUser As New ApplicationUser() With {.UserName = "user"}
            If um.FindByName(regularUser.UserName) Is Nothing Then
                ir = um.Create(regularUser, "user123")
                If Not ir.Succeeded Then
                    Return ir.Succeeded
                End If
            Else
                regularUser = um.FindByName("user")
            End If

            If regularUser IsNot Nothing And um.IsInRole(regularUser.Id, "User") = False Then
                ir = um.AddToRole(regularUser.Id, "User")
                If Not ir.Succeeded Then
                    Return ir.Succeeded
                End If
            End If

            Try
                Dim customerInformation As New CustomerAttributes()
                customerInformation.FirstName = regularUser.UserName + " first"
                customerInformation.LastName = regularUser.UserName + " last"
                customerInformation.Email = ConfigurationManager.AppSettings("VALID_EMAIL")
                customerInformation.SystemID = regularUser.Id.ToString()

                Dim creditCardInfo As New CreditCardAttributes()
                creditCardInfo.FullNumber = "1"
                creditCardInfo.CVV = "123"
                creditCardInfo.ExpirationMonth = Date.Now.Month
                creditCardInfo.ExpirationYear = Date.Now.Year + 2

                creditCardInfo.BillingAddress = "123 Main St."
                creditCardInfo.BillingCity = "Kingston"
                creditCardInfo.BillingState = "ON"
                creditCardInfo.BillingZip = "H0H 0H0"
                creditCardInfo.BillingCountry = "Canada"

                Dim product = Chargify.GetProductList().FirstOrDefault(Function(p) p.Value.PriceInCents > 0)
                Dim newSubscription = Chargify.CreateSubscription(product.Value.Handle, customerInformation, creditCardInfo)
            Catch ex As Exception
                Elmah.ErrorLog.GetDefault(Nothing).Log(New Elmah.[Error](ex))
                Return False
            End Try

            Return True
        End Function

        Public ReadOnly Property Chargify() As ChargifyConnect
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

End Namespace
