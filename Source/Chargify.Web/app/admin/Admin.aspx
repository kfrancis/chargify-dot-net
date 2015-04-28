<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="Admin.aspx.vb" Inherits="app_admin_Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Administration
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
    <link href="../../css/screen/yaml.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <div class="hlist">
	    <ul>
		    <li><asp:LinkButton ID="logoutBtn" runat="server">Logout</asp:LinkButton></li>
		    <li><a href="../Dashboard.aspx">Dashboard</a></li>
            <li class="active"><strong>Administration</strong></li>
	    </ul>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
    <div class="info">
        <p>
            Welcome to the administration home. From here you will be able to do the following:
        </p>
        <ul>
            <li><a href="#users">Clear all users</a></li>
            <li><a href="#specificUsers">Commands on specific users</a></li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <h2>Administration > Home</h2>
    <br />
    <h3 id="users">Users</h3>
    <div class="yform columnar">
        <p class="info">This will clear all the users from the system, including the current user. Before logging out, the sample users are recreated. Before hitting the button, please make sure you clear out the test data from your Chargify test site.</p>
        <div class="type-button">
            <asp:Button ID="clearUsersBtn" runat="server" Text="Clear All Users" OnClientClick="return confirm('Are you sure you wish to delete all users/customers?\nYou MUST make sure you clear your test data (only subscriptions and customers) before you continue, as a new customer will be created automatically.');" />
        </div>
    </div>
    <h3 id="specificUsers">User List</h3>
    <p class="info">
        You can use the delete <asp:Image ID="Image1" runat="server" style="position: relative; top: 4px;" ImageUrl="~/images/delete.gif" /> button to delete a user, 
        or you can use the charge <asp:Image ID="Image2" runat="server" style="position: relative; top: 4px;" ImageUrl="~/images/wallet.gif" /> button to create a one-time charge for a user.
    </p>
    <asp:UpdatePanel ID="updatePanel1" runat="server" UpdateMode="Always"> 
        <ContentTemplate>
            <asp:GridView ID="usersGv" runat="server" AutoGenerateColumns="false" CssClass="full" AllowPaging="true" PageSize="3">
                <Columns>
                    <asp:TemplateField ItemStyle-Width="35px">
                        <ItemTemplate>
                            <asp:ImageButton ID="deleteBtn" Visible='<%# Eval("Customer") %>' runat="server" CausesValidation="false" OnClientClick="if(confirm('Are you sure you want to delete this user?')==false){return false;}" 
                                CommandName="DeleteMe" CommandArgument='<%# Eval("UserID") %>' ImageUrl="~/images/delete.gif" Height="16px" Width="16px" style="cursor: pointer;" ToolTip="Delete this user .." />
                            <asp:ImageButton ID="editBtn" Visible='<%# Eval("Customer") %>' runat="server" CausesValidation="false" ToolTip="Charge this user .."
                                CommandName="EditMe" CommandArgument='<%# Eval("UserID") %>' ImageUrl="~/images/wallet.gif" Height="16px" Width="16px" style="cursor: pointer;" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="UserName" HeaderText="User Name" />
                    <asp:BoundField DataField="LastActivityDate" HeaderText="Last Login" />
                </Columns>
            </asp:GridView>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="usersGv" EventName="PageIndexChanging" />
        </Triggers>
    </asp:UpdatePanel>
    
    <p class="dimmed">
        An administrator only has the command options when the user is a customer, for now. There shouldn't be a case where a user exists in this app, but not in chargify - as signing up is a requirement for this demo.
    </p>
</asp:Content>