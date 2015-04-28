<%@ Page Language="VB" MasterPageFile="~/site.master" AutoEventWireup="false" CodeFile="CDExample.aspx.vb" Inherits="CDExample" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphTitleContent" Runat="Server">
    Subscribe (Chargify Direct)
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphHead" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphMenu" Runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="cphLeftContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="cphMainContent" Runat="Server">
    <form method="post" action="https://api.chargify.com/api/v2/signups">
        <input type="hidden" name="secure[api_id]" value="a71df900-4c23-012e-61bb-005056a209b7" />
        <input type="hidden" name="secure[signature]" value="de207064954420cd1f62aaf21eca825ba8a47c75"/>
    </form>
</asp:Content>