using System.Data.SqlClient;
using Order.Domain.Models;
using Order.Infra.DataConnector;
using Order.Infra.Repositories;

namespace Order.IntegrationTests;

[Trait("Category", "Integration")]
public class SqlServerRepositoryTests
{
    [Fact]
    public async Task ClientAndProductRepositories_PersistChangesInSqlServer()
    {
        var connectionString = Environment.GetEnvironmentVariable("ORDER_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connectionString), "ORDER_TEST_CONNECTION must point to the SQL Server test database.");

        await InitializeSchema(connectionString!);
        var databaseConnection = new SqlConnectionStringBuilder(connectionString!) { InitialCatalog = "OrderDb" }.ConnectionString;

        using var connector = new SqlConnector(databaseConnection);
        var clients = new ClientRepository(connector);
        var products = new ProductRepository(connector);
        var suffix = Guid.NewGuid().ToString("N");
        var clientId = $"it-client-{suffix}";
        var productId = $"it-product-{suffix}";

        try
        {
            var client = new ClientModel
            {
                Id = clientId,
                Name = "Integration Client",
                Email = $"{suffix}@example.test",
                PhoneNumber = "21999990000",
                Address = "Test address",
                CreatedAt = DateTime.UtcNow
            };
            await clients.CreateAsync(client);

            var loadedClient = await clients.GetByIdAsync(clientId);
            Assert.NotNull(loadedClient);
            Assert.Equal(client.Name, loadedClient!.Name);

            client.Name = "Updated Integration Client";
            await clients.UpdateAsync(client);
            Assert.Equal(client.Name, (await clients.GetByIdAsync(clientId))!.Name);

            var product = new ProductModel
            {
                Id = productId,
                Description = "Integration Product",
                SellValue = 25.50m,
                Stock = 8,
                CreatedAt = DateTime.UtcNow
            };
            await products.CreateAsync(product);

            var loadedProduct = await products.GetByIdAsync(productId);
            Assert.NotNull(loadedProduct);
            Assert.Equal(25.50m, loadedProduct!.SellValue);
            Assert.Equal(8, loadedProduct.Stock);

            product.Stock = 5;
            await products.UpdateAsync(product);
            Assert.Equal(5, (await products.GetByIdAsync(productId))!.Stock);
        }
        finally
        {
            await products.DeleteAsync(productId);
            await clients.DeleteAsync(clientId);
        }
    }

    private static async Task InitializeSchema(string connectionString)
    {
        var scriptPath = Path.Combine(AppContext.BaseDirectory, "schema.sql");
        var script = await File.ReadAllTextAsync(scriptPath);
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        foreach (var batch in System.Text.RegularExpressions.Regex.Split(script, @"(?im)^\s*GO\s*$"))
        {
            if (string.IsNullOrWhiteSpace(batch)) continue;
            await using var command = new SqlCommand(batch, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}
