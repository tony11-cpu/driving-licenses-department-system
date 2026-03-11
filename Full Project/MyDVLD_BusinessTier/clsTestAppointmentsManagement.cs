using MyDVLD_DataTier;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyDVLD_BusinessTier.clsTestTypesManagement;

namespace MyDVLD_BusinessTier
{
    public class clsTestAppointmentsManagement
    {
        public enum enMode : byte { AddNewAppointemnt = 1 , UpdateAppointment = 2}

        private enMode _AppointmentMode = enMode.AddNewAppointemnt;
        public clsTestTypesManagement TestName { get; set; } = clsTestTypesManagement.Find(-1);
        public int? TestAppointmentID { get; private set; } = null;
        public int LocalDrivingLicenseApplicationID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public float PaidFees { get; set; }
        public int CreatedByUserID { get; set; }
        public bool IsAppointmentLocked { get; set; } = false;
        public int RetakeTestApplicationID { get; set; }


        public clsTestAppointmentsManagement()
        {
            _AppointmentMode = enMode.AddNewAppointemnt;

            TestAppointmentID = null;
            LocalDrivingLicenseApplicationID = -1;
            AppointmentDate = DateTime.Now;
            PaidFees = -1;
            CreatedByUserID = -1;
            IsAppointmentLocked = false;
            RetakeTestApplicationID = -1;
            TestName = clsTestTypesManagement.Find(-1);
        }

        private clsTestAppointmentsManagement(int testAppID, int testTypeID , int localDrivingLicenseApplicationID,
                                              DateTime appointmentDate,int paidFees, int createdByUserID, 
                                              bool isAppointmentLocked, int retakeTestApplicationID) 
        {
            _AppointmentMode = enMode.UpdateAppointment;

            TestName = clsTestTypesManagement.Find(testTypeID);
            TestAppointmentID = testAppID;
            LocalDrivingLicenseApplicationID = localDrivingLicenseApplicationID;
            AppointmentDate = appointmentDate;
            PaidFees = paidFees;
            CreatedByUserID = createdByUserID;
            IsAppointmentLocked = isAppointmentLocked;
            RetakeTestApplicationID = retakeTestApplicationID;
        }

        public static DataTable GetAllTestAppointments(int LocalDrivingLicenseApplicationID , enTestMode TestMode)
        {
            string Mode = "";

            switch (TestMode)
            {
                case enTestMode.VisionTest:
                    Mode = "Vision";
                    break;
                case enTestMode.WrittenTest:
                    Mode = "Written";
                    break;
                case enTestMode.PracticalTest:
                    Mode = "Practical";
                    break;
            }

            return clsTestAppointmentsDataTier.GetAllTestsAppointmentsWithLocalDrivingLicenseID(LocalDrivingLicenseApplicationID , Mode);
        }

        public static clsTestAppointmentsManagement Find(int AppointmentID)
        {
            int testTypeID = -1;
            int localDrivingLicenseApplicationID = -1;
            DateTime appointmentDate = DateTime.Now;
            int paidFees = -1;
            int createdByUserID = -1;
            bool isAppointmentLocked = false;
            int retakeTestApplicationID = -1;

            if (clsTestAppointmentsDataTier.GetTestAppointmentWithID(AppointmentID,ref testTypeID, ref localDrivingLicenseApplicationID
                                                                     ,ref appointmentDate , ref paidFees ,  ref createdByUserID , 
                                                                     ref isAppointmentLocked , ref retakeTestApplicationID))
            {
                return new clsTestAppointmentsManagement(AppointmentID , testTypeID , localDrivingLicenseApplicationID , appointmentDate,
                                                             paidFees ,createdByUserID ,isAppointmentLocked ,
                                                             retakeTestApplicationID);
            }
            else
                return new clsTestAppointmentsManagement();
        }

        public static bool IsActiveLocalAppAppointementExists(int LocalDriningLicenseAppID , int TestType) => clsTestAppointmentsDataTier.IsTestAppointmentExistsWithLocalDrivingLicenseApplicationID(LocalDriningLicenseAppID, TestType);

        private bool _AddNewAppointments()
        {
            TestAppointmentID = clsTestAppointmentsDataTier.AddNewTestAppointmentToTheDataBase((int)TestName.TestType , LocalDrivingLicenseApplicationID
                                                                                              , AppointmentDate, PaidFees, CreatedByUserID,
                                                                                                IsAppointmentLocked, RetakeTestApplicationID);

            return TestAppointmentID != null;
        }
        
        private bool _UpdateAppointment() => clsTestAppointmentsDataTier.UpdateTestAppointmentInDB(TestAppointmentID, (int)TestName.TestType, LocalDrivingLicenseApplicationID
                                                                        , AppointmentDate, PaidFees, CreatedByUserID,
                                                                         IsAppointmentLocked, RetakeTestApplicationID);

        public bool Save()
        {
            switch (_AppointmentMode)
            {
                case enMode.AddNewAppointemnt:
                    if (_AddNewAppointments())
                    {
                        _AppointmentMode = enMode.UpdateAppointment;
                        return true;
                    }
                    else
                        return false;
                case enMode.UpdateAppointment:
                    return _UpdateAppointment();
            }

            return false;
        }
    }
}
