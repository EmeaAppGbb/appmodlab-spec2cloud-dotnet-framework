using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RiverdalePermitSystem.Web.Pages
{
    public partial class InspectionSchedule : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadInspections();
            }
        }

        private void LoadInspections()
        {
            DataTable dt = InspectionDataAccess.GetUpcomingInspections();
            gvInspections.DataSource = dt;
            gvInspections.DataBind();
        }

        protected void btnSchedule_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    string inspectionId = InspectionDataAccess.ScheduleInspection(
                        txtPermitId.Text,
                        ddlInspectionType.SelectedValue,
                        DateTime.Parse(txtRequestedDate.Text),
                        txtNotes.Text
                    );

                    lblMessage.Text = $"Inspection {inspectionId} scheduled successfully!";
                    lblMessage.Visible = true;

                    txtPermitId.Text = string.Empty;
                    ddlInspectionType.SelectedIndex = 0;
                    txtRequestedDate.Text = string.Empty;
                    txtNotes.Text = string.Empty;

                    LoadInspections();
                }
                catch (Exception ex)
                {
                    lblMessage.Text = $"Error scheduling inspection: {ex.Message}";
                    lblMessage.CssClass = "warning-message";
                    lblMessage.Visible = true;
                }
            }
        }

        protected void gvInspections_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string inspectionId = e.CommandArgument.ToString();

            if (e.CommandName == "CompleteInspection")
            {
                InspectionDataAccess.CompleteInspection(inspectionId, "Passed");
                LoadInspections();
            }
            else if (e.CommandName == "CancelInspection")
            {
                InspectionDataAccess.CancelInspection(inspectionId);
                LoadInspections();
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadInspections();
        }
    }
}
