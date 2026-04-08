using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RiverdalePermitSystem.Web
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadRecentPermits();
            }
        }

        private void LoadRecentPermits()
        {
            DataTable dt = PermitDataAccess.GetRecentPermits(10);
            gvRecentPermits.DataSource = dt;
            gvRecentPermits.DataBind();
            lblLastUpdate.Text = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt");
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRecentPermits();
        }

        protected void gvRecentPermits_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewDetails")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = gvRecentPermits.Rows[rowIndex];
                string permitId = row.Cells[0].Text;
                Response.Redirect($"~/Pages/PermitSearch.aspx?permitId={permitId}");
            }
        }
    }
}
