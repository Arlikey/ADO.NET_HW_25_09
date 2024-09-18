using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.NET_HW_25_09
{
    internal class AdditionalTask2
    {
        static string connectionString;
        static void Main(string[] args)
        {
            connectionString = GetConnectionString();
            //GetAvgSalaryEmployeesOlderThan(30);
            //GetCountEmployeesOfEveryPosition();
            //GetMaxSalaryOfEmployeesByAge();
            //DeleteById(2);
            UpdateSalariesById([1, 3], 100_000);
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

        private static void GetAvgSalaryEmployeesOlderThan(int age)
        {
            ExecuteQuery(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT AVG(Salary) FROM Employees WHERE Age > @age
                    """, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@age", age));

                object avgSalary = sqlCommand.ExecuteScalar();
                Console.WriteLine(avgSalary);
            });
        }
        private static void GetCountEmployeesOfEveryPosition()
        {
            ExecuteQuery(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT Position, COUNT(*) AS Count FROM Employees GROUP BY Position
                    """, connection);

                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Position"]}: {reader["Count"]}");
                }
            });
        }

        private static void GetMaxSalaryOfEmployeesByAge()
        {
            ExecuteQuery(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT Age, MAX(Salary) AS MaxSalary FROM Employees GROUP BY Age
                    """, connection);

                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Age"]}: {reader["MaxSalary"]}");
                }
            });
        }

        private static void DeleteById(int id)
        {
            ExecuteQuery(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    DELETE FROM Employees WHERE Id = @id
                    """, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@id", id));
                sqlCommand.ExecuteNonQuery();
            });
        }

        private static void UpdateSalariesById(int[] ids, decimal salary)
        {
            ExecuteQuery(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand($"""
                    UPDATE Employees
                    SET Salary = @salary
                    WHERE Id IN ({string.Join(',', ids)})
                    """, connection);
                sqlCommand.Parameters.Add(new SqlParameter("@salary", salary));
                sqlCommand.ExecuteNonQuery();
            });
        }
    }
}