<%@ Page Title="Apply for Permit" Language="C#" MasterPageFile="~/MasterPages/Site.Master" AutoEventWireup="true" CodeBehind="PermitApplication.aspx.cs" Inherits="RiverdalePermitSystem.Web.Pages.PermitApplication" EnableViewState="true" %>
<%@ Register Src="~/UserControls/AddressLookup.ascx" TagPrefix="uc" TagName="AddressLookup" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Building Permit Application</h2>

    <div class="wizard-steps">
        <div class="wizard-step" id="step1Indicator" runat="server">
            1. Property Information
        </div>
        <div class="wizard-step" id="step2Indicator" runat="server">
            2. Applicant Details
        </div>
        <div class="wizard-step" id="step3Indicator" runat="server">
            3. Project Details
        </div>
        <div class="wizard-step" id="step4Indicator" runat="server">
            4. Review & Submit
        </div>
    </div>

    <asp:MultiView ID="mvWizard" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwStep1" runat="server">
            <div class="form-section">
                <h3>Step 1: Property Information</h3>
                <div class="form-row">
                    <label>Property Address:</label>
                    <uc:AddressLookup ID="ucAddress" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ControlToValidate="ucAddress$txtAddress" 
                        ErrorMessage="Property address is required" Display="Dynamic" CssClass="error-message" ValidationGroup="Step1" />
                </div>
                <div class="form-row">
                    <label>Parcel Number:</label>
                    <asp:TextBox ID="txtParcelNumber" runat="server" MaxLength="50" />
                    <asp:RequiredFieldValidator ID="rfvParcel" runat="server" ControlToValidate="txtParcelNumber" 
                        ErrorMessage="Parcel number is required" Display="Dynamic" CssClass="error-message" ValidationGroup="Step1" />
                </div>
                <div class="form-row">
                    <label>Zoning District:</label>
                    <asp:DropDownList ID="ddlZoning" runat="server">
                        <asp:ListItem Value="">-- Select Zoning --</asp:ListItem>
                        <asp:ListItem Value="R1">R1 - Single Family Residential</asp:ListItem>
                        <asp:ListItem Value="R2">R2 - Multi-Family Residential</asp:ListItem>
                        <asp:ListItem Value="C1">C1 - Commercial</asp:ListItem>
                        <asp:ListItem Value="I1">I1 - Industrial</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvZoning" runat="server" ControlToValidate="ddlZoning" 
                        InitialValue="" ErrorMessage="Zoning district is required" Display="Dynamic" CssClass="error-message" ValidationGroup="Step1" />
                </div>
                <p>
                    <asp:Button ID="btnStep1Next" runat="server" Text="Next >>" CssClass="button-primary" OnClick="btnStep1Next_Click" ValidationGroup="Step1" />
                </p>
            </div>
        </asp:View>

        <asp:View ID="vwStep2" runat="server">
            <div class="form-section">
                <h3>Step 2: Applicant Details</h3>
                <div class="form-row">
                    <label>Applicant Name:</label>
                    <asp:TextBox ID="txtApplicantName" runat="server" MaxLength="100" />
                    <asp:RequiredFieldValidator ID="rfvApplicantName" runat="server" ControlToValidate="txtApplicantName" 
                        ErrorMessage="Applicant name is required" Display="Dynamic" CssClass="error-message" ValidationGroup="Step2" />
                </div>
                <div class="form-row">
                    <label>Email:</label>
                    <asp:TextBox ID="txtEmail" runat="server" MaxLength="100" />
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" 
                        ErrorMessage="Email is required" Display="Dynamic" CssClass="error-message" ValidationGroup="Step2" />
                    <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                        ErrorMessage="Invalid email format" Display="Dynamic" CssClass="error-message" ValidationGroup="Step2" />
                </div>
                <div class="form-row">
                    <label>Phone:</label>
                    <asp:TextBox ID="txtPhone" runat="server" MaxLength="20" />
                    <asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="txtPhone" 
                        ErrorMessage="Phone is required" Display="Dynamic" CssClass="error-message" ValidationGroup="Step2" />
                </div>
                <div class="form-row">
                    <label>Company:</label>
                    <asp:TextBox ID="txtCompany" runat="server" MaxLength="100" />
                </div>
                <div class="form-row">
                    <label>Contractor License #:</label>
                    <asp:TextBox ID="txtLicenseNumber" runat="server" MaxLength="50" />
                </div>
                <p>
                    <asp:Button ID="btnStep2Prev" runat="server" Text="<< Previous" CssClass="button-secondary" OnClick="btnStep2Prev_Click" CausesValidation="false" />
                    <asp:Button ID="btnStep2Next" runat="server" Text="Next >>" CssClass="button-primary" OnClick="btnStep2Next_Click" ValidationGroup="Step2" />
                </p>
            </div>
        </asp:View>

        <asp:View ID="vwStep3" runat="server">
            <div class="form-section">
                <h3>Step 3: Project Details</h3>
                <div class="form-row">
                    <label>Permit Type:</label>
                    <asp:DropDownList ID="ddlPermitType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPermitType_SelectedIndexChanged">
                        <asp:ListItem Value="">-- Select Type --</asp:ListItem>
                        <asp:ListItem Value="NewConstruction">New Construction</asp:ListItem>
                        <asp:ListItem Value="Addition">Addition/Alteration</asp:ListItem>
                        <asp:ListItem Value="Electrical">Electrical</asp:ListItem>
                        <asp:ListItem Value="Plumbing">Plumbing</asp:ListItem>
                        <asp:ListItem Value="Mechanical">Mechanical</asp:ListItem>
                        <asp:ListItem Value="Demolition">Demolition</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvPermitType" runat="server" ControlToValidate="ddlPermitType" 
                        InitialValue="" ErrorMessage="Permit type is required" Display="Dynamic" CssClass="error-message" ValidationGroup="Step3" />
                </div>
                <div class="form-row">
                    <label>Project Description:</label>
                    <asp:TextBox ID="txtProjectDescription" runat="server" TextMode="MultiLine" Rows="5" MaxLength="1000" />
                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtProjectDescription" 
                        ErrorMessage="Project description is required" Display="Dynamic" CssClass="error-message" ValidationGroup="Step3" />
                </div>
                <div class="form-row">
                    <label>Estimated Cost:</label>
                    <asp:TextBox ID="txtEstimatedCost" runat="server" MaxLength="15" />
                    <asp:RequiredFieldValidator ID="rfvCost" runat="server" ControlToValidate="txtEstimatedCost" 
                        ErrorMessage="Estimated cost is required" Display="Dynamic" CssClass="error-message" ValidationGroup="Step3" />
                    <asp:RangeValidator ID="rvCost" runat="server" ControlToValidate="txtEstimatedCost"
                        MinimumValue="1" MaximumValue="99999999" Type="Double"
                        ErrorMessage="Cost must be a valid number" Display="Dynamic" CssClass="error-message" ValidationGroup="Step3" />
                </div>
                <div class="form-row">
                    <label>Square Footage:</label>
                    <asp:TextBox ID="txtSquareFootage" runat="server" MaxLength="10" />
                </div>
                <asp:UpdatePanel ID="upFeeCalculation" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="form-row">
                            <label>Estimated Permit Fee:</label>
                            <asp:Label ID="lblEstimatedFee" runat="server" Font-Bold="true" Text="$0.00" />
                            <asp:Button ID="btnCalculateFee" runat="server" Text="Calculate Fee" CssClass="button-secondary" OnClick="btnCalculateFee_Click" CausesValidation="false" />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnCalculateFee" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
                <p>
                    <asp:Button ID="btnStep3Prev" runat="server" Text="<< Previous" CssClass="button-secondary" OnClick="btnStep3Prev_Click" CausesValidation="false" />
                    <asp:Button ID="btnStep3Next" runat="server" Text="Next >>" CssClass="button-primary" OnClick="btnStep3Next_Click" ValidationGroup="Step3" />
                </p>
            </div>
        </asp:View>

        <asp:View ID="vwStep4" runat="server">
            <div class="form-section">
                <h3>Step 4: Review & Submit</h3>
                <asp:Panel ID="pnlReview" runat="server">
                    <h4>Property Information</h4>
                    <p><strong>Address:</strong> <asp:Label ID="lblReviewAddress" runat="server" /></p>
                    <p><strong>Parcel:</strong> <asp:Label ID="lblReviewParcel" runat="server" /></p>
                    <p><strong>Zoning:</strong> <asp:Label ID="lblReviewZoning" runat="server" /></p>

                    <h4>Applicant Details</h4>
                    <p><strong>Name:</strong> <asp:Label ID="lblReviewApplicant" runat="server" /></p>
                    <p><strong>Email:</strong> <asp:Label ID="lblReviewEmail" runat="server" /></p>
                    <p><strong>Phone:</strong> <asp:Label ID="lblReviewPhone" runat="server" /></p>

                    <h4>Project Details</h4>
                    <p><strong>Permit Type:</strong> <asp:Label ID="lblReviewPermitType" runat="server" /></p>
                    <p><strong>Description:</strong> <asp:Label ID="lblReviewDescription" runat="server" /></p>
                    <p><strong>Estimated Cost:</strong> <asp:Label ID="lblReviewCost" runat="server" /></p>
                    <p><strong>Permit Fee:</strong> <asp:Label ID="lblReviewFee" runat="server" /></p>
                </asp:Panel>
                <asp:Panel ID="pnlSubmitMessage" runat="server" Visible="false" CssClass="success-message">
                    <h3>Application Submitted Successfully!</h3>
                    <p>Your permit application has been submitted. Permit ID: <asp:Label ID="lblPermitId" runat="server" Font-Bold="true" /></p>
                    <p>You will receive an email confirmation at <asp:Label ID="lblConfirmEmail" runat="server" />.</p>
                    <p>Expected review time: 5-10 business days.</p>
                    <p><asp:HyperLink ID="lnkTrackPermit" runat="server" NavigateUrl="~/Pages/PermitSearch.aspx">Track Your Permit</asp:HyperLink></p>
                </asp:Panel>
                <p>
                    <asp:Button ID="btnStep4Prev" runat="server" Text="<< Previous" CssClass="button-secondary" OnClick="btnStep4Prev_Click" CausesValidation="false" />
                    <asp:Button ID="btnSubmit" runat="server" Text="Submit Application" CssClass="button-primary" OnClick="btnSubmit_Click" CausesValidation="false" />
                </p>
            </div>
        </asp:View>
    </asp:MultiView>
</asp:Content>
