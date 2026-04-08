using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RiverdalePermitSystem.Web.Pages
{
    public partial class PermitSearch : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string permitId = Request.QueryString["permitId"];
                if (!string.IsNullOrEmpty(permitId))
                {
                    txtPermitId.Text = permitId;
                    btnSearch_Click(null, null);
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvPermits.DataBind();
            pnlPermitDetails.Visible = false;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtPermitId.Text = string.Empty;
            txtAddress.Text = string.Empty;
            ddlPermitType.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            gvPermits.DataBind();
            pnlPermitDetails.Visible = false;
        }

        protected void gvPermits_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string permitId = e.CommandArgument.ToString();
            
            if (e.CommandName == "ViewDetails")
            {
                Session["SelectedPermitId"] = permitId;
                pnlPermitDetails.Visible = true;
                fvPermitDetails.DataBind();
            }
            else if (e.CommandName == "GenerateReport")
            {
                Response.Write($"<script>alert('Crystal Report generation for Permit {permitId} would be triggered here.');</script>");
            }
        }

        protected void gvPermits_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPermits.PageIndex = e.NewPageIndex;
            gvPermits.DataBind();
        }

        protected void btnCloseDetails_Click(object sender, EventArgs e)
        {
            pnlPermitDetails.Visible = false;
        }
    }
}
