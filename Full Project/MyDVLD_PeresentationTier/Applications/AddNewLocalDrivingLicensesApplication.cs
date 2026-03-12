using MyDVLD_BusinessTier;
using MyDVLD_PeresentationTier.People_FRMs.People_Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyDVLD_PeresentationTier.Applications
{
    public partial class AddOrUpdateLocalDrivingLicensesApplication : Form
    {
        private clsLocalLicesnseApplicationManagement _NewLocalLicenseApp = clsLocalLicesnseApplicationManagement.Find(-1);
        private int _PersonID = -1;
        private int _LicenseClassID = -1;

        public AddOrUpdateLocalDrivingLicensesApplication(int LDLAid = -1)
        {
            InitializeComponent();

            _NewLocalLicenseApp = clsLocalLicesnseApplicationManagement.Find(LDLAid);
        }

        private void btnCloseFRM_Click_1(object sender, EventArgs e) => this.Close();

        private void _RefreshFRM()
        {
            tabControl1.SelectedIndex = 0;

            ctrlShowPersonDetailsWithSearch1.ResetFRM();

            ctrlShowPersonDetailsWithSearch1.FilterEnabled = true;
            btnResetApplicationFRM.Enabled = false;
            btnSaveApplication.Enabled = false;
            AppInputesPanel.Enabled = false;
            lblApplicationCreateDate.Text = DateTime.Now.ToShortDateString();
            lblUserCreateApplication.Text = clsUtil.SignedInUser.Username;

            lblDLAppID.Text = "[???]";
            cbLicensesClasses.SelectedIndex = 2;
            lblAddOrUpdateLDLA.Text = "New Local Diving Licenses Application";

            _LicenseClassID = -1;
            _NewLocalLicenseApp = clsLocalLicesnseApplicationManagement.Find(-1);
        }

        private void _FillUpdateInputeData()
        {
            cbLicensesClasses.SelectedIndex = _NewLocalLicenseApp.LicenseClass.ClassID - 1;
            lblApplicationCreateDate.Text = _NewLocalLicenseApp.ApplicationDate.ToString();
            lblUserCreateApplication.Text = _NewLocalLicenseApp.CreatedByUser.Username;
            _PersonID = _NewLocalLicenseApp.ApplicantPerson.PersonID;
            AppInputesPanel.Enabled = true;
            ctrlShowPersonDetailsWithSearch1.LoadPersonInfo(_PersonID);
        }

        private void _FillAllClass()
        {
            foreach (string LicenseClass in clsLicensesClassesManagement.AllALicencesClasses())
                cbLicensesClasses.Items.Add(LicenseClass);
        }

        private void AddNewLocalDrivingLicensesApplication_Load(object sender, EventArgs e)
        {
            _FillAllClass();

            lblApplicationFees.Text = clsApplicationTypeManagement.ApplicationFeesString(clsApplicationTypeManagement.enApplicationService.NewLocalDrivingLicense);

            if (_NewLocalLicenseApp.LocalApplicationID != -1)
            {
                _FillUpdateInputeData();
                _UpdateFRMLoad();

                return;
            }

            _RefreshFRM();
        }

        private void _UpdateFRMLoad()
        {
            lblAddOrUpdateLDLA.Text = "Update Local Driving Licenses Application";   
            lblDLAppID.Text = _NewLocalLicenseApp.LocalApplicationID.ToString();
            btnResetApplicationFRM.Enabled = true;
            ctrlShowPersonDetailsWithSearch1.FilterEnabled = false;
            btnSaveApplication.Enabled = true;
        }

        private bool _CheckForPersonAge()
        {
            clsPeopleManagement person = clsPeopleManagement.Find(_PersonID);

            int age = DateTime.Today.Year - person.DateOfBirth.Year;
            if (DateTime.Today < person.DateOfBirth.AddYears(age))
                age--;

            return age >= clsLicensesClassesManagement.Find(cbLicensesClasses.SelectedIndex + 1).MinimumAllowedAge;
        }

        private void btnNextFRM_Click(object sender, EventArgs e)
        {
            _PersonID = ctrlShowPersonDetailsWithSearch1.PersonId;

            if (_PersonID == -1)
            {
                MessageBox.Show("Please Make Sure To Connect The Application With A Person..!", "No Person Selected!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlShowPersonDetailsWithSearch1.FillterFocus();

                return;
            }

            AppInputesPanel.Enabled = true;
            btnSaveApplication.Enabled = true;
            tabControl1.SelectedIndex = 1;
        }

        private void _FillApplicationData()
        {                                            
            _NewLocalLicenseApp.ApplicationDate = DateTime.Now;
            _NewLocalLicenseApp.ApplicationStatus = clsApplicationsManagement.enApplicationStatus.New;
            _NewLocalLicenseApp.ApplicantPerson = clsPeopleManagement.Find(_PersonID);
            _NewLocalLicenseApp.ApplicationType = clsApplicationTypeManagement.Find(1);
            _NewLocalLicenseApp.PaidFees = Convert.ToSingle(lblApplicationFees.Text);
            _NewLocalLicenseApp.CreatedByUser = clsUtil.SignedInUser;
            _NewLocalLicenseApp.LicenseClass = clsLicensesClassesManagement.Find(cbLicensesClasses.SelectedIndex + 1);
        }

        private bool _ValidatePerson()
        {
            if (!_CheckForPersonAge())
            {
                MessageBox.Show("Person Selected Does Not Meet The Minimum Age Requirement For The Selected License Class!", "Age Requirement Not Met!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            _LicenseClassID = cbLicensesClasses.SelectedIndex + 1;

            if (clsLicenseManagement.Find(clsDriversManagement.FindByPersonID(_PersonID).DriverID ?? -1, _LicenseClassID).IsActive)
            {
                MessageBox.Show("Person Selected Already Has An Active License With The Selected Class!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (clsLocalLicesnseApplicationManagement.IsPersonAppliedForLicenseClass(_PersonID, _LicenseClassID))
            {
                MessageBox.Show("Person Selected Already Has Applications With The Same License Class And The App Is Still Acitve!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void btnSaveApplication_Click(object sender, EventArgs e)
        {
            if (!_ValidatePerson())
                return;

            _FillApplicationData();

            if(_NewLocalLicenseApp.Save())
            {
                _UpdateFRMLoad();
                MessageBox.Show("Local Driving License Applications Saved Successfully To The Sysem.", "Saved Successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("Error While Saving New Local Driving License Applications To The Sysem.", "Failed To Save.", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnResetApplicationFRM_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are You Sure You Want To Reset The Form And Start Again?" , "Reset The Form" ,
                MessageBoxButtons.YesNo , MessageBoxIcon.Question) == DialogResult.Yes)
                _RefreshFRM();
        }

        private void AddOrUpdateLocalDrivingLicensesApplication_Activated(object sender, EventArgs e) => ctrlShowPersonDetailsWithSearch1.FillterFocus();
    }
}
