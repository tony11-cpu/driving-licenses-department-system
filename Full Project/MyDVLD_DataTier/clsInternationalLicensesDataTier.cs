using MyDVLD_DataTier;
using System;
using System.Data;
using System.Data.SqlClient;

namespace MyDVLD_DataAccess
{
    public class clsInternationalLicensesData
    {
        public static bool GetInternationalLicenseByID(int InternationalLicenseID,ref int ApplicationID,ref int DriverID,
                                                       ref int IssuedUsingLocalLicenseID,ref DateTime IssueDate
                                                      ,ref DateTime ExpirationDate,ref bool IsActive,ref int CreatedByUserID)
        {
            bool isFound = false;
            string query = @"SELECT * FROM InternationalLicenses WHERE InternationalLicenseID = @InternationalLicenseID;";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isFound = true;
                                ApplicationID = (int)reader["ApplicationID"];
                                DriverID = (int)reader["DriverID"];
                                IssuedUsingLocalLicenseID = (int)reader["IssuedUsingLocalLicenseID"];
                                IssueDate = (DateTime)reader["IssueDate"];
                                ExpirationDate = (DateTime)reader["ExpirationDate"];
                                IsActive = (bool)reader["IsActive"];
                                CreatedByUserID = (int)reader["CreatedByUserID"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isFound = false;
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return isFound;
        }

        public static bool GetInternationalLicensesByDriverID(int DriverID ,ref int InternationalLicenseID, ref int ApplicationID, ref int IssuedUsingLocalLicenseID,
            ref DateTime IssueDate, ref DateTime ExpirationDate, ref bool IsActive, ref int CreatedByUserID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString);

            string query = @"SELECT * FROM InternationalLicenses WHERE DriverID = @DriverID;";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DriverID", DriverID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isFound = true;

                    InternationalLicenseID = (int)reader["InternationalLicenseID"];
                    ApplicationID = (int)reader["ApplicationID"];
                    IssuedUsingLocalLicenseID = (int)reader["IssuedUsingLocalLicenseID"];
                    IssueDate = (DateTime)reader["IssueDate"];
                    ExpirationDate = (DateTime)reader["ExpirationDate"];
                    IsActive = (bool)reader["IsActive"];
                    CreatedByUserID = (int)reader["CreatedByUserID"];
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                isFound = false;
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            finally
            {
                connection.Close();
            }
            return isFound;
        }



        public static int? AddNewInternationalLicense(int ApplicationID, int DriverID, int IssuedUsingLocalLicenseID,
                                              bool IsActive, int CreatedByUserID)
        {
            int? NewID = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_AddNewInternationalLicense", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        command.Parameters.AddWithValue("@DriverID", DriverID);
                        command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
                        command.Parameters.AddWithValue("@IsActive", IsActive);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        SqlParameter outParam = new SqlParameter("@NewInternationalLicenseID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outParam);

                        command.ExecuteNonQuery();

                        if (outParam.Value != DBNull.Value && (int)outParam.Value > 0)
                            NewID = (int)outParam.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return NewID;
        }

        public static bool UpdateInternationalLicense(int InternationalLicenseID, int ApplicationID, int DriverID,
            int IssuedUsingLocalLicenseID, DateTime IssueDate, DateTime ExpirationDate, bool IsActive, int CreatedByUserID)
        {
            bool IsUpdated = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_UpdateInternationalLicense", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);
                        command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        command.Parameters.AddWithValue("@DriverID", DriverID);
                        command.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", IssuedUsingLocalLicenseID);
                        command.Parameters.AddWithValue("@IssueDate", IssueDate);
                        command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
                        command.Parameters.AddWithValue("@IsActive", IsActive);
                        command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        SqlParameter outParam = new SqlParameter("@IsUpdated", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outParam);

                        command.ExecuteNonQuery();

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

        public static bool DeleteInternationalLicense(int InternationalLicenseID)
        {
            bool IsDeleted = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_DeleteInternationalLicense", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);

                        SqlParameter outParam = new SqlParameter("@IsDeleted", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outParam);

                        command.ExecuteNonQuery();

                        IsDeleted = outParam.Value != DBNull.Value && Convert.ToBoolean(outParam.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return IsDeleted;
        }

        public static DataTable GetAllInternationalLicenses()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_GetAllInternationalLicenses", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                                dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                dt = new DataTable();
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return dt;
        }

        public static bool IsDriverHasActiveInternationalLicense(int DriverID)
        {
            bool IsActive = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("SP_IsDriverHasActiveInternationalLicense", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@DriverID", DriverID);

                        SqlParameter outParam = new SqlParameter("@IsActive", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(outParam);

                        command.ExecuteNonQuery();

                        IsActive = outParam.Value != DBNull.Value && Convert.ToBoolean(outParam.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return IsActive;
        }
    }
}
