using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

class Program
{
    static string connectionString;
    static void Main(string[] args)
    {
        connectionString = GetConnectionString();

        InsertInventory(new Inventory { Name = "Sofa", Quantity = 2, Status = "In stock"});
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

    private static void InsertInventory(Inventory inventory)
    {
        ExecuteQuery(connection =>
        {
            SqlCommand cmd = new SqlCommand("[dbo].[sp_AddInventory]", connection) 
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new SqlParameter("@ObjectName", inventory.Name));
            cmd.Parameters.Add(new SqlParameter("@ObjectQuantity", inventory.Quantity));
            cmd.Parameters.Add(new SqlParameter("@ObjectStatus", inventory.Status));
            SqlParameter id = new SqlParameter()
            {
                ParameterName = "@ObjectID",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };
            cmd.Parameters.Add(id);

            cmd.ExecuteNonQuery();

            Console.WriteLine(id.Value);
        });
    }
}

class Inventory
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; }
}