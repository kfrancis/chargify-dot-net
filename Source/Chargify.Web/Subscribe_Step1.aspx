<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="Subscribe_Step1.aspx.vb" Inherits="Subscribe_Step1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Subscribe - Step 1
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
    <style type="text/css">
        .columnar div.type-text span { margin-left: 30%; }
        .columnar div.type-options input { margin-left: 30%; }
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
        <p>Please fill in the following information.</p>
        <p>When finished, please click '<strong>Continue ...</strong>' to be taken to the Chargify product signup page.</p>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <div class="yform columnar">
        <p>
            Once you complete this information and click '<strong>Continue ...</strong>', you will be directed to Chargify's subscription signup page. 
            Fill out the rest of the information and then click '<strong>Place My Order</strong>' to complete this process.
        </p>
        <div id="errorMessage" class="warning">
            <h3>Uh Oh!</h3>
            <span>&nbsp;</span>
        </div>
        <fieldset>
            <legend>User Info</legend>
            <div class="type-text">
                <asp:Label ID="usernameLbl" runat="server" AssociatedControlID="usernameTb" Text="Username"></asp:Label>
                <asp:TextBox ID="usernameTb" runat="server" CssClass="required" minlength="5" TabIndex="1"></asp:TextBox>
                <span class="comment dimmed">Just remember, this username is public in this demo ..</span>
            </div>
            <div class="type-text">
                <asp:Label ID="passwordLbl" runat="server" AssociatedControlID="passwordTb" Text="Password"></asp:Label>
                <asp:TextBox ID="passwordTb" runat="server" TextMode="Password" CssClass="required" minlength="5" TabIndex="2"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="passwordConfirmLbl" runat="server" AssociatedControlID="passwordConfirmTb" Text="Again?"></asp:Label>
                <asp:TextBox ID="passwordConfirmTb" runat="server" TextMode="Password" TabIndex="3"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="secretQuestionLbl" runat="server" AssociatedControlID="secretQuestionTb" Text="Question"></asp:Label>
                <asp:TextBox ID="secretQuestionTb" runat="server" CssClass="required" TabIndex="4"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="secretAnswerLbl" runat="server" AssociatedControlID="secretAnswerTb" Text="Answer"></asp:Label>
                <asp:TextBox ID="secretAnswerTb" runat="server" CssClass="required" TabIndex="5"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="emailLbl" runat="server" AssociatedControlID="emailTb" Text="Email"></asp:Label>
                <asp:TextBox ID="emailTb" runat="server" CssClass="required email" TabIndex="6"></asp:TextBox>
            </div>
            <div class="type-options">
                <asp:Button ID="submitBtn" CssClass="button green large" runat="server" Text="Continue ..." TabIndex="7" />
            </div>
        </fieldset>
        <div class="dimmed">
            <p>
                This page only asks the required information to make a new system user, the rest of the billing information will be gathered on the Chargify hosted signup page.
                After the user has successfully entered the information, the signup page will redirect the user to the next step in the process.
            </p>
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
        });
    </script>
</asp:Content>