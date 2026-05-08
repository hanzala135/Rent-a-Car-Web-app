using SmartCarRentACar.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SmartCarRentACar.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly string _connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;";

        public MessageRepository()
        {
            EnsureTableExistsAsync().Wait(); // constructors can't be async
        }

        private async Task EnsureTableExistsAsync()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string sql = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Messages' AND xtype='U')
                CREATE TABLE Messages (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    SenderName NVARCHAR(100) NOT NULL,
                    Email NVARCHAR(100) NOT NULL,
                    Content NVARCHAR(MAX) NULL
                )";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<Message>> GetAll()
        {
            var messages = new List<Message>();
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string sql = "SELECT * FROM Messages";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        messages.Add(new Message
                        {
                            Id = (int)reader["Id"],
                            SenderName = reader["SenderName"].ToString(),
                            Email = reader["Email"].ToString(),
                            Content = reader["Content"].ToString()
                        });
                    }
                }
            }
            return messages;
        }

        public async Task Add(Message message)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string sql = @"INSERT INTO Messages (SenderName, Email, Content)
                               VALUES (@SenderName, @Email, @Content)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SenderName", message.SenderName);
                    cmd.Parameters.AddWithValue("@Email", message.Email);
                    cmd.Parameters.AddWithValue("@Content", message.Content ?? "");
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
