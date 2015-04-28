<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="Subscribe.aspx.vb" Inherits="Subscribe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
    <style type="text/css">
        .columnar div.type-text span { margin-left: 30%; }
        .goodCoupon { color: Green; }
        .badCoupon { color: Red; }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <!-- This menu is static on purpose, since the Subscribe page should only appear when you are on this page. -->
    <div class="hlist">
	    <ul>
		    <li><a href="default.aspx">Home</a></li>
		    <li><a href="Plans.aspx">Plans</a></li>
		    <li class="active"><strong>Subscribe</strong></li>
	    </ul>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
    <h2><asp:Literal ID="planNameLtr" runat="server"></asp:Literal> Plan</h2>
    <div class="info">
        <p>On this page, the user is required to complete the form which allows the system to both create a user, and create a corresponding customer in the associated Chargify account.</p>
        <p>When the user clicks on the 'Place My Order' button, the system will create an ASP.NET membership user first, then create the Chargify customer second.</p>
        <p><i>You can simulate a non-working credit card by using '2' as the number instead of the default '1' ...</i></p>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <p>Please enter the following information and click '<strong>Place My Order</strong>' to complete the signup process.</p>
    <div class="yform columnar">
        <div id="errorMessage" class="warning">
            <h3>Uh Oh!</h3>
            <span>&nbsp;</span>
        </div>
        <fieldset>
            <legend>User Info</legend>
            <div class="type-text">
                <asp:Label ID="usernameLbl" runat="server" AssociatedControlID="usernameTb" Text="Username"></asp:Label>
                <asp:TextBox ID="usernameTb" runat="server" CssClass="required" minlength="5"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="passwordLbl" runat="server" AssociatedControlID="passwordTb" Text="Password"></asp:Label>
                <asp:TextBox ID="passwordTb" runat="server" TextMode="Password" CssClass="required" minlength="5"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="passwordConfirmLbl" runat="server" AssociatedControlID="passwordConfirmTb" Text="Again?"></asp:Label>
                <asp:TextBox ID="passwordConfirmTb" runat="server" TextMode="Password"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="secretQuestionLbl" runat="server" AssociatedControlID="secretQuestionTb" Text="Question"></asp:Label>
                <asp:TextBox ID="secretQuestionTb" runat="server" CssClass="required"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="secretAnswerLbl" runat="server" AssociatedControlID="secretAnswerTb" Text="Answer"></asp:Label>
                <asp:TextBox ID="secretAnswerTb" runat="server" CssClass="required"></asp:TextBox>
            </div>
        </fieldset>
        <fieldset>
            <legend>Personal Info</legend>
            <div class="type-text">
                <asp:Label ID="emailLbl" runat="server" AssociatedControlID="emailTb" Text="Email"></asp:Label>
                <asp:TextBox ID="emailTb" runat="server" CssClass="required email"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="fNameLbl" runat="server" AssociatedControlID="fNameTb" Text="First Name"></asp:Label>
                <asp:TextBox ID="fNameTb" runat="server" CssClass="required"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="lNameLbl" runat="server" AssociatedControlID="lNameTb" Text="Last Name"></asp:Label>
                <asp:TextBox ID="lNameTb" runat="server" CssClass="required"></asp:TextBox>
            </div>
        </fieldset>
        <asp:Panel ID="billingAddressPnl" runat="server" Visible="false">
            <fieldset>
                <legend>Billing Address</legend>
                <div class="type-text">
                    <asp:Label ID="addressLbl" runat="server" AssociatedControlID="addressTb" Text="Street Address"></asp:Label>
                    <asp:TextBox ID="addressTb" runat="server" CssClass="required"></asp:TextBox>
                </div>
                <div class="type-text">
                    <asp:Label ID="cityLbl" runat="server" AssociatedControlID="cityTb" Text="City"></asp:Label>
                    <asp:TextBox ID="cityTb" runat="server" CssClass="required"></asp:TextBox>
                </div>
                <div class="type-select">
                    <asp:Label ID="provinceLbl" runat="server" AssociatedControlID="provinceDDL" Text="Province"></asp:Label>
                    <asp:DropDownList ID="provinceDDL" runat="server" CssClass="required">
                        <asp:ListItem Text="----" Value="" />
                        <asp:ListItem Value="AB" Text="Alberta" />
                        <asp:ListItem Value="BC" Text="British Columbia" />
                        <asp:ListItem Value="MB" Text="Manitoba" />
                        <asp:ListItem Value="NL" Text="Newfoundland and Labrador" />
                        <asp:ListItem Value="NB" Text="New Brunswick" />
                        <asp:ListItem Value="NT" Text="Northwest Territories" />
                        <asp:ListItem Value="NS" Text="Nova Scotia" />
                        <asp:ListItem Value="ON" Text="Ontario" />
                        <asp:ListItem Value="PE" Text="Prince Edward Island" />
                        <asp:ListItem Value="SK" Text="Saskatchewan" />
                        <asp:ListItem Value="NU" Text="Nunavut" />
                        <asp:ListItem Value="YT" Text="Yukon" />
                    </asp:DropDownList>
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
        </asp:Panel>
        <fieldset>
            <legend>Coupons?</legend>
            <div class="type-text">
                <asp:Label ID="couponLbl" runat="server" AssociatedControlID="couponTb" Text="Coupon Code"></asp:Label>
                <asp:TextBox ID="couponTb" runat="server"></asp:TextBox>
                <span id="msg">TEST123 always validates (no exp), EXPTEST123 is a coupon, but expired.</span>
            </div>
        </fieldset>
        <asp:Panel ID="ccDetailsPnl" runat="server" Visible="false">
            <fieldset>
            <legend>Credit Card Details</legend>           
            <div class="subcolumns">
                <div class="c50l">
                    <div class="subcl type-text">
                        <asp:Label ID="creditCardNumberLbl" runat="server" AssociatedControlID="creditCardNumberTb" Text="CC Number"></asp:Label>
                        <asp:TextBox ID="creditCardNumberTb" runat="server" Text="1" CssClass="required"></asp:TextBox>
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
        </asp:Panel>
        <div class="type-option">
            <asp:Button ID="submitBtn" CssClass="button green large" runat="server" Text="Place My Order" />
        </div>
    </div>
    <script type="text/javascript">
        var myValidator = null;
        $(document).ready(function () {
            $("#errorMessage").hide();
            $myForm = $("#aspnetForm"); // instance of myform for faster dom-select
            myValidator = $myForm.validate({
                invalidHandler: function (e, validator) {
                    var errors = validator.numberOfInvalids();
                    if (errors) {
                        var message = errors == 1
                                    ? 'You missed 1 field. It has been highlighted'
                                    : 'You missed ' + errors + ' fields.  They have been highlighted';
                        $("#errorMessage span").html(message);
                        $(".comment").hide();
                        $("#errorMessage").show();
                    } else {
                        $(".comment").show();
                        $("#errorMessage").hide();
                    }
                },
                errorPlacement: function (error, element) {
                    element.parent().addClass("error");
                    error.prependTo(element.parent());
                },
                errorClass: "message",
                errorElement: "strong",
                onkeyup: false, // disable it - make some problems with errorPlacement 
                onclick: false,  // disable it - make some problems with errorPlacement
                rules: {
                    '<%= passwordConfirmTb.UniqueID %>': {
                        required: true,
                        minlength: 5,
                        equalTo: '#<%= passwordTb.ClientID %>'
                    }
                }
            });

            /* Handle error message containers */
            var errorHandlerContainer = function () {
                $(this).valid(); // validate once field before show/hide messages
                var $cont = $(this).parent();
                var haserror = $("strong.message:visible", $cont).size();
                if (haserror < 1) { $cont.removeClass("error"); $(".comment", $cont).show(); } else { $cont.addClass("error"); $(".comment", $cont).hide(); }

                var errors = myValidator.numberOfInvalids();
                if (errors) {
                    var message = errors == 1
                                    ? 'You missed 1 field. It has been highlighted'
                                    : 'You missed ' + errors + ' fields.  They have been highlighted';
                    $("#errorMessage span").html(message);
                    $("#errorMessage").show();
                } else {
                    $("#errorMessage").hide();
                }
            }

            $(":input", $myForm).blur(errorHandlerContainer); // check error fields on blur on all input fields
            $("select", $myForm).change(errorHandlerContainer); // check error fields on change on select/radio fields

            $("#<%= couponTb.ClientID %>").blur(function () {
                var couponCode = $("#<%= couponTb.ClientID %>").val();
                if (couponCode !== "") {
                    $.ajax({
                        type: "POST",
                        url: "Subscribe.aspx/IsCouponValid",
                        data: "{'couponCode':'" + couponCode + "'}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (result) {
                            if (result.d === false) {
                                var spanTxt = $("span#msg", $("#<%= couponTb.ClientID %>").parent());
                                spanTxt.removeClass("goodCoupon");
                                if (!spanTxt.hasClass("badCoupon")) spanTxt.addClass("badCoupon");
                                spanTxt.text("Coupon invalid!");
                            } else {
                                var spanTxt = $("span#msg", $("#<%= couponTb.ClientID %>").parent());
                                spanTxt.removeClass("badCoupon");
                                if (!spanTxt.hasClass("goodCoupon")) spanTxt.addClass("goodCoupon");
                                spanTxt.text("Coupon valid!");
                            }
                        },
                        error: function (result) {
                            alert(result.status + " " + result.statusText);
                        }
                    });
                } else {
                    var spanTxt = $("span#msg", $("#<%= couponTb.ClientID %>").parent());
                    spanTxt.removeClass("badCoupon");
                    spanTxt.removeClass("goodCoupon");
                    spanTxt.text("TEST123 always validates (no exp), EXPTEST123 is a coupon, but expired.");
                }
            });
        });
    </script>
</asp:Content>