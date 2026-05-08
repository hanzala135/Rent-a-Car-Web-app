using SmartCarRentACar.Models;
using Microsoft.Data.SqlClient;

namespace SmartCarRentACar.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly string _connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;";

        public BookingRepository()
        {
            EnsureTableExists().Wait(); 
        }


        private async Task EnsureTableExists()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string sql = @"
IF OBJECT_ID('dbo.Bookings', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Bookings
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        CarName NVARCHAR(100) NOT NULL,
        CustomerName NVARCHAR(100) NOT NULL,
        Phone NVARCHAR(50) NOT NULL,
        FromDate DATETIME NOT NULL,
        ToDate DATETIME NOT NULL,
        Status NVARCHAR(50) NOT NULL
    )
END";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

 
        public async Task<IEnumerable<Booking>> GetAll()
        {
            var bookings = new List<Booking>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var sql = "SELECT * FROM Bookings";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        bookings.Add(new Booking
                        {
                            Id = (int)reader["Id"],
                            CarName = reader["CarName"].ToString(),
                            CustomerName = reader["CustomerName"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            FromDate = (DateTime)reader["FromDate"],
                            ToDate = (DateTime)reader["ToDate"],
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }
            return bookings;
        }

        public async Task<Booking?> GetById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var sql = "SELECT * FROM Bookings WHERE Id=@Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Booking
                            {
                                Id = (int)reader["Id"],
                                CarName = reader["CarName"].ToString(),
                                CustomerName = reader["CustomerName"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                FromDate = (DateTime)reader["FromDate"],
                                ToDate = (DateTime)reader["ToDate"],
                                Status = reader["Status"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task Add(Booking booking)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var sql = @"INSERT INTO Bookings 
(CarName, CustomerName, Phone, FromDate, ToDate, Status)
VALUES (@CarName, @CustomerName, @Phone, @FromDate, @ToDate, @Status)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CarName", booking.CarName);
                    cmd.Parameters.AddWithValue("@CustomerName", booking.CustomerName);
                    cmd.Parameters.AddWithValue("@Phone", booking.Phone);
                    cmd.Parameters.AddWithValue("@FromDate", booking.FromDate);
                    cmd.Parameters.AddWithValue("@ToDate", booking.ToDate);
                    cmd.Parameters.AddWithValue("@Status", booking.Status ?? "Pending");

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task Update(Booking booking)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var sql = @"UPDATE Bookings SET Status=@Status WHERE Id=@Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", booking.Id);
                    cmd.Parameters.AddWithValue("@Status", booking.Status);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
