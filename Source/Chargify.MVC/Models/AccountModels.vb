Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Globalization

#Region "Services"
' The FormsAuthentication type is sealed and contains static members, so it is difficult to
' unit test code that calls its members. The interface and helper class below demonstrate
' how to create an abstract wrapper around such a type in order to make the AccountController
' code unit testable.

Public Interface IMembershipService
    ReadOnly Property MinPasswordLength() As Integer

    Function ValidateUser(ByVal userName As String, ByVal password As String) As Boolean
    Function CreateUser(ByVal userName As String, ByVal password As String, ByVal email As String) As MembershipCreateStatus
    Function ChangePassword(ByVal userName As String, ByVal oldPassword As String, ByVal newPassword As String) As Boolean
End Interface

Public Class AccountMembershipService
    Implements IMembershipService

    Private ReadOnly _provider As MembershipProvider

    Public Sub New()
        Me.New(Nothing)
    End Sub

    Public Sub New(ByVal provider As MembershipProvider)
        _provider = If(provider, Membership.Provider)
    End Sub

    Public ReadOnly Property MinPasswordLength() As Integer Implements IMembershipService.MinPasswordLength
        Get
            Return _provider.MinRequiredPasswordLength
        End Get
    End Property

    Public Function ValidateUser(ByVal userName As String, ByVal password As String) As Boolean Implements IMembershipService.ValidateUser
        ValidationUtil.ValidateRequiredStringValue(userName, "userName")
        ValidationUtil.ValidateRequiredStringValue(password, "password")

        Return _provider.ValidateUser(userName, password)
    End Function

    Public Function CreateUser(ByVal userName As String, ByVal password As String, ByVal email As String) As MembershipCreateStatus Implements IMembershipService.CreateUser
        ValidationUtil.ValidateRequiredStringValue(userName, "userName")
        ValidationUtil.ValidateRequiredStringValue(password, "password")
        ValidationUtil.ValidateRequiredStringValue(email, "email")

        Dim status As MembershipCreateStatus
        _provider.CreateUser(userName, password, email, Nothing, Nothing, True, Nothing, status)
        Return status
    End Function

    Public Function ChangePassword(ByVal userName As String, ByVal oldPassword As String, ByVal newPassword As String) As Boolean Implements IMembershipService.ChangePassword
        ValidationUtil.ValidateRequiredStringValue(userName, "userName")
        ValidationUtil.ValidateRequiredStringValue(oldPassword, "oldPassword")
        ValidationUtil.ValidateRequiredStringValue(newPassword, "newPassword")

        ' The underlying ChangePassword() will throw an exception rather
        ' than return false in certain failure scenarios.
        Try
            Dim currentUser As MembershipUser = _provider.GetUser(userName, True)
            Return currentUser.ChangePassword(oldPassword, newPassword)
        Catch ex As ArgumentException
            Return False
        Catch ex As MembershipPasswordException
            Return False
        End Try
    End Function
End Class

Public Interface IFormsAuthenticationService
    Sub SignIn(ByVal userName As String, ByVal createPersistentCookie As Boolean)
    Sub SignOut()
End Interface

Public Class FormsAuthenticationService
    Implements IFormsAuthenticationService

    Public Sub SignIn(ByVal userName As String, ByVal createPersistentCookie As Boolean) Implements IFormsAuthenticationService.SignIn
        ValidationUtil.ValidateRequiredStringValue(userName, "userName")

        FormsAuthentication.SetAuthCookie(userName, createPersistentCookie)
    End Sub

    Public Sub SignOut() Implements IFormsAuthenticationService.SignOut
        FormsAuthentication.SignOut()
    End Sub
End Class

Friend Class ValidationUtil
    Private Const _stringRequiredErrorMessage As String = "Value cannot be null or empty."

    Public Shared Sub ValidateRequiredStringValue(ByVal value As String, ByVal parameterName As String)
        If String.IsNullOrEmpty(value) Then
            Throw New ArgumentException(_stringRequiredErrorMessage, parameterName)
        End If
    End Sub
End Class
#End Region

#Region "Models"
<PropertiesMustMatch("NewPassword", "ConfirmPassword", ErrorMessage:="The new password and confirmation password do not match.")> _
Public Class ChangePasswordModel
    Private oldPasswordValue As String
    Private newPasswordValue As String
    Private confirmPasswordValue As String

    <Required()> _
    <DataType(DataType.Password)> _
    <DisplayName("Current password")> _
    Public Property OldPassword() As String
        Get
            Return oldPasswordValue
        End Get
        Set(ByVal value As String)
            oldPasswordValue = value
        End Set
    End Property

    <Required()> _
    <ValidatePasswordLength()> _
    <DataType(DataType.Password)> _
    <DisplayName("New password")> _
    Public Property NewPassword() As String
        Get
            Return newPasswordValue
        End Get
        Set(ByVal value As String)
            newPasswordValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Password)> _
    <DisplayName("Confirm new password")> _
    Public Property ConfirmPassword() As String
        Get
            Return confirmPasswordValue
        End Get
        Set(ByVal value As String)
            confirmPasswordValue = value
        End Set
    End Property
