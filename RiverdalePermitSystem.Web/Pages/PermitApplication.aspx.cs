using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace RiverdalePermitSystem.Web.Pages
{
    public partial class PermitApplication : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UpdateWizardSteps();
            }
        }

        private void UpdateWizardSteps()
        {
            int activeStep = mvWizard.ActiveViewIndex;
            
            step1Indicator.Attributes["class"] = activeStep == 0 ? "wizard-step active" : (activeStep > 0 ? "wizard-step completed" : "wizard-step");
            step2Indicator.Attributes["class"] = activeStep == 1 ? "wizard-step active" : (activeStep > 1 ? "wizard-step completed" : "wizard-step");
            step3Indicator.Attributes["class"] = activeStep == 2 ? "wizard-step active" : (activeStep > 2 ? "wizard-step completed" : "wizard-step");
            step4Indicator.Attributes["class"] = activeStep == 3 ? "wizard-step active" : "wizard-step";
        }

        protected void btnStep1Next_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveToViewState();
                mvWizard.ActiveViewIndex = 1;
                UpdateWizardSteps();
            }
        }

        protected void btnStep2Prev_Click(object sender, EventArgs e)
        {
            mvWizard.ActiveViewIndex = 0;
            UpdateWizardSteps();
        }

        protected void btnStep2Next_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveToViewState();
                mvWizard.ActiveViewIndex = 2;
                UpdateWizardSteps();
            }
        }

        protected void btnStep3Prev_Click(object sender, EventArgs e)
        {
            mvWizard.ActiveViewIndex = 1;
            UpdateWizardSteps();
        }

        protected void btnStep3Next_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveToViewState();
                LoadReviewData();
                mvWizard.ActiveViewIndex = 3;
                UpdateWizardSteps();
            }
        }

        protected void btnStep4Prev_Click(object sender, EventArgs e)
        {
            mvWizard.ActiveViewIndex = 2;
            UpdateWizardSteps();
        }

        protected void ddlPermitType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateFee();
        }

        protected void btnCalculateFee_Click(object sender, EventArgs e)
        {
            CalculateFee();
        }

        private void CalculateFee()
        {
            decimal estimatedCost = 0;
            if (decimal.TryParse(txtEstimatedCost.Text, out estimatedCost) && estimatedCost > 0)
            {
                decimal fee = PermitDataAccess.CalculatePermitFee(ddlPermitType.SelectedValue, estimatedCost);
                lblEstimatedFee.Text = fee.ToString("C");
                ViewState["CalculatedFee"] = fee;
            }
            else
            {
                lblEstimatedFee.Text = "$0.00";
            }
        }

        private void SaveToViewState()
        {
            ViewState["PropertyAddress"] = ucAddress.Address;
            ViewState["ParcelNumber"] = txtParcelNumber.Text;
            ViewState["Zoning"] = ddlZoning.SelectedValue;
            ViewState["ApplicantName"] = txtApplicantName.Text;
            ViewState["Email"] = txtEmail.Text;
            ViewState["Phone"] = txtPhone.Text;
            ViewState["Company"] = txtCompany.Text;
            ViewState["LicenseNumber"] = txtLicenseNumber.Text;
            ViewState["PermitType"] = ddlPermitType.SelectedValue;
            ViewState["ProjectDescription"] = txtProjectDescription.Text;
            ViewState["EstimatedCost"] = txtEstimatedCost.Text;
            ViewState["SquareFootage"] = txtSquareFootage.Text;
        }

        private void LoadReviewData()
        {
            lblReviewAddress.Text = ViewState["PropertyAddress"]?.ToString();
            lblReviewParcel.Text = ViewState["ParcelNumber"]?.ToString();
            lblReviewZoning.Text = ViewState["Zoning"]?.ToString();
            lblReviewApplicant.Text = ViewState["ApplicantName"]?.ToString();
            lblReviewEmail.Text = ViewState["Email"]?.ToString();
            lblReviewPhone.Text = ViewState["Phone"]?.ToString();
            lblReviewPermitType.Text = ViewState["PermitType"]?.ToString();
            lblReviewDescription.Text = ViewState["ProjectDescription"]?.ToString();
            lblReviewCost.Text = decimal.Parse(ViewState["EstimatedCost"]?.ToString() ?? "0").ToString("C");
            lblReviewFee.Text = ViewState["CalculatedFee"]?.ToString() ?? "$0.00";
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtApplicant = new DataTable();
                dtApplicant.Columns.Add("Name");
                dtApplicant.Columns.Add("Email");
                dtApplicant.Columns.Add("Phone");
                dtApplicant.Columns.Add("Company");
                dtApplicant.Columns.Add("LicenseNumber");
                DataRow dr = dtApplicant.NewRow();
                dr["Name"] = ViewState["ApplicantName"];
                dr["Email"] = ViewState["Email"];
                dr["Phone"] = ViewState["Phone"];
                dr["Company"] = ViewState["Company"] ?? "";
                dr["LicenseNumber"] = ViewState["LicenseNumber"] ?? "";
                dtApplicant.Rows.Add(dr);

                string permitId = PermitDataAccess.SubmitPermitApplication(
                    ViewState["PropertyAddress"]?.ToString(),
                    ViewState["ParcelNumber"]?.ToString(),
                    ViewState["Zoning"]?.ToString(),
                    ViewState["PermitType"]?.ToString(),
                    ViewState["ProjectDescription"]?.ToString(),
                    decimal.Parse(ViewState["EstimatedCost"]?.ToString() ?? "0"),
                    dtApplicant
                );

                lblPermitId.Text = permitId;
                lblConfirmEmail.Text = ViewState["Email"]?.ToString();
                
                EmailHelper.SendPermitConfirmation(
                    ViewState["Email"]?.ToString(),
                    ViewState["ApplicantName"]?.ToString(),
                    permitId
                );

                pnlReview.Visible = false;
                pnlSubmitMessage.Visible = true;
                btnStep4Prev.Visible = false;
                btnSubmit.Visible = false;

                Session["SubmittedPermitData"] = dtApplicant;
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('Error submitting application: {ex.Message}');</script>");
            }
        }
    }
}
