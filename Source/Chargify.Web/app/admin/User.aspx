<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="User.aspx.vb" Inherits="app_admin_User" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    User Edit
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
    <link href="../../css/screen/yaml.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <div class="hlist">
	    <ul>
		    <li><asp:LinkButton ID="logoutBtn" runat="server">Logout</asp:LinkButton></li>
		    <li><a href="../Dashboard.aspx">Dashboard</a></li>
            <li><a href="Admin.aspx">Administration</a></li>
            <li class="active"><strong>User</strong></li>
	    </ul>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
    <div class="info">
        <p>
            Welcome to the user administration. From here you will be able to do the following:
        </p>
        <ul>
            <li><a href="#charge">Create a one-time charge</a></li>            
            <li><a href="#credit">Create a one-time credit</a></li>            
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <div class="yform columnar">
        <h3 id="charge">One-Time Charge</h3>
        <fieldset>
            <legend>Options</legend>
            <div class="type-text">
                <asp:Label ID="amountLbl" runat="server" Text="Amount" AssociatedControlID="amountTb"></asp:Label>
                <asp:TextBox ID="amountTb" CssClass="required number" runat="server"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="reasonLbl" runat="server" Text="Reason" AssociatedControlID="reasonTb"></asp:Label>
                <asp:TextBox ID="reasonTb" CssClass="required" runat="server"></asp:TextBox>
            </div>
            <div class="type-button">
                <asp:Button ID="submitBtn" runat="server" Text="Submit One-Time Charge" />
            </div>
        </fieldset>
        <asp:Panel ID="resultPnl" runat="server">
            <asp:Literal ID="resultLtr" runat="server"></asp:Literal>
        </asp:Panel>
        <h3 id="credit">One-Time Credit</h3>
        <fieldset>
            <legend>Options</legend>
            <div class="type-text">
                <asp:Label ID="creditAmountLbl" runat="server" Text="Amount" AssociatedControlID="creditAmountTb"></asp:Label>
                <asp:TextBox ID="creditAmountTb" CssClass="required number" runat="server"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="creditReasonLbl" runat="server" Text="Reason" AssociatedControlID="creditReasonTb"></asp:Label>
                <asp:TextBox ID="creditReasonTb" CssClass="required" runat="server"></asp:TextBox>
            </div>
            <div class="type-button">
                <asp:Button ID="creditAcctBtn" runat="server" Text="Credit Account" />
            </div>
        </fieldset>
    </div>
    <h3>Usages</h3>
    <asp:Repeater ID="usageRpt" runat="server">
        <HeaderTemplate>
            <table class="full">
                <tr>
                    <th scope="col" style="text-align: center;" align="center">ID</th>
                    <th scope="col" align="center">Quantity</th>
                    <th scope="col" align="left">Memo</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td style="width: 30px;" align="center">
                    <strong><%# Eval("ID")%></strong>
                </td>
                <td style="width: 50px;" align="center">
                    <%# Eval("Quantity") %> 
                </td>
                <td align="left">
                    <%# Eval("Memo")%>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
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
</asp:Content>