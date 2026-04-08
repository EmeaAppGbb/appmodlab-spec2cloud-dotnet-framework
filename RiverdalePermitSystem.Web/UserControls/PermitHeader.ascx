<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PermitHeader.ascx.cs" Inherits="RiverdalePermitSystem.Web.UserControls.PermitHeader" %>

<div style="background-color: #f0f0f0; padding: 15px; margin-bottom: 20px; border: 1px solid #ccc;">
    <h3 style="margin: 0 0 10px 0;">Permit Information</h3>
    <p><strong>Permit ID:</strong> <asp:Label ID="lblPermitId" runat="server" /></p>
    <p><strong>Property Address:</strong> <asp:Label ID="lblPropertyAddress" runat="server" /></p>
    <p><strong>Status:</strong> <asp:Label ID="lblStatus" runat="server" Font-Bold="true" /></p>
</div>
