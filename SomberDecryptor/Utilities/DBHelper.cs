using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Npgsql;

namespace SomberDecryptor.Utilities
{
    public class DBHelper
    {
        private readonly string _connectionString;

        public DBHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool ValidateDecryptionKey(string username, string hexKey)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();
                    
                    string keyQuery = @"
                        SELECT decryption_key 
                        FROM users 
                        WHERE username = @username";

                    var user = connection.QueryFirstOrDefault<dynamic>(keyQuery, new { username });
                    
                    if (user == null)
                    {
                        Console.WriteLine($"Kullanıcı {username} için kayıt bulunamadı");
                        return false;
                    }
                    
                    byte[] storedKey = user.decryption_key;
                    string storedHexKey = BitConverter.ToString(storedKey).Replace("-", "");
                    
                    return storedHexKey.Equals(hexKey, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Doğrulama hatası: {ex.Message}");
                return false;
            }
        }

        public int Execute(string query, object parameters = null)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();
                return connection.Execute(query, parameters);
            }
        }
    }
} 