using System.Data.SqlClient;

namespace SmartCarRentACar.Database
{
    public static class DatabaseHelper
    {
        private static readonly string masterConnection =
            "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;";

        public static readonly string dbName = "SmartCarDB";

        public static readonly string connectionString =
            $"Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Database={dbName};";

    
        public static void Initialize()
        {
            CreateDatabaseIfNotExists();
            CreateTablesIfNotExists();
        }

        private static void CreateDatabaseIfNotExists()
        {
            using (SqlConnection conn = new SqlConnection(masterConnection))
            {
                conn.Open();

                string checkDb = $@"
                    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{dbName}')
                    BEGIN
                        CREATE DATABASE {dbName}
                    END";

                SqlCommand cmd = new SqlCommand(checkDb, conn);
                cmd.ExecuteNonQuery();
            }
        }

        private static void CreateTablesIfNotExists()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string createCarsTable = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cars' AND xtype='U')
                    CREATE TABLE Cars (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Name NVARCHAR(100),
                        Model NVARCHAR(100),
                        RentPerDay DECIMAL(10,2)
                    )";

                string createBookingsTable = @"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Bookings' AND xtype='U')
                    CREATE TABLE Bookings (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        CustomerName NVARCHAR(100),
                        Contact NVARCHAR(50),
                        CarId INT,
                        StartDate DATE,
                        EndDate DATE,
                        FOREIGN KEY (CarId) REFERENCES Cars(Id)
                    )";

                SqlCommand cmd1 = new SqlCommand(createCarsTable, conn);
                SqlCommand cmd2 = new SqlCommand(createBookingsTable, conn);

                cmd1.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();
            }
        }
    }
}
