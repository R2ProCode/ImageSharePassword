using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSharePassword.Data
{
    public class ImageSharePasswordRespository
    {
        private readonly string _connectionString;

        public ImageSharePasswordRespository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Image GetImageById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Images WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();

            using var reader = command.ExecuteReader();
            if(reader.Read())
            {
                return new Image
                {
                    Id = (int)reader["Id"],
                    Password = (string)reader["Password"],
                    ImagePath = (string)reader["ImagePath"],
                    Views = (int)reader["Views"]
                };
            }
            return null;
        }

        public void AddImage(Image image)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Images (Password, ImagePath, Views) VALUES (@Password, @ImagePath, 0); SELECT SCOPE_IDENTITY();";
            command.Parameters.AddWithValue("@Password", image.Password);
            command.Parameters.AddWithValue("@ImagePath", image.ImagePath);
            connection.Open();
            image.Id = (int)(decimal)command.ExecuteScalar();
        }

        public string GetPasswordForImage(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT Password FROM Images WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return (string)command.ExecuteScalar();
        }

        public void IncrementViews(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = @"UPDATE Images SET Views = Views + 1 WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
