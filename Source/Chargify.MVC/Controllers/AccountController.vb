Imports System.Diagnostics.CodeAnalysis
Imports System.Security.Principal
Imports System.Web.Routing

<HandleError()> _
Public Class AccountController
    Inherits System.Web.Mvc.Controller

    Private ReadOnly _formsService As IFormsAuthenticationService
    Private ReadOnly _membershipService As IMembershipService

    ' This constructor is used by the MVC framework to instantiate the controller using
    ' the default forms authentication and membership providers.
    Public Sub New()
        Me.New(Nothing, Nothing)
    End Sub

    ' This constructor is not used by the MVC framework but is instead provided for ease
    ' of unit testing this type. See the comments in AccountModels.vb for more information.
    Public Sub New(ByVal formsService As IFormsAuthenticationService, ByVal membershipService As IMembershipService)
        _formsService = If(formsService, New FormsAuthenticationService)
        _membershipService = If(membershipService, New AccountMembershipService())
    End Sub

    Public ReadOnly Property FormsService() As IFormsAuthenticationService
        Get
            Return _formsService
        End Get
    End Property

    Public ReadOnly Property MembershipService() As IMembershipService
        Get
            Return _membershipService
        End Get
    End Property

    Protected Overrides Sub Initialize(ByVal requestContext As RequestContext)
        If TypeOf requestContext.HttpContext.User.Identity Is WindowsIdentity Then
            Throw New InvalidOperationException("Windows authentication is not supported.")
        Else
            MyBase.Initialize(requestContext)
        End If
    End Sub

    Protected Overrides Sub OnActionExecuting(ByVal filterContext As ActionExecutingContext)
        ViewData("PasswordLength") = MembershipService.MinPasswordLength

        MyBase.OnActionExecuting(filterContext)
    End Sub

    <Authorize()> _
    Public Function ChangePassword() As ActionResult
        Return View()
    End Function

    <Authorize()> _
    <HttpPost()> _
    Public Function ChangePassword(ByVal model As ChangePasswordModel) As ActionResult
        If ModelState.IsValid Then
            If MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword) Then
                Return RedirectToAction("ChangePasswordSuccess")
            Else
                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.")
            End If
        End If

        ' If we got this far, something failed, redisplay form
        Return View(model)
    End Function

    Public Function ChangePasswordSuccess() As ActionResult
        Return View()
    End Function

    Public Function LogOff() As ActionResult
        FormsService.SignOut()

        Return RedirectToAction("Index", "Home")
    End Function

    Public Function LogOn() As ActionResult
        Return View()
    End Function

    <HttpPost()> _
    <SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", _
            Justification:="Needs to take same parameter type as Controller.Redirect()")> _
    Public Function LogOn(ByVal model As LogOnModel, ByVal returnUrl As String) As ActionResult
        If ModelState.IsValid Then
            If MembershipService.ValidateUser(model.UserName, model.Password) Then
                FormsService.SignIn(model.UserName, model.RememberMe)
                If Not String.IsNullOrEmpty(returnUrl) Then
                    Return Redirect(returnUrl)
                Else
                    Return RedirectToAction("Dashboard", "Application")
                End If
            Else
                ModelState.AddModelError("", "The user name or password provided is incorrect.")
            End If
        End If

        ' If we got this far, something failed, redisplay form
        Return View(model)
    End Function

    Public Function Register() As ActionResult
        Return View()
    End Function

    <HttpPost()> _
    Public Function Register(ByVal model As RegisterModel) As ActionResult
        If ModelState.IsValid Then
            ' Attempt to register the user
            Dim createStatus As MembershipCreateStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email)

            If createStatus = MembershipCreateStatus.Success Then
                FormsService.SignIn(model.UserName, False)
                Return RedirectToAction("Index", "Home")
            Else
                ModelState.AddModelError("", ErrorCodeToString(createStatus))
            End If
        End If

        ' If we got this far, something failed, redisplay form
        Return View(model)
    End Function

    Private Shared Function ErrorCodeToString(ByVal createStatus As MembershipCreateStatus) As String
        ' See http://go.microsoft.com/fwlink/?LinkID=177550 for
        ' a full list of status codes.
        Select Case createStatus
            Case MembershipCreateStatus.DuplicateUserName
                Return "Username already exists. Please enter a different user name."

            Case MembershipCreateStatus.DuplicateEmail
                Return "A username for that e-mail address already exists. Please enter a different e-mail address."

            Case MembershipCreateStatus.InvalidPassword
                Return "The password provided is invalid. Please enter a valid password value."

            Case MembershipCreateStatus.InvalidEmail
                Return "The e-mail address provided is invalid. Please check the value and try again."

            Case MembershipCreateStatus.InvalidAnswer
                Return "The password retrieval answer provided is invalid. Please check the value and try again."

            Case MembershipCreateStatus.InvalidQuestion
                Return "The password retrieval question provided is invalid. Please check the value and try again."

            Case MembershipCreateStatus.InvalidUserName
                Return "The user name provided is invalid. Please check the value and try again."

            Case MembershipCreateStatus.ProviderError
                Return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator."

            Case MembershipCreateStatus.UserRejected
                Return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator."

            Case Else
                Return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator."
        End Select

    End Function
End Class
