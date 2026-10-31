using Dapper;
using Order.Domain.Interfaces.Repositories;
using Order.Domain.Interfaces.Repositories.DataConnector;
using Order.Domain.Models;

namespace Order.Infra.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbConnector _dbConnector;

        public OrderRepository(IDbConnector dbConnector)
        {
            _dbConnector = dbConnector;
        }

        // Column order matters: Dapper splits the row on "Id" into Order, Client and User.
        const string baseSql = @"SELECT o.[Id]
                                       ,o.[CreatedAt]
                                       ,c.[Id]
                                       ,c.[Name]
                                       ,u.[Id]
                                       ,u.[Name]
                                   FROM [dbo].[Order] o
                                   JOIN [dbo].[Client] c ON o.[ClientId] = c.[Id]
                                   JOIN [dbo].[User] u ON o.[UserId] = u.[Id]
                                  WHERE 1 = 1 ";

        public async Task CreateAsync(OrderModel order)
        {
            string sql = @"INSERT INTO [dbo].[Order]
                                  ([Id], [ClientId], [UserId], [CreatedAt])
                           VALUES (@Id, @ClientId, @UserId, @CreatedAt)";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new
            {
                order.Id,
                ClientId = order.Client.Id,
                UserId = order.User.Id,
                order.CreatedAt
            }, _dbConnector.DbTransaction);

            if (order.Items != null)
            {
                foreach (var item in order.Items)
                    await CreateItemAsync(item);
            }
        }

        public async Task CreateItemAsync(OrderItemModel item)
        {
            string sql = @"INSERT INTO [dbo].[OrderItem]
                                  ([Id], [OrderId], [ProductId], [SellValue], [Quantity], [TotalAmount], [CreatedAt])
                           VALUES (@Id, @OrderId, @ProductId, @SellValue, @Quantity, @TotalAmount, @CreatedAt)";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new
            {
                item.Id,
                OrderId = item.Order.Id,
                ProductId = item.Product.Id,
                item.SellValue,
                item.Quantity,
                item.TotalAmount,
                item.CreatedAt
            }, _dbConnector.DbTransaction);
        }

        public async Task DeleteAsync(string orderId)
        {
            // Items reference the order through a foreign key, so they must be removed first.
            await _dbConnector.DbConnection.ExecuteAsync(
                "DELETE FROM [dbo].[OrderItem] WHERE [OrderId] = @Id",
                new { Id = orderId },
                _dbConnector.DbTransaction);

            await _dbConnector.DbConnection.ExecuteAsync(
                "DELETE FROM [dbo].[Order] WHERE [Id] = @Id",
                new { Id = orderId },
                _dbConnector.DbTransaction);
        }

        public async Task DeleteItemAsync(string itemId)
        {
            string sql = "DELETE FROM [dbo].[OrderItem] WHERE [Id] = @Id";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new { Id = itemId }, _dbConnector.DbTransaction);
        }

        public async Task<bool> ExistsByIdAsync(string orderId)
        {
            string sql = "SELECT COUNT(1) FROM [dbo].[Order] WHERE [Id] = @Id";

            var count = await _dbConnector.DbConnection.ExecuteScalarAsync<int>(sql, new { Id = orderId }, _dbConnector.DbTransaction);

            return count > 0;
        }

        public async Task<OrderModel> GetByIdAsync(string orderId)
        {
            string sql = $"{baseSql} AND o.[Id] = @Id";

            var orders = await _dbConnector.DbConnection.QueryAsync<OrderModel, ClientModel, UserModel, OrderModel>(
                sql: sql,
                map: (order, client, user) =>
                {
                    order.Client = client;
                    order.User = user;
                    return order;
                },
                param: new { Id = orderId },
                splitOn: "Id",
                transaction: _dbConnector.DbTransaction);

            return orders.FirstOrDefault();
        }

        public async Task<List<OrderModel>> ListByFilterAsync(string orderId = null, string clientId = null, string userId = null)
        {
            string sql = baseSql;

            if (!string.IsNullOrWhiteSpace(orderId))
                sql += " AND o.[Id] = @OrderId";

            if (!string.IsNullOrWhiteSpace(clientId))
                sql += " AND o.[ClientId] = @ClientId";

            if (!string.IsNullOrWhiteSpace(userId))
                sql += " AND o.[UserId] = @UserId";

            var orders = await _dbConnector.DbConnection.QueryAsync<OrderModel, ClientModel, UserModel, OrderModel>(
                sql: sql,
                map: (order, client, user) =>
                {
                    order.Client = client;
                    order.User = user;
                    return order;
                },
                param: new { OrderId = orderId, ClientId = clientId, UserId = userId },
                splitOn: "Id",
                transaction: _dbConnector.DbTransaction);

            return orders.ToList();
        }

        public async Task<List<OrderItemModel>> ListItemByOrderIdAsync(string orderId)
        {
            // Column order matters: Dapper splits the row on "Id" into OrderItem, Order and Product.
            string sql = @"SELECT oi.[Id]
                                 ,oi.[SellValue]
                                 ,oi.[Quantity]
                                 ,oi.[TotalAmount]
                                 ,oi.[CreatedAt]
                                 ,oi.[OrderId] AS [Id]
                                 ,oi.[ProductId] AS [Id]
                                 ,p.[Description]
                             FROM [dbo].[OrderItem] oi
                             JOIN [dbo].[Product] p ON oi.[ProductId] = p.[Id]
                            WHERE oi.[OrderId] = @OrderId";

            var items = await _dbConnector.DbConnection.QueryAsync<OrderItemModel, OrderModel, ProductModel, OrderItemModel>(
                sql: sql,
                map: (item, order, product) =>
                {
                    item.Order = order;
                    item.Product = product;
                    return item;
                },
                param: new { OrderId = orderId },
                splitOn: "Id",
                transaction: _dbConnector.DbTransaction);

            return items.ToList();
        }

        public async Task UpdateAsync(OrderModel order)
        {
            string sql = @"UPDATE [dbo].[Order]
                              SET [ClientId] = @ClientId
                                 ,[UserId] = @UserId
                            WHERE [Id] = @Id";

            await _dbConnector.DbConnection.ExecuteAsync(sql, new
            {
                order.Id,
                ClientId = order.Client.Id,
                UserId = order.User.Id
            }, _dbConnector.DbTransaction);

            if (order.Items != null && order.Items.Any())
            {
                await _dbConnector.DbConnection.ExecuteAsync(
                    "DELETE FROM [dbo].[OrderItem] WHERE [OrderId] = @OrderId",
                    new { OrderId = order.Id },
                    _dbConnector.DbTransaction);

                foreach (var item in order.Items)
                    await CreateItemAsync(item);
            }
        }
    }
}
