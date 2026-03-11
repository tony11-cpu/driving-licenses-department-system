using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MyDVLD_DataTier
{
    public class clsTestAppointmentsDataTier
    {
        public static DataTable GetAllTestsAppointmentsWithLocalDrivingLicenseID(int LocalDrivingLicenseID , string TestMode)
        {
            DataTable dt = new DataTable();

            SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString);

            string Query = @"select TestAppointments_View.TestAppointmentID , TestAppointments_View.AppointmentDate , TestAppointments_View.PaidFees , TestAppointments_View.IsLocked From TestAppointments_View   
                             Where TestAppointments_View.LocalDrivingLicenseApplicationID = @LDLAID
                             and TestAppointments_View.TestTypeTitle like @TestMode + '%';";

            using (SqlCommand cmd = new SqlCommand(Query, connection))
            {
                cmd.Parameters.AddWithValue("@LDLAID", LocalDrivingLicenseID);
                cmd.Parameters.AddWithValue("@TestMode", TestMode);

                try
                {
                    connection.Open();

                    using (SqlDataReader Reader = cmd.ExecuteReader())
                        dt.Load(Reader);
                }
                catch (Exception ex)
                {
                    clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                }
                finally { connection.Close(); }
            }
            
            return dt;
        }


        public static bool GetTestAppointmentWithID(int? TestAppointmentID , ref int testTypeID, ref int localDrivingLicenseApplicationID,
                                                    ref DateTime appointmentDate, ref int paidFees, ref int createdByUserID,
                                                    ref bool isAppointmentLocked, ref int retakeTestApplicationID)
        {
            if (TestAppointmentID == null)
                return false;

            bool IsTestFound = false;

            string Query = "Select * From TestAppointments Where TestAppointmentID = @ID;";

            SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString);

            SqlCommand cmd = new SqlCommand(Query , connection);
            cmd.Parameters.AddWithValue("@ID" , TestAppointmentID);

            try
            {
                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    IsTestFound = true;

                    testTypeID = Convert.ToInt32(reader["TestTypeID"]);
                    localDrivingLicenseApplicationID = Convert.ToInt32(reader["LocalDrivingLicenseApplicationID"]);
                    appointmentDate = Convert.ToDateTime(reader["AppointmentDate"]);
                    paidFees = Convert.ToInt32(reader["PaidFees"]);
                    createdByUserID = Convert.ToInt32(reader["CreatedByUserID"]);
                    isAppointmentLocked = Convert.ToBoolean(reader["IsLocked"]);
                    retakeTestApplicationID = (reader["RetakeTestApplicationID"] == null || reader["RetakeTestApplicationID"] == DBNull.Value)? -1 : Convert.ToInt32(reader["RetakeTestApplicationID"]);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                IsTestFound = false;
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            finally { connection.Close(); }

            return IsTestFound;
        }

        public static bool IsTestAppointmentExistsWithLocalDrivingLicenseApplicationID(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            bool IsTestFound = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_IsTestAppointmentExistsByLocalDrivingLicenseApplicationID", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter returnParam = new SqlParameter();
                        returnParam.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(returnParam);

                        cmd.Parameters.AddWithValue("@LocalAppID", LocalDrivingLicenseApplicationID);
                        cmd.Parameters.AddWithValue("@TestTypeID", TestTypeID);

                        cmd.ExecuteNonQuery();

                        IsTestFound = Convert.ToInt32(returnParam.Value) == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return IsTestFound;
        }

        public static int? AddNewTestAppointmentToTheDataBase(int TestTypeID, int LocalDrivingAppID, DateTime AppointmentDate,
                                                       float PaidFees, int CreatedByUserID, bool IsLocked, int RetakeTestAppID)
        {
            int? NewTestAppointmentID = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_AddNewTestAppointment", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                        cmd.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingAppID);
                        cmd.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);
                        cmd.Parameters.AddWithValue("@PaidFees", PaidFees);
                        cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        cmd.Parameters.AddWithValue("@IsLocked", IsLocked);
                        cmd.Parameters.AddWithValue("@RetakeTestApplicationID",
                            (RetakeTestAppID <= 0) ? (object)DBNull.Value : RetakeTestAppID);

                        SqlParameter outParam = new SqlParameter("@NewTestAppointmentID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outParam);

                        cmd.ExecuteNonQuery();

                        if (outParam.Value != DBNull.Value && (int)outParam.Value > 0)
                            NewTestAppointmentID = (int)outParam.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return NewTestAppointmentID;
        }


        public static bool UpdateTestAppointmentInDB(int? TestAppointmentID, int TestTypeID, int LocalDrivingAppID, DateTime AppointmentDate,
                                              float PaidFees, int CreatedByUserID, bool IsLocked, int RetakeTestAppID)
        {
            if (TestAppointmentID == null)
                return false;

            bool IsUpdated = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_UpdateTestAppointment", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID.Value);
                        cmd.Parameters.AddWithValue("@TestTypeID", TestTypeID);
                        cmd.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingAppID);
                        cmd.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);
                        cmd.Parameters.AddWithValue("@PaidFees", PaidFees);
                        cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        cmd.Parameters.AddWithValue("@IsLocked", IsLocked);
                        cmd.Parameters.AddWithValue("@RetakeTestApplicationID", (RetakeTestAppID <= 0) ? (object)DBNull.Value : RetakeTestAppID);

                        SqlParameter outParam = new SqlParameter("@IsUpdated", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outParam);

                        cmd.ExecuteNonQuery();

                        IsUpdated = outParam.Value != DBNull.Value && Convert.ToBoolean(outParam.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return IsUpdated;
        }
    }
}
