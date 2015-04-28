#Region "Author Note"
' Subscribe.aspx/Subscribe.aspx.vb
' Author: Kori Francis <twitter.com/djbyter>
#End Region

Imports ChargifyNET
Imports System.Data
Imports System.Web.Services
Imports System.Web.Script.Services

Partial Class Subscribe
    Inherits ChargifyPage

#Region "Page Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            InitData()

            Dim plan As String = Request.QueryString("plan")

            ' Only allow the user to stay on this page IF the Product handle is correct
            If Not String.IsNullOrEmpty(plan) Then
                If IsChargifyProduct(plan) Then
                    Dim planName As String = UCase(Left(plan, 1)) & LCase(Mid(plan, 2))
                    Me.planNameLtr.Text = planName

                    ' If the plan requires a credit card, then display this fieldset to the user.
                    Me.ccDetailsPnl.Visible = RequireCreditCard(plan)

                    ' If the plan requires a billing address, then display the fields to the user.
                    Me.billingAddressPnl.Visible = RequireBillingAddress(plan)

                Else
                    Response.Redirect(ResolveUrl("~/Plans.aspx"))
                End If
            Else
                Response.Redirect(ResolveUrl("~/Plans.aspx"))
            End If

        End If
    End Sub

#End Region

#Region "Data Methods"

    Private Sub InitData()
        Dim dt As New DataTable()
        dt.Columns.Add("Year", GetType(String))
        For i As Integer = 0 To 10
            Dim year As String = (Date.Today.Year + i).ToString()
            dt.Rows.Add(year)
        Next
        Me.expirationYearDDL.DataSource = dt
        Me.expirationYearDDL.DataBind()
    End Sub

#End Region

#Region "Utility Methods"

    Private Function IsChargifyProduct(ByVal productHandle As String) As Boolean
        Dim result = From a In Chargify.GetProductList().Values _
                     Where a.Handle = productHandle _
                     Select a

        If result IsNot Nothing Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function RequireCreditCard(ByVal productHandle As String) As Boolean
        Dim result As IProduct = (From a In Chargify.GetProductList().Values _
                                  Where a.Handle = productHandle _
                                  Select a).SingleOrDefault()
        If result IsNot Nothing Then
            Return result.RequireCreditCard
        Else
            Throw New ArgumentException("Not a valid product")
        End If
    End Function

    Private Function RequireBillingAddress(ByVal productHandle As String) As Boolean
        Dim result As IProduct = (From a In Chargify.GetProductList().Values _
                                  Where a.Handle = productHandle _
                                  Select a).SingleOrDefault()
        If result IsNot Nothing Then
            ' TODO: There's currently no require_billing_address, or request_billing_address field in API->Product
            ' so for now, just always request it.
            Return True
        Else
            Throw New ArgumentException("Not a valid product")
        End If
    End Function

#End Region

#Region "Control Methods"

    Protected Sub submitBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles submitBtn.Click
        Try
            ' Try and create the system user
            Dim userStatus As MembershipCreateStatus
            Dim user As MembershipUser = Membership.CreateUser(Me.usernameTb.Text, Me.passwordTb.Text, Me.emailTb.Text, Me.secretQuestionTb.Text, Me.secretAnswerTb.Text, True, userStatus)
            If userStatus <> MembershipCreateStatus.Success Then
                Throw New Exception(userStatus.ToString())
            Else
                Roles.AddUserToRole(user.UserName, "User")
            End If

            If user.ProviderUserKey = Guid.Empty Then
                Throw New Exception("UserID Empty!")
            End If

            ' Alright, start filling in some information for Chargify

            Dim customerInformation As New CustomerAttributes()
            customerInformation.FirstName = Me.fNameTb.Text
            customerInformation.LastName = Me.lNameTb.Text
            customerInformation.Email = Me.emailTb.Text
            ' Create a new guid, this would be the Membership UserID if we were creating a new user simultaneously
            customerInformation.SystemID = user.ProviderUserKey.ToString()

            Dim productHandle As String = Request.QueryString("plan")
            Dim newSubscription As ISubscription = Nothing

            If Me.ccDetailsPnl.Visible Then
                ' Since the panel is visible, then the cc info is required.
                Dim creditCardInfo As New CreditCardAttributes()
                creditCardInfo.FullNumber = Me.creditCardNumberTb.Text
                creditCardInfo.CVV = Me.cvvTb.Text
                creditCardInfo.ExpirationMonth = CInt(Me.expirationMonthDDL.SelectedValue)
                creditCardInfo.ExpirationYear = CInt(Me.expirationYearDDL.SelectedValue)

                creditCardInfo.BillingAddress = Me.addressTb.Text
                creditCardInfo.BillingCity = Me.cityTb.Text
                creditCardInfo.BillingState = Me.provinceDDL.SelectedValue
                creditCardInfo.BillingZip = Me.postalCodeTb.Text
                creditCardInfo.BillingCountry = Me.countryDDL.SelectedValue

                newSubscription = Chargify.CreateSubscription(productHandle, customerInformation, creditCardInfo)
            Else
                ' CC info not required, make different call.
                newSubscription = Chargify.CreateSubscription(productHandle, customerInformation)
            End If

            Dim url As String = ResolveUrl("~/SubscribeResult.aspx?r=true&p=" & productHandle)
            Response.Redirect(url)

        Catch ex As Exception
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error", "alert('" & ex.Message & "');", True)
        End Try
    End Sub

#End Region

#Region "Web Methods"

    <WebMethod()> _
    <ScriptMethod()> _
    Public Shared Function IsCouponValid(ByVal couponCode As String) As Boolean
        ' Use the clsCommon shared method for use in webmethods, just so you don't need to write all that code
        ' over and over again.
        Dim productFamilyID As Integer = CInt(ConfigurationManager.AppSettings("PRODUCT_FAMILY"))
        Dim chargify As ChargifyConnect = clsCommon.Chargify
        Dim foundCoupon As ICoupon = chargify.FindCoupon(productFamilyID, couponCode)
        If foundCoupon IsNot Nothing Then
            If foundCoupon.EndDate = DateTime.MinValue Then
                ' Coupon had no exp date, always exists.
                Return True
            ElseIf Date.Now > foundCoupon.EndDate Then
                ' Coupon Exists but has ended
                Return False
            End If
            ' Coupon exists and still valid
            Return True
        Else
            ' Coupon not found
            Return False
        End If
    End Function

#End Region

End Class
