using System;
using System.Data;
using System.Web.UI;

namespace RiverdalePermitSystem.Web.Pages
{
    public partial class Dashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            DataTable stats = PermitDataAccess.GetDashboardStatistics();
            if (stats.Rows.Count > 0)
            {
                lblTotalPermits.Text = stats.Rows[0]["TotalPermits"].ToString();
                lblPendingReview.Text = stats.Rows[0]["PendingReview"].ToString();
                lblInspectionsToday.Text = stats.Rows[0]["InspectionsToday"].ToString();
                lblMonthlyRevenue.Text = decimal.Parse(stats.Rows[0]["MonthlyRevenue"].ToString()).ToString("C");
            }

            DataTable activity = PermitDataAccess.GetRecentActivity(20);
            gvRecentActivity.DataSource = activity;
            gvRecentActivity.DataBind();

            DataTable statusSummary = PermitDataAccess.GetPermitsByStatus();
            gvStatusSummary.DataSource = statusSummary;
            gvStatusSummary.DataBind();

            lblLastUpdate.Text = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
        }

        protected void btnRefreshStats_Click(object sender, EventArgs e)
        {
            LoadDashboardData();
        }
    }
}
