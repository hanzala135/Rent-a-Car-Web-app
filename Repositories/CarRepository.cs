using SmartCarRentACar.Models;
using Microsoft.Data.SqlClient;

namespace SmartCarRentACar.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly string _connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;";

        public CarRepository()
        {
            EnsureTableExists().Wait(); // constructor can't be async
        }

        private async Task EnsureTableExists()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string sql = @"
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cars' AND xtype='U')
CREATE TABLE Cars (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CarName NVARCHAR(100) NOT NULL,
    Model NVARCHAR(50) NOT NULL,
    PricePerDay INT NOT NULL,
    IsAvailable BIT NOT NULL,
    ImageUrl NVARCHAR(255) NULL
)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<Car>> GetAll()
        {
            var cars = new List<Car>();

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string sql = "SELECT * FROM Cars";

                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        cars.Add(new Car
                        {
                            Id = (int)reader["Id"],
                            CarName = reader["CarName"].ToString(),
                            Model = reader["Model"].ToString(),
                            PricePerDay = (int)reader["PricePerDay"],
                            IsAvailable = (bool)reader["IsAvailable"],
                            ImageUrl = reader["ImageUrl"].ToString()
                        });
                    }
                }
            }
            return cars;
        }

        public async Task<Car?> GetById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string sql = "SELECT * FROM Cars WHERE Id=@Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Car
                            {
                                Id = (int)reader["Id"],
                                CarName = reader["CarName"].ToString(),
                                Model = reader["Model"].ToString(),
                                PricePerDay = (int)reader["PricePerDay"],
                                IsAvailable = (bool)reader["IsAvailable"],
                                ImageUrl = reader["ImageUrl"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        public async Task Add(Car car)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string sql = @"INSERT INTO Cars 
(CarName, Model, PricePerDay, IsAvailable, ImageUrl)
VALUES (@CarName, @Model, @PricePerDay, @IsAvailable, @ImageUrl)";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CarName", car.CarName);
                    cmd.Parameters.AddWithValue("@Model", car.Model);
                    cmd.Parameters.AddWithValue("@PricePerDay", car.PricePerDay);
                    cmd.Parameters.AddWithValue("@IsAvailable", car.IsAvailable);
                    cmd.Parameters.AddWithValue("@ImageUrl", car.ImageUrl ?? "");

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task Update(Car car)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string sql = @"UPDATE Cars SET 
CarName=@CarName,
Model=@Model,
PricePerDay=@PricePerDay,
IsAvailable=@IsAvailable,
ImageUrl=@ImageUrl
WHERE Id=@Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", car.Id);
                    cmd.Parameters.AddWithValue("@CarName", car.CarName);
                    cmd.Parameters.AddWithValue("@Model", car.Model);
                    cmd.Parameters.AddWithValue("@PricePerDay", car.PricePerDay);
                    cmd.Parameters.AddWithValue("@IsAvailable", car.IsAvailable);
                    cmd.Parameters.AddWithValue("@ImageUrl", car.ImageUrl ?? "");

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task Delete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string sql = "DELETE FROM Cars WHERE Id=@Id";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
