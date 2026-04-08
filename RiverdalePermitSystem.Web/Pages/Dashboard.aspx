<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="RiverdalePermitSystem.Web.Pages.Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Administrator Dashboard</h2>

    <asp:UpdatePanel ID="upDashboard" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="dashboard-stats">
                <div class="stat-card">
                    <h3>Total Permits</h3>
                    <div class="number"><asp:Label ID="lblTotalPermits" runat="server" /></div>
                </div>
                <div class="stat-card">
                    <h3>Pending Review</h3>
                    <div class="number"><asp:Label ID="lblPendingReview" runat="server" /></div>
                </div>
                <div class="stat-card">
                    <h3>Inspections Today</h3>
                    <div class="number"><asp:Label ID="lblInspectionsToday" runat="server" /></div>
                </div>
                <div class="stat-card">
                    <h3>Revenue (Month)</h3>
                    <div class="number"><asp:Label ID="lblMonthlyRevenue" runat="server" /></div>
                </div>
            </div>
            <p style="text-align: right; margin-bottom: 20px;">
                <asp:Button ID="btnRefreshStats" runat="server" Text="Refresh" CssClass="button-secondary" OnClick="btnRefreshStats_Click" />
                Last updated: <asp:Label ID="lblLastUpdate" runat="server" />
            </p>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="form-section">
        <h3>Recent Activity</h3>
        <asp:GridView ID="gvRecentActivity" runat="server" AutoGenerateColumns="False" CssClass="grid">
            <Columns>
                <asp:BoundField DataField="Timestamp" HeaderText="Time" DataFormatString="{0:MM/dd/yyyy HH:mm}" />
                <asp:BoundField DataField="ActivityType" HeaderText="Type" />
                <asp:BoundField DataField="PermitId" HeaderText="Permit #" />
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:BoundField DataField="UserName" HeaderText="User" />
            </Columns>
        </asp:GridView>
    </div>

    <div class="form-section">
        <h3>Permits by Status</h3>
        <asp:UpdatePanel ID="upStatusChart" runat="server">
            <ContentTemplate>
                <asp:GridView ID="gvStatusSummary" runat="server" AutoGenerateColumns="False" CssClass="grid">
                    <Columns>
                        <asp:BoundField DataField="Status" HeaderText="Status" />
                        <asp:BoundField DataField="Count" HeaderText="Count" />
                        <asp:BoundField DataField="TotalValue" HeaderText="Total Value" DataFormatString="{0:C}" />
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
