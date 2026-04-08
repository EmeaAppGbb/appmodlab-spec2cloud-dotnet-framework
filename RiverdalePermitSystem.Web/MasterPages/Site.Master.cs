using System;
using System.Web.UI;

namespace RiverdalePermitSystem.Web.MasterPages
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblUserName.Text = Session["UserId"]?.ToString().Substring(0, 8) ?? "Guest";
                lblUserRole.Text = Session["UserRole"]?.ToString() ?? "Guest";
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Default.aspx");
        }
    }
}
