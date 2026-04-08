using System;
using System.Collections.Generic;
using System.Web.UI;

namespace RiverdalePermitSystem.Web.UserControls
{
    public partial class AddressLookup : UserControl
    {
        public string Address
        {
            get { return txtAddress.Text; }
            set { txtAddress.Text = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLookup_Click(object sender, EventArgs e)
        {
            List<string> addresses = new List<string>
            {
                "123 Main Street, Riverdale",
                "456 Oak Avenue, Riverdale",
                "789 Elm Drive, Riverdale",
                "321 Maple Lane, Riverdale",
                "654 Pine Road, Riverdale"
            };

            lstSuggestions.DataSource = addresses;
            lstSuggestions.DataBind();
            pnlSuggestions.Visible = true;
        }

        protected void lstSuggestions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSuggestions.SelectedIndex >= 0)
            {
                txtAddress.Text = lstSuggestions.SelectedValue;
                pnlSuggestions.Visible = false;
            }
        }
    }
}
