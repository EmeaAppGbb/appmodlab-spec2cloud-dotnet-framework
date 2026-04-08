<%@ Page Title="Inspection Schedule" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="InspectionSchedule.aspx.cs" Inherits="RiverdalePermitSystem.Web.Pages.InspectionSchedule" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Schedule Building Inspection</h2>

    <div class="form-section">
        <h3>Schedule New Inspection</h3>
        <div class="form-row">
            <label>Permit ID:</label>
            <asp:TextBox ID="txtPermitId" runat="server" MaxLength="50" />
            <asp:RequiredFieldValidator ID="rfvPermitId" runat="server" ControlToValidate="txtPermitId"
                ErrorMessage="Permit ID is required" Display="Dynamic" CssClass="error-message" ValidationGroup="Schedule" />
        </div>
        <div class="form-row">
            <label>Inspection Type:</label>
            <asp:DropDownList ID="ddlInspectionType" runat="server">
                <asp:ListItem Value="">-- Select Type --</asp:ListItem>
                <asp:ListItem Value="Foundation">Foundation</asp:ListItem>
                <asp:ListItem Value="Framing">Framing</asp:ListItem>
                <asp:ListItem Value="Electrical">Electrical</asp:ListItem>
                <asp:ListItem Value="Plumbing">Plumbing</asp:ListItem>
                <asp:ListItem Value="Mechanical">Mechanical</asp:ListItem>
                <asp:ListItem Value="Final">Final</asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="rfvInspectionType" runat="server" ControlToValidate="ddlInspectionType"
                InitialValue="" ErrorMessage="Inspection type is required" Display="Dynamic" CssClass="error-message" ValidationGroup="Schedule" />
        </div>
        <div class="form-row">
            <label>Requested Date:</label>
            <asp:TextBox ID="txtRequestedDate" runat="server" TextMode="Date" />
            <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="txtRequestedDate"
                ErrorMessage="Date is required" Display="Dynamic" CssClass="error-message" ValidationGroup="Schedule" />
        </div>
        <div class="form-row">
            <label>Notes:</label>
            <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="3" MaxLength="500" />
        </div>
        <p>
            <asp:Button ID="btnSchedule" runat="server" Text="Schedule Inspection" CssClass="button-primary" OnClick="btnSchedule_Click" ValidationGroup="Schedule" />
            <asp:Label ID="lblMessage" runat="server" CssClass="success-message" Visible="false" />
        </p>
    </div>

    <div class="form-section">
        <h3>Upcoming Inspections</h3>
        <asp:UpdatePanel ID="upInspections" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:GridView ID="gvInspections" runat="server" AutoGenerateColumns="False" CssClass="grid"
                    OnRowCommand="gvInspections_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="InspectionId" HeaderText="Inspection #" />
                        <asp:BoundField DataField="PermitId" HeaderText="Permit #" />
                        <asp:BoundField DataField="InspectionType" HeaderText="Type" />
                        <asp:BoundField DataField="ScheduledDate" HeaderText="Scheduled" DataFormatString="{0:MM/dd/yyyy}" />
                        <asp:BoundField DataField="Status" HeaderText="Status" />
                        <asp:BoundField DataField="InspectorName" HeaderText="Inspector" />
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <asp:Button ID="btnComplete" runat="server" Text="Complete" CommandName="CompleteInspection"
                                    CommandArgument='<%# Eval("InspectionId") %>' CssClass="button-secondary" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CommandName="CancelInspection"
                                    CommandArgument='<%# Eval("InspectionId") %>' CssClass="button-secondary" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <p>
                    <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="button-secondary" OnClick="btnRefresh_Click" />
                </p>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
