namespace RiverdalePermitSystem.Web.Pages
{
    public partial class PermitApplication
    {
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl step1Indicator;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl step2Indicator;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl step3Indicator;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl step4Indicator;
        protected global::System.Web.UI.WebControls.MultiView mvWizard;
        protected global::System.Web.UI.WebControls.View vwStep1;
        protected global::RiverdalePermitSystem.Web.UserControls.AddressLookup ucAddress;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvAddress;
        protected global::System.Web.UI.WebControls.TextBox txtParcelNumber;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvParcel;
        protected global::System.Web.UI.WebControls.DropDownList ddlZoning;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvZoning;
        protected global::System.Web.UI.WebControls.Button btnStep1Next;
        protected global::System.Web.UI.WebControls.View vwStep2;
        protected global::System.Web.UI.WebControls.TextBox txtApplicantName;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvApplicantName;
        protected global::System.Web.UI.WebControls.TextBox txtEmail;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvEmail;
        protected global::System.Web.UI.WebControls.RegularExpressionValidator revEmail;
        protected global::System.Web.UI.WebControls.TextBox txtPhone;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvPhone;
        protected global::System.Web.UI.WebControls.TextBox txtCompany;
        protected global::System.Web.UI.WebControls.TextBox txtLicenseNumber;
        protected global::System.Web.UI.WebControls.Button btnStep2Prev;
        protected global::System.Web.UI.WebControls.Button btnStep2Next;
        protected global::System.Web.UI.WebControls.View vwStep3;
        protected global::System.Web.UI.WebControls.DropDownList ddlPermitType;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvPermitType;
        protected global::System.Web.UI.WebControls.TextBox txtProjectDescription;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvDescription;
        protected global::System.Web.UI.WebControls.TextBox txtEstimatedCost;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvCost;
        protected global::System.Web.UI.WebControls.RangeValidator rvCost;
        protected global::System.Web.UI.WebControls.TextBox txtSquareFootage;
        protected global::System.Web.UI.UpdatePanel upFeeCalculation;
        protected global::System.Web.UI.WebControls.Label lblEstimatedFee;
        protected global::System.Web.UI.WebControls.Button btnCalculateFee;
        protected global::System.Web.UI.WebControls.Button btnStep3Prev;
        protected global::System.Web.UI.WebControls.Button btnStep3Next;
        protected global::System.Web.UI.WebControls.View vwStep4;
        protected global::System.Web.UI.WebControls.Panel pnlReview;
        protected global::System.Web.UI.WebControls.Label lblReviewAddress;
        protected global::System.Web.UI.WebControls.Label lblReviewParcel;
        protected global::System.Web.UI.WebControls.Label lblReviewZoning;
        protected global::System.Web.UI.WebControls.Label lblReviewApplicant;
        protected global::System.Web.UI.WebControls.Label lblReviewEmail;
        protected global::System.Web.UI.WebControls.Label lblReviewPhone;
        protected global::System.Web.UI.WebControls.Label lblReviewPermitType;
        protected global::System.Web.UI.WebControls.Label lblReviewDescription;
        protected global::System.Web.UI.WebControls.Label lblReviewCost;
        protected global::System.Web.UI.WebControls.Label lblReviewFee;
        protected global::System.Web.UI.WebControls.Panel pnlSubmitMessage;
        protected global::System.Web.UI.WebControls.Label lblPermitId;
        protected global::System.Web.UI.WebControls.Label lblConfirmEmail;
        protected global::System.Web.UI.WebControls.HyperLink lnkTrackPermit;
        protected global::System.Web.UI.WebControls.Button btnStep4Prev;
        protected global::System.Web.UI.WebControls.Button btnSubmit;
    }
}
