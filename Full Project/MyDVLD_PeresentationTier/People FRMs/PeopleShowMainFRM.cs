using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyDVLD_BusinessTier;

namespace MyDVLD_PeresentationTier
{
    public partial class PeopleShowMainFRM : Form
    {
        public PeopleShowMainFRM()
        {
            InitializeComponent();
        }

        private void _RefreshPeopleFRM()
        {
            dgvPeopleManagment.DataSource = clsPeopleManagement.GetAllPeople();
            lblNumberOfPeople.Text = dgvPeopleManagment.Rows.Count.ToString();

            if (dgvPeopleManagment.RowCount < 1)
                return;

            dgvPeopleManagment.Columns[0].HeaderText = "Person ID";
            dgvPeopleManagment.Columns[1].HeaderText = "National No.";
            dgvPeopleManagment.Columns[2].HeaderText = "First Name";
            dgvPeopleManagment.Columns[3].HeaderText = "Second Name";
            dgvPeopleManagment.Columns[4].HeaderText = "Third Name";
            dgvPeopleManagment.Columns[5].HeaderText = "Last Name";
            dgvPeopleManagment.Columns[6].HeaderText = "Date Of Birth";
            dgvPeopleManagment.Columns[7].HeaderText = "Gender";
            dgvPeopleManagment.Columns[11].HeaderText = "Country Name";
            dgvPeopleManagment.Columns[12].HeaderText = "Image Path";
        }

        public static Action RefreshPeople;

        private void PeopleShowMainFRM_Load(object sender, EventArgs e)
        {
            _RefreshPeopleFRM();

            RefreshPeople += _RefreshPeopleFRM;
            cbFillterBy.SelectedIndex = 0;
        }

        private void btnCloseFRM_Click(object sender, EventArgs e) => Close();

        private void cbFillterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFillterBy.SelectedIndex == 0)
            {
                txtFilterBy.Visible = false;
                return;
            }

            txtFilterBy.Visible = true;
            txtFilterBy.Focus();
        }

        private void txtFilterBy_TextChanged(object sender, EventArgs e)
        {
            string FilterBy = cbFillterBy.SelectedItem.ToString().Trim();
            string FilterValue = txtFilterBy.Text.Trim().ToLower();
            string FilterColumn = string.Empty;

            if (string.IsNullOrEmpty(FilterValue))
            {
                _RefreshPeopleFRM();
                return;
            }

            switch (FilterBy)
            {
                case "Person ID":
                    FilterColumn = "PersonID";
                    break;
                case "National No.":
                    FilterColumn = "NationalNo";
                    break;
                case "First Name":
                    FilterColumn = "FirstName";
                    break;
                case "Second Name":
                    FilterColumn = "SecondName";
                    break;
                case "Third Name":
                    FilterColumn = "ThirdName";
                    break;
                case "Last Name":
                    FilterColumn = "LastName";
                    break;
                case "Nationality":
                    FilterColumn = "CountryName";
                    break;
                case "Phone":
                    FilterColumn = "Phone";
                    break;
                case "Email":
                    FilterColumn = "Email";
                    break;
                case "Gender":
                    FilterColumn = "Gender";
                    if (FilterValue == "male" || FilterValue == "m" || FilterValue == "ma" || FilterValue == "mal")
                    {
                        FilterValue = 0.ToString();
                    }
                    else if (FilterValue == "female" || FilterValue == "f" || FilterValue == "fe" || FilterValue == "fem" || FilterValue == "fema" || FilterValue == "femal")
                    {
                        FilterValue = 1.ToString();
                    }
                    break;
                default:
                    FilterColumn = "None";
                    break;
            }

            dgvPeopleManagment.DataSource = clsPeopleManagement.GetPeopleFiltered(FilterColumn, FilterValue);
            lblNumberOfPeople.Text = dgvPeopleManagment.Rows.Count.ToString();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int SelectedPersonID = Convert.ToInt32(dgvPeopleManagment.CurrentRow.Cells[0].Value);

            if (MessageBox.Show($"Are you sure you want to delete this person with id:{SelectedPersonID}?", 
                "Delete Person", MessageBoxButtons.YesNo, MessageBoxIcon.Warning , MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if(clsPeopleManagement.DeletePersonByID(SelectedPersonID))
                {
                    MessageBox.Show("Person deleted successfully.", "Delete Person", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshPeopleFRM();
                }
                else
                    MessageBox.Show("Failed to delete person.", "Delete Person", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void phoneCallToolStripMenuItem_Click(object sender, EventArgs e) => MessageBox.Show("This feature did not implemented yet", "Phone Call", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private void sendEmailToolStripMenuItem_Click(object sender, EventArgs e) => MessageBox.Show("This feature did not implemented yet", "Phone Call", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private void _ShowPersonDetails()
        {
            ShowPersonDetailsFRM ShowDetails = new ShowPersonDetailsFRM((int)dgvPeopleManagment.CurrentRow.Cells[0].Value);
            ShowDetails.ShowDialog();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e) => _ShowPersonDetails();

        private void dgvPeopleManagment_MouseDoubleClick(object sender, MouseEventArgs e) => _ShowPersonDetails();

        private void updatePersonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddOrUpdatePersonFRM AddPerson = new AddOrUpdatePersonFRM((int)dgvPeopleManagment.CurrentRow.Cells[0].Value);
            AddPerson.ShowDialog();
            _RefreshPeopleFRM();
        }

        private void btnAddPerson_Click(object sender, EventArgs e)
        {
            AddOrUpdatePersonFRM AddPerson = new AddOrUpdatePersonFRM();
            AddPerson.ShowDialog();
            _RefreshPeopleFRM();
        }

        private void txtFilterBy_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFillterBy.SelectedIndex == 1 || cbFillterBy.SelectedIndex == 10)
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true; 
        }
    }
}
