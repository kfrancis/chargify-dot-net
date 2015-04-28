Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Globalization

#Region "Models"

<PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessage:="The password and confirmation password do not match.")> _
Public Class SubscribeRemoteModel

#Region "Private Members"

    Private userNameValue As String
    Private passwordValue As String
    Private confirmPasswordValue As String
    Private secretQuestionValue As String
    Private secretAnswerValue As String
    Private emailAddressValue As String

#End Region

#Region "Accessors"

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("Username")> _
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

    <Required()> _
    <DataType(DataType.Password)> _
    <DisplayName("Again?")> _
    Public Property ConfirmPassword() As String
        Get
            Return confirmPasswordValue
        End Get
        Set(ByVal value As String)
            confirmPasswordValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("Question")> _
    Public Property SecretQuestion() As String
        Get
            Return secretQuestionValue
        End Get
        Set(ByVal value As String)
            secretQuestionValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("Answer")> _
    Public Property SecretAnswer() As String
        Get
            Return secretAnswerValue
        End Get
        Set(ByVal value As String)
            secretAnswerValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.EmailAddress)> _
    <DisplayName("Email")> _
    Public Property EmailAddress() As String
        Get
            Return emailAddressValue
        End Get
        Set(ByVal value As String)
            emailAddressValue = value
        End Set
    End Property

#End Region

End Class

<PropertiesMustMatch("Password", "ConfirmPassword", ErrorMessage:="The password and confirmation password do not match.")> _
Public Class SubscribeLocalModel

#Region "Private Members"

    Private userNameValue As String
    Private passwordValue As String
    Private confirmPasswordValue As String
    Private secretQuestionValue As String
    Private secretAnswerValue As String

    Private emailAddressValue As String
    Private firstNameValue As String
    Private lastNameValue As String
    Private streetAddressValue As String
    Private cityValue As String
    Private provinceValue As IEnumerable(Of SelectListItem)
    Private countryValue As String
    Private postalCodeValue As String

    Private ccNumberValue As String
    Private ccvValue As String
    Private expirationMonthValue As Integer
    Private expirationYearValue As Integer

#End Region

#Region "Accessors"

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("Username")> _
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

    <Required()> _
    <DataType(DataType.Password)> _
    <DisplayName("Again?")> _
    Public Property ConfirmPassword() As String
        Get
            Return confirmPasswordValue
        End Get
        Set(ByVal value As String)
            confirmPasswordValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("Question")> _
    Public Property SecretQuestion() As String
        Get
            Return secretQuestionValue
        End Get
        Set(ByVal value As String)
            secretQuestionValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("Answer")> _
    Public Property SecretAnswer() As String
        Get
            Return secretAnswerValue
        End Get
        Set(ByVal value As String)
            secretAnswerValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.EmailAddress)> _
    <DisplayName("Email")> _
    Public Property EmailAddress() As String
        Get
            Return emailAddressValue
        End Get
        Set(ByVal value As String)
            emailAddressValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("First Name")> _
    Public Property FirstName() As String
        Get
            Return firstNameValue
        End Get
        Set(ByVal value As String)
            firstNameValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("Last Name")> _
    Public Property LastName() As String
        Get
            Return lastNameValue
        End Get
        Set(ByVal value As String)
            lastNameValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("Street Address")> _
    Public Property StreetAddress() As String
        Get
            Return streetAddressValue
        End Get
        Set(ByVal value As String)
            streetAddressValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("City")> _
    Public Property City() As String
        Get
            Return cityValue
        End Get
        Set(ByVal value As String)
            cityValue = value
        End Set
    End Property

    <Required()> _
    <DisplayName("Province")> _
    Public Property Province() As IEnumerable(Of SelectListItem)
        Get
            Return provinceValue
        End Get
        Set(ByVal value As IEnumerable(Of SelectListItem))
            provinceValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("Country")> _
    Public Property Country() As String
        Get
            Return countryValue
        End Get
        Set(ByVal value As String)
            countryValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("Postal Code")> _
    Public Property PostalCode() As String
        Get
            Return postalCodeValue
        End Get
        Set(ByVal value As String)
            postalCodeValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("CC Number")> _
    <DefaultValue("1")> _
    Public Property CreditCardNumber() As String
        Get
            Return ccNumberValue
        End Get
        Set(ByVal value As String)
            ccNumberValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("CCV")> _
    <StringLength(3)> _
    Public Property CCV() As String
        Get
            Return ccvValue
        End Get
        Set(ByVal value As String)
            ccvValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("Exp. Month")> _
    Public Property ExpirationMonth() As Integer
        Get
            Return expirationMonthValue
        End Get
        Set(ByVal value As Integer)
            expirationMonthValue = value
        End Set
    End Property

    <Required()> _
    <DataType(DataType.Text)> _
    <DisplayName("Exp. Year")> _
    Public Property ExpirationYear() As Integer
        Get
            Return expirationYearValue
        End Get
        Set(ByVal value As Integer)
            expirationYearValue = value
        End Set
    End Property

#End Region

End Class

#End Region

Public Class Province

    Private nameValue As String
    Private provinceCodeValue As String

    Public Property Name() As String
        Get
            Return nameValue
        End Get
        Set(ByVal value As String)
            nameValue = value
        End Set
    End Property

    Public Property ProvinceCode() As String
        Get
            Return provinceCodeValue
        End Get
        Set(ByVal value As String)
            provinceCodeValue = value
        End Set
    End Property

End Class