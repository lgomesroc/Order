using System.Data.SqlClient;
using Order.Domain.Common;
using Order.Domain.Models;
using Order.Domain.Services;
using Order.Infra.DataConnector;
using Order.Infra.Repositories;

namespace Order.IntegrationTests;

[Trait("Category", "Integration")]
public class SqlServerRepositoryTests
{
    [Fact]
    public async Task OrderService_PersistsOrderAndRestoresStockOnDelete()
    {
        var connectionString = Environment.GetEnvironmentVariable("ORDER_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connectionString), "ORDER_TEST_CONNECTION must point to the SQL Server test database.");

        await InitializeSchema(connectionString!);
        var databaseConnection = new SqlConnectionStringBuilder(connectionString!) { InitialCatalog = "OrderDb" }.ConnectionString;

        using var connector = new SqlConnector(databaseConnection);
        var clients = new ClientRepository(connector);
        var products = new ProductRepository(connector);
        var users = new UserRepository(connector);
        var orders = new OrderRepository(connector);
        var unitOfWork = new UnitOfWork(connector);
        var orderService = new OrderService(new Order.Domain.Common.TimeProvider(), new Generators(), unitOfWork);

        var suffix = Guid.NewGuid().ToString("N");
        var clientId = suffix;
        var productId = suffix;
        var secondProductId = Guid.NewGuid().ToString("N");
        var userId = suffix;
        string? createdOrderId = null;

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

            var user = new UserModel
            {
                Id = userId,
                Name = "Integration User",
                Login = $"it-{suffix}",
                PasswordHash = "test-password-hash",
                CreatedAt = DateTime.UtcNow
            };
            await users.CreateAsync(user);

            var product = new ProductModel
            {
                Id = productId,
                Description = "Integration Product",
                SellValue = 25.50m,
                Stock = 5,
                CreatedAt = DateTime.UtcNow
            };
            await products.CreateAsync(product);
            await products.CreateAsync(new ProductModel
            {
                Id = secondProductId,
                Description = "Limited Product",
                SellValue = 10m,
                Stock = 1,
                CreatedAt = DateTime.UtcNow
            });

            var createResult = await orderService.CreateAsync(new OrderModel
            {
                Client = new ClientModel { Id = clientId },
                User = new UserModel { Id = userId },
                Items = new List<OrderItemModel>
                {
                    new() { Product = new ProductModel { Id = productId }, Quantity = 2 }
                }
            });

            Assert.Empty(createResult.Report);

            var createdOrders = await orderService.ListByFiltersAsync(null, clientId, userId);
            Assert.Empty(createdOrders.Report);
            var createdOrder = Assert.Single(createdOrders.Data);
            createdOrderId = createdOrder.Id;

            var details = await orderService.GetByIdAsync(createdOrderId);
            Assert.Empty(details.Report);
            var item = Assert.Single(details.Data!.Items);
            Assert.Equal(productId, item.Product.Id);
            Assert.Equal(2, item.Quantity);
            Assert.Equal(51.00m, item.TotalAmount);

            Assert.Equal(3, (await products.GetByIdAsync(productId))!.Stock);

            var deleteResult = await orderService.DeleteAsync(createdOrderId);
            Assert.Empty(deleteResult.Report);
            createdOrderId = null;

            Assert.Equal(5, (await products.GetByIdAsync(productId))!.Stock);

            var rejectedOrder = await orderService.CreateAsync(new OrderModel
            {
                Client = new ClientModel { Id = clientId },
                User = new UserModel { Id = userId },
                Items = new List<OrderItemModel>
                {
                    new() { Product = new ProductModel { Id = productId }, Quantity = 2 },
                    new() { Product = new ProductModel { Id = secondProductId }, Quantity = 2 }
                }
            });

            Assert.NotEmpty(rejectedOrder.Report);
            Assert.Equal(5, (await products.GetByIdAsync(productId))!.Stock);
            Assert.Equal(1, (await products.GetByIdAsync(secondProductId))!.Stock);
            var ordersAfterRejection = await orderService.ListByFiltersAsync(null, clientId, userId);
            Assert.Empty(ordersAfterRejection.Data);
        }
        finally
        {
            if (!string.IsNullOrWhiteSpace(createdOrderId))
                await orders.DeleteAsync(createdOrderId);
            await products.DeleteAsync(productId);
            await products.DeleteAsync(secondProductId);
            await users.DeleteAsync(userId);
            await clients.DeleteAsync(clientId);
        }
    }


    [Fact]
    public async Task ProductRepository_ConcurrentReservations_DoNotOversellStock()
    {
        var connectionString = Environment.GetEnvironmentVariable("ORDER_TEST_CONNECTION");
        Assert.False(string.IsNullOrWhiteSpace(connectionString), "ORDER_TEST_CONNECTION must point to the SQL Server test database.");

        await InitializeSchema(connectionString!);
        var databaseConnection = new SqlConnectionStringBuilder(connectionString!) { InitialCatalog = "OrderDb" }.ConnectionString;
        var productId = Guid.NewGuid().ToString("N");

        try
        {
            using (var setupConnector = new SqlConnector(databaseConnection))
            {
                await new ProductRepository(setupConnector).CreateAsync(new ProductModel
                {
                    Id = productId,
                    Description = "Concurrent reservation test",
                    SellValue = 10m,
                    Stock = 5,
                    CreatedAt = DateTime.UtcNow
                });
            }

            using var firstConnector = new SqlConnector(databaseConnection);
            using var secondConnector = new SqlConnector(databaseConnection);
            var firstRepository = new ProductRepository(firstConnector);
            var secondRepository = new ProductRepository(secondConnector);

            var reservations = await Task.WhenAll(
                firstRepository.TryDecreaseStockAsync(productId, 4),
                secondRepository.TryDecreaseStockAsync(productId, 4));

            Assert.Single(reservations, reserved => reserved);
            Assert.Single(reservations, reserved => !reserved);

            using var verificationConnector = new SqlConnector(databaseConnection);
            var finalProduct = await new ProductRepository(verificationConnector).GetByIdAsync(productId);
            Assert.NotNull(finalProduct);
            Assert.Equal(1, finalProduct.Stock);
        }
        finally
        {
            using var cleanupConnector = new SqlConnector(databaseConnection);
            await new ProductRepository(cleanupConnector).DeleteAsync(productId);
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