End Class

Public Class LogOnModel
    Private userNameValue As String
    Private passwordValue As String
    Private rememberMeValue As Boolean

    <Required()> _
    <DisplayName("User name")> _
    Public Property UserName() As String
        Get
            Return userNameValue
        End Get
        Set(ByVal value As String)
            userNameValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Password)> _
    <DisplayName("Password")> _
    Public Property Password() As String
        Get
            Return passwordValue
        End Get
        Set(ByVal value As String)
            passwordValue = value
        End Set
    End Property

    <DisplayName("Remember me?")> _
    Public Property RememberMe() As Boolean
        Get
            Return rememberMeValue
        End Get
        Set(ByVal value As Boolean)
            rememberMeValue = value
        End Set
    End Property
End Class

<PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessage:="The password and confirmation password do not match.")> _
Public Class RegisterModel
    Private userNameValue As String
    Private passwordValue As String
    Private confirmPasswordValue As String
    Private emailValue As String

    <Required()> _
    <DisplayName("User name")> _
    Public Property UserName() As String
        Get
            Return userNameValue
        End Get
        Set(ByVal value As String)
            userNameValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.EmailAddress)> _
    <DisplayName("Email address")> _
    Public Property Email() As String
        Get
            Return emailValue
        End Get
        Set(ByVal value As String)
            emailValue = value
        End Set
    End Property

    <Required()> _
    <ValidatePasswordLength()> _
    <DataType(DataType.Password)> _
    <DisplayName("Password")> _
    Public Property Password() As String
        Get
            Return passwordValue
        End Get
        Set(ByVal value As String)
            passwordValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Password)> _
    <DisplayName("Confirm password")> _
    Public Property ConfirmPassword() As String
        Get
            Return confirmPasswordValue
        End Get
        Set(ByVal value As String)
            confirmPasswordValue = value
        End Set
    End Property
End Class
#End Region

#Region "Validation"
<AttributeUsage(AttributeTargets.Class, AllowMultiple:=True, Inherited:=False)> _
Public NotInheritable Class PropertiesMustMatchAttribute
    Inherits ValidationAttribute

    Private Const _defaultErrorMessage As String = "'{0}' and '{1}' do not match."

    Private ReadOnly _confirmProperty As String
    Private ReadOnly _originalProperty As String
    Private ReadOnly _typeId As New Object()

    Public Sub New(ByVal originalProperty As String, ByVal confirmProperty As String)
        MyBase.New(_defaultErrorMessage)

        _originalProperty = originalProperty
        _confirmProperty = confirmProperty
    End Sub

    Public ReadOnly Property ConfirmProperty() As String
        Get
            Return _confirmProperty
        End Get
    End Property

    Public ReadOnly Property OriginalProperty() As String
        Get
            Return _originalProperty
        End Get
    End Property

    Public Overrides ReadOnly Property TypeId() As Object
        Get
            Return _typeId
        End Get
    End Property

    Public Overrides Function FormatErrorMessage(ByVal name As String) As String
        Return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString, OriginalProperty, ConfirmProperty)
    End Function

    Public Overrides Function IsValid(ByVal value As Object) As Boolean
        Dim properties As PropertyDescriptorCollection = TypeDescriptor.GetProperties(value)
        Dim originalValue = properties.Find(OriginalProperty, True).GetValue(value)
        Dim confirmValue = properties.Find(ConfirmProperty, True).GetValue(value)
        Return Object.Equals(originalValue, confirmValue)
    End Function
End Class

<AttributeUsage(AttributeTargets.Field Or AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)> _
Public NotInheritable Class ValidatePasswordLengthAttribute
    Inherits ValidationAttribute

    Private Const _defaultErrorMessage As String = "'{0}' must be at least {1} characters long."

    Private ReadOnly _minCharacters As Integer = Membership.Provider.MinRequiredPasswordLength

    Public Sub New()
        MyBase.New(_defaultErrorMessage)
    End Sub

    Public Overrides Function FormatErrorMessage(ByVal name As String) As String
        Return String.Format(CultureInfo.CurrentUICulture, ErrorMessageString, name, _minCharacters)
    End Function

    Public Overrides Function IsValid(ByVal value As Object) As Boolean
        Dim valueAsString As String = TryCast(value, String)
        Return (Not valueAsString Is Nothing) AndAlso (valueAsString.Length >= _minCharacters)
    End Function
End Class
#End Region
