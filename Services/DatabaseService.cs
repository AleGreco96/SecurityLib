using Microsoft.Data.SqlClient;
using SecurityLib.Models;
using System.Data;

namespace SecurityLib.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString = string.Empty;
        private readonly SqlCommand _sqlCommand = new();
        public bool IsDbOnLine = false;

        public DatabaseService(string connectionString) 
        {
            try
            {
                _connectionString = connectionString;
                using (SqlConnection connection = new(_connectionString))
                {
                    connection.Open();
                    IsDbOnLine = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore di Connessione al DB: {ex.Message}");
            }
        }

        public bool AddCredentialsOnDb(Credential credential)
        {
            bool result = false;

            try
            {
                using (SqlConnection connection = new(_connectionString))
                {
                    connection.Open();

                    _sqlCommand.Parameters.Clear();
                    _sqlCommand.CommandText = "InsertCredentials";
                    _sqlCommand.Connection = connection;
                    _sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    _sqlCommand.Parameters.AddWithValue("@CustomerID", credential.CustomerId);
                    _sqlCommand.Parameters.AddWithValue("@EmailAddress", credential.EmailAddress);
                    _sqlCommand.Parameters.AddWithValue("@PasswordHash", credential.PasswordHash);
                    _sqlCommand.Parameters.AddWithValue("@PasswordSalt", credential.PasswordSalt);
                    _sqlCommand.Parameters.AddWithValue("@Role", credential.Role);
                    _sqlCommand.Parameters.AddWithValue("@ModifiedDate", credential.ModifiedDate);

                    _sqlCommand.ExecuteNonQuery();

                }

                result = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return result;

        }

        public async Task<Credential> ReadCredentialsOnDb(string EmailAddress)
        {
            Credential credential = new();

            try
            {
                using (SqlConnection connection = new(_connectionString))
                {
                    await connection.OpenAsync();

                    _sqlCommand.Parameters.Clear();
                    _sqlCommand.CommandType = CommandType.Text;
                    _sqlCommand.CommandText = "SELECT * FROM Credential WHERE EmailAddress = @EmailAddress";
                    _sqlCommand.Connection = connection;
                    _sqlCommand.Parameters.AddWithValue("@EmailAddress", EmailAddress);

                    SqlDataReader reader = await _sqlCommand.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
                    {
                        credential.CustomerId   = Convert.ToInt16(reader["CustomerId"]);
                        credential.EmailAddress = reader["EmailAddress"].ToString()!;
                        credential.PasswordHash = reader["PasswordHash"].ToString()!;
                        credential.PasswordSalt = reader["PasswordSalt"].ToString()!;
                        credential.Role         = reader["Role"].ToString()!;
                        credential.ModifiedDate = Convert.ToDateTime(reader["ModifiedDate"]);
                    }
                    reader.Close();
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return credential;
        }
    }
}


