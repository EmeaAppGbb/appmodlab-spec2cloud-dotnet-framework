<%@ Page Title="Plan Review" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="PlanReview.aspx.cs" Inherits="RiverdalePermitSystem.Web.Pages.PlanReview" %>
<%@ Register Src="~/UserControls/PermitHeader.ascx" TagPrefix="uc" TagName="PermitHeader" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Plan Review Management</h2>

    <div class="form-section">
        <h3>Select Permit for Review</h3>
        <div class="form-row">
            <label>Permit ID:</label>
            <asp:TextBox ID="txtPermitId" runat="server" MaxLength="50" />
            <asp:Button ID="btnLoadPermit" runat="server" Text="Load" CssClass="button-secondary" OnClick="btnLoadPermit_Click" />
        </div>
    </div>

    <asp:Panel ID="pnlReviewForm" runat="server" Visible="false">
        <uc:PermitHeader ID="ucPermitHeader" runat="server" />

        <div class="form-section">
            <h3>Plan Review Details</h3>
            <div class="form-row">
                <label>Review Type:</label>
                <asp:DropDownList ID="ddlReviewType" runat="server">
                    <asp:ListItem Value="Structural">Structural</asp:ListItem>
                    <asp:ListItem Value="Electrical">Electrical</asp:ListItem>
                    <asp:ListItem Value="Plumbing">Plumbing</asp:ListItem>
                    <asp:ListItem Value="Mechanical">Mechanical</asp:ListItem>
                    <asp:ListItem Value="Fire">Fire Safety</asp:ListItem>
                    <asp:ListItem Value="Zoning">Zoning Compliance</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-row">
                <label>Reviewer:</label>
                <asp:Label ID="lblReviewer" runat="server" Font-Bold="true" />
            </div>
            <div class="form-row">
                <label>Review Status:</label>
                <asp:DropDownList ID="ddlReviewStatus" runat="server">
                    <asp:ListItem Value="Pending">Pending</asp:ListItem>
                    <asp:ListItem Value="InProgress">In Progress</asp:ListItem>
                    <asp:ListItem Value="Approved">Approved</asp:ListItem>
                    <asp:ListItem Value="ApprovedWithConditions">Approved with Conditions</asp:ListItem>
                    <asp:ListItem Value="Rejected">Rejected</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-row">
                <label>Comments:</label>
                <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" Rows="5" MaxLength="2000" />
            </div>
            <div class="form-row">
                <label>Deficiencies:</label>
                <asp:CheckBoxList ID="cblDeficiencies" runat="server">
                    <asp:ListItem Value="MissingDocumentation">Missing Documentation</asp:ListItem>
                    <asp:ListItem Value="InsufficientDetails">Insufficient Details</asp:ListItem>
                    <asp:ListItem Value="CodeViolation">Building Code Violation</asp:ListItem>
                    <asp:ListItem Value="ZoningIssue">Zoning Issue</asp:ListItem>
                    <asp:ListItem Value="StructuralConcern">Structural Concern</asp:ListItem>
                </asp:CheckBoxList>
            </div>
            <p>
                <asp:Button ID="btnSubmitReview" runat="server" Text="Submit Review" CssClass="button-primary" OnClick="btnSubmitReview_Click" />
                <asp:Label ID="lblReviewMessage" runat="server" CssClass="success-message" Visible="false" />
            </p>
        </div>

        <div class="form-section">
            <h3>Review History</h3>
            <asp:GridView ID="gvReviewHistory" runat="server" AutoGenerateColumns="False" CssClass="grid">
                <Columns>
                    <asp:BoundField DataField="ReviewDate" HeaderText="Date" DataFormatString="{0:MM/dd/yyyy}" />
                    <asp:BoundField DataField="ReviewType" HeaderText="Type" />
                    <asp:BoundField DataField="ReviewerName" HeaderText="Reviewer" />
                    <asp:BoundField DataField="Status" HeaderText="Status" />
                    <asp:BoundField DataField="Comments" HeaderText="Comments" />
                </Columns>
            </asp:GridView>
        </div>
    </asp:Panel>
</asp:Content>
