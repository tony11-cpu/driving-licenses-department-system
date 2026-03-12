using MyDVLD_BusinessTier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyDVLD_PeresentationTier.Users_FRMs
{
    public partial class AddNewUserFRM : Form
    {
        public AddNewUserFRM(int UserID = -1)
        {
            InitializeComponent();

            if (UserID != -1)
                NewUser = clsUsersManagement.Find(UserID);
        }

        private void btnCloseFRM_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void UpdatePersonMode()
        {
            ctrlShowPersonDetailsWithSearch1.FilterEnabled = false;
            lblUserId.Text = NewUser.UserID.ToString().Trim();
            lblAddNewUserUI.Text = "Update User";
            pUserInfo.Enabled = true;

            tbPassword.Enabled = false;
            tbConfirmPassword.Enabled = false;

            this.Text = "Update Person";
        }

        private clsUsersManagement NewUser = clsUsersManagement.Find(-1);

        private bool _ValidateUser()
        {
            if (NewUser.PerosnInfo.PersonID == -1)
            {
                MessageBox.Show($"Please Make Sure To Connect With Person To Contiue The Proccess!", "Connect To Person!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (clsUsersManagement.IsUserExists(NewUser.PerosnInfo.PersonID) && NewUser.Mode == clsUsersManagement.UserMode.AddNew)
            {
                MessageBox.Show($"User With Person Id {NewUser.PerosnInfo.PersonID} Is Exists Please Try Another One!", "User Exsist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void btnNextFRM_Click(object sender, EventArgs e)
        {
            if (_ValidateUser())
                _Contiue();
        }

        private void _Contiue()
        {
            rbIsActiveCheckBox.Checked = true;

            AddNewUserTabControl.SelectedTab = AddNewUserTabControl.TabPages["tpLoginInfo"];

            pUserInfo.Enabled = true;
            btnSaveUser.Enabled = true;
        }

        private void tbUsername_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(tbUsername.Text) || (clsUsersManagement.IsUserExists(tbUsername.Text.Trim()) 
                                                      && (NewUser.Mode == clsUsersManagement.UserMode.AddNew)))
            {
                e.Cancel = true;
                errorProvider1.SetError(tbUsername, "Please Make Sure To Enter Username And It Must Be Not Existed Before!");
            }
            else
                errorProvider1.SetError(tbUsername, "");

            if (NewUser.Mode == clsUsersManagement.UserMode.Update)
            {
                if (NewUser.Username != tbUsername.Text.Trim())
                {
                    if (clsUsersManagement.IsUserExists(tbUsername.Text.Trim()))
                    {
                        e.Cancel = true;
                        errorProvider1.SetError(tbUsername, "Username is used by another user");
                        return;
                    }
                    else
                        errorProvider1.SetError(tbUsername, null);
                }
            }
        }

        private void tbPassword_Validating(object sender, CancelEventArgs e)
        {
            if ((string.IsNullOrEmpty(tbPassword.Text)
                || tbPassword.Text.Length < 8
                || !tbPassword.Text.All(Char.IsLetterOrDigit)
                && tbPassword.Enabled))
            {
                e.Cancel = true;
                errorProvider1.SetError(tbPassword, "Please Make Sure The Password Does Not Contain Special Chars And It Must Be 8 (Letters/Numbers) Or More!");
            }
            else
                errorProvider1.SetError(tbPassword, "");
        }

        private void tbConfirmPassword_Validating(object sender, CancelEventArgs e)
        {
            if (tbConfirmPassword.Text != tbPassword.Text)
            {
                e.Cancel = true;
                errorProvider1.SetError(tbConfirmPassword, "Please Make Sure The Password Is The Same");
            }
            else
                errorProvider1.SetError(tbConfirmPassword, "");
        }

        private void btnSaveUser_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
                return;

            NewUser.Username = tbUsername.Text.Trim();
            NewUser.Password = tbConfirmPassword.Text.Trim();
            NewUser.IsActive = rbIsActiveCheckBox.Checked;

            if (NewUser.Save())
            {
                UpdatePersonMode();
                MessageBox.Show($"User Data Added Successfully.", "User Data Saved!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show($"User Can't Be Added, Please Try Agian!", "Error While Saving", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void AddNewUserFRM_Load(object sender, EventArgs e)
        {
            if(NewUser.PerosnInfo.PersonID != 0)
            {
                if (NewUser.PerosnInfo.PersonID == -1)
                    return;

                ctrlShowPersonDetailsWithSearch1.LoadPersonInfo(NewUser.PerosnInfo.PersonID);

                tbUsername.Text = NewUser.Username;
                tbPassword.Text = NewUser.Password;
                tbConfirmPassword.Text = tbPassword.Text;
                rbIsActiveCheckBox.Checked = NewUser.IsActive;
                btnSaveUser.Enabled = true;

                UpdatePersonMode();
            }
        }

        private void ctrlShowPersonDetailsWithSearch1_OnPersonSelected(object sender, int e)
        {
            if (e != 0)
                NewUser.PerosnInfo = clsPeopleManagement.Find(e);
        }
    }
}
