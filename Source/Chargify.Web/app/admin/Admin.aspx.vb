#Region "Author Note"
' Admin.aspx/Admin.aspx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Imports System.Data
Imports ChargifyNET

Partial Class app_admin_Admin
    Inherits ChargifyPage

#Region "Page Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then

            usersGv.DataSource = GetDataSource()
            usersGv.DataBind()

        End If
    End Sub

#End Region

#Region "Data Methods"

    Private Function GetDataSource() As DataSet
        Dim ds As New DataSet
        Dim dt As New DataTable()
        dt.Columns.Add("UserID", GetType(Guid))
        dt.Columns.Add("UserName", GetType(String))
        dt.Columns.Add("LastActivityDate", GetType(DateTime))
        dt.Columns.Add("Customer", GetType(Boolean))

        Dim muc As MembershipUserCollection = Membership.GetAllUsers()
        For Each u As MembershipUser In muc

            Dim isCustomer As Boolean = False
            Dim customer As ICustomer = Chargify.LoadCustomer(u.ProviderUserKey.ToString())
            If customer IsNot Nothing Then
                isCustomer = True
            End If
            dt.Rows.Add(u.ProviderUserKey.ToString(), u.UserName, u.LastActivityDate, isCustomer)
        Next

        ds.Tables.Add(dt)
        Return ds

    End Function

#End Region

#Region "Control Methods"

    Protected Sub clearUsersBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles clearUsersBtn.Click
        Dim usersDeleted As Boolean = False
        Dim systemUsers As MembershipUserCollection = Membership.GetAllUsers()

        For Each systemUser As MembershipUser In systemUsers

            If Membership.DeleteUser(systemUser.UserName, True) Then
                ' No issue
            Else
                ' Issue
            End If

        Next

        ' This will delete EVERY user in the system, including the admin. So add the admin back into the system ..
        CreateAdministratorUser()
        CreateRegularUserWithSubscription()

        ' Now log the administrator out, as they'll need to signin again.
        Dim url As String = ResolveUrl("~/Logout.aspx")
        Response.Redirect(url)
    End Sub

    Private Sub CreateAdministratorUser()
        Dim userStatus As MembershipCreateStatus
        Dim adminUser As MembershipUser = Membership.CreateUser("admin", "admin", "kfrancis@clinicalsupportsystems.com", "What is not red", "Blue", True, userStatus)
        If userStatus <> MembershipCreateStatus.Success Then
            Throw New Exception(userStatus.ToString())
        Else
            Roles.AddUserToRole(adminUser.UserName, "Administrator")
        End If
    End Sub

    Private Sub CreateRegularUserWithSubscription()

        Dim userStatus As MembershipCreateStatus
        Dim regularUser As MembershipUser = Membership.CreateUser("user", "password", "kfrancis@clinicalsupportsystems.com", "What is not red", "Blue", True, userStatus)
        If userStatus <> MembershipCreateStatus.Success Then
            Throw New Exception(userStatus.ToString())
        Else
            Roles.AddUserToRole(regularUser.UserName, "User")
        End If

        Dim customerInformation As New CustomerAttributes()
        customerInformation.FirstName = "Test"
        customerInformation.LastName = "User"
        customerInformation.Email = ConfigurationManager.AppSettings("VALID_EMAIL")
        ' Create a new guid, this would be the Membership UserID if we were creating a new user simultaneously
        customerInformation.SystemID = regularUser.ProviderUserKey.ToString()

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

        Dim newSubscription As ISubscription = Chargify.CreateSubscription("basic", customerInformation, creditCardInfo)
    End Sub

    Protected Sub usersGv_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles usersGv.PageIndexChanging
        usersGv.PageIndex = e.NewPageIndex
        usersGv.DataSource = GetDataSource()
        usersGv.DataBind()
    End Sub


    Protected Sub usersGv_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles usersGv.RowCommand
        If e.CommandName = "DeleteMe" Then
            Dim arg As String = e.CommandArgument.ToString()
            If arg.IsGuid() Then
                Dim userID As Guid = New Guid(arg)

                Dim user As MembershipUser = Membership.GetUser(userID)
                If user IsNot Nothing Then

                    Dim customer As ICustomer = Chargify.LoadCustomer(userID.ToString())
                    If customer IsNot Nothing Then

                        ' As of now, Chargify has the method for deleting a customer - but it will always return 'Forbidden' and thus fail.
                        ' To delete the associated customers, you'll need to log into your site via chargify.com and delete them through 
                        ' their administration tool.
                        If Chargify.DeleteCustomer(customer.ChargifyID) = False Then
                            ' Since we can't delete the customer, just leave ..

                            Page.ClientScript.RegisterStartupScript(GetType(Page), "notice", "alert('Temporarily unavailable as the Chargify customer cannot be deleted through the API. Yet.');", True)

                            Exit Sub
                        End If

                    End If

                    ' Finally, delete the membership user 
                    Membership.DeleteUser(user.UserName, True)

                End If

            End If
        ElseIf e.CommandName = "EditMe" Then
            Dim arg As String = e.CommandArgument.ToString()
            If arg.IsGuid() Then
                Dim appDirectory As String = ConfigurationManager.AppSettings("SECURE_APPLICATION_DIRECTORY")
                Dim url As String = ResolveUrl("~/" & appDirectory & "/admin/User.aspx?uid=" & arg)
                Response.Redirect(url)
            End If
        End If
    End Sub

#End Region

#Region "Navigation Methods"

    Protected Sub logoutBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles logoutBtn.Click
        Dim url As String = ResolveUrl("~/Logout.aspx")
        Response.Redirect(url)
    End Sub

#End Region

End Class
