using MyDVLD_BusinessTier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyDVLD_PeresentationTier.Licenses.Detain_Licenses
{
    public partial class ShowDetainedLicensesListFRM : Form
    {
        public ShowDetainedLicensesListFRM() => InitializeComponent();

        private DataTable _dtAllDetainedLicense = new DataTable();

        private void btnCloseFRM_Click(object sender, EventArgs e) => this.Close();

        private void _LoadDetainedDataGridView()
        {
            _dtAllDetainedLicense = clsDetainedLicensesManagement.GetAllDetainedLicenses();
            dgvDetainedLicensesManagment.DataSource = _dtAllDetainedLicense;

            if (dgvDetainedLicensesManagment.RowCount < 1)
                return;

            dgvDetainedLicensesManagment.Columns[0].HeaderText = "D.ID";
            dgvDetainedLicensesManagment.Columns[0].Width = 90;

            dgvDetainedLicensesManagment.Columns[1].HeaderText = "L.ID";
            dgvDetainedLicensesManagment.Columns[1].Width = 90;

            dgvDetainedLicensesManagment.Columns[2].HeaderText = "D.Date";
            dgvDetainedLicensesManagment.Columns[2].Width = 160;

            dgvDetainedLicensesManagment.Columns[3].HeaderText = "Is Released";
            dgvDetainedLicensesManagment.Columns[3].Width = 110;

            dgvDetainedLicensesManagment.Columns[4].HeaderText = "Fine Fees";
            dgvDetainedLicensesManagment.Columns[4].Width = 110;

            dgvDetainedLicensesManagment.Columns[5].HeaderText = "Release Date";
            dgvDetainedLicensesManagment.Columns[5].Width = 160;

            dgvDetainedLicensesManagment.Columns[6].HeaderText = "N.No.";
            dgvDetainedLicensesManagment.Columns[6].Width = 90;

            dgvDetainedLicensesManagment.Columns[7].HeaderText = "Full Name";
            dgvDetainedLicensesManagment.Columns[7].Width = 260;

            dgvDetainedLicensesManagment.Columns[8].HeaderText = "Rlease App.ID";
            dgvDetainedLicensesManagment.Columns[8].Width = 150;

            lblNumberOfDetainedLicenses.Text = dgvDetainedLicensesManagment.RowCount.ToString();
        }

        private void ShowDetainedLicensesListFRM_Load(object sender, EventArgs e)
        {
            cbFillterOptions.SelectedIndex = 0;
            ReleaseDetainedLicenseToolStripe.Enabled = false;

            _LoadDetainedDataGridView();
        }

        private void btnReleasebtnDetainLicense_Click(object sender, EventArgs e)
        {
            ReleaseDetainedLicenseFRM releaseDetainedLicenseFRM = new ReleaseDetainedLicenseFRM();
            releaseDetainedLicenseFRM.Show();

            _LoadDetainedDataGridView();
        }

        private void btnDetainLicense_Click(object sender, EventArgs e)
        {
            DetainLicensesFRM detainLicensesFRM = new DetainLicensesFRM();
            detainLicensesFRM.Show();

            _LoadDetainedDataGridView();
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            ReleaseDetainedLicenseToolStripe.Enabled = 
                         (Boolean)dgvDetainedLicensesManagment.CurrentRow.Cells[3].Value ? false : 
                         (!clsLicenseManagement.Find((int)dgvDetainedLicensesManagment.CurrentRow.Cells[1].Value).IsActive? false : true);
        }

        private void showPerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPersonDetailsFRM ShowPersonDetails = new ShowPersonDetailsFRM(dgvDetainedLicensesManagment.CurrentRow.Cells[6].Value.ToString());
            ShowPersonDetails.ShowDialog();
        }

        private void showLicenseInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowLocalLicenseInfoFRM LicenseInfo = new ShowLocalLicenseInfoFRM((int)dgvDetainedLicensesManagment.CurrentRow.Cells[1].Value);
            LicenseInfo.ShowDialog();
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int PersonID = clsPeopleManagement.Find(dgvDetainedLicensesManagment.CurrentRow.Cells[6].Value.ToString()).PersonID;

            ShowLicensesHistoryFRM LicenseHis = new ShowLicensesHistoryFRM(PersonID);
            LicenseHis.ShowDialog();
        }

        private void ReleaseDetainedLicenseToolStripe_Click(object sender, EventArgs e)
        {
            ReleaseDetainedLicenseFRM DetainedLicense = new ReleaseDetainedLicenseFRM((int)dgvDetainedLicensesManagment.CurrentRow.Cells[0].Value);
            DetainedLicense.ShowDialog();

            _LoadDetainedDataGridView();
        }

        private void tbFillterValue_TextChanged(object sender, EventArgs e)
        {
            if(dgvDetainedLicensesManagment.RowCount < 1)
                return;

            string FilterColumn = "";
            switch (cbFillterOptions.Text)
            {
                case "Detain ID":
                    FilterColumn = "DetainID";
                    break;
                case "Is Released":
                    FilterColumn = "IsReleased";
                    break;
                case "National No.":
                    FilterColumn = "NationalNo";
                    break;
                case "Full Name":
                    FilterColumn = "FullName";
                    break;
                case "Release Application ID":
                    FilterColumn = "ReleaseApplicationID";
                    break;
                default:
                    FilterColumn = "None";
                    break;
            }

            if (tbFillterValue.Text.Trim() == "" || FilterColumn == "None")
            {
                _dtAllDetainedLicense.DefaultView.RowFilter = "";
                lblNumberOfDetainedLicenses.Text = dgvDetainedLicensesManagment.Rows.Count.ToString();
                return;
            }


            if (FilterColumn == "DetainID" || FilterColumn == "ReleaseApplicationID")
                _dtAllDetainedLicense.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, tbFillterValue.Text.Trim());
            else
                _dtAllDetainedLicense.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterColumn, tbFillterValue.Text.Trim());

            lblNumberOfDetainedLicenses.Text = _dtAllDetainedLicense.Rows.Count.ToString();
        }

        private void cbFillterOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFillterOptions.Text == "Is Released")
            {
                tbFillterValue.Visible = false;
                cbIsReleased.Visible = true;
                cbIsReleased.Focus();
                cbIsReleased.SelectedIndex = 0;
            }
            else
            {
                tbFillterValue.Visible = (cbFillterOptions.Text != "None");
                cbIsReleased.Visible = false;

                if (cbFillterOptions.Text == "None")
                {
                    tbFillterValue.Enabled = false;
                }
                else
                    tbFillterValue.Enabled = true;

                tbFillterValue.Text = "";
                tbFillterValue.Focus();
            }
        }

        private void cbIsReleased_SelectedIndexChanged(object sender, EventArgs e)
        {
            string FilterColumn = "IsReleased";
            string FilterValue = cbIsReleased.Text;

            switch (FilterValue)
            {
                case "All":
                    break;
                case "Yes":
                    FilterValue = "1";
                    break;
                case "No":
                    FilterValue = "0";
                    break;
            }

            if (FilterValue == "All")
                _dtAllDetainedLicense.DefaultView.RowFilter = "";
            else
                _dtAllDetainedLicense.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, FilterValue);

            lblNumberOfDetainedLicenses.Text = _dtAllDetainedLicense.Rows.Count.ToString();
        }

        private void cbFillterOptions_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFillterOptions.Text == "Detain ID" || cbFillterOptions.Text == "Release Application ID")
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
