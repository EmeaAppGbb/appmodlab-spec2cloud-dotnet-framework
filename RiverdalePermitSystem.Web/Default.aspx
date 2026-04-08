<%@ Page Title="Home" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RiverdalePermitSystem.Web.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Welcome to Riverdale City Building Permit System</h2>
    
    <div class="form-section">
        <h3>Quick Actions</h3>
        <p>
            <asp:HyperLink ID="lnkNewPermit" runat="server" NavigateUrl="~/Pages/PermitApplication.aspx" CssClass="button-primary">Apply for New Permit</asp:HyperLink>
            <asp:HyperLink ID="lnkSearchPermits" runat="server" NavigateUrl="~/Pages/PermitSearch.aspx" CssClass="button-secondary">Search Existing Permits</asp:HyperLink>
        </p>
    </div>

    <div class="form-section">
        <h3>Recent Permits</h3>
        <asp:UpdatePanel ID="upRecentPermits" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView ID="gvRecentPermits" runat="server" AutoGenerateColumns="False" CssClass="grid" 
                    EmptyDataText="No recent permits found." OnRowCommand="gvRecentPermits_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="PermitId" HeaderText="Permit #" />
                        <asp:BoundField DataField="ApplicationDate" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy}" />
                        <asp:BoundField DataField="PropertyAddress" HeaderText="Property Address" />
                        <asp:BoundField DataField="PermitType" HeaderText="Type" />
                        <asp:BoundField DataField="Status" HeaderText="Status" />
                        <asp:BoundField DataField="EstimatedCost" HeaderText="Estimated Cost" DataFormatString="{0:C}" />
                        <asp:ButtonField ButtonType="Button" CommandName="ViewDetails" Text="View" />
                    </Columns>
                </asp:GridView>
                <p style="margin-top: 10px;">
                    <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="button-secondary" OnClick="btnRefresh_Click" />
                    Last updated: <asp:Label ID="lblLastUpdate" runat="server" />
                </p>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div class="form-section">
        <h3>System Information</h3>
        <p><strong>Available Permit Types:</strong></p>
        <ul>
            <li>Building Permit - New Construction</li>
            <li>Building Permit - Addition/Alteration</li>
            <li>Electrical Permit</li>
            <li>Plumbing Permit</li>
            <li>Mechanical Permit</li>
            <li>Demolition Permit</li>
        </ul>
        <p><strong>Processing Time:</strong> Most permits are reviewed within 5-10 business days.</p>
        <p><strong>Support:</strong> For assistance, contact permits@riverdalecity.gov or call (555) 123-4567</p>
    </div>
</asp:Content>
