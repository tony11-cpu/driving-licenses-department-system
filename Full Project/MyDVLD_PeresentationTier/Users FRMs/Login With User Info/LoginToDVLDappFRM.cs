using MyDVLD_BusinessTier;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Configuration;

namespace MyDVLD_PeresentationTier.Users_FRMs
{
    public partial class LoginToDVLDappFRM : Form
    {
        private string _UserInfo = null;

        private readonly string _RegistaryUserKey = @"HKEY_CURRENT_USER\Software\DVLD1ProjUserLoggedInData";
        private readonly string _LoginDataSavedVariableName = "UserLoginInformation1";

        private readonly byte _LoginTrails = Convert.ToByte(ConfigurationManager.AppSettings["LoginTrials"]);
        private byte _LoginCounter = 1;

        private bool _CheckUserInfoBeforeLogin()
        {
            clsUtil.SignedInUser = clsUsersManagement.Find(tbUsername.Text.Trim(), tbPassword.Text);

            if (clsUtil.SignedInUser.UserID == -1)
            {
                if (_LoginTrails != _LoginCounter)
                {
                    MessageBox.Show($"Invalid Username/Password! \nYou Have {_LoginTrails - _LoginCounter++} Trials left!", "Login Failed!"
                   , MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("You Exeeds The Number Of Trials To Login!", "Failed To Login!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    clsUtil.LogEvent($"User Try To Login Exeeds Number Of Trials (3)!", EventLogEntryType.Error);
                    this.Close();

                    return false;
                }

                return false;
            }

            if (!clsUtil.SignedInUser.IsActive)
            {
                MessageBox.Show("User Is Deactivated Please Contact Your Admin!", "User Is Not Active!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            _LoginCounter = 1;
            return true;
        }

        public LoginToDVLDappFRM() => InitializeComponent();

        private void btnCloseFRM_Click(object sender, EventArgs e)
        {
            _SaveUserInfo();
            Close();
        }

        private bool _ValidateInputes()
        {
            bool IsInputesValid = true;

            if (string.IsNullOrEmpty(tbUsername.Text))
            {
                errorProvider1.SetError(tbUsername, "Please Make Sure Username Does Not Contain Special Chars Or Empty!");
                IsInputesValid = false;
            }
            else
                errorProvider1.SetError(tbUsername, "");

            if (string.IsNullOrEmpty(tbPassword.Text) || !tbPassword.Text.All(Char.IsLetterOrDigit))
            {
                errorProvider1.SetError(tbPassword, "Please Make Sure Password Does Not Contain Special Chars Or Empty!");
                IsInputesValid = false;
            }
            else
                errorProvider1.SetError(tbPassword, "");

            return IsInputesValid;
        }

        private bool _ValidateBeforeLogin()
        {
            if (!this._ValidateInputes())
            {
                MessageBox.Show("Please Check For Missing Inputes Requirments Before Trying To Login!", "Wrong Inputes!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return _CheckUserInfoBeforeLogin();
        }

        /// <summary>
        /// Validates User As To Met All Business Requirments The Load User Data And Check For Saving...
        /// </summary>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if(!_ValidateBeforeLogin()) 
                return;

            _SaveUserInfo();

            MainAppFRM appFRM = new MainAppFRM();
            appFRM.ShowDialog();
        }

        /// <summary>
        /// Save User Info To Registery Key - "UserLoginInformation1".
        /// If Any Exeption Happen It Shows The Error And Close The Login
        /// </summary>
        private void _SaveUserInfo()
        {
            if (RememberMeCheckBox.Checked)
                _UserInfo = tbUsername.Text.Trim() + "," + clsUtil.clsSecurity.Encrypt(tbPassword.Text.Trim()) + "," + "1";
            else
                _UserInfo = tbUsername.Text.Trim() + "," + clsUtil.clsSecurity.Encrypt(tbPassword.Text.Trim()) + "," + "0";

            try
            {
                Registry.SetValue(_RegistaryUserKey, _LoginDataSavedVariableName, _UserInfo, RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Saving User (UserName , Password And Activity) To Win/Registery With Exception Message: {ex.Message}"
                    , "Error Adding User Creditations!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                clsUtil.LogEvent(ex.Message, EventLogEntryType.Error);

                Close();
            }
        }

        /// <summary>
        /// It Load The User Data If Rember Me Is Checked.
        /// If No Value Found In The Rigestry It Return - If Any Thing Wrong Happen It Shows The Error And Close.
        /// </summary>
        private void LoginToDVLDappFRM_Load(object sender, EventArgs e)
        {
            try
            {
                _UserInfo = Registry.GetValue(_RegistaryUserKey, _LoginDataSavedVariableName, null) as string;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Getting User (UserName , Password And Activity) From Win/Registery With Exception Message: {ex.Message}"
                    , "Error Getting User Creditations!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                clsUtil.LogEvent(ex.Message, EventLogEntryType.Error);

                this.Close();
            }

            if (string.IsNullOrEmpty(_UserInfo))
                return;

            if (_UserInfo.Length < 3) return;

            if (_UserInfo.Split(',')[2] == "1")
            {
                tbUsername.Text = _UserInfo.Split(',')[0];
                tbPassword.Text = clsUtil.clsSecurity.Decrypt(_UserInfo.Split(',')[1]);
                RememberMeCheckBox.Checked = true;
            }
        }
    }
}
