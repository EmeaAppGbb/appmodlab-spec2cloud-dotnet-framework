namespace RiverdalePermitSystem.Web.Pages
{
    public partial class InspectionSchedule
    {
        protected global::System.Web.UI.WebControls.TextBox txtPermitId;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvPermitId;
        protected global::System.Web.UI.WebControls.DropDownList ddlInspectionType;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvInspectionType;
        protected global::System.Web.UI.WebControls.TextBox txtRequestedDate;
        protected global::System.Web.UI.WebControls.RequiredFieldValidator rfvDate;
        protected global::System.Web.UI.WebControls.TextBox txtNotes;
        protected global::System.Web.UI.WebControls.Button btnSchedule;
        protected global::System.Web.UI.WebControls.Label lblMessage;
        protected global::System.Web.UI.UpdatePanel upInspections;
        protected global::System.Web.UI.WebControls.GridView gvInspections;
        protected global::System.Web.UI.WebControls.Button btnRefresh;
    }
}
