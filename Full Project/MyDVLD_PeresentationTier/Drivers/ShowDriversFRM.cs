using MyDVLD_BusinessTier;
using MyDVLD_PeresentationTier.Licenses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyDVLD_PeresentationTier.Drivers
{
    public partial class ShowDriversFRM : Form
    {
        public ShowDriversFRM() => InitializeComponent();

        private void btnCloseFRM_Click(object sender, EventArgs e) => this.Close();

        private void _LoadDrivers()
        {
            _dtAllDrivers = clsDriversManagement.GetAllDrivers();
            dgvDriverManagment.DataSource = _dtAllDrivers;

            if (dgvDriverManagment.RowCount < 1)
                return;

            dgvDriverManagment.Columns[0].HeaderText = "Driver ID";
            dgvDriverManagment.Columns[0].Width = 90;

            dgvDriverManagment.Columns[1].HeaderText = "Person ID";
            dgvDriverManagment.Columns[1].Width = 100;

            dgvDriverManagment.Columns[2].HeaderText = "National No.";
            dgvDriverManagment.Columns[2].Width = 120;

            dgvDriverManagment.Columns[3].HeaderText = "Full Name";
            dgvDriverManagment.Columns[3].Width = 250;

            dgvDriverManagment.Columns[4].HeaderText = "Date";
            dgvDriverManagment.Columns[4].Width = 150;

            dgvDriverManagment.Columns[5].HeaderText = "Active Licenses";
            dgvDriverManagment.Columns[5].Width = 120;

            lblNumberOfDrivers.Text = dgvDriverManagment.RowCount.ToString();
        }

        private DataTable _dtAllDrivers = new DataTable();

        private void ShowDriversFRM_Load(object sender, EventArgs e)
        {
            cbFillterOptions.SelectedIndex = 0;
            _LoadDrivers();
        }

        private void cbFillterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbFillterValue.Text = string.Empty;

            if (cbFillterOptions.SelectedIndex != 0)
                tbFillterValue.Visible = true;
            else
                tbFillterValue.Visible = false;
        }

        private void tbFillterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFillterOptions.SelectedIndex == 1 || cbFillterOptions.SelectedIndex == 2)
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
        }

        private void tbFillterValue_TextChanged(object sender, EventArgs e)
        {
            if (dgvDriverManagment.RowCount < 1)
                return;

            string FilterColumn = "";

            switch (cbFillterOptions.Text)
            {
                case "Driver ID":
                    FilterColumn = "DriverID";
                    break;
                case "Person ID":
                    FilterColumn = "PersonID";
                    break;
                case "National No.":
                    FilterColumn = "NationalNo";
                    break;
                case "Full Name":
                    FilterColumn = "FullName";
                    break;
                default:
                    FilterColumn = "None";
                    break;
            }

            if (tbFillterValue.Text.Trim() == "" || FilterColumn == "None")
            {
                _dtAllDrivers.DefaultView.RowFilter = "";
                lblNumberOfDrivers.Text = dgvDriverManagment.Rows.Count.ToString();
                return;
            }

            if (FilterColumn != "FullName" && FilterColumn != "NationalNo")
                _dtAllDrivers.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, tbFillterValue.Text.Trim());
            else
                _dtAllDrivers.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterColumn, tbFillterValue.Text.Trim());

            lblNumberOfDrivers.Text = _dtAllDrivers.Rows.Count.ToString();
        }

        private void showPerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowPersonDetailsFRM ShowPersonDetails = new ShowPersonDetailsFRM((int)dgvDriverManagment.CurrentRow.Cells[1].Value);
            ShowPersonDetails.ShowDialog();
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowLicensesHistoryFRM LicensesHistory = new ShowLicensesHistoryFRM((int)dgvDriverManagment.CurrentRow.Cells[1].Value);
            LicensesHistory.ShowDialog();
        }
    }
}
