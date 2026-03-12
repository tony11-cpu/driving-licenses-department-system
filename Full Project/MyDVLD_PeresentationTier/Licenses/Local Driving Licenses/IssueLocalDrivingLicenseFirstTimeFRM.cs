using MyDVLD_BusinessTier;
using MyDVLD_PeresentationTier.Applications.Application_User_CTRL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MyDVLD_BusinessTier.clsLicenseManagement;

namespace MyDVLD_PeresentationTier.Licenses
{
    public partial class IssueLocalDrivingLicenseFirstTimeFRM : Form
    {
        private int _LocalAppID = -1;

        public IssueLocalDrivingLicenseFirstTimeFRM(int AppID)
        {
            InitializeComponent();

            _LocalAppID = AppID;
        }

        private void btnCloseFRM_Click(object sender, EventArgs e) => this.Close();

        private bool _CreateAndSaveNewLicense()
        {
            _NewLicense.Driver = _NewDriver;

            _NewLicense.Notes = tbNotes.Text;
            _NewLicense.LicenseClass = ctrlShowLocalApplicationDetails1.LocalInfoCard.LicenseClass;
            _NewLicense.ApplicationID = ctrlShowLocalApplicationDetails1.LocalInfoCard.ApplicationID;
            _NewLicense.CreatedByUserID = ctrlShowLocalApplicationDetails1.LocalInfoCard.CreatedByUser.UserID;
            _NewLicense.ExpirationDate = DateTime.Now.AddYears(ctrlShowLocalApplicationDetails1.LocalInfoCard.LicenseClass.DefaultValidityLength);
            _NewLicense.IsActive = true;
            _NewLicense.IssueReason = (short)enIssueReason.FirstTime;
            _NewLicense.PaidFees = clsApplicationTypeManagement.Find((short)enIssueReason.FirstTime).ApplicationFees;

            return _NewLicense.Save();
        }

        private clsLicenseManagement _NewLicense = clsLicenseManagement.Find(-1);
        private clsDriversManagement _NewDriver = clsDriversManagement.Find(-1);

        [Conditional("DEBUG")]
        private void _LogDriverInfoWhileDebugging()
        {
            MessageBox.Show($"New Driver Added!\nDriver ID: {_NewDriver.DriverID}.\nDriver Name: {_NewDriver.Person.FullName}." +
                $"\nDriver National No.: {_NewDriver.Person.NationalNo}."
                , "New Driver Added To System..!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void _AddNewDriver()
        {
            if (!clsDriversManagement.IsDriverExists(ctrlShowLocalApplicationDetails1.LocalInfoCard.ApplicantPerson.PersonID))
            {
                _NewDriver.Person = ctrlShowLocalApplicationDetails1.LocalInfoCard.ApplicantPerson;
                _NewDriver.CreatedByUserID = ctrlShowLocalApplicationDetails1.LocalInfoCard.CreatedByUser.UserID;

                _NewDriver.Save();
            }
            else
                _NewDriver = clsDriversManagement.FindByPersonID(ctrlShowLocalApplicationDetails1.LocalInfoCard.ApplicantPerson.PersonID);
        }

        private void _CreateNewLicenese()
        {
            if (_CreateAndSaveNewLicense())
            {
                if (ctrlShowLocalApplicationDetails1.LocalInfoCard.CompleteApplication())
                {
                    MessageBox.Show($"License Added Successfully, With Id: {_NewLicense.LicenseID}.", "License Added Successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                    MessageBox.Show("Error While Issuing New Driving License!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("Error While Issuing New Driving License!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _AddNewDriver();
            _CreateNewLicenese();
        }

        private void IssueLocalDrivingLicenseFirstTimeFRM_Load(object sender, EventArgs e)
        {
            if(!clsLocalLicesnseApplicationManagement.IsLocalApplicationExists(_LocalAppID))
            {
                MessageBox.Show("Application Not Found , New License Can't Be Issued!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = false;
                return;
            }

            ctrlShowLocalApplicationDetails1.LoadLocalAppDetails(_LocalAppID);
        }
    }
}
