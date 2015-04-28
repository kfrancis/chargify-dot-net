<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="ChargifyDirectExample.aspx.vb" Inherits="ChargifyDirectExample" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Chargify Direct Example
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
    <style type="text/css">
        .columnar div.type-text span { margin-left: 30%; }
        .columnar div.type-options input { margin-left: 30%; }
        .signature_result {
          border: 1px solid #000;
          background-color: #c0c0c0;
          padding: 20px;
          margin-bottom: 10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
    <!-- This menu is static on purpose, since the Subscribe page should only appear when you are on this page. -->
    <div class="hlist">
	    <ul>
		    <li><a href="default.aspx">Home</a></li>
		    <li class="active"><strong>Chargify Direct Test</strong></li>
	    </ul>
    </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
    <div class="info">
        <p>Please fill in the following information.</p>
        <p>When finished, please click '<strong>Compute Signature ...</strong>' and you will be shown the result.</p>
    </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <asp:Panel ID="sigResultPnl" runat="server" CssClass="signature_result" Visible="false">
        <h3>Inputs</h3>
        <pre><code>api_id = <asp:Literal ID="apiIDLtr" runat="server"></asp:Literal>
timestamp = <asp:Literal ID="timestampLtr" runat="server"></asp:Literal>
nonce = <asp:Literal ID="nonceLtr" runat="server"></asp:Literal>
data = <asp:Literal ID="dataLtr" runat="server"></asp:Literal></code></pre>
        <h3>API Secret:</h3>        
        <pre><code><asp:Literal ID="apiSecretLtr" runat="server"></asp:Literal></code></pre>
        <h3>Signature:</h3>        
        <pre><code><asp:Literal ID="signatureLtr" runat="server"></asp:Literal></code></pre>
        <h3>Example Form:</h3>
        <asp:Literal ID="formLtr" runat="server"></asp:Literal>
    </asp:Panel>
    <h1>Compute a new signature</h1>
    <div class="yform columnar">
        <fieldset>
            <div class="type-text">
                <asp:Label ID="apiIDLbl" runat="server" Text="API ID (Required)" AssociatedControlID="apiIDTb"></asp:Label>
                <asp:TextBox ID="apiIDTb" runat="server"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="timestampLbl" runat="server" Text="Timestamp (Optional)" AssociatedControlID="timestampTb"></asp:Label>
                <asp:TextBox ID="timestampTb" runat="server"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="nonceLbl" runat="server" Text="Nonce (Optional)" AssociatedControlID="nonceTb"></asp:Label>
                <asp:TextBox ID="nonceTb" runat="server"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="dataLbl" runat="server" Text="Data (Optional)" AssociatedControlID="dataTb"></asp:Label>
                <asp:TextBox ID="dataTb" runat="server"></asp:TextBox>
            </div>
            <div class="type-text">
                <asp:Label ID="secretLbl" runat="server" Text="API Secret Key" AssociatedControlID="secretTb"></asp:Label>
                <asp:TextBox ID="secretTb" runat="server"></asp:TextBox><br />
                <span style="font-size: x-small;">Don't put your production secret hete, this is not a secure connection.</span>
            </div>
            <div class="type-options">
                <asp:Button ID="computeSignatureBtn" CssClass="button green large" runat="server" Text="Compute Signature ..." TabIndex="7" />
                or
                <asp:LinkButton ID="resetLnkBtn" runat="server">Reset</asp:LinkButton>
            </div>
        </fieldset>
    </div>
</asp:Content>