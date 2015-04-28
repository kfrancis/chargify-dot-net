<%@ Page Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage(Of Chargify.MVC.SubscribeLocalModel)" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphMainContent" runat="server">

    <p>Please enter the following information and click '<strong>Place My Order</strong>' to complete the signup process.</p>
    <% Using Html.BeginForm() %>
        <%= Html.ValidationSummary(True, "Please correct the errors and try again.")%>
        <div class="yform columnar">
            <div id="errorMessage" class="warning" style="visibility: hidden; display: none;">
                <h3>Uh Oh!</h3>
                <span>&nbsp;</span>
            </div>
            <fieldset>
                <legend>User Info</legend>
                <div class="type-text">
                    <%= Html.LabelFor(Function(m) m.UserName)%>
                    <%= Html.TextBoxFor(Function(m) m.UserName, New With {.class = "required", .minlength = "5"})%>
                </div>
                <div class="type-text">
                    <%= Html.LabelFor(Function(m) m.Password)%>
                    <%= Html.PasswordFor(Function(m) m.Password, New With {.class = "required", .minlength = "5"})%>
                </div>
                <div class="type-text">
                    <%= Html.LabelFor(Function(m) m.ConfirmPassword)%>
                    <%= Html.PasswordFor(Function(m) m.ConfirmPassword, New With {.class = "required"})%>
                </div>
                <div class="type-text">
                    <%= Html.LabelFor(Function(m) m.SecretQuestion)%>
                    <%= Html.TextBoxFor(Function(m) m.SecretQuestion, New With {.class = "required"})%>
                </div>
                <div class="type-text">
                    <%= Html.LabelFor(Function(m) m.SecretAnswer)%>
                    <%= Html.TextBoxFor(Function(m) m.SecretAnswer, New With {.class = "required"})%>
                </div>
            </fieldset>
            <fieldset>
                <legend>Billing Info</legend>
                <div class="type-text">
                    <%= Html.LabelFor(Function(m) m.EmailAddress)%>
                    <%= Html.TextBoxFor(Function(m) m.EmailAddress, New With {.class = "required"})%>
                </div>
                <div class="type-text">
                    <%= Html.LabelFor(Function(m) m.FirstName)%>
                    <%= Html.TextBoxFor(Function(m) m.FirstName, New With {.class = "required"})%>
                </div>
                <div class="type-text">
                    <%= Html.LabelFor(Function(m) m.LastName)%>
                    <%= Html.TextBoxFor(Function(m) m.LastName, New With {.class = "required"})%>
                </div>
                <div class="type-text">
                    <%= Html.LabelFor(Function(m) m.StreetAddress)%>
                    <%= Html.TextBoxFor(Function(m) m.StreetAddress, New With {.class = "required"})%>
                </div>
                <div class="type-text">
                    <%= Html.LabelFor(Function(m) m.City)%>
                    <%= Html.TextBoxFor(Function(m) m.City, New With {.class = "required"})%>>
                </div>
                <div class="type-select">
                    <%= Html.LabelFor(Function(m) m.Province)%>
                    <%= Html.DropDownListFor(Function(m) m.Province, DirectCast(ViewData("Provinces"), SelectList), "-- Please Select --", New With {.class = "required"})%>
                </div>
                <div class="type-select">
                    <asp:Label ID="countryLbl" runat="server" AssociatedControlID="countryDDL" Text="Country"></asp:Label>
                    <asp:DropDownList ID="countryDDL" runat="server" CssClass="required">
                        <asp:ListItem Text="----" Value="" />
                        <asp:ListItem Value="Canada" Text="Canada" Selected="True" />
                    </asp:DropDownList>
                </div>
                <div class="type-text">
                    <asp:Label ID="postalCodeLbl" runat="server" AssociatedControlID="postalCodeTb" Text="Postal Code"></asp:Label>
                    <asp:TextBox ID="postalCodeTb" runat="server" CssClass="required"></asp:TextBox>
                </div>
            </fieldset>
            <fieldset>
                <legend>Credit Card Details</legend>           
                <div class="subcolumns">
                    <div class="c50l">
                        <div class="subcl type-text">
                            <%= Html.LabelFor(Function(m) m.CreditCardNumber)%>
                            <%= Html.TextBoxFor(Function(m) m.CreditCardNumber, New With {.class = "required"})%>
                        </div>
                        <div class="subcl type-select">
                            <asp:Label ID="expirationMonthLbl" runat="server" AssociatedControlID="expirationMonthDDL" Text="Exp. Month"></asp:Label>
                            <asp:DropDownList ID="expirationMonthDDL" runat="server" CssClass="required">
                                <asp:ListItem Text="----" Value="" />
                                <asp:ListItem Text="January" Value="1" />
                                <asp:ListItem Text="February" Value="2" />
                                <asp:ListItem Text="March" Value="3" />
                                <asp:ListItem Text="April" Value="4" />
                                <asp:ListItem Text="May" Value="5" />
                                <asp:ListItem Text="June" Value="6" />
                                <asp:ListItem Text="July" Value="7" />
                                <asp:ListItem Text="August" Value="8" />
                                <asp:ListItem Text="September" Value="9" />
                                <asp:ListItem Text="October" Value="10" />
                                <asp:ListItem Text="November" Value="11" />
                                <asp:ListItem Text="December" Value="12" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="c50r">
                        <div class="subcr type-text">
                            <asp:Label ID="cvvLbl" runat="server" AssociatedControlID="cvvTb" Text="CCV"></asp:Label>
                            <asp:TextBox ID="cvvTb" runat="server" CssClass="required" minlength="3" MaxLength="3"></asp:TextBox>
                        </div>
                         <div class="subcr type-select">
                            <asp:Label ID="expirationYearLbl" runat="server" AssociatedControlID="expirationYearDDL" Text="Exp. Year"></asp:Label>
                            <asp:DropDownList ID="expirationYearDDL" runat="server" CssClass="required" AppendDataBoundItems="true" DataTextField="Year" DataValueField="Year">
                                <asp:ListItem Text="Select One" Value="" Selected="True" />
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </fieldset>
            <div class="type-button">
                <input type="submit" value="Place My Order" class="submit" />
            </div>
        </div>
    <% End Using %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphTitleContent" runat="server">
    Subscribe
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphHead" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphMenu" runat="server">
    <div class="hlist">
	    <ul>
		    <li><%= Html.ActionLink("Home", "Index", "Home")%></li>
		    <li><%= Html.ActionLink("Plans", "Index")%></li>
		    <li class="active"><strong>Subscribe</strong></li>
	    </ul>
    </div>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="cphLeftContent" runat="server">
    <h2><asp:Literal ID="planNameLtr" runat="server"></asp:Literal> Plan</h2>
    <div class="info">
        <p>On this page, the user is required to complete the form which allows the system to both create a user, and create a corresponding customer in the associated Chargify account.</p>
        <p>When the user clicks on the 'Place My Order' button, the system will create an ASP.NET membership user first, then create the Chargify customer second.</p>
        <p><i>You can simulate a non-working credit card by using '2' as the number instead of the default '1' ...</i></p>
    </div>
</asp:Content>
