<%@ Page Title="Search Permits" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="PermitSearch.aspx.cs" Inherits="RiverdalePermitSystem.Web.Pages.PermitSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Search Building Permits</h2>

    <div class="form-section">
        <h3>Search Criteria</h3>
        <div class="form-row">
            <label>Permit ID:</label>
            <asp:TextBox ID="txtPermitId" runat="server" MaxLength="50" />
        </div>
        <div class="form-row">
            <label>Property Address:</label>
            <asp:TextBox ID="txtAddress" runat="server" MaxLength="200" />
        </div>
        <div class="form-row">
            <label>Permit Type:</label>
            <asp:DropDownList ID="ddlPermitType" runat="server">
                <asp:ListItem Value="">-- All Types --</asp:ListItem>
                <asp:ListItem Value="NewConstruction">New Construction</asp:ListItem>
                <asp:ListItem Value="Addition">Addition/Alteration</asp:ListItem>
                <asp:ListItem Value="Electrical">Electrical</asp:ListItem>
                <asp:ListItem Value="Plumbing">Plumbing</asp:ListItem>
                <asp:ListItem Value="Mechanical">Mechanical</asp:ListItem>
                <asp:ListItem Value="Demolition">Demolition</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="form-row">
            <label>Status:</label>
            <asp:DropDownList ID="ddlStatus" runat="server">
                <asp:ListItem Value="">-- All Statuses --</asp:ListItem>
                <asp:ListItem Value="Submitted">Submitted</asp:ListItem>
                <asp:ListItem Value="Under Review">Under Review</asp:ListItem>
                <asp:ListItem Value="Approved">Approved</asp:ListItem>
                <asp:ListItem Value="Issued">Issued</asp:ListItem>
                <asp:ListItem Value="Expired">Expired</asp:ListItem>
                <asp:ListItem Value="Rejected">Rejected</asp:ListItem>
            </asp:DropDownList>
        </div>
        <p>
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="button-primary" OnClick="btnSearch_Click" />
            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="button-secondary" OnClick="btnClear_Click" CausesValidation="false" />
        </p>
    </div>

    <div class="form-section">
        <h3>Search Results</h3>
        <asp:ObjectDataSource ID="odsPermits" runat="server" 
            TypeName="PermitDataAccess" 
            SelectMethod="SearchPermits"
            EnablePaging="true"
            MaximumRowsParameterName="pageSize"
            StartRowIndexParameterName="startRow">
            <SelectParameters>
                <asp:ControlParameter Name="permitId" ControlID="txtPermitId" PropertyName="Text" Type="String" />
                <asp:ControlParameter Name="address" ControlID="txtAddress" PropertyName="Text" Type="String" />
                <asp:ControlParameter Name="permitType" ControlID="ddlPermitType" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter Name="status" ControlID="ddlStatus" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>

        <asp:GridView ID="gvPermits" runat="server" AutoGenerateColumns="False" 
            DataSourceID="odsPermits" AllowPaging="True" PageSize="20"
            CssClass="grid" EmptyDataText="No permits found matching your criteria."
            OnRowCommand="gvPermits_RowCommand" OnPageIndexChanging="gvPermits_PageIndexChanging">
            <Columns>
                <asp:BoundField DataField="PermitId" HeaderText="Permit #" SortExpression="PermitId" />
                <asp:BoundField DataField="ApplicationDate" HeaderText="Application Date" DataFormatString="{0:MM/dd/yyyy}" />
                <asp:BoundField DataField="PropertyAddress" HeaderText="Property Address" />
                <asp:BoundField DataField="PermitType" HeaderText="Type" />
                <asp:BoundField DataField="Status" HeaderText="Status" />
                <asp:BoundField DataField="EstimatedCost" HeaderText="Cost" DataFormatString="{0:C}" />
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button ID="btnView" runat="server" Text="View" CommandName="ViewDetails" 
                            CommandArgument='<%# Eval("PermitId") %>' CssClass="button-secondary" />
                        <asp:Button ID="btnReport" runat="server" Text="Report" CommandName="GenerateReport" 
                            CommandArgument='<%# Eval("PermitId") %>' CssClass="button-secondary" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" />
        </asp:GridView>
    </div>

    <asp:Panel ID="pnlPermitDetails" runat="server" Visible="false" CssClass="form-section">
        <h3>Permit Details</h3>
        <asp:FormView ID="fvPermitDetails" runat="server" DataSourceID="odsPermitDetails" RenderOuterTable="false">
            <ItemTemplate>
                <div class="form-row">
                    <label>Permit ID:</label>
                    <asp:Label ID="lblPermitId" runat="server" Text='<%# Eval("PermitId") %>' Font-Bold="true" />
                </div>
                <div class="form-row">
                    <label>Status:</label>
                    <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") %>' />
                </div>
                <div class="form-row">
                    <label>Application Date:</label>
                    <asp:Label ID="lblAppDate" runat="server" Text='<%# Eval("ApplicationDate", "{0:MM/dd/yyyy}") %>' />
                </div>
                <div class="form-row">
                    <label>Property Address:</label>
                    <asp:Label ID="lblAddress" runat="server" Text='<%# Eval("PropertyAddress") %>' />
                </div>
                <div class="form-row">
                    <label>Parcel Number:</label>
                    <asp:Label ID="lblParcel" runat="server" Text='<%# Eval("ParcelNumber") %>' />
                </div>
                <div class="form-row">
                    <label>Permit Type:</label>
                    <asp:Label ID="lblType" runat="server" Text='<%# Eval("PermitType") %>' />
                </div>
                <div class="form-row">
                    <label>Estimated Cost:</label>
                    <asp:Label ID="lblCost" runat="server" Text='<%# Eval("EstimatedCost", "{0:C}") %>' />
                </div>
                <div class="form-row">
                    <label>Applicant:</label>
                    <asp:Label ID="lblApplicant" runat="server" Text='<%# Eval("ApplicantName") %>' />
                </div>
            </ItemTemplate>
        </asp:FormView>
        <asp:ObjectDataSource ID="odsPermitDetails" runat="server" 
            TypeName="PermitDataAccess" 
            SelectMethod="GetPermitById">
            <SelectParameters>
                <asp:SessionParameter Name="permitId" SessionField="SelectedPermitId" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <p>
            <asp:Button ID="btnCloseDetails" runat="server" Text="Close" CssClass="button-secondary" OnClick="btnCloseDetails_Click" />
        </p>
    </asp:Panel>
</asp:Content>
