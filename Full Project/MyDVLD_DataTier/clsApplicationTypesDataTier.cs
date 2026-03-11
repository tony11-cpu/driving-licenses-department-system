using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyDVLD_DataTier
{
    public class clsApplicationTypesDataTier
    {
        public static DataTable GetAllAppTypes()
        {
            DataTable tb = new DataTable();
            string Query = "Select * From ApplicationTypes Order By ApplicationFees Desc;";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                            tb.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return tb;
        }

        public static bool UpdateApplicationType(int AppID, string Title, float Fees)
        {
            bool IsAppUpdated = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_UpdateApplicationType", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@AppID", AppID);
                        cmd.Parameters.AddWithValue("@Title", Title);
                        cmd.Parameters.AddWithValue("@Fees", Fees);

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

        public static bool GetApplicationByID(int AppID, ref string Title, ref float Fees)
        {
            bool UserGot = false;
            string Query = "Select * From ApplicationTypes Where ApplicationTypeID = @APPID";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, connection))
                    {
                        cmd.Parameters.AddWithValue("@APPID", AppID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                UserGot = true;

                                Title = reader["ApplicationTypeTitle"].ToString();
                                Fees = Convert.ToSingle(reader["ApplicationFees"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                UserGot = false;
            }

            return UserGot;
        }
    }
}
