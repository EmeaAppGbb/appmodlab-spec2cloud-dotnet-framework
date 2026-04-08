using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RiverdalePermitSystem.Web.Pages
{
    public partial class PlanReview : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblReviewer.Text = Session["UserId"]?.ToString().Substring(0, 8) ?? "Reviewer";
            }
        }

        protected void btnLoadPermit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPermitId.Text))
            {
                DataTable permit = PermitDataAccess.GetPermitById(txtPermitId.Text);
                if (permit.Rows.Count > 0)
                {
                    ucPermitHeader.PermitId = txtPermitId.Text;
                    ucPermitHeader.PropertyAddress = permit.Rows[0]["PropertyAddress"].ToString();
                    ucPermitHeader.Status = permit.Rows[0]["Status"].ToString();

                    LoadReviewHistory();

                    pnlReviewForm.Visible = true;
                }
                else
                {
                    Response.Write("<script>alert('Permit not found.');</script>");
                }
            }
        }

        private void LoadReviewHistory()
        {
            DataTable history = PermitDataAccess.GetPlanReviewHistory(txtPermitId.Text);
            gvReviewHistory.DataSource = history;
            gvReviewHistory.DataBind();
        }

        protected void btnSubmitReview_Click(object sender, EventArgs e)
        {
            try
            {
                string deficiencies = string.Empty;
                foreach (ListItem item in cblDeficiencies.Items)
                {
                    if (item.Selected)
                    {
                        deficiencies += item.Value + "; ";
                    }
                }

                string reviewId = PermitDataAccess.SubmitPlanReview(
                    txtPermitId.Text,
                    ddlReviewType.SelectedValue,
                    lblReviewer.Text,
                    ddlReviewStatus.SelectedValue,
                    txtComments.Text,
                    deficiencies
                );

                lblReviewMessage.Text = $"Review {reviewId} submitted successfully!";
                lblReviewMessage.Visible = true;

                ddlReviewStatus.SelectedIndex = 0;
                txtComments.Text = string.Empty;
                cblDeficiencies.ClearSelection();

                LoadReviewHistory();
            }
            catch (Exception ex)
            {
                lblReviewMessage.Text = $"Error submitting review: {ex.Message}";
                lblReviewMessage.CssClass = "warning-message";
                lblReviewMessage.Visible = true;
            }
        }
    }
}
