using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDVLD_DataTier
{
    public class clsApplicationsDataTier
    {
        public static bool IsApplicationExists(int AppID)
        {
            bool IsAppExist = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    Connection.Open();

                    string Query = @"SELECT Found = 1 FROM Applications WHERE ApplicationID = @AppID;";

                    using (SqlCommand cmd = new SqlCommand(Query, Connection))
                    {
                        cmd.Parameters.AddWithValue("@AppID", AppID);

                        IsAppExist = (Convert.ToInt32(cmd.ExecuteScalar()) > 0);
                    }
                }
            }
            catch (Exception ex) 
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                IsAppExist = false;
            }

            return IsAppExist;
        }

        public static bool GetApplicationDataByAppID(int ApplicationID, ref int ApplicantPersonID, ref DateTime ApplicationDate,
                                                 ref int ApplicationTypeID, ref short ApplicationStatus, ref DateTime LastStatusDate,
                                                 ref float PaidFees, ref int CreatedByUserID)
        {
            bool IsAppFound = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    string Query = @"Select * From GetDetailedApplicationDataByApplicationID(@AppID)";

                    using (SqlCommand cmd = new SqlCommand(Query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AppID", ApplicationID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ApplicantPersonID = Convert.ToInt32(reader["ApplicantPersonID"]);
                                ApplicationDate = Convert.ToDateTime(reader["ApplicationDate"]);
                                ApplicationTypeID = Convert.ToInt32(reader["ApplicationTypeID"]);
                                ApplicationStatus = Convert.ToInt16(reader["ApplicationStatus"]);
                                LastStatusDate = Convert.ToDateTime(reader["LastStatusDate"]);
                                PaidFees = Convert.ToSingle(reader["PaidFees"]);
                                CreatedByUserID = Convert.ToInt32(reader["CreatedByUserID"]);

                                IsAppFound = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsAppFound = false;
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return IsAppFound;
        }

        public static int AddNewApplicationToDB(int ApplicantPersonID, DateTime ApplicationDate, int ApplicationTypeID,
                                         short ApplicationStatus, DateTime LastStatusDate, float PaidFees, int CreatedByUserID)
        {
            int NewAppID = -1;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_AddNewApplication", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                        cmd.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                        cmd.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                        cmd.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                        cmd.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
                        cmd.Parameters.AddWithValue("@PaidFees", PaidFees);
                        cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        SqlParameter OutParam = new SqlParameter("@NewAppID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        cmd.Parameters.Add(OutParam);
                        cmd.ExecuteNonQuery();

                        if (int.TryParse(OutParam.Value.ToString(), out int NewID) && NewID > 0)
                            NewAppID = NewID;
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return NewAppID;
        }

        public static bool UpdateApplicationInDB(int ApplicationID, int ApplicantPersonID, DateTime ApplicationDate,
                                           int ApplicationTypeID, short ApplicationStatus, DateTime LastStatusDate,
                                           float PaidFees, int CreatedByUserID)
        {
            bool IsAppUpdated = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_UpdateApplication", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AppID", ApplicationID);
                        cmd.Parameters.AddWithValue("@NewPersonID", ApplicantPersonID);
                        cmd.Parameters.AddWithValue("@NewApplicationDate", ApplicationDate);
                        cmd.Parameters.AddWithValue("@NewApplicationTypeID", ApplicationTypeID);
                        cmd.Parameters.AddWithValue("@NewStatus", ApplicationStatus);
                        cmd.Parameters.AddWithValue("@NewLastStatusDate", LastStatusDate);
                        cmd.Parameters.AddWithValue("@NewPaidFees", PaidFees);
                        cmd.Parameters.AddWithValue("@NewCreatedByUserID", CreatedByUserID);

                        IsAppUpdated = cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                IsAppUpdated = false;
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return IsAppUpdated;
        }

        public static bool DeleteApplicationFormDB(int ApplicationID)
        {
            bool IsAppDeleted = false;
            string Query = @"DELETE FROM Applications
                             WHERE Applications.ApplicationID = @AppID;";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, connection))
                    {
                        cmd.Parameters.AddWithValue("@AppID", ApplicationID);

                        IsAppDeleted = (cmd.ExecuteNonQuery() > 0);
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return IsAppDeleted;
        }
    }
}
