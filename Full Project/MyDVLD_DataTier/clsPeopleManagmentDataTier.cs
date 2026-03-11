using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace MyDVLD_DataTier
{
    public class clsPeopleManagmentDataTier
    {
        public static bool IsPersonExistInDB_ById(int PersonId)
        {
            bool IsPersonExist = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    Connection.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_IsPersonExists", Connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter ReturnIsFound = new SqlParameter();
                        ReturnIsFound.Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add(ReturnIsFound);

                        cmd.Parameters.AddWithValue("@PersonId", PersonId);

                        cmd.ExecuteNonQuery();

                        IsPersonExist = Convert.ToInt32(ReturnIsFound.Value) == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return IsPersonExist;
        }

        public static int AddPersonToDataBase(string NationalNo, string FirstName, string SecondName,
                                               string ThirdName, string LastName, DateTime DateOfBirth,
                                               byte Gender, string Address, string Phone, string Email,
                                               int NationalityCountryId, string ImagePath)
        {
            int PersonAddedId = -1;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    Connection.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_AddNewPerson", Connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@NationalNo", NationalNo);
                        cmd.Parameters.AddWithValue("@FirstName", FirstName);
                        cmd.Parameters.AddWithValue("@SecondName", SecondName);
                        cmd.Parameters.AddWithValue("@LastName", LastName);
                        cmd.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                        cmd.Parameters.AddWithValue("@Gender", Gender);
                        cmd.Parameters.AddWithValue("@Address", Address);
                        cmd.Parameters.AddWithValue("@Phone", Phone);
                        cmd.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryId);
                        cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(ImagePath) ? (object)DBNull.Value : ImagePath);
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? (object)DBNull.Value : Email);
                        cmd.Parameters.AddWithValue("@ThirdName", string.IsNullOrEmpty(ThirdName) ? (object)DBNull.Value : ThirdName);

                        SqlParameter OutParm = new SqlParameter("@NewPersonID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        cmd.Parameters.Add(OutParm);
                        cmd.ExecuteNonQuery();

                        if (OutParm.Value != DBNull.Value && (int)OutParm.Value > 0)
                            PersonAddedId = (int)OutParm.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return PersonAddedId;
        }

        public static bool GetPersonInfoById(int PersonId, ref string NationalNo, ref string FirstName, ref string SecondName,
                                         ref string ThirdName, ref string LastName, ref DateTime DateOfBirth,
                                         ref string Gender, ref string Address, ref string Phone, ref string Email,
                                         ref int NationalityCountryId, ref string ImagePath)
        {
            bool PersonGotInfo = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    Connection.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_GetPersonInfo", Connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@PersonID", PersonId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                NationalNo = reader["NationalNo"].ToString();
                                FirstName = reader["FirstName"].ToString();
                                SecondName = reader["SecondName"].ToString();
                                ThirdName = (reader["ThirdName"] == DBNull.Value) ? null : reader["ThirdName"].ToString();
                                LastName = reader["LastName"].ToString();
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]);
                                Gender = (Convert.ToInt32(reader["Gender"]) == 0) ? "Male" : "Female";
                                Address = reader["Address"].ToString();
                                Phone = reader["Phone"].ToString();
                                NationalityCountryId = Convert.ToInt32(reader["NationalityCountryId"]);
                                Email = (reader["Email"] == DBNull.Value) ? string.Empty : reader["Email"].ToString();
                                ImagePath = (reader["ImagePath"] == DBNull.Value) ? string.Empty : reader["ImagePath"].ToString();

                                PersonGotInfo = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PersonGotInfo = false;
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return PersonGotInfo;
        }

        public static bool GetPersonInfoByNastionalNumber(string NationalNumber, ref int PersonId, ref string FirstName, ref string SecondName,
                                           ref string ThirdName, ref string LastName, ref DateTime DateOfBirth,
                                           ref string Gender, ref string Address, ref string Phone, ref string Email,
                                           ref int NationalityCountryId, ref string ImagePath)
        {
            bool PersonGotInfo = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    Connection.Open();

                    using(SqlCommand cmd = new SqlCommand("SP_GetPersonInfo_ByNationalNo", Connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@NationalNo", NationalNumber);

                        using(SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                PersonId = (int)reader["PersonId"];
                                FirstName = reader["FirstName"].ToString();
                                SecondName = reader["SecondName"].ToString();
                                ThirdName = (reader["ThirdName"] == DBNull.Value) ? null : reader["ThirdName"].ToString();
                                LastName = reader["LastName"].ToString();
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]);
                                Gender = (Convert.ToInt32(reader["Gender"]) == 0) ? "Male" : "Female";
                                Address = reader["Address"].ToString();
                                Phone = reader["Phone"].ToString();
                                NationalityCountryId = Convert.ToInt32(reader["NationalityCountryId"]);
                                Email = (reader["Email"] == DBNull.Value) ? null : reader["Email"].ToString();
                                ImagePath = (reader["ImagePath"] == DBNull.Value) ? null : reader["ImagePath"].ToString();

                                PersonGotInfo = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PersonGotInfo = false;
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return PersonGotInfo;
        }


        public static bool IsPersonExistInDB_ByNationalNumber(string NationalNo)
        {
            bool IsPersonExist = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    Connection.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_IsPersonExists_ByNotionalNumber", Connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter returnParameter = new SqlParameter();
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        cmd.Parameters.Add(returnParameter);
                        cmd.Parameters.AddWithValue("@NationalNumber", NationalNo);

                        cmd.ExecuteNonQuery();

                        IsPersonExist = Convert.ToInt32(returnParameter.Value) == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                IsPersonExist = false;
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return IsPersonExist;
        }

        public static bool DeletePersonByIdFromDatabase(int PersonId)
        {
            bool IsPersonDeleted = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    Connection.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_DeletePerson", Connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter IsDeletedOut = new SqlParameter("@IsDeleted", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };

                        cmd.Parameters.AddWithValue("@PersonID", PersonId);
                        cmd.Parameters.Add(IsDeletedOut);

                        cmd.ExecuteNonQuery();

                        IsPersonDeleted = Convert.ToBoolean(IsDeletedOut.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                IsPersonDeleted = false;
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }

            return IsPersonDeleted;
        }

        public static bool UpdatePersonWithIdInDataBase(int PersonId, string NationalNo, string FirstName, string SecondName,
                                             string ThirdName, string LastName, DateTime DateOfBirth,
                                             byte Gender, string Address, string Phone, string Email,
                                             int NationalityCountryId, string ImagePath)
        {
            bool IsPersonUpdated = false;

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    Connection.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_UpdatePerson", Connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@PersonID", PersonId);
                        cmd.Parameters.AddWithValue("@NationalNo", NationalNo);
                        cmd.Parameters.AddWithValue("@FirstName", FirstName);
                        cmd.Parameters.AddWithValue("@SecondName", SecondName);
                        cmd.Parameters.AddWithValue("@LastName", LastName);
                        cmd.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                        cmd.Parameters.AddWithValue("@Gender", Gender);
                        cmd.Parameters.AddWithValue("@Address", Address);
                        cmd.Parameters.AddWithValue("@Phone", Phone);
                        cmd.Parameters.AddWithValue("@NationalityCountryId", NationalityCountryId);
                        cmd.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(ImagePath) ? (object)DBNull.Value : ImagePath);
                        cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(Email) ? (object)DBNull.Value : Email);
                        cmd.Parameters.AddWithValue("@ThirdName", string.IsNullOrEmpty(ThirdName) ? (object)DBNull.Value : ThirdName);

                        SqlParameter OutParm = new SqlParameter("@IsPersonUpdated", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };

                        cmd.Parameters.Add(OutParm);

                        cmd.ExecuteNonQuery();

                        IsPersonUpdated = OutParm.Value == null ? false : Convert.ToBoolean(OutParm.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return false;
            }

            return IsPersonUpdated;
        }


        [Obsolete("It Will Be Automatic From The DataGridVeiw Soon , Cause This Method Is Vunrable!")]
        public static DataTable GetPeopleFilteredFromDB(string FilterColumn, string FilterValue)
        {
            DataTable PeopleTable = new DataTable();

            string Query = $@"SELECT People.PersonID, People.NationalNo, People.FirstName, People.SecondName, People.ThirdName, People.LastName,
                              People.DateOfBirth, People.Gender, People.Address, People.Phone, People.Email,
                              Countries.CountryName, People.ImagePath
                              FROM People INNER JOIN Countries
                              ON People.NationalityCountryID = Countries.CountryID 
                              WHERE {FilterColumn} LIKE @FilterValue + '%';";

            try
            {
                using (SqlConnection Connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    Connection.Open();

                    using (SqlCommand cmd = new SqlCommand(Query, Connection))
                    {
                        cmd.Parameters.AddWithValue("@FilterValue", FilterValue);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            PeopleTable.Columns.Add("PersonID", typeof(int));
                            PeopleTable.Columns.Add("NationalNo", typeof(string));
                            PeopleTable.Columns.Add("FirstName", typeof(string));
                            PeopleTable.Columns.Add("SecondName", typeof(string));
                            PeopleTable.Columns.Add("ThirdName", typeof(string));
                            PeopleTable.Columns.Add("LastName", typeof(string));
                            PeopleTable.Columns.Add("DateOfBirth", typeof(DateTime));
                            PeopleTable.Columns.Add("Gender", typeof(string));
                            PeopleTable.Columns.Add("Address", typeof(string));
                            PeopleTable.Columns.Add("Phone", typeof(string));
                            PeopleTable.Columns.Add("Email", typeof(string));
                            PeopleTable.Columns.Add("CountryName", typeof(string));
                            PeopleTable.Columns.Add("ImagePath", typeof(string));

                            string genderString = "";

                            while (reader.Read())
                            {
                                if (Convert.ToInt32(reader["Gender"]) == 0)
                                    genderString = "Male";
                                else
                                    genderString = "Female";

                                PeopleTable.Rows.Add(reader["PersonID"], reader["NationalNo"], reader["FirstName"], reader["SecondName"],
                                                    reader["ThirdName"], reader["LastName"], reader["DateOfBirth"], genderString,
                                                    reader["Address"], reader["Phone"], reader["Email"], reader["CountryName"], reader["ImagePath"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsDB_Util.clsEventLog.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
            return PeopleTable;
        }

        public static DataTable GetPeopleFromDataBase()
        {

            DataTable dt = new DataTable();
            string query = @"Select * From ListPeopleInSystem()
                                ORDER BY ListPeopleInSystem.FirstName";

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDB_Util.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
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
    }
}
