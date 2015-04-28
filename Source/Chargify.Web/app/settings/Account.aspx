<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="Account.aspx.vb" Inherits="app_settings_Account" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Account
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
    <style type="text/css">
        .columnar div.type-text span { margin-left: 30%; }
        .columnar div.type-options input { margin-left: 30%; }
    </style>
    <link href="../../sys/libs/colorbox/css/colorbox.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <div class="hlist">
	    <ul>
		    <li><asp:LinkButton ID="logoutBtn" runat="server">Logout</asp:LinkButton></li>
		    <li><a href="../Dashboard.aspx">Dashboard</a></li>
		    <li class="active"><strong>Account</strong></li>
	    </ul>
    </div>    
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
    <ul> 
        <li>
            <div class="floatbox">
                <asp:Image ID="Image3" runat="server" CssClass="float_left" ImageUrl="~/images/key.png" />
                <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/app/settings/Account.aspx">Account Information</asp:HyperLink>
                <p>Summary, change password, edit billing</p>
            </div>
        </li>
        <li>
            <div class="floatbox">
                <asp:Image ID="Image1" runat="server" CssClass="float_left" ImageUrl="~/images/walletBig.gif" />
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/app/settings/BillingHistory.aspx">Billing History</asp:HyperLink>
                <p>Billing history, transactions</p>
            </div>
        </li>
        <li>
            <div class="floatbox">
                <asp:Image ID="Image2" runat="server" CssClass="float_left" ImageUrl="~/images/buy.png" />
                <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/app/settings/ChangeSubscription.aspx">Upgrade & Extend</asp:HyperLink>
                <p>More power, more features on demand</p>
            </div>
        </li>
    </ul>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <asp:ScriptManagerProxy ID="scriptManagerProxy1" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/sys/libs/colorbox/js/jquery.colorbox-min.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#<%= remoteLnk.ClientID %>").colorbox({ width: "800px", height: "750px", iframe: true, scrolling: false });
        });
    </script>
    <h2>Settings > Account Information</h2>
    <br />
    <h3>General</h3>
    <div class="info">
        <table>
            <tr>
                <td>Your Email: </td>
                <td class="dimmed"><asp:Literal ID="emailLtr" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <td>Last Login: </td>
                <td class="dimmed"><asp:Literal ID="lastLoginLtr" runat="server"></asp:Literal></td>
            </tr>
        </table>
    </div>
    <br />
    <h3>Edit Billing Info</h3>
    <div class="yform columnar">
        <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <asp:Panel ID="paymentPanel" runat="server">
                    <asp:Literal ID="paymentMsgLtr" runat="server"></asp:Literal>
                    <div class="subcolumns">
                        <div class="c50l">
                            <div class="subcl">
                                <fieldset>
                                    <legend>Details&nbsp;
                                        <asp:DropDownList ID="ccDDL" style="margin-top: 1px;" runat="server" AutoPostBack="true">
                                        </asp:DropDownList>
                                    </legend>
                                    <asp:Panel ID="cardPnl" runat="server" CssClass="type-text">
                                        <asp:Label ID="cardNumberLbl" runat="server" AssociatedControlID="cardNumberTb" Text="Card Number"></asp:Label>
                                        <asp:TextBox ID="cardNumberTb" runat="server" CssClass="required"></asp:TextBox>
                                    </asp:Panel>
                                    <asp:Panel ID="ccvPnl" runat="server" CssClass="type-text">
                                        <asp:Label ID="cvvLbl" runat="server" AssociatedControlID="cvvTb" Text="CCV"></asp:Label>
                                        <asp:TextBox ID="cvvTb" runat="server" CssClass="required" minlength="3" MaxLength="3"></asp:TextBox>
                                        <span>(<a href="http://en.wikipedia.org/wiki/Card_Security_Code" target="_blank">click here</a> for more info)</span>
                                    </asp:Panel>
                                    <div class="type-text">
                                        <asp:Label ID="fNameLbl" runat="server" AssociatedControlID="fNameTb" Text="First Name"></asp:Label>
                                        <asp:TextBox ID="fNameTb" runat="server" CssClass="required"></asp:TextBox>
                                    </div>
                                    <div class="type-text">
                                        <asp:Label ID="lNameLbl" runat="server" AssociatedControlID="lNameTb" Text="Last Name"></asp:Label>
                                        <asp:TextBox ID="lNameTb" runat="server" CssClass="required"></asp:TextBox>
                                    </div>
                                    <div class="type-select">
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
                                    <div class="type-select">
                                        <asp:Label ID="expirationYearLbl" runat="server" AssociatedControlID="expirationYearDDL" Text="Exp. Year"></asp:Label>
                                        <asp:DropDownList ID="expirationYearDDL" runat="server" CssClass="required" AppendDataBoundItems="true" DataTextField="Year" DataValueField="Year">
                                            <asp:ListItem Text="Select One" Value="" Selected="True" />
                                        </asp:DropDownList>
                                    </div>
                                    <div class="type-options">
                                        <asp:Button ID="updateCCBtn" CssClass="button green medium" runat="server" Text="Update" UseSubmitBehavior="true" />
                                    </div>
                                </fieldset>
                            </div>
                        </div>
                        <div class="c50r">
                            <div class="subcr">
                                <fieldset>
                                    <legend>Update via Hosted Page</legend>
                                    <p class="dimmed">This url for this link is generated securely, using the method stated <a href="http://support.chargify.com/faqs/technical/generating-hosted-page-urls" target="_blank">here</a>:</p>
                                    <div class="type-button">
                                        <asp:HyperLink ID="remoteLnk" runat="server" CssClass="button green medium" title="Secure Chargify Page">Update</asp:HyperLink>
                                    </div>
                                </fieldset>
                            </div>
                        </div>
                    </div>    
                </asp:Panel>            
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ccDDL" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="updateCCBtn" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
        <script type="text/javascript">
            var myVal = null;
            $(document).ready(function () {
                $myForm = $("#aspnetForm"); // instance of myform for faster dom-select
                myVal = $myForm.validate({
                    errorPlacement: function (error, element) {
                        element.parent().addClass("error");
                        error.prependTo(element.parent());
                    },
                    errorClass: "message",
                    errorElement: "strong",
                    onkeyup: false, // disable it - make some problems with errorPlacement 
                    onclick: false  // disable it - make some problems with errorPlacement
                });

                /* Handle error message containers */
                var errorHandlerContainer = function () {
                    $(this).valid(); // validate once field before show/hide messages
                    var $cont = $(this).parent();
                    var haserror = $("strong.message:visible", $cont).size();
                    if (haserror < 1) { $cont.removeClass("error"); } else { $cont.addClass("error"); }
                }

                $(":input", $myForm).blur(errorHandlerContainer); // check error fields on blur on all input fields
                $("select", $myForm).change(errorHandlerContainer); // check error fields on change on select/radio fields
            });
        </script>
    </div>
    <asp:Panel ID="cancelAcctPnl" runat="server">
        <h3>Cancel Account</h3>
        <div class="important">
            <strong>Once your account is cancelled, all your information will be immediately and permanently deleted.</strong><br />
            Are you sure you want to cancel your account?
        </div>
        <asp:Button ID="cancelBtn" runat="server" CssClass="button red medium" UseSubmitBehavior="false" Text="Cancel My Account" />
    </asp:Panel>
</asp:Content>