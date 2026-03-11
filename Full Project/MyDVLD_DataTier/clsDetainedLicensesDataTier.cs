using MyDVLD_DataTier;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MyDVLD_BusinessTier
{
    public class clsDetainedLicensesDataTier
    {
        public static bool IsLicenseDetainedWithID(int License)
        {
            bool isDetained = false;
            string query = @"Select IsDetained = 1 From DetainedLicenses Where DetainedLicenses.LicenseID = @LicenseID and IsReleased = 0;";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@LicenseID", License);

                        using (SqlDataReader reader = command.ExecuteReader())
                            if (reader.Read())
                                isDetained = Convert.ToBoolean(reader["IsDetained"]);
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return isDetained;
        }

        public static int? AddNewDeatinedLicenseToDB(int LicenseID, DateTime DetainDate, float FineFees , int CreatedByUserID, 
                          bool IsReleased , DateTime? ReleaseDate = null , int ReleasedByUserID = -1 , int ReleaseAppID = -1)
        {
            int? NewDetainedLicenseID = null;
            string query = @"Insert Into DetainedLicenses (LicenseID, DetainDate, FineFees , CreatedByUserID , IsReleased , ReleaseDate, ReleasedByUserID , ReleaseApplicationID)
                             Values (@LicenseID, @DetainDate, @FineFees , @CreatedByUserID , @IsReleased , @ReleaseDate, @ReleasedByUserID , @ReleaseAppID);
                             SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@LicenseID", LicenseID);
                command.Parameters.AddWithValue("@DetainDate", DetainDate);
                command.Parameters.AddWithValue("@FineFees", FineFees);
                command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                command.Parameters.AddWithValue("@IsReleased", IsReleased);
                command.Parameters.AddWithValue("@ReleaseAppID", ReleaseAppID == -1? (object)DBNull.Value : ReleaseAppID);
                command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID == -1 ? (object)DBNull.Value : ReleasedByUserID);
                command.Parameters.AddWithValue("@ReleaseDate", !ReleaseDate.HasValue? (object)DBNull.Value : ReleaseDate);

                try
                {
                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int NewID))
                        NewDetainedLicenseID = NewID;
                }
                catch (Exception ex)
                {
                    clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                }
            }

            return NewDetainedLicenseID;
        }


        public static bool UpdateDetainedLicense(int DetainLiceneID, DateTime DetainDate, float FineFees, int CreatedByUserID,
           bool IsReleased, DateTime ReleaseDate, int ReleasedByUserID, int ReleaseAppID)
        {
            bool updated = false;

            try
            {
                using (SqlConnection con = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_UpdateDetainedLicense", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@DetainLicenseID", DetainLiceneID);
                        cmd.Parameters.AddWithValue("@DetainDate", DetainDate);
                        cmd.Parameters.AddWithValue("@FineFees", FineFees);
                        cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        cmd.Parameters.AddWithValue("@IsReleased", IsReleased);
                        cmd.Parameters.AddWithValue("@ReleaseAppID", ReleaseAppID == -1 ? (object)DBNull.Value : ReleaseAppID);
                        cmd.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID == -1 ? (object)DBNull.Value : ReleasedByUserID);
                        cmd.Parameters.AddWithValue("@ReleaseDate", ReleaseDate == default(DateTime) ? (object)DBNull.Value : ReleaseDate);

                        updated = cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                updated = false;
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return updated;
        }

        public static DataTable GetAllDetainedLicenses()
        {
            DataTable detainedLicensesTable = new DataTable();
            string query = @"select * from detainedLicenses_View order by IsReleased ,DetainID;";

            using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader()) 
                       detainedLicensesTable.Load(reader);
                }
                catch (Exception ex)
                {
                    clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                }
            }

            return detainedLicensesTable;
        }

        public static bool GetDetainedLicenseInfo(int DetainedLicenseID, ref int LicenseID, ref DateTime DetainDate,
                                                  ref float FineFees, ref int CreatedByUserID, ref bool IsReleased,
                                                  ref DateTime ReleaseDate, ref int ReleasedByUserID, ref int ReleaseApplicationID)
        {
            bool found = false;
            string query = @"SELECT * FROM DetainedLicenses WHERE DetainID = @DetainedLicenseID;";

            using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@DetainedLicenseID", DetainedLicenseID);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            found = true;

                            LicenseID = Convert.ToInt32(reader["LicenseID"]);
                            DetainDate = Convert.ToDateTime(reader["DetainDate"]);
                            FineFees = Convert.ToSingle(reader["FineFees"]);
                            CreatedByUserID = Convert.ToInt32(reader["CreatedByUserID"]);
                            IsReleased = Convert.ToBoolean(reader["IsReleased"]);

                            ReleaseDate = reader["ReleaseDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["ReleaseDate"]);
                            ReleasedByUserID = reader["ReleasedByUserID"] == DBNull.Value ? -1 : Convert.ToInt32(reader["ReleasedByUserID"]);
                            ReleaseApplicationID = reader["ReleaseApplicationID"] == DBNull.Value ? -1 : Convert.ToInt32(reader["ReleaseApplicationID"]);
                        }
                    }
                }
                catch (Exception ex)
                { 
                    found = false;
                    clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                }
            }

            return found;
        }

        public static bool GetDetainedLicenseInfoByLicenseID(int LicenseID, ref int DetainedLicenseID, ref DateTime DetainDate,
            ref float FineFees, ref int CreatedByUserID, ref bool IsReleased, ref DateTime ReleaseDate,
            ref int ReleasedByUserID, ref int ReleaseApplicationID)
        {
            bool found = false;

            string query = @"SELECT * FROM DetainedLicenses WHERE LicenseID = @LicenseID
                             Order By DetainID Desc;";

            using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@LicenseID", LicenseID);

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            found = true;

                            DetainedLicenseID = Convert.ToInt32(reader["DetainID"]);
                            DetainDate = Convert.ToDateTime(reader["DetainDate"]);
                            FineFees = Convert.ToSingle(reader["FineFees"]);
                            CreatedByUserID = Convert.ToInt32(reader["CreatedByUserID"]);
                            IsReleased = Convert.ToBoolean(reader["IsReleased"]);

                            ReleaseDate = reader["ReleaseDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["ReleaseDate"]);
                            ReleasedByUserID = reader["ReleasedByUserID"] == DBNull.Value ? -1 : Convert.ToInt32(reader["ReleasedByUserID"]);
                            ReleaseApplicationID = reader["ReleaseApplicationID"] == DBNull.Value ? -1 : Convert.ToInt32(reader["ReleaseApplicationID"]);
                        }

                    }
                }
                catch (Exception ex)
                {
                    found = false;
                    clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                }
            }

            return found;
        }
    }
}
