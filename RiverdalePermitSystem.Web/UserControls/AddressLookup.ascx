<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddressLookup.ascx.cs" Inherits="RiverdalePermitSystem.Web.UserControls.AddressLookup" %>

<asp:TextBox ID="txtAddress" runat="server" MaxLength="200" />
<asp:Button ID="btnLookup" runat="server" Text="Lookup" OnClick="btnLookup_Click" CausesValidation="false" />
<asp:Panel ID="pnlSuggestions" runat="server" Visible="false" style="position: absolute; background-color: white; border: 1px solid #ccc; max-height: 200px; overflow-y: auto;">
    <asp:ListBox ID="lstSuggestions" runat="server" AutoPostBack="true" OnSelectedIndexChanged="lstSuggestions_SelectedIndexChanged" />
</asp:Panel>
