using System;
using System.Web.UI;

namespace RiverdalePermitSystem.Web.UserControls
{
    public partial class PermitHeader : UserControl
    {
        public string PermitId
        {
            get { return lblPermitId.Text; }
            set { lblPermitId.Text = value; }
        }

        public string PropertyAddress
        {
            get { return lblPropertyAddress.Text; }
            set { lblPropertyAddress.Text = value; }
        }

        public string Status
        {
            get { return lblStatus.Text; }
            set { lblStatus.Text = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
