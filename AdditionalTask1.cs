using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET_HW_25_09
{
    internal class AdditionalTask1
    {
        static string connectionString;
        static void Main(string[] args)
        {
            connectionString = GetConnectionString();
            //GetInfoHighestBridge();
            //GetCountOfBridgesLessThan(2700);
            //InsertBridge(new Bridge { Name = "Sydney Harbour Bridge", Height = 134, Width = 49, CountOfPiers = 4, Length = 1149});
        }

        private static string GetConnectionString()
        {
            var configurationBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = configurationBuilder.Build();

            return configuration.GetConnectionString("DefaultConnection");
        }

        private static void ExecuteQuery(Action<SqlConnection> action)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlConnection.Open();
                    action(sqlConnection);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void GetInfoHighestBridge()
        {
            ExecuteQuery(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT [Name], [Height], [Width], [CountOfPiers], [Length] FROM Bridges
                    WHERE Height = (SELECT MAX(Height) FROM Bridges)
                    """, connection);

                SqlDataReader reader = sqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine($"Name: {reader["Name"]}, Height: {reader["Height"]}, " +
                        $"Width: {reader["Width"]}, Count of Piers: {reader["CountOfPiers"]}, Length: {reader["Length"]}");
                }
            });
        }

        private static void GetCountOfBridgesLessThan(int length)
        {
            ExecuteQuery(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT COUNT(*) FROM Bridges
                    WHERE Length <= @length
                    """, connection);

                sqlCommand.Parameters.Add(new SqlParameter("@length", length));
                object count = sqlCommand.ExecuteScalar();
                Console.WriteLine(count);
            });
        }

        private static void InsertBridge(Bridge bridge)
        {
            ExecuteQuery(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    INSERT INTO [Bridges] ([Name], [Height], [Width], [CountOfPiers], [Length])
                    VALUES (@name, @height, @width, @countOfPiers, @length)
                    SELECT @id = SCOPE_IDENTITY()
                    """, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@name", bridge.Name));
                sqlCommand.Parameters.Add(new SqlParameter("@height", bridge.Height));
                sqlCommand.Parameters.Add(new SqlParameter("@width", bridge.Width));
                sqlCommand.Parameters.Add(new SqlParameter("@countOfPiers", bridge.CountOfPiers));
                sqlCommand.Parameters.Add(new SqlParameter("@length", bridge.Length));
                SqlParameter id = new SqlParameter
                {
                    ParameterName = "@id",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };
                sqlCommand.Parameters.Add(id);

                sqlCommand.ExecuteNonQuery();
                Console.WriteLine(id.Value);
            });
        }
    }
}

class Bridge
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public int CountOfPiers { get; set; }
    public int Length { get; set; }
}
